namespace EasyWeChatModels.Dto;

/// <summary>
/// 版本检查响应
/// </summary>
public class VersionCheckResponse
{
    /// <summary>
    /// 需要刷新的字典编码列表
    /// </summary>
    public List<string> NeedRefresh { get; set; } = new();

    /// <summary>
    /// 所有字典的最新版本
    /// </summary>
    public Dictionary<string, int> AllVersions { get; set; } = new();
}