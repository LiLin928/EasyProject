namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// Ant流程定义 DTO
/// </summary>
public class AntWorkflowDto
{
    /// <summary>流程ID</summary>
    public Guid Id { get; set; }

    /// <summary>流程名称</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>流程编码</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>分类编码</summary>
    public string CategoryCode { get; set; } = string.Empty;

    /// <summary>分类名称</summary>
    public string? CategoryName { get; set; }

    /// <summary>流程描述</summary>
    public string? Description { get; set; }

    /// <summary>状态：0草稿/1待审核/2已发布/3拒绝/4停用</summary>
    public int Status { get; set; }

    /// <summary>当前版本号</summary>
    public string CurrentVersion { get; set; } = "1.0";

    /// <summary>DAG配置JSON</summary>
    public string? FlowConfig { get; set; }

    /// <summary>创建人ID</summary>
    public Guid CreatorId { get; set; }

    /// <summary>创建人姓名</summary>
    public string? CreatorName { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreateTime { get; set; }

    /// <summary>更新时间</summary>
    public DateTime? UpdateTime { get; set; }
}