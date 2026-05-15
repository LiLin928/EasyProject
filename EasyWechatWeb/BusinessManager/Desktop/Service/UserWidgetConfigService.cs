using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Text.Json;

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

        // 获取用户角色
        var userRole = await _db.Queryable<UserRole>()
            .Where(x => x.UserId == userId)
            .FirstAsync();

        var availableWidgets = new List<AvailableWidgetDto>();
        if (userRole != null)
        {
            // 获取角色可用的组件
            availableWidgets = await _db.Queryable<RoleWidgetConfig, DesktopWidget>(
                (rc, w) => new JoinQueryInfos(JoinType.Left, rc.WidgetId == w.Id))
                .Where((rc, w) => rc.RoleId == userRole.RoleId && rc.IsEnabled && w.Status == 1)
                .Select((rc, w) => new AvailableWidgetDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Type = (int)w.Type,
                    Icon = w.Icon,
                    DefaultWidth = w.DefaultWidth,
                    DefaultHeight = w.DefaultHeight,
                    IsUserEnabled = userConfigs.Any(uc => uc.WidgetId == w.Id)
                })
                .ToListAsync();
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

            case DataSourceType.Statistics:
                return await GetStatisticsDataAsync(widget.DataSourceConfig);

            default:
                return new object();
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
    /// 获取统计数据（从数据库查询）
    /// </summary>
    private async Task<object> GetStatisticsDataAsync(string? dataSourceConfig)
    {
        if (string.IsNullOrEmpty(dataSourceConfig))
        {
            return new object();
        }

        try
        {
            var config = JsonSerializer.Deserialize<StatisticsConfigDto>(dataSourceConfig);
            if (config == null)
            {
                return new object();
            }

            // 根据配置查询数据库
            // 示例：查询订单数量
            if (config.TableName == "Order")
            {
                var query = _db.Queryable<Order>();

                // 应用条件过滤
                if (config.Conditions != null)
                {
                    foreach (var condition in config.Conditions)
                    {
                        // 简化的条件处理
                        if (condition.Field == "Status" && int.TryParse(condition.Value, out var status))
                        {
                            query = query.Where(x => x.Status == status);
                        }
                    }
                }

                var count = await query.CountAsync();

                return new
                {
                    value = count,
                    label = config.Label ?? "数量"
                };
            }

            // 其他统计类型可以根据需要扩展
            return new object();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取统计数据失败");
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
    public string? ApiUrl { get; set; }
    public Dictionary<string, string>? FieldMapping { get; set; }
    public int RefreshInterval { get; set; }
    public Dictionary<string, object>? QueryParams { get; set; }
}

/// <summary>
/// 统计配置DTO（内部使用）
/// </summary>
internal class StatisticsConfigDto
{
    public string? TableName { get; set; }
    public string? Label { get; set; }
    public List<ConditionDto>? Conditions { get; set; }
}

/// <summary>
/// 条件DTO（内部使用）
/// </summary>
internal class ConditionDto
{
    public string? Field { get; set; }
    public string? Operator { get; set; }
    public string? Value { get; set; }
}