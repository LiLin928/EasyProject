<!-- src/components/etl/DagDesigner/configs/FileConfig.vue -->
<template>
  <el-card shadow="never" class="config-card">
    <template #header>
      <span>{{ t('etl.dag.node.fileConfig') }}</span>
    </template>
    <el-form
      ref="formRef"
      :model="localConfig"
      :rules="formRules"
      label-width="80px"
      size="small"
      :disabled="readonly"
    >
      <el-form-item :label="t('etl.dag.node.fileOperation')" prop="fileOperation">
        <el-select
          v-model="localConfig.fileOperation"
          :placeholder="t('etl.dag.node.selectFileOperation')"
          style="width: 100%"
          @change="handleOperationChange"
        >
          <el-option
            v-for="op in fileOperationOptions"
            :key="op.value"
            :label="op.label"
            :value="op.value"
          >
            <div style="display: flex; justify-content: space-between; align-items: center">
              <span>{{ op.label }}</span>
              <el-tag size="small" type="info">{{ op.desc }}</el-tag>
            </div>
          </el-option>
        </el-select>
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.fileType')" prop="fileType">
        <el-select
          v-model="localConfig.fileType"
          :placeholder="t('etl.dag.node.selectFileType')"
          style="width: 100%"
          @change="emitUpdate"
        >
          <el-option label="CSV" value="csv" />
          <el-option label="Excel" value="excel" />
          <el-option label="JSON" value="json" />
          <el-option label="XML" value="xml" />
          <el-option label="TXT" value="txt" />
        </el-select>
      </el-form-item>

      <!-- 数据源选择器（用于 FTP/SFTP 等远程文件） -->
      <el-form-item :label="t('etl.dag.node.datasource')">
        <DatasourceSelector
          v-model="datasourceIdValue"
          :datasources="fileDatasources"
          :placeholder="t('etl.dag.node.datasourceOptional')"
        />
        <div class="form-tip">{{ t('etl.dag.node.datasourceTip') }}</div>
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.filePath')" prop="filePath">
        <el-input
          v-model="localConfig.filePath"
          :placeholder="t('etl.dag.node.filePathPlaceholder')"
          @change="emitUpdate"
        />
      </el-form-item>

      <!-- 移动/复制操作需要目标路径 -->
      <el-form-item
        v-if="localConfig.fileOperation === 'move' || localConfig.fileOperation === 'copy'"
        :label="t('etl.dag.node.targetPath')"
        prop="localPath"
      >
        <el-input
          v-model="localConfig.localPath"
          :placeholder="t('etl.dag.node.targetPathPlaceholder')"
          @change="emitUpdate"
        />
      </el-form-item>

      <!-- Excel 特有配置 -->
      <el-form-item v-if="localConfig.fileType === 'excel'" :label="t('etl.dag.node.sheetName')">
        <el-input
          v-model="localConfig.sheetName"
          :placeholder="t('etl.dag.node.sheetNamePlaceholder')"
          @change="emitUpdate"
        />
      </el-form-item>

      <!-- CSV 分隔符配置 -->
      <el-form-item v-if="localConfig.fileType === 'csv'" :label="t('etl.dag.node.delimiter')">
        <el-input
          v-model="localConfig.delimiter"
          :placeholder="t('etl.dag.node.delimiterPlaceholder')"
          style="width: 100px"
          @change="emitUpdate"
        />
        <div class="form-tip">{{ t('etl.dag.node.delimiterTip') }}</div>
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.encoding')">
        <el-select
          v-model="localConfig.encoding"
          :placeholder="t('etl.dag.node.selectEncoding')"
          style="width: 100%"
          @change="emitUpdate"
        >
          <el-option label="UTF-8" value="utf-8" />
          <el-option label="GBK" value="gbk" />
          <el-option label="GB2312" value="gb2312" />
          <el-option label="ISO-8859-1" value="iso-8859-1" />
        </el-select>
      </el-form-item>

      <!-- 读取操作需要输出变量 -->
      <el-form-item
        v-if="localConfig.fileOperation === 'read'"
        :label="t('etl.dag.node.outputVariable')"
        prop="outputVariable"
      >
        <VariableInput
          v-model="outputVariableValue"
          :placeholder="t('etl.dag.node.outputVariablePlaceholder')"
        />
      </el-form-item>

      <!-- 写入操作需要输入变量 -->
      <el-form-item
        v-if="localConfig.fileOperation === 'write'"
        :label="t('etl.dag.node.inputVariable')"
        prop="inputVariable"
      >
        <VariableInput
          v-model="inputVariableValue"
          :placeholder="t('etl.dag.node.inputVariablePlaceholder')"
        />
      </el-form-item>

      <!-- 写入操作包含表头选项 -->
      <el-form-item
        v-if="localConfig.fileOperation === 'write'"
        :label="t('etl.dag.node.includeHeader')"
      >
        <el-switch
          v-model="localConfig.includeHeader"
          @change="emitUpdate"
        />
        <div class="form-tip">{{ t('etl.dag.node.includeHeaderTip') }}</div>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { useEtlStore } from '@/stores/etlStore'
