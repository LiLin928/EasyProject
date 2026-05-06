namespace EasyWeChatModels.Dto;

/// <summary>
/// 客户查询参数
/// </summary>
public class QueryCustomerDto
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
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 昵称关键字
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    public Guid? LevelId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}