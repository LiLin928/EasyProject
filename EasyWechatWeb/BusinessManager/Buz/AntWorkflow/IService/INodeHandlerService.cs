using EasyWeChatModels.Enums;
using EasyWeChatModels.Models.AntWorkflow;

namespace BusinessManager.Buz.IService;

/// <summary>
/// Ant节点处理器服务接口
/// </summary>
public interface INodeHandlerService
{
    /// <summary>处理的节点类型</summary>
    AntNodeType NodeType { get; }

    /// <summary>
    /// 处理节点进入
    /// </summary>
    Task HandleEnterAsync(NodeHandlerContext context);

    /// <summary>
    /// 处理节点完成
    /// </summary>
    Task HandleCompleteAsync(NodeHandlerContext context);

    /// <summary>
    /// 获取下一节点ID列表
    /// </summary>
    Task<List<string>> GetNextNodesAsync(NodeHandlerContext context);
}