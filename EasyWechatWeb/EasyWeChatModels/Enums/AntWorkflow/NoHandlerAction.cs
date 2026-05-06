namespace EasyWeChatModels.Enums;

/// <summary>
/// 无处理人时的操作枚举
/// </summary>
public enum NoHandlerAction
{
    /// <summary>自动通过</summary>
    AutoPass = 1,
    /// <summary>转交管理员</summary>
    Transfer = 2,
    /// <summary>自动拒绝</summary>
    AutoReject = 3
}