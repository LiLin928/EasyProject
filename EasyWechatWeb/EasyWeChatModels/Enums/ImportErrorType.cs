namespace EasyWeChatModels.Enums;

/// <summary>
/// 导入错误类型枚举
/// </summary>
/// <remarks>
/// 用于分类导入过程中出现的不同类型错误。
/// 方便错误统计和针对性处理。
/// </remarks>
public enum ImportErrorType
{
    /// <summary>
    /// 数据验证错误
    /// </summary>
    /// <remarks>
    /// 数据不符合业务规则或格式要求。
    /// 如必填字段为空、格式不正确、超出长度限制等。
    /// </remarks>
    Validation = 1,

    /// <summary>
    /// 数据重复错误
    /// </summary>
    /// <remarks>
    /// 数据在系统中已存在，违反唯一性约束。
    /// 如用户名已存在、角色编码已存在等。
    /// </remarks>
    Duplicate = 2,

    /// <summary>
    /// 数据格式错误
    /// </summary>
    /// <remarks>
    /// Excel 数据格式与目标类型不匹配。
    /// 如日期格式错误、数值格式错误等。
    /// </remarks>
    Format = 3,

    /// <summary>
    /// 数据库错误
    /// </summary>
    /// <remarks>
    /// 数据库操作过程中出现的错误。
    /// 如插入失败、外键约束错误等。
    /// </remarks>
    Database = 4,

    /// <summary>
    /// 其他错误
    /// </summary>
    /// <remarks>
    /// 未分类的其他类型错误。
    /// 用于兜底处理未知错误类型。
    /// </remarks>
    Other = 5
}