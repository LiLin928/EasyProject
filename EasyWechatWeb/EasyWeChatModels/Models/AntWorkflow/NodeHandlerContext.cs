using SqlSugar;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Models;

namespace EasyWeChatModels.Models.AntWorkflow;

/// <summary>
/// 节点处理器上下文
/// </summary>
public class NodeHandlerContext
{
    /// <summary>数据库连接</summary>
    public required ISqlSugarClient Db { get; set; }

    /// <summary>流程实例</summary>
    public required AntWorkflowInstance Instance { get; set; }

    /// <summary>DAG配置</summary>
    public required DagConfig DagConfig { get; set; }

    /// <summary>当前DAG节点</summary>
    public required DagNode DagNode { get; set; }

    /// <summary>实例节点记录</summary>
    public required AntWorkflowInstanceNode InstanceNode { get; set; }

    /// <summary>业务数据JSON</summary>
    public string? BusinessData { get; set; }

    /// <summary>表单数据JSON</summary>
    public string? FormData { get; set; }

    /// <summary>操作用户ID</summary>
    public Guid? OperatorId { get; set; }

    /// <summary>操作用户姓名</summary>
    public string? OperatorName { get; set; }

    /// <summary>审批意见</summary>
    public string? ApproveDesc { get; set; }
}