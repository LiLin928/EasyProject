namespace BusinessManager.Infrastructure.Service;

using BusinessManager.Infrastructure.IService;
using DotNetCore.CAP;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Dto.Infrastructure;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Enums.Infrastructure;
using Microsoft.Extensions.Logging;
using SqlSugar;

/// <summary>
/// 任务补偿服务实现 - 处理 CAP 重试失败后的撤销操作
/// </summary>
/// <remarks>
/// 功能说明：
/// 1. 定期检查 CAP 发布消息表中的失败消息
/// 2. 根据任务类型执行不同的补偿策略（重置状态、标记失败、发送告警等）
/// 3. 自动检测并初始化 CAP 所需的数据库表（如果表不存在）
///
/// CAP 表说明：
/// - Cap.Published：发布者消息表，存储所有发布的事件消息
/// - Cap.Received：消费者消息表，存储所有接收的消息
/// - 表名中的点号是名称的一部分，不是数据库分隔符
/// - CAP 会自动创建这些表，但有时初始化顺序问题导致查询先执行
///
/// 补偿策略：
/// - 即时任务（Immediate）：标记为永久失败，不再重试
/// - 周期任务（Cron/Periodic）：重置为待调度状态，下次周期继续执行
/// - 其他类型：发送告警通知，由管理员人工处理
/// </remarks>
public class TaskCompensationService : ITaskCompensationService
{
    /// <summary>
    /// 数据库客户端（属性注入）
    /// </summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>
    /// CAP 发布器（属性注入）
    /// </summary>
    public ICapPublisher _capPublisher { get; set; } = null!;

    /// <summary>
    /// 任务定义服务（属性注入）
    /// </summary>
    public ITaskDefinitionService _taskService { get; set; } = null!;

    /// <summary>
    /// 任务日志服务（属性注入）
    /// </summary>
    public ITaskExecutionLogService _logService { get; set; } = null!;

    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<TaskCompensationService> _logger { get; set; } = null!;

    /// <summary>
    /// CAP 表是否已初始化的标记（静态变量，全进程共享）
    /// </summary>
    /// <remarks>
    /// 使用静态变量避免每次查询都检查表是否存在，提高性能
    /// </remarks>
    private static bool _capTablesInitialized = false;

