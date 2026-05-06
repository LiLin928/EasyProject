using System.Collections.Generic;
using System.Threading.Tasks;
using EasyWeChatModels.Dto.LogQuery;

namespace BusinessManager.Basic.IService;

/// <summary>
/// 日志查询服务接口
/// </summary>
public interface ILogQueryService
{
    /// <summary>
    /// 查询日志列表（分页）
    /// </summary>
    /// <param name="request">查询请求参数</param>
    /// <returns>分页日志列表</returns>
    Task<LogQueryResponseDto> QueryLogsAsync(LogQueryRequestDto request);

    /// <summary>
    /// 获取单条日志详情
    /// </summary>
    /// <param name="environment">环境标识</param>
    /// <param name="id">ES 文档 ID</param>
    /// <returns>日志详情，不存在时返回 null</returns>
    Task<LogDetailDto?> GetLogDetailAsync(string environment, string id);

    /// <summary>
    /// 获取可用的环境配置列表
    /// </summary>
    /// <returns>环境名称列表</returns>
    Task<List<string>> GetAvailableEnvironmentsAsync();
}