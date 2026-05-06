namespace BusinessManager.Tasks;

using BusinessManager.Infrastructure.IService;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using InfrastructureManager.Quartz;
using Microsoft.Extensions.Logging;
using SqlSugar;
using global::System.Net.Http;
using global::System.Text;

/// <summary>
/// 事务性任务执行器 - 保证业务数据一致性
/// </summary>
public class TransactionalTaskExecutor
{
    /// <summary>
    /// 数据库客户端（属性注入）
    /// </summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>
    /// 任务日志服务（属性注入）
    /// </summary>
    public ITaskExecutionLogService _logService { get; set; } = null!;

    /// <summary>
    /// 任务定义服务（属性注入）
    /// </summary>
    public ITaskDefinitionService _taskService { get; set; } = null!;

    /// <summary>
    /// 事件总线服务（属性注入，可能为 null 如果 CAP 未启用）
    /// </summary>
    public IEventBusService? _eventBus { get; set; }

    /// <summary>
    /// 任务执行器工厂（属性注入）
    /// </summary>
    public InfrastructureManager.Quartz.TaskExecutorFactory _executorFactory { get; set; } = null!;

    /// <summary>
    /// HTTP 客户端工厂（属性注入）
    /// </summary>
    public IHttpClientFactory? _httpClientFactory { get; set; }

    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<TransactionalTaskExecutor> _logger { get; set; } = null!;

