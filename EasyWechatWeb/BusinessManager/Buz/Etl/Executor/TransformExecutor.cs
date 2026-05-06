using BusinessManager.Buz.Etl.Engine;
using EasyWeChatModels.Models.Etl;
using EasyWeChatModels.Models.Etl.NodeConfigs;

namespace BusinessManager.Buz.Etl.Executor;

/// <summary>
/// 转换节点执行器
/// 对数据进行转换处理：字段映射、过滤、聚合、脚本
/// </summary>
public class TransformExecutor : BaseExecutor
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public override string NodeType => "transform";

    /// <summary>
    /// 执行数据转换
    /// </summary>
    public override async Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node)
    {
        var startTime = DateTime.Now;

        try
        {
            // 1. 解析节点配置
            var config = ParseConfig<TransformNodeConfig>(node.Config);

            // 2. 验证必要字段
            var validationError = ValidateConfig(node);
            if (validationError != null)
            {
                return CreateFailResult(validationError);
            }

            // 3. 获取输入数据
            var inputData = GetInputData(context, node.Id, config.InputVariable);
            if (inputData == null)
            {
                return CreateFailResult($"输入变量 '{config.InputVariable}' 不存在或数据为空");
            }

            // 4. 根据转换类型执行转换
            var transformedData = await ExecuteTransformAsync(config, inputData, context, node.Id);

            // 5. 构建输出变量
            var outputs = new Dictionary<string, object>
            {
                [config.OutputVariable] = transformedData,
                ["_count"] = transformedData.Count,
                ["_transformType"] = config.TransformType,
                ["_transformTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            return CreateSuccessResult(outputs, transformedData.Count);
        }
        catch (Exception ex)
        {
            return CreateFailResult($"执行数据转换失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 验证配置
    /// </summary>
    public override string? ValidateConfig(DagNode node)
    {
        var config = ParseConfig<TransformNodeConfig>(node.Config);

        if (string.IsNullOrEmpty(config.InputVariable))
        {
            return "输入变量名不能为空";
        }

        if (string.IsNullOrEmpty(config.OutputVariable))
        {
            return "输出变量名不能为空";
        }

        // 根据转换类型检查必要字段
        switch (config.TransformType)
        {
            case "mapping":
                if (config.FieldMapping == null || config.FieldMapping.Count == 0)
                {
                    return "映射转换需要配置字段映射列表";
                }
                break;

            case "filter":
                if (string.IsNullOrEmpty(config.FilterExpression))
                {
                    return "过滤转换需要配置过滤表达式";
                }
                break;

            case "aggregate":
                if (config.AggregateConfig == null)
                {
                    return "聚合转换需要配置聚合配置";
                }
                break;

            case "script":
                if (string.IsNullOrEmpty(config.Script))
                {
                    return "脚本转换需要配置脚本内容";
                }
                break;

            default:
                return $"不支持的转换类型: {config.TransformType}";
        }

        return null;
    }

    /// <summary>
    /// 执行转换
    /// </summary>
    private async Task<List<Dictionary<string, object>>> ExecuteTransformAsync(
        TransformNodeConfig config,
        List<Dictionary<string, object>> inputData,
        EtlExecutionContext context,
        string nodeId)
    {
        switch (config.TransformType)
        {
            case "mapping":
                return ExecuteMapping(inputData, config.FieldMapping!);

            case "filter":
                return ExecuteFilter(inputData, config.FilterExpression!, context, nodeId);

            case "aggregate":
                return ExecuteAggregate(inputData, config.AggregateConfig!);

            case "script":
                return await ExecuteScriptAsync(inputData, config.Script!, config.ScriptLanguage ?? "javascript");

            default:
                throw new NotSupportedException($"不支持的转换类型: {config.TransformType}");
        }
    }

    // ========== 字段映射转换 ==========

    /// <summary>
    /// 执行字段映射转换
    /// </summary>
    private List<Dictionary<string, object>> ExecuteMapping(
        List<Dictionary<string, object>> inputData,
        List<FieldMappingItem> fieldMapping)
    {
        var result = new List<Dictionary<string, object>>();

        foreach (var row in inputData)
        {
            var newRow = new Dictionary<string, object>();

            foreach (var mapping in fieldMapping)
            {
                // 获取源字段值
                var sourceValue = row.TryGetValue(mapping.SourceField, out var val) ? val : null;

                // 应用转换表达式（如果有）
                var targetValue = ApplyFieldTransform(sourceValue, mapping.Transform, mapping.DataType);

                // 设置到目标字段
                newRow[mapping.TargetField] = targetValue!;
            }

            result.Add(newRow);
        }

        return result;
    }

    /// <summary>
    /// 应用字段转换表达式
    /// </summary>
    private object? ApplyFieldTransform(object? sourceValue, string? transform, string? dataType)
    {
        if (sourceValue == null)
        {
            return null;
        }

        // 数据类型转换
        if (!string.IsNullOrEmpty(dataType))
        {
            sourceValue = ConvertDataType(sourceValue, dataType);
        }

        // 转换表达式
        if (string.IsNullOrEmpty(transform))
        {
            return sourceValue;
        }

        // 简单转换函数
        var valueStr = sourceValue.ToString() ?? string.Empty;
        return transform.ToLower() switch
        {
            "toupper" => valueStr.ToUpper(),
            "tolower" => valueStr.ToLower(),
            "trim" => valueStr.Trim(),
            "substring" => valueStr.Length > 10 ? valueStr.Substring(0, 10) : valueStr,
            "toString" => sourceValue.ToString(),
            "toInt" => Convert.ToInt32(sourceValue),
            "toDouble" => Convert.ToDouble(sourceValue),
            "toBool" => Convert.ToBoolean(sourceValue),
            _ => sourceValue // 未识别的转换，保持原值
        };
    }

    /// <summary>
    /// 数据类型转换
    /// </summary>
    private object ConvertDataType(object value, string dataType)
    {
        try
        {
            return dataType.ToLower() switch
            {
                "string" => value.ToString() ?? string.Empty,
                "int" => Convert.ToInt32(value),
                "long" => Convert.ToInt64(value),
                "double" => Convert.ToDouble(value),
                "decimal" => Convert.ToDecimal(value),
                "bool" => Convert.ToBoolean(value),
                "datetime" => Convert.ToDateTime(value),
                "guid" => Guid.Parse(value.ToString() ?? string.Empty),
                _ => value
            };
        }
        catch
        {
            return value; // 转换失败，保持原值
        }
    }

    // ========== 数据过滤转换 ==========

    /// <summary>
    /// 执行数据过滤转换
    /// </summary>
    private List<Dictionary<string, object>> ExecuteFilter(
        List<Dictionary<string, object>> inputData,
        string filterExpression,
        EtlExecutionContext context,
        string nodeId)
    {
        var result = new List<Dictionary<string, object>>();
        var upstreamVars = GetUpstreamVariables(context, nodeId);

        foreach (var row in inputData)
        {
            if (EvaluateFilterExpression(row, filterExpression, upstreamVars))
            {
                result.Add(row);
            }
        }

        return result;
    }

    /// <summary>
    /// 评估过滤表达式
    /// </summary>
    private bool EvaluateFilterExpression(
        Dictionary<string, object> row,
        string expression,
        Dictionary<string, object> upstreamVars)
    {
        // 替换变量
        var processedExpression = ReplaceVariables(expression, row, upstreamVars);

        // 简单表达式解析（支持基本比较和逻辑运算）
        // 格式示例: "age > 18", "status == 'active'", "price >= 100 && quantity > 0"

        try
        {
            // 处理 AND/OR 逻辑
            if (processedExpression.Contains("&&"))
            {
                var parts = processedExpression.Split("&&");
                return parts.All(p => EvaluateSimpleExpression(p.Trim()));
            }

            if (processedExpression.Contains("||"))
            {
                var parts = processedExpression.Split("||");
                return parts.Any(p => EvaluateSimpleExpression(p.Trim()));
            }

            return EvaluateSimpleExpression(processedExpression);
        }
        catch
        {
            return false; // 解析失败，默认不匹配
        }
    }

    /// <summary>
    /// 评估简单表达式
    /// </summary>
    private bool EvaluateSimpleExpression(string expression)
    {
        // 支持的操作符: ==, !=, >, <, >=, <=, contains
        var operators = new[] { "==", "!=", ">=", "<=", ">", "<", "contains" };

        foreach (var op in operators)
        {
            if (expression.Contains(op))
            {
                var parts = expression.Split(new[] { op }, StringSplitOptions.None);
                if (parts.Length != 2) continue;

                var left = parts[0].Trim().TrimQuotes();
                var right = parts[1].Trim().TrimQuotes();

                return EvaluateComparison(left, right, op);
            }
        }

        return true; // 无法解析，默认返回 true
    }

    /// <summary>
    /// 评估比较操作
    /// </summary>
    private bool EvaluateComparison(string left, string right, string op)
    {
        // 尝试数值比较
        if (double.TryParse(left, out var leftNum) && double.TryParse(right, out var rightNum))
        {
            return op switch
            {
                "==" => leftNum == rightNum,
                "!=" => leftNum != rightNum,
                ">" => leftNum > rightNum,
                "<" => leftNum < rightNum,
                ">=" => leftNum >= rightNum,
                "<=" => leftNum <= rightNum,
                _ => false
            };
        }

        // 字符串比较
        return op switch
        {
            "==" => left == right,
            "!=" => left != right,
            "contains" => left.Contains(right),
            ">" => left.CompareTo(right) > 0,
            "<" => left.CompareTo(right) < 0,
            ">=" => left.CompareTo(right) >= 0,
            "<=" => left.CompareTo(right) <= 0,
            _ => false
        };
    }

    /// <summary>
    /// 替换表达式中的变量
    /// </summary>
    private string ReplaceVariables(
        string expression,
        Dictionary<string, object> rowData,
        Dictionary<string, object> upstreamVars)
    {
        var result = expression;

        // 替换行数据变量（格式: ${field}）
        foreach (var (key, value) in rowData)
        {
            var placeholder = $"${{{key}}}";
            if (result.Contains(placeholder))
            {
                var replacement = value is string ? $"'{value}'" : value?.ToString() ?? "null";
                result = result.Replace(placeholder, replacement);
            }
        }

        // 替换上游变量（格式: @{varName}）
        foreach (var (key, value) in upstreamVars)
        {
            var placeholder = $"@{{{key}}}";
            if (result.Contains(placeholder))
            {
                var replacement = value is string ? $"'{value}'" : value?.ToString() ?? "null";
                result = result.Replace(placeholder, replacement);
            }
        }

        return result;
    }

    // ========== 数据聚合转换 ==========

    /// <summary>
    /// 执行数据聚合转换
    /// </summary>
    private List<Dictionary<string, object>> ExecuteAggregate(
        List<Dictionary<string, object>> inputData,
        AggregateConfig aggregateConfig)
    {
        // 1. 分组
        var groupedData = GroupData(inputData, aggregateConfig.GroupBy);

        // 2. 计算聚合
        var result = new List<Dictionary<string, object>>();

        foreach (var group in groupedData)
        {
            var row = new Dictionary<string, object>();

            // 设置分组字段值
            foreach (var groupKey in group.Key)
            {
                row[groupKey.Key] = groupKey.Value;
            }

            // 计算聚合值
            foreach (var agg in aggregateConfig.Aggregations)
            {
                var aggValue = CalculateAggregate(group.List, agg);
                row[agg.Alias ?? $"{agg.Function}_{agg.Field}"] = aggValue;
            }

            result.Add(row);
        }

        return result;
    }

    /// <summary>
    /// 分组数据
    /// </summary>
    private List<GroupResult> GroupData(
        List<Dictionary<string, object>> inputData,
        List<string>? groupBy)
    {
        if (groupBy == null || groupBy.Count == 0)
        {
            // 不分组，整体作为一组
            return new List<GroupResult>
            {
                new GroupResult
                {
                    Key = new Dictionary<string, object>(),
                    List = inputData
                }
            };
        }

        var groups = new Dictionary<string, GroupResult>();

        foreach (var row in inputData)
        {
            // 构建分组键
            var keyValues = new Dictionary<string, object>();
            var keyStr = string.Empty;

            foreach (var groupField in groupBy)
            {
                if (row.TryGetValue(groupField, out var value))
                {
                    keyValues[groupField] = value ?? string.Empty;
                    keyStr += $"{groupField}:{value}|";
                }
            }

            if (!groups.ContainsKey(keyStr))
            {
                groups[keyStr] = new GroupResult
                {
                    Key = keyValues,
                    List = new List<Dictionary<string, object>>()
                };
            }

            groups[keyStr].List.Add(row);
        }

        return groups.Values.ToList();
    }

    /// <summary>
    /// 计算聚合值
    /// </summary>
    private object CalculateAggregate(List<Dictionary<string, object>> groupData, AggregateItem agg)
    {
        var values = groupData
            .Where(row => row.ContainsKey(agg.Field))
            .Select(row => row[agg.Field])
            .Where(v => v != null)
            .ToList();

        if (values.Count == 0)
        {
            return 0;
        }

        // 尝试转换为数值
        var numericValues = values
            .Select(v => Convert.ToDouble(v))
            .ToList();

        return agg.Function.ToLower() switch
        {
            "sum" => numericValues.Sum(),
            "avg" => numericValues.Average(),
            "count" => values.Count,
            "max" => numericValues.Max(),
            "min" => numericValues.Min(),
            _ => 0
        };
    }

    // ========== 脚本转换 ==========

    /// <summary>
    /// 执行脚本转换
    /// </summary>
    private async Task<List<Dictionary<string, object>>> ExecuteScriptAsync(
        List<Dictionary<string, object>> inputData,
        string script,
        string scriptLanguage)
    {
        // JavaScript 脚本执行（使用简单的反射模拟）
        // 完整实现需要 Jint 库（在后续优化中添加）

        if (scriptLanguage == "javascript")
        {
            // 目前返回原数据（脚本执行需要 Jint 库支持）
            // TODO: 集成 Jint 库实现真正的 JavaScript 执行
            await Task.CompletedTask;
            return inputData;
        }

        // Python/SQL 脚本（需要外部执行）
        // TODO: 实现外部脚本执行
        await Task.CompletedTask;
        return inputData;
    }

    // ========== 辅助类 ==========

    /// <summary>
    /// 分组结果
    /// </summary>
    private class GroupResult
    {
        public Dictionary<string, object> Key { get; set; } = new();
        public List<Dictionary<string, object>> List { get; set; } = new();
    }
}

/// <summary>
/// 字符串扩展方法
/// </summary>
internal static class StringExtensions
{
    /// <summary>
    /// 去除引号
    /// </summary>
    public static string TrimQuotes(this string str)
    {
        return str.Trim().Trim('\'', '"', '`');
    }
}