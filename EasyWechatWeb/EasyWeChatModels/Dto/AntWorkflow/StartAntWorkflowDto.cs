using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 发起流程 DTO
/// </summary>
public class StartAntWorkflowDto
{
    /// <summary>流程定义ID</summary>
    [Required(ErrorMessage = "流程定义ID不能为空")]
    public Guid WorkflowId { get; set; }

    /// <summary>流程标题</summary>
    [MaxLength(200)]
    public string? Title { get; set; }

    /// <summary>业务类型编码</summary>
    [MaxLength(50)]
    public string? BusinessType { get; set; }

    /// <summary>业务单据ID</summary>
    [MaxLength(100)]
    public string? BusinessId { get; set; }

    /// <summary>业务数据JSON</summary>
    public string? BusinessData { get; set; }

    /// <summary>表单数据JSON</summary>
    public string? FormData { get; set; }
}