using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 文件查询 DTO
/// </summary>
/// <remarks>
/// 用于查询文件记录列表时的筛选条件和分页参数。
/// </remarks>
public class QueryFileDto
{
    /// <summary>
    /// 页码索引，从1开始
    /// </summary>
    /// <remarks>
    /// 分页查询的页码，默认为第1页。
    /// </remarks>
    /// <example>1</example>
    [Required(ErrorMessage = "页码不能为空")]
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    /// <remarks>
    /// 分页查询每页显示的记录数，默认为10条。
    /// 最大建议不超过100。
    /// </remarks>
    /// <example>10</example>
    [Required(ErrorMessage = "每页数量不能为空")]
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 上传用户ID
    /// </summary>
    /// <remarks>
    /// 按上传用户ID筛选，可选参数。
    /// </remarks>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 业务ID
    /// </summary>
    /// <remarks>
    /// 按业务ID筛选，可选参数。
    /// 用于查询特定业务关联的所有文件。
    /// </remarks>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid? BusinessId { get; set; }

    /// <summary>
    /// 文件扩展名
    /// </summary>
    /// <remarks>
    /// 按文件扩展名筛选，可选参数。
    /// </remarks>
    /// <example>pdf</example>
    public string? FileExt { get; set; }

    /// <summary>
    /// 文件名关键字
    /// </summary>
    /// <remarks>
    /// 按文件名模糊搜索，可选参数。
    /// </remarks>
    /// <example>report</example>
    public string? FileName { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 按状态筛选，可选参数。
    /// 1-正常，0-已删除。不传则默认查询正常文件。
    /// </remarks>
    /// <example>1</example>
    public int? Status { get; set; }
}