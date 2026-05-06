using BusinessManager.Buz.AntWorkflow.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Microsoft.Extensions.Logging;
using SqlSugar;
// 使用 global:: 前缀确保指向实体类而不是命名空间文件夹
using AntWorkflowEntity = global::EasyWeChatModels.Entitys.AntWorkflow;

namespace BusinessManager.Buz.AntWorkflow.Service;

/// <summary>
/// 业务审核点服务实现
/// </summary>
public class BusinessAuditPointService : BaseService<BusinessAuditPoint>, IBusinessAuditPointService
{
    /// <summary>日志服务（Autofac 属性注入）</summary>
    public ILogger<BusinessAuditPointService> _logger { get; set; } = null!;

    #region 查询方法

    /// <inheritdoc/>
    public async Task<PageResponse<BusinessAuditPointDto>> GetPageListAsync(QueryBusinessAuditPointDto query)
    {
        // 先查询主表
        var queryable = _db.Queryable<BusinessAuditPoint>()
            .WhereIF(!string.IsNullOrEmpty(query.Code), bp => bp.Code == query.Code)
            .WhereIF(!string.IsNullOrEmpty(query.Name), bp => bp.Name.Contains(query.Name!))
            .WhereIF(!string.IsNullOrEmpty(query.Category), bp => bp.Category == query.Category)
            .WhereIF(!string.IsNullOrEmpty(query.TableName), bp => bp.TableName == query.TableName)
            .WhereIF(query.Status.HasValue, bp => bp.Status == query.Status!.Value);

        // 排序和分页
        var totalCount = await queryable.CountAsync();
        var rawList = await queryable
            .OrderBy(bp => bp.Sort)
            .OrderByDescending(bp => bp.CreateTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // 获取 Workflow 名称
        var workflowIds = rawList.Where(x => x.WorkflowId != Guid.Empty).Select(x => x.WorkflowId).Distinct().ToList();
        var workflows = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => workflowIds.Contains(w.Id))
            .ToListAsync();
        var workflowDict = workflows.ToDictionary(w => w.Id, w => w.Name);

        // 在内存中转换为 DTO
        var list = rawList.Select(item => new BusinessAuditPointDto
        {
            Id = item.Id,
            Code = item.Code,
            Name = item.Name,
            Category = item.Category,
            WorkflowId = item.WorkflowId,
            WorkflowName = workflowDict.GetValueOrDefault(item.WorkflowId),
            TableName = item.TableName,
            StatusField = item.StatusField,
            AuditStatusValue = item.AuditStatusValue,
            PassStatusValue = item.PassStatusValue,
            RejectStatusValue = item.RejectStatusValue,
            WithdrawStatusValue = item.WithdrawStatusValue,
            AuditPageUrl = item.AuditPageUrl,
            TitleTemplate = item.TitleTemplate,
            CodeField = item.CodeField,
            PassCallbackApi = item.PassCallbackApi,
            RejectCallbackApi = item.RejectCallbackApi,
            Status = item.Status,
            Sort = item.Sort,
            Remark = item.Remark,
            CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = item.UpdateTime.HasValue ? item.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
        }).ToList();

        return PageResponse<BusinessAuditPointDto>.Create(list, totalCount, query.PageIndex, query.PageSize);
    }

    /// <inheritdoc/>
    public async Task<BusinessAuditPointDto?> GetByIdAsync(Guid id)
    {
        var rawResult = await _db.Queryable<BusinessAuditPoint, AntWorkflowEntity>(
            (bp, wf) => new JoinQueryInfos(
                JoinType.Left, bp.WorkflowId == wf.Id
            ))
            .Where((bp, wf) => bp.Id == id)
            .Select((bp, wf) => new
            {
                bp.Id,
                bp.Code,
                bp.Name,
                bp.Category,
                bp.WorkflowId,
                WorkflowName = wf.Name,
                bp.TableName,
                bp.StatusField,
                bp.AuditStatusValue,
                bp.PassStatusValue,
                bp.RejectStatusValue,
                bp.WithdrawStatusValue,
                bp.AuditPageUrl,
                bp.TitleTemplate,
                bp.CodeField,
                bp.PassCallbackApi,
                bp.RejectCallbackApi,
                bp.Status,
                bp.Sort,
                bp.Remark,
                bp.CreateTime,
                bp.UpdateTime
            })
            .FirstAsync();

        if (rawResult == null) return null;

        return new BusinessAuditPointDto
        {
            Id = rawResult.Id,
            Code = rawResult.Code,
            Name = rawResult.Name,
            Category = rawResult.Category,
            WorkflowId = rawResult.WorkflowId,
            WorkflowName = rawResult.WorkflowName,
            TableName = rawResult.TableName,
            StatusField = rawResult.StatusField,
            AuditStatusValue = rawResult.AuditStatusValue,
            PassStatusValue = rawResult.PassStatusValue,
            RejectStatusValue = rawResult.RejectStatusValue,
            WithdrawStatusValue = rawResult.WithdrawStatusValue,
            AuditPageUrl = rawResult.AuditPageUrl,
            TitleTemplate = rawResult.TitleTemplate,
            CodeField = rawResult.CodeField,
            PassCallbackApi = rawResult.PassCallbackApi,
            RejectCallbackApi = rawResult.RejectCallbackApi,
            Status = rawResult.Status,
            Sort = rawResult.Sort,
            Remark = rawResult.Remark,
            CreateTime = rawResult.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = rawResult.UpdateTime.HasValue ? rawResult.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
        };
    }

