<template>
  <div class="branch-editor">
    <!-- Table Header -->
    <div v-if="!readonly && internalBranches.length > 0" class="editor-header">
      <div class="header-cell name-cell">{{ t('etl.dag.node.branchName') }}</div>
      <div v-if="allowDefault" class="header-cell default-cell">{{ t('etl.dag.node.defaultBranch') }}</div>
      <div class="header-cell action-cell">{{ t('etl.dag.node.action') }}</div>
    </div>

    <!-- Table Body -->
    <div class="editor-body">
      <div
        v-for="(branch, index) in internalBranches"
        :key="branch.id"
        class="editor-row"
      >
        <!-- Branch Name -->
        <div class="row-cell name-cell">
          <el-input
            :model-value="branch.name"
            :placeholder="t('etl.dag.node.branchNamePlaceholder')"
            :disabled="readonly"
            size="small"
            @update:model-value="(val: string) => handleUpdateName(index, val)"
          />
        </div>

        <!-- Default Checkbox -->
        <div v-if="allowDefault" class="row-cell default-cell">
          <el-checkbox
            :model-value="(branch as ConditionBranch).isDefault || false"
            :disabled="readonly"
            @update:model-value="(val: CheckboxValueType) => handleUpdateDefault(index, val as boolean)"
          />
        </div>

        <!-- Actions -->
        <div class="row-cell action-cell">
          <el-button
            v-if="!readonly && internalBranches.length > 1"
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
      <div v-if="internalBranches.length === 0" class="empty-state">
        <span class="empty-text">{{ t('etl.dag.node.noBranch') }}</span>
      </div>
    </div>

    <!-- Add Button -->
    <div v-if="!readonly" class="editor-footer">
      <el-button type="primary" link @click="handleAdd">
        <el-icon><Plus /></el-icon>
        {{ t('etl.dag.node.addBranch') }}
      </el-button>
    </div>

    <!-- Readonly Display -->
    <div v-if="readonly && internalBranches.length > 0" class="readonly-display">
      <div
        v-for="branch in internalBranches"
        :key="branch.id"
        class="readonly-item"
      >
        <span class="branch-name">{{ branch.name }}</span>
        <span v-if="allowDefault && 'isDefault' in branch && branch.isDefault" class="default-badge">{{ t('etl.dag.node.defaultBadge') }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Plus } from '@element-plus/icons-vue'
import type { CheckboxValueType } from 'element-plus'
import { generateGuid } from '@/utils/guid'
import { useLocale } from '@/composables/useLocale'

/**
 * BranchEditor - 分支编辑器组件
 *
 * 功能特性：
 * - 支持添加/删除分支
 * - 支持设置默认分支（allowDefault 模式）
 * - 支持只读模式
 * - 至少保留一个分支
 * - 设置默认分支时自动取消其他分支的默认状态
 */

const { t } = useLocale()

/**
 * 条件分支类型
 */
export interface ConditionBranch {
  id: string
  name: string
  isDefault?: boolean
}

/**
 * 并行分支类型
 */
export interface ParallelBranch {
  id: string
  name: string
}

/**
 * Props 定义
 */
interface Props {
  /** 分支列表 */
  modelValue: ConditionBranch[] | ParallelBranch[]
  /** 是否允许设置默认分支 */
  allowDefault?: boolean
  /** 只读模式 */
  readonly?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => [],
  allowDefault: false,
  readonly: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: ConditionBranch[] | ParallelBranch[]): void
}>()

/**
 * 内部分支数据
 */
const internalBranches = ref<(ConditionBranch | ParallelBranch)[]>([])

/**
 * 初始化内部数据
 */
const initializeInternalItems = () => {
  if (Array.isArray(props.modelValue)) {
    internalBranches.value = [...props.modelValue]
  } else {
    internalBranches.value = []
  }
}

/**
 * 监听 modelValue 变化
 */
watch(
  () => props.modelValue,
  () => {
    initializeInternalItems()
  },
  { immediate: true, deep: true }
)

/**
 * 添加新分支
 */
const handleAdd = () => {
  const newBranch: ConditionBranch = {
    id: generateGuid(),
    name: '',
    isDefault: false,
  }
  internalBranches.value.push(newBranch)
  emitChange()
}

/**
 * 删除分支
 */
const handleDelete = (index: number) => {
  // 至少保留一个分支
  if (internalBranches.value.length <= 1) {
    return
  }
  internalBranches.value.splice(index, 1)
  emitChange()
}

/**
 * 更新分支名称
 */
const handleUpdateName = (index: number, name: string) => {
  internalBranches.value[index].name = name
  emitChange()
}

/**
 * 更新默认分支状态
 * 设置默认分支时自动取消其他分支的默认状态
 */
const handleUpdateDefault = (index: number, isDefault: boolean) => {
  if (isDefault) {
    // 取消所有其他分支的默认状态
    internalBranches.value.forEach((branch, i) => {
      if ('isDefault' in branch) {
        branch.isDefault = i === index
      }
    })
  } else {
    // 直接取消当前分支的默认状态
    const branch = internalBranches.value[index]
    if ('isDefault' in branch) {
      branch.isDefault = false
    }
  }
  emitChange()
}

/**
 * 发送数据变更事件
 */
const emitChange = () => {
  emit('update:modelValue', [...internalBranches.value])
}
</script>

<style scoped lang="scss">
.branch-editor {
  width: 100%;
  border: 1px solid var(--el-border-color-lighter);
  border-radius: 4px;
  overflow: hidden;
}

.editor-header {
  display: flex;
  background-color: var(--el-fill-color-light);
  border-bottom: 1px solid var(--el-border-color-lighter);

  .header-cell {
    padding: 8px 12px;
    font-size: 13px;
    font-weight: 500;
    color: var(--el-text-color-regular);
    flex-shrink: 0;

    &.name-cell {
      flex: 1;
      min-width: 120px;
    }

    &.default-cell {
      width: 80px;
      text-align: center;
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

  .editor-row {
    display: flex;
    border-bottom: 1px solid var(--el-border-color-lighter);

    &:last-child {
      border-bottom: none;
    }

    .row-cell {
      padding: 4px;
      flex-shrink: 0;

      &.name-cell {
        flex: 1;
        min-width: 120px;

        :deep(.el-input) {
          width: 100%;
        }
      }

      &.default-cell {
        width: 80px;
        display: flex;
        align-items: center;
        justify-content: center;
      }

      &.action-cell {
        width: 60px;
        display: flex;
        align-items: center;
        justify-content: center;
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
  border-top: 1px solid var(--el-border-color-lighter);
  background-color: var(--el-fill-color-blank);
}

.readonly-display {
  padding: 8px 12px;

  .readonly-item {
    padding: 6px 0;
    font-size: 13px;
    color: var(--el-text-color-regular);
    display: flex;
    align-items: center;
    gap: 8px;

    .branch-name {
      flex: 1;
    }

    .default-badge {
      font-size: 12px;
      padding: 2px 6px;
      background-color: var(--el-color-primary-light-9);
      color: var(--el-color-primary);
      border-radius: 4px;
    }
  }
}
</style>