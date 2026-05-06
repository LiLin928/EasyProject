namespace BusinessManager.Infrastructure.IService;

using EasyWeChatModels.Dto.Infrastructure;
using EasyWeChatModels.Enums.Infrastructure;

/// <summary>
/// 任务补偿服务接口 - 处理 CAP 重试失败后的撤销操作
/// </summary>
public interface ITaskCompensationService
{
    /// <summary>
    /// 执行补偿操作
    /// </summary>
    /// <param name="compensation">补偿数据</param>
    /// <returns>补偿结果</returns>
    Task<CompensationResult> ExecuteCompensationAsync(TaskCompensationDto compensation);

    /// <summary>
    /// 检查 CAP 失败消息并执行补偿
    /// </summary>
    /// <returns>处理的失败消息数量</returns>
    Task<int> CheckAndProcessFailedMessagesAsync();

    /// <summary>
    /// 重置任务状态为待调度（允许重新执行）
    /// </summary>
    /// <param name="taskId">任务ID</param>
    Task ResetTaskToPendingAsync(Guid taskId);

    /// <summary>
    /// 标记任务为永久失败
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="errorMessage">错误消息</param>
    Task MarkTaskAsPermanentFailureAsync(Guid taskId, string errorMessage);

    /// <summary>
    /// 记录补偿日志
    /// </summary>
    /// <param name="compensation">补偿数据</param>
    /// <param name="result">补偿结果</param>
    Task LogCompensationAsync(TaskCompensationDto compensation, CompensationResult result);
}

/// <summary>
/// 补偿结果
/// </summary>
public class CompensationResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 结果消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 补偿类型
    /// </summary>
    public CompensationType CompensationType { get; set; }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static CompensationResult Success(CompensationType type, string message = "补偿执行成功")
    {
        return new CompensationResult { IsSuccess = true, Message = message, CompensationType = type };
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static CompensationResult Failed(string message)
    {
        return new CompensationResult { IsSuccess = false, Message = message };
    }
}