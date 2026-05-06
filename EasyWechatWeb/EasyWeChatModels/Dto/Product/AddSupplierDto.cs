namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加供应商参数
/// </summary>
public class AddSupplierDto
{
    /// <summary>
    /// 供应商名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 三证合一码
    /// </summary>
    public string? UnifiedCode { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    public string? Contact { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; } = 1;
}