using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 业务审核点配置表
/// </summary>
/// <remarks>
/// 用于配置各业务模块的审核点，关联审批流程，定义状态字段映射等
/// 参考设计：类似于钉钉审批的业务审批配置
/// </remarks>
[SugarTable("BusinessAuditPoint", "业务审核点配置表")]
public class BusinessAuditPoint
{
    /// <summary>
    /// 审核点ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 审核点编码
    /// </summary>
    /// <remarks>
    /// 唯一标识，如：ProductAudit、OrderAudit 等
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "审核点编码")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 审核点名称
    /// </summary>
    /// <remarks>
    /// 显示名称，如：商品审核、订单审核 等
    /// </remarks>
    [SugarColumn(Length = 100, ColumnDescription = "审核点名称")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 审核点分类
    /// </summary>
    /// <remarks>
    /// 分类路径，如：商品管理->商品审核、订单管理->订单审核
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "审核点分类")]
    public string? Category { get; set; }

    /// <summary>
    /// 关联的流程定义ID
    /// </summary>
    /// <remarks>
    /// 关联 AntWorkflow.Id，定义审批流程
    /// </remarks>
    [SugarColumn(ColumnDescription = "关联流程定义ID")]
    public Guid WorkflowId { get; set; }

    /// <summary>
    /// 处理表名
    /// </summary>
    /// <remarks>
    /// 业务表名称，如：Product、Order 等
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "处理表名")]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// 主键字段名
    /// </summary>
    /// <remarks>
    /// 业务表的主键字段名，默认为 "Id"。
    /// 用于条件分支查询时关联业务数据：WHERE {PrimaryKeyField} = {BusinessId}
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "主键字段名")]
    public string PrimaryKeyField { get; set; } = "Id";

    /// <summary>
    /// 状态字段名
    /// </summary>
    /// <remarks>
    /// 业务表中的状态字段名，如：Status、AuditStatus 等
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "状态字段名")]
    public string StatusField { get; set; } = "Status";

    /// <summary>
    /// 待审核状态值
    /// </summary>
    /// <remarks>
    /// 提交审核后设置的状态值，如：2（表示审核中）
    /// </remarks>
    [SugarColumn(ColumnDescription = "待审核状态值")]
    public int AuditStatusValue { get; set; } = 2;

    /// <summary>
    /// 审核通过状态值
    /// </summary>
    /// <remarks>
    /// 审核通过后设置的状态值，如：1（表示上架/启用）
    /// </remarks>
    [SugarColumn(ColumnDescription = "审核通过状态值")]
    public int PassStatusValue { get; set; } = 1;

    /// <summary>
    /// 审核驳回状态值
    /// </summary>
    /// <remarks>
    /// 审核驳回后设置的状态值，如：3（表示驳回）
    /// </remarks>
    [SugarColumn(ColumnDescription = "审核驳回状态值")]
    public int RejectStatusValue { get; set; } = 3;

    /// <summary>
    /// 撤回状态值
    /// </summary>
    /// <remarks>
    /// 发起人撤回后设置的状态值，如：0（表示待提交）
    /// </remarks>
    [SugarColumn(ColumnDescription = "撤回状态值")]
    public int WithdrawStatusValue { get; set; } = 0;

    /// <summary>
    /// 审核页URL
    /// </summary>
    /// <remarks>
    /// 审核详情页面路径，如：/buz/product/edit?id={BusinessId}
    /// 支持 {BusinessId} 占位符
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "审核页URL")]
    public string? AuditPageUrl { get; set; }

    /// <summary>
    /// 审核标题模板
    /// </summary>
    /// <remarks>
    /// 审核标题格式模板，如：商品审核-【{Name}】
    /// 支持 {Name}、{SkuCode} 等占位符，从业务数据 JSON 中取值
    /// </remarks>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "审核标题模板")]
    public string? TitleTemplate { get; set; }

    /// <summary>
    /// 单据编号字段
    /// </summary>
    /// <remarks>
    /// 业务表中的编号字段名，如：SkuCode、OrderNo 等，用于显示审核记录
    /// </remarks>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "单据编号字段")]
    public string? CodeField { get; set; }

    /// <summary>
    /// 审核通过回调API
    /// </summary>
    /// <remarks>
    /// 审核通过时调用的后端API路径，如：/api/product/audit-pass/{BusinessId}
    /// 可为空，使用默认状态更新逻辑
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "审核通过回调API")]
    public string? PassCallbackApi { get; set; }

    /// <summary>
    /// 审核驳回回调API
    /// </summary>
    /// <remarks>
    /// 审核驳回时调用的后端API路径，如：/api/product/audit-reject/{BusinessId}
    /// 可为空，使用默认状态更新逻辑
    /// </remarks>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "审核驳回回调API")]
    public string? RejectCallbackApi { get; set; }

    /// <summary>
    /// 启用状态
    /// </summary>
    /// <remarks>
    /// 1-启用，0-禁用
    /// </remarks>
    [SugarColumn(ColumnDescription = "启用状态：1-启用，0-禁用")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 排序号
    /// </summary>
    [SugarColumn(ColumnDescription = "排序号")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "备注")]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建人ID")]
    public Guid CreatorId { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}