using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessManager.Basic.IService;
using CommonManager.Elasticsearch;
using EasyWeChatModels.Options;
using EasyWeChatModels.Dto.LogQuery;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BusinessManager.Basic.Service;

/// <summary>
/// 日志查询服务实现
/// </summary>
public class LogQueryService : ILogQueryService
{
    private readonly IElasticsearchClientFactory _clientFactory;
    private readonly ElasticsearchQueryOptions _options;
    private readonly ILogger<LogQueryService> _logger;

    public LogQueryService(
        IElasticsearchClientFactory clientFactory,
        IOptions<ElasticsearchQueryOptions> options,
        ILogger<LogQueryService> logger)
    {
        _clientFactory = clientFactory;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// 计算索引范围：根据时间范围生成索引名称列表
    /// </summary>
    private List<string> CalculateIndices(string indexPrefix, DateTime? start, DateTime? end)
    {
        var startDate = start ?? DateTime.Today.AddDays(-7);
        var endDate = end ?? DateTime.Today;

        // 确保 startDate <= endDate
        if (startDate > endDate)
        {
            (startDate, endDate) = (endDate, startDate);
        }

        var indices = new List<string>();
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            indices.Add($"{indexPrefix}-{date:yyyy.MM.dd}");
        }

        return indices;
    }

    /// <summary>
    /// 获取可用的环境配置列表
    /// </summary>
    public Task<List<string>> GetAvailableEnvironmentsAsync()
    {
        return Task.FromResult(_clientFactory.GetAvailableEnvironments());
    }

    /// <summary>
    /// 构建 Elasticsearch 搜索请求
    /// </summary>
    private SearchRequest BuildSearchRequest(LogQueryRequestDto request, List<string> indices)
    {
        var mustClauses = new List<Query>();

        // 时间范围 - 使用 DateRangeQuery
        if (request.StartTime != null || request.EndTime != null)
        {
            var rangeQuery = new DateRangeQuery("@timestamp"!);
            if (request.StartTime != null)
            {
                rangeQuery.Gte = request.StartTime.Value;
            }
            if (request.EndTime != null)
            {
                rangeQuery.Lte = request.EndTime.Value;
            }
            mustClauses.Add(rangeQuery);
        }

        // 日志级别（精确匹配）- 使用 level.keyword 进行精确匹配
        if (!string.IsNullOrEmpty(request.Level))
        {
            var termQuery = new TermQuery("level.keyword"!);
            termQuery.Value = request.Level;
            mustClauses.Add(termQuery);
        }

        // 请求路径（模糊匹配）- Serilog 存储在 fields 对象内
        if (!string.IsNullOrEmpty(request.RequestPath))
        {
            var wildcardQuery = new WildcardQuery("fields.RequestPath"!);
            wildcardQuery.Value = $"*{request.RequestPath}*";
            mustClauses.Add(wildcardQuery);
        }

        // 用户ID - Serilog 存储在 fields 对象内，使用 keyword 字段精确匹配
        if (request.UserId != null)
        {
            var termQuery = new TermQuery("fields.UserId.keyword"!);
            termQuery.Value = request.UserId.Value.ToString();
            mustClauses.Add(termQuery);
        }

        // HTTP 方法 - Serilog 存储在 fields 对象内，使用 keyword 字段精确匹配
        if (!string.IsNullOrEmpty(request.Method))
        {
            var termQuery = new TermQuery("fields.Method.keyword"!);
            termQuery.Value = request.Method.ToUpper();
            mustClauses.Add(termQuery);
        }

        // IP 地址 - Serilog 存储在 fields 对象内，使用 keyword 字段精确匹配
        if (!string.IsNullOrEmpty(request.IpAddress))
        {
            var termQuery = new TermQuery("fields.IpAddress.keyword"!);
            termQuery.Value = request.IpAddress;
            mustClauses.Add(termQuery);
        }

        // 消息关键字（全文搜索）
        if (!string.IsNullOrEmpty(request.MessageKeyword))
        {
            var matchQuery = new MatchQuery("message"!);
            matchQuery.Query = request.MessageKeyword;
            mustClauses.Add(matchQuery);
        }

        // 异常关键字（全文搜索）- 搜索 exceptions.Message 和 exceptions.StackTraceString
        if (!string.IsNullOrEmpty(request.ExceptionKeyword))
        {
            // 使用 bool should 查询同时搜索多个异常相关字段
            var exceptionQuery = new BoolQuery();
            var shouldClauses = new List<Query>();

            // 搜索异常消息
            var messageMatch = new MatchQuery("exceptions.Message"!);
            messageMatch.Query = request.ExceptionKeyword;
            shouldClauses.Add(messageMatch);

            // 搜索堆栈跟踪
            var stackMatch = new MatchQuery("exceptions.StackTraceString"!);
            stackMatch.Query = request.ExceptionKeyword;
            shouldClauses.Add(stackMatch);

            // 搜索异常类名
            var classMatch = new MatchQuery("exceptions.ClassName"!);
            classMatch.Query = request.ExceptionKeyword;
            shouldClauses.Add(classMatch);

            exceptionQuery.Should = shouldClauses;
            exceptionQuery.MinimumShouldMatch = 1;
            mustClauses.Add(exceptionQuery);
        }

        // 来源机器名 - Serilog 存储在 fields 对象内，使用 keyword 字段精确匹配
        if (!string.IsNullOrEmpty(request.MachineName))
        {
            var termQuery = new TermQuery("fields.MachineName.keyword"!);
            termQuery.Value = request.MachineName;
            mustClauses.Add(termQuery);
        }

        // 执行时长范围 - Serilog 存储在 fields 对象内
        if (request.MinDuration != null || request.MaxDuration != null)
        {
            var durationRange = new NumberRangeQuery("fields.Duration"!);
            if (request.MinDuration != null)
            {
                durationRange.Gte = request.MinDuration.Value;
            }
            if (request.MaxDuration != null)
            {
                durationRange.Lte = request.MaxDuration.Value;
            }
            mustClauses.Add(durationRange);
        }

        var boolQuery = new BoolQuery();
        boolQuery.Must = mustClauses;

        var indicesArray = indices.ToArray();
        var searchRequest = new SearchRequest(indicesArray)
        {
            Query = boolQuery,
            From = (request.PageIndex - 1) * request.PageSize,
            Size = request.PageSize,
            Sort = new[]
            {
                SortOptions.Field("@timestamp"!, new FieldSort { Order = SortOrder.Desc })
            }
        };

        return searchRequest;
    }

