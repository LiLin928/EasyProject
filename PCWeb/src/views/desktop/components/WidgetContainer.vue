<!-- 文件: PCWeb/src/views/desktop/components/WidgetContainer.vue -->

<template>
  <el-card shadow="hover" class="widget-container" :body-style="{ padding: '20px' }">
    <!-- 组件标题 -->
    <template #header>
      <div class="widget-header">
        <span class="widget-title">{{ widget.widgetName }}</span>
        <el-icon v-if="widget.icon" class="widget-icon">
          <component :is="widget.icon" />
        </el-icon>
      </div>
    </template>

    <!-- 动态加载组件 -->
    <component
      :is="rendererComponent"
      :widget="widget"
      :data="mockData"
      v-if="rendererComponent"
    />
    <EmptyWidget v-else :widget="widget" />
  </el-card>
</template>

<script setup lang="ts">
import { computed, defineAsyncComponent } from 'vue'
import type { UserWidgetConfigDto, WidgetDataResponse } from '@/types/desktopWidget'
import { WidgetType } from '@/types/desktopWidget'
import EmptyWidget from './renderers/EmptyWidget.vue'

// Props
const props = defineProps<{
  widget: UserWidgetConfigDto
}>()

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

// Mock 数据生成（用于预览）
const mockData = computed<WidgetDataResponse>(() => {
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
        images: [
          'https://picsum.photos/200/150?random=1',
          'https://picsum.photos/200/150?random=2',
          'https://picsum.photos/200/150?random=3',
        ],
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
})
</script>

<style scoped lang="scss">
.widget-container {
  height: 100%;
  min-height: 200px;

  .widget-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .widget-title {
      font-size: 14px;
      font-weight: 600;
      color: #303133;
    }

    .widget-icon {
      font-size: 18px;
      color: #409eff;
    }
  }
}
</style>