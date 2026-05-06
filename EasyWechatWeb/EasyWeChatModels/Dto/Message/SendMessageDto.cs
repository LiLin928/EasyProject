using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 发送消息请求 DTO
/// </summary>
/// <remarks>
/// 用于发送新消息时提交的消息内容和接收者信息
/// </remarks>
public class SendMessageDto
{
    /// <summary>
    /// 消息标题
    /// </summary>
    /// <remarks>
    /// 消息标题，必填，长度限制200字符
    /// </remarks>
    /// <example>审批通过通知</example>
    [Required(ErrorMessage = "消息标题不能为空")]
    [StringLength(200, ErrorMessage = "消息标题长度不能超过200字符")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    /// <remarks>
    /// 消息详细内容，必填
    /// </remarks>
    /// <example>您的请假申请已通过审批，请假时间：2024-01-02至2024-01-03</example>
    [Required(ErrorMessage = "消息内容不能为空")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 消息类型
    /// </summary>
    /// <remarks>
    /// 消息类型：1-系统消息 2-通知 3-提醒，默认为2（通知）
    /// </remarks>
    /// <example>2</example>
    public int Type { get; set; } = 2;

    /// <summary>
    /// 消息级别
    /// </summary>
    /// <remarks>
    /// 消息级别：1-普通 2-重要 3-紧急，默认为1（普通）
    /// </remarks>
    /// <example>1</example>
    public int Level { get; set; } = 1;

    /// <summary>
    /// 接收者用户ID列表
    /// </summary>
    /// <remarks>
    /// 接收消息的用户ID列表，群发时传入多个用户ID。
    /// 为空或null时表示发送给所有用户（需要管理员权限）
    /// </remarks>
    /// <example>[1, 2, 3]</example>
    public List<Guid>? UserIds { get; set; }

    /// <summary>
    /// 发送者ID
    /// </summary>
    /// <remarks>
    /// 发送消息的用户ID，可选。为空时表示系统发送
    /// </remarks>
    /// <example>1</example>
    public Guid? SenderId { get; set; }

    /// <summary>
    /// 发送者名称
    /// </summary>
    /// <remarks>
    /// 发送者显示名称，可选。为空时显示"系统"
    /// </remarks>
    /// <example>管理员</example>
    public string? SenderName { get; set; }
}