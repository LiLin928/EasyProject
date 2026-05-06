using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Etl.IService;

/// <summary>
/// ETL执行监控服务接口
/// </summary>
public interface IEtlExecutionService
{
    /// <summary>
    /// 获取执行分页列表
    /// </summary>
    Task<PageResponse<EtlExecutionDto>> GetPageListAsync(QueryEtlExecutionDto query);

    /// <summary>
    /// 获取执行详情
    /// </summary>
    Task<EtlExecutionDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 获取执行统计
    /// </summary>
    Task<EtlExecutionStatisticsDto> GetStatisticsAsync(string? dateStart, string? dateEnd);

    /// <summary>
    /// 取消执行
    /// </summary>
    Task<int> CancelAsync(Guid id);

    /// <summary>
    /// 重试执行
    /// </summary>
    Task<Guid> RetryAsync(Guid id, Guid? triggerUserId, string? triggerUserName);

    /// <summary>
    /// 获取执行日志
    /// </summary>
    Task<List<EtlExecutionLogDto>> GetLogsAsync(Guid id, string? nodeId, string? level);

    /// <summary>
    /// 添加执行日志
    /// </summary>
    Task AddLogAsync(Guid executionId, string level, string message, string? nodeId = null, string? nodeName = null);

    /// <summary>
    /// 获取节点执行详情
    /// </summary>
    Task<EtlNodeExecutionDto?> GetNodeExecutionAsync(Guid executionId, string nodeId);

    /// <summary>
    /// 获取执行的所有节点执行记录
    /// </summary>
    Task<List<EtlNodeExecutionDto>> GetNodeExecutionsAsync(Guid executionId);

    /// <summary>
    /// 创建节点执行记录
    /// </summary>
    Task<Guid> CreateNodeExecutionAsync(Guid executionId, string nodeId, string? nodeName, string? nodeType);

    /// <summary>
    /// 更新节点执行状态
    /// </summary>
    Task UpdateNodeExecutionAsync(Guid nodeExecutionId, string status, object? input = null, object? output = null, string? error = null);

    /// <summary>
    /// 获取正在运行的执行列表
    /// </summary>
    Task<List<EtlExecutionDto>> GetRunningExecutionsAsync();

    /// <summary>
    /// 获取今日统计
    /// </summary>
    Task<EtlTodayStatisticsDto> GetTodayStatisticsAsync();

    /// <summary>
    /// 获取执行状态
    /// </summary>
    Task<EtlExecutionStatusDto?> GetStatusAsync(Guid id);
}

/// <summary>
/// 执行日志DTO
/// </summary>
public class EtlExecutionLogDto
{
    /// <summary>
    /// 时间
    /// </summary>
    public string Time { get; set; } = string.Empty;

    /// <summary>
    /// 级别
    /// </summary>
    public string Level { get; set; } = "info";

    /// <summary>
    /// 消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 节点ID
    /// </summary>
    public string? NodeId { get; set; }

    /// <summary>
    /// 节点名称
    /// </summary>
    public string? NodeName { get; set; }
}

/// <summary>
/// 节点执行详情DTO
/// </summary>
public class EtlNodeExecutionDto
{
    /// <summary>
    /// 节点ID
    /// </summary>
    public string NodeId { get; set; } = string.Empty;

    /// <summary>
    /// 节点名称
    /// </summary>
    public string NodeName { get; set; } = string.Empty;

    /// <summary>
    /// 节点类型
    /// </summary>
    public string? NodeType { get; set; }

    /// <summary>
    /// 节点配置
    /// </summary>
    public string? NodeConfig { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 开始时间
    /// </summary>
    public string? StartTime { get; set; }

    /// <summary>
    /// 结束时间
    /// </summary>
    public string? EndTime { get; set; }

    /// <summary>
    /// 耗时（毫秒）
    /// </summary>
    public long? Duration { get; set; }

    /// <summary>
    /// 输入数据
    /// </summary>
    public object? Input { get; set; }

    /// <summary>
    /// 输出数据
    /// </summary>
    public object? Output { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? Error { get; set; }
}

/// <summary>
/// 执行状态DTO
/// </summary>
public class EtlExecutionStatusDto
{
    /// <summary>
    /// ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 进度
    /// </summary>
    public int Progress { get; set; }

    /// <summary>
    /// 当前节点ID
    /// </summary>
    public string? CurrentNodeId { get; set; }

    /// <summary>
    /// 当前节点名称
    /// </summary>
    public string? CurrentNodeName { get; set; }

    /// <summary>
    /// 已完成节点数
    /// </summary>
    public int CompletedNodes { get; set; }

    /// <summary>
    /// 总节点数
    /// </summary>
    public int TotalNodes { get; set; }
}