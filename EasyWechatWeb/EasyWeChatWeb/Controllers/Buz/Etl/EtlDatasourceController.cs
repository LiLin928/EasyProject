using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Etl;

/// <summary>
/// ETL 数据源控制器
/// </summary>
/// <remarks>
/// 提供ETL数据源管理接口，路由前缀为 /api/etl/datasource
/// </remarks>
[ApiController]
[Route("api/etl/datasource")]
[Authorize]
public class EtlDatasourceController : BaseController
{
    public IDatasourceService _datasourceService { get; set; } = null!;
    public ILogger<EtlDatasourceController> _logger { get; set; } = null!;

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
            _logger.LogError(ex, "获取ETL数据源列表失败");
            return Error<PageResponse<DatasourceDto>>("获取数据源列表失败");
        }
    }

    /// <summary>
    /// 获取数据源详情
    /// </summary>
    [HttpGet("detail")]
    [ProducesResponseType(typeof(ApiResponse<DatasourceDto>), 200)]
    public async Task<ApiResponse<DatasourceDto>> GetDetail([FromQuery] Guid id)
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
    /// 创建数据源
    /// </summary>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] CreateDatasourceDto dto)
    {
        try
        {
            var creatorId = GetCurrentUserId();
            var id = await _datasourceService.AddAsync(dto, creatorId);
            return Success(id, "创建成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建数据源失败");
            return Error<Guid>("创建数据源失败");
        }
    }

    /// <summary>
    /// 更新数据源
    /// </summary>
    [HttpPost("update")]
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
    public async Task<ApiResponse<int>> Delete([FromBody] DeleteDatasourceDto dto)
    {
        try
        {
            var count = 0;
            foreach (var id in dto.Ids)
            {
                count += await _datasourceService.DeleteAsync(id);
            }
            return Success(count, "删除成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除数据源失败");
            return Error<int>("删除数据源失败");
        }
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    [HttpPost("test")]
    [ProducesResponseType(typeof(ApiResponse<TestConnectionResultDto>), 200)]
    public async Task<ApiResponse<TestConnectionResultDto>> TestConnection([FromBody] TestConnectionDto dto)
    {
        try
        {
            if (dto.Id.HasValue)
            {
                var result = await _datasourceService.TestConnectionAsync(dto.Id.Value);
                return Success(result);
            }

            // 如果没有ID，则测试传入的配置（暂时返回失败）
            return Success(new TestConnectionResultDto
            {
                Success = false,
                Message = "请先保存数据源后再测试连接"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "测试连接失败");
            return Error<TestConnectionResultDto>("测试连接失败");
        }
    }

    /// <summary>
    /// 测试查询
    /// </summary>
    [HttpPost("test-query")]
    [ProducesResponseType(typeof(ApiResponse<TestQueryResultDto>), 200)]
    public async Task<ApiResponse<TestQueryResultDto>> TestQuery([FromBody] TestQueryDto dto)
    {
        try
        {
            if (!dto.Id.HasValue)
            {
                return Error<TestQueryResultDto>("请提供数据源ID");
            }

            if (string.IsNullOrEmpty(dto.Sql))
            {
                return Error<TestQueryResultDto>("请提供SQL语句");
            }

            var result = await _datasourceService.TestQueryAsync(dto.Id.Value, dto.Sql);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "测试查询失败");
            return Error<TestQueryResultDto>(ex.Message);
        }
    }
}

/// <summary>
/// 删除数据源DTO
/// </summary>
public class DeleteDatasourceDto
{
    /// <summary>
    /// 数据源ID列表
    /// </summary>
    public List<Guid> Ids { get; set; } = new List<Guid>();
}

/// <summary>
/// 测试连接DTO
/// </summary>
public class TestConnectionDto
{
    /// <summary>
    /// 数据源ID（可选）
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// 配置信息（可选）
    /// </summary>
    public object? Config { get; set; }

    /// <summary>
    /// 数据源类型（可选）
    /// </summary>
    public string? Type { get; set; }
}

/// <summary>
/// 测试查询DTO
/// </summary>
public class TestQueryDto
{
    /// <summary>
    /// 数据源ID
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// SQL语句
    /// </summary>
    public string? Sql { get; set; }
}