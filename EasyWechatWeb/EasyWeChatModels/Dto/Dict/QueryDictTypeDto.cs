namespace EasyWeChatModels.Dto;

/// <summary>
/// 字典类型查询参数
/// </summary>
public class QueryDictTypeDto
{
    /// <summary>
    /// 页码，从1开始
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 字典编码（模糊查询）
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 字典名称（模糊查询）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}