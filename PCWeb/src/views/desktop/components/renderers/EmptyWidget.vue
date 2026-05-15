<!-- 文件: PCWeb/src/views/desktop/components/renderers/EmptyWidget.vue -->

<template>
  <el-empty description="未知组件类型" :image-size="80">
    <template #description>
      <p class="empty-description">
        未找到对应的组件渲染器
      </p>
      <p class="empty-info">
        组件ID: {{ widget.id }}
      </p>
      <p class="empty-info">
        组件名称: {{ widget.widgetName }}
      </p>
      <p class="empty-info">
        组件类型: {{ widget.widgetType }}
      </p>
    </template>
  </el-empty>
</template>

<script setup lang="ts">
import type { UserWidgetConfigDto } from '@/types/desktopWidget'

// Props
defineProps<{
  widget: UserWidgetConfigDto
}>()
</script>

<style scoped lang="scss">
.empty-description {
  font-size: 14px;
  color: #909399;
  margin-bottom: 8px;
}

.empty-info {
  font-size: 12px;
  color: #c0c4cc;
  margin: 4px 0;
}
</style>