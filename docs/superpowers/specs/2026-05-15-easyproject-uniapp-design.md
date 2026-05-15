# EasyProject UniApp 移动端设计文档

> 文档版本: 1.0
> 创建日期: 2026-05-15
> 项目: EasyProject UniApp Mobile

---

## 1. 项目概述

### 1.1 项目目标

基于现有PCWeb项目，开发一个UniApp移动端管理平台，支持多角色（管理员、销售、仓库等）移动办公场景。

### 1.2 核心功能

| 功能模块 | 优先级 | 说明 |
|----------|--------|------|
| 工作流审批 | P0 | 待办/已办/抄送任务列表、审批操作、审批进度查看 |
| 商品管理 | P0 | 商品CRUD、分类管理、库存查看、上下架操作 |
| 基础管理 | P0 | 用户管理、角色管理、部门管理、菜单管理、字典管理 |
| 报表查看 | P0 | 报表列表、数据图表展示、筛选查询 |
| 大屏查看 | P0 | 已发布大屏列表、全屏展示、缩放刷新 |
| 日志查看 | P0 | 操作日志列表、筛选查询 |

### 1.3 发布平台

- 微信小程序
- H5网页
- Android App
- iOS App

---

## 2. 技术架构

### 2.1 技术栈

| 类别 | 技术选型 | 版本 | 说明 |
|------|----------|------|------|
| 框架 | uni-app + Vue 3 | 3.x | 支持多端发布 |
| 语言 | TypeScript | 5.0 | 类型安全 |
| UI库 | uView UI | 2.x | uni-app生态主流UI库 |
| 状态管理 | Pinia | 2.x | 与PCWeb一致 |
| HTTP请求 | uni.request | - | 适配多端 |
| 图表 | uni-echarts | - | ECharts适配版 |
| 推送 | uni-push2 | - | DCloud推送服务 |

### 2.2 数据架构

与PCWeb共享后端API（EasyWechatWeb），新增移动端独有API：

| API模块 | 说明 |
|---------|------|
| 推送通知 | 推送注册、消息列表 |
| 扫码功能 | 二维码/条码识别 |
| 设备信息 | 设备标识、版本检查 |

### 2.3 目录结构

```
UniAppMobile/
├── pages/                       # 页面目录
│   ├── login/                   # 登录页
│   ├── index/                   # 工作台首页
│   ├── workflow/                # 工作流模块
│   │   ├── todo/                # 待办任务
│   │   ├── done/                # 已办任务
│   │   ├── cc/                  # 抄送任务
│   │   └── detail/              # 任务详情/审批
│   ├── product/                 # 商品模块
│   │   ├── list/                # 商品列表
│   │   ├── detail/              # 商品详情
│   │   ├── edit/                # 商品新增/编辑
│   │   ├── category/            # 分类管理
│   │   └── stock/               # 库存查看
│   ├── basic/                   # 基础管理模块
│   │   ├── user/                # 用户管理
│   │   ├── role/                # 角色管理
│   │   ├── department/          # 部门管理
│   │   ├── menu/                # 菜单管理
│   │   └── dict/                # 字典管理
│   ├── report/                  # 报表模块
│   │   ├── list/                # 报表列表
│   │   └ view/                  # 报表查看
│   ├── screen/                  # 大屏模块
│   │   ├── list/                # 大屏列表
│   │   └ view/                  # 大屏查看
│   └── log/                     # 日志模块
│       └── list/                # 日志列表
│
├── components/                  # 通用组件
│   ├── NavBar/                  # 导航栏
│   ├── TabBar/                  # 底部标签栏
│   ├── Empty/                   # 空状态
│   ├── Loading/                 # 加载状态
│   ├── CardItem/                # 卡片列表项
│   ├── ApprovalBar/             # 审批操作栏
│   ├── ProgressTimeline/        # 进度时间线
│   ├── ChartContainer/          # 图表容器
│   └── SearchBar/               # 搜索栏
│
├── api/                         # API封装
│   ├── auth/                    # 认证API
│   ├── workflow/                # 工作流API
│   ├── product/                 # 商品API
│   ├── basic/                   # 基础管理API
│   ├── report/                  # 报表API
│   ├── screen/                  # 大屏API
│   ├── log/                     # 日志API
│   └── mobile/                  # 移动端独有API
│
├── stores/                      # 状态管理
│   ├── user.ts                  # 用户状态
│   ├── app.ts                   # 应用状态
│   └── workflow.ts              # 工作流状态
│
├── utils/                       # 工具函数
│   ├── request.ts               # HTTP请求封装
│   ├── auth.ts                  # 认证工具
│   ├── storage.ts               # 本地存储
│   └── platform.ts              # 平台判断
│
├── config/                      # 配置
│   ├── env.ts                   # 环境配置
│   ├── api.config.ts            # API路径
│   └── theme.config.ts          # 主题配置
│
├── types/                       # TypeScript类型
│   ├── index.ts                 # 类型导出
│   ├── workflow.types.ts        # 工作流类型
│   ├── product.types.ts         # 商品类型
│   ├── report.types.ts          # 报表类型
│   └── response.types.ts        # API响应类型
│
├── static/                      # 静态资源
├── uni_modules/                 # uni-app模块
│   └── uview-ui/                # uView UI
│
├── App.vue                      # 应用入口
├── main.ts                      # 主入口
├── manifest.json                # uni-app配置
├── pages.json                   # 页面路由配置
└── uni.scss                     # 全局样式变量
```

