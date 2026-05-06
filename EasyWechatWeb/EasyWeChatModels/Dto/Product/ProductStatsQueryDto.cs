namespace EasyWeChatModels.Dto;

/// <summary>
/// 统计查询参数
/// </summary>
public class ProductStatsQueryDto
{
    /// <summary>
    /// 开始日期
    /// </summary>
    public string? StartDate { get; set; }

    /// <summary>
    /// 结束日期
    /// </summary>
    public string? EndDate { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Top数量
    /// </summary>
    public int? Top { get; set; } = 10;
}