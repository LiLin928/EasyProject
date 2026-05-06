<!-- src/components/etl/DagDesigner/configs/ApiConfig.vue -->
<template>
  <el-card shadow="never" class="config-card">
    <template #header>
      <span>{{ t('etl.dag.node.apiConfig') }}</span>
    </template>
    <el-form
      ref="formRef"
      :model="localConfig"
      :rules="formRules"
      label-width="80px"
      size="small"
      :disabled="readonly"
    >
      <!-- URL + Method -->
      <el-form-item :label="t('etl.dag.node.apiUrl')" prop="apiUrl">
        <div class="url-method-row">
          <el-select
            v-model="localConfig.apiMethod"
            :placeholder="t('etl.dag.node.selectApiMethod')"
            style="width: 100px"
            @change="emitUpdate"
          >
            <el-option label="GET" value="GET" />
            <el-option label="POST" value="POST" />
            <el-option label="PUT" value="PUT" />
            <el-option label="DELETE" value="DELETE" />
          </el-select>
          <el-input
            v-model="localConfig.apiUrl"
            :placeholder="t('etl.dag.node.apiUrlPlaceholder')"
            style="flex: 1"
            @change="emitUpdate"
          />
        </div>
        <div class="form-tip">{{ t('etl.dag.node.apiUrlTip') }}</div>
      </el-form-item>

      <!-- Headers 配置 -->
      <el-divider content-position="left">{{ t('etl.dag.node.headers') }}</el-divider>
      <KeyValueEditor
        v-model="apiHeadersRecord"
        :readonly="readonly"
        :default-item="{ key: '', value: '' }"
        @update:model-value="handleApiHeadersUpdate"
      />

      <!-- Body 配置 (非 GET 请求) -->
      <template v-if="localConfig.apiMethod !== 'GET'">
        <el-divider content-position="left">{{ t('etl.dag.node.requestBody') }}</el-divider>
        <el-form-item :label="t('etl.dag.node.bodyType')">
          <el-radio-group v-model="localConfig.apiBodyType" @change="emitUpdate">
            <el-radio value="json">JSON</el-radio>
            <el-radio value="form">Form</el-radio>
            <el-radio value="raw">Raw</el-radio>
          </el-radio-group>
        </el-form-item>

        <!-- JSON Body - 使用 MonacoEditor -->
        <el-form-item v-if="localConfig.apiBodyType === 'json'" prop="apiBody">
          <MonacoEditor
            v-model="apiBodyContent"
            language="json"
            height="200px"
            :readonly="readonly"
            :show-format="true"
            :show-fullscreen="true"
            @update:model-value="handleApiBodyUpdate"
          />
        </el-form-item>

        <!-- Form Body - 使用 KeyValueEditor -->
        <el-form-item v-else-if="localConfig.apiBodyType === 'form'" prop="apiBody">
          <KeyValueEditor
            v-model="formDataList"
            :readonly="readonly"
            :default-item="{ key: '', value: '' }"
            @update:model-value="handleFormDataUpdate"
          />
        </el-form-item>

        <!-- Raw Body -->
        <el-form-item v-else prop="apiBody">
          <el-input
            v-model="localConfig.apiBody"
            type="textarea"
            :rows="6"
            :placeholder="t('etl.dag.node.rawBodyPlaceholder')"
            @change="emitUpdate"
          />
        </el-form-item>
      </template>

      <!-- Timeout + Retry -->
      <el-form-item :label="t('etl.dag.node.timeout')">
        <el-input-number
          v-model="localConfig.timeout"
          :min="1"
          :max="600"
          :placeholder="t('etl.dag.node.timeoutPlaceholder')"
          style="width: 100%"
          @change="emitUpdate"
        />
        <div class="form-tip">{{ t('etl.dag.node.timeoutTip') }}</div>
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.retryOnFailure')">
        <el-switch
          v-model="localConfig.retryOnFailure"
          @change="emitUpdate"
        />
      </el-form-item>

      <!-- Output Variable -->
      <el-form-item :label="t('etl.dag.node.outputVariable')">
        <VariableInput
          v-model="outputVariableName"
          :placeholder="t('etl.dag.node.outputVariablePlaceholder')"
          :disabled="readonly"
          @update:model-value="handleOutputVariableUpdate"
        />
      </el-form-item>

      <!-- Response Mapping -->
      <el-divider content-position="left">{{ t('etl.dag.node.responseMapping') }}</el-divider>
      <FieldMappingEditor
        v-model="responseMappingFields"
        :readonly="readonly"
        @update:model-value="updateResponseMapping"
      />
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import type { ApiNodeConfig, FieldMappingItem } from '@/types/etl'
import { KeyValueEditor, MonacoEditor, VariableInput, FieldMappingEditor } from './shared'

