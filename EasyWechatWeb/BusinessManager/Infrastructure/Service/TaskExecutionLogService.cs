namespace BusinessManager.Infrastructure.Service;

using BusinessManager.Infrastructure.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

/// <summary>
/// 任务执行日志服务实现
/// </summary>
public class TaskExecutionLogService : ITaskExecutionLogService
{
    public ISqlSugarClient _db { get; set; } = null!;
    public ILogger<TaskExecutionLogService> _logger { get; set; } = null!;

    public async Task<PageResponse<TaskExecutionLogDto>> GetPageListAsync(QueryTaskExecutionLogDto query)
    {
        var queryable = _db.Queryable<TaskExecutionLog>()
            .WhereIF(!string.IsNullOrEmpty(query.JobName), x => x.JobName.Contains(query.JobName!))
            .WhereIF(query.Status.HasValue, x => x.Status == query.Status!.Value)
            .WhereIF(query.TriggerType.HasValue, x => x.TriggerType == query.TriggerType!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.StartTime), x => x.StartTime >= DateTime.Parse(query.StartTime!))
            .WhereIF(!string.IsNullOrEmpty(query.EndTime), x => x.StartTime <= DateTime.Parse(query.EndTime!))
            .OrderByDescending(x => x.StartTime);

        var total = await queryable.CountAsync();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize);

        var result = list.Adapt<List<TaskExecutionLogDto>>();
        foreach (var item in result)
        {
            item.StatusText = GetStatusText(item.Status);
        }

        return new PageResponse<TaskExecutionLogDto>
        {
            List = result,
            Total = total,
            PageIndex = query.PageIndex,
            PageSize = query.PageSize
        };
    }

    public async Task<TaskExecutionLogDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<TaskExecutionLog>()
            .Where(x => x.Id == id)
            .FirstAsync();

        if (entity == null) return null;

        var dto = entity.Adapt<TaskExecutionLogDto>();
        dto.StatusText = GetStatusText(dto.Status);
        return dto;
    }

    public async Task<int> ClearAsync(int retentionDays)
    {
        var cutoffDate = DateTime.Now.AddDays(-retentionDays);
        return await _db.Deleteable<TaskExecutionLog>()
            .Where(x => x.CreateTime < cutoffDate)
            .ExecuteCommandAsync();
    }

    public async Task<TaskLogTrendDto> GetTrendAsync(int days = 7)
    {
        var startDate = DateTime.Now.AddDays(-days).Date;
        var endDate = DateTime.Now.Date;

        var logs = await _db.Queryable<TaskExecutionLog>()
            .Where(x => x.StartTime >= startDate && x.StartTime <= endDate)
            .ToListAsync();

        var points = new List<TaskLogTrendPointDto>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dayLogs = logs.Where(x => x.StartTime.Date == date).ToList();
            var executeCount = dayLogs.Count;
            var successCount = dayLogs.Count(x => x.Status == 1);
            var successRate = executeCount > 0 ? (double)successCount / executeCount * 100 : 0;

            points.Add(new TaskLogTrendPointDto
            {
                Date = date.ToString("yyyy-MM-dd"),
                ExecuteCount = executeCount,
                SuccessCount = successCount,
                SuccessRate = Math.Round(successRate, 2)
            });
        }

        return new TaskLogTrendDto { Points = points };
    }

    public async Task<TaskStatisticsDto> GetTodayStatisticsAsync()
    {
        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);

        var todayLogs = await _db.Queryable<TaskExecutionLog>()
            .Where(x => x.StartTime >= today && x.StartTime < tomorrow)
            .ToListAsync();

        return new TaskStatisticsDto
        {
            TotalCount = 0,
            EnabledCount = 0,
            PausedCount = 0,
            TodayExecuted = todayLogs.Count,
            TodaySuccess = todayLogs.Count(x => x.Status == 1),
            TodayFailure = todayLogs.Count(x => x.Status == 2)
        };
    }

    private string GetStatusText(int status)
    {
        return status switch
        {
            0 => "执行中",
            1 => "成功",
            2 => "失败",
            3 => "取消",
            _ => "未知"
        };
    }

    /// <summary>
    /// 记录执行日志
    /// </summary>
    public async Task<Guid> LogExecutionAsync(TaskExecutionLog log)
    {
        log.CreateTime = DateTime.Now;
        await _db.Insertable(log).ExecuteCommandAsync();
        return log.Id;
    }

    /// <summary>
    /// 更新执行结果
    /// </summary>
    public async Task UpdateExecutionResultAsync(Guid logId, int status, string? resultMessage = null, string? exceptionMessage = null, string? exceptionStackTrace = null)
    {
        var endTime = DateTime.Now;
        var log = await _db.Queryable<TaskExecutionLog>().Where(x => x.Id == logId).FirstAsync();
        if (log != null)
        {
            var duration = (long)(endTime - log.StartTime).TotalMilliseconds;
            await _db.Updateable<TaskExecutionLog>()
                .SetColumns(x => x.Status == status)
                .SetColumns(x => x.EndTime == endTime)
                .SetColumns(x => x.Duration == duration)
                .SetColumnsIF(resultMessage != null, x => x.ResultMessage == resultMessage!)
                .SetColumnsIF(exceptionMessage != null, x => x.ExceptionMessage == exceptionMessage!)
                .SetColumnsIF(exceptionStackTrace != null, x => x.ExceptionStackTrace == exceptionStackTrace!)
                .Where(x => x.Id == logId)
                .ExecuteCommandAsync();
        }
    }
}