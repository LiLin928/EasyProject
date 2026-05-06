using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 复制 Ant 流程 DTO
/// </summary>
public class CopyAntWorkflowDto
{
    /// <summary>源流程ID</summary>
    [Required(ErrorMessage = "源流程ID不能为空")]
    public Guid SourceId { get; set; }

    /// <summary>新流程名称</summary>
    [Required(ErrorMessage = "新流程名称不能为空")]
    [MaxLength(100, ErrorMessage = "流程名称不能超过100个字符")]
    public string NewName { get; set; } = string.Empty;

    /// <summary>新流程编码</summary>
    [MaxLength(50)]
    public string? NewCode { get; set; }
}