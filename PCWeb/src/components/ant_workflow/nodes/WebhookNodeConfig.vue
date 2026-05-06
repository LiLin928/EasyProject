<!-- src/components/ant_workflow/nodes/WebhookNodeConfig.vue -->
<template>
  <div class="webhook-node-config">
    <el-form :model="localConfig" label-width="100px">
      <el-form-item :label="t('antWorkflow.nodeConfig.webhookConfig.url')">
        <el-input v-model="localConfig.url" placeholder="https://webhook.example.com/callback" />
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.webhookConfig.method')">
        <el-select v-model="localConfig.method">
          <el-option label="GET" value="GET" />
          <el-option label="POST" value="POST" />
          <el-option label="PUT" value="PUT" />
          <el-option label="DELETE" value="DELETE" />
          <el-option label="PATCH" value="PATCH" />
        </el-select>
      </el-form-item>

      <!-- Headers Editor -->
      <el-form-item :label="t('antWorkflow.nodeConfig.webhookConfig.headers')">
        <KeyValueEditor :data="headersData" @update="handleHeadersUpdate" />
      </el-form-item>

      <!-- Body Input -->
      <el-form-item :label="t('antWorkflow.nodeConfig.webhookConfig.body')">
        <el-input
          v-model="localConfig.body"
          type="textarea"
          :rows="4"
          placeholder='{"key": "value"}'
        />
      </el-form-item>

      <el-form-item :label="t('antWorkflow.nodeConfig.webhookConfig.trigger')">
        <el-radio-group v-model="localConfig.trigger">
          <el-radio value="before">{{ t('antWorkflow.nodeConfig.webhookConfig.triggerBefore') }}</el-radio>
          <el-radio value="after">{{ t('antWorkflow.nodeConfig.webhookConfig.triggerAfter') }}</el-radio>
          <el-radio value="manual">{{ t('antWorkflow.nodeConfig.webhookConfig.triggerManual') }}</el-radio>
        </el-radio-group>
      </el-form-item>

      <!-- Auth Config -->
      <el-form-item :label="t('antWorkflow.nodeConfig.webhookConfig.authConfig')">
        <el-select v-model="localConfig.authConfig!.type" @change="handleAuthTypeChange">
          <el-option :label="t('antWorkflow.nodeConfig.webhookConfig.authNone')" value="none" />
          <el-option :label="t('antWorkflow.nodeConfig.webhookConfig.authBasic')" value="basic" />
          <el-option :label="t('antWorkflow.nodeConfig.webhookConfig.authBearer')" value="bearer" />
          <el-option :label="t('antWorkflow.nodeConfig.webhookConfig.authApiKey')" value="api_key" />
        </el-select>
      </el-form-item>

      <!-- Basic Auth Config -->
      <template v-if="localConfig.authConfig?.type === 'basic'">
        <el-form-item label="Username">
          <el-input v-model="localConfig.authConfig.username" placeholder="Enter username" />
        </el-form-item>
        <el-form-item label="Password">
          <el-input v-model="localConfig.authConfig.password" type="password" placeholder="Enter password" show-password />
        </el-form-item>
      </template>

      <!-- Bearer Token Config -->
      <template v-if="localConfig.authConfig?.type === 'bearer'">
        <el-form-item label="Token">
          <el-input v-model="localConfig.authConfig.token" type="password" placeholder="Enter bearer token" show-password />
        </el-form-item>
      </template>

      <!-- API Key Config -->
      <template v-if="localConfig.authConfig?.type === 'api_key'">
        <el-form-item label="Key Name">
          <el-input v-model="localConfig.authConfig.keyName" placeholder="X-API-Key" />
        </el-form-item>
        <el-form-item label="Key Value">
          <el-input v-model="localConfig.authConfig.keyValue" type="password" placeholder="Enter API key value" show-password />
        </el-form-item>
        <el-form-item label="Add To">
          <el-select v-model="localConfig.authConfig.addTo">
            <el-option label="Header" value="header" />
            <el-option label="Query" value="query" />
          </el-select>
        </el-form-item>
      </template>

      <el-form-item :label="t('antWorkflow.nodeConfig.webhookConfig.timeout')">
        <el-input-number v-model="localConfig.timeout" :min="1000" :max="60000" :step="1000" />
      </el-form-item>

      <!-- Retry Config -->
      <el-divider content-position="left">{{ t('antWorkflow.nodeConfig.webhookConfig.retryConfig') }}</el-divider>
      <el-form-item :label="t('antWorkflow.nodeConfig.webhookConfig.retryCount')">
        <el-input-number v-model="localConfig.retryConfig!.count" :min="0" :max="10" />
      </el-form-item>
      <el-form-item :label="t('antWorkflow.nodeConfig.webhookConfig.retryInterval')">
        <el-input-number v-model="localConfig.retryConfig!.interval" :min="100" :max="10000" :step="100" />
      </el-form-item>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { useLocale } from '@/composables/useLocale'
