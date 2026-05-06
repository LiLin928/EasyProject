namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 表单字段DTO
/// </summary>
public class AntFormFieldDto
{
    /// <summary>字段ID</summary>
    public Guid Id { get; set; }

    /// <summary>业务类型编码</summary>
    public string BusinessType { get; set; } = string.Empty;

    /// <summary>字段ID</summary>
    public string FieldId { get; set; } = string.Empty;

    /// <summary>字段名称</summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>字段标签</summary>
    public string FieldLabel { get; set; } = string.Empty;

    /// <summary>字段类型</summary>
    public string FieldType { get; set; } = string.Empty;

    /// <summary>是否必填</summary>
    public int Required { get; set; }

    /// <summary>默认值JSON</summary>
    public string? DefaultValue { get; set; }

    /// <summary>选项配置JSON</summary>
    public string? Options { get; set; }

    /// <summary>占位符</summary>
    public string? Placeholder { get; set; }

    /// <summary>字段顺序</summary>
    public int Order { get; set; }

    /// <summary>创建时间</summary>
    public DateTime CreateTime { get; set; }
}

/// <summary>
/// 创建表单字段DTO
/// </summary>
public class CreateFormFieldDto
{
    /// <summary>业务类型编码</summary>
    public string BusinessType { get; set; } = string.Empty;

    /// <summary>字段ID</summary>
    public string FieldId { get; set; } = string.Empty;

    /// <summary>字段名称</summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>字段标签</summary>
    public string FieldLabel { get; set; } = string.Empty;

    /// <summary>字段类型</summary>
    public string FieldType { get; set; } = string.Empty;

    /// <summary>是否必填</summary>
    public int Required { get; set; }

    /// <summary>默认值JSON</summary>
    public string? DefaultValue { get; set; }

    /// <summary>选项配置JSON</summary>
    public string? Options { get; set; }

    /// <summary>占位符</summary>
    public string? Placeholder { get; set; }

    /// <summary>字段顺序</summary>
    public int Order { get; set; }
}

/// <summary>
/// 更新表单字段DTO
/// </summary>
public class UpdateFormFieldDto
{
    /// <summary>字段ID</summary>
    public Guid Id { get; set; }

    /// <summary>字段名称</summary>
    public string? FieldName { get; set; }

    /// <summary>字段标签</summary>
    public string? FieldLabel { get; set; }

    /// <summary>字段类型</summary>
    public string? FieldType { get; set; }

    /// <summary>是否必填</summary>
    public int? Required { get; set; }

    /// <summary>默认值JSON</summary>
    public string? DefaultValue { get; set; }

    /// <summary>选项配置JSON</summary>
    public string? Options { get; set; }

    /// <summary>占位符</summary>
    public string? Placeholder { get; set; }

    /// <summary>字段顺序</summary>
    public int? Order { get; set; }
}

/// <summary>
/// 字段排序DTO
/// </summary>
public class FormFieldOrderDto
{
    /// <summary>字段ID</summary>
    public Guid Id { get; set; }

    /// <summary>排序</summary>
    public int Order { get; set; }
}