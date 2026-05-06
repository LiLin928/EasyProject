using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 操作日志服务接口
/// </summary>
/// <remarks>
/// 提供操作日志的管理功能，包括日志记录、查询、删除和清理等。
/// 用于系统操作审计，记录用户的所有操作行为。
/// </remarks>
public interface IOperateLogService
{
    /// <summary>
    /// 获取操作日志分页列表
    /// </summary>
    /// <param name="query">查询条件参数，包含分页信息和筛选条件</param>
    /// <returns>分页后的操作日志列表</returns>
    Task<PageResponse<OperateLogDto>> GetPageListAsync(QueryOperateLogDto query);

    /// <summary>
    /// 根据ID获取操作日志详情
    /// </summary>
    /// <param name="id">操作日志ID</param>
    /// <returns>操作日志详细信息；日志不存在返回null</returns>
    Task<OperateLogDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 添加操作日志记录
    /// </summary>
    /// <param name="dto">添加操作日志参数，包含操作信息、请求参数、结果等</param>
    /// <returns>新创建的操作日志ID</returns>
    Task<Guid> AddAsync(AddOperateLogDto dto);

    /// <summary>
    /// 删除指定操作日志
    /// </summary>
    /// <param name="id">要删除的操作日志ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 清理过期的操作日志
    /// </summary>
    /// <param name="retentionDays">日志保留天数，超过此天数的日志将被删除</param>
    /// <returns>删除的日志数量</returns>
    Task<int> ClearAsync(int retentionDays);

    /// <summary>
    /// 按月份查询操作日志（跨分表查询）
    /// </summary>
    /// <param name="year">年份</param>
    /// <param name="month">月份（1-12）</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <returns>分页后的操作日志列表</returns>
    /// <remarks>
    /// 查询指定月份所有分表的数据。
    /// 例如查询2026年4月的数据，会查询 OperateLog_20260401 到 OperateLog_20260430 所有分表。
    /// </remarks>
    Task<PageResponse<OperateLogDto>> GetByMonthAsync(int year, int month, int pageIndex = 1, int pageSize = 20);
}