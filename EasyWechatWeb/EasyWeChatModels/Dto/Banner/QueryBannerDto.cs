namespace EasyWeChatModels.Dto;

/// <summary>
/// 轮播图查询 DTO
/// </summary>
public class QueryBannerDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 状态筛选
    /// </summary>
    public int? Status { get; set; }
}