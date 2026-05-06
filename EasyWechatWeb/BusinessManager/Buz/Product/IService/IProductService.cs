using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 商品服务接口
/// </summary>
/// <remarks>
/// 提供商品管理、分类管理、库存管理、统计报表等功能
/// </remarks>
public interface IProductService
{
    #region 商品管理

    /// <summary>
    /// 获取商品分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页商品列表</returns>
    Task<PageResponse<ProductDto>> GetPageListAsync(QueryProductDto query);

    /// <summary>
    /// 根据ID获取商品详情
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>商品信息</returns>
    Task<ProductDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 添加商品
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的商品ID</returns>
    Task<Guid> AddAsync(AddProductDto dto);

    /// <summary>
    /// 更新商品
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateAsync(UpdateProductDto dto);

    /// <summary>
    /// 删除商品
    /// </summary>
    /// <param name="id">商品ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 批量删除商品
    /// </summary>
    /// <param name="ids">商品ID列表</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteBatchAsync(List<Guid> ids);

    /// <summary>
    /// 批量更新商品状态
    /// </summary>
    /// <param name="ids">商品ID列表</param>
    /// <param name="status">状态值</param>
    /// <returns>影响的行数</returns>
    Task<int> BatchUpdateStatusAsync(List<Guid> ids, int status);

    /// <summary>
    /// 批量更新商品分类
    /// </summary>
    /// <param name="ids">商品ID列表</param>
    /// <param name="categoryId">目标分类ID</param>
    /// <returns>影响的行数</returns>
    Task<int> BatchUpdateCategoryAsync(List<Guid> ids, Guid categoryId);

    /// <summary>
    /// 批量更新商品标签
    /// </summary>
    /// <param name="ids">商品ID列表</param>
    /// <param name="isHot">是否热销</param>
    /// <param name="isNew">是否新品</param>
    /// <returns>影响的行数</returns>
    Task<int> BatchUpdateTagAsync(List<Guid> ids, bool? isHot, bool? isNew);

    /// <summary>
    /// 批量添加商品
    /// </summary>
    /// <param name="products">商品列表</param>
    /// <returns>新创建的商品ID列表</returns>
    Task<List<Guid>> BatchAddAsync(List<AddProductDto> products);

    #endregion

    #region 分类管理

    /// <summary>
    /// 获取分类树形列表
    /// </summary>
    /// <returns>分类树形列表</returns>
    Task<List<CategoryDto>> GetCategoryTreeAsync();

    /// <summary>
    /// 添加分类
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的分类ID</returns>
    Task<Guid> AddCategoryAsync(AddCategoryDto dto);

    /// <summary>
    /// 更新分类
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateCategoryAsync(UpdateCategoryDto dto);

    /// <summary>
    /// 删除分类
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteCategoryAsync(Guid id);

    #endregion

    #region 库存管理

    /// <summary>
    /// 获取库存分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页库存列表</returns>
    Task<PageResponse<StockDto>> GetStockListAsync(QueryStockDto query);

    /// <summary>
    /// 库存调整（入库、出库、调整）
    /// </summary>
    /// <param name="dto">调整参数</param>
    /// <returns>影响的行数</returns>
    Task<int> AdjustStockAsync(StockAdjustDto dto);

    /// <summary>
    /// 获取库存变动记录分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页库存变动记录列表</returns>
    Task<PageResponse<StockRecordDto>> GetStockRecordListAsync(QueryStockRecordDto query);

    /// <summary>
    /// 获取低库存预警列表
    /// </summary>
    /// <returns>低库存商品列表</returns>
    Task<List<StockDto>> GetLowStockListAsync();

    #endregion

    #region 统计报表

    /// <summary>
    /// 获取商品销量统计
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>销量统计列表</returns>
    Task<List<ProductSalesStatsDto>> GetSalesStatsAsync(ProductStatsQueryDto query);

    /// <summary>
    /// 获取销量趋势
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>销量趋势列表</returns>
    Task<List<SalesTrendDto>> GetSalesTrendAsync(ProductStatsQueryDto query);

    /// <summary>
    /// 获取库存统计
    /// </summary>
    /// <returns>库存统计信息</returns>
    Task<StockStatisticsDto> GetStockStatisticsAsync();

    /// <summary>
    /// 获取商品概览统计
    /// </summary>
    /// <returns>概览统计信息</returns>
    Task<ProductOverviewStatsDto> GetOverviewStatsAsync();

    /// <summary>
    /// 获取销量排行
    /// </summary>
    /// <param name="limit">数量限制</param>
    /// <param name="sortBy">排序方式：sales-销量，amount-金额</param>
    /// <returns>销量排行列表</returns>
    Task<List<ProductSalesStatsDto>> GetSalesRankingAsync(int limit = 10, string sortBy = "sales");

    /// <summary>
    /// 获取分类销量统计
    /// </summary>
    /// <param name="startDate">开始日期</param>
    /// <param name="endDate">结束日期</param>
    /// <returns>分类销量统计列表</returns>
    Task<List<CategorySalesStatsDto>> GetCategorySalesStatsAsync(DateTime? startDate = null, DateTime? endDate = null);

    /// <summary>
    /// 获取分类详情
    /// </summary>
    /// <param name="id">分类ID</param>
    /// <returns>分类信息</returns>
    Task<CategoryDto?> GetCategoryByIdAsync(Guid id);

    /// <summary>
    /// 库存入库
    /// </summary>
    /// <param name="dto">入库参数</param>
    /// <returns>影响的行数</returns>
    Task<int> StockInAsync(StockInDto dto);

    #endregion

    #region 审核管理

    /// <summary>
    /// 提交审核
    /// </summary>
    /// <param name="dto">提交审核参数</param>
    /// <param name="userId">提交人ID</param>
    /// <param name="userName">提交人姓名</param>
    /// <returns>工作流实例ID</returns>
    Task<Guid> SubmitAuditAsync(SubmitAuditDto dto, Guid userId, string userName);

    /// <summary>
    /// 撤回审核
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <param name="userId">撤回人ID</param>
    /// <param name="reason">撤回原因</param>
    /// <returns>影响的行数</returns>
    Task<int> WithdrawAuditAsync(Guid productId, Guid userId, string? reason = null);

    /// <summary>
    /// 审核通过
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <param name="auditorId">审核人ID</param>
    /// <returns>影响的行数</returns>
    Task<int> AuditPassAsync(Guid productId, Guid auditorId);

    /// <summary>
    /// 审核驳回
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <param name="auditorId">审核人ID</param>
    /// <param name="dto">驳回参数</param>
    /// <returns>影响的行数</returns>
    Task<int> AuditRejectAsync(Guid productId, Guid auditorId, AuditRejectDto dto);

    #endregion
}