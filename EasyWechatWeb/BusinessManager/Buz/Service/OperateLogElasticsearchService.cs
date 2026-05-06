using System;
using System.Threading.Tasks;
using CommonManager.Elasticsearch;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Options;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 操作日志 Elasticsearch 索引服务
/// </summary>
/// <remarks>
/// 负责将操作日志写入 Elasticsearch，支持异步写入不阻塞主流程。
/// 使用日期分区索引，便于按时间范围查询和清理。
/// </remarks>
public class OperateLogElasticsearchService
{
    private readonly OperateLogElasticsearchOptions _options;
    private readonly ElasticsearchClient _client;
    private readonly ILogger<OperateLogElasticsearchService> _logger;

    public OperateLogElasticsearchService(
        IOptions<OperateLogElasticsearchOptions> options,
        ILogger<OperateLogElasticsearchService> logger)
    {
        _options = options.Value;
        _logger = logger;

        // 创建 ES 客户端
        if (_options.Enabled && !string.IsNullOrEmpty(_options.Url))
        {
            var settings = new ElasticsearchClientSettings(new Uri(_options.Url))
                .DefaultIndex(_options.IndexPrefix)
                .RequestTimeout(TimeSpan.FromMilliseconds(_options.WriteTimeout));

            _client = new ElasticsearchClient(settings);
        }
        else
        {
            _client = null!;
        }
    }

    /// <summary>
    /// 是否启用 ES 存储
    /// </summary>
    public bool IsEnabled => _options.Enabled && _client != null;

    /// <summary>
    /// 计算索引名称（按日期分区）
    /// </summary>
    private string GetIndexName(DateTime createTime)
    {
        return $"{_options.IndexPrefix}-{createTime:yyyy.MM.dd}";
    }

    /// <summary>
    /// 将操作日志索引到 Elasticsearch
    /// </summary>
    /// <param name="dto">操作日志 DTO</param>
    /// <param name="id">日志 ID</param>
    public async Task IndexAsync(AddOperateLogDto dto, Guid id)
    {
        if (!IsEnabled)
        {
            return;
        }

        try
        {
            var createTime = DateTime.Now;
            var document = new OperateLogDocument
            {
                Id = id.ToString(),
                UserId = dto.UserId?.ToString(),
                UserName = dto.UserName,
                Module = dto.Module,
                Action = dto.Action,
                Method = dto.Method,
                Url = dto.Url,
                Ip = dto.Ip,
                Location = dto.Location,
                Params = dto.Params,
                Result = dto.Result,
                Status = dto.Status,
                ErrorMsg = dto.ErrorMsg,
                Duration = dto.Duration,
                CreateTime = createTime,
                Timestamp = createTime
            };

            var indexName = GetIndexName(createTime);

            var response = await _client.IndexAsync(document, idx => idx
                .Index(indexName)
                .Id(id.ToString()));

            if (!response.IsValidResponse)
            {
                _logger.LogWarning("ES 索引操作日志失败，ID: {Id}, 错误: {Error}",
                    id, response.DebugInformation);
            }
            else
            {
                _logger.LogDebug("ES 索引操作日志成功，ID: {Id}, 索引: {Index}",
                    id, indexName);
            }
        }
        catch (Exception ex)
        {
            // ES 写入失败不影响主流程，仅记录日志
            _logger.LogError(ex, "ES 索引操作日志异常，ID: {Id}", id);
        }
    }

    /// <summary>
    /// 异步索引操作日志（Fire-and-Forget）
    /// </summary>
    /// <param name="dto">操作日志 DTO</param>
    /// <param name="id">日志 ID</param>
    public void IndexAsyncFireAndForget(AddOperateLogDto dto, Guid id)
    {
        if (!IsEnabled || !_options.AsyncWrite)
        {
            return;
        }

        // 异步执行，不阻塞主流程
        _ = Task.Run(async () =>
        {
            try
            {
                await IndexAsync(dto, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ES 异步索引操作日志失败，ID: {Id}", id);
            }
        });
    }
}