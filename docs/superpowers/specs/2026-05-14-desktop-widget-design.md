# 桌面组件自动配置系统设计文档

> **创建日期**: 2026-05-14
> **状态**: 待审核

---

## 1. 概述

### 1.1 功能目标

为 EasyProject 管理后台提供可配置的个性化桌面系统，支持：
- 管理员定义组件库，用户在范围内自由调整布局
- 多种组件类型：统计卡片、数据列表、图片展示、图表统计
- 按角色分配桌面组件，新用户自动继承角色模板
- 栅格比例布局系统，自动适配不同屏幕尺寸

### 1.2 设计决策总结

| 决策项 | 选择方案 | 说明 |
|--------|----------|------|
| 配置模式 | 灵活配置模式 | 管理员定义组件库，用户自由调整 |
| 组件类型 | 全部4种 | 统计卡片、数据列表、图片展示、图表统计 |
| 尺寸系统 | 栅格比例 | 宽度1-12栅格，高度固定像素 |
| 角色分配 | A+B组合 | 角色模板 + 用户自由调整 |
| 数据源 | 灵活配置 | 每个组件独立配置API/静态/统计 |
| 管理界面 | 弹窗编辑 | 分区展示配置项 |
| 用户个性化 | 尺寸调整 | 启用组件 + 调整栅格宽度 |

---

## 2. 系统架构

### 2.1 模块划分

