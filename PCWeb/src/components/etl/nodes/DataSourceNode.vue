<!-- src/components/etl/nodes/DataSourceNode.vue -->
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
import { TaskNodeType, type DataSourceNodeConfig } from '@/types/etl'
import { nodeStyleMap } from '../utils/nodeRegistry'

// Props
const props = withDefaults(defineProps<{
  /** 节点名称 */
  name: string
  /** 节点配置 */
  config?: DataSourceNodeConfig
  /** 是否选中 */
  selected?: boolean
  /** 运行状态 */
  status?: 'idle' | 'running' | 'success' | 'error' | 'skipped'
}>(), {
  selected: false,
})

// 样式配置
const styleConfig = nodeStyleMap[TaskNodeType.DATASOURCE]

// 描述信息
const description = computed(() => {
  if (!props.config) return '从数据源读取数据'
  const { queryType, tableName, outputVariable } = props.config
  if (queryType === 'table' && tableName) {
    return `表: ${tableName} -> ${outputVariable || '变量'}`
  }
  return `输出: ${outputVariable || '变量'}`
})
</script>