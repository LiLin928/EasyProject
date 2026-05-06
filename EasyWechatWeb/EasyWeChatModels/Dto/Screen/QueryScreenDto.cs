using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 大屏列表查询参数 DTO
/// </summary>
/// <remarks>
/// 用于分页查询大屏列表
/// </remarks>
public class QueryScreenDto
{
    /// <summary>
    /// 页码
    /// </summary>
    /// <example>1</example>
    [Range(1, int.MaxValue, ErrorMessage = "页码必须大于0")]
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    /// <example>12</example>
    [Range(1, int.MaxValue, ErrorMessage = "每页数量必须大于0")]
    public int PageSize { get; set; } = 12;

    /// <summary>
    /// 大屏名称（模糊搜索）
    /// </summary>
    /// <example>销售</example>
    public string? Name { get; set; }

    /// <summary>
    /// 是否公开筛选
    /// </summary>
    /// <remarks>
    /// 不传则查询全部，0-私有，1-公开
    /// </remarks>
    /// <example>1</example>
    public int? IsPublic { get; set; }
}