---

## 3. UI/UX设计

### 3.1 设计风格

- **主风格**: Flat Design + Minimalism（简洁扁平化）
- **辅助风格**: Micro-interactions + Soft UI Evolution（微交互、柔和阴影）
- **设计语言**: 与PCWeb Element Plus保持视觉一致性

### 3.2 颜色方案

```
主色系:
  Primary:     #2563EB  信任蓝（品牌主色）
  Secondary:   #3B82F6  浅蓝（次要强调）
  Accent:      #EA580C  操作橙（CTA按钮）

功能色:
  Success:     #059669  成功绿（通过、完成）
  Warning:     #D97706  警告橙（待处理）
  Error:       #DC2626  错误红（拒绝、失败）
  Info:        #0891B2  信息蓝（提示）

中性色:
  Background:  #F8FAFC  背景白
  Foreground:  #1E293B  文字深灰
  Card:        #FFFFFF  卡片白
  Muted:       #64748B  辅助文字灰
  Border:      #E2E8F0  边框灰
```

### 3.3 全局导航

**底部TabBar（4个）**

| Tab | 图标 | 页面 | 说明 |
|-----|------|------|------|
| 工作台 | House | /pages/index/index | 首页/数据概览/快捷入口 |
| 工作流 | Refresh | /pages/workflow/todo/index | 待办任务（默认） |
| 商品 | Box | /pages/product/list/index | 商品列表 |
| 基础 | Setting | /pages/basic/user/index | 基础管理 |

**工作台快捷入口（8宫格）**

| 入口 | 页面路径 |
|------|----------|
| 报表 | /pages/report/list/index |
| 大屏 | /pages/screen/list/index |
| 日志 | /pages/log/list/index |
| 用户 | /pages/basic/user/index |
| 角色 | /pages/basic/role/index |
| 部门 | /pages/basic/department/index |
| 菜单 | /pages/basic/menu/index |
| 字典 | /pages/basic/dict/index |

### 3.4 UX设计要点

