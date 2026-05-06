using EasyWeChatModels.Dto.AntWorkflow;
using CommonManager.Base;

namespace BusinessManager.Buz.IService;

/// <summary>
/// Ant流程任务服务接口
/// </summary>
public interface IAntWorkflowTaskService
{
    /// <summary>
    /// 获取待办任务列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <param name="userId">用户ID</param>
    /// <param name="isAdmin">是否为管理员（管理员可查看所有任务）</param>
    /// <returns>分页结果</returns>
    Task<PageResponse<AntWorkflowTaskDto>> GetTodoTasksAsync(QueryTodoTaskDto query, Guid userId, bool isAdmin = false);

    /// <summary>
    /// 获取已办任务列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <param name="userId">用户ID</param>
    /// <param name="isAdmin">是否为管理员（管理员可查看所有任务）</param>
    /// <returns>分页结果</returns>
    Task<PageResponse<AntWorkflowTaskDto>> GetDoneTasksAsync(QueryDoneTaskDto query, Guid userId, bool isAdmin = false);

    /// <summary>
    /// 获取抄送任务列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <param name="userId">用户ID</param>
    /// <param name="isAdmin">是否为管理员（管理员可查看所有任务）</param>
    /// <returns>分页结果</returns>
    Task<PageResponse<AntWorkflowCcDto>> GetCcTasksAsync(QueryCcTaskDto query, Guid userId, bool isAdmin = false);

    /// <summary>
    /// 获取任务详情
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <returns>任务详情，不存在返回null</returns>
    Task<AntWorkflowTaskDto?> GetTaskDetailAsync(Guid taskId);

    /// <summary>
    /// 审批通过
    /// </summary>
    /// <param name="dto">审批参数</param>
    /// <param name="userId">用户ID</param>
    /// <param name="userName">用户姓名</param>
    /// <returns>影响行数</returns>
    Task<int> ApproveAsync(ApproveAntTaskDto dto, Guid userId, string userName);

    /// <summary>
    /// 审批驳回
    /// </summary>
    /// <param name="dto">驳回参数</param>
    /// <param name="userId">用户ID</param>
    /// <param name="userName">用户姓名</param>
    /// <returns>影响行数</returns>
    Task<int> RejectAsync(RejectAntTaskDto dto, Guid userId, string userName);

    /// <summary>
    /// 转办任务
    /// </summary>
    /// <param name="dto">转办参数</param>
    /// <param name="userId">用户ID</param>
    /// <param name="userName">用户姓名</param>
    /// <returns>影响行数</returns>
    Task<int> TransferAsync(TransferAntTaskDto dto, Guid userId, string userName);

    /// <summary>
    /// 加签
    /// </summary>
    /// <param name="dto">加签参数</param>
    /// <param name="userId">用户ID</param>
    /// <param name="userName">用户姓名</param>
    /// <returns>影响行数</returns>
    Task<int> AddSignerAsync(AddSignerAntTaskDto dto, Guid userId, string userName);

    /// <summary>
    /// 标记抄送已读
    /// </summary>
    /// <param name="ids">抄送记录ID列表</param>
    /// <param name="userId">用户ID</param>
    /// <returns>影响行数</returns>
    Task<int> MarkCcReadAsync(List<Guid> ids, Guid userId);
}