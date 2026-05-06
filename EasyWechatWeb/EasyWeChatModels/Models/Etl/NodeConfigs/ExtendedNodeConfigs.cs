namespace EasyWeChatModels.Models.Etl.NodeConfigs;

/// <summary>
/// SQL节点配置
/// </summary>
public class SqlNodeConfig
{
    /// <summary>数据源ID</summary>
    public string DatasourceId { get; set; } = string.Empty;

    /// <summary>SQL类型: query, insert, update, delete, ddl</summary>
    public string SqlType { get; set; } = "query";

    /// <summary>SQL语句</summary>
    public string Sql { get; set; } = string.Empty;

    /// <summary>输出变量名（query 类型）</summary>
    public string? OutputVariable { get; set; }

    /// <summary>参数（JSON格式）</summary>
    public string? Parameters { get; set; }
}

/// <summary>
/// API节点配置
/// </summary>
public class ApiNodeConfig
{
    /// <summary>API地址</summary>
    public string ApiUrl { get; set; } = string.Empty;

    /// <summary>HTTP方法: GET, POST, PUT, DELETE</summary>
    public string ApiMethod { get; set; } = "GET";

    /// <summary>请求头（JSON格式）</summary>
    public string? ApiHeaders { get; set; }

    /// <summary>请求体</summary>
    public string? ApiBody { get; set; }

    /// <summary>超时时间（秒）</summary>
    public int? Timeout { get; set; }

    /// <summary>失败重试次数</summary>
    public int? RetryOnFailure { get; set; }

    /// <summary>输出变量名</summary>
    public string OutputVariable { get; set; } = "response";

    /// <summary>响应字段映射</summary>
    public List<FieldMappingItem>? ResponseMapping { get; set; }
}

/// <summary>
/// 文件节点配置
/// </summary>
public class FileNodeConfig
{
    /// <summary>文件操作: read, write, delete, move, copy</summary>
    public string Operation { get; set; } = "read";

    /// <summary>文件路径</summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>文件类型: csv, excel, json, txt</summary>
    public string FileType { get; set; } = "csv";

    /// <summary>分隔符（CSV）</summary>
    public string? Delimiter { get; set; }

    /// <summary>编码</summary>
    public string? Encoding { get; set; }

    /// <summary>Sheet名称（Excel）</summary>
    public string? SheetName { get; set; }

    /// <summary>输入变量名（write 操作）</summary>
    public string? InputVariable { get; set; }

    /// <summary>输出变量名（read 操作）</summary>
    public string? OutputVariable { get; set; }

    /// <summary>目标路径（move, copy 操作）</summary>
    public string? TargetPath { get; set; }
}

/// <summary>
/// 脚本节点配置
/// </summary>
public class ScriptNodeConfig
{
    /// <summary>脚本语言: javascript, python, shell</summary>
    public string ScriptLanguage { get; set; } = "javascript";

    /// <summary>脚本内容</summary>
    public string Script { get; set; } = string.Empty;

    /// <summary>脚本路径（外部文件）</summary>
    public string? ScriptPath { get; set; }

    /// <summary>输入变量名</summary>
    public string? InputVariable { get; set; }

    /// <summary>输出变量名</summary>
    public string OutputVariable { get; set; } = "result";

    /// <summary>参数</summary>
    public string? Parameters { get; set; }
}

/// <summary>
/// 条件节点配置
/// </summary>
public class ConditionNodeConfig
{
    /// <summary>条件类型: simple, expression</summary>
    public string ConditionType { get; set; } = "simple";

    /// <summary>条件规则列表</summary>
    public List<ConditionRule>? Rules { get; set; }

    /// <summary>条件表达式（expression 类型）</summary>
    public string? Expression { get; set; }

    /// <summary>输入变量名</summary>
    public string? InputVariable { get; set; }

    /// <summary>默认分支标签</summary>
    public string DefaultBranch { get; set; } = "default";
}

/// <summary>
/// 条件规则
/// </summary>
public class ConditionRule
{
    /// <summary>字段名</summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>操作符: eq, ne, gt, lt, gte, lte, contains, regex</summary>
    public string Operator { get; set; } = "eq";

    /// <summary>比较值</summary>
    public object Value { get; set; } = string.Empty;

    /// <summary>分支标签</summary>
    public string BranchLabel { get; set; } = "default";
}

/// <summary>
/// 并行节点配置
/// </summary>
public class ParallelNodeConfig
{
    /// <summary>等待模式: all（全部完成），any（任意完成）</summary>
    public string WaitMode { get; set; } = "all";
}

/// <summary>
/// 通知节点配置
/// </summary>
public class NotificationNodeConfig
{
    /// <summary>通知类型: message, webhook</summary>
    public string NotificationType { get; set; } = "message";

    /// <summary>接收人列表</summary>
    public List<string> Recipients { get; set; } = new();

    /// <summary>通知标题</summary>
    public string? Title { get; set; }

    /// <summary>通知内容</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>触发条件: always, success, failure</summary>
    public string TriggerCondition { get; set; } = "always";

    /// <summary>Webhook地址（webhook 类型）</summary>
    public string? WebhookUrl { get; set; }
}

/// <summary>
/// 子流程节点配置
/// </summary>
public class SubflowNodeConfig
{
    /// <summary>子流程ID</summary>
    public Guid SubflowPipelineId { get; set; }

    /// <summary>输入参数映射</summary>
    public List<FieldMappingItem>? InputMapping { get; set; }

    /// <summary>输出参数映射</summary>
    public List<FieldMappingItem>? OutputMapping { get; set; }

    /// <summary>等待子流程完成</summary>
    public bool WaitForCompletion { get; set; } = true;
}