using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using SqlSugar;
using System.Text.Json;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 列配置模板服务实现
/// </summary>
public class ColumnTemplateService : BaseService<ColumnTemplate>, IColumnTemplateService
{
    public ILogger<ColumnTemplateService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取列模板列表（分页）
    /// </summary>
    public async Task<PageResponse<ColumnTemplateDto>> GetPageListAsync(QueryColumnTemplateDto query)
    {
        var total = new RefAsync<int>();
        var list = await _db.Queryable<ColumnTemplate>()
            .WhereIF(!string.IsNullOrEmpty(query.Name), t => t.Name.Contains(query.Name!))
            .WhereIF(!string.IsNullOrEmpty(query.Type), t => t.Type == query.Type)
            .OrderByDescending(t => t.CreateTime)
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        var dtos = list.Select(t => new ColumnTemplateDto
        {
            Id = t.Id,
            Name = t.Name,
            Type = t.Type,
            Description = t.Description,
            DatasourceId = t.DatasourceId,
            SqlQuery = t.SqlQuery,
            Columns = JsonSerializer.Deserialize<List<ColumnConfigDto>>(t.ColumnConfigs) ?? new(),
            CreateTime = t.CreateTime,
            UpdateTime = t.UpdateTime
        }).ToList();

        return PageResponse<ColumnTemplateDto>.Create(dtos, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取列模板详情
    /// </summary>
    public new async Task<ColumnTemplateDto?> GetByIdAsync(Guid id)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null) return null;

        return new ColumnTemplateDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Type = entity.Type,
            Description = entity.Description,
            DatasourceId = entity.DatasourceId,
            SqlQuery = entity.SqlQuery,
            Columns = JsonSerializer.Deserialize<List<ColumnConfigDto>>(entity.ColumnConfigs) ?? new(),
            CreateTime = entity.CreateTime,
            UpdateTime = entity.UpdateTime
        };
    }

    /// <summary>
    /// 获取单列模板列表
    /// </summary>
    public async Task<List<ColumnTemplateDto>> GetSingleTemplatesAsync()
    {
        var list = await _db.Queryable<ColumnTemplate>()
            .Where(t => t.Type == "single")
            .OrderBy(t => t.Name)
            .ToListAsync();

        return list.Select(t => new ColumnTemplateDto
        {
            Id = t.Id,
            Name = t.Name,
            Type = t.Type,
            Columns = JsonSerializer.Deserialize<List<ColumnConfigDto>>(t.ColumnConfigs) ?? new()
        }).ToList();
    }

    /// <summary>
    /// 添加列模板
    /// </summary>
    public async Task<Guid> AddAsync(CreateColumnTemplateDto dto, Guid creatorId)
    {
        var exists = await GetFirstAsync(t => t.Name == dto.Name);
        if (exists != null)
        {
            throw BusinessException.BadRequest("模板名称已存在");
        }

        var entity = new ColumnTemplate
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Type = dto.Type,
            Description = dto.Description,
            DatasourceId = dto.DatasourceId,
            SqlQuery = dto.SqlQuery,
            ColumnConfigs = JsonSerializer.Serialize(dto.Columns),
            CreatorId = creatorId,
            CreateTime = DateTime.Now
        };

        return await InsertAsync(entity);
    }

    /// <summary>
    /// 更新列模板
    /// </summary>
    public async Task<int> UpdateAsync(UpdateColumnTemplateDto dto)
    {
        var entity = await base.GetByIdAsync(dto.Id);
        if (entity == null)
        {
            throw BusinessException.NotFound("模板不存在");
        }

        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.Description != null) entity.Description = dto.Description;
        if (dto.Columns != null) entity.ColumnConfigs = JsonSerializer.Serialize(dto.Columns);
        entity.UpdateTime = DateTime.Now;

        return await base.UpdateAsync(entity);
    }

    /// <summary>
    /// 删除列模板
    /// </summary>
    public new async Task<int> DeleteAsync(Guid id)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null)
        {
            throw BusinessException.NotFound("模板不存在");
        }

        return await base.DeleteAsync(id);
    }

    /// <summary>
    /// 从SQL获取列信息
    /// </summary>
    public async Task<List<DetectedColumnDto>> FetchColumnsAsync(FetchColumnsDto dto)
    {
        var datasource = await _db.Queryable<Datasource>()
            .Where(d => d.Id == dto.DatasourceId)
            .FirstAsync();

        if (datasource == null)
        {
            throw BusinessException.NotFound("数据源不存在");
        }

        var columns = new List<DetectedColumnDto>();

        try
        {
            if (datasource.Type == "mysql")
            {
                var connectionString = $"Server={datasource.Host};Port={datasource.Port};Database={datasource.Database};User ID={datasource.Username};Password={datasource.Password};Charset=utf8mb4;";
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                // 解析SQL获取列信息
                var wrappedSql = $"SELECT * FROM ({dto.SqlQuery}) AS _tmp LIMIT 0";
                using var cmd = new MySqlCommand(wrappedSql, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                var schema = reader.GetColumnSchema();
                foreach (var col in schema)
                {
                    columns.Add(new DetectedColumnDto
                    {
                        Field = col.ColumnName,
                        Type = col.DataTypeName ?? "string"
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取列信息失败: DatasourceId={DatasourceId}", dto.DatasourceId);
            throw BusinessException.BadRequest($"获取列信息失败: {ex.Message}");
        }

        return columns;
    }
}