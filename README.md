# EasyProject - 全栈电商管理系统

一个完整的全栈电商管理系统，包含后端 API 服务、PC 端管理后台和微信小程序商城。

## 📋 项目概述

| 子项目 | 技术栈 | 说明 |
|--------|--------|------|
| **EasyWechatWeb** | .NET 8.0 + ASP.NET Core | 后端 API 服务 |
| **PCWeb** | Vue 3 + TypeScript + Element Plus | PC 端管理后台 |
| **WeChatWeb** | 微信小程序 + TypeScript | 微信小程序鲜花商城 |

## 🏗️ 项目架构

```
EasyProject/
├── EasyWechatWeb/          # 后端 API 服务
│   ├── EasyWeChatWeb/      # Web API 主项目
│   ├── BusinessManager/    # 业务逻辑层
│   ├── EasyWeChatModels/   # 数据模型层
│   └── CommonManager/      # 通用组件层
│
├── PCWeb/                  # PC 端管理后台
│   ├── src/
│   │   ├── api/           # API 请求封装
│   │   ├── components/    # 通用组件
│   │   ├── views/         # 页面组件
│   │   ├── stores/        # Pinia 状态管理
│   │   ├── router/        # 路由配置
│   │   └── utils/         # 工具函数
│   └── ...
│
└── WeChatWeb/              # 微信小程序
    ├── pages/              # 页面
    ├── components/         # 组件
    ├── services/           # 业务服务
    ├── stores/             # 状态管理
    ├── adapters/           # 数据适配层
    ├── config/             # 环境配置
    └── types/              # 类型定义
```

## 🚀 快速开始

### 环境要求

| 环境 | 版本要求 |
|------|----------|
| .NET SDK | 8.0+ |
| Node.js | 18+ |
| MySQL | 8.0+ |
| Redis | 6.0+（可选） |
| MinIO | 最新版（可选） |

### 后端启动

```bash
# 进入后端目录
cd EasyWechatWeb/EasyWeChatWeb

# 配置数据库连接（修改 appsettings.json）
# 运行项目
dotnet run
```

后端 API 地址：`http://localhost:7600`
Swagger 文档：`http://localhost:7600/swagger`

### 前端启动

```bash
# 进入前端目录
cd PCWeb

# 安装依赖
pnpm install

# 启动开发服务器
pnpm run dev
```

前端访问地址：`http://localhost:5173`

### 微信小程序启动

1. 安装依赖：
```bash
cd WeChatWeb
npm install
```

2. 微信开发者工具导入项目
3. 勾选「不校验合法域名」选项
4. 点击「编译」运行

## 🔧 后端技术栈

| 技术 | 版本 | 说明 |
|------|------|------|
| .NET | 8.0 | 基础运行框架 |
| ASP.NET Core | 8.0 | Web API 框架 |
| SqlSugar | 5.1.4 | ORM 框架（主从分离） |
| Autofac | 8.0.0 | IOC 容器（属性注入） |
| MySQL | 8.0 | 数据库 |
| JWT | - | 双 Token 认证机制 |
| Serilog | 3.1.1 | 结构化日志 |
| MinIO | 最新 | 文件存储 |
| NPOI | 2.7.0 | Excel 处理 |
| Swagger | 6.5.0 | API 文档 |
| Mapster | 10.0.0 | 对象映射 |

### 核心特性

- ✅ **属性注入** - Autofac 程序集扫描自动注册
- ✅ **Guid 主键** - 所有实体使用 Guid 主键，便于分布式场景
- ✅ **双 Token 机制** - AccessToken + RefreshToken
- ✅ **Token 黑名单** - 登出机制完善，支持强制下线
- ✅ **主从分离** - SqlSugar 支持读写分离
- ✅ **Mock 模式** - 微信登录支付支持 Mock，便于开发测试

## 💻 前端技术栈

