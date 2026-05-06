using BusinessManager.Etl.IService;
using BusinessManager.Buz.Etl.Engine;
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
/// ETL任务流服务实现
/// </summary>
public class EtlPipelineService : BaseService, IEtlPipelineService
{
    public ILogger<EtlPipelineService> _logger { get; set; } = null!;
    public IEtlExecutionEngine _executionEngine { get; set; } = null!;

    // 静态 Mapster 配置，忽略 Tags 字段的自动映射（因为需要手动处理 JSON 解析）
    private static readonly TypeAdapterConfig _pipelineConfig = new TypeAdapterConfig();

    static EtlPipelineService()
    {
        _pipelineConfig.ForType<EtlPipeline, EtlPipelineDto>()
            .Map(dest => dest.Tags, src => (List<string>?)null);
    }

    public async Task<PageResponse<EtlPipelineDto>> GetPageListAsync(QueryEtlPipelineDto query)
    {
        var total = new RefAsync<int>();
        var list = await _db.Queryable<EtlPipeline>()
            .WhereIF(!string.IsNullOrEmpty(query.Name), p => p.Name.Contains(query.Name!))
            .WhereIF(!string.IsNullOrEmpty(query.Status), p => p.Status == query.Status)
            .OrderByDescending(p => p.CreateTime)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        var dtos = list.Adapt<List<EtlPipelineDto>>(_pipelineConfig);
        foreach (var dto in dtos)
        {
            var entity = list.FirstOrDefault(x => x.Id == dto.Id);
            if (entity != null)
            {
                dto.CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                dto.UpdateTime = entity.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss");
                dto.PublishTime = entity.PublishTime?.ToString("yyyy-MM-dd HH:mm:ss");
                // 解析 Tags
                dto.Tags = ParseTags(entity.Tags);
            }
        }

        return PageResponse<EtlPipelineDto>.Create(dtos, total.Value, query.PageIndex, query.PageSize);
    }

    public async Task<EtlPipelineDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<EtlPipeline>()
            .FirstAsync(p => p.Id == id);

        if (entity == null) return null;

