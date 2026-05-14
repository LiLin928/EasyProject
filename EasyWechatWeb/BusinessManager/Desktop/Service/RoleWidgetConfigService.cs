using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Desktop.Service;

public class RoleWidgetConfigService : BaseService<RoleWidgetConfig>, IRoleWidgetConfigService
{
    public ILogger<RoleWidgetConfigService> _logger { get; set; } = null!;
    public ISqlSugarClient _db { get; set; } = null!;

    public async Task<List<RoleWidgetConfigDto>> GetByRoleIdAsync(Guid roleId)
    {
        var list = await _db.Queryable<RoleWidgetConfig, DesktopWidget>(
            (rc, w) => new JoinQueryInfos(JoinType.Left, rc.WidgetId == w.Id))
            .Where((rc, w) => rc.RoleId == roleId)
            .OrderBy((rc, w) => rc.SortOrder)
            .Select((rc, w) => new RoleWidgetConfigDto
            {
                Id = rc.Id,
                RoleId = rc.RoleId,
                WidgetId = rc.WidgetId,
                WidgetName = w.Name,
                WidgetType = w.Type,
                DefaultWidth = w.DefaultWidth,
                DefaultHeight = w.DefaultHeight,
                SortOrder = rc.SortOrder,
                IsEnabled = rc.IsEnabled
            })
            .ToListAsync();
        return list;
    }

    public async Task<bool> SaveAsync(AddRoleWidgetConfigDto dto)
    {
        // 先删除该角色的所有配置
        await _db.Deleteable<RoleWidgetConfig>()
            .Where(x => x.RoleId == dto.RoleId)
            .ExecuteCommandAsync();

        // 批量插入新配置
        var entities = dto.Widgets.Select(w => new RoleWidgetConfig
        {
            Id = Guid.NewGuid(),
            RoleId = dto.RoleId,
            WidgetId = w.WidgetId,
            SortOrder = w.SortOrder,
            IsEnabled = w.IsEnabled,
            CreateTime = DateTime.Now
        }).ToList();

        if (entities.Count > 0)
        {
            await _db.Insertable(entities).ExecuteCommandAsync();
        }
        return true;
    }

    public async Task<List<AvailableWidgetDto>> GetAvailableWidgetsAsync(Guid roleId)
    {
        // 获取所有启用的组件
        var allWidgets = await _db.Queryable<DesktopWidget>()
            .Where(x => x.Status == 1)
            .ToListAsync();

        // 获取该角色已分配的组件
        var assignedIds = await _db.Queryable<RoleWidgetConfig>()
            .Where(x => x.RoleId == roleId && x.IsEnabled)
            .Select(x => x.WidgetId)
            .ToListAsync();

        return allWidgets.Select(w => new AvailableWidgetDto
        {
            Id = w.Id,
            Name = w.Name,
            Type = (int)w.Type,
            Icon = w.Icon,
            DefaultWidth = w.DefaultWidth,
            DefaultHeight = w.DefaultHeight,
            IsUserEnabled = assignedIds.Contains(w.Id)
        }).ToList();
    }
}