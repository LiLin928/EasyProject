using System.Collections.Generic;

namespace EasyWeChatModels.Dto.LogQuery;

/// <summary>
/// 日志查询响应结果
/// </summary>
public class LogQueryResponseDto
{
    /// <summary>
    /// 总记录数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 日志条目列表
    /// </summary>
    public List<LogEntryDto> Items { get; set; } = new();

    /// <summary>
    /// 实际查询的索引列表（用于调试）
    /// </summary>
    public List<string> QueriedIndices { get; set; } = new();
}