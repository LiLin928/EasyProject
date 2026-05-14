# 桌面组件自动配置系统 - 阶段五：数据刷新机制与测试

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 实现组件数据刷新机制（手动刷新、定时刷新），完成系统集成测试

**Architecture:** 前端轮询刷新 + 后端数据缓存，支持多类型数据源代理

**Tech Stack:** Vue 3.3 + TypeScript 5.0 + Pinia 2.1 + ECharts 6.0

---

## 文件结构

### 需要创建/修改的文件

```
PCWeb/src/
├── composables/
│   └── useWidgetRefresh.ts           # 组件刷新组合式函数(新建)
│
├── views/desktop/
│   └── index.vue                      # 集成刷新机制(修改)
│   └── components/
│   │   └── CardWidget.vue             # 集成刷新机制(修改)
│   │   └── ListWidget.vue             # 集成刷新机制(修改)
│   │   └── ChartWidget.vue            # 集成刷新机制(修改)
│
├── stores/
│   └── desktopStore.ts                # 添加刷新状态管理(修改)
│
└── types/
│   └── desktopWidget.ts               # 添加刷新相关类型(修改)

EasyWechatWeb/
├── Controllers/Desktop/
│   └── UserWidgetController.cs        # 添加数据代理接口(修改)
│
└── BusinessManager/Desktop/
│   └── Service/UserWidgetService.cs   # 添加数据代理逻辑(修改)
```

---

## Task 1: 扩展刷新相关类型定义

**Files:**
- Modify: `PCWeb/src/types/desktopWidget.ts`

- [ ] **Step 1: 在类型文件末尾添加刷新相关类型**

```typescript
// 文件: PCWeb/src/types/desktopWidget.ts
// ... 保留原有内容，添加以下内容 ...

/**
 * 刷新间隔选项
 */
export const refreshIntervalOptions = [
  { label: '手动刷新', value: 0 },
  { label: '30秒', value: 30 },
  { label: '1分钟', value: 60 },
  { label: '5分钟', value: 300 },
  { label: '10分钟', value: 600 },
]

/**
 * 组件数据响应
 */
export interface WidgetDataResponse {
  value?: number
  label?: string
  trend?: 'up' | 'down' | 'flat'
  trendValue?: number
  list?: Array<{
    id: string
    title: string
    status?: string
    time?: string
  }>
  chartData?: {
    type: 'bar' | 'line' | 'pie'
    xAxis?: string[]
    series?: Array<{
      name: string
      data: number[]
    }>
  }
  images?: Array<{
    url: string
    title?: string
    link?: string
  }>
}

/**
 * 刷新状态
 */
export interface RefreshState {
  widgetId: string
  loading: boolean
  lastRefreshTime: number
  error?: string
}
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/types/desktopWidget.ts
git commit -m "feat(desktop): add refresh related types

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 2: 创建组件刷新组合式函数

**Files:**
- Create: `PCWeb/src/composables/useWidgetRefresh.ts`

- [ ] **Step 1: 创建刷新组合式函数**

```typescript
// 文件: PCWeb/src/composables/useWidgetRefresh.ts

import { ref, onUnmounted } from 'vue'
import { getWidgetData } from '@/api/basic/userWidgetConfigApi'
import type { WidgetDataResponse, RefreshState } from '@/types'

/**
 * 组件数据刷新组合式函数
 */
export function useWidgetRefresh(widgetId: string, refreshInterval: number) {
  const data = ref<WidgetDataResponse | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)
  const lastRefreshTime = ref(Date.now())

  let timer: number | null = null

  // 刷新数据
  const refresh = async () => {
    loading.value = true
    error.value = null
    try {
      const result = await getWidgetData(widgetId)
      data.value = result
      lastRefreshTime.value = Date.now()
    } catch (err: any) {
      error.value = err.message || '获取数据失败'
    } finally {
      loading.value = false
    }
  }

  // 启动定时刷新
  const startAutoRefresh = () => {
    if (refreshInterval > 0) {
      timer = window.setInterval(refresh, refreshInterval * 1000)
    }
  }

  // 停止定时刷新
  const stopAutoRefresh = () => {
    if (timer) {
      clearInterval(timer)
      timer = null
    }
  }

  // 组件卸载时清理
  onUnmounted(() => {
    stopAutoRefresh()
  })

  return {
    data,
    loading,
    error,
    lastRefreshTime,
    refresh,
    startAutoRefresh,
    stopAutoRefresh,
  }
}

