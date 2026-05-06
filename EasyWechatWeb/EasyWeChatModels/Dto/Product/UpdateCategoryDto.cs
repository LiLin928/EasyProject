namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新分类参数
/// </summary>
public class UpdateCategoryDto : AddCategoryDto
{
    /// <summary>
    /// 分类ID
    /// </summary>
    public Guid Id { get; set; }
}