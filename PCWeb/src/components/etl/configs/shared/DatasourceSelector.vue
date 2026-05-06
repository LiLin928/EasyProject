<!-- src/components/etl/DagDesigner/configs/shared/DatasourceSelector.vue -->
<template>
  <el-select
    :model-value="modelValue"
    :placeholder="placeholder"
    :disabled="disabled"
    :clearable="true"
    :filterable="true"
    style="width: 100%"
    @update:model-value="handleChange"
  >
    <el-option
      v-for="ds in datasources"
      :key="ds.id"
      :label="ds.name"
      :value="ds.id"
    >
      <div class="datasource-option">
        <span class="datasource-icon">{{ getTypeIcon(ds.type) }}</span>
        <span class="datasource-name">{{ ds.name }}</span>
        <el-tag
          v-if="ds.description"
          size="small"
          type="info"
          class="datasource-desc"
        >
          {{ truncateText(ds.description, 20) }}
        </el-tag>
      </div>
    </el-option>
  </el-select>
</template>

<script setup lang="ts">
import type { Datasource, DatasourceType } from '@/types/etl/datasource'

/**
 * DatasourceSelector - 数据源选择器组件
 *
 * 支持功能：
 * - v-model 双向绑定
 * - 过滤搜索
 * - 数据源类型图标显示
 * - 禁用状态
 * - 描述信息截断显示
 */

// Props 定义
interface Props {
  /** 选中的数据源 ID */
  modelValue: string
  /** 数据源列表 */
  datasources: Datasource[]
  /** 禁用状态 */
  disabled?: boolean
  /** 占位提示 */
  placeholder?: string
}

withDefaults(defineProps<Props>(), {
  disabled: false,
  placeholder: '请选择数据源',
})

// Emits 定义
const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
}>()

/**
 * 数据源类型图标映射
 */
const typeIconMap: Record<DatasourceType, string> = {
  mysql: '🐬',
  postgresql: '🐘',
  oracle: '🔴',
  sqlserver: '🪟',
  mongodb: '🍃',
  redis: '🔴',
  elasticsearch: '🔍',
  http_api: '🌐',
  ftp: '📁',
  sftp: '🔒',
  hive: '🐝',
  clickhouse: '⚡',
}

/**
 * 获取数据源类型图标
 */
const getTypeIcon = (type: DatasourceType): string => {
  return typeIconMap[type] || '📦'
}

/**
 * 截断文本
 */
const truncateText = (text: string, maxLength: number): string => {
  if (!text) return ''
  return text.length > maxLength ? `${text.slice(0, maxLength)}...` : text
}

/**
 * 处理选择变更
 */
const handleChange = (value: string) => {
  emit('update:modelValue', value)
}
</script>

<style scoped lang="scss">
.datasource-option {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;
}

.datasource-icon {
  font-size: 16px;
  flex-shrink: 0;
}

.datasource-name {
  flex: 1;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.datasource-desc {
  flex-shrink: 0;
  max-width: 120px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}
</style>