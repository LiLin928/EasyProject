using BusinessManager.Etl.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Text.Json;

namespace BusinessManager.Etl.Service;

/// <summary>
/// ETL执行监控服务实现
/// </summary>
public class EtlExecutionService : BaseService, IEtlExecutionService
{
    public ILogger<EtlExecutionService> _logger { get; set; } = null!;

    public async Task<PageResponse<EtlExecutionDto>> GetPageListAsync(QueryEtlExecutionDto query)
    {
        // 处理别名字段：优先使用 DateStart/DateEnd，如果没有则使用 StartTimeBegin/StartTimeEnd
        var startTimeBegin = query.DateStart ?? query.StartTimeBegin;
        var startTimeEnd = query.DateEnd ?? query.StartTimeEnd;

        var total = new RefAsync<int>();
        var list = await _db.Queryable<EtlExecution>()
            .WhereIF(query.PipelineId.HasValue, e => e.PipelineId == query.PipelineId!.Value)
            .WhereIF(query.ScheduleId.HasValue, e => e.ScheduleId == query.ScheduleId!.Value)
            .WhereIF(!string.IsNullOrEmpty(query.Status), e => e.Status == query.Status)
            .WhereIF(!string.IsNullOrEmpty(startTimeBegin), e => e.StartTime >= DateTime.Parse(startTimeBegin!))
            .WhereIF(!string.IsNullOrEmpty(startTimeEnd), e => e.StartTime <= DateTime.Parse(startTimeEnd!))
            .OrderByDescending(e => e.CreateTime)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        var dtos = list.Adapt<List<EtlExecutionDto>>();
        foreach (var dto in dtos)
        {
            dto.CreateTime = list.FirstOrDefault(x => x.Id == dto.Id)?.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            dto.StartTime = list.FirstOrDefault(x => x.Id == dto.Id)?.StartTime?.ToString("yyyy-MM-dd HH:mm:ss");
            dto.EndTime = list.FirstOrDefault(x => x.Id == dto.Id)?.EndTime?.ToString("yyyy-MM-dd HH:mm:ss");
        }

        return PageResponse<EtlExecutionDto>.Create(dtos, total.Value, query.PageIndex, query.PageSize);
    }

    public async Task<EtlExecutionDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<EtlExecution>()
            .FirstAsync(e => e.Id == id);

        if (entity == null) return null;

