<!-- 文件: PCWeb/src/views/basic/desktop/role-config/components/LayoutPreview.vue -->
<template>
  <div class="layout-preview">
    <div v-if="widgets.length === 0" class="empty-preview">
      <el-empty description="暂无已分配组件" :image-size="80" />
    </div>

    <div v-else class="preview-grid">
      <div
        v-for="widget in sortedWidgets"
        :key="widget.widgetId"
        class="preview-card"
        :style="{ width: getWidgetWidth(widget.defaultWidth) }"
      >
        <div class="card-header">
          <el-icon class="card-icon">
            <component :is="getWidgetIcon(widget.widgetType)" />
          </el-icon>
          <span class="card-title">{{ widget.widgetName }}</span>
        </div>
        <div class="card-body">
          <el-tag size="small" :type="widget.isEnabled ? 'success' : 'info'">
            {{ widget.isEnabled ? '启用' : '禁用' }}
          </el-tag>
          <span class="card-size">{{ widget.defaultWidth }}栅格</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import {
  DataLine,
  List,
  Picture,
  TrendCharts,
} from '@element-plus/icons-vue'
import type { RoleWidgetConfig } from '@/types'
import { WidgetType } from '@/types/desktopWidget'

const props = defineProps<{
  widgets: RoleWidgetConfig[]
}>()

// 按排序字段排序的组件列表
const sortedWidgets = computed(() => {
  return [...props.widgets]
    .filter(w => w.isEnabled)
    .sort((a, b) => a.sortOrder - b.sortOrder)
})

// 计算组件宽度百分比
const getWidgetWidth = (gridCount: number) => {
  // 12栅格系统：每栅格约 8.33%
  const percentage = (gridCount / 12) * 100
  return `${percentage}%`
}

// 根据组件类型获取图标
const getWidgetIcon = (type: WidgetType) => {
  switch (type) {
    case WidgetType.Card:
      return DataLine
    case WidgetType.List:
      return List
    case WidgetType.Image:
      return Picture
    case WidgetType.Chart:
      return TrendCharts
    default:
      return DataLine
  }
}
</script>

<style scoped lang="scss">
.layout-preview {
  background: #f5f7fa;
  border-radius: 8px;
  padding: 16px;
  min-height: 200px;

  .empty-preview {
    display: flex;
    justify-content: center;
    align-items: center;
    min-height: 200px;
  }

  .preview-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 12px;
  }

  .preview-card {
    background: #fff;
    border-radius: 6px;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    padding: 12px;
    min-width: calc(100% / 12 * 3); // 最小3栅格
    transition: all 0.3s;

    &:hover {
      box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
      transform: translateY(-2px);
    }

    .card-header {
      display: flex;
      align-items: center;
      gap: 8px;
      margin-bottom: 8px;

      .card-icon {
        font-size: 18px;
        color: #409eff;
      }

      .card-title {
        font-size: 14px;
        font-weight: 500;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }
    }

    .card-body {
      display: flex;
      justify-content: space-between;
      align-items: center;

      .card-size {
        font-size: 12px;
        color: #909399;
      }
    }
  }
}
</style>