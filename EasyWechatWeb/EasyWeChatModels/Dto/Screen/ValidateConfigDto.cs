namespace EasyWeChatModels.Dto;

/// <summary>
/// 验证配置请求 DTO
/// </summary>
/// <remarks>
/// 用于验证大屏组件配置是否有效
/// </remarks>
public class ValidateConfigDto
{
    /// <summary>
    /// 待验证的组件列表
    /// </summary>
    public List<ValidateComponentDto>? Components { get; set; }
}