        var dto = entity.Adapt<EtlPipelineDto>(_pipelineConfig);
        dto.CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
        dto.UpdateTime = entity.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss");
        dto.PublishTime = entity.PublishTime?.ToString("yyyy-MM-dd HH:mm:ss");
        // 解析 Tags
        dto.Tags = ParseTags(entity.Tags);
        return dto;
    }

    private List<string>? ParseTags(string? tagsJson)
    {
        if (string.IsNullOrEmpty(tagsJson)) return null;
        try
        {
            return JsonSerializer.Deserialize<List<string>>(tagsJson);
        }
        catch
        {
            return null;
        }
    }

    private string? SerializeTags(List<string>? tags)
    {
        if (tags == null || tags.Count == 0) return null;
        return JsonSerializer.Serialize(tags);
    }

    public async Task<Guid> AddAsync(CreateEtlPipelineDto dto, Guid creatorId, string? creatorName)
    {
        var exists = await _db.Queryable<EtlPipeline>()
            .AnyAsync(p => p.Name == dto.Name);
        if (exists)
        {
            throw BusinessException.BadRequest("任务流名称已存在");
        }

        var entity = new EtlPipeline
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Category = dto.Category,
            Tags = SerializeTags(dto.Tags),
            Timeout = dto.Timeout,
            RetryCount = dto.RetryCount,
            Concurrency = dto.Concurrency,
            FailureStrategy = dto.FailureStrategy,
            DagConfig = dto.DagConfig,
            Status = "draft",
            Version = 1,
            CreatorId = creatorId,
            CreatorName = creatorName,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    public async Task<int> UpdateAsync(UpdateEtlPipelineDto dto)
    {
        var entity = await _db.Queryable<EtlPipeline>()
            .FirstAsync(p => p.Id == dto.Id);
        if (entity == null)
        {
            throw BusinessException.NotFound("任务流不存在");
        }

        if (dto.Name != null && dto.Name != entity.Name)
        {
            var exists = await _db.Queryable<EtlPipeline>()
                .AnyAsync(p => p.Name == dto.Name && p.Id != dto.Id);
            if (exists)
            {
                throw BusinessException.BadRequest("任务流名称已存在");
            }
            entity.Name = dto.Name;
        }

        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.Category != null) entity.Category = dto.Category;
        if (dto.Tags != null) entity.Tags = SerializeTags(dto.Tags);
        if (dto.Timeout.HasValue) entity.Timeout = dto.Timeout.Value;
        if (dto.RetryCount.HasValue) entity.RetryCount = dto.RetryCount.Value;
        if (dto.Concurrency.HasValue) entity.Concurrency = dto.Concurrency.Value;
        if (dto.FailureStrategy != null) entity.FailureStrategy = dto.FailureStrategy;
        if (dto.DagConfig != null)
        {
            // 支持传入JSON对象或字符串，使用 camelCase 序列化保持与前端一致
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            entity.DagConfig = dto.DagConfig is string str
                ? str
                : JsonSerializer.Serialize(dto.DagConfig, jsonOptions);
        }
        entity.Version++;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task<int> DeleteAsync(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0) return 0;

        // 检查是否有调度关联
        var hasSchedule = await _db.Queryable<EtlSchedule>()
            .AnyAsync(s => ids.Contains(s.PipelineId));
        if (hasSchedule)
        {
            throw BusinessException.BadRequest("有调度任务关联的任务流无法删除");
        }

        return await _db.Deleteable<EtlPipeline>()
            .Where(p => ids.Contains(p.Id))
            .ExecuteCommandAsync();
    }

    public async Task<int> PublishAsync(Guid id)
    {
        var entity = await _db.Queryable<EtlPipeline>()
            .FirstAsync(p => p.Id == id);
        if (entity == null)
        {
            throw BusinessException.NotFound("任务流不存在");
        }

        entity.Status = "published";
        entity.PublishTime = DateTime.Now;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task<int> UnpublishAsync(Guid id)
    {
        var entity = await _db.Queryable<EtlPipeline>()
            .FirstAsync(p => p.Id == id);
        if (entity == null)
        {
            throw BusinessException.NotFound("任务流不存在");
        }

        entity.Status = "draft";
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    public async Task<Guid> CopyAsync(Guid id, string name)
    {
        var source = await _db.Queryable<EtlPipeline>()
            .FirstAsync(p => p.Id == id);
        if (source == null)
        {
            throw BusinessException.NotFound("源任务流不存在");
        }

        var exists = await _db.Queryable<EtlPipeline>()
            .AnyAsync(p => p.Name == name);
        if (exists)
        {
            throw BusinessException.BadRequest("任务流名称已存在");
        }

        var entity = new EtlPipeline
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = source.Description,
            Category = source.Category,
            Tags = source.Tags,
            Timeout = source.Timeout,
            RetryCount = source.RetryCount,
            Concurrency = source.Concurrency,
            FailureStrategy = source.FailureStrategy,
            DagConfig = source.DagConfig,
            Status = "draft",
            Version = 1,
            CreatorId = source.CreatorId,
            CreatorName = source.CreatorName,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    public async Task<Guid> ExecuteAsync(Guid id, string? paramsJson, Guid? triggerUserId, string? triggerUserName)
    {
        _logger.LogInformation("开始执行任务流，任务流ID: {PipelineId}", id);

        var pipeline = await _db.Queryable<EtlPipeline>()
            .FirstAsync(p => p.Id == id);
        if (pipeline == null)
        {
            _logger.LogWarning("任务流不存在，任务流ID: {PipelineId}", id);
            throw BusinessException.NotFound("任务流不存在");
        }

        var execution = new EtlExecution
        {
            Id = Guid.NewGuid(),
            PipelineId = id,
            PipelineName = pipeline.Name,
            Status = "pending",
            ExecuteParams = paramsJson,
            TriggerType = "manual",
            TriggerUserId = triggerUserId,
            TriggerUserName = triggerUserName,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(execution).ExecuteCommandAsync();
        _logger.LogInformation("创建执行记录成功，执行ID: {ExecutionId}", execution.Id);

        // 启动异步执行（后台执行，不阻塞）
        try
        {
            _logger.LogInformation("启动异步执行引擎，执行引擎是否注入: {HasEngine}", _executionEngine != null);

            // 使用异步模式执行，立即返回执行ID
            if (_executionEngine != null)
            {
                _ = _executionEngine.ExecuteAsyncAsync(execution.Id);
            }
            else
            {
                _logger.LogWarning("执行引擎未注入，无法启动异步执行");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动任务流执行失败，执行ID: {ExecutionId}", execution.Id);
        }

        return execution.Id;
    }

    public async Task<string?> GetDagConfigAsync(Guid id)
    {
        var entity = await _db.Queryable<EtlPipeline>()
            .FirstAsync(p => p.Id == id);
        return entity?.DagConfig;
    }

    public async Task<int> SaveDagConfigAsync(Guid id, string dagConfig)
    {
        var entity = await _db.Queryable<EtlPipeline>()
            .FirstAsync(p => p.Id == id);
        if (entity == null)
        {
            throw BusinessException.NotFound("任务流不存在");
        }

        entity.DagConfig = dagConfig;
        entity.Version++;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }
}