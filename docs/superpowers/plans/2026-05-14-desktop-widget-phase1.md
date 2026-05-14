# 桌面组件自动配置系统 - 阶段一：后端数据模型与API接口

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完成桌面组件配置系统的后端数据模型和API接口，包括实体类、枚举、DTO、Service和Controller

**Architecture:** 采用 SqlSugar ORM + Autofac 属性注入模式，遵循项目既有的分层架构（Entity → Dto → Service → Controller），主键使用 Guid 类型

**Tech Stack:** .NET 8.0 + ASP.NET Core + SqlSugar 5.1.4 + Autofac 8.0.0 + Mapster 10.0.0

---

## 文件结构

### 需要创建的文件

```
EasyWechatWeb/
├── EasyWeChatModels/
│   ├── Enums/
│   │   ├── WidgetType.cs              # 组件类型枚举
│   │   └── DataSourceType.cs          # 数据源类型枚举
│   ├── Entitys/Desktop/
│   │   ├── DesktopWidget.cs           # 组件实体
│   │   ├── RoleWidgetConfig.cs        # 角色组件配置实体
│   │   └── UserWidgetConfig.cs        # 用户组件配置实体
│   └── Dto/Desktop/
│   │   ├── DesktopWidgetDto.cs        # 组件查询DTO
│   │   ├── AddDesktopWidgetDto.cs     # 新增组件DTO
│   │   ├── UpdateDesktopWidgetDto.cs  # 更新组件DTO
│   │   ├── QueryDesktopWidgetDto.cs   # 组件查询参数DTO
│   │   ├── RoleWidgetConfigDto.cs     # 角色配置DTO
│   │   ├── AddRoleWidgetConfigDto.cs  # 保存角色配置DTO
│   │   ├── UserWidgetConfigDto.cs     # 用户配置DTO
│   │   ├── SaveUserWidgetConfigDto.cs # 保存用户配置DTO
│   │   └── UserDesktopDto.cs          # 用户桌面数据DTO
│
├── BusinessManager/Desktop/
│   ├── IService/
│   │   ├── IDesktopWidgetService.cs   # 组件服务接口
│   │   ├── IRoleWidgetConfigService.cs # 角色配置服务接口
│   │   └── IUserWidgetConfigService.cs # 用户配置服务接口
│   └── Service/
│   │   ├── DesktopWidgetService.cs    # 组件服务实现
│   │   ├── RoleWidgetConfigService.cs # 角色配置服务实现
│   │   └── UserWidgetConfigService.cs # 用户配置服务实现
│
└── EasyWeChatWeb/Controllers/Desktop/
    ├── DesktopWidgetController.cs     # 组件管理Controller
    ├── RoleWidgetConfigController.cs  # 角色配置Controller
    └── UserWidgetConfigController.cs  # 用户配置Controller
```

---

## Task 1: 创建组件类型枚举

**Files:**
- Create: `EasyWechatWeb/EasyWeChatModels/Enums/WidgetType.cs`

- [ ] **Step 1: 创建 WidgetType 枚举类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Enums/WidgetType.cs
namespace EasyWeChatModels.Enums;

/// <summary>
/// 桌面组件类型枚举
/// </summary>
public enum WidgetType
{
    /// <summary>统计卡片</summary>
    Card = 1,
    /// <summary>数据列表</summary>
    List = 2,
    /// <summary>图片展示</summary>
    Image = 3,
    /// <summary>图表统计</summary>
    Chart = 4
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatModels && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatModels/Enums/WidgetType.cs
git commit -m "feat(desktop): add WidgetType enum

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 2: 创建数据源类型枚举

**Files:**
- Create: `EasyWechatWeb/EasyWeChatModels/Enums/DataSourceType.cs`

- [ ] **Step 1: 创建 DataSourceType 枚举类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Enums/DataSourceType.cs
namespace EasyWeChatModels.Enums;

/// <summary>
/// 数据源类型枚举
/// </summary>
public enum DataSourceType
{
    /// <summary>API接口</summary>
    Api = 1,
    /// <summary>静态配置</summary>
    Static = 2,
    /// <summary>实时统计</summary>
    Statistics = 3
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatModels && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatModels/Enums/DataSourceType.cs
git commit -m "feat(desktop): add DataSourceType enum

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 3: 创建桌面组件实体

**Files:**
- Create: `EasyWechatWeb/EasyWeChatModels/Entitys/Desktop/DesktopWidget.cs`

- [ ] **Step 1: 创建 DesktopWidget 实体类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Entitys/Desktop/DesktopWidget.cs
using SqlSugar;
using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Entitys;

[SugarTable("DesktopWidget", "桌面组件表")]
public class DesktopWidget
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "组件名称", Length = 50)]
    public string Name { get; set; } = string.Empty;

    [SugarColumn(ColumnDescription = "组件类型")]
    public WidgetType Type { get; set; } = WidgetType.Card;

    [SugarColumn(ColumnDescription = "图标名称", Length = 50, IsNullable = true)]
    public string? Icon { get; set; }

    [SugarColumn(ColumnDescription = "默认宽度(栅格数1-12)")]
    public int DefaultWidth { get; set; } = 3;

    [SugarColumn(ColumnDescription = "默认高度(像素)")]
    public int DefaultHeight { get; set; } = 120;

    [SugarColumn(ColumnDescription = "数据源类型")]
    public DataSourceType DataSourceType { get; set; } = DataSourceType.Api;

    [SugarColumn(ColumnDescription = "数据源配置(JSON)", ColumnDataType = "text", IsNullable = true)]
    public string? DataSourceConfig { get; set; }

    [SugarColumn(ColumnDescription = "交互配置(JSON)", ColumnDataType = "text", IsNullable = true)]
    public string? InteractionConfig { get; set; }

    [SugarColumn(ColumnDescription = "状态: 0禁用 1启用")]
    public int Status { get; set; } = 1;

    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
    public DateTime? UpdateTime { get; set; }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatModels && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatModels/Entitys/Desktop/DesktopWidget.cs
