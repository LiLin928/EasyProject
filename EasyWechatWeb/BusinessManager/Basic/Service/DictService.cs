using BusinessManager.Basic.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using SqlSugar;

namespace BusinessManager.Basic.Service;

/// <summary>
/// 字典服务实现
/// </summary>
public class DictService : BaseService, IDictService
{
    #region 字典类型

    /// <summary>
    /// 获取字典类型分页列表
    /// </summary>
    public async Task<PageResponse<DictTypeDto>> GetDictTypePageListAsync(QueryDictTypeDto query)
    {
        var queryable = _db.Queryable<DictType>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(!string.IsNullOrEmpty(query.Code), t => t.Code.Contains(query.Code!))
            .WhereIF(!string.IsNullOrEmpty(query.Name), t => t.Name.Contains(query.Name!))
            .WhereIF(query.Status.HasValue, t => t.Status == query.Status!.Value)
            .OrderByDescending(t => t.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 转换 DTO
        var dtoList = list.Select(t => new DictTypeDto
        {
            Id = t.Id,
            Code = t.Code,
            Name = t.Name,
            Description = t.Description,
            Status = t.Status,
            Version = t.Version,
            LabelZh = t.LabelZh,
            LabelEn = t.LabelEn,
            CreateTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = t.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PageResponse<DictTypeDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取所有字典类型列表（不分页）
    /// </summary>
    public async Task<List<DictTypeDto>> GetDictTypeListAsync()
    {
        var list = await _db.Queryable<DictType>()
            .Where(t => t.Status == 1)
            .OrderBy(t => t.Code)
            .ToListAsync();

        return list.Select(t => new DictTypeDto
        {
            Id = t.Id,
            Code = t.Code,
            Name = t.Name,
            Description = t.Description,
            Status = t.Status,
            Version = t.Version,
            LabelZh = t.LabelZh,
            LabelEn = t.LabelEn,
            CreateTime = t.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = t.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();
    }

    /// <summary>
    /// 根据ID获取字典类型详情
    /// </summary>
    public async Task<DictTypeDto?> GetDictTypeByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<DictType>()
            .FirstAsync(t => t.Id == id);

        if (entity == null) return null;

        return new DictTypeDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status,
            Version = entity.Version,
            LabelZh = entity.LabelZh,
            LabelEn = entity.LabelEn,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = entity.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 根据编码获取字典类型
    /// </summary>
    public async Task<DictTypeDto?> GetDictTypeByCodeAsync(string code)
    {
        var entity = await _db.Queryable<DictType>()
            .FirstAsync(t => t.Code == code);

        if (entity == null) return null;

        return new DictTypeDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Description = entity.Description,
            Status = entity.Status,
            Version = entity.Version,
            LabelZh = entity.LabelZh,
            LabelEn = entity.LabelEn,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = entity.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 添加字典类型
    /// </summary>
    public async Task<Guid> AddDictTypeAsync(AddDictTypeDto dto)
    {
        // 检查编码是否已存在
        var exists = await _db.Queryable<DictType>().AnyAsync(t => t.Code == dto.Code);
        if (exists)
        {
            throw new CommonManager.Error.BusinessException($"字典编码 {dto.Code} 已存在");
        }

        var entity = new DictType
        {
            Id = Guid.NewGuid(),
            Code = dto.Code,
            Name = dto.Name,
            Description = dto.Description,
            Status = dto.Status,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 更新字典类型
    /// </summary>
    public async Task<int> UpdateDictTypeAsync(UpdateDictTypeDto dto)
    {
        var entity = await _db.Queryable<DictType>().FirstAsync(t => t.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("字典类型不存在");
        }

        // 更新字段
        if (!string.IsNullOrEmpty(dto.Name))
        {
            entity.Name = dto.Name;
        }
        if (dto.Description != null)
        {
            entity.Description = dto.Description;
        }
        if (dto.Status.HasValue)
        {
            entity.Status = dto.Status.Value;
        }
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 删除字典类型
    /// </summary>
    public async Task<int> DeleteDictTypeAsync(Guid id)
    {
        // 获取字典类型信息
        var dictType = await _db.Queryable<DictType>().FirstAsync(t => t.Id == id);
        if (dictType == null)
        {
            return 0;
        }

        // 删除关联的字典数据
        await _db.Deleteable<DictData>().Where(d => d.TypeCode == dictType.Code).ExecuteCommandAsync();

        // 删除字典类型
        return await _db.Deleteable<DictType>().Where(t => t.Id == id).ExecuteCommandAsync();
    }

    #endregion

    #region 字典数据

    /// <summary>
    /// 获取字典数据分页列表
    /// </summary>
    public async Task<PageResponse<DictDataDto>> GetDictDataPageListAsync(QueryDictDataDto query)
    {
        var queryable = _db.Queryable<DictData>()
            .Where(d => d.TypeCode == query.TypeCode)
            // 条件筛选 - 使用 WhereIF
            .WhereIF(query.Status.HasValue, d => d.Status == query.Status!.Value)
            .OrderBy(d => d.Sort)
            .OrderByDescending(d => d.CreateTime);

        // 分页
        var total = new RefAsync<int>();
        var list = await queryable.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 转换 DTO
        var dtoList = list.Select(d => new DictDataDto
        {
            Id = d.Id,
            TypeCode = d.TypeCode,
            Label = d.Label,
            Value = d.Value,
            Sort = d.Sort,
            Status = d.Status,
            LabelZh = d.LabelZh,
            LabelEn = d.LabelEn,
            CreateTime = d.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = d.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PageResponse<DictDataDto>.Create(dtoList, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 根据ID获取字典数据详情
    /// </summary>
    public async Task<DictDataDto?> GetDictDataByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<DictData>()
            .FirstAsync(d => d.Id == id);

        if (entity == null) return null;

        return new DictDataDto
        {
            Id = entity.Id,
            TypeCode = entity.TypeCode,
            Label = entity.Label,
            Value = entity.Value,
            Sort = entity.Sort,
            Status = entity.Status,
            LabelZh = entity.LabelZh,
            LabelEn = entity.LabelEn,
            CreateTime = entity.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = entity.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    /// <summary>
    /// 根据类型编码获取字典数据列表（用于下拉选项）
    /// </summary>
    public async Task<List<DictDataDto>> GetDictDataByCodeAsync(string code)
    {
        var list = await _db.Queryable<DictData>()
            .Where(d => d.TypeCode == code && d.Status == 1)
            .OrderBy(d => d.Sort)
            .ToListAsync();

        return list.Select(d => new DictDataDto
        {
            Id = d.Id,
            TypeCode = d.TypeCode,
            Label = d.Label,
            Value = d.Value,
            Sort = d.Sort,
            Status = d.Status,
            LabelZh = d.LabelZh,
            LabelEn = d.LabelEn,
            CreateTime = d.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = d.UpdateTime?.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();
    }

    /// <summary>
    /// 添加字典数据
    /// </summary>
    public async Task<Guid> AddDictDataAsync(AddDictDataDto dto)
    {
        // 检查字典类型是否存在
        var dictType = await _db.Queryable<DictType>().FirstAsync(t => t.Code == dto.TypeCode);
        if (dictType == null)
        {
            throw new CommonManager.Error.BusinessException($"字典类型 {dto.TypeCode} 不存在");
        }

        var entity = new DictData
        {
            Id = Guid.NewGuid(),
            TypeCode = dto.TypeCode,
            Label = dto.Label,
            LabelZh = dto.LabelZh ?? dto.Label,  // 如果未提供 LabelZh，使用 Label
            LabelEn = dto.LabelEn,
            Value = dto.Value,
            Sort = dto.Sort,
            Status = dto.Status,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();

        // 递增字典类型版本号
        dictType.Version += 1;
        dictType.UpdateTime = DateTime.Now;
        await _db.Updateable(dictType).ExecuteCommandAsync();

        return entity.Id;
    }

    /// <summary>
    /// 更新字典数据
    /// </summary>
    public async Task<int> UpdateDictDataAsync(UpdateDictDataDto dto)
    {
        var entity = await _db.Queryable<DictData>().FirstAsync(d => d.Id == dto.Id);
        if (entity == null)
        {
            throw new CommonManager.Error.BusinessException("字典数据不存在");
        }

        // 更新字段
        if (!string.IsNullOrEmpty(dto.Label))
        {
            entity.Label = dto.Label;
        }
        if (!string.IsNullOrEmpty(dto.LabelZh))
        {
            entity.LabelZh = dto.LabelZh;
        }
        if (!string.IsNullOrEmpty(dto.LabelEn))
        {
            entity.LabelEn = dto.LabelEn;
        }
        if (!string.IsNullOrEmpty(dto.Value))
        {
            entity.Value = dto.Value;
        }
        if (dto.Sort.HasValue)
        {
            entity.Sort = dto.Sort.Value;
        }
        if (dto.Status.HasValue)
        {
            entity.Status = dto.Status.Value;
        }
        entity.UpdateTime = DateTime.Now;

        var result = await _db.Updateable(entity).ExecuteCommandAsync();

        // 递增字典类型版本号
        var dictType = await _db.Queryable<DictType>().FirstAsync(t => t.Code == entity.TypeCode);
        if (dictType != null)
        {
            dictType.Version += 1;
            dictType.UpdateTime = DateTime.Now;
            await _db.Updateable(dictType).ExecuteCommandAsync();
        }

        return result;
    }

    /// <summary>
    /// 删除字典数据
    /// </summary>
    public async Task<int> DeleteDictDataAsync(Guid id)
    {
        // 获取字典数据以获取 TypeCode
        var dictData = await _db.Queryable<DictData>().FirstAsync(d => d.Id == id);
        if (dictData == null) return 0;

        var result = await _db.Deleteable<DictData>().Where(d => d.Id == id).ExecuteCommandAsync();

        // 递增字典类型版本号
        var dictType = await _db.Queryable<DictType>().FirstAsync(t => t.Code == dictData.TypeCode);
        if (dictType != null)
        {
            dictType.Version += 1;
            dictType.UpdateTime = DateTime.Now;
            await _db.Updateable(dictType).ExecuteCommandAsync();
        }

        return result;
    }

    /// <summary>
    /// 批量删除字典数据
    /// </summary>
    public async Task<int> DeleteDictDataBatchAsync(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0) return 0;

        // 获取要删除的字典数据以获取 TypeCode
        var dictDataList = await _db.Queryable<DictData>()
            .Where(d => ids.Contains(d.Id))
            .ToListAsync();

        if (dictDataList.Count == 0) return 0;

        var result = await _db.Deleteable<DictData>()
            .Where(d => ids.Contains(d.Id))
            .ExecuteCommandAsync();

        // 递增所有涉及的字典类型版本号
        var typeCodes = dictDataList.Select(d => d.TypeCode).Distinct().ToList();
        var dictTypes = await _db.Queryable<DictType>()
            .Where(t => typeCodes.Contains(t.Code))
            .ToListAsync();

        foreach (var dictType in dictTypes)
        {
            dictType.Version += 1;
            dictType.UpdateTime = DateTime.Now;
        }

        if (dictTypes.Count > 0)
        {
            await _db.Updateable(dictTypes).ExecuteCommandAsync();
        }

        return result;
    }

    #endregion

    #region 批量获取与版本检查

    /// <summary>
    /// 批量获取字典数据（含版本信息）
    /// </summary>
    public async Task<Dictionary<string, DictDataWithVersionDto>> GetDictDataBatchAsync(List<string> codes)
    {
        var result = new Dictionary<string, DictDataWithVersionDto>();

        foreach (var code in codes)
        {
            var dictData = await GetDictDataWithVersionByCodeAsync(code);
            if (dictData != null)
            {
                result[code] = dictData;
            }
        }

        return result;
    }

    /// <summary>
    /// 检查字典版本
    /// </summary>
    public async Task<VersionCheckResponse> CheckDictVersionAsync(Dictionary<string, int> localVersions)
    {
        var response = new VersionCheckResponse();

        // 获取所有涉及的字典类型
        var codes = localVersions.Keys.ToList();
        var dictTypes = await _db.Queryable<DictType>()
            .Where(t => codes.Contains(t.Code))
            .ToListAsync();

        foreach (var dictType in dictTypes)
        {
            response.AllVersions[dictType.Code] = dictType.Version;

            // 如果本地版本低于服务器版本，需要刷新
            if (localVersions.TryGetValue(dictType.Code, out var localVersion))
            {
                if (localVersion < dictType.Version)
                {
                    response.NeedRefresh.Add(dictType.Code);
                }
            }
            else
            {
                // 本地没有此字典，需要刷新
                response.NeedRefresh.Add(dictType.Code);
            }
        }

        return response;
    }

    /// <summary>
    /// 根据编码获取字典数据（含版本信息）
    /// </summary>
    public async Task<DictDataWithVersionDto?> GetDictDataWithVersionByCodeAsync(string code)
    {
        // 获取字典类型（含版本）
        var dictType = await _db.Queryable<DictType>()
            .FirstAsync(t => t.Code == code);

        if (dictType == null) return null;

        // 获取字典数据
        var dataList = await _db.Queryable<DictData>()
            .Where(d => d.TypeCode == code && d.Status == 1)
            .OrderBy(d => d.Sort)
            .ToListAsync();

        return new DictDataWithVersionDto
        {
            Version = dictType.Version,
            Items = dataList.Select(d => new DictDataItemDto
            {
                Value = d.Value,
                LabelZh = d.LabelZh,
                LabelEn = d.LabelEn,
                Sort = d.Sort,
                Status = d.Status
            }).ToList()
        };
    }

    #endregion
}