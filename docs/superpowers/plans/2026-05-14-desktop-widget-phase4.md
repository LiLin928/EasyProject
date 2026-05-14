# 桌面组件自动配置系统 - 阶段四：用户桌面页面与组件渲染器

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 完成用户桌面页面，用户可查看和调整自己的桌面布局，包含四种组件渲染器（卡片、列表、图片、图表）

**Architecture:** 采用 Vue 3 Composition API + 动态组件渲染 + ECharts图表库 + 栅格布局系统

**Tech Stack:** Vue 3.3 + TypeScript 5.0 + Element Plus 2.4 + ECharts 6.0 + Vite 5.4

---

## 文件结构

### 需要创建的文件

```
PCWeb/src/
├── types/
│   └── desktopWidget.ts              # 添加用户配置类型(修改)
│
├── api/basic/
│   └── userWidgetConfigApi.ts        # 用户配置API封装(新建)
│
├── stores/
│   └── desktopStore.ts               # 桌面状态管理
│
├── views/
│   ├── desktop/
│   │   ├── index.vue                 # 用户桌面主页面(重写现有)
│   │   └── components/
│   │   │   ├── LayoutSettingDialog.vue # 布局设置弹窗
│   │   │   ├── WidgetContainer.vue   # 组件容器包装器
│   │   │   └── renderers/
│   │   │   ├── CardWidget.vue        # 统计卡片渲染器
│   │   │   ├── ListWidget.vue        # 数据列表渲染器
│   │   │   ├── ImageWidget.vue       # 图片展示渲染器
│   │   │   └── ChartWidget.vue       # 图表统计渲染器
│   │   │   └── EmptyWidget.vue       # 空组件占位
```

---

## Task 1: 扩展用户配置类型定义

**Files:**
- Modify: `PCWeb/src/types/desktopWidget.ts`

- [ ] **Step 1: 添加用户配置类型**

在文件末尾添加：

```typescript
// ... 保留原有内容，添加以下内容 ...

/**
 * 用户组件配置项
 */
export interface UserWidgetConfigItem {
  widgetId: string
  width: number
  isEnabled: boolean
  sortOrder: number
}

/**
 * 用户组件配置DTO
 */
export interface UserWidgetConfigDto {
  id: string
  widgetId: string
  widgetName: string
  widgetType: WidgetType
  width: number
  defaultHeight: number
  icon?: string
  dataSourceConfig?: string
  interactionConfig?: string
  isEnabled: boolean
  sortOrder: number
}

/**
 * 保存用户配置参数
 */
export interface SaveUserWidgetConfigParams {
  widgets: UserWidgetConfigItem[]
}

/**
 * 用户桌面数据DTO
 */
export interface UserDesktopDto {
  widgets: UserWidgetConfigDto[]
  availableWidgets: AvailableWidget[]
}

/**
 * 组件数据响应
 */
export interface WidgetDataResponse {
  value: number | string
  label?: string
  list?: Array<{ id: string; name: string; status: number; time?: string }>
  images?: Array<{ url: string; title: string }>
  chartData?: {
    type: 'bar' | 'line' | 'pie'
    data: Array<{ name: string; value: number }>
  }
}
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/types/desktopWidget.ts
git commit -m "feat(desktop): add user widget config types

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 2: 创建用户配置API封装

**Files:**
- Create: `PCWeb/src/api/basic/userWidgetConfigApi.ts`

- [ ] **Step 1: 创建API封装文件**

```typescript
// 文件: PCWeb/src/api/basic/userWidgetConfigApi.ts

import { get, post } from '@/utils/request'
import type {
  UserDesktopDto,
  SaveUserWidgetConfigParams,
  WidgetDataResponse,
} from '@/types'

/**
 * 获取当前用户的桌面数据
 */
export function getUserDesktop() {
  return get<UserDesktopDto>('/api/desktop/user-config/my')
}

/**
 * 保存用户桌面配置
 * @param data 配置数据
 */
export function saveUserWidgetConfig(data: SaveUserWidgetConfigParams) {
  return post<boolean>('/api/desktop/user-config/save', data)
}

/**
 * 重置用户桌面为角色默认布局
 */
export function resetUserDesktop() {
  return post<boolean>('/api/desktop/user-config/reset')
}

/**
 * 获取组件数据(刷新)
 * @param widgetId 组件ID
 */
