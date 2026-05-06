// BusinessManager/Buz/AntWorkflow/Service/ApproverResolverService.cs
using Newtonsoft.Json.Linq;
using SqlSugar;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.IService;
using CommonManager.Base;
using Microsoft.Extensions.Logging;

namespace BusinessManager.Buz.AntWorkflow.Service;

/// <summary>
/// 审批人获取服务实现
/// </summary>
public class ApproverResolverService : BaseService<User>, IApproverResolverService
{
    /// <summary>数据库客户端（Autofac 属性注入）</summary>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>日志服务（Autofac 属性注入）</summary>
    public ILogger<ApproverResolverService> _logger { get; set; } = null!;

    /// <inheritdoc/>
    public async Task<List<NodeUser>> GetApproversAsync(NodeHandlerContext context, ApproverNodeConfig config)
    {
        var handlers = new List<NodeUser>();

        switch (config.SetType)
        {
            case ApproverSetType.FixedUser:
                handlers = config.NodeUserList ?? new List<NodeUser>();
                break;

            case ApproverSetType.InitiatorSelf:
                if (context.Instance.InitiatorId.HasValue)
                {
                    var initiator = await _db.Queryable<User>()
                        .Where(u => u.Id == context.Instance.InitiatorId.Value)
                        .FirstAsync();
                    handlers.Add(new NodeUser
                    {
                        TargetId = context.Instance.InitiatorId.Value,
                        Name = initiator?.UserName ?? "",
                        Type = 1
                    });
                }
                break;

            case ApproverSetType.Role:
                handlers = await GetRoleUsersAsync(config);
                break;

            case ApproverSetType.Supervisor:
                var supervisor = await GetSupervisorAsync(context.Instance.InitiatorId ?? Guid.Empty);
                if (supervisor != null) handlers.Add(supervisor);
                break;

            case ApproverSetType.MultiSupervisor:
                handlers = await GetMultiSupervisorAsync(
                    context.Instance.InitiatorId ?? Guid.Empty,
                    config.DirectorLevel ?? 1);
                break;

            case ApproverSetType.FormField:
                handlers = await GetFormFieldUsersAsync(context, config);
                break;
        }

        _logger.LogInformation("获取审批人: SetType={SetType}, Count={Count}", config.SetType, handlers.Count);
        return handlers;
    }

    /// <summary>
    /// 根据角色获取审批人
    /// </summary>
    private async Task<List<NodeUser>> GetRoleUsersAsync(ApproverNodeConfig config)
    {
        var handlers = new List<NodeUser>();

        if (config.NodeUserList == null || config.NodeUserList.Count == 0)
            return handlers;

        var roleId = config.NodeUserList.FirstOrDefault()?.TargetId ?? Guid.Empty;
        if (roleId == Guid.Empty) return handlers;

        // 查询角色下的用户
        var roleUsers = await _db.Queryable<UserRole>()
            .Where(ur => ur.RoleId == roleId)
            .ToListAsync();

        var userIds = roleUsers.Select(ur => ur.UserId).ToList();
        var users = await _db.Queryable<User>()
            .Where(u => userIds.Contains(u.Id) && u.Status == 1)
            .ToListAsync();

        handlers = users.Select(u => new NodeUser
        {
            TargetId = u.Id,
            Name = u.UserName,
            Type = 1
        }).ToList();

        return handlers;
    }

    /// <inheritdoc/>
    public async Task<NodeUser?> GetSupervisorAsync(Guid userId)
    {
        var user = await _db.Queryable<User>()
            .Where(u => u.Id == userId)
            .FirstAsync();

        if (user?.DepartmentId == null) return null;

        var dept = await _db.Queryable<Department>()
            .Where(d => d.Id == user.DepartmentId.Value)
            .FirstAsync();

        if (dept?.ManagerId == null) return null;

        var manager = await _db.Queryable<User>()
            .Where(u => u.Id == dept.ManagerId.Value && u.Status == 1)
            .FirstAsync();

        return manager != null
            ? new NodeUser { TargetId = dept.ManagerId.Value, Name = manager.UserName, Type = 1 }
            : null;
    }

    /// <inheritdoc/>
    public async Task<List<NodeUser>> GetMultiSupervisorAsync(Guid userId, int level)
    {
        var result = new List<NodeUser>();
        var currentUserId = userId;

        for (int i = 0; i < level; i++)
        {
            var supervisor = await GetSupervisorAsync(currentUserId);
            if (supervisor == null) break;

            result.Add(supervisor);
            currentUserId = supervisor.TargetId;
        }

        return result;
    }

    /// <summary>
    /// 从表单字段获取审批人
    /// </summary>
    private async Task<List<NodeUser>> GetFormFieldUsersAsync(NodeHandlerContext context, ApproverNodeConfig config)
    {
        var handlers = new List<NodeUser>();

        if (string.IsNullOrEmpty(context.FormData)) return handlers;

        var formData = JObject.Parse(context.FormData);
        var fieldId = config.NodeUserList?.FirstOrDefault()?.TargetId ?? Guid.Empty;
        var fieldIdStr = fieldId.ToString();

        if (fieldId == Guid.Empty || !formData.TryGetValue(fieldIdStr, out var fieldValue))
            return handlers;

        // 字段值可能是单个用户ID或用户ID数组
        if (fieldValue.Type == JTokenType.Array)
        {
            foreach (var id in fieldValue)
            {
                var userId = Guid.Parse(id.ToString());
                var user = await _db.Queryable<User>()
                    .Where(u => u.Id == userId && u.Status == 1)
                    .FirstAsync();
                if (user != null)
                {
                    handlers.Add(new NodeUser { TargetId = userId, Name = user.UserName, Type = 1 });
                }
            }
        }
        else
        {
            var userId = Guid.Parse(fieldValue.ToString());
            var user = await _db.Queryable<User>()
                .Where(u => u.Id == userId && u.Status == 1)
                .FirstAsync();
            if (user != null)
            {
                handlers.Add(new NodeUser { TargetId = userId, Name = user.UserName, Type = 1 });
            }
        }

        return handlers;
    }

    /// <inheritdoc/>
    public async Task<NodeUser?> GetAdminAsync()
    {
        // 获取管理员角色的用户（假设管理员角色名称为 "admin"）
        var adminRole = await _db.Queryable<Role>()
            .Where(r => r.RoleName == "admin")
            .FirstAsync();

        if (adminRole == null) return null;

        var roleUser = await _db.Queryable<UserRole>()
            .Where(ur => ur.RoleId == adminRole.Id)
            .FirstAsync();

        if (roleUser == null) return null;

        var admin = await _db.Queryable<User>()
            .Where(u => u.Id == roleUser.UserId && u.Status == 1)
            .FirstAsync();

        return admin != null
            ? new NodeUser { TargetId = admin.Id, Name = admin.UserName, Type = 1 }
            : null;
    }
}