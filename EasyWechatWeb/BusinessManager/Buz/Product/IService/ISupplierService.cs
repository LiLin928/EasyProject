using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 供应商服务接口
/// </summary>
/// <remarks>
/// 提供供应商管理、商品供应商关联等功能
/// </remarks>
public interface ISupplierService
{
    #region 供应商管理

    /// <summary>
    /// 获取供应商分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页供应商列表</returns>
    Task<PageResponse<SupplierDto>> GetPageListAsync(QuerySupplierDto query);

    /// <summary>
    /// 根据ID获取供应商详情
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <returns>供应商信息</returns>
    Task<SupplierDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 添加供应商
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的供应商ID</returns>
    Task<Guid> AddAsync(AddSupplierDto dto);

    /// <summary>
    /// 更新供应商
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateSupplierDto dto);

    /// <summary>
    /// 删除供应商
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    #endregion

    #region 商品供应商关联

    /// <summary>
    /// 获取商品的供应商列表
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>商品供应商关联列表</returns>
    Task<List<ProductSupplierDto>> GetProductSuppliersAsync(Guid productId);

    /// <summary>
    /// 绑定商品供应商
    /// </summary>
    /// <param name="dto">绑定参数</param>
    /// <returns>新创建的关联ID</returns>
    Task<Guid> BindProductSupplierAsync(BindProductSupplierDto dto);

    /// <summary>
    /// 解绑商品供应商
    /// </summary>
    /// <param name="id">关联ID</param>
    /// <returns>影响的行数</returns>
    Task<int> UnbindProductSupplierAsync(Guid id);

    /// <summary>
    /// 设置默认供应商
    /// </summary>
    /// <param name="id">商品供应商关联ID</param>
    /// <returns>影响的行数</returns>
    Task<int> SetDefaultSupplierAsync(Guid id);

    /// <summary>
    /// 获取供应商的商品列表（含绑定信息）
    /// </summary>
    /// <param name="supplierId">供应商ID</param>
    /// <returns>商品供应商关联列表</returns>
    Task<List<ProductSupplierDto>> GetSupplierProductsAsync(Guid supplierId);

    #endregion
}