namespace EasyWeChatModels.Dto;

/// <summary>
/// 批量获取字典数据请求
/// </summary>
public class BatchDictDataRequest
{
    /// <summary>
    /// 字典编码列表
    /// </summary>
    public List<string> Codes { get; set; } = new();
}