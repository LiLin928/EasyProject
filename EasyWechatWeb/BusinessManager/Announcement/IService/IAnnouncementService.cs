using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Announcement.IService;

/// <summary>
/// 公告服务接口
/// </summary>
/// <remarks>
/// 提供系统公告的管理功能，包括公告的增删改查、发布撤回、置顶管理等。
/// 支持全员公告和定向公告，提供公告阅读状态跟踪和统计功能。
/// </remarks>
public interface IAnnouncementService
{
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
    Task<PageResponse<AnnouncementDto>> GetPageListAsync(QueryAnnouncementDto query);

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
    Task<AnnouncementDto?> GetByIdAsync(Guid id, Guid? currentUserId = null);

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
    Task<Guid> AddAsync(AddAnnouncementDto dto, Guid creatorId);

    /// <summary>
    /// 更新公告
    /// </summary>
    /// <param name="dto">更新公告参数，包含公告ID和更新的内容</param>
    /// <returns>影响的行数</returns>
    /// <remarks>
    /// 更新公告的标题、内容、类型、级别等信息。
    /// 只能更新草稿状态的公告，已发布的公告不可更新。
    /// 更新时会记录更新时间。
    /// </remarks>
    Task<int> UpdateAsync(UpdateAnnouncementDto dto);

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
    Task<int> PublishAsync(Guid id);

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
    Task<int> RecallAsync(Guid id);

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
    Task<int> DeleteAsync(Guid id);

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
    Task<int> ToggleTopAsync(Guid id, int isTop);

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
    Task<int> MarkReadAsync(Guid announcementId, Guid userId);

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
    Task<List<AnnouncementDto>> GetUnreadListAsync(Guid userId);

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
    Task<ReadStatsDto> GetReadStatsAsync(Guid announcementId);

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
    Task<PageResponse<ReadDetailDto>> GetReadDetailAsync(Guid announcementId, int pageIndex, int pageSize, int? isRead = null);
}