| 类别 | 要点 | 实施 |
|------|------|------|
| 触摸交互 | Touch Target Size | 按钮/点击区域 ≥ 44×44px |
| 触摸交互 | Touch Spacing | 相邻点击区域间距 ≥ 8px |
| 加载状态 | Loading Indicators | 超过300ms显示骨架屏 |
| 反馈 | Empty States | 空列表显示引导信息 |
| 反馈 | Toast Notifications | 成功/警告消息3-5秒消失 |
| 表单 | Input Labels | 所有输入框上方显示标签 |
| 表单 | Error Placement | 错误信息显示在输入框下方 |

---

## 4. 功能模块详细设计

### 4.1 工作流审批模块

| 页面 | 功能 | 操作 |
|------|------|------|
| 待办任务列表 | 任务列表、搜索、筛选（紧急/普通）、排序 | ✅ |
| 已办任务列表 | 已处理任务、查看结果、时间筛选 | ✅ |
| 抄送任务列表 | 抄送任务、已读/未读标记 | ✅ |
| 任务详情页 | 基本信息、表单数据、审批进度时间线 | ✅ |
| 审批操作 | 通过、拒绝、转办、加签、审批意见、抄送 | ✅ |

**移动端限制：**
- ❌ 流程设计器 → 提示去PC端操作
- ❌ 流程模板管理 → 提示去PC端操作

### 4.2 商品管理模块

| 页面 | 功能 | 操作 |
|------|------|------|
| 商品列表 | 搜索、分类筛选、分页、上下架状态 | ✅ |
| 商品详情 | 基本信息、图片、SKU、库存、上下架操作 | ✅ |
| 商品新增 | 表单填写、图片上传、SKU配置 | ✅ |
| 商品编辑 | 修改商品信息、修改SKU、修改价格 | ✅ |
| 批量导入导出 | Excel导入导出 | ✅ |
| 库存调整 | 选择商品、调整数量、调整原因 | ✅ |
| 库存记录 | 变动历史、出入库记录 | ✅ |
| 商品分类 | 分类树、新增、编辑、删除分类 | ✅ |

### 4.3 基础管理模块

| 页面 | 功能 | 操作 |
|------|------|------|
| 用户列表 | 搜索、筛选、分页 | ✅ |
| 用户详情 | 用户信息、角色、部门 | ✅ |
| 用户新增 | 基础信息、角色分配、密码设置 | ✅ |
| 用户编辑 | 修改信息、修改角色、重置密码 | ✅ |
| 用户状态 | 启用/禁用 | ✅ |
| 角色列表 | 搜索、分页 | ✅ |
| 角色新增 | 角色信息、权限分配 | ✅ |
| 角色编辑 | 修改信息、修改权限 | ✅ |
| 部门管理 | 部门树、新增、编辑、删除 | ✅ |
| 菜单管理 | 菜单树、新增、编辑、删除 | ✅ |
| 字典管理 | 字典类型、字典数据、CRUD | ✅ |

### 4.4 报表查看模块

| 页面 | 功能 | 操作 |
|------|------|------|
| 报表列表 | 分类筛选、搜索、分页 | ✅ |
| 报表查看 | 时间筛选、关键指标、图表展示、数据表格 | ✅ |

**移动端限制：**
- ❌ 报表设计器 → 提示去PC端操作
- ❌ 创建/编辑报表 → 提示去PC端操作

### 4.5 大屏查看模块

| 页面 | 功能 | 操作 |
|------|------|------|
| 大屏列表 | 搜索、分页 | ✅ |
| 大屏查看 | WebView嵌入、全屏展示、缩放、刷新 | ✅ |

**实现方式：** 使用WebView嵌入PCWeb已发布的大屏URL

### 4.6 日志查看模块

| 页面 | 功能 | 操作 |
|------|------|------|
| 日志列表 | 时间筛选、类型筛选、操作人搜索、分页 | ✅ |
| 日志详情 | 操作时间、操作人、操作内容、操作结果 | ✅ |

---

## 5. 技术实现要点

### 5.1 HTTP请求封装

使用uni.request替代Axios，封装统一的请求方法：

