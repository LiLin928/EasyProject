using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 商品规格控制器
/// </summary>
/// <remarks>
/// 提供商品规格和SKU管理功能接口
/// </remarks>
[ApiController]
[Route("api/product/spec")]
[Authorize]
public class ProductSpecController : BaseController
{
    /// <summary>
    /// 商品规格服务接口
    /// </summary>
    public IProductSpecService _productSpecService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<ProductSpecController> _logger { get; set; } = null!;

    #region 规格管理

    /// <summary>
    /// 获取商品规格列表
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>规格列表</returns>
    [HttpGet("list/{productId}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductSpecDto>>), 200)]
    public async Task<ApiResponse<List<ProductSpecDto>>> GetSpecList(Guid productId)
    {
        try
        {
            var result = await _productSpecService.GetSpecListAsync(productId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取商品规格列表失败: {ProductId}", productId);
            return Error<List<ProductSpecDto>>("获取商品规格列表失败");
        }
    }

    /// <summary>
    /// 获取规格详情
    /// </summary>
    /// <param name="id">规格ID</param>
    /// <returns>规格详情</returns>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductSpecDto>), 200)]
    public async Task<ApiResponse<ProductSpecDto>> GetSpecDetail(Guid id)
    {
        try
        {
            var result = await _productSpecService.GetSpecByIdAsync(id);
            if (result == null)
            {
                return Error<ProductSpecDto>("规格不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取规格详情失败: {Id}", id);
            return Error<ProductSpecDto>("获取规格详情失败");
        }
    }

    /// <summary>
    /// 创建规格
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的规格ID</returns>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> CreateSpec([FromBody] AddProductSpecDto dto)
    {
        try
        {
            var result = await _productSpecService.CreateSpecAsync(dto);
            return Success(result, "创建成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建规格失败");
            return Error<Guid>("创建规格失败");
        }
    }

    /// <summary>
    /// 更新规格
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateSpec([FromBody] UpdateProductSpecDto dto)
    {
        try
        {
            var result = await _productSpecService.UpdateSpecAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新规格失败: {Id}", dto.Id);
            return Error<int>("更新规格失败");
        }
    }

    /// <summary>
    /// 删除规格
    /// </summary>
    /// <param name="id">规格ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteSpec([FromBody] Guid id)
    {
        try
        {
            var result = await _productSpecService.DeleteSpecAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除规格失败: {Id}", id);
            return Error<int>("删除规格失败");
        }
    }

    /// <summary>
    /// 添加规格选项
    /// </summary>
    /// <param name="specId">规格ID</param>
    /// <param name="option">选项参数</param>
    /// <returns>新创建的选项ID</returns>
    [HttpPost("option/add/{specId}")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> AddSpecOption(Guid specId, [FromBody] AddSpecOptionDto option)
    {
        try
        {
            var result = await _productSpecService.AddSpecOptionAsync(specId, option);
            return Success(result, "添加成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加规格选项失败: {SpecId}", specId);
            return Error<Guid>("添加规格选项失败");
        }
    }

    /// <summary>
    /// 删除规格选项
    /// </summary>
    /// <param name="optionId">选项ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("option/delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteSpecOption([FromBody] Guid optionId)
    {
        try
        {
            var result = await _productSpecService.DeleteSpecOptionAsync(optionId);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除规格选项失败: {OptionId}", optionId);
            return Error<int>("删除规格选项失败");
        }
    }

    #endregion

    #region SKU管理

    /// <summary>
    /// 获取商品SKU列表
    /// </summary>
    /// <param name="productId">商品ID</param>
    /// <returns>SKU列表</returns>
    [HttpGet("sku/list/{productId}")]
    [ProducesResponseType(typeof(ApiResponse<List<ProductSkuDto>>), 200)]
    public async Task<ApiResponse<List<ProductSkuDto>>> GetSkuList(Guid productId)
    {
        try
        {
            var result = await _productSpecService.GetSkuListAsync(productId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取SKU列表失败: {ProductId}", productId);
            return Error<List<ProductSkuDto>>("获取SKU列表失败");
        }
    }

    /// <summary>
    /// 获取SKU详情
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <returns>SKU详情</returns>
    [HttpGet("sku/detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<ProductSkuDto>), 200)]
    public async Task<ApiResponse<ProductSkuDto>> GetSkuDetail(Guid id)
    {
        try
        {
            var result = await _productSpecService.GetSkuByIdAsync(id);
            if (result == null)
            {
                return Error<ProductSkuDto>("SKU不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取SKU详情失败: {Id}", id);
            return Error<ProductSkuDto>("获取SKU详情失败");
        }
    }

    /// <summary>
    /// 保存SKU
    /// </summary>
    /// <param name="dto">保存参数</param>
    /// <returns>SKU ID</returns>
    [HttpPost("sku/save")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> SaveSku([FromBody] SaveProductSkuDto dto)
    {
        try
        {
            var result = await _productSpecService.SaveSkuAsync(dto);
            return Success(result, "保存成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存SKU失败");
            return Error<Guid>("保存SKU失败");
        }
    }

    /// <summary>
    /// 删除SKU
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("sku/delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteSku([FromBody] Guid id)
    {
        try
        {
            var result = await _productSpecService.DeleteSkuAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除SKU失败: {Id}", id);
            return Error<int>("删除SKU失败");
        }
    }

    /// <summary>
    /// 批量生成SKU
    /// </summary>
    /// <param name="dto">生成参数</param>
    /// <returns>生成的SKU数量</returns>
    [HttpPost("sku/generate")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> GenerateSkus([FromBody] GenerateSkuDto dto)
    {
        try
        {
            var result = await _productSpecService.GenerateSkusAsync(dto);
            return Success(result, $"成功生成{result}个SKU");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "生成SKU失败");
            return Error<int>("生成SKU失败");
        }
    }

    /// <summary>
    /// 更新SKU库存
    /// </summary>
    /// <param name="id">SKU ID</param>
    /// <param name="stock">库存变化量</param>
    /// <returns>影响的行数</returns>
    [HttpPut("sku/stock/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateSkuStock(Guid id, [FromQuery] int stock)
    {
        try
        {
            var result = await _productSpecService.UpdateSkuStockAsync(id, stock);
            return Success(result, "库存更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新SKU库存失败: {Id}", id);
            return Error<int>("更新SKU库存失败");
        }
    }

    #endregion
}