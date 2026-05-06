using BusinessManager.Basic.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Options;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;
using System.Text.Json;

namespace BusinessManager.Basic.Service;

/// <summary>
/// 大屏服务实现类
/// </summary>
/// <remarks>
/// 实现大屏相关的业务逻辑，包括大屏的增删改查、发布管理、分享权限、组件管理等功能。
/// 继承自<see cref="BaseService{ScreenConfig}"/>，使用SqlSugar进行数据库操作。
/// 大屏是可视化数据展示的核心载体，支持多种组件类型和灵活的布局配置。
/// </remarks>
public class ScreenService : BaseService<ScreenConfig>, IScreenService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<ScreenService> _logger { get; set; } = null!;

    /// <summary>
    /// 大屏模块配置选项
    /// </summary>
    public IOptions<ScreenOptions> _screenOptions { get; set; } = null!;

    #region 大屏 CRUD

    /// <summary>
    /// 获取大屏分页列表
    /// </summary>
    /// <param name="query">查询参数，包含分页信息和筛选条件</param>
    /// <returns>分页大屏列表，包含大屏基本信息</returns>
    /// <remarks>
    /// 查询流程：
    /// <list type="number">
    ///     <item>构建查询条件，默认过滤已删除状态</item>
    ///     <item>根据名称关键字进行模糊查询（可选）</item>
    ///     <item>根据公开状态进行精确匹配（可选）</item>
    ///     <item>按创建时间降序排序</item>
    ///     <item>分页查询并转换为DTO</item>
    /// </list>
    /// </remarks>
    public async Task<PageResponse<ScreenListDto>> GetPageListAsync(QueryScreenDto query)
    {
        var queryable = _db.Queryable<ScreenConfig>()
            .Where(s => s.Status == 1)
            .WhereIF(!string.IsNullOrEmpty(query.Name), s => s.Name.Contains(query.Name!))
            .WhereIF(query.IsPublic.HasValue, s => s.IsPublic == query.IsPublic)
            .OrderByDescending(s => s.CreateTime);

        var total = await queryable.CountAsync();
        var list = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize);

        var dtoList = list.Adapt<List<ScreenListDto>>();
        return PageResponse<ScreenListDto>.Create(dtoList, total, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取大屏详情（含组件列表）
    /// </summary>
    /// <param name="id">大屏ID</param>
    /// <returns>大屏详细配置，包含所有组件；大屏不存在返回null</returns>
    /// <remarks>
    /// 查询流程：
    /// <list type="number">
    ///     <item>查询大屏基本信息，过滤已删除状态</item>
    ///     <item>查询关联的所有组件，按排序顺序排列</item>
    ///     <item>组装完整的大屏配置DTO</item>
    /// </list>
    /// </remarks>
    public new async Task<ScreenConfigDto?> GetByIdAsync(Guid id)
    {
        var screen = await _db.Queryable<ScreenConfig>()
            .Where(s => s.Id == id && s.Status == 1)
            .FirstAsync();

        if (screen == null) return null;

        var components = await _db.Queryable<ScreenComponent>()
            .Where(c => c.ScreenId == id)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();

        var dto = screen.Adapt<ScreenConfigDto>();
        dto.Components = components.Adapt<List<ScreenComponentDto>>();
        return dto;
    }

    /// <summary>
    /// 创建大屏
    /// </summary>
    /// <param name="dto">创建大屏参数，包含名称、描述、样式等</param>
    /// <returns>新创建大屏的ID</returns>
    /// <remarks>
    /// 创建流程：
    /// <list type="number">
    ///     <item>映射DTO到实体</item>
    ///     <item>生成新的Guid作为主键</item>
    ///     <item>设置默认状态为1（正常）</item>
    ///     <item>设置创建时间为当前时间</item>
    ///     <item>插入记录并返回ID</item>
    /// </list>
    /// </remarks>
    public async Task<Guid> CreateAsync(CreateScreenDto dto)
    {
        var screen = dto.Adapt<ScreenConfig>();
        screen.Id = Guid.NewGuid();
        screen.Status = 1;
        screen.CreateTime = DateTime.Now;

        await InsertAsync(screen);
        return screen.Id;
    }

    /// <summary>
    /// 更新大屏信息
    /// </summary>
    /// <param name="dto">更新大屏参数，包含大屏ID和需要更新的字段</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 大屏不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 支持部分更新，只更新DTO中非空的字段。
    /// 可更新字段包括：名称、描述、缩略图、样式、权限配置、公开状态、组件列表。
    /// </remarks>
    public async Task<int> UpdateAsync(UpdateScreenDto dto)
    {
        var screen = await base.GetByIdAsync(dto.Id);
        if (screen == null)
        {
            throw BusinessException.NotFound("大屏不存在");
        }

        if (dto.Name != null) screen.Name = dto.Name;
        if (dto.Description != null) screen.Description = dto.Description;
        if (dto.Thumbnail != null) screen.Thumbnail = dto.Thumbnail;
        if (dto.Style != null) screen.Style = dto.Style;
        if (dto.Permissions != null) screen.Permissions = dto.Permissions;
        if (dto.IsPublic.HasValue) screen.IsPublic = dto.IsPublic.Value;
        screen.UpdateTime = DateTime.Now;

        // 更新大屏基本信息
        var result = await base.UpdateAsync(screen);

        // 处理组件列表更新
        if (dto.Components != null)
        {
            // 删除现有组件
            await _db.Deleteable<ScreenComponent>()
                .Where(c => c.ScreenId == dto.Id)
                .ExecuteCommandAsync();

            // 插入新组件
            for (int i = 0; i < dto.Components.Count; i++)
            {
                var compDto = dto.Components[i];
                var component = new ScreenComponent
                {
                    Id = compDto.Id != Guid.Empty ? compDto.Id : Guid.NewGuid(),
                    ScreenId = dto.Id,
                    ComponentType = compDto.ComponentType,
                    PositionX = compDto.PositionX,
                    PositionY = compDto.PositionY,
                    Width = compDto.Width,
                    Height = compDto.Height,
                    Rotation = compDto.Rotation,
                    Locked = compDto.Locked,
                    Visible = compDto.Visible,
                    DataSource = compDto.DataSource ?? "{}",
                    Config = compDto.Config ?? "{}",
                    StyleConfig = compDto.StyleConfig ?? "{}",
                    DataBinding = compDto.DataBinding ?? "{}",
                    SortOrder = i,
                    CreateTime = DateTime.Now
                };

                await _db.Insertable(component).ExecuteCommandAsync();
            }
        }

        return result;
    }

    /// <summary>
    /// 删除大屏（批量）
    /// </summary>
    /// <param name="ids">要删除的大屏ID列表</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 删除流程：
    /// <list type="number">
    ///     <item>删除关联的所有组件记录</item>
    ///     <item>删除关联的所有发布记录</item>
    ///     <item>软删除大屏配置（设置状态为0）</item>
    /// </list>
    /// 这是软删除，数据不会永久移除，只是标记为已删除状态。
    /// </remarks>
    public async Task<int> DeleteAsync(List<Guid> ids)
    {
        // 删除关联组件
        await _db.Deleteable<ScreenComponent>()
            .Where(c => ids.Contains(c.ScreenId))
            .ExecuteCommandAsync();

        // 删除发布记录
        await _db.Deleteable<ScreenPublish>()
            .Where(p => ids.Contains(p.ScreenId))
            .ExecuteCommandAsync();

        // 删除大屏（软删除）
        return await _db.Updateable<ScreenConfig>()
            .Where(s => ids.Contains(s.Id))
            .SetColumns(s => s.Status == 0)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 复制大屏
    /// </summary>
    /// <param name="id">要复制的大屏ID</param>
    /// <returns>复制后新大屏的完整配置</returns>
    /// <exception cref="BusinessException">
    /// 大屏不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 复制流程：
    /// <list type="number">
    ///     <item>获取原大屏完整配置（包含组件）</item>
    ///     <item>创建新大屏，名称添加"(副本)"后缀</item>
    ///     <item>设置新大屏为私有（IsPublic=0）</item>
    ///     <item>逐个复制所有组件到新大屏</item>
    ///     <item>返回新大屏完整配置</item>
    /// </list>
    /// </remarks>
    public async Task<ScreenConfigDto> CopyAsync(Guid id)
    {
        var original = await GetByIdAsync(id);
        if (original == null)
        {
            throw BusinessException.NotFound("大屏不存在");
        }

        // 创建新大屏
        var newScreen = new ScreenConfig
        {
            Id = Guid.NewGuid(),
            Name = original.Name + " (副本)",
            Description = original.Description,
            Thumbnail = original.Thumbnail,
            Style = original.Style,
            Permissions = original.Permissions,
            IsPublic = 0, // 复制后默认私有
            Status = 1,
            CreatedBy = original.CreatedBy,
            CreateTime = DateTime.Now
        };

        await InsertAsync(newScreen);

        // 复制组件
        foreach (var comp in original.Components)
        {
            var newComp = new ScreenComponent
            {
                Id = Guid.NewGuid(),
                ScreenId = newScreen.Id,
                ComponentType = comp.ComponentType,
                PositionX = comp.PositionX,
                PositionY = comp.PositionY,
                Width = comp.Width,
                Height = comp.Height,
                Rotation = comp.Rotation,
                Locked = comp.Locked,
                Visible = comp.Visible,
                DataSource = comp.DataSource,
                Config = comp.Config,
                StyleConfig = comp.StyleConfig,
                DataBinding = comp.DataBinding,
                SortOrder = comp.SortOrder,
                CreateTime = DateTime.Now
            };
            await _db.Insertable(newComp).ExecuteCommandAsync();
        }

        return (await GetByIdAsync(newScreen.Id))!;
    }

    #endregion

    #region 分享权限

    /// <summary>
    /// 设置分享配置
    /// </summary>
    /// <param name="dto">分享配置参数，包含大屏ID和权限配置</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 大屏不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 权限配置为JSON格式，可配置用户和角色的访问权限。
    /// </remarks>
    public async Task<int> ShareAsync(ShareScreenDto dto)
    {
        var screen = await base.GetByIdAsync(dto.Id);
        if (screen == null)
        {
            throw BusinessException.NotFound("大屏不存在");
        }

        screen.Permissions = dto.Permissions;
        screen.UpdateTime = DateTime.Now;

        return await base.UpdateAsync(screen);
    }

    /// <summary>
    /// 获取可分享用户列表
    /// </summary>
    /// <param name="screenId">大屏ID</param>
    /// <returns>可分享的用户列表</returns>
    /// <remarks>
    /// 查询所有状态正常的用户，供分享配置时选择。
    /// </remarks>
    public async Task<List<UserDto>> GetShareableUsersAsync(Guid screenId)
    {
        var users = await _db.Queryable<User>()
            .Where(u => u.Status == 1)
            .ToListAsync();

        return users.Adapt<List<UserDto>>();
    }

    /// <summary>
    /// 获取可分享角色列表
    /// </summary>
    /// <param name="screenId">大屏ID</param>
    /// <returns>可分享的角色列表</returns>
    /// <remarks>
    /// 查询所有状态正常的角色，供分享配置时选择。
    /// </remarks>
    public async Task<List<RoleDto>> GetShareableRolesAsync(Guid screenId)
    {
        var roles = await _db.Queryable<Role>()
            .Where(r => r.Status == 1)
            .ToListAsync();

        return roles.Adapt<List<RoleDto>>();
    }

    #endregion

    #region 发布管理

    /// <summary>
    /// 发布大屏
    /// </summary>
    /// <param name="screenId">要发布的大屏ID</param>
    /// <returns>发布结果，包含发布ID和访问URL</returns>
    /// <exception cref="BusinessException">
    /// 大屏不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 发布流程：
    /// <list type="number">
    ///     <item>获取大屏完整配置（包含组件）</item>
    ///     <item>生成发布ID和访问URL</item>
    ///     <item>序列化大屏完整数据为JSON快照</item>
    ///     <item>创建发布记录并保存</item>
    ///     <item>返回发布结果</item>
    /// </list>
    /// 发布后的大屏数据与原大屏独立，修改原大屏不影响已发布版本。
    /// </remarks>
    public async Task<PublishResultDto> PublishAsync(Guid screenId)
    {
        // 使用自定义的 GetByIdAsync 方法获取完整的大屏配置（包含组件）
        var screen = await GetByIdAsync(screenId);
        if (screen == null)
        {
            throw BusinessException.NotFound("大屏不存在");
        }

        // 创建发布记录
        var publishId = Guid.NewGuid();
        var publishUrl = $"/screen/published/{publishId}";

        // 序列化完整数据快照（包含组件）
        var screenData = JsonSerializer.Serialize(screen);

        var publish = new ScreenPublish
        {
            Id = publishId,
            ScreenId = screenId,
            ScreenName = screen.Name,
            ScreenData = screenData,
            PublishUrl = publishUrl,
            PublishedBy = screen.CreatedBy, // 实际应获取当前用户
            PublishTime = DateTime.Now,
            ViewCount = 0,
            Status = 1
        };

        await _db.Insertable(publish).ExecuteCommandAsync();

        return new PublishResultDto
        {
            PublishId = publishId,
            PublishUrl = publishUrl
        };
    }

    /// <summary>
    /// 下架大屏
    /// </summary>
    /// <param name="screenId">大屏ID（与publishId二选一）</param>
    /// <param name="publishId">发布ID（与screenId二选一）</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 支持两种下架方式：
    /// <list type="bullet">
    ///     <item>通过发布ID下架指定发布记录</item>
    ///     <item>通过大屏ID下架该大屏的所有发布记录</item>
    /// </list>
    /// 下架后状态变为0，但数据保留，可以重新上架。
    /// </remarks>
    public async Task<int> UnpublishAsync(Guid? screenId, Guid? publishId)
    {
        if (publishId.HasValue)
        {
            return await _db.Updateable<ScreenPublish>()
                .Where(p => p.Id == publishId.Value)
                .SetColumns(p => p.Status == 0)
                .ExecuteCommandAsync();
        }

        if (screenId.HasValue)
        {
            return await _db.Updateable<ScreenPublish>()
                .Where(p => p.ScreenId == screenId.Value)
                .SetColumns(p => p.Status == 0)
                .ExecuteCommandAsync();
        }

        return 0;
    }

    /// <summary>
    /// 重新上架
    /// </summary>
    /// <param name="publishId">发布ID</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 将已下架的发布记录状态恢复为1，使其重新可访问。
    /// </remarks>
    public async Task<int> RepublishAsync(Guid publishId)
    {
        return await _db.Updateable<ScreenPublish>()
            .Where(p => p.Id == publishId)
            .SetColumns(p => p.Status == 1)
            .ExecuteCommandAsync();
    }

    /// <summary>
    /// 获取发布后的大屏数据（前端兼容格式）
    /// </summary>
    /// <param name="publishId">发布ID</param>
    /// <returns>已发布大屏的完整配置；不存在返回null</returns>
    /// <remarks>
    /// 获取流程：
    /// <list type="number">
    ///     <item>查询发布记录，过滤已下架状态</item>
    ///     <item>增加访问计数</item>
    ///     <item>反序列化数据快照为DTO</item>
    ///     <item>转换为前端兼容格式</item>
    ///     <item>返回完整大屏配置</item>
    /// </list>
    /// 每次访问都会增加访问计数，用于统计大屏的访问热度。
    /// </remarks>
    public async Task<ScreenFrontendDto?> GetPublishedAsync(Guid publishId)
    {
        var publish = await _db.Queryable<ScreenPublish>()
            .Where(p => p.Id == publishId && p.Status == 1)
            .FirstAsync();

        if (publish == null) return null;

        // 增加访问计数
        await _db.Updateable<ScreenPublish>()
            .Where(p => p.Id == publishId)
            .SetColumns(p => p.ViewCount == p.ViewCount + 1)
            .ExecuteCommandAsync();

        // 反序列化快照数据
        var screen = JsonSerializer.Deserialize<ScreenConfigDto>(publish.ScreenData);
        if (screen == null) return null;

        // 转换为前端兼容格式
        return ConvertToFrontendDto(screen);
    }

    /// <summary>
    /// 将后端 DTO 转换为前端兼容格式
    /// </summary>
    /// <param name="screen">后端大屏配置 DTO</param>
    /// <returns>前端兼容的大屏配置 DTO</returns>
    /// <remarks>
    /// 转换内容包括：
    /// <list type="bullet">
    ///     <item>字段名从 PascalCase 转换为 camelCase</item>
    ///     <item>组件的扁平位置/尺寸结构转换为嵌套对象</item>
    ///     <item>JSON 字符串字段解析为对象</item>
    /// </list>
    /// </remarks>
    private ScreenFrontendDto ConvertToFrontendDto(ScreenConfigDto screen)
    {
        var frontendDto = new ScreenFrontendDto
        {
            Id = screen.Id.ToString(),
            Name = screen.Name,
            Description = screen.Description ?? string.Empty,
            Thumbnail = screen.Thumbnail,
            IsPublic = screen.IsPublic,
            CreatedBy = screen.CreatedBy.ToString(),
            CreatedAt = screen.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdatedAt = screen.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        };

        // 解析 Style JSON
        try
        {
            if (!string.IsNullOrEmpty(screen.Style) && screen.Style != "{}")
            {
                frontendDto.Style = JsonSerializer.Deserialize<ScreenStyleDto>(screen.Style);
            }
        }
        catch
        {
            frontendDto.Style = new ScreenStyleDto();
        }

        // 解析 Permissions JSON
        try
        {
            if (!string.IsNullOrEmpty(screen.Permissions) && screen.Permissions != "{}")
            {
                frontendDto.Permissions = JsonSerializer.Deserialize<ScreenPermissionsDto>(screen.Permissions);
            }
        }
        catch
        {
            frontendDto.Permissions = new ScreenPermissionsDto();
        }

        // 转换组件列表
        frontendDto.Components = screen.Components.Select(comp => new ScreenComponentFrontendDto
        {
            Id = comp.Id.ToString(),
            Type = comp.ComponentType,
            Position = new ComponentPositionDto { X = comp.PositionX, Y = comp.PositionY },
            Size = new ComponentSizeDto { Width = comp.Width, Height = comp.Height },
            Rotation = comp.Rotation,
            Locked = comp.Locked == 1,
            Visible = comp.Visible == 1,
            DataSource = SafeParseJson(comp.DataSource),
            Config = SafeParseJson(comp.Config),
            Style = SafeParseJson(comp.StyleConfig),
            DataBinding = SafeParseJson(comp.DataBinding)
        }).ToList();

        return frontendDto;
    }

    /// <summary>
    /// 安全解析 JSON 字符串为对象
    /// </summary>
    /// <param name="json">JSON 字符串</param>
    /// <returns>解析后的对象，解析失败返回 null</returns>
    private object? SafeParseJson(string? json)
    {
        if (string.IsNullOrEmpty(json) || json == "{}")
            return null;

        try
        {
            return JsonSerializer.Deserialize<object>(json);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 获取大屏发布状态信息
    /// </summary>
    /// <param name="screenId">大屏ID</param>
    /// <returns>发布状态信息；未发布返回null</returns>
    /// <remarks>
    /// 查询指定大屏的最新发布记录，返回发布状态信息。
    /// 如果大屏未发布或已下架，返回Published=false的状态信息。
    /// </remarks>
    public async Task<PublishInfoDto?> GetPublishInfoAsync(Guid screenId)
    {
        var publish = await _db.Queryable<ScreenPublish>()
            .Where(p => p.ScreenId == screenId && p.Status == 1)
            .OrderByDescending(p => p.PublishTime)
            .FirstAsync();

        if (publish == null)
        {
            return new PublishInfoDto { Published = false };
        }

        return new PublishInfoDto
        {
            Published = true,
            PublishId = publish.Id,
            PublishUrl = publish.PublishUrl,
            PublishedAt = publish.PublishTime,
            ViewCount = publish.ViewCount
        };
    }

    /// <summary>
    /// 获取发布记录列表
    /// </summary>
    /// <param name="query">查询参数，包含分页信息</param>
    /// <returns>分页发布记录列表</returns>
    /// <remarks>
    /// 查询所有发布记录，支持按大屏名称筛选。
    /// 按发布时间降序排序，包含已发布和已下架的记录。
    /// </remarks>
    public async Task<PageResponse<ScreenPublishDto>> GetPublishListAsync(QueryScreenDto query)
    {
        var queryable = _db.Queryable<ScreenPublish>()
            .WhereIF(!string.IsNullOrEmpty(query.Name), p => p.ScreenName.Contains(query.Name!))
            .OrderByDescending(p => p.PublishTime);

        var total = await queryable.CountAsync();
        var list = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize);

        var dtoList = list.Select(p => new ScreenPublishDto
        {
            PublishId = p.Id,
            ScreenId = p.ScreenId,
            ScreenName = p.ScreenName,
            ScreenData = p.ScreenData,
            PublishUrl = p.PublishUrl,
            PublishedBy = p.PublishedBy,
            PublishTime = p.PublishTime,
            ViewCount = p.ViewCount,
            Status = p.Status
        }).ToList();

        return PageResponse<ScreenPublishDto>.Create(dtoList, total, query.PageIndex, query.PageSize);
    }

    #endregion

    #region 数据源（Mock模式）

    /// <summary>
    /// 获取数据源选项列表
    /// </summary>
    /// <returns>可用的数据源选项列表</returns>
    /// <remarks>
    /// Mock模式下返回配置的模拟数据源列表。
    /// 实际模式查询真实数据源表。
    /// </remarks>
    public async Task<List<DatasourceOptionDto>> GetDatasourcesAsync()
    {
        if (_screenOptions.Value.IsUseMock)
        {
            return _screenOptions.Value.MockDatasources
                .Select(m => new DatasourceOptionDto
                {
                    Id = Guid.TryParse(m.Id, out var guid) ? guid : Guid.Empty,
                    Name = m.Name,
                    Type = m.Type
                }).ToList();
        }

        // 从数据库查询数据源列表
        var datasources = await _db.Queryable<Datasource>()
            .Where(d => d.Status == "connected")
            .OrderByDescending(d => d.CreateTime)
            .ToListAsync();

        return datasources.Select(d => new DatasourceOptionDto
        {
            Id = d.Id,
            Name = d.Name,
            Type = d.Type
        }).ToList();
    }

    /// <summary>
    /// 执行SQL查询
    /// </summary>
    /// <param name="dto">执行SQL参数，包含数据源ID和SQL语句</param>
    /// <returns>SQL查询结果，包含数据和列信息</returns>
    /// <remarks>
    /// Mock模式下返回模拟数据，用于前端组件开发调试。
    /// 实际模式应连接真实数据源执行SQL（待实现）。
    /// </remarks>
    public async Task<SqlResultDto> ExecuteSqlAsync(ExecuteSqlDto dto)
    {
        if (_screenOptions.Value.IsUseMock)
        {
            // 返回模拟数据
            return new SqlResultDto
            {
                Data = new List<object>
                {
                    new { name = "示例数据1", value = 100 },
                    new { name = "示例数据2", value = 200 },
                    new { name = "示例数据3", value = 150 }
                },
                Columns = new List<SqlColumnInfo>
                {
                    new SqlColumnInfo { Name = "name", Type = "string" },
                    new SqlColumnInfo { Name = "value", Type = "number" }
                }
            };
        }

        // 获取数据源信息
        var datasource = await _db.Queryable<Datasource>()
            .Where(d => d.Id == dto.DatasourceId)
            .FirstAsync();

        if (datasource == null)
        {
            throw new BusinessException("数据源不存在");
        }

        // 根据数据源类型执行SQL
        if (datasource.Type == "mysql")
        {
            var connectionString = $"Server={datasource.Host};Port={datasource.Port};Database={datasource.Database};User ID={datasource.Username};Password={datasource.Password};Charset=utf8mb4;";

            // 使用 SqlSugar 动态连接执行 SQL
            var db = new SqlSugarScope(new ConnectionConfig
            {
                ConnectionString = connectionString,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true
            });

            // 执行 SQL 查询
            var dataTable = db.Ado.GetDataTable(dto.Sql);

            var result = new SqlResultDto
            {
                Columns = new List<SqlColumnInfo>(),
                Data = new List<object>()
            };

            // 获取列信息
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var column = dataTable.Columns[i];
                result.Columns.Add(new SqlColumnInfo
                {
                    Name = column.ColumnName,
                    Type = column.DataType.Name.ToLower()
                });
            }

            // 获取数据
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                var row = dataTable.Rows[i];
                var rowDict = new Dictionary<string, object>();
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    var value = row[j];
                    rowDict[result.Columns[j].Name] = value == DBNull.Value ? null! : value;
                }
                result.Data.Add(rowDict);
            }

            return result;
        }
        else
        {
            throw new BusinessException($"暂不支持 {datasource.Type} 类型数据源的SQL执行");
        }
    }

    /// <summary>
    /// 验证SQL语法
    /// </summary>
    /// <param name="dto">验证SQL参数，包含数据源ID和SQL语句</param>
    /// <returns>验证结果，包含是否有效和错误信息</returns>
    /// <remarks>
    /// Mock模式下默认返回验证成功。
    /// 实际模式应连接真实数据源验证SQL（待实现）。
    /// </remarks>
    public async Task<SqlValidateDto> ValidateSqlAsync(ValidateSqlDto dto)
    {
        if (_screenOptions.Value.IsUseMock)
        {
            return new SqlValidateDto
            {
                Valid = true,
                Message = "SQL语法验证通过",
                Columns = new List<SqlColumnInfo>
                {
                    new SqlColumnInfo { Name = "name", Type = "string" },
                    new SqlColumnInfo { Name = "value", Type = "number" }
                }
            };
        }

        // TODO: 实际验证SQL
        return new SqlValidateDto { Valid = true };
    }

    #endregion

    #region 配置验证

    /// <summary>
    /// 验证大屏组件配置
    /// </summary>
    /// <param name="componentsJson">组件列表JSON字符串</param>
    /// <returns>验证结果，包含是否有效和错误列表</returns>
    /// <remarks>
    /// 验证JSON格式是否正确，能否解析为组件列表。
    /// </remarks>
    public async Task<ValidateResultDto> ValidateConfigAsync(string componentsJson)
    {
        var result = new ValidateResultDto { Valid = true };

        try
        {
            var components = JsonSerializer.Deserialize<List<object>>(componentsJson);
            if (components == null)
            {
                result.Valid = false;
                result.Errors.Add("组件配置JSON格式错误");
            }
        }
        catch (JsonException ex)
        {
            result.Valid = false;
            result.Errors.Add($"JSON解析错误: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// 验证单个组件配置
    /// </summary>
    /// <param name="component">组件配置对象</param>
    /// <returns>验证结果，包含是否有效和错误列表</returns>
    /// <remarks>
    /// 验证组件是否包含必需的type字段。
    /// </remarks>
    public async Task<ValidateResultDto> ValidateComponentAsync(object component)
    {
        var result = new ValidateResultDto { Valid = true };

        try
        {
            var json = JsonSerializer.Serialize(component);
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

            if (dict == null || !dict.ContainsKey("type"))
            {
                result.Valid = false;
                result.Errors.Add("组件必须包含type字段");
            }
        }
        catch (Exception ex)
        {
            result.Valid = false;
            result.Errors.Add($"组件验证错误: {ex.Message}");
        }

        return result;
    }

    #endregion

    #region 组件管理

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <param name="dto">创建组件参数，包含大屏ID、组件类型和配置</param>
    /// <returns>新创建组件的ID</returns>
    /// <remarks>
    /// 添加流程：
    /// <list type="number">
    ///     <item>映射DTO到实体</item>
    ///     <item>生成新的Guid作为主键</item>
    ///     <item>设置创建时间为当前时间</item>
    ///     <item>插入记录并返回ID</item>
    /// </list>
    /// </remarks>
    public async Task<Guid> AddComponentAsync(CreateComponentDto dto)
    {
        var component = dto.Adapt<ScreenComponent>();
        component.Id = Guid.NewGuid();
        component.CreateTime = DateTime.Now;

        await _db.Insertable(component).ExecuteCommandAsync();
        return component.Id;
    }

    /// <summary>
    /// 更新组件
    /// </summary>
    /// <param name="dto">更新组件参数，包含组件ID和需要更新的字段</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 组件不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 支持部分更新，只更新DTO中非空的字段。
    /// 可更新字段包括：位置、尺寸、旋转、锁定状态、可见状态、数据源配置、组件配置、样式配置、数据绑定、排序顺序。
    /// </remarks>
    public async Task<int> UpdateComponentAsync(UpdateComponentDto dto)
    {
        var component = await _db.Queryable<ScreenComponent>()
            .Where(c => c.Id == dto.Id)
            .FirstAsync();

        if (component == null)
        {
            throw BusinessException.NotFound("组件不存在");
        }

        if (dto.PositionX.HasValue) component.PositionX = dto.PositionX.Value;
        if (dto.PositionY.HasValue) component.PositionY = dto.PositionY.Value;
        if (dto.Width.HasValue) component.Width = dto.Width.Value;
        if (dto.Height.HasValue) component.Height = dto.Height.Value;
        if (dto.Rotation.HasValue) component.Rotation = dto.Rotation.Value;
        if (dto.Locked.HasValue) component.Locked = dto.Locked.Value;
        if (dto.Visible.HasValue) component.Visible = dto.Visible.Value;
        if (dto.DataSource != null) component.DataSource = dto.DataSource;
        if (dto.Config != null) component.Config = dto.Config;
        if (dto.StyleConfig != null) component.StyleConfig = dto.StyleConfig;
        if (dto.DataBinding != null) component.DataBinding = dto.DataBinding;
        if (dto.SortOrder.HasValue) component.SortOrder = dto.SortOrder.Value;
        component.UpdateTime = DateTime.Now;

        return await _db.Updateable(component).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除组件
    /// </summary>
    /// <param name="componentId">要删除的组件ID</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 物理删除组件记录，数据将永久移除。
    /// </remarks>
    public async Task<int> DeleteComponentAsync(Guid componentId)
    {
        return await _db.Deleteable<ScreenComponent>()
            .Where(c => c.Id == componentId)
            .ExecuteCommandAsync();
    }

    #endregion
}