```typescript
// utils/request.ts
export function get<T>(url: string, params?: object): Promise<T>
export function post<T>(url: string, data?: object): Promise<T>
export function del<T>(url: string, data?: object): Promise<T>
```

### 5.2 登录验证守卫

仅验证登录状态，不做权限检查：

```typescript
// App.vue
uni.addInterceptor('navigateTo', {
  invoke(e) {
    const userStore = useUserStore()
    const publicPages = ['/pages/login/index', '/pages/report/publish/index']
    // 未登录跳转登录页
    if (!publicPages.some(p => url.startsWith(p)) && !userStore.isLoggedIn) {
      uni.navigateTo({ url: '/pages/login/index' })
      return false
    }
    return true
  },
})
```

### 5.3 多平台适配

```typescript
// utils/platform.ts
export function getPlatform(): 'mp-weixin' | 'h5' | 'app' | 'app-plus'
export function isWeixin(): boolean
export function isH5(): boolean
export function isApp(): boolean

export const platformUtils = {
  scanCode(): Promise<string>  // 扫码功能
  registerPush()               // 推送注册
}
```

### 5.4 uView UI主题定制

```scss
// uni.scss
$u-primary: #2563EB;
$u-success: #059669;
$u-warning: #D97706;
$u-error: #DC2626;
$u-info: #0891B2;
$u-main-color: #1E293B;
$u-bg-color: #F8FAFC;
$u-border-color: #E2E8F0;
$u-button-height: 88rpx;
$u-tabbar-height: 100rpx;
```

---

## 6. 开发计划

### 6.1 里程碑规划

| 阶段 | 时间 | 内容 |
|------|------|------|
| Phase 1 | 第1-2周 | 项目基础搭建、登录认证、全局导航 |
| Phase 2 | 第3-4周 | 工作流审批模块 |
| Phase 3 | 第5-6周 | 商品管理模块 |
| Phase 4 | 第7-8周 | 基础管理模块 |
| Phase 5 | 第9-10周 | 报表与大屏模块 |
| Phase 6 | 第11-12周 | 日志模块、工作台完善、多端测试 |
| Phase 7 | 第13周 | 性能优化、安全检查、打包发布 |

### 6.2 页面开发清单（共23个页面）

| 序号 | 页面 | 预计工时 |
|------|------|----------|
| 1 | 登录页 | 1天 |
| 2 | 工作台首页 | 2天 |
| 3-6 | 工作流模块（4个页面） | 7天 |
| 7-11 | 商品模块（5个页面） | 7天 |
| 12-18 | 基础管理模块（7个页面） | 10天 |
| 19-20 | 报表模块（2个页面） | 3天 |
| 21-22 | 大屏模块（2个页面） | 2天 |
| 23 | 日志模块（1个页面） | 2天 |

**总计：约33个工作日（预计2.5-3个月）**

---

## 7. 技术风险与应对

| 风险点 | 应对措施 |
|--------|----------|
| ECharts渲染问题 | 使用uni-echarts官方适配版本 |
| WebView大屏加载慢 | 加载动画 + WebView缓存优化 |
| 多平台样式差异 | rpx单位 + 平台动态调整 |
| H5扫码不支持 | 提示使用App/小程序，提供手动输入 |
| 推送通知复杂 | 使用uni-push2统一服务 |

---

## 附录

### A. 设计决策记录

| 决策 | 选择 | 原因 |
|------|------|------|
| 功能范围 | 功能完整型（含新增编辑） | 用户需要完整操作能力 |
| UI框架 | uView UI | 与Element Plus风格一致，生态成熟 |
| 导航结构 | 4个TabBar | 报表等通过快捷入口访问 |
| 权限控制 | 仅登录验证 | 简化实现，后续可扩展 |

### B. 相关文档

- PCWeb项目规范: `PCWeb/CLAUDE.md`
- 后端API规范: `EasyWechatWeb/CLAUDE.md`
- 主项目规范: `CLAUDE.md`