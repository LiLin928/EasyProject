namespace EasyWeChatModels.Dto.AntWorkflow;

/// <summary>
/// 查询 Ant 流程列表 DTO
/// </summary>
public class QueryAntWorkflowDto
{
    /// <summary>页码</summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>每页数量</summary>
    public int PageSize { get; set; } = 10;

    /// <summary>流程名称（模糊查询）</summary>
    public string? Name { get; set; }

    /// <summary>流程编码</summary>
    public string? Code { get; set; }

    /// <summary>分类编码</summary>
    public string? CategoryCode { get; set; }

    /// <summary>状态</summary>
    public int? Status { get; set; }
}