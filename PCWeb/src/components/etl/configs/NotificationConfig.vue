<!-- src/components/etl/DagDesigner/configs/NotificationConfig.vue -->
<template>
  <el-card shadow="never" class="config-card">
    <template #header>
      <span>{{ t('etl.dag.node.notificationConfig') }}</span>
    </template>
    <el-form
      ref="formRef"
      :model="localConfig"
      :rules="formRules"
      label-width="80px"
      size="small"
      :disabled="readonly"
    >
      <!-- Notification Type -->
      <el-form-item :label="t('etl.dag.node.notificationType')" prop="notificationType">
        <el-select
          v-model="localConfig.notificationType"
          :placeholder="t('etl.dag.node.selectNotificationType')"
          style="width: 100%"
          @change="handleNotificationTypeChange"
        >
          <el-option label="Email" value="email" />
          <el-option label="SMS" value="sms" />
          <el-option label="Webhook" value="webhook" />
          <el-option label="Message" value="message" />
        </el-select>
      </el-form-item>

      <!-- Trigger On -->
      <el-form-item :label="t('etl.dag.node.triggerOn')">
        <el-radio-group v-model="localConfig.triggerOn" @change="emitUpdate">
          <el-radio value="always">{{ t('etl.dag.node.triggerAlways') }}</el-radio>
          <el-radio value="success">{{ t('etl.dag.node.triggerSuccess') }}</el-radio>
          <el-radio value="failure">{{ t('etl.dag.node.triggerFailure') }}</el-radio>
        </el-radio-group>
        <div class="form-tip">{{ t('etl.dag.node.triggerOnTip') }}</div>
      </el-form-item>

      <!-- Email Config -->
      <template v-if="localConfig.notificationType === 'email'">
        <el-divider content-position="left">{{ t('etl.dag.node.emailConfig') }}</el-divider>

        <!-- Recipients -->
        <el-form-item :label="t('etl.dag.node.recipients')" prop="emailRecipients">
          <div class="tag-input-container">
            <el-tag
              v-for="recipient in emailRecipientsList"
              :key="recipient"
              closable
              :disable-transitions="false"
              @close="handleRemoveEmailRecipient(recipient)"
            >
              {{ recipient }}
            </el-tag>
            <el-input
              v-if="showEmailRecipientInput"
              ref="emailRecipientInputRef"
              v-model="newEmailRecipient"
              class="tag-input"
              size="small"
              :placeholder="t('etl.dag.node.recipientPlaceholder')"
              @keyup.enter="handleAddEmailRecipient"
              @blur="handleAddEmailRecipient"
            />
            <el-button
              v-else
              class="tag-button"
              size="small"
              @click="showEmailRecipientInput = true"
            >
              {{ t('etl.dag.node.addRecipient') }}
            </el-button>
          </div>
        </el-form-item>

        <!-- Subject -->
        <el-form-item :label="t('etl.dag.node.emailSubject')" prop="emailSubject">
          <el-input
            v-model="localConfig.emailConfig!.subject"
            :placeholder="t('etl.dag.node.emailSubjectPlaceholder')"
            @change="emitUpdate"
          />
        </el-form-item>

        <!-- Body -->
        <el-form-item :label="t('etl.dag.node.emailBody')">
          <MonacoEditor
            v-model="localConfig.emailConfig!.body"
            language="markdown"
            height="200px"
            :readonly="readonly"
            :show-format="false"
            :show-fullscreen="true"
            @update:model-value="emitUpdate"
          />
          <div class="form-tip">{{ t('etl.dag.node.emailBodyTip') }}</div>
        </el-form-item>

        <!-- Attachments -->
        <el-form-item :label="t('etl.dag.node.attachments')">
          <el-select
            v-model="localConfig.emailConfig!.attachments"
            multiple
            filterable
            allow-create
            default-first-option
            :placeholder="t('etl.dag.node.attachmentsPlaceholder')"
            style="width: 100%"
            @change="emitUpdate"
          />
          <div class="form-tip">{{ t('etl.dag.node.attachmentsTip') }}</div>
        </el-form-item>
      </template>

      <!-- SMS Config -->
      <template v-if="localConfig.notificationType === 'sms'">
        <el-divider content-position="left">{{ t('etl.dag.node.smsConfig') }}</el-divider>

        <!-- Recipients -->
        <el-form-item :label="t('etl.dag.node.smsRecipients')" prop="smsRecipients">
          <div class="tag-input-container">
            <el-tag
              v-for="recipient in smsRecipientsList"
              :key="recipient"
              closable
              :disable-transitions="false"
              @close="handleRemoveSmsRecipient(recipient)"
            >
              {{ recipient }}
            </el-tag>
            <el-input
              v-if="showSmsRecipientInput"
              ref="smsRecipientInputRef"
              v-model="newSmsRecipient"
              class="tag-input"
              size="small"
              :placeholder="t('etl.dag.node.smsRecipientPlaceholder')"
              @keyup.enter="handleAddSmsRecipient"
              @blur="handleAddSmsRecipient"
            />
            <el-button
              v-else
              class="tag-button"
              size="small"
              @click="showSmsRecipientInput = true"
            >
              {{ t('etl.dag.node.addRecipient') }}
            </el-button>
          </div>
        </el-form-item>

        <!-- Content -->
        <el-form-item :label="t('etl.dag.node.smsContent')" prop="smsContent">
          <el-input
            v-model="smsContent"
            type="textarea"
            :rows="4"
            :maxlength="500"
            show-word-limit
            :placeholder="t('etl.dag.node.smsContentPlaceholder')"
            @change="emitUpdate"
          />
          <div class="form-tip">{{ t('etl.dag.node.smsContentTip') }}</div>
        </el-form-item>
      </template>

      <!-- Webhook Config -->
      <template v-if="localConfig.notificationType === 'webhook'">
        <el-divider content-position="left">{{ t('etl.dag.node.webhookConfig') }}</el-divider>

        <!-- URL + Method -->
        <el-form-item :label="t('etl.dag.node.webhookUrl')" prop="webhookUrl">
          <div class="url-method-row">
            <el-select
              v-model="localConfig.webhookConfig!.method"
              :placeholder="t('etl.dag.node.selectWebhookMethod')"
              style="width: 100px"
              @change="emitUpdate"
            >
              <el-option label="GET" value="GET" />
              <el-option label="POST" value="POST" />
            </el-select>
            <el-input
              v-model="localConfig.webhookConfig!.url"
              :placeholder="t('etl.dag.node.webhookUrlPlaceholder')"
              style="flex: 1"
              @change="emitUpdate"
            />
          </div>
        </el-form-item>

        <!-- Headers -->
        <el-form-item :label="t('etl.dag.node.headers')">
          <KeyValueEditor
            v-model="webhookHeaders"
            :readonly="readonly"
            :default-item="{ key: '', value: '' }"
            @update:model-value="handleWebhookHeadersUpdate"
          />
        </el-form-item>

        <!-- Body (POST only) -->
        <template v-if="localConfig.webhookConfig!.method === 'POST'">
          <el-form-item :label="t('etl.dag.node.requestBody')">
            <MonacoEditor
              v-model="webhookBody"
              language="json"
              height="150px"
              :readonly="readonly"
              :show-format="true"
              :show-fullscreen="true"
              @update:model-value="handleWebhookBodyUpdate"
            />
            <div class="form-tip">{{ t('etl.dag.node.webhookBodyTip') }}</div>
          </el-form-item>
        </template>
      </template>

      <!-- Message Config -->
      <template v-if="localConfig.notificationType === 'message'">
        <el-divider content-position="left">{{ t('etl.dag.node.messageConfig') }}</el-divider>

        <!-- Recipients -->
        <el-form-item :label="t('etl.dag.node.messageRecipients')" prop="messageRecipients">
          <div class="tag-input-container">
            <el-tag
              v-for="recipient in messageRecipientsList"
              :key="recipient"
              closable
              :disable-transitions="false"
              @close="handleRemoveMessageRecipient(recipient)"
            >
              {{ recipient }}
            </el-tag>
            <el-input
              v-if="showMessageRecipientInput"
              ref="messageRecipientInputRef"
              v-model="newMessageRecipient"
              class="tag-input"
              size="small"
              :placeholder="t('etl.dag.node.messageRecipientPlaceholder')"
              @keyup.enter="handleAddMessageRecipient"
              @blur="handleAddMessageRecipient"
            />
            <el-button
              v-else
              class="tag-button"
              size="small"
              @click="showMessageRecipientInput = true"
            >
              {{ t('etl.dag.node.addRecipient') }}
            </el-button>
          </div>
        </el-form-item>

        <!-- Title -->
        <el-form-item :label="t('etl.dag.node.messageTitle')" prop="messageTitle">
          <el-input
            v-model="localConfig.messageConfig!.title"
            :placeholder="t('etl.dag.node.messageTitlePlaceholder')"
            @change="emitUpdate"
          />
        </el-form-item>

        <!-- Content -->
        <el-form-item :label="t('etl.dag.node.messageContent')">
          <el-input
            v-model="localConfig.messageConfig!.content"
            type="textarea"
            :rows="6"
            :placeholder="t('etl.dag.node.messageContentPlaceholder')"
            @change="emitUpdate"
          />
          <div class="form-tip">{{ t('etl.dag.node.messageContentTip') }}</div>
        </el-form-item>
      </template>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { ref, watch, nextTick, computed } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import type { NotificationNodeConfig } from '@/types/etl'
