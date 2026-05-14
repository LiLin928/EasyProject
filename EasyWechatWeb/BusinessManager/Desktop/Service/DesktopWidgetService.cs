using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Desktop.Service;

public class DesktopWidgetService : BaseService<DesktopWidget>, IDesktopWidgetService
{
    public ILogger<DesktopWidgetService> _logger { get; set; } = null!;
    public ISqlSugarClient _db { get; set; } = null!;

    public async Task<(List<DesktopWidgetDto> list, int total)> GetListAsync(QueryDesktopWidgetDto query)
    {
        var queryBuilder = _db.Queryable<DesktopWidget>()
            .WhereIF(!string.IsNullOrEmpty(query.Name), x => x.Name.Contains(query.Name!))
            .WhereIF(query.Type.HasValue, x => x.Type == query.Type!.Value)
            .WhereIF(query.Status.HasValue, x => x.Status == query.Status!.Value)
            .OrderByDescending(x => x.CreateTime);

        var total = await queryBuilder.CountAsync();
        var list = await queryBuilder
            .ToPageListAsync(query.PageIndex, query.PageSize);

        return (list.Adapt<List<DesktopWidgetDto>>(), total);
    }

    public async Task<DesktopWidgetDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<DesktopWidget>()
            .Where(x => x.Id == id)
            .FirstAsync();
        return entity?.Adapt<DesktopWidgetDto>();
    }

    public async Task<Guid> AddAsync(AddDesktopWidgetDto dto)
    {
        var entity = dto.Adapt<DesktopWidget>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(UpdateDesktopWidgetDto dto)
    {
        var entity = await _db.Queryable<DesktopWidget>()
            .Where(x => x.Id == dto.Id)
            .FirstAsync();
        if (entity == null) return false;

        dto.Adapt(entity);
        entity.UpdateTime = DateTime.Now;
        await _db.Updateable(entity).ExecuteCommandAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _db.Deleteable<DesktopWidget>()
            .Where(x => x.Id == id)
            .ExecuteCommandAsync();
        return result > 0;
    }

    public async Task<List<DesktopWidgetDto>> GetEnabledListAsync()
    {
        var list = await _db.Queryable<DesktopWidget>()
            .Where(x => x.Status == 1)
            .OrderBy(x => x.CreateTime)
            .ToListAsync();
        return list.Adapt<List<DesktopWidgetDto>>();
    }
}