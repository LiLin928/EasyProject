namespace EasyWeChatModels.Dto;

/// <summary>
/// 查询业务审核点列表 DTO
/// </summary>
public class QueryBusinessAuditPointDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 审核点编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 审核点名称（模糊查询）
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 审核点分类
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 处理表名
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// 启用状态
    /// </summary>
    public int? Status { get; set; }
}