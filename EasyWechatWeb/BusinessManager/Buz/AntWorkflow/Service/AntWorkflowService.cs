using SqlSugar;
using EasyWeChatModels.Dto.AntWorkflow;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Utility;
using AntWorkflowEntity = EasyWeChatModels.Entitys.AntWorkflow;

namespace BusinessManager.Buz.Service;

/// <summary>
/// Ant流程服务实现
/// </summary>
public class AntWorkflowService : IAntWorkflowService
{
    /// <summary>数据库上下文（Autofac 属性注入）</summary>
    public ISqlSugarClient _db { get; set; } = null!;

    #region 查询方法

    /// <inheritdoc/>
    public async Task<PageResponse<AntWorkflowDto>> GetPageListAsync(QueryAntWorkflowDto query)
    {
        var queryable = _db.Queryable<AntWorkflowEntity>()
            // 条件筛选 - 使用 WhereIF
            .WhereIF(!string.IsNullOrEmpty(query.Name), w => w.Name.Contains(query.Name!))
            .WhereIF(!string.IsNullOrEmpty(query.Code), w => w.Code == query.Code)
            .WhereIF(!string.IsNullOrEmpty(query.CategoryCode), w => w.CategoryCode == query.CategoryCode)
            .WhereIF(query.Status.HasValue, w => w.Status == query.Status!.Value);

        // 排序和分页
        var totalCount = await queryable.CountAsync();
        var list = await queryable
            .OrderByDescending(w => w.CreateTime)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        // 获取分类字典数据
        var categoryDict = await GetCategoryDictAsync();

        // 转换为DTO
        var dtoList = list.Select(w => new AntWorkflowDto
        {
            Id = w.Id,
            Name = w.Name,
            Code = w.Code,
            CategoryCode = w.CategoryCode,
            CategoryName = categoryDict.GetValueOrDefault(w.CategoryCode),
            Description = w.Description,
            Status = w.Status,
            CurrentVersion = w.CurrentVersion,
            FlowConfig = w.FlowConfig,
            CreatorId = w.CreatorId,
            CreateTime = w.CreateTime,
            UpdateTime = w.UpdateTime
        }).ToList();

        return PageResponse<AntWorkflowDto>.Create(dtoList, totalCount, query.PageIndex, query.PageSize);
    }

    /// <inheritdoc/>
    public async Task<AntWorkflowDto?> GetByIdAsync(Guid id)
    {
        var workflow = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Id == id)
            .FirstAsync();

        if (workflow == null)
        {
            return null;
        }

        // 获取分类名称
        var categoryDict = await GetCategoryDictAsync();

