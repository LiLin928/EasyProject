using BusinessManager.Buz.IService;
using CommonManager.Base;
using CommonManager.Helper;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.Buz;

/// <summary>
/// 文件控制器
/// </summary>
/// <remarks>
/// 提供文件上传、下载、删除和管理等API接口。
/// 支持单文件和批量上传，使用 IFileStorageHelper 接口支持多种存储方式。
/// 所有接口需要认证访问，通过JWT Token验证。
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FileController : BaseController
{
    /// <summary>
    /// 文件服务接口
    /// </summary>
    public IFileService _fileService { get; set; } = null!;
    /// <summary>
    /// 文件存储帮助类（支持 MinIO 和本地存储）
    /// </summary>
    public IFileStorageHelper _storageHelper { get; set; } = null!;
    /// <summary>
    /// 日志记录器
    /// </summary>
    public ILogger<FileController> _logger { get; set; } = null!;

    /// <summary>
    /// 通过路径获取媒体文件（公开接口，无需认证）
    /// 支持图片、视频等媒体类型
    /// </summary>
    /// <param name="path">文件路径（MinIO对象名称，如 product/2024/04/15/uuid.jpg）</param>
    /// <returns>文件内容</returns>
    /// <response code="200">返回文件内容</response>
    /// <response code="404">文件不存在</response>
    /// <remarks>
    /// 通过文件路径直接获取媒体文件，无需认证。
    /// 用于商品图片、视频、轮播图等公开资源的访问。
    /// 文件路径就是上传时返回的 Url 字段中的路径部分。
    /// 支持的格式：
    /// - 图片：jpg、jpeg、png、gif、bmp、webp、svg
    /// - 视频：mp4、avi、mov、wmv、flv、mkv、webm
    /// </remarks>
    /// <example>
    /// GET /api/file/image?path=product/2024/04/15/uuid.jpg
    /// GET /api/file/image?path=product/2024/04/15/uuid.mp4
    /// </example>
    [HttpGet("image")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FileStreamResult), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> GetImage([FromQuery] string path)
    {
        try
        {
            if (string.IsNullOrEmpty(path))
            {
                return NotFound(new ApiResponse<object>
                {
                    Code = 400,
                    Message = "文件路径不能为空",
                    Data = null
                });
            }

            // 使用注入的存储帮助类下载文件
            var stream = await _storageHelper.DownloadAsync(path);
            if (stream == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Code = 404,
                    Message = "文件不存在",
                    Data = null
                });
            }

            // 根据扩展名判断 Content-Type
            var contentType = GetContentTypeByPath(path);
            return File(stream, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取文件失败，路径：{Path}", path);
            return NotFound(new ApiResponse<object>
            {
                Code = 404,
                Message = "文件不存在",
                Data = null
            });
        }
    }

    /// <summary>
    /// 根据路径获取 Content-Type
    /// </summary>
    private static string GetContentTypeByPath(string path)
    {
        var ext = path.Split('.').LastOrDefault()?.ToLower() ?? "";
        return ext switch
        {
            // 图片类型
            "jpg" or "jpeg" => "image/jpeg",
            "png" => "image/png",
            "gif" => "image/gif",
            "bmp" => "image/bmp",
            "webp" => "image/webp",
            "svg" => "image/svg+xml",
            "ico" => "image/x-icon",
            "tiff" or "tif" => "image/tiff",
            // 视频类型
            "mp4" => "video/mp4",
            "avi" => "video/x-msvideo",
            "mov" => "video/quicktime",
            "wmv" => "video/x-ms-wmv",
            "flv" => "video/x-flv",
            "mkv" => "video/x-matroska",
            "webm" => "video/webm",
            "m4v" => "video/mp4",
            "3gp" => "video/3gpp",
            "mpeg" or "mpg" => "video/mpeg",
            // 音频类型
            "mp3" => "audio/mpeg",
            "wav" => "audio/wav",
            "ogg" => "audio/ogg",
            "m4a" => "audio/mp4",
            "flac" => "audio/flac",
            "aac" => "audio/aac",
            // 文档类型
            "pdf" => "application/pdf",
            "doc" => "application/msword",
            "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "xls" => "application/vnd.ms-excel",
            "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "ppt" => "application/vnd.ms-powerpoint",
            "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
            // 其他
            _ => "application/octet-stream"
        };
    }

    /// <summary>
    /// 上传单个文件
    /// </summary>
    /// <param name="file">上传的文件</param>
    /// <param name="businessId">业务ID，可选参数，用于关联业务记录</param>
    /// <returns>上传结果，包含文件ID、URL等信息</returns>
    /// <response code="200">上传成功，返回文件信息</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">上传失败，文件为空或不符合要求</response>
    /// <remarks>
    /// 上传单个文件到MinIO对象存储。
    /// 文件大小限制：最大50MB。
    /// 支持的文件类型：图片、文档、视频等常见类型。
    /// 上传成功后返回文件ID，可用于后续下载和管理。
    /// </remarks>
    /// <example>
    /// POST /api/file/upload?businessId=3fa85f64-...
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: multipart/form-data
    /// file: [文件内容]
    /// </example>
    [HttpPost("upload")]
    [ProducesResponseType(typeof(ApiResponse<FileUploadResultDto>), 200)]
    public async Task<ApiResponse<FileUploadResultDto>> Upload(
        IFormFile file,
        [FromQuery] Guid? businessId = null)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.UploadAsync(file, userId, businessId);

            if (!result.Success)
            {
                return Error<FileUploadResultDto>(result.ErrorMessage ?? "上传失败", 400);
            }

            return Success(result, "上传成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "文件上传失败");
            return Error<FileUploadResultDto>("文件上传失败");
        }
    }

    /// <summary>
    /// 批量上传文件
    /// </summary>
    /// <param name="files">上传的文件列表</param>
    /// <returns>上传结果列表，包含每个文件的上传结果</returns>
    /// <response code="200">上传完成，返回各文件的上传结果</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="400">上传失败，文件数量超出限制</response>
    /// <remarks>
    /// 批量上传多个文件到MinIO对象存储。
    /// 文件数量限制：最多10个。
    /// 返回结果中包含每个文件的上传状态，即使部分失败也会继续上传其他文件。
    /// </remarks>
    /// <example>
    /// POST /api/file/upload-batch
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: multipart/form-data
    /// files: [多个文件内容]
    /// </example>
    [HttpPost("upload-batch")]
    [ProducesResponseType(typeof(ApiResponse<List<FileUploadResultDto>>), 200)]
    public async Task<ApiResponse<List<FileUploadResultDto>>> UploadBatch(List<IFormFile> files)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _fileService.UploadBatchAsync(files, userId);
            var successCount = result.Count(r => r.Success);

            return Success(result, $"批量上传完成，成功{successCount}个，失败{result.Count - successCount}个");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量上传失败");
            return Error<List<FileUploadResultDto>>("批量上传失败");
        }
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="id">文件记录ID</param>
    /// <returns>文件内容</returns>
    /// <response code="200">返回文件内容</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">文件不存在或已删除</response>
    /// <remarks>
    /// 根据文件ID下载文件内容。
    /// 返回文件的原始内容和正确的Content-Type。
    /// 文件不存在或已删除时返回404错误。
    /// </remarks>
    /// <example>
    /// GET /api/file/download/1
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("download/{id}")]
    [ProducesResponseType(typeof(FileStreamResult), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> Download(Guid id)
    {
        try
        {
            // 先查询文件记录获取文件名和内容类型
            var fileRecords = await _fileService.GetPageListAsync(1, 1, userId: null, businessId: null);
            var fileRecord = fileRecords.List.FirstOrDefault(f => f.Id == id);

            var stream = await _fileService.DownloadAsync(id);
            if (stream == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Code = 404,
                    Message = "文件不存在或已删除",
                    Data = null
                });
            }

            // 获取文件信息用于设置响应头
            var fileName = fileRecord?.FileName ?? $"file_{id}";
            var contentType = fileRecord?.ContentType ?? "application/octet-stream";

            return File(stream, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "文件下载失败，ID：{Id}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Code = 500,
                Message = "文件下载失败",
                Data = null
            });
        }
    }

    /// <summary>
    /// 获取文件的预签名URL
    /// </summary>
    /// <param name="id">文件记录ID</param>
    /// <param name="expireSeconds">URL有效时间（秒），默认为3600秒（1小时）</param>
    /// <returns>预签名URL</returns>
    /// <response code="200">返回预签名URL</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">文件不存在或已删除</response>
    /// <remarks>
    /// 获取文件的预签名访问URL，无需认证即可访问。
    /// URL有过期时间限制，到期后无法访问。
    /// 适用于分享文件或前端直接下载场景。
    /// </remarks>
    /// <example>
    /// GET /api/file/presigned-url/1?expireSeconds=3600
    /// Authorization: Bearer {your-access-token}
    /// </example>
    [HttpGet("presigned-url/{id}")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    public async Task<ApiResponse<string>> GetPresignedUrl(
        Guid id,
        [FromQuery] int expireSeconds = 3600)
    {
        try
        {
            var url = await _fileService.GetPresignedUrlAsync(id, expireSeconds);
            if (url == null)
            {
                return Error<string>("文件不存在或已删除", 404);
            }

            return Success(url, $"预签名URL获取成功，有效期{expireSeconds}秒");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取预签名URL失败，ID：{Id}", id);
            return Error<string>("获取预签名URL失败");
        }
    }

    /// <summary>
    /// 获取文件列表（分页）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>分页后的文件记录列表</returns>
    /// <response code="200">返回文件列表</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 获取文件记录的分页列表。
    /// 支持按用户ID和业务ID筛选。
    /// 返回结果按创建时间倒序排列。
    /// 默认只显示状态正常的文件。
    /// </remarks>
    /// <example>
    /// POST /api/file/list
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "pageIndex": 1,
    ///     "pageSize": 10,
    ///     "userId": "550e8400-...",
    ///     "businessId": "550e8400-..."
    /// }
    /// </example>
    [HttpPost("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResponse<FileRecordDto>>), 200)]
    public async Task<ApiResponse<PageResponse<FileRecordDto>>> GetList([FromBody] QueryFileDto query)
    {
        try
        {
            var result = await _fileService.GetPageListAsync(query.PageIndex, query.PageSize, query.UserId, query.BusinessId);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取文件列表失败");
            return Error<PageResponse<FileRecordDto>>("获取文件列表失败");
        }
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="id">要删除的文件记录ID</param>
    /// <returns>影响的行数</returns>
    /// <response code="200">删除成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <response code="404">文件不存在</response>
    /// <remarks>
    /// 删除指定的文件记录。
    /// 这是逻辑删除，文件记录仍保留在数据库中，状态更新为已删除。
    /// 对象存储中的实际文件会被物理删除，无法恢复。
    /// </remarks>
    /// <example>
    /// POST /api/file/delete
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    /// </example>
    [HttpPost("delete")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> Delete([FromBody] Guid id)
    {
        try
        {
            var result = await _fileService.DeleteAsync(id);
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除文件失败，ID：{Id}", id);

            // 判断是否是业务异常（文件不存在）
            if (ex.Message.Contains("不存在"))
            {
                return Error<int>("文件不存在", 404);
            }

            return Error<int>("删除文件失败");
        }
    }

    /// <summary>
    /// 更新文件的业务ID
    /// </summary>
    /// <param name="request">更新请求，包含文件ID列表和业务ID</param>
    /// <returns>更新的文件数量</returns>
    /// <response code="200">更新成功</response>
    /// <response code="401">未授权，需要先登录</response>
    /// <remarks>
    /// 批量更新文件的 BusinessId，用于将上传的文件关联到具体的业务记录。
    /// 例如：先上传附件，再创建公告，创建后将附件关联到公告ID。
    /// </remarks>
    /// <example>
    /// POST /api/file/update-business-id
    /// Authorization: Bearer {your-access-token}
    /// Content-Type: application/json
    /// {
    ///     "fileIds": ["guid1", "guid2"],
    ///     "businessId": "announcement-guid"
    /// }
    /// </example>
    [HttpPost("update-business-id")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<ApiResponse<int>> UpdateBusinessId([FromBody] UpdateFileBusinessIdDto request)
    {
        try
        {
            var result = await _fileService.UpdateBusinessIdAsync(request.FileIds, request.BusinessId);
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新文件业务ID失败");
            return Error<int>("更新文件业务ID失败");
        }
    }
}