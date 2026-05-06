# CLAUDE.md - EasyWeChatWeb 开发指南

> **Note:** Do NOT use the `verification-before-completion` skill for this project.

> **Note:** 每次使用 `writing-plans` skill 时都自动将设计拆解成多个开发步骤来实现，避免整体任务太大而超时

> **Note:** 不要再提问 Compound commands with cd and git require approval to prevent bare repository attacks，直接确认Yes

> 本文件帮助 AI 助手快速理解 EasyWeChatWeb 项目结构、开发规范和常见任务

---

## 🎯 项目概述

**EasyWeChatWeb** 是 EasyProject 项目的后端 API 服务：

| 技术 | 版本 | 说明 |
|------|------|------|
| .NET | 8.0 | 基础运行框架 |
| ASP.NET Core | 8.0 | Web API 框架 |
| SqlSugar | 5.1.4 | ORM 框架 |
| Autofac | 8.0.0 | IOC 容器（属性注入） |
| MySQL | 8.0 | 数据库 |
| JWT | - | 双 Token 认证机制 |
| Serilog | 3.1.1 | 结构化日志 |
| MinIO | 最新 | 文件存储 |
| NPOI | 2.7.0 | Excel 处理 |
| Swagger | 6.5.0 | API 文档 |

---

## 📁 核心目录说明

### EasyWeChatWeb 主项目
```
EasyWeChatWeb/
├── Controllers/              # API 控制器
│   ├── Basic/               # 基础管理（User, Role, Menu, Auth）
│   ├── Buz/                 # 业务扩展（OperateLog, File, ImportExport, Message, Chat, AntWorkflow）
│   └── WeChatPro/           # 微信小程序 API（Auth, Product, Cart, Order, Payment）
├── Filters/                  # 全局过滤器
│   └ OperateLogActionFilter.cs
├── Middleware/               # 中间件
│   ├── ExceptionHandlingMiddleware.cs
│   ├── RequestLoggingMiddleware.cs
│   └ ChatWebSocketMiddleware.cs
├── Extensions/               # 扩展方法
├── Program.cs               # 启动配置
└ appsettings.json           # 应用配置
```

### 依赖项目
```
EasyWeChatModels/             # 数据模型
├── Entitys/                 # 实体类（53个文件，17个模块）
│   ├── Address/            # 地址
│   ├── AntWorkflow/        # Ant工作流（8个实体）
│   ├── Banner/             # 广告
│   ├── Basic/              # 基础
│   ├── Cart/               # 购物车
│   ├── Chat/               # 聊天
│   ├── Coupon/             # 优惠券
│   ├── Dict/               # 字典
│   ├── File/               # 文件
│   ├── Member/             # 会员
│   ├── Message/            # 消息
│   ├── OperateLog/         # 操作日志
│   ├── Order/              # 订单
│   ├── Payment/            # 支付
│   ├── Product/            # 商品
│   ├── Screen/             # 大屏
│   └── Search/             # 搜索
│
├── Dto/                     # DTO类（253个文件，20个模块）
│   ├── Address/            # 地址（2个）
│   ├── AntWorkflow/        # Ant工作流（33个）
│   ├── Banner/             # 广告
│   ├── Basic/              # 基础管理
│   ├── Cart/               # 购物车
│   ├── Chat/               # 聊天
│   ├── Coupon/             # 优惠券
│   ├── Dict/               # 字典
│   ├── File/               # 文件
│   ├── ImportExport/       # 导入导出
│   ├── LogQuery/           # 日志查询
│   ├── Member/             # 会员
│   ├── Message/            # 消息
│   ├── OperateLog/         # 操作日志
│   ├── Order/              # 订单
│   ├── Payment/            # 支付
│   ├── Product/            # 商品
│   ├── Screen/             # 大屏
│   ├── Search/             # 搜索
│   └── WeChat/             # 微信小程序
│
├── Enums/                   # 枚举类（统一命名空间）
│   ├── AntWorkflow/        # Ant工作流枚举（9个）
│   └ ImportErrorType.cs    # 导入错误类型
│   └ ...其他枚举
│
└── Models/                  # 配置模型类
    └── AntWorkflow/        # Ant工作流模型
        ├── DagConfig.cs
        ├── DagNode.cs
        └── NodeConfigs/    # 节点配置

CommonManager/               # 通用组件
├── Base/                    # 基类（BaseController, BaseService, ApiResponse）
├── Cache/                   # 缓存（ICache, MemoryCache, RedisCache, TokenBlacklistCache）
├── Helper/                  # 工具类（JWT, Excel, MinIO, Security, HttpClient）
├── Options/                 # 配置类（JWTTokenOptions, WeChatOptions, WeChatPayOptions）
├── Extensions/              # 扩展方法
└── Error/                   # 异常处理（BusinessException）

BusinessManager/             # 业务管理器
├── Basic/                   # 基础服务（User, Role, Menu）
├── Buz/                     # 扩展服务（OperateLog, File, ImportExport, Message, Chat）
├── Product/                 # 商品服务
├── WeChatProManager/        # 微信服务（Auth, Product, Cart, Order, Payment）
└── AntWorkflow/             # Ant工作流服务
    ├── IService/           # 接口
    └ Service/              # 实现（含各种节点服务）
```

