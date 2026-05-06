using BusinessManager.Buz.Etl.Engine;
using EasyWeChatModels.Models.Etl;

namespace BusinessManager.Buz.Etl.Executor;

/// <summary>
/// ETL 节点执行器接口
/// 所有节点执行器必须实现此接口
/// </summary>
public interface IEtlNodeExecutor
{
    /// <summary>
    /// 处理的节点类型
    /// 例如: datasource, sql, transform, output, api, file, script, condition, parallel, notification, subflow
    /// </summary>
    string NodeType { get; }

    /// <summary>
    /// 执行节点
    /// </summary>
    /// <param name="context">执行上下文（包含变量池、上游状态等）</param>
    /// <param name="node">节点配置</param>
    /// <returns>节点执行结果</returns>
    Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node);

    /// <summary>
    /// 验证节点配置是否有效（可选实现）
    /// </summary>
    /// <param name="node">节点配置</param>
    /// <returns>验证结果，如果配置无效返回错误信息</returns>
    string? ValidateConfig(DagNode node) => null;

    /// <summary>
    /// 执行前检查上游状态（可选重写）
    /// </summary>
    /// <param name="context">执行上下文</param>
    /// <param name="node">节点配置</param>
    /// <returns>是否可以执行，如果不能执行返回原因</returns>
    Task<(bool canExecute, string? reason)> CheckUpstreamAsync(EtlExecutionContext context, DagNode node)
    {
        // 默认检查：上游必须全部完成
        if (!context.IsUpstreamCompleted(node.Id))
        {
            return Task.FromResult<(bool canExecute, string? reason)>((false, "上游节点未全部完成"));
        }
        return Task.FromResult<(bool canExecute, string? reason)>((true, null));
    }
}