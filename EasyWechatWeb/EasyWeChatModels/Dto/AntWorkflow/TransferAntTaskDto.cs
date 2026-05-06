using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 转办任务 DTO
/// </summary>
public class TransferAntTaskDto
{
    /// <summary>任务ID</summary>
    [Required(ErrorMessage = "任务ID不能为空")]
    public Guid TaskId { get; set; }

    /// <summary>转交给用户ID</summary>
    [Required(ErrorMessage = "转交用户不能为空")]
    public Guid TransferToUserId { get; set; }

    /// <summary>转交原因</summary>
    [MaxLength(500)]
    public string? TransferReason { get; set; }
}