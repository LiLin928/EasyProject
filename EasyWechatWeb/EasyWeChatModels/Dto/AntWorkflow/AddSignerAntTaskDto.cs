namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 加签请求DTO
/// </summary>
public class AddSignerAntTaskDto
{
    /// <summary>当前任务ID</summary>
    public Guid TaskId { get; set; }

    /// <summary>加签用户ID列表</summary>
    public List<Guid> SignerIds { get; set; } = new();

    /// <summary>加签类型：before（前加签）/ after（后加签）</summary>
    public string SignerType { get; set; } = "before";

    /// <summary>加签原因</summary>
    public string? Reason { get; set; }
}