export function getWidgetData(widgetId: string) {
  return get<WidgetDataResponse>(`/api/desktop/user-config/data/${widgetId}`)
}
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/api/basic/userWidgetConfigApi.ts
git commit -m "feat(desktop): add user widget config API

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 3: 创建桌面状态管理Store

**Files:**
- Create: `PCWeb/src/stores/desktopStore.ts`

- [ ] **Step 1: 创建桌面Store**

```typescript
// 文件: PCWeb/src/stores/desktopStore.ts

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { getUserDesktop, saveUserWidgetConfig, resetUserDesktop } from '@/api/basic/userWidgetConfigApi'
import type { UserDesktopDto, UserWidgetConfigDto, UserWidgetConfigItem, AvailableWidget } from '@/types'

export const useDesktopStore = defineStore('desktop', () => {
  // State
  const widgets = ref<UserWidgetConfigDto[]>([])
  const availableWidgets = ref<AvailableWidget[]>([])
  const loading = ref(false)
  const saving = ref(false)

  // Getter - 启用的组件按排序排列
  const enabledWidgets = computed(() =>
    widgets.value
      .filter(w => w.isEnabled)
      .sort((a, b) => a.sortOrder - b.sortOrder)
  )

  // Actions
  async function fetchDesktop() {
    loading.value = true
    try {
      const data = await getUserDesktop()
      widgets.value = data.widgets
      availableWidgets.value = data.availableWidgets
    } finally {
      loading.value = false
    }
  }

  async function saveConfig(items: UserWidgetConfigItem[]) {
    saving.value = true
    try {
      await saveUserWidgetConfig({ widgets: items })
      await fetchDesktop()
    } finally {
      saving.value = false
    }
  }

  async function resetToDefault() {
    saving.value = true
    try {
      await resetUserDesktop()
      await fetchDesktop()
    } finally {
      saving.value = false
    }
  }

  function $reset() {
    widgets.value = []
    availableWidgets.value = []
    loading.value = false
    saving.value = false
  }

  return {
    widgets,
    availableWidgets,
    loading,
    saving,
    enabledWidgets,
    fetchDesktop,
    saveConfig,
    resetToDefault,
    $reset,
  }
})
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/stores/desktopStore.ts
git commit -m "feat(desktop): add desktop store

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 4: 重写用户桌面主页面

**Files:**
- Modify: `PCWeb/src/views/desktop/index.vue` (重写)

- [ ] **Step 1: 重写用户桌面页面**

```vue
<!-- 文件: PCWeb/src/views/desktop/index.vue -->
<template>
  <div class="desktop-container" v-loading="store.loading">
    <!-- 欢迎卡片 -->
    <el-card shadow="never" class="welcome-card">
      <div class="welcome-content">
        <el-avatar :size="64" :src="userAvatar" class="avatar">
          <el-icon :size="32"><User /></el-icon>
        </el-avatar>
        <div class="welcome-info">
          <h2 class="greeting">{{ greeting }}</h2>
          <p class="user-name">{{ userName }}</p>
        </div>
        <div class="action-buttons">
          <el-button type="primary" @click="handleSetting">
            <el-icon><Setting /></el-icon>
            设置布局
          </el-button>
          <el-button @click="handleRefresh">
            <el-icon><Refresh /></el-icon>
            刷新数据
          </el-button>
        </div>
      </div>
    </el-card>

    <!-- 组件网格布局 -->
    <div class="widget-grid" v-if="store.enabledWidgets.length > 0">
      <WidgetContainer
        v-for="widget in store.enabledWidgets"
        :key="widget.widgetId"
        :widget="widget"
        :style="{
          width: `${(widget.width / 12) * 100}%`,
        }"
      />
    </div>

    <el-empty v-else description="暂无桌面组件，请点击设置布局添加" />

    <!-- 布局设置弹窗 -->
    <LayoutSettingDialog v-model="settingDialogVisible" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { User, Setting, Refresh } from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'
import { useDesktopStore } from '@/stores/desktopStore'
import WidgetContainer from './components/WidgetContainer.vue'
import LayoutSettingDialog from './components/LayoutSettingDialog.vue'

const router = useRouter()
const userStore = useUserStore()
const store = useDesktopStore()

const settingDialogVisible = ref(false)

