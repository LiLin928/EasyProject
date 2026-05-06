namespace EasyWeChatModels.Enums;

/// <summary>
/// 任务类型枚举
/// </summary>
public enum TaskType
{
    /// <summary>Cron 定时任务</summary>
    Cron = 0,

    /// <summary>即时执行（业务触发）</summary>
    Immediate = 1,

    /// <summary>周期任务（每天/每月）</summary>
    Periodic = 2
}