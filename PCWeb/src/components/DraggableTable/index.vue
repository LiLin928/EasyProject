<!-- 可拖拽表格组件 -->
<template>
  <div class="draggable-table">
    <!-- 提示信息 -->
    <div v-if="showTips" class="table-tips">
      <el-tag type="info" size="small">
        <el-icon><InfoFilled /></el-icon>
        {{ tipsText }}
      </el-tag>
    </div>

    <!-- 表格 -->
    <el-table
      ref="tableRef"
      v-loading="loading"
      :data="data"
      :border="border"
      :row-key="rowKey"
      @selection-change="handleSelectionChange"
      @sort-change="handleSortChange"
    >
      <!-- 选择列 -->
      <el-table-column v-if="selection" type="selection" width="50" align="center" />

      <!-- 动态列 -->
      <el-table-column
        v-for="col in visibleColumns"
        :key="col.prop"
        :prop="col.prop"
        :label="col.label"
        :min-width="col.minWidth"
        :width="col.width"
        :align="col.align"
        :sortable="col.sortable ? 'custom' : false"
        :fixed="col.fixed"
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
      style="margin-top: 16px; justify-content: flex-end"
      @size-change="handlePageChange"
      @current-change="handlePageChange"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick, onUnmounted } from 'vue'
import { ElMessage } from 'element-plus'
import { InfoFilled } from '@element-plus/icons-vue'
import Sortable from 'sortablejs'

// 列配置类型
export interface TableColumn {
  prop: string
  label: string
  minWidth?: number
  width?: number
  align?: 'left' | 'center' | 'right'
  sortable?: boolean
  fixed?: 'left' | 'right' | boolean
  visible?: boolean
}

// Props
const props = withDefaults(defineProps<{
  data: any[]
  columns: TableColumn[]
  loading?: boolean
  rowKey?: string
  border?: boolean
  selection?: boolean
  pagination?: boolean
  total?: number
  pageIndex?: number
  pageSize?: number
  pageSizes?: number[]
  paginationLayout?: string
  showTips?: boolean
  tipsText?: string
  draggableRow?: boolean
  draggableColumn?: boolean
}>(), {
  rowKey: 'id',
  border: true,
  selection: false,
  pagination: true,
  total: 0,
  pageIndex: 1,
  pageSize: 10,
  pageSizes: () => [10, 20, 50, 100],
  paginationLayout: 'total, sizes, prev, pager, next, jumper',
  showTips: true,
  tipsText: '支持拖拽行和列调整顺序',
  draggableRow: true,
  draggableColumn: true,
})

// Emits
const emit = defineEmits<{
  (e: 'update:pageIndex', value: number): void
  (e: 'update:pageSize', value: number): void
  (e: 'selection-change', rows: any[]): void
  (e: 'sort-change', sort: { prop: string; order: string | null }): void
  (e: 'page-change'): void
  (e: 'row-order-change', data: any[]): void
  (e: 'column-order-change', columns: TableColumn[]): void
}>()

// Refs
const tableRef = ref()
let rowSortableInstance: Sortable | null = null
let colSortableInstance: Sortable | null = null

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

// 可见列（支持拖拽排序）
const visibleColumns = ref<TableColumn[]>([...props.columns])

// 监听 columns 变化
watch(() => props.columns, (newCols) => {
  visibleColumns.value = [...newCols]
}, { deep: true })

// 监听数据变化，重新初始化拖拽
watch(() => props.data, () => {
  nextTick(() => {
    initDraggables()
  })
}, { flush: 'post' })

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

// ==================== 拖拽功能 ====================

const initDraggables = () => {
  if (props.draggableRow) {
    initRowSortable()
  }
  if (props.draggableColumn) {
    initColumnSortable()
  }
}

// 拖拽行
const initRowSortable = () => {
  const el = tableRef.value?.$el.querySelector('.el-table__body-wrapper tbody')
  if (!el) return

  // 销毁旧实例
  if (rowSortableInstance) {
    rowSortableInstance.destroy()
  }

  rowSortableInstance = new Sortable(el, {
    animation: 150,
    ghostClass: 'sortable-ghost-row',
    chosenClass: 'sortable-chosen-row',
    onEnd: (evt) => {
      const { oldIndex, newIndex } = evt
      if (oldIndex === newIndex) return

      // 创建新数组并交换
      const newData = [...props.data]
      const [movedRow] = newData.splice(oldIndex!, 1)
      newData.splice(newIndex!, 0, movedRow)

      emit('row-order-change', newData)
      ElMessage.success('行顺序已更新')
    },
  })
}

// 拖拽列
const initColumnSortable = () => {
  const headerWrapper = tableRef.value?.$el.querySelector('.el-table__header-wrapper')
  if (!headerWrapper) return

  const headerTr = headerWrapper.querySelector('tr')
  if (!headerTr) return

  // 销毁旧实例
  if (colSortableInstance) {
    colSortableInstance.destroy()
  }

  colSortableInstance = new Sortable(headerTr, {
    animation: 150,
    ghostClass: 'sortable-ghost-col',
    chosenClass: 'sortable-chosen-col',
    filter: '.el-table-column--selection',
    draggable: 'th:not(.el-table-column--selection)',
    onEnd: (evt) => {
      const { oldIndex, newIndex } = evt

      // 计算实际索引（如果有选择列需要减1）
      const offset = props.selection ? 1 : 0
      const actualOldIndex = oldIndex! - offset
      const actualNewIndex = newIndex! - offset

      if (actualOldIndex === actualNewIndex) return
      if (actualOldIndex < 0 || actualNewIndex < 0) return
      if (actualOldIndex >= visibleColumns.value.length || actualNewIndex >= visibleColumns.value.length) return

      // 交换列配置
      const newColumns = [...visibleColumns.value]
      const [movedCol] = newColumns.splice(actualOldIndex, 1)
      newColumns.splice(actualNewIndex, 0, movedCol)
      visibleColumns.value = newColumns

      emit('column-order-change', newColumns)
      ElMessage.success('列顺序已更新')
    },
  })
}

// 组件卸载时清理
onUnmounted(() => {
  if (rowSortableInstance) {
    rowSortableInstance.destroy()
  }
  if (colSortableInstance) {
    colSortableInstance.destroy()
  }
})

// 暴露方法
defineExpose({
  tableRef,
  refreshDraggables: initDraggables,
})
</script>

<style scoped lang="scss">
.draggable-table {
  .table-tips {
    margin-bottom: 12px;
  }
}

// 拖拽行样式
.sortable-ghost-row {
  opacity: 0.4;
  background-color: #ecfdf5 !important;
}

.sortable-chosen-row {
  background-color: #d1fae5 !important;
}

// 拖拽列样式
.sortable-ghost-col {
  opacity: 0.4;
  background-color: #dbeafe !important;
}

.sortable-chosen-col {
  background-color: #bfdbfe !important;
}

:deep(.el-table__row) {
  cursor: move;
}

:deep(.el-table__header th) {
  cursor: move;

  &.el-table-column--selection {
    cursor: default;
  }
}
</style>