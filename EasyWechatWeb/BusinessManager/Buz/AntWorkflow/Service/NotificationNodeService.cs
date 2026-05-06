using Newtonsoft.Json;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.IService;
using BusinessManager.Buz.AntWorkflow.IService;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 通知节点处理器服务
/// </summary>
public class NotificationNodeService : INodeHandlerService
{
    public AntNodeType NodeType => AntNodeType.Notification;

    /// <summary>
    /// 审批人解析服务（属性注入）
    /// </summary>
    public IApproverResolverService _approverResolverService { get; set; } = null!;

    /// <inheritdoc/>
    public async Task HandleEnterAsync(NodeHandlerContext context)
    {
        // 解析通知节点配置（通知配置可能在 EndNodeConfig 或独立配置中）
        var notificationConfig = ParseNotificationConfig(context.DagNode.Config?.ToString());

        if (notificationConfig == null || !notificationConfig.Enabled)
        {
            // 通知未启用，直接完成
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            return;
        }

        // 发送通知
        await SendNotification(notificationConfig, context);

        // 通知节点自动完成
        context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
        await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task HandleCompleteAsync(NodeHandlerContext context)
    {
        // 通知节点无需额外处理
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetNextNodesAsync(NodeHandlerContext context)
    {
        // 根据边找到下一节点
        var edges = context.DagConfig.Edges
            .Where(e => e.SourceNodeId == context.DagNode.Id)
            .ToList();

        return edges.Select(e => e.TargetNodeId).ToList();
    }

    /// <summary>
    /// 解析通知配置
    /// </summary>
    private NotificationConfig? ParseNotificationConfig(string? configJson)
    {
        if (string.IsNullOrEmpty(configJson))
            return null;

        try
        {
            // 尝试直接解析为 NotificationConfig
            var config = JsonConvert.DeserializeObject<NotificationConfig>(configJson);
            if (config != null)
                return config;

            // 尝试解析为 EndNodeConfig（通知可能在结束节点配置中）
            var endConfig = JsonConvert.DeserializeObject<EndNodeConfig>(configJson);
            return endConfig?.Notification;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 发送通知
    /// </summary>
    private async Task SendNotification(NotificationConfig config, NodeHandlerContext context)
    {
        // 构建完整接收人列表
        var recipients = await BuildRecipientsList(config, context);

        if (recipients.Count == 0)
        {
            return;
        }

        // 构建通知内容
        var title = BuildNotificationTitle(config, context);
        var content = BuildNotificationContent(config, context);

        // 根据通知类型发送
        switch (config.Type?.ToLower())
        {
            case "message":
                // 发送系统消息
                await SendSystemMessage(recipients, title, content, context);
                break;

            case "email":
                // 发送邮件（需要集成邮件服务）
                // TODO: 集成邮件服务
                break;

            case "sms":
                // 发送短信（需要集成短信服务）
                // TODO: 集成短信服务
                break;

            case "wechat":
                // 发送微信通知（需要集成微信模板消息）
                // TODO: 集成微信通知服务
                break;

            default:
                // 默认发送系统消息
                await SendSystemMessage(recipients, title, content, context);
                break;
        }
    }

    /// <summary>
    /// 构建完整接收人列表
    /// </summary>
    private async Task<List<NodeUser>> BuildRecipientsList(NotificationConfig config, NodeHandlerContext context)
    {
        var recipients = new List<NodeUser>();

        // 添加配置的接收人
        if (config.Recipients != null && config.Recipients.Count > 0)
        {
            recipients.AddRange(config.Recipients);
        }

        // 发送给发起人
        if (config.SendToInitiator && context.Instance.InitiatorId.HasValue)
        {
            var initiator = await context.Db.Queryable<User>()
                .Where(u => u.Id == context.Instance.InitiatorId.Value)
                .FirstAsync();

            if (initiator != null)
            {
                recipients.Add(new NodeUser
                {
                    TargetId = initiator.Id,
                    Name = initiator.UserName,
                    Type = 1
                });
            }
        }

        // 发送给主管
        if (config.SendToSupervisor && context.Instance.InitiatorId.HasValue)
        {
            var supervisor = await _approverResolverService.GetSupervisorAsync(context.Instance.InitiatorId.Value);
            if (supervisor != null)
            {
                recipients.Add(supervisor);
            }
        }

        // 去重（根据 TargetId）
        return recipients.DistinctBy(r => r.TargetId).ToList();
    }

    /// <summary>
    /// 构建通知标题
    /// </summary>
    private string BuildNotificationTitle(NotificationConfig config, NodeHandlerContext context)
    {
        if (!string.IsNullOrEmpty(config.Title))
        {
            return ReplaceVariables(config.Title, context);
        }

        // 默认标题
        var statusText = GetStatusText(context.Instance.Status);
        return $"流程通知：{context.Instance.Title ?? "流程"} - {statusText}";
    }

    /// <summary>
    /// 构建通知内容
    /// </summary>
    private string BuildNotificationContent(NotificationConfig config, NodeHandlerContext context)
    {
        if (!string.IsNullOrEmpty(config.Content))
        {
            return ReplaceVariables(config.Content, context);
        }

        // 默认内容
        var statusText = GetStatusText(context.Instance.Status);
        return $"您的流程「{context.Instance.Title ?? "流程"}」状态已变更为：{statusText}";
    }

    /// <summary>
    /// 替换变量
    /// </summary>
    private string ReplaceVariables(string template, NodeHandlerContext context)
    {
        var result = template;

        // 替换实例信息
        result = result.Replace("{{InstanceId}}", context.Instance.Id.ToString());
        result = result.Replace("{{Title}}", context.Instance.Title ?? "");
        result = result.Replace("{{Status}}", GetStatusText(context.Instance.Status));
        result = result.Replace("{{InitiatorId}}", context.Instance.InitiatorId?.ToString() ?? "");

        // 替换节点信息
        result = result.Replace("{{NodeName}}", context.DagNode.Name);
        result = result.Replace("{{NodeId}}", context.DagNode.Id.ToString());

        // 替换操作人信息
        result = result.Replace("{{OperatorId}}", context.OperatorId?.ToString() ?? "");
        result = result.Replace("{{OperatorName}}", context.OperatorName ?? "");
        result = result.Replace("{{ApproveDesc}}", context.ApproveDesc ?? "");

        return result;
    }

    /// <summary>
    /// 获取状态文本
    /// </summary>
    private string GetStatusText(int status)
    {
        return status switch
        {
            0 => "待提交",
            1 => "审批中",
            2 => "已通过",
            3 => "已驳回",
            4 => "已撤回",
            5 => "已终止",
            _ => "未知状态"
        };
    }

    /// <summary>
    /// 发送系统消息
    /// </summary>
    private async Task SendSystemMessage(List<NodeUser> recipients, string title, string content, NodeHandlerContext context)
    {
        // 创建消息记录（使用现有的 Message 实体）
        foreach (var recipient in recipients)
        {
            try
            {
                var message = new EasyWeChatModels.Entitys.Message
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    Content = content,
                    Type = 2, // 通知类型
                    Level = 1, // 普通级别
                    SenderId = context.OperatorId ?? Guid.Empty,
                    SenderName = context.OperatorName ?? "系统",
                    Status = 1,
                    CreateTime = DateTime.Now
                };

                await context.Db.Insertable(message).ExecuteCommandAsync();

                // 创建消息用户关联
                var messageUser = new EasyWeChatModels.Entitys.MessageUser
                {
                    Id = Guid.NewGuid(),
                    MessageId = message.Id,
                    UserId = recipient.TargetId,
                    IsRead = false,
                    CreateTime = DateTime.Now
                };

                await context.Db.Insertable(messageUser).ExecuteCommandAsync();
            }
            catch (Exception)
            {
                // 记录错误但不阻塞流程
            }
        }
    }
}