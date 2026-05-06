<!-- src/components/etl/DagDesigner/configs/OutputConfig.vue -->
<template>
  <el-card shadow="never" class="config-card">
    <template #header>
      <span>{{ t('etl.dag.node.outputConfig') }}</span>
    </template>
    <el-form
      ref="formRef"
      :model="localConfig"
      :rules="formRules"
      label-width="100px"
      size="small"
      :disabled="readonly"
    >
      <el-form-item :label="t('etl.dag.node.datasource')" prop="datasourceId">
        <DatasourceSelector
          v-model="localConfig.datasourceId"
          :datasources="datasources"
          :placeholder="t('etl.dag.node.selectDatasource')"
          @update:model-value="emitUpdate"
        />
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.outputType')" prop="outputType">
        <el-select
          v-model="localConfig.outputType"
          :placeholder="t('etl.dag.node.selectOutputType')"
          style="width: 100%"
          @change="emitUpdate"
        >
          <el-option
            v-for="type in outputTypeOptions"
            :key="type.value"
            :label="type.label"
            :value="type.value"
          >
            <div style="display: flex; justify-content: space-between; align-items: center">
              <span>{{ type.label }}</span>
              <el-tag size="small" type="info">{{ type.desc }}</el-tag>
            </div>
          </el-option>
        </el-select>
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.targetTable')" prop="tableName">
        <el-input
          v-model="localConfig.tableName"
          :placeholder="t('etl.dag.node.targetTablePlaceholder')"
          @change="emitUpdate"
        />
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.inputVariable')" prop="inputVariable">
        <VariableInput
          v-model="localConfig.inputVariable"
          :placeholder="t('etl.dag.node.inputVariablePlaceholder')"
          @update:model-value="emitUpdate"
        />
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.batchSize')">
        <el-input-number
          v-model="localConfig.batchSize"
          :min="1"
          :max="10000"
          :placeholder="t('etl.dag.node.batchSizePlaceholder')"
          style="width: 100%"
          @change="emitUpdate"
        />
        <div class="form-tip">{{ t('etl.dag.node.batchSizeTip') }}</div>
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.onConflict')">
        <el-select
          v-model="localConfig.onConflict"
          :placeholder="t('etl.dag.node.selectOnConflict')"
          style="width: 100%"
          @change="emitUpdate"
        >
          <el-option
            v-for="option in onConflictOptions"
            :key="option.value"
            :label="option.label"
            :value="option.value"
          />
        </el-select>
      </el-form-item>

      <!-- 字段映射 -->
      <el-divider content-position="left">{{ t('etl.dag.node.fieldMapping') }}</el-divider>
      <el-form-item label="">
        <FieldMappingEditor
          v-model="fieldMappingItems"
          :readonly="readonly"
          @update:model-value="emitUpdate"
        />
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { useEtlStore } from '@/stores/etlStore'
import { DatasourceSelector, VariableInput, FieldMappingEditor } from './shared'
import type { OutputNodeConfig, FieldMappingItem } from '@/types/etl/taskNode'

const { t } = useLocale()
const etlStore = useEtlStore()

// Props
const props = defineProps<{
  /** 配置数据 */
  config: OutputNodeConfig
  /** 只读模式 */
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', config: OutputNodeConfig): void
}>()

// 表单引用
const formRef = ref<FormInstance>()

// 从 etlStore 获取数据源列表
const datasources = computed(() => etlStore.datasources)

// 输出类型选项
const outputTypeOptions = [
  { label: '插入', value: 'insert', desc: '新增数据' },
  { label: '更新', value: 'update', desc: '更新数据' },
  { label: '合并', value: 'merge', desc: '存在则更新，不存在则插入' },
  { label: '清空后插入', value: 'truncate_insert', desc: '清空表后插入' },
]

// 冲突处理选项
const onConflictOptions = [
  { label: '跳过', value: 'skip' },
  { label: '更新', value: 'update' },
  { label: '报错', value: 'error' },
]

// 本地配置
const localConfig = ref<OutputNodeConfig>({
  datasourceId: '',
  outputType: 'insert',
  tableName: '',
  inputVariable: '',
  fieldMapping: [],
  batchSize: 100,
  onConflict: 'error',
})

// 字段映射列表 computed (for FieldMappingEditor)
const fieldMappingItems = computed<FieldMappingItem[]>({
  get: () => localConfig.value.fieldMapping || [],
  set: (value) => {
    localConfig.value.fieldMapping = value
  },
})

// 表单验证规则
const formRules = computed<FormRules>(() => ({
  datasourceId: [
    { required: true, message: t('etl.dag.node.datasourceRequired'), trigger: 'change' },
  ],
  outputType: [
    { required: true, message: t('etl.dag.node.outputTypeRequired'), trigger: 'change' },
  ],
  tableName: [
    { required: true, message: t('etl.dag.node.targetTableRequired'), trigger: 'blur' },
  ],
  inputVariable: [
    { required: true, message: t('etl.dag.node.inputVariableRequired'), trigger: 'blur' },
    {
      pattern: /^[a-zA-Z_][a-zA-Z0-9_]*$/,
      message: t('etl.dag.node.variableNameInvalid'),
      trigger: 'blur',
    },
  ],
}))

// 监听 props 变化
watch(
  () => props.config,
  (newConfig) => {
    if (newConfig) {
      localConfig.value = {
        datasourceId: newConfig.datasourceId || '',
        outputType: newConfig.outputType || 'insert',
        tableName: newConfig.tableName || '',
        inputVariable: newConfig.inputVariable || '',
        fieldMapping: newConfig.fieldMapping || [],
        batchSize: newConfig.batchSize ?? 100,
        onConflict: newConfig.onConflict || 'error',
      }
    }
  },
  { immediate: true, deep: true }
)

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