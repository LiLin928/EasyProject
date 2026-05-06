# 微信小程序鲜花商城

基于 TypeScript 的微信小程序鲜花商城项目。

## 技术栈

- 微信小程序原生框架
- TypeScript
- 自定义状态管理
- 智能 Mock 数据

## 快速开始

### 1. 安装依赖

```bash
npm install
```

### 2. 微信开发者工具导入

1. 打开微信开发者工具
2. 选择「导入项目」
3. 选择本项目目录
4. 填入 AppID（测试可选择「测试号」）
5. 点击「导入」

### 3. 开发设置

在微信开发者工具中：

- 点击「详情」→「本地设置」
- ✅ 勾选「不校验合法域名、web-view（业务域名）、TLS版本以及HTTPS证书」
- ✅ 勾选「启用自定义处理命令」（如使用 TypeScript 编译）

### 4. 运行项目

点击工具栏的「编译」按钮即可运行。

## 项目结构

```
WeChatWeb/
├── pages/          # 页面（13个页面）
│   ├── index/      # 首页
│   ├── classify/   # 分类
│   ├── shopping/   # 购物车
│   ├── my/         # 个人中心
│   ├── details/    # 商品详情
│   ├── search/     # 搜索
│   ├── login/      # 登录
│   ├── pay/        # 支付（确认订单、结果页）
│   ├── order/      # 订单（列表、详情）
│   └── address/    # 地址（列表、编辑）
│
├── components/     # 组件
│   └── product-card/  # 商品卡片
│
├── services/       # 业务服务（6个）
│   ├── auth.service.ts     # 认证服务
│   ├── product.service.ts  # 商品服务
│   ├── cart.service.ts     # 购物车服务
│   ├── order.service.ts    # 订单服务
│   ├── address.service.ts  # 地址服务
│   └── payment.service.ts  # 支付服务
│
├── stores/         # 状态管理（4个）
│   ├── base.store.ts   # Store 基类
│   ├── user.store.ts   # 用户状态
│   ├── cart.store.ts   # 购物车状态
│   └── order.store.ts  # 订单状态
│
├── adapters/       # 数据适配层
│   ├── adapter.interface.ts  # 适配器接口
│   └── mock.adapter.ts       # Mock 适配器
│
├── config/         # 环境配置
│   ├── env.dev.ts   # 开发环境
│   ├── env.test.ts  # 测试环境
│   ├── env.prod.ts  # 生产环境
│   └ api.config.ts  # API 路径定义
│
├── types/          # TypeScript 类型定义
├── utils/          # 工具函数
├── mock-data/      # Mock 数据
└── static/         # 静态资源
```

## 功能模块

| 模块 | 功能 |
|------|------|
| 首页 | 轮播图、分类入口、热销推荐、新品上架 |
| 分类 | 左侧分类导航、右侧商品列表 |
| 搜索 | 关键词搜索、搜索历史、热门推荐 |
| 商品详情 | 图片预览、数量选择、加购购买 |
| 购物车 | 商品管理、数量调整、选中结算 |
| 订单 | 订单列表、订单详情、取消订单 |
| 支付 | 确认订单、支付流程（Mock） |
| 地址 | 地址管理、新增编辑、设为默认 |
| 个人中心 | 用户信息、订单入口、退出登录 |

## Mock 数据

项目使用智能 Mock，数据存储在 Storage 中：

- **购物车**：支持累加计算、选中管理
- **订单**：支持状态流转（待支付→已支付→配送中→已完成）
- **地址**：支持增删改查、默认地址

## 开发规范

- 使用 TypeScript 编写
- 类型定义集中在 `types/` 目录（8个类型文件）
- 服务层单例模式（`getInstance()`）
- Store 继承 `BaseStore`，支持订阅/通知

## 添加静态资源

### TabBar 图标

如需添加 TabBar 图标：

1. 准备 81×81 像素的 PNG 图标
2. 放入 `static/tabs/` 目录
3. 修改 `app.json` 中的 tabBar 配置：

```json
{
  "tabBar": {
    "list": [
      {
        "pagePath": "pages/index/index",
        "text": "首页",
        "iconPath": "static/tabs/home.png",
        "selectedIconPath": "static/tabs/home-active.png"
      }
    ]
  }
}
```

### 推荐图标资源

- [阿里巴巴 iconfont](https://www.iconfont.cn/)
- [Flaticon](https://www.flaticon.com/)
- [Icons8](https://icons8.com/)

## 开发规范

- 使用 TypeScript 编写
- 类型定义集中在 `types/` 目录
- 页面特有逻辑放在页面目录
- 通用组件放在 `components/` 目录

## 项目特点

1. **TypeScript 支持**：完整的类型定义（8个类型文件）
2. **分层架构**：Services/Stores/Adapters 清晰分层
3. **Mock 数据**：智能 Mock，无后端即可运行完整流程
4. **状态管理**：自定义轻量级 Store（观察者模式）
5. **适配层设计**：轻松切换 Mock/API
6. **多环境配置**：支持开发/测试/生产环境切换

## 环境配置

项目支持多环境配置，通过 `config/` 目录管理：

| 环境 | 文件 | API 地址 |
|------|------|----------|
| 开发 | `env.dev.ts` | `http://localhost:7600/api/wechat` |
| 测试 | `env.test.ts` | 测试服务器地址 |
| 生产 | `env.prod.ts` | 生产服务器地址 |

切换环境：修改 `config/index.ts` 中的 `currentEnv` 配置。

## API 路径

所有 API 路径定义在 `config/api.config.ts`：

```typescript
// 用户模块
AUTH: '/auth',
WX_LOGIN: '/auth/wxLogin',

// 商品模块
PRODUCT: '/product',
PRODUCT_LIST: '/product/list',
PRODUCT_DETAIL: '/product/detail',
// ...
```

## 切换到真实 API

1. 实现 `adapters/api.adapter.ts`
2. 在适配器配置中设置 `USE_MOCK = false`
3. 在微信公众平台配置服务器域名

## 常见问题

### 真机调试报错

确保在微信公众平台配置了 request 合法域名，或在开发者工具中勾选「不校验合法域名」。

### TypeScript 编译错误

确保已安装 TypeScript：
```bash
npm install typescript --save-dev
```

### 图片不显示

项目使用网络图片占位，确保网络正常。如需使用本地图片，请修改相关代码。

## License

MIT