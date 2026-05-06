namespace EasyWeChatModels.Dto;

/// <summary>
/// 供应商查询参数
/// </summary>
public class QuerySupplierDto
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
    /// 供应商名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}