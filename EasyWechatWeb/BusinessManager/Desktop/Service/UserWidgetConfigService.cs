using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BusinessManager.Desktop.Service;

public class UserWidgetConfigService : BaseService<UserWidgetConfig>, IUserWidgetConfigService
{
    public ILogger<UserWidgetConfigService> _logger { get; set; } = null!;
    public new ISqlSugarClient _db { get; set; } = null!;
    public IHttpClientFactory _httpClientFactory { get; set; } = null!;

    public async Task<UserDesktopDto> GetUserDesktopAsync(Guid userId)
    {
        // 获取用户已启用的组件配置
        var userConfigs = await _db.Queryable<UserWidgetConfig, DesktopWidget>(
            (uc, w) => new JoinQueryInfos(JoinType.Left, uc.WidgetId == w.Id))
            .Where((uc, w) => uc.UserId == userId && uc.IsEnabled)
            .OrderBy((uc, w) => uc.SortOrder)
            .Select((uc, w) => new UserWidgetConfigDto
            {
                Id = uc.Id,
                WidgetId = uc.WidgetId,
                WidgetName = w.Name,
                WidgetType = w.Type,
                Width = uc.Width,
                DefaultHeight = w.DefaultHeight,
                Icon = w.Icon,
                DataSourceType = w.DataSourceType,
                DataSourceConfig = w.DataSourceConfig,
                InteractionConfig = w.InteractionConfig,
                IsEnabled = uc.IsEnabled,
                SortOrder = uc.SortOrder
            })
            .ToListAsync();

        // 获取用户已配置的组件ID列表（用于后续内存匹配）
        var userWidgetIds = userConfigs.Select(uc => uc.WidgetId).ToList();

        // 获取用户角色
        var userRole = await _db.Queryable<UserRole>()
            .Where(x => x.UserId == userId)
            .FirstAsync();

        var availableWidgets = new List<AvailableWidgetDto>();
        if (userRole != null)
        {
            // 获取角色可用的组件（在内存中计算 IsUserEnabled，避免 CAST 字符集冲突）
            var roleWidgetConfigs = await _db.Queryable<RoleWidgetConfig, DesktopWidget>(
                (rc, w) => new JoinQueryInfos(JoinType.Left, rc.WidgetId == w.Id))
                .Where((rc, w) => rc.RoleId == userRole.RoleId && rc.IsEnabled && w.Status == 1)
                .Select((rc, w) => new
                {
                    Id = w.Id,
                    Name = w.Name,
                    Type = (int)w.Type,
                    Icon = w.Icon,
                    DefaultWidth = w.DefaultWidth,
                    DefaultHeight = w.DefaultHeight
                })
                .ToListAsync();

            // 在内存中计算 IsUserEnabled
            availableWidgets = roleWidgetConfigs.Select(w => new AvailableWidgetDto
            {
                Id = w.Id,
                Name = w.Name,
                Type = w.Type,
                Icon = w.Icon,
                DefaultWidth = w.DefaultWidth,
                DefaultHeight = w.DefaultHeight,
                IsUserEnabled = userWidgetIds.Contains(w.Id)
            }).ToList();
        }

        return new UserDesktopDto
        {
            Widgets = userConfigs,
            AvailableWidgets = availableWidgets
        };
    }

    public async Task<bool> SaveAsync(Guid userId, SaveUserWidgetConfigDto dto)
    {
        // 先删除该用户的所有配置
        await _db.Deleteable<UserWidgetConfig>()
            .Where(x => x.UserId == userId)
            .ExecuteCommandAsync();

        // 批量插入新配置
        var entities = dto.Widgets.Select(w => new UserWidgetConfig
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WidgetId = w.WidgetId,
            Width = w.Width,
            IsEnabled = w.IsEnabled,
            SortOrder = w.SortOrder,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now
        }).ToList();

