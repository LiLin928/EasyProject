namespace EasyWeChatModels.Enums;

/// <summary>
/// 流程状态枚举
/// </summary>
public enum WorkflowStatus
{
    /// <summary>草稿</summary>
    Draft = 0,
    /// <summary>待审核</summary>
    Pending = 1,
    /// <summary>已发布</summary>
    Published = 2,
    /// <summary>已拒绝</summary>
    Rejected = 3,
    /// <summary>已停用</summary>
    Disabled = 4
}