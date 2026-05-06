using BusinessManager.Buz.IService;
using BusinessManager.Buz.Service;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 操作日志控制器
/// </summary>
/// <remarks>
/// 提供操作日志的管理功能，包括日志查询、详情查看、删除和清理等。
/// 用于系统操作审计，记录用户的所有操作行为。
/// 所有接口需要授权访问。
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OperateLogController : BaseController
{
    /// <summary>
    /// 操作日志服务接口
    /// </summary>
    public IOperateLogService _operateLogService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<OperateLogController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取操作日志列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页后的操作日志列表</returns>
    /// <response code="200">成功获取操作日志列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 支持多种筛选条件进行日志查询：
    /// 查询结果按操作时间倒序排列，最新的日志排在最前面。
    /// </remarks>
    /// <example>
    /// POST /api/operatelog/list
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "pageIndex": 1,
    ///     "pageSize": 10,
    ///     "userId": "3fa85f64-...",
    ///     "startTime": "2024-01-01",
    ///     "endTime": "2024-01-31"
    /// }
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<OperateLogDto>>), 200)]
    public async Task<ApiResponse<PageResponse<OperateLogDto>>> GetList([FromBody] QueryOperateLogDto query)
    {
        try
        {
            var result = await _operateLogService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取操作日志列表失败");
            return Error<PageResponse<OperateLogDto>>("获取操作日志列表失败");
        }
    }

    /// <summary>
    /// 获取操作日志详情
    /// </summary>
    /// <param name="id">操作日志ID</param>
    /// <returns>操作日志详细信息</returns>
    /// <response code="200">成功获取操作日志详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">操作日志不存在</response>
    /// <remarks>
    /// 根据日志ID获取完整的操作日志信息，包括：
    /// <list type="bullet">
    ///     <item>用户信息：用户ID、用户名</item>
    ///     <item>操作信息：模块、动作、请求方法、请求地址</item>
    ///     <item>请求详情：IP地址、操作地点、请求参数</item>
    ///     <item>执行结果：操作结果、状态、错误信息、执行时长</item>
    ///     <item>时间信息：操作时间</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// GET /api/operatelog/detail/1
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<OperateLogDto>), 200)]
    public async Task<ApiResponse<OperateLogDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _operateLogService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<OperateLogDto>("操作日志不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取操作日志详情失败: {Id}", id);
            return Error<OperateLogDto>("获取操作日志详情失败");
        }
    }

    /// <summary>
    /// 删除操作日志
    /// </summary>
    /// <param name="id">要删除的操作日志ID</param>
    /// <returns>删除影响的行数</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">操作日志不存在</response>
    /// <remarks>
    /// 物理删除指定的操作日志记录，数据将永久移除。
    /// 删除前会检查日志是否存在，不存在则返回404错误。
    /// 通常只有管理员才有权限删除日志。
    /// </remarks>
    /// <example>
    /// POST /api/operatelog/delete
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    /// </example>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _operateLogService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            _logger.LogWarning(ex, "删除操作日志失败: {Id}", id);
            if (ex.Message.Contains("不存在"))
            {
                return Error<int>("操作日志不存在", 404);
            }
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除操作日志失败: {Id}", id);
            return Error<int>("删除操作日志失败");
        }
    }

    /// <summary>
    /// 清理过期操作日志
    /// </summary>
    /// <param name="retentionDays">日志保留天数，超过此天数的日志将被删除，默认为30天</param>
    /// <returns>删除的日志数量</returns>
    /// <response code="200">清理成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 清理超过指定天数的操作日志记录：
    /// <list type="bullet">
    ///     <item>默认保留30天的日志</item>
    ///     <item>清理操作是物理删除，数据将永久移除</item>
    ///     <item>建议定期执行清理，避免数据量过大影响性能</item>
    ///     <item>通常只有管理员才有权限执行清理操作</item>
    /// </list>
    /// 清理流程：
    /// <list type="number">
    ///     <item>计算保留截止时间（当前时间 - retentionDays）</item>
    ///     <item>删除创建时间早于截止时间的所有日志</item>
    ///     <item>返回删除的记录数量</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// POST /api/operatelog/clear?retentionDays=30
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpPost("clear")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Clear([FromQuery] int retentionDays = 30)
    {
        try
        {
            var result = await _operateLogService.ClearAsync(retentionDays);
            return Success(result, $"清理完成，删除了 {result} 条日志");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            _logger.LogWarning(ex, "清理操作日志失败");
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清理操作日志失败");
            return Error<int>("清理操作日志失败");
        }
    }

    /// <summary>
    /// 按月份查询操作日志（跨分表查询）
    /// </summary>
    /// <param name="year">年份，如 2026</param>
    /// <param name="month">月份（1-12），如 4</param>
    /// <param name="pageIndex">页码，从1开始，默认为1</param>
    /// <param name="pageSize">每页数量，默认为20</param>
    /// <returns>分页后的操作日志列表</returns>
    /// <response code="200">成功获取操作日志列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">参数错误，月份必须在1-12之间</response>
    /// <remarks>
    /// 查询指定月份的所有操作日志，自动跨分表查询。
    ///
    /// 分表说明：
    /// - 系统按天自动创建分表，如 OperateLog_20260407
    /// - 此接口会查询指定月份的所有分表（如4月查询 OperateLog_20260401 到 OperateLog_20260430）
    /// - 适合按月统计、导出等场景
    ///
    /// 使用示例：
    /// <list type="bullet">
    ///     <item>查询2026年4月的日志：GET /api/operatelog/month?year=2026&amp;month=4</item>
    ///     <item>查询2026年1月的日志（分页）：GET /api/operatelog/month?year=2026&amp;month=1&amp;pageIndex=2&amp;pageSize=50</item>
    /// </list>
    /// </remarks>
    /// <example>
    /// GET /api/operatelog/month?year=2026&amp;month=4
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("month")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<OperateLogDto>>), 200)]
    public async Task<ApiResponse<PageResponse<OperateLogDto>>> GetByMonth(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var result = await _operateLogService.GetByMonthAsync(year, month, pageIndex, pageSize);
            return Success(result);
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            _logger.LogWarning(ex, "按月份查询操作日志失败: year={Year}, month={Month}", year, month);
            return Error<PageResponse<OperateLogDto>>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "按月份查询操作日志失败: year={Year}, month={Month}", year, month);
            return Error<PageResponse<OperateLogDto>>("按月份查询操作日志失败");
        }
    }
}