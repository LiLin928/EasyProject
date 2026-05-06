using BusinessManager.Etl.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Etl.Service;

/// <summary>
/// ETL调度任务服务实现
/// </summary>
public class EtlScheduleService : BaseService, IEtlScheduleService
{
    public ILogger<EtlScheduleService> _logger { get; set; } = null!;

    public async Task<PageResponse<EtlScheduleDto>> GetPageListAsync(QueryEtlScheduleDto query)
    {
        var total = new RefAsync<int>();
        var list = await _db.Queryable<EtlSchedule>()
            .WhereIF(!string.IsNullOrEmpty(query.Name), s => s.Name.Contains(query.Name!))
            .WhereIF(query.PipelineId.HasValue, s => s.PipelineId == query.PipelineId!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.Status), s => s.Status == query.Status)
            .OrderByDescending(s => s.CreateTime)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        var dtos = list.Adapt<List<EtlScheduleDto>>();
        foreach (var dto in dtos)
        {
            var entity = list.FirstOrDefault(x => x.Id == dto.Id);
            if (entity != null)
            {
                dto.CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                dto.UpdateTime = entity.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss");
                dto.LastExecuteTime = entity.LastExecuteTime?.ToString("yyyy-MM-dd HH:mm:ss");

                // 计算下次执行时间
                if (entity.Enabled && !string.IsNullOrEmpty(entity.CronExpression))
                {
                    dto.NextExecuteTime = CalculateNextExecuteTime(entity.CronExpression, entity.LastExecuteTime);
                }
                else if (entity.NextExecuteTime.HasValue)
                {
                    dto.NextExecuteTime = entity.NextExecuteTime?.ToString("yyyy-MM-dd HH:mm:ss");
                }

                // 解析 cron 描述
                dto.CronDescription = GetCronDescription(entity.CronExpression, entity.ScheduleType, entity.IntervalSeconds);
            }
        }

