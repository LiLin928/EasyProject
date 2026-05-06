using BusinessManager.Basic.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Basic;

/// <summary>
/// 大屏管理控制器
/// 提供大屏的增删改查、发布管理、分享权限、组件管理等功能
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScreenController : BaseController
{
    /// <summary>
    /// 大屏服务接口
    /// </summary>
    private readonly IScreenService _screenService;

    /// <summary>
    /// 日志记录器
    /// </summary>
    private readonly ILogger<ScreenController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="screenService">大屏服务接口</param>
    /// <param name="logger">日志记录器</param>
    public ScreenController(IScreenService screenService, ILogger<ScreenController> logger)
    {
        _screenService = screenService;
        _logger = logger;
    }

    #region 大屏 CRUD

    /// <summary>
    /// 获取大屏列表（分页）
    /// </summary>
    /// <param name="dto">查询参数</param>
    /// <returns>分页后的大屏列表</returns>
    /// <response code="200">成功获取大屏列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 支持通过名称进行模糊搜索。
    /// 返回结果按创建时间倒序排列。
    /// </remarks>
    /// <example>
    /// POST /api/screen/list
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "pageIndex": 1,
    ///     "pageSize": 12,
    ///     "name": "销售",
    ///     "isPublic": 1
    /// }
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<ScreenListDto>>), 200)]
    public async Task<ApiResponse<PageResponse<ScreenListDto>>> GetList([FromBody] QueryScreenDto dto)
    {
        try
        {
            var result = await _screenService.GetPageListAsync(dto);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取大屏列表失败");
            return Error<PageResponse<ScreenListDto>>("获取大屏列表失败");
        }
    }

    /// <summary>
    /// 获取大屏详情
    /// </summary>
    /// <param name="id">大屏ID</param>
    /// <returns>大屏详细配置，包含所有组件</returns>
    /// <response code="200">成功获取大屏详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">大屏不存在</response>
    /// <remarks>
    /// 根据大屏ID获取大屏的完整配置，包括基本信息和所有组件配置。
    /// </remarks>
    /// <example>
    /// GET /api/screen/detail/00000000-0000-0000-0000-000000000001
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<ScreenConfigDto>), 200)]
    public async Task<ApiResponse<ScreenConfigDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _screenService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<ScreenConfigDto>("大屏不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取大屏详情失败: {Id}", id);
            return Error<ScreenConfigDto>("获取大屏详情失败");
        }
    }

    /// <summary>
    /// 创建大屏
    /// </summary>
    /// <param name="dto">创建大屏请求参数，包含名称、描述、样式等</param>
    /// <returns>新创建大屏的ID</returns>
    /// <response code="200">创建成功，返回新大屏ID</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 创建新的大屏，名称必须提供。
    /// 创建者会自动设置为当前登录用户。
    /// </remarks>
    /// <example>
    /// POST /api/screen/create
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "name": "销售数据大屏",
    ///     "description": "展示销售数据的可视化大屏",
    ///     "style": "{\"backgroundColor\":\"#1a1a2e\",\"gridSize\":10}",
    ///     "isPublic": 0
    /// }
    /// </example>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] CreateScreenDto dto)
    {
        try
        {
            var id = await _screenService.CreateAsync(dto);
            return Success(id, "创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建大屏失败");
            return Error<Guid>("创建大屏失败");
        }
    }

    /// <summary>
    /// 更新大屏
    /// </summary>
    /// <param name="dto">更新大屏请求参数，包含大屏ID和需要更新的字段</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 更新大屏信息，只更新请求中提供的字段。
    /// </remarks>
    /// <example>
    /// POST /api/screen/update
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "id": "00000000-0000-0000-0000-000000000001",
    ///     "name": "新名称",
    ///     "description": "新描述"
    /// }
    /// </example>
    [HttpPost("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateScreenDto dto)
    {
        try
        {
            var result = await _screenService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新大屏失败: {Id}", dto.Id);
            return Error<int>("更新大屏失败");
        }
    }

    /// <summary>
    /// 删除大屏（批量）
    /// </summary>
    /// <param name="dto">删除大屏请求参数，包含要删除的大屏ID列表</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 批量删除大屏，同时删除大屏的所有组件和发布记录。
    /// </remarks>
    /// <example>
    /// POST /api/screen/delete
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "ids": ["00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002"]
    /// }
    /// </example>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] DeleteScreenDto dto)
    {
        try
        {
            var result = await _screenService.DeleteAsync(dto.Ids);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除大屏失败: {Ids}", dto.Ids);
            return Error<int>("删除大屏失败");
        }
    }

    /// <summary>
    /// 复制大屏
    /// </summary>
    /// <param name="id">要复制的大屏ID</param>
    /// <returns>复制后新大屏的完整配置</returns>
    /// <response code="200">复制成功，返回新大屏配置</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">源大屏不存在</response>
    /// <remarks>
    /// 复制一个大屏及其所有组件，生成一个完全相同的新大屏。
    /// 新大屏名称会自动添加"-副本"后缀。
    /// </remarks>
    /// <example>
    /// POST /api/screen/copy/00000000-0000-0000-0000-000000000001
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("copy/{id}")]
    [ProducesResponseType(typeof(ApiResponse<ScreenConfigDto>), 200)]
    public async Task<ApiResponse<ScreenConfigDto>> Copy(Guid id)
    {
        try
        {
            var result = await _screenService.CopyAsync(id);
            return Success(result, "复制成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "复制大屏失败: {Id}", id);
            return Error<ScreenConfigDto>("复制大屏失败");
        }
    }

    #endregion

    #region 分享权限

    /// <summary>
    /// 设置大屏分享权限
    /// </summary>
    /// <param name="dto">分享配置参数，包含大屏ID和权限配置</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">设置成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 设置大屏的分享权限，包括查看权限和编辑权限。
    /// 权限配置为JSON格式，包含view和edit两个数组。
    /// </remarks>
    /// <example>
    /// POST /api/screen/share
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "id": "00000000-0000-0000-0000-000000000001",
    ///     "permissions": "{\"view\":[\"user1\",\"user2\"],\"edit\":[\"user1\"]}"
    /// }
    /// </example>
    [HttpPost("share")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Share([FromBody] ShareScreenDto dto)
    {
        try
        {
            var result = await _screenService.ShareAsync(dto);
            return Success(result, "设置分享权限成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设置大屏分享权限失败: {Id}", dto.Id);
            return Error<int>("设置分享权限失败");
        }
    }

    /// <summary>
    /// 获取可分享的用户列表
    /// </summary>
    /// <param name="id">大屏ID</param>
    /// <returns>可分享的用户列表</returns>
    /// <response code="200">成功获取用户列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取当前大屏可以分享给的用户列表，排除已拥有权限的用户和创建者。
    /// </remarks>
    /// <example>
    /// POST /api/screen/shareable-users/00000000-0000-0000-0000-000000000001
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("shareable-users/{id}")]
    [ProducesResponseType(typeof(ApiResponse<List<UserDto>>), 200)]
    public async Task<ApiResponse<List<UserDto>>> GetShareableUsers(Guid id)
    {
        try
        {
            var result = await _screenService.GetShareableUsersAsync(id);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取可分享用户列表失败: {Id}", id);
            return Error<List<UserDto>>("获取可分享用户列表失败");
        }
    }

    /// <summary>
    /// 获取可分享的角色列表
    /// </summary>
    /// <param name="id">大屏ID</param>
    /// <returns>可分享的角色列表</returns>
    /// <response code="200">成功获取角色列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取当前大屏可以分享给的角色列表，用于按角色批量分享。
    /// </remarks>
    /// <example>
    /// POST /api/screen/shareable-roles/00000000-0000-0000-0000-000000000001
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("shareable-roles/{id}")]
    [ProducesResponseType(typeof(ApiResponse<List<RoleDto>>), 200)]
    public async Task<ApiResponse<List<RoleDto>>> GetShareableRoles(Guid id)
    {
        try
        {
            var result = await _screenService.GetShareableRolesAsync(id);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取可分享角色列表失败: {Id}", id);
            return Error<List<RoleDto>>("获取可分享角色列表失败");
        }
    }

    #endregion

    #region 发布管理

    /// <summary>
    /// 发布大屏
    /// </summary>
    /// <param name="dto">发布大屏请求参数，包含大屏ID</param>
    /// <returns>发布结果，包含发布ID和访问URL</returns>
    /// <response code="200">发布成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 发布大屏生成公开访问链接。
    /// 发布时会保存大屏当前配置的快照。
    /// </remarks>
    /// <example>
    /// POST /api/screen/publish
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "screenId": "00000000-0000-0000-0000-000000000001"
    /// }
    /// </example>
    [HttpPost("publish")]
    [ProducesResponseType(typeof(ApiResponse<PublishResultDto>), 200)]
    public async Task<ApiResponse<PublishResultDto>> Publish([FromBody] PublishScreenDto dto)
    {
        try
        {
            var result = await _screenService.PublishAsync(dto.ScreenId);
            return Success(result, "发布成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发布大屏失败: {ScreenId}", dto.ScreenId);
            return Error<PublishResultDto>("发布大屏失败");
        }
    }

    /// <summary>
    /// 下架大屏
    /// </summary>
    /// <param name="dto">下架大屏请求参数，包含大屏ID或发布ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">下架成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 下架已发布的大屏，使其公开访问链接失效。
    /// 可以通过大屏ID或发布ID指定要下架的发布记录。
    /// </remarks>
    /// <example>
    /// POST /api/screen/unpublish
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "screenId": "00000000-0000-0000-0000-000000000001"
    /// }
    /// </example>
    [HttpPost("unpublish")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Unpublish([FromBody] UnpublishScreenDto dto)
    {
        try
        {
            var result = await _screenService.UnpublishAsync(dto.ScreenId, dto.PublishId);
            return Success(result, "下架成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "下架大屏失败: ScreenId={ScreenId}, PublishId={PublishId}", dto.ScreenId, dto.PublishId);
            return Error<int>("下架大屏失败");
        }
    }

    /// <summary>
    /// 重新上架大屏
    /// </summary>
    /// <param name="dto">重新上架请求参数，包含发布ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">重新上架成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 将已下架的大屏重新上架，恢复其公开访问链接。
    /// </remarks>
    /// <example>
    /// POST /api/screen/republish
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "publishId": "00000000-0000-0000-0000-000000000001"
    /// }
    /// </example>
    [HttpPost("republish")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Republish([FromBody] RepublishScreenDto dto)
    {
        try
        {
            var result = await _screenService.RepublishAsync(dto.PublishId);
            return Success(result, "重新上架成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重新上架大屏失败: {PublishId}", dto.PublishId);
            return Error<int>("重新上架大屏失败");
        }
    }

    /// <summary>
    /// 获取已发布的大屏配置
    /// </summary>
    /// <param name="publishId">发布ID</param>
    /// <returns>已发布大屏的完整配置</returns>
    /// <response code="200">成功获取发布配置</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">发布记录不存在</response>
    /// <remarks>
    /// 根据发布ID获取已发布大屏的配置快照。
    /// 返回的是发布时保存的配置，而非当前最新配置。
    /// 返回格式与前端 TypeScript 类型完全兼容（camelCase 字段名、嵌套对象结构）。
    /// </remarks>
    /// <example>
    /// GET /api/screen/published/00000000-0000-0000-0000-000000000001
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("published/{publishId}")]
    [ProducesResponseType(typeof(ApiResponse<ScreenFrontendDto>), 200)]
    public async Task<ApiResponse<ScreenFrontendDto>> GetPublished(Guid publishId)
    {
        try
        {
            var result = await _screenService.GetPublishedAsync(publishId);
            if (result == null)
            {
                return Error<ScreenFrontendDto>("发布记录不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取发布大屏配置失败: {PublishId}", publishId);
            return Error<ScreenFrontendDto>("获取发布大屏配置失败");
        }
    }

    /// <summary>
    /// 获取大屏发布状态信息
    /// </summary>
    /// <param name="id">大屏ID</param>
    /// <returns>发布状态信息</returns>
    /// <response code="200">成功获取发布状态</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取大屏的发布状态，包括是否已发布、发布URL、发布时间等信息。
    /// </remarks>
    /// <example>
    /// GET /api/screen/publish-info/00000000-0000-0000-0000-000000000001
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("publish-info/{id}")]
    [ProducesResponseType(typeof(ApiResponse<PublishInfoDto>), 200)]
    public async Task<ApiResponse<PublishInfoDto>> GetPublishInfo(Guid id)
    {
        try
        {
            var result = await _screenService.GetPublishInfoAsync(id);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取大屏发布状态失败: {Id}", id);
            return Error<PublishInfoDto>("获取发布状态失败");
        }
    }

    /// <summary>
    /// 获取发布记录列表
    /// </summary>
    /// <param name="query">查询参数，包含分页和筛选条件</param>
    /// <returns>分页发布记录列表</returns>
    /// <response code="200">成功获取发布记录列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取所有大屏的发布记录，包括发布时间、发布状态、浏览次数等信息。
    /// </remarks>
    /// <example>
    /// POST /api/screen/publish-list
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// { "pageIndex": 1, "pageSize": 10 }
    /// </example>
    [HttpPost("publish-list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<ScreenPublishDto>>), 200)]
    public async Task<ApiResponse<PageResponse<ScreenPublishDto>>> GetPublishList(
        [FromBody] QueryScreenDto query)
    {
        try
        {
            var result = await _screenService.GetPublishListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取发布记录列表失败");
            return Error<PageResponse<ScreenPublishDto>>("获取发布记录列表失败");
        }
    }

    #endregion

    #region 数据源（Mock模式）

    /// <summary>
    /// 获取数据源列表
    /// </summary>
    /// <returns>可用的数据源选项列表</returns>
    /// <response code="200">成功获取数据源列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取系统中配置的所有数据源，用于组件数据绑定。
    /// </remarks>
    /// <example>
    /// POST /api/screen/datasources
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("datasources")]
    [ProducesResponseType(typeof(ApiResponse<List<DatasourceOptionDto>>), 200)]
    public async Task<ApiResponse<List<DatasourceOptionDto>>> GetDatasources()
    {
        try
        {
            var result = await _screenService.GetDatasourcesAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取数据源列表失败");
            return Error<List<DatasourceOptionDto>>("获取数据源列表失败");
        }
    }

    /// <summary>
    /// 执行SQL查询
    /// </summary>
    /// <param name="dto">执行SQL请求参数，包含数据源ID和SQL语句</param>
    /// <returns>SQL查询结果，包含数据和列信息</returns>
    /// <response code="200">成功执行SQL查询</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 在指定数据源上执行SQL查询，返回查询结果。
    /// 支持参数化查询，参数以JSON格式传递。
    /// </remarks>
    /// <example>
    /// POST /api/screen/execute-sql
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "datasourceId": "00000000-0000-0000-0000-000000000001",
    ///     "sql": "SELECT * FROM users WHERE status = @status",
    ///     "params": "{\"status\":1}"
    /// }
    /// </example>
    [HttpPost("execute-sql")]
    [ProducesResponseType(typeof(ApiResponse<SqlResultDto>), 200)]
    public async Task<ApiResponse<SqlResultDto>> ExecuteSql([FromBody] ExecuteSqlDto dto)
    {
        try
        {
            var result = await _screenService.ExecuteSqlAsync(dto);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行SQL失败: DatasourceId={DatasourceId}, Sql={Sql}", dto.DatasourceId, dto.Sql);
            return Error<SqlResultDto>("执行SQL失败");
        }
    }

    /// <summary>
    /// 验证SQL语法
    /// </summary>
    /// <param name="dto">验证SQL请求参数，包含数据源ID和SQL语句</param>
    /// <returns>验证结果，包含是否有效和错误信息</returns>
    /// <response code="200">验证完成</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 验证SQL语句语法是否正确，不实际执行查询。
    /// 验证成功时返回预期的列信息。
    /// </remarks>
    /// <example>
    /// POST /api/screen/validate-sql
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "datasourceId": "00000000-0000-0000-0000-000000000001",
    ///     "sql": "SELECT name, age FROM users WHERE status = 1"
    /// }
    /// </example>
    [HttpPost("validate-sql")]
    [ProducesResponseType(typeof(ApiResponse<SqlValidateDto>), 200)]
    public async Task<ApiResponse<SqlValidateDto>> ValidateSql([FromBody] ValidateSqlDto dto)
    {
        try
        {
            var result = await _screenService.ValidateSqlAsync(dto);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证SQL失败: DatasourceId={DatasourceId}, Sql={Sql}", dto.DatasourceId, dto.Sql);
            return Error<SqlValidateDto>("验证SQL失败");
        }
    }

    #endregion

    #region 配置验证

    /// <summary>
    /// 验证大屏配置
    /// </summary>
    /// <param name="dto">验证配置请求参数，包含待验证的组件列表</param>
    /// <returns>验证结果，包含是否有效和错误列表</returns>
    /// <response code="200">验证完成</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 验证大屏整体配置是否有效，包括所有组件的配置验证。
    /// </remarks>
    /// <example>
    /// POST /api/screen/validate-config
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "components": [
    ///         {"component": "{\"type\":\"chart-line\",\"dataSource\":{},\"config\":{}}"}
    ///     ]
    /// }
    /// </example>
    [HttpPost("validate-config")]
    [ProducesResponseType(typeof(ApiResponse<ValidateResultDto>), 200)]
    public async Task<ApiResponse<ValidateResultDto>> ValidateConfig([FromBody] ValidateConfigDto dto)
    {
        try
        {
            // 将组件列表转换为JSON字符串进行验证
            var componentsJson = dto.Components != null
                ? System.Text.Json.JsonSerializer.Serialize(dto.Components)
                : "[]";
            var result = await _screenService.ValidateConfigAsync(componentsJson);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证大屏配置失败");
            return Error<ValidateResultDto>("验证大屏配置失败");
        }
    }

    /// <summary>
    /// 验证单个组件配置
    /// </summary>
    /// <param name="dto">验证组件请求参数，包含组件配置JSON</param>
    /// <returns>验证结果，包含是否有效和错误列表</returns>
    /// <response code="200">验证完成</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 验证单个组件的配置是否有效，包括位置、大小、数据源等配置。
    /// </remarks>
    /// <example>
    /// POST /api/screen/validate-component
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "component": "{\"type\":\"chart-line\",\"width\":400,\"height\":300}"
    /// }
    /// </example>
    [HttpPost("validate-component")]
    [ProducesResponseType(typeof(ApiResponse<ValidateResultDto>), 200)]
    public async Task<ApiResponse<ValidateResultDto>> ValidateComponent([FromBody] ValidateComponentDto dto)
    {
        try
        {
            var componentJson = dto.Component ?? "{}";
            var componentObj = System.Text.Json.JsonSerializer.Deserialize<object>(componentJson);
            var result = await _screenService.ValidateComponentAsync(componentObj ?? new {});
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "验证组件配置失败");
            return Error<ValidateResultDto>("验证组件配置失败");
        }
    }

    #endregion

    #region 组件管理

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <param name="dto">创建组件请求参数，包含大屏ID、组件类型和配置</param>
    /// <returns>新创建组件的ID</returns>
    /// <response code="200">添加成功，返回新组件ID</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 在指定大屏上添加一个新组件。
    /// 组件类型包括 chart-line, chart-bar, chart-pie, text, image, video, map 等。
    /// </remarks>
    /// <example>
    /// POST /api/screen/component/add
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "screenId": "00000000-0000-0000-0000-000000000001",
    ///     "componentType": "chart-line",
    ///     "positionX": 100,
    ///     "positionY": 200,
    ///     "width": 400,
    ///     "height": 300,
    ///     "config": "{\"title\":\"销售趋势\"}"
    /// }
    /// </example>
    [HttpPost("component/add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> AddComponent([FromBody] CreateComponentDto dto)
    {
        try
        {
            var id = await _screenService.AddComponentAsync(dto);
            return Success(id, "添加组件成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加组件失败: ScreenId={ScreenId}, ComponentType={ComponentType}", dto.ScreenId, dto.ComponentType);
            return Error<Guid>("添加组件失败");
        }
    }

    /// <summary>
    /// 更新组件
    /// </summary>
    /// <param name="dto">更新组件请求参数，包含组件ID和需要更新的字段</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 更新组件属性，包括位置、大小、配置、数据源等。
    /// 只更新请求中提供的字段。
    /// </remarks>
    /// <example>
    /// POST /api/screen/component/update
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "id": "00000000-0000-0000-0000-000000000001",
    ///     "positionX": 150,
    ///     "positionY": 250,
    ///     "width": 450
    /// }
    /// </example>
    [HttpPost("component/update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateComponent([FromBody] UpdateComponentDto dto)
    {
        try
        {
            var result = await _screenService.UpdateComponentAsync(dto);
            return Success(result, "更新组件成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新组件失败: {Id}", dto.Id);
            return Error<int>("更新组件失败");
        }
    }

    /// <summary>
    /// 删除组件
    /// </summary>
    /// <param name="id">要删除的组件ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 从大屏上删除指定组件。
    /// </remarks>
    /// <example>
    /// POST /api/screen/component/delete/00000000-0000-0000-0000-000000000001
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("component/delete/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteComponent(Guid id)
    {
        try
        {
            var result = await _screenService.DeleteComponentAsync(id);
            return Success(result, "删除组件成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除组件失败: {Id}", id);
            return Error<int>("删除组件失败");
        }
    }

    #endregion
}