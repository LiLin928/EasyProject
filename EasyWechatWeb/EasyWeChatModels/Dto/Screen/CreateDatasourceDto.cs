using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建数据源请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新数据源时提交的信息
/// </remarks>
public class CreateDatasourceDto
{
    /// <summary>
    /// 数据源名称
    /// </summary>
    /// <remarks>
    /// 名称必须唯一，长度限制100字符
    /// </remarks>
    /// <example>生产环境MySQL</example>
    [Required(ErrorMessage = "数据源名称不能为空")]
    [StringLength(100, ErrorMessage = "名称长度不能超过100")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 数据源类型
    /// </summary>
    /// <remarks>
    /// 支持的类型：mysql、postgresql、sqlserver、oracle
    /// </remarks>
    /// <example>mysql</example>
    [Required(ErrorMessage = "数据源类型不能为空")]
    public string Type { get; set; } = "mysql";

    /// <summary>
    /// 主机地址
    /// </summary>
    /// <example>192.168.1.100</example>
    [Required(ErrorMessage = "主机地址不能为空")]
    public string Host { get; set; } = string.Empty;

    /// <summary>
    /// 端口号
    /// </summary>
    /// <example>3306</example>
    [Required(ErrorMessage = "端口不能为空")]
    public int Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    /// <example>easyproject</example>
    [Required(ErrorMessage = "数据库名称不能为空")]
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// 用户名
    /// </summary>
    /// <example>root</example>
    [Required(ErrorMessage = "用户名不能为空")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    /// <remarks>
    /// 数据库连接密码，将进行加密存储
    /// </remarks>
    /// <example>password123</example>
    [Required(ErrorMessage = "密码不能为空")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    /// <example>生产环境主数据库</example>
    public string? Description { get; set; }
}