| 技术 | 版本 | 说明 |
|------|------|------|
| Vue | 3.3 | 渐进式 JavaScript 框架 |
| TypeScript | 5.0 | 类型安全的 JavaScript |
| Element Plus | 2.4 | Vue 3 UI 组件库 |
| Vite | 5.4 | 下一代前端构建工具 |
| Pinia | 2.1 | Vue 3 状态管理 |
| Vue Router | 4.1 | 官方路由管理器 |
| Axios | 1.5 | HTTP 请求库 |
| vue-i18n | 9.5 | 国际化方案 |
| ECharts | 6.0 | 数据可视化图表 |
| WangEditor | 5.1 | 富文本编辑器 |
| AntV X6 | 2.18 | 图编辑引擎（工作流设计器） |

### 核心功能

- 🎨 **工作流设计器** - 钉钉审批风格的纯 Vue3 实现
- 📊 **大屏设计器** - 拖拽式布局，多数据源支持
- 🔄 **ETL 设计器** - 数据处理流程可视化
- 📈 **报表管理** - 多维度数据分析
- 🌐 **多语言支持** - 中文/英文切换

## 📱 微信小程序技术栈

| 技术 | 说明 |
|------|------|
| 微信小程序原生框架 | 官方框架 |
| TypeScript | 类型安全 |
| 自定义状态管理 | 观察者模式 Store |
| 智能 Mock 数据 | 无后端可运行完整流程 |

### 功能模块

| 模块 | 功能 |
|------|------|
| 首页 | 轮播图、分类入口、热销推荐 |
| 分类 | 左侧分类导航、右侧商品列表 |
| 搜索 | 关键词搜索、搜索历史 |
| 商品详情 | 图片预览、数量选择、加购购买 |
| 购物车 | 商品管理、数量调整、选中结算 |
| 订单 | 订单列表、订单详情、取消订单 |
| 支付 | 确认订单、支付流程（Mock） |
| 地址 | 地址管理、新增编辑、设为默认 |

## ✅ 已完成功能模块

### 后端 API

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
| 聊天服务 | ✅ | WebSocket 实时聊天 |
| 微信登录 | ✅ | code2session + Mock 模式 |
| 微信支付 | ✅ | JSAPI 支付 + 回调处理 |
| Ant工作流 | ✅ | 完整流程引擎 + 多种节点类型 |
| 商品管理 | ✅ | 商品CRUD + 规格管理 |
| 订单管理 | ✅ | 订单创建 + 支付 + 发货 |
| 购物车 | ✅ | 添加 + 修改 + 删除 |
| 会员管理 | ✅ | 会员等级 + 积分管理 |
| 优惠券 | ✅ | 优惠券创建 + 发放 + 使用 |
| 大屏展示 | ✅ | 数据可视化配置 |
| 字典管理 | ✅ | 字典类型 + 字典数据 |

### PC 端管理后台

| 模块 | 状态 | 说明 |
|------|------|------|
| 登录/工作台 | ✅ | JWT 登录 + 工作台首页 |
| 基础管理 | ✅ | 用户/角色/菜单/字典管理 |
| 业务管理 | ✅ | 商品/订单/客户/退款管理 |
| 工作流设计 | ✅ | 可视化流程设计器 |
| 大屏管理 | ✅ | 拖拽式大屏设计 |
| ETL 管理 | ✅ | 数据处理流程设计 |
| 报表管理 | ✅ | 多维度数据分析 |

## 🔑 核心设计规范

### 后端开发规范

```csharp
// 实体类：Guid 主键
[SugarTable("Product", "产品表")]
public class Product
{
    [SugarColumn(IsPrimaryKey = true)]
    public Guid Id { get; set; } = Guid.NewGuid();  // ✅ Guid 主键
}

// 属性注入规范
public class ProductService : BaseService<Product>, IProductService
{
    public ISqlSugarClient _db { get; set; } = null!;        // 必须使用 null! 标记
    public ILogger<ProductService> _logger { get; set; } = null!;
}
```

### 前端开发规范

