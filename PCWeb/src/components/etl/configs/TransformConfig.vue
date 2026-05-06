<!-- src/components/etl/DagDesigner/configs/TransformConfig.vue -->
<template>
  <el-card shadow="never" class="config-card">
    <template #header>
      <span>{{ t('etl.dag.node.transformConfig') }}</span>
    </template>
    <el-form
      ref="formRef"
      :model="localConfig"
      :rules="formRules"
      label-width="80px"
      size="small"
      :disabled="readonly"
    >
      <el-form-item :label="t('etl.dag.node.transformType')" prop="transformType">
        <el-select
          v-model="localConfig.transformType"
          :placeholder="t('etl.dag.node.selectTransformType')"
          style="width: 100%"
          @change="handleTransformTypeChange"
        >
          <el-option
            v-for="type in transformTypeOptions"
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

      <el-form-item :label="t('etl.dag.node.inputVariable')" prop="inputVariable">
        <VariableInput
          v-model="localConfig.inputVariable"
          :placeholder="t('etl.dag.node.inputVariablePlaceholder')"
          :disabled="readonly"
          @update:model-value="emitUpdate"
        />
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.outputVariable')" prop="outputVariable">
        <VariableInput
          v-model="localConfig.outputVariable"
          :placeholder="t('etl.dag.node.outputVariablePlaceholder')"
          :disabled="readonly"
          @update:model-value="emitUpdate"
        />
      </el-form-item>

      <!-- 字段映射模式 -->
      <template v-if="localConfig.transformType === 'mapping'">
        <el-divider content-position="left">{{ t('etl.dag.node.fieldMapping') }}</el-divider>
        <el-form-item>
          <FieldMappingEditor
            v-model="localConfig.fieldMapping!"
            :readonly="readonly"
            @update:model-value="emitUpdate"
          />
        </el-form-item>
      </template>

      <!-- 数据过滤模式 -->
      <template v-if="localConfig.transformType === 'filter'">
        <el-divider content-position="left">{{ t('etl.dag.node.filterExpression') }}</el-divider>
        <el-form-item prop="filterExpression">
          <MonacoEditor
            v-model="localConfig.filterExpression!"
            language="javascript"
            height="200px"
            :readonly="readonly"
            :show-format="false"
            :show-fullscreen="true"
            @update:model-value="emitUpdate"
          />
          <div class="form-tip">
            {{ t('etl.dag.node.filterExpressionTip') }}
          </div>
        </el-form-item>
      </template>

      <!-- 数据聚合模式 -->
      <template v-if="localConfig.transformType === 'aggregate'">
        <el-divider content-position="left">{{ t('etl.dag.node.aggregateConfig') }}</el-divider>

        <el-form-item :label="t('etl.dag.node.groupBy')">
          <el-select
            v-model="localConfig.aggregateConfig!.groupBy"
            multiple
            filterable
            allow-create
            :placeholder="t('etl.dag.node.groupByPlaceholder')"
            style="width: 100%"
            :disabled="readonly"
            @change="emitUpdate"
          />
        </el-form-item>

        <el-form-item :label="t('etl.dag.node.aggregations')">
          <div class="aggregate-editor">
            <KeyValueEditor
              v-model="aggregateItems"
              :columns="aggregateColumns"
              :readonly="readonly"
              :default-item="defaultAggregateItem"
              @update:model-value="handleAggregateChange"
            />
          </div>
        </el-form-item>
      </template>

      <!-- 脚本转换模式 -->
      <template v-if="localConfig.transformType === 'script'">
        <el-divider content-position="left">{{ t('etl.dag.node.scriptConfig') }}</el-divider>

        <el-form-item :label="t('etl.dag.node.scriptLanguage')" prop="scriptLanguage">
          <el-select
            v-model="localConfig.scriptLanguage"
            :placeholder="t('etl.dag.node.selectScriptLanguage')"
            style="width: 100%"
            :disabled="readonly"
            @change="handleScriptLanguageChange"
          >
            <el-option label="JavaScript" value="javascript" />
            <el-option label="Python" value="python" />
            <el-option label="SQL" value="sql" />
          </el-select>
        </el-form-item>

        <el-form-item :label="t('etl.dag.node.script')" prop="script">
          <MonacoEditor
            v-model="localConfig.script!"
            :language="scriptLanguageForMonaco"
            height="250px"
            :readonly="readonly"
            :show-format="localConfig.scriptLanguage === 'sql'"
            :show-fullscreen="true"
            @update:model-value="emitUpdate"
          />
          <div class="form-tip">
            {{ t('etl.dag.node.scriptTip') }}
          </div>
        </el-form-item>
      </template>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import type {
  TransformNodeConfig,
  AggregateItem,
} from '@/types/etl/taskNode'
import VariableInput from './shared/VariableInput.vue'
import MonacoEditor from './shared/MonacoEditor.vue'
import FieldMappingEditor from './shared/FieldMappingEditor.vue'
import KeyValueEditor, { type Column } from './shared/EtlDagKeyValueEditor.vue'

const { t } = useLocale()

// Props
const props = defineProps<{
  /** 配置数据 */
  config: TransformNodeConfig
  /** 只读模式 */
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', config: TransformNodeConfig): void
}>()

// 表单引用
const formRef = ref<FormInstance>()

