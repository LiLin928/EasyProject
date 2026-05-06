namespace EasyWeChatModels.Dto;

/// <summary>
/// 业务审核点 DTO
/// </summary>
public class BusinessAuditPointDto
{
    /// <summary>
    /// 审核点ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 审核点编码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 审核点名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 审核点分类
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 关联流程定义ID
    /// </summary>
    public Guid WorkflowId { get; set; }

    /// <summary>
    /// 关联流程名称
    /// </summary>
    public string? WorkflowName { get; set; }

    /// <summary>
    /// 处理表名
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// 主键字段名
    /// </summary>
    public string PrimaryKeyField { get; set; } = "Id";

    /// <summary>
    /// 状态字段名
    /// </summary>
    public string StatusField { get; set; } = "Status";

    /// <summary>
    /// 待审核状态值
    /// </summary>
    public int AuditStatusValue { get; set; } = 2;

    /// <summary>
    /// 审核通过状态值
    /// </summary>
    public int PassStatusValue { get; set; } = 1;

    /// <summary>
    /// 审核驳回状态值
    /// </summary>
    public int RejectStatusValue { get; set; } = 3;

    /// <summary>
    /// 撤回状态值
    /// </summary>
    public int WithdrawStatusValue { get; set; } = 0;

    /// <summary>
    /// 审核页URL
    /// </summary>
    public string? AuditPageUrl { get; set; }

    /// <summary>
    /// 审核标题模板
    /// </summary>
    public string? TitleTemplate { get; set; }

    /// <summary>
    /// 单据编号字段
    /// </summary>
    public string? CodeField { get; set; }

    /// <summary>
    /// 审核通过回调API
    /// </summary>
    public string? PassCallbackApi { get; set; }

    /// <summary>
    /// 审核驳回回调API
    /// </summary>
    public string? RejectCallbackApi { get; set; }

    /// <summary>
    /// 启用状态：1-启用，0-禁用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string? CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdateTime { get; set; }
}