/**
 * 批量刷新管理器
 */
export function useBatchRefresh() {
  const refreshStates = ref<Map<string, RefreshState>>(new Map())

  // 更新单个组件刷新状态
  const updateRefreshState = (widgetId: string, state: Partial<RefreshState>) => {
    const current = refreshStates.value.get(widgetId) || {
      widgetId,
      loading: false,
      lastRefreshTime: 0,
    }
    refreshStates.value.set(widgetId, { ...current, ...state })
  }

  // 获取所有刷新状态
  const getAllRefreshStates = () => {
    return Array.from(refreshStates.value.values())
  }

  return {
    refreshStates,
    updateRefreshState,
    getAllRefreshStates,
  }
}
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/composables/useWidgetRefresh.ts
git commit -m "feat(desktop): add widget refresh composable

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 3: 后端添加数据代理接口

**Files:**
- Modify: `EasyWechatWeb/Controllers/Desktop/UserWidgetController.cs`
- Modify: `BusinessManager/Desktop/Service/UserWidgetService.cs`

- [ ] **Step 1: 在Service接口添加数据代理方法定义**

```csharp
// 文件: BusinessManager/Desktop/IService/IUserWidgetService.cs
// 在接口末尾添加方法定义

/// <summary>
/// 获取组件数据（代理数据源）
/// </summary>
Task<object> GetWidgetDataAsync(Guid widgetId, Guid userId);
```

- [ ] **Step 2: 在Service实现添加数据代理逻辑**

```csharp
// 文件: BusinessManager/Desktop/Service/UserWidgetService.cs
// 在类末尾添加实现

public async Task<object> GetWidgetDataAsync(Guid widgetId, Guid userId)
{
    // 获取组件配置
    var widget = await _db.Queryable<DesktopWidget>()
        .Where(x => x.Id == widgetId)
        .FirstAsync();

    if (widget == null)
    {
        throw new Exception("组件不存在");
    }

    // 根据数据源类型获取数据
    switch (widget.DataSourceType)
    {
        case (int)DataSourceType.Api:
            return await GetApiDataAsync(widget.DataSourceConfig);
        case (int)DataSourceType.Static:
            return GetStaticData(widget.DataSourceConfig);
        case (int)DataSourceType.Statistics:
            return await GetStatisticsDataAsync(widget.DataSourceConfig);
        default:
            return new { };
    }
}

private async Task<object> GetApiDataAsync(string configJson)
{
    try
    {
        var config = JsonSerializer.Deserialize<DataSourceConfig>(configJson);
        if (config == null || string.IsNullOrEmpty(config.ApiUrl))
        {
            return new { };
        }

        // 使用HttpClient调用API
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(config.ApiUrl);
        var content = await response.Content.ReadAsStringAsync();
        
        // 解析并映射字段
        var data = JsonSerializer.Deserialize<JsonElement>(content);
        return MapFields(data, config.FieldMapping);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "获取API数据失败: {Config}", configJson);
        return new { error = "获取数据失败" };
    }
}

private object GetStaticData(string configJson)
{
    try
    {
        return JsonSerializer.Deserialize<object>(configJson) ?? new { };
    }
    catch
    {
        return new { };
    }
}

private async Task<object> GetStatisticsDataAsync(string configJson)
{
    // 统计数据需要根据配置查询数据库
    // 示例：订单数量统计
    try
    {
        var config = JsonSerializer.Deserialize<StatisticsConfig>(configJson);
        if (config == null)
        {
            return new { };
        }

        // 根据配置查询对应数据
        // 这里需要根据业务具体实现
        var count = await _db.Queryable<Order>()
            .WhereIF(config.Status.HasValue, x => x.Status == config.Status.Value)
            .CountAsync();

        return new
        {
            value = count,
            label = config.Label ?? "总数",
            trend = "flat"
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "获取统计数据失败: {Config}", configJson);
        return new { };
    }
}

private object MapFields(JsonElement data, Dictionary<string, string>? fieldMapping)
{
    if (fieldMapping == null || !fieldMapping.Any())
    {
        return JsonSerializer.Deserialize<object>(data.GetRawText()) ?? new { };
    }

    var result = new Dictionary<string, object>();
    foreach (var mapping in fieldMapping)
    {
        if (data.TryGetProperty(mapping.Value, out var value))
        {
            result[mapping.Key] = GetJsonValue(value);
        }
    }
    return result;
}

private object GetJsonValue(JsonElement element)
{
    return element.ValueKind switch
    {
        JsonValueKind.Number => element.GetDecimal(),
        JsonValueKind.String => element.GetString() ?? string.Empty,
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null!,
        _ => JsonSerializer.Deserialize<object>(element.GetRawText()) ?? new { }
    };
}

// 辅助类定义
private class DataSourceConfig
{
    public string? ApiUrl { get; set; }
    public Dictionary<string, string>? FieldMapping { get; set; }
    public int RefreshInterval { get; set; }
}

private class StatisticsConfig
{
    public string? Label { get; set; }
    public int? Status { get; set; }
    public string? TableName { get; set; }
}
```

