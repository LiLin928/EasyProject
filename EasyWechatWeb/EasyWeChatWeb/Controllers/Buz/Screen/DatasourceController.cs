using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 数据源控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DatasourceController : BaseController
{
    public IDatasourceService _datasourceService { get; set; } = null!;
    public ILogger<DatasourceController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取数据源列表
    /// </summary>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<DatasourceDto>>), 200)]
    public async Task<ApiResponse<PageResponse<DatasourceDto>>> GetList([FromBody] QueryDatasourceDto query)
    {
        try
        {
            var result = await _datasourceService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取数据源列表失败");
            return Error<PageResponse<DatasourceDto>>("获取数据源列表失败");
        }
    }

    /// <summary>
    /// 获取所有数据源（下拉选择用）
    /// </summary>
    [HttpPost("all")]
    [ProducesResponseType(typeof(ApiResponse<List<DatasourceDto>>), 200)]
    public async Task<ApiResponse<List<DatasourceDto>>> GetAll()
    {
        try
        {
            var result = await _datasourceService.GetAllAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取数据源列表失败");
            return Error<List<DatasourceDto>>("获取数据源列表失败");
        }
    }

    /// <summary>
    /// 获取数据源详情
    /// </summary>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<DatasourceDto>), 200)]
    public async Task<ApiResponse<DatasourceDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _datasourceService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<DatasourceDto>("数据源不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取数据源详情失败: {Id}", id);
            return Error<DatasourceDto>("获取数据源详情失败");
        }
    }

    /// <summary>
    /// 添加数据源
    /// </summary>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] CreateDatasourceDto dto)
    {
        try
        {
            var creatorId = GetCurrentUserId();
            var id = await _datasourceService.AddAsync(dto, creatorId);
            return Success(id, "添加成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加数据源失败");
            return Error<Guid>("添加数据源失败");
        }
    }

    /// <summary>
    /// 更新数据源
    /// </summary>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateDatasourceDto dto)
    {
        try
        {
            var result = await _datasourceService.UpdateAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新数据源失败: {Id}", dto.Id);
            return Error<int>("更新数据源失败");
        }
    }

    /// <summary>
    /// 删除数据源
    /// </summary>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _datasourceService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除数据源失败: {Id}", id);
            return Error<int>("删除数据源失败");
        }
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    [HttpPost("test/{id}")]
    [ProducesResponseType(typeof(ApiResponse<TestConnectionResultDto>), 200)]
    public async Task<ApiResponse<TestConnectionResultDto>> TestConnection(Guid id)
    {
        try
        {
            var result = await _datasourceService.TestConnectionAsync(id);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "测试连接失败: {Id}", id);
            return Error<TestConnectionResultDto>("测试连接失败");
        }
    }

    /// <summary>
    /// 测试连接（按配置，用于未保存的数据源）
    /// </summary>
    [HttpPost("test-config")]
    [ProducesResponseType(typeof(ApiResponse<TestConnectionResultDto>), 200)]
    public async Task<ApiResponse<TestConnectionResultDto>> TestConnectionByConfig([FromBody] CreateDatasourceDto dto)
    {
        try
        {
            var result = await _datasourceService.TestConnectionByConfigAsync(dto);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "测试连接失败");
            return Error<TestConnectionResultDto>("测试连接失败");
        }
    }

    /// <summary>
    /// 获取支持的数据库类型
    /// </summary>
    [HttpPost("dbtypes")]
    [ProducesResponseType(typeof(ApiResponse<List<DbTypeInfoDto>>), 200)]
    public async Task<ApiResponse<List<DbTypeInfoDto>>> GetDbTypes()
    {
        try
        {
            var result = await _datasourceService.GetDbTypesAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取数据库类型失败");
            return Error<List<DbTypeInfoDto>>("获取数据库类型失败");
        }
    }
}