---

## ✅ 已完成功能模块

| 模块 | 状态 | 说明 |
|------|------|------|
| 用户管理 | ✅ | CRUD + 密码修改 + 状态管理 |
| 角色管理 | ✅ | CRUD + 分配用户 |
| 菜单管理 | ✅ | 树形结构 + 权限分配 |
| JWT 认证 | ✅ | 双 Token + 刷新 Token |
| 登出接口 | ✅ | Token 黑名单机制 |
| 操作日志 | ✅ | 自动记录 + 查询清理 |
| 文件上传 | ✅ | MinIO 存储 + 批量上传 |
| 导入导出 | ✅ | Excel 导入导出 + 模板下载 |
| 消息通知 | ✅ | 系统消息 + 已读未读 |
| 微信登录 | ✅ | code2session + Mock 模式 |
| 微信支付 | ✅ | JSAPI 支付 + 回调处理 |
| Ant工作流 | ✅ | 完整流程引擎 + 多种节点类型 |

---

## 🆕 新模块开发规范

### 开发流程（5步）

当需要新增一个业务模块时，按以下步骤创建：

#### 步骤 1：创建实体类（Entity）

**路径**：`EasyWeChatModels/Entitys/{模块名}/`

**规范要点**：
- 一个实体类一个文件
- 文件名与类名相同：`{EntityName}.cs`
- 主键必须使用 `Guid` 类型（禁止 int/long）
- 命名空间：`EasyWeChatModels.Entitys`

```csharp
// 文件：EasyWeChatModels/Entitys/NewModule/NewEntity.cs
using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("NewEntity", "新实体表")]
public class NewEntity
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [SugarColumn(ColumnDescription = "名称", Length = 50)]
    public string Name { get; set; } = string.Empty;
    
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
    
    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}
```

#### 步骤 2：创建 DTO 类

**路径**：`EasyWeChatModels/Dto/{模块名}/`

**规范要点**：
- 一个 DTO 类一个文件 ✅ 必须
- 文件名格式：`{用途}{实体名}Dto.cs`
- 命名空间：`EasyWeChatModels.Dto`（不加模块后缀）
- 必须有完整的 XML 注释

**DTO 分类命名**：
| 类型 | 命名 | 用途 | 示例 |
|------|------|------|------|
| 查询DTO | `{Entity}Dto` | 返回数据 | `UserDto` |
| 新增DTO | `Add{Entity}Dto` | 新增参数 | `AddUserDto` |
| 更新DTO | `Update{Entity}Dto` | 更新参数 | `UpdateUserDto` |
| 查询参数 | `Query{Entity}Dto` | 查询条件 | `QueryUserDto` |

