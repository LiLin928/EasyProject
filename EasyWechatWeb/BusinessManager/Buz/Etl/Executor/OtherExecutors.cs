using BusinessManager.Buz.Etl.Engine;
using EasyWeChatModels.Models.Etl;
using EasyWeChatModels.Models.Etl.NodeConfigs;

namespace BusinessManager.Buz.Etl.Executor;

/// <summary>
/// 脚本节点执行器
/// 执行脚本代码
/// </summary>
public class ScriptExecutor : BaseExecutor
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public override string NodeType => "script";

    /// <summary>
    /// 执行脚本
    /// </summary>
    public override async Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node)
    {
        try
        {
            var config = ParseConfig<ScriptNodeConfig>(node.Config);

            // 目前返回原数据（脚本执行需要 Jint 库支持）
            // TODO: 集成 Jint 库实现真正的 JavaScript 执行

            var outputs = new Dictionary<string, object>
            {
                [config.OutputVariable] = new { message = "脚本执行器待实现", scriptLanguage = config.ScriptLanguage },
                ["_executeTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await Task.CompletedTask;
            return CreateSuccessResult(outputs);
        }
        catch (Exception ex)
        {
            return CreateFailResult($"执行脚本失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 验证配置
    /// </summary>
    public override string? ValidateConfig(DagNode node)
    {
        var config = ParseConfig<ScriptNodeConfig>(node.Config);

        if (string.IsNullOrEmpty(config.Script) && string.IsNullOrEmpty(config.ScriptPath))
            return "脚本内容或脚本路径不能为空";

        if (string.IsNullOrEmpty(config.OutputVariable))
            return "输出变量名不能为空";

        return null;
    }
}

/// <summary>
/// 条件节点执行器
/// 根据条件判断执行分支
/// </summary>
public class ConditionExecutor : BaseExecutor
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public override string NodeType => "condition";

    /// <summary>
    /// 执行条件判断
    /// </summary>
    public override async Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node)
    {
        try
        {
            var config = ParseConfig<ConditionNodeConfig>(node.Config);

            // 获取上游变量
            var upstreamVars = GetUpstreamVariables(context, node.Id);

            // 获取输入数据
            var inputData = config.InputVariable != null
                ? GetInputData(context, node.Id, config.InputVariable)
                : null;

            // 评估条件
            var branchLabel = EvaluateCondition(config, upstreamVars, inputData);

            var outputs = new Dictionary<string, object>
            {
                ["_branch"] = branchLabel,
                ["_conditionResult"] = true,
                ["_executeTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await Task.CompletedTask;
            return CreateSuccessResult(outputs);
        }
        catch (Exception ex)
        {
            return CreateFailResult($"执行条件判断失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 验证配置
    /// </summary>
    public override string? ValidateConfig(DagNode node)
    {
        var config = ParseConfig<ConditionNodeConfig>(node.Config);

        if (config.ConditionType == "simple" && (config.Rules == null || config.Rules.Count == 0))
            return "简单条件需要配置规则列表";

        if (config.ConditionType == "expression" && string.IsNullOrEmpty(config.Expression))
            return "表达式条件需要配置条件表达式";

        return null;
    }

    private string EvaluateCondition(ConditionNodeConfig config,
        Dictionary<string, object> upstreamVars,
        List<Dictionary<string, object>>? inputData)
    {
        if (config.ConditionType == "expression")
        {
            return EvaluateExpression(config.Expression!, upstreamVars);
        }

        if (config.Rules != null && config.Rules.Count > 0)
        {
            foreach (var rule in config.Rules)
            {
                if (EvaluateRule(rule, upstreamVars, inputData))
                {
                    return rule.BranchLabel;
                }
            }
        }

        return config.DefaultBranch;
    }

    private string EvaluateExpression(string expression, Dictionary<string, object> variables)
    {
        var result = expression;

        foreach (var (key, value) in variables)
        {
            var placeholder = $"${{{key}}}";
            if (result.Contains(placeholder))
            {
                var replacement = value is string ? $"'{value}'" : value?.ToString() ?? "null";
                result = result.Replace(placeholder, replacement);
            }
        }

        // 简单表达式评估（支持基本比较）
        return EvaluateSimpleExpression(result) ? "true" : "false";
    }

    private bool EvaluateRule(ConditionRule rule,
        Dictionary<string, object> upstreamVars,
        List<Dictionary<string, object>>? inputData)
    {
        object? fieldValue = null;

        // 从上游变量获取字段值
        if (upstreamVars.TryGetValue(rule.Field, out var value))
        {
            fieldValue = value;
        }

        // 从输入数据获取
        if (fieldValue == null && inputData != null && inputData.Count > 0)
        {
            inputData[0].TryGetValue(rule.Field, out fieldValue);
        }

        // 比较值
        return CompareValues(fieldValue, rule.Value, rule.Operator);
    }

    private bool CompareValues(object? left, object right, string operatorStr)
    {
        if (left == null)
            return operatorStr == "ne";

        var leftStr = left.ToString() ?? string.Empty;
        var rightStr = right.ToString() ?? string.Empty;

        return operatorStr.ToLower() switch
        {
            "eq" => leftStr == rightStr,
            "ne" => leftStr != rightStr,
            "gt" => double.TryParse(leftStr, out var l) && double.TryParse(rightStr, out var r) && l > r,
            "lt" => double.TryParse(leftStr, out var l) && double.TryParse(rightStr, out var r) && l < r,
            "gte" => double.TryParse(leftStr, out var l) && double.TryParse(rightStr, out var r) && l >= r,
            "lte" => double.TryParse(leftStr, out var l) && double.TryParse(rightStr, out var r) && l <= r,
            "contains" => leftStr.Contains(rightStr),
            "regex" => System.Text.RegularExpressions.Regex.IsMatch(leftStr, rightStr),
            _ => false
        };
    }

    private bool EvaluateSimpleExpression(string expression)
    {
        var operators = new[] { "==", "!=", ">=", "<=", ">", "<" };
        foreach (var op in operators)
        {
            if (expression.Contains(op))
            {
                var parts = expression.Split(new[] { op }, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    var left = parts[0].Trim().Trim('\'', '"');
                    var right = parts[1].Trim().Trim('\'', '"');
                    return CompareValues(left, right, op);
                }
            }
        }
        return true;
    }
}

/// <summary>
/// 并行节点执行器
/// 并行执行多个分支
/// </summary>
public class ParallelExecutor : BaseExecutor
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public override string NodeType => "parallel";

    /// <summary>
    /// 执行并行节点
    /// </summary>
    public override async Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node)
    {
        try
        {
            var config = ParseConfig<ParallelNodeConfig>(node.Config);

            // 并行节点主要是控制节点，由执行引擎处理并行组
            // 这里只记录并行模式

            var outputs = new Dictionary<string, object>
            {
                ["_waitMode"] = config.WaitMode,
                ["_executeTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await Task.CompletedTask;
            return CreateSuccessResult(outputs);
        }
        catch (Exception ex)
        {
            return CreateFailResult($"执行并行节点失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 验证配置
    /// </summary>
    public override string? ValidateConfig(DagNode node)
    {
        var config = ParseConfig<ParallelNodeConfig>(node.Config);

        if (config.WaitMode != "all" && config.WaitMode != "any")
            return "等待模式必须是 all 或 any";

        return null;
    }
}

/// <summary>
/// 通知节点执行器
/// 发送通知消息
/// </summary>
public class NotificationExecutor : BaseExecutor
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public override string NodeType => "notification";

    /// <summary>
    /// 执行通知发送
    /// </summary>
    public override async Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node)
    {
        try
        {
            var config = ParseConfig<NotificationNodeConfig>(node.Config);

            // 获取上游变量
            var upstreamVars = GetUpstreamVariables(context, node.Id);

            // 替换内容中的变量
            var content = ReplaceVariables(config.Content, upstreamVars);
            var title = config.Title != null ? ReplaceVariables(config.Title, upstreamVars) : null;

            // 根据通知类型发送
            switch (config.NotificationType)
            {
                case "message":
                    await SendMessageAsync(context, config, title, content);
                    break;

                case "webhook":
                    await SendWebhookAsync(config, title, content);
                    break;
            }

            var outputs = new Dictionary<string, object>
            {
                ["_notificationSent"] = true,
                ["_recipients"] = config.Recipients,
                ["_executeTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return CreateSuccessResult(outputs);
        }
        catch (Exception ex)
        {
            return CreateFailResult($"发送通知失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 验证配置
    /// </summary>
    public override string? ValidateConfig(DagNode node)
    {
        var config = ParseConfig<NotificationNodeConfig>(node.Config);

        if (string.IsNullOrEmpty(config.Content))
            return "通知内容不能为空";

        if (config.NotificationType == "webhook" && string.IsNullOrEmpty(config.WebhookUrl))
            return "Webhook通知需要配置Webhook地址";

        return null;
    }

    private string ReplaceVariables(string? template, Dictionary<string, object> variables)
    {
        if (string.IsNullOrEmpty(template))
            return template ?? string.Empty;

        var result = template;
        foreach (var (key, value) in variables)
        {
            var placeholder = $"${{{key}}}";
            if (result.Contains(placeholder))
            {
                result = result.Replace(placeholder, value?.ToString() ?? string.Empty);
            }
        }
        return result;
    }

    private async Task SendMessageAsync(EtlExecutionContext context, NotificationNodeConfig config, string? title, string content)
    {
        // TODO: 集成消息服务发送通知
        await Task.CompletedTask;
    }

    private async Task SendWebhookAsync(NotificationNodeConfig config, string? title, string content)
    {
        if (string.IsNullOrEmpty(config.WebhookUrl))
            return;

        using var httpClient = new HttpClient();
        var payload = new { title, content, recipients = config.Recipients };
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

        await httpClient.PostAsync(config.WebhookUrl, new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json"));
    }
}