// 用户信息
const userInfo = computed(() => userStore.userInfo)
const userName = computed(() => userInfo.value?.nickname || userInfo.value?.username || '用户')
const userAvatar = computed(() => userInfo.value?.avatar || '')

// 根据时间生成问候语
const greeting = computed(() => {
  const hour = new Date().getHours()
  if (hour < 6) return '凌晨好'
  if (hour < 9) return '早上好'
  if (hour < 12) return '上午好'
  if (hour < 14) return '中午好'
  if (hour < 17) return '下午好'
  if (hour < 19) return '傍晚好'
  if (hour < 22) return '晚上好'
  return '夜深了'
})

// 生命周期
onMounted(() => {
  store.fetchDesktop()
})

// 设置布局
const handleSetting = () => {
  settingDialogVisible.value = true
}

// 刷新数据
const handleRefresh = () => {
  store.fetchDesktop()
}
</script>

<style scoped lang="scss">
.desktop-container {
  padding: 20px;
  background-color: #f5f7fa;
  min-height: calc(100vh - 60px);
}

.welcome-card {
  margin-bottom: 20px;

  .welcome-content {
    display: flex;
    align-items: center;
    gap: 20px;

    .avatar {
      flex-shrink: 0;
      background-color: #409eff;
    }

    .welcome-info {
      flex: 1;

      .greeting {
        margin: 0;
        font-size: 20px;
        font-weight: 600;
        color: #303133;
      }

      .user-name {
        margin: 8px 0;
        font-size: 14px;
        color: #606266;
      }
    }

    .action-buttons {
      display: flex;
      gap: 12px;
    }
  }
}

.widget-grid {
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
}
</style>
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/views/desktop/index.vue
git commit -m "feat(desktop): rewrite user desktop page

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 5: 创建组件容器包装器

**Files:**
- Create: `PCWeb/src/views/desktop/components/WidgetContainer.vue`

- [ ] **Step 1: 创建组件容器**

```vue
<!-- 文件: PCWeb/src/views/desktop/components/WidgetContainer.vue -->
<template>
  <el-card
    shadow="hover"
    class="widget-container"
    :style="{ minHeight: `${widget.defaultHeight}px` }"
    @click="handleClick"
  >
    <template #header>
      <div class="widget-header">
        <el-icon v-if="widget.icon">
          <component :is="widget.icon" />
        </el-icon>
        <span class="widget-title">{{ widget.widgetName }}</span>
      </div>
    </template>

    <div class="widget-body" v-loading="loading">
      <component
        :is="rendererComponent"
        :widget="widget"
        :data="widgetData"
      />
    </div>
  </el-card>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRouter } from 'vue-router'
import { WidgetType } from '@/types'
import type { UserWidgetConfigDto, WidgetDataResponse } from '@/types'
import CardWidget from './renderers/CardWidget.vue'
import ListWidget from './renderers/ListWidget.vue'
import ImageWidget from './renderers/ImageWidget.vue'
import ChartWidget from './renderers/ChartWidget.vue'
import EmptyWidget from './renderers/EmptyWidget.vue'

const props = defineProps<{
  widget: UserWidgetConfigDto
}>()

const router = useRouter()
const loading = ref(false)
const widgetData = ref<WidgetDataResponse | null>(null)

// 根据类型选择渲染器
const rendererComponent = computed(() => {
  const map = {
    [WidgetType.Card]: CardWidget,
    [WidgetType.List]: ListWidget,
    [WidgetType.Image]: ImageWidget,
    [WidgetType.Chart]: ChartWidget,
  }
  return map[props.widget.widgetType] || EmptyWidget
})

// 解析数据源配置
const dataSourceConfig = computed(() => {
  if (!props.widget.dataSourceConfig) return null
  try {
    return JSON.parse(props.widget.dataSourceConfig)
  } catch {
    return null
  }
})

// 解析交互配置
const interactionConfig = computed(() => {
  if (!props.widget.interactionConfig) return null
  try {
    return JSON.parse(props.widget.interactionConfig)
  } catch {
    return null
  }
})

// 加载组件数据
const loadWidgetData = async () => {
  if (!dataSourceConfig.value?.apiUrl) return
  
  loading.value = true
  try {
    // 这里简化处理，实际应该根据配置调用API
    // 暂时使用模拟数据
    widgetData.value = generateMockData(props.widget.widgetType)
  } finally {
    loading.value = false
  }
}

// 生成模拟数据
function generateMockData(type: WidgetType): WidgetDataResponse {
  switch (type) {
    case WidgetType.Card:
      return {
        value: Math.floor(Math.random() * 100),
        label: '↑ 今日新增',
      }
    case WidgetType.List:
      return {
        list: [
          { id: '1', name: '订单001', status: 1, time: '10:30' },
          { id: '2', name: '订单002', status: 2, time: '11:00' },
          { id: '3', name: '订单003', status: 3, time: '11:30' },
        ],
      }
    case WidgetType.Image:
      return {
        images: [
          { url: 'https://via.placeholder.com/200x150', title: '示例图片' },
        ],
      }
    case WidgetType.Chart:
      return {
        chartData: {
          type: 'bar',
          data: [
            { name: '周一', value: 120 },
            { name: '周二', value: 200 },
            { name: '周三', value: 150 },
            { name: '周四', value: 80 },
            { name: '周五', value: 70 },
          ],
        },
      }
    default:
      return { value: 0 }
  }
}

// 点击跳转
const handleClick = () => {
  if (interactionConfig.value?.targetUrl) {
    router.push(interactionConfig.value.targetUrl)
  }
}

// 生命周期
onMounted(() => {
  loadWidgetData()
})
</script>

<style scoped lang="scss">
.widget-container {
  cursor: pointer;
  transition: all 0.3s;
  box-sizing: border-box;
  min-width: 150px;

  &:hover {
    transform: translateY(-2px);
  }

  .widget-header {
    display: flex;
    align-items: center;
    gap: 8px;

    .widget-title {
      font-size: 14px;
      font-weight: 500;
    }
  }

  .widget-body {
    min-height: 60px;
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
git add PCWeb/src/views/desktop/components/WidgetContainer.vue
git commit -m "feat(desktop): add widget container wrapper

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 6: 创建统计卡片渲染器

**Files:**
- Create: `PCWeb/src/views/desktop/components/renderers/CardWidget.vue`

- [ ] **Step 1: 创建卡片渲染器**

```vue
<!-- 文件: PCWeb/src/views/desktop/components/renderers/CardWidget.vue -->
<template>
  <div class="card-widget">
    <div class="card-value">{{ data?.value || 0 }}</div>
    <div class="card-label">{{ data?.label || '' }}</div>
  </div>
