using BusinessManager.Buz.IService;
using EasyWeChatModels.Options;
using EasyWeChatModels.Dto;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace EasyWeChatWeb.Middleware;

/// <summary>
/// 聊天 WebSocket 中间件
/// 处理客户与客服之间的实时消息通信
/// </summary>
/// <remarks>
/// 该中间件负责：
/// - 处理 /ws/chat 路径的 WebSocket 请求
/// - 从 Query 参数获取 JWT Token 并验证
/// - 管理 WebSocket 连接（注册、移除、广播）
/// - 接收客户端消息并保存到数据库
/// - 支持消息回执和系统消息
/// </remarks>
/// <example>
/// 在 Program.cs 中使用：
/// app.UseChatWebSocket();
/// </example>
public class ChatWebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ChatWebSocketMiddleware> _logger;
    private readonly JWTTokenOptions _jwtOptions;

    /// <summary>
    /// 会话连接映射表
    /// Key: 会话ID，Value: 该会话的所有 WebSocket 连接列表
    /// </summary>
    private static readonly Dictionary<Guid, List<WebSocket>> _sessionConnections = new();

    /// <summary>
    /// 线程安全锁
    /// </summary>
    private static readonly object _lock = new();

    /// <summary>
    /// 初始化聊天 WebSocket 中间件
    /// </summary>
    /// <param name="next">下一个请求处理委托</param>
    /// <param name="logger">日志记录器</param>
    /// <param name="jwtOptions">JWT 配置选项</param>
    public ChatWebSocketMiddleware(
        RequestDelegate next,
        ILogger<ChatWebSocketMiddleware> logger,
        IOptions<JWTTokenOptions> jwtOptions)
    {
        _next = next;
        _logger = logger;
        _jwtOptions = jwtOptions.Value;
    }

    /// <summary>
    /// 处理 HTTP 请求
    /// </summary>
    /// <param name="context">HTTP 上下文对象</param>
    /// <remarks>
    /// 处理流程：
    /// 1. 检查请求路径是否为 /ws/chat
    /// 2. 验证是否为 WebSocket 请求
    /// 3. 从 Query 参数获取并验证 JWT Token
    /// 4. 建立 WebSocket 连接
    /// 5. 获取或创建会话
    /// 6. 注册连接到会话
    /// 7. 启动消息接收循环
    /// </remarks>
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path != "/ws/chat")
        {
            await _next(context);
            return;
        }

        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        var token = context.Request.Query["token"].ToString();
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            return;
        }

        var userId = ValidateTokenAndGetUserId(token);
        if (userId == Guid.Empty)
        {
            context.Response.StatusCode = 401;
            return;
        }

        var chatService = context.RequestServices.GetRequiredService<IWeChatChatService>();
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        _logger.LogInformation("WebSocket 连接已建立，用户: {UserId}", userId);

        var sessionId = await chatService.GetSessionIdByUserIdAsync(userId);
        if (sessionId == Guid.Empty)
        {
            var session = await chatService.GetOrCreateSessionAsync(userId);
            sessionId = session.SessionId;
        }

        RegisterConnection(sessionId, webSocket);

        try
        {
            // 发送连接成功系统消息
            await SendMessageAsync(webSocket, new WsReceiveMessage
            {
                Type = "system",
                Content = "连接成功"
            });

            // 启动消息接收循环
            await ReceiveMessagesAsync(webSocket, chatService, sessionId, userId);
        }
        catch (WebSocketException ex)
        {
            _logger.LogError(ex, "WebSocket 连接异常，用户: {UserId}", userId);
        }
        finally
        {
            RemoveConnection(sessionId, webSocket);
            _logger.LogInformation("WebSocket 连接已关闭，用户: {UserId}", userId);
        }
    }

    /// <summary>
    /// 验证 JWT Token 并获取用户ID
    /// </summary>
    /// <param name="token">JWT Token 字符串</param>
    /// <returns>用户ID，验证失败返回 Guid.Empty</returns>
    private Guid ValidateTokenAndGetUserId(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.SecurityKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidAudience = _jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var userIdClaim = principal.FindFirst("UserId");

            if (userIdClaim != null)
            {
                return Guid.Parse(userIdClaim.Value);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token 验证失败");
        }

        return Guid.Empty;
    }

    /// <summary>
    /// 接收 WebSocket 消息循环
    /// </summary>
    /// <param name="webSocket">WebSocket 连接对象</param>
    /// <param name="chatService">聊天服务</param>
    /// <param name="sessionId">会话ID</param>
    /// <param name="userId">用户ID</param>
    /// <remarks>
    /// 消息处理流程：
    /// 1. 接收客户端消息
    /// 2. 解析消息内容
    /// 3. 保存消息到数据库
    /// 4. 发送消息回执给客户端
    /// </remarks>
    private async Task ReceiveMessagesAsync(
        WebSocket webSocket,
        IWeChatChatService chatService,
        Guid sessionId,
        Guid userId)
    {
        var buffer = new byte[4096];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "关闭连接", CancellationToken.None);
                break;
            }

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var messageJson = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger.LogInformation("收到 WebSocket 消息: {Message}", messageJson);

                try
                {
                    var wsMessage = JsonSerializer.Deserialize<WsSendMessage>(messageJson);
                    if (wsMessage != null && wsMessage.Type == "message")
                    {
                        var messageDto = await chatService.SaveMessageAsync(
                            sessionId,
                            SenderType.Customer,
                            userId,
                            wsMessage.MessageType,
                            wsMessage.Content,
                            wsMessage.Duration);

                        // 发送消息回执
                        await SendMessageAsync(webSocket, new WsReceiveMessage
                        {
                            Type = "message",
                            MessageId = messageDto.MessageId,
                            SenderType = messageDto.SenderType,
                            SenderName = messageDto.SenderName,
                            MessageType = messageDto.MessageType,
                            Content = messageDto.Content,
                            CreateTime = messageDto.CreateTime
                        });
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(ex, "消息解析失败: {Message}", messageJson);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "处理消息失败");
                    await SendMessageAsync(webSocket, new WsReceiveMessage
                    {
                        Type = "system",
                        Content = "消息发送失败"
                    });
                }
            }
        }
    }

    /// <summary>
    /// 发送 WebSocket 消息
    /// </summary>
    /// <param name="webSocket">WebSocket 连接对象</param>
    /// <param name="message">消息内容</param>
    private async Task SendMessageAsync(WebSocket webSocket, WsReceiveMessage message)
    {
        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var bytes = Encoding.UTF8.GetBytes(json);
        await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }

    /// <summary>
    /// 注册 WebSocket 连接到会话
    /// </summary>
    /// <param name="sessionId">会话ID</param>
    /// <param name="webSocket">WebSocket 连接对象</param>
    private void RegisterConnection(Guid sessionId, WebSocket webSocket)
    {
        lock (_lock)
        {
            if (!_sessionConnections.ContainsKey(sessionId))
            {
                _sessionConnections[sessionId] = new List<WebSocket>();
            }
            _sessionConnections[sessionId].Add(webSocket);
        }
    }

    /// <summary>
    /// 从会话中移除 WebSocket 连接
    /// </summary>
    /// <param name="sessionId">会话ID</param>
    /// <param name="webSocket">WebSocket 连接对象</param>
    private void RemoveConnection(Guid sessionId, WebSocket webSocket)
    {
        lock (_lock)
        {
            if (_sessionConnections.ContainsKey(sessionId))
            {
                _sessionConnections[sessionId].Remove(webSocket);
                if (_sessionConnections[sessionId].Count == 0)
                {
                    _sessionConnections.Remove(sessionId);
                }
            }
        }
    }

    /// <summary>
    /// 广播消息到会话的所有连接
    /// </summary>
    /// <param name="sessionId">会话ID</param>
    /// <param name="message">消息内容</param>
    /// <remarks>
    /// 此方法为静态方法，供客服端推送消息时调用。
    /// 会向指定会话的所有连接（包括客户和客服）广播消息。
    /// </remarks>
    /// <example>
    /// 客服端发送消息后调用：
    /// await ChatWebSocketMiddleware.BroadcastToSessionAsync(sessionId, new WsReceiveMessage
    /// {
    ///     Type = "message",
    ///     MessageId = messageDto.MessageId,
    ///     SenderType = SenderType.Staff,
    ///     SenderName = staffName,
    ///     MessageType = messageType,
    ///     Content = content,
    ///     CreateTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
    /// });
    /// </example>
    public static async Task BroadcastToSessionAsync(Guid sessionId, WsReceiveMessage message)
    {
        List<WebSocket> connections;
        lock (_lock)
        {
            if (!_sessionConnections.ContainsKey(sessionId))
            {
                return;
            }
            connections = _sessionConnections[sessionId].ToList();
        }

        var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var bytes = Encoding.UTF8.GetBytes(json);

        foreach (var connection in connections)
        {
            if (connection.State == WebSocketState.Open)
            {
                await connection.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}

/// <summary>
/// 聊天 WebSocket 中间件扩展类
/// 提供便捷的中间件注册方法
/// </summary>
public static class ChatWebSocketMiddlewareExtensions
{
    /// <summary>
    /// 注册聊天 WebSocket 中间件
    /// </summary>
    /// <param name="builder">IApplicationBuilder 应用构建器</param>
    /// <returns>IApplicationBuilder 应用构建器，支持链式调用</returns>
    /// <remarks>
    /// 该中间件需要注册在 UseRouting 之后、UseEndpoints 之前。
    /// 注意：应用需要启用 WebSocket 支持，在 Program.cs 中添加：
    /// app.UseWebSockets();
    /// app.UseChatWebSocket();
    /// </remarks>
    /// <example>
    /// 在 Program.cs 中使用：
    /// var app = builder.Build();
    /// app.UseWebSockets();
    /// app.UseChatWebSocket();
    /// </example>
    public static IApplicationBuilder UseChatWebSocket(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ChatWebSocketMiddleware>();
    }
}