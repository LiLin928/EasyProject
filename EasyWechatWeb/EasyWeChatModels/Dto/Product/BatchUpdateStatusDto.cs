namespace EasyWeChatModels.Dto;

/// <summary>
/// 批量更新状态参数
/// </summary>
public class BatchUpdateStatusDto
{
    /// <summary>
    /// 商品ID列表
    /// </summary>
    public List<Guid> Ids { get; set; } = new List<Guid>();

    /// <summary>
    /// 状态值
    /// </summary>
    public int Status { get; set; }
}