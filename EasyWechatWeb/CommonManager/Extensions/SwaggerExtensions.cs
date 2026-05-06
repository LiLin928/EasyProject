using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace CommonManager.Extensions;

/// <summary>
/// Swagger 扩展类
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// 注册 Swagger 服务
    /// </summary>
    public static IServiceCollection AddSwaggerService(this IServiceCollection services, string title = "EasyWeChatWeb API")
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = title,
                Version = "v1",
                Description = "EasyWeChatWeb 后端 API 服务"
            });

            // JWT 认证配置
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        return services;
    }

    /// <summary>
    /// 启用 Swagger UI 中间件
    /// </summary>
    public static IApplicationBuilder UseSwaggerService(this IApplicationBuilder app, string routePrefix = "swagger")
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyWeChatWeb API v1");
            options.RoutePrefix = routePrefix;
        });

        return app;
    }
}