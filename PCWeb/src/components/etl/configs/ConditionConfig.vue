<!-- src/components/etl/DagDesigner/configs/ConditionConfig.vue -->
<template>
  <el-card shadow="never" class="config-card">
    <template #header>
      <span>{{ t('etl.dag.node.conditionConfig') }}</span>
    </template>
    <el-form
      ref="formRef"
      :model="localConfig"
      :rules="formRules"
      label-width="80px"
      size="small"
      :disabled="readonly"
    >
      <el-form-item :label="t('etl.dag.node.conditionType')" prop="conditionType">
        <el-radio-group
          v-model="localConfig.conditionType"
          @change="handleConditionTypeChange"
        >
          <el-radio-button label="simple">{{ t('etl.dag.node.simpleCondition') }}</el-radio-button>
          <el-radio-button label="expression">{{ t('etl.dag.node.expressionCondition') }}</el-radio-button>
        </el-radio-group>
      </el-form-item>

      <el-form-item :label="t('etl.dag.node.inputVariable')" prop="inputVariable">
        <VariableInput
          v-model="localConfig.inputVariable!"
          :placeholder="t('etl.dag.node.inputVariablePlaceholder')"
          :disabled="readonly"
          @update:model-value="emitUpdate"
        />
        <div class="form-tip">
          {{ t('etl.dag.node.inputVariableTip') }}
        </div>
      </el-form-item>

      <!-- 简单条件模式 -->
      <template v-if="localConfig.conditionType === 'simple'">
        <el-divider content-position="left">{{ t('etl.dag.node.conditionRules') }}</el-divider>
        <el-form-item>
          <ConditionRuleEditor
            v-model="internalConditions!"
            :readonly="readonly"
            @update:model-value="handleConditionsChange"
          />
        </el-form-item>
      </template>

      <!-- 表达式模式 -->
      <template v-if="localConfig.conditionType === 'expression'">
        <el-divider content-position="left">{{ t('etl.dag.node.expressionConfig') }}</el-divider>
        <el-form-item prop="expression">
          <MonacoEditor
            v-model="localConfig.expression!"
            language="javascript"
            height="200px"
            :readonly="readonly"
            :show-format="false"
            :show-fullscreen="true"
            @update:model-value="emitUpdate"
          />
          <div class="form-tip">
            {{ t('etl.dag.node.expressionTip') }}
          </div>
        </el-form-item>
      </template>

      <!-- 分支配置 -->
      <el-divider content-position="left">{{ t('etl.dag.node.branchConfig') }}</el-divider>
      <el-form-item>
        <BranchEditor
          v-model="localConfig.branches"
          :allow-default="true"
          :readonly="readonly"
          @update:model-value="emitUpdate"
        />
        <div class="form-tip">
          {{ t('etl.dag.node.branchConfigTip') }}
        </div>
      </el-form-item>
    </el-form>
  </el-card>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import type { ConditionNodeConfig, ConditionRule } from '@/types/etl/taskNode'
import { generateGuid } from '@/utils/guid'
import {
  VariableInput,
  MonacoEditor,
  ConditionRuleEditor,
  BranchEditor,
} from './shared'

const { t } = useLocale()

// Props
const props = defineProps<{
  /** 配置数据 */
  config: ConditionNodeConfig
  /** 只读模式 */
  readonly?: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'update', config: ConditionNodeConfig): void
}>()

// 表单引用
const formRef = ref<FormInstance>()

// 内部条件数据（用于 ConditionRuleEditor）
const internalConditions = ref<{ rules: ConditionRule[]; logic: 'and' | 'or' }>({
  rules: [],
  logic: 'and',
})

// 本地配置
const localConfig = ref<ConditionNodeConfig>({
  conditionType: 'simple',
  inputVariable: '',
  conditions: {
    rules: [],
    logic: 'and',
  },
  expression: '',
  branches: [
    { id: generateGuid(), name: '满足条件', isDefault: false },
    { id: generateGuid(), name: '不满足条件', isDefault: true },
  ],
})

// 表单验证规则
const formRules = computed<FormRules>(() => ({
  conditionType: [
    { required: true, message: t('etl.dag.node.conditionTypeRequired'), trigger: 'change' },
  ],
  expression: [
    {
      required: localConfig.value.conditionType === 'expression',
      message: t('etl.dag.node.expressionRequired'),
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
        conditionType: newConfig.conditionType || 'simple',
        inputVariable: newConfig.inputVariable || '',
        conditions: newConfig.conditions || {
          rules: [],
          logic: 'and',
        },
        expression: newConfig.expression || '',
        branches: newConfig.branches || [
          { id: generateGuid(), name: '满足条件', isDefault: false },
          { id: generateGuid(), name: '不满足条件', isDefault: true },
        ],
      }
      // 同步内部条件数据
      internalConditions.value = localConfig.value.conditions || {
        rules: [],
        logic: 'and',
      }
    }
  },
  { immediate: true, deep: true }
)

// 处理条件类型变更
const handleConditionTypeChange = () => {
  // 切换类型时初始化对应配置
  if (localConfig.value.conditionType === 'simple') {
    localConfig.value.conditions = localConfig.value.conditions || {
      rules: [],
      logic: 'and',
    }
    localConfig.value.expression = ''
  } else {
    localConfig.value.expression = localConfig.value.expression || ''
    localConfig.value.conditions = undefined
  }
  emitUpdate()
}

// 处理条件规则变更
const handleConditionsChange = (value: { rules: ConditionRule[]; logic: 'and' | 'or' }) => {
  internalConditions.value = value
  localConfig.value.conditions = value
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
  line-height: 1.5;
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

:deep(.el-radio-group) {
  display: flex;
  width: 100%;
}

:deep(.el-radio-button) {
  flex: 1;

  .el-radio-button__inner {
    width: 100%;
  }
}
</style>