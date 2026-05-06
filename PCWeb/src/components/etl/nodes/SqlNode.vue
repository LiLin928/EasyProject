<!-- src/components/etl/nodes/SqlNode.vue -->
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
import { TaskNodeType } from '@/types/etl'
import type { SqlNodeConfig } from '@/types/etl'
import { nodeStyleMap } from '../utils/nodeRegistry'

// Props
const props = withDefaults(defineProps<{
  /** 节点名称 */
  name: string
  /** 节点配置 */
  config?: SqlNodeConfig
  /** 是否选中 */
  selected?: boolean
  /** 运行状态 */
  status?: 'idle' | 'running' | 'success' | 'error' | 'skipped'
}>(), {
  selected: false,
})

// 样式配置
const styleConfig = nodeStyleMap[TaskNodeType.SQL]

// 描述信息
const description = computed(() => {
  if (!props.config) return '执行 SQL 查询'
  const { sqlType, outputVariable } = props.config
  const typeMap: Record<string, string> = {
    query: '查询',
    insert: '插入',
    update: '更新',
    delete: '删除',
    ddl: 'DDL',
  }
  const typeText = typeMap[sqlType] || '查询'
  if (outputVariable) {
    return `${typeText} -> ${outputVariable}`
  }
  return typeText
})
</script>