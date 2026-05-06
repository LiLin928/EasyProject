using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Etl.IService;

/// <summary>
/// ETL调度任务服务接口
/// </summary>
public interface IEtlScheduleService
{
    /// <summary>
    /// 获取调度分页列表
    /// </summary>
    Task<PageResponse<EtlScheduleDto>> GetPageListAsync(QueryEtlScheduleDto query);

    /// <summary>
    /// 获取调度详情
    /// </summary>
    Task<EtlScheduleDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建调度
    /// </summary>
    Task<Guid> AddAsync(CreateEtlScheduleDto dto, Guid creatorId, string? creatorName);

    /// <summary>
    /// 更新调度
    /// </summary>
    Task<int> UpdateAsync(UpdateEtlScheduleDto dto);

    /// <summary>
    /// 删除调度
    /// </summary>
    Task<int> DeleteAsync(List<Guid> ids);

    /// <summary>
    /// 启用调度
    /// </summary>
    Task<int> EnableAsync(Guid id);

    /// <summary>
    /// 禁用调度
    /// </summary>
    Task<int> DisableAsync(Guid id);

    /// <summary>
    /// 立即执行调度
    /// </summary>
    Task<Guid> ExecuteNowAsync(Guid id, Guid? triggerUserId, string? triggerUserName);

    /// <summary>
    /// 获取调度执行历史
    /// </summary>
    Task<PageResponse<EtlExecutionDto>> GetExecutionHistoryAsync(Guid id, int pageIndex, int pageSize);

    /// <summary>
    /// 获取调度统计信息
    /// </summary>
    Task<EtlScheduleStatisticsDto?> GetStatisticsAsync(Guid id);
}

/// <summary>
/// 调度统计DTO
/// </summary>
public class EtlScheduleStatisticsDto
{
    /// <summary>
    /// 总执行次数
    /// </summary>
    public int TotalExecutions { get; set; }

    /// <summary>
    /// 成功次数
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// 失败次数
    /// </summary>
    public int FailureCount { get; set; }

    /// <summary>
    /// 平均耗时（毫秒）
    /// </summary>
    public long AvgDuration { get; set; }

    /// <summary>
    /// 最后执行时间
    /// </summary>
    public string? LastExecutionTime { get; set; }
}