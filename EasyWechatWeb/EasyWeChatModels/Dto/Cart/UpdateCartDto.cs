namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新购物车 DTO
/// </summary>
public class UpdateCartDto
{
    /// <summary>
    /// 购物车项ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 数量
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 是否选中
    /// </summary>
    public bool? Selected { get; set; }
}