<!-- 基础表格组件（无拖拽功能） -->
<template>
  <div class="base-table">
    <!-- 表格 -->
    <el-table
      ref="tableRef"
      v-loading="loading"
      :data="data"
      :border="border"
      :row-key="rowKey"
      :height="height"
      :max-height="maxHeight"
      :stripe="stripe"
      :size="size"
      @selection-change="handleSelectionChange"
      @sort-change="handleSortChange"
      @row-click="handleRowClick"
      @row-dblclick="handleRowDblclick"
    >
      <!-- 选择列 -->
      <el-table-column v-if="selection" type="selection" width="50" align="center" />

      <!-- 序号列 -->
      <el-table-column v-if="showIndex" type="index" label="序号" width="60" align="center" />

      <!-- 动态列 -->
      <el-table-column
        v-for="col in columns"
        :key="col.prop"
        :prop="col.prop"
        :label="col.label"
        :min-width="col.minWidth"
        :width="col.width"
        :align="col.align"
        :sortable="col.sortable ? 'custom' : false"
        :fixed="col.fixed"
        :show-overflow-tooltip="col.showOverflowTooltip !== false"
      >
        <template #default="scope">
          <!-- 自定义插槽 -->
          <slot v-if="$slots[col.prop]" :name="col.prop" :row="scope.row" :$index="scope.$index" />
          <!-- 默认显示 -->
          <template v-else>{{ scope.row[col.prop] }}</template>
        </template>
      </el-table-column>

      <!-- 操作列插槽 -->
      <slot name="operation" />
    </el-table>

    <!-- 分页 -->
    <el-pagination
      v-if="pagination"
      v-model:current-page="localPageIndex"
      v-model:page-size="localPageSize"
      :total="total"
      :page-sizes="pageSizes"
      :layout="paginationLayout"
      :background="paginationBackground"
      style="margin-top: 16px; justify-content: flex-end"
      @size-change="handlePageChange"
      @current-change="handlePageChange"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'

// 列配置类型
export interface TableColumn {
  prop: string
  label: string
  minWidth?: number
  width?: number
  align?: 'left' | 'center' | 'right'
  sortable?: boolean
  fixed?: 'left' | 'right' | boolean
  showOverflowTooltip?: boolean
}

// Props
const props = withDefaults(defineProps<{
  data: any[]
  columns: TableColumn[]
  loading?: boolean
  rowKey?: string
  border?: boolean
  stripe?: boolean
  size?: 'large' | 'default' | 'small'
  height?: number | string
  maxHeight?: number | string
  selection?: boolean
  showIndex?: boolean
  pagination?: boolean
  total?: number
  pageIndex?: number
  pageSize?: number
  pageSizes?: number[]
  paginationLayout?: string
  paginationBackground?: boolean
}>(), {
  rowKey: 'id',
  border: true,
  stripe: false,
  size: 'default',
  selection: false,
  showIndex: false,
  pagination: true,
  total: 0,
  pageIndex: 1,
  pageSize: 10,
  pageSizes: () => [10, 20, 50, 100],
  paginationLayout: 'total, sizes, prev, pager, next, jumper',
  paginationBackground: false,
})

// Emits
const emit = defineEmits<{
  (e: 'update:pageIndex', value: number): void
  (e: 'update:pageSize', value: number): void
  (e: 'selection-change', rows: any[]): void
  (e: 'sort-change', sort: { prop: string; order: string | null }): void
  (e: 'page-change'): void
  (e: 'row-click', row: any, column: any, event: MouseEvent): void
  (e: 'row-dblclick', row: any, column: any, event: MouseEvent): void
}>()

// Refs
const tableRef = ref()

// 响应式数据
const localPageIndex = ref(props.pageIndex)
const localPageSize = ref(props.pageSize)

// 监听 props 变化
watch(() => props.pageIndex, (val) => {
  localPageIndex.value = val
})

watch(() => props.pageSize, (val) => {
  localPageSize.value = val
})

// 监听本地变化，触发 emit
watch(localPageIndex, (val) => {
  emit('update:pageIndex', val)
})

watch(localPageSize, (val) => {
  emit('update:pageSize', val)
})

// 事件处理
const handleSelectionChange = (rows: any[]) => {
  emit('selection-change', rows)
}

const handleSortChange = ({ prop, order }: { prop: string; order: string | null }) => {
  emit('sort-change', { prop, order })
}

const handlePageChange = () => {
  emit('page-change')
}

const handleRowClick = (row: any, column: any, event: MouseEvent) => {
  emit('row-click', row, column, event)
}

const handleRowDblclick = (row: any, column: any, event: MouseEvent) => {
  emit('row-dblclick', row, column, event)
}

// 暴露方法和表格实例
defineExpose({
  tableRef,
  // 透传 el-table 方法
  clearSelection: () => tableRef.value?.clearSelection(),
  toggleRowSelection: (row: any, selected?: boolean) => tableRef.value?.toggleRowSelection(row, selected),
  toggleAllSelection: () => tableRef.value?.toggleAllSelection(),
  setCurrentRow: (row: any) => tableRef.value?.setCurrentRow(row),
  clearSort: () => tableRef.value?.clearSort(),
  clearFilter: (columnKeys?: string[]) => tableRef.value?.clearFilter(columnKeys),
  doLayout: () => tableRef.value?.doLayout(),
  sort: (prop: string, order: string) => tableRef.value?.sort(prop, order),
})
</script>

<style scoped lang="scss">
.base-table {
  :deep(.el-pagination) {
    display: flex;
  }
}
</style>