<template>
  <div class="condition-rule-editor">
    <!-- Logic Selector -->
    <div class="logic-selector">
      <el-radio-group
        v-model="internalLogic"
        :disabled="readonly"
        size="small"
        @change="handleLogicChange"
      >
        <el-radio-button label="and">{{ t('etl.dag.node.allSatisfied') }}</el-radio-button>
        <el-radio-button label="or">{{ t('etl.dag.node.anySatisfied') }}</el-radio-button>
      </el-radio-group>
    </div>

    <!-- Table Header -->
    <div v-if="!readonly && internalRules.length > 0" class="editor-header">
      <div class="header-cell field-cell">{{ t('etl.dag.node.field') }}</div>
      <div class="header-cell operator-cell">{{ t('etl.dag.node.operator') }}</div>
      <div class="header-cell value-cell">{{ t('etl.dag.node.value') }}</div>
      <div class="header-cell action-cell">{{ t('etl.dag.node.action') }}</div>
    </div>

    <!-- Table Body -->
    <div class="editor-body">
      <div
        v-for="(rule, index) in internalRules"
        :key="index"
        class="editor-row"
      >
        <!-- Field Input -->
        <div class="row-cell field-cell">
          <el-input
            :model-value="rule.field"
            :placeholder="t('etl.dag.node.fieldPlaceholder')"
            :disabled="readonly"
            size="small"
            @update:model-value="(val: string) => handleUpdate(index, 'field', val)"
          />
        </div>

        <!-- Operator Select -->
        <div class="row-cell operator-cell">
          <el-select
            :model-value="rule.operator"
            :placeholder="t('etl.dag.node.operatorPlaceholder')"
            :disabled="readonly"
            size="small"
            style="width: 100%"
            @update:model-value="(val: string) => handleUpdate(index, 'operator', val)"
          >
            <el-option
              v-for="opt in operatorOptions"
              :key="opt.value"
              :label="opt.label"
              :value="opt.value"
            />
          </el-select>
        </div>

        <!-- Value Input -->
        <div class="row-cell value-cell">
          <el-input
            :model-value="rule.value"
            :placeholder="t('etl.dag.node.valuePlaceholder')"
            :disabled="readonly"
            size="small"
            @update:model-value="(val: string) => handleUpdate(index, 'value', val)"
          />
        </div>

        <!-- Actions -->
        <div class="row-cell action-cell">
          <el-button
            v-if="!readonly"
            type="danger"
            link
            size="small"
            @click="handleDelete(index)"
          >
            {{ t('etl.dag.node.delete') }}
          </el-button>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="internalRules.length === 0" class="empty-state">
        <span class="empty-text">{{ t('etl.dag.node.noConditionRules') }}</span>
      </div>
    </div>

    <!-- Add Button -->
    <div v-if="!readonly" class="editor-footer">
      <el-button type="primary" link @click="handleAdd">
        <el-icon><Plus /></el-icon>
        {{ t('etl.dag.node.addConditionRule') }}
      </el-button>
    </div>

    <!-- Readonly Display -->
    <div v-if="readonly && internalRules.length > 0" class="readonly-display">
      <div class="logic-display">
        <el-tag size="small" type="info">
          {{ internalLogic === 'and' ? t('etl.dag.node.allSatisfied') : t('etl.dag.node.anySatisfied') }}
        </el-tag>
      </div>
      <div
        v-for="(rule, index) in internalRules"
        :key="index"
        class="readonly-item"
      >
        <span class="field">{{ rule.field || '-' }}</span>
        <span class="operator">{{ getOperatorLabel(rule.operator) }}</span>
        <span class="value">{{ rule.value || '-' }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Plus } from '@element-plus/icons-vue'
import type { ConditionRule } from '@/types/etl/taskNode'
import { useLocale } from '@/composables/useLocale'

const { t } = useLocale()

/**
 * ConditionRuleEditor - 条件规则编辑器组件
 *
 * 功能特性：
 * - 支持多条条件规则编辑
 * - 支持逻辑关系选择（AND/OR）
 * - 支持 8 种操作符
 * - 支持只读模式
 */

/**
 * 操作符选项
 */
const operatorOptions = [
  { value: 'eq', label: t('etl.dag.operator.eq') },
  { value: 'ne', label: t('etl.dag.operator.ne') },
  { value: 'gt', label: t('etl.dag.operator.gt') },
  { value: 'lt', label: t('etl.dag.operator.lt') },
  { value: 'gte', label: t('etl.dag.operator.gte') },
  { value: 'lte', label: t('etl.dag.operator.lte') },
  { value: 'contains', label: t('etl.dag.operator.contains') },
  { value: 'regex', label: t('etl.dag.operator.regex') },
]

