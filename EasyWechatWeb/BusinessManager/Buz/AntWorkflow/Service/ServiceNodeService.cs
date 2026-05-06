using Jint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.IService;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 服务任务节点处理器服务
/// </summary>
public class ServiceNodeService : INodeHandlerService
{
    public AntNodeType NodeType => AntNodeType.Service;

    /// <inheritdoc/>
    public async Task HandleEnterAsync(NodeHandlerContext context)
    {
        // 解析服务任务配置
        var config = JsonConvert.DeserializeObject<ServiceNodeConfig>(context.DagNode.Config?.ToString() ?? "");
        if (config == null)
        {
            // 没有配置，自动完成
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            return;
        }

        // 执行服务任务
        var log = new AntWorkflowServiceLog
        {
            Id = Guid.NewGuid(),
            InstanceId = context.Instance.Id,
            NodeId = context.DagNode.Id,
            NodeName = context.DagNode.Name,
            TaskType = config.TaskType,
            ExecuteStatus = 0, // 待执行
            ExecutorId = context.OperatorId,
            ExecutorName = context.OperatorName
        };

        try
        {
            var result = await ExecuteServiceTask(config, context);

            log.ExecuteStatus = result.Success ? 1 : 2; // 成功/失败
            log.ExecuteTime = DateTime.Now;
            log.RequestData = result.RequestData;
            log.ResponseData = result.ResponseData;
            log.ErrorMessage = result.ErrorMessage;

            // 根据错误处理策略决定是否继续
            var errorHandling = config.ErrorHandling ?? new ErrorHandlingConfig();
            if (!result.Success)
            {
                if (errorHandling.Strategy?.ToLower() == "stop")
                {
                    // 停止流程
                    context.Instance.Status = (int)InstanceStatus.Terminated;
                    context.Instance.FinishTime = DateTime.Now;
                    context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
                    await context.Db.Updateable(context.Instance).ExecuteCommandAsync();
                    await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
                }
                else if (errorHandling.Strategy?.ToLower() == "continue")
                {
                    // 续流程
                    context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
                    await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
                }
                else if (errorHandling.Strategy?.ToLower() == "retry" && errorHandling.RetryCount > 0)
                {
                    // 重试逻辑（简化实现）
                    for (int i = 0; i < errorHandling.RetryCount && !result.Success; i++)
                    {
                        await Task.Delay(errorHandling.RetryInterval * 1000);
                        result = await ExecuteServiceTask(config, context);
                        log.RetryCount = i + 1;
                    }

                    log.ExecuteStatus = result.Success ? 1 : 2;
                    log.ResponseData = result.ResponseData;
                    log.ErrorMessage = result.ErrorMessage;

                    context.InstanceNode.ApproveStatus = result.Success
                        ? (int)NodeApproveStatus.Completed
                        : (int)NodeApproveStatus.Completed; // 即使失败也标记完成（继续策略）
                    await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
                }
            }
            else
            {
                // 成功，标记节点完成
                context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
                await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            }
        }
        catch (Exception ex)
        {
            log.ExecuteStatus = 2; // 失败
            log.ExecuteTime = DateTime.Now;
            log.ErrorMessage = ex.Message;

            // 默认继续流程
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
        }

        // 记录执行日志
        await context.Db.Insertable(log).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task HandleCompleteAsync(NodeHandlerContext context)
    {
        // 服务任务无需额外处理
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
    /// 执行服务任务
    /// </summary>
    private async Task<ServiceTaskResult> ExecuteServiceTask(ServiceNodeConfig config, NodeHandlerContext context)
    {
        var result = new ServiceTaskResult();

        try
        {
            switch (config.TaskType?.ToLower())
            {
                case "api":
                    result = await ExecuteApiCall(config.ApiConfig!, context);
                    break;

                case "script":
                    result = ExecuteScript(config.ScriptConfig!, context);
                    break;

                case "webhook":
                    result = await ExecuteApiCall(config.ApiConfig!, context);
                    break;

                default:
                    result.ErrorMessage = "未知的任务类型";
                    result.Success = false;
                    break;
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 执行 API 调用
    /// </summary>
    private async Task<ServiceTaskResult> ExecuteApiCall(ApiConfig apiConfig, NodeHandlerContext context)
    {
        var result = new ServiceTaskResult();

        if (apiConfig == null || string.IsNullOrEmpty(apiConfig.Url))
        {
            result.Success = false;
            result.ErrorMessage = "API配置无效";
            return result;
        }

        try
        {
            // 构建请求体（支持字段映射）
            var requestBody = BuildRequestBody(apiConfig, context);
            result.RequestData = requestBody;

            // 创建 HTTP 客户端
            using var httpClient = new HttpClient();

            // 设置请求头
            if (apiConfig.Headers != null)
            {
                foreach (var header in apiConfig.Headers)
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            // 发送请求
            HttpResponseMessage response;
            var method = apiConfig.Method?.ToUpper() ?? "POST";

            if (method == "GET")
            {
                response = await httpClient.GetAsync(apiConfig.Url);
            }
            else if (method == "POST")
            {
                var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
                response = await httpClient.PostAsync(apiConfig.Url, content);
            }
            else if (method == "PUT")
            {
                var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
                response = await httpClient.PutAsync(apiConfig.Url, content);
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = $"不支持的HTTP方法: {method}";
                return result;
            }

            result.ResponseData = await response.Content.ReadAsStringAsync();
            result.Success = response.IsSuccessStatusCode;

            if (!response.IsSuccessStatusCode)
            {
                result.ErrorMessage = $"HTTP错误: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 构建请求体
    /// </summary>
    private string BuildRequestBody(ApiConfig apiConfig, NodeHandlerContext context)
    {
        // 如果有自定义请求体模板，使用模板
        if (!string.IsNullOrEmpty(apiConfig.Body))
        {
            // 替换模板中的变量
            var body = apiConfig.Body;

            // 替换业务数据变量
            if (!string.IsNullOrEmpty(context.BusinessData))
            {
                var businessData = JObject.Parse(context.BusinessData);
                foreach (var prop in businessData.Properties())
                {
                    body = body.Replace($"{{{{{prop.Name}}}}}", prop.Value?.ToString() ?? "");
                }
            }

            // 替换表单数据变量
            if (!string.IsNullOrEmpty(context.FormData))
            {
                var formData = JObject.Parse(context.FormData);
                foreach (var prop in formData.Properties())
                {
                    body = body.Replace($"{{{{{prop.Name}}}}}", prop.Value?.ToString() ?? "");
                }
            }

            // 替换实例信息变量
            body = body.Replace("{{InstanceId}}", context.Instance.Id.ToString());
            body = body.Replace("{{WorkflowId}}", context.Instance.WorkflowId.ToString());
            body = body.Replace("{{InitiatorId}}", context.Instance.InitiatorId?.ToString() ?? "");
            body = body.Replace("{{Title}}", context.Instance.Title ?? "");

            return body;
        }

        // 否则使用字段映射构建
        if (apiConfig.FieldMappings != null && apiConfig.FieldMappings.Count > 0)
        {
            var requestObj = new JObject();

            // 解析业务数据和表单数据
            var businessData = string.IsNullOrEmpty(context.BusinessData)
                ? new JObject()
                : JObject.Parse(context.BusinessData);
            var formData = string.IsNullOrEmpty(context.FormData)
                ? new JObject()
                : JObject.Parse(context.FormData);

            foreach (var mapping in apiConfig.FieldMappings)
            {
                var sourceValue = GetFieldValue(mapping.SourceField!, businessData, formData);
                requestObj[mapping.TargetField!] = JToken.FromObject(sourceValue ?? new object());
            }

            return requestObj.ToString();
        }

        // 默认发送业务数据
        return context.BusinessData ?? "{}";
    }

    /// <summary>
    /// 获取字段值
    /// </summary>
    private object? GetFieldValue(string fieldId, JObject businessData, JObject formData)
    {
        if (businessData.TryGetValue(fieldId, out var value))
        {
            return value?.ToObject<object?>();
        }

        if (formData.TryGetValue(fieldId, out var formValue))
        {
            return formValue?.ToObject<object?>();
        }

        return null;
    }

    /// <summary>
    /// 执行脚本（使用 Jint 执行 JavaScript）
    /// </summary>
    private ServiceTaskResult ExecuteScript(ScriptConfig scriptConfig, NodeHandlerContext context)
    {
        var result = new ServiceTaskResult();

        if (scriptConfig == null || string.IsNullOrEmpty(scriptConfig.Script))
        {
            result.Success = false;
            result.ErrorMessage = "脚本配置无效";
            return result;
        }

        if (scriptConfig.Format?.ToLower() != "javascript")
        {
            result.Success = false;
            result.ErrorMessage = $"暂不支持 {scriptConfig.Format} 格式";
            return result;
        }

        try
        {
            var engine = new Engine(options =>
            {
                options.TimeoutInterval(TimeSpan.FromSeconds(30));
                options.LimitMemory(4_000_000); // 4MB 内存限制
            })
                .SetValue("businessData", JObject.Parse(context.BusinessData ?? "{}"))
                .SetValue("formData", JObject.Parse(context.FormData ?? "{}"))
                .SetValue("instanceId", context.Instance.Id.ToString())
                .SetValue("workflowId", context.Instance.WorkflowId.ToString())
                .SetValue("initiatorId", context.Instance.InitiatorId?.ToString() ?? "");

            engine.Execute(scriptConfig.Script);

            var scriptResult = engine.GetValue("result")?.ToObject();
            var resultVariable = scriptConfig.ResultVariable ?? "result";

            // 存储结果到 FormData
            StoreResultVariable(context, resultVariable, scriptResult);

            result.RequestData = scriptConfig.Script;
            result.ResponseData = JsonConvert.SerializeObject(scriptResult);
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// 存储脚本执行结果到 FormData
    /// </summary>
    private void StoreResultVariable(NodeHandlerContext context, string variableName, object? value)
    {
        if (string.IsNullOrEmpty(variableName) || value == null) return;

        var formData = JObject.Parse(context.FormData ?? "{}");
        formData[variableName] = JToken.FromObject(value);
        context.FormData = formData.ToString();
        context.Instance.FormData = context.FormData;
    }

    /// <summary>
    /// 服务任务执行结果
    /// </summary>
    private class ServiceTaskResult
    {
        public bool Success { get; set; }
        public string? RequestData { get; set; }
        public string? ResponseData { get; set; }
        public string? ErrorMessage { get; set; }
    }
}