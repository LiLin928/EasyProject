namespace EasyWeChatModels.Dto;

/// <summary>
/// 测试连接结果 DTO
/// </summary>
/// <remarks>
/// 用于返回数据源连接测试的结果信息
/// </remarks>
public class TestConnectionResultDto
{
    /// <summary>
    /// 是否连接成功
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }

    /// <summary>
    /// 连接结果消息
    /// </summary>
    /// <example>连接成功</example>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 数据库服务器版本
    /// </summary>
    /// <example>8.0.33</example>
    public string? ServerVersion { get; set; }

    /// <summary>
    /// 连接耗时（毫秒）
    /// </summary>
    /// <example>156</example>
    public int? ConnectionTime { get; set; }
}