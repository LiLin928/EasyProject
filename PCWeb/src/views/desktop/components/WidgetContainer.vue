<!-- 文件: PCWeb/src/views/desktop/components/WidgetContainer.vue -->

<template>
  <el-card
    shadow="hover"
    class="widget-container"
    :body-style="{ padding: '20px' }"
    @click="handleCardClick"
  >
    <!-- 组件标题 -->
    <template #header>
      <div class="widget-header">
        <span class="widget-title">{{ widget.widgetName }}</span>
        <div class="header-actions">
          <!-- 刷新按钮 -->
          <el-button
            link
            type="primary"
            :loading="refreshState?.loading"
            @click.stop="handleRefresh"
          >
            <el-icon><Refresh /></el-icon>
          </el-button>
          <el-icon v-if="widget.icon" class="widget-icon">
            <component :is="widget.icon" />
          </el-icon>
        </div>
      </div>
    </template>

    <!-- 刷新错误提示 -->
    <el-alert
      v-if="refreshState?.error"
      type="error"
      :title="refreshState.error"
      show-icon
      closable
      class="error-alert"
    />

    <!-- 动态加载组件 -->
    <component
      :is="rendererComponent"
      :widget="widget"
      :data="widgetData"
      :loading="refreshState?.loading"
      v-if="rendererComponent"
    />
    <EmptyWidget v-else :widget="widget" />

    <!-- 刷新时间显示 -->
    <div v-if="refreshState?.lastRefreshTime" class="refresh-time">
      <span>更新于 {{ formatTime(refreshState.lastRefreshTime) }}</span>
    </div>

    <!-- 点击跳转提示 -->
    <div v-if="hasInteraction" class="click-hint">
      <span>点击查看详情</span>
    </div>
  </el-card>
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent } from 'vue'
import { useRouter } from 'vue-router'
import { Refresh } from '@element-plus/icons-vue'
import type { UserWidgetConfigDto, WidgetDataResponse, RefreshState } from '@/types/desktopWidget'
import { WidgetType } from '@/types/desktopWidget'
import { useDesktopStore } from '@/stores/desktopStore'
import { useMenuStore } from '@/stores/permission'
import EmptyWidget from './renderers/EmptyWidget.vue'

// Props
const props = defineProps<{
  widget: UserWidgetConfigDto
}>()

// 桌面状态管理
const desktopStore = useDesktopStore()
const router = useRouter()
const menuStore = useMenuStore()

// 组件渲染器映射
const rendererMap: Record<WidgetType, any> = {
  [WidgetType.Card]: defineAsyncComponent(() => import('./renderers/CardWidget.vue')),
  [WidgetType.List]: defineAsyncComponent(() => import('./renderers/ListWidget.vue')),
  [WidgetType.Image]: defineAsyncComponent(() => import('./renderers/ImageWidget.vue')),
  [WidgetType.Chart]: defineAsyncComponent(() => import('./renderers/ChartWidget.vue')),
}

// 根据组件类型获取渲染器
const rendererComponent = computed(() => {
  return rendererMap[props.widget.widgetType] || null
})

// 获取刷新状态
const refreshState = computed<RefreshState | undefined>(() => {
  return desktopStore.getRefreshState(props.widget.widgetId)
})

// 获取组件数据（优先使用从后端获取的数据）
const widgetData = computed<WidgetDataResponse>(() => {
  // 从 store 获取实际数据
  const fetchedData = desktopStore.getWidgetDataById(props.widget.widgetId)
  if (fetchedData) {
    return fetchedData
  }

  // 否则使用 Mock 数据（用于预览）
  return generateMockData()
})

// 解析交互配置
const interactionConfig = computed(() => {
  if (props.widget.interactionConfig) {
    try {
      return JSON.parse(props.widget.interactionConfig)
    } catch {
      return null
    }
  }
  return null
})

// 是否有交互配置
const hasInteraction = computed(() => {
  return interactionConfig.value?.menuId || interactionConfig.value?.path
})

// 刷新处理
function handleRefresh() {
  desktopStore.manualRefresh(props.widget.widgetId)
}

// 点击卡片跳转
function handleCardClick() {
  const config = interactionConfig.value
  if (!config) return

  if (config.path) {
    router.push(config.path)
  } else if (config.menuId) {
    // 从菜单列表查找对应的 path
    const menu = findMenuById(config.menuId, menuStore.menuList)
    if (menu?.path) {
      router.push(menu.path)
    }
  }
}

// 递归查找菜单
function findMenuById(menuId: string, menus: any[]): any {
  for (const menu of menus) {
    if (menu.id === menuId) return menu
    if (menu.children?.length) {
      const found = findMenuById(menuId, menu.children)
      if (found) return found
    }
  }
  return null
}

// 格式化时间
function formatTime(date: Date): string {
  const now = new Date()
  const diff = Math.floor((now.getTime() - date.getTime()) / 1000)

  if (diff < 60) return '刚刚'
  if (diff < 3600) return `${Math.floor(diff / 60)}分钟前`
  if (diff < 86400) return `${Math.floor(diff / 3600)}小时前`
  return date.toLocaleString('zh-CN', { hour: '2-digit', minute: '2-digit' })
}

// Mock 数据生成（用于预览）
function generateMockData(): WidgetDataResponse {
  const type = props.widget.widgetType

  switch (type) {
    case WidgetType.Card:
      return {
        value: Math.floor(Math.random() * 10000),
        label: props.widget.widgetName,
      }

    case WidgetType.List:
      return {
        list: [
          { id: '1', name: '订单A', status: 1, statusLabel: '待处理', time: '10分钟前' },
          { id: '2', name: '订单B', status: 2, statusLabel: '已完成', time: '30分钟前' },
          { id: '3', name: '订单C', status: 0, statusLabel: '待审核', time: '1小时前' },
        ],
      }

    case WidgetType.Image:
      return {
        menus: [],
      }

    case WidgetType.Chart:
      const chartTypes: ('bar' | 'line' | 'pie')[] = ['bar', 'line', 'pie']
      const randomType = chartTypes[Math.floor(Math.random() * chartTypes.length)]

      if (randomType === 'pie') {
        return {
          chartData: {
            type: 'pie',
            title: props.widget.widgetName,
            pieData: [
              { name: '类型A', value: 30 },
              { name: '类型B', value: 50 },
              { name: '类型C', value: 20 },
            ],
          },
        }
      } else {
        return {
          chartData: {
            type: randomType,
            title: props.widget.widgetName,
            xAxis: ['周一', '周二', '周三', '周四', '周五', '周六', '周日'],
            yAxis: [120, 200, 150, 80, 70, 110, 130],
          },
        }
      }

    default:
      return {}
  }
}
</script>

<style scoped lang="scss">
.widget-container {
  height: 100%;
  min-height: 200px;
  cursor: pointer;

  &:hover {
    .click-hint {
      opacity: 1;
    }
  }

  .widget-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .widget-title {
      font-size: 14px;
      font-weight: 600;
      color: #303133;
    }

    .header-actions {
      display: flex;
      align-items: center;
      gap: 8px;

      .widget-icon {
        font-size: 18px;
        color: #409eff;
      }
    }
  }

  .error-alert {
    margin-bottom: 12px;
  }

  .refresh-time {
    margin-top: 8px;
    text-align: right;
    font-size: 12px;
    color: #909399;
  }

  .click-hint {
    margin-top: 8px;
    text-align: center;
    font-size: 12px;
    color: #409eff;
    opacity: 0;
    transition: opacity 0.3s ease;
  }
}
</style>