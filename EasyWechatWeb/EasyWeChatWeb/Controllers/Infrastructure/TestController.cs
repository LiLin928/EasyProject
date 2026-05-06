using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EasyWeChatWeb.Controllers.Infrastructure;

/// <summary>
/// 测试接口控制器
/// </summary>
/// <remarks>
/// 提供测试用的 API 接口，供定时任务 API 调用模式使用
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // 允许匿名访问，供定时任务调用
public class TestController : ControllerBase
{
    /// <summary>
    /// 日志记录器（属性注入）
    /// </summary>
    public ILogger<TestController> _logger { get; set; } = null!;

    /// <summary>
    /// HelloWorld 测试接口
    /// </summary>
    /// <returns>返回 HelloWorld 消息</returns>
    [HttpGet("helloworld")]
    [HttpPost("helloworld")]
    public IActionResult HelloWorld()
    {
        var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _logger.LogInformation("HelloWorld API 被调用 - {Time}", now);

        return Ok(new
        {
            success = true,
            message = "HelloWorld!",
            time = now
        });
    }

    /// <summary>
    /// 带参数的 HelloWorld 测试接口
    /// </summary>
    /// <param name="data">请求数据</param>
    /// <returns>返回 HelloWorld 消息</returns>
    [HttpPost("helloworld-with-data")]
    public IActionResult HelloWorldWithData([FromBody] object? data)
    {
        var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        _logger.LogInformation("HelloWorld API 被调用（带参数） - {Time}, Data: {Data}", now, data);

        return Ok(new
        {
            success = true,
            message = "HelloWorld!",
            time = now,
            receivedData = data
        });
    }
}