        var dto = entity.Adapt<EtlExecutionDto>();
        dto.CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
        dto.StartTime = entity.StartTime?.ToString("yyyy-MM-dd HH:mm:ss");
        dto.EndTime = entity.EndTime?.ToString("yyyy-MM-dd HH:mm:ss");
        return dto;
    }

    public async Task<EtlExecutionStatisticsDto> GetStatisticsAsync(string? dateStart, string? dateEnd)
    {
        var query = _db.Queryable<EtlExecution>();

        if (!string.IsNullOrEmpty(dateStart))
        {
            var start = DateTime.Parse(dateStart);
            query = query.Where(e => e.CreateTime >= start);
        }
        if (!string.IsNullOrEmpty(dateEnd))
        {
            var end = DateTime.Parse(dateEnd);
            query = query.Where(e => e.CreateTime <= end);
        }

        var total = await query.CountAsync();
        var success = await query.Where(e => e.Status == "success").CountAsync();
        var failure = await query.Where(e => e.Status == "failure").CountAsync();
        var running = await query.Where(e => e.Status == "running").CountAsync();
        var pending = await query.Where(e => e.Status == "pending").CountAsync();

        var successExecutions = await query
            .Where(e => e.Status == "success" && e.Duration.HasValue)
            .ToListAsync();

        var avgDuration = successExecutions.Count > 0
            ? (long)successExecutions.Average(e => e.Duration!.Value)
            : 0;

        var successRate = total > 0 ? (double)success / total * 100 : 0;

        return new EtlExecutionStatisticsDto
        {
            TotalCount = total,
            SuccessCount = success,
            FailureCount = failure,
            RunningCount = running,
            PendingCount = pending,
            AvgDuration = avgDuration,
            SuccessRate = successRate
        };
    }

    public async Task<int> CancelAsync(Guid id)
    {
        var entity = await _db.Queryable<EtlExecution>()
            .FirstAsync(e => e.Id == id);
        if (entity == null)
        {
            throw BusinessException.NotFound("执行记录不存在");
        }

        if (entity.Status != "running" && entity.Status != "pending")
        {
            throw BusinessException.BadRequest("只能取消运行中或待执行的任务");
        }

        entity.Status = "cancelled";
        entity.EndTime = DateTime.Now;
        entity.Duration = entity.StartTime.HasValue
            ? (long)(DateTime.Now - entity.StartTime.Value).TotalMilliseconds
            : 0;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task<Guid> RetryAsync(Guid id, Guid? triggerUserId, string? triggerUserName)
    {
        var original = await _db.Queryable<EtlExecution>()
            .FirstAsync(e => e.Id == id);
        if (original == null)
        {
            throw BusinessException.NotFound("原执行记录不存在");
        }

        var execution = new EtlExecution
        {
            Id = Guid.NewGuid(),
            PipelineId = original.PipelineId,
            PipelineName = original.PipelineName,
            ScheduleId = original.ScheduleId,
            Status = "pending",
            ExecuteParams = original.ExecuteParams,
            TriggerType = "manual",
            TriggerUserId = triggerUserId,
            TriggerUserName = triggerUserName,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(execution).ExecuteCommandAsync();
        return execution.Id;
    }

    public async Task<List<EtlExecutionLogDto>> GetLogsAsync(Guid id, string? nodeId, string? level)
    {
        var query = _db.Queryable<EtlExecutionLog>()
            .Where(e => e.ExecutionId == id)
            .WhereIF(!string.IsNullOrEmpty(nodeId), e => e.NodeId == nodeId)
            .WhereIF(!string.IsNullOrEmpty(level), e => e.Level == level!.ToLower())
            .OrderBy(e => e.LogTime);

        var list = await query.ToListAsync();

        return list.Select(e => new EtlExecutionLogDto
        {
            Time = e.LogTime.ToString("yyyy-MM-dd HH:mm:ss"),
            Level = e.Level,
            Message = e.Message,
            NodeId = e.NodeId,
            NodeName = e.NodeName
        }).ToList();
    }

    public async Task AddLogAsync(Guid executionId, string level, string message, string? nodeId = null, string? nodeName = null)
    {
        var log = new EtlExecutionLog
        {
            ExecutionId = executionId,
            Level = level.ToLower(),
            Message = message,
            NodeId = nodeId,
            NodeName = nodeName,
            LogTime = DateTime.Now
        };

        await _db.Insertable(log).ExecuteCommandAsync();
    }

    public async Task<EtlNodeExecutionDto?> GetNodeExecutionAsync(Guid executionId, string nodeId)
    {
        var entity = await _db.Queryable<EtlNodeExecution>()
            .Where(e => e.ExecutionId == executionId && e.NodeId == nodeId)
            .FirstAsync();

        if (entity == null) return null;

        return new EtlNodeExecutionDto
        {
            NodeId = entity.NodeId,
            NodeName = entity.NodeName ?? string.Empty,
            NodeType = entity.NodeType,
            NodeConfig = entity.NodeConfig,
            Status = entity.Status,
            StartTime = entity.StartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            EndTime = entity.EndTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            Duration = entity.Duration,
            Input = ParseJsonToObject(entity.InputData),
            Output = ParseJsonToObject(entity.OutputData),
            Error = entity.ErrorMessage
        };
    }

    public async Task<List<EtlNodeExecutionDto>> GetNodeExecutionsAsync(Guid executionId)
    {
        var list = await _db.Queryable<EtlNodeExecution>()
            .Where(e => e.ExecutionId == executionId)
            .OrderBy(e => e.StartTime)
            .ToListAsync();

        return list.Select(e => new EtlNodeExecutionDto
        {
            NodeId = e.NodeId,
            NodeName = e.NodeName ?? string.Empty,
            NodeType = e.NodeType,
            NodeConfig = e.NodeConfig,
            Status = e.Status,
            StartTime = e.StartTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            EndTime = e.EndTime?.ToString("yyyy-MM-dd HH:mm:ss"),
            Duration = e.Duration,
            Input = ParseJsonToObject(e.InputData),
            Output = ParseJsonToObject(e.OutputData),
            Error = e.ErrorMessage
        }).ToList();
    }

    public async Task<Guid> CreateNodeExecutionAsync(Guid executionId, string nodeId, string? nodeName, string? nodeType)
    {
        var nodeExecution = new EtlNodeExecution
        {
            ExecutionId = executionId,
            NodeId = nodeId,
            NodeName = nodeName,
            NodeType = nodeType,
            Status = "pending",
            CreateTime = DateTime.Now
        };

        await _db.Insertable(nodeExecution).ExecuteCommandAsync();
        return nodeExecution.Id;
    }

    public async Task UpdateNodeExecutionAsync(Guid nodeExecutionId, string status, object? input = null, object? output = null, string? error = null)
    {
        var entity = await _db.Queryable<EtlNodeExecution>()
            .FirstAsync(e => e.Id == nodeExecutionId);

        if (entity == null) return;

        entity.Status = status;
        entity.UpdateTime = DateTime.Now;

        if (status == "running" && entity.StartTime == null)
        {
            entity.StartTime = DateTime.Now;
        }

        if (status == "success" || status == "failure" || status == "skipped")
        {
            entity.EndTime = DateTime.Now;
            if (entity.StartTime.HasValue)
            {
                entity.Duration = (long)(DateTime.Now - entity.StartTime.Value).TotalMilliseconds;
            }
        }

        if (input != null)
        {
            entity.InputData = JsonSerializer.Serialize(input);
        }

        if (output != null)
        {
            entity.OutputData = JsonSerializer.Serialize(output);
        }

        if (error != null)
        {
            entity.ErrorMessage = error;
        }

        await _db.Updateable(entity).ExecuteCommandAsync();
    }

    private object? ParseJsonToObject(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try
        {
            return JsonSerializer.Deserialize<object>(json);
        }
        catch
        {
            return json;
        }
    }

    public async Task<List<EtlExecutionDto>> GetRunningExecutionsAsync()
    {
        var list = await _db.Queryable<EtlExecution>()
            .Where(e => e.Status == "running")
            .OrderByDescending(e => e.CreateTime)
            .ToListAsync();

        var dtos = list.Adapt<List<EtlExecutionDto>>();
        foreach (var dto in dtos)
        {
            dto.CreateTime = list.FirstOrDefault(x => x.Id == dto.Id)?.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            dto.StartTime = list.FirstOrDefault(x => x.Id == dto.Id)?.StartTime?.ToString("yyyy-MM-dd HH:mm:ss");
        }

        return dtos;
    }

    public async Task<EtlTodayStatisticsDto> GetTodayStatisticsAsync()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var total = await _db.Queryable<EtlExecution>()
            .Where(e => e.CreateTime >= today && e.CreateTime < tomorrow)
            .CountAsync();

        var running = await _db.Queryable<EtlExecution>()
            .Where(e => e.Status == "running")
            .CountAsync();

        var success = await _db.Queryable<EtlExecution>()
            .Where(e => e.Status == "success" && e.CreateTime >= today && e.CreateTime < tomorrow)
            .CountAsync();

        var failure = await _db.Queryable<EtlExecution>()
            .Where(e => e.Status == "failure" && e.CreateTime >= today && e.CreateTime < tomorrow)
            .CountAsync();

        var pending = await _db.Queryable<EtlExecution>()
            .Where(e => e.Status == "pending")
            .CountAsync();

        return new EtlTodayStatisticsDto
        {
            Total = total,
            Running = running,
            Success = success,
            Failure = failure,
            Pending = pending
        };
    }

    public async Task<EtlExecutionStatusDto?> GetStatusAsync(Guid id)
    {
        var entity = await _db.Queryable<EtlExecution>()
            .FirstAsync(e => e.Id == id);

        if (entity == null) return null;

        return new EtlExecutionStatusDto
        {
            Id = entity.Id.ToString(),
            Status = entity.Status,
            Progress = entity.Progress,
            CurrentNodeId = entity.CurrentNodeId,
            CurrentNodeName = entity.CurrentNodeName,
            CompletedNodes = entity.CompletedNodes,
            TotalNodes = entity.TotalNodes
        };
    }
}