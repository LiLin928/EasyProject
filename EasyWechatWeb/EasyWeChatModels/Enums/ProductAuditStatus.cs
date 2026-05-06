namespace EasyWeChatModels.Enums;

/// <summary>
/// 商品审核状态枚举
/// </summary>
/// <remarks>
/// 用于标识商品在不同审核流程阶段的状态。
/// 与 AntWorkflow 工作流引擎集成，支持自定义审核流程。
/// </remarks>
public enum ProductAuditStatus
{
    /// <summary>
    /// 待提交
    /// </summary>
    /// <remarks>
    /// 商品信息已填写，但尚未提交审核。
    /// 用户可以继续编辑商品信息。
    /// </remarks>
    WaitSubmit = 0,

    /// <summary>
    /// 待审核
    /// </summary>
    /// <remarks>
    /// 商品已提交审核，等待审核人员处理。
    /// 此状态下商品信息不可修改。
    /// </remarks>
    Pending = 1,

    /// <summary>
    /// 已驳回
    /// </summary>
    /// <remarks>
    /// 审核未通过，商品被退回。
    /// 用户可以根据审核意见修改后重新提交。
    /// </remarks>
    Rejected = 2,

    /// <summary>
    /// 已通过
    /// </summary>
    /// <remarks>
    /// 审核通过，商品可以上架销售。
    /// 商品状态可从下架改为上架。
    /// </remarks>
    Passed = 3,

    /// <summary>
    /// 已撤回
    /// </summary>
    /// <remarks>
    /// 用户主动撤回审核申请。
    /// 商品回到待提交状态，可重新编辑。
    /// </remarks>
    Withdrawn = 4
}