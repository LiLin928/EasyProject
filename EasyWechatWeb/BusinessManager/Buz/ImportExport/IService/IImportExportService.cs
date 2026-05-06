using EasyWeChatModels.Dto;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 数据导入导出服务接口
/// </summary>
/// <remarks>
/// 提供用户和角色的数据导入导出功能。
/// 支持 Excel 格式文件 (.xlsx) 的读取和生成。
/// </remarks>
public interface IImportExportService
{
    /// <summary>
    /// 导出用户数据
    /// </summary>
    /// <param name="userIds">要导出的用户ID列表，为空时导出全部用户</param>
    /// <returns>Excel 文件的字节数组，可直接用于 HTTP 文件响应</returns>
    Task<byte[]> ExportUsersAsync(List<Guid>? userIds = null);

    /// <summary>
    /// 导入用户数据
    /// </summary>
    /// <param name="fileData">Excel 文件的字节数组，来自上传文件</param>
    /// <param name="operatorId">操作用户ID，用于记录操作来源</param>
    /// <returns>导入结果 DTO，包含成功数量、失败数量和错误详情</returns>
    Task<ImportResultDto> ImportUsersAsync(byte[] fileData, Guid operatorId);

    /// <summary>
    /// 导出角色数据
    /// </summary>
    /// <param name="roleIds">要导出的角色ID列表，为空时导出全部角色</param>
    /// <returns>Excel 文件的字节数组，可直接用于 HTTP 文件响应</returns>
    Task<byte[]> ExportRolesAsync(List<Guid>? roleIds = null);

    /// <summary>
    /// 导入角色数据
    /// </summary>
    /// <param name="fileData">Excel 文件的字节数组，来自上传文件</param>
    /// <param name="operatorId">操作用户ID，用于记录操作来源</param>
    /// <returns>导入结果 DTO，包含成功数量、失败数量和错误详情</returns>
    Task<ImportResultDto> ImportRolesAsync(byte[] fileData, Guid operatorId);

    /// <summary>
    /// 获取导入模板
    /// </summary>
    /// <param name="type">模板类型：User-用户模板，Role-角色模板</param>
    /// <returns>Excel 模板文件的字节数组</returns>
    Task<byte[]> GetImportTemplateAsync(string type);
}