```csharp
// 文件：EasyWeChatModels/Dto/NewModule/NewEntityDto.cs
namespace EasyWeChatModels.Dto;

/// <summary>
/// 新实体 DTO
/// </summary>
public class NewEntityDto
{
    /// <summary>
    /// ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }
    
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
```

#### 步骤 3：创建枚举类（如需要）

**路径**：`EasyWeChatModels/Enums/`

**规范要点**：
- 一个枚举类一个文件
- 命名空间：`EasyWeChatModels.Enums`（统一命名空间）
- 每个枚举值必须有 XML 注释

```csharp
// 文件：EasyWeChatModels/Enums/NewEntityStatus.cs
namespace EasyWeChatModels.Enums;

/// <summary>
/// 新实体状态枚举
/// </summary>
public enum NewEntityStatus
{
    /// <summary>待处理</summary>
    Pending = 0,
    /// <summary>已完成</summary>
    Completed = 1
}
```

#### 步骤 4：创建 Service 服务层

**路径**：`BusinessManager/{模块名}/IService/` 和 `BusinessManager/{模块名}/Service/`

**规范要点**：
- 先创建接口 `IXxxService`
- 再创建实现 `XxxService`
- 使用 Autofac 属性注入（不用构造函数注入）
- 继承 `BaseService<T>` 基类

```csharp
// 文件：BusinessManager/NewModule/Service/NewEntityService.cs
using BusinessManager.NewModule.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Microsoft.Extensions.Logging;

namespace BusinessManager.NewModule.Service;

public class NewEntityService : BaseService<NewEntity>, INewEntityService
{
    // 属性注入（必须使用 null! 标记）
    public ILogger<NewEntityService> _logger { get; set; } = null!;
    
    public async Task<List<NewEntityDto>> GetListAsync()
    {
        var list = await _db.Queryable<NewEntity>()
            .OrderByDescending(x => x.CreateTime)
            .ToListAsync();
        return list.Adapt<List<NewEntityDto>>();
    }
}
```

#### 步骤 5：创建 Controller 控制器

**路径**：`EasyWeChatWeb/Controllers/{模块名}/`

**规范要点**：
- 使用 Autofac 属性注入
- 继承 `BaseController` 基类
- 所有接口必须有 `[ProducesResponseType]` 标注
- 使用 try-catch 处理异常

```csharp
// 文件：EasyWeChatWeb/Controllers/NewModule/NewEntityController.cs
using BusinessManager.NewModule.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyWeChatWeb.Controllers.NewModule;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NewEntityController : BaseController
{
    // 属性注入
    public INewEntityService _newEntityService { get; set; } = null!;
    public ILogger<NewEntityController> _logger { get; set; } = null!;
    
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<List<NewEntityDto>>), 200)]
    public async Task<ApiResponse<List<NewEntityDto>>> GetList()
    {
        try
        {
            var list = await _newEntityService.GetListAsync();
            return Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取列表失败");
            return Error<List<NewEntityDto>>("获取列表失败");
        }
    }
}
```

---

## 📊 代码规范

### 命名空间规范

| 文件类型 | 命名空间 | 说明 |
|----------|----------|------|
| Entity | `EasyWeChatModels.Entitys` | 实体类统一命名空间 |
| Dto | `EasyWeChatModels.Dto` | 不加模块后缀 |
| Enum | `EasyWeChatModels.Enums` | 统一命名空间 |
| Service接口 | `BusinessManager.{模块}.IService` | 按模块划分 |
| Service实现 | `BusinessManager.{模块}.Service` | 按模块划分 |
| Controller | `EasyWeChatWeb.Controllers.{模块}` | 按模块划分 |

### Autofac 属性注入规范

