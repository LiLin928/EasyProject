using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 供应商控制器
/// </summary>
/// <remarks>
/// 提供供应商管理、商品供应商关联等接口
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SupplierController : BaseController
{
    /// <summary>
    /// 供应商服务接口
    /// </summary>
    public ISupplierService _supplierService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<SupplierController> _logger { get; set; } = null!;

    #region 供应商管理

    /// <summary>
    /// 获取供应商列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页供应商列表</returns>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<SupplierDto>>), 200)]
    public async Task<ApiResponse<PageResponse<SupplierDto>>> GetList([FromBody] QuerySupplierDto query)
    {
        try
        {
            var result = await _supplierService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取供应商列表失败");
            return Error<PageResponse<SupplierDto>>("获取供应商列表失败");
        }
    }

    /// <summary>
    /// 获取供应商详情
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <returns>供应商详情</returns>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<SupplierDto>), 200)]
    public async Task<ApiResponse<SupplierDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _supplierService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<SupplierDto>("供应商不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取供应商详情失败: {Id}", id);
            return Error<SupplierDto>("获取供应商详情失败");
        }
    }

    /// <summary>
    /// 添加供应商
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的供应商ID</returns>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddSupplierDto dto)
    {
        try
        {
            var id = await _supplierService.AddAsync(dto);
            return Success(id, "添加成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加供应商失败");
            return Error<Guid>("添加供应商失败");
        }
    }

    /// <summary>
    /// 更新供应商
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateSupplierDto dto)
    {
        try
        {
            var result = await _supplierService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新供应商失败: {Id}", dto.Id);
            return Error<int>("更新供应商失败");
        }
    }

    /// <summary>
    /// 删除供应商
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _supplierService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除供应商失败: {Id}", id);
            return Error<int>("删除供应商失败");
        }
    }

    /// <summary>
    /// 批量删除供应商
    /// </summary>
    /// <param name="ids">供应商ID列表</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete-batch")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteBatch([FromBody] List<Guid> ids)
    {
        try
        {
            var count = 0;
            foreach (var id in ids)
            {
                count += await _supplierService.DeleteAsync(id);
            }
            return Success(count, "批量删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除供应商失败");
            return Error<int>("批量删除供应商失败");
        }
    }

    #endregion

    #region 商品供应商关联

    /// <summary>
    /// 获取商品的供应商列表
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>商品供应商关联列表</returns>
    [HttpPost("product/{productId}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductSupplierDto>>), 200)]
    public async Task<ApiResponse<List<ProductSupplierDto>>> GetProductSuppliers(Guid productId)
    {
        try
        {
            var result = await _supplierService.GetProductSuppliersAsync(productId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取商品供应商列表失败: {ProductId}", productId);
            return Error<List<ProductSupplierDto>>("获取商品供应商列表失败");
        }
    }

    /// <summary>
    /// 绑定商品供应商
    /// </summary>
    /// <param name="dto">绑定参数</param>
    /// <returns>新创建的关联ID</returns>
    [HttpPost("bind")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> BindProductSupplier([FromBody] BindProductSupplierDto dto)
    {
        try
        {
            var id = await _supplierService.BindProductSupplierAsync(dto);
            return Success(id, "绑定成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "绑定商品供应商失败");
            return Error<Guid>("绑定商品供应商失败");
        }
    }

    /// <summary>
    /// 解绑商品供应商
    /// </summary>
    /// <param name="id">关联ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("unbind")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UnbindProductSupplier([FromBody] Guid id)
    {
        try
        {
            var result = await _supplierService.UnbindProductSupplierAsync(id);
            return Success(result, "解绑成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解绑商品供应商失败: {Id}", id);
            return Error<int>("解绑商品供应商失败");
        }
    }

    /// <summary>
    /// 设置默认供应商
    /// </summary>
    /// <param name="id">商品供应商关联ID</param>
    /// <returns>影响的行数</returns>
    [HttpPut("set-default/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> SetDefaultSupplier(Guid id)
    {
        try
        {
            var result = await _supplierService.SetDefaultSupplierAsync(id);
            return Success(result, "设置成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设置默认供应商失败: {Id}", id);
            return Error<int>("设置默认供应商失败");
        }
    }

    /// <summary>
    /// 获取供应商的商品列表（含绑定信息）
    /// </summary>
    /// <param name="supplierId">供应商ID</param>
    /// <returns>商品供应商关联列表</returns>
    [HttpPost("products/{supplierId}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductSupplierDto>>), 200)]
    public async Task<ApiResponse<List<ProductSupplierDto>>> GetSupplierProducts(Guid supplierId)
    {
        try
        {
            var result = await _supplierService.GetSupplierProductsAsync(supplierId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取供应商商品列表失败: {SupplierId}", supplierId);
            return Error<List<ProductSupplierDto>>("获取供应商商品列表失败");
        }
    }

    #endregion
}