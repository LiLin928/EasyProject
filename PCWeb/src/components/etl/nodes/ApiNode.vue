<!-- src/components/etl/nodes/ApiNode.vue -->
<template>
  <BaseTaskNode
    :name="name"
    :icon="styleConfig.icon"
    :description="description"
    :style-config="styleConfig"
    :selected="selected"
    :status="status"
  />
</template>

<script setup lang="ts">
import { computed } from 'vue'
import BaseTaskNode from './BaseTaskNode.vue'
import { TaskNodeType, type ApiNodeConfig } from '@/types/etl'
import { nodeStyleMap } from '../utils/nodeRegistry'

// Props
const props = withDefaults(defineProps<{
  /** 节点名称 */
  name: string
  /** 节点配置 */
  config?: ApiNodeConfig
  /** 是否选中 */
  selected?: boolean
  /** 运行状态 */
  status?: 'idle' | 'running' | 'success' | 'error' | 'skipped'
}>(), {
  selected: false,
})

// 样式配置
const styleConfig = nodeStyleMap[TaskNodeType.API]

// 描述信息
const description = computed(() => {
  if (!props.config) return '调用外部 API'
  const { apiMethod, apiUrl, outputVariable } = props.config
  // 简化 URL 显示
  const shortUrl = apiUrl ? apiUrl.split('/').slice(0, 3).join('/') + '...' : ''
  return `${apiMethod || 'GET'} ${shortUrl}${outputVariable ? ` -> ${outputVariable}` : ''}`
})
</script>