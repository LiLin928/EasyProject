namespace EasyWeChatModels.Dto;

/// <summary>
/// 数据源信息 DTO
/// </summary>
/// <remarks>
/// 用于返回数据源基本信息，不包含敏感信息如密码
/// </remarks>
public class DatasourceDto
{
    /// <summary>
    /// 数据源ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 数据源名称
    /// </summary>
    /// <example>生产环境MySQL</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 数据源类型
    /// </summary>
    /// <remarks>
    /// 支持的类型：mysql、postgresql、sqlserver、oracle
    /// </remarks>
    /// <example>mysql</example>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 主机地址
    /// </summary>
    /// <example>192.168.1.100</example>
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// 端口号
    /// </summary>
    /// <example>3306</example>
    public int Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    /// <example>easyproject</example>
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    /// <example>root</example>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 数据源状态：active-正常，inactive-禁用
    /// </remarks>
    /// <example>active</example>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 最后连接时间
    /// </summary>
    /// <example>2024-01-01 12:00:00</example>
    public DateTime? LastConnectionTime { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <example>生产环境主数据库</example>
    public string? Description { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <example>2024-01-01 00:00:00</example>
    public DateTime? CreateTime { get; set; }
}