using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新业务审核点 DTO
/// </summary>
public class UpdateBusinessAuditPointDto
{
    /// <summary>
    /// 审核点ID
    /// </summary>
    [Required(ErrorMessage = "审核点ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// 审核点编码
    /// </summary>
    [Required(ErrorMessage = "审核点编码不能为空")]
    [MaxLength(50, ErrorMessage = "审核点编码不能超过50个字符")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 审核点名称
    /// </summary>
    [Required(ErrorMessage = "审核点名称不能为空")]
    [MaxLength(100, ErrorMessage = "审核点名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 审核点分类
    /// </summary>
    [MaxLength(200)]
    public string? Category { get; set; }

    /// <summary>
    /// 关联流程定义ID
    /// </summary>
    [Required(ErrorMessage = "关联流程不能为空")]
    public Guid WorkflowId { get; set; }

    /// <summary>
    /// 处理表名
    /// </summary>
    [Required(ErrorMessage = "处理表名不能为空")]
    [MaxLength(50, ErrorMessage = "处理表名不能超过50个字符")]
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// 主键字段名
    /// </summary>
    [MaxLength(50, ErrorMessage = "主键字段名不能超过50个字符")]
    public string PrimaryKeyField { get; set; } = "Id";

    /// <summary>
    /// 状态字段名
    /// </summary>
    [MaxLength(50)]
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
    [MaxLength(500)]
    public string? AuditPageUrl { get; set; }

    /// <summary>
    /// 审核标题模板
    /// </summary>
    [MaxLength(200)]
    public string? TitleTemplate { get; set; }

    /// <summary>
    /// 单据编号字段
    /// </summary>
    [MaxLength(50)]
    public string? CodeField { get; set; }

    /// <summary>
    /// 审核通过回调API
    /// </summary>
    [MaxLength(500)]
    public string? PassCallbackApi { get; set; }

    /// <summary>
    /// 审核驳回回调API
    /// </summary>
    [MaxLength(500)]
    public string? RejectCallbackApi { get; set; }

    /// <summary>
    /// 启用状态：1-启用，0-禁用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 备注
    /// </summary>
    [MaxLength(500)]
    public string? Remark { get; set; }
}