```
┌─────────────────────────────────────────────────────────────┐
│                      桌面组件配置系统                          │
├─────────────────────────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐       │
│  │ 组件库管理    │  │ 角色分配管理  │  │ 用户桌面管理  │       │
│  │ (管理员)     │  │ (管理员)     │  │ (用户)       │       │
│  └──────────────┘  └──────────────┘  └──────────────┘       │
│         │                 │                 │               │
│         ▼                 ▼                 ▼               │
│  ┌──────────────────────────────────────────────────┐      │
│  │              数据存储层                            │      │
│  │  DesktopWidget | RoleWidgetConfig | UserWidgetConfig │  │
│  └──────────────────────────────────────────────────┘      │
│         │                                                   │
│         ▼                                                   │
│  ┌──────────────────────────────────────────────────┐      │
│  │              渲染引擎                              │      │
│  │  组件渲染器 | 布局计算器 | 数据刷新器              │      │
│  └──────────────────────────────────────────────────┘      │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 核心流程

**管理员配置流程**：
```
1. 创建组件 → 配置类型、尺寸、数据源、交互
2. 分配给角色 → 选择组件、设置默认布局
3. 发布 → 用户可使用
```

**用户使用流程**：
```
1. 登录 → 获取角色默认布局
2. 打开布局设置 → 启用/禁用组件、调整栅格宽度
3. 保存 → 个性化布局生效
4. 恢复默认 → 回到角色模板布局
```

---

## 3. 数据模型设计

### 3.1 组件实体 (DesktopWidget)

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| Name | string(50) | 组件名称 |
| Type | int | 组件类型：1卡片/2列表/3图片/4图表 |
| Icon | string(50) | 图标名称（Element Plus图标） |
| DefaultWidth | int | 默认宽度（栅格数，1-12） |
| DefaultHeight | int | 默认高度（像素） |
| DataSourceType | int | 数据源类型：1API/2静态/3统计 |
| DataSourceConfig | string(JSON) | 数据源配置（JSON） |
| InteractionConfig | string(JSON) | 交互配置（JSON） |
| Status | int | 状态：0禁用/1启用 |
| CreateTime | DateTime | 创建时间 |
| UpdateTime | DateTime | 更新时间 |

**DataSourceConfig JSON 结构**：

```json
{
  "apiUrl": "/api/order/count?status=1",
  "fieldMapping": {
    "value": "count",
    "label": "typeName"
  },
  "refreshInterval": 30,  // 秒，0表示手动刷新
  "queryParams": {}       // 额外查询参数
}
```

**InteractionConfig JSON 结构**：

```json
{
  "clickAction": "navigate",  // navigate/openModal/showDetail
  "targetUrl": "/buz/order/list?status=1",
  "targetParams": {}
}
```

### 3.2 角色组件配置 (RoleWidgetConfig)

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| RoleId | Guid | 角色ID |
| WidgetId | Guid | 组件ID |
| SortOrder | int | 排序序号 |
| IsEnabled | bool | 是否启用（角色模板中） |
| CreateTime | DateTime | 创建时间 |

### 3.3 用户组件配置 (UserWidgetConfig)

| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| UserId | Guid | 用户ID |
| WidgetId | Guid | 组件ID |
| Width | int | 用户自定义宽度（栅格数） |
| IsEnabled | bool | 是否启用 |
| SortOrder | int | 排序序号 |
| CreateTime | DateTime | 创建时间 |
| UpdateTime | DateTime | 更新时间 |

---

## 4. 组件类型详细设计

### 4.1 统计卡片组件

**布局结构**：
```
┌─────────────────────┐
│     组件名称        │  ← 标题区
│                     │
│      128           │  ← 数字区（大字体）
│                     │
│   ↑ 3 今日新增      │  ← 状态说明
└─────────────────────┘
```

**数据源配置**：
- 类型：API接口
- 字段映射：`value` → 数字显示，`label` → 状态说明
- 刷新频率：30秒/1分钟/5分钟

**交互行为**：
- 点击 → 跳转到配置的路由（如待审核列表）
- 鼠标悬停 → 显示详情提示

### 4.2 数据列表组件

**布局结构**：
```
┌─────────────────────┐
│     组件标题        │
├─────────────────────┤
│ 订单001    [已完成] │
│ 订单002    [待发货] │
│ 订单003    [待支付] │
│ ...                 │
└─────────────────────┘
```

**数据源配置**：
- 类型：API接口
- 字段映射：主字段、状态字段、时间字段
- 刷新频率：1分钟/5分钟

**交互行为**：
- 点击行 → 查看详情/跳转详情页

### 4.3 图片展示组件

**布局结构**：
```
┌─────────────────────┐
│     组件标题        │
├─────────────────────┤
│                     │
│    [商品图片]        │
│                     │
├─────────────────────┤
│    商品名称          │
└─────────────────────┘
```

**数据源配置**：
- 类型：静态配置 或 API接口
- 配置项：图片URL、标题、链接地址
- 支持单图或轮播（最多5张）

### 4.4 图表统计组件

**布局结构**：
```
┌─────────────────────┐
│     组件标题        │
├─────────────────────┤
│                     │
│    [ECharts图表]     │
│                     │
└─────────────────────┘
```

**数据源配置**：
- 类型：API接口 或 实时统计
- 配置项：图表类型（柱状/折线/饼图）、数据字段、时间范围
- 刷新频率：5分钟/10分钟

---

## 5. 响应式布局设计

### 5.1 栅格系统

采用 12 栅格系统，组件宽度可配置为：

| 栅格数 | 宽度比例 | 适用场景 |
|--------|----------|----------|
| 1 | 8.33% | 最小卡片 |
| 2 | 16.67% | 小卡片 |
| 3 | 25% | 标准卡片 |
| 4 | 33.33% | 中等卡片 |
| 6 | 50% | 半行宽度 |
| 8 | 66.67% | 大列表/图表 |
| 12 | 100% | 全行宽度 |

### 5.2 响应式断点

| 断点名称 | 最小宽度 | 栅格计算方式 | 推荐列数 |
|----------|----------|--------------|----------|
| xs（超小屏） | <576px | 12栅格 = 100% | 单列 |
| sm（小屏） | ≥576px | 12栅格 = 100% | 2-3列 |
| md（中屏） | ≥768px | 12栅格 = 全屏宽 | 3-4列 |
| lg（大屏） | ≥992px | 12栅格 = 全屏宽 | 4-6列 |
| xl（超大屏） | ≥1200px | 12栅格 = 全屏宽 | 6-8列 |

### 5.3 布局计算逻辑

```typescript
// 前端布局计算
function calculateLayout(widgets: UserWidgetConfig[]) {
  const screenWidth = window.innerWidth;
  const breakpoints = {
    xs: 576,
    sm: 768,
    md: 992,
    lg: 1200
  };

  // 根据断点计算实际列数
  let columns = 1;
  if (screenWidth >= breakpoints.xl) columns = 12;
  else if (screenWidth >= breakpoints.lg) columns = 8;
  else if (screenWidth >= breakpoints.md) columns = 6;
  else if (screenWidth >= breakpoints.sm) columns = 4;
  else columns = 1;

  // 计算每个组件的实际宽度
  return widgets.map(w => ({
    ...w,
    actualWidth: (w.width / 12) * 100 + '%'
  }));
}
```

---

## 6. API 接口设计

### 6.1 组件管理 API

| 接口 | 方法 | 说明 |
|------|------|------|
| /api/desktop/widget/list | GET | 获取组件列表（分页） |
| /api/desktop/widget/add | POST | 创建组件 |
| /api/desktop/widget/update | POST | 更新组件 |
| /api/desktop/widget/delete/{id} | DELETE | 删除组件 |
| /api/desktop/widget/detail/{id} | GET | 获取组件详情 |

### 6.2 角色分配 API

| 接口 | 方法 | 说明 |
|------|------|------|
| /api/desktop/role-config/list/{roleId} | GET | 获取角色的组件配置列表 |
| /api/desktop/role-config/save | POST | 保存角色组件配置 |
| /api/desktop/role-config/widget-options | GET | 获取可分配组件列表 |

### 6.3 用户桌面 API

| 接口 | 方法 | 说明 |
|------|------|------|
| /api/desktop/user-config/my | GET | 获取当前用户的桌面配置 |
| /api/desktop/user-config/save | POST | 保存用户桌面配置 |
| /api/desktop/user-config/reset | POST | 重置为角色默认布局 |
| /api/desktop/user-config/data/{widgetId} | GET | 获取组件数据（带刷新） |

---

## 7. 前端页面设计

### 7.1 组件管理页面

**路径**: `/basic/desktop/widget`

**功能**:
- 组件列表展示（名称、类型、尺寸、状态）
- 新建/编辑组件弹窗
- 删除组件确认

**编辑弹窗分区**:
1. 基本信息：名称、类型、图标、状态
2. 尺寸配置：宽度（栅格选择器）、高度（像素输入）
3. 数据源配置：API地址、字段映射、刷新频率
4. 交互配置：点击跳转路由

### 7.2 角色桌面分配页面

**路径**: `/basic/desktop/role-config`

**功能**:
- 选择角色
- 从组件库选择组件分配给角色
- 设置组件默认启用状态和排序
- 预览角色桌面布局效果

### 7.3 用户桌面页面

**路径**: `/desktop`

**功能**:
- 展示用户启用的组件（栅格布局）
- 组件数据实时刷新
- 点击卡片跳转对应列表

**布局设置弹窗**:
- 可用组件列表（勾选启用）
- 已启用组件尺寸调整（栅格宽度）
- 保存/恢复默认按钮

---

## 8. 数据刷新机制

### 8.1 刷新策略

| 刷新方式 | 适用场景 | 实现方式 |
|----------|----------|----------|
| 手动刷新 | 低频更新数据 | 用户点击刷新按钮 |
| 定时刷新 | 统计数据 | 根据配置间隔轮询 |
| 实时推送 | 关键数据 | WebSocket 推送（可选） |

### 8.2 刷新间隔配置

```typescript
const refreshIntervals = [
  { label: '手动', value: 0 },
  { label: '30秒', value: 30 },
  { label: '1分钟', value: 60 },
  { label: '5分钟', value: 300 },
  { label: '10分钟', value: 600 }
];
```

---

## 9. 权限控制

### 9.1 权限点设计

| 权限点 | 说明 | 角色 |
|--------|------|------|
| desktop:widget:view | 查看组件列表 | admin |
| desktop:widget:add | 创建组件 | admin |
| desktop:widget:edit | 编辑组件 | admin |
| desktop:widget:delete | 删除组件 | admin |
| desktop:role-config | 角色桌面分配 | admin |
| desktop:user-config | 用户桌面个性化 | 所有用户 |

### 9.2 菜单配置

```json
{
  "name": "桌面配置",
  "path": "/basic/desktop",
  "icon": "Monitor",
  "children": [
    {
      "name": "组件管理",
      "path": "/basic/desktop/widget",
      "icon": "Grid"
    },
    {
      "name": "角色分配",
      "path": "/basic/desktop/role-config",
      "icon": "UserFilled"
    }
  ]
}
```

---

## 10. 实施计划概要

### 10.1 开发阶段

| 阶段 | 内容 | 预估时间 |
|------|------|----------|
| 阶段一 | 后端数据模型 + API接口 | 1-2天 |
| 阶段二 | 前端组件管理页面 | 1-2天 |
| 阶段三 | 前端角色分配页面 | 1天 |
| 阶段四 | 前端用户桌面页面 + 组件渲染器 | 2-3天 |
| 阶段五 | 数据刷新机制 + 测试 | 1天 |

### 10.2 技术要点

- 后端使用 SqlSugar ORM，实体主键使用 Guid
- 前端使用 Vue3 Composition API + Element Plus
- 组件渲染器使用动态组件（`<component :is="...">`）
- 图表组件使用 ECharts
- 响应式布局使用 CSS Grid + Flexbox

---

## 11. 附录

### 11.1 组件类型枚举

```csharp
public enum WidgetType
{
    Card = 1,      // 统计卡片
    List = 2,      // 数据列表
    Image = 3,     // 图片展示
    Chart = 4      // 图表统计
}
```

```typescript
// 前端枚举
export enum WidgetType {
  Card = 1,
  List = 2,
  Image = 3,
  Chart = 4
}

export const widgetTypeLabels = {
  [WidgetType.Card]: '统计卡片',
  [WidgetType.List]: '数据列表',
  [WidgetType.Image]: '图片展示',
  [WidgetType.Chart]: '图表统计'
};
```

### 11.2 数据源类型枚举

```csharp
public enum DataSourceType
{
    Api = 1,       // API接口
    Static = 2,    // 静态配置
    Statistics = 3 // 实时统计
}
```

---

**文档版本**: 1.0
**最后更新**: 2026-05-14