using SqlSugar;
using BusinessManager.Buz.IService;
using EasyWeChatModels.Dto.AntWorkflow;
using EasyWeChatModels.Entitys;

namespace BusinessManager.Buz.Service;

/// <summary>
/// Ant流程表单字段服务实现
/// </summary>
public class AntWorkflowFormFieldService : IAntWorkflowFormFieldService
{
    /// <summary>数据库上下文（Autofac 属性注入）</summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <inheritdoc/>
    public async Task<List<AntFormFieldDto>> GetListByBusinessTypeAsync(string businessType)
    {
        var list = await _db.Queryable<AntWorkflowFormField>()
            .Where(f => f.BusinessType == businessType)
            .OrderBy(f => f.Order)
            .ToListAsync();

        return list.Select(f => new AntFormFieldDto
        {
            Id = f.Id,
            BusinessType = f.BusinessType,
            FieldId = f.FieldId,
            FieldName = f.FieldName,
            FieldLabel = f.FieldLabel,
            FieldType = f.FieldType,
            Required = f.Required,
            DefaultValue = f.DefaultValue,
            Options = f.Options,
            Placeholder = f.Placeholder,
            Order = f.Order,
            CreateTime = f.CreateTime
        }).ToList();
    }

    /// <inheritdoc/>
    public async Task<Guid> AddAsync(CreateFormFieldDto dto)
    {
        // 检查是否已存在相同业务类型和字段ID
        var exists = await _db.Queryable<AntWorkflowFormField>()
            .Where(f => f.BusinessType == dto.BusinessType && f.FieldId == dto.FieldId)
            .FirstAsync();

        if (exists != null)
        {
            throw new Exception($"字段 {dto.FieldId} 已存在于业务类型 {dto.BusinessType}");
        }

        var field = new AntWorkflowFormField
        {
            Id = Guid.NewGuid(),
            BusinessType = dto.BusinessType,
            FieldId = dto.FieldId,
            FieldName = dto.FieldName,
            FieldLabel = dto.FieldLabel,
            FieldType = dto.FieldType,
            Required = dto.Required,
            DefaultValue = dto.DefaultValue,
            Options = dto.Options,
            Placeholder = dto.Placeholder,
            Order = dto.Order,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(field).ExecuteCommandAsync();
        return field.Id;
    }

    /// <inheritdoc/>
    public async Task<int> UpdateAsync(UpdateFormFieldDto dto)
    {
        var field = await _db.Queryable<AntWorkflowFormField>()
            .Where(f => f.Id == dto.Id)
            .FirstAsync();

        if (field == null) return 0;

        if (dto.FieldName != null) field.FieldName = dto.FieldName;
        if (dto.FieldLabel != null) field.FieldLabel = dto.FieldLabel;
        if (dto.FieldType != null) field.FieldType = dto.FieldType;
        if (dto.Required.HasValue) field.Required = dto.Required.Value;
        if (dto.DefaultValue != null) field.DefaultValue = dto.DefaultValue;
        if (dto.Options != null) field.Options = dto.Options;
        if (dto.Placeholder != null) field.Placeholder = dto.Placeholder;
        if (dto.Order.HasValue) field.Order = dto.Order.Value;

        return await _db.Updateable(field).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<int> DeleteAsync(Guid id)
    {
        return await _db.Deleteable<AntWorkflowFormField>()
            .Where(f => f.Id == id)
            .ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<int> UpdateOrderAsync(List<FormFieldOrderDto> orders)
    {
        var count = 0;
        foreach (var order in orders)
        {
            var result = await _db.Updateable<AntWorkflowFormField>()
                .SetColumns(f => f.Order == order.Order)
                .Where(f => f.Id == order.Id)
                .ExecuteCommandAsync();
            count += result;
        }
        return count;
    }
}