namespace EasyWeChatModels.Dto;

/// <summary>
/// 部门信息 DTO
/// </summary>
/// <remarks>
/// 用于返回部门基本信息，支持树形结构
/// </remarks>
public class DepartmentDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 上级部门ID
    /// </summary>
    /// <remarks>
    /// 上级部门ID，根部门为 null
    /// </remarks>
    /// <example>null</example>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    /// <example>技术部</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    /// <example>TECH</example>
    public string? Code { get; set; }

    /// <summary>
    /// 部门完整路径
    /// </summary>
    /// <remarks>
    /// 如：总公司/技术部/前端组
    /// </remarks>
    /// <example>总公司/技术部</example>
    public string? FullPath { get; set; }

    /// <summary>
    /// 部门层级
    /// </summary>
    /// <remarks>
    /// 1=一级，2=二级，3=三级...
    /// </remarks>
    /// <example>2</example>
    public int Level { get; set; }

    /// <summary>
    /// 部门主管ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000010</example>
    public Guid? ManagerId { get; set; }

    /// <summary>
    /// 部门负责人姓名
    /// </summary>
    /// <example>张三</example>
    public string? LeaderName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    /// <example>13800138000</example>
    public string? Phone { get; set; }

    /// <summary>
    /// 部门邮箱
    /// </summary>
    /// <example>tech@example.com</example>
    public string? Email { get; set; }

    /// <summary>
    /// 部门描述
    /// </summary>
    /// <example>技术研发部门</example>
    public string? Description { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>
    /// 显示顺序，数值越小越靠前
    /// </remarks>
    /// <example>1</example>
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 部门状态：1-正常，0-禁用
    /// </remarks>
    /// <example>1</example>
    public int Status { get; set; }

    /// <summary>
    /// 成员数量
    /// </summary>
    /// <remarks>
    /// 部门下的用户数量（统计字段）
    /// </remarks>
    /// <example>10</example>
    public int MemberCount { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 子部门列表
    /// </summary>
    /// <remarks>
    /// 当前部门的子部门集合，用于构建树形结构
    /// </remarks>
    public List<DepartmentDto>? Children { get; set; }
}