/**
 * Props 定义
 */
interface Props {
  /** 条件规则数据 */
  modelValue: {
    rules: ConditionRule[]
    logic: 'and' | 'or'
  }
  /** 只读模式 */
  readonly?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  readonly: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: { rules: ConditionRule[]; logic: 'and' | 'or' }): void
}>()

/**
 * 内部规则数组
 */
const internalRules = ref<ConditionRule[]>([])

/**
 * 内部逻辑关系
 */
const internalLogic = ref<'and' | 'or'>('and')

/**
 * 初始化内部数据
 */
const initializeInternalData = () => {
  if (props.modelValue) {
    internalRules.value = props.modelValue.rules ? [...props.modelValue.rules] : []
    internalLogic.value = props.modelValue.logic || 'and'
  } else {
    internalRules.value = []
    internalLogic.value = 'and'
  }
}

/**
 * 监听 modelValue 变化
 */
watch(
  () => props.modelValue,
  () => {
    initializeInternalData()
  },
  { immediate: true, deep: true }
)

/**
 * 获取默认规则
 */
const getDefaultRule = (): ConditionRule => ({
  field: '',
  operator: 'eq',
  value: '',
})

/**
 * 添加新规则
 */
const handleAdd = () => {
  internalRules.value.push(getDefaultRule())
  emitChange()
}

/**
 * 删除规则
 */
const handleDelete = (index: number) => {
  internalRules.value.splice(index, 1)
  emitChange()
}

/**
 * 更新规则字段
 */
const handleUpdate = (index: number, prop: keyof ConditionRule, value: string) => {
  internalRules.value[index][prop] = value as any
  emitChange()
}

/**
 * 逻辑关系变更
 */
const handleLogicChange = () => {
  emitChange()
}

/**
 * 获取操作符显示标签
 */
const getOperatorLabel = (operator: string): string => {
  const option = operatorOptions.find((opt) => opt.value === operator)
  return option ? option.label : operator
}

/**
 * 发送数据变更事件
 */
const emitChange = () => {
  emit('update:modelValue', {
    rules: [...internalRules.value],
    logic: internalLogic.value,
  })
}
</script>

<style scoped lang="scss">
.condition-rule-editor {
  width: 100%;
}

.logic-selector {
  margin-bottom: 12px;

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
}

.editor-header {
  display: flex;
  background-color: var(--el-fill-color-light);
  border: 1px solid var(--el-border-color-lighter);
  border-bottom: none;
  border-radius: 4px 4px 0 0;

  .header-cell {
    padding: 8px 12px;
    font-size: 13px;
    font-weight: 500;
    color: var(--el-text-color-regular);
    flex-shrink: 0;

    &.field-cell {
      flex: 1;
    }

    &.operator-cell {
      width: 150px;
    }

    &.value-cell {
      flex: 1;
    }

    &.action-cell {
      width: 60px;
      text-align: center;
    }
  }
}

.editor-body {
  max-height: 300px;
  overflow-y: auto;
  border: 1px solid var(--el-border-color-lighter);
  border-radius: 0 0 4px 4px;

  .editor-row {
    display: flex;
    border-bottom: 1px solid var(--el-border-color-lighter);

    &:last-child {
      border-bottom: none;
    }

    .row-cell {
      padding: 4px;
      flex-shrink: 0;

      &.field-cell {
        flex: 1;
      }

      &.operator-cell {
        width: 150px;
      }

      &.value-cell {
        flex: 1;
      }

      &.action-cell {
        width: 60px;
        display: flex;
        align-items: center;
        justify-content: center;
      }

      :deep(.el-input),
      :deep(.el-select) {
        width: 100%;
      }
    }
  }

  .empty-state {
    padding: 32px;
    text-align: center;

    .empty-text {
      color: var(--el-text-color-placeholder);
      font-size: 13px;
    }
  }
}

.editor-footer {
  padding: 8px 12px;
  margin-top: 8px;
}

.readonly-display {
  .logic-display {
    margin-bottom: 8px;
  }

  .readonly-item {
    padding: 6px 0;
    font-size: 13px;
    color: var(--el-text-color-regular);
    display: flex;
    align-items: center;
    gap: 8px;

    .field {
      font-weight: 500;
      min-width: 80px;
    }

    .operator {
      color: var(--el-color-primary);
      font-weight: 500;
      min-width: 100px;
    }

    .value {
      flex: 1;
    }
  }
}
</style>