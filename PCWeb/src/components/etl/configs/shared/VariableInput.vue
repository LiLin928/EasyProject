<template>
  <div class="variable-input">
    <el-input
      :model-value="modelValue"
      :placeholder="placeholder"
      :disabled="disabled"
      @update:model-value="handleUpdate"
    >
      <template v-if="showPrefix" #prefix>
        <span class="variable-prefix">${</span>
      </template>
      <template v-if="showSuffix" #suffix>
        <span class="variable-suffix">}</span>
      </template>
    </el-input>
    <el-tooltip
      content="使用 ${变量名} 语法引用其他节点的输出"
      placement="top"
    >
      <el-icon class="help-icon">
        <QuestionFilled />
      </el-icon>
    </el-tooltip>
  </div>
</template>

<script setup lang="ts">
import { QuestionFilled } from '@element-plus/icons-vue'

/**
 * 变量名输入组件
 * 用于在 ETL 节点配置中输入变量名，支持显示 ${变量名} 格式
 */

interface Props {
  /** 变量名（v-model 绑定值） */
  modelValue: string
  /** 占位提示 */
  placeholder?: string
  /** 禁用状态 */
  disabled?: boolean
  /** 显示前缀 ${ */
  showPrefix?: boolean
  /** 显示后缀 } */
  showSuffix?: boolean
}

withDefaults(defineProps<Props>(), {
  modelValue: '',
  placeholder: '请输入变量名',
  disabled: false,
  showPrefix: true,
  showSuffix: true,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

/**
 * 处理输入值更新
 */
const handleUpdate = (value: string) => {
  emit('update:modelValue', value)
}
</script>

<style scoped lang="less">
.variable-input {
  display: flex;
  align-items: center;
  gap: 8px;

  :deep(.el-input) {
    flex: 1;
  }

  .variable-prefix,
  .variable-suffix {
    color: var(--el-color-primary);
    font-weight: 500;
  }

  .help-icon {
    color: var(--el-color-info);
    font-size: 16px;
    cursor: help;
    transition: color 0.2s;

    &:hover {
      color: var(--el-color-primary);
    }
  }
}
</style>