- [ ] **Step 3: 在Controller添加数据代理接口**

```csharp
// 文件: EasyWechatWeb/Controllers/Desktop/UserWidgetController.cs
// 在类末尾添加接口

/// <summary>
/// 获取组件数据（刷新）
/// </summary>
[HttpGet("data/{widgetId}")]
[ProducesResponseType(typeof(ApiResponse<object>), 200)]
public async Task<ApiResponse<object>> GetWidgetData(Guid widgetId)
{
    try
    {
        var userId = GetCurrentUserId();
        var data = await _userWidgetService.GetWidgetDataAsync(widgetId, userId);
        return Success(data);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "获取组件数据失败: WidgetId={WidgetId}", widgetId);
        return Error<object>("获取数据失败");
    }
}
```

- [ ] **Step 4: 验证后端编译**

Run: `cd EasyWechatWeb/EasyWeChatWeb && dotnet build`
Expected: 编译成功

- [ ] **Step 5: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add BusinessManager/Desktop/IService/IUserWidgetService.cs
git add BusinessManager/Desktop/Service/UserWidgetService.cs
git add EasyWechatWeb/Controllers/Desktop/UserWidgetController.cs
git commit -m "feat(desktop): add widget data proxy endpoint

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 4: 前端API添加数据获取接口

**Files:**
- Modify: `PCWeb/src/api/basic/userWidgetConfigApi.ts`

- [ ] **Step 1: 在API文件添加数据获取函数**

```typescript
// 文件: PCWeb/src/api/basic/userWidgetConfigApi.ts
// 在文件末尾添加

/**
 * 获取组件数据（刷新）
 * @param widgetId 组件ID
 */
export function getWidgetData(widgetId: string) {
  return get<WidgetDataResponse>(`/api/desktop/user-config/data/${widgetId}`)
}

/**
 * 刷新所有组件数据
 */
export function refreshAllWidgets() {
  return post<boolean>('/api/desktop/user-config/refresh-all')
}
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/api/basic/userWidgetConfigApi.ts
git commit -m "feat(desktop): add widget data refresh API

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 5: 集成刷新机制到桌面页面

**Files:**
- Modify: `PCWeb/src/views/desktop/index.vue`
- Modify: `PCWeb/src/stores/desktopStore.ts`

- [ ] **Step 1: 在桌面Store添加刷新状态管理**

```typescript
// 文件: PCWeb/src/stores/desktopStore.ts
// 在现有内容后添加

