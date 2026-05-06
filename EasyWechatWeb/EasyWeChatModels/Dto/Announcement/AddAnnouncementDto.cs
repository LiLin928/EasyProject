using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加公告请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新公告时提交的公告信息
/// </remarks>
public class AddAnnouncementDto
{
    /// <summary>
    /// 标题
    /// </summary>
    /// <remarks>
    /// 公告标题，长度限制200字符
    /// </remarks>
    /// <example>系统升级通知</example>
    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200, ErrorMessage = "标题长度不能超过200字符")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 内容（富文本）
    /// </summary>
    /// <remarks>
    /// 公告正文内容，支持富文本格式
    /// </remarks>
    /// <example>系统将于今晚22:00进行升级维护...</example>
    [Required(ErrorMessage = "内容不能为空")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 类型：1全员公告 2定向公告
    /// </summary>
    /// <remarks>
    /// 全员公告发送给所有用户，定向公告发送给指定角色用户
    /// </remarks>
    /// <example>1</example>
    [Required(ErrorMessage = "类型不能为空")]
    public int Type { get; set; } = 1;

    /// <summary>
    /// 级别：1普通 2重要 3紧急
    /// </summary>
    /// <remarks>
    /// 公告级别，用于区分重要程度
    /// </remarks>
    /// <example>2</example>
    [Required(ErrorMessage = "级别不能为空")]
    public int Level { get; set; } = 1;

    /// <summary>
    /// 目标角色ID列表
    /// </summary>
    /// <remarks>
    /// 定向公告时指定接收的角色ID列表，全员公告时不需要
    /// </remarks>
    /// <example>["3fa85f64-5717-4562-b3fc-2c963f66afa6"]</example>
    public List<Guid>? TargetRoleIds { get; set; }

    /// <summary>
    /// 是否置顶：0否 1是
    /// </summary>
    /// <example>1</example>
    public int IsTop { get; set; } = 0;
}