using SqlSugar;

namespace EasyWeChatModels.Entitys;

/// <summary>
/// 操作日志实体类，记录用户操作行为
/// </summary>
/// <remarks>
/// 用于记录用户在系统中的所有操作行为，包括操作时间、操作人、操作内容、请求参数等。
/// 支持按模块、用户、时间范围查询，便于审计追溯。
///
/// 使用 SqlSugar SplitTable 按天分表，每天一张表，表名格式：OperateLog_20260407
/// </remarks>
[SugarTable("OperateLog_{year}{month}{day}", "操作日志表")]
[SplitTable(SplitType.Day)]  // 按天分表
public class OperateLog
{
    /// <summary>
    /// 日志ID（主键）
    /// </summary>
    /// <remarks>
    /// 操作日志的唯一标识，自动生成Guid，用于查询和删除操作日志记录。
    /// </remarks>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [SugarColumn(IsPrimaryKey = true, ColumnDescription = "日志ID")]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 操作用户ID
    /// </summary>
    /// <remarks>
    /// 关联 User 表的 Id 字段，记录执行操作的用户。
    /// 当用户未登录时可为空，如登录失败等场景。
    /// </remarks>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [SugarColumn(IsNullable = true, ColumnDescription = "操作用户ID")]
    public Guid? UserId { get; set; }

    /// <summary>
    /// 操作用户名
    /// </summary>
    /// <remarks>
    /// 执行操作的用户名称，冗余存储便于查询显示。
    /// 最大长度50字符。
    /// </remarks>
    /// <example>admin</example>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "操作用户名")]
    public string? UserName { get; set; }

    /// <summary>
    /// 操作模块
    /// </summary>
    /// <remarks>
    /// 操作所属的功能模块名称，用于分类统计和筛选。
    /// 如：用户管理、角色管理、菜单管理等。
    /// 最大长度100字符。
    /// </remarks>
    /// <example>用户管理</example>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "操作模块")]
    public string? Module { get; set; }

    /// <summary>
    /// 操作动作
    /// </summary>
    /// <remarks>
    /// 具体的操作动作描述，如新增、编辑、删除、查询等。
    /// 最大长度100字符。
    /// </remarks>
    /// <example>新增用户</example>
    [SugarColumn(Length = 100, IsNullable = true, ColumnDescription = "操作动作")]
    public string? Action { get; set; }

    /// <summary>
    /// 请求方法（GET/POST/PUT/DELETE）
    /// </summary>
    /// <remarks>
    /// HTTP请求方法类型，用于区分不同类型的API调用。
    /// 最大长度20字符。
    /// </remarks>
    /// <example>POST</example>
    [SugarColumn(Length = 20, IsNullable = true, ColumnDescription = "请求方法")]
    public string? Method { get; set; }

    /// <summary>
    /// 请求地址
    /// </summary>
    /// <remarks>
    /// API请求的URL地址，包含路径部分，不含域名。
    /// 最大长度500字符。
    /// </remarks>
    /// <example>/api/user/add</example>
    [SugarColumn(Length = 500, IsNullable = true, ColumnDescription = "请求地址")]
    public string? Url { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    /// <remarks>
    /// 操作请求来源的IP地址，用于安全审计和地域分析。
    /// 最大长度50字符，支持IPv4和IPv6格式。
    /// </remarks>
    /// <example>192.168.1.100</example>
    [SugarColumn(Length = 50, IsNullable = true, ColumnDescription = "IP地址")]
    public string? Ip { get; set; }

    /// <summary>
    /// 操作地点
    /// </summary>
    /// <remarks>
    /// 根据IP地址解析的地理位置信息，如城市、区域等。
    /// 最大长度200字符。
    /// </remarks>
    /// <example>北京市朝阳区</example>
    [SugarColumn(Length = 200, IsNullable = true, ColumnDescription = "操作地点")]
    public string? Location { get; set; }

    /// <summary>
    /// 请求参数（JSON格式）
    /// </summary>
    /// <remarks>
    /// API请求的参数内容，JSON格式存储。
    /// 用于问题排查和操作审计，敏感信息应脱敏处理。
    /// </remarks>
    /// <example>{"userName":"test","realName":"测试用户"}</example>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "请求参数")]
    public string? Params { get; set; }

    /// <summary>
    /// 操作结果
    /// </summary>
    /// <remarks>
    /// 操作的返回结果内容，JSON格式存储。
    /// 用于问题排查和操作审计。
    /// </remarks>
    /// <example>{"code":200,"message":"操作成功","data":1}</example>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "操作结果")]
    public string? Result { get; set; }

    /// <summary>
    /// 状态（1=成功 0=失败）
    /// </summary>
    /// <remarks>
    /// 操作执行的状态标识。
    /// 1表示操作成功执行，0表示操作失败（如异常、权限不足等）。
    /// 默认值为1。
    /// </remarks>
    /// <example>1</example>
    [SugarColumn(ColumnDescription = "状态：1=成功，0=失败")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 错误信息
    /// </summary>
    /// <remarks>
    /// 操作失败时的错误信息内容，用于问题排查。
    /// 操作成功时为空。
    /// </remarks>
    /// <example>用户名已存在</example>
    [SugarColumn(ColumnDataType = "text", IsNullable = true, ColumnDescription = "错误信息")]
    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    /// <remarks>
    /// API请求从开始到结束的耗时，单位为毫秒。
    /// 用于性能分析和慢请求排查。
    /// </remarks>
    /// <example>156</example>
    [SugarColumn(IsNullable = true, ColumnDescription = "执行时长（毫秒）")]
    public long? Duration { get; set; }

    /// <summary>
    /// 操作时间
    /// </summary>
    /// <remarks>
    /// 操作发生的时间，默认为记录创建时间。
    /// 用于时间范围查询和排序。
    /// 此字段作为分表依据，数据会根据此字段存入对应日期的分表。
    /// </remarks>
    /// <example>2024-01-15 10:30:00</example>
    [SugarColumn(ColumnDescription = "操作时间")]
    [SplitField]  // 分表字段
    public DateTime? CreateTime { get; set; }
}