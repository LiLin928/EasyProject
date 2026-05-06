using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新数据源请求 DTO
/// </summary>
/// <remarks>
/// 用于更新数据源信息时提交的数据，所有字段均为可选
/// </remarks>
public class UpdateDatasourceDto
{
    /// <summary>
    /// 数据源ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    [Required(ErrorMessage = "数据源ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// 数据源名称
    /// </summary>
    /// <example>生产环境MySQL</example>
    [StringLength(100, ErrorMessage = "名称长度不能超过100")]
    public string? Name { get; set; }

    /// <summary>
    /// 主机地址
    /// </summary>
    /// <example>192.168.1.100</example>
    public string? Host { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    /// <example>3306</example>
    public int? Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    /// <example>easyproject</example>
    public string? Database { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    /// <example>root</example>
    public string? Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    /// <remarks>
    /// 更新时密码可选，不传则保持原密码不变
    /// </remarks>
    /// <example>newpassword123</example>
    public string? Password { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    /// <example>生产环境主数据库</example>
    public string? Description { get; set; }
}