    /// <inheritdoc/>
    public async Task<BusinessAuditPointDto?> GetByCodeAsync(string code)
    {
        var rawResult = await _db.Queryable<BusinessAuditPoint, AntWorkflowEntity>(
            (bp, wf) => new JoinQueryInfos(
                JoinType.Left, bp.WorkflowId == wf.Id
            ))
            .Where((bp, wf) => bp.Code == code)
            .Select((bp, wf) => new
            {
                bp.Id,
                bp.Code,
                bp.Name,
                bp.Category,
                bp.WorkflowId,
                WorkflowName = wf.Name,
                bp.TableName,
                bp.StatusField,
                bp.AuditStatusValue,
                bp.PassStatusValue,
                bp.RejectStatusValue,
                bp.WithdrawStatusValue,
                bp.AuditPageUrl,
                bp.TitleTemplate,
                bp.CodeField,
                bp.PassCallbackApi,
                bp.RejectCallbackApi,
                bp.Status,
                bp.Sort,
                bp.Remark,
                bp.CreateTime,
                bp.UpdateTime
            })
            .FirstAsync();

        if (rawResult == null) return null;

        return new BusinessAuditPointDto
        {
            Id = rawResult.Id,
            Code = rawResult.Code,
            Name = rawResult.Name,
            Category = rawResult.Category,
            WorkflowId = rawResult.WorkflowId,
            WorkflowName = rawResult.WorkflowName,
            TableName = rawResult.TableName,
            StatusField = rawResult.StatusField,
            AuditStatusValue = rawResult.AuditStatusValue,
            PassStatusValue = rawResult.PassStatusValue,
            RejectStatusValue = rawResult.RejectStatusValue,
            WithdrawStatusValue = rawResult.WithdrawStatusValue,
            AuditPageUrl = rawResult.AuditPageUrl,
            TitleTemplate = rawResult.TitleTemplate,
            CodeField = rawResult.CodeField,
            PassCallbackApi = rawResult.PassCallbackApi,
            RejectCallbackApi = rawResult.RejectCallbackApi,
            Status = rawResult.Status,
            Sort = rawResult.Sort,
            Remark = rawResult.Remark,
            CreateTime = rawResult.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = rawResult.UpdateTime.HasValue ? rawResult.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
        };
    }

