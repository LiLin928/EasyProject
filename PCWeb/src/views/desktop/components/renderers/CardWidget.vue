<!-- 文件: PCWeb/src/views/desktop/components/renderers/CardWidget.vue -->

<template>
  <div class="card-widget">
    <div class="card-value">{{ formattedValue }}</div>
    <div class="card-label">{{ data?.label || widget.widgetName }}</div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { UserWidgetConfigDto, WidgetDataResponse } from '@/types/desktopWidget'

// Props
const props = defineProps<{
  widget: UserWidgetConfigDto
  data?: WidgetDataResponse
}>()

// 格式化数值显示
const formattedValue = computed(() => {
  const value = props.data?.value || 0

  // 根据数值大小决定格式化方式
  if (value >= 10000) {
    return `${(value / 10000).toFixed(1)}万`
  }

  return value.toLocaleString()
})
</script>

<style scoped lang="scss">
.card-widget {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 20px;
  text-align: center;

  .card-value {
    font-size: 36px;
    font-weight: 700;
    color: #409eff;
    margin-bottom: 10px;
  }

  .card-label {
    font-size: 14px;
    color: #909399;
  }
}
</style>