import { DatasourceSelector, VariableInput } from './shared'
import type { FileNodeConfig } from '@/types/etl'

const { t } = useLocale()
const etlStore = useEtlStore()

// Props
const props = defineProps<{
  /** 配置数据 */
  config: FileNodeConfig
  /** 只读模式 */
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', config: FileNodeConfig): void
}>()

// 表单引用
const formRef = ref<FormInstance>()

// 文件操作选项
const fileOperationOptions = [
  { label: '读取', value: 'read', desc: '读取文件内容' },
  { label: '写入', value: 'write', desc: '写入数据到文件' },
  { label: '删除', value: 'delete', desc: '删除文件' },
  { label: '移动', value: 'move', desc: '移动文件' },
  { label: '复制', value: 'copy', desc: '复制文件' },
]

// 文件相关数据源（FTP/SFTP）
const fileDatasources = computed(() => {
  // 过滤出文件相关数据源（FTP/SFTP）
  // 也允许使用其他数据源，但主要是 FTP/SFTP
  return etlStore.datasources.filter(ds =>
    ds.type === 'ftp' || ds.type === 'sftp'
  )
})

// 数据源 ID 值（处理可选类型）
const datasourceIdValue = computed({
  get: () => localConfig.value.datasourceId || '',
  set: (val: string) => {
    localConfig.value.datasourceId = val
    emitUpdate()
  },
})

// 输出变量值（处理可选类型）
const outputVariableValue = computed({
  get: () => localConfig.value.outputVariable || '',
  set: (val: string) => {
    localConfig.value.outputVariable = val
    emitUpdate()
  },
})

// 输入变量值（处理可选类型）
const inputVariableValue = computed({
  get: () => localConfig.value.inputVariable || '',
  set: (val: string) => {
    localConfig.value.inputVariable = val
    emitUpdate()
  },
})

// 本地配置
const localConfig = ref<FileNodeConfig>({
  fileOperation: 'read',
  fileType: 'csv',
  datasourceId: '',
  filePath: '',
  localPath: '',
  sheetName: '',
  delimiter: ',',
  encoding: 'utf-8',
  outputVariable: 'fileData',
  inputVariable: '',
  includeHeader: true,
})

// 表单验证规则
const formRules = computed<FormRules>(() => ({
  fileOperation: [
    { required: true, message: t('etl.dag.node.fileOperationRequired'), trigger: 'change' },
  ],
  fileType: [
    { required: true, message: t('etl.dag.node.fileTypeRequired'), trigger: 'change' },
  ],
  filePath: [
    { required: true, message: t('etl.dag.node.filePathRequired'), trigger: 'blur' },
  ],
  outputVariable: [
    {
      required: localConfig.value.fileOperation === 'read',
      message: t('etl.dag.node.outputVariableRequired'),
      trigger: 'blur',
    },
    {
      pattern: /^[a-zA-Z_][a-zA-Z0-9_]*$/,
      message: t('etl.dag.node.variableNameInvalid'),
      trigger: 'blur',
    },
  ],
  inputVariable: [
    {
      required: localConfig.value.fileOperation === 'write',
      message: t('etl.dag.node.inputVariableRequired'),
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
        fileOperation: newConfig.fileOperation || 'read',
        fileType: newConfig.fileType || 'csv',
        datasourceId: newConfig.datasourceId || '',
        filePath: newConfig.filePath || '',
        localPath: newConfig.localPath || '',
        sheetName: newConfig.sheetName || '',
        delimiter: newConfig.delimiter || ',',
        encoding: newConfig.encoding || 'utf-8',
        outputVariable: newConfig.outputVariable || 'fileData',
        inputVariable: newConfig.inputVariable || '',
        includeHeader: newConfig.includeHeader ?? true,
      }
    }
  },
  { immediate: true, deep: true }
)

// 处理操作类型变更
const handleOperationChange = () => {
  // 重置相关字段
  if (localConfig.value.fileOperation !== 'read') {
    localConfig.value.outputVariable = ''
  }
  if (localConfig.value.fileOperation !== 'write') {
    localConfig.value.inputVariable = ''
    localConfig.value.includeHeader = true
  }
  if (localConfig.value.fileOperation !== 'move' && localConfig.value.fileOperation !== 'copy') {
    localConfig.value.localPath = ''
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

:deep(.el-divider__text) {
  font-size: 12px;
  color: #606266;
}
</style>