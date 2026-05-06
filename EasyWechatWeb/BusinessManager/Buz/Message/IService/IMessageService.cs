using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 消息服务接口
/// </summary>
/// <remarks>
/// 提供消息的发送、接收、阅读状态管理等功能。
/// 支持系统消息、通知、提醒等多种消息类型。
/// 支持单发和群发消息，支持消息的阅读状态跟踪和删除管理。
/// </remarks>
public interface IMessageService
{
    /// <summary>
    /// 获取用户的消息列表（分页）
    /// </summary>
    /// <param name="userId">用户ID，获取该用户的消息列表</param>
    /// <param name="pageIndex">页码索引，从1开始</param>
    /// <param name="pageSize">每页显示数量</param>
    /// <param name="type">消息类型筛选，可选。1-系统消息 2-通知 3-提醒</param>
    /// <param name="isRead">阅读状态筛选，可选。null-全部 0-未读 1-已读</param>
    /// <returns>分页消息列表，包含消息详情和用户阅读状态</returns>
    Task<PageResponse<MessageDto>> GetPageListAsync(Guid userId, int pageIndex, int pageSize, int? type = null, int? isRead = null);

    /// <summary>
    /// 获取用户的未读消息列表
    /// </summary>
    /// <param name="userId">用户ID，获取该用户的未读消息</param>
    /// <returns>未读消息列表，最多返回100条</returns>
    Task<List<MessageDto>> GetUnreadListAsync(Guid userId);

    /// <summary>
    /// 获取用户的未读消息数量
    /// </summary>
    /// <param name="userId">用户ID，统计该用户的未读消息</param>
    /// <returns>未读消息数量</returns>
    Task<int> GetUnreadCountAsync(Guid userId);

    /// <summary>
    /// 获取消息详情
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <param name="userId">用户ID，用于获取该用户对此消息的阅读状态</param>
    /// <returns>消息详细信息，包含用户阅读状态；消息不存在或用户无权限查看返回null</returns>
    Task<MessageDto?> GetByIdAsync(Guid id, Guid userId);

    /// <summary>
    /// 发送消息给指定用户
    /// </summary>
    /// <param name="dto">发送消息参数，包含消息内容和接收者用户ID列表</param>
    /// <returns>新创建的消息ID</returns>
    Task<Guid> SendAsync(SendMessageDto dto);

    /// <summary>
    /// 发送消息给所有用户
    /// </summary>
    /// <param name="dto">发送消息参数，包含消息内容。UserIds参数将被忽略</param>
    /// <returns>新创建的消息ID</returns>
    Task<Guid> SendToAllAsync(SendMessageDto dto);

    /// <summary>
    /// 标记消息为已读
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <param name="userId">用户ID，标记该用户对此消息的阅读状态</param>
    /// <returns>影响的行数</returns>
    Task<int> MarkReadAsync(Guid id, Guid userId);

    /// <summary>
    /// 标记用户所有消息为已读
    /// </summary>
    /// <param name="userId">用户ID，标记该用户所有未读消息为已读</param>
    /// <returns>影响的行数，即标记为已读的消息数量</returns>
    Task<int> MarkAllReadAsync(Guid userId);

    /// <summary>
    /// 删除消息（软删除）
    /// </summary>
    /// <param name="id">消息ID</param>
    /// <param name="userId">用户ID，删除该用户与此消息的关联</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id, Guid userId);
}