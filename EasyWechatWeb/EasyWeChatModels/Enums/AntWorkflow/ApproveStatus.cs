namespace EasyWeChatModels.Enums;

/// <summary>
/// 审批结果枚举
/// </summary>
public enum ApproveStatus
{
    /// <summary>通过</summary>
    Pass = 1,
    /// <summary>驳回</summary>
    Reject = 2,
    /// <summary>转交</summary>
    Transfer = 3,
    /// <summary>撤回</summary>
    Withdraw = 4,
    /// <summary>加签</summary>
    AddSigner = 5
}