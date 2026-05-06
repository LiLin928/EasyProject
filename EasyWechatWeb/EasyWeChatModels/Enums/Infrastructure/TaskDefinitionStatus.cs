namespace EasyWeChatModels.Enums;

/// <summary>
/// 任务定义状态枚举
/// </summary>
public enum TaskDefinitionStatus
{
    /// <summary>待调度</summary>
    Pending = 0,

    /// <summary>已调度</summary>
    Scheduled = 1,

    /// <summary>暂停</summary>
    Paused = 2,

    /// <summary>已完成</summary>
    Completed = 3,

    /// <summary>失败</summary>
    Failed = 4
}