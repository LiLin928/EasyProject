using BusinessManager.Buz.AntWorkflow.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz.AntWorkflow;

/// <summary>
/// 业务审核点控制器
/// </summary>
/// <remarks>
/// 提供业务审核点的增删改查接口。
/// 业务审核点定义了哪些业务表需要走审批流程，以及审批相关的配置信息。
/// 所有接口需要认证访问，通过JWT Token验证。
/// </remarks>
[ApiController]
[Route("api/business-audit-point")]
[Authorize]
public class BusinessAuditPointController : BaseController
{
    /// <summary>
    /// 业务审核点服务接口（Autofac 属性注入）
    /// </summary>
    public IBusinessAuditPointService _auditPointService { get; set; } = null!;

    /// <summary>
    /// 日志记录器（Autofac 属性注入）
    /// </summary>
    public ILogger<BusinessAuditPointController> _logger { get; set; } = null!;

    #region 查询接口

    /// <summary>
    /// 获取审核点列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页审核点列表</returns>
    /// <response code="200">返回审核点列表</response>
    /// <response code="401">未授权，需要先登录</response>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<BusinessAuditPointDto>>), 200)]
    public async Task<ApiResponse<PageResponse<BusinessAuditPointDto>>> GetList([FromBody] QueryBusinessAuditPointDto query)
    {
        try
        {
            var result = await _auditPointService.GetPageListAsync(query);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取审核点列表失败");
            return Error<PageResponse<BusinessAuditPointDto>>("获取审核点列表失败");
        }
    }

    /// <summary>
    /// 获取审核点详情
    /// </summary>
    /// <param name="id">审核点ID</param>
    /// <returns>审核点详细信息</returns>
    /// <response code="200">返回审核点详情</response>
    /// <response code="401">未授权</response>
    /// <response code="404">审核点不存在</response>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<BusinessAuditPointDto>), 200)]
    public async Task<ApiResponse<BusinessAuditPointDto>> GetDetail(Guid id)
    {
        try
        {
            var result = await _auditPointService.GetByIdAsync(id);
            if (result == null)
            {
                return Error<BusinessAuditPointDto>("审核点不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取审核点详情失败，ID：{Id}", id);
            return Error<BusinessAuditPointDto>("获取审核点详情失败");
        }
    }

    /// <summary>
    /// 根据编码获取审核点
    /// </summary>
    /// <param name="code">审核点编码</param>
    /// <returns>审核点详细信息</returns>
    /// <response code="200">返回审核点详情</response>
    /// <response code="401">未授权</response>
    /// <response code="404">审核点不存在</response>
    [HttpGet("by-code/{code}")]
    [ProducesResponseType(typeof(ApiResponse<BusinessAuditPointDto>), 200)]
    public async Task<ApiResponse<BusinessAuditPointDto>> GetByCode(string code)
    {
        try
        {
            var result = await _auditPointService.GetByCodeAsync(code);
            if (result == null)
            {
                return Error<BusinessAuditPointDto>("审核点不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据编码获取审核点失败，编码：{Code}", code);
            return Error<BusinessAuditPointDto>("获取审核点失败");
        }
    }

    /// <summary>
    /// 根据表名获取可用审核点列表
    /// </summary>
    /// <param name="tableName">处理表名</param>
    /// <returns>审核点列表</returns>
    /// <response code="200">返回审核点列表</response>
    /// <response code="401">未授权</response>
    [HttpGet("by-table/{tableName}")]
    [ProducesResponseType(typeof(ApiResponse<List<BusinessAuditPointDto>>), 200)]
    public async Task<ApiResponse<List<BusinessAuditPointDto>>> GetByTableName(string tableName)
    {
        try
        {
            var result = await _auditPointService.GetByTableNameAsync(tableName);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据表名获取审核点列表失败，表名：{TableName}", tableName);
            return Error<List<BusinessAuditPointDto>>("获取审核点列表失败");
        }
    }

    #endregion

    #region CRUD 操作

    /// <summary>
    /// 新增审核点
    /// </summary>
    /// <param name="dto">新增审核点参数</param>
    /// <returns>新创建的审核点ID</returns>
    /// <response code="200">创建成功，返回审核点ID</response>
    /// <response code="401">未授权</response>
    /// <response code="400">参数验证失败或编码重复</response>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddBusinessAuditPointDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var id = await _auditPointService.AddAsync(dto, userId);
            return Success(id, "新增审核点成功");
        }
        catch (BusinessException ex)
        {
            return Error<Guid>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "新增审核点失败");
            return Error<Guid>("新增审核点失败");
        }
    }

    /// <summary>
    /// 更新审核点
    /// </summary>
    /// <param name="dto">更新审核点参数</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">审核点不存在</response>
    /// <response code="400">参数验证失败或编码重复</response>
    [HttpPost("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateBusinessAuditPointDto dto)
    {
        try
        {
            var result = await _auditPointService.UpdateAsync(dto);
            if (result == 0)
            {
                return Error<int>("审核点不存在", 404);
            }
            return Success(result, "更新审核点成功");
        }
        catch (BusinessException ex)
        {
            return Error<int>(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新审核点失败，ID：{Id}", dto.Id);
            return Error<int>("更新审核点失败");
        }
    }

    /// <summary>
    /// 删除审核点
    /// </summary>
    /// <param name="id">审核点ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">审核点不存在</response>
    [HttpDelete("delete/{id}")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete(Guid id)
    {
        try
        {
            var result = await _auditPointService.DeleteAsync(id);
            if (result == 0)
            {
                return Error<int>("审核点不存在", 404);
            }
            return Success(result, "删除审核点成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除审核点失败，ID：{Id}", id);
            return Error<int>("删除审核点失败");
        }
    }

    #endregion
}