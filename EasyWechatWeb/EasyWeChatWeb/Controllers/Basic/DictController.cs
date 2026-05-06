using BusinessManager.Basic.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Basic;

/// <summary>
/// 字典控制器
/// </summary>
/// <remarks>
/// 提供字典类型和字典数据的管理接口
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DictController : BaseController
{
    /// <summary>
    /// 字典服务接口
    /// </summary>
    public IDictService _dictService { get; set; } = null!;

    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<DictController> _logger { get; set; } = null!;

    #region 字典类型接口

    /// <summary>
    /// 获取字典类型列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页字典类型列表</returns>
    [HttpPost("type/list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<DictTypeDto>>), 200)]
    public async Task<ApiResponse<PageResponse<DictTypeDto>>> GetDictTypeList([FromBody] QueryDictTypeDto query)
    {
        try
        {
            var result = await _dictService.GetDictTypePageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取字典类型列表失败");
            return Error<PageResponse<DictTypeDto>>("获取字典类型列表失败");
        }
    }

    /// <summary>
    /// 获取所有字典类型列表（不分页）
    /// </summary>
    /// <returns>字典类型列表</returns>
    [HttpPost("type/all")]
    [ProducesResponseType(typeof(ApiResponse<List<DictTypeDto>>), 200)]
    public async Task<ApiResponse<List<DictTypeDto>>> GetAllDictTypes()
    {
        try
        {
            var result = await _dictService.GetDictTypeListAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有字典类型失败");
            return Error<List<DictTypeDto>>("获取所有字典类型失败");
        }
    }

    /// <summary>
    /// 获取字典类型详情
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <returns>字典类型详情</returns>
    [HttpGet("type/detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<DictTypeDto>), 200)]
    public async Task<ApiResponse<DictTypeDto>> GetDictTypeDetail(Guid id)
    {
        try
        {
            var result = await _dictService.GetDictTypeByIdAsync(id);
            if (result == null)
            {
                return Error<DictTypeDto>("字典类型不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取字典类型详情失败: {Id}", id);
            return Error<DictTypeDto>("获取字典类型详情失败");
        }
    }

    /// <summary>
    /// 添加字典类型
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的字典类型ID</returns>
    [HttpPost("type/add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> AddDictType([FromBody] AddDictTypeDto dto)
    {
        try
        {
            var id = await _dictService.AddDictTypeAsync(dto);
            return Success(id, "添加成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加字典类型失败");
            return Error<Guid>("添加字典类型失败");
        }
    }

    /// <summary>
    /// 更新字典类型
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("type/update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateDictType([FromBody] UpdateDictTypeDto dto)
    {
        try
        {
            var result = await _dictService.UpdateDictTypeAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新字典类型失败: {Id}", dto.Id);
            return Error<int>("更新字典类型失败");
        }
    }

    /// <summary>
    /// 删除字典类型
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("type/delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteDictType([FromBody] Guid id)
    {
        try
        {
            var result = await _dictService.DeleteDictTypeAsync(id);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除字典类型失败: {Id}", id);
            return Error<int>("删除字典类型失败");
        }
    }

    #endregion

    #region 字典数据接口

    /// <summary>
    /// 获取字典数据列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页字典数据列表</returns>
    [HttpPost("data/list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<DictDataDto>>), 200)]
    public async Task<ApiResponse<PageResponse<DictDataDto>>> GetDictDataList([FromBody] QueryDictDataDto query)
    {
        try
        {
            var result = await _dictService.GetDictDataPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取字典数据列表失败");
            return Error<PageResponse<DictDataDto>>("获取字典数据列表失败");
        }
    }

    /// <summary>
    /// 获取字典数据详情
    /// </summary>
    /// <param name="id">字典数据ID</param>
    /// <returns>字典数据详情</returns>
    [HttpGet("data/detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<DictDataDto>), 200)]
    public async Task<ApiResponse<DictDataDto>> GetDictDataDetail(Guid id)
    {
        try
        {
            var result = await _dictService.GetDictDataByIdAsync(id);
            if (result == null)
            {
                return Error<DictDataDto>("字典数据不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取字典数据详情失败: {Id}", id);
            return Error<DictDataDto>("获取字典数据详情失败");
        }
    }

    /// <summary>
    /// 根据字典类型编码获取字典数据列表
    /// </summary>
    /// <param name="code">字典类型编码</param>
    /// <returns>字典数据列表</returns>
    /// <remarks>
    /// 此接口用于前端下拉选项，返回启用状态的字典数据
    /// </remarks>
    [HttpPost("data/by-code/{code}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<List<DictDataDto>>), 200)]
    public async Task<ApiResponse<List<DictDataDto>>> GetDictDataByCode(string code)
    {
        try
        {
            var result = await _dictService.GetDictDataByCodeAsync(code);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据编码获取字典数据失败: {Code}", code);
            return Error<List<DictDataDto>>("获取字典数据失败");
        }
    }

    /// <summary>
    /// 添加字典数据
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的字典数据ID</returns>
    [HttpPost("data/add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> AddDictData([FromBody] AddDictDataDto dto)
    {
        try
        {
            var id = await _dictService.AddDictDataAsync(dto);
            return Success(id, "添加成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加字典数据失败");
            return Error<Guid>("添加字典数据失败");
        }
    }

    /// <summary>
    /// 更新字典数据
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    [HttpPut("data/update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateDictData([FromBody] UpdateDictDataDto dto)
    {
        try
        {
            var result = await _dictService.UpdateDictDataAsync(dto);
            return Success(result, "更新成功");
        }
        catch (CommonManager.Error.BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新字典数据失败: {Id}", dto.Id);
            return Error<int>("更新字典数据失败");
        }
    }

    /// <summary>
    /// 删除字典数据
    /// </summary>
    /// <param name="id">字典数据ID</param>
    /// <returns>影响的行数</returns>
    [HttpPost("data/delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteDictData([FromBody] Guid id)
    {
        try
        {
            var result = await _dictService.DeleteDictDataAsync(id);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除字典数据失败: {Id}", id);
            return Error<int>("删除字典数据失败");
        }
    }

    /// <summary>
    /// 批量删除字典数据
    /// </summary>
    /// <param name="ids">字典数据ID列表</param>
    /// <returns>影响的行数</returns>
    [HttpPost("data/delete-batch")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> DeleteDictDataBatch([FromBody] List<Guid> ids)
    {
        try
        {
            var result = await _dictService.DeleteDictDataBatchAsync(ids);
            return Success(result, "批量删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量删除字典数据失败");
            return Error<int>("批量删除字典数据失败");
        }
    }

    /// <summary>
    /// 批量获取字典数据
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <returns>字典编码 -> 字典数据</returns>
    [HttpPost("data/batch")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<Dictionary<string, DictDataWithVersionDto>>), 200)]
    public async Task<ApiResponse<Dictionary<string, DictDataWithVersionDto>>> GetDictDataBatch([FromBody] BatchDictDataRequest request)
    {
        try
        {
            var result = await _dictService.GetDictDataBatchAsync(request.Codes);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量获取字典数据失败");
            return Error<Dictionary<string, DictDataWithVersionDto>>("批量获取字典数据失败");
        }
    }

    /// <summary>
    /// 检查字典版本
    /// </summary>
    /// <param name="request">请求参数</param>
    /// <returns>版本检查结果</returns>
    [HttpPost("version/check")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<VersionCheckResponse>), 200)]
    public async Task<ApiResponse<VersionCheckResponse>> CheckDictVersion([FromBody] VersionCheckRequest request)
    {
        try
        {
            var result = await _dictService.CheckDictVersionAsync(request.Versions);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查字典版本失败");
            return Error<VersionCheckResponse>("检查字典版本失败");
        }
    }

    /// <summary>
    /// 根据字典类型编码获取字典数据（含版本信息）
    /// </summary>
    /// <param name="code">字典类型编码</param>
    /// <returns>字典数据（含版本）</returns>
    [HttpGet("data/with-version/{code}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<DictDataWithVersionDto>), 200)]
    public async Task<ApiResponse<DictDataWithVersionDto>> GetDictDataWithVersion(string code)
    {
        try
        {
            var result = await _dictService.GetDictDataWithVersionByCodeAsync(code);
            if (result == null)
            {
                return Error<DictDataWithVersionDto>("字典数据不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取字典数据失败: {Code}", code);
            return Error<DictDataWithVersionDto>("获取字典数据失败");
        }
    }

    #endregion
}