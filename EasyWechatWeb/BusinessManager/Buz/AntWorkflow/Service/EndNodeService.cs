using EasyWeChatModels.Dto;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models;
using EasyWeChatModels.Models.AntWorkflow;
using EasyWeChatModels.Models.NodeConfigs;
using BusinessManager.Buz.AntWorkflow.IService;
using BusinessManager.Buz.IService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 结束节点处理器服务
/// </summary>
public class EndNodeService : INodeHandlerService
{
    /// <summary>业务审核点服务（Autofac 属性注入）</summary>
    public IBusinessAuditPointService _businessAuditPointService { get; set; } = null!;

    /// <summary>HTTP客户端工厂（Autofac 属性注入）</summary>
    public IHttpClientFactory _httpClientFactory { get; set; } = null!;

    /// <summary>日志记录器（Autofac 属性注入）</summary>
    public ILogger<EndNodeService> _logger { get; set; } = null!;

    public AntNodeType NodeType => AntNodeType.End;

    /// <inheritdoc/>
    public async Task HandleEnterAsync(NodeHandlerContext context)
    {
        // 结束节点进入时，完成整个流程
        context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
        context.Instance.Status = (int)InstanceStatus.Passed;
        context.Instance.FinishTime = DateTime.Now;

        await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
        await context.Db.Updateable(context.Instance).ExecuteCommandAsync();

        // 执行回调处理
        await ExecuteCallbackAsync(context);

        // 在流程结束后检查是否是子流程，若有父流程则唤醒
        await ResumeParentWorkflowIfNeeded(context);
    }

