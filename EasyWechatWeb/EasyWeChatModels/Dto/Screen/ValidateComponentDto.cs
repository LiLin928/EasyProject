namespace EasyWeChatModels.Dto;

/// <summary>
/// 验证组件请求 DTO
/// </summary>
/// <remarks>
/// 用于验证单个组件配置
/// </remarks>
public class ValidateComponentDto
{
    /// <summary>
    /// 组件配置（JSON格式）
    /// </summary>
    /// <example>{"type":"chart-line","dataSource":{},"config":{}}</example>
    public string? Component { get; set; }
}