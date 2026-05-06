using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 数据源服务接口
/// </summary>
public interface IDatasourceService
{
    /// <summary>
    /// 获取数据源列表（分页）
    /// </summary>
    Task<PageResponse<DatasourceDto>> GetPageListAsync(QueryDatasourceDto query);

    /// <summary>
    /// 获取数据源详情
    /// </summary>
    Task<DatasourceDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 获取所有数据源（下拉选择用）
    /// </summary>
    Task<List<DatasourceDto>> GetAllAsync();

    /// <summary>
    /// 添加数据源
    /// </summary>
    Task<Guid> AddAsync(CreateDatasourceDto dto, Guid creatorId);

    /// <summary>
    /// 更新数据源
    /// </summary>
    Task<int> UpdateAsync(UpdateDatasourceDto dto);

    /// <summary>
    /// 删除数据源
    /// </summary>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 测试连接
    /// </summary>
    Task<TestConnectionResultDto> TestConnectionAsync(Guid id);

    /// <summary>
    /// 测试连接（按配置，用于未保存的数据源）
    /// </summary>
    Task<TestConnectionResultDto> TestConnectionByConfigAsync(CreateDatasourceDto dto);

    /// <summary>
    /// 获取支持的数据库类型
    /// </summary>
    Task<List<DbTypeInfoDto>> GetDbTypesAsync();

    /// <summary>
    /// 测试查询
    /// </summary>
    Task<TestQueryResultDto> TestQueryAsync(Guid id, string sql);
}

/// <summary>
/// 测试查询结果DTO
/// </summary>
public class TestQueryResultDto
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public List<object> Data { get; set; } = new();

    /// <summary>
    /// 列名
    /// </summary>
    public List<string> Columns { get; set; } = new();

    /// <summary>
    /// 行数
    /// </summary>
    public int RowCount { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? Error { get; set; }
}