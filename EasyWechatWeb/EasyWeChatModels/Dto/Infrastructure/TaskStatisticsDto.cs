namespace EasyWeChatModels.Dto;

/// <summary>
/// 任务统计响应 DTO
/// </summary>
public class TaskStatisticsDto
{
    /// <summary>
    /// 总任务数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 启用任务数
    /// </summary>
    public int EnabledCount { get; set; }

    /// <summary>
    /// 暂停任务数
    /// </summary>
    public int PausedCount { get; set; }

    /// <summary>
    /// 今日执行次数
    /// </summary>
    public int TodayExecuted { get; set; }

    /// <summary>
    /// 今日成功次数
    /// </summary>
    public int TodaySuccess { get; set; }

    /// <summary>
    /// 今日失败次数
    /// </summary>
    public int TodayFailure { get; set; }
}