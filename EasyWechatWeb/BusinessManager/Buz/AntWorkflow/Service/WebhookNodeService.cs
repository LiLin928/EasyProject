using Newtonsoft.Json;
using SqlSugar;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.IService;

namespace BusinessManager.Buz.Service;

/// <summary>
/// Webhook节点处理器服务
/// </summary>
/// <remarks>
/// Webhook节点用于接收外部系统的回调通知，触发流程继续执行
/// 支持三种触发模式：before（节点进入前执行）、after（节点退出后执行）、manual（等待手动触发）
/// </remarks>
public class WebhookNodeService : INodeHandlerService
{
    public AntNodeType NodeType => AntNodeType.Webhook;

    /// <inheritdoc/>
    public async Task HandleEnterAsync(NodeHandlerContext context)
    {
        // 解析 Webhook 配置
        var config = JsonConvert.DeserializeObject<WebhookNodeConfig>(context.DagNode.Config?.ToString() ?? "");
        if (config == null || string.IsNullOrEmpty(config.Url))
        {
            // 没有配置，自动完成
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            return;
        }

        switch (config.Trigger?.ToLower())
        {
            case "before":
                // 节点进入前立即执行Webhook，然后自动完成节点
                await ExecuteWebhookAsync(config, context);
                context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
                await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
                break;

            case "after":
                // 节点保持处理中状态，等待流程完成后再执行Webhook
                await CreateWebhookWaitingTask(config, context);
                context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Processing;
                await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
                break;

            case "manual":
                // 等待手动触发Webhook回调
                await CreateWebhookWaitingTask(config, context);
                context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Processing;
                await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
                break;

            default:
                // 默认为after模式
                await CreateWebhookWaitingTask(config, context);
                context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Processing;
                await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
                break;
        }
    }

    /// <summary>
    /// 执行Webhook调用
    /// </summary>
    private async Task ExecuteWebhookAsync(WebhookNodeConfig config, NodeHandlerContext context)
    {
        var retryCount = config.RetryConfig?.Count ?? 0;
        var retryInterval = config.RetryConfig?.Interval ?? 1000;
        var lastError = string.Empty;

        for (int attempt = 0; attempt <= retryCount; attempt++)
        {
            try
            {
                using var httpClient = new HttpClient();
                httpClient.Timeout = TimeSpan.FromMilliseconds(config.Timeout);

                // 添加认证头
                ApplyAuthHeaders(httpClient, config);

                // 构建请求
                var request = BuildRequest(config, context);
                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    await LogWebhookExecution(context, config, true, responseBody, attempt);
                    return;
                }

                lastError = $"HTTP {response.StatusCode}: {await response.Content.ReadAsStringAsync()}";
                if (attempt < retryCount)
                {
                    await Task.Delay(retryInterval);
                }
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                if (attempt < retryCount)
                {
                    await Task.Delay(retryInterval);
                }
            }
        }

