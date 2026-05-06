namespace EasyWeChatModels.Dto;

/// <summary>
/// 商品 DTO
/// </summary>
public class ProductDto
{
    /// <summary>
    /// 商品ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// SKU码
    /// </summary>
    public string SkuCode { get; set; } = string.Empty;

    /// <summary>
    /// 商品名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 价格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 原价
    /// </summary>
    public decimal? OriginalPrice { get; set; }

    /// <summary>
    /// 主图
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// 详情图列表
    /// </summary>
    public List<string>? Images { get; set; }

    /// <summary>
    /// 分类ID
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// 分类名称
    /// </summary>
    public string? CategoryName { get; set; }

    /// <summary>
    /// 分类信息
    /// </summary>
    public CategoryDto? Category { get; set; }

    /// <summary>
    /// 库存
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// 库存预警阈值
    /// </summary>
    public int AlertThreshold { get; set; }

    /// <summary>
    /// 销量
    /// </summary>
    public int Sales { get; set; }

    /// <summary>
    /// 是否热销
    /// </summary>
    public bool IsHot { get; set; }

    /// <summary>
    /// 是否新品
    /// </summary>
    public bool IsNew { get; set; }

    /// <summary>
    /// 详情富文本
    /// </summary>
    public string? Detail { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string CreateTime { get; set; } = string.Empty;

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdateTime { get; set; }

    /// <summary>
    /// 审核状态
    /// </summary>
    /// <remarks>
    /// 0-待提交，1-待审核，2-已驳回，3-已通过，4-已撤回
    /// </remarks>
    public int AuditStatus { get; set; }

    /// <summary>
    /// 工作流实例ID
    /// </summary>
    public Guid? WorkflowInstanceId { get; set; }

    /// <summary>
    /// 审核节点编码
    /// </summary>
    public string? AuditPointCode { get; set; }

    /// <summary>
    /// 审核时间
    /// </summary>
    public string? AuditTime { get; set; }

    /// <summary>
    /// 审核人ID
    /// </summary>
    public Guid? AuditorId { get; set; }

    /// <summary>
    /// 审核备注
    /// </summary>
    public string? AuditRemark { get; set; }
}