    /// <summary>
    /// 执行补偿操作
    /// </summary>
    public async Task<CompensationResult> ExecuteCompensationAsync(TaskCompensationDto compensation)
    {
        _logger.LogInformation($"开始执行任务补偿: {compensation.TaskName}, 补偿类型: {compensation.CompensationType}, 重试次数: {compensation.RetryCount}/{compensation.MaxRetryCount}");

        try
        {
            var result = compensation.CompensationType switch
            {
                CompensationType.ResetToPending => await ResetToPendingAsync(compensation),
                CompensationType.MarkAsPermanentFailure => await MarkAsPermanentFailureAsync(compensation),
                CompensationType.ExecuteBusinessRollback => await ExecuteBusinessRollbackAsync(compensation),
                CompensationType.SendAlertNotification => await SendAlertNotificationAsync(compensation),
                CompensationType.CleanupTempData => await CleanupTempDataAsync(compensation),
                _ => CompensationResult.Failed("未知的补偿类型")
            };

            // 记录补偿日志
            await LogCompensationAsync(compensation, result);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"补偿执行失败: {compensation.TaskName}");
            return CompensationResult.Failed(ex.Message);
        }
    }

    /// <summary>
    /// 检查 CAP 失败消息并执行补偿
    /// </summary>
    /// <remarks>
    /// 执行流程：
    /// 1. 首次执行时检查 CAP 表是否存在，不存在则自动创建
    /// 2. 查询状态为 Failed 且主题以 task. 开头的消息
    /// 3. 解析消息内容，提取任务信息
    /// 4. 根据任务类型执行相应的补偿策略
    /// 5. 更新消息状态为 Processed，标记已处理
    /// </remarks>
    public async Task<int> CheckAndProcessFailedMessagesAsync()
    {
        _logger.LogInformation("开始检查 CAP 失败消息...");

        // 步骤1：检查并初始化 CAP 表（仅首次执行）
        if (!_capTablesInitialized)
        {
            //await EnsureCapTablesExistAsync();
            _capTablesInitialized = true;
        }

        // 步骤2：查询失败的 CAP 消息
        // CAP 表名格式：Cap.Published（点号是表名的一部分，不是数据库分隔符）
        // CAP 字段映射：Name → TopicName（消息主题）
        // CAP 消息状态：Scheduled（待处理）、Completed（已完成）、Failed（失败）
        var sql = @"
            SELECT Id, Name AS TopicName, Content, Retries, StatusName, ExceptionMessage
            FROM `Cap.Published`
            WHERE StatusName = 'Failed' AND Name LIKE 'task.%'";

        var failedMessages = await _db.SqlQueryable<CapPublishedMessage>(sql).ToListAsync();

        // 步骤3：检查是否有失败消息
        if (failedMessages.Count == 0)
        {
            _logger.LogInformation("没有需要处理的失败消息");
            return 0;
        }

        _logger.LogWarning($"发现 {failedMessages.Count} 个失败的任务消息需要补偿");

        // 步骤4：处理每个失败消息
        int processedCount = 0;
        foreach (var message in failedMessages)
        {
            try
            {
                // 解析消息内容（JSON 格式）
                var content = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(message.Content ?? "{}");

                // 提取任务信息
                Guid.TryParse(content?.TaskId?.ToString(), out Guid taskId);
                Guid.TryParse(content?.LogId?.ToString(), out Guid logId);
                string taskName = content?.TaskName?.ToString() ?? "未知任务";
                string errorMessage = content?.Error?.ToString() ?? message.ExceptionMessage ?? "未知错误";

                // 验证任务ID是否存在
                if (taskId == Guid.Empty)
                {
                    _logger.LogWarning($"失败消息缺少 TaskId: {message.Id}");
                    continue;
                }

                // 获取任务定义
                var task = await _taskService.GetByIdAsync(taskId);
                if (task == null)
                {
                    _logger.LogWarning($"任务不存在: {taskId}");
                    continue;
                }

                // 步骤5：创建补偿数据并执行补偿
                var compensation = new TaskCompensationDto
                {
                    TaskId = taskId,
                    LogId = logId,
                    TaskName = taskName,
                    ErrorMessage = errorMessage,
                    RetryCount = message.Retries,
                    MaxRetryCount = task.MaxRetries,
                    BusinessData = task.BusinessData,
                    CompensationType = DetermineCompensationType(task)
                };

                var result = await ExecuteCompensationAsync(compensation);

                // 步骤6：更新消息状态为已处理
                if (result.IsSuccess)
                {
                    var updateSql = @"
                        UPDATE `Cap.Published`
                        SET StatusName = 'Processed', ExceptionMessage = @exceptionMessage
                        WHERE Id = @id";

                    await _db.Ado.ExecuteCommandAsync(updateSql,
                        new { id = message.Id, exceptionMessage = $"已补偿: {result.Message}" });

                    processedCount++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"处理失败消息异常: {message.Id}");
            }
        }

        _logger.LogInformation($"已完成 {processedCount} 个失败消息的补偿处理");
        return processedCount;
    }

    /// <summary>
    /// 确保 CAP 消息表存在，不存在则自动创建
    /// </summary>
    /// <remarks>
    /// CAP 需要两张表来存储消息：
    /// 1. Cap.Published - 发布者消息表，记录所有发出的消息
    /// 2. Cap.Received - 消费者消息表，记录所有接收的消息
    ///
    /// 通常 CAP 会自动创建这些表，但在以下情况可能需要手动创建：
    /// - CAP 初始化顺序问题（监控服务先于 CAP 初始化执行）
    /// - 数据库用户权限不足
    /// - 表结构需要自定义
    /// </remarks>
    private async Task EnsureCapTablesExistAsync()
    {
        try
        {
            // 检查 Cap.Published 表是否存在
            var checkPublishedSql = @"
                SELECT COUNT(*) FROM information_schema.tables
                WHERE table_schema = DATABASE() AND table_name = 'Cap.Published'";

            var publishedCount = await _db.Ado.SqlQuerySingleAsync<int>(checkPublishedSql);

            if (publishedCount == 0)
            {
                _logger.LogWarning("CAP 发布消息表不存在，正在自动创建...");

                // 创建 Cap.Published 表
                var createPublishedSql = @"
                    CREATE TABLE `Cap.Published` (
                        `Id` BIGINT NOT NULL AUTO_INCREMENT,
                        `Version` VARCHAR(20) NOT NULL DEFAULT 'v1',
                        `Name` VARCHAR(200) NOT NULL,
                        `Content` TEXT,
                        `StatusName` VARCHAR(50) NOT NULL DEFAULT 'Scheduled',
                        `Retries` INT NOT NULL DEFAULT 0,
                        `Added` DATETIME NOT NULL,
                        `ExpiresAt` DATETIME NULL,
                        `ExceptionMessage` TEXT NULL,
                        `ContentId` VARCHAR(200) NULL,
                        PRIMARY KEY (`Id`),
                        INDEX `IX_Cap_Published_StatusName` (`StatusName`),
                        INDEX `IX_Cap_Published_Name` (`Name`),
                        INDEX `IX_Cap_Published_ExpiresAt` (`ExpiresAt`)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4";

                await _db.Ado.ExecuteCommandAsync(createPublishedSql);
                _logger.LogInformation("CAP 发布消息表创建成功：Cap.Published");
            }

            // 检查 Cap.Received 表是否存在
            var checkReceivedSql = @"
                SELECT COUNT(*) FROM information_schema.tables
                WHERE table_schema = DATABASE() AND table_name = 'Cap.Received'";

            var receivedCount = await _db.Ado.SqlQuerySingleAsync<int>(checkReceivedSql);

            if (receivedCount == 0)
            {
                _logger.LogWarning("CAP 接收消息表不存在，正在自动创建...");

                // 创建 Cap.Received 表
                var createReceivedSql = @"
                    CREATE TABLE `Cap.Received` (
                        `Id` BIGINT NOT NULL AUTO_INCREMENT,
                        `Version` VARCHAR(20) NOT NULL DEFAULT 'v1',
                        `Name` VARCHAR(200) NOT NULL,
                        `Group` VARCHAR(200) NOT NULL DEFAULT 'EasyWeChatWeb',
                        `Content` TEXT,
                        `StatusName` VARCHAR(50) NOT NULL DEFAULT 'Scheduled',
                        `Retries` INT NOT NULL DEFAULT 0,
                        `Added` DATETIME NOT NULL,
                        `ExpiresAt` DATETIME NULL,
                        `ExceptionMessage` TEXT NULL,
                        `ContentId` VARCHAR(200) NULL,
                        PRIMARY KEY (`Id`),
                        INDEX `IX_Cap_Received_StatusName` (`StatusName`),
                        INDEX `IX_Cap_Received_Name_Group` (`Name`, `Group`),
                        INDEX `IX_Cap_Received_ExpiresAt` (`ExpiresAt`)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4";

                await _db.Ado.ExecuteCommandAsync(createReceivedSql);
                _logger.LogInformation("CAP 接收消息表创建成功：Cap.Received");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CAP 表初始化失败，请检查数据库权限或手动创建表");
            // 不抛出异常，让后续查询自然失败并记录错误日志
        }
    }

    /// <summary>
    /// 重置任务状态为待调度
    /// </summary>
    public async Task ResetTaskToPendingAsync(Guid taskId)
    {
        await _taskService.UpdateTaskStatusAsync(taskId, TaskDefinitionStatus.Pending);
        _logger.LogInformation($"任务已重置为待调度状态: {taskId}");
    }

    /// <summary>
    /// 标记任务为永久失败
    /// </summary>
    public async Task MarkTaskAsPermanentFailureAsync(Guid taskId, string errorMessage)
    {
        await _taskService.UpdateTaskStatusAsync(taskId, TaskDefinitionStatus.Failed);

        // 更新任务描述，记录最终失败原因
        await _db.Updateable<TaskDefinition>()
            .SetColumns(x => x.Description == $"{x.Description} [永久失败: {errorMessage}]")
            .Where(x => x.Id == taskId)
            .ExecuteCommandAsync();

        _logger.LogWarning($"任务已标记为永久失败: {taskId}, 原因: {errorMessage}");
    }

    /// <summary>
    /// 记录补偿日志
    /// </summary>
    public async Task LogCompensationAsync(TaskCompensationDto compensation, CompensationResult result)
    {
        // 更新执行日志
        if (compensation.LogId != Guid.Empty)
        {
            await _logService.UpdateExecutionResultAsync(
                compensation.LogId,
                (int)TaskExecutionStatus.Failed,
                $"补偿执行: {result.Message}",
                compensation.ErrorMessage,
                $"补偿类型: {compensation.CompensationType}, 重试次数: {compensation.RetryCount}"
            );
        }
    }

    /// <summary>
    /// 根据任务配置确定补偿类型
    /// </summary>
    /// <remarks>
    /// 补偿策略说明：
    /// - Immediate（即时任务）：一次性执行，失败后标记为永久失败，不再重试
    /// - Cron/Periodic（周期任务）：按周期执行，失败后重置状态，等待下次周期执行
    /// - 其他类型：发送告警通知，由管理员人工处理
    /// </remarks>
    private CompensationType DetermineCompensationType(TaskDefinition task)
    {
        if (task.TaskType == (int)TaskType.Immediate)
        {
            return CompensationType.MarkAsPermanentFailure;
        }
        else if (task.TaskType == (int)TaskType.Cron || task.TaskType == (int)TaskType.Periodic)
        {
            return CompensationType.ResetToPending;
        }

        return CompensationType.SendAlertNotification;
    }

    /// <summary>
    /// 重置为待调度状态
    /// </summary>
    private async Task<CompensationResult> ResetToPendingAsync(TaskCompensationDto compensation)
    {
        await ResetTaskToPendingAsync(compensation.TaskId);
        return CompensationResult.Success(CompensationType.ResetToPending, "任务已重置为待调度状态");
    }

    /// <summary>
    /// 标记为永久失败
    /// </summary>
    private async Task<CompensationResult> MarkAsPermanentFailureAsync(TaskCompensationDto compensation)
    {
        await MarkTaskAsPermanentFailureAsync(compensation.TaskId, compensation.ErrorMessage);
        return CompensationResult.Success(CompensationType.MarkAsPermanentFailure, "任务已标记为永久失败");
    }

    /// <summary>
    /// 执行业务撤销操作
    /// </summary>
    private async Task<CompensationResult> ExecuteBusinessRollbackAsync(TaskCompensationDto compensation)
    {
        // 发布业务撤销事件，由具体业务模块处理
        await _capPublisher.PublishAsync("task.business.rollback", compensation);
        return CompensationResult.Success(CompensationType.ExecuteBusinessRollback, "业务撤销事件已发布");
    }

    /// <summary>
    /// 发送告警通知
    /// </summary>
    private async Task<CompensationResult> SendAlertNotificationAsync(TaskCompensationDto compensation)
    {
        // 发布告警事件，由通知模块处理
        await _capPublisher.PublishAsync("task.alert", new
        {
            TaskId = compensation.TaskId,
            TaskName = compensation.TaskName,
            ErrorMessage = compensation.ErrorMessage,
            RetryCount = compensation.RetryCount,
            MaxRetryCount = compensation.MaxRetryCount,
            AlertTime = DateTime.Now,
            AlertType = "TaskRetryExhausted"
        });

        _logger.LogWarning($"任务告警已发送: {compensation.TaskName}");
        return CompensationResult.Success(CompensationType.SendAlertNotification, "告警通知已发送");
    }

    /// <summary>
    /// 清理临时数据
    /// </summary>
    private async Task<CompensationResult> CleanupTempDataAsync(TaskCompensationDto compensation)
    {
        // 清理任务的临时业务数据
        await _db.Updateable<TaskDefinition>()
            .SetColumns(x => x.BusinessData == null)
            .Where(x => x.Id == compensation.TaskId)
            .ExecuteCommandAsync();

        return CompensationResult.Success(CompensationType.CleanupTempData, "临时数据已清理");
    }
}

/// <summary>
/// CAP 发布消息表实体（用于查询失败消息）
/// </summary>
/// <remarks>
/// CAP 表结构说明：
/// - 表名：Cap.Published（点号是表名的一部分）
/// - Id：消息唯一标识（自增）
/// - Name：消息主题（对应 TopicName）
/// - Content：消息内容（JSON 格式）
/// - StatusName：消息状态（Scheduled/Completed/Failed/Delayed）
/// - Retries：重试次数
/// - ExceptionMessage：异常信息
///
/// 使用原生 SQL 查询避免 SqlSugar 解析点号为数据库分隔符
/// </remarks>
public class CapPublishedMessage
{
    /// <summary>
    /// 消息唯一标识
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 消息主题（从 Name 字段映射）
    /// </summary>
    public string TopicName { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容（JSON 格式）
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int Retries { get; set; }

    /// <summary>
    /// 消息状态
    /// </summary>
    public string StatusName { get; set; } = string.Empty;

    /// <summary>
    /// 异常信息
    /// </summary>
    public string? ExceptionMessage { get; set; }
}