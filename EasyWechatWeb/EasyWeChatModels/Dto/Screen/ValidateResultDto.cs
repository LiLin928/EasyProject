namespace EasyWeChatModels.Dto;

/// <summary>
/// 配置验证结果 DTO
/// </summary>
/// <remarks>
/// 用于返回大屏配置验证的结果
/// </remarks>
public class ValidateResultDto
{
    /// <summary>
    /// 是否有效
    /// </summary>
    /// <example>true</example>
    public bool Valid { get; set; }

    /// <summary>
    /// 错误信息列表
    /// </summary>
    /// <example>["组件宽度不能为负数", "数据源配置无效"]</example>
    public List<string> Errors { get; set; } = new();
}