namespace EasyWeChatModels.Enums.Infrastructure;

/// <summary>
/// 补偿类型枚举
/// </summary>
public enum CompensationType
{
    /// <summary>
    /// 重置任务状态为待调度
    /// </summary>
    ResetToPending = 0,

    /// <summary>
    /// 标记任务为永久失败
    /// </summary>
    MarkAsPermanentFailure = 1,

    /// <summary>
    /// 执行业务撤销操作
    /// </summary>
    ExecuteBusinessRollback = 2,

    /// <summary>
    /// 发送告警通知
    /// </summary>
    SendAlertNotification = 3,

    /// <summary>
    /// 清理临时数据
    /// </summary>
    CleanupTempData = 4
}