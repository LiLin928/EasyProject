namespace EasyWeChatModels.Dto;

/// <summary>
/// 添加操作日志请求 DTO
/// </summary>
/// <remarks>
/// 用于创建新的操作日志记录时提交的数据。
/// 通常由系统自动记录，不需要手动调用。
/// </remarks>
public class AddOperateLogDto
{
    /// <summary>
    /// 操作用户ID
    /// </summary>
    /// <remarks>
    /// 执行操作的用户ID，可选参数。
    /// 未登录操作时可为空。
    /// </remarks>
    /// <example>550e8400-e29b-41d4-a716-446655440000</example>
    public Guid? UserId { get; set; }

    /// <summary>
    /// 操作用户名
    /// </summary>
    /// <remarks>
    /// 执行操作的用户名称。
    /// </remarks>
    /// <example>admin</example>
    public string? UserName { get; set; }

    /// <summary>
    /// 操作模块
    /// </summary>
    /// <remarks>
    /// 操作所属的功能模块名称。
    /// </remarks>
    /// <example>用户管理</example>
    public string? Module { get; set; }

    /// <summary>
    /// 操作动作
    /// </summary>
    /// <remarks>
    /// 具体的操作动作描述。
    /// </remarks>
    /// <example>新增用户</example>
    public string? Action { get; set; }

    /// <summary>
    /// 请求方法
    /// </summary>
    /// <remarks>
    /// HTTP请求方法类型。
    /// </remarks>
    /// <example>POST</example>
    public string? Method { get; set; }

    /// <summary>
    /// 请求地址
    /// </summary>
    /// <remarks>
    /// API请求的URL地址。
    /// </remarks>
    /// <example>/api/user/add</example>
    public string? Url { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    /// <remarks>
    /// 操作请求来源的IP地址。
    /// </remarks>
    /// <example>192.168.1.100</example>
    public string? Ip { get; set; }

    /// <summary>
    /// 操作地点
    /// </summary>
    /// <remarks>
    /// 根据IP地址解析的地理位置信息。
    /// </remarks>
    /// <example>北京市朝阳区</example>
    public string? Location { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    /// <remarks>
    /// API请求的参数内容，JSON格式存储。
    /// </remarks>
    /// <example>{"userName":"test"}</example>
    public string? Params { get; set; }

    /// <summary>
    /// 操作结果
    /// </summary>
    /// <remarks>
    /// 操作的返回结果内容，JSON格式存储。
    /// </remarks>
    /// <example>{"code":200}</example>
    public string? Result { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    /// <remarks>
    /// 操作执行的状态：1-成功，0-失败。默认值为1。
    /// </remarks>
    /// <example>1</example>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 错误信息
    /// </summary>
    /// <remarks>
    /// 操作失败时的错误信息内容。
    /// </remarks>
    /// <example>用户名已存在</example>
    public string? ErrorMsg { get; set; }

    /// <summary>
    /// 执行时长（毫秒）
    /// </summary>
    /// <remarks>
    /// API请求的耗时，单位为毫秒。
    /// </remarks>
    /// <example>156</example>
    public long? Duration { get; set; }
}