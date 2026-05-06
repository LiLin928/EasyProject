<!-- src/components/etl/nodes/TransformNode.vue -->
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
import { TaskNodeType, type TransformNodeConfig } from '@/types/etl'
import { nodeStyleMap } from '../utils/nodeRegistry'

// Props
const props = withDefaults(defineProps<{
  /** 节点名称 */
  name: string
  /** 节点配置 */
  config?: TransformNodeConfig
  /** 是否选中 */
  selected?: boolean
  /** 运行状态 */
  status?: 'idle' | 'running' | 'success' | 'error' | 'skipped'
}>(), {
  selected: false,
})

// 样式配置
const styleConfig = nodeStyleMap[TaskNodeType.TRANSFORM]

// 描述信息
const description = computed(() => {
  if (!props.config) return '数据转换处理'
  const { transformType, inputVariable, outputVariable } = props.config
  const typeMap: Record<string, string> = {
    mapping: '字段映射',
    filter: '数据过滤',
    aggregate: '数据聚合',
    script: '脚本转换',
  }
  const typeText = typeMap[transformType] || '转换'
  return `${inputVariable || '输入'} -> ${typeText} -> ${outputVariable || '输出'}`
})
</script>