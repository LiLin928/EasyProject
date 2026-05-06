using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Helper;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Entitys;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 数据导入导出服务实现类
/// </summary>
/// <remarks>
/// 实现用户和角色的数据导入导出功能。
/// 继承自 BaseService，使用 SqlSugar 进行数据库操作。
/// 使用 ExcelHelper 进行 Excel 文件的读取和生成。
///
/// 主要功能：
/// - 用户数据的导出和导入
/// - 角色数据的导出和导入
/// - 导入模板的生成
///
/// 导入验证规则：
/// - 必填字段验证
/// - 格式验证（手机号、邮箱）
/// - 唯一性验证（用户名、角色编码）
/// - 数据长度验证
/// </remarks>
/// <example>
/// <code>
/// // 服务注册（Autofac 自动扫描注册）
/// // 命名规则：以 Service 结尾的类自动注册为接口实现
///
/// // 使用示例
/// var service = new ImportExportService(logger);
/// var userBytes = await service.ExportUsersAsync(new List&lt;int&gt; { 1, 2, 3 });
/// var importResult = await service.ImportUsersAsync(fileBytes, operatorId);
/// </code>
/// </example>
public class ImportExportService : BaseService<User>, IImportExportService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<ImportExportService> _logger { get; set; } = null!;

    /// <summary>
    /// 导出用户数据
    /// </summary>
    /// <param name="userIds">要导出的用户ID列表，为空时导出全部用户</param>
    /// <returns>Excel 文件的字节数组</returns>
    /// <remarks>
    /// 导出流程：
    /// 1. 根据 userIds 查询用户数据
    /// 2. 查询每个用户的角色信息
    /// 3. 转换为导出 DTO 格式
    /// 4. 生成 Excel 文件
    ///
    /// 不导出敏感字段如密码。
    /// </remarks>
    public async Task<byte[]> ExportUsersAsync(List<Guid>? userIds = null)
    {
        _logger.LogInformation("开始导出用户数据，用户ID列表: {UserIds}",
            userIds == null || userIds.Count == 0 ? "全部" : string.Join(",", userIds));

        // 查询用户数据
        var query = _db.Queryable<User>()
            .WhereIF(userIds != null && userIds.Count > 0, u => userIds!.Contains(u.Id))
            .OrderByDescending(u => u.CreateTime);

        var users = await query.ToListAsync();

        // 转换为导出 DTO
        var exportData = new List<UserExportDto>();

        foreach (var user in users)
        {
            // 查询用户角色
            var roles = await _db.Queryable<UserRole, Role>((ur, r) => new JoinQueryInfos(JoinType.Left, ur.RoleId == r.Id))
                .Where((ur, r) => ur.UserId == user.Id)
                .Select((ur, r) => r.RoleName)
                .ToListAsync();

            var dto = new UserExportDto
            {
                UserName = user.UserName,
                RealName = user.RealName,
                Phone = user.Phone,
                Email = user.Email,
                Status = user.Status == 1 ? "正常" : "禁用",
                CreateTime = user.CreateTime,
                Roles = roles.Count > 0 ? string.Join(",", roles) : ""
            };

            exportData.Add(dto);
        }

        // 生成 Excel 文件
        var bytes = ExcelHelper.SaveToExcelBytes(exportData);

        _logger.LogInformation("用户数据导出完成，共导出 {Count} 条记录", exportData.Count);

        return bytes;
    }

    /// <summary>
    /// 导入用户数据
    /// </summary>
    /// <param name="fileData">Excel 文件的字节数组</param>
    /// <param name="operatorId">操作用户ID</param>
    /// <returns>导入结果 DTO</returns>
    /// <remarks>
    /// 导入流程：
    /// 1. 读取 Excel 数据
    /// 2. 逐行验证数据
    /// 3. 插入验证通过的数据
    /// 4. 返回导入结果
    ///
    /// 验证规则：
    /// - 用户名必填、唯一
    /// - 密码必填
    /// - 手机号格式验证
    /// - 邵箱格式验证
    /// </remarks>
    public async Task<ImportResultDto> ImportUsersAsync(byte[] fileData, Guid operatorId)
    {
        _logger.LogInformation("开始导入用户数据，操作用户ID: {OperatorId}", operatorId);

        var result = new ImportResultDto
        {
            ImportType = "User",
            ImportTime = DateTime.Now,
            OperatorId = operatorId
        };

        try
        {
            // 读取 Excel 数据
            var importData = ExcelHelper.ReadFromExcelBytes<UserImportDto>(fileData).ToList();
            result.TotalCount = importData.Count;

            _logger.LogInformation("读取到 {Count} 条用户数据", importData.Count);

            // 逐行验证和导入
            for (var i = 0; i < importData.Count; i++)
            {
                var row = importData[i];
                var rowNumber = i + 2; // Excel 行号（从第2行开始，第1行是标题）

                // 验证数据
                var errors = ValidateUserImportData(row, rowNumber);

                if (errors.Count > 0)
                {
                    result.Errors.AddRange(errors);
                    result.FailCount++;
                    continue;
                }

                // 检查用户名是否已存在
                var exists = await _db.Queryable<User>()
                    .Where(u => u.UserName == row.UserName)
                    .FirstAsync();

                if (exists != null)
                {
                    result.Errors.Add(new ImportErrorDto
                    {
                        RowNumber = rowNumber,
                        FieldName = "用户名",
                        FieldValue = row.UserName,
                        ErrorMessage = $"用户名 '{row.UserName}' 已存在",
                        ErrorType = ImportErrorType.Duplicate
                    });
                    result.FailCount++;
                    continue;
                }

                // 插入用户数据
                try
                {
                    var user = new User
                    {
                        UserName = row.UserName,
                        Password = SecurityHelper.Md5(row.Password),
                        RealName = row.RealName,
                        Phone = row.Phone,
                        Email = row.Email,
                        Status = row.Status,
                        CreateTime = DateTime.Now
                    };

                    var userId = user.Id;
                    await _db.Insertable(user).ExecuteCommandAsync();

                    // 处理角色分配
                    if (!string.IsNullOrEmpty(row.Roles))
                    {
                        var roleNames = row.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        var roleIds = await _db.Queryable<Role>()
                            .Where(r => roleNames.Contains(r.RoleName))
                            .Select(r => r.Id)
                            .ToListAsync();

                        if (roleIds.Count > 0)
                        {
                            var userRoles = roleIds.Select(roleId => new UserRole
                            {
                                UserId = userId,
                                RoleId = roleId,
                                CreateTime = DateTime.Now
                            }).ToList();

                            await _db.Insertable(userRoles).ExecuteCommandAsync();
                        }
                    }

                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "插入用户数据失败，行号: {RowNumber}", rowNumber);
                    result.Errors.Add(new ImportErrorDto
                    {
                        RowNumber = rowNumber,
                        ErrorMessage = $"数据插入失败: {ex.Message}",
                        ErrorType = ImportErrorType.Database
                    });
                    result.FailCount++;
                }
            }

            _logger.LogInformation("用户数据导入完成，成功: {Success}，失败: {Fail}",
                result.SuccessCount, result.FailCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "读取 Excel 文件失败");
            result.Errors.Add(new ImportErrorDto
            {
                RowNumber = 0,
                ErrorMessage = $"读取 Excel 文件失败: {ex.Message}",
                ErrorType = ImportErrorType.Format
            });
        }

        return result;
    }

    /// <summary>
    /// 导出角色数据
    /// </summary>
    /// <param name="roleIds">要导出的角色ID列表，为空时导出全部角色</param>
    /// <returns>Excel 文件的字节数组</returns>
    /// <remarks>
    /// 导出流程：
    /// 1. 根据 roleIds 查询角色数据
    /// 2. 转换为导出 DTO 格式
    /// 3. 生成 Excel 文件
    /// </remarks>
    public async Task<byte[]> ExportRolesAsync(List<Guid>? roleIds = null)
    {
        _logger.LogInformation("开始导出角色数据，角色ID列表: {RoleIds}",
            roleIds == null || roleIds.Count == 0 ? "全部" : string.Join(",", roleIds));

        // 查询角色数据
        var query = _db.Queryable<Role>()
            .WhereIF(roleIds != null && roleIds.Count > 0, r => roleIds!.Contains(r.Id))
            .OrderByDescending(r => r.CreateTime);

        var roles = await query.ToListAsync();

        // 转换为导出 DTO
        var exportData = roles.Select(r => new RoleExportDto
        {
            RoleName = r.RoleName,
            RoleCode = r.RoleCode,
            Description = r.Description,
            Status = r.Status == 1 ? "正常" : "禁用",
            CreateTime = r.CreateTime
        }).ToList();

        // 生成 Excel 文件
        var bytes = ExcelHelper.SaveToExcelBytes(exportData);

        _logger.LogInformation("角色数据导出完成，共导出 {Count} 条记录", exportData.Count);

        return bytes;
    }

    /// <summary>
    /// 导入角色数据
    /// </summary>
    /// <param name="fileData">Excel 文件的字节数组</param>
    /// <param name="operatorId">操作用户ID</param>
    /// <returns>导入结果 DTO</returns>
    /// <remarks>
    /// 导入流程：
    /// 1. 读取 Excel 数据
    /// 2. 逐行验证数据
    /// 3. 插入验证通过的数据
    /// 4. 返回导入结果
    ///
    /// 验证规则：
    /// - 角色名称必填、唯一
    /// - 角色编码必填、唯一
    /// </remarks>
    public async Task<ImportResultDto> ImportRolesAsync(byte[] fileData, Guid operatorId)
    {
        _logger.LogInformation("开始导入角色数据，操作用户ID: {OperatorId}", operatorId);

        var result = new ImportResultDto
        {
            ImportType = "Role",
            ImportTime = DateTime.Now,
            OperatorId = operatorId
        };

        try
        {
            // 读取 Excel 数据
            var importData = ExcelHelper.ReadFromExcelBytes<RoleImportDto>(fileData).ToList();
            result.TotalCount = importData.Count;

            _logger.LogInformation("读取到 {Count} 条角色数据", importData.Count);

            // 逐行验证和导入
            for (var i = 0; i < importData.Count; i++)
            {
                var row = importData[i];
                var rowNumber = i + 2; // Excel 行号（从第2行开始，第1行是标题）

                // 验证数据
                var errors = ValidateRoleImportData(row, rowNumber);

                if (errors.Count > 0)
                {
                    result.Errors.AddRange(errors);
                    result.FailCount++;
                    continue;
                }

                // 检查角色名称是否已存在
                var existsByName = await _db.Queryable<Role>()
                    .Where(r => r.RoleName == row.RoleName)
                    .FirstAsync();

                if (existsByName != null)
                {
                    result.Errors.Add(new ImportErrorDto
                    {
                        RowNumber = rowNumber,
                        FieldName = "角色名称",
                        FieldValue = row.RoleName,
                        ErrorMessage = $"角色名称 '{row.RoleName}' 已存在",
                        ErrorType = ImportErrorType.Duplicate
                    });
                    result.FailCount++;
                    continue;
                }

                // 检查角色编码是否已存在
                var existsByCode = await _db.Queryable<Role>()
                    .Where(r => r.RoleCode == row.RoleCode)
                    .FirstAsync();

                if (existsByCode != null)
                {
                    result.Errors.Add(new ImportErrorDto
                    {
                        RowNumber = rowNumber,
                        FieldName = "角色编码",
                        FieldValue = row.RoleCode,
                        ErrorMessage = $"角色编码 '{row.RoleCode}' 已存在",
                        ErrorType = ImportErrorType.Duplicate
                    });
                    result.FailCount++;
                    continue;
                }

                // 插入角色数据
                try
                {
                    var role = new Role
                    {
                        RoleName = row.RoleName,
                        RoleCode = row.RoleCode,
                        Description = row.Description,
                        Status = row.Status,
                        CreateTime = DateTime.Now
                    };

                    await _db.Insertable(role).ExecuteCommandAsync();
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "插入角色数据失败，行号: {RowNumber}", rowNumber);
                    result.Errors.Add(new ImportErrorDto
                    {
                        RowNumber = rowNumber,
                        ErrorMessage = $"数据插入失败: {ex.Message}",
                        ErrorType = ImportErrorType.Database
                    });
                    result.FailCount++;
                }
            }

            _logger.LogInformation("角色数据导入完成，成功: {Success}，失败: {Fail}",
                result.SuccessCount, result.FailCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "读取 Excel 文件失败");
            result.Errors.Add(new ImportErrorDto
            {
                RowNumber = 0,
                ErrorMessage = $"读取 Excel 文件失败: {ex.Message}",
                ErrorType = ImportErrorType.Format
            });
        }

        return result;
    }

    /// <summary>
    /// 获取导入模板
    /// </summary>
    /// <param name="type">模板类型：User 或 Role</param>
    /// <returns>Excel 模板文件的字节数组</returns>
    /// <remarks>
    /// 生成包含列标题和示例数据的空白模板。
    /// 用户可下载模板填写数据后上传导入。
    /// </remarks>
    public Task<byte[]> GetImportTemplateAsync(string type)
    {
        _logger.LogInformation("获取导入模板，类型: {Type}", type);

        byte[] bytes;

        switch (type.ToLower())
        {
            case "user":
                // 用户导入模板（包含示例数据）
                var userTemplate = new List<UserImportDto>
                {
                    new UserImportDto
                    {
                        UserName = "示例用户",
                        RealName = "真实姓名",
                        Phone = "13800138000",
                        Email = "example@example.com",
                        Password = "123456",
                        Status = 1,
                        Roles = "角色1,角色2"
                    }
                };
                bytes = ExcelHelper.SaveToExcelBytes(userTemplate);
                break;

            case "role":
                // 角色导入模板（包含示例数据）
                var roleTemplate = new List<RoleImportDto>
                {
                    new RoleImportDto
                    {
                        RoleName = "示例角色",
                        RoleCode = "example_role",
                        Description = "角色描述",
                        Status = 1
                    }
                };
                bytes = ExcelHelper.SaveToExcelBytes(roleTemplate);
                break;

            default:
                throw new ArgumentException($"不支持的模板类型: {type}", nameof(type));
        }

        _logger.LogInformation("导入模板生成完成，类型: {Type}", type);

        return Task.FromResult(bytes);
    }

    /// <summary>
    /// 验证用户导入数据
    /// </summary>
    /// <param name="row">用户导入数据行</param>
    /// <param name="rowNumber">Excel 行号</param>
    /// <returns>验证错误列表，验证通过时为空列表</returns>
    /// <remarks>
    /// 验证规则：
    /// - 用户名必填，长度不超过50字符
    /// - 密码必填，长度不超过50字符
    /// - 手机号格式验证（可选，11位数字）
    /// - 邵箱格式验证（可选）
    /// </remarks>
    private List<ImportErrorDto> ValidateUserImportData(UserImportDto row, int rowNumber)
    {
        var errors = new List<ImportErrorDto>();

        // 用户名验证
        if (string.IsNullOrWhiteSpace(row.UserName))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = rowNumber,
                FieldName = "用户名",
                ErrorMessage = "用户名不能为空",
                ErrorType = ImportErrorType.Validation
            });
        }
        else if (row.UserName.Length > 50)
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = rowNumber,
                FieldName = "用户名",
                FieldValue = row.UserName,
                ErrorMessage = "用户名长度不能超过50字符",
                ErrorType = ImportErrorType.Validation
            });
        }

        // 密码验证
        if (string.IsNullOrWhiteSpace(row.Password))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = rowNumber,
                FieldName = "密码",
                ErrorMessage = "密码不能为空",
                ErrorType = ImportErrorType.Validation
            });
        }
        else if (row.Password.Length > 50)
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = rowNumber,
                FieldName = "密码",
                FieldValue = row.Password,
                ErrorMessage = "密码长度不能超过50字符",
                ErrorType = ImportErrorType.Validation
            });
        }

        // 手机号格式验证（可选）
        if (!string.IsNullOrWhiteSpace(row.Phone))
        {
            if (!IsValidPhone(row.Phone))
            {
                errors.Add(new ImportErrorDto
                {
                    RowNumber = rowNumber,
                    FieldName = "手机号",
                    FieldValue = row.Phone,
                    ErrorMessage = "手机号格式不正确，应为11位数字",
                    ErrorType = ImportErrorType.Validation
                });
            }
        }

        // 邮箱格式验证（可选）
        if (!string.IsNullOrWhiteSpace(row.Email))
        {
            if (!IsValidEmail(row.Email))
            {
                errors.Add(new ImportErrorDto
                {
                    RowNumber = rowNumber,
                    FieldName = "邮箱",
                    FieldValue = row.Email,
                    ErrorMessage = "邮箱格式不正确",
                    ErrorType = ImportErrorType.Validation
                });
            }
        }

        return errors;
    }

    /// <summary>
    /// 验证角色导入数据
    /// </summary>
    /// <param name="row">角色导入数据行</param>
    /// <param name="rowNumber">Excel 行号</param>
    /// <returns>验证错误列表，验证通过时为空列表</returns>
    /// <remarks>
    /// 验证规则：
    /// - 角色名称必填，长度不超过50字符
    /// - 角色编码必填，长度不超过50字符
    /// </remarks>
    private List<ImportErrorDto> ValidateRoleImportData(RoleImportDto row, int rowNumber)
    {
        var errors = new List<ImportErrorDto>();

        // 角色名称验证
        if (string.IsNullOrWhiteSpace(row.RoleName))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = rowNumber,
                FieldName = "角色名称",
                ErrorMessage = "角色名称不能为空",
                ErrorType = ImportErrorType.Validation
            });
        }
        else if (row.RoleName.Length > 50)
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = rowNumber,
                FieldName = "角色名称",
                FieldValue = row.RoleName,
                ErrorMessage = "角色名称长度不能超过50字符",
                ErrorType = ImportErrorType.Validation
            });
        }

        // 角色编码验证
        if (string.IsNullOrWhiteSpace(row.RoleCode))
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = rowNumber,
                FieldName = "角色编码",
                ErrorMessage = "角色编码不能为空",
                ErrorType = ImportErrorType.Validation
            });
        }
        else if (row.RoleCode.Length > 50)
        {
            errors.Add(new ImportErrorDto
            {
                RowNumber = rowNumber,
                FieldName = "角色编码",
                FieldValue = row.RoleCode,
                ErrorMessage = "角色编码长度不能超过50字符",
                ErrorType = ImportErrorType.Validation
            });
        }

        return errors;
    }

    /// <summary>
    /// 验证手机号格式
    /// </summary>
    /// <param name="phone">手机号字符串</param>
    /// <returns>格式正确返回 true，否则返回 false</returns>
    /// <remarks>
    /// 验证规则：11位数字，以1开头。
    /// </remarks>
    private static bool IsValidPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        // 简单验证：11位数字，以1开头
        return phone.Length == 11 && phone.StartsWith("1") && phone.All(char.IsDigit);
    }

    /// <summary>
    /// 验证邮箱格式
    /// </summary>
    /// <param name="email">邮箱字符串</param>
    /// <returns>格式正确返回 true，否则返回 false</returns>
    /// <remarks>
    /// 验证规则：包含 @ 符号，@ 前后都有内容。
    /// </remarks>
    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // 简单验证：包含 @ 符号，@ 前后都有内容
        var parts = email.Split('@');
        return parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]) && !string.IsNullOrWhiteSpace(parts[1]);
    }
}