    /// <summary>
    /// 在事务中执行任务
    /// </summary>
    public async Task<TaskExecutionResult> ExecuteWithTransactionAsync(Guid taskDefinitionId, Func<Task> action)
    {
        var task = await _taskService.GetByIdAsync(taskDefinitionId);
        if (task == null)
        {
            return TaskExecutionResult.Failed("任务不存在");
        }

        var logId = Guid.NewGuid();
        var startTime = DateTime.Now;

        try
        {
            // 1. 开始事务
            _db.Ado.BeginTran();

            // 2. 创建执行日志（状态：执行中）
            var executionLog = new TaskExecutionLog
            {
                Id = logId,
                JobName = task.TaskName,
                JobGroup = task.TaskGroup,
                Status = (int)TaskExecutionStatus.Running,
                StartTime = startTime,
                TriggerType = task.TaskType == (int)TaskType.Immediate ? (int)TriggerType.Manual : (int)TriggerType.Cron
            };
            await _logService.LogExecutionAsync(executionLog);

            // 3. 更新任务状态为执行中
            await _taskService.UpdateTaskStatusAsync(taskDefinitionId, TaskDefinitionStatus.Scheduled);
            await _taskService.UpdateNextExecuteTimeAsync(taskDefinitionId, task.TaskType == (int)TaskType.Immediate ? null : _taskService.CalculateNextExecuteTime(task));

            // 4. 执行业务逻辑
            await action();

            // 5. 计算执行时长
            var endTime = DateTime.Now;
            var duration = (long)(endTime - startTime).TotalMilliseconds;

            // 6. 更新任务状态（周期任务保持 Scheduled，一次性任务改为 Completed）
            var newStatus = task.TaskType == (int)TaskType.Immediate
                ? TaskDefinitionStatus.Completed
                : TaskDefinitionStatus.Scheduled;
            await _taskService.UpdateTaskStatusAsync(taskDefinitionId, newStatus);

            // 7. 更新执行日志（成功）
            await _logService.UpdateExecutionResultAsync(logId, (int)TaskExecutionStatus.Success, "执行成功", null, null);

            // 8. 更新最后执行时间和下次执行时间
            await _db.Updateable<TaskDefinition>()
                .SetColumns(x => x.LastExecuteTime == endTime)
                .SetColumnsIF(task.TaskType != (int)TaskType.Immediate, x => x.NextExecuteTime == _taskService.CalculateNextExecuteTime(task))
                .Where(x => x.Id == taskDefinitionId)
                .ExecuteCommandAsync();

            // 9. 提交事务
            _db.Ado.CommitTran();

            // 10. 发布完成事件（CAP）- 事务外发送，失败不影响任务结果
            try
            {
                if (_eventBus != null)
                {
                    await _eventBus.PublishAsync("task.completed", new { TaskId = taskDefinitionId, LogId = logId, TaskName = task.TaskName });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "发布任务完成事件失败（不影响任务结果）: {TaskName}", task.TaskName);
            }

            // 11. 如果有 API 回调配置，发布回调事件（事务外）
            try
            {
                if (_eventBus != null && !string.IsNullOrEmpty(task.ApiEndpoint))
                {
                    await _eventBus.PublishAsync("api.callback.execute", new ApiCallbackPayload
                    {
                        TaskId = taskDefinitionId,
                        Endpoint = task.ApiEndpoint,
                        Method = task.ApiMethod ?? "POST",
                        Payload = task.ApiPayload,
                        LogId = logId
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "发布 API 回调事件失败（不影响任务结果）: {TaskName}", task.TaskName);
            }

            _logger.LogInformation($"任务执行成功: {task.TaskName}, 耗时: {duration}ms");

            return TaskExecutionResult.Success("执行成功", logId);
        }
        catch (Exception ex)
        {
            // 回滚事务
            _db.Ado.RollbackTran();

            // 记录失败日志
            var endTime = DateTime.Now;
            await _logService.UpdateExecutionResultAsync(logId, (int)TaskExecutionStatus.Failed, null, ex.Message, ex.StackTrace);

            // 更新任务状态为失败
            await _taskService.UpdateTaskStatusAsync(taskDefinitionId, TaskDefinitionStatus.Failed);

            // 发布失败事件（如果事件总线可用）
            if (_eventBus != null)
            {
                await _eventBus.PublishAsync("task.failed", new { TaskId = taskDefinitionId, LogId = logId, TaskName = task.TaskName, Error = ex.Message });
            }

            _logger.LogError(ex, $"任务执行失败: {task.TaskName}");

            return TaskExecutionResult.Failed(ex.Message, logId);
        }
    }

    /// <summary>
    /// 执行处理器任务
    /// </summary>
    public async Task<TaskExecutionResult> ExecuteHandlerAsync(Guid taskDefinitionId)
    {
        var task = await _taskService.GetByIdAsync(taskDefinitionId);
        if (task == null)
        {
            return TaskExecutionResult.Failed("任务不存在");
        }

        return await ExecuteWithTransactionAsync(taskDefinitionId, async () =>
        {
            // 根据执行器类型选择执行方式
            if (task.ExecutorType == 0)  // 反射执行
            {
                if (string.IsNullOrEmpty(task.HandlerType))
                {
                    throw new Exception("处理器类型未配置");
                }
                var result = await _executorFactory.ExecuteAsync(task.HandlerType, task.HandlerMethod, task.BusinessData);
                if (!result.IsSuccess)
                {
                    throw new Exception(result.Message ?? "处理器执行失败");
                }
            }
            else if (task.ExecutorType == 1)  // API 调用
            {
                if (string.IsNullOrEmpty(task.ApiEndpoint))
                {
                    throw new Exception("API 地址未配置");
                }

                // 执行 API 调用
                var apiResult = await ExecuteApiCallAsync(task.ApiEndpoint, task.ApiMethod ?? "POST", task.ApiPayload, task.TimeoutSeconds);
                if (!apiResult.IsSuccess)
                {
                    throw new Exception(apiResult.Message ?? "API 调用失败");
                }
            }
            else
            {
                throw new Exception($"未知的执行器类型: {task.ExecutorType}");
            }
        });
    }

    /// <summary>
    /// 执行 API 调用
    /// </summary>
    private async Task<TaskExecutionResult> ExecuteApiCallAsync(string endpoint, string method, string? payload, int timeoutSeconds = 60)
    {
        try
        {
            _logger.LogInformation("执行 API 调用: {Endpoint}, 方法: {Method}", endpoint, method);

            // 使用 HttpClientFactory 创建客户端（如果可用），否则创建新实例
            var httpClient = _httpClientFactory?.CreateClient() ?? new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            // 构建请求
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(endpoint),
                Method = method.ToUpper() == "GET" ? HttpMethod.Get : HttpMethod.Post
            };

            // 添加请求体（POST 请求）
            if (method.ToUpper() == "POST" && !string.IsNullOrEmpty(payload))
            {
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            }
            else if (method.ToUpper() == "POST")
            {
                request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
            }

            // 发送请求
            var response = await httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("API 响应: {StatusCode}, 内容: {Content}", response.StatusCode, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                return TaskExecutionResult.Failed($"API 调用失败: {response.StatusCode}, {responseContent}");
            }

            return TaskExecutionResult.Success($"API 调用成功: {response.StatusCode}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API 调用网络错误: {Endpoint}", endpoint);
            return TaskExecutionResult.Failed($"网络错误: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "API 调用超时: {Endpoint}", endpoint);
            return TaskExecutionResult.Failed($"请求超时（{timeoutSeconds}秒）");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "API 调用异常: {Endpoint}", endpoint);
            return TaskExecutionResult.Failed(ex.Message);
        }
    }
}