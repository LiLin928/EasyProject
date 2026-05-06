using BusinessManager.Infrastructure.IService;
using BusinessManager.Tasks;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using global::Quartz;
using SqlSugar;

namespace EasyWeChatWeb.Controllers.Infrastructure;

/// <summary>
/// 任务管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : BaseController
{
    /// <summary>
    /// 数据库客户端（属性注入）
    /// </summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>
    /// 任务定义服务接口（属性注入）
    /// </summary>
    public ITaskDefinitionService _taskDefinitionService { get; set; } = null!;

    /// <summary>
    /// 任务执行服务接口（属性注入）
    /// </summary>
    public ITaskExecutorService _taskExecutorService { get; set; } = null!;

    /// <summary>
    /// Quartz 调度器（属性注入）
    /// </summary>
    public IScheduler _scheduler { get; set; } = null!;

    /// <summary>
    /// 任务执行日志服务接口（属性注入）
    /// </summary>
    public ITaskExecutionLogService _taskExecutionLogService { get; set; } = null!;

    /// <summary>
    /// 日志记录器（属性注入）
    /// </summary>
    public ILogger<TaskController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取任务列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页后的任务列表</returns>
    /// <response code="200">成功获取任务列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 支持多种筛选条件进行任务查询：
    /// - 任务名称：模糊搜索
    /// - 任务类型：0-Cron定时，1-即时执行，2-周期任务
    /// - 状态：0-待调度，1-已调度，2-暂停，3-已完成，4-失败
    /// - 任务分组：按分组筛选
    /// - 时间范围：按创建时间筛选
    /// 查询结果按创建时间倒序排列。
    /// </remarks>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<TaskDefinitionDto>>), 200)]
    public async Task<ApiResponse<PageResponse<TaskDefinitionDto>>> GetList([FromBody] QueryTaskDefinitionDto query)
    {
        try
        {
            var queryable = _db.Queryable<TaskDefinition>()
                .WhereIF(!string.IsNullOrEmpty(query.TaskName), x => x.TaskName.Contains(query.TaskName!))
                .WhereIF(query.TaskType.HasValue, x => x.TaskType == query.TaskType!.Value)
                .WhereIF(query.Status.HasValue, x => x.Status == query.Status!.Value)
                .WhereIF(!string.IsNullOrEmpty(query.TaskGroup), x => x.TaskGroup == query.TaskGroup)
                .WhereIF(!string.IsNullOrEmpty(query.StartTime), x => x.CreateTime >= DateTime.Parse(query.StartTime!))
                .WhereIF(!string.IsNullOrEmpty(query.EndTime), x => x.CreateTime <= DateTime.Parse(query.EndTime!).AddDays(1))
                .OrderByDescending(x => x.CreateTime);

            var total = await queryable.CountAsync();
            var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize);

            var dtoList = list.Adapt<List<TaskDefinitionDto>>();
            foreach (var dto in dtoList)
            {
                dto.TaskTypeText = GetTaskTypeText(dto.TaskType);
                dto.StatusText = GetStatusText(dto.Status);
            }

            var result = PageResponse<TaskDefinitionDto>.Create(dtoList, total, query.PageIndex, query.PageSize);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务列表失败");
            return Error<PageResponse<TaskDefinitionDto>>("获取任务列表失败");
        }
    }

    /// <summary>
    /// 获取任务详情
    /// </summary>
    /// <param name="id">任务ID</param>
    /// <returns>任务详细信息</returns>
    /// <response code="200">成功获取任务详情</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">任务不存在</response>
    /// <remarks>
    /// 根据任务ID获取完整的任务定义信息，包括：
    /// - 基本信息：任务名称、分组、类型、描述
    /// - 执行配置：Cron表达式、执行时间、处理器类型
    /// - 状态信息：当前状态、下次执行时间、最后执行时间
    /// </remarks>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<TaskDefinitionDto>), 200)]
    public async Task<ApiResponse<TaskDefinitionDto>> GetDetail(Guid id)
    {
        try
        {
            var task = await _taskDefinitionService.GetByIdAsync(id);
            if (task == null)
            {
                return Error<TaskDefinitionDto>("任务不存在", 404);
            }

            var dto = task.Adapt<TaskDefinitionDto>();
            dto.TaskTypeText = GetTaskTypeText(dto.TaskType);
            dto.StatusText = GetStatusText(dto.Status);
            return Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务详情失败: {Id}", id);
            return Error<TaskDefinitionDto>("获取任务详情失败");
        }
    }

    /// <summary>
    /// 创建任务
    /// </summary>
    /// <param name="dto">创建任务参数</param>
    /// <returns>新创建的任务ID</returns>
    /// <response code="200">创建成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">参数验证失败</response>
    /// <remarks>
    /// 创建新的任务定义，支持多种任务类型：
    /// - Cron定时任务：需要提供 CronExpression
    /// - 即时任务：立即执行一次
    /// - 周期任务：每天/每月指定时间执行
    /// - 指定时间任务：在特定时间执行一次
    /// </remarks>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Create([FromBody] CreateTaskDto dto)
    {
        try
        {
            var taskId = await _taskDefinitionService.CreateTaskAsync(dto);
            return Success(taskId, "任务创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建任务失败");
            return Error<Guid>("创建任务失败");
        }
    }

    /// <summary>
    /// 更新任务配置
    /// </summary>
    /// <param name="dto">更新任务参数</param>
    /// <returns>更新结果</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">任务不存在</response>
    /// <response code="400">参数验证失败</response>
    /// <remarks>
    /// 更新已存在的任务配置，支持修改：
    /// - 任务名称、分组、描述
    /// - 执行配置（Cron表达式、执行时间等）
    /// - 处理器配置（处理器类型、方法名、API地址等）
    /// - 重试和超时配置
    /// 注意：更新执行配置后会重新计算下次执行时间。
    /// </remarks>
    [HttpPost("update")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Update([FromBody] UpdateTaskDto dto)
    {
        try
        {
            if (!dto.Id.HasValue || dto.Id.Value == Guid.Empty)
            {
                return Error<int>("任务ID不能为空", 400);
            }

            var existingTask = await _taskDefinitionService.GetByIdAsync(dto.Id.Value);
            if (existingTask == null)
            {
                return Error<int>("任务不存在", 404);
            }

            // 更新任务配置
            var task = dto.Adapt<TaskDefinition>();
            task.Id = dto.Id.Value;
            task.UpdateTime = DateTime.Now;

            // 重新计算下次执行时间
            task.NextExecuteTime = _taskDefinitionService.CalculateNextExecuteTime(task);

            var result = await _db.Updateable(task).ExecuteCommandAsync();
            return Success(result, "任务更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新任务失败: {Id}", dto.Id);
            return Error<int>("更新任务失败");
        }
    }

    /// <summary>
    /// 批量删除任务
    /// </summary>
    /// <param name="ids">任务ID列表</param>
    /// <returns>删除影响的行数</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">参数错误</response>
    /// <remarks>
    /// 批量删除指定的任务定义：
    /// - 支持一次删除多个任务
    /// - 只能删除暂停状态或已完成状态的任务
    /// - 正在执行的任务无法删除
    /// - 删除操作是物理删除，数据将永久移除
    /// </remarks>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] List<Guid> ids)
    {
        try
        {
            if (ids == null || ids.Count == 0)
            {
                return Error<int>("请选择要删除的任务", 400);
            }

            // 只允许删除暂停或已完成状态的任务
            var result = await _db.Deleteable<TaskDefinition>()
                .Where(x => ids.Contains(x.Id))
                .Where(x => x.Status == (int)TaskDefinitionStatus.Paused || x.Status == (int)TaskDefinitionStatus.Completed)
                .ExecuteCommandAsync();

            if (result == 0)
            {
                return Error<int>("只能删除暂停或已完成状态的任务", 400);
            }

            return Success(result, $"成功删除 {result} 个任务");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除任务失败");
            return Error<int>("删除任务失败");
        }
    }

    /// <summary>
    /// 暂停任务
    /// </summary>
    [HttpPost("pause")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<ApiResponse<object>> Pause([FromBody] TaskIdRequestDto request)
    {
        try
        {
            if (request?.Id == null || request.Id == Guid.Empty)
            {
                return Error<object>("任务ID无效", 400);
            }

            var task = await _taskDefinitionService.GetByIdAsync(request.Id);
            if (task == null)
            {
                return Error<object>("任务不存在", 404);
            }

            // 1. 暂停 Quartz 调度器中的任务
            var jobKey = new JobKey(task.TaskName, task.TaskGroup);
            if (await _scheduler.CheckExists(jobKey))
            {
                await _scheduler.PauseJob(jobKey);
                _logger.LogInformation("Quartz 任务已暂停: {JobKey}", jobKey);
            }

            // 2. 更新数据库状态
            await _taskDefinitionService.PauseTaskAsync(request.Id);
            return Success("任务已暂停");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "暂停任务失败: {Id}", request?.Id);
            return Error<object>("暂停任务失败");
        }
    }

    /// <summary>
    /// 恢复任务
    /// </summary>
    [HttpPost("resume")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<ApiResponse<object>> Resume([FromBody] TaskIdRequestDto request)
    {
        try
        {
            if (request?.Id == null || request.Id == Guid.Empty)
            {
                return Error<object>("任务ID无效", 400);
            }

            var task = await _taskDefinitionService.GetByIdAsync(request.Id);
            if (task == null)
            {
                return Error<object>("任务不存在", 404);
            }

            // 1. 恢复 Quartz 调度器中的任务
            var jobKey = new JobKey(task.TaskName, task.TaskGroup);
            if (await _scheduler.CheckExists(jobKey))
            {
                await _scheduler.ResumeJob(jobKey);
                _logger.LogInformation("Quartz 任务已恢复: {JobKey}", jobKey);
            }

            // 2. 更新数据库状态
            await _taskDefinitionService.ResumeTaskAsync(request.Id);
            return Success("任务已恢复");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "恢复任务失败: {Id}", request?.Id);
            return Error<object>("恢复任务失败");
        }
    }

    /// <summary>
    /// 立即执行任务
    /// </summary>
    [HttpPost("trigger")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<ApiResponse<object>> Trigger([FromBody] TaskIdRequestDto request)
    {
        try
        {
            if (request?.Id == null || request.Id == Guid.Empty)
            {
                return Error<object>("任务ID无效", 400);
            }

            var task = await _taskDefinitionService.GetByIdAsync(request.Id);
            if (task == null)
            {
                return Error<object>("任务不存在", 404);
            }

            _logger.LogInformation("手动触发任务执行: {TaskName}", task.TaskName);

            // 直接调用任务执行服务执行任务
            var result = await _taskExecutorService.ExecuteTaskAsync(request.Id);

            if (result.IsSuccess)
            {
                return Success<object>(new { message = result.Message, logId = result.LogId }, "任务执行成功");
            }
            else
            {
                return Error<object>(result.Message ?? "任务执行失败");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "触发任务失败: {Id}", request?.Id);
            return Error<object>("触发任务失败");
        }
    }

    /// <summary>
    /// 获取任务统计信息
    /// </summary>
    /// <returns>任务统计数据</returns>
    /// <response code="200">成功获取统计信息</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取任务执行的统计信息，包括：
    /// - 任务数量统计：总任务数、各状态任务数
    /// - 今日执行统计：执行次数、成功次数、失败次数、成功率
    /// - 性能指标：平均执行时长
    /// </remarks>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<TaskStatisticsDto>), 200)]
    public async Task<ApiResponse<TaskStatisticsDto>> GetStatistics()
    {
        try
        {
            var statistics = await _taskExecutionLogService.GetTodayStatisticsAsync();
            return Success(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取任务统计失败");
            return Error<TaskStatisticsDto>("获取任务统计失败");
        }
    }

    #region 私有方法

    /// <summary>
    /// 获取任务类型文本描述
    /// </summary>
    private string GetTaskTypeText(int taskType)
    {
        return taskType switch
        {
            (int)TaskType.Cron => "Cron定时",
            (int)TaskType.Immediate => "即时执行",
            (int)TaskType.Periodic => "周期任务",
            _ => "未知"
        };
    }

    /// <summary>
    /// 获取任务状态文本描述
    /// </summary>
    private string GetStatusText(int status)
    {
        return status switch
        {
            (int)TaskDefinitionStatus.Pending => "待调度",
            (int)TaskDefinitionStatus.Scheduled => "已调度",
            (int)TaskDefinitionStatus.Paused => "暂停",
            (int)TaskDefinitionStatus.Completed => "已完成",
            (int)TaskDefinitionStatus.Failed => "失败",
            _ => "未知"
        };
    }

    #endregion
}

/// <summary>
/// 任务ID请求DTO
/// </summary>
public class TaskIdRequestDto
{
    /// <summary>
    /// 任务ID
    /// </summary>
    public Guid Id { get; set; }
}