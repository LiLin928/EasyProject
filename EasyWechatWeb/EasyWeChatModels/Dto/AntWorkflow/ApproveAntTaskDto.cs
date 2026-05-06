using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 审批通过 DTO
/// </summary>
public class ApproveAntTaskDto
{
    /// <summary>任务ID</summary>
    [Required(ErrorMessage = "任务ID不能为空")]
    public Guid TaskId { get; set; }

    /// <summary>审批意见</summary>
    [MaxLength(500)]
    public string? ApproveDesc { get; set; }

    /// <summary>表单数据JSON</summary>
    public string? FormData { get; set; }
}