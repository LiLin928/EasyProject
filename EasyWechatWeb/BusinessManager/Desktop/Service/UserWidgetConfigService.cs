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
    public ISqlSugarClient _db { get; set; } = null!;
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
    public async Task<object> GetWidgetDataAsync(Guid widgetId, Guid userId)
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

        // 根据数据源类型获取数据
        switch (widget.DataSourceType)
        {
            case DataSourceType.Api:
                return await GetApiDataAsync(widget.DataSourceConfig);

            case DataSourceType.Static:
                return GetStaticData(widget.DataSourceConfig);

            case DataSourceType.Sql:
                return await GetSqlDataAsync(widget.DataSourceConfig);

            case DataSourceType.Report:
                return await GetReportDataAsync(widget.DataSourceConfig);

            default:
                return new object();
        }
    }

    /// <summary>
    /// 执行SQL获取数据
    /// </summary>
    private async Task<object> GetSqlDataAsync(string? dataSourceConfig)
    {
        if (string.IsNullOrEmpty(dataSourceConfig))
        {
            return new Dictionary<string, object>();
        }

        try
        {
            var config = JsonSerializer.Deserialize<SqlConfigDto>(dataSourceConfig);
            if (config == null || string.IsNullOrEmpty(config.Sql))
            {
                return new Dictionary<string, object>();
            }

            // 安全检查：只允许 SELECT 语句
            var sql = config.Sql.Trim();
            if (!sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning($"非法SQL语句，只允许SELECT: {sql}");
                return new Dictionary<string, object> { ["value"] = 0, ["label"] = "SQL不允许" };
            }

            // 执行 SQL 查询
            var result = await _db.Ado.GetDataTableAsync(sql);

            // 对于 COUNT 查询，返回单值
            if (result.Rows.Count > 0 && result.Columns.Count == 1)
            {
                var value = result.Rows[0][0];
                return new Dictionary<string, object>
                {
                    ["value"] = Convert.ToInt64(value),
                    ["label"] = config.Label ?? "数量"
                };
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

            return new Dictionary<string, object> { ["list"] = list };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行SQL失败");
            return new Dictionary<string, object> { ["value"] = 0, ["label"] = "执行失败" };
        }
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
            if (config == null || config.ReportId == Guid.Empty)
            {
                return new Dictionary<string, object>();
            }

            // 获取报表信息（使用 ColumnTemplate 作为报表模板）
            var report = await _db.Queryable<ColumnTemplate>()
                .Where(x => x.Id == config.ReportId)
                .FirstAsync();

            if (report == null)
            {
                return new Dictionary<string, object> { ["error"] = "报表不存在" };
            }

            // 返回报表信息，前端根据此渲染
            return new Dictionary<string, object>
            {
                ["reportId"] = report.Id,
                ["reportName"] = report.Name,
                ["reportType"] = report.Type,
                ["config"] = report.ColumnConfigs
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取报表数据失败");
            return new Dictionary<string, object> { ["error"] = "获取失败" };
        }
    }

    /// <summary>
    /// 获取API数据
    /// </summary>
    private async Task<object> GetApiDataAsync(string? dataSourceConfig)
    {
        if (string.IsNullOrEmpty(dataSourceConfig))
        {
            return new object();
        }

        try
        {
            var config = JsonSerializer.Deserialize<DataSourceConfigDto>(dataSourceConfig);
            if (config == null || string.IsNullOrEmpty(config.ApiUrl))
            {
                return new object();
            }

            // 使用HttpClient调用API
            var httpClient = _httpClientFactory.CreateClient("WidgetDataClient");
            var response = await httpClient.GetAsync(config.ApiUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"API请求失败: {config.ApiUrl}, 状态码: {response.StatusCode}");
                return new object();
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiData = JsonSerializer.Deserialize<object>(content);

            // 字段映射转换
            if (config.FieldMapping != null && apiData != null)
            {
                return MapFields(apiData, config.FieldMapping);
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

    [JsonPropertyName("params")]
    public Dictionary<string, object>? QueryParams { get; set; }

    public Dictionary<string, string>? FieldMapping { get; set; }
    public int RefreshInterval { get; set; }
}

/// <summary>
/// SQL配置DTO（内部使用）
/// </summary>
internal class SqlConfigDto
{
    /// <summary>SQL语句（只允许SELECT）</summary>
    public string? Sql { get; set; }

    /// <summary>显示标签（用于统计卡片）</summary>
    public string? Label { get; set; }

    /// <summary>刷新间隔（秒）</summary>
    public int RefreshInterval { get; set; }
}

/// <summary>
/// 报表配置DTO（内部使用）
/// </summary>
internal class ReportConfigDto
{
    /// <summary>报表ID</summary>
    public Guid ReportId { get; set; }

    /// <summary>刷新间隔（秒）</summary>
    public int RefreshInterval { get; set; }
}