    /// <inheritdoc/>
    public async Task HandleCompleteAsync(NodeHandlerContext context)
    {
        // 结束节点无需额外处理
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetNextNodesAsync(NodeHandlerContext context)
    {
        // 结束节点没有下一节点
        return new List<string>();
    }

    #region 私有方法

    /// <summary>
    /// 执行回调处理
    /// </summary>
    /// <param name="context">节点处理上下文</param>
    private async Task ExecuteCallbackAsync(NodeHandlerContext context)
    {
        try
        {
            // 判断流程是通过还是驳回结束
            var isPassed = context.Instance.Status == (int)InstanceStatus.Passed;

            if (isPassed)
            {
                await ExecutePassCallbackAsync(context);
            }
            else
            {
                await ExecuteRejectCallbackAsync(context);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行回调处理失败，实例ID: {InstanceId}", context.Instance.Id);
        }
    }

    /// <summary>
    /// 执行审核通过回调
    /// </summary>
    /// <param name="context">节点处理上下文</param>
    private async Task ExecutePassCallbackAsync(NodeHandlerContext context)
    {
        try
        {
            // 获取审核点配置
            var auditPointCode = context.Instance.BusinessData;
            if (string.IsNullOrEmpty(auditPointCode))
            {
                _logger.LogWarning("无法获取审核点编码，实例ID: {InstanceId}", context.Instance.Id);
                return;
            }

            // 从 BusinessData JSON 中解析审核点编码
            var businessData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(context.Instance.BusinessData ?? "{}");
            var auditPointCodeValue = businessData?.TryGetValue("AuditPointCode", out var code) == true ? code?.ToString() : null;

            if (string.IsNullOrEmpty(auditPointCodeValue))
            {
                // 尝试从实例节点获取审核点编码
                auditPointCodeValue = await GetAuditPointCodeFromInstance(context);
            }

            if (string.IsNullOrEmpty(auditPointCodeValue))
            {
                _logger.LogWarning("无法获取审核点编码，使用默认更新逻辑，实例ID: {InstanceId}", context.Instance.Id);
                await ExecuteDefaultUpdateAsync(context, true);
                return;
            }

            var auditPoint = await _businessAuditPointService.GetByCodeAsync(auditPointCodeValue);
            if (auditPoint == null)
            {
                _logger.LogWarning("审核点配置不存在: {AuditPointCode}", auditPointCodeValue);
                await ExecuteDefaultUpdateAsync(context, true);
                return;
            }

            // 执行通过回调API
            if (!string.IsNullOrEmpty(auditPoint.PassCallbackApi))
            {
                var businessId = context.Instance.BusinessId;
                var callbackUrl = auditPoint.PassCallbackApi.Replace("{BusinessId}", businessId);

                await CallInternalApiAsync(callbackUrl, new { InstanceId = context.Instance.Id });
            }
            else
            {
                // 使用默认更新逻辑
                await ExecuteDefaultUpdateAsync(context, true, auditPoint);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行审核通过回调失败，实例ID: {InstanceId}", context.Instance.Id);
        }
    }

    /// <summary>
    /// 执行审核驳回回调
    /// </summary>
    /// <param name="context">节点处理上下文</param>
    private async Task ExecuteRejectCallbackAsync(NodeHandlerContext context)
    {
        try
        {
            // 获取驳回原因
            var rejectReason = await GetRejectReasonAsync(context);

            // 获取审核点编码
            var auditPointCodeValue = await GetAuditPointCodeFromInstance(context);

            if (string.IsNullOrEmpty(auditPointCodeValue))
            {
                _logger.LogWarning("无法获取审核点编码，使用默认更新逻辑，实例ID: {InstanceId}", context.Instance.Id);
                await ExecuteDefaultUpdateAsync(context, false, null, rejectReason);
                return;
            }

            var auditPoint = await _businessAuditPointService.GetByCodeAsync(auditPointCodeValue);
            if (auditPoint == null)
            {
                _logger.LogWarning("审核点配置不存在: {AuditPointCode}", auditPointCodeValue);
                await ExecuteDefaultUpdateAsync(context, false, null, rejectReason);
                return;
            }

            // 执行驳回回调API
            if (!string.IsNullOrEmpty(auditPoint.RejectCallbackApi))
            {
                var businessId = context.Instance.BusinessId;
                var callbackUrl = auditPoint.RejectCallbackApi.Replace("{BusinessId}", businessId);

                await CallInternalApiAsync(callbackUrl, new
                {
                    InstanceId = context.Instance.Id,
                    RejectReason = rejectReason
                });
            }
            else
            {
                // 使用默认更新逻辑
                await ExecuteDefaultUpdateAsync(context, false, auditPoint, rejectReason);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行审核驳回回调失败，实例ID: {InstanceId}", context.Instance.Id);
        }
    }

    /// <summary>
    /// 从实例获取审核点编码
    /// </summary>
    /// <param name="context">节点处理上下文</param>
    /// <returns>审核点编码</returns>
    private async Task<string?> GetAuditPointCodeFromInstance(NodeHandlerContext context)
    {
        // 从 BusinessData JSON 中解析审核点编码
        if (!string.IsNullOrEmpty(context.Instance.BusinessData))
        {
            try
            {
                var businessData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(context.Instance.BusinessData);
                if (businessData?.TryGetValue("AuditPointCode", out var code) == true)
                {
                    return code?.ToString();
                }
            }
            catch
            {
                // 解析失败，忽略
            }
        }

        // 从业务表中查询审核点编码
        if (!string.IsNullOrEmpty(context.Instance.BusinessId))
        {
            try
            {
                var businessId = Guid.Parse(context.Instance.BusinessId);
                var tableName = context.Instance.BusinessType;

                // 查询业务表获取审核点编码
                var entity = await context.Db.Queryable<Product>()
                    .Where(p => p.Id == businessId)
                    .FirstAsync();

                return entity?.AuditPointCode;
            }
            catch
            {
                // 解析失败，忽略
            }
        }

        return null;
    }

    /// <summary>
    /// 获取驳回原因
    /// </summary>
    /// <param name="context">节点处理上下文</param>
    /// <returns>驳回原因</returns>
    private async Task<string?> GetRejectReasonAsync(NodeHandlerContext context)
    {
        // 从审批节点记录中获取驳回原因
        var rejectNode = await context.Db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.InstanceId == context.Instance.Id)
            .Where(n => n.ApproveStatus == (int)NodeApproveStatus.Rejected)
            .OrderByDescending(n => n.CreateTime)
            .FirstAsync();

        return rejectNode?.ApproveDesc;
    }

    /// <summary>
    /// 调用内部API
    /// </summary>
    /// <param name="url">API URL</param>
    /// <param name="data">请求数据</param>
    private async Task CallInternalApiAsync(string url, object data)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("InternalApi");
            var json = System.Text.Json.JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("调用内部API失败: {Url}, 状态码: {StatusCode}", url, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "调用内部API异常: {Url}", url);
        }
    }

    /// <summary>
    /// 执行默认更新逻辑（动态表更新）
    /// </summary>
    /// <param name="context">节点处理上下文</param>
    /// <param name="isPassed">是否通过</param>
    /// <param name="auditPoint">审核点配置</param>
    /// <param name="rejectReason">驳回原因</param>
    private async Task ExecuteDefaultUpdateAsync(NodeHandlerContext context, bool isPassed,
        BusinessAuditPointDto? auditPoint = null, string? rejectReason = null)
    {
        try
        {
            if (string.IsNullOrEmpty(context.Instance.BusinessId))
            {
                _logger.LogWarning("业务ID为空，无法执行更新，实例ID: {InstanceId}", context.Instance.Id);
                return;
            }

            var tableName = context.Instance.BusinessType; // 动态表名
            var businessId = Guid.Parse(context.Instance.BusinessId);

            var passStatus = auditPoint?.PassStatusValue ?? 3;
            var rejectStatus = auditPoint?.RejectStatusValue ?? 2;

            // 动态 SQL 更新（使用 SqlSugar.Ado）
            var sql = isPassed
                ? $"UPDATE {tableName} SET AuditStatus = @PassStatus, Status = 1, AuditTime = NOW(), UpdateTime = NOW() WHERE Id = @BusinessId"
                : $"UPDATE {tableName} SET AuditStatus = @RejectStatus, AuditRemark = @RejectReason, UpdateTime = NOW() WHERE Id = @BusinessId";

            var parameters = new List<SugarParameter>
            {
                new("@PassStatus", passStatus),
                new("@RejectStatus", rejectStatus),
                new("@RejectReason", rejectReason ?? ""),
                new("@BusinessId", businessId)
            };

            await context.Db.Ado.ExecuteCommandAsync(sql, parameters);

            _logger.LogInformation("动态表更新完成: Table={Table}, BusinessId={Id}, IsPassed={Passed}",
                tableName, businessId, isPassed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行默认更新逻辑失败，实例ID: {InstanceId}", context.Instance.Id);
        }
    }

    /// <summary>
    /// 如果当前是子流程，唤醒父流程继续
    /// </summary>
    /// <param name="context">节点处理上下文</param>
    private async Task ResumeParentWorkflowIfNeeded(NodeHandlerContext context)
    {
        if (!context.Instance.ParentInstanceId.HasValue) return;

        _logger.LogInformation("子流程完成，唤醒父流程: ParentInstanceId={ParentId}",
            context.Instance.ParentInstanceId.Value);

        // 获取父流程实例
        var parentInstance = await context.Db.Queryable<AntWorkflowInstance>()
            .Where(i => i.Id == context.Instance.ParentInstanceId.Value)
            .FirstAsync();

        if (parentInstance == null) return;

        var parentNodeId = context.Instance.ParentNodeId;
        if (string.IsNullOrEmpty(parentNodeId)) return;

        // 获取父流程的子流程节点
        var parentInstanceNode = await context.Db.Queryable<AntWorkflowInstanceNode>()
            .Where(n => n.InstanceId == parentInstance.Id && n.NodeId == parentNodeId)
            .FirstAsync();

        if (parentInstanceNode == null) return;

        // 删除父流程的子流程等待任务
        await context.Db.Deleteable<AntWorkflowCurrentTask>()
            .Where(t => t.InstanceId == parentInstance.Id && t.NodeId == parentNodeId)
            .ExecuteCommandAsync();

        // 更新父节点状态为已完成
        parentInstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
        await context.Db.Updateable(parentInstanceNode).ExecuteCommandAsync();

        // 应用输出参数映射（如果有）
        ApplyOutputMappings(context, parentInstance, parentNodeId);

        await context.Db.Updateable(parentInstance).ExecuteCommandAsync();

        _logger.LogInformation("父流程子流程节点完成，继续推进");
    }

    /// <summary>
    /// 应用输出参数映射
    /// </summary>
    /// <param name="context">节点处理上下文</param>
    /// <param name="parentInstance">父流程实例</param>
    /// <param name="parentNodeId">父节点ID</param>
    private void ApplyOutputMappings(NodeHandlerContext context, AntWorkflowInstance parentInstance, string parentNodeId)
    {
        try
        {
            var dagConfig = JsonConvert.DeserializeObject<DagConfig>(parentInstance.FlowConfig ?? "");
            var parentNode = dagConfig?.Nodes.FirstOrDefault(n => n.Id == parentNodeId);
            var subflowConfig = JsonConvert.DeserializeObject<SubflowNodeConfig>(parentNode?.Config?.ToString() ?? "");

            if (subflowConfig?.OutputMappings == null || subflowConfig.OutputMappings.Count == 0) return;

            var subflowFormData = JObject.Parse(context.Instance.FormData ?? "{}");
            var parentFormData = JObject.Parse(parentInstance.FormData ?? "{}");

            foreach (var mapping in subflowConfig.OutputMappings)
            {
                if (subflowFormData.TryGetValue(mapping.SourceField ?? "", out var value))
                {
                    parentFormData[mapping.TargetField ?? mapping.SourceField ?? ""] = value;
                }
            }

            parentInstance.FormData = parentFormData.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "输出参数映射失败");
        }
    }

    #endregion
}