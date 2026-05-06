using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Error;
using CommonManager.Helper;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using EasyWeChatModels.Options;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Buz.Service;

/// <summary>
/// 文件服务实现类
/// </summary>
/// <remarks>
/// 实现文件相关的业务逻辑，包括文件上传、下载、删除和管理等功能。
/// 继承自<see cref="BaseService{FileRecord}"/>，使用SqlSugar进行数据库操作。
/// 使用 IFileStorageHelper 接口进行文件存储操作，支持 MinIO 和本地存储切换。
/// </remarks>
public class FileService : BaseService<FileRecord>, IFileService
{
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<FileService> _logger { get; set; } = null!;
    /// <summary>
    /// 配置帮助类
    /// </summary>
    public AppSettingHelper _appSetting { get; set; } = null!;
    /// <summary>
    /// 文件存储帮助类（支持 MinIO 和本地存储）
    /// </summary>
    public IFileStorageHelper _storageHelper { get; set; } = null!;
    /// <summary>
    /// MinIO配置选项（兼容旧代码）
    /// </summary>
    public MinioOption _minioOption => _appSetting.GetSection<MinioOption>("MinioOption");
    /// <summary>
    /// 本地存储配置选项
    /// </summary>
    public LocalStorageOption _localStorageOption => _appSetting.GetSection<LocalStorageOption>("LocalStorageOption");

    /// <summary>
    /// 单文件最大大小（50MB）
    /// </summary>
    private const long MaxFileSize = 50 * 1024 * 1024;

    /// <summary>
    /// 批量上传最大文件数
    /// </summary>
    private const int MaxBatchCount = 10;

    /// <summary>
    /// 允许的文件扩展名列表
    /// </summary>
    private static readonly string[] AllowedExtensions = new[]
    {
        // 图片类型
        "jpg", "jpeg", "png", "gif", "bmp", "webp", "svg",
        // 文档类型
        "pdf", "doc", "docx", "xls", "xlsx", "ppt", "pptx", "txt",
        // 音视频类型
        "mp3", "mp4", "avi", "mov", "wav",
        // 压缩文件
        "zip", "rar", "7z"
    };