git commit -m "feat(desktop): add DesktopWidget entity

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 4: 创建角色组件配置实体

**Files:**
- Create: `EasyWechatWeb/EasyWeChatModels/Entitys/Desktop/RoleWidgetConfig.cs`

- [ ] **Step 1: 创建 RoleWidgetConfig 实体类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Entitys/Desktop/RoleWidgetConfig.cs
using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("RoleWidgetConfig", "角色组件配置表")]
public class RoleWidgetConfig
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "角色ID")]
    public Guid RoleId { get; set; }

    [SugarColumn(ColumnDescription = "组件ID")]
    public Guid WidgetId { get; set; }

    [SugarColumn(ColumnDescription = "排序序号")]
    public int SortOrder { get; set; } = 0;

    [SugarColumn(ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatModels && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatModels/Entitys/Desktop/RoleWidgetConfig.cs
git commit -m "feat(desktop): add RoleWidgetConfig entity

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 5: 创建用户组件配置实体

**Files:**
- Create: `EasyWechatWeb/EasyWeChatModels/Entitys/Desktop/UserWidgetConfig.cs`

- [ ] **Step 1: 创建 UserWidgetConfig 实体类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Entitys/Desktop/UserWidgetConfig.cs
using SqlSugar;

namespace EasyWeChatModels.Entitys;

[SugarTable("UserWidgetConfig", "用户组件配置表")]
public class UserWidgetConfig
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [SugarColumn(ColumnDescription = "用户ID")]
    public Guid UserId { get; set; }

    [SugarColumn(ColumnDescription = "组件ID")]
    public Guid WidgetId { get; set; }

    [SugarColumn(ColumnDescription = "用户自定义宽度(栅格数)")]
    public int Width { get; set; } = 3;

    [SugarColumn(ColumnDescription = "是否启用")]
    public bool IsEnabled { get; set; } = true;

    [SugarColumn(ColumnDescription = "排序序号")]
    public int SortOrder { get; set; } = 0;

    [SugarColumn(ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; } = DateTime.Now;

    [SugarColumn(ColumnDescription = "更新时间", IsNullable = true)]
    public DateTime? UpdateTime { get; set; }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatModels && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatModels/Entitys/Desktop/UserWidgetConfig.cs
git commit -m "feat(desktop): add UserWidgetConfig entity

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 6: 创建组件查询DTO

**Files:**
- Create: `EasyWechatWeb/EasyWeChatModels/Dto/Desktop/DesktopWidgetDto.cs`

- [ ] **Step 1: 创建 DesktopWidgetDto 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Dto/Desktop/DesktopWidgetDto.cs
using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 桌面组件 DTO
/// </summary>
public class DesktopWidgetDto
{
    /// <summary>
    /// ID
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型
    /// </summary>
    public WidgetType Type { get; set; }

    /// <summary>
    /// 图标名称
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 默认宽度(栅格数)
    /// </summary>
    public int DefaultWidth { get; set; }

    /// <summary>
    /// 默认高度(像素)
    /// </summary>
    public int DefaultHeight { get; set; }

    /// <summary>
    /// 数据源类型
    /// </summary>
    public DataSourceType DataSourceType { get; set; }

    /// <summary>
    /// 数据源配置(JSON)
    /// </summary>
    public string? DataSourceConfig { get; set; }

    /// <summary>
    /// 交互配置(JSON)
    /// </summary>
    public string? InteractionConfig { get; set; }

    /// <summary>
    /// 状态: 0禁用 1启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatModels && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatModels/Dto/Desktop/DesktopWidgetDto.cs
git commit -m "feat(desktop): add DesktopWidgetDto

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 7: 创建新增组件DTO

**Files:**
- Create: `EasyWechatWeb/EasyWeChatModels/Dto/Desktop/AddDesktopWidgetDto.cs`

- [ ] **Step 1: 创建 AddDesktopWidgetDto 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Dto/Desktop/AddDesktopWidgetDto.cs
using System.ComponentModel.DataAnnotations;
using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 新增桌面组件 DTO
/// </summary>
public class AddDesktopWidgetDto
{
    /// <summary>
    /// 组件名称
    /// </summary>
    [Required(ErrorMessage = "组件名称不能为空")]
    [MaxLength(50, ErrorMessage = "组件名称最长50字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型
    /// </summary>
    [Required(ErrorMessage = "组件类型不能为空")]
    public WidgetType Type { get; set; }

    /// <summary>
    /// 图标名称
    /// </summary>
    [MaxLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// 默认宽度(栅格数1-12)
    /// </summary>
    [Range(1, 12, ErrorMessage = "宽度必须在1-12之间")]
    public int DefaultWidth { get; set; } = 3;

    /// <summary>
    /// 默认高度(像素)
    /// </summary>
    [Range(60, 500, ErrorMessage = "高度必须在60-500之间")]
    public int DefaultHeight { get; set; } = 120;

    /// <summary>
    /// 数据源类型
    /// </summary>
    public DataSourceType DataSourceType { get; set; } = DataSourceType.Api;

    /// <summary>
    /// 数据源配置(JSON)
    /// </summary>
    public string? DataSourceConfig { get; set; }

    /// <summary>
    /// 交互配置(JSON)
    /// </summary>
    public string? InteractionConfig { get; set; }

    /// <summary>
    /// 状态: 0禁用 1启用
    /// </summary>
    public int Status { get; set; } = 1;
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatModels && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatModels/Dto/Desktop/AddDesktopWidgetDto.cs
git commit -m "feat(desktop): add AddDesktopWidgetDto

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 8: 创建更新组件DTO

**Files:**
- Create: `EasyWechatWeb/EasyWeChatModels/Dto/Desktop/UpdateDesktopWidgetDto.cs`

- [ ] **Step 1: 创建 UpdateDesktopWidgetDto 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Dto/Desktop/UpdateDesktopWidgetDto.cs
using System.ComponentModel.DataAnnotations;
using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 更新桌面组件 DTO
/// </summary>
public class UpdateDesktopWidgetDto
{
    /// <summary>
    /// ID
    /// </summary>
    [Required(ErrorMessage = "ID不能为空")]
    public Guid Id { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    [Required(ErrorMessage = "组件名称不能为空")]
    [MaxLength(50, ErrorMessage = "组件名称最长50字符")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型
    /// </summary>
    [Required(ErrorMessage = "组件类型不能为空")]
    public WidgetType Type { get; set; }

    /// <summary>
    /// 图标名称
    /// </summary>
    [MaxLength(50)]
    public string? Icon { get; set; }

    /// <summary>
    /// 默认宽度(栅格数1-12)
    /// </summary>
    [Range(1, 12, ErrorMessage = "宽度必须在1-12之间")]
    public int DefaultWidth { get; set; } = 3;

    /// <summary>
    /// 默认高度(像素)
    /// </summary>
    [Range(60, 500, ErrorMessage = "高度必须在60-500之间")]
    public int DefaultHeight { get; set; } = 120;

    /// <summary>
    /// 数据源类型
    /// </summary>
    public DataSourceType DataSourceType { get; set; } = DataSourceType.Api;

    /// <summary>
    /// 数据源配置(JSON)
    /// </summary>
    public string? DataSourceConfig { get; set; }

    /// <summary>
    /// 交互配置(JSON)
    /// </summary>
    public string? InteractionConfig { get; set; }

    /// <summary>
    /// 状态: 0禁用 1启用
    /// </summary>
    public int Status { get; set; } = 1;
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatModels && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatModels/Dto/Desktop/UpdateDesktopWidgetDto.cs
git commit -m "feat(desktop): add UpdateDesktopWidgetDto

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 9: 创建组件查询参数DTO

**Files:**
- Create: `EasyWechatWeb/EasyWeChatModels/Dto/Desktop/QueryDesktopWidgetDto.cs`

- [ ] **Step 1: 创建 QueryDesktopWidgetDto 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Dto/Desktop/QueryDesktopWidgetDto.cs
using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 桌面组件查询参数 DTO
/// </summary>
public class QueryDesktopWidgetDto
{
    /// <summary>
    /// 页码
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页数量
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// 组件名称(模糊搜索)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 组件类型
    /// </summary>
    public WidgetType? Type { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public int? Status { get; set; }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatModels && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatModels/Dto/Desktop/QueryDesktopWidgetDto.cs
git commit -m "feat(desktop): add QueryDesktopWidgetDto

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 10: 创建角色配置相关DTO

**Files:**
- Create: `EasyWechatWeb/EasyWeChatModels/Dto/Desktop/RoleWidgetConfigDto.cs`
- Create: `EasyWechatWeb/EasyWeChatModels/Dto/Desktop/AddRoleWidgetConfigDto.cs`

- [ ] **Step 1: 创建 RoleWidgetConfigDto 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Dto/Desktop/RoleWidgetConfigDto.cs
using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 角色组件配置 DTO
/// </summary>
public class RoleWidgetConfigDto
{
    /// <summary>
    /// 配置ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// 组件ID
    /// </summary>
    public Guid WidgetId { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    public string WidgetName { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型
    /// </summary>
    public WidgetType WidgetType { get; set; }

    /// <summary>
    /// 默认宽度
    /// </summary>
    public int DefaultWidth { get; set; }

    /// <summary>
    /// 默认高度
    /// </summary>
    public int DefaultHeight { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
}
```

- [ ] **Step 2: 创建 AddRoleWidgetConfigDto 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Dto/Desktop/AddRoleWidgetConfigDto.cs
using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 保存角色组件配置 DTO
/// </summary>
public class AddRoleWidgetConfigDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [Required(ErrorMessage = "角色ID不能为空")]
    public Guid RoleId { get; set; }

    /// <summary>
    /// 组件配置列表
    /// </summary>
    public List<RoleWidgetConfigItem> Widgets { get; set; } = new();
}

/// <summary>
/// 角色组件配置项
/// </summary>
public class RoleWidgetConfigItem
{
    /// <summary>
    /// 组件ID
    /// </summary>
    public Guid WidgetId { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
}
```

- [ ] **Step 3: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatModels && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 4: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatModels/Dto/Desktop/RoleWidgetConfigDto.cs
git add EasyWechatWeb/EasyWeChatModels/Dto/Desktop/AddRoleWidgetConfigDto.cs
git commit -m "feat(desktop): add RoleWidgetConfig DTOs

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 11: 创建用户配置相关DTO

**Files:**
- Create: `EasyWechatWeb/EasyWeChatModels/Dto/Desktop/UserWidgetConfigDto.cs`
- Create: `EasyWechatWeb/EasyWeChatModels/Dto/Desktop/SaveUserWidgetConfigDto.cs`
- Create: `EasyWechatWeb/EasyWeChatModels/Dto/Desktop/UserDesktopDto.cs`

- [ ] **Step 1: 创建 UserWidgetConfigDto 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Dto/Desktop/UserWidgetConfigDto.cs
using EasyWeChatModels.Enums;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 用户组件配置 DTO
/// </summary>
public class UserWidgetConfigDto
{
    /// <summary>
    /// 配置ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 组件ID
    /// </summary>
    public Guid WidgetId { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    public string WidgetName { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型
    /// </summary>
    public WidgetType WidgetType { get; set; }

    /// <summary>
    /// 用户自定义宽度(栅格数)
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 默认高度
    /// </summary>
    public int DefaultHeight { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 数据源配置
    /// </summary>
    public string? DataSourceConfig { get; set; }

    /// <summary>
    /// 交互配置
    /// </summary>
    public string? InteractionConfig { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; }
}
```

- [ ] **Step 2: 创建 SaveUserWidgetConfigDto 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Dto/Desktop/SaveUserWidgetConfigDto.cs
using System.ComponentModel.DataAnnotations;

namespace EasyWeChatModels.Dto;

/// <summary>
/// 保存用户桌面配置 DTO
/// </summary>
public class SaveUserWidgetConfigDto
{
    /// <summary>
    /// 组件配置列表
    /// </summary>
    public List<UserWidgetConfigItem> Widgets { get; set; } = new();
}

/// <summary>
/// 用户组件配置项
/// </summary>
public class UserWidgetConfigItem
{
    /// <summary>
    /// 组件ID
    /// </summary>
    public Guid WidgetId { get; set; }

    /// <summary>
    /// 宽度(栅格数1-12)
    /// </summary>
    public int Width { get; set; } = 3;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 排序序号
    /// </summary>
    public int SortOrder { get; set; } = 0;
}
```

- [ ] **Step 3: 创建 UserDesktopDto 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatModels/Dto/Desktop/UserDesktopDto.cs
namespace EasyWeChatModels.Dto;

/// <summary>
/// 用户桌面数据 DTO
/// </summary>
public class UserDesktopDto
{
    /// <summary>
    /// 用户已启用的组件列表
    /// </summary>
    public List<UserWidgetConfigDto> Widgets { get; set; } = new();

    /// <summary>
    /// 用户可用的全部组件列表(角色组件库)
    /// </summary>
    public List<AvailableWidgetDto> AvailableWidgets { get; set; } = new();
}

/// <summary>
/// 可用组件 DTO
/// </summary>
public class AvailableWidgetDto
{
    /// <summary>
    /// 组件ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 组件名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 组件类型
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 默认宽度
    /// </summary>
    public int DefaultWidth { get; set; }

    /// <summary>
    /// 默认高度
    /// </summary>
    public int DefaultHeight { get; set; }

    /// <summary>
    /// 用户是否已启用
    /// </summary>
    public bool IsUserEnabled { get; set; }
}
```

- [ ] **Step 4: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatModels && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 5: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatModels/Dto/Desktop/UserWidgetConfigDto.cs
git add EasyWechatWeb/EasyWeChatModels/Dto/Desktop/SaveUserWidgetConfigDto.cs
git add EasyWechatWeb/EasyWeChatModels/Dto/Desktop/UserDesktopDto.cs
git commit -m "feat(desktop): add UserWidgetConfig DTOs

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 12: 创建组件服务接口

**Files:**
- Create: `EasyWechatWeb/BusinessManager/Desktop/IService/IDesktopWidgetService.cs`

- [ ] **Step 1: 创建 IDesktopWidgetService 接口**

```csharp
// 文件: EasyWechatWeb/BusinessManager/Desktop/IService/IDesktopWidgetService.cs
using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Desktop.IService;

public interface IDesktopWidgetService : IBaseService
{
    /// <summary>
    /// 获取组件列表(分页)
    /// </summary>
    Task<(List<DesktopWidgetDto> list, int total)> GetListAsync(QueryDesktopWidgetDto query);

    /// <summary>
    /// 获取组件详情
    /// </summary>
    Task<DesktopWidgetDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建组件
    /// </summary>
    Task<Guid> AddAsync(AddDesktopWidgetDto dto);

    /// <summary>
    /// 更新组件
    /// </summary>
    Task<bool> UpdateAsync(UpdateDesktopWidgetDto dto);

    /// <summary>
    /// 删除组件
    /// </summary>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 获取所有启用的组件列表(用于分配)
    /// </summary>
    Task<List<DesktopWidgetDto>> GetEnabledListAsync();
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/BusinessManager && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/BusinessManager/Desktop/IService/IDesktopWidgetService.cs
git commit -m "feat(desktop): add IDesktopWidgetService interface

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 13: 创建角色配置服务接口

**Files:**
- Create: `EasyWechatWeb/BusinessManager/Desktop/IService/IRoleWidgetConfigService.cs`

- [ ] **Step 1: 创建 IRoleWidgetConfigService 接口**

```csharp
// 文件: EasyWechatWeb/BusinessManager/Desktop/IService/IRoleWidgetConfigService.cs
using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Desktop.IService;

public interface IRoleWidgetConfigService : IBaseService
{
    /// <summary>
    /// 获取角色的组件配置列表
    /// </summary>
    Task<List<RoleWidgetConfigDto>> GetByRoleIdAsync(Guid roleId);

    /// <summary>
    /// 保存角色组件配置(批量)
    /// </summary>
    Task<bool> SaveAsync(AddRoleWidgetConfigDto dto);

    /// <summary>
    /// 获取角色可用的组件列表(用于分配选择)
    /// </summary>
    Task<List<AvailableWidgetDto>> GetAvailableWidgetsAsync(Guid roleId);
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/BusinessManager && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/BusinessManager/Desktop/IService/IRoleWidgetConfigService.cs
git commit -m "feat(desktop): add IRoleWidgetConfigService interface

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 14: 创建用户配置服务接口

**Files:**
- Create: `EasyWechatWeb/BusinessManager/Desktop/IService/IUserWidgetConfigService.cs`

- [ ] **Step 1: 创建 IUserWidgetConfigService 接口**

```csharp
// 文件: EasyWechatWeb/BusinessManager/Desktop/IService/IUserWidgetConfigService.cs
using CommonManager.Base;
using EasyWeChatModels.Dto;

namespace BusinessManager.Desktop.IService;

public interface IUserWidgetConfigService : IBaseService
{
    /// <summary>
    /// 获取用户桌面数据
    /// </summary>
    Task<UserDesktopDto> GetUserDesktopAsync(Guid userId);

    /// <summary>
    /// 保存用户桌面配置
    /// </summary>
    Task<bool> SaveAsync(Guid userId, SaveUserWidgetConfigDto dto);

    /// <summary>
    /// 重置用户桌面为角色默认布局
    /// </summary>
    Task<bool> ResetAsync(Guid userId);

    /// <summary>
    /// 初始化用户桌面(从角色模板复制)
    /// </summary>
    Task<bool> InitFromRoleAsync(Guid userId, Guid roleId);
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/BusinessManager && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/BusinessManager/Desktop/IService/IUserWidgetConfigService.cs
git commit -m "feat(desktop): add IUserWidgetConfigService interface

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 15: 创建组件服务实现

**Files:**
- Create: `EasyWechatWeb/BusinessManager/Desktop/Service/DesktopWidgetService.cs`

- [ ] **Step 1: 创建 DesktopWidgetService 实现类**

```csharp
// 文件: EasyWechatWeb/BusinessManager/Desktop/Service/DesktopWidgetService.cs
using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Desktop.Service;

public class DesktopWidgetService : BaseService<DesktopWidget>, IDesktopWidgetService
{
    public ILogger<DesktopWidgetService> _logger { get; set; } = null!;
    public ISqlSugarClient _db { get; set; } = null!;

    public async Task<(List<DesktopWidgetDto> list, int total)> GetListAsync(QueryDesktopWidgetDto query)
    {
        var queryBuilder = _db.Queryable<DesktopWidget>()
            .WhereIF(!string.IsNullOrEmpty(query.Name), x => x.Name.Contains(query.Name!))
            .WhereIF(query.Type.HasValue, x => x.Type == query.Type!.Value)
            .WhereIF(query.Status.HasValue, x => x.Status == query.Status!.Value)
            .OrderByDescending(x => x.CreateTime);

        var total = await queryBuilder.CountAsync();
        var list = await queryBuilder
            .ToPageListAsync(query.PageIndex, query.PageSize);

        return (list.Adapt<List<DesktopWidgetDto>>(), total);
    }

    public async Task<DesktopWidgetDto?> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<DesktopWidget>()
            .Where(x => x.Id == id)
            .FirstAsync();
        return entity?.Adapt<DesktopWidgetDto>();
    }

    public async Task<Guid> AddAsync(AddDesktopWidgetDto dto)
    {
        var entity = dto.Adapt<DesktopWidget>();
        entity.Id = Guid.NewGuid();
        entity.CreateTime = DateTime.Now;
        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    public async Task<bool> UpdateAsync(UpdateDesktopWidgetDto dto)
    {
        var entity = await _db.Queryable<DesktopWidget>()
            .Where(x => x.Id == dto.Id)
            .FirstAsync();
        if (entity == null) return false;

        dto.Adapt(entity);
        entity.UpdateTime = DateTime.Now;
        await _db.Updateable(entity).ExecuteCommandAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _db.Deleteable<DesktopWidget>()
            .Where(x => x.Id == id)
            .ExecuteCommandAsync();
        return result > 0;
    }

    public async Task<List<DesktopWidgetDto>> GetEnabledListAsync()
    {
        var list = await _db.Queryable<DesktopWidget>()
            .Where(x => x.Status == 1)
            .OrderBy(x => x.CreateTime)
            .ToListAsync();
        return list.Adapt<List<DesktopWidgetDto>>();
    }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/BusinessManager && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/BusinessManager/Desktop/Service/DesktopWidgetService.cs
git commit -m "feat(desktop): implement DesktopWidgetService

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 16: 创建角色配置服务实现

**Files:**
- Create: `EasyWechatWeb/BusinessManager/Desktop/Service/RoleWidgetConfigService.cs`

- [ ] **Step 1: 创建 RoleWidgetConfigService 实现类**

```csharp
// 文件: EasyWechatWeb/BusinessManager/Desktop/Service/RoleWidgetConfigService.cs
using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Desktop.Service;

public class RoleWidgetConfigService : BaseService<RoleWidgetConfig>, IRoleWidgetConfigService
{
    public ILogger<RoleWidgetConfigService> _logger { get; set; } = null!;
    public ISqlSugarClient _db { get; set; } = null!;

    public async Task<List<RoleWidgetConfigDto>> GetByRoleIdAsync(Guid roleId)
    {
        var list = await _db.Queryable<RoleWidgetConfig, DesktopWidget>(
            (rc, w) => new JoinQueryInfos(JoinType.Left, rc.WidgetId == w.Id))
            .Where((rc, w) => rc.RoleId == roleId)
            .OrderBy((rc, w) => rc.SortOrder)
            .Select((rc, w) => new RoleWidgetConfigDto
            {
                Id = rc.Id,
                RoleId = rc.RoleId,
                WidgetId = rc.WidgetId,
                WidgetName = w.Name,
                WidgetType = w.Type,
                DefaultWidth = w.DefaultWidth,
                DefaultHeight = w.DefaultHeight,
                SortOrder = rc.SortOrder,
                IsEnabled = rc.IsEnabled
            })
            .ToListAsync();
        return list;
    }

    public async Task<bool> SaveAsync(AddRoleWidgetConfigDto dto)
    {
        // 先删除该角色的所有配置
        await _db.Deleteable<RoleWidgetConfig>()
            .Where(x => x.RoleId == dto.RoleId)
            .ExecuteCommandAsync();

        // 批量插入新配置
        var entities = dto.Widgets.Select(w => new RoleWidgetConfig
        {
            Id = Guid.NewGuid(),
            RoleId = dto.RoleId,
            WidgetId = w.WidgetId,
            SortOrder = w.SortOrder,
            IsEnabled = w.IsEnabled,
            CreateTime = DateTime.Now
        }).ToList();

        if (entities.Count > 0)
        {
            await _db.Insertable(entities).ExecuteCommandAsync();
        }
        return true;
    }

    public async Task<List<AvailableWidgetDto>> GetAvailableWidgetsAsync(Guid roleId)
    {
        // 获取所有启用的组件
        var allWidgets = await _db.Queryable<DesktopWidget>()
            .Where(x => x.Status == 1)
            .ToListAsync();

        // 获取该角色已分配的组件
        var assignedIds = await _db.Queryable<RoleWidgetConfig>()
            .Where(x => x.RoleId == roleId && x.IsEnabled)
            .Select(x => x.WidgetId)
            .ToListAsync();

        return allWidgets.Select(w => new AvailableWidgetDto
        {
            Id = w.Id,
            Name = w.Name,
            Type = (int)w.Type,
            Icon = w.Icon,
            DefaultWidth = w.DefaultWidth,
            DefaultHeight = w.DefaultHeight,
            IsUserEnabled = assignedIds.Contains(w.Id)
        }).ToList();
    }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/BusinessManager && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/BusinessManager/Desktop/Service/RoleWidgetConfigService.cs
git commit -m "feat(desktop): implement RoleWidgetConfigService

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 17: 创建用户配置服务实现

**Files:**
- Create: `EasyWechatWeb/BusinessManager/Desktop/Service/UserWidgetConfigService.cs`

- [ ] **Step 1: 创建 UserWidgetConfigService 实现类**

```csharp
// 文件: EasyWechatWeb/BusinessManager/Desktop/Service/UserWidgetConfigService.cs
using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using EasyWeChatModels.Entitys;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace BusinessManager.Desktop.Service;

public class UserWidgetConfigService : BaseService<UserWidgetConfig>, IUserWidgetConfigService
{
    public ILogger<UserWidgetConfigService> _logger { get; set; } = null!;
    public ISqlSugarClient _db { get; set; } = null!;

    public async Task<UserDesktopDto> GetUserDesktopAsync(Guid userId)
    {
        // 获取用户已启用的组件配置
        var userConfigs = await _db.Queryable<UserWidgetConfig, DesktopWidget>(
            (uc, w) => new JoinQueryInfos(JoinType.Left, uc.WidgetId == w.Id))
            .Where((uc, w) => uc.UserId == userId && uc.IsEnabled)
            .OrderBy((uc, w) => uc.SortOrder)
            .Select((uc, w) => new UserWidgetConfigDto
            {
                Id = uc.Id,
                WidgetId = uc.WidgetId,
                WidgetName = w.Name,
                WidgetType = w.Type,
                Width = uc.Width,
                DefaultHeight = w.DefaultHeight,
                Icon = w.Icon,
                DataSourceConfig = w.DataSourceConfig,
                InteractionConfig = w.InteractionConfig,
                IsEnabled = uc.IsEnabled,
                SortOrder = uc.SortOrder
            })
            .ToListAsync();

        // 获取用户角色
        var userRole = await _db.Queryable<UserRole>()
            .Where(x => x.UserId == userId)
            .FirstAsync();

        var availableWidgets = new List<AvailableWidgetDto>();
        if (userRole != null)
        {
            // 获取角色可用的组件
            availableWidgets = await _db.Queryable<RoleWidgetConfig, DesktopWidget>(
                (rc, w) => new JoinQueryInfos(JoinType.Left, rc.WidgetId == w.Id))
                .Where((rc, w) => rc.RoleId == userRole.RoleId && rc.IsEnabled && w.Status == 1)
                .Select((rc, w) => new AvailableWidgetDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Type = (int)w.Type,
                    Icon = w.Icon,
                    DefaultWidth = w.DefaultWidth,
                    DefaultHeight = w.DefaultHeight,
                    IsUserEnabled = userConfigs.Any(uc => uc.WidgetId == w.Id)
                })
                .ToListAsync();
        }

        return new UserDesktopDto
        {
            Widgets = userConfigs,
            AvailableWidgets = availableWidgets
        };
    }

    public async Task<bool> SaveAsync(Guid userId, SaveUserWidgetConfigDto dto)
    {
        // 先删除该用户的所有配置
        await _db.Deleteable<UserWidgetConfig>()
            .Where(x => x.UserId == userId)
            .ExecuteCommandAsync();

        // 批量插入新配置
        var entities = dto.Widgets.Select(w => new UserWidgetConfig
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WidgetId = w.WidgetId,
            Width = w.Width,
            IsEnabled = w.IsEnabled,
            SortOrder = w.SortOrder,
            CreateTime = DateTime.Now,
            UpdateTime = DateTime.Now
        }).ToList();

        if (entities.Count > 0)
        {
            await _db.Insertable(entities).ExecuteCommandAsync();
        }
        return true;
    }

    public async Task<bool> ResetAsync(Guid userId)
    {
        // 获取用户角色
        var userRole = await _db.Queryable<UserRole>()
            .Where(x => x.UserId == userId)
            .FirstAsync();
        if (userRole == null) return false;

        // 从角色模板初始化
        return await InitFromRoleAsync(userId, userRole.RoleId);
    }

    public async Task<bool> InitFromRoleAsync(Guid userId, Guid roleId)
    {
        // 获取角色的组件配置
        var roleConfigs = await _db.Queryable<RoleWidgetConfig>()
            .Where(x => x.RoleId == roleId && x.IsEnabled)
            .ToListAsync();

        // 获取组件的默认尺寸
        var widgets = await _db.Queryable<DesktopWidget>()
            .Where(x => x.Status == 1)
            .ToListAsync();

        // 先删除用户的现有配置
        await _db.Deleteable<UserWidgetConfig>()
            .Where(x => x.UserId == userId)
            .ExecuteCommandAsync();

        // 从角色模板复制配置
        var entities = roleConfigs.Select(rc =>
        {
            var widget = widgets.FirstOrDefault(w => w.Id == rc.WidgetId);
            return new UserWidgetConfig
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                WidgetId = rc.WidgetId,
                Width = widget?.DefaultWidth ?? 3,
                IsEnabled = true,
                SortOrder = rc.SortOrder,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
        }).ToList();

        if (entities.Count > 0)
        {
            await _db.Insertable(entities).ExecuteCommandAsync();
        }
        return true;
    }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/BusinessManager && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/BusinessManager/Desktop/Service/UserWidgetConfigService.cs
git commit -m "feat(desktop): implement UserWidgetConfigService

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 18: 创建组件管理Controller

**Files:**
- Create: `EasyWechatWeb/EasyWeChatWeb/Controllers/Desktop/DesktopWidgetController.cs`

- [ ] **Step 1: 创建 DesktopWidgetController 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatWeb/Controllers/Desktop/DesktopWidgetController.cs
using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EasyWeChatWeb.Controllers.Desktop;

[ApiController]
[Route("api/desktop/widget")]
[Authorize]
public class DesktopWidgetController : BaseController
{
    public IDesktopWidgetService _widgetService { get; set; } = null!;
    public ILogger<DesktopWidgetController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取组件列表(分页)
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(typeof(ApiResponse<PageResult<DesktopWidgetDto>>), 200)]
    public async Task<ApiResponse<PageResult<DesktopWidgetDto>>> GetList([FromQuery] QueryDesktopWidgetDto query)
    {
        try
        {
            var (list, total) = await _widgetService.GetListAsync(query);
            return Success(new PageResult<DesktopWidgetDto>
            {
                List = list,
                Total = total,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取组件列表失败");
            return Error<PageResult<DesktopWidgetDto>>("获取组件列表失败");
        }
    }

    /// <summary>
    /// 获取组件详情
    /// </summary>
    [HttpGet("detail/{id}")]
    [ProducesResponseType(typeof(ApiResponse<DesktopWidgetDto>), 200)]
    public async Task<ApiResponse<DesktopWidgetDto>> GetById(string id)
    {
        try
        {
            var widgetId = Guid.Parse(id);
            var dto = await _widgetService.GetByIdAsync(widgetId);
            if (dto == null)
                return Error<DesktopWidgetDto>("组件不存在");
            return Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取组件详情失败");
            return Error<DesktopWidgetDto>("获取组件详情失败");
        }
    }

    /// <summary>
    /// 创建组件
    /// </summary>
    [HttpPost("add")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 200)]
    public async Task<ApiResponse<Guid>> Add([FromBody] AddDesktopWidgetDto dto)
    {
        try
        {
            var id = await _widgetService.AddAsync(dto);
            return Success(id, "创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建组件失败");
            return Error<Guid>("创建组件失败");
        }
    }

    /// <summary>
    /// 更新组件
    /// </summary>
    [HttpPost("update")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Update([FromBody] UpdateDesktopWidgetDto dto)
    {
        try
        {
            var result = await _widgetService.UpdateAsync(dto);
            if (!result)
                return Error<bool>("组件不存在");
            return Success(result, "更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新组件失败");
            return Error<bool>("更新组件失败");
        }
    }

    /// <summary>
    /// 删除组件
    /// </summary>
    [HttpDelete("delete/{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Delete(string id)
    {
        try
        {
            var widgetId = Guid.Parse(id);
            var result = await _widgetService.DeleteAsync(widgetId);
            if (!result)
                return Error<bool>("组件不存在");
            return Success(result, "删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除组件失败");
            return Error<bool>("删除组件失败");
        }
    }

    /// <summary>
    /// 获取所有启用的组件列表(用于分配)
    /// </summary>
    [HttpGet("enabled-list")]
    [ProducesResponseType(typeof(ApiResponse<List<DesktopWidgetDto>>), 200)]
    public async Task<ApiResponse<List<DesktopWidgetDto>>> GetEnabledList()
    {
        try
        {
            var list = await _widgetService.GetEnabledListAsync();
            return Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取启用组件列表失败");
            return Error<List<DesktopWidgetDto>>("获取启用组件列表失败");
        }
    }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatWeb && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatWeb/Controllers/Desktop/DesktopWidgetController.cs
git commit -m "feat(desktop): add DesktopWidgetController

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 19: 创建角色配置Controller

**Files:**
- Create: `EasyWechatWeb/EasyWeChatWeb/Controllers/Desktop/RoleWidgetConfigController.cs`

- [ ] **Step 1: 创建 RoleWidgetConfigController 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatWeb/Controllers/Desktop/RoleWidgetConfigController.cs
using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EasyWeChatWeb.Controllers.Desktop;

[ApiController]
[Route("api/desktop/role-config")]
[Authorize]
public class RoleWidgetConfigController : BaseController
{
    public IRoleWidgetConfigService _roleConfigService { get; set; } = null!;
    public ILogger<RoleWidgetConfigController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取角色的组件配置列表
    /// </summary>
    [HttpGet("list/{roleId}")]
    [ProducesResponseType(typeof(ApiResponse<List<RoleWidgetConfigDto>>), 200)]
    public async Task<ApiResponse<List<RoleWidgetConfigDto>>> GetByRoleId(string roleId)
    {
        try
        {
            var id = Guid.Parse(roleId);
            var list = await _roleConfigService.GetByRoleIdAsync(id);
            return Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取角色组件配置失败");
            return Error<List<RoleWidgetConfigDto>>("获取角色组件配置失败");
        }
    }

    /// <summary>
    /// 保存角色组件配置
    /// </summary>
    [HttpPost("save")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Save([FromBody] AddRoleWidgetConfigDto dto)
    {
        try
        {
            var result = await _roleConfigService.SaveAsync(dto);
            return Success(result, "保存成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存角色组件配置失败");
            return Error<bool>("保存角色组件配置失败");
        }
    }

    /// <summary>
    /// 获取角色可用的组件列表(用于分配选择)
    /// </summary>
    [HttpGet("available-widgets/{roleId}")]
    [ProducesResponseType(typeof(ApiResponse<List<AvailableWidgetDto>>), 200)]
    public async Task<ApiResponse<List<AvailableWidgetDto>>> GetAvailableWidgets(string roleId)
    {
        try
        {
            var id = Guid.Parse(roleId);
            var list = await _roleConfigService.GetAvailableWidgetsAsync(id);
            return Success(list);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取可用组件列表失败");
            return Error<List<AvailableWidgetDto>>("获取可用组件列表失败");
        }
    }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatWeb && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatWeb/Controllers/Desktop/RoleWidgetConfigController.cs
git commit -m "feat(desktop): add RoleWidgetConfigController

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 20: 创建用户配置Controller

**Files:**
- Create: `EasyWechatWeb/EasyWeChatWeb/Controllers/Desktop/UserWidgetConfigController.cs`

- [ ] **Step 1: 创建 UserWidgetConfigController 类**

```csharp
// 文件: EasyWechatWeb/EasyWeChatWeb/Controllers/Desktop/UserWidgetConfigController.cs
using BusinessManager.Desktop.IService;
using CommonManager.Base;
using EasyWeChatModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace EasyWeChatWeb.Controllers.Desktop;

[ApiController]
[Route("api/desktop/user-config")]
[Authorize]
public class UserWidgetConfigController : BaseController
{
    public IUserWidgetConfigService _userConfigService { get; set; } = null!;
    public ILogger<UserWidgetConfigController> _logger { get; set; } = null!;

    /// <summary>
    /// 获取当前用户的桌面数据
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(ApiResponse<UserDesktopDto>), 200)]
    public async Task<ApiResponse<UserDesktopDto>> GetMyDesktop()
    {
        try
        {
            var userId = GetCurrentUserId();
            var dto = await _userConfigService.GetUserDesktopAsync(userId);
            return Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取用户桌面数据失败");
            return Error<UserDesktopDto>("获取用户桌面数据失败");
        }
    }

    /// <summary>
    /// 保存用户桌面配置
    /// </summary>
    [HttpPost("save")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Save([FromBody] SaveUserWidgetConfigDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _userConfigService.SaveAsync(userId, dto);
            return Success(result, "保存成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存用户桌面配置失败");
            return Error<bool>("保存用户桌面配置失败");
        }
    }

    /// <summary>
    /// 重置用户桌面为角色默认布局
    /// </summary>
    [HttpPost("reset")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<ApiResponse<bool>> Reset()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _userConfigService.ResetAsync(userId);
            return Success(result, "重置成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重置用户桌面失败");
            return Error<bool>("重置用户桌面失败");
        }
    }

    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim!);
    }
}
```

- [ ] **Step 2: 验证编译通过**

Run: `cd EasyWechatWeb/EasyWeChatWeb && dotnet build --no-restore`
Expected: Build succeeded

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatWeb/Controllers/Desktop/UserWidgetConfigController.cs
git commit -m "feat(desktop): add UserWidgetConfigController

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 21: 创建数据库迁移SQL

**Files:**
- Create: `EasyWechatWeb/EasyWeChatWeb/Scripts/desktop_widget_tables.sql`

- [ ] **Step 1: 创建SQL脚本**

```sql
-- 文件: EasyWechatWeb/EasyWeChatWeb/Scripts/desktop_widget_tables.sql
-- 桌面组件配置系统数据库表

-- 组件表
DROP TABLE IF EXISTS `DesktopWidget`;
CREATE TABLE `DesktopWidget` (
    `Id` CHAR(36) NOT NULL,
    `Name` VARCHAR(50) NOT NULL,
    `Type` INT NOT NULL DEFAULT 1,
    `Icon` VARCHAR(50) NULL,
    `DefaultWidth` INT NOT NULL DEFAULT 3,
    `DefaultHeight` INT NOT NULL DEFAULT 120,
    `DataSourceType` INT NOT NULL DEFAULT 1,
    `DataSourceConfig` TEXT NULL,
    `InteractionConfig` TEXT NULL,
    `Status` INT NOT NULL DEFAULT 1,
    `CreateTime` DATETIME NOT NULL,
    `UpdateTime` DATETIME NULL,
    PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='桌面组件表';

-- 角色组件配置表
DROP TABLE IF EXISTS `RoleWidgetConfig`;
CREATE TABLE `RoleWidgetConfig` (
    `Id` CHAR(36) NOT NULL,
    `RoleId` CHAR(36) NOT NULL,
    `WidgetId` CHAR(36) NOT NULL,
    `SortOrder` INT NOT NULL DEFAULT 0,
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1,
    `CreateTime` DATETIME NOT NULL,
    PRIMARY KEY (`Id`),
    INDEX `idx_role_id` (`RoleId`),
    INDEX `idx_widget_id` (`WidgetId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='角色组件配置表';

-- 用户组件配置表
DROP TABLE IF EXISTS `UserWidgetConfig`;
CREATE TABLE `UserWidgetConfig` (
    `Id` CHAR(36) NOT NULL,
    `UserId` CHAR(36) NOT NULL,
    `WidgetId` CHAR(36) NOT NULL,
    `Width` INT NOT NULL DEFAULT 3,
    `IsEnabled` TINYINT(1) NOT NULL DEFAULT 1,
    `SortOrder` INT NOT NULL DEFAULT 0,
    `CreateTime` DATETIME NOT NULL,
    `UpdateTime` DATETIME NULL,
    PRIMARY KEY (`Id`),
    INDEX `idx_user_id` (`UserId`),
    INDEX `idx_widget_id` (`WidgetId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户组件配置表';
```

- [ ] **Step 2: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add EasyWechatWeb/EasyWeChatWeb/Scripts/desktop_widget_tables.sql
git commit -m "feat(desktop): add database migration script

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 22: 完整编译验证

- [ ] **Step 1: 编译整个项目**

Run: `cd EasyWechatWeb && dotnet build`
Expected: Build succeeded with no errors

- [ ] **Step 2: 运行项目验证API**

Run: `cd EasyWechatWeb/EasyWeChatWeb && dotnet run`
Expected: Server starts on http://localhost:7600

- [ ] **Step 3: 检查Swagger**

访问 http://localhost:7600/swagger，验证以下API接口存在：
- GET /api/desktop/widget/list
- GET /api/desktop/widget/detail/{id}
- POST /api/desktop/widget/add
- POST /api/desktop/widget/update
- DELETE /api/desktop/widget/delete/{id}
- GET /api/desktop/widget/enabled-list
- GET /api/desktop/role-config/list/{roleId}
- POST /api/desktop/role-config/save
- GET /api/desktop/role-config/available-widgets/{roleId}
- GET /api/desktop/user-config/my
- POST /api/desktop/user-config/save
- POST /api/desktop/user-config/reset

---

## Spec Coverage Check

设计文档覆盖情况：

| 设计文档章节 | 实现Task |
|-------------|----------|
| 3.1 组件实体 DesktopWidget | Task 3 |
| 3.2 角色组件配置 RoleWidgetConfig | Task 4 |
| 3.3 用户组件配置 UserWidgetConfig | Task 5 |
| 11.1 组件类型枚举 WidgetType | Task 1 |
| 11.2 数据源类型枚举 DataSourceType | Task 2 |
| 6.1 组件管理 API | Task 18 |
| 6.2 角色分配 API | Task 19 |
| 6.3 用户桌面 API | Task 20 |

---

**文档版本**: 1.0
**最后更新**: 2026-05-14