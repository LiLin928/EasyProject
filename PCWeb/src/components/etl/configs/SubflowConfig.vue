<!-- src/components/etl/DagDesigner/configs/SubflowConfig.vue -->
<template>
  <el-card shadow="never" class="config-card">
    <template #header>
      <span>{{ t('etl.dag.node.subflowConfig') }}</span>
    </template>
    <el-form
      ref="formRef"
      :model="localConfig"
      :rules="formRules"
      label-width="100px"
      size="small"
      :disabled="readonly"
    >
      <!-- 任务流选择 -->
      <el-form-item :label="t('etl.dag.node.pipeline')" prop="pipelineId">
        <el-select
          v-model="localConfig.pipelineId"
          :placeholder="t('etl.dag.node.selectPipeline')"
          style="width: 100%"
          @change="emitUpdate"
        >
          <el-option
            v-for="pipeline in etlStore.pipelines"
            :key="pipeline.id"
            :label="pipeline.name"
            :value="pipeline.id"
          >
            <div style="display: flex; justify-content: space-between; align-items: center">
              <span>{{ pipeline.name }}</span>
              <span style="color: var(--el-text-color-secondary); font-size: 12px; margin-left: 8px">
                {{ pipeline.currentVersion }}
              </span>
            </div>
          </el-option>
        </el-select>
        <div class="form-tip">{{ t('etl.dag.node.pipelineTip') }}</div>
      </el-form-item>

      <!-- 输入映射 -->
      <el-divider content-position="left">{{ t('etl.dag.node.inputMapping') }}</el-divider>
      <FieldMappingEditor
        v-model="inputMappingFields"
        :readonly="readonly"
        @update:model-value="updateInputMapping"
      />
      <div class="form-tip">{{ t('etl.dag.node.inputMappingTip') }}</div>

      <!-- 输出映射 -->
      <el-divider content-position="left">{{ t('etl.dag.node.outputMapping') }}</el-divider>
      <FieldMappingEditor
        v-model="outputMappingFields"
        :readonly="readonly"
        @update:model-value="updateOutputMapping"
      />
      <div class="form-tip">{{ t('etl.dag.node.outputMappingTip') }}</div>

      <!-- 异步执行 -->
      <el-form-item :label="t('etl.dag.node.asyncExecution')">
        <el-switch
          v-model="localConfig.async"
          @change="emitUpdate"
        />
        <div class="form-tip">{{ t('etl.dag.node.asyncExecutionTip') }}</div>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { useEtlStore } from '@/stores/etlStore'
import type { SubflowNodeConfig, FieldMappingItem } from '@/types/etl/taskNode'
import { FieldMappingEditor } from './shared'

const { t } = useLocale()
const etlStore = useEtlStore()

// Props
const props = defineProps<{
  /** 配置数据 */
  config: SubflowNodeConfig
  /** 只读模式 */
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', config: SubflowNodeConfig): void
}>()

// 表单引用
const formRef = ref<FormInstance>()

// 本地配置
const localConfig = ref<SubflowNodeConfig>({
  pipelineId: '',
  inputMapping: [],
  outputMapping: [],
  async: false,
})

// 输入映射字段列表（用于 FieldMappingEditor）
const inputMappingFields = computed<FieldMappingItem[]>({
  get: () => localConfig.value.inputMapping || [],
  set: (value) => {
    localConfig.value.inputMapping = value
  },
})

// 输出映射字段列表（用于 FieldMappingEditor）
const outputMappingFields = computed<FieldMappingItem[]>({
  get: () => localConfig.value.outputMapping || [],
  set: (value) => {
    localConfig.value.outputMapping = value
  },
})

// 表单验证规则
const formRules = computed<FormRules>(() => ({
  pipelineId: [
    { required: true, message: t('etl.dag.node.pipelineRequired'), trigger: 'blur' },
  ],
}))

// 监听 props 变化
watch(
  () => props.config,
  (newConfig) => {
    if (newConfig) {
      localConfig.value = {
        pipelineId: newConfig.pipelineId || '',
        inputMapping: newConfig.inputMapping || [],
        outputMapping: newConfig.outputMapping || [],
        async: newConfig.async ?? false,
      }
    }
  },
  { immediate: true, deep: true }
)

// 更新输入映射
const updateInputMapping = (fields: FieldMappingItem[]) => {
  localConfig.value.inputMapping = fields
  emitUpdate()
}

// 更新输出映射
const updateOutputMapping = (fields: FieldMappingItem[]) => {
  localConfig.value.outputMapping = fields
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

:deep(.el-divider__text) {
  font-size: 12px;
  color: #606266;
}
</style>