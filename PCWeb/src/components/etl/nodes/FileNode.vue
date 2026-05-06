<!-- src/components/etl/nodes/FileNode.vue -->
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
import { TaskNodeType, type FileNodeConfig } from '@/types/etl'
import { nodeStyleMap } from '../utils/nodeRegistry'

// Props
const props = withDefaults(defineProps<{
  /** 节点名称 */
  name: string
  /** 节点配置 */
  config?: FileNodeConfig
  /** 是否选中 */
  selected?: boolean
  /** 运行状态 */
  status?: 'idle' | 'running' | 'success' | 'error' | 'skipped'
}>(), {
  selected: false,
})

// 样式配置
const styleConfig = nodeStyleMap[TaskNodeType.FILE]

// 描述信息
const description = computed(() => {
  if (!props.config) return '文件读写处理'
  const { fileOperation, fileType, filePath, outputVariable } = props.config
  const opMap: Record<string, string> = {
    read: '读取',
    write: '写入',
    delete: '删除',
    move: '移动',
    copy: '复制',
  }
  const opText = opMap[fileOperation] || '处理'
  const fileText = fileType || ''
  return `${opText} ${fileText}${filePath ? ` 文件` : ''}${outputVariable ? ` -> ${outputVariable}` : ''}`
})
</script>