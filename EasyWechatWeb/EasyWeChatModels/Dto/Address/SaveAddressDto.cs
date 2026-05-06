namespace EasyWeChatModels.Dto;

/// <summary>
/// 保存地址 DTO
/// </summary>
public class SaveAddressDto
{
    /// <summary>
    /// 地址ID（更新时必填）
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// 收货人姓名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 手机号
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 省份
    /// </summary>
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 区县
    /// </summary>
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// 详细地址
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// 是否默认
    /// </summary>
    public bool IsDefault { get; set; }
}