namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 查询我的流程实例 DTO
/// </summary>
public class QueryMyInstanceDto
{
    /// <summary>页码</summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>每页数量</summary>
    public int PageSize { get; set; } = 10;

    /// <summary>流程名称（模糊查询）</summary>
    public string? WorkflowName { get; set; }

    /// <summary>业务类型</summary>
    public string? BusinessType { get; set; }

    /// <summary>状态</summary>
    public int? Status { get; set; }
}