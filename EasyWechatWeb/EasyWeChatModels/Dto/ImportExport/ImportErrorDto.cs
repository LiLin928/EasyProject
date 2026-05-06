using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 导入错误详情 DTO
/// </summary>
/// <remarks>
/// 用于记录单条记录导入失败的详细信息。
/// 包括错误发生的行号、字段名和具体错误消息。
/// 用于精确定位导入问题。
/// </remarks>
public class ImportErrorDto
{
    /// <summary>
    /// 错误发生的 Excel 行号
    /// </summary>
    /// <remarks>
    /// Excel 文件中的行号（从数据行开始计数，不含标题行）。
    /// 用于定位错误数据在文件中的位置。
    /// </remarks>
    /// <example>10</example>
    public int RowNumber { get; set; }

    /// <summary>
    /// 发生错误的字段名
    /// </summary>
    /// <remarks>
    /// 出现问题的字段名称，对应 Excel 列标题。
    /// 可为空表示整行数据有问题。
    /// </remarks>
    /// <example>UserName</example>
    public string? FieldName { get; set; }

    /// <summary>
    /// 字段的原始值
    /// </summary>
    /// <remarks>
    /// Excel 中该字段的原值。
    /// 用于帮助用户识别具体的数据问题。
    /// </remarks>
    /// <example>admin</example>
    public string? FieldValue { get; set; }

    /// <summary>
    /// 错误消息
    /// </summary>
    /// <remarks>
    /// 具体的错误描述信息。
    /// 说明为什么该数据导入失败。
    /// </remarks>
    /// <example>用户名已存在</example>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 错误类型
    /// </summary>
    /// <remarks>
    /// 错误的类型分类，如验证错误、格式错误、数据库错误等。
    /// 用于错误分类处理和统计。
    /// </remarks>
    /// <example>ImportErrorType.Validation</example>
    public ImportErrorType ErrorType { get; set; }
}