        return PageResponse<EtlScheduleDto>.Create(dtos, total.Value, query.PageIndex, query.PageSize);
    }

    public async Task<EtlScheduleDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<EtlSchedule>()
            .FirstAsync(s => s.Id == id);

        if (entity == null) return null;

        var dto = entity.Adapt<EtlScheduleDto>();
        dto.CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
        dto.UpdateTime = entity.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss");
        dto.LastExecuteTime = entity.LastExecuteTime?.ToString("yyyy-MM-dd HH:mm:ss");

        // 计算下次执行时间
        if (entity.Enabled && !string.IsNullOrEmpty(entity.CronExpression))
        {
            dto.NextExecuteTime = CalculateNextExecuteTime(entity.CronExpression, entity.LastExecuteTime);
        }
        else if (entity.NextExecuteTime.HasValue)
        {
            dto.NextExecuteTime = entity.NextExecuteTime?.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // 解析 cron 描述
        dto.CronDescription = GetCronDescription(entity.CronExpression, entity.ScheduleType, entity.IntervalSeconds);

        return dto;
    }

    public async Task<Guid> AddAsync(CreateEtlScheduleDto dto, Guid creatorId, string? creatorName)
    {
        // 检查任务流是否存在
        var pipeline = await _db.Queryable<EtlPipeline>()
            .FirstAsync(p => p.Id == dto.PipelineId);
        if (pipeline == null)
        {
            throw BusinessException.BadRequest("关联的任务流不存在");
        }

        var entity = new EtlSchedule
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            PipelineId = dto.PipelineId,
            PipelineName = pipeline.Name,
            ScheduleType = dto.ScheduleType,
            CronExpression = dto.CronExpression,
            IntervalSeconds = dto.IntervalSeconds,
            ExecuteParams = dto.ExecuteParams,
            Enabled = false,
            Status = "paused",
            CreatorId = creatorId,
            CreatorName = creatorName,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    public async Task<int> UpdateAsync(UpdateEtlScheduleDto dto)
    {
        var entity = await _db.Queryable<EtlSchedule>()
            .FirstAsync(s => s.Id == dto.Id);
        if (entity == null)
        {
            throw BusinessException.NotFound("调度任务不存在");
        }

        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.ScheduleType != null) entity.ScheduleType = dto.ScheduleType;
        if (dto.CronExpression != null) entity.CronExpression = dto.CronExpression;
        if (dto.IntervalSeconds.HasValue) entity.IntervalSeconds = dto.IntervalSeconds.Value;
        if (dto.ExecuteParams != null) entity.ExecuteParams = dto.ExecuteParams;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task<int> DeleteAsync(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0) return 0;

        return await _db.Deleteable<EtlSchedule>()
            .Where(s => ids.Contains(s.Id))
            .ExecuteCommandAsync();
    }

    public async Task<int> EnableAsync(Guid id)
    {
        var entity = await _db.Queryable<EtlSchedule>()
            .FirstAsync(s => s.Id == id);
        if (entity == null)
        {
            throw BusinessException.NotFound("调度任务不存在");
        }

        // 检查任务流是否已发布
        var pipeline = await _db.Queryable<EtlPipeline>()
            .FirstAsync(p => p.Id == entity.PipelineId);
        if (pipeline == null || pipeline.Status != "published")
        {
            throw BusinessException.BadRequest("关联的任务流未发布");
        }

        entity.Enabled = true;
        entity.Status = "active";
        entity.UpdateTime = DateTime.Now;

        // 计算下次执行时间
        if (!string.IsNullOrEmpty(entity.CronExpression))
        {
            entity.NextExecuteTime = CalculateNextExecuteTimeDateTime(entity.CronExpression);
        }

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task<int> DisableAsync(Guid id)
    {
        var entity = await _db.Queryable<EtlSchedule>()
            .FirstAsync(s => s.Id == id);
        if (entity == null)
        {
            throw BusinessException.NotFound("调度任务不存在");
        }

        entity.Enabled = false;
        entity.Status = "paused";
        entity.NextExecuteTime = null;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task<Guid> ExecuteNowAsync(Guid id, Guid? triggerUserId, string? triggerUserName)
    {
        var schedule = await _db.Queryable<EtlSchedule>()
            .FirstAsync(s => s.Id == id);
        if (schedule == null)
        {
            throw BusinessException.NotFound("调度任务不存在");
        }

        var execution = new EtlExecution
        {
            Id = Guid.NewGuid(),
            PipelineId = schedule.PipelineId,
            PipelineName = schedule.PipelineName,
            ScheduleId = id,
            Status = "pending",
            ExecuteParams = schedule.ExecuteParams,
            TriggerType = "schedule",
            TriggerUserId = triggerUserId,
            TriggerUserName = triggerUserName,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(execution).ExecuteCommandAsync();

        // 更新调度统计
        schedule.ExecuteCount++;
        schedule.LastExecuteTime = DateTime.Now;

        // 计算下次执行时间
        if (schedule.Enabled && !string.IsNullOrEmpty(schedule.CronExpression))
        {
            schedule.NextExecuteTime = CalculateNextExecuteTimeDateTime(schedule.CronExpression);
        }

        schedule.UpdateTime = DateTime.Now;
        await _db.Updateable(schedule).ExecuteCommandAsync();

        return execution.Id;
    }

    public async Task<PageResponse<EtlExecutionDto>> GetExecutionHistoryAsync(Guid id, int pageIndex, int pageSize)
    {
        var total = new RefAsync<int>();
        var list = await _db.Queryable<EtlExecution>()
            .Where(e => e.ScheduleId == id)
            .OrderByDescending(e => e.CreateTime)
            .ToPageListAsync(pageIndex, pageSize, total);

        var dtos = list.Adapt<List<EtlExecutionDto>>();
        foreach (var dto in dtos)
        {
            dto.CreateTime = list.FirstOrDefault(x => x.Id == dto.Id)?.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            dto.StartTime = list.FirstOrDefault(x => x.Id == dto.Id)?.StartTime?.ToString("yyyy-MM-dd HH:mm:ss");
            dto.EndTime = list.FirstOrDefault(x => x.Id == dto.Id)?.EndTime?.ToString("yyyy-MM-dd HH:mm:ss");
        }

        return PageResponse<EtlExecutionDto>.Create(dtos, total.Value, pageIndex, pageSize);
    }

    public async Task<EtlScheduleStatisticsDto?> GetStatisticsAsync(Guid id)
    {
        var schedule = await _db.Queryable<EtlSchedule>()
            .FirstAsync(s => s.Id == id);
        if (schedule == null) return null;

        var executions = await _db.Queryable<EtlExecution>()
            .Where(e => e.ScheduleId == id && e.Status == "success")
            .ToListAsync();

        var avgDuration = executions.Count > 0 && executions.Any(e => e.Duration.HasValue)
            ? (long)executions.Where(e => e.Duration.HasValue).Average(e => e.Duration!.Value)
            : 0;

        return new EtlScheduleStatisticsDto
        {
            TotalExecutions = schedule.ExecuteCount,
            SuccessCount = schedule.SuccessCount,
            FailureCount = schedule.FailureCount,
            AvgDuration = avgDuration,
            LastExecutionTime = schedule.LastExecuteTime?.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 计算 cron 表达式的下次执行时间（字符串格式）
    /// </summary>
    private string? CalculateNextExecuteTime(string cronExpression, DateTime? lastExecuteTime)
    {
        var nextTime = CalculateNextExecuteTimeDateTime(cronExpression, lastExecuteTime);
        return nextTime?.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// 计算 cron 表达式的下次执行时间
    /// </summary>
    private DateTime? CalculateNextExecuteTimeDateTime(string cronExpression, DateTime? lastExecuteTime = null)
    {
        try
        {
            var parts = cronExpression.Split(' ');
            if (parts.Length != 5) return null;

            var now = lastExecuteTime ?? DateTime.Now;

            // 解析 cron 字段：秒 分 时 日 月 周
            // 标准格式：分 时 日 月 周
            var minute = ParseCronField(parts[0], 0, 59);
            var hour = ParseCronField(parts[1], 0, 23);
            var day = ParseCronField(parts[2], 1, 31);
            var month = ParseCronField(parts[3], 1, 12);
            // 周字段暂时不处理

            // 简单实现：找到下一个匹配的时间
            var next = now.AddMinutes(1);
            for (int i = 0; i < 366 * 24 * 60; i++) // 最多查找一年
            {
                if (IsValidTime(next, minute, hour, day, month))
                {
                    return next;
                }
                next = next.AddMinutes(1);
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 解析 cron 字段，返回允许的值列表
    /// </summary>
    private List<int> ParseCronField(string field, int min, int max)
    {
        var values = new List<int>();

        if (field == "*")
        {
            for (int i = min; i <= max; i++) values.Add(i);
            return values;
        }

        // 处理逗号分隔
        foreach (var part in field.Split(','))
        {
            if (part.Contains('/'))
            {
                // 步长格式：*/5 或 0/5
                var stepParts = part.Split('/');
                var start = stepParts[0] == "*" ? min : int.Parse(stepParts[0]);
                var step = int.Parse(stepParts[1]);
                for (int i = start; i <= max; i += step) values.Add(i);
            }
            else if (part.Contains('-'))
            {
                // 范围格式：1-5
                var rangeParts = part.Split('-');
                var rangeStart = int.Parse(rangeParts[0]);
                var rangeEnd = int.Parse(rangeParts[1]);
                for (int i = rangeStart; i <= rangeEnd; i++) values.Add(i);
            }
            else
            {
                values.Add(int.Parse(part));
            }
        }

        return values;
    }

    /// <summary>
    /// 检查时间是否匹配 cron 条件
    /// </summary>
    private bool IsValidTime(DateTime time, List<int> minutes, List<int> hours, List<int> days, List<int> months)
    {
        return minutes.Contains(time.Minute) &&
               hours.Contains(time.Hour) &&
               days.Contains(time.Day) &&
               months.Contains(time.Month);
    }

    /// <summary>
    /// 获取 cron 表达式的中文描述
    /// </summary>
    private string? GetCronDescription(string? cronExpression, string scheduleType, int? intervalSeconds)
    {
        if (scheduleType == "interval" && intervalSeconds.HasValue)
        {
            if (intervalSeconds.Value < 60)
                return $"每 {intervalSeconds.Value} 秒执行一次";
            else if (intervalSeconds.Value < 3600)
                return $"每 {intervalSeconds.Value / 60} 分钟执行一次";
            else if (intervalSeconds.Value < 86400)
                return $"每 {intervalSeconds.Value / 3600} 小时执行一次";
            else
                return $"每 {intervalSeconds.Value / 86400} 天执行一次";
        }

        if (string.IsNullOrEmpty(cronExpression)) return null;

        try
        {
            var parts = cronExpression.Split(' ');
            if (parts.Length != 5) return cronExpression;

            var minute = parts[0];
            var hour = parts[1];
            var day = parts[2];
            var month = parts[3];
            var weekday = parts[4];

            // 常见 cron 表达式的中文描述
            if (cronExpression == "0 * * * *") return "每小时执行一次";
            if (cronExpression == "0 0 * * *") return "每天 00:00 执行";
            if (cronExpression == "0 0 * * 1-5") return "工作日 00:00 执行";
            if (cronExpression == "0 0 1 * *") return "每月 1 日 00:00 执行";
            if (cronExpression == "0 0 1 1 *") return "每年 1 月 1 日 00:00 执行";
            if (cronExpression == "0 0 1 1,7 *") return "每年 1 月和 7 月 1 日 00:00 执行";
            if (cronExpression.StartsWith("0 ") && hour != "*" && day == "*" && month == "*" && weekday == "*")
            {
                var hourValue = int.Parse(hour);
                return $"每天 {hourValue}:00 执行";
            }
            if (cronExpression.StartsWith("*/") && minute.Contains('/') && hour == "*" && day == "*" && month == "*")
            {
                var minuteStep = int.Parse(minute.Split('/')[1]);
                return $"每 {minuteStep} 分钟执行一次";
            }
            if (cronExpression.StartsWith("0 */") && hour.Contains('/') && day == "*" && month == "*")
            {
                var hourStep = int.Parse(hour.Split('/')[1]);
                return $"每 {hourStep} 小时执行一次";
            }

            // 通用描述
            var desc = new List<string>();

            if (minute == "*") desc.Add("每分钟");
            else if (minute.StartsWith("*/")) desc.Add($"每 {minute.Split('/')[1]} 分钟");
            else desc.Add($"{minute} 分");

            if (hour == "*") desc.Add("每小时");
            else if (hour.StartsWith("*/")) desc.Add($"每 {hour.Split('/')[1]} 小时");
            else desc.Add($"{hour} 时");

            if (day == "*") desc.Add("每天");
            else desc.Add($"{day} 日");

            if (month == "*") desc.Add("每月");
            else desc.Add($"{month} 月");

            if (weekday != "*")
            {
                var weekdayNames = new[] { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
                if (weekday.Contains('-'))
                {
                    var range = weekday.Split('-');
                    desc.Add($"{weekdayNames[int.Parse(range[0])]}到{weekdayNames[int.Parse(range[1])]}");
                }
                else
                {
                    desc.Add(weekdayNames[int.Parse(weekday)]);
                }
            }

            return string.Join(" ", desc) + "执行";
        }
        catch
        {
            return cronExpression;
        }
    }
}