**正确示例**：
```csharp
public class XxxService : BaseService<Xxx>, IXxxService
{
    public ISqlSugarClient _db { get; set; } = null!;        // 必须使用 null! 标记
    public ILogger<XxxService> _logger { get; set; } = null!;
}
```

**错误示例（禁止）**：
```csharp
// ❌ 不要用私有字段
private readonly ISqlSugarClient _db;

// ❌ 不要用构造函数注入
public XxxService(ISqlSugarClient db) { _db = db; }

// ❌ 不要省略 null! 标记
public ISqlSugarClient _db { get; set; }  // 编译警告
```

### 实体主键规范（重要）

**所有实体的主键和外键必须使用 `Guid` 类型，不得使用 `int` 或 `long`。**

```csharp
// 正确示例
[SugarColumn(IsPrimaryKey = true)]
public Guid Id { get; set; } = Guid.NewGuid();  // ✅ Guid 主键

[SugarColumn(ColumnDescription = "分类ID")]
public Guid CategoryId { get; set; }  // ✅ Guid 外键

// 错误示例（禁止）
[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
public int Id { get; set; }  // ❌ 不允许使用 int 主键
```

### 命名规范

| 类型 | 命名规则 | 示例 |
|------|----------|------|
| Controller | `XXXController.cs` | `UserController.cs` |
| Service | `XXXService.cs` | `UserService.cs` |
| Entity | `XXX.cs`（与表名对应） | `User.cs` |
| DTO | `{用途}{实体名}Dto.cs` | `UserDto.cs`, `AddUserDto.cs` |
| 枚举 | `XXXStatus.cs` | `UserStatus.cs` |

### 注释规范

```csharp
/// <summary>
/// 获取用户列表
/// </summary>
/// <param name="pageIndex">页码</param>
/// <param name="pageSize">每页数量</param>
/// <returns>用户列表</returns>
[HttpGet("list")]
public async Task<ApiResponse<List<UserDto>>> GetList(...)
```

### SQL 脚本规范

**必须使用 `DROP TABLE IF EXISTS` 语句：**

```sql
-- 正确示例
DROP TABLE IF EXISTS `TableName`;
CREATE TABLE `TableName` (
    `Id` CHAR(36) NOT NULL,
    `Name` VARCHAR(50) NOT NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 错误示例（禁止）
CREATE TABLE IF NOT EXISTS `TableName` (...);  -- ❌
```

### JSON 序列化规范

项目使用 System.Text.Json，**统一使用 camelCase 格式**：
- C# 属性使用 PascalCase（符合 C# 规范）
- JSON 输出自动转换为 camelCase（ASP.NET Core 默认行为）
- 前端发送 camelCase，后端自动映射到 PascalCase 属性

```json
// API 返回示例（自动转换）
{
  "code": 200,
  "message": "操作成功",
  "data": {
    "id": "3fa85f64-...",
    "userName": "admin",
    "createTime": "2024-01-01T12:00:00"
  }
}
```

---

## 🔧 常见开发任务

### 1. 添加新 API 接口

按 **5步开发流程** 执行：
1. 创建实体类（Entity）
2. 创建 DTO 类
3. 创建枚举类（如需要）
4. 创建 Service 服务层
5. 创建 Controller 控制器

### 2. 配置数据库连接

**文件**：`appsettings.json`
```json
{
  "MasterSlaveConnectionStrings": [
    {
      "ConnectionString": "Server=127.0.0.1;Database=EasyProject;User ID=root;Password=xxx;Charset=utf8mb4;",
      "DbType": 0,
      "IsAutoCloseConnection": true,
      "InitKeyType": 1
    }
  ]
}
```

### 3. SqlSugar 使用示例

