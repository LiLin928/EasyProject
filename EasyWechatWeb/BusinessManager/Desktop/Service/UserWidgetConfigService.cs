using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Desktop.Service;

public class UserWidgetConfigService : BaseService<UserWidgetConfig>, IUserWidgetConfigService
{
    public ILogger<UserWidgetConfigService> _logger { get; set; } = null!;
    public ISqlSugarClient _db { get; set; } = null!;

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
}