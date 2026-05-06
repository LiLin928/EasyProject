using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 微信商品服务接口
/// </summary>
public interface IWeChatProductService
{
    /// <summary>
    /// 获取商品列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>商品分页列表</returns>
    Task<PageResponse<ProductDto>> GetProductListAsync(QueryProductDto query);

    /// <summary>
    /// 获取商品详情
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>商品详情</returns>
    Task<ProductDto?> GetProductByIdAsync(Guid id);

    /// <summary>
    /// 获取分类列表
    /// </summary>
    /// <returns>分类列表</returns>
    Task<List<CategoryDto>> GetCategoriesAsync();

    /// <summary>
    /// 搜索商品
    /// </summary>
    /// <param name="keyword">关键字</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>商品分页列表</returns>
    Task<PageResponse<ProductDto>> SearchProductsAsync(string keyword, int pageIndex, int pageSize);
}