    /// <summary>
    /// 上传单个文件
    /// </summary>
    /// <param name="file">上传的文件，来自HTTP请求的IFormFile</param>
    /// <param name="userId">上传用户ID，用于记录上传者</param>
    /// <param name="businessId">业务ID，可选参数，用于关联业务记录</param>
    /// <returns>上传结果，包含文件ID、URL等信息</returns>
    /// <remarks>
    /// 上传流程：
    /// <list type="number">
    ///     <item>验证文件是否为空</item>
    ///     <item>验证文件大小是否符合限制（最大50MB）</item>
    ///     <item>验证文件扩展名是否在允许列表中</item>
    ///     <item>生成唯一的对象名称（格式：file/年/月/日/UUID.扩展名）</item>
    ///     <item>使用MinioHelper上传文件到对象存储</item>
    ///     <item>创建文件记录保存到数据库</item>
    ///     <item>生成预签名URL并返回上传结果</item>
    /// </list>
    /// 注意：businessId 为空时，文件不会关联业务，后续需要调用 UpdateBusinessIdAsync 关联。
    /// </remarks>
    public async Task<FileUploadResultDto> UploadAsync(IFormFile file, Guid userId, Guid? businessId = null)
    {
        _logger.LogInformation("开始上传文件，用户：{UserId}, 业务ID：{BusinessId}, 文件名：{FileName}",
            userId, businessId?.ToString() ?? "null", file?.FileName);

        var result = new FileUploadResultDto
        {
            FileName = file?.FileName ?? "",
            Success = false
        };

        try
        {
            // 验证文件是否为空
            if (file == null || file.Length == 0)
            {
                result.ErrorMessage = "文件不能为空";
                _logger.LogWarning("上传失败：文件为空");
                return result;
            }

            // 验证文件大小
            if (file.Length > MaxFileSize)
            {
                result.ErrorMessage = $"文件大小超出限制，最大允许{MaxFileSize / (1024 * 1024)}MB";
                _logger.LogWarning("上传失败：文件大小超出限制，大小：{FileSize}", file.Length);
                return result;
            }

            // 获取文件扩展名
            var fileExt = GetFileExtension(file.FileName);
            if (string.IsNullOrEmpty(fileExt))
            {
                result.ErrorMessage = "无法识别文件类型";
                _logger.LogWarning("上传失败：无法识别文件类型");
                return result;
            }

            // 验证文件扩展名
            if (!IsAllowedExtension(fileExt))
            {
                result.ErrorMessage = $"不允许上传该类型的文件（.{fileExt}）";
                _logger.LogWarning("上传失败：不允许的文件类型：{FileExt}", fileExt);
                return result;
            }

            // 获取内容类型
            var contentType = file.ContentType;
            if (string.IsNullOrEmpty(contentType))
            {
                contentType = GetContentType(fileExt);
            }

            // 生成对象名称
            var objectName = GenerateObjectName(fileExt);

            // 获取存储帮助类
            var storageHelper = GetStorageHelper();

            // MinIO 特有操作：确保桶存在并设置公开策略
            if (storageHelper is MinioHelper minioStorage)
            {
                if (!await minioStorage.BucketExistsAsync(_minioOption.BucketName))
                {
                    await minioStorage.MakeBucketAsync(_minioOption.BucketName);
                    _logger.LogInformation("存储桶 {BucketName} 已创建", _minioOption.BucketName);
                }
                await minioStorage.SetBucketPublicAsync(_minioOption.BucketName);
            }

            // 上传文件
            using var stream = file.OpenReadStream();
            var uploadSuccess = await storageHelper.UploadAsync(objectName, stream, contentType);

            if (!uploadSuccess)
            {
                result.ErrorMessage = "文件上传失败";
                _logger.LogError("上传失败：存储上传失败");
                return result;
            }

            // 获取文件URL
            var fileUrl = storageHelper.GetFileUrl(objectName);

            // 获取桶名称（本地存储使用配置中的目录名）
            var bucketName = storageHelper is MinioHelper
                ? _minioOption.BucketName
                : _localStorageOption.RootPath;

            // 创建文件记录
            var fileRecord = new FileRecord
            {
                FileName = file.FileName,
                FilePath = objectName,
                FileSize = file.Length,
                FileExt = fileExt,
                ContentType = contentType,
                BucketName = bucketName,
                ObjectName = objectName,
                Url = fileUrl,
                UserId = userId,
                BusinessId = businessId,
                Status = 1,
                CreateTime = DateTime.Now
            };

            // 保存到数据库
            var id = await InsertAsync(fileRecord);

            // 获取预签名URL
            var presignedUrl = await storageHelper.GetPresignedUrlAsync(objectName, 3600);

            result.Id = id;
            result.FileSize = file.Length;
            result.FileExt = fileExt;
            result.ContentType = contentType;
            result.Url = fileUrl;
            result.PresignedUrl = presignedUrl;
            result.Success = true;

            _logger.LogInformation("文件上传成功，ID：{Id}, 对象名：{ObjectName}", id, objectName);

            return result;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"上传异常：{ex.Message}";
            _logger.LogError(ex, "文件上传异常，文件名：{FileName}", file?.FileName);
            return result;
        }
    }

