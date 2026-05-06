namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新供应商参数
/// </summary>
public class UpdateSupplierDto : AddSupplierDto
{
    /// <summary>
    /// 供应商ID
    /// </summary>
    public Guid Id { get; set; }
}