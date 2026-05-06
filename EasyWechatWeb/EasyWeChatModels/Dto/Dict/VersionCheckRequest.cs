namespace EasyWeChatModels.Dto;

/// <summary>
/// 版本检查请求
/// </summary>
public class VersionCheckRequest
{
    /// <summary>
    /// 字典编码 -> 本地版本号
    /// </summary>
    public Dictionary<string, int> Versions { get; set; } = new();
}