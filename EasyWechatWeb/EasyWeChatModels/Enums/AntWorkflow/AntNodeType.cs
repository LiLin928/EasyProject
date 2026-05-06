namespace EasyWeChatModels.Enums;

/// <summary>
/// 节点类型枚举
/// </summary>
public enum AntNodeType
{
    /// <summary>发起人节点</summary>
    Start = 0,
    /// <summary>审批人节点</summary>
    Approver = 1,
    /// <summary>抄送人节点</summary>
    Copyer = 2,
    /// <summary>条件分支节点</summary>
    Condition = 4,
    /// <summary>服务任务节点</summary>
    Service = 5,
    /// <summary>通知节点</summary>
    Notification = 6,
    /// <summary>并行分支节点</summary>
    Parallel = 7,
    /// <summary>会签节点</summary>
    CounterSign = 8,
    /// <summary>子流程节点</summary>
    Subflow = 9,
    /// <summary>Webhook节点</summary>
    Webhook = 10,
    /// <summary>结束节点</summary>
    End = 99
}