<template>
  <div class="field-mapping-editor">
    <!-- Table Header -->
    <div v-if="!readonly && internalItems.length > 0" class="editor-header">
      <div class="header-cell" style="width: 30%">源字段</div>
      <div class="header-cell" style="width: 30%">目标字段</div>
      <div class="header-cell" style="width: 20%">转换函数</div>
      <div class="header-cell" style="width: 15%">数据类型</div>
      <div class="header-cell action-cell" style="width: 5%">操作</div>
    </div>

    <!-- Table Body -->
    <div class="editor-body">
      <div
        v-for="(item, index) in internalItems"
        :key="index"
        class="editor-row"
      >
        <!-- Source Field -->
        <div class="row-cell" style="width: 30%">
          <el-input
            :model-value="item.sourceField"
            placeholder="请输入源字段"
            :disabled="readonly"
            size="small"
            @update:model-value="(val: string) => handleUpdate(index, 'sourceField', val)"
          />
        </div>

        <!-- Target Field -->
        <div class="row-cell" style="width: 30%">
          <el-input
            :model-value="item.targetField"
            placeholder="请输入目标字段"
            :disabled="readonly"
            size="small"
            @update:model-value="(val: string) => handleUpdate(index, 'targetField', val)"
          />
        </div>

        <!-- Transform Function -->
        <div class="row-cell" style="width: 20%">
          <el-select
            :model-value="item.transform"
            placeholder="选择转换函数"
            :disabled="readonly"
            :clearable="true"
            size="small"
            style="width: 100%"
            @update:model-value="(val: string) => handleUpdate(index, 'transform', val)"
          >
            <el-option
              v-for="opt in transformOptions"
              :key="opt.value"
              :label="opt.label"
              :value="opt.value"
            />
          </el-select>
        </div>

        <!-- Data Type -->
        <div class="row-cell" style="width: 15%">
          <el-select
            :model-value="item.dataType"
            placeholder="数据类型"
            :disabled="readonly"
            :clearable="true"
            size="small"
            style="width: 100%"
            @update:model-value="(val: string) => handleUpdate(index, 'dataType', val)"
          >
            <el-option
              v-for="opt in dataTypeOptions"
              :key="opt.value"
              :label="opt.label"
              :value="opt.value"
            />
          </el-select>
        </div>

        <!-- Actions -->
        <div class="row-cell action-cell" style="width: 5%">
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
        <span class="empty-text">暂无字段映射</span>
      </div>
    </div>

    <!-- Add Button -->
    <div v-if="!readonly" class="editor-footer">
      <el-button type="primary" link @click="handleAdd">
        <el-icon><Plus /></el-icon>
        添加映射
      </el-button>
    </div>

    <!-- Readonly Display -->
    <div v-if="readonly && internalItems.length > 0" class="readonly-display">
      <div
        v-for="(item, index) in internalItems"
        :key="index"
        class="readonly-item"
      >
        <span class="item-label">{{ item.sourceField || '-' }}</span>
        <span class="item-arrow">
          <el-icon><Right /></el-icon>
        </span>
        <span class="item-label">{{ item.targetField || '-' }}</span>
        <span v-if="item.transform" class="item-badge transform">
          {{ getTransformLabel(item.transform) }}
        </span>
        <span v-if="item.dataType" class="item-badge data-type">
          {{ getDataTypeLabel(item.dataType) }}
        </span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Plus, Right } from '@element-plus/icons-vue'
import type { FieldMappingItem } from '@/types/etl/taskNode'

/**
 * FieldMappingEditor - 字段映射编辑器组件
 *
 * 功能特性：
 * - 表格形式展示字段映射关系
 * - 支持源字段、目标字段、转换函数、数据类型四列
 * - 支持添加/删除映射行
 * - 支持只读模式
 */

/**
 * Props 定义
 */
interface Props {
  /** 字段映射数据 */
  modelValue: FieldMappingItem[]
  /** 只读模式 */
  readonly?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => [],
  readonly: false,
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: FieldMappingItem[]): void
}>()

/**
 * 转换函数选项
 */
const transformOptions = [
  { value: 'uppercase', label: '转大写' },
  { value: 'lowercase', label: '转小写' },
  { value: 'trim', label: '去除空格' },
  { value: 'date_format', label: '日期格式化' },
  { value: 'number_format', label: '数字格式化' },
]

/**
 * 数据类型选项
 */
const dataTypeOptions = [
  { value: 'string', label: '字符串' },
  { value: 'number', label: '数字' },
  { value: 'date', label: '日期' },
  { value: 'boolean', label: '布尔' },
]

/**
 * 内部数据数组
 */
const internalItems = ref<FieldMappingItem[]>([])

/**
 * 监听 modelValue 变化
 */
watch(
  () => props.modelValue,
  (newValue) => {
    internalItems.value = Array.isArray(newValue) ? [...newValue] : []
  },
  { immediate: true, deep: true }
)

/**
 * 获取转换函数标签
 */
const getTransformLabel = (value: string): string => {
  const option = transformOptions.find((opt) => opt.value === value)
  return option ? option.label : value
}

/**
 * 获取数据类型标签
 */
const getDataTypeLabel = (value: string): string => {
  const option = dataTypeOptions.find((opt) => opt.value === value)
  return option ? option.label : value
}

/**
 * 获取默认新行数据
 */
const getDefaultItem = (): FieldMappingItem => ({
  sourceField: '',
  targetField: '',
  transform: undefined,
  dataType: undefined,
})

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
const handleUpdate = (
  index: number,
  prop: keyof FieldMappingItem,
  value: string | undefined
) => {
  internalItems.value[index][prop] = value as never
  emitChange()
}

/**
 * 发送数据变更事件
 */
const emitChange = () => {
  emit('update:modelValue', [...internalItems.value])
}
</script>

<style scoped lang="scss">
.field-mapping-editor {
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
    display: flex;
    align-items: center;
    padding: 6px 0;
    font-size: 13px;
    color: var(--el-text-color-regular);
    border-bottom: 1px dashed var(--el-border-color-lighter);

    &:last-child {
      border-bottom: none;
    }

    .item-label {
      font-weight: 500;
      min-width: 80px;
    }

    .item-arrow {
      margin: 0 8px;
      color: var(--el-text-color-placeholder);
      font-size: 12px;
    }

    .item-badge {
      margin-left: 8px;
      padding: 2px 6px;
      font-size: 12px;
      border-radius: 4px;

      &.transform {
        background-color: var(--el-color-primary-light-9);
        color: var(--el-color-primary);
      }

      &.data-type {
        background-color: var(--el-color-success-light-9);
        color: var(--el-color-success);
      }
    }
  }
}
</style>