using BusinessManager.Buz.Etl.Engine;
using EasyWeChatModels.Models.Etl;
using EasyWeChatModels.Models.Etl.NodeConfigs;

namespace BusinessManager.Buz.Etl.Executor;

/// <summary>
/// 子流程节点执行器
/// 调用其他任务流作为子流程
/// </summary>
public class SubflowExecutor : BaseExecutor
{
    /// <summary>
    /// 节点类型
    /// </summary>
    public override string NodeType => "subflow";

    /// <summary>
    /// 执行子流程
    /// </summary>
    public override async Task<EtlNodeResult> ExecuteAsync(EtlExecutionContext context, DagNode node)
    {
        try
        {
            var config = ParseConfig<SubflowNodeConfig>(node.Config);

            // 获取上游变量
            var upstreamVars = GetUpstreamVariables(context, node.Id);

            // 构建子流程参数
            var subflowParams = BuildSubflowParams(config, upstreamVars);

            // TODO: 实现真正的子流程调用（需要创建新的执行记录并执行）

            var outputs = new Dictionary<string, object>
            {
                ["_subflowPipelineId"] = config.SubflowPipelineId.ToString(),
                ["_subflowParams"] = subflowParams,
                ["_executeTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            await Task.CompletedTask;
            return CreateSuccessResult(outputs);
        }
        catch (Exception ex)
        {
            return CreateFailResult($"执行子流程失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 验证配置
    /// </summary>
    public override string? ValidateConfig(DagNode node)
    {
        var config = ParseConfig<SubflowNodeConfig>(node.Config);

        if (config.SubflowPipelineId == Guid.Empty)
            return "子流程ID不能为空";

        return null;
    }

    private Dictionary<string, object> BuildSubflowParams(SubflowNodeConfig config, Dictionary<string, object> upstreamVars)
    {
        var paramsDict = new Dictionary<string, object>();

        if (config.InputMapping != null)
        {
            foreach (var mapping in config.InputMapping)
            {
                if (upstreamVars.TryGetValue(mapping.SourceField, out var value))
                {
                    paramsDict[mapping.TargetField] = value;
                }
            }
        }

        return paramsDict;
    }
}