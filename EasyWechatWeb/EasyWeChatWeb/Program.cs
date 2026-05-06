using Autofac;
using Autofac.Extensions.DependencyInjection;
using BusinessManager.Extensions;
using CommonManager.Cache;
using CommonManager.Extensions;
using CommonManager.Logging;
using CommonManager.Utility;
using EasyWeChatModels.Options;
using EasyWeChatWeb.Filters;
using EasyWeChatWeb.Middleware;
using InfrastructureManager.Extensions;
using Serilog;

// ============================================
// 1. Serilog 初始化（最早执行）
// ============================================
var esEnabled = false;
var esUrl = "";
var indexFormat = "";

// 先加载配置以便读取 ES 设置
var tempConfig = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
    .Build();

esEnabled = tempConfig.GetValue<bool>("Serilog:Elasticsearch:Enabled");
esUrl = tempConfig["Serilog:Elasticsearch:Url"] ?? "";
indexFormat = tempConfig["Serilog:Elasticsearch:IndexFormat"] ?? "";

// 根据 ES 配置选择初始化方式
if (esEnabled && !string.IsNullOrEmpty(esUrl))
{
    CommonManager.Logging.SerilogConfiguration.ConfigureWithElasticsearch(
        esUrl: esUrl,
        indexFormat: indexFormat
    );
}
else
{
    CommonManager.Logging.SerilogConfiguration.Configure();
}

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 2. 配置加载
// ============================================
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// ============================================
// 3. Serilog 集成
// ============================================
builder.Host.UseSerilog();

// ============================================
// 4. Autofac DI 容器替换
// ============================================
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    // 创建 Autofac 模块注册器，传入配置以支持 AutofacInject 特性读取配置
    var module = new AutofacModuleRegister(builder.Configuration);
    // 添加 InfrastructureManager 程序集（基础设施层）
    module.AddAssembly("InfrastructureManager");
    // 添加 BusinessManager 程序集（包含所有 Service）
    module.AddAssembly("BusinessManager");
    // 添加 CommonManager 程序集（包含 Extensions、Filters）
    module.AddAssembly("CommonManager");
    // 添加 EasyWeChatWeb 程序集（包含所有 Controller、Middleware）
    module.AddAssembly(typeof(Program).Assembly);
    containerBuilder.RegisterModule(module);

    // 注册 IConfiguration，让 AppSettingHelper 等类可以解析
    containerBuilder.RegisterInstance(builder.Configuration).As<IConfiguration>();

    // 微信 API 服务（单例）
    containerBuilder.RegisterType<BusinessManager.Buz.Service.WeChatApiService>()
        .As<BusinessManager.Buz.Service.WeChatApiService>()
        .SingleInstance();

    // ETL 执行引擎（属性注入）
    containerBuilder.RegisterType<BusinessManager.Buz.Etl.Engine.EtlExecutionEngine>()
        .As<BusinessManager.Buz.Etl.Engine.IEtlExecutionEngine>()
        .InstancePerLifetimeScope()
        .OnActivated(e =>
        {
            // 手动注入属性
            var instance = e.Instance;

            // 注入 ISqlSugarClient
            if (instance._db == null)
            {
                instance._db = e.Context.Resolve<SqlSugar.ISqlSugarClient>();
            }

            // 注入 ILogger（通过 ILoggerFactory 创建）
            if (instance._logger == null)
            {
                var loggerFactory = e.Context.Resolve<Microsoft.Extensions.Logging.ILoggerFactory>();
                instance._logger = loggerFactory.CreateLogger<BusinessManager.Buz.Etl.Engine.EtlExecutionEngine>();
            }

            // 注入 ETL 执行日志服务
            if (instance._etlLogService == null)
            {
                instance._etlLogService = e.Context.Resolve<BusinessManager.Buz.Etl.Log.IEtlExecutionLogService>();
            }
        });

    // EtlExecutionLogService 已通过 AutofacModuleRegister 批量注册，无需单独注册

    // 事务性任务执行器（属性注入）
    containerBuilder.RegisterType<BusinessManager.Tasks.TransactionalTaskExecutor>()
        .AsSelf()
        .InstancePerLifetimeScope()
        .OnActivated(e =>
        {
            var instance = e.Instance;
            var properties = instance.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(p => p.Name.StartsWith("_") && p.CanWrite);

            foreach (var property in properties)
            {
                if (property.GetValue(instance) != null)
                    continue;

                try
                {
                    if (e.Context.TryResolve(property.PropertyType, out var service))
                    {
                        property.SetValue(instance, service);
                    }
                }
                catch
                {
                    // 忽略解析失败
                }
            }
        });

    // Quartz 任务调度（使用 Autofac 注册）
    containerBuilder.AddQuartzService(builder.Configuration);

    // 文件存储服务（根据配置注入 MinIO 或本地存储）
    var storageType = builder.Configuration["LocalStorageOption:StorageType"] ?? "local";
    if (storageType == "minio")
    {
        // MinIO 存储配置
        var minioOption = builder.Configuration.GetSection("MinioOption").Get<EasyWeChatModels.Options.MinioOption>();
        if (minioOption != null && !string.IsNullOrEmpty(minioOption.Endpoint))
        {
            containerBuilder.Register(c => new CommonManager.Helper.MinioHelper(
                minioOption.Endpoint,
                minioOption.AccessKey,
                minioOption.SecretKey,
                minioOption.BucketName,
                minioOption.UseSSL
            )).As<CommonManager.Helper.IFileStorageHelper>().SingleInstance();
            Log.Information("文件存储：使用 MinIO 对象存储");
        }
        else
        {
            // MinIO 配置无效，fallback 到本地存储
            RegisterLocalStorage(containerBuilder, builder.Configuration);
            Log.Warning("MinIO 配置无效，fallback 到本地存储");
        }
    }
    else
    {
        // 本地存储配置
        RegisterLocalStorage(containerBuilder, builder.Configuration);
        Log.Information("文件存储：使用本地文件存储");
    }

    // 任务启动调度器（只在 Quartz 启用时注册）
    var quartzEnabled = builder.Configuration.GetValue<bool>("Quartz:Enabled");
    if (quartzEnabled)
    {
        containerBuilder.RegisterType<BusinessManager.Tasks.TaskStartupScheduler>()
            .As<Microsoft.Extensions.Hosting.IHostedService>()
            .SingleInstance();
    }
});