</template>

<script setup lang="ts">
import type { UserWidgetConfigDto, WidgetDataResponse } from '@/types'

defineProps<{
  widget: UserWidgetConfigDto
  data: WidgetDataResponse | null
}>()
</script>

<style scoped lang="scss">
.card-widget {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 16px;
  text-align: center;

  .card-value {
    font-size: 36px;
    font-weight: 700;
    color: #409eff;
    line-height: 1.2;
  }

  .card-label {
    font-size: 12px;
    color: #67c23a;
    margin-top: 8px;
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
git add PCWeb/src/views/desktop/components/renderers/CardWidget.vue
git commit -m "feat(desktop): add card widget renderer

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 7: 创建数据列表渲染器

**Files:**
- Create: `PCWeb/src/views/desktop/components/renderers/ListWidget.vue`

- [ ] **Step 1: 创建列表渲染器**

```vue
<!-- 文件: PCWeb/src/views/desktop/components/renderers/ListWidget.vue -->
<template>
  <div class="list-widget">
    <div v-if="data?.list && data.list.length > 0" class="list-items">
      <div v-for="item in data.list" :key="item.id" class="list-item">
        <span class="item-name">{{ item.name }}</span>
        <el-tag :type="getStatusType(item.status)" size="small">
          {{ getStatusLabel(item.status) }}
        </el-tag>
      </div>
    </div>
    <el-empty v-else description="暂无数据" :image-size="40" />
  </div>
</template>

<script setup lang="ts">
import type { UserWidgetConfigDto, WidgetDataResponse } from '@/types'

defineProps<{
  widget: UserWidgetConfigDto
  data: WidgetDataResponse | null
}>()

function getStatusType(status: number): string {
  const map: Record<number, string> = {
    1: 'success',
    2: 'warning',
    3: 'info',
    4: 'danger',
  }
  return map[status] || 'info'
}

function getStatusLabel(status: number): string {
  const map: Record<number, string> = {
    1: '已完成',
    2: '待处理',
    3: '进行中',
    4: '已取消',
  }
  return map[status] || '未知'
}
</script>

<style scoped lang="scss">
.list-widget {
  .list-items {
    .list-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 8px 0;
      border-bottom: 1px solid #ebeef5;

      &:last-child {
        border-bottom: none;
      }

      .item-name {
        font-size: 13px;
        color: #606266;
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
git add PCWeb/src/views/desktop/components/renderers/ListWidget.vue
git commit -m "feat(desktop): add list widget renderer

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 8: 创建图片展示渲染器

**Files:**
- Create: `PCWeb/src/views/desktop/components/renderers/ImageWidget.vue`

- [ ] **Step 1: 创建图片渲染器**

```vue
<!-- 文件: PCWeb/src/views/desktop/components/renderers/ImageWidget.vue -->
<template>
  <div class="image-widget">
    <div v-if="data?.images && data.images.length > 0" class="image-container">
      <el-image
        v-for="(img, index) in data.images"
        :key="index"
        :src="img.url"
        :alt="img.title"
        fit="cover"
        class="widget-image"
        :preview-src-list="data.images.map(i => i.url)"
      />
    </div>
    <el-empty v-else description="暂无图片" :image-size="40" />
  </div>
</template>

<script setup lang="ts">
import type { UserWidgetConfigDto, WidgetDataResponse } from '@/types'

defineProps<{
  widget: UserWidgetConfigDto
  data: WidgetDataResponse | null
}>()
</script>

<style scoped lang="scss">
.image-widget {
  .image-container {
    display: flex;
    gap: 8px;

    .widget-image {
      width: 100%;
      height: 120px;
      border-radius: 4px;
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
git add PCWeb/src/views/desktop/components/renderers/ImageWidget.vue
git commit -m "feat(desktop): add image widget renderer

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 9: 创建图表统计渲染器

**Files:**
- Create: `PCWeb/src/views/desktop/components/renderers/ChartWidget.vue`

- [ ] **Step 1: 创建图表渲染器**

```vue
<!-- 文件: PCWeb/src/views/desktop/components/renderers/ChartWidget.vue -->
<template>
  <div class="chart-widget" ref="chartRef"></div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, onUnmounted } from 'vue'
import * as echarts from 'echarts'
import type { UserWidgetConfigDto, WidgetDataResponse } from '@/types'

const props = defineProps<{
  widget: UserWidgetConfigDto
  data: WidgetDataResponse | null
}>()

const chartRef = ref<HTMLDivElement | null>(null)
let chartInstance: echarts.ECharts | null = null

// 初始化图表
const initChart = () => {
  if (!chartRef.value) return
  
  chartInstance = echarts.init(chartRef.value)
  
  if (!props.data?.chartData) return
  
  const { type, data } = props.data.chartData
  let option: echarts.EChartsOption = {}
  
  switch (type) {
    case 'bar':
      option = {
        xAxis: {
          type: 'category',
          data: data.map(d => d.name),
        },
        yAxis: {
          type: 'value',
        },
        series: [{
          data: data.map(d => d.value),
          type: 'bar',
          itemStyle: {
            color: '#409eff',
          },
        }],
      }
      break
    case 'line':
      option = {
        xAxis: {
          type: 'category',
          data: data.map(d => d.name),
        },
        yAxis: {
          type: 'value',
        },
        series: [{
          data: data.map(d => d.value),
          type: 'line',
          smooth: true,
          itemStyle: {
            color: '#67c23a',
          },
        }],
      }
      break
    case 'pie':
      option = {
        series: [{
          type: 'pie',
          radius: '60%',
          data: data.map(d => ({ name: d.name, value: d.value })),
        }],
      }
      break
  }
  
  chartInstance.setOption(option)
}

// 监听数据变化
watch(() => props.data, () => {
  if (chartInstance) {
    initChart()
  }
}, { deep: true })

// 生命周期
onMounted(() => {
  initChart()
})

onUnmounted(() => {
  if (chartInstance) {
    chartInstance.dispose()
  }
})
</script>

<style scoped lang="scss">
.chart-widget {
  width: 100%;
  height: 100%;
  min-height: 150px;
}
</style>
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/views/desktop/components/renderers/ChartWidget.vue
git commit -m "feat(desktop): add chart widget renderer

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 10: 创建空组件占位

**Files:**
- Create: `PCWeb/src/views/desktop/components/renderers/EmptyWidget.vue`

- [ ] **Step 1: 创建空组件**

```vue
<!-- 文件: PCWeb/src/views/desktop/components/renderers/EmptyWidget.vue -->
<template>
  <el-empty description="组件类型未识别" :image-size="40" />
</template>

<script setup lang="ts">
import type { UserWidgetConfigDto, WidgetDataResponse } from '@/types'

defineProps<{
  widget: UserWidgetConfigDto
  data: WidgetDataResponse | null
}>()
</script>
```

- [ ] **Step 2: 验证编译**

Run: `cd PCWeb && pnpm run build`
Expected: 编译成功

- [ ] **Step 3: Commit**

```bash
cd D:/4-MyProject/EasyProject
git add PCWeb/src/views/desktop/components/renderers/EmptyWidget.vue
git commit -m "feat(desktop): add empty widget placeholder

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 11: 创建布局设置弹窗

**Files:**
- Create: `PCWeb/src/views/desktop/components/LayoutSettingDialog.vue`

- [ ] **Step 1: 创建布局设置弹窗**

```vue
<!-- 文件: PCWeb/src/views/desktop/components/LayoutSettingDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    title="桌面布局设置"
    width="700px"
    :close-on-click-modal="false"
  >
    <div class="setting-content">
      <!-- 可用组件列表 -->
      <div class="available-section">
        <div class="section-title">可用组件（从角色组件库中选择）</div>
        <div class="widget-grid">
          <div
            v-for="widget in store.availableWidgets"
            :key="widget.id"
            class="widget-card"
            :class="{ selected: isSelected(widget.id) }"
            @click="toggleWidget(widget.id)"
          >
            <div class="widget-check">
              <el-checkbox :model-value="isSelected(widget.id)" />
            </div>
            <div class="widget-info">
              <span class="widget-name">{{ widget.name }}</span>
              <el-tag size="small" :type="getTypeTagType(widget.type)">
                {{ getTypeLabel(widget.type) }}
              </el-tag>
            </div>
          </div>
        </div>
      </div>

      <!-- 已启用组件 - 调整尺寸 -->
      <div class="enabled-section">
        <div class="section-title">已启用组件 - 调整栅格宽度</div>
        <div class="enabled-list">
          <div
            v-for="widget in localWidgets"
            :key="widget.widgetId"
            class="enabled-item"
          >
            <div class="item-info">
              <span class="item-name">{{ getWidgetName(widget.widgetId) }}</span>
              <span class="item-size">当前: {{ widget.width }}栅格</span>
            </div>
            <div class="item-width">
              <el-radio-group v-model="widget.width" size="small">
                <el-radio-button :value="2">2</el-radio-button>
                <el-radio-button :value="3">3</el-radio-button>
                <el-radio-button :value="4">4</el-radio-button>
                <el-radio-button :value="6">6</el-radio-button>
                <el-radio-button :value="8">8</el-radio-button>
              </el-radio-group>
            </div>
          </div>
        </div>
      </div>
    </div>

    <template #footer>
      <el-button type="primary" :loading="saving" @click="handleSave">
        保存布局
      </el-button>
      <el-button type="warning" @click="handleReset">
        恢复默认
      </el-button>
      <el-button @click="visible = false">取消</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { useDesktopStore } from '@/stores/desktopStore'
import { WidgetType, widgetTypeLabels } from '@/types'
import type { UserWidgetConfigItem, AvailableWidget } from '@/types'

const props = defineProps<{
  modelValue: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const store = useDesktopStore()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const saving = ref(false)

// 本地编辑的组件列表
const localWidgets = ref<UserWidgetConfigItem[]>([])

// 监听弹窗打开
watch(visible, (val) => {
  if (val) {
    // 从store复制当前配置
    localWidgets.value = store.widgets
      .filter(w => w.isEnabled)
      .map(w => ({
        widgetId: w.widgetId,
        width: w.width,
        isEnabled: true,
        sortOrder: w.sortOrder,
      }))
  }
})

// 判断组件是否被选中
function isSelected(widgetId: string): boolean {
  return localWidgets.value.some(w => w.widgetId === widgetId)
}

// 切换组件启用状态
function toggleWidget(widgetId: string) {
  const index = localWidgets.value.findIndex(w => w.widgetId === widgetId)
  if (index > -1) {
    localWidgets.value.splice(index, 1)
  } else {
    const widget = store.availableWidgets.find(w => w.id === widgetId)
    if (widget) {
      localWidgets.value.push({
        widgetId,
        width: widget.defaultWidth,
        isEnabled: true,
        sortOrder: localWidgets.value.length,
      })
    }
  }
}

// 获取组件名称
function getWidgetName(widgetId: string): string {
  const widget = store.availableWidgets.find(w => w.id === widgetId)
  return widget?.name || '未知组件'
}

// 获取类型标签颜色
function getTypeTagType(type: number): string {
  const map: Record<number, string> = {
    [WidgetType.Card]: 'primary',
    [WidgetType.List]: 'success',
    [WidgetType.Image]: 'warning',
    [WidgetType.Chart]: 'danger',
  }
  return map[type] || 'info'
}

// 获取类型标签
function getTypeLabel(type: number): string {
  return widgetTypeLabels[type as WidgetType] || '未知'
}

// 保存布局
const handleSave = async () => {
  saving.value = true
  try {
    await store.saveConfig(localWidgets.value)
    ElMessage.success('保存成功')
    visible.value = false
  } catch (error) {
    ElMessage.error('保存失败')
  } finally {
    saving.value = false
  }
}

// 恢复默认
const handleReset = async () => {
  try {
    await store.resetToDefault()
    localWidgets.value = store.widgets
      .filter(w => w.isEnabled)
      .map(w => ({
        widgetId: w.widgetId,
        width: w.width,
        isEnabled: true,
        sortOrder: w.sortOrder,
      }))
    ElMessage.success('已恢复默认布局')
  } catch (error) {
    ElMessage.error('恢复失败')
  }
}
</script>

<style scoped lang="scss">
.setting-content {
  .section-title {
    font-size: 14px;
    font-weight: 600;
    color: #303133;
    margin-bottom: 12px;
  }

  .available-section {
    margin-bottom: 20px;

    .widget-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
      gap: 12px;

      .widget-card {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 12px;
        border: 1px solid #e4e7ed;
        border-radius: 8px;
        cursor: pointer;
        transition: all 0.2s;

        &:hover {
          background-color: #f5f7fa;
        }

        &.selected {
          border-color: #409eff;
          background-color: #ecf5ff;
        }

        .widget-info {
          display: flex;
          flex-direction: column;
          gap: 4px;

          .widget-name {
            font-size: 13px;
            font-weight: 500;
          }
        }
      }
    }
  }

  .enabled-section {
    .enabled-list {
      .enabled-item {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 12px;
        background: #f9fafc;
        border-radius: 8px;
        margin-bottom: 8px;

        .item-info {
          .item-name {
            font-size: 13px;
            font-weight: 500;
            color: #303133;
          }

          .item-size {
            font-size: 12px;
            color: #909399;
            margin-left: 12px;
          }
        }
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
git add PCWeb/src/views/desktop/components/LayoutSettingDialog.vue
git commit -m "feat(desktop): add layout setting dialog

Co-Authored-By: lilin <565387073@qq.com>"
```

---

## Task 12: 完整验证与测试

- [ ] **Step 1: 启动前端开发服务器**

Run: `cd PCWeb && pnpm run dev`
Expected: 服务器启动

- [ ] **Step 2: 访问用户桌面页面**

访问路径: `/desktop`
验证：
- 欢迎卡片正常显示
- 组件网格布局正确渲染
- 各类型组件渲染器正常显示
- 点击组件跳转正确

- [ ] **Step 3: 测试布局设置**

点击"设置布局"按钮：
- 可用组件列表正确显示
- 选择/取消组件功能正常
- 调整栅格宽度功能正常
- 保存/恢复默认功能正常

---

## Spec Coverage Check

设计文档覆盖情况（阶段四）：

| 设计文档章节 | 实现Task |
|-------------|----------|
| 7.3 用户桌面页面 | Task 4 |
| 4.1 统计卡片组件 | Task 6 |
| 4.2 数据列表组件 | Task 7 |
| 4.3 图片展示组件 | Task 8 |
| 4.4 图表统计组件 | Task 9 |
| 布局设置弹窗 | Task 11 |
| 栅格布局系统 | Task 5 |

---

**文档版本**: 1.0
**最后更新**: 2026-05-14