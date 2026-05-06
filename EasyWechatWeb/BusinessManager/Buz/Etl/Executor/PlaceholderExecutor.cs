using BusinessManager.Buz.Etl.Engine;
using EasyWeChatModels.Models.Etl;

namespace BusinessManager.Buz.Etl.Executor;

/// <summary>
/// 占位执行器
/// 用于未实现的节点类型，返回失败结果
/// </summary>
public class PlaceholderExecutor : IEtlNodeExecutor
{
    /// <summary>
    /// 节点类型（占位符）
    /// </summary>
    public string NodeType => "placeholder";

    /// <summary>
    /// 执行节点（返回未实现错误）
    /// </summary>
    public Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node)
    {
        var result = EtlNodeResult.FailResult($"节点类型 '{node.Type}' 的执行器尚未实现");
        return Task.FromResult(result);
    }

    /// <summary>
    /// 验证配置（返回未实现错误）
    /// </summary>
    public string? ValidateConfig(DagNode node)
    {
        return $"节点类型 '{node.Type}' 的执行器尚未实现";
    }
}