namespace EasyWeChatModels.Enums;

/// <summary>
/// CAP 消息状态枚举
/// </summary>
public enum CapMessageStatus
{
    /// <summary>待处理</summary>
    Pending = 0,

    /// <summary>成功</summary>
    Success = 1,

    /// <summary>失败</summary>
    Failed = 2,

    /// <summary>重试中</summary>
    Retrying = 3
}