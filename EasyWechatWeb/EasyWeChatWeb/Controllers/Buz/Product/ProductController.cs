using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 商品控制器
/// </summary>
/// <remarks>
/// 提供商品管理、分类管理、库存管理、统计报表等接口
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : BaseController
{
    /// <summary>
    /// 商品服务接口
    /// </summary>
    public IProductService _productService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<ProductController> _logger { get; set; } = null!;

    #region 商品管理

    /// <summary>
    /// 获取商品列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页商品列表</returns>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<ProductDto>>), 200)]
    public async Task<ApiResponse<PageResponse<ProductDto>>> GetList([FromBody] QueryProductDto query)
    {
        try
        {
            var result = await _productService.GetPageListAsync(query);
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
    /// <returns>商品详情</returns>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductDto>), 200)]
    public async Task<ApiResponse<ProductDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _productService.GetByIdAsync(id);
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
    /// 添加商品
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的商品ID</returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddProductDto dto)
    {
        try
        {
            var id = await _productService.AddAsync(dto);
            return Success(id, "添加成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加商品失败");
            return Error<Guid>("添加商品失败");
        }
    }

    /// <summary>
    /// 更新商品
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateProductDto dto)
    {
        try
        {
            var result = await _productService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新商品失败: {Id}", dto.Id);
            return Error<int>("更新商品失败");
        }
    }

    /// <summary>
    /// 删除商品
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _productService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除商品失败: {Id}", id);
            return Error<int>("删除商品失败");
        }
    }

    /// <summary>
    /// 批量删除商品
    /// </summary>
    /// <param name="ids">商品ID列表</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete-batch")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteBatch([FromBody] List<Guid> ids)
    {
        try
        {
            var result = await _productService.DeleteBatchAsync(ids);
            return Success(result, "批量删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除商品失败");
            return Error<int>("批量删除商品失败");
        }
    }

    /// <summary>
    /// 批量更新商品状态
    /// </summary>
    /// <param name="dto">批量更新参数</param>
    /// <returns>影响的数量</returns>
    [HttpPut("batch-status")]
    [ProducesResponseType(typeof(ApiResponse<BatchOperationResponseDto>), 200)]
    public async Task<ApiResponse<BatchOperationResponseDto>> BatchUpdateStatus([FromBody] BatchUpdateStatusDto dto)
    {
        try
        {
            var count = await _productService.BatchUpdateStatusAsync(dto.Ids, dto.Status);
            return Success(new BatchOperationResponseDto { Count = count }, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量更新商品状态失败");
            return Error<BatchOperationResponseDto>("批量更新商品状态失败");
        }
    }

    /// <summary>
    /// 批量更新商品分类
    /// </summary>
    /// <param name="dto">批量更新参数</param>
    /// <returns>影响的数量</returns>
    [HttpPut("batch-category")]
    [ProducesResponseType(typeof(ApiResponse<BatchOperationResponseDto>), 200)]
    public async Task<ApiResponse<BatchOperationResponseDto>> BatchUpdateCategory([FromBody] BatchUpdateCategoryDto dto)
    {
        try
        {
            var count = await _productService.BatchUpdateCategoryAsync(dto.Ids, dto.CategoryId);
            return Success(new BatchOperationResponseDto { Count = count }, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量更新商品分类失败");
            return Error<BatchOperationResponseDto>("批量更新商品分类失败");
        }
    }

    /// <summary>
    /// 批量更新商品标签
    /// </summary>
    /// <param name="dto">批量更新参数</param>
    /// <returns>影响的数量</returns>
    [HttpPut("batch-tag")]
    [ProducesResponseType(typeof(ApiResponse<BatchOperationResponseDto>), 200)]
    public async Task<ApiResponse<BatchOperationResponseDto>> BatchUpdateTag([FromBody] BatchUpdateTagDto dto)
    {
        try
        {
            var count = await _productService.BatchUpdateTagAsync(dto.Ids, dto.IsHot, dto.IsNew);
            return Success(new BatchOperationResponseDto { Count = count }, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量更新商品标签失败");
            return Error<BatchOperationResponseDto>("批量更新商品标签失败");
        }
    }

    /// <summary>
    /// 批量创建商品
    /// </summary>
    /// <param name="dto">批量创建参数</param>
    /// <returns>新创建的商品ID列表和数量</returns>
    [HttpPost("batch-create")]
    [ProducesResponseType(typeof(ApiResponse<BatchCreateProductResponseDto>), 200)]
    public async Task<ApiResponse<BatchCreateProductResponseDto>> BatchCreate([FromBody] BatchCreateProductDto dto)
    {
        try
        {
            var ids = await _productService.BatchAddAsync(dto.Products);
            var response = new BatchCreateProductResponseDto
            {
                Count = ids.Count,
                Ids = ids
            };
            return Success(response, "创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量创建商品失败");
            return Error<BatchCreateProductResponseDto>("批量创建商品失败");
        }
    }

    #endregion

    #region 分类管理

    /// <summary>
    /// 获取分类树形列表
    /// </summary>
    /// <returns>分类树形列表</returns>
    [HttpPost("category/list")]
    [ProducesResponseType(typeof(ApiResponse<List<CategoryDto>>), 200)]
    public async Task<ApiResponse<List<CategoryDto>>> GetCategoryList()
    {
        try
        {
            var result = await _productService.GetCategoryTreeAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取分类列表失败");
            return Error<List<CategoryDto>>("获取分类列表失败");
        }
    }

    /// <summary>
    /// 获取分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>分类详情</returns>
    [HttpGet("category/detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<CategoryDto>), 200)]
    public async Task<ApiResponse<CategoryDto>> GetCategoryDetail(Guid id)
    {
        try
        {
            var result = await _productService.GetCategoryByIdAsync(id);
            if (result == null)
            {
                return Error<CategoryDto>("分类不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取分类详情失败: {Id}", id);
            return Error<CategoryDto>("获取分类详情失败");
        }
    }

    /// <summary>
    /// 添加分类
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的分类ID</returns>
    [HttpPost("category/add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> AddCategory([FromBody] AddCategoryDto dto)
    {
        try
        {
            var id = await _productService.AddCategoryAsync(dto);
            return Success(id, "添加成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加分类失败");
            return Error<Guid>("添加分类失败");
        }
    }

    /// <summary>
    /// 更新分类
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("category/update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateCategory([FromBody] UpdateCategoryDto dto)
    {
        try
        {
            var result = await _productService.UpdateCategoryAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新分类失败: {Id}", dto.Id);
            return Error<int>("更新分类失败");
        }
    }

    /// <summary>
    /// 删除分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("category/delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteCategory([FromBody] Guid id)
    {
        try
        {
            var result = await _productService.DeleteCategoryAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除分类失败: {Id}", id);
            return Error<int>("删除分类失败");
        }
    }

    #endregion

    #region 库存管理

    /// <summary>
    /// 获取库存列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页库存列表</returns>
    [HttpPost("stock/list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<StockDto>>), 200)]
    public async Task<ApiResponse<PageResponse<StockDto>>> GetStockList([FromBody] QueryStockDto query)
    {
        try
        {
            var result = await _productService.GetStockListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取库存列表失败");
            return Error<PageResponse<StockDto>>("获取库存列表失败");
        }
    }

    /// <summary>
    /// 库存调整
    /// </summary>
    /// <param name="dto">调整参数</param>
    /// <returns>影响的行数</returns>
    [HttpPost("stock/adjust")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> AdjustStock([FromBody] StockAdjustDto dto)
    {
        try
        {
            var result = await _productService.AdjustStockAsync(dto);
            return Success(result, "调整成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "库存调整失败");
            return Error<int>("库存调整失败");
        }
    }

    /// <summary>
    /// 获取库存记录
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页库存记录列表</returns>
    [HttpPost("stock/record")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<StockRecordDto>>), 200)]
    public async Task<ApiResponse<PageResponse<StockRecordDto>>> GetStockRecord([FromBody] QueryStockRecordDto query)
    {
        try
        {
            var result = await _productService.GetStockRecordListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取库存记录失败");
            return Error<PageResponse<StockRecordDto>>("获取库存记录失败");
        }
    }

    /// <summary>
    /// 获取库存预警列表
    /// </summary>
    /// <returns>低库存商品列表</returns>
    [HttpPost("stock/alert")]
    [ProducesResponseType(typeof(ApiResponse<List<StockDto>>), 200)]
    public async Task<ApiResponse<List<StockDto>>> GetStockAlert()
    {
        try
        {
            var result = await _productService.GetLowStockListAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取库存预警列表失败");
            return Error<List<StockDto>>("获取库存预警列表失败");
        }
    }

    /// <summary>
    /// 库存入库
    /// </summary>
    /// <param name="dto">入库参数</param>
    /// <returns>影响的行数</returns>
    [HttpPost("stock/in")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> StockIn([FromBody] StockInDto dto)
    {
        try
        {
            var result = await _productService.StockInAsync(dto);
            return Success(result, "入库成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "库存入库失败");
            return Error<int>("库存入库失败");
        }
    }

    #endregion

    #region 统计报表

    /// <summary>
    /// 获取销量统计
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>销量统计列表</returns>
    [HttpGet("stats/sales")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductSalesStatsDto>>), 200)]
    public async Task<ApiResponse<List<ProductSalesStatsDto>>> GetSalesStats([FromQuery] ProductStatsQueryDto query)
    {
        try
        {
            var result = await _productService.GetSalesStatsAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取销量统计失败");
            return Error<List<ProductSalesStatsDto>>("获取销量统计失败");
        }
    }

    /// <summary>
    /// 获取销量趋势
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>销量趋势列表</returns>
    [HttpGet("stats/trend")]
    [ProducesResponseType(typeof(ApiResponse<List<SalesTrendDto>>), 200)]
    public async Task<ApiResponse<List<SalesTrendDto>>> GetSalesTrend([FromQuery] ProductStatsQueryDto query)
    {
        try
        {
            var result = await _productService.GetSalesTrendAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取销量趋势失败");
            return Error<List<SalesTrendDto>>("获取销量趋势失败");
        }
    }

    /// <summary>
    /// 获取库存统计
    /// </summary>
    /// <returns>库存统计信息</returns>
    [HttpPost("stats/stock")]
    [ProducesResponseType(typeof(ApiResponse<StockStatisticsDto>), 200)]
    public async Task<ApiResponse<StockStatisticsDto>> GetStockStatistics()
    {
        try
        {
            var result = await _productService.GetStockStatisticsAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取库存统计失败");
            return Error<StockStatisticsDto>("获取库存统计失败");
        }
    }

    /// <summary>
    /// 获取概览统计
    /// </summary>
    /// <returns>概览统计信息</returns>
    [HttpGet("stats/overview")]
    [ProducesResponseType(typeof(ApiResponse<ProductOverviewStatsDto>), 200)]
    public async Task<ApiResponse<ProductOverviewStatsDto>> GetOverviewStats()
    {
        try
        {
            var result = await _productService.GetOverviewStatsAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取概览统计失败");
            return Error<ProductOverviewStatsDto>("获取概览统计失败");
        }
    }

    /// <summary>
    /// 获取销量排行
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <param name="sortBy">排序方式：sales-销量，amount-金额</param>
    /// <returns>销量排行列表</returns>
    [HttpGet("stats/ranking")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductSalesStatsDto>>), 200)]
    public async Task<ApiResponse<List<ProductSalesStatsDto>>> GetSalesRanking([FromQuery] int limit = 10, [FromQuery] string sortBy = "sales")
    {
        try
        {
            var result = await _productService.GetSalesRankingAsync(limit, sortBy);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取销量排行失败");
            return Error<List<ProductSalesStatsDto>>("获取销量排行失败");
        }
    }

    /// <summary>
    /// 获取分类销量统计
    /// </summary>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <returns>分类销量统计列表</returns>
    [HttpGet("stats/category")]
    [ProducesResponseType(typeof(ApiResponse<List<CategorySalesStatsDto>>), 200)]
    public async Task<ApiResponse<List<CategorySalesStatsDto>>> GetCategorySalesStats([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var result = await _productService.GetCategorySalesStatsAsync(startDate, endDate);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取分类销量统计失败");
            return Error<List<CategorySalesStatsDto>>("获取分类销量统计失败");
        }
    }

    #endregion

    #region 审核管理

    /// <summary>
    /// 提交审核
    /// </summary>
    /// <param name="dto">提交审核参数</param>
    /// <returns>工作流实例ID</returns>
    [HttpPost("submit-audit")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> SubmitAudit([FromBody] SubmitAuditDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userName = User.Identity?.Name ?? "System";
            var instanceId = await _productService.SubmitAuditAsync(dto, userId, userName);
            return Success(instanceId, "提交审核成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "提交审核失败: {ProductId}", dto.ProductId);
            return Error<Guid>("提交审核失败");
        }
    }

    /// <summary>
    /// 撤回审核
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <param name="dto">撤回参数</param>
    /// <returns>影响的行数</returns>
    [HttpPost("withdraw-audit")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> WithdrawAudit([FromBody] WithdrawAuditDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _productService.WithdrawAuditAsync(dto.ProductId, userId, dto.Reason);
            return Success(result, "撤回审核成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "撤回审核失败: {ProductId}", dto.ProductId);
            return Error<int>("撤回审核失败");
        }
    }

    /// <summary>
    /// 审核通过
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("audit-pass")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> AuditPass([FromBody] Guid productId)
    {
        try
        {
            var auditorId = GetCurrentUserId();
            var result = await _productService.AuditPassAsync(productId, auditorId);
            return Success(result, "审核通过成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "审核通过失败: {ProductId}", productId);
            return Error<int>("审核通过失败");
        }
    }

    /// <summary>
    /// 审核驳回
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <param name="dto">驳回参数</param>
    /// <returns>影响的行数</returns>
    [HttpPost("audit-reject")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> AuditReject([FromBody] AuditRejectRequestDto dto)
    {
        try
        {
            var auditorId = GetCurrentUserId();
            var result = await _productService.AuditRejectAsync(dto.ProductId, auditorId, new AuditRejectDto { RejectReason = dto.RejectReason });
            return Success(result, "审核驳回成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "审核驳回失败: {ProductId}", dto.ProductId);
            return Error<int>("审核驳回失败");
        }
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    /// <returns>当前用户ID</returns>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new CommonManager.Error.BusinessException("无法获取当前用户信息");
        }
        return Guid.Parse(userIdClaim);
    }

    #endregion
}