const { t } = useLocale()

// Props
const props = defineProps<{
  /** 配置数据 */
  config: ApiNodeConfig
  /** 只读模式 */
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', config: ApiNodeConfig): void
}>()

// 表单引用
const formRef = ref<FormInstance>()

// Form 数据列表（用于 Form 类型的 Body）
const formDataList = ref<{ key: string; value: string }[]>([])

// API headers computed (for KeyValueEditor)
const apiHeadersRecord = computed<Record<string, string>>({
  get: () => localConfig.value.apiHeaders || {},
  set: (value) => {
    localConfig.value.apiHeaders = value
  },
})

// API body computed (for MonacoEditor)
const apiBodyContent = computed<string>({
  get: () => localConfig.value.apiBody || '',
  set: (value) => {
    localConfig.value.apiBody = value
  },
})

// Output variable computed (for VariableInput)
const outputVariableName = computed<string>({
  get: () => localConfig.value.outputVariable || '',
  set: (value) => {
    localConfig.value.outputVariable = value
  },
})

// 本地配置
const localConfig = ref<ApiNodeConfig>({
  apiUrl: '',
  apiMethod: 'GET',
  apiHeaders: {},
  apiBody: '',
  apiBodyType: 'json',
  timeout: 30,
  retryOnFailure: false,
  outputVariable: 'response',
  responseMapping: {
    fields: [],
  },
})

// 响应映射字段列表（用于 FieldMappingEditor）
const responseMappingFields = computed<FieldMappingItem[]>({
  get: () => localConfig.value.responseMapping?.fields || [],
  set: (value) => {
    if (localConfig.value.responseMapping) {
      localConfig.value.responseMapping.fields = value
    } else {
      localConfig.value.responseMapping = { fields: value }
    }
  },
})

// 表单验证规则
const formRules = computed<FormRules>(() => ({
  apiUrl: [
    { required: true, message: t('etl.dag.node.apiUrlRequired'), trigger: 'blur' },
  ],
}))

// 监听 props 变化
watch(
  () => props.config,
  (newConfig) => {
    if (newConfig) {
      localConfig.value = {
        apiUrl: newConfig.apiUrl || '',
        apiMethod: newConfig.apiMethod || 'GET',
        apiHeaders: newConfig.apiHeaders || {},
        apiBody: newConfig.apiBody || '',
        apiBodyType: newConfig.apiBodyType || 'json',
        timeout: newConfig.timeout ?? 30,
        retryOnFailure: newConfig.retryOnFailure ?? false,
        outputVariable: newConfig.outputVariable || 'response',
        responseMapping: newConfig.responseMapping || { fields: [] },
      }

      // 解析 form body
      if (newConfig.apiBodyType === 'form' && newConfig.apiBody) {
        try {
          const params = new URLSearchParams(newConfig.apiBody)
          formDataList.value = Array.from(params.entries()).map(([key, value]) => ({ key, value }))
        } catch {
          formDataList.value = []
        }
      } else {
        formDataList.value = []
      }
    }
  },
  { immediate: true, deep: true }
)

// 更新 Form 数据到 apiBody
const updateFormData = (items: { key: string; value: string }[]) => {
  formDataList.value = items
  const params = new URLSearchParams()
  items.forEach(item => {
    if (item.key) {
      params.append(item.key, item.value)
    }
  })
  localConfig.value.apiBody = params.toString()
  emitUpdate()
}

// 处理 KeyValueEditor form 数据更新
const handleFormDataUpdate = (value: Record<string, string> | any[]) => {
  updateFormData(value as { key: string; value: string }[])
}

// 更新 API headers
const handleApiHeadersUpdate = (value: Record<string, string> | any[]) => {
  localConfig.value.apiHeaders = value as Record<string, string>
  emitUpdate()
}

// 更新 API body
const handleApiBodyUpdate = (body: string) => {
  localConfig.value.apiBody = body
  emitUpdate()
}

// 更新 output variable
const handleOutputVariableUpdate = (value: string) => {
  localConfig.value.outputVariable = value
  emitUpdate()
}

// 更新响应映射
const updateResponseMapping = (fields: FieldMappingItem[]) => {
  localConfig.value.responseMapping = { fields }
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

:deep(.el-divider__text) {
  font-size: 12px;
  color: #606266;
}

:deep(.el-textarea__inner) {
  font-family: 'Monaco', 'Menlo', 'Consolas', monospace;
  font-size: 12px;
}
</style>