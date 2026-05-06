using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Basic.IService;

/// <summary>
/// 字典服务接口
/// </summary>
/// <remarks>
/// 提供字典类型和字典数据的管理功能
/// </remarks>
public interface IDictService
{
    #region 字典类型

    /// <summary>
    /// 获取字典类型分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页字典类型列表</returns>
    Task<PageResponse<DictTypeDto>> GetDictTypePageListAsync(QueryDictTypeDto query);

    /// <summary>
    /// 获取所有字典类型列表（不分页）
    /// </summary>
    /// <returns>字典类型列表</returns>
    Task<List<DictTypeDto>> GetDictTypeListAsync();

    /// <summary>
    /// 根据ID获取字典类型详情
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <returns>字典类型信息</returns>
    Task<DictTypeDto?> GetDictTypeByIdAsync(Guid id);

    /// <summary>
    /// 根据编码获取字典类型
    /// </summary>
    /// <param name="code">字典编码</param>
    /// <returns>字典类型信息</returns>
    Task<DictTypeDto?> GetDictTypeByCodeAsync(string code);

    /// <summary>
    /// 添加字典类型
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的字典类型ID</returns>
    Task<Guid> AddDictTypeAsync(AddDictTypeDto dto);

    /// <summary>
    /// 更新字典类型
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateDictTypeAsync(UpdateDictTypeDto dto);

    /// <summary>
    /// 删除字典类型
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteDictTypeAsync(Guid id);

    #endregion

    #region 字典数据

    /// <summary>
    /// 获取字典数据分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页字典数据列表</returns>
    Task<PageResponse<DictDataDto>> GetDictDataPageListAsync(QueryDictDataDto query);

    /// <summary>
    /// 根据ID获取字典数据详情
    /// </summary>
    /// <param name="id">字典数据ID</param>
    /// <returns>字典数据信息</returns>
    Task<DictDataDto?> GetDictDataByIdAsync(Guid id);

    /// <summary>
    /// 根据类型编码获取字典数据列表（用于下拉选项）
    /// </summary>
    /// <param name="code">字典类型编码</param>
    /// <returns>字典数据列表</returns>
    Task<List<DictDataDto>> GetDictDataByCodeAsync(string code);

    /// <summary>
    /// 添加字典数据
    /// </summary>
    /// <param name="dto">添加参数</param>
    /// <returns>新创建的字典数据ID</returns>
    Task<Guid> AddDictDataAsync(AddDictDataDto dto);

    /// <summary>
    /// 更新字典数据
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>影响的行数</returns>
    Task<int> UpdateDictDataAsync(UpdateDictDataDto dto);

    /// <summary>
    /// 删除字典数据
    /// </summary>
    /// <param name="id">字典数据ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteDictDataAsync(Guid id);

    /// <summary>
    /// 批量删除字典数据
    /// </summary>
    /// <param name="ids">字典数据ID列表</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteDictDataBatchAsync(List<Guid> ids);

    /// <summary>
    /// 批量获取字典数据（含版本信息）
    /// </summary>
    /// <param name="codes">字典编码列表</param>
    /// <returns>字典编码 -> 字典数据（含版本）</returns>
    Task<Dictionary<string, DictDataWithVersionDto>> GetDictDataBatchAsync(List<string> codes);

    /// <summary>
    /// 检查字典版本
    /// </summary>
    /// <param name="localVersions">本地版本号</param>
    /// <returns>版本检查结果</returns>
    Task<VersionCheckResponse> CheckDictVersionAsync(Dictionary<string, int> localVersions);

    /// <summary>
    /// 根据编码获取字典数据（含版本信息）
    /// </summary>
    /// <param name="code">字典编码</param>
    /// <returns>字典数据（含版本）</returns>
    Task<DictDataWithVersionDto?> GetDictDataWithVersionByCodeAsync(string code);

    #endregion
}