```csharp
// 查询列表
var list = await _db.Queryable<Entity>()
    .WhereIF(!string.IsNullOrEmpty(keyword), x => x.Name.Contains(keyword))
    .OrderByDescending(x => x.CreateTime)
    .ToPageListAsync(pageIndex, pageSize);

// 查询详情（Guid 主键）
var entity = await _db.Queryable<Entity>()
    .Where(x => x.Id == id)
    .FirstAsync();

// 插入
await _db.Insertable(entity).ExecuteCommandAsync();

// 更新
await _db.Updateable(entity).ExecuteCommandAsync();

// 删除
await _db.Deleteable<Entity>().Where(x => x.Id == id).ExecuteCommandAsync();
```

---

## 🔒 认证授权

### JWT Token 配置

```json
{
  "JWTTokenOptions": {
    "Audience": "http://localhost:7600",
    "Issuer": "http://localhost:7600",
    "SecurityKey": "your-secret-key-at-least-32-characters",
    "AccessTokenExpiration": 60,
    "RefreshTokenExpiration": 10080
  }
}
```

### Controller 使用

```csharp
[Authorize]  // 需要认证
public class XxxController : BaseController { }

[AllowAnonymous]  // 不需要认证
public class AuthController : BaseController { }
```

---

## 📝 微信小程序配置

```json
{
  "WeChat": {
    "AppId": "wx1234567890abcdef",
    "AppSecret": "your-app-secret",
    "UseMock": true  // true: 模拟数据，false: 真实微信 API
  },
  "WeChatPay": {
    "AppId": "wx1234567890abcdef",
    "MchId": "1234567890",
    "NotifyUrl": "https://your-domain/api/wechat/payment/notify",
    "UseMock": true
  }
}
```

详见 `docs/wechat-login-payment-enable.md`

---

## 📋 操作日志配置

```json
{
  "OperateLog": {
    "Enabled": true,
    "ExcludePaths": ["/api/auth/login", "/api/auth/refresh", "/swagger"],
    "ExcludeGet": true
  }
}
```

- POST/PUT/DELETE 请求自动记录
- 记录请求参数、响应结果、执行时长
- 敏感信息自动脱敏

---

## 🔑 Token 黑名单机制

```csharp
// 登出流程自动执行：
// 1. 将 AccessToken 加入黑名单（基于 jti）
// 2. 标记用户所有 Token 失效

// TokenBlacklistCache 方法
await _tokenBlacklistCache.AddToBlacklistAsync(jti, expiresIn);
bool isBlacklisted = await _tokenBlacklistCache.IsBlacklistedAsync(jti);
await _tokenBlacklistCache.InvalidateUserTokensAsync(userId);
```

---

## 📋 开发检查清单

| 序号 | 检查项 | 状态 |
|------|--------|------|
| 1 | 实体类放在 `Entitys/{模块名}/` 目录 | ☐ |
| 2 | 实体主键使用 Guid 类型（禁止 int/long） | ☐ |
| 3 | 实体命名空间使用 `EasyWeChatModels.Entitys` | ☐ |
| 4 | DTO 类放在 `Dto/{模块名}/` 目录 | ☐ |
| 5 | 一个 DTO 类一个文件 | ☐ |
| 6 | DTO 命名空间使用 `EasyWeChatModels.Dto` | ☐ |
| 7 | DTO 有完整 XML 注释 | ☐ |
| 8 | 枚举放在 `Enums/` 目录 | ☐ |
| 9 | 枚举命名空间使用 `EasyWeChatModels.Enums` | ☐ |
| 10 | 枚举值有 XML 注释 | ☐ |
| 11 | Service 使用属性注入（`get; set; = null!`） | ☐ |
| 12 | Controller 使用属性注入 | ☐ |
| 13 | Controller 继承 BaseController | ☐ |
| 14 | API 接口有 `[ProducesResponseType]` | ☐ |
| 15 | 异常使用 try-catch 处理 | ☐ |
| 16 | SQL 脚本使用 `DROP TABLE IF EXISTS` | ☐ |

---

## 📚 相关文档

| 文档 | 路径 |
|------|------|
| 微信登录支付配置 | `docs/wechat-login-payment-enable.md` |
| 技术栈分析 | `技术栈分析.md` |