// ============================================
// 5. 服务注册
// ============================================
// Elasticsearch 日志查询配置
builder.Services.Configure<ElasticsearchQueryOptions>(
    builder.Configuration.GetSection("ElasticsearchQuery"));
builder.Services.AddSingleton<CommonManager.Elasticsearch.IElasticsearchClientFactory,
    CommonManager.Elasticsearch.ElasticsearchClientFactory>();

// 操作日志 Elasticsearch 存储配置
builder.Services.Configure<OperateLogElasticsearchOptions>(
    builder.Configuration.GetSection("OperateLogElasticsearch"));
builder.Services.AddSingleton<BusinessManager.Buz.Service.OperateLogElasticsearchService>();

// 控制器（添加全局过滤器）
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<EasyWeChatWeb.Filters.OperateLogActionFilter>();
})
.AddJsonOptions(options =>
{
    // JSON 输出使用驼峰命名
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;

    // 注册空字符串转 null 的 Converters
    // 处理前端传递的空字符串，自动转换为 null
    options.JsonSerializerOptions.Converters.Add(new EasyWeChatWeb.JsonConverters.EmptyStringToNullConverter());
    options.JsonSerializerOptions.Converters.Add(new EasyWeChatWeb.JsonConverters.EmptyStringToNullableGuidConverter());
    options.JsonSerializerOptions.Converters.Add(new EasyWeChatWeb.JsonConverters.EmptyStringToNullableIntConverter());
    options.JsonSerializerOptions.Converters.Add(new EasyWeChatWeb.JsonConverters.EmptyStringToNullableLongConverter());
    options.JsonSerializerOptions.Converters.Add(new EasyWeChatWeb.JsonConverters.EmptyStringToNullableBoolConverter());

    // 注册 DateTime Converters，支持 YYYY-MM-DD HH:mm:ss 格式
    options.JsonSerializerOptions.Converters.Add(new EasyWeChatWeb.JsonConverters.FlexibleDateTimeConverter());
    options.JsonSerializerOptions.Converters.Add(new EasyWeChatWeb.JsonConverters.FlexibleDateTimeConverterForNonNullable());
})
.AddControllersAsServices();

// SqlSugar
builder.Services.AddSqlSugarService(builder.Configuration);

