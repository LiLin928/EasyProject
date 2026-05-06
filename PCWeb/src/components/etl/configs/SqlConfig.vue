<!-- src/components/etl/DagDesigner/configs/SqlConfig.vue -->
<template>
  <el-card shadow="never" class="config-card">
    <template #header>
      <span>{{ t('etl.dag.node.sqlConfig') }}</span>
    </template>
    <el-form
      ref="formRef"
      :model="localConfig"
      :rules="formRules"
      label-width="80px"
      size="small"
      :disabled="readonly"
    >
      <!-- 数据源选择 -->
      <el-form-item :label="t('etl.dag.node.datasource')" prop="datasourceId">
        <DatasourceSelector
          v-model="localConfig.datasourceId"
          :datasources="etlStore.datasources"
          :disabled="readonly"
          :placeholder="t('etl.dag.node.selectDatasource')"
          @update:model-value="emitUpdate"
        />
      </el-form-item>

      <!-- SQL 类型选择 -->
      <el-form-item :label="t('etl.dag.node.sqlType')" prop="sqlType">
        <el-select
          v-model="localConfig.sqlType"
          :placeholder="t('etl.dag.node.selectSqlType')"
          style="width: 100%"
          @change="handleSqlTypeChange"
        >
          <el-option
            v-for="type in sqlTypeOptions"
            :key="type.value"
            :label="type.label"
            :value="type.value"
          >
            <span style="float: left">{{ type.label }}</span>
            <span style="float: right; color: #8492a6; font-size: 12px">
              {{ type.desc }}
            </span>
          </el-option>
        </el-select>
      </el-form-item>

      <!-- SQL 语句编辑 -->
      <el-form-item :label="t('etl.dag.node.sqlStatement')" prop="sql">
        <MonacoEditor
          v-model="localConfig.sql"
          language="sql"
          height="200px"
          :readonly="readonly"
          :show-format="true"
          :show-fullscreen="true"
          @change="emitUpdate"
        />
        <div class="form-tip">{{ t('etl.dag.node.sqlStatementTip') }}</div>
      </el-form-item>

      <!-- SQL 参数配置 -->
      <el-form-item :label="t('etl.dag.node.parameters')">
        <KeyValueEditor
          v-model="parameterArray"
          :columns="parameterColumns"
          :readonly="readonly"
          :default-item="defaultParameter"
          @update:model-value="handleParametersChange"
        />
        <div class="form-tip">{{ t('etl.dag.node.parametersTip') }}</div>
      </el-form-item>

      // 查询类型时显示输出变量 -->
      <el-form-item
        v-if="localConfig.sqlType === 'query'"
        :label="t('etl.dag.node.outputVariable')"
        prop="outputVariable"
      >
        <VariableInput
          v-model="outputVariableValue"
          :disabled="readonly"
          :placeholder="t('etl.dag.node.outputVariablePlaceholder')"
          :show-prefix="true"
          :show-suffix="true"
          @update:model-value="handleOutputVariableChange"
        />
        <div class="form-tip">{{ t('etl.dag.node.outputVariableTip') }}</div>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { useEtlStore } from '@/stores/etlStore'
import type { SqlNodeConfig, SqlParameter } from '@/types/etl/taskNode'
import DatasourceSelector from './shared/DatasourceSelector.vue'
import MonacoEditor from './shared/MonacoEditor.vue'
import VariableInput from './shared/VariableInput.vue'
import KeyValueEditor from './shared/EtlDagKeyValueEditor.vue'
import type { Column } from './shared/EtlDagKeyValueEditor.vue'

const { t } = useLocale()

// Props
const props = defineProps<{
  /** 配置数据 */
  config: SqlNodeConfig
  /** 只读模式 */
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', config: SqlNodeConfig): void
}>()

// ETL Store
const etlStore = useEtlStore()

// 表单引用
const formRef = ref<FormInstance>()

// SQL 类型选项
const sqlTypeOptions = [
  { label: '查询 (SELECT)', value: 'query', desc: '返回数据结果' },
  { label: '插入 (INSERT)', value: 'insert', desc: '插入新数据' },
  { label: '更新 (UPDATE)', value: 'update', desc: '更新已有数据' },
  { label: '删除 (DELETE)', value: 'delete', desc: '删除数据' },
  { label: 'DDL', value: 'ddl', desc: '结构操作' },
]

// 参数列配置
const parameterColumns: Column[] = [
  { prop: 'name', label: '参数名', type: 'input', width: 100, placeholder: '名称' },
  { prop: 'type', label: '类型', type: 'select', width: 100, placeholder: '类型', options: ['string', 'number', 'date', 'variable'] },
  { prop: 'value', label: '值', type: 'input', width: 150, placeholder: '值' },
]

// 默认参数（作为 Record<string, string> 传递给 KeyValueEditor）
const defaultParameter: Record<string, string> = {
  name: '',
  type: 'string',
  value: '',
}

// 本地配置
const localConfig = ref<SqlNodeConfig>({
  datasourceId: '',
  sqlType: 'query',
  sql: '',
  parameters: [],
  outputVariable: '',
})

// 参数数组（用于 KeyValueEditor，转换为 any[] 格式）
const parameterArray = ref<Record<string, string>[]>([])

// 输出变量值（确保是字符串类型）
const outputVariableValue = computed({
  get: () => localConfig.value.outputVariable || '',
  set: (value: string) => {
    localConfig.value.outputVariable = value
  },
})

// 表单验证规则
const formRules = computed<FormRules>(() => ({
  datasourceId: [
    { required: true, message: t('etl.dag.node.datasourceRequired'), trigger: 'change' },
  ],
  sqlType: [
    { required: true, message: t('etl.dag.node.sqlTypeRequired'), trigger: 'change' },
  ],
  sql: [
    { required: true, message: t('etl.dag.node.sqlRequired'), trigger: 'blur' },
  ],
  outputVariable: [
    {
      required: localConfig.value.sqlType === 'query',
      message: t('etl.dag.node.outputVariableRequired'),
      trigger: 'blur',
    },
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
        sqlType: newConfig.sqlType || 'query',
        sql: newConfig.sql || '',
        parameters: newConfig.parameters || [],
        outputVariable: newConfig.outputVariable || '',
      }
      // 将 SqlParameter[] 转换为 Record<string, string>[] 用于 KeyValueEditor
      parameterArray.value = (newConfig.parameters || []).map(p => ({
        name: p.name,
        type: p.type,
        value: p.value,
      }))
    }
  },
  { immediate: true, deep: true }
)

// 处理参数变更（从 KeyValueEditor 获取并转换回 SqlParameter[]）
const handleParametersChange = (value: Record<string, string>[] | Record<string, string>) => {
  if (Array.isArray(value)) {
    localConfig.value.parameters = value.map(item => ({
      name: item.name || '',
      type: (item.type as SqlParameter['type']) || 'string',
      value: item.value || '',
    })) as SqlParameter[]
  }
  emitUpdate()
}

// 处理输出变量变更
const handleOutputVariableChange = (value: string) => {
  localConfig.value.outputVariable = value
  emitUpdate()
}

// 处理 SQL 类型变更
const handleSqlTypeChange = () => {
  // 非查询类型时清空输出变量
  if (localConfig.value.sqlType !== 'query') {
    localConfig.value.outputVariable = ''
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
</style>