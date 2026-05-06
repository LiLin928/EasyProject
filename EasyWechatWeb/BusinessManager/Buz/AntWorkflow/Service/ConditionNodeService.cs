using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EasyWeChatModels.Enums;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models.NodeConfigs;
using EasyWeChatModels.Models.AntWorkflow;
using BusinessManager.Buz.IService;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 条件分支节点处理器服务
/// </summary>
public class ConditionNodeService : INodeHandlerService
{
    public AntNodeType NodeType => AntNodeType.Condition;

    /// <inheritdoc/>
    public async Task HandleEnterAsync(NodeHandlerContext context)
    {
        // 解析条件节点配置
        var config = JsonConvert.DeserializeObject<ConditionNodeConfig>(context.DagNode.Config?.ToString() ?? "");
        if (config == null || config.ConditionNodes == null || config.ConditionNodes.Count == 0)
        {
            // 没有条件配置，自动完成
            context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
            await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
            return;
        }

        // 条件分支节点自动完成，不阻塞流程
        context.InstanceNode.ApproveStatus = (int)NodeApproveStatus.Completed;
        await context.Db.Updateable(context.InstanceNode).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    public async Task HandleCompleteAsync(NodeHandlerContext context)
    {
        // 条件分支无需额外处理
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetNextNodesAsync(NodeHandlerContext context)
    {
        // 解析条件节点配置
        var config = JsonConvert.DeserializeObject<ConditionNodeConfig>(context.DagNode.Config?.ToString() ?? "");
        if (config == null || config.ConditionNodes == null || config.ConditionNodes.Count == 0)
        {
            // 没有条件配置，返回所有可能的下一节点
            return GetAllNextNodes(context);
        }

        // 解析业务数据
        var businessData = ParseBusinessData(context.BusinessData);

        // 按优先级排序，找到第一个满足条件的分支
        var sortedBranches = config.ConditionNodes
            .OrderByDescending(b => b.Priority)
            .ToList();

        string? matchedBranchId = null;

        foreach (var branch in sortedBranches)
        {
            if (branch.IsDefault)
            {
                // 默认分支作为备选
                matchedBranchId = branch.Id;
                continue;
            }

            if (EvaluateBranch(branch, businessData))
            {
                matchedBranchId = branch.Id;
                break;
            }
        }

        // 如果没有匹配的分支，使用默认分支
        if (matchedBranchId == null)
        {
            var defaultBranch = sortedBranches.FirstOrDefault(b => b.IsDefault);
            matchedBranchId = defaultBranch?.Id;
        }

        // 根据匹配的分支ID找到对应的边
        var edges = context.DagConfig.Edges
            .Where(e => e.SourceNodeId == context.DagNode.Id)
            .ToList();

        // 查找匹配分支的下一节点
        // 边的 SourcePort 或 Condition 中包含分支ID信息
        var matchedEdges = edges.Where(e =>
        {
            // 检查边的条件配置
            if (e.Condition != null && e.Condition.BranchId == matchedBranchId)
            {
                return true;
            }
            // 检查边的 SourcePort（可能包含分支ID）
            if (e.SourcePort == matchedBranchId)
            {
                return true;
            }
            return false;
        }).ToList();

        if (matchedEdges.Count > 0)
        {
            return matchedEdges.Select(e => e.TargetNodeId).ToList();
        }

        // 如果没有匹配的边，返回所有下一节点（兜底）
        return GetAllNextNodes(context);
    }

    /// <summary>
    /// 获取所有可能的下一节点
    /// </summary>
    private List<string> GetAllNextNodes(NodeHandlerContext context)
    {
        return context.DagConfig.Edges
            .Where(e => e.SourceNodeId == context.DagNode.Id)
            .Select(e => e.TargetNodeId)
            .ToList();
    }

    /// <summary>
    /// 解析业务数据JSON
    /// </summary>
    private Dictionary<string, object?> ParseBusinessData(string? businessData)
    {
        if (string.IsNullOrEmpty(businessData))
        {
            return new Dictionary<string, object?>();
        }

        try
        {
            var jObject = JsonConvert.DeserializeObject<JObject>(businessData);
            if (jObject == null)
            {
                return new Dictionary<string, object?>();
            }

            var result = new Dictionary<string, object?>();
            foreach (var prop in jObject.Properties())
            {
                result[prop.Name] = prop.Value?.ToObject<object?>();
            }
            return result;
        }
        catch
        {
            return new Dictionary<string, object?>();
        }
    }

    /// <summary>
    /// 评估分支条件是否满足
    /// </summary>
    private bool EvaluateBranch(ConditionBranch branch, Dictionary<string, object?> businessData)
    {
        if (branch.ConditionRules == null || branch.ConditionRules.Count == 0)
        {
            return false;
        }

        // 所有规则都需要满足（AND逻辑）
        foreach (var rule in branch.ConditionRules)
        {
            if (!EvaluateRule(rule, businessData))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 评估单个条件规则
    /// </summary>
    private bool EvaluateRule(ConditionRule rule, Dictionary<string, object?> businessData)
    {
        var fieldValue = businessData.GetValueOrDefault(rule.FieldId);

        switch (rule.Operator)
        {
            case ConditionOperator.Eq:
                return EqualsValue(fieldValue, rule.Value);

            case ConditionOperator.Ne:
                return !EqualsValue(fieldValue, rule.Value);

            case ConditionOperator.Gt:
                return CompareNumeric(fieldValue, rule.Value) > 0;

            case ConditionOperator.Gte:
                return CompareNumeric(fieldValue, rule.Value) >= 0;

            case ConditionOperator.Lt:
                return CompareNumeric(fieldValue, rule.Value) < 0;

            case ConditionOperator.Lte:
                return CompareNumeric(fieldValue, rule.Value) <= 0;

            case ConditionOperator.Contains:
                return ContainsValue(fieldValue, rule.Value);

            case ConditionOperator.NotContains:
                return !ContainsValue(fieldValue, rule.Value);

            case ConditionOperator.Empty:
                return fieldValue == null || IsEmptyValue(fieldValue);

            case ConditionOperator.NotEmpty:
                return fieldValue != null && !IsEmptyValue(fieldValue);

            case ConditionOperator.In:
                return InList(fieldValue, rule.Value);

            case ConditionOperator.NotIn:
                return !InList(fieldValue, rule.Value);

            default:
                return false;
        }
    }

    /// <summary>
    /// 判断值相等
    /// </summary>
    private bool EqualsValue(object? fieldValue, object? ruleValue)
    {
        if (fieldValue == null && ruleValue == null) return true;
        if (fieldValue == null || ruleValue == null) return false;

        // 转换为字符串比较（兼容不同类型）
        return fieldValue.ToString() == ruleValue.ToString();
    }

    /// <summary>
    /// 数值比较
    /// </summary>
    private int CompareNumeric(object? fieldValue, object? ruleValue)
    {
        if (fieldValue == null || ruleValue == null) return 0;

        try
        {
            var fieldNum = Convert.ToDecimal(fieldValue);
            var ruleNum = Convert.ToDecimal(ruleValue);
            return fieldNum.CompareTo(ruleNum);
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// 包含判断
    /// </summary>
    private bool ContainsValue(object? fieldValue, object? ruleValue)
    {
        if (fieldValue == null || ruleValue == null) return false;

        var fieldStr = fieldValue.ToString();
        var ruleStr = ruleValue.ToString();

        return fieldStr?.Contains(ruleStr ?? "") ?? false;
    }

    /// <summary>
    /// 判断是否为空值
    /// </summary>
    private bool IsEmptyValue(object? value)
    {
        if (value == null) return true;

        var strValue = value.ToString();
        return string.IsNullOrEmpty(strValue) || strValue == "null" || strValue == "[]";
    }

    /// <summary>
    /// 判断是否在列表中
    /// </summary>
    private bool InList(object? fieldValue, object? ruleValue)
    {
        if (fieldValue == null || ruleValue == null) return false;

        try
        {
            // ruleValue 应该是一个数组/列表
            var list = JsonConvert.DeserializeObject<List<object>>(JsonConvert.SerializeObject(ruleValue));
            if (list == null) return false;

            var fieldStr = fieldValue.ToString();
            return list.Any(item => item.ToString() == fieldStr);
        }
        catch
        {
            return false;
        }
    }
}