import { KeyValueEditor, MonacoEditor } from './shared'

const { t } = useLocale()

// Props
const props = defineProps<{
  /** 配置数据 */
  config: NotificationNodeConfig
  /** 只读模式 */
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', config: NotificationNodeConfig): void
}>()

// 表单引用
const formRef = ref<FormInstance>()

// Email recipient input refs
const emailRecipientInputRef = ref<HTMLInputElement>()
const showEmailRecipientInput = ref(false)
const newEmailRecipient = ref('')
const emailRecipientsList = ref<string[]>([])

// SMS recipient input refs
const smsRecipientInputRef = ref<HTMLInputElement>()
const showSmsRecipientInput = ref(false)
const newSmsRecipient = ref('')
const smsRecipientsList = ref<string[]>([])

// Message recipient input refs
const messageRecipientInputRef = ref<HTMLInputElement>()
const showMessageRecipientInput = ref(false)
const newMessageRecipient = ref('')
const messageRecipientsList = ref<string[]>([])

// SMS content (stored separately since NotificationNodeConfig doesn't have smsConfig)
const smsContent = ref('')

// Webhook headers computed (for KeyValueEditor)
const webhookHeaders = computed<Record<string, string>>({
  get: () => localConfig.value.webhookConfig?.headers || {},
  set: (value) => {
    if (localConfig.value.webhookConfig) {
      localConfig.value.webhookConfig.headers = value
    }
  },
})

