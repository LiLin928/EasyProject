using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 大屏组件实体类
/// </summary>
/// <remarks>
/// 用于存储大屏中各个组件的配置信息，包括位置、尺寸、数据源等
/// </remarks>
[SugarTable("ScreenComponent", "大屏组件明细表")]
public class ScreenComponent
{
    /// <summary>
    /// 组件ID（主键）
    /// </summary>
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 大屏ID
    /// </summary>
    /// <remarks>
    /// 所属大屏的外键关联
    /// </remarks>
    [SugarColumn(ColumnDescription = "大屏ID")]
    public Guid ScreenId { get; set; }

    /// <summary>
    /// 组件类型
    /// </summary>
    /// <remarks>
    /// 组件类型标识，如 chart、table、image 等，长度限制50字符
    /// </remarks>
    [SugarColumn(Length = 50, ColumnDescription = "组件类型")]
    public string ComponentType { get; set; } = string.Empty;

    /// <summary>
    /// X坐标位置
    /// </summary>
    /// <remarks>
    /// 组件在大屏中的X轴位置（像素），默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "X坐标位置")]
    public int PositionX { get; set; } = 0;

    /// <summary>
    /// Y坐标位置
    /// </summary>
    /// <remarks>
    /// 组件在大屏中的Y轴位置（像素），默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "Y坐标位置")]
    public int PositionY { get; set; } = 0;

    /// <summary>
    /// 宽度
    /// </summary>
    /// <remarks>
    /// 组件的宽度（像素），默认值为400
    /// </remarks>
    [SugarColumn(ColumnDescription = "宽度")]
    public int Width { get; set; } = 400;

    /// <summary>
    /// 高度
    /// </summary>
    /// <remarks>
    /// 组件的高度（像素），默认值为300
    /// </remarks>
    [SugarColumn(ColumnDescription = "高度")]
    public int Height { get; set; } = 300;

    /// <summary>
    /// 旋转角度
    /// </summary>
    /// <remarks>
    /// 组件的旋转角度（度），默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "旋转角度")]
    public int Rotation { get; set; } = 0;

    /// <summary>
    /// 是否锁定
    /// </summary>
    /// <remarks>
    /// 组件是否被锁定不可编辑：1-锁定，0-未锁定。默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否锁定：1-锁定，0-未锁定")]
    public int Locked { get; set; } = 0;

    /// <summary>
    /// 是否可见
    /// </summary>
    /// <remarks>
    /// 组件是否可见：1-可见，0-隐藏。默认值为1
    /// </remarks>
    [SugarColumn(ColumnDescription = "是否可见：1-可见，0-隐藏")]
    public int Visible { get; set; } = 1;

    /// <summary>
    /// 数据源配置
    /// </summary>
    /// <remarks>
    /// 组件的数据源配置，JSON格式存储，默认为空对象
    /// </remarks>
    [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "数据源配置")]
    public string DataSource { get; set; } = "{}";

    /// <summary>
    /// 组件配置
    /// </summary>
    /// <remarks>
    /// 组件的基本配置项，JSON格式存储，默认为空对象
    /// </remarks>
    [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "组件配置")]
    public string Config { get; set; } = "{}";

    /// <summary>
    /// 样式配置
    /// </summary>
    /// <remarks>
    /// 组件的样式配置，JSON格式存储，默认为空对象
    /// </remarks>
    [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "样式配置")]
    public string StyleConfig { get; set; } = "{}";

    /// <summary>
    /// 数据绑定配置
    /// </summary>
    /// <remarks>
    /// 组件的数据绑定配置，JSON格式存储，默认为空对象
    /// </remarks>
    [SugarColumn(ColumnDataType = "longtext", ColumnDescription = "数据绑定配置")]
    public string DataBinding { get; set; } = "{}";

    /// <summary>
    /// 排序顺序
    /// </summary>
    /// <remarks>
    /// 组件在图层中的排序顺序，数值越小越靠前，默认值为0
    /// </remarks>
    [SugarColumn(ColumnDescription = "排序顺序")]
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 创建时间
    /// </summary>
    /// <remarks>
    /// 组件记录创建时间，默认为当前时间
    /// </remarks>
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新时间
    /// </summary>
    /// <remarks>
    /// 组件信息最后更新时间，可为空
    /// </remarks>
    [SugarColumn(IsNullable = true, ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }
}