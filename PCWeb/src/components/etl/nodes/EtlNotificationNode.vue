<!-- src/components/etl/nodes/EtlNotificationNode.vue -->
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
import { TaskNodeType, type NotificationNodeConfig } from '@/types/etl'
import { nodeStyleMap } from '../utils/nodeRegistry'

// Props
const props = withDefaults(defineProps<{
  /** 节点名称 */
  name: string
  /** 节点配置 */
  config?: NotificationNodeConfig
  /** 是否选中 */
  selected?: boolean
  /** 运行状态 */
  status?: 'idle' | 'running' | 'success' | 'error' | 'skipped'
}>(), {
  selected: false,
})

// 样式配置
const styleConfig = nodeStyleMap[TaskNodeType.NOTIFICATION]

// 描述信息
const description = computed(() => {
  if (!props.config) return '发送通知消息'
  const { notificationType, triggerOn } = props.config
  const typeMap: Record<string, string> = {
    email: '邮件',
    sms: '短信',
    webhook: 'Webhook',
    message: '站内消息',
  }
  const typeText = typeMap[notificationType] || '通知'
  const triggerMap: Record<string, string> = {
    always: '始终',
    success: '成功时',
    failure: '失败时',
  }
  const triggerText = triggerMap[triggerOn] || ''
  return `${typeText}${triggerText ? ` (${triggerText})` : ''}`
})
</script>