// Webhook body computed (for MonacoEditor)
const webhookBody = computed<string>({
  get: () => localConfig.value.webhookConfig?.body || '',
  set: (value) => {
    if (localConfig.value.webhookConfig) {
      localConfig.value.webhookConfig.body = value
    }
  },
})

// 本地配置
const localConfig = ref<NotificationNodeConfig>({
  notificationType: 'email',
  triggerOn: 'always',
  emailConfig: {
    recipients: [],
    subject: '',
    body: '',
    attachments: [],
  },
  webhookConfig: {
    url: '',
    method: 'POST',
    headers: {},
    body: '',
  },
  messageConfig: {
    recipients: [],
    title: '',
    content: '',
  },
})

// 表单验证规则
const formRules = computed<FormRules>(() => ({
  notificationType: [
    { required: true, message: t('etl.dag.node.notificationTypeRequired'), trigger: 'change' },
  ],
  emailRecipients: [
    {
      validator: () => emailRecipientsList.value.length > 0,
      message: t('etl.dag.node.recipientsRequired'),
      trigger: 'change',
    },
  ],
  emailSubject: [
    { required: true, message: t('etl.dag.node.emailSubjectRequired'), trigger: 'blur' },
  ],
  smsRecipients: [
    {
      validator: () => smsRecipientsList.value.length > 0,
      message: t('etl.dag.node.recipientsRequired'),
      trigger: 'change',
    },
  ],
  smsContent: [
    { required: true, message: t('etl.dag.node.smsContentRequired'), trigger: 'blur' },
  ],
  webhookUrl: [
    { required: true, message: t('etl.dag.node.webhookUrlRequired'), trigger: 'blur' },
  ],
  messageRecipients: [
    {
      validator: () => messageRecipientsList.value.length > 0,
      message: t('etl.dag.node.recipientsRequired'),
      trigger: 'change',
    },
  ],
  messageTitle: [
    { required: true, message: t('etl.dag.node.messageTitleRequired'), trigger: 'blur' },
  ],
}))

// 监听 props 变化
watch(
  () => props.config,
  (newConfig) => {
    if (newConfig) {
      localConfig.value = {
        notificationType: newConfig.notificationType || 'email',
        triggerOn: newConfig.triggerOn || 'always',
        emailConfig: newConfig.emailConfig || {
          recipients: [],
          subject: '',
          body: '',
          attachments: [],
        },
        webhookConfig: newConfig.webhookConfig || {
          url: '',
          method: 'POST',
          headers: {},
          body: '',
        },
        messageConfig: newConfig.messageConfig || {
          recipients: [],
          title: '',
          content: '',
        },
      }

      // Initialize recipient lists
      emailRecipientsList.value = localConfig.value.emailConfig?.recipients || []
      smsRecipientsList.value = [] // SMS recipients would be stored separately
      messageRecipientsList.value = localConfig.value.messageConfig?.recipients || []
      smsContent.value = '' // SMS content would be stored separately
    }
  },
  { immediate: true, deep: true }
)

