using EasyWeChatModels.Enums;
using Autofac;
using BusinessManager.Buz.IService;
using BusinessManager.Buz.Service;
using BusinessManager.Buz.AntWorkflow.Service;

namespace BusinessManager.Factory;

/// <summary>
/// 节点处理器工厂
/// </summary>
public class NodeHandlerFactory
{
    /// <summary>Autofac 组件上下文（属性注入）</summary>
    public IComponentContext _context { get; set; } = null!;

    /// <summary>
    /// 根据节点类型获取处理器
    /// </summary>
    public INodeHandlerService? GetHandler(AntNodeType nodeType)
    {
        return nodeType switch
        {
            AntNodeType.Start => _context.Resolve<StartNodeService>(),
            AntNodeType.End => _context.Resolve<EndNodeService>(),
            AntNodeType.Approver => _context.Resolve<ApproverNodeService>(),
            AntNodeType.Copyer => _context.Resolve<CopyerNodeService>(),
            AntNodeType.Condition => _context.Resolve<ConditionNodeService>(),
            AntNodeType.Parallel => _context.Resolve<ParallelNodeService>(),
            AntNodeType.Service => _context.Resolve<ServiceNodeService>(),
            AntNodeType.Notification => _context.Resolve<NotificationNodeService>(),
            AntNodeType.CounterSign => _context.Resolve<CounterSignNodeService>(),
            AntNodeType.Webhook => _context.Resolve<WebhookNodeService>(),
            AntNodeType.Subflow => _context.Resolve<SubflowNodeService>(),
            _ => null
        };
    }
}