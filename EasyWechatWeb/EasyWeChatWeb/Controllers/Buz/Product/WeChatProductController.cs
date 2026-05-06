using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.WeChatPro;

/// <summary>
/// 微信商品控制器
/// 提供商品列表、详情、分类和搜索功能
/// </summary>
[ApiController]
[Route("api/wechat/product")]
[Authorize]
public class WeChatProductController : BaseController
{
    /// <summary>
    /// 微信商品服务接口
    /// </summary>
    public IWeChatProductService _productService { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<WeChatProductController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取商品列表（分页）
    /// </summary>
    /// <param name="query">查询参数，包含分页、分类、筛选条件等</param>
    /// <returns>商品分页列表</returns>
    /// <response code="200">成功获取商品列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 支持按分类、关键字、价格区间、热销、新品等条件筛选。
    /// 支持按价格、销量、创建时间等字段排序。
    /// </remarks>
    /// <example>
    /// POST /api/wechat/product/list
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "pageIndex": 1,
    ///     "pageSize": 10,
    ///     "categoryId": "xxx",
    ///     "isHot": true
    /// }
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<ProductDto>>), 200)]
    public async Task<ApiResponse<PageResponse<ProductDto>>> GetList([FromBody] QueryProductDto query)
    {
        try
        {
            var result = await _productService.GetProductListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取商品列表失败");
            return Error<PageResponse<ProductDto>>("获取商品列表失败");
        }
    }

    /// <summary>
    /// 获取商品详情
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>商品详细信息</returns>
    /// <response code="200">成功获取商品详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">商品不存在</response>
    /// <remarks>
    /// 获取商品的详细信息，包括名称、价格、库存、详情图、详情富文本等。
    /// </remarks>
    /// <example>
    /// GET /api/wechat/product/{id}
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
    public async Task<ApiResponse<ProductDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result == null)
            {
                return Error<ProductDto>("商品不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取商品详情失败: {Id}", id);
            return Error<ProductDto>("获取商品详情失败");
        }
    }

    /// <summary>
    /// 获取分类列表
    /// </summary>
    /// <returns>分类列表</returns>
    /// <response code="200">成功获取分类列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取所有商品分类，用于商品筛选和分类展示。
    /// </remarks>
    /// <example>
    /// GET /api/wechat/product/categories
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryDto>>), 200)]
    public async Task<ApiResponse<List<CategoryDto>>> GetCategories()
    {
        try
        {
            var result = await _productService.GetCategoriesAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取分类列表失败");
            return Error<List<CategoryDto>>("获取分类列表失败");
        }
    }

    /// <summary>
    /// 商品搜索
    /// </summary>
    /// <param name="keyword">搜索关键字</param>
    /// <param name="pageIndex">页码，从1开始，默认为1</param>
    /// <param name="pageSize">每页数量，默认为10</param>
    /// <returns>商品分页列表</returns>
    /// <response code="200">成功搜索商品</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 通过关键字搜索商品，搜索范围包括商品名称和描述。
    /// </remarks>
    /// <example>
    /// GET /api/wechat/product/search?keyword=手机&amp;pageIndex=1&amp;pageSize=10
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<ProductDto>>), 200)]
    public async Task<ApiResponse<PageResponse<ProductDto>>> Search(
        [FromQuery] string keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Error<PageResponse<ProductDto>>("搜索关键字不能为空", 400);
            }

            var result = await _productService.SearchProductsAsync(keyword, pageIndex, pageSize);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "商品搜索失败: {Keyword}", keyword);
            return Error<PageResponse<ProductDto>>("商品搜索失败");
        }
    }
}