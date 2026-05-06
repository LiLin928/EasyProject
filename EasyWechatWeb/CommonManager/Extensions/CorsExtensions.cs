using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommonManager.Extensions;

/// <summary>
/// CORS（跨域资源共享）扩展类
/// </summary>
public static class CorsExtensions
{
    /// <summary>
    /// 注册 CORS 服务
    /// </summary>
    public static IServiceCollection AddCorsService(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("CorsOrigins").Get<string[]>() ?? new string[] { "*" };

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            options.AddPolicy("AllowSpecific", builder =>
            {
                builder.WithOrigins(origins)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        return services;
    }
}