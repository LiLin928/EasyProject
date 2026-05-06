namespace EasyWeChatModels.Enums;

/// <summary>
/// 实例状态枚举
/// </summary>
public enum InstanceStatus
{
    /// <summary>待提交</summary>
    WaitSubmit = 0,
    /// <summary>审批中</summary>
    Approving = 1,
    /// <summary>已通过</summary>
    Passed = 2,
    /// <summary>已驳回</summary>
    Rejected = 3,
    /// <summary>已撤回</summary>
    Withdrawn = 4,
    /// <summary>已终止</summary>
    Terminated = 5
}