        return new AntWorkflowDto
        {
            Id = workflow.Id,
            Name = workflow.Name,
            Code = workflow.Code,
            CategoryCode = workflow.CategoryCode,
            CategoryName = categoryDict.GetValueOrDefault(workflow.CategoryCode),
            Description = workflow.Description,
            Status = workflow.Status,
            CurrentVersion = workflow.CurrentVersion,
            FlowConfig = workflow.FlowConfig,
            CreatorId = workflow.CreatorId,
            CreateTime = workflow.CreateTime,
            UpdateTime = workflow.UpdateTime
        };
    }

    /// <inheritdoc/>
    public async Task<AntWorkflowEntity?> GetByCodeAsync(string code)
    {
        return await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Code == code)
            .FirstAsync();
    }

    /// <inheritdoc/>
    public async Task<List<AntWorkflowDto>> GetPublishedListAsync(string? categoryCode = null)
    {
        var queryable = _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Status == (int)WorkflowStatus.Published)
            .WhereIF(!string.IsNullOrEmpty(categoryCode), w => w.CategoryCode == categoryCode);

        var list = await queryable
            .OrderByDescending(w => w.CreateTime)
            .ToListAsync();

        // 获取分类名称
        var categoryDict = await GetCategoryDictAsync();

        return list.Select(w => new AntWorkflowDto
        {
            Id = w.Id,
            Name = w.Name,
            Code = w.Code,
            CategoryCode = w.CategoryCode,
            CategoryName = categoryDict.GetValueOrDefault(w.CategoryCode),
            Description = w.Description,
            Status = w.Status,
            CurrentVersion = w.CurrentVersion,
            FlowConfig = w.FlowConfig,
            CreatorId = w.CreatorId,
            CreateTime = w.CreateTime
        }).ToList();
    }

    #endregion

    #region CRUD方法

    /// <inheritdoc/>
    public async Task<Guid> CreateAsync(CreateAntWorkflowDto dto, Guid creatorId)
    {
        // 检查编码是否重复
        var exists = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Code == dto.Code)
            .FirstAsync();

        if (exists != null)
        {
            throw new Exception($"流程编码 '{dto.Code}' 已存在");
        }

        var workflow = new AntWorkflowEntity
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Code = dto.Code,
            CategoryCode = dto.CategoryCode,
            Description = dto.Description,
            Status = (int)WorkflowStatus.Draft,
            CurrentVersion = "1.0",
            FlowConfig = dto.FlowConfig,
            CreatorId = creatorId,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(workflow).ExecuteCommandAsync();
        return workflow.Id;
    }

    /// <inheritdoc/>
    public async Task<int> UpdateAsync(UpdateAntWorkflowDto dto)
    {
        var workflow = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Id == dto.Id)
            .FirstAsync();

        if (workflow == null)
        {
            return 0;
        }

        // 已发布的流程可以随时编辑，正在运行的实例使用实例自己保存的配置副本
        // 只有待审核状态的流程不能编辑
        if (workflow.Status == (int)WorkflowStatus.Pending)
        {
            throw new Exception("待审核状态的流程不能编辑");
        }

        workflow.Name = dto.Name;
        workflow.CategoryCode = dto.CategoryCode;
        workflow.Description = dto.Description;
        workflow.FlowConfig = dto.FlowConfig;
        workflow.UpdateTime = DateTime.Now;

        return await _db.Updateable(workflow).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<int> DeleteAsync(Guid id)
    {
        var workflow = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Id == id)
            .FirstAsync();

        if (workflow == null)
        {
            return 0;
        }

        // 只有草稿、已拒绝、已停用状态可以删除
        if (workflow.Status == (int)WorkflowStatus.Published || workflow.Status == (int)WorkflowStatus.Pending)
        {
            throw new Exception("已发布或待审核状态的流程不能删除");
        }

        return await _db.Deleteable<AntWorkflowEntity>()
            .Where(w => w.Id == id)
            .ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<int> PublishAsync(Guid id, Guid publisherId, string publisherName)
    {
        var workflow = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Id == id)
            .FirstAsync();

        if (workflow == null)
        {
            return 0;
        }

        // 只有草稿状态可以发布
        if (workflow.Status != (int)WorkflowStatus.Draft)
        {
            throw new Exception("只有草稿状态的流程可以发布");
        }

        // 更新流程状态
        workflow.Status = (int)WorkflowStatus.Published;
        workflow.UpdateTime = DateTime.Now;

        // 创建版本记录
        var version = new AntWorkflowVersion
        {
            Id = Guid.NewGuid(),
            WorkflowId = id,
            Version = workflow.CurrentVersion,
            FlowConfig = workflow.FlowConfig,
            PublishTime = DateTime.Now,
            PublisherId = publisherId,
            PublisherName = publisherName,
            Status = (int)WorkflowStatus.Published
        };

        // 使用事务
        var result = await _db.Ado.UseTranAsync(async () =>
        {
            await _db.Updateable(workflow).ExecuteCommandAsync();
            await _db.Insertable(version).ExecuteCommandAsync();
        });

        if (!result.IsSuccess)
        {
            throw new Exception($"发布失败：{result.ErrorMessage}");
        }

        return 1;
    }

    /// <inheritdoc/>
    public async Task<int> DisableAsync(Guid id)
    {
        var workflow = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Id == id)
            .FirstAsync();

        if (workflow == null)
        {
            return 0;
        }

        if (workflow.Status != (int)WorkflowStatus.Published)
        {
            throw new Exception("只有已发布状态的流程可以停用");
        }

        workflow.Status = (int)WorkflowStatus.Disabled;
        workflow.UpdateTime = DateTime.Now;

        return await _db.Updateable(workflow).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<int> EnableAsync(Guid id)
    {
        var workflow = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Id == id)
            .FirstAsync();

        if (workflow == null)
        {
            return 0;
        }

        if (workflow.Status != (int)WorkflowStatus.Disabled)
        {
            throw new Exception("只有已停用状态的流程可以启用");
        }

        workflow.Status = (int)WorkflowStatus.Published;
        workflow.UpdateTime = DateTime.Now;

        return await _db.Updateable(workflow).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<Guid> CopyAsync(CopyAntWorkflowDto dto, Guid creatorId)
    {
        var source = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Id == dto.SourceId)
            .FirstAsync();

        if (source == null)
        {
            throw new Exception("源流程不存在");
        }

        // 确定新编码
        var newCode = dto.NewCode;
        if (string.IsNullOrEmpty(newCode))
        {
            newCode = $"{source.Code}_copy";
        }

        // 检查编码是否重复
        var exists = await _db.Queryable<AntWorkflowEntity>()
            .Where(w => w.Code == newCode)
            .FirstAsync();

        if (exists != null)
        {
            throw new Exception($"流程编码 '{newCode}' 已存在");
        }

        var workflow = new AntWorkflowEntity
        {
            Id = Guid.NewGuid(),
            Name = dto.NewName,
            Code = newCode,
            CategoryCode = source.CategoryCode,
            Description = source.Description,
            Status = (int)WorkflowStatus.Draft,
            CurrentVersion = "1.0",
            FlowConfig = source.FlowConfig,
            CreatorId = creatorId,
            CreateTime = DateTime.Now
        };

        await _db.Insertable(workflow).ExecuteCommandAsync();
        return workflow.Id;
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 获取工作流分类字典
    /// </summary>
    private async Task<Dictionary<string, string>> GetCategoryDictAsync()
    {
        var dictData = await _db.Queryable<DictData>()
            .Where(d => d.TypeCode == "workflow_category" && d.Status == 1)
            .ToListAsync();

        return dictData.ToDictionary(d => d.Value, d => d.LabelZh ?? d.Label);
    }

    #endregion
}