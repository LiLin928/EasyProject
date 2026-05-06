using BusinessManager.Buz.Etl.Engine;
using BusinessManager.Buz.Etl.Log;
using EasyWeChatModels.Models.Etl;
using EasyWeChatModels.Models.Etl.NodeConfigs;
using Newtonsoft.Json;
using SqlSugar;

namespace BusinessManager.Buz.Etl.Executor;

/// <summary>
/// ETL 执行器基类
/// 提供公共辅助方法，具体执行器可以继承此类
/// </summary>
public abstract class BaseExecutor : IEtlNodeExecutor
{
    /// <summary>
    /// 处理的节点类型（子类必须实现）
    /// </summary>
    public abstract string NodeType { get; }

    /// <summary>
    /// 执行节点（子类必须实现）
    /// </summary>
    public abstract Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node);

    /// <summary>
    /// 写入执行日志
    /// </summary>
    protected async Task WriteLogAsync(EtlExecutionContext context, string level, string message, string? nodeId = null, string? nodeName = null)
    {
        if (context.Logger != null)
        {
            await context.Logger.LogAsync(context.ExecutionId, nodeId, nodeName, level, message);
        }
    }

    /// <summary>
    /// 写入信息日志
    /// </summary>
    protected async Task LogInfoAsync(EtlExecutionContext context, string message, string? nodeId = null, string? nodeName = null)
    {
        if (context.Logger != null)
        {
            await context.Logger.LogInfoAsync(context.ExecutionId, nodeId, nodeName, message);
        }
    }

    /// <summary>
    /// 写入警告日志
    /// </summary>
    protected async Task LogWarningAsync(EtlExecutionContext context, string message, string? nodeId = null, string? nodeName = null)
    {
        if (context.Logger != null)
        {
            await context.Logger.LogWarningAsync(context.ExecutionId, nodeId, nodeName, message);
        }
    }

    /// <summary>
    /// 写入错误日志
    /// </summary>
    protected async Task LogErrorAsync(EtlExecutionContext context, string message, string? nodeId = null, string? nodeName = null)
    {
        if (context.Logger != null)
        {
            await context.Logger.LogErrorAsync(context.ExecutionId, nodeId, nodeName, message);
        }
    }

    /// <summary>
    /// 解析节点配置 JSON（支持 camelCase 属性名）
    /// </summary>
    protected T ParseConfig<T>(string? configJson) where T : class, new()
    {
        if (string.IsNullOrEmpty(configJson))
        {
            return new T();
        }

        try
        {
            // 使用 camelCase 反序列化设置，匹配前端传递的属性名
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.DeserializeObject<T>(configJson, settings) ?? new T();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"解析节点配置失败: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 获取上游变量
    /// </summary>
    protected Dictionary<string, object> GetUpstreamVariables(EtlExecutionContext context, string nodeId)
    {
        return context.GetUpstreamVariables(nodeId);
    }

    /// <summary>
    /// 获取特定变量值
    /// </summary>
    protected object? GetVariable(EtlExecutionContext context, string nodeId, string variableName)
    {
        return context.GetVariable(nodeId, variableName);
    }

    /// <summary>
    /// 获取输入变量数据（通常是一个数据列表）
    /// </summary>
    protected List<Dictionary<string, object>>? GetInputData(EtlExecutionContext context, string nodeId, string inputVariable)
    {
        var value = context.GetVariable(nodeId, inputVariable);
        if (value == null)
        {
            return null;
        }

        // 尝试转换为列表
        if (value is List<Dictionary<string, object>> list)
        {
            return list;
        }

        // 尝试从 JSON 解析
        if (value is string jsonStr)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonStr);
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// 设置输出变量
    /// </summary>
    protected void SetOutputVariables(EtlExecutionContext context, string nodeId, Dictionary<string, object> outputs)
    {
        context.SetOutputVariables(nodeId, outputs);
    }

    /// <summary>
    /// 设置单个输出变量
    /// </summary>
    protected void SetOutputVariable(EtlExecutionContext context, string nodeId, string variableName, object value)
    {
        context.SetOutputVariable(nodeId, variableName, value);
    }

    /// <summary>
    /// 验证配置是否有效（子类可选重写）
    /// </summary>
    public virtual string? ValidateConfig(DagNode node)
    {
        return null;
    }

    /// <summary>
    /// 检查上游状态（默认实现）
    /// </summary>
    public virtual async Task<(bool canExecute, string? reason)> CheckUpstreamAsync(EtlExecutionContext context, DagNode node)
    {
        if (!context.IsUpstreamCompleted(node.Id))
        {
            return (false, "上游节点未全部完成");
        }
        return (true, null);
    }

    /// <summary>
    /// 创建成功结果
    /// </summary>
    protected EtlNodeResult CreateSuccessResult(Dictionary<string, object> outputs, int? processedRows = null)
    {
        return EtlNodeResult.SuccessResult(outputs, processedRows);
    }

    /// <summary>
    /// 创建失败结果
    /// </summary>
    protected EtlNodeResult CreateFailResult(string errorMessage, int retryCount = 0)
    {
        return EtlNodeResult.FailResult(errorMessage, retryCount);
    }

    /// <summary>
    /// 创建空结果（无输出）
    /// </summary>
    protected EtlNodeResult CreateEmptyResult()
    {
        return EtlNodeResult.EmptyResult();
    }
}