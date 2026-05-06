namespace EasyWeChatModels.Options;

/// <summary>
/// 微信小程序配置选项
/// </summary>
public class WeChatOptions
{
    /// <summary>
    /// 小程序 AppID
    /// </summary>
    public string AppId { get; set; } = string.Empty;

    /// <summary>
    /// 小程序 AppSecret
    /// </summary>
    public string AppSecret { get; set; } = string.Empty;

    /// <summary>
    /// 是否使用 Mock 模式
    /// </summary>
    public bool UseMock { get; set; } = true;
}