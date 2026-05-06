namespace EasyWeChatModels.Dto;

/// <summary>
/// ETL任务流DTO
/// </summary>
public class EtlPipelineDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int? RetryCount { get; set; }

    /// <summary>
    /// 并发数
    /// </summary>
    public int? Concurrency { get; set; }

    /// <summary>
    /// 失败策略
    /// </summary>
    public string? FailureStrategy { get; set; }

    /// <summary>
    /// DAG配置（JSON）
    /// </summary>
    public string? DagConfig { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public string Status { get; set; } = "draft";

    /// <summary>
    /// 版本号
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    public Guid CreatorId { get; set; }

    /// <summary>
    /// 创建人名称
    /// </summary>
    public string? CreatorName { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public string? CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdateTime { get; set; }

    /// <summary>
    /// 发布时间
    /// </summary>
    public string? PublishTime { get; set; }
}

/// <summary>
/// 查询任务流参数
/// </summary>
public class QueryEtlPipelineDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 名称关键字
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 状态筛选
    /// </summary>
    public string? Status { get; set; }
}

/// <summary>
/// 创建任务流参数
/// </summary>
public class CreateEtlPipelineDto
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int? RetryCount { get; set; }

    /// <summary>
    /// 并发数
    /// </summary>
    public int? Concurrency { get; set; }

    /// <summary>
    /// 失败策略
    /// </summary>
    public string? FailureStrategy { get; set; }

    /// <summary>
    /// DAG配置
    /// </summary>
    public string? DagConfig { get; set; }
}

/// <summary>
/// 更新任务流参数
/// </summary>
public class UpdateEtlPipelineDto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 分类
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// 超时时间（秒）
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int? RetryCount { get; set; }

    /// <summary>
    /// 并发数
    /// </summary>
    public int? Concurrency { get; set; }

    /// <summary>
    /// 失败策略
    /// </summary>
    public string? FailureStrategy { get; set; }

    /// <summary>
    /// DAG配置（支持传入JSON对象或字符串）
    /// </summary>
    public object? DagConfig { get; set; }
}