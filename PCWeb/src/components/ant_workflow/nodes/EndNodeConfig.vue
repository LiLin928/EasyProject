<!-- src/components/ant_workflow/nodes/EndNodeConfig.vue -->
<template>
  <div class="end-node-config">
    <el-form :model="localConfig" label-width="100px">
      <!-- 结束类型 -->
      <el-form-item :label="t('antWorkflow.nodeConfig.endConfig.endType')">
        <el-radio-group v-model="localConfig.endType">
          <el-radio value="success">{{ t('antWorkflow.nodeConfig.endConfig.endTypeSuccess') }}</el-radio>
          <el-radio value="reject">{{ t('antWorkflow.nodeConfig.endConfig.endTypeReject') }}</el-radio>
          <el-radio value="cancel">{{ t('antWorkflow.nodeConfig.endConfig.endTypeCancel') }}</el-radio>
        </el-radio-group>
      </el-form-item>

      <!-- 通知配置开关 -->
      <el-form-item :label="t('antWorkflow.nodeConfig.endConfig.notification')">
        <el-switch v-model="localConfig.notification.enabled" />
      </el-form-item>

      <!-- 通知详细配置（仅在启用时显示） -->
      <template v-if="localConfig.notification.enabled">
        <el-form-item :label="t('antWorkflow.nodeConfig.endConfig.notificationType')">
          <el-select v-model="localConfig.notification.type">
            <el-option value="message" :label="t('antWorkflow.nodeConfig.notificationConfig.message')" />
            <el-option value="email" :label="t('antWorkflow.nodeConfig.notificationConfig.email')" />
            <el-option value="sms" :label="t('antWorkflow.nodeConfig.notificationConfig.sms')" />
            <el-option value="wechat" :label="t('antWorkflow.nodeConfig.notificationConfig.wechat')" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeConfig.endConfig.notificationTitle')">
          <el-input v-model="localConfig.notification.title" placeholder="流程已完成" />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeConfig.endConfig.notificationContent')">
          <el-input
            v-model="localConfig.notification.content"
            type="textarea"
            :rows="3"
            placeholder="您的流程已成功结束"
          />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeConfig.endConfig.notificationRecipients')">
          <UserRoleSelector
            :selected="selectorTargets"
            mode="multiple"
            :allowed-types="[SelectorTargetType.USER, SelectorTargetType.ROLE, SelectorTargetType.FORM_FIELD]"
            @update="handleSelectorUpdate"
          />
        </el-form-item>
      </template>

      <!-- 回调 URL -->
      <el-form-item :label="t('antWorkflow.nodeConfig.endConfig.callbackUrl')">
        <el-input
          v-model="localConfig.callbackUrl"
          :placeholder="t('antWorkflow.nodeConfig.endConfig.callbackUrlPlaceholder')"
        />
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { useLocale } from '@/composables/useLocale'
import { EndType, type NotificationRecipient } from '@/types/antWorkflow'
import UserRoleSelector from '../common/UserRoleSelector.vue'
import { SelectorTargetType, type SelectorTarget } from '@/types/antWorkflow/selector'

const { t } = useLocale()

interface EndConfig {
  endType: EndType
  notification: {
    enabled: boolean
    type: 'message' | 'email' | 'sms' | 'wechat'
    title?: string
    content?: string
    recipients: NotificationRecipient[]
  }
  callbackUrl?: string
}

const props = defineProps<{ config: EndConfig }>()
const emit = defineEmits<{ (e: 'update', config: EndConfig): void }>()

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

const localConfig = ref<EndConfig>({
  endType: EndType.SUCCESS,
  notification: {
    enabled: false,
    type: 'message',
    title: '',
    content: '',
    recipients: [],
  },
  callbackUrl: '',
})

// Convert NotificationRecipient[] to SelectorTarget[] for UserRoleSelector
const selectorTargets = computed<SelectorTarget[]>({
  get: () => {
    return (localConfig.value.notification.recipients || []).map((recipient) => ({
      id: recipient.value,
      name: recipient.name || recipient.value,
      type: recipient.type === 'user' ? SelectorTargetType.USER : recipient.type === 'role' ? SelectorTargetType.ROLE : SelectorTargetType.FORM_FIELD,
    }))
  },
  set: (targets: SelectorTarget[]) => {
    localConfig.value.notification.recipients = targets.map((target) => ({
      type: target.type === SelectorTargetType.USER ? 'user' : target.type === SelectorTargetType.ROLE ? 'role' : 'formField',
      value: target.id,
      name: target.name,
    }))
  },
})

const handleSelectorUpdate = (selected: SelectorTarget[]) => {
  selectorTargets.value = selected
}

// 监听 props.config，更新 localConfig
watch(() => props.config, (c) => {
  isUpdatingFromProps.value = true

  localConfig.value = {
    endType: c?.endType || EndType.SUCCESS,
    notification: {
      enabled: c?.notification?.enabled || false,
      type: c?.notification?.type || 'message',
      title: c?.notification?.title || '',
      content: c?.notification?.content || '',
      recipients: c?.notification?.recipients || [],
    },
    callbackUrl: c?.callbackUrl || '',
  }

  setTimeout(() => { isUpdatingFromProps.value = false }, 0)
}, { immediate: true })

// 监听 localConfig，emit update
watch(localConfig, (c) => {
  if (!isUpdatingFromProps.value) {
    emit('update', c)
  }
}, { deep: true })
</script>

<style scoped lang="scss">
.end-node-config {
  padding: 0;
}
</style>