namespace BusinessManager.Infrastructure.Service;

using BusinessManager.Infrastructure.IService;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Enums;
using Microsoft.Extensions.Logging;
using SqlSugar;

/// <summary>
/// CAP 日志查询服务实现
/// </summary>
public class CapLogService : ICapLogService
{
    /// <summary>
    /// 数据库客户端（属性注入）
    /// </summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>
    /// 日志器（属性注入）
    /// </summary>
    public ILogger<CapLogService> _logger { get; set; } = null!;

    /// <summary>
    /// 查询发布消息日志
    /// </summary>
    public async Task<List<CapMessageLogDto>> GetPublishedLogsAsync(CapLogQueryDto query)
    {
        // CAP 内置表 cap.published
        var list = await _db.Queryable<dynamic>()
            .AS("cap.published")
            .WhereIF(!string.IsNullOrEmpty(query.Topic), "Name LIKE CONCAT('%', @Topic, '%')", new { Topic = query.Topic })
            .WhereIF(query.Status.HasValue, "Status = @Status", new { Status = query.Status })
            .OrderBy("CreatedTime DESC")
            .Take(query.PageSize)
            .ToListAsync();

        return ConvertToDto(list, CapMessageType.Published);
    }

    /// <summary>
    /// 查询接收消息日志
    /// </summary>
    public async Task<List<CapMessageLogDto>> GetReceivedLogsAsync(CapLogQueryDto query)
    {
        // CAP 内置表 cap.received
        var list = await _db.Queryable<dynamic>()
            .AS("cap.received")
            .WhereIF(!string.IsNullOrEmpty(query.Topic), "Name LIKE CONCAT('%', @Topic, '%')", new { Topic = query.Topic })
            .WhereIF(query.Status.HasValue, "Status = @Status", new { Status = query.Status })
            .OrderBy("CreatedTime DESC")
            .Take(query.PageSize)
            .ToListAsync();

        return ConvertToDto(list, CapMessageType.Received);
    }

    /// <summary>
    /// 清理超过指定天数的日志
    /// </summary>
    public async Task<int> ClearOldLogsAsync(int days)
    {
        var cutoffDate = DateTime.Now.AddDays(-days);
        var publishedCount = await _db.Deleteable<dynamic>()
            .AS("cap.published")
            .Where("CreatedTime < @CutoffDate", new { CutoffDate = cutoffDate })
            .ExecuteCommandAsync();

        var receivedCount = await _db.Deleteable<dynamic>()
            .AS("cap.received")
            .Where("CreatedTime < @CutoffDate", new { CutoffDate = cutoffDate })
            .ExecuteCommandAsync();

        return publishedCount + receivedCount;
    }

    private List<CapMessageLogDto> ConvertToDto(List<dynamic> list, CapMessageType messageType)
    {
        var result = new List<CapMessageLogDto>();
        foreach (var item in list)
        {
            var dto = new CapMessageLogDto
            {
                Id = Guid.TryParse(item.Id?.ToString(), out Guid id) ? id : Guid.NewGuid(),
                MessageType = (int)messageType,
                Topic = item.Name?.ToString() ?? string.Empty,
                Content = item.Content?.ToString(),
                Status = Convert.ToInt32(item.Status ?? 0),
                Retries = Convert.ToInt32(item.Retries ?? 0),
                GroupId = item.Group?.ToString(),
                ExceptionMessage = item.ExceptionMessage?.ToString(),
                CreateTime = Convert.ToDateTime(item.CreatedTime ?? DateTime.Now),
                ProcessTime = item.ProcessTime != null ? Convert.ToDateTime(item.ProcessTime) : null
            };
            dto.StatusText = GetStatusText(dto.Status);
            result.Add(dto);
        }
        return result;
    }

    private string GetStatusText(int status)
    {
        return status switch
        {
            (int)CapMessageStatus.Pending => "待处理",
            (int)CapMessageStatus.Success => "成功",
            (int)CapMessageStatus.Failed => "失败",
            (int)CapMessageStatus.Retrying => "重试中",
            _ => "未知"
        };
    }
}