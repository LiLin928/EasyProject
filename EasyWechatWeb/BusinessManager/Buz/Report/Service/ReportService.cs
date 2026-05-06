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
/// 报表服务实现
/// </summary>
public class ReportService : BaseService<Report>, IReportService
{
    public ILogger<ReportService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取报表列表（分页）
    /// </summary>
    public async Task<PageResponse<ReportDto>> GetPageListAsync(QueryReportDto query)
    {
        var total = new RefAsync<int>();
        var list = await _db.Queryable<Report, Datasource, User>(
            (r, d, u) => new JoinQueryInfos(
                JoinType.Left, r.DatasourceId == d.Id,
                JoinType.Left, r.CreatorId == u.Id
            ))
            .WhereIF(!string.IsNullOrEmpty(query.Name), (r, d, u) => r.Name.Contains(query.Name!))
            .WhereIF(!string.IsNullOrEmpty(query.Category), (r, d, u) => r.Category == query.Category)
            .OrderByDescending((r, d, u) => r.CreateTime)
            .Select((r, d, u) => new ReportDto
            {
                Id = r.Id,
                Name = r.Name,
                Category = r.Category,
                DatasourceId = r.DatasourceId,
                DatasourceName = d.Name,
                ChartType = r.ChartType,
                ShowChart = r.ShowChart,
                ShowTable = r.ShowTable,
                Creator = u.RealName ?? u.UserName,
                CreateTime = r.CreateTime,
                UpdateTime = r.UpdateTime
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<ReportDto>.Create(list, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取报表详情
    /// </summary>
    public new async Task<ReportDto?> GetByIdAsync(Guid id)
    {
        var report = await _db.Queryable<Report>()
            .Where(r => r.Id == id)
            .FirstAsync();

        if (report == null)
        {
            return null;
        }

        var datasource = await _db.Queryable<Datasource>()
            .Where(d => d.Id == report.DatasourceId)
            .FirstAsync();

        var entity = new ReportDto
        {
            Id = report.Id,
            Name = report.Name,
            Category = report.Category,
            DatasourceId = report.DatasourceId,
            DatasourceName = datasource?.Name,
            SqlQuery = report.SqlQuery,
            ShowChart = report.ShowChart,
            ShowTable = report.ShowTable,
            ChartType = report.ChartType,
            XAxisField = report.XAxisField,
            YAxisField = report.YAxisField,
            Aggregation = report.Aggregation,
            AutoColumns = report.AutoColumns,
            ColumnTemplateId = report.ColumnTemplateId,
            ColumnConfigs = string.IsNullOrEmpty(report.ColumnConfigs) ? null : JsonSerializer.Deserialize<List<ColumnConfigDto>>(report.ColumnConfigs),
            CreateTime = report.CreateTime,
            UpdateTime = report.UpdateTime
        };

        return entity;
    }

    /// <summary>
    /// 添加报表
    /// </summary>
    public async Task<Guid> AddAsync(CreateReportDto dto, Guid creatorId)
    {
        var exists = await GetFirstAsync(r => r.Name == dto.Name);
        if (exists != null)
        {
            throw BusinessException.BadRequest("报表名称已存在");
        }

        var entity = new Report
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Category = dto.Category,
            DatasourceId = dto.DatasourceId,
            SqlQuery = dto.SqlQuery,
            ShowChart = dto.ShowChart,
            ShowTable = dto.ShowTable,
            ChartType = dto.ChartType,
            XAxisField = dto.XAxisField,
            YAxisField = dto.YAxisField,
            Aggregation = dto.Aggregation,
            AutoColumns = dto.AutoColumns,
            ColumnTemplateId = dto.ColumnTemplateId,
            ColumnConfigs = dto.ColumnConfigs != null ? JsonSerializer.Serialize(dto.ColumnConfigs) : null,
            CreatorId = creatorId,
            CreateTime = DateTime.Now
        };

        return await InsertAsync(entity);
    }

    /// <summary>
    /// 更新报表
    /// </summary>
    public async Task<int> UpdateAsync(UpdateReportDto dto)
    {
        var entity = await base.GetByIdAsync(dto.Id);
        if (entity == null)
        {
            throw BusinessException.NotFound("报表不存在");
        }

        if (dto.Name != null) entity.Name = dto.Name;
        if (dto.Category != null) entity.Category = dto.Category;
        if (dto.DatasourceId.HasValue) entity.DatasourceId = dto.DatasourceId.Value;
        if (dto.SqlQuery != null) entity.SqlQuery = dto.SqlQuery;
        if (dto.ShowChart.HasValue) entity.ShowChart = dto.ShowChart.Value;
        if (dto.ShowTable.HasValue) entity.ShowTable = dto.ShowTable.Value;
        if (dto.ChartType != null) entity.ChartType = dto.ChartType;
        if (dto.XAxisField != null) entity.XAxisField = dto.XAxisField;
        if (dto.YAxisField != null) entity.YAxisField = dto.YAxisField;
        if (dto.Aggregation != null) entity.Aggregation = dto.Aggregation;
        if (dto.AutoColumns.HasValue) entity.AutoColumns = dto.AutoColumns.Value;
        if (dto.ColumnTemplateId.HasValue) entity.ColumnTemplateId = dto.ColumnTemplateId;
        if (dto.ColumnConfigs != null) entity.ColumnConfigs = JsonSerializer.Serialize(dto.ColumnConfigs);
        entity.UpdateTime = DateTime.Now;

        return await base.UpdateAsync(entity);
    }

    /// <summary>
    /// 删除报表
    /// </summary>
    public new async Task<int> DeleteAsync(Guid id)
    {
        var entity = await base.GetByIdAsync(id);
        if (entity == null)
        {
            throw BusinessException.NotFound("报表不存在");
        }

        return await base.DeleteAsync(id);
    }

    /// <summary>
    /// 预览报表
    /// </summary>
    public async Task<PreviewResultDto> PreviewAsync(PreviewReportDto dto)
    {
        var datasource = await _db.Queryable<Datasource>()
            .Where(d => d.Id == dto.DatasourceId)
            .FirstAsync();

        if (datasource == null)
        {
            throw BusinessException.NotFound("数据源不存在");
        }

        return await ExecuteQueryAsync(datasource, dto.SqlQuery, dto.ChartType, dto.XAxisField, dto.YAxisField, dto.Aggregation);
    }

    /// <summary>
    /// 执行报表查询
    /// </summary>
    public async Task<PreviewResultDto> ExecuteAsync(Guid id)
    {
        var report = await base.GetByIdAsync(id);
        if (report == null)
        {
            throw BusinessException.NotFound("报表不存在");
        }

        var datasource = await _db.Queryable<Datasource>()
            .Where(d => d.Id == report.DatasourceId)
            .FirstAsync();

        if (datasource == null)
        {
            throw BusinessException.NotFound("数据源不存在");
        }

        return await ExecuteQueryAsync(datasource, report.SqlQuery, report.ChartType, report.XAxisField, report.YAxisField, report.Aggregation);
    }

    /// <summary>
    /// 获取报表分类列表
    /// </summary>
    public Task<List<ReportCategoryDto>> GetCategoriesAsync()
    {
        var categories = new List<ReportCategoryDto>
        {
            new ReportCategoryDto { Id = "sales", Name = "销售报表" },
            new ReportCategoryDto { Id = "product", Name = "商品报表" },
            new ReportCategoryDto { Id = "customer", Name = "客户报表" },
            new ReportCategoryDto { Id = "finance", Name = "财务报表" },
            new ReportCategoryDto { Id = "inventory", Name = "库存报表" }
        };
        return Task.FromResult(categories);
    }

    /// <summary>
    /// 获取发布报表数据（公开访问）
    /// </summary>
    public async Task<PublishReportDto?> GetPublishDataAsync(Guid id)
    {
        var report = await base.GetByIdAsync(id);
        if (report == null)
        {
            return null;
        }

        var datasource = await _db.Queryable<Datasource>()
            .Where(d => d.Id == report.DatasourceId)
            .FirstAsync();

        if (datasource == null)
        {
            return null;
        }

        // 执行查询获取数据
        var previewResult = await ExecuteQueryAsync(datasource, report.SqlQuery, report.ChartType, report.XAxisField, report.YAxisField, report.Aggregation);

        // 构建发布数据
        var publishData = new PublishReportDto
        {
            Report = new ReportDto
            {
                Id = report.Id,
                Name = report.Name,
                Category = report.Category,
                DatasourceId = report.DatasourceId,
                SqlQuery = report.SqlQuery,
                ShowChart = report.ShowChart,
                ShowTable = report.ShowTable,
                ChartType = report.ChartType,
                XAxisField = report.XAxisField,
                YAxisField = report.YAxisField,
                Aggregation = report.Aggregation,
                AutoColumns = report.AutoColumns,
                ColumnTemplateId = report.ColumnTemplateId,
                ColumnConfigs = string.IsNullOrEmpty(report.ColumnConfigs) ? null : JsonSerializer.Deserialize<List<ColumnConfigDto>>(report.ColumnConfigs),
                UpdateTime = report.UpdateTime
            },
            ChartData = previewResult.ChartData,
            TableData = previewResult.TableData,
            ColumnConfigs = string.IsNullOrEmpty(report.ColumnConfigs) ? null : JsonSerializer.Deserialize<List<ColumnConfigDto>>(report.ColumnConfigs),
            DetectedColumns = previewResult.DetectedColumns,
            Summary = previewResult.Summary
        };

        return publishData;
    }

    /// <summary>
    /// 执行SQL查询
    /// </summary>
    private async Task<PreviewResultDto> ExecuteQueryAsync(Datasource datasource, string sql, string chartType, string? xAxisField, string? yAxisField, string aggregation)
    {
        var result = new PreviewResultDto();

        try
        {
            if (datasource.Type == "mysql")
            {
                var connectionString = $"Server={datasource.Host};Port={datasource.Port};Database={datasource.Database};User ID={datasource.Username};Password={datasource.Password};Charset=utf8mb4;";
                using var connection = new MySqlConnection(connectionString);
                await connection.OpenAsync();

                using var cmd = new MySqlCommand(sql, connection);
                using var reader = await cmd.ExecuteReaderAsync();

                var schema = reader.GetColumnSchema();
                foreach (var col in schema)
                {
                    result.DetectedColumns.Add(new DetectedColumnDto
                    {
                        Field = col.ColumnName,
                        Type = col.DataTypeName ?? "string"
                    });
                }

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i) ?? DBNull.Value;
                    }
                    result.TableData.Add(row);
                }
            }

            // 生成图表数据
            if (!string.IsNullOrEmpty(xAxisField) && !string.IsNullOrEmpty(yAxisField) && result.TableData.Count > 0)
            {
                var grouped = result.TableData
                    .GroupBy(row => row.ContainsKey(xAxisField) ? row[xAxisField]?.ToString() ?? "" : "")
                    .Select(g => new ChartDataDto
                    {
                        Name = g.Key,
                        Value = AggregateValue(g.ToList(), yAxisField, aggregation)
                    })
                    .ToList();

                result.ChartData = grouped;
                result.Summary.Total = grouped.Sum(x => x.Value);
                result.Summary.Count = grouped.Count;
                result.Summary.Avg = grouped.Count > 0 ? grouped.Average(x => x.Value) : 0;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行报表查询失败");
            throw BusinessException.BadRequest($"执行查询失败: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// 聚合计算
    /// </summary>
    private double AggregateValue(List<Dictionary<string, object>> rows, string field, string aggregation)
    {
        var values = rows
            .Where(r => r.ContainsKey(field) && r[field] != null && r[field] != DBNull.Value)
            .Select(r => Convert.ToDouble(r[field]))
            .ToList();

        if (values.Count == 0) return 0;

        return aggregation.ToLower() switch
        {
            "sum" => values.Sum(),
            "avg" => values.Average(),
            "count" => values.Count,
            _ => values.Sum()
        };
    }
}