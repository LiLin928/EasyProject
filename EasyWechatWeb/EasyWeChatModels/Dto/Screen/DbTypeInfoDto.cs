namespace EasyWeChatModels.Dto;

/// <summary>
/// 数据库类型信息 DTO
/// </summary>
/// <remarks>
/// 用于返回支持的数据库类型信息
/// </remarks>
public class DbTypeInfoDto
{
    /// <summary>
    /// 数据库类型代码
    /// </summary>
    /// <example>mysql</example>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 数据库类型名称
    /// </summary>
    /// <example>MySQL</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 默认端口
    /// </summary>
    /// <example>3306</example>
    public int DefaultPort { get; set; }
}