// 刷新状态
const refreshStates = ref<Map<string, { loading: boolean; lastTime: number }>>(new Map())

// 全局刷新定时器
let globalTimer: number | null = null

// 初始化自动刷新
function initAutoRefresh(widgets: UserWidgetConfig[]) {
  stopAutoRefresh()
  
  // 找出最小刷新间隔（排除手动刷新）
  const intervals = widgets
    .filter(w => w.isEnabled && w.refreshInterval > 0)
    .map(w => w.refreshInterval)
  
  if (intervals.length === 0) return
  
  const minInterval = Math.min(...intervals)
  globalTimer = window.setInterval(() => {
    refreshWidgets()
  }, minInterval * 1000)
}

// 停止自动刷新
function stopAutoRefresh() {
  if (globalTimer) {
    clearInterval(globalTimer)
    globalTimer = null
  }
}

// 刷新所有组件
async function refreshWidgets() {
  const enabledWidgets = widgets.value.filter(w => w.isEnabled)
  for (const widget of enabledWidgets) {
    await refreshWidget(widget.widgetId)
  }
}

// 刷新单个组件
async function refreshWidget(widgetId: string) {
  refreshStates.value.set(widgetId, { loading: true, lastTime: Date.now() })
  try {
    const data = await getWidgetData(widgetId)
    widgetData.value.set(widgetId, data)
  } catch (error) {
    console.error(`刷新组件 ${widgetId} 失败:`, error)
  } finally {
    const state = refreshStates.value.get(widgetId)
    if (state) {
      state.loading = false
      refreshStates.value.set(widgetId, state)
    }
  }
}

// 手动刷新指定组件
function manualRefresh(widgetId: string) {
  return refreshWidget(widgetId)
}

// 在导出中添加
return {
  // ... 原有导出
  refreshStates,
  initAutoRefresh,
  stopAutoRefresh,
  refreshWidgets,
  manualRefresh,
}
```

- [ ] **Step 2: 在桌面页面集成刷新机制**

```vue
<!-- 文件: PCWeb/src/views/desktop/index.vue -->
<!-- 在 script setup 中修改 -->

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'
import { useDesktopStore } from '@/stores/desktopStore'

const desktopStore = useDesktopStore()

onMounted(async () => {
  await desktopStore.loadUserConfig()
  // 初始化自动刷新
  desktopStore.initAutoRefresh(desktopStore.widgets)
})

onUnmounted(() => {
  // 清理定时器
  desktopStore.stopAutoRefresh()
})
</script>
```

- [ ] **Step 3: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 4: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/stores/desktopStore.ts
git add PCWeb/src/views/desktop/index.vue
git commit -m "feat(desktop): integrate auto refresh to desktop

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 6: 集成刷新机制到卡片组件

**Files:**
- Modify: `PCWeb/src/views/desktop/components/CardWidget.vue`

- [ ] **Step 1: 在卡片组件添加刷新按钮和状态显示**

```vue
<!-- 文件: PCWeb/src/views/desktop/components/CardWidget.vue -->
<!-- 修改 template -->

<template>
  <div class="card-widget">
    <div class="widget-header">
      <div class="header-left">
        <el-icon v-if="config?.icon">
          <component :is="config.icon" />
        </el-icon>
        <span class="widget-name">{{ config?.widgetName }}</span>
      </div>
      <div class="header-right">
        <el-button
          link
          :loading="refreshState?.loading"
          @click="handleRefresh"
        >
          <el-icon><Refresh /></el-icon>
        </el-button>
      </div>
    </div>

    <div class="widget-body" v-loading="refreshState?.loading">
      <div class="value-section">
        <span class="value">{{ data?.value ?? 0 }}</span>
        <div v-if="data?.trend" class="trend">
          <el-icon v-if="data.trend === 'up'" color="#67c23a"><CaretTop /></el-icon>
          <el-icon v-else-if="data.trend === 'down'" color="#f56c6c"><CaretBottom /></el-icon>
          <span class="trend-value" :class="data.trend">
            {{ data.trendValue ?? '' }}
          </span>
        </div>
      </div>
      <div class="label-section">
        <span class="label">{{ data?.label ?? '' }}</span>
        <span class="refresh-time" v-if="refreshState?.lastTime">
          {{ formatTime(refreshState.lastTime) }}
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { Refresh, CaretTop, CaretBottom } from '@element-plus/icons-vue'
import { useDesktopStore } from '@/stores/desktopStore'
import type { UserWidgetConfig, WidgetDataResponse } from '@/types'