// Handle notification type change
const handleNotificationTypeChange = () => {
  emitUpdate()
}

// Email recipient handlers
const handleAddEmailRecipient = () => {
  if (newEmailRecipient.value) {
    emailRecipientsList.value.push(newEmailRecipient.value)
    localConfig.value.emailConfig!.recipients = [...emailRecipientsList.value]
    newEmailRecipient.value = ''
    emitUpdate()
  }
  showEmailRecipientInput.value = false
}

const handleRemoveEmailRecipient = (recipient: string) => {
  emailRecipientsList.value = emailRecipientsList.value.filter((r) => r !== recipient)
  localConfig.value.emailConfig!.recipients = [...emailRecipientsList.value]
  emitUpdate()
}

// Watch showEmailRecipientInput to focus input
watch(showEmailRecipientInput, (val) => {
  if (val) {
    nextTick(() => {
      emailRecipientInputRef.value?.focus()
    })
  }
})

// SMS recipient handlers
const handleAddSmsRecipient = () => {
  if (newSmsRecipient.value) {
    smsRecipientsList.value.push(newSmsRecipient.value)
    newSmsRecipient.value = ''
    emitUpdate()
  }
  showSmsRecipientInput.value = false
}

const handleRemoveSmsRecipient = (recipient: string) => {
  smsRecipientsList.value = smsRecipientsList.value.filter((r) => r !== recipient)
  emitUpdate()
}

// Watch showSmsRecipientInput to focus input
watch(showSmsRecipientInput, (val) => {
  if (val) {
    nextTick(() => {
      smsRecipientInputRef.value?.focus()
    })
  }
})

// Message recipient handlers
const handleAddMessageRecipient = () => {
  if (newMessageRecipient.value) {
    messageRecipientsList.value.push(newMessageRecipient.value)
    localConfig.value.messageConfig!.recipients = [...messageRecipientsList.value]
    newMessageRecipient.value = ''
    emitUpdate()
  }
  showMessageRecipientInput.value = false
}

const handleRemoveMessageRecipient = (recipient: string) => {
  messageRecipientsList.value = messageRecipientsList.value.filter((r) => r !== recipient)
  localConfig.value.messageConfig!.recipients = [...messageRecipientsList.value]
  emitUpdate()
}

// Watch showMessageRecipientInput to focus input
watch(showMessageRecipientInput, (val) => {
  if (val) {
    nextTick(() => {
      messageRecipientInputRef.value?.focus()
    })
  }
})

// Webhook handlers
const handleWebhookHeadersUpdate = (value: Record<string, string> | any[]) => {
  if (localConfig.value.webhookConfig) {
    localConfig.value.webhookConfig.headers = value as Record<string, string>
  }
  emitUpdate()
}

const handleWebhookBodyUpdate = (body: string) => {
  if (localConfig.value.webhookConfig) {
    localConfig.value.webhookConfig.body = body
  }
  emitUpdate()
}

// 发送更新
const emitUpdate = () => {
  emit('update', { ...localConfig.value })
}

// 暴露验证方法
defineExpose({
  validate: () => formRef.value?.validate(),
  resetFields: () => formRef.value?.resetFields(),
})
</script>

<style scoped lang="scss">
.config-card {
  margin: 0 0 16px 0;
  border: none;

  :deep(.el-card__header) {
    padding: 12px 16px;
    font-size: 13px;
    font-weight: 500;
    color: #303133;
    border-bottom: 1px solid #e4e7ed;
  }

  :deep(.el-card__body) {
    padding: 16px;
  }
}

.form-tip {
  margin-top: 4px;
  font-size: 11px;
  color: #909399;
  line-height: 1.4;
}

.url-method-row {
  display: flex;
  gap: 8px;
  align-items: center;
}

.tag-input-container {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  align-items: center;
  width: 100%;

  .el-tag {
    margin: 0;
  }

  .tag-input {
    width: 150px;
    vertical-align: middle;
  }

  .tag-button {
    vertical-align: middle;
  }
}

:deep(.el-divider__text) {
  font-size: 12px;
  color: #606266;
}

:deep(.el-textarea__inner) {
  font-family: 'Monaco', 'Menlo', 'Consolas', monospace;
  font-size: 12px;
}
</style>