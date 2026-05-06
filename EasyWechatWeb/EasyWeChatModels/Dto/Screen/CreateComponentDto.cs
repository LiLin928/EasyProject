using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 创建组件请求 DTO
/// </summary>
/// <remarks>
/// 用于在大屏上创建新组件
/// </remarks>
public class CreateComponentDto
{
    /// <summary>
    /// 所属大屏ID
    /// </summary>
    /// <example>00000000-0000-0000-0000-000000000001</example>
    [Required(ErrorMessage = "大屏ID不能为空")]
    public Guid ScreenId { get; set; }

    /// <summary>
    /// 组件类型
    /// </summary>
    /// <example>chart-line</example>
    [Required(ErrorMessage = "组件类型不能为空")]
    [MaxLength(50, ErrorMessage = "组件类型长度不能超过50个字符")]
    public string ComponentType { get; set; } = string.Empty;

    /// <summary>
    /// X轴位置
    /// </summary>
    /// <example>100</example>
    [Range(0, int.MaxValue, ErrorMessage = "X轴位置不能为负数")]
    public int PositionX { get; set; }

    /// <summary>
    /// Y轴位置
    /// </summary>
    /// <example>200</example>
    [Range(0, int.MaxValue, ErrorMessage = "Y轴位置不能为负数")]
    public int PositionY { get; set; }

    /// <summary>
    /// 组件宽度
    /// </summary>
    /// <example>400</example>
    [Range(0, int.MaxValue, ErrorMessage = "组件宽度不能为负数")]
    public int Width { get; set; }

    /// <summary>
    /// 组件高度
    /// </summary>
    /// <example>300</example>
    [Range(0, int.MaxValue, ErrorMessage = "组件高度不能为负数")]
    public int Height { get; set; }

    /// <summary>
    /// 数据源配置（JSON格式）
    /// </summary>
    /// <example>{"type":"static","data":[1,2,3]}</example>
    public string DataSource { get; set; } = "{}";

    /// <summary>
    /// 组件配置（JSON格式）
    /// </summary>
    /// <example>{"title":"销售趋势","showLegend":true}</example>
    public string Config { get; set; } = "{}";

    /// <summary>
    /// 样式配置（JSON格式）
    /// </summary>
    /// <example>{"borderColor":"#409eff","borderWidth":1}</example>
    public string StyleConfig { get; set; } = "{}";

    /// <summary>
    /// 数据绑定配置（JSON格式）
    /// </summary>
    /// <example>{"xField":"date","yField":"value"}</example>
    public string DataBinding { get; set; } = "{}";
}