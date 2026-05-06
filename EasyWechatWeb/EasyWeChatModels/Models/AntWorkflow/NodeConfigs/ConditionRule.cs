using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Models.NodeConfigs;

/// <summary>
/// 条件规则（基于业务数据字段判断）
/// </summary>
public class ConditionRule
{
    /// <summary>字段ID（指向业务类型字段）</summary>
    public string FieldId { get; set; } = string.Empty;

    /// <summary>字段名称</summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>字段类型</summary>
    public string FieldType { get; set; } = "string";

    /// <summary>操作符</summary>
    public ConditionOperator Operator { get; set; }

    /// <summary>比较值</summary>
    public object? Value { get; set; }
}