import KeyValueEditor from '../common/KeyValueEditor.vue'

const { t } = useLocale()

interface KeyValueItem { key: string; value: string }

interface AuthConfig {
  type: 'none' | 'basic' | 'bearer' | 'api_key'
  // Basic auth
  username?: string
  password?: string
  // Bearer token
  token?: string
  // API Key
  keyName?: string
  keyValue?: string
  addTo?: 'header' | 'query'
}

interface RetryConfig {
  count: number
  interval: number
}

interface WebhookConfig {
  url: string
  method: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH'
  headers?: Record<string, string>
  body?: string
  trigger?: 'before' | 'after' | 'manual'
  timeout?: number
  authConfig?: AuthConfig
  retryConfig?: RetryConfig
}

const props = defineProps<{ config: WebhookConfig }>()
const emit = defineEmits<{ (e: 'update', config: WebhookConfig): void }>()

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

const localConfig = ref<WebhookConfig>({
  url: '',
  method: 'POST',
  headers: {},
  body: '',
  trigger: 'after',
  timeout: 30000,
  authConfig: { type: 'none' },
  retryConfig: { count: 0, interval: 1000 },
})

// Convert headers object to array for KeyValueEditor
const headersData = computed<KeyValueItem[]>(() => {
  const headers = localConfig.value.headers || {}
  return Object.entries(headers).map(([key, value]) => ({ key, value }))
})

// Handle headers update from KeyValueEditor
const handleHeadersUpdate = (data: KeyValueItem[]) => {
  const headers: Record<string, string> = {}
  data.forEach(item => {
    if (item.key) headers[item.key] = item.value
  })
  localConfig.value.headers = headers
}

// Reset auth config when type changes
const handleAuthTypeChange = () => {
  const type = localConfig.value.authConfig!.type
  localConfig.value.authConfig = { type }

  if (type === 'basic') {
    localConfig.value.authConfig.username = ''
    localConfig.value.authConfig.password = ''
  } else if (type === 'bearer') {
    localConfig.value.authConfig.token = ''
  } else if (type === 'api_key') {
    localConfig.value.authConfig.keyName = 'X-API-Key'
    localConfig.value.authConfig.keyValue = ''
    localConfig.value.authConfig.addTo = 'header'
  }
}

// 监听 props.config，更新 localConfig（防止循环）
watch(
  () => props.config,
  (c) => {
    if (c) {
      isUpdatingFromProps.value = true
      localConfig.value = {
        url: '',
        method: 'POST',
        headers: {},
        body: '',
        trigger: 'after',
        timeout: 30000,
        authConfig: { type: 'none' },
        retryConfig: { count: 0, interval: 1000 },
        ...c,
      }
      setTimeout(() => { isUpdatingFromProps.value = false }, 0)
    }
  },
  { immediate: true }
)

// 监听 localConfig，emit update（防止循环）
watch(localConfig, (c) => {
  if (!isUpdatingFromProps.value) {
    emit('update', c)
  }
}, { deep: true })
</script>

<style scoped lang="scss">
.webhook-node-config {
  padding: 0;

  .el-divider {
    margin: 16px 0;
  }
}
</style>