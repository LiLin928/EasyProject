namespace EasyWeChatModels.Dto;

/// <summary>
/// 执行趋势数据点 DTO
/// </summary>
public class TaskLogTrendPointDto
{
    /// <summary>
    /// 日期（格式：yyyy-MM-dd）
    /// </summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>
    /// 执行次数
    /// </summary>
    public int ExecuteCount { get; set; }

    /// <summary>
    /// 成功次数
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 成功率（百分比）
    /// </summary>
    public double SuccessRate { get; set; }
}

/// <summary>
/// 执行趋势响应 DTO
/// </summary>
public class TaskLogTrendDto
{
    /// <summary>
    /// 趋势数据点列表（按日期排序）
    /// </summary>
    public List<TaskLogTrendPointDto> Points { get; set; } = new();
}