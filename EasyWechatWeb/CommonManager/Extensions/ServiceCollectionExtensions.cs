using CommonManager.Cache;
using EasyWeChatModels.Options;
using CommonManager.SqlSugar;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CommonManager.Extensions;

/// <summary>
/// 服务注册扩展类
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 注册 SqlSugar 数据库服务
    /// </summary>
    public static IServiceCollection AddSqlSugarService(this IServiceCollection services, IConfiguration configuration)
    {
        var dbOptions = configuration.GetSection("MasterSlaveConnectionStrings")
            .Get<List<DbConnectionOptions>>() ?? new List<DbConnectionOptions>();

        services.AddSqlSugar(dbOptions);
        return services;
    }

    /// <summary>
    /// 注册缓存服务（通过 Autofac 属性注入）
    /// </summary>
    public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        var useRedis = configuration.GetValue<bool>("IsUseRedis");

        if (useRedis)
        {
            // Redis 缓存服务（通过 Autofac 属性注入 IConfiguration）
            services.AddSingleton<ICache, RedisCacheService>();
        }
        else
        {
            // 内存缓存服务（需要先注册 IMemoryCache）
            services.AddMemoryCache();
            services.AddSingleton<ICache, MemoryCacheService>();
        }

        return services;
    }

    /// <summary>
    /// 注册 JWT 认证服务
    /// </summary>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection("JWTTokenOptions").Get<JWTTokenOptions>();

        if (jwtOptions == null)
        {
            throw new ArgumentNullException("JWTTokenOptions configuration is missing");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey))
            };
        });

        return services;
    }
}