        if (entities.Count > 0)
        {
            await _db.Insertable(entities).ExecuteCommandAsync();
        }
        return true;
    }

    public async Task<bool> ResetAsync(Guid userId)
    {
        // 获取用户角色
        var userRole = await _db.Queryable<UserRole>()
            .Where(x => x.UserId == userId)
            .FirstAsync();
        if (userRole == null) return false;

        // 从角色模板初始化
        return await InitFromRoleAsync(userId, userRole.RoleId);
    }

    public async Task<bool> InitFromRoleAsync(Guid userId, Guid roleId)
    {
        // 获取角色的组件配置
        var roleConfigs = await _db.Queryable<RoleWidgetConfig>()
            .Where(x => x.RoleId == roleId && x.IsEnabled)
            .ToListAsync();

        // 获取组件的默认尺寸
        var widgets = await _db.Queryable<DesktopWidget>()
            .Where(x => x.Status == 1)
            .ToListAsync();

        // 先删除用户的现有配置
        await _db.Deleteable<UserWidgetConfig>()
            .Where(x => x.UserId == userId)
            .ExecuteCommandAsync();

        // 从角色模板复制配置
        var entities = roleConfigs.Select(rc =>
        {
            var widget = widgets.FirstOrDefault(w => w.Id == rc.WidgetId);
            return new UserWidgetConfig
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                WidgetId = rc.WidgetId,
                Width = widget?.DefaultWidth ?? 3,
                IsEnabled = true,
                SortOrder = rc.SortOrder,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
        }).ToList();

        if (entities.Count > 0)
        {
            await _db.Insertable(entities).ExecuteCommandAsync();
        }
        return true;
    }

    /// <summary>
    /// 获取组件数据（代理接口）
    /// </summary>
    public async Task<object> GetWidgetDataAsync(Guid widgetId, Guid userId, string? authToken = null)
    {
        // 获取组件配置
        var widget = await _db.Queryable<DesktopWidget>()
            .Where(x => x.Id == widgetId && x.Status == 1)
            .FirstAsync();

        if (widget == null)
        {
            throw new Exception("组件不存在或已禁用");
        }

        // 验证用户是否有权限访问此组件
        var userRole = await _db.Queryable<UserRole>()
            .Where(x => x.UserId == userId)
            .FirstAsync();

        if (userRole != null)
        {
            var hasAccess = await _db.Queryable<RoleWidgetConfig>()
                .Where(x => x.RoleId == userRole.RoleId && x.WidgetId == widgetId && x.IsEnabled)
                .AnyAsync();

            if (!hasAccess)
            {
                throw new Exception("无权限访问此组件");
            }
        }

        // 获取用户详细信息（用于SQL参数替换）
        var userInfo = await GetUserInfoAsync(userId, userRole?.RoleId);

        // 根据数据源类型获取数据
        _logger.LogInformation($"组件 {widgetId} 数据源类型: {widget.DataSourceType}, 配置: {widget.DataSourceConfig}");
        switch (widget.DataSourceType)
        {
            case DataSourceType.Api:
                _logger.LogInformation($"执行API数据获取");
                return await GetApiDataAsync(widget.DataSourceConfig, authToken);

            case DataSourceType.Static:
                _logger.LogInformation($"执行静态数据获取");
                return GetStaticData(widget.DataSourceConfig);

            case DataSourceType.Sql:
                _logger.LogInformation($"执行SQL数据获取");
                return await GetSqlDataAsync(widget.DataSourceConfig, userInfo);

            case DataSourceType.Report:
                _logger.LogInformation($"执行报表数据获取");
                return await GetReportDataAsync(widget.DataSourceConfig);

            default:
                _logger.LogWarning($"未知数据源类型: {widget.DataSourceType}");
                return new object();
        }
    }

    /// <summary>
    /// 执行SQL获取数据
    /// </summary>
    private async Task<object> GetSqlDataAsync(string? dataSourceConfig, UserInfoParams userInfo)
    {
        _logger.LogInformation($"GetSqlDataAsync 开始执行, dataSourceConfig: {dataSourceConfig}");
        if (string.IsNullOrEmpty(dataSourceConfig))
        {
            _logger.LogWarning("dataSourceConfig 为空");
            return new Dictionary<string, object>();
        }

        try
        {
            var config = JsonSerializer.Deserialize<SqlConfigDto>(dataSourceConfig);
            _logger.LogInformation($"SQL配置解析结果: sql={config?.Sql}, label={config?.Label}");
            if (config == null || string.IsNullOrEmpty(config.Sql))
            {
                _logger.LogWarning("SQL配置解析失败或SQL语句为空");
                return new Dictionary<string, object>();
            }

            // 安全检查：只允许 SELECT 语句
            var sql = config.Sql.Trim();
            if (!sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning($"非法SQL语句，只允许SELECT: {sql}");
                return new Dictionary<string, object> { ["value"] = 0, ["label"] = "SQL不允许" };
            }

            // 替换SQL中的用户参数
            sql = ReplaceSqlParameters(sql, userInfo);
            _logger.LogInformation($"执行SQL: {sql}");

            // 执行 SQL 查询
            var result = await _db.Ado.GetDataTableAsync(sql);
            _logger.LogInformation($"SQL执行结果: 行数={result.Rows.Count}, 列数={result.Columns.Count}");

            // 对于 COUNT 查询，返回单值
            if (result.Rows.Count > 0 && result.Columns.Count == 1)
            {
                var value = result.Rows[0][0];
                var returnValue = new Dictionary<string, object>
                {
                    ["value"] = Convert.ToInt64(value),
                    ["label"] = config.Label ?? "数量"
                };
                _logger.LogInformation($"返回COUNT结果: value={returnValue["value"]}, label={returnValue["label"]}");
                return returnValue;
            }

            // 对于多列查询，返回列表
            var list = new List<Dictionary<string, object>>();
            foreach (System.Data.DataRow row in result.Rows)
            {
                var item = new Dictionary<string, object>();
                foreach (System.Data.DataColumn col in result.Columns)
                {
                    item[col.ColumnName] = row[col];
                }
                list.Add(item);
            }

            _logger.LogInformation($"返回列表结果: 条数={list.Count}");
            return new Dictionary<string, object> { ["list"] = list };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行SQL失败");
            return new Dictionary<string, object> { ["value"] = 0, ["label"] = "执行失败" };
        }
    }

    /// <summary>
    /// 获取用户信息（用于SQL参数）
    /// </summary>
    private async Task<UserInfoParams> GetUserInfoAsync(Guid userId, Guid? roleId)
    {
        var user = await _db.Queryable<User>()
            .Where(x => x.Id == userId)
            .FirstAsync();

        return new UserInfoParams
        {
            UserId = userId,
            UserName = user?.UserName ?? "",
            RoleId = roleId ?? Guid.Empty,
            DepartmentId = user?.DepartmentId ?? Guid.Empty,
            RealName = user?.RealName ?? "",
        };
    }

    /// <summary>
    /// 替换SQL中的用户参数
    /// 支持的参数: {userId}, {userName}, {roleId}, {departmentId}, {realName}
    /// </summary>
    private string ReplaceSqlParameters(string sql, UserInfoParams userInfo)
    {
        // 使用正则表达式匹配参数占位符
        var result = sql;

        // 替换各个参数
        result = result.Replace("{userId}", userInfo.UserId.ToString());
        result = result.Replace("{userName}", userInfo.UserName);
        result = result.Replace("{roleId}", userInfo.RoleId.ToString());
        result = result.Replace("{departmentId}", userInfo.DepartmentId.ToString());
        result = result.Replace("{realName}", userInfo.RealName);

        // GUID 参数需要加引号（用于字符串比较）
        // 如果SQL中有 '{userId}' 形式，也支持
        result = result.Replace("'{userId}'", $"'{userInfo.UserId}'");
        result = result.Replace("'{roleId}'", $"'{userInfo.RoleId}'");
        result = result.Replace("'{departmentId}'", $"'{userInfo.DepartmentId}'");
        result = result.Replace("'{userName}'", $"'{userInfo.UserName}'");
        result = result.Replace("'{realName}'", $"'{userInfo.RealName}'");

        return result;
    }

    /// <summary>
    /// 获取报表数据
    /// </summary>
    private async Task<object> GetReportDataAsync(string? dataSourceConfig)
    {
        if (string.IsNullOrEmpty(dataSourceConfig))
        {
            return new Dictionary<string, object>();
        }

        try
        {
            var config = JsonSerializer.Deserialize<ReportConfigDto>(dataSourceConfig);
            _logger.LogInformation($"Report配置: reportId={config?.ReportId}, reportName={config?.ReportName}");
            if (config == null || config.ReportId == Guid.Empty)
            {
                return new Dictionary<string, object>();
            }

            // 获取报表信息
            var report = await _db.Queryable<Report>()
                .Where(x => x.Id == config.ReportId)
                .FirstAsync();

            if (report == null)
            {
                _logger.LogWarning($"报表不存在: {config.ReportId}");
                return new Dictionary<string, object> { ["error"] = "报表不存在" };
            }

            _logger.LogInformation($"报表信息: name={report.Name}, chartType={report.ChartType}, sqlQuery={report.SqlQuery}");

            // 如果报表有 SQL 查询，执行并返回图表数据
            if (!string.IsNullOrEmpty(report.SqlQuery))
            {
                var sql = report.SqlQuery.Trim();
                if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                {
                    var result = await _db.Ado.GetDataTableAsync(sql);
                    _logger.LogInformation($"报表SQL执行结果: 行数={result.Rows.Count}, 列数={result.Columns.Count}");

                    // 根据图表类型生成 chartData
                    return GenerateChartData(report, result);
                }
            }

            // 没有 SQL 查询，返回报表配置信息
            return new Dictionary<string, object>
            {
                ["reportId"] = report.Id,
                ["reportName"] = report.Name,
                ["chartType"] = report.ChartType
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表数据失败");
            return new Dictionary<string, object> { ["error"] = "获取失败" };
        }
    }

    /// <summary>
    /// 根据报表配置和查询结果生成图表数据
    /// </summary>
    private object GenerateChartData(Report report, System.Data.DataTable result)
    {
        var chartData = new Dictionary<string, object>();
        var chartType = report.ChartType?.ToLower() ?? "bar";

        chartData["type"] = chartType;
        chartData["title"] = report.Name;

        if (chartType == "pie")
        {
            // 饼图数据
            var pieData = new List<Dictionary<string, object>>();
            foreach (System.Data.DataRow row in result.Rows)
            {
                var item = new Dictionary<string, object>();
                // 使用第一列作为名称，第二列作为值
                if (result.Columns.Count >= 2)
                {
                    item["name"] = row[0]?.ToString() ?? "";
                    item["value"] = Convert.ToInt64(row[1]);
                }
                pieData.Add(item);
            }
            chartData["pieData"] = pieData;
        }
        else
        {
            // 柱状图/折线图数据
            var xAxis = new List<string>();
            var yAxis = new List<long>();

            foreach (System.Data.DataRow row in result.Rows)
            {
                if (result.Columns.Count >= 2)
                {
                    xAxis.Add(row[0]?.ToString() ?? "");
                    yAxis.Add(Convert.ToInt64(row[1]));
                }
            }
            chartData["xAxis"] = xAxis;
            chartData["yAxis"] = yAxis;
        }

        return new Dictionary<string, object> { ["chartData"] = chartData };
    }

    /// <summary>
    /// 获取API数据
    /// </summary>
    private async Task<object> GetApiDataAsync(string? dataSourceConfig, string? authToken)
    {
        if (string.IsNullOrEmpty(dataSourceConfig))
        {
            return new object();
        }

        try
        {
            var config = JsonSerializer.Deserialize<DataSourceConfigDto>(dataSourceConfig);
            _logger.LogInformation($"API配置: api={config?.ApiUrl}, method={config?.Method}");
            if (config == null || string.IsNullOrEmpty(config.ApiUrl))
            {
                return new object();
            }

            // 使用HttpClient调用API
            var httpClient = _httpClientFactory.CreateClient("WidgetDataClient");

            // 添加认证 Token（内部 API 需要认证）
            if (!string.IsNullOrEmpty(authToken))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);
            }

            HttpResponseMessage response;
            var method = config.Method?.ToUpper() ?? "GET";

            if (method == "POST")
            {
                // POST 请求
                var jsonContent = JsonSerializer.Serialize(config.QueryParams ?? new object());
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                response = await httpClient.PostAsync(config.ApiUrl, content);
            }
            else
            {
                // GET 请求
                response = await httpClient.GetAsync(config.ApiUrl);
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"API请求失败: {config.ApiUrl}, 方法: {method}, 状态码: {response.StatusCode}");
                return new object();
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"API响应内容: {responseContent}");

            // 解析响应，提取 data 字段（标准 API 响应格式：{code, message, data}）
            var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;

            object? apiData;
            if (root.TryGetProperty("data", out var dataElement))
            {
                // 提取 data 字段内容
                apiData = JsonSerializer.Deserialize<object>(dataElement.GetRawText());
                _logger.LogInformation($"提取data字段: {dataElement.GetRawText()}");
            }
            else
            {
                // 没有 data 字段，直接返回整个响应
                apiData = JsonSerializer.Deserialize<object>(responseContent);
            }

            // 字段映射转换（如果有配置）
            if (config.FieldMapping != null && apiData != null)
            {
                return MapFields(apiData, config.FieldMapping);
            }

            // 自动处理列表数据的字段转换（针对工作流待办等常见场景）
            if (apiData is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Object)
            {
                if (jsonElement.TryGetProperty("list", out var listElement) && listElement.ValueKind == JsonValueKind.Array)
                {
                    return NormalizeListData(listElement);
                }
            }

            return apiData ?? new object();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取API数据失败");
            return new object();
        }
    }

    /// <summary>
    /// 标准化列表数据（将不同API的字段转换为统一的 WidgetListItem 格式）
    /// </summary>
    private object NormalizeListData(JsonElement listElement)
    {
        var list = new List<Dictionary<string, object>>();

        foreach (var item in listElement.EnumerateArray())
        {
            var normalizedItem = new Dictionary<string, object>();

            // ID
            if (item.TryGetProperty("id", out var idProp))
            {
                normalizedItem["id"] = idProp.GetString() ?? "";
            }

            // 名称：优先 name，其次 instanceTitle、title
            if (item.TryGetProperty("name", out var nameProp))
            {
                normalizedItem["name"] = nameProp.GetString() ?? "";
            }
            else if (item.TryGetProperty("instanceTitle", out var titleProp))
            {
                normalizedItem["name"] = titleProp.GetString() ?? "";
            }
            else if (item.TryGetProperty("title", out var tProp))
            {
                normalizedItem["name"] = tProp.GetString() ?? "";
            }

            // 状态：优先 status，其次 nodeType
            if (item.TryGetProperty("status", out var statusProp))
            {
                normalizedItem["status"] = statusProp.GetInt32();
            }
            else if (item.TryGetProperty("nodeType", out var nodeTypeProp))
            {
                // 工作流节点类型转换为状态码
                var nodeType = nodeTypeProp.GetInt32();
                normalizedItem["status"] = MapNodeTypeToStatus(nodeType);
                normalizedItem["statusLabel"] = GetNodeTypeLabel(nodeType);
            }

            // 状态标签
            if (!normalizedItem.ContainsKey("statusLabel"))
            {
                if (item.TryGetProperty("statusLabel", out var labelProp))
                {
                    normalizedItem["statusLabel"] = labelProp.GetString() ?? "";
                }
                else if (normalizedItem.ContainsKey("status"))
                {
                    normalizedItem["statusLabel"] = GetStatusLabel((int)normalizedItem["status"]);
                }
            }

            // 时间：优先 time，其次 entryTime、createTime
            if (item.TryGetProperty("time", out var timeProp))
            {
                normalizedItem["time"] = timeProp.GetString() ?? "";
            }
            else if (item.TryGetProperty("entryTime", out var entryProp))
            {
                normalizedItem["time"] = FormatRelativeTime(entryProp.GetString());
            }
            else if (item.TryGetProperty("createTime", out var createProp))
            {
                normalizedItem["time"] = FormatRelativeTime(createProp.GetString());
            }

            list.Add(normalizedItem);
        }

        _logger.LogInformation($"标准化列表数据: {list.Count} 条");
        return new Dictionary<string, object> { ["list"] = list };
    }

    /// <summary>
    /// 工作流节点类型映射为状态码
    /// </summary>
    private int MapNodeTypeToStatus(int nodeType)
    {
        // nodeType: 1=发起人, 2=审批, 3=抄送, 4=条件, 5=服务
        // status: 0=待处理, 1=进行中, 2=已完成, 3=异常
        return nodeType == 2 ? 0 : nodeType == 5 ? 1 : 0;
    }

    /// <summary>
    /// 获取节点类型标签
    /// </summary>
    private string GetNodeTypeLabel(int nodeType)
    {
        var labels = new Dictionary<int, string>
        {
            [1] = "发起",
            [2] = "审批",
            [3] = "抄送",
            [4] = "条件",
            [5] = "服务",
        };
        return labels.TryGetValue(nodeType, out var label) ? label : "待处理";
    }

    /// <summary>
    /// 获取状态标签
    /// </summary>
    private string GetStatusLabel(int status)
    {
        var labels = new Dictionary<int, string>
        {
            [0] = "待处理",
            [1] = "进行中",
            [2] = "已完成",
            [3] = "异常",
        };
        return labels.TryGetValue(status, out var label) ? label : "未知";
    }

    /// <summary>
    /// 格式化相对时间
    /// </summary>
    private string FormatRelativeTime(string? dateTimeStr)
    {
        if (string.IsNullOrEmpty(dateTimeStr)) return "";

        try
        {
            var dateTime = DateTime.Parse(dateTimeStr);
            var diff = DateTime.Now - dateTime;
            var totalSeconds = (int)diff.TotalSeconds;

            if (totalSeconds < 60) return "刚刚";
            if (totalSeconds < 3600) return $"{totalSeconds / 60}分钟前";
            if (totalSeconds < 86400) return $"{totalSeconds / 3600}小时前";
            if (totalSeconds < 604800) return $"{totalSeconds / 86400}天前";
            return dateTime.ToString("MM-dd");
        }
        catch
        {
            return dateTimeStr;
        }
    }

    /// <summary>
    /// 获取静态数据
    /// </summary>
    private object GetStaticData(string? dataSourceConfig)
    {
        if (string.IsNullOrEmpty(dataSourceConfig))
        {
            return new object();
        }

        try
        {
            var data = JsonSerializer.Deserialize<object>(dataSourceConfig);
            return data ?? new object();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "解析静态数据失败");
            return new object();
        }
    }

    /// <summary>
    /// 字段映射转换
    /// </summary>
    private object MapFields(object sourceData, Dictionary<string, string> fieldMapping)
    {
        // 简化的字段映射处理
        if (sourceData is JsonElement jsonElement)
        {
            var result = new Dictionary<string, object>();

            foreach (var mapping in fieldMapping)
            {
                var sourceField = mapping.Key;
                var targetField = mapping.Value;

                if (jsonElement.TryGetProperty(sourceField, out var propertyValue))
                {
                    result[targetField] = GetJsonValue(propertyValue);
                }
            }

            return result;
        }

        return sourceData;
    }

    /// <summary>
    /// 从JsonElement获取值
    /// </summary>
    private object GetJsonValue(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Number:
                return element.GetInt32();
            case JsonValueKind.String:
                return element.GetString() ?? string.Empty;
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            default:
                return element.ToString();
        }
    }
}

