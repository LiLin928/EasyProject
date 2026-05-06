using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Http;

namespace BusinessManager.Buz.IService;

/// <summary>
/// 文件服务接口
/// </summary>
/// <remarks>
/// 提供文件上传、下载、删除和管理等功能。
/// 使用MinIO对象存储进行文件存储，支持单文件和批量上传。
/// 支持预签名URL生成，实现安全的临时访问。
/// </remarks>
public interface IFileService
{
    /// <summary>
    /// 上传单个文件
    /// </summary>
    /// <param name="file">上传的文件，来自HTTP请求的IFormFile</param>
    /// <param name="userId">上传用户ID，用于记录上传者</param>
    /// <param name="businessId">业务ID，可选参数，用于关联业务记录</param>
    /// <returns>上传结果，包含文件ID、URL等信息</returns>
    Task<FileUploadResultDto> UploadAsync(IFormFile file, Guid userId, Guid? businessId = null);

    /// <summary>
    /// 批量上传文件
    /// </summary>
    /// <param name="files">上传的文件列表</param>
    /// <param name="userId">上传用户ID</param>
    /// <returns>上传结果列表，包含每个文件的上传结果</returns>
    Task<List<FileUploadResultDto>> UploadBatchAsync(List<IFormFile> files, Guid userId);

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="id">文件记录ID</param>
    /// <returns>文件数据流，文件不存在或已删除返回null</returns>
    Task<Stream?> DownloadAsync(Guid id);

    /// <summary>
    /// 获取文件的预签名URL
    /// </summary>
    /// <param name="id">文件记录ID</param>
    /// <param name="expireSeconds">URL有效时间（秒），默认为3600秒（1小时）</param>
    /// <returns>预签名URL，文件不存在返回null</returns>
    Task<string?> GetPresignedUrlAsync(Guid id, int expireSeconds = 3600);

    /// <summary>
    /// 获取文件记录分页列表
    /// </summary>
    /// <param name="pageIndex">页码索引，从1开始</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="userId">上传用户ID筛选，可选</param>
    /// <param name="businessId">业务ID筛选，可选</param>
    /// <returns>分页后的文件记录列表</returns>
    Task<PageResponse<FileRecordDto>> GetPageListAsync(int pageIndex, int pageSize, Guid? userId = null, Guid? businessId = null);

    /// <summary>
    /// 删除文件记录
    /// </summary>
    /// <param name="id">要删除的文件记录ID</param>
    /// <returns>影响的行数</returns>
    Task<int> DeleteAsync(Guid id);

    /// <summary>
    /// 更新文件的业务ID
    /// </summary>
    /// <param name="fileIds">文件ID列表</param>
    /// <param name="businessId">业务ID</param>
    /// <returns>更新的文件数量</returns>
    Task<int> UpdateBusinessIdAsync(List<Guid> fileIds, Guid businessId);
}