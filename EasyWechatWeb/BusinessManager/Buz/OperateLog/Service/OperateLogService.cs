using BusinessManager.Buz.IService;
using BusinessManager.Buz.Service;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 操作日志服务实现类
/// </summary>
/// <remarks>
/// 实现操作日志相关的业务逻辑，包括日志记录、查询、删除和清理等功能。
/// 继承自<see cref="BaseService{OperateLog}"/>，使用SqlSugar进行数据库操作。
/// 支持按多种条件查询日志，便于系统审计和问题排查。
///
/// 分表说明：
/// - 使用 SqlSugar SplitTable 按天分表
/// - 表名格式：OperateLog_20260407
/// - 查询时自动跨分表查询
///
/// ES 存储说明：
/// - 支持同时写入 MySQL 和 Elasticsearch
/// - ES 写入可配置为异步（不阻塞主流程）
/// - 索引格式：operate-log-{yyyy.MM.dd}
/// </remarks>
public class OperateLogService : BaseService<OperateLog>, IOperateLogService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<OperateLogService> _logger { get; set; } = null!;

    /// <summary>
    /// Elasticsearch 索引服务
    /// </summary>
    public OperateLogElasticsearchService _esService { get; set; } = null!;

    /// <summary>
    /// 获取操作日志分页列表
    /// </summary>
    /// <param name="query">查询条件参数，包含分页信息和筛选条件</param>
    /// <returns>分页后的操作日志列表</returns>
    /// <remarks>
    /// 分表查询说明：
    /// - 使用 SplitTable() 自动查询所有分表
    /// - 通过时间条件过滤数据
    /// - 如果未指定时间范围，默认查询近30天的数据
    /// </remarks>
    public async Task<PageResponse<OperateLogDto>> GetPageListAsync(QueryOperateLogDto query)
    {
        _logger.LogInformation("开始查询操作日志列表，条件：UserId={UserId}, Module={Module}, Status={Status}",
            query.UserId, query.Module, query.Status);

        var total = new RefAsync<int>();

        // 确定时间范围（用于过滤）
        var startTime = query.StartTime ?? DateTime.Now.AddDays(-30);
        var endTime = query.EndTime ?? DateTime.Now;

        var list = await _db.Queryable<OperateLog>()
            .SplitTable()
            .WhereIF(query.UserId.HasValue, log => log.UserId == query.UserId!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.UserName), log => log.UserName!.Contains(query.UserName!))
            .WhereIF(!string.IsNullOrEmpty(query.Module), log => log.Module == query.Module)
            .WhereIF(!string.IsNullOrEmpty(query.Action), log => log.Action!.Contains(query.Action!))
            .WhereIF(query.Status.HasValue, log => log.Status == query.Status!.Value)
            .WhereIF(query.StartTime.HasValue, log => log.CreateTime >= query.StartTime!.Value)
            .WhereIF(query.EndTime.HasValue, log => log.CreateTime <= query.EndTime!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.Ip), log => log.Ip!.Contains(query.Ip!))
            .Where(log => log.CreateTime >= startTime && log.CreateTime <= endTime)  // 时间范围过滤
            .OrderByDescending(log => log.CreateTime)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        var dtoList = list.Adapt<List<OperateLogDto>>();

        _logger.LogInformation("查询操作日志列表完成，总数：{Total}", total.Value);

        return PageResponse<OperateLogDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 根据ID获取操作日志详情
    /// </summary>
    /// <param name="id">操作日志ID</param>
    /// <returns>操作日志详细信息；日志不存在返回null</returns>
    /// <remarks>
    /// 分表查询说明：
    /// 由于不知道日志在哪张分表，查询近7天的分表。
    /// </remarks>
    public new async Task<OperateLogDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("开始查询操作日志详情，ID：{Id}", id);

        // 查询近7天的分表
        var startTime = DateTime.Now.AddDays(-7);
        var endTime = DateTime.Now;

        var log = await _db.Queryable<OperateLog>()
            .SplitTable()
            .Where(l => l.Id == id && l.CreateTime >= startTime && l.CreateTime <= endTime)
            .FirstAsync();

        if (log == null)
        {
            _logger.LogWarning("操作日志不存在，ID：{Id}", id);
            return null;
        }

        var dto = log.Adapt<OperateLogDto>();
        _logger.LogInformation("查询操作日志详情完成，ID：{Id}", id);

        return dto;
    }

    /// <summary>
    /// 添加操作日志记录
    /// </summary>
    /// <param name="dto">添加操作日志参数，包含操作信息、请求参数、结果等</param>
    /// <returns>新创建的操作日志ID</returns>
    /// <remarks>
    /// 分表规则：根据 CreateTime 字段自动路由到对应日期的分表（如 OperateLog_20260407）
    ///
    /// ES 存储：
    /// - 若 ES 服务启用且配置为异步写入，则 Fire-and-Forget 方式写入 ES
    /// - 若 ES 服务启用且配置为同步写入，则等待 ES 写入完成
    /// </remarks>
    public async Task<Guid> AddAsync(AddOperateLogDto dto)
    {
        _logger.LogInformation("开始添加操作日志，用户：{UserName}, 操作：{Module}-{Action}",
            dto.UserName, dto.Module, dto.Action);

        var log = dto.Adapt<OperateLog>();
        log.CreateTime = DateTime.Now;

        var id = log.Id;

        // 写入 MySQL（使用 SplitTable 自动根据 CreateTime 路由到对应分表）
        await _db.Insertable(log).SplitTable().ExecuteCommandAsync();

        // 同时写入 Elasticsearch（异步，不阻塞主流程）
        if (_esService != null && _esService.IsEnabled)
        {
            _esService.IndexAsyncFireAndForget(dto, id);
        }

        _logger.LogInformation("添加操作日志完成，ID：{Id}", id);

        return id;
    }

    /// <summary>
    /// 删除指定操作日志
    /// </summary>
    /// <param name="id">要删除的操作日志ID</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 当日志不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 分表删除说明：
    /// 查询近7天的分表查找并删除。
    /// </remarks>
    public new async Task<int> DeleteAsync(Guid id)
    {
        _logger.LogInformation("开始删除操作日志，ID：{Id}", id);

        var startTime = DateTime.Now.AddDays(-7);
        var endTime = DateTime.Now;

        // 先查找日志
        var log = await _db.Queryable<OperateLog>()
            .SplitTable()
            .Where(l => l.Id == id && l.CreateTime >= startTime && l.CreateTime <= endTime)
            .FirstAsync();

        if (log == null)
        {
            _logger.LogWarning("操作日志不存在，无法删除，ID：{Id}", id);
            throw BusinessException.NotFound("操作日志不存在");
        }

        // 删除日志（根据日志的创建时间确定分表）
        var result = await _db.Deleteable<OperateLog>()
            .Where(l => l.Id == id)
            .SplitTable()
            .ExecuteCommandAsync();

        _logger.LogInformation("删除操作日志完成，ID：{Id}, 影响行数：{Result}", id, result);

        return result;
    }

    /// <summary>
    /// 清理过期的操作日志
    /// </summary>
    /// <param name="retentionDays">日志保留天数，超过此天数的日志将被删除</param>
    /// <returns>删除的日志数量</returns>
    /// <remarks>
    /// 清理超过指定天数的分表中的日志。
    /// </remarks>
    public async Task<int> ClearAsync(int retentionDays)
    {
        _logger.LogInformation("开始清理过期操作日志，保留天数：{RetentionDays}", retentionDays);

        if (retentionDays < 0)
        {
            _logger.LogWarning("保留天数不能为负数：{RetentionDays}", retentionDays);
            throw BusinessException.BadRequest("保留天数不能为负数");
        }

        var cutoffTime = DateTime.Now.AddDays(-retentionDays);

        // 删除早于截止时间的日志
        var deletedCount = await _db.Deleteable<OperateLog>()
            .Where(log => log.CreateTime < cutoffTime)
            .SplitTable()
            .ExecuteCommandAsync();

        _logger.LogInformation("清理过期操作日志完成，删除数量：{DeletedCount}, 截止时间：{CutoffTime}",
            deletedCount, cutoffTime);

        return deletedCount;
    }

    /// <summary>
    /// 按月份查询操作日志（跨分表查询）
    /// </summary>
    /// <param name="year">年份</param>
    /// <param name="month">月份（1-12）</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>分页后的操作日志列表</returns>
    /// <remarks>
    /// 查询指定月份所有分表的数据。
    /// 例如查询2026年4月的数据，会查询该月所有分表。
    /// </remarks>
    public async Task<PageResponse<OperateLogDto>> GetByMonthAsync(int year, int month, int pageIndex = 1, int pageSize = 20)
    {
        _logger.LogInformation("开始按月份查询操作日志，年份：{Year}, 月份：{Month}", year, month);

        // 验证月份有效性
        if (month < 1 || month > 12)
        {
            throw BusinessException.BadRequest("月份必须在1-12之间");
        }

        // 计算该月的第一天和最后一天
        var firstDay = new DateTime(year, month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var total = new RefAsync<int>();
        var list = await _db.Queryable<OperateLog>()
            .SplitTable()
            .Where(log => log.CreateTime >= firstDay && log.CreateTime <= lastDay.AddDays(1).AddSeconds(-1))  // 该月时间范围
            .OrderByDescending(log => log.CreateTime)
            .ToPageListAsync(pageIndex, pageSize, total);

        var dtoList = list.Adapt<List<OperateLogDto>>();

        _logger.LogInformation("按月份查询操作日志完成，年份：{Year}, 月份：{Month}, 总数：{Total}",
            year, month, total.Value);

        return PageResponse<OperateLogDto>.Create(dtoList, total.Value, pageIndex, pageSize);
    }
}