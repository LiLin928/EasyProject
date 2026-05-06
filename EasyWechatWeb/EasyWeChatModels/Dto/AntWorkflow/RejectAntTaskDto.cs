using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 审批驳回 DTO
/// </summary>
public class RejectAntTaskDto
{
    /// <summary>任务ID</summary>
    [Required(ErrorMessage = "任务ID不能为空")]
    public Guid TaskId { get; set; }

    /// <summary>驳回原因</summary>
    [Required(ErrorMessage = "驳回原因不能为空")]
    [MaxLength(500)]
    public string RejectReason { get; set; } = string.Empty;
}