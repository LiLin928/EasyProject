namespace EasyWeChatModels.Enums;

/// <summary>
/// 节点审批状态枚举
/// </summary>
public enum NodeApproveStatus
{
    /// <summary>待处理</summary>
    Pending = 0,
    /// <summary>处理中</summary>
    Processing = 1,
    /// <summary>已完成</summary>
    Completed = 2,
    /// <summary>已跳过</summary>
    Skipped = 3,
    /// <summary>已驳回</summary>
    Rejected = 4
}