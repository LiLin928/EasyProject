<!-- src/components/etl/nodes/ScriptNode.vue -->
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
import { TaskNodeType, type ScriptNodeConfig } from '@/types/etl'
import { nodeStyleMap } from '../utils/nodeRegistry'

// Props
const props = withDefaults(defineProps<{
  /** 节点名称 */
  name: string
  /** 节点配置 */
  config?: ScriptNodeConfig
  /** 是否选中 */
  selected?: boolean
  /** 运行状态 */
  status?: 'idle' | 'running' | 'success' | 'error' | 'skipped'
}>(), {
  selected: false,
})

// 样式配置
const styleConfig = nodeStyleMap[TaskNodeType.SCRIPT]

// 描述信息
const description = computed(() => {
  if (!props.config) return '执行自定义脚本'
  const { scriptType, scriptPath, outputVariable } = props.config
  const typeMap: Record<string, string> = {
    shell: 'Shell',
    python: 'Python',
    javascript: 'JavaScript',
  }
  const typeText = typeMap[scriptType] || '脚本'
  return `${typeText}${scriptPath ? `: ${scriptPath}` : ''}${outputVariable ? ` -> ${outputVariable}` : ''}`
})
</script>