const props = defineProps<{
  config: UserWidgetConfig
}>()

const desktopStore = useDesktopStore()

const data = computed(() => 
  desktopStore.widgetData.get(props.config.widgetId) as WidgetDataResponse | undefined
)

const refreshState = computed(() =>
  desktopStore.refreshStates.get(props.config.widgetId)
)

// 手动刷新
const handleRefresh = () => {
  desktopStore.manualRefresh(props.config.widgetId)
}

// 格式化时间
const formatTime = (timestamp: number) => {
  const date = new Date(timestamp)
  return `${date.getHours()}:${date.getMinutes()}:${date.getSeconds()}`
}

// 点击跳转
const handleClick = () => {
  if (props.config.interactionConfig?.clickAction === 'navigate') {
    const url = props.config.interactionConfig.targetUrl
    if (url) {
      window.location.href = url
    }
  }
}
</script>

<style scoped lang="scss">
.card-widget {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);
  padding: 16px;
  cursor: pointer;
  transition: all 0.3s;

  &:hover {
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
  }

  .widget-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 12px;
    font-size: 14px;
    color: #606266;

    .header-left {
      display: flex;
      align-items: center;
      gap: 8px;
    }
  }

  .widget-body {
    .value-section {
      display: flex;
      align-items: baseline;
      gap: 8px;

      .value {
        font-size: 28px;
        font-weight: 600;
        color: #303133;
      }

      .trend {
        display: flex;
        align-items: center;
        font-size: 12px;

        .trend-value.up {
          color: #67c23a;
        }
        .trend-value.down {
          color: #f56c6c;
        }
      }
    }

    .label-section {
      margin-top: 8px;
      display: flex;
      justify-content: space-between;
      font-size: 12px;
      color: #909399;

      .refresh-time {
        font-size: 10px;
        color: #c0c4cc;
      }
    }
  }
}
</style>
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/views/desktop/components/CardWidget.vue
git commit -m "feat(desktop): add refresh to card widget

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 7: 集成刷新机制到列表组件

**Files:**
- Modify: `PCWeb/src/views/desktop/components/ListWidget.vue`

- [ ] **Step 1: 在列表组件添加刷新功能**