// CAP 事件总线
builder.Services.AddCapService(builder.Configuration);

// HttpClient（用于 API 回调）
builder.Services.AddHttpClient();

// CAP 失败消息监控服务 - 定期检查重试耗尽的消息并执行补偿（只有 CAP 启用时才注册）
builder.Services.AddCapFailedMessageMonitor(builder.Configuration);

// 缓存
builder.Services.AddCacheService(builder.Configuration);

// JWT 认证
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS
builder.Services.AddCorsService(builder.Configuration);

// Options 配置
builder.Services.Configure<WeChatOptions>(builder.Configuration.GetSection("WeChat"));
builder.Services.Configure<WeChatPayOptions>(builder.Configuration.GetSection("WeChatPay"));
builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));
builder.Services.Configure<ScreenOptions>(builder.Configuration.GetSection("Screen"));

// Swagger（开发环境）
if (builder.Configuration.GetValue<bool>("IsUseSwagger"))
{
    builder.Services.AddSwaggerService();
}

// 注意：以下服务已通过 Autofac 自动注入，无需手动注册：
// - ScreenService、WorkflowService、WorkflowVersionService、PublishRequestService
// - WorkflowTemplateService、WorkflowInstanceService、WorkflowTaskService、WorkflowCCService
// - LogQueryService、TokenBlacklistCache
// - 所有 *Service、*Filter、*Factory 结尾的类

// ============================================
// 6. 构建应用
// ============================================
var app = builder.Build();

// ============================================
// 7. 中间件管道配置
// ============================================
// 异常处理（最外层）
app.UseMiddleware<EasyWeChatWeb.Middleware.ExceptionHandlingMiddleware>();

// 开发环境 Swagger
if (app.Configuration.GetValue<bool>("IsUseSwagger"))
{
    app.UseSwaggerService();
}

// HTTPS 重定向
app.UseHttpsRedirection();

// 静态文件服务（本地存储时启用）
var localStorageOptionSection = builder.Configuration.GetSection("LocalStorageOption");
var localStorageOption = localStorageOptionSection.Get<EasyWeChatModels.Options.LocalStorageOption>();
if (localStorageOption?.StorageType == "local" && !string.IsNullOrEmpty(localStorageOption.RootPath))
{
    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), localStorageOption.RootPath);
    if (!Directory.Exists(uploadsPath))
    {
        Directory.CreateDirectory(uploadsPath);
    }
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
        RequestPath = localStorageOption.PublicUrlPrefix ?? "/uploads"
    });
    Log.Information("静态文件服务已启用，路径：{Path}, URL前缀：{Prefix}", uploadsPath, localStorageOption.PublicUrlPrefix);
}

// CORS
app.UseCors("AllowSpecific");

// 认证授权
app.UseAuthentication();
app.UseAuthorization();

// WebSocket 支持
app.UseWebSockets();

// 聊天 WebSocket 中间件
app.UseMiddleware<EasyWeChatWeb.Middleware.ChatWebSocketMiddleware>();

// 请求日志
app.UseMiddleware<EasyWeChatWeb.Middleware.RequestLoggingMiddleware>();

// 控制器路由
app.MapControllers();

// ============================================
// 8. 启动运行
// ============================================
Log.Information("EasyWeChatWeb API 启动成功，监听端口: 7600");
app.Run();

// ============================================
// 辅助方法
// ============================================

/// <summary>
/// 注册本地文件存储服务
/// </summary>
static void RegisterLocalStorage(Autofac.ContainerBuilder containerBuilder, IConfiguration configuration)
{
    var localStorageOption = configuration.GetSection("LocalStorageOption").Get<EasyWeChatModels.Options.LocalStorageOption>();
    var rootPath = localStorageOption?.RootPath ?? "uploads";
    var publicUrlPrefix = localStorageOption?.PublicUrlPrefix ?? "/uploads";
    var baseUrl = localStorageOption?.BaseUrl ?? "http://localhost:7600";

    containerBuilder.Register(c =>
    {
        var logger = c.Resolve<Microsoft.Extensions.Logging.ILoggerFactory>()
            .CreateLogger<CommonManager.Helper.LocalStorageHelper>();
        return new CommonManager.Helper.LocalStorageHelper(rootPath, publicUrlPrefix, baseUrl, logger);
    }).As<CommonManager.Helper.IFileStorageHelper>().SingleInstance();
}