using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 微信用户实体类
/// </summary>
/// <remarks>
/// 继承自 User，扩展微信特有的字段
/// </remarks>
[SugarTable("WeChatUser", "微信用户表")]
public class WeChatUser : User
{
    /// <summary>
    /// 微信OpenId
    /// </summary>
    /// <remarks>
    /// 微信用户唯一标识，用于微信授权登录
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "微信OpenId")]
    public string OpenId { get; set; } = string.Empty;

    /// <summary>
    /// 微信UnionId
    /// </summary>
    /// <remarks>
    /// 微信开放平台唯一标识，用于跨应用用户识别，可为空
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "微信UnionId")]
    public string? UnionId { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    /// <remarks>
    /// 微信用户昵称，长度限制100字符
    /// </remarks>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "昵称")]
    public string? Nickname { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    /// <remarks>
    /// 微信用户头像地址
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "头像URL")]
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    /// <remarks>
    /// 性别：0-未知，1-男，2-女
    /// </remarks>
    [SugarColumn(ColumnDescription = "性别：0-未知，1-男，2-女")]
    public int Gender { get; set; } = 0;

    /// <summary>
    /// 微信登录时间
    /// </summary>
    /// <remarks>
    /// 用户最近一次微信登录的时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "微信登录时间")]
    public DateTime? WxLoginTime { get; set; }

    /// <summary>
    /// 会员等级ID
    /// </summary>
    /// <remarks>
    /// 关联的会员等级，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "会员等级ID")]
    public Guid? LevelId { get; set; }

    /// <summary>
    /// 累计消费金额
    /// </summary>
    /// <remarks>
    /// 用户累计消费总金额，用于会员等级计算
    /// </remarks>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "累计消费金额")]
    public decimal TotalSpent { get; set; } = 0;

    /// <summary>
    /// 可用积分
    /// </summary>
    /// <remarks>
    /// 用户当前可用的积分余额
    /// </remarks>
    [SugarColumn(ColumnDescription = "可用积分")]
    public int Points { get; set; } = 0;

    /// <summary>
    /// 绑定手机号时间
    /// </summary>
    /// <remarks>
    /// 记录用户绑定手机号的时间，可用于验证绑定状态
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "绑定手机号时间")]
    public DateTime? BindPhoneTime { get; set; }
}