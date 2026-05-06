using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 创建 Ant 流程 DTO
/// </summary>
public class CreateAntWorkflowDto
{
    /// <summary>流程名称</summary>
    [Required(ErrorMessage = "流程名称不能为空")]
    [MaxLength(100, ErrorMessage = "流程名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>流程编码</summary>
    [Required(ErrorMessage = "流程编码不能为空")]
    [MaxLength(50, ErrorMessage = "流程编码不能超过50个字符")]
    public string Code { get; set; } = string.Empty;

    /// <summary>分类编码</summary>
    [MaxLength(50)]
    public string CategoryCode { get; set; } = string.Empty;

    /// <summary>流程描述</summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>DAG配置JSON</summary>
    public string? FlowConfig { get; set; }
}