// 转换类型选项
const transformTypeOptions = [
  { label: '字段映射', value: 'mapping', desc: '重命名和转换字段' },
  { label: '数据过滤', value: 'filter', desc: '按条件筛选数据' },
  { label: '数据聚合', value: 'aggregate', desc: '分组统计汇总' },
  { label: '脚本转换', value: 'script', desc: '自定义脚本处理' },
]

// 本地配置
const localConfig = ref<TransformNodeConfig>({
  transformType: 'mapping',
  inputVariable: '',
  outputVariable: 'result',
  fieldMapping: [],
  filterExpression: '',
  aggregateConfig: {
    groupBy: [],
    aggregations: [],
  },
  script: '',
  scriptLanguage: 'javascript',
})

// 聚合列配置
const aggregateColumns: Column[] = [
  { prop: 'function', label: '聚合函数', type: 'select', placeholder: '选择函数', options: ['sum', 'avg', 'count', 'max', 'min'] },
  { prop: 'field', label: '字段', type: 'input', placeholder: '输入字段名' },
  { prop: 'alias', label: '别名', type: 'input', placeholder: '输出字段名' },
]

// 默认聚合项
const defaultAggregateItem = { function: 'sum', field: '', alias: '' }

// 聚合项数组（用于 KeyValueEditor）
const aggregateItems = ref<any[]>([])

// 转换脚本语言为 Monaco 支持的语言
const scriptLanguageForMonaco = computed(() => {
  const languageMap: Record<string, 'javascript' | 'python' | 'sql'> = {
    javascript: 'javascript',
    python: 'python',
    sql: 'sql',
  }
  return languageMap[localConfig.value.scriptLanguage || 'javascript']
})

// 表单验证规则
const formRules = computed<FormRules>(() => ({
  transformType: [
    { required: true, message: t('etl.dag.node.transformTypeRequired'), trigger: 'change' },
  ],
  inputVariable: [
    { required: true, message: t('etl.dag.node.inputVariableRequired'), trigger: 'blur' },
    {
      pattern: /^[a-zA-Z_][a-zA-Z0-9_]*$/,
      message: t('etl.dag.node.variableNameInvalid'),
      trigger: 'blur',
    },
  ],
  outputVariable: [
    { required: true, message: t('etl.dag.node.outputVariableRequired'), trigger: 'blur' },
    {
      pattern: /^[a-zA-Z_][a-zA-Z0-9_]*$/,
      message: t('etl.dag.node.variableNameInvalid'),
      trigger: 'blur',
    },
  ],
  filterExpression: [
    {
      required: localConfig.value.transformType === 'filter',
      message: t('etl.dag.node.filterExpressionRequired'),
      trigger: 'blur',
    },
  ],
  script: [
    {
      required: localConfig.value.transformType === 'script',
      message: t('etl.dag.node.scriptRequired'),
      trigger: 'blur',
    },
  ],
  scriptLanguage: [
    {
      required: localConfig.value.transformType === 'script',
      message: t('etl.dag.node.scriptLanguageRequired'),
      trigger: 'change',
    },
  ],
}))

// 监听 props 变化
watch(
  () => props.config,
  (newConfig) => {
    if (newConfig) {
      localConfig.value = {
        transformType: newConfig.transformType || 'mapping',
        inputVariable: newConfig.inputVariable || '',
        outputVariable: newConfig.outputVariable || 'result',
        fieldMapping: newConfig.fieldMapping || [],
        filterExpression: newConfig.filterExpression || '',
        aggregateConfig: newConfig.aggregateConfig || {
          groupBy: [],
          aggregations: [],
        },
        script: newConfig.script || '',
        scriptLanguage: newConfig.scriptLanguage || 'javascript',
      }
      // 同步聚合项
      aggregateItems.value = localConfig.value.aggregateConfig?.aggregations || []
    }
  },
  { immediate: true, deep: true }
)

// 处理转换类型变更
const handleTransformTypeChange = () => {
  // 切换类型时初始化对应配置
  switch (localConfig.value.transformType) {
    case 'mapping':
      localConfig.value.fieldMapping = localConfig.value.fieldMapping || []
      break
    case 'filter':
      localConfig.value.filterExpression = localConfig.value.filterExpression || ''
      break
    case 'aggregate':
      localConfig.value.aggregateConfig = localConfig.value.aggregateConfig || {
        groupBy: [],
        aggregations: [],
      }
      aggregateItems.value = localConfig.value.aggregateConfig.aggregations
      break
    case 'script':
      localConfig.value.script = localConfig.value.script || ''
      localConfig.value.scriptLanguage = localConfig.value.scriptLanguage || 'javascript'
      break
  }
  emitUpdate()
}

// 处理脚本语言变更
const handleScriptLanguageChange = () => {
  emitUpdate()
}

// 处理聚合项变更
const handleAggregateChange = (value: Record<string, string> | any[]) => {
  const items = value as any[]
  aggregateItems.value = items
  if (localConfig.value.aggregateConfig) {
    localConfig.value.aggregateConfig.aggregations = items.map((item) => ({
      function: item.function as AggregateItem['function'],
      field: item.field,
      alias: item.alias,
    }))
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
  margin-top: 8px;
  font-size: 11px;
  color: #909399;
  line-height: 1.4;
}

.aggregate-editor {
  width: 100%;
}

:deep(.el-divider__text) {
  font-size: 12px;
  color: #606266;
}

:deep(.el-form-item) {
  margin-bottom: 16px;
}

:deep(.el-form-item__label) {
  font-size: 13px;
}
</style>