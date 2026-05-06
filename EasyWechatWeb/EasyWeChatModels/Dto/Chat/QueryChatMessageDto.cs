namespace EasyWeChatModels.Dto;

/// <summary>
/// 消息查询 DTO
/// </summary>
public class QueryChatMessageDto
{
    /// <summary>
    /// 会话ID
    /// </summary>
    public Guid SessionId { get; set; }

    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 20;
}