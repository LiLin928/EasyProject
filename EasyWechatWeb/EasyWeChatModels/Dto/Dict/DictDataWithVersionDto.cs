namespace EasyWeChatModels.Dto;

/// <summary>
/// 字典数据项（含版本）
/// </summary>
public class DictDataWithVersionDto
{
    /// <summary>
    /// 版本号
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 字典数据列表
    /// </summary>
    public List<DictDataItemDto> Items { get; set; } = new();
}