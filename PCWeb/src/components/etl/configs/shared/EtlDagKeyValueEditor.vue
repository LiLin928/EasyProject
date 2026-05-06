<template>
  <div class="key-value-editor">
    <!-- Table Header -->
    <div v-if="!readonly && internalItems.length > 0" class="editor-header">
      <div
        v-for="col in computedColumns"
        :key="col.prop"
        class="header-cell"
        :style="{ width: col.width || 'auto' }"
      >
        {{ col.label }}
      </div>
      <div class="header-cell action-cell">操作</div>
    </div>

    <!-- Table Body -->
    <div class="editor-body">
      <div
        v-for="(item, index) in internalItems"
        :key="index"
        class="editor-row"
      >
        <div
          v-for="col in computedColumns"
          :key="col.prop"
          class="row-cell"
          :style="{ width: col.width || 'auto' }"
        >
          <!-- Input Type -->
          <el-input
            v-if="col.type === 'input'"
            :model-value="item[col.prop]"
            :placeholder="col.placeholder"
            :disabled="readonly"
            size="small"
            @update:model-value="(val: string) => handleUpdate(index, col.prop, val)"
          />
          <!-- Select Type -->
          <el-select
            v-else-if="col.type === 'select'"
            :model-value="item[col.prop]"
            :placeholder="col.placeholder"
            :disabled="readonly"
            :clearable="true"
            size="small"
            style="width: 100%"
            @update:model-value="(val: string) => handleUpdate(index, col.prop, val)"
          >
            <el-option
              v-for="opt in col.options"
              :key="opt"
              :label="opt"
              :value="opt"
            />
          </el-select>
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
            删除
          </el-button>
        </div>
      </div>

      <!-- Empty State -->
      <div v-if="internalItems.length === 0" class="empty-state">
        <span class="empty-text">暂无数据</span>
      </div>
    </div>

    <!-- Add Button -->
    <div v-if="!readonly" class="editor-footer">
      <el-button type="primary" link @click="handleAdd">
        <el-icon><Plus /></el-icon>
        添加一行
      </el-button>
    </div>

    <!-- Readonly Display -->
    <div v-if="readonly && internalItems.length > 0" class="readonly-display">
      <div
        v-for="(item, index) in internalItems"
        :key="index"
        class="readonly-item"
      >
        <template v-for="(col, colIndex) in computedColumns" :key="col.prop">
          <span v-if="colIndex > 0" class="separator">:</span>
          <span class="item-value">{{ item[col.prop] || '-' }}</span>
        </template>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Plus } from '@element-plus/icons-vue'

/**
 * KeyValueEditor - 键值对编辑器组件
 *
 * 功能特性：
 * - 支持动态列配置
 * - 支持输入框和下拉选择两种列类型
 * - 支持添加/删除行
 * - 支持只读模式
 * - 支持对象和数组两种数据格式
 */

/**
 * 列配置接口
 */
export interface Column {
  /** 列属性名 */
  prop: string
  /** 列标题 */
  label: string
  /** 列类型：input 输入框，select 下拉选择 */
  type: 'input' | 'select'
  /** 列宽度 */
  width?: number | string
  /** 占位提示 */
  placeholder?: string
  /** 下拉选项（type 为 select 时使用） */
  options?: string[]
}

/**
 * Props 定义
 */
interface Props {
  /** 键值对数据（支持对象或数组格式） */
  modelValue: Record<string, string> | any[]
  /** 列配置 */
  columns?: Column[]
  /** 只读模式 */
  readonly?: boolean
  /** 新行默认值 */
  defaultItem?: Record<string, string>
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => ({}),
  columns: undefined,
  readonly: false,
  defaultItem: undefined,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: Record<string, string> | any[]): void
}>()

/**
 * 默认列配置
 */
const defaultColumns: Column[] = [
  { prop: 'key', label: '键', type: 'input', placeholder: '请输入' },
  { prop: 'value', label: '值', type: 'input', placeholder: '请输入' },
]

/**
 * 计算后的列配置
 */
const computedColumns = computed<Column[]>(() => {
  return props.columns || defaultColumns
})

/**
 * 内部数据数组
 */
const internalItems = ref<Record<string, string>[]>([])

/**
 * 判断是否为对象格式
 */
const isObjectFormat = (value: unknown): value is Record<string, string> => {
  return value !== null && typeof value === 'object' && !Array.isArray(value)
}

/**
 * 初始化内部数据
 */
const initializeInternalItems = () => {
  if (isObjectFormat(props.modelValue)) {
    // 对象格式转换为数组
    internalItems.value = Object.entries(props.modelValue).map(([key, value]) => ({
      key,
      value,
    }))
  } else if (Array.isArray(props.modelValue)) {
    // 数组格式直接使用
    internalItems.value = [...props.modelValue]
  } else {
    internalItems.value = []
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
 * 获取默认新行数据
 */
const getDefaultItem = (): Record<string, string> => {
  if (props.defaultItem) {
    return { ...props.defaultItem }
  }
  // 根据 columns 生成默认值
  const item: Record<string, string> = {}
  computedColumns.value.forEach((col) => {
    item[col.prop] = ''
  })
  return item
}

/**
 * 添加新行
 */
const handleAdd = () => {
  internalItems.value.push(getDefaultItem())
  emitChange()
}

/**
 * 删除行
 */
const handleDelete = (index: number) => {
  internalItems.value.splice(index, 1)
  emitChange()
}

/**
 * 更新单元格值
 */
const handleUpdate = (index: number, prop: string, value: string) => {
  internalItems.value[index][prop] = value
  emitChange()
}

/**
 * 发送数据变更事件
 */
const emitChange = () => {
  if (isObjectFormat(props.modelValue)) {
    // 保持对象格式输出
    const result: Record<string, string> = {}
    internalItems.value.forEach((item) => {
      const key = item.key || ''
      const value = item.value || ''
      if (key) {
        result[key] = value
      }
    })
    emit('update:modelValue', result)
  } else {
    // 保持数组格式输出
    emit('update:modelValue', [...internalItems.value])
  }
}
</script>

<style scoped lang="scss">
.key-value-editor {
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

      :deep(.el-input),
      :deep(.el-select) {
        width: 100%;
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
    padding: 4px 0;
    font-size: 13px;
    color: var(--el-text-color-regular);

    .separator {
      margin: 0 8px;
      color: var(--el-text-color-placeholder);
    }

    .item-value {
      &:first-child {
        font-weight: 500;
      }
    }
  }
}
</style>