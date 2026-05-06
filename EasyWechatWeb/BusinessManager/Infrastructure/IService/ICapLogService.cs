namespace BusinessManager.Infrastructure.IService;

using EasyWeChatModels.Dto;

/// <summary>
/// CAP 日志查询服务接口
/// </summary>
public interface ICapLogService
{
    /// <summary>
    /// 查询发布消息日志（从 CAP 内置表）
    /// </summary>
    Task<List<CapMessageLogDto>> GetPublishedLogsAsync(CapLogQueryDto query);

    /// <summary>
    /// 查询接收消息日志（从 CAP 内置表）
    /// </summary>
    Task<List<CapMessageLogDto>> GetReceivedLogsAsync(CapLogQueryDto query);

    /// <summary>
    /// 清理超过指定天数的日志
    /// </summary>
    Task<int> ClearOldLogsAsync(int days);
}