```vue
<!-- 文件: PCWeb/src/views/desktop/components/ListWidget.vue -->
<!-- 修改 template -->

<template>
  <div class="list-widget">
    <div class="widget-header">
      <span class="widget-name">{{ config?.widgetName }}</span>
      <el-button
        link
        :loading="refreshState?.loading"
        @click="handleRefresh"
      >
        <el-icon><Refresh /></el-icon>
      </el-button>
    </div>

    <div class="widget-body" v-loading="refreshState?.loading">
      <div
        v-for="item in data?.list"
        :key="item.id"
        class="list-item"
        @click="handleItemClick(item)"
      >
        <span class="item-title">{{ item.title }}</span>
        <el-tag v-if="item.status" size="small">{{ item.status }}</el-tag>
        <span v-if="item.time" class="item-time">{{ item.time }}</span>
      </div>
      <el-empty v-if="!data?.list?.length" description="暂无数据" :image-size="60" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Refresh } from '@element-plus/icons-vue'
import { useDesktopStore } from '@/stores/desktopStore'
import type { UserWidgetConfig, WidgetDataResponse } from '@/types'

const props = defineProps<{
  config: UserWidgetConfig
}>()

const desktopStore = useDesktopStore()

const data = computed(() => 
  desktopStore.widgetData.get(props.config.widgetId) as WidgetDataResponse | undefined
)

const refreshState = computed(() =>
  desktopStore.refreshStates.get(props.config.widgetId)
)

const handleRefresh = () => {
  desktopStore.manualRefresh(props.config.widgetId)
}

const handleItemClick = (item: any) => {
  if (props.config.interactionConfig?.clickAction === 'navigate') {
    const url = props.config.interactionConfig.targetUrl?.replace('{id}', item.id)
    if (url) {
      window.location.href = url
    }
  }
}
</script>

<style scoped lang="scss">
.list-widget {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);

  .widget-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 12px 16px;
    border-bottom: 1px solid #ebeef5;
    font-size: 14px;
    color: #606266;
  }

  .widget-body {
    padding: 8px;
    max-height: 200px;
    overflow-y: auto;

    .list-item {
      display: flex;
      align-items: center;
      padding: 8px 12px;
      cursor: pointer;
      transition: background 0.2s;
      border-radius: 4px;

      &:hover {
        background: #f5f7fa;
      }

      .item-title {
        flex: 1;
        font-size: 13px;
      }

      .item-time {
        font-size: 12px;
        color: #909399;
        margin-left: 8px;
      }
    }
  }
}
</style>
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/views/desktop/components/ListWidget.vue
git commit -m "feat(desktop): add refresh to list widget

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 8: 集成刷新机制到图表组件

**Files:**
- Modify: `PCWeb/src/views/desktop/components/ChartWidget.vue`

- [ ] **Step 1: 在图表组件添加刷新功能**

```vue
<!-- 文件: PCWeb/src/views/desktop/components/ChartWidget.vue -->
<!-- 修改 template -->

<template>
  <div class="chart-widget">
    <div class="widget-header">
      <span class="widget-name">{{ config?.widgetName }}</span>
      <el-button
        link
        :loading="refreshState?.loading"
        @click="handleRefresh"
      >
        <el-icon><Refresh /></el-icon>
      </el-button>
    </div>

    <div class="widget-body" v-loading="refreshState?.loading">
      <div ref="chartRef" class="chart-container"></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { Refresh } from '@element-plus/icons-vue'
import * as echarts from 'echarts'
import { useDesktopStore } from '@/stores/desktopStore'
import type { UserWidgetConfig, WidgetDataResponse } from '@/types'

const props = defineProps<{
  config: UserWidgetConfig
}>()

const desktopStore = useDesktopStore()
const chartRef = ref<HTMLDivElement>()
let chartInstance: echarts.ECharts | null = null

const data = computed(() => 
  desktopStore.widgetData.get(props.config.widgetId) as WidgetDataResponse | undefined
)

const refreshState = computed(() =>
  desktopStore.refreshStates.get(props.config.widgetId)
)

const handleRefresh = () => {
  desktopStore.manualRefresh(props.config.widgetId)
}

// 初始化图表
const initChart = () => {
  if (!chartRef.value) return
  
  chartInstance = echarts.init(chartRef.value)
  updateChart()
}

// 更新图表数据
const updateChart = () => {
  if (!chartInstance || !data.value?.chartData) return
  
  const chartData = data.value.chartData
  const option: echarts.EChartsOption = {
    tooltip: { trigger: 'axis' },
    legend: { data: chartData.series?.map(s => s.name) || [] },
    xAxis: { type: 'category', data: chartData.xAxis || [] },
    yAxis: { type: 'value' },
    series: chartData.series?.map(s => ({
      name: s.name,
      type: chartData.type,
      data: s.data,
    })) || [],
  }
  
  chartInstance.setOption(option)
}

// 监听数据变化
watch(data, updateChart)

// 窗口resize
const handleResize = () => {
  chartInstance?.resize()
}

onMounted(() => {
  initChart()
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  chartInstance?.dispose()
  window.removeEventListener('resize', handleResize)
})
</script>