        // 所有重试都失败，记录日志
        await LogWebhookExecution(context, config, false, lastError, retryCount);
    }

    /// <summary>
    /// 应用认证配置到HttpClient
    /// </summary>
    private void ApplyAuthHeaders(HttpClient httpClient, WebhookNodeConfig config)
    {
        if (config.AuthConfig == null || config.AuthConfig.Type == "none")
        {
            return;
        }

        switch (config.AuthConfig.Type.ToLower())
        {
            case "basic":
                if (!string.IsNullOrEmpty(config.AuthConfig.Username) && !string.IsNullOrEmpty(config.AuthConfig.Password))
                {
                    var authValue = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{config.AuthConfig.Username}:{config.AuthConfig.Password}"));
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {authValue}");
                }
                break;

            case "bearer":
                if (!string.IsNullOrEmpty(config.AuthConfig.Token))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.AuthConfig.Token}");
                }
                break;

            case "api_key":
                if (!string.IsNullOrEmpty(config.AuthConfig.KeyName) && !string.IsNullOrEmpty(config.AuthConfig.KeyValue))
                {
                    if (config.AuthConfig.AddTo == "header")
                    {
                        httpClient.DefaultRequestHeaders.Add(config.AuthConfig.KeyName, config.AuthConfig.KeyValue);
                    }
                    // query 模式在 BuildRequest 中处理
                }
                break;
        }
    }

    /// <summary>
    /// 构建HTTP请求
    /// </summary>
    private HttpRequestMessage BuildRequest(WebhookNodeConfig config, NodeHandlerContext context)
    {
        var url = config.Url;

        // 处理API Key认证的query模式
        if (config.AuthConfig?.Type == "api_key" && config.AuthConfig.AddTo == "query")
        {
            var separator = url.Contains("?") ? "&" : "?";
            url = $"{url}{separator}{config.AuthConfig.KeyName}={Uri.EscapeDataString(config.AuthConfig.KeyValue ?? "")}";
        }

        var request = new HttpRequestMessage(new System.Net.Http.HttpMethod(config.Method), url);

        // 添加自定义请求头
        if (config.Headers != null)
        {
            foreach (var header in config.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        // 添加请求体
        if (!string.IsNullOrEmpty(config.Body) && config.Method != "GET")
        {
            request.Content = new System.Net.Http.StringContent(config.Body, System.Text.Encoding.UTF8, "application/json");
        }

        return request;
    }

    /// <summary>
    /// 记录Webhook执行日志
    /// </summary>
    private async Task LogWebhookExecution(NodeHandlerContext context, WebhookNodeConfig config, bool success, string response, int attemptCount)
    {
        var serviceLog = new AntWorkflowServiceLog
        {
            Id = Guid.NewGuid(),
            InstanceId = context.Instance.Id,
            NodeId = context.DagNode.Id,
            NodeName = context.DagNode.Name,
            TaskType = "webhook",
            ExecuteStatus = success ? 1 : 2,
            ExecuteTime = DateTime.Now,
            RequestData = JsonConvert.SerializeObject(new
            {
                url = config.Url,
                method = config.Method,
                trigger = config.Trigger,
                attempt = attemptCount
            }),
            ResponseData = response
        };

        await context.Db.Insertable(serviceLog).ExecuteCommandAsync();
    }

    /// <summary>
    /// 创建Webhook等待任务
    /// </summary>
    private async Task CreateWebhookWaitingTask(WebhookNodeConfig config, NodeHandlerContext context)
    {
        var webhookTask = new AntWorkflowCurrentTask
        {
            Id = Guid.NewGuid(),
            InstanceId = context.Instance.Id,
            InstanceNodeId = context.InstanceNode.Id,
            NodeId = context.DagNode.Id,
            NodeType = (int)AntNodeType.Webhook,
            HandlerId = Guid.Empty,
            HandlerType = 0,
            EntryTime = DateTime.Now,
            TaskType = 3,
            ActiveStatus = 1
        };

        await context.Db.Insertable(webhookTask).ExecuteCommandAsync();

        // 记录Webhook URL（供外部系统调用）
        var serviceLog = new AntWorkflowServiceLog
        {
            Id = Guid.NewGuid(),
            InstanceId = context.Instance.Id,
            NodeId = context.DagNode.Id,
            NodeName = context.DagNode.Name,
            TaskType = "webhook",
            ExecuteStatus = 0,
            RequestData = $"Webhook URL: /api/ant-workflow/webhook/callback/{webhookTask.Id}"
        };

        await context.Db.Insertable(serviceLog).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task HandleCompleteAsync(NodeHandlerContext context)
    {
        // 解析配置判断触发模式
        var config = JsonConvert.DeserializeObject<WebhookNodeConfig>(context.DagNode.Config?.ToString() ?? "");

        // after 触发模式：节点完成时执行 Webhook
        if (config != null && config.Trigger?.ToLower() == "after")
        {
            await ExecuteWebhookAsync(config, context);
        }

        // 删除 Webhook 待办任务
        var task = await context.Db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.InstanceId == context.Instance.Id && t.NodeId == context.DagNode.Id && t.ActiveStatus == 1)
            .FirstAsync();

        if (task != null)
        {
            await context.Db.Deleteable(task).ExecuteCommandAsync();
        }

        // 更新节点状态为已完成
        context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
        await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetNextNodesAsync(NodeHandlerContext context)
    {
        // 只有节点完成时才推进
        if (context.InstanceNode.ApproveStatus != (int)NodeApproveStatus.Completed)
        {
            return new List<string>();
        }

        var edges = context.DagConfig.Edges
            .Where(e => e.SourceNodeId == context.DagNode.Id)
            .ToList();

        return edges.Select(e => e.TargetNodeId).ToList();
    }

    /// <summary>
    /// 处理 Webhook 回调（供外部调用）
    /// </summary>
    public async Task<bool> ProcessWebhookCallbackAsync(Guid taskId, string callbackData, ISqlSugarClient db)
    {
        var task = await db.Queryable<AntWorkflowCurrentTask>()
            .Where(t => t.Id == taskId && t.TaskType == 3 && t.ActiveStatus == 1)
            .FirstAsync();

        if (task == null) return false;

        // 更新执行日志
        var serviceLog = await db.Queryable<AntWorkflowServiceLog>()
            .Where(l => l.InstanceId == task.InstanceId && l.NodeId == task.NodeId && l.TaskType == "webhook")
            .OrderByDescending(l => l.Id)
            .FirstAsync();

        if (serviceLog != null)
        {
            serviceLog.ExecuteStatus = 1; // 成功
            serviceLog.ExecuteTime = DateTime.Now;
            serviceLog.ResponseData = callbackData;
            await db.Updateable(serviceLog).ExecuteCommandAsync();
        }

        // 标记任务完成
        task.ActiveStatus = 0;
        await db.Updateable(task).ExecuteCommandAsync();

        // 更新节点状态
        var instanceNode = await db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.Id == task.InstanceNodeId)
            .FirstAsync();

        if (instanceNode != null)
        {
            instanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await db.Updateable(instanceNode).ExecuteCommandAsync();
        }

        return true;
    }
}