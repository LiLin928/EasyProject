<!-- src/components/etl/DagDesigner/configs/ScriptConfig.vue -->
<template>
  <el-card shadow="never" class="config-card">
    <template #header>
      <span>{{ t('etl.dag.node.scriptConfig') }}</span>
    </template>
    <el-form
      ref="formRef"
      :model="localConfig"
      :rules="formRules"
      label-width="80px"
      size="small"
      :disabled="readonly"
    >
      <!-- 脚本类型选择 -->
      <el-form-item :label="t('etl.dag.node.scriptType')" prop="scriptType">
        <el-select
          v-model="localConfig.scriptType"
          :placeholder="t('etl.dag.node.selectScriptType')"
          style="width: 100%"
          @change="handleScriptTypeChange"
        >
          <el-option
            v-for="type in scriptTypeOptions"
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

      <!-- 脚本编辑 -->
      <el-form-item :label="t('etl.dag.node.scriptContent')" prop="script">
        <MonacoEditor
          v-model="localConfig.script"
          :language="monacoLanguage"
          height="200px"
          :readonly="readonly"
          :show-format="localConfig.scriptType === 'javascript'"
          :show-fullscreen="true"
          @change="emitUpdate"
        />
        <div class="form-tip">{{ t('etl.dag.node.scriptContentTip') }}</div>
      </el-form-item>

      <!-- 脚本路径（可选） -->
      <el-form-item :label="t('etl.dag.node.scriptPath')">
        <el-input
          v-model="localConfig.scriptPath"
          :placeholder="t('etl.dag.node.scriptPathPlaceholder')"
          @change="emitUpdate"
        />
        <div class="form-tip">{{ t('etl.dag.node.scriptPathTip') }}</div>
      </el-form-item>

      <!-- 参数配置 -->
      <el-divider content-position="left">{{ t('etl.dag.node.parameters') }}</el-divider>
      <KeyValueEditor
        v-model="parametersValue"
        :readonly="readonly"
        :default-item="{ key: '', value: '' }"
        @update:model-value="handleParametersUpdate"
      />
      <div class="form-tip">{{ t('etl.dag.node.scriptParametersTip') }}</div>

      <!-- 环境变量配置 -->
      <el-divider content-position="left">{{ t('etl.dag.node.envVariables') }}</el-divider>
      <KeyValueEditor
        v-model="envVariablesValue"
        :readonly="readonly"
        :default-item="{ key: '', value: '' }"
        @update:model-value="handleEnvVariablesUpdate"
      />
      <div class="form-tip">{{ t('etl.dag.node.envVariablesTip') }}</div>

      <!-- 工作目录 -->
      <el-form-item :label="t('etl.dag.node.workingDirectory')">
        <el-input
          v-model="workingDirectoryValue"
          :placeholder="t('etl.dag.node.workingDirectoryPlaceholder')"
          @change="emitUpdate"
        />
        <div class="form-tip">{{ t('etl.dag.node.workingDirectoryTip') }}</div>
      </el-form-item>

      <!-- 输出变量 -->
      <el-form-item :label="t('etl.dag.node.outputVariable')" prop="outputVariable">
        <VariableInput
          v-model="outputVariableValue"
          :disabled="readonly"
          :placeholder="t('etl.dag.node.outputVariablePlaceholder')"
          :show-prefix="true"
          :show-suffix="true"
          @update:model-value="handleOutputVariableUpdate"
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
import type { ScriptNodeConfig } from '@/types/etl/taskNode'
import { KeyValueEditor, MonacoEditor, VariableInput } from './shared'

const { t } = useLocale()

// Props
const props = defineProps<{
  /** 配置数据 */
  config: ScriptNodeConfig
  /** 只读模式 */
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', config: ScriptNodeConfig): void
}>()

// 表单引用
const formRef = ref<FormInstance>()

// 脚本类型选项
const scriptTypeOptions = [
  { label: 'Shell', value: 'shell', desc: 'Shell/Bash 脚本' },
  { label: 'Python', value: 'python', desc: 'Python 脚本' },
  { label: 'JavaScript', value: 'javascript', desc: 'JavaScript/Node.js 脚本' },
]

// Monaco 语言映射
const monacoLanguage = computed(() => {
  const languageMap: Record<string, 'shell' | 'python' | 'javascript'> = {
    shell: 'shell',
    python: 'python',
    javascript: 'javascript',
  }
  return languageMap[localConfig.value.scriptType] || 'shell'
})

// 本地配置
const localConfig = ref<ScriptNodeConfig>({
  scriptType: 'shell',
  script: '',
  scriptPath: '',
  parameters: {},
  envVariables: {},
  workingDirectory: '',
  outputVariable: '',
})

// 计算属性 - 用于处理可选字段
const parametersValue = computed<Record<string, string>>({
  get: () => localConfig.value.parameters || {},
  set: (value: Record<string, string>) => {
    localConfig.value.parameters = value
  },
})

const envVariablesValue = computed<Record<string, string>>({
  get: () => localConfig.value.envVariables || {},
  set: (value: Record<string, string>) => {
    localConfig.value.envVariables = value
  },
})

const workingDirectoryValue = computed<string>({
  get: () => localConfig.value.workingDirectory || '',
  set: (value: string) => {
    localConfig.value.workingDirectory = value
  },
})

const outputVariableValue = computed<string>({
  get: () => localConfig.value.outputVariable || '',
  set: (value: string) => {
    localConfig.value.outputVariable = value
  },
})

// 处理参数更新
const handleParametersUpdate = (value: Record<string, string> | any[]) => {
  if (!Array.isArray(value)) {
    localConfig.value.parameters = value
  }
  emitUpdate()
}

// 处理环境变量更新
const handleEnvVariablesUpdate = (value: Record<string, string> | any[]) => {
  if (!Array.isArray(value)) {
    localConfig.value.envVariables = value
  }
  emitUpdate()
}

// 处理输出变量更新
const handleOutputVariableUpdate = (value: string) => {
  localConfig.value.outputVariable = value
  emitUpdate()
}

// 表单验证规则
const formRules = computed<FormRules>(() => ({
  scriptType: [
    { required: true, message: t('etl.dag.node.scriptTypeRequired'), trigger: 'change' },
  ],
  script: [
    { required: true, message: t('etl.dag.node.scriptRequired'), trigger: 'blur' },
  ],
  outputVariable: [
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
        scriptType: newConfig.scriptType || 'shell',
        script: newConfig.script || '',
        scriptPath: newConfig.scriptPath || '',
        parameters: newConfig.parameters || {},
        envVariables: newConfig.envVariables || {},
        workingDirectory: newConfig.workingDirectory || '',
        outputVariable: newConfig.outputVariable || '',
      }
    }
  },
  { immediate: true, deep: true }
)

// 处理脚本类型变更
const handleScriptTypeChange = () => {
  // 切换脚本类型时，可以清空脚本内容（可选）
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