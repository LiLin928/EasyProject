using SqlSugar;
using EasyWeChatModels.Dto.AntWorkflow;
using EasyWeChatModels.Entitys;
using BusinessManager.Buz.IService;

namespace BusinessManager.Buz.Service;

/// <summary>
/// Ant流程版本服务实现
/// </summary>
public class AntWorkflowVersionService : IAntWorkflowVersionService
{
    /// <summary>数据库上下文（Autofac 属性注入）</summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <inheritdoc/>
    public async Task<List<AntWorkflowVersionDto>> GetListByWorkflowIdAsync(Guid workflowId)
    {
        var list = await _db.Queryable<AntWorkflowVersion>()
            .Where(v => v.WorkflowId == workflowId)
            .OrderByDescending(v => v.PublishTime)
            .ToListAsync();

        return list.Select(v => new AntWorkflowVersionDto
        {
            Id = v.Id,
            WorkflowId = v.WorkflowId,
            Version = v.Version,
            FlowConfig = v.FlowConfig,
            PublishTime = v.PublishTime,
            PublisherId = v.PublisherId,
            PublisherName = v.PublisherName,
            Status = v.Status,
            Remark = v.Remark
        }).ToList();
    }

    /// <inheritdoc/>
    public async Task<AntWorkflowVersionDto?> GetByIdAsync(Guid id)
    {
        var version = await _db.Queryable<AntWorkflowVersion>()
            .Where(v => v.Id == id)
            .FirstAsync();

        if (version == null)
        {
            return null;
        }

        return new AntWorkflowVersionDto
        {
            Id = version.Id,
            WorkflowId = version.WorkflowId,
            Version = version.Version,
            FlowConfig = version.FlowConfig,
            PublishTime = version.PublishTime,
            PublisherId = version.PublisherId,
            PublisherName = version.PublisherName,
            Status = version.Status,
            Remark = version.Remark
        };
    }

    /// <inheritdoc/>
    public async Task<AntWorkflowVersionDto?> GetLatestVersionAsync(Guid workflowId)
    {
        var version = await _db.Queryable<AntWorkflowVersion>()
            .Where(v => v.WorkflowId == workflowId)
            .OrderByDescending(v => v.PublishTime)
            .FirstAsync();

        if (version == null)
        {
            return null;
        }

        return new AntWorkflowVersionDto
        {
            Id = version.Id,
            WorkflowId = version.WorkflowId,
            Version = version.Version,
            FlowConfig = version.FlowConfig,
            PublishTime = version.PublishTime,
            PublisherId = version.PublisherId,
            PublisherName = version.PublisherName,
            Status = version.Status,
            Remark = version.Remark
        };
    }
}