    /// <summary>
    /// 查询日志列表（分页）
    /// </summary>
    public async Task<LogQueryResponseDto> QueryLogsAsync(LogQueryRequestDto request)
    {
        try
        {
            // 获取环境配置
            if (!_options.Environments.ContainsKey(request.Environment))
            {
                _logger.LogWarning("环境 '{Environment}' 不存在", request.Environment);
                return new LogQueryResponseDto
                {
                    Total = 0,
                    Items = new List<LogEntryDto>(),
                    QueriedIndices = new List<string>()
                };
            }

            var config = _options.Environments[request.Environment];
            var client = _clientFactory.GetClient(request.Environment);

            // 使用通配符索引查询，避免不存在的索引导致 404
            // ES 会自动根据 @timestamp 时间范围过滤结果
            var indices = new List<string> { $"{config.IndexPrefix}-*" };

            // 构建搜索请求
            var searchRequest = BuildSearchRequest(request, indices);

            // 执行查询
            var response = await client.SearchAsync<object>(searchRequest);

            if (!response.IsValidResponse)
            {
                _logger.LogError("ES 查询失败: {DebugInformation}", response.DebugInformation);
                return new LogQueryResponseDto
                {
                    Total = 0,
                    Items = new List<LogEntryDto>(),
                    QueriedIndices = indices
                };
            }

            // 映射结果
            var items = response.Hits.Select(hit => MapToLogEntryDto(hit)).ToList();

            return new LogQueryResponseDto
            {
                Total = (int)response.Total,
                Items = items,
                QueriedIndices = indices
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询日志失败，环境: {Environment}", request.Environment);
            throw;
        }
    }

    /// <summary>
    /// 将 ES Hit 映射为 LogEntryDto
    /// </summary>
    private LogEntryDto MapToLogEntryDto(Hit<object> hit)
    {
        var source = hit.Source as System.Text.Json.JsonElement?;
        var dto = new LogEntryDto
        {
            Id = hit.Id ?? ""
        };

        if (source != null && source.Value.ValueKind == System.Text.Json.JsonValueKind.Object)
        {
            var obj = source.Value;

            // 解析顶层字段
            if (obj.TryGetProperty("@timestamp", out var ts))
            {
                dto.Timestamp = DateTime.Parse(ts.GetString() ?? "");
            }
            if (obj.TryGetProperty("level", out var level))
            {
                dto.Level = level.GetString() ?? "";
            }
            if (obj.TryGetProperty("message", out var msg))
            {
                dto.Message = msg.GetString() ?? "";
            }

            // 解析 fields 对象内的字段（Serilog Elasticsearch Sink 存储结构）
            if (obj.TryGetProperty("fields", out var fields) && fields.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                // 请求路径
                if (fields.TryGetProperty("RequestPath", out var path) || fields.TryGetProperty("Path", out path))
                {
                    dto.RequestPath = path.GetString();
                }
                // HTTP 方法
                if (fields.TryGetProperty("Method", out var method))
                {
                    dto.Method = method.GetString();
                }
                // 用户ID
                if (fields.TryGetProperty("UserId", out var userId))
                {
                    dto.UserId = userId.GetString();
                }
                // 用户名
                if (fields.TryGetProperty("UserName", out var userName))
                {
                    dto.UserName = userName.GetString();
                }
                // IP 地址
                if (fields.TryGetProperty("IpAddress", out var ip))
                {
                    dto.IpAddress = ip.GetString();
                }
                // 执行时长
                if (fields.TryGetProperty("Duration", out var duration))
                {
                    if (duration.ValueKind == System.Text.Json.JsonValueKind.Number)
                    {
                        dto.Duration = duration.GetInt32();
                    }
                }
                // 机器名
                if (fields.TryGetProperty("MachineName", out var machine))
                {
                    dto.MachineName = machine.GetString();
                }
                // 环境名
                if (fields.TryGetProperty("EnvironmentName", out var env))
                {
                    dto.Environment = env.GetString();
                }
                // 异常信息
                if (fields.TryGetProperty("Exception", out var exc))
                {
                    var excStr = exc.GetString() ?? "";
                    dto.Exception = excStr.Length > 200 ? excStr.Substring(0, 200) + "..." : excStr;
                }
            }

            // 兼容顶层字段（某些特殊配置可能存储在顶层）
            if (string.IsNullOrEmpty(dto.RequestPath) && obj.TryGetProperty("RequestPath", out var topPath))
            {
                dto.RequestPath = topPath.GetString();
            }
            if (string.IsNullOrEmpty(dto.Method) && obj.TryGetProperty("Method", out var topMethod))
            {
                dto.Method = topMethod.GetString();
            }
            if (string.IsNullOrEmpty(dto.UserId) && obj.TryGetProperty("UserId", out var topUserId))
            {
                dto.UserId = topUserId.GetString();
            }
            if (string.IsNullOrEmpty(dto.UserName) && obj.TryGetProperty("UserName", out var topUserName))
            {
                dto.UserName = topUserName.GetString();
            }
            if (string.IsNullOrEmpty(dto.IpAddress) && obj.TryGetProperty("IpAddress", out var topIp))
            {
                dto.IpAddress = topIp.GetString();
            }
            if (dto.Duration == null && obj.TryGetProperty("Duration", out var topDuration))
            {
                if (topDuration.ValueKind == System.Text.Json.JsonValueKind.Number)
                {
                    dto.Duration = topDuration.GetInt32();
                }
            }
            if (string.IsNullOrEmpty(dto.MachineName) && obj.TryGetProperty("MachineName", out var topMachine))
            {
                dto.MachineName = topMachine.GetString();
            }
            if (string.IsNullOrEmpty(dto.Exception) && obj.TryGetProperty("Exception", out var topExc))
            {
                var excStr = topExc.GetString() ?? "";
                dto.Exception = excStr.Length > 200 ? excStr.Substring(0, 200) + "..." : excStr;
            }
        }

        return dto;
    }

    /// <summary>
    /// 获取单条日志详情
    /// </summary>
    public async Task<LogDetailDto?> GetLogDetailAsync(string environment, string id)
    {
        try
        {
            if (!_options.Environments.ContainsKey(environment))
            {
                _logger.LogWarning("环境 '{Environment}' 不存在", environment);
                return null;
            }

            var config = _options.Environments[environment];
            var client = _clientFactory.GetClient(environment);

            // ES 8.x 不支持通配符索引的 Get API，使用 Multi Get 或搜索查询
            // 使用 mget API 跨多个索引查询
            var indices = CalculateIndices(config.IndexPrefix, DateTime.Today.AddDays(-7), DateTime.Today);

            // 使用搜索查询按 ID 查找（兼容多索引）
            var searchResponse = await client.SearchAsync<object>(s => s
                .Index(indices.ToArray())
                .Size(1)
                .Query(q => q
                    .Ids(i => i.Values(new[] { id }))
                )
            );

            if (!searchResponse.IsValidResponse || searchResponse.Hits.Count == 0)
            {
                _logger.LogWarning("日志不存在，ID: {Id}", id);
                return null;
            }

            var hit = searchResponse.Hits.First();
            if (hit.Source == null)
            {
                _logger.LogWarning("日志文档源为空，ID: {Id}", id);
                return null;
            }

            // 映射为详情 DTO
            return MapToLogDetailDto(id, hit.Source);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取日志详情失败，ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// 将 ES 文档映射为 LogDetailDto
    /// </summary>
    private LogDetailDto MapToLogDetailDto(string id, object source)
    {
        var jsonElement = source as System.Text.Json.JsonElement?;
        var dto = new LogDetailDto
        {
            Id = id
        };

        if (jsonElement != null && jsonElement.Value.ValueKind == System.Text.Json.JsonValueKind.Object)
        {
            var obj = jsonElement.Value;

            // 解析顶层字段
            if (obj.TryGetProperty("@timestamp", out var ts))
            {
                dto.Timestamp = DateTime.Parse(ts.GetString() ?? "");
            }
            if (obj.TryGetProperty("level", out var level))
            {
                dto.Level = level.GetString() ?? "";
            }
            if (obj.TryGetProperty("message", out var msg))
            {
                dto.Message = msg.GetString() ?? "";
            }

            // 解析 fields 对象内的字段（Serilog Elasticsearch Sink 存储结构）
            if (obj.TryGetProperty("fields", out var fields) && fields.ValueKind == System.Text.Json.JsonValueKind.Object)
            {
                if (fields.TryGetProperty("RequestPath", out var path) || fields.TryGetProperty("Path", out path))
                {
                    dto.RequestPath = path.GetString();
                }
                if (fields.TryGetProperty("Method", out var method))
                {
                    dto.Method = method.GetString();
                }
                if (fields.TryGetProperty("UserId", out var userId))
                {
                    dto.UserId = userId.GetString();
                }
                if (fields.TryGetProperty("UserName", out var userName))
                {
                    dto.UserName = userName.GetString();
                }
                if (fields.TryGetProperty("IpAddress", out var ip))
                {
                    dto.IpAddress = ip.GetString();
                }
                if (fields.TryGetProperty("Duration", out var duration))
                {
                    if (duration.ValueKind == System.Text.Json.JsonValueKind.Number)
                    {
                        dto.Duration = duration.GetInt32();
                    }
                }
                if (fields.TryGetProperty("MachineName", out var machine))
                {
                    dto.MachineName = machine.GetString();
                }
                if (fields.TryGetProperty("EnvironmentName", out var env))
                {
                    dto.Environment = env.GetString();
                }
                if (fields.TryGetProperty("Exception", out var exc))
                {
                    dto.Exception = exc.GetString();
                }
                if (fields.TryGetProperty("StackTrace", out var stack))
                {
                    dto.StackTrace = stack.GetString();
                }
            }

            // 兼容顶层字段（某些特殊配置可能存储在顶层）
            if (string.IsNullOrEmpty(dto.RequestPath) && obj.TryGetProperty("RequestPath", out var topPath))
            {
                dto.RequestPath = topPath.GetString();
            }
            if (string.IsNullOrEmpty(dto.Method) && obj.TryGetProperty("Method", out var topMethod))
            {
                dto.Method = topMethod.GetString();
            }
            if (string.IsNullOrEmpty(dto.UserId) && obj.TryGetProperty("UserId", out var topUserId))
            {
                dto.UserId = topUserId.GetString();
            }
            if (string.IsNullOrEmpty(dto.UserName) && obj.TryGetProperty("UserName", out var topUserName))
            {
                dto.UserName = topUserName.GetString();
            }
            if (string.IsNullOrEmpty(dto.IpAddress) && obj.TryGetProperty("IpAddress", out var topIp))
            {
                dto.IpAddress = topIp.GetString();
            }
            if (dto.Duration == null && obj.TryGetProperty("Duration", out var topDuration))
            {
                if (topDuration.ValueKind == System.Text.Json.JsonValueKind.Number)
                {
                    dto.Duration = topDuration.GetInt32();
                }
            }
            if (string.IsNullOrEmpty(dto.MachineName) && obj.TryGetProperty("MachineName", out var topMachine))
            {
                dto.MachineName = topMachine.GetString();
            }
            if (string.IsNullOrEmpty(dto.Exception) && obj.TryGetProperty("Exception", out var topExc))
            {
                dto.Exception = topExc.GetString();
            }
            if (string.IsNullOrEmpty(dto.StackTrace) && obj.TryGetProperty("StackTrace", out var topStack))
            {
                dto.StackTrace = topStack.GetString();
            }

            // 将整个文档作为 Properties
            dto.Properties = source;
        }

        return dto;
    }
}