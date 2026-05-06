<!-- src/components/ant_workflow/nodes/NotificationNodeConfig.vue -->
<template>
  <div class="notification-node-config">
    <el-form :model="localConfig" label-width="100px">
      <el-form-item :label="t('antWorkflow.nodeConfig.notificationConfig.notificationType')">
        <el-radio-group v-model="localConfig.notificationType">
          <el-radio value="message">{{ t('antWorkflow.nodeConfig.notificationConfig.message') }}</el-radio>
          <el-radio value="email">{{ t('antWorkflow.nodeConfig.notificationConfig.email') }}</el-radio>
          <el-radio value="sms">{{ t('antWorkflow.nodeConfig.notificationConfig.sms') }}</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.notificationConfig.title')">
        <el-input v-model="localConfig.title" />
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.notificationConfig.content')">
        <el-input v-model="localConfig.content" type="textarea" :rows="3" />
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.notificationConfig.recipients')">
        <UserRoleSelector
          :selected="selectorTargets"
          mode="multiple"
          :allowed-types="[SelectorTargetType.USER, SelectorTargetType.ROLE, SelectorTargetType.DEPT, SelectorTargetType.FORM_FIELD]"
          @update="handleSelectorUpdate"
        />
      </el-form-item>
      <el-form-item>
        <el-checkbox v-model="localConfig.sendToInitiator">{{ t('antWorkflow.nodeConfig.notificationConfig.sendToInitiator') }}</el-checkbox>
      </el-form-item>
      <el-form-item>
        <el-checkbox v-model="localConfig.sendToSupervisor">{{ t('antWorkflow.nodeConfig.notificationConfig.sendToSupervisor') }}</el-checkbox>
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { useLocale } from '@/composables/useLocale'
import UserRoleSelector from '../common/UserRoleSelector.vue'
import { SelectorTargetType, type SelectorTarget } from '@/types/antWorkflow/selector'
import type { NotificationRecipient } from '@/types/antWorkflow'

const { t } = useLocale()

interface NotificationConfig {
  notificationType: 'message' | 'email' | 'sms'
  title?: string
  content?: string
  recipients: NotificationRecipient[]
  sendToInitiator?: boolean
  sendToSupervisor?: boolean
}

const props = defineProps<{ config: NotificationConfig }>()
const emit = defineEmits<{ (e: 'update', config: NotificationConfig): void }>()

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

const localConfig = ref<NotificationConfig>({
  notificationType: 'message',
  recipients: [],
  sendToInitiator: false,
  sendToSupervisor: false,
})

// Convert NotificationRecipient[] to SelectorTarget[] for UserRoleSelector
const selectorTargets = computed<SelectorTarget[]>(() => {
  return (localConfig.value.recipients || []).map(r => ({
    id: r.value,
    name: r.name || r.value,
    type: r.type as SelectorTargetType,
  }))
})

// Convert SelectorTarget[] to NotificationRecipient[] and merge with checkbox states
const handleSelectorUpdate = (selected: SelectorTarget[]) => {
  const recipients: NotificationRecipient[] = selected.map(item => ({
    type: item.type as NotificationRecipient['type'],
    value: item.id,
    name: item.name,
  }))

  // Add initiator if checkbox is checked
  if (localConfig.value.sendToInitiator) {
    recipients.push({ type: 'initiator', value: 'initiator', name: t('antWorkflow.nodeConfig.notificationConfig.recipientInitiator') })
  }

  // Add supervisor if checkbox is checked
  if (localConfig.value.sendToSupervisor) {
    recipients.push({ type: 'supervisor', value: 'supervisor', name: t('antWorkflow.nodeConfig.notificationConfig.recipientSupervisor') })
  }

  localConfig.value.recipients = recipients
}

// Update recipients when checkboxes change
watch([() => localConfig.value.sendToInitiator, () => localConfig.value.sendToSupervisor], () => {
  // Filter out initiator and supervisor from recipients, then re-add based on checkbox states
  const otherRecipients = localConfig.value.recipients.filter(r => r.type !== 'initiator' && r.type !== 'supervisor')

  if (localConfig.value.sendToInitiator) {
    otherRecipients.push({ type: 'initiator', value: 'initiator', name: t('antWorkflow.nodeConfig.notificationConfig.recipientInitiator') })
  }

  if (localConfig.value.sendToSupervisor) {
    otherRecipients.push({ type: 'supervisor', value: 'supervisor', name: t('antWorkflow.nodeConfig.notificationConfig.recipientSupervisor') })
  }

  localConfig.value.recipients = otherRecipients
})

// 监听 props.config，更新 localConfig（防止循环，并确保默认值）
watch(() => props.config, (c) => {
  isUpdatingFromProps.value = true

  // 合并传入配置与默认配置，确保所有字段都有值
  localConfig.value = {
    notificationType: c?.notificationType || 'message',
    title: c?.title || '',
    content: c?.content || '',
    recipients: c?.recipients || [],
    sendToInitiator: c?.recipients?.some(r => r.type === 'initiator') ?? false,
    sendToSupervisor: c?.recipients?.some(r => r.type === 'supervisor') ?? false,
  }

  setTimeout(() => { isUpdatingFromProps.value = false }, 0)
}, { immediate: true })

// 监听 localConfig，emit update（防止循环）
watch(localConfig, (c) => {
  if (!isUpdatingFromProps.value) {
    emit('update', c)
  }
}, { deep: true })
</script>

<style scoped lang="scss">.notification-node-config { padding: 0; }</style>