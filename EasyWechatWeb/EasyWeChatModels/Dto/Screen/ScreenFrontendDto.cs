namespace EasyWeChatModels.Dto;

/// <summary>
/// 大屏前端响应 DTO
/// </summary>
/// <remarks>
/// 用于前端展示，字段名和结构与前端 TypeScript 类型匹配
/// </remarks>
public class ScreenFrontendDto
{
    /// <summary>
    /// 大屏ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 大屏名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 大屏描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 缩略图URL
    /// </summary>
    public string? Thumbnail { get; set; }

    /// <summary>
    /// 大屏样式配置（已解析为对象）
    /// </summary>
    public ScreenStyleDto? Style { get; set; }

    /// <summary>
    /// 组件列表（已转换为前端格式）
    /// </summary>
    public List<ScreenComponentFrontendDto> Components { get; set; } = new();

    /// <summary>
    /// 权限配置（已解析为对象）
    /// </summary>
    public ScreenPermissionsDto? Permissions { get; set; }

    /// <summary>
    /// 是否公开
    /// </summary>
    public int IsPublic { get; set; }

    /// <summary>
    /// 创建者ID
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// 创建时间
    /// </summary>
    public string? CreatedAt { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public string? UpdatedAt { get; set; }
}

/// <summary>
/// 大屏样式 DTO
/// </summary>
public class ScreenStyleDto
{
    /// <summary>
    /// 背景样式
    /// </summary>
    public string Background { get; set; } = string.Empty;

    /// <summary>
    /// 宽度
    /// </summary>
    public int Width { get; set; } = 1920;

    /// <summary>
    /// 高度
    /// </summary>
    public int Height { get; set; } = 1080;
}

/// <summary>
/// 大屏权限 DTO
/// </summary>
public class ScreenPermissionsDto
{
    /// <summary>
    /// 分享的用户ID列表
    /// </summary>
    public List<string> SharedUsers { get; set; } = new();

    /// <summary>
    /// 分享的角色ID列表
    /// </summary>
    public List<string> SharedRoles { get; set; } = new();
}

/// <summary>
/// 大屏组件前端 DTO
/// </summary>
/// <remarks>
/// 字段名和结构与前端 TypeScript ScreenComponent 类型匹配
/// </remarks>
public class ScreenComponentFrontendDto
{
    /// <summary>
    /// 组件ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型（前端用 type）
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 位置（前端用 position 对象）
    /// </summary>
    public ComponentPositionDto Position { get; set; } = new();

    /// <summary>
    /// 尺寸（前端用 size 对象）
    /// </summary>
    public ComponentSizeDto Size { get; set; } = new();

    /// <summary>
    /// 旋转角度
    /// </summary>
    public int? Rotation { get; set; }

    /// <summary>
    /// 是否锁定
    /// </summary>
    public bool? Locked { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool? Visible { get; set; }

    /// <summary>
    /// 数据源配置（已解析为对象）
    /// </summary>
    public object? DataSource { get; set; }

    /// <summary>
    /// 组件配置（已解析为对象）
    /// </summary>
    public object? Config { get; set; }

    /// <summary>
    /// 样式配置（已解析为对象）
    /// </summary>
    public object? Style { get; set; }

    /// <summary>
    /// 数据绑定配置（已解析为对象）
    /// </summary>
    public object? DataBinding { get; set; }
}

/// <summary>
/// 组件位置 DTO
/// </summary>
public class ComponentPositionDto
{
    /// <summary>
    /// X坐标
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Y坐标
    /// </summary>
    public int Y { get; set; }
}

/// <summary>
/// 组件尺寸 DTO
/// </summary>
public class ComponentSizeDto
{
    /// <summary>
    /// 宽度
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 高度
    /// </summary>
    public int Height { get; set; }
}