```typescript
// 类型定义：统一使用 camelCase
interface Product {
  id: string;              // GUID 主键（string）
  name: string;
  categoryId: string;
  originalPrice: number;
  isHot: boolean;
}

// API 封装
export function getProductList(params: ProductListParams) {
  return get<{ list: Product[]; total: number }>('/api/product/list', params)
}
```

## 📁 核心目录说明

### EasyWechatWeb 项目结构

```
EasyWechatWeb/
├── EasyWeChatWeb/
│   ├── Controllers/       # API 控制器
│   │   ├── Basic/        # 基础管理（User, Role, Menu, Auth）
│   │   ├── Buz/          # 业务扩展
│   │   └── WeChatPro/    # 微信小程序 API
│   ├── Middleware/       # 中间件
│   └ Extensions/        # 扩展方法
│   └ Program.cs         # 启动配置
│   └ appsettings.json   # 应用配置
│
├── BusinessManager/       # 业务管理器
│   ├── Basic/            # 基础服务
│   ├── Buz/              # 扩展服务
│   ├── Product/          # 商品服务
│   ├── WeChatProManager/ # 微信服务
│   └ AntWorkflow/        # Ant工作流服务
│
├── EasyWeChatModels/      # 数据模型
│   ├── Entitys/          # 实体类（17个模块）
│   ├── Dto/              # DTO类（20个模块）
│   ├── Enums/            # 枚举类
│   └ Models/             # 配置模型类
│
└── CommonManager/         # 通用组件
    ├── Base/             # 基类
    ├── Cache/            # 缓存
    ├── Helper/           # 工具类
    └ Options/            # 配置类
    └ Extensions/         # 扩展方法
    └ Error/              # 异常处理
```

### PCWeb 项目结构

```
PCWeb/src/
├── api/                   # API 请求封装
├── components/            # 通用组件
│   ├── BaseTable/        # 高级表格组件
│   ├── ImageUpload/      # 图片上传组件
│   ├── WangEditor/       # 富文本编辑器
│   ├── workflow/         # 工作流设计器
│   └── etl/              # ETL 设计器
│
├── views/                 # 页面组件
│   ├── basic/            # 基础管理
│   ├── workflow/         # 工作流管理
│   ├── report/           # 报表管理
│   ├── screen/           # 大屏管理
│   └── buz/              # 业务管理
│
├── stores/                # Pinia 状态管理
├── router/                # 路由配置
├── composables/           # 组合式函数
├── utils/                 # 工具函数
├── types/                 # TypeScript 类型
├── locales/               # i18n 语言包
└── config/                # 配置文件
```

### WeChatWeb 项目结构

```
WeChatWeb/
├── pages/                 # 页面（13个页面）
├── components/            # 组件
├── services/              # 业务服务（6个）
├── stores/                # 状态管理（4个）
├── adapters/              # 数据适配层
├── config/                # 环境配置
├── types/                 # TypeScript 类型定义
├── utils/                 # 工具函数
├── mock-data/             # Mock 数据
└── static/                # 静态资源
```

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

### 微信小程序配置

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

## 📚 相关文档

| 文档 | 路径 |
|------|------|
| 后端开发指南 | `EasyWechatWeb/CLAUDE.md` |
| 技术栈分析 | `EasyWechatWeb/技术栈分析.md` |
| 前端开发规范 | `PCWeb/CLAUDE.md` |
| 小程序开发规范 | `WeChatWeb/CLAUDE.md` |
| 小程序设计文档 | `WeChatWeb/WeChatDesign.md` |

## 🤝 开发团队

本项目采用以下开发规范：

- **后端**：属性注入 + Guid 主键 + 双 Token 机制
- **前端**：TypeScript 类型安全 + 组合式 API
- **小程序**：分层架构 + 自定义状态管理

## 使用说明
本项目代码仅供个人学习与交流使用，未经作者书面许可，**禁止任何形式的商业使用、分发或二次修改**。
如需商用或其他授权，请联系：[565387073@qq.com]


---

**EasyProject** - 全栈电商管理系统