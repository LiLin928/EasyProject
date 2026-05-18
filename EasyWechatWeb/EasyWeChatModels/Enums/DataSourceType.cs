namespace EasyWeChatModels.Enums;

/// <summary>
/// 数据源类型枚举
/// </summary>
public enum DataSourceType
{
    /// <summary>API接口（调用后端API）</summary>
    Api = 1,

    /// <summary>静态配置（直接返回配置数据）</summary>
    Static = 2,

    /// <summary>SQL执行（直接执行SQL语句）</summary>
    Sql = 3,

    /// <summary>报表数据（从已发布报表获取）</summary>
    Report = 4
}