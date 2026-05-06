namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新规格参数
/// </summary>
public class UpdateProductSpecDto
{
    /// <summary>
    /// 规格ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 规格名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 是否必选
    /// </summary>
    public bool? Required { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int? Sort { get; set; }
}