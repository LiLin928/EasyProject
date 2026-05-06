namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 通知配置
/// </summary>
public class NotificationConfig
{
    /// <summary>是否启用通知</summary>
    public bool Enabled { get; set; }

    /// <summary>通知类型：message/email/sms/wechat</summary>
    public string Type { get; set; } = "message";

    /// <summary>消息标题</summary>
    public string? Title { get; set; }

    /// <summary>消息内容</summary>
    public string? Content { get; set; }

    /// <summary>接收人列表</summary>
    public List<NodeUser>? Recipients { get; set; }

    /// <summary>发送给发起人</summary>
    public bool SendToInitiator { get; set; } = false;

    /// <summary>发送给主管</summary>
    public bool SendToSupervisor { get; set; } = false;
}