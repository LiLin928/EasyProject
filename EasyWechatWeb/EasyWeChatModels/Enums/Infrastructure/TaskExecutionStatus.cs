namespace EasyWeChatModels.Enums;

/// <summary>
/// 任务执行状态枚举
/// </summary>
public enum TaskExecutionStatus
{
    /// <summary>执行中</summary>
    Running = 0,

    /// <summary>成功</summary>
    Success = 1,

    /// <summary>失败</summary>
    Failed = 2,

    /// <summary>取消</summary>
    Cancelled = 3
}