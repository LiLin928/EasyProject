namespace EasyWeChatModels.Enums;

/// <summary>
/// 条件操作符枚举
/// </summary>
public enum ConditionOperator
{
    /// <summary>等于</summary>
    Eq = 0,
    /// <summary>不等于</summary>
    Ne = 1,
    /// <summary>大于</summary>
    Gt = 2,
    /// <summary>大于等于</summary>
    Gte = 3,
    /// <summary>小于</summary>
    Lt = 4,
    /// <summary>小于等于</summary>
    Lte = 5,
    /// <summary>包含</summary>
    Contains = 6,
    /// <summary>不包含</summary>
    NotContains = 7,
    /// <summary>为空</summary>
    Empty = 8,
    /// <summary>不为空</summary>
    NotEmpty = 9,
    /// <summary>在列表中</summary>
    In = 10,
    /// <summary>不在列表中</summary>
    NotIn = 11
}