<style scoped lang="scss">
.chart-widget {
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1);

  .widget-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 12px 16px;
    border-bottom: 1px solid #ebeef5;
    font-size: 14px;
    color: #606266;
  }

  .widget-body {
    padding: 16px;

    .chart-container {
      width: 100%;
      height: 200px;
    }
  }
}
</style>
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/views/desktop/components/ChartWidget.vue
git commit -m "feat(desktop): add refresh to chart widget

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 9: 后端完整集成测试

- [ ] **Step 1: 启动后端服务**

Run: `cd EasyWechatWeb/EasyWeChatWeb && dotnet run`
Expected: 服务启动在 http://localhost:7600

- [ ] **Step 2: 使用 Swagger 测试API**

访问: `http://localhost:7600/swagger`

测试以下接口：
1. `POST /api/desktop/widget/add` - 创建测试组件
2. `POST /api/desktop/role-config/save` - 保存角色配置
3. `GET /api/desktop/user-config/my` - 获取用户桌面配置
4. `GET /api/desktop/user-config/data/{widgetId}` - 获取组件数据

- [ ] **Step 3: 验证数据代理逻辑**

创建不同类型组件测试：
- API数据源组件
- 静态数据源组件
- 统计数据源组件

验证数据获取和字段映射正确。

---

## Task 10: 前端完整集成测试

- [ ] **Step 1: 启动前端开发服务器**

Run: `cd PCWeb && pnpm run dev`
Expected: 服务启动在 http://localhost:5173

- [ ] **Step 2: 测试组件管理页面**

访问: `http://localhost:5173/basic/desktop/widget`

验证：
- 组件列表正常显示
- 新建组件功能正常
- 编辑组件功能正常（4个分区）
- 删除组件功能正常
- 状态切换正常

- [ ] **Step 3: 测试角色分配页面**

访问: `http://localhost:5173/basic/desktop/role-config`

验证：
- 角色选择器正常
- 添加组件弹窗正常
- 排序和启用状态切换正常
- 布局预览正确显示
- 保存配置正常

- [ ] **Step 4: 测试用户桌面页面**

访问: `http://localhost:5173/desktop`

验证：
- 用户桌面正常加载
- 组件正确渲染（卡片、列表、图表）
- 手动刷新功能正常
- 自动刷新按配置间隔执行
- 点击跳转功能正常
- 布局设置弹窗正常
- 保存/恢复默认功能正常

---

## Task 11: 响应式布局测试

- [ ] **Step 1: 测试不同屏幕宽度**

在浏览器开发者工具中测试不同断点：

| 断点 | 宽度 | 预期列数 |
|------|------|----------|
| xs | <576px | 单列 |
| sm | 576px-768px | 2-3列 |
| md | 768px-992px | 3-4列 |
| lg | 992px-1200px | 4-6列 |
| xl | ≥1200px | 6-8列 |

- [ ] **Step 2: 验证栅格计算**

检查不同栅格宽度的组件：
- 3栅格组件在不同断点的实际宽度
- 6栅格组件在不同断点的实际宽度
- 12栅格组件在不同断点的实际宽度

---

## Task 12: 最终提交与文档更新

- [ ] **Step 1: 确认所有更改已提交**

Run: `cd D:/4-MyProject/EasyProject && git status`
Expected: 无未提交更改

- [ ] **Step 2: 更新设计文档状态**

修改 `docs/superpowers/specs/2026-05-14-desktop-widget-design.md`:

```markdown
> **状态**: 已完成
```

- [ ] **Step 3: Commit状态更新**

```bash
cd D:/4-MyProject/EasyProject
git add docs/superpowers/specs/2026-05-14-desktop-widget-design.md
git commit -m "docs: update desktop widget design status to completed

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Spec Coverage Check

设计文档覆盖情况（阶段五）：

| 设计文档章节 | 实现Task |
|-------------|----------|
| 8.1 刷新策略 | Task 2, Task 5-8 |
| 8.2 刷新间隔配置 | Task 1, Task 5 |
| 数据源代理 | Task 3, Task 4 |
| 系统测试 | Task 9-11 |

---

**文档版本**: 1.0
**最后更新**: 2026-05-14