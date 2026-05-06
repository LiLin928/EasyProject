using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 商品实体类
/// </summary>
/// <remarks>
/// 用于存储商品信息，包括价格、库存、分类等
/// </remarks>
[SugarTable("Product", "商品表")]
public class Product
{
    /// <summary>
    /// 商品ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 商品名称
    /// </summary>
    /// <remarks>
    /// 商品的显示名称，长度限制100字符
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "商品名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// SKU码（商品唯一标识）
    /// </summary>
    /// <remarks>
    /// 用于商品库存管理和供应商关联的唯一编码
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "SKU码")]
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// 商品描述
    /// </summary>
    /// <remarks>
    /// 商品的简短描述，长度限制500字符，可为空
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "商品描述")]
    public string? Description { get; set; }

    /// <summary>
    /// 销售价格
    /// </summary>
    /// <remarks>
    /// 商品的当前销售价格，保留两位小数
    /// </remarks>
    [SugarColumn(DecimalDigits = 2, ColumnDescription = "销售价格")]
    public decimal Price { get; set; }

    /// <summary>
    /// 原价
    /// </summary>
    /// <remarks>
    /// 商品的原价/划线价，保留两位小数，可为空
    /// </remarks>
    [SugarColumn(DecimalDigits = 2, IsNullable = true, ColumnDescription = "原价")]
    public decimal? OriginalPrice { get; set; }

    /// <summary>
    /// 主图
    /// </summary>
    /// <remarks>
    /// 商品主图URL地址，长度限制500字符
    /// </remarks>
    [SugarColumn(Length = 500, ColumnDescription = "主图")]
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// 商品图片列表
    /// </summary>
    /// <remarks>
    /// 商品详情图片URL列表，JSON格式存储，可为空
    /// </remarks>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "商品图片列表")]
    public string? Images { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    /// <remarks>
    /// 所属商品分类的外键
    /// </remarks>
    [SugarColumn(ColumnDescription = "分类ID")]
    public Guid CategoryId { get; set; }

    /// <summary>
    /// 库存数量
    /// </summary>
    /// <remarks>
    /// 商品的库存数量
    /// </remarks>
    [SugarColumn(ColumnDescription = "库存数量")]
    public int Stock { get; set; } = 0;

    /// <summary>
    /// 库存预警阈值
    /// </summary>
    /// <remarks>
    /// 当库存低于此值时触发预警，默认为10
    /// </remarks>
    [SugarColumn(ColumnDescription = "库存预警阈值")]
    public int AlertThreshold { get; set; } = 10;

    /// <summary>
    /// 销量
    /// </summary>
    /// <remarks>
    /// 商品的累计销售数量
    /// </remarks>
    [SugarColumn(ColumnDescription = "销量")]
    public int Sales { get; set; } = 0;

    /// <summary>
    /// 是否热销
    /// </summary>
    /// <remarks>
    /// 是否为热销商品：true-是，false-否
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否热销")]
    public bool IsHot { get; set; } = false;

    /// <summary>
    /// 是否新品
    /// </summary>
    /// <remarks>
    /// 是否为新品推荐：true-是，false-否
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否新品")]
    public bool IsNew { get; set; } = false;

    /// <summary>
    /// 商品详情
    /// </summary>
    /// <remarks>
    /// 商品的详细描述，富文本格式，可为空
    /// </remarks>
    [SugarColumn(ColumnDataType = "longtext", IsNullable = true, ColumnDescription = "商品详情")]
    public string? Detail { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 商品状态：1-上架，0-下架。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "状态：1-上架，0-下架")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 商品记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <remarks>
    /// 商品信息最后更新时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 审核状态
    /// </summary>
    /// <remarks>
    /// 商品审核状态：0-待提交，1-待审核，2-已驳回，3-已通过，4-已撤回。
    /// 默认值为0（待提交）。与 AntWorkflow 工作流引擎集成。
    /// </remarks>
    [SugarColumn(ColumnDescription = "审核状态：0-待提交，1-待审核，2-已驳回，3-已通过，4-已撤回")]
    public int AuditStatus { get; set; } = 0;

    /// <summary>
    /// 工作流实例ID
    /// </summary>
    /// <remarks>
    /// 关联 AntWorkflow 工作流实例的外键。
    /// 当商品进入审核流程时，记录对应的工作流实例ID，可为空。
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "工作流实例ID")]
    public Guid? WorkflowInstanceId { get; set; }

    /// <summary>
    /// 审核节点编码
    /// </summary>
    /// <remarks>
    /// 当前审核流程所处的节点编码。
    /// 用于标识商品在审核流程中的具体节点位置，长度限制50字符，可为空。
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "审核节点编码")]
    public string? AuditPointCode { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    /// <remarks>
    /// 商品最后一次审核操作的时间。
    /// 包括提交审核、审核通过、审核驳回、撤回等操作的时间，可为空。
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "审核时间")]
    public DateTime? AuditTime { get; set; }

    /// <summary>
    /// 审核人ID
    /// </summary>
    /// <remarks>
    /// 最后一次审核操作的用户ID。
    /// 关联用户表的外键，可为空。
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "审核人ID")]
    public Guid? AuditorId { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    /// <remarks>
    /// 审核意见或备注信息。
    /// 审核人员在审核时填写的意见，长度限制500字符，可为空。
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "审核备注")]
    public string? AuditRemark { get; set; }
}