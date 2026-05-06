<!-- src/components/etl/nodes/OutputNode.vue -->
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
import { TaskNodeType, type OutputNodeConfig } from '@/types/etl'
import { nodeStyleMap } from '../utils/nodeRegistry'

// Props
const props = withDefaults(defineProps<{
  /** 节点名称 */
  name: string
  /** 节点配置 */
  config?: OutputNodeConfig
  /** 是否选中 */
  selected?: boolean
  /** 运行状态 */
  status?: 'idle' | 'running' | 'success' | 'error' | 'skipped'
}>(), {
  selected: false,
})

// 样式配置
const styleConfig = nodeStyleMap[TaskNodeType.OUTPUT]

// 描述信息
const description = computed(() => {
  if (!props.config) return '数据写入输出'
  const { outputType, tableName, inputVariable } = props.config
  const typeMap: Record<string, string> = {
    insert: '插入',
    update: '更新',
    merge: '合并',
    truncate_insert: '清空插入',
  }
  const typeText = typeMap[outputType] || '写入'
  return `${inputVariable || '变量'} -> ${typeText} -> ${tableName || '表'}`
})
</script>