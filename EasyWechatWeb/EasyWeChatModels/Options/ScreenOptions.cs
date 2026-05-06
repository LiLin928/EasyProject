namespace EasyWeChatModels.Options;

/// <summary>
/// 大屏模块配置选项
/// </summary>
public class ScreenOptions
{
    /// <summary>
    /// 是否使用Mock数据
    /// </summary>
    public bool IsUseMock { get; set; } = true;

    /// <summary>
    /// Mock数据源列表
    /// </summary>
    public List<MockDatasourceConfig> MockDatasources { get; set; } = new()
    {
        new MockDatasourceConfig { Id = "mock-mysql-1", Name = "模拟MySQL数据源", Type = "mysql" },
        new MockDatasourceConfig { Id = "mock-postgres-1", Name = "模拟PostgreSQL数据源", Type = "postgresql" },
        new MockDatasourceConfig { Id = "mock-api-1", Name = "模拟API数据源", Type = "api" }
    };
}

/// <summary>
/// Mock数据源配置
/// </summary>
public class MockDatasourceConfig
{
    /// <summary>
    /// 数据源唯一标识
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 数据源显示名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 数据源类型（mysql、postgresql、api）
    /// </summary>
    public string Type { get; set; } = string.Empty;
}