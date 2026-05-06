<!-- src/components/ant_workflow/nodes/ServiceNodeConfig.vue -->
<template>
  <div class="service-node-config">
    <el-form :model="localConfig" label-width="100px">
      <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.taskType')">
        <el-radio-group v-model="localConfig.taskType">
          <el-radio value="api">{{ t('antWorkflow.nodeConfig.serviceConfig.apiCall') }}</el-radio>
          <el-radio value="script">{{ t('antWorkflow.nodeConfig.serviceConfig.script') }}</el-radio>
          <el-radio value="expression">{{ t('antWorkflow.nodeConfig.serviceConfig.expression') }}</el-radio>
        </el-radio-group>
      </el-form-item>
      <template v-if="localConfig.taskType === 'api'">
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.apiUrl')">
          <el-input v-model="localConfig.apiConfig!.url" placeholder="https://api.example.com/endpoint" />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.apiMethod')">
          <el-select v-model="localConfig.apiConfig!.method">
            <el-option label="GET" value="GET" />
            <el-option label="POST" value="POST" />
            <el-option label="PUT" value="PUT" />
            <el-option label="DELETE" value="DELETE" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.apiHeaders')">
          <KeyValueEditor :data="headersData" @update="handleHeadersUpdate" />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.apiBody')">
          <el-input
            v-model="localConfig.apiConfig!.body"
            type="textarea"
            :rows="4"
            placeholder='{"key": "value"}'
          />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.resultVariable')">
          <el-input v-model="localConfig.apiConfig!.resultVariable" placeholder="result" />
        </el-form-item>
      </template>
      <template v-if="localConfig.taskType === 'script'">
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.scriptFormat')">
          <el-select v-model="localConfig.scriptConfig!.format">
            <el-option label="JavaScript" value="javascript" />
            <el-option label="Groovy" value="groovy" />
            <el-option label="Python" value="python" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.scriptContent')">
          <el-input v-model="localConfig.scriptConfig!.script" type="textarea" :rows="4" />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.resultVariable')">
          <el-input v-model="localConfig.scriptConfig!.resultVariable" placeholder="result" />
        </el-form-item>
      </template>
      <template v-if="localConfig.taskType === 'expression'">
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.expressionContent')">
          <el-input v-model="localConfig.expressionConfig!.expression" />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.resultVariable')">
          <el-input v-model="localConfig.expressionConfig!.resultVariable" placeholder="result" />
        </el-form-item>
      </template>
      <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.errorHandling')">
        <el-select v-model="localConfig.errorHandling!.strategy">
          <el-option :label="t('antWorkflow.nodeConfig.serviceConfig.continueOnError')" value="continue" />
          <el-option :label="t('antWorkflow.nodeConfig.serviceConfig.stopOnError')" value="stop" />
          <el-option :label="t('antWorkflow.nodeConfig.serviceConfig.retryOnError')" value="retry" />
        </el-select>
      </el-form-item>
      <template v-if="localConfig.errorHandling!.strategy === 'retry'">
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.retryCount')">
          <el-input-number v-model="localConfig.errorHandling!.retryCount" :min="1" :max="10" />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.nodeConfig.serviceConfig.retryInterval')">
          <el-input-number v-model="localConfig.errorHandling!.retryInterval" :min="1" :max="300" />
        </el-form-item>
      </template>
    </el-form>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import { useLocale } from '@/composables/useLocale'
import KeyValueEditor from '../common/KeyValueEditor.vue'

const { t } = useLocale()

interface KeyValueItem {
  key: string
  value: string
}

interface ApiConfig {
  url: string
  method: 'GET' | 'POST' | 'PUT' | 'DELETE'
  headers?: Record<string, string>
  body?: string
  resultVariable?: string
}

interface ScriptConfig {
  script: string
  format?: 'javascript' | 'groovy' | 'python'
  resultVariable?: string
}

interface ExpressionConfig {
  expression: string
  resultVariable?: string
}

interface ErrorHandling {
  strategy: 'continue' | 'stop' | 'retry'
  retryCount?: number
  retryInterval?: number
}

interface ServiceConfig {
  taskType: 'api' | 'script' | 'expression'
  apiConfig?: ApiConfig
  scriptConfig?: ScriptConfig
  expressionConfig?: ExpressionConfig
  errorHandling?: ErrorHandling
}

const props = defineProps<{ config: ServiceConfig }>()
const emit = defineEmits<{ (e: 'update', config: ServiceConfig): void }>()

// 防止循环更新的标记
const isUpdatingFromProps = ref(false)

const localConfig = ref<ServiceConfig>({
  taskType: 'api',
  apiConfig: { url: '', method: 'POST', headers: {}, body: '', resultVariable: '' },
  scriptConfig: { script: '', format: 'javascript', resultVariable: '' },
  expressionConfig: { expression: '', resultVariable: '' },
  errorHandling: { strategy: 'stop', retryCount: 3, retryInterval: 5 }
})

// 监听 props.config，更新 localConfig（防止循环）
watch(
  () => props.config,
  (c) => {
    if (c) {
      isUpdatingFromProps.value = true
      localConfig.value = {
        ...c,
        apiConfig: { ...localConfig.value.apiConfig, ...c.apiConfig },
        scriptConfig: { ...localConfig.value.scriptConfig, ...c.scriptConfig },
        expressionConfig: { ...localConfig.value.expressionConfig, ...c.expressionConfig },
        errorHandling: { ...localConfig.value.errorHandling, ...c.errorHandling }
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

// Headers computed and handlers for KeyValueEditor
const headersData = computed<KeyValueItem[]>(() => {
  const headers = localConfig.value.apiConfig?.headers || {}
  return Object.entries(headers).map(([key, value]) => ({ key, value }))
})

const handleHeadersUpdate = (data: KeyValueItem[]) => {
  const headers: Record<string, string> = {}
  data.forEach((item) => {
    if (item.key) {
      headers[item.key] = item.value
    }
  })
  localConfig.value.apiConfig!.headers = headers
}
</script>

<style scoped lang="scss">
.service-node-config {
  padding: 0;
}
</style>