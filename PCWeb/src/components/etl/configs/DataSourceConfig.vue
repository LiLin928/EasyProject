<!-- src/components/etl/DagDesigner/configs/DataSourceConfig.vue -->
<template>
  <el-card shadow="never" class="config-card">
    <template #header>
      <span>{{ t('etl.dag.node.datasourceConfig') }}</span>
    </template>
    <el-form
      ref="formRef"
      :model="localConfig"
      :rules="formRules"
      label-width="80px"
      size="small"
      :disabled="readonly"
    >
      <el-form-item :label="t('etl.dag.node.datasource')" prop="datasourceId">
        <DatasourceSelector
          v-model="localConfig.datasourceId"
          :datasources="datasources"
          :disabled="readonly"
          :placeholder="t('etl.dag.node.selectDatasource')"
          @update:model-value="emitUpdate"
        />
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.queryType')" prop="queryType">
        <el-radio-group v-model="localConfig.queryType" @change="handleQueryTypeChange">
          <el-radio value="table">{{ t('etl.dag.node.tableQuery') }}</el-radio>
          <el-radio value="sql">{{ t('etl.dag.node.sqlQuery') }}</el-radio>
        </el-radio-group>
      </el-form-item>

      <!-- 表查询模式 -->
      <template v-if="localConfig.queryType === 'table'">
        <el-form-item :label="t('etl.dag.node.tableName')" prop="tableName">
          <el-input
            v-model="localConfig.tableName"
            :placeholder="t('etl.dag.node.tableNamePlaceholder')"
            @change="emitUpdate"
          />
        </el-form-item>

        <el-form-item :label="t('etl.dag.node.columns')">
          <el-select
            v-model="localConfig.columns"
            multiple
            filterable
            allow-create
            :placeholder="t('etl.dag.node.columnsPlaceholder')"
            style="width: 100%"
            @change="emitUpdate"
          >
            <el-option
              v-for="col in columnOptions"
              :key="col"
              :label="col"
              :value="col"
            />
          </el-select>
          <div class="form-tip">{{ t('etl.dag.node.columnsTip') }}</div>
        </el-form-item>

        <el-form-item :label="t('etl.dag.node.whereClause')">
          <el-input
            v-model="localConfig.whereClause"
            type="textarea"
            :rows="2"
            :placeholder="t('etl.dag.node.whereClausePlaceholder')"
            @change="emitUpdate"
          />
        </el-form-item>
      </template>

      <!-- SQL 查询模式 -->
      <template v-else>
        <el-form-item :label="t('etl.dag.node.sql')" prop="sql">
          <MonacoEditor
            v-model="sqlContent"
            language="sql"
            height="300px"
            :readonly="readonly"
            :show-format="true"
            :show-fullscreen="true"
            @change="emitUpdate"
          />
          <div class="form-tip">{{ t('etl.dag.node.sqlTip') }}</div>
        </el-form-item>
      </template>

      <el-form-item :label="t('etl.dag.node.limit')">
        <el-input-number
          v-model="localConfig.limit"
          :min="0"
          :max="100000"
          :placeholder="t('etl.dag.node.limitPlaceholder')"
          style="width: 100%"
          @change="emitUpdate"
        />
        <div class="form-tip">{{ t('etl.dag.node.limitTip') }}</div>
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.outputVariable')" prop="outputVariable">
        <VariableInput
          v-model="localConfig.outputVariable"
          :disabled="readonly"
          :placeholder="t('etl.dag.node.outputVariablePlaceholder')"
          @update:model-value="emitUpdate"
        />
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { ref, watch, computed, onMounted } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { DatasourceSelector, MonacoEditor, VariableInput } from './shared'
import { useEtlStore } from '@/stores/etlStore'
import { storeToRefs } from 'pinia'
import type { DataSourceNodeConfig } from '@/types/etl'

const { t } = useLocale()

// Props
const props = defineProps<{
  /** 配置数据 */
  config: DataSourceNodeConfig
  /** 只读模式 */
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', config: DataSourceNodeConfig): void
}>()

// 表单引用
const formRef = ref<FormInstance>()

// 从 etlStore 获取数据源列表
const etlStore = useEtlStore()
const { datasources } = storeToRefs(etlStore)

// 组件挂载时加载数据源列表
onMounted(async () => {
  if (datasources.value.length === 0) {
    await etlStore.loadDatasources()
  }
})

// 模拟列选项（实际应根据选中的表动态获取）
const columnOptions = ['id', 'name', 'created_at', 'updated_at', 'status', 'type']

// SQL content computed (for MonacoEditor)
const sqlContent = computed<string>({
  get: () => localConfig.value.sql || '',
  set: (value) => {
    localConfig.value.sql = value
  },
})

// 本地配置
const localConfig = ref<DataSourceNodeConfig>({
  datasourceId: '',
  queryType: 'table',
  tableName: '',
  sql: '',
  columns: [],
  whereClause: '',
  limit: 1000,
  outputVariable: 'data',
})

// 表单验证规则
const formRules = computed<FormRules>(() => ({
  datasourceId: [
    { required: true, message: t('etl.dag.node.datasourceRequired'), trigger: 'change' },
  ],
  queryType: [
    { required: true, message: t('etl.dag.node.queryTypeRequired'), trigger: 'change' },
  ],
  tableName: [
    {
      required: localConfig.value.queryType === 'table',
      message: t('etl.dag.node.tableNameRequired'),
      trigger: 'blur',
    },
  ],
  sql: [
    {
      required: localConfig.value.queryType === 'sql',
      message: t('etl.dag.node.sqlRequired'),
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
}))

// 监听 props 变化
watch(
  () => props.config,
  (newConfig) => {
    if (newConfig) {
      localConfig.value = {
        datasourceId: newConfig.datasourceId || '',
        queryType: newConfig.queryType || 'table',
        tableName: newConfig.tableName || '',
        sql: newConfig.sql || '',
        columns: newConfig.columns || [],
        whereClause: newConfig.whereClause || '',
        limit: newConfig.limit ?? 1000,
        outputVariable: newConfig.outputVariable || 'data',
      }
    }
  },
  { immediate: true, deep: true }
)

// 处理查询类型变更
const handleQueryTypeChange = () => {
  // 切换类型时清空相关字段
  if (localConfig.value.queryType === 'table') {
    localConfig.value.sql = ''
  } else {
    localConfig.value.tableName = ''
    localConfig.value.columns = []
    localConfig.value.whereClause = ''
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

:deep(.el-textarea__inner) {
  font-family: 'Monaco', 'Menlo', 'Consolas', monospace;
  font-size: 12px;
}
</style>