    /// <inheritdoc/>
    public async Task<List<BusinessAuditPointDto>> GetByTableNameAsync(string tableName)
    {
        var rawList = await _db.Queryable<BusinessAuditPoint, AntWorkflowEntity>(
            (bp, wf) => new JoinQueryInfos(
                JoinType.Left, bp.WorkflowId == wf.Id
            ))
            .Where((bp, wf) => bp.TableName == tableName)
            .Where((bp, wf) => bp.Status == 1)  // 只查询启用状态的审核点
            .OrderBy((bp, wf) => bp.Sort)
            .Select((bp, wf) => new
            {
                bp.Id,
                bp.Code,
                bp.Name,
                bp.Category,
                bp.WorkflowId,
                WorkflowName = wf.Name,
                bp.TableName,
                bp.StatusField,
                bp.AuditStatusValue,
                bp.PassStatusValue,
                bp.RejectStatusValue,
                bp.WithdrawStatusValue,
                bp.AuditPageUrl,
                bp.TitleTemplate,
                bp.CodeField,
                bp.PassCallbackApi,
                bp.RejectCallbackApi,
                bp.Status,
                bp.Sort,
                bp.Remark,
                bp.CreateTime,
                bp.UpdateTime
            })
            .ToListAsync();

        // 在内存中转换为 DTO
        var list = rawList.Select(item => new BusinessAuditPointDto
        {
            Id = item.Id,
            Code = item.Code,
            Name = item.Name,
            Category = item.Category,
            WorkflowId = item.WorkflowId,
            WorkflowName = item.WorkflowName,
            TableName = item.TableName,
            StatusField = item.StatusField,
            AuditStatusValue = item.AuditStatusValue,
            PassStatusValue = item.PassStatusValue,
            RejectStatusValue = item.RejectStatusValue,
            WithdrawStatusValue = item.WithdrawStatusValue,
            AuditPageUrl = item.AuditPageUrl,
            TitleTemplate = item.TitleTemplate,
            CodeField = item.CodeField,
            PassCallbackApi = item.PassCallbackApi,
            RejectCallbackApi = item.RejectCallbackApi,
            Status = item.Status,
            Sort = item.Sort,
            Remark = item.Remark,
            CreateTime = item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
            UpdateTime = item.UpdateTime.HasValue ? item.UpdateTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : null
        }).ToList();

        return list;
    }

    #endregion

    #region CRUD 方法

    /// <inheritdoc/>
    public async Task<Guid> AddAsync(AddBusinessAuditPointDto dto, Guid creatorId)
    {
        // 检查编码唯一性
        var exists = await _db.Queryable<BusinessAuditPoint>()
            .Where(bp => bp.Code == dto.Code)
            .FirstAsync();

        if (exists != null)
        {
            throw new BusinessException($"审核点编码 '{dto.Code}' 已存在");
        }

        var entity = new BusinessAuditPoint
        {
            Id = Guid.NewGuid(),
            Code = dto.Code,
            Name = dto.Name,
            Category = dto.Category,
            WorkflowId = dto.WorkflowId,
            TableName = dto.TableName,
            StatusField = dto.StatusField,
            AuditStatusValue = dto.AuditStatusValue,
            PassStatusValue = dto.PassStatusValue,
            RejectStatusValue = dto.RejectStatusValue,
            WithdrawStatusValue = dto.WithdrawStatusValue,
            AuditPageUrl = dto.AuditPageUrl,
            TitleTemplate = dto.TitleTemplate,
            CodeField = dto.CodeField,
            PassCallbackApi = dto.PassCallbackApi,
            RejectCallbackApi = dto.RejectCallbackApi,
            Status = dto.Status,
            Sort = dto.Sort,
            Remark = dto.Remark,
            CreatorId = creatorId,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <inheritdoc/>
    public async Task<int> UpdateAsync(UpdateBusinessAuditPointDto dto)
    {
        // 检查编码唯一性（排除自身）
        var exists = await _db.Queryable<BusinessAuditPoint>()
            .Where(bp => bp.Code == dto.Code && bp.Id != dto.Id)
            .FirstAsync();

        if (exists != null)
        {
            throw new BusinessException($"审核点编码 '{dto.Code}' 已存在");
        }

        var entity = await _db.Queryable<BusinessAuditPoint>()
            .Where(bp => bp.Id == dto.Id)
            .FirstAsync();

        if (entity == null)
        {
            return 0;
        }

        entity.Code = dto.Code;
        entity.Name = dto.Name;
        entity.Category = dto.Category;
        entity.WorkflowId = dto.WorkflowId;
        entity.TableName = dto.TableName;
        entity.StatusField = dto.StatusField;
        entity.AuditStatusValue = dto.AuditStatusValue;
        entity.PassStatusValue = dto.PassStatusValue;
        entity.RejectStatusValue = dto.RejectStatusValue;
        entity.WithdrawStatusValue = dto.WithdrawStatusValue;
        entity.AuditPageUrl = dto.AuditPageUrl;
        entity.TitleTemplate = dto.TitleTemplate;
        entity.CodeField = dto.CodeField;
        entity.PassCallbackApi = dto.PassCallbackApi;
        entity.RejectCallbackApi = dto.RejectCallbackApi;
        entity.Status = dto.Status;
        entity.Sort = dto.Sort;
        entity.Remark = dto.Remark;
        entity.UpdateTime = DateTime.Now;

        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<int> DeleteAsync(Guid id)
    {
        return await _db.Deleteable<BusinessAuditPoint>()
            .Where(bp => bp.Id == id)
            .ExecuteCommandAsync();
    }

    #endregion
}