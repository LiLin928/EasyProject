using BusinessManager.Buz.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto.AntWorkflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// Ant流程表单字段控制器
/// </summary>
/// <remarks>
/// 提供表单字段管理API接口。
/// 支持按业务类型管理字段配置。
/// 所有接口需要认证访问，通过JWT Token验证。
/// </remarks>
[ApiController]
[Route("api/ant-workflow/form-field")]
[Authorize]
public class AntWorkflowFormFieldController : BaseController
{
    private readonly IAntWorkflowFormFieldService _formFieldService;
    private readonly ILogger<AntWorkflowFormFieldController> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    public AntWorkflowFormFieldController(
        IAntWorkflowFormFieldService formFieldService,
        ILogger<AntWorkflowFormFieldController> logger)
    {
        _formFieldService = formFieldService;
        _logger = logger;
    }

    /// <summary>
    /// 获取业务类型表单字段列表
    /// </summary>
    /// <param name="businessType">业务类型编码</param>
    /// <returns>字段列表</returns>
    /// <response code="200">返回字段列表</response>
    /// <response code="401">未授权</response>
    [HttpGet("list/{businessType}")]
    [ProducesResponseType(typeof(ApiResponse<List<AntFormFieldDto>>), 200)]
    public async Task<ApiResponse<List<AntFormFieldDto>>> GetList(string businessType)
    {
        try
        {
            var result = await _formFieldService.GetListByBusinessTypeAsync(businessType);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取表单字段列表失败，业务类型：{BusinessType}", businessType);
            return Error<List<AntFormFieldDto>>("获取表单字段列表失败");
        }
    }

    /// <summary>
    /// 添加表单字段
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新字段ID</returns>
    /// <response code="200">添加成功，返回字段ID</response>
    /// <response code="401">未授权</response>
    /// <response code="400">参数验证失败或字段已存在</response>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] CreateFormFieldDto dto)
    {
        try
        {
            var id = await _formFieldService.AddAsync(dto);
            return Success(id, "添加表单字段成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "添加表单字段失败");
            return Error<Guid>(ex.Message);
        }
    }

    /// <summary>
    /// 更新表单字段
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">字段不存在</response>
    [HttpPut("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateFormFieldDto dto)
    {
        try
        {
            var result = await _formFieldService.UpdateAsync(dto);
            if (result == 0)
            {
                return Error<int>("表单字段不存在", 404);
            }
            return Success(result, "更新表单字段成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新表单字段失败，字段ID：{Id}", dto.Id);
            return Error<int>("更新表单字段失败");
        }
    }

    /// <summary>
    /// 删除表单字段
    /// </summary>
    /// <param name="id">字段ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权</response>
    /// <response code="404">字段不存在</response>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _formFieldService.DeleteAsync(id);
            if (result == 0)
            {
                return Error<int>("表单字段不存在", 404);
            }
            return Success(result, "删除表单字段成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除表单字段失败，字段ID：{Id}", id);
            return Error<int>("删除表单字段失败");
        }
    }

    /// <summary>
    /// 批量更新字段顺序
    /// </summary>
    /// <param name="orders">排序参数列表</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权</response>
    [HttpPut("order")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateOrder([FromBody] List<FormFieldOrderDto> orders)
    {
        try
        {
            var result = await _formFieldService.UpdateOrderAsync(orders);
            return Success(result, "更新字段顺序成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新字段顺序失败");
            return Error<int>("更新字段顺序失败");
        }
    }
}