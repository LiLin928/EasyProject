using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 商品规格服务接口
/// </summary>
/// <remarks>
/// 提供商品规格和SKU管理功能
/// </remarks>
public interface IProductSpecService
{
    #region 规格管理

    /// <summary>
    /// 获取商品规格列表
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>规格列表（含选项）</returns>
    Task<List<ProductSpecDto>> GetSpecListAsync(Guid productId);

    /// <summary>
    /// 获取规格详情
    /// </summary>
    /// <param name="id">规格ID</param>
    /// <returns>规格详情</returns>
    Task<ProductSpecDto?> GetSpecByIdAsync(Guid id);

    /// <summary>
    /// 创建规格
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的规格ID</returns>
    Task<Guid> CreateSpecAsync(AddProductSpecDto dto);

    /// <summary>
    /// 更新规格
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateSpecAsync(UpdateProductSpecDto dto);

    /// <summary>
    /// 删除规格
    /// </summary>
    /// <param name="id">规格ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteSpecAsync(Guid id);

    /// <summary>
    /// 添加规格选项
    /// </summary>
    /// <param name="specId">规格ID</param>
    /// <param name="option">选项参数</param>
    /// <returns>新创建的选项ID</returns>
    Task<Guid> AddSpecOptionAsync(Guid specId, AddSpecOptionDto option);

    /// <summary>
    /// 删除规格选项
    /// </summary>
    /// <param name="optionId">选项ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteSpecOptionAsync(Guid optionId);

    #endregion

    #region SKU管理

    /// <summary>
    /// 获取商品SKU列表
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>SKU列表</returns>
    Task<List<ProductSkuDto>> GetSkuListAsync(Guid productId);

    /// <summary>
    /// 获取SKU详情
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <returns>SKU详情</returns>
    Task<ProductSkuDto?> GetSkuByIdAsync(Guid id);

    /// <summary>
    /// 保存SKU（创建或更新）
    /// </summary>
    /// <param name="dto">保存参数</param>
    /// <returns>SKU ID</returns>
    Task<Guid> SaveSkuAsync(SaveProductSkuDto dto);

    /// <summary>
    /// 删除SKU
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteSkuAsync(Guid id);

    /// <summary>
    /// 批量生成SKU
    /// </summary>
    /// <param name="dto">生成参数</param>
    /// <returns>生成的SKU数量</returns>
    Task<int> GenerateSkusAsync(GenerateSkuDto dto);

    /// <summary>
    /// 更新SKU库存
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <param name="stock">库存变化量（正数增加，负数减少）</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateSkuStockAsync(Guid id, int stock);

    #endregion
}