/// <summary>
/// 数据源配置DTO（内部使用）
/// </summary>
internal class DataSourceConfigDto
{
    [JsonPropertyName("api")]
    public string? ApiUrl { get; set; }

    [JsonPropertyName("method")]
    public string? Method { get; set; }

    [JsonPropertyName("params")]
    public Dictionary<string, object>? QueryParams { get; set; }

    [JsonPropertyName("fieldMapping")]
    public Dictionary<string, string>? FieldMapping { get; set; }

    [JsonPropertyName("refreshInterval")]
    public int RefreshInterval { get; set; }
}

/// <summary>
/// SQL配置DTO（内部使用）
/// </summary>
internal class SqlConfigDto
{
    /// <summary>SQL语句（只允许SELECT）</summary>
    [JsonPropertyName("sql")]
    public string? Sql { get; set; }

    /// <summary>显示标签（用于统计卡片）</summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>刷新间隔（秒）</summary>
    [JsonPropertyName("refreshInterval")]
    public int RefreshInterval { get; set; }
}

/// <summary>
/// 报表配置DTO（内部使用）
/// </summary>
internal class ReportConfigDto
{
    /// <summary>报表ID</summary>
    [JsonPropertyName("reportId")]
    public Guid ReportId { get; set; }

    /// <summary>报表名称</summary>
    [JsonPropertyName("reportName")]
    public string? ReportName { get; set; }

    /// <summary>刷新间隔（秒）</summary>
    [JsonPropertyName("refreshInterval")]
    public int RefreshInterval { get; set; }
}

/// <summary>
/// 用户信息参数（用于SQL参数替换）
/// </summary>
internal class UserInfoParams
{
    /// <summary>用户ID</summary>
    public Guid UserId { get; set; }

    /// <summary>用户名</summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>角色ID</summary>
    public Guid RoleId { get; set; }

    /// <summary>部门ID</summary>
    public Guid DepartmentId { get; set; }

    /// <summary>真实姓名</summary>
    public string RealName { get; set; } = string.Empty;
}