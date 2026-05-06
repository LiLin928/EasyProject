using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 消息服务实现类
/// </summary>
/// <remarks>
/// 实现消息相关的业务逻辑，包括消息发送、接收、阅读状态管理等功能。
/// 继承自<see cref="BaseService{Message}"/>，使用SqlSugar进行数据库操作。
/// 支持单发和群发消息，支持消息类型和级别分类。
/// </remarks>
public class MessageService : BaseService<Message>, IMessageService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<MessageService> _logger { get; set; } = null!;

    /// <summary>
    /// 获取消息类型名称
    /// </summary>
    /// <param name="type">消息类型代码</param>
    /// <returns>消息类型的中文描述</returns>
    /// <remarks>
    /// 类型映射：1-系统消息 2-通知 3-提醒
    /// </remarks>
    private static string GetTypeName(int type)
    {
        return type switch
        {
            1 => "系统消息",
            2 => "通知",
            3 => "提醒",
            _ => "未知"
        };
    }

    /// <summary>
    /// 获取消息级别名称
    /// </summary>
    /// <param name="level">消息级别代码</param>
    /// <returns>消息级别的中文描述</returns>
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
    /// 获取用户的消息列表（分页）
    /// </summary>
    /// <param name="userId">用户ID，获取该用户的消息列表</param>
    /// <param name="pageIndex">页码索引，从1开始</param>
    /// <param name="pageSize">每页显示数量</param>
    /// <param name="type">消息类型筛选，可选。1-系统消息 2-通知 3-提醒</param>
    /// <param name="isRead">阅读状态筛选，可选。null-全部 0-未读 1-已读</param>
    /// <returns>分页消息列表，包含消息详情和用户阅读状态</returns>
    /// <remarks>
    /// 查询逻辑：
    /// <list type="number">
    ///     <item>从MessageUser表查询用户的消息关联记录</item>
    ///     <item>关联Message表获取消息详情</item>
    ///     <item>过滤已删除的消息</item>
    ///     <item>根据类型和阅读状态筛选</item>
    ///     <item>按创建时间倒序排列</item>
    /// </list>
    /// </remarks>
    public async Task<PageResponse<MessageDto>> GetPageListAsync(Guid userId, int pageIndex, int pageSize, int? type = null, int? isRead = null)
    {
        var total = new RefAsync<int>();

        var query = _db.Queryable<MessageUser, Message>((mu, m) => new JoinQueryInfos(
            JoinType.Inner, mu.MessageId == m.Id
        ))
            .Where((mu, m) => mu.UserId == userId && mu.IsDeleted == false && m.Status == 1)
            .WhereIF(type.HasValue, (mu, m) => m.Type == type!.Value)
            .WhereIF(isRead.HasValue, (mu, m) => mu.IsRead == (isRead!.Value == 1))
            .OrderByDescending((mu, m) => m.CreateTime);

        var list = await query
            .Select((mu, m) => new MessageDto
            {
                Id = m.Id,
                Title = m.Title,
                Content = m.Content,
                Type = m.Type,
                TypeName = GetTypeName(m.Type),
                Level = m.Level,
                LevelName = GetLevelName(m.Level),
                SenderId = m.SenderId,
                SenderName = m.SenderName,
                IsRead = mu.IsRead,
                ReadTime = mu.ReadTime,
                CreateTime = m.CreateTime,
                MessageUserId = mu.Id
            })
            .ToPageListAsync(pageIndex, pageSize, total);

        return PageResponse<MessageDto>.Create(list, total.Value, pageIndex, pageSize);
    }

    /// <summary>
    /// 获取用户的未读消息列表
    /// </summary>
    /// <param name="userId">用户ID，获取该用户的未读消息</param>
    /// <returns>未读消息列表，最多返回100条</returns>
    /// <remarks>
    /// 查询用户所有未读且未删除的消息。
    /// 按消息级别倒序（紧急优先）和创建时间倒序排列。
    /// </remarks>
    public async Task<List<MessageDto>> GetUnreadListAsync(Guid userId)
    {
        var list = await _db.Queryable<MessageUser, Message>((mu, m) => new JoinQueryInfos(
            JoinType.Inner, mu.MessageId == m.Id
        ))
            .Where((mu, m) => mu.UserId == userId && mu.IsRead == false && mu.IsDeleted == false && m.Status == 1)
            .OrderByDescending((mu, m) => m.Level)
            .OrderByDescending((mu, m) => m.CreateTime)
            .Select((mu, m) => new MessageDto
            {
                Id = m.Id,
                Title = m.Title,
                Content = m.Content,
                Type = m.Type,
                TypeName = GetTypeName(m.Type),
                Level = m.Level,
                LevelName = GetLevelName(m.Level),
                SenderId = m.SenderId,
                SenderName = m.SenderName,
                IsRead = mu.IsRead,
                ReadTime = mu.ReadTime,
                CreateTime = m.CreateTime,
                MessageUserId = mu.Id
            })
            .Take(100)
            .ToListAsync();

        return list;
    }

    /// <summary>
    /// 获取用户的未读消息数量
    /// </summary>
    /// <param name="userId">用户ID，统计该用户的未读消息</param>
    /// <returns>未读消息数量</returns>
    /// <remarks>
    /// 统计MessageUser表中IsRead=false且IsDeleted=false的记录数。
    /// </remarks>
    public async Task<int> GetUnreadCountAsync(Guid userId)
    {
        return await _db.Queryable<MessageUser, Message>((mu, m) => new JoinQueryInfos(
            JoinType.Inner, mu.MessageId == m.Id
        ))
            .Where((mu, m) => mu.UserId == userId && mu.IsRead == false && mu.IsDeleted == false && m.Status == 1)
            .CountAsync();
    }

    /// <summary>
    /// 获取消息详情
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <param name="userId">用户ID，用于获取该用户对此消息的阅读状态</param>
    /// <returns>消息详细信息，包含用户阅读状态；消息不存在或用户无权限查看返回null</returns>
    /// <remarks>
    /// 获取消息详情时会自动将消息标记为已读。
    /// 通过MessageUser关联验证用户是否有权限查看此消息。
    /// </remarks>
    public async Task<MessageDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var result = await _db.Queryable<MessageUser, Message>((mu, m) => new JoinQueryInfos(
            JoinType.Inner, mu.MessageId == m.Id
        ))
            .Where((mu, m) => mu.MessageId == id && mu.UserId == userId && mu.IsDeleted == false && m.Status == 1)
            .Select((mu, m) => new MessageDto
            {
                Id = m.Id,
                Title = m.Title,
                Content = m.Content,
                Type = m.Type,
                TypeName = GetTypeName(m.Type),
                Level = m.Level,
                LevelName = GetLevelName(m.Level),
                SenderId = m.SenderId,
                SenderName = m.SenderName,
                IsRead = mu.IsRead,
                ReadTime = mu.ReadTime,
                CreateTime = m.CreateTime,
                MessageUserId = mu.Id
            })
            .FirstAsync();

        if (result == null)
        {
            return null;
        }

        // 自动标记为已读
        if (!result.IsRead)
        {
            await MarkReadAsync(id, userId);
            result.IsRead = true;
            result.ReadTime = DateTime.Now;
        }

        return result;
    }

    /// <summary>
    /// 发送消息给指定用户
    /// </summary>
    /// <param name="dto">发送消息参数，包含消息内容和接收者用户ID列表</param>
    /// <returns>新创建的消息ID</returns>
    /// <remarks>
    /// 发送流程：
    /// <list type="number">
    ///     <item>创建Message记录</item>
    ///     <item>获取UserIds列表中的用户信息</item>
    ///     <item>为每个用户创建MessageUser关联记录</item>
    /// </list>
    /// </remarks>
    public async Task<Guid> SendAsync(SendMessageDto dto)
    {
        // 创建消息记录
        var message = new Message
        {
            Title = dto.Title,
            Content = dto.Content,
            Type = dto.Type,
            Level = dto.Level,
            SenderId = dto.SenderId,
            SenderName = dto.SenderName ?? "系统",
            Status = 1,
            CreateTime = DateTime.Now
        };

        // 插入消息并获取ID
        var messageId = message.Id;
        await _db.Insertable(message).ExecuteCommandAsync();

        // 为接收者创建关联记录
        if (dto.UserIds != null && dto.UserIds.Count > 0)
        {
            var messageUsers = dto.UserIds.Select(userId => new MessageUser
            {
                MessageId = messageId,
                UserId = userId,
                IsRead = false,
                IsDeleted = false,
                CreateTime = DateTime.Now
            }).ToList();

            await _db.Insertable(messageUsers).ExecuteCommandAsync();
        }

        _logger.LogInformation("消息发送成功，ID: {MessageId}, 接收人数: {ReceiverCount}",
            messageId, dto.UserIds?.Count ?? 0);

        return messageId;
    }

    /// <summary>
    /// 发送消息给所有用户
    /// </summary>
    /// <param name="dto">发送消息参数，包含消息内容。UserIds参数将被忽略</param>
    /// <returns>新创建的消息ID</returns>
    /// <remarks>
    /// 系统广播消息，发送给系统中所有状态正常的用户。
    /// 查询所有Status=1的用户并创建MessageUser关联记录。
    /// </remarks>
    public async Task<Guid> SendToAllAsync(SendMessageDto dto)
    {
        // 创建消息记录
        var message = new Message
        {
            Title = dto.Title,
            Content = dto.Content,
            Type = dto.Type,
            Level = dto.Level,
            SenderId = dto.SenderId,
            SenderName = dto.SenderName ?? "系统",
            Status = 1,
            CreateTime = DateTime.Now
        };

        // 插入消息并获取ID
        var messageId = message.Id;
        await _db.Insertable(message).ExecuteCommandAsync();

        // 获取所有活跃用户
        var users = await _db.Queryable<User>()
            .Where(u => u.Status == 1)
            .Select(u => u.Id)
            .ToListAsync();

        if (users.Count > 0)
        {
            var messageUsers = users.Select(userId => new MessageUser
            {
                MessageId = messageId,
                UserId = userId,
                IsRead = false,
                IsDeleted = false,
                CreateTime = DateTime.Now
            }).ToList();

            await _db.Insertable(messageUsers).ExecuteCommandAsync();
        }

        _logger.LogInformation("系统广播消息发送成功，ID: {MessageId}, 接收人数: {ReceiverCount}",
            messageId, users.Count);

        return messageId;
    }

    /// <summary>
    /// 标记消息为已读
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <param name="userId">用户ID，标记该用户对此消息的阅读状态</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 更新MessageUser表中对应记录的IsRead和ReadTime字段。
    /// 只更新未读的消息，已读的消息不会被重复更新。
    /// </remarks>
    public async Task<int> MarkReadAsync(Guid id, Guid userId)
    {
        var affected = await _db.Updateable<MessageUser>()
            .Where(mu => mu.MessageId == id && mu.UserId == userId && mu.IsRead == false)
            .SetColumns(mu => mu.IsRead == true)
            .SetColumns(mu => mu.ReadTime == DateTime.Now)
            .ExecuteCommandAsync();

        if (affected > 0)
        {
            _logger.LogInformation("用户 {UserId} 已读消息 {MessageId}", userId, id);
        }

        return affected;
    }

    /// <summary>
    /// 标记用户所有消息为已读
    /// </summary>
    /// <param name="userId">用户ID，标记该用户所有未读消息为已读</param>
    /// <returns>影响的行数，即标记为已读的消息数量</returns>
    /// <remarks>
    /// 一键将所有未读且未删除的消息标记为已读。
    /// 批量更新MessageUser表中符合条件的所有记录。
    /// </remarks>
    public async Task<int> MarkAllReadAsync(Guid userId)
    {
        var affected = await _db.Updateable<MessageUser>()
            .Where(mu => mu.UserId == userId && mu.IsRead == false && mu.IsDeleted == false)
            .SetColumns(mu => mu.IsRead == true)
            .SetColumns(mu => mu.ReadTime == DateTime.Now)
            .ExecuteCommandAsync();

        if (affected > 0)
        {
            _logger.LogInformation("用户 {UserId} 已全部标记已读，数量: {Count}", userId, affected);
        }

        return affected;
    }

    /// <summary>
    /// 删除消息（软删除）
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <param name="userId">用户ID，删除该用户与此消息的关联</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 软删除，只更新MessageUser表的IsDeleted和DeleteTime字段。
    /// 消息本身不会被物理删除，其他用户仍可查看。
    /// 只删除未删除的关联记录。
    /// </remarks>
    public async Task<int> DeleteAsync(Guid id, Guid userId)
    {
        var affected = await _db.Updateable<MessageUser>()
            .Where(mu => mu.MessageId == id && mu.UserId == userId && mu.IsDeleted == false)
            .SetColumns(mu => mu.IsDeleted == true)
            .SetColumns(mu => mu.DeleteTime == DateTime.Now)
            .ExecuteCommandAsync();

        if (affected > 0)
        {
            _logger.LogInformation("用户 {UserId} 已删除消息 {MessageId}", userId, id);
        }

        return affected;
    }
}