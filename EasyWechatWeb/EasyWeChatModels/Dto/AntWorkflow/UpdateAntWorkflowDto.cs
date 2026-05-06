using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 更新 Ant 流程 DTO
/// </summary>
public class UpdateAntWorkflowDto
{
    /// <summary>流程ID</summary>
    [Required(ErrorMessage = "流程ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>流程名称</summary>
    [Required(ErrorMessage = "流程名称不能为空")]
    [MaxLength(100, ErrorMessage = "流程名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>分类编码</summary>
    [MaxLength(50)]
    public string CategoryCode { get; set; } = string.Empty;

    /// <summary>流程描述</summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>DAG配置JSON</summary>
    public string? FlowConfig { get; set; }
}