    /// <summary>
    /// 批量上传文件
    /// </summary>
    /// <param name="files">上传的文件列表</param>
    /// <param name="userId">上传用户ID</param>
    /// <returns>上传结果列表，包含每个文件的上传结果</returns>
    /// <remarks>
    /// 批量上传流程：
    /// <list type="number">
    ///     <item>验证文件列表数量是否超出限制（最多10个）</item>
    ///     <item>遍历文件列表，逐个调用UploadAsync上传</item>
    ///     <item>收集所有上传结果返回</item>
    /// </list>
    /// 即使某些文件上传失败，其他文件仍会继续上传。
    /// </remarks>
    public async Task<List<FileUploadResultDto>> UploadBatchAsync(List<IFormFile> files, Guid userId)
    {
        _logger.LogInformation("开始批量上传文件，用户：{UserId}, 文件数：{Count}",
            userId, files?.Count ?? 0);

        var results = new List<FileUploadResultDto>();

        // 验证文件列表
        if (files == null || files.Count == 0)
        {
            _logger.LogWarning("批量上传失败：文件列表为空");
            return results;
        }

        // 验证文件数量
        if (files.Count > MaxBatchCount)
        {
            _logger.LogWarning("批量上传失败：文件数量超出限制，数量：{Count}", files.Count);
            throw BusinessException.BadRequest($"批量上传最多允许{MaxBatchCount}个文件");
        }

        // 逐个上传文件
        foreach (var file in files)
        {
            var result = await UploadAsync(file, userId);
            results.Add(result);
        }

        var successCount = results.Count(r => r.Success);
        _logger.LogInformation("批量上传完成，总数：{Total}, 成功：{Success}, 失败：{Failed}",
            files.Count, successCount, files.Count - successCount);

        return results;
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="id">文件记录ID</param>
    /// <returns>文件数据流，文件不存在或已删除返回null</returns>
    /// <remarks>
    /// 下载流程：
    /// <list type="number">
    ///     <item>查询文件记录是否存在且状态正常</item>
    ///     <item>使用MinioHelper从对象存储下载文件流</item>
    ///     <item>返回MemoryStream供调用方处理</item>
    /// </list>
    /// 返回的Stream需要调用方手动释放。
    /// </remarks>
    public async Task<Stream?> DownloadAsync(Guid id)
    {
        _logger.LogInformation("开始下载文件，ID：{Id}", id);

        // 查询文件记录
        var fileRecord = await _db.Queryable<FileRecord>()
            .Where(f => f.Id == id && f.Status == 1)
            .FirstAsync();

        if (fileRecord == null)
        {
            _logger.LogWarning("下载失败：文件不存在或已删除，ID：{Id}", id);
            return null;
        }

        // 获取存储帮助类并下载
        var storageHelper = GetStorageHelper();
        var stream = await storageHelper.DownloadAsync(fileRecord.ObjectName);

        if (stream == null)
        {
            _logger.LogError("下载失败：存储下载失败，对象名：{ObjectName}", fileRecord.ObjectName);
            return null;
        }

        _logger.LogInformation("文件下载成功，ID：{Id}, 文件名：{FileName}", id, fileRecord.FileName);

        return stream;
    }

    /// <summary>
    /// 获取文件的预签名URL
    /// </summary>
    /// <param name="id">文件记录ID</param>
    /// <param name="expireSeconds">URL有效时间（秒），默认为3600秒（1小时）</param>
    /// <returns>预签名URL，文件不存在返回null</returns>
    /// <remarks>
    /// 预签名URL流程：
    /// <list type="number">
    ///     <item>查询文件记录是否存在</item>
    ///     <item>使用MinioHelper生成预签名URL</item>
    ///     <item>返回带有签名信息的临时访问URL</item>
    /// </list>
    /// 预签名URL有过期时间限制，到期后无法访问。
    /// </remarks>
    public async Task<string?> GetPresignedUrlAsync(Guid id, int expireSeconds = 3600)
    {
        _logger.LogInformation("开始获取预签名URL，ID：{Id}, 有效期：{ExpireSeconds}秒", id, expireSeconds);

        // 查询文件记录
        var fileRecord = await _db.Queryable<FileRecord>()
            .Where(f => f.Id == id && f.Status == 1)
            .FirstAsync();

        if (fileRecord == null)
        {
            _logger.LogWarning("获取预签名URL失败：文件不存在或已删除，ID：{Id}", id);
            return null;
        }

        // 获取存储帮助类并获取预签名URL
        var storageHelper = GetStorageHelper();
        var url = await storageHelper.GetPresignedUrlAsync(fileRecord.ObjectName, expireSeconds);

        if (url == null)
        {
            _logger.LogError("获取预签名URL失败：存储操作失败，对象名：{ObjectName}", fileRecord.ObjectName);
            return null;
        }

        _logger.LogInformation("预签名URL获取成功，ID：{Id}", id);

        return url;
    }

    /// <summary>
    /// 获取文件记录分页列表
    /// </summary>
    /// <param name="pageIndex">页码索引，从1开始</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="userId">上传用户ID筛选，可选</param>
    /// <param name="businessId">业务ID筛选，可选</param>
    /// <returns>分页后的文件记录列表</returns>
    /// <remarks>
    /// 查询流程：
    /// <list type="number">
    ///     <item>根据筛选条件构建查询表达式</item>
    ///     <item>默认只查询状态为正常的文件</item>
    ///     <item>按创建时间倒序排序</item>
    ///     <item>执行分页查询获取总数和数据列表</item>
    ///     <item>使用Mapster将实体转换为DTO</item>
    ///     <item>构建分页响应对象返回</item>
    /// </list>
    /// </remarks>
    public async Task<PageResponse<FileRecordDto>> GetPageListAsync(int pageIndex, int pageSize, Guid? userId = null, Guid? businessId = null)
    {
        _logger.LogInformation("开始查询文件列表，页码：{PageIndex}, 每页：{PageSize}, 用户：{UserId}, 业务ID：{BusinessId}",
            pageIndex, pageSize, userId, businessId);

        var total = new RefAsync<int>();
        var list = await _db.Queryable<FileRecord>()
            .Where(f => f.Status == 1)
            .WhereIF(userId.HasValue, f => f.UserId == userId!.Value)
            .WhereIF(businessId.HasValue, f => f.BusinessId == businessId!.Value)
            .OrderByDescending(f => f.CreateTime)
            .ToPageListAsync(pageIndex, pageSize, total);

        var dtoList = list.Adapt<List<FileRecordDto>>();

        _logger.LogInformation("文件列表查询完成，总数：{Total}", total.Value);

        return PageResponse<FileRecordDto>.Create(dtoList, total.Value, pageIndex, pageSize);
    }

    /// <summary>
    /// 删除文件记录
    /// </summary>
    /// <param name="id">要删除的文件记录ID</param>
    /// <returns>影响的行数</returns>
    /// <exception cref="BusinessException">
    /// 文件不存在时抛出NotFound异常
    /// </exception>
    /// <remarks>
    /// 删除流程：
    /// <list type="number">
    ///     <item>查询文件记录是否存在</item>
    ///     <item>不存在则抛出BusinessException.NotFound异常</item>
    ///     <item>使用MinioHelper从对象存储删除文件</item>
    ///     <item>更新文件记录状态为已删除(Status=0)</item>
    /// </list>
    /// 这是逻辑删除，文件记录仍保留在数据库中。
    /// 对象存储中的文件会被物理删除。
    /// </remarks>
    public new async Task<int> DeleteAsync(Guid id)
    {
        _logger.LogInformation("开始删除文件，ID：{Id}", id);

        // 查询文件记录
        var fileRecord = await _db.Queryable<FileRecord>()
            .Where(f => f.Id == id)
            .FirstAsync();

        if (fileRecord == null)
        {
            _logger.LogWarning("删除失败：文件不存在，ID：{Id}", id);
            throw BusinessException.NotFound("文件不存在");
        }

        // 从存储删除文件
        var storageHelper = GetStorageHelper();
        var deleteSuccess = await storageHelper.RemoveAsync(fileRecord.ObjectName);

        if (!deleteSuccess)
        {
            _logger.LogWarning("存储删除失败，对象名：{ObjectName}，继续更新数据库状态", fileRecord.ObjectName);
        }

        // 更新状态为已删除
        var result = await _db.Updateable<FileRecord>()
            .SetColumns(f => f.Status == 0)
            .Where(f => f.Id == id)
            .ExecuteCommandAsync();

        _logger.LogInformation("文件删除成功，ID：{Id}, 影响行数：{Result}", id, result);

        return result;
    }

    /// <summary>
    /// 更新文件的业务ID
    /// </summary>
    /// <param name="fileIds">文件ID列表</param>
    /// <param name="businessId">业务ID</param>
    /// <returns>更新的文件数量</returns>
    /// <remarks>
    /// 批量更新文件的 BusinessId，用于将上传的文件关联到具体的业务记录。
    /// 例如：先上传附件，再创建公告，创建后将附件关联到公告ID。
    /// </remarks>
    public async Task<int> UpdateBusinessIdAsync(List<Guid> fileIds, Guid businessId)
    {
        if (fileIds == null || fileIds.Count == 0)
        {
            return 0;
        }

        _logger.LogInformation("开始更新文件业务ID，文件数：{Count}, 业务ID：{BusinessId}",
            fileIds.Count, businessId);

        var result = await _db.Updateable<FileRecord>()
            .SetColumns(f => new FileRecord { BusinessId = businessId })
            .Where(f => fileIds.Contains(f.Id) && f.Status == 1)
            .ExecuteCommandAsync();

        _logger.LogInformation("文件业务ID更新完成，影响行数：{Result}", result);
        return result;
    }

    /// <summary>
    /// 获取文件存储帮助类实例
    /// </summary>
    /// <returns>IFileStorageHelper实例</returns>
    /// <remarks>
    /// 直接返回注入的存储帮助类，支持 MinIO 和本地存储。
    /// </remarks>
    private IFileStorageHelper GetStorageHelper()
    {
        return _storageHelper;
    }

    /// <summary>
    /// 生成唯一的对象名称
    /// </summary>
    /// <param name="fileExt">文件扩展名</param>
    /// <returns>生成的对象名称</returns>
    /// <remarks>
    /// 格式：file/年/月/日/UUID.扩展名
    /// 例如：file/2024/01/15/uuid.pdf
    /// </remarks>
    private static string GenerateObjectName(string fileExt)
    {
        var now = DateTime.Now;
        var uuid = Guid.NewGuid().ToString("N");
        return $"file/{now.Year}/{now.Month:D2}/{now.Day:D2}/{uuid}.{fileExt}";
    }

    /// <summary>
    /// 获取文件扩展名
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <returns>文件扩展名（不含点号），无法获取返回空字符串</returns>
    /// <remarks>
    /// 从文件名中提取扩展名，去除点号。
    /// </remarks>
    private static string GetFileExtension(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            return string.Empty;

        var lastDotIndex = fileName.LastIndexOf('.');
        if (lastDotIndex < 0 || lastDotIndex == fileName.Length - 1)
            return string.Empty;

        return fileName.Substring(lastDotIndex + 1).ToLower();
    }

    /// <summary>
    /// 验证文件扩展名是否在允许列表中
    /// </summary>
    /// <param name="fileExt">文件扩展名</param>
    /// <returns>允许返回true，不允许返回false</returns>
    /// <remarks>
    /// 检查扩展名是否在安全文件类型白名单中。
    /// </remarks>
    private static bool IsAllowedExtension(string fileExt)
    {
        return AllowedExtensions.Contains(fileExt.ToLower());
    }

    /// <summary>
    /// 根据文件扩展名获取对应的 MIME Content-Type
    /// </summary>
    /// <param name="extension">文件扩展名（不含点号），如 "jpg"、"pdf"</param>
    /// <returns>对应的 MIME Content-Type</returns>
    /// <remarks>
    /// 支持常见的图片、文档、音视频等文件类型。
    /// 未识别的扩展名返回 "application/octet-stream"（通用二进制流）。
    /// </remarks>
    private static string GetContentType(string extension)
    {
        return extension.ToLower() switch
        {
            "jpg" or "jpeg" => "image/jpeg",
            "png" => "image/png",
            "gif" => "image/gif",
            "bmp" => "image/bmp",
            "webp" => "image/webp",
            "svg" => "image/svg+xml",
            "pdf" => "application/pdf",
            "doc" => "application/msword",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "xls" => "application/vnd.ms-excel",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ppt" => "application/vnd.ms-powerpoint",
            "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            "txt" => "text/plain",
            "html" => "text/html",
            "css" => "text/css",
            "js" => "application/javascript",
            "json" => "application/json",
            "xml" => "application/xml",
            "zip" => "application/zip",
            "rar" => "application/x-rar-compressed",
            "7z" => "application/x-7z-compressed",
            "mp3" => "audio/mpeg",
            "mp4" => "video/mp4",
            "avi" => "video/x-msvideo",
            "mov" => "video/quicktime",
            "wav" => "audio/wav",
            _ => "application/octet-stream"
        };
    }
}