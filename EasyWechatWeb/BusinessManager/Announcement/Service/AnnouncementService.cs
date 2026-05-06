using BusinessManager.Announcement.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.Text.Json;

// 使用别名解决命名空间与实体类命名冲突
using AnnouncementEntity = EasyWeChatModels.Entitys.Announcement;

namespace BusinessManager.Announcement.Service;

/// <summary>
/// 公告服务实现类
/// </summary>
/// <remarks>
/// 实现系统公告相关的业务逻辑，包括公告的增删改查、发布撤回、置顶管理、阅读状态跟踪等功能。
/// 继承自<see cref="BaseService{AnnouncementEntity}"/>，使用SqlSugar进行数据库操作。
/// 支持全员公告和定向公告，提供完整的阅读统计和详情查询功能。
/// </remarks>
public class AnnouncementService : BaseService<AnnouncementEntity>, IAnnouncementService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<AnnouncementService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取公告类型名称
    /// </summary>
    /// <param name="type">公告类型代码</param>
    /// <returns>公告类型的中文描述</returns>
    /// <remarks>
    /// 类型映射：1-全员公告 2-定向公告
    /// </remarks>
    private static string GetTypeName(int type)
    {
        return type switch
        {
            1 => "全员公告",
            2 => "定向公告",
            _ => "未知"
        };
    }

    /// <summary>
    /// 获取公告级别名称
    /// </summary>
    /// <param name="level">公告级别代码</param>
    /// <returns>公告级别的中文描述</returns>
    /// <remarks>
    /// 级别映射：1-普通 2-重要 3-紧急
    /// </remarks>
    private static string GetLevelName(int level)
    {
        return level switch
        {
            1 => "普通",
            2 => "重要",
            3 => "紧急",
            _ => "未知"
        };
    }

    /// <summary>
    /// 获取公告状态名称
    /// </summary>
    /// <param name="status">公告状态代码</param>
    /// <returns>公告状态的中文描述</returns>
    /// <remarks>
    /// 状态映射：0-草稿 1-已发布 2-已撤回
    /// </remarks>
    private static string GetStatusName(int status)
    {
        return status switch
        {
            0 => "草稿",
            1 => "已发布",
            2 => "已撤回",
            _ => "未知"
        };
    }

    /// <summary>
    /// 分页获取公告列表
    /// </summary>
    /// <param name="query">查询条件，包含标题关键字、类型、级别、状态等筛选条件</param>
    /// <returns>分页公告列表，包含公告基本信息和阅读统计数据</returns>
    /// <remarks>
    /// 支持按标题关键字模糊搜索，按类型、级别、状态、是否置顶等条件筛选。
    /// 支持按发布时间范围筛选。
    /// 返回结果按置顶状态和创建时间排序，置顶公告优先显示。
    /// </remarks>
    public async Task<PageResponse<AnnouncementDto>> GetPageListAsync(QueryAnnouncementDto query)
    {
        var total = new RefAsync<int>();

        // 构建查询条件
        var queryBuilder = _db.Queryable<AnnouncementEntity>()
            .WhereIF(!string.IsNullOrEmpty(query.Title), a => a.Title.Contains(query.Title!))
            .WhereIF(query.Type.HasValue, a => a.Type == query.Type!.Value)
            .WhereIF(query.Level.HasValue, a => a.Level == query.Level!.Value)
            .WhereIF(query.Status.HasValue, a => a.Status == query.Status!.Value)
            .WhereIF(query.IsTop.HasValue, a => a.IsTop == query.IsTop!.Value)
            .WhereIF(query.CreatorId.HasValue, a => a.CreatorId == query.CreatorId!.Value)
            .WhereIF(query.StartTime.HasValue, a => a.PublishTime >= query.StartTime!.Value)
            .WhereIF(query.EndTime.HasValue, a => a.PublishTime <= query.EndTime!.Value)
            // 排序：置顶优先，然后按创建时间倒序
            .OrderByDescending(a => a.IsTop)
            .OrderByDescending(a => a.CreateTime);

        var list = await queryBuilder.ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 手动映射为 DTO，避免 Mapster 自动转换 TargetRoleIds 字段失败
        var dtos = new List<AnnouncementDto>();
        foreach (var entity in list)
        {
            var dto = new AnnouncementDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                Type = entity.Type,
                Level = entity.Level,
                IsTop = entity.IsTop,
                TopTime = entity.TopTime,
                PublishTime = entity.PublishTime,
                RecallTime = entity.RecallTime,
                Status = entity.Status,
                CreatorId = entity.CreatorId,
                CreateTime = entity.CreateTime,
                UpdateTime = entity.UpdateTime,
                // TargetRoleIds 稍后手动处理
            };

            // 手动解析 TargetRoleIds JSON 字符串
            if (!string.IsNullOrEmpty(entity.TargetRoleIds))
            {
                try
                {
                    var jsonStr = entity.TargetRoleIds.Trim();
                    if (jsonStr.StartsWith("[") && jsonStr.EndsWith("]"))
                    {
                        dto.TargetRoleIds = JsonSerializer.Deserialize<List<Guid>>(jsonStr);
                    }
                }
                catch { /* 忽略解析错误 */ }
            }

            dtos.Add(dto);
        }

        // 填充额外信息
        foreach (var dto in dtos)
        {
            dto.TypeName = GetTypeName(dto.Type);
            dto.LevelName = GetLevelName(dto.Level);
            dto.StatusName = GetStatusName(dto.Status);

            // 获取创建人名称
            var creator = await _db.Queryable<User>()
                .Where(u => u.Id == dto.CreatorId)
                .Select(u => u.RealName ?? u.UserName)
                .FirstAsync();
            dto.CreatorName = creator;

            // 获取目标角色名称（TargetRoleIds 已在上面的手动映射中处理）
            if (dto.TargetRoleIds != null && dto.TargetRoleIds.Count > 0)
            {
                var roleNames = await _db.Queryable<Role>()
                    .Where(r => dto.TargetRoleIds.Contains(r.Id))
                    .Select(r => r.RoleName)
                    .ToListAsync();
                dto.TargetRoleNames = roleNames;
            }

            // 统计阅读人数和总目标人数
            var readCount = await _db.Queryable<AnnouncementRead>()
                .Where(ar => ar.AnnouncementId == dto.Id && ar.IsRead == 1)
                .CountAsync();

            var totalCount = await _db.Queryable<AnnouncementRead>()
                .Where(ar => ar.AnnouncementId == dto.Id)
                .CountAsync();

            dto.ReadCount = readCount;
            dto.TotalCount = totalCount;
        }

        return PageResponse<AnnouncementDto>.Create(dtos, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取公告详情
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <param name="currentUserId">当前用户ID，可选。用于返回当前用户的阅读状态</param>
    /// <returns>公告详细信息，包含内容和阅读统计数据；公告不存在返回null</returns>
    /// <remarks>
    /// 获取公告的完整信息，包括富文本内容和目标角色信息。
    /// 同时返回阅读人数和总目标人数等统计数据。
    /// 如果提供currentUserId，会返回该用户的阅读状态和附件列表。
    /// </remarks>
    public async Task<AnnouncementDto?> GetByIdAsync(Guid id, Guid? currentUserId = null)
    {
        var announcement = await base.GetByIdAsync(id);
        if (announcement == null)
        {
            return null;
        }

        // 手动映射为 DTO，避免 Mapster 自动转换 TargetRoleIds 字段失败
        var dto = new AnnouncementDto
        {
            Id = announcement.Id,
            Title = announcement.Title,
            Content = announcement.Content,
            Type = announcement.Type,
            Level = announcement.Level,
            IsTop = announcement.IsTop,
            TopTime = announcement.TopTime,
            PublishTime = announcement.PublishTime,
            RecallTime = announcement.RecallTime,
            Status = announcement.Status,
            CreatorId = announcement.CreatorId,
            CreateTime = announcement.CreateTime,
            UpdateTime = announcement.UpdateTime,
            // TargetRoleIds 稍后手动处理
        };

        // 填充额外信息
        dto.TypeName = GetTypeName(dto.Type);
        dto.LevelName = GetLevelName(dto.Level);
        dto.StatusName = GetStatusName(dto.Status);

        // 获取创建人名称
        var creator = await _db.Queryable<User>()
            .Where(u => u.Id == dto.CreatorId)
            .Select(u => u.RealName ?? u.UserName)
            .FirstAsync();
        dto.CreatorName = creator;

        // 解析目标角色ID
        if (!string.IsNullOrEmpty(announcement.TargetRoleIds))
        {
            try
            {
                // 确保字符串是有效的 JSON 格式
                var jsonStr = announcement.TargetRoleIds.Trim();
                if (jsonStr.StartsWith("[") && jsonStr.EndsWith("]"))
                {
                    dto.TargetRoleIds = JsonSerializer.Deserialize<List<Guid>>(jsonStr);

                    // 获取目标角色名称
                    if (dto.TargetRoleIds != null && dto.TargetRoleIds.Count > 0)
                    {
                        var roleNames = await _db.Queryable<Role>()
                            .Where(r => dto.TargetRoleIds.Contains(r.Id))
                            .Select(r => r.RoleName)
                            .ToListAsync();
                        dto.TargetRoleNames = roleNames;
                    }
                }
                else
                {
                    _logger.LogWarning("公告 {Id} 的 TargetRoleIds 不是有效的 JSON 格式: {TargetRoleIds}", id, announcement.TargetRoleIds);
                    dto.TargetRoleIds = null;
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "解析公告 {Id} 的 TargetRoleIds 失败", id);
                dto.TargetRoleIds = null;
            }
        }

        // 统计阅读人数和总目标人数
        var readCount = await _db.Queryable<AnnouncementRead>()
            .Where(ar => ar.AnnouncementId == id && ar.IsRead == 1)
            .CountAsync();

        var totalCount = await _db.Queryable<AnnouncementRead>()
            .Where(ar => ar.AnnouncementId == id)
            .CountAsync();

        dto.ReadCount = readCount;
        dto.TotalCount = totalCount;

        // 查询附件列表 - 只根据 BusinessId 查询
        var attachments = await _db.Queryable<FileRecord>()
            .Where(f => f.BusinessId.HasValue && f.BusinessId.Value == id && f.Status == 1)
            .OrderByDescending(f => f.CreateTime)
            .ToListAsync();

        if (attachments.Count > 0)
        {
            dto.Attachments = attachments.Adapt<List<FileRecordDto>>();
        }

        // 查询当前用户阅读状态
        if (currentUserId.HasValue)
        {
            var readRecord = await _db.Queryable<AnnouncementRead>()
                .Where(ar => ar.AnnouncementId == id && ar.UserId == currentUserId.Value)
                .FirstAsync();

            dto.IsRead = readRecord?.IsRead ?? 0;
        }

        return dto;
    }

    /// <summary>
    /// 新增公告
    /// </summary>
    /// <param name="dto">新增公告参数，包含标题、内容、类型、级别等信息</param>
    /// <param name="creatorId">创建人ID</param>
    /// <returns>新创建的公告ID</returns>
    /// <remarks>
    /// 创建新的公告，默认状态为草稿。
    /// 定向公告需要指定目标角色ID列表。
    /// 可选择是否置顶，置顶公告会在列表中优先显示。
    /// </remarks>
    public async Task<Guid> AddAsync(AddAnnouncementDto dto, Guid creatorId)
    {
        var announcement = dto.Adapt<AnnouncementEntity>();
        announcement.CreatorId = creatorId;
        announcement.CreateTime = DateTime.Now;
        announcement.Status = 0; // 默认草稿状态

        // 处理目标角色ID（定向公告）
        if (dto.Type == 2 && dto.TargetRoleIds != null && dto.TargetRoleIds.Count > 0)
        {
            announcement.TargetRoleIds = JsonSerializer.Serialize(dto.TargetRoleIds);
        }

        // 处理置顶
        if (dto.IsTop == 1)
        {
            announcement.TopTime = DateTime.Now;
        }

        var id = await InsertAsync(announcement);

        _logger.LogInformation("公告创建成功，ID: {AnnouncementId}, 标题: {Title}, 创建人: {CreatorId}",
            id, announcement.Title, creatorId);

        return id;
    }

    /// <summary>
    /// 更新公告
    /// </summary>
    /// <param name="dto">更新公告参数，包含公告ID和更新的内容</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 更新公告的标题、内容、类型、级别等信息。
    /// 只能更新草稿或已撤回状态的公告，已发布的公告不可更新。
    /// 撤回的公告更新后状态变为草稿，需要重新发布。
    /// 更新时会记录更新时间。
    /// </remarks>
    public async Task<int> UpdateAsync(UpdateAnnouncementDto dto)
    {
        var announcement = await base.GetByIdAsync(dto.Id);
        if (announcement == null)
        {
            throw BusinessException.NotFound("公告不存在");
        }

        // 检查公告状态，只能更新草稿(0)或已撤回(2)状态的公告
        if (announcement.Status == 1)
        {
            throw BusinessException.BadRequest("已发布的公告不可编辑，请先撤回");
        }

        // 更新字段
        announcement.Title = dto.Title;
        announcement.Content = dto.Content;
        announcement.Type = dto.Type;
        announcement.Level = dto.Level;
        announcement.IsTop = dto.IsTop;
        announcement.UpdateTime = DateTime.Now;

        // 如果是撤回状态的公告，编辑后恢复为草稿状态
        if (announcement.Status == 2)
        {
            announcement.Status = 0; // 恢复为草稿
            announcement.RecallTime = null; // 清除撤回时间
        }

        // 处理目标角色ID（定向公告）
        if (dto.Type == 2 && dto.TargetRoleIds != null && dto.TargetRoleIds.Count > 0)
        {
            announcement.TargetRoleIds = JsonSerializer.Serialize(dto.TargetRoleIds);
        }
        else
        {
            announcement.TargetRoleIds = null;
        }

        // 处理置顶时间
        if (dto.IsTop == 1 && announcement.TopTime == null)
        {
            announcement.TopTime = DateTime.Now;
        }
        else if (dto.IsTop == 0)
        {
            announcement.TopTime = null;
        }

        var result = await base.UpdateAsync(announcement);

        _logger.LogInformation("公告更新成功，ID: {AnnouncementId}, 标题: {Title}, 原状态: {OldStatus}, 新状态: {NewStatus}",
            announcement.Id, announcement.Title, 2, announcement.Status);

        return result;
    }

    /// <summary>
    /// 发布公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 将草稿状态的公告发布，发布后用户可以查看。
    /// 发布时会记录发布时间，同时创建阅读记录。
    /// 全员公告为所有用户创建阅读记录，定向公告为指定角色用户创建阅读记录。
    /// 不能发布已撤回的公告，需要重新编辑后再发布。
    /// </remarks>
    public async Task<int> PublishAsync(Guid id)
    {
        var announcement = await base.GetByIdAsync(id);
        if (announcement == null)
        {
            throw BusinessException.NotFound("公告不存在");
        }

        // 检查公告状态，只有草稿状态可以发布
        if (announcement.Status != 0)
        {
            throw BusinessException.BadRequest("只能发布草稿状态的公告");
        }

        // 更新公告状态
        announcement.Status = 1;
        announcement.PublishTime = DateTime.Now;
        announcement.UpdateTime = DateTime.Now;

        var result = await base.UpdateAsync(announcement);

        // 创建阅读记录
        List<Guid> targetUserIds;

        if (announcement.Type == 1) // 全员公告
        {
            // 获取所有状态正常的用户
            targetUserIds = await _db.Queryable<User>()
                .Where(u => u.Status == 1)
                .Select(u => u.Id)
                .ToListAsync();
        }
        else // 定向公告
        {
            // 解析目标角色ID
            var targetRoleIds = new List<Guid>();
            if (!string.IsNullOrEmpty(announcement.TargetRoleIds))
            {
                targetRoleIds = JsonSerializer.Deserialize<List<Guid>>(announcement.TargetRoleIds) ?? new List<Guid>();
            }

            if (targetRoleIds.Count == 0)
            {
                throw BusinessException.BadRequest("定向公告未指定目标角色");
            }

            // 获取目标角色用户
            targetUserIds = await _db.Queryable<UserRole>()
                .Where(ur => targetRoleIds.Contains(ur.RoleId))
                .GroupBy(ur => ur.UserId)
                .Select(ur => ur.UserId)
                .ToListAsync();
        }

        // 创建阅读记录
        if (targetUserIds.Count > 0)
        {
            // 检查已存在的阅读记录（公告可能曾被发布后撤回）
            var existingUserIds = await _db.Queryable<AnnouncementRead>()
                .Where(r => r.AnnouncementId == id)
                .Select(r => r.UserId)
                .ToListAsync();

            // 只为不存在记录的用户创建阅读记录
            var newUserIds = targetUserIds.Except(existingUserIds).ToList();
            if (newUserIds.Count > 0)
            {
                var readRecords = newUserIds.Select(userId => new AnnouncementRead
                {
                    AnnouncementId = id,
                    UserId = userId,
                    IsRead = 0,
                    CreateTime = DateTime.Now
                }).ToList();

                await _db.Insertable(readRecords).ExecuteCommandAsync();
            }
        }

        _logger.LogInformation("公告发布成功，ID: {AnnouncementId}, 标题: {Title}, 目标用户数: {UserCount}",
            id, announcement.Title, targetUserIds.Count);

        return result;
    }

    /// <summary>
    /// 撤回公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 将已发布的公告撤回，撤回后用户无法查看。
    /// 撤回时会记录撤回时间，公告状态变为已撤回。
    /// 只能撤回已发布的公告，草稿状态的公告不需要撤回。
    /// </remarks>
    public async Task<int> RecallAsync(Guid id)
    {
        var announcement = await base.GetByIdAsync(id);
        if (announcement == null)
        {
            throw BusinessException.NotFound("公告不存在");
        }

        // 检查公告状态，只有已发布状态可以撤回
        if (announcement.Status != 1)
        {
            throw BusinessException.BadRequest("只能撤回已发布的公告");
        }

        // 更新公告状态
        announcement.Status = 2;
        announcement.RecallTime = DateTime.Now;
        announcement.UpdateTime = DateTime.Now;

        // 取消置顶
        if (announcement.IsTop == 1)
        {
            announcement.IsTop = 0;
            announcement.TopTime = null;
        }

        var result = await base.UpdateAsync(announcement);

        _logger.LogInformation("公告撤回成功，ID: {AnnouncementId}, 标题: {Title}",
            id, announcement.Title);

        return result;
    }

    /// <summary>
    /// 删除公告
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 删除公告及其阅读记录。
    /// 只能删除草稿或已撤回状态的公告，已发布的公告不可删除。
    /// 删除操作不可恢复，需谨慎使用。
    /// </remarks>
    public new async Task<int> DeleteAsync(Guid id)
    {
        var announcement = await base.GetByIdAsync(id);
        if (announcement == null)
        {
            throw BusinessException.NotFound("公告不存在");
        }

        // 检查公告状态，已发布的公告不能删除
        if (announcement.Status == 1)
        {
            throw BusinessException.BadRequest("已发布的公告不能删除，请先撤回");
        }

        // 删除阅读记录
        await _db.Deleteable<AnnouncementRead>()
            .Where(ar => ar.AnnouncementId == id)
            .ExecuteCommandAsync();

        // 删除公告
        var result = await base.DeleteAsync(id);

        _logger.LogInformation("公告删除成功，ID: {AnnouncementId}, 标题: {Title}",
            id, announcement.Title);

        return result;
    }

    /// <summary>
    /// 切换置顶状态
    /// </summary>
    /// <param name="id">公告ID</param>
    /// <param name="isTop">置顶状态：0取消置顶 1置顶</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 切换公告的置顶状态。
    /// 置顶公告会在列表中优先显示，取消置顶后按正常排序显示。
    /// 置顶时会记录置顶时间，取消置顶时清除置顶时间。
    /// 只有已发布的公告可以置顶。
    /// </remarks>
    public async Task<int> ToggleTopAsync(Guid id, int isTop)
    {
        var announcement = await base.GetByIdAsync(id);
        if (announcement == null)
        {
            throw BusinessException.NotFound("公告不存在");
        }

        // 检查公告状态，只有已发布的公告可以置顶
        if (announcement.Status != 1)
        {
            throw BusinessException.BadRequest("只有已发布的公告可以置顶");
        }

        // 更新置顶状态
        announcement.IsTop = isTop;
        announcement.TopTime = isTop == 1 ? DateTime.Now : null;
        announcement.UpdateTime = DateTime.Now;

        var result = await base.UpdateAsync(announcement);

        _logger.LogInformation("公告置顶状态更新，ID: {AnnouncementId}, 标题: {Title}, 是否置顶: {IsTop}",
            id, announcement.Title, isTop);

        return result;
    }

    /// <summary>
    /// 标记公告为已读
    /// </summary>
    /// <param name="announcementId">公告ID</param>
    /// <param name="userId">用户ID</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 标记指定用户对指定公告的阅读状态为已读。
    /// 标记时会记录阅读时间。
    /// 如果用户已经标记为已读，再次标记会更新阅读时间。
    /// </remarks>
    public async Task<int> MarkReadAsync(Guid announcementId, Guid userId)
    {
        // 检查公告是否存在且已发布
        var announcement = await base.GetByIdAsync(announcementId);
        if (announcement == null)
        {
            throw BusinessException.NotFound("公告不存在");
        }

        if (announcement.Status != 1)
        {
            throw BusinessException.BadRequest("只能标记已发布的公告");
        }

        // 检查阅读记录是否存在
        var readRecord = await _db.Queryable<AnnouncementRead>()
            .Where(ar => ar.AnnouncementId == announcementId && ar.UserId == userId)
            .FirstAsync();

        if (readRecord == null)
        {
            // 如果用户不是目标用户，仍然创建阅读记录（表示用户主动查看）
            readRecord = new AnnouncementRead
            {
                AnnouncementId = announcementId,
                UserId = userId,
                IsRead = 1,
                ReadTime = DateTime.Now,
                CreateTime = DateTime.Now
            };

            await _db.Insertable(readRecord).ExecuteCommandAsync();

            _logger.LogInformation("用户 {UserId} 创建并标记已读公告 {AnnouncementId}",
                userId, announcementId);

            return 1;
        }

        // 更新阅读状态
        var affected = await _db.Updateable<AnnouncementRead>()
            .Where(ar => ar.AnnouncementId == announcementId && ar.UserId == userId)
            .SetColumns(ar => new AnnouncementRead { IsRead = 1, ReadTime = DateTime.Now })
            .ExecuteCommandAsync();

        if (affected > 0)
        {
            _logger.LogInformation("用户 {UserId} 已读公告 {AnnouncementId}",
                userId, announcementId);
        }

        return affected;
    }

    /// <summary>
    /// 获取用户的未读公告列表
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <returns>未读公告列表，最多返回100条</returns>
    /// <remarks>
    /// 获取指定用户的未读公告列表。
    /// 只返回已发布且未撤回的公告。
    /// 置顶公告优先显示，然后按发布时间倒序排列。
    /// </remarks>
    public async Task<List<AnnouncementDto>> GetUnreadListAsync(Guid userId)
    {
        // 查询用户的未读公告
        var list = await _db.Queryable<AnnouncementRead, AnnouncementEntity>((ar, a) => new JoinQueryInfos(
            JoinType.Inner, ar.AnnouncementId == a.Id
        ))
            .Where((ar, a) => ar.UserId == userId && ar.IsRead == 0 && a.Status == 1)
            .OrderByDescending((ar, a) => a.IsTop)
            .OrderByDescending((ar, a) => a.PublishTime)
            .Select((ar, a) => new AnnouncementDto
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                Type = a.Type,
                Level = a.Level,
                IsTop = a.IsTop,
                TopTime = a.TopTime,
                PublishTime = a.PublishTime,
                Status = a.Status,
                CreatorId = a.CreatorId,
                CreateTime = a.CreateTime
            })
            .Take(100)
            .ToListAsync();

        // 填充额外信息
        foreach (var dto in list)
        {
            dto.TypeName = GetTypeName(dto.Type);
            dto.LevelName = GetLevelName(dto.Level);
            dto.StatusName = GetStatusName(dto.Status);

            // 获取创建人名称
            var creator = await _db.Queryable<User>()
                .Where(u => u.Id == dto.CreatorId)
                .Select(u => u.RealName ?? u.UserName)
                .FirstAsync();
            dto.CreatorName = creator;
        }

        return list;
    }

    /// <summary>
    /// 获取公告阅读统计
    /// </summary>
    /// <param name="announcementId">公告ID</param>
    /// <returns>阅读统计数据，包含已读人数、未读人数、阅读率等</returns>
    /// <remarks>
    /// 统计指定公告的阅读情况。
    /// 返回已阅读人数、未阅读人数、总目标人数和阅读率。
    /// 阅读率 = 已阅读人数 / 总目标人数 * 100%。
    /// </remarks>
    public async Task<ReadStatsDto> GetReadStatsAsync(Guid announcementId)
    {
        // 检查公告是否存在
        var announcement = await base.GetByIdAsync(announcementId);
        if (announcement == null)
        {
            throw BusinessException.NotFound("公告不存在");
        }

        // 统计已读人数
        var readCount = await _db.Queryable<AnnouncementRead>()
            .Where(ar => ar.AnnouncementId == announcementId && ar.IsRead == 1)
            .CountAsync();

        // 统计总目标人数
        var totalCount = await _db.Queryable<AnnouncementRead>()
            .Where(ar => ar.AnnouncementId == announcementId)
            .CountAsync();

        // 计算未读人数
        var unreadCount = totalCount - readCount;

        // 计算阅读率
        var readRate = totalCount > 0 ? Math.Round((decimal)readCount / totalCount * 100, 2) : 0;

        return new ReadStatsDto
        {
            AnnouncementId = announcementId,
            Title = announcement.Title,
            ReadCount = readCount,
            UnreadCount = unreadCount,
            TotalCount = totalCount,
            ReadRate = readRate
        };
    }

    /// <summary>
    /// 获取公告阅读详情列表（分页）
    /// </summary>
    /// <param name="announcementId">公告ID</param>
    /// <param name="pageIndex">页码，从1开始</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="isRead">阅读状态筛选，可选。null-全部 0-未读 1-已读</param>
    /// <returns>分页阅读详情列表，包含用户信息和阅读状态</returns>
    /// <remarks>
    /// 获取指定公告的用户阅读详情列表。
    /// 包含用户基本信息（用户名、真实姓名、角色）和阅读状态（是否已读、阅读时间）。
    /// 支持按阅读状态筛选。
    /// 按阅读时间倒序排列，未读用户按创建时间排序。
    /// </remarks>
    public async Task<PageResponse<ReadDetailDto>> GetReadDetailAsync(Guid announcementId, int pageIndex, int pageSize, int? isRead = null)
    {
        // 检查公告是否存在
        var announcement = await base.GetByIdAsync(announcementId);
        if (announcement == null)
        {
            throw BusinessException.NotFound("公告不存在");
        }

        var total = new RefAsync<int>();

        // 查询阅读详情
        var query = _db.Queryable<AnnouncementRead, User, UserRole, Role>(
            (ar, u, ur, r) => new JoinQueryInfos(
                JoinType.Inner, ar.UserId == u.Id,
                JoinType.Left, ur.UserId == u.Id,
                JoinType.Left, ur.RoleId == r.Id
            ))
            .Where((ar, u, ur, r) => ar.AnnouncementId == announcementId)
            .WhereIF(isRead.HasValue, (ar, u, ur, r) => ar.IsRead == isRead!.Value)
            // 排序：已读按阅读时间倒序，未读按创建时间倒序
            .OrderByDescending((ar, u, ur, r) => ar.IsRead)
            .OrderByDescending((ar, u, ur, r) => ar.ReadTime)
            .OrderByDescending((ar, u, ur, r) => ar.CreateTime);

        var list = await query
            .Select((ar, u, ur, r) => new ReadDetailDto
            {
                Id = ar.Id,
                AnnouncementId = ar.AnnouncementId,
                UserId = ar.UserId,
                UserName = u.UserName,
                RealName = u.RealName,
                RoleName = r.RoleName,
                IsRead = ar.IsRead,
                ReadTime = ar.ReadTime,
                CreateTime = ar.CreateTime
            })
            .ToPageListAsync(pageIndex, pageSize, total);

        // 处理用户有多个角色的情况（合并角色名称）
        var userIds = list.Select(x => x.UserId).Distinct().ToList();
        var userRoles = await _db.Queryable<UserRole, Role>((ur, r) => new JoinQueryInfos(
            JoinType.Left, ur.RoleId == r.Id
        ))
            .Where((ur, r) => userIds.Contains(ur.UserId))
            .Select((ur, r) => new { UserId = ur.UserId, RoleName = r.RoleName })
            .ToListAsync();

        // 按用户ID分组角色名称
        var roleDict = userRoles.GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.RoleName).ToList());

        // 更新角色名称（多个角色用逗号分隔）
        foreach (var dto in list)
        {
            if (roleDict.TryGetValue(dto.UserId, out var roles))
            {
                dto.RoleName = roles.Count > 0 ? string.Join(",", roles) : "";
            }
        }

        return PageResponse<ReadDetailDto>.Create(list, total.Value, pageIndex, pageSize);
    }
}