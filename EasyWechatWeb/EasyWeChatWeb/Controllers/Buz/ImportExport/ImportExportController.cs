using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 数据导入导出控制器
/// </summary>
/// <remarks>
/// 提供用户和角色数据的导入导出功能接口。
/// 支持 Excel 格式文件 (.xlsx) 的读取和生成。
///
/// 功能包括：
/// - 用户数据批量导出和导入
/// - 角色数据批量导出和导入
/// - 导入模板下载
///
/// 所有接口需要 JWT Token 认证。
/// 导入结果包含详细的错误信息，便于用户修正数据。
/// </remarks>
/// <example>
/// <code>
/// // 导出用户
/// POST /api/importexport/export-users
/// Authorization: Bearer {token}
/// Content-Type: application/json
/// Body: [1, 2, 3]  // 用户ID列表，可选
///
/// // 导入用户
/// POST /api/importexport/import-users
/// Authorization: Bearer {token}
/// Content-Type: multipart/form-data
/// Body: file (Excel 文件)
///
/// // 下载模板
/// GET /api/importexport/template/User
/// Authorization: Bearer {token}
/// </code>
/// </example>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImportExportController : BaseController
{
    /// <summary>
    /// 导入导出服务接口
    /// </summary>
    public IImportExportService _importExportService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<ImportExportController> _logger { get; set; } = null!;

    /// <summary>
    /// 导出用户数据
    /// </summary>
    /// <param name="userIds">要导出的用户ID列表，为空时导出全部用户</param>
    /// <returns>Excel 文件下载响应</returns>
    /// <response code="200">成功导出，返回 Excel 文件</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="500">服务器内部错误</response>
    /// <remarks>
    /// 导出流程：
    /// <list type="number">
    ///     <item><description>根据 userIds 查询用户数据（空列表则查询全部）</description></item>
    ///     <item><description>生成 Excel 文件并返回</description></item>
    /// </list>
    ///
    /// 导出字段：用户名、真实姓名、手机号、邮箱、状态、创建时间、角色。
    /// 不导出敏感字段如密码。
    ///
    /// 文件命名格式：users_{yyyyMMdd_HHmmss}.xlsx
    /// </remarks>
    /// <example>
    /// <code>
    /// // 导出指定用户
    /// POST /api/importexport/export-users
    /// Authorization: Bearer {token}
    /// Content-Type: application/json
    /// Body: [1, 2, 3]
    ///
    /// // 导出全部用户
    /// POST /api/importexport/export-users
    /// Authorization: Bearer {token}
    /// Content-Type: application/json
    /// Body: null  或 []
    /// </code>
    /// </example>
    [HttpPost("export-users")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    public async Task<IActionResult> ExportUsers([FromBody] List<Guid>? userIds = null)
    {
        try
        {
            _logger.LogInformation("开始导出用户数据，操作用户: {OperatorId}", GetCurrentUserId());

            var bytes = await _importExportService.ExportUsersAsync(userIds);

            var fileName = $"users_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            _logger.LogInformation("用户数据导出完成，文件名: {FileName}", fileName);

            return File(bytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "导出用户数据失败");
            return StatusCode(500, ApiResponse<object>.Error("导出用户数据失败"));
        }
    }

    /// <summary>
    /// 导入用户数据
    /// </summary>
    /// <param name="file">上传的 Excel 文件</param>
    /// <returns>导入结果，包含成功数量、失败数量和错误详情</returns>
    /// <response code="200">导入完成，返回导入结果（可能包含部分失败）</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">文件格式错误或文件为空</response>
    /// <response code="500">服务器内部错误</response>
    /// <remarks>
    /// 导入流程：
    /// <list type="number">
    ///     <item><description>读取上传的 Excel 文件</description></item>
    ///     <item><description>逐行验证数据格式和业务规则</description></item>
    ///     <item><description>验证用户名唯一性</description></item>
    ///     <item><description>插入验证通过的用户数据</description></item>
    ///     <item><description>返回导入结果和错误详情</description></item>
    /// </list>
    ///
    /// Excel 列标题要求：
    /// - 用户名（必填）
    /// - 密码（必填）
    /// - 真实姓名（可选）
    /// - 手机号（可选，11位数字）
    /// - 邮箱（可选，需符合邮箱格式）
    /// - 状态（可选，1-正常，0-禁用，默认为1）
    /// - 角色（可选，多个角色用逗号分隔）
    ///
    /// 导入结果包含：
    /// - 成功数量和失败数量
    /// - 每条失败记录的错误详情
    /// - 失败原因包括：验证失败、用户名重复、插入失败等
    /// </remarks>
    /// <example>
    /// <code>
    /// // 导入用户
    /// POST /api/importexport/import-users
    /// Authorization: Bearer {token}
    /// Content-Type: multipart/form-data
    /// Form Data: file = users.xlsx
    ///
    /// // 响应示例
    /// {
    ///     "code": 200,
    ///     "message": "导入完成，成功95条，失败5条",
    ///     "data": {
    ///         "successCount": 95,
    ///         "failCount": 5,
    ///         "totalCount": 100,
    ///         "hasErrors": true,
    ///         "errors": [
    ///             {
    ///                 "rowNumber": 10,
    ///                 "fieldName": "用户名",
    ///                 "fieldValue": "admin",
    ///                 "errorMessage": "用户名 'admin' 已存在",
    ///                 "errorType": 2
    ///             }
    ///         ]
    ///     }
    /// }
    /// </code>
    /// </example>
    [HttpPost("import-users")]
    [ProducesResponseType(typeof(ApiResponse<ImportResultDto>), 200)]
    public async Task<ApiResponse<ImportResultDto>> ImportUsers(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return Error<ImportResultDto>("上传文件不能为空", 400);
            }

            // 检查文件扩展名
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx")
            {
                return Error<ImportResultDto>("只支持 .xlsx 格式的 Excel 文件", 400);
            }

            _logger.LogInformation("开始导入用户数据，文件名: {FileName}, 操作用户: {OperatorId}",
                file.FileName, GetCurrentUserId());

            // 读取文件内容
            using var stream = file.OpenReadStream();
            var bytes = new byte[file.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 执行导入
            var operatorId = GetCurrentUserId();
            var result = await _importExportService.ImportUsersAsync(bytes, operatorId);

            _logger.LogInformation("用户数据导入完成，成功: {SuccessCount}，失败: {FailCount}",
                result.SuccessCount, result.FailCount);

            return Success(result, result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "导入用户数据失败");
            return Error<ImportResultDto>("导入用户数据失败");
        }
    }

    /// <summary>
    /// 导出角色数据
    /// </summary>
    /// <param name="roleIds">要导出的角色ID列表，为空时导出全部角色</param>
    /// <returns>Excel 文件下载响应</returns>
    /// <response code="200">成功导出，返回 Excel 文件</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="500">服务器内部错误</response>
    /// <remarks>
    /// 导出流程：
    /// <list type="number">
    ///     <item><description>根据 roleIds 查询角色数据（空列表则查询全部）</description></item>
    ///     <item><description>生成 Excel 文件并返回</description></item>
    /// </list>
    ///
    /// 导出字段：角色名称、角色编码、描述、状态、创建时间。
    ///
    /// 文件命名格式：roles_{yyyyMMdd_HHmmss}.xlsx
    /// </remarks>
    /// <example>
    /// <code>
    /// // 导出指定角色
    /// POST /api/importexport/export-roles
    /// Authorization: Bearer {token}
    /// Content-Type: application/json
    /// Body: [1, 2]
    ///
    /// // 导出全部角色
    /// POST /api/importexport/export-roles
    /// Authorization: Bearer {token}
    /// Content-Type: application/json
    /// Body: null  或 []
    /// </code>
    /// </example>
    [HttpPost("export-roles")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    public async Task<IActionResult> ExportRoles([FromBody] List<Guid>? roleIds = null)
    {
        try
        {
            _logger.LogInformation("开始导出角色数据，操作用户: {OperatorId}", GetCurrentUserId());

            var bytes = await _importExportService.ExportRolesAsync(roleIds);

            var fileName = $"roles_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            _logger.LogInformation("角色数据导出完成，文件名: {FileName}", fileName);

            return File(bytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "导出角色数据失败");
            return StatusCode(500, ApiResponse<object>.Error("导出角色数据失败"));
        }
    }

    /// <summary>
    /// 导入角色数据
    /// </summary>
    /// <param name="file">上传的 Excel 文件</param>
    /// <returns>导入结果，包含成功数量、失败数量和错误详情</returns>
    /// <response code="200">导入完成，返回导入结果（可能包含部分失败）</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">文件格式错误或文件为空</response>
    /// <response code="500">服务器内部错误</response>
    /// <remarks>
    /// 导入流程：
    /// <list type="number">
    ///     <item><description>读取上传的 Excel 文件</description></item>
    ///     <item><description>逐行验证数据格式和业务规则</description></item>
    ///     <item><description>验证角色名称和编码唯一性</description></item>
    ///     <item><description>插入验证通过的角色数据</description></item>
    ///     <item><description>返回导入结果和错误详情</description></item>
    /// </list>
    ///
    /// Excel 列标题要求：
    /// - 角色名称（必填）
    /// - 角色编码（必填）
    /// - 描述（可选）
    /// - 状态（可选，1-正常，0-禁用，默认为1）
    ///
    /// 导入结果包含：
    /// - 成功数量和失败数量
    /// - 每条失败记录的错误详情
    /// - 失败原因包括：验证失败、名称重复、编码重复、插入失败等
    /// </remarks>
    /// <example>
    /// <code>
    /// // 导入角色
    /// POST /api/importexport/import-roles
    /// Authorization: Bearer {token}
    /// Content-Type: multipart/form-data
    /// Form Data: file = roles.xlsx
    ///
    /// // 响应示例
    /// {
    ///     "code": 200,
    ///     "message": "导入完成，成功8条，失败2条",
    ///     "data": {
    ///         "successCount": 8,
    ///         "failCount": 2,
    ///         "totalCount": 10,
    ///         "hasErrors": true,
    ///         "errors": [
    ///             {
    ///                 "rowNumber": 5,
    ///                 "fieldName": "角色编码",
    ///                 "fieldValue": "admin",
    ///                 "errorMessage": "角色编码 'admin' 已存在",
    ///                 "errorType": 2
    ///             }
    ///         ]
    ///     }
    /// }
    /// </code>
    /// </example>
    [HttpPost("import-roles")]
    [ProducesResponseType(typeof(ApiResponse<ImportResultDto>), 200)]
    public async Task<ApiResponse<ImportResultDto>> ImportRoles(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return Error<ImportResultDto>("上传文件不能为空", 400);
            }

            // 检查文件扩展名
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx")
            {
                return Error<ImportResultDto>("只支持 .xlsx 格式的 Excel 文件", 400);
            }

            _logger.LogInformation("开始导入角色数据，文件名: {FileName}, 操作用户: {OperatorId}",
                file.FileName, GetCurrentUserId());

            // 读取文件内容
            using var stream = file.OpenReadStream();
            var bytes = new byte[file.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 执行导入
            var operatorId = GetCurrentUserId();
            var result = await _importExportService.ImportRolesAsync(bytes, operatorId);

            _logger.LogInformation("角色数据导入完成，成功: {SuccessCount}，失败: {FailCount}",
                result.SuccessCount, result.FailCount);

            return Success(result, result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "导入角色数据失败");
            return Error<ImportResultDto>("导入角色数据失败");
        }
    }

    /// <summary>
    /// 获取导入模板
    /// </summary>
    /// <param name="type">模板类型：User-用户模板，Role-角色模板</param>
    /// <returns>Excel 模板文件下载响应</returns>
    /// <response code="200">成功返回模板文件</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">模板类型参数无效</response>
    /// <response code="500">服务器内部错误</response>
    /// <remarks>
    /// 生成包含列标题和示例数据的空白模板。
    /// 用户可下载模板填写数据后上传导入。
    ///
    /// 模板特点：
    /// - 包含所有必填和可选字段的列标题
    /// - 第一行为示例数据，便于理解格式
    /// - 列标题使用中文，便于理解
    ///
    /// 支持的类型：
    /// - User：用户导入模板
    /// - Role：角色导入模板
    ///
    /// 文件命名格式：{type}_import_template.xlsx
    /// </remarks>
    /// <example>
    /// <code>
    /// // 获取用户导入模板
    /// GET /api/importexport/template/User
    /// Authorization: Bearer {token}
    ///
    /// // 获取角色导入模板
    /// GET /api/importexport/template/Role
    /// Authorization: Bearer {token}
    ///
    /// // 响应：返回 Excel 文件下载
    /// Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
    /// Content-Disposition: attachment; filename=User_import_template.xlsx
    /// </code>
    /// </example>
    [HttpGet("template/{type}")]
    [ProducesResponseType(typeof(FileContentResult), 200)]
    public async Task<IActionResult> GetTemplate(string type)
    {
        try
        {
            _logger.LogInformation("获取导入模板，类型: {Type}, 操作用户: {OperatorId}",
                type, GetCurrentUserId());

            var bytes = await _importExportService.GetImportTemplateAsync(type);

            var fileName = $"{type}_import_template.xlsx";
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            _logger.LogInformation("导入模板生成完成，类型: {Type}, 文件名: {FileName}", type, fileName);

            return File(bytes, contentType, fileName);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "模板类型参数无效: {Type}", type);
            return BadRequest(ApiResponse<object>.Error($"不支持的模板类型: {type}", 400));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取导入模板失败，类型: {Type}", type);
            return StatusCode(500, ApiResponse<object>.Error("获取导入模板失败"));
        }
    }
}