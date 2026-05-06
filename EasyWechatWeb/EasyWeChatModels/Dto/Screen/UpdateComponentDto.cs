using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新组件请求 DTO
/// </summary>
/// <remarks>
/// 用于更新组件属性，所有字段均为可选
/// </remarks>
public class UpdateComponentDto
{
    /// <summary>
    /// 组件ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    [Required(ErrorMessage = "组件ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// X轴位置
    /// </summary>
    /// <example>100</example>
    [Range(0, int.MaxValue, ErrorMessage = "X轴位置不能为负数")]
    public int? PositionX { get; set; }

    /// <summary>
    /// Y轴位置
    /// </summary>
    /// <example>200</example>
    [Range(0, int.MaxValue, ErrorMessage = "Y轴位置不能为负数")]
    public int? PositionY { get; set; }

    /// <summary>
    /// 组件宽度
    /// </summary>
    /// <example>400</example>
    [Range(0, int.MaxValue, ErrorMessage = "组件宽度不能为负数")]
    public int? Width { get; set; }

    /// <summary>
    /// 组件高度
    /// </summary>
    /// <example>300</example>
    [Range(0, int.MaxValue, ErrorMessage = "组件高度不能为负数")]
    public int? Height { get; set; }

    /// <summary>
    /// 旋转角度
    /// </summary>
    /// <example>0</example>
    public int? Rotation { get; set; }

    /// <summary>
    /// 是否锁定
    /// </summary>
    /// <remarks>
    /// 0-未锁定，1-锁定
    /// </remarks>
    /// <example>0</example>
    public int? Locked { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <remarks>
    /// 0-隐藏，1-可见
    /// </remarks>
    /// <example>1</example>
    public int? Visible { get; set; }

    /// <summary>
    /// 数据源配置（JSON格式）
    /// </summary>
    /// <example>{"type":"static","data":[1,2,3]}</example>
    public string? DataSource { get; set; }

    /// <summary>
    /// 组件配置（JSON格式）
    /// </summary>
    /// <example>{"title":"销售趋势","showLegend":true}</example>
    public string? Config { get; set; }

    /// <summary>
    /// 样式配置（JSON格式）
    /// </summary>
    /// <example>{"borderColor":"#409eff","borderWidth":1}</example>
    public string? StyleConfig { get; set; }

    /// <summary>
    /// 数据绑定配置（JSON格式）
    /// </summary>
    /// <example>{"xField":"date","yField":"value"}</example>
    public string? DataBinding { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    /// <example>1</example>
    public int? SortOrder { get; set; }
}