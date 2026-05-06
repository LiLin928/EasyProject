using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 收货地址实体类
/// </summary>
/// <remarks>
/// 用于存储用户收货地址信息
/// </remarks>
[SugarTable("Address", "收货地址表")]
public class Address
{
    /// <summary>
    /// 地址ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 用户ID
    /// </summary>
    /// <remarks>
    /// 所属用户的外键
    /// </remarks>
    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    /// <summary>
    /// 收货人姓名
    /// </summary>
    /// <remarks>
    /// 收货人的姓名，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "收货人姓名")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    /// <remarks>
    /// 收货人联系电话，长度限制20字符
    /// </remarks>
    [SugarColumn(Length = 20, ColumnDescription = "联系电话")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 省份
    /// </summary>
    /// <remarks>
    /// 收货地址所在省份，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "省份")]
    public string Province { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    /// <remarks>
    /// 收货地址所在城市，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "城市")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 区/县
    /// </summary>
    /// <remarks>
    /// 收货地址所在区/县，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "区/县")]
    public string District { get; set; } = string.Empty;

    /// <summary>
    /// 详细地址
    /// </summary>
    /// <remarks>
    /// 收货详细地址，长度限制200字符
    /// </remarks>
    [SugarColumn(Length = 200, ColumnDescription = "详细地址")]
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// 是否默认地址
    /// </summary>
    /// <remarks>
    /// 是否为用户的默认收货地址：true-是，false-否
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否默认地址")]
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 地址状态：1-正常，0-已删除。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-正常，0-已删除")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 地址记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <remarks>
    /// 地址记录最后更新时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}