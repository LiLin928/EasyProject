using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 部门实体类
/// </summary>
/// <remarks>
/// 用于存储组织架构中的部门信息，支持树形结构
/// </remarks>
[SugarTable("Department", "部门表")]
public class Department
{
    /// <summary>
    /// 部门ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(ColumnDescription = "部门名称", Length = 100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 上级部门ID
    /// </summary>
    [SugarColumn(ColumnDescription = "上级部门ID")]
    public Guid? ParentId { get; set; }

    /// <summary>
    /// 部门主管ID
    /// </summary>
    [SugarColumn(ColumnDescription = "部门主管ID")]
    public Guid? ManagerId { get; set; }

    /// <summary>
    /// 部门编码
    /// </summary>
    [SugarColumn(ColumnDescription = "部门编码", Length = 50)]
    public string? Code { get; set; }

    /// <summary>
    /// 部门完整路径
    /// </summary>
    /// <remarks>
    /// 如：总公司/技术部/前端组，用于快速定位和显示
    /// </remarks>
    [SugarColumn(ColumnDescription = "部门完整路径", Length = 500)]
    public string? FullPath { get; set; }

    /// <summary>
    /// 部门层级
    /// </summary>
    /// <remarks>
    /// 1=一级（总公司），2=二级，3=三级...
    /// </remarks>
    [SugarColumn(ColumnDescription = "部门层级")]
    public int Level { get; set; } = 1;

    /// <summary>
    /// 部门负责人姓名
    /// </summary>
    /// <remarks>
    /// 冗余字段，方便显示，从用户表同步
    /// </remarks>
    [SugarColumn(ColumnDescription = "部门负责人姓名", Length = 50)]
    public string? LeaderName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(ColumnDescription = "联系电话", Length = 20)]
    public string? Phone { get; set; }

    /// <summary>
    /// 部门邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "部门邮箱", Length = 100)]
    public string? Email { get; set; }

    /// <summary>
    /// 部门描述
    /// </summary>
    [SugarColumn(ColumnDescription = "部门描述", Length = 500)]
    public string? Description { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}