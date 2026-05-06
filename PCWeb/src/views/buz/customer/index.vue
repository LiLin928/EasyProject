<!-- src/views/buz/customer/index.vue -->
<template>
  <div class="customer-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('customer.list') }}</span>
          <div class="header-buttons">
            <el-dropdown trigger="click" @command="handleExport">
              <el-button type="success">
                <el-icon><Download /></el-icon>
                {{ t('customer.export') }}
                <el-icon class="el-icon--right"><ArrowDown /></el-icon>
              </el-button>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item command="current">
                    {{ t('common.export.exportCurrentPage') }}
                  </el-dropdown-item>
                  <el-dropdown-item command="all">
                    {{ t('common.export.exportAll') }}
                  </el-dropdown-item>
                  <el-dropdown-item command="selected" :disabled="selectedRows.length === 0">
                    {{ t('common.export.exportSelected') }}
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
            <el-button
              type="danger"
              :disabled="selectedRows.length === 0"
              @click="handleBatchDelete"
            >
              <el-icon><Delete /></el-icon>
              {{ t('customer.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('customer.addCustomer') }}
            </el-button>
          </div>
        </div>
      </template>

      <!-- Search form -->
      <SearchForm
        :items="searchItems"
        v-model="queryParams"
        @search="handleSearch"
        @reset="handleReset"
      />

      <!-- Table -->
      <DraggableTable
        ref="draggableTableRef"
        :data="tableData"
        :columns="tableColumns"
        :loading="loading"
        :selection="true"
        :total="total"
        :page-index="queryParams.pageIndex"
        :page-size="queryParams.pageSize"
        :tips-text="t('common.table.dragTip')"
        @update:page-index="queryParams.pageIndex = $event"
        @update:page-size="queryParams.pageSize = $event"
        @selection-change="handleSelectionChange"
        @sort-change="handleSortChange"
        @page-change="handleSearch"
        @row-order-change="handleRowOrderChange"
        @column-order-change="handleColumnOrderChange"
      >
        <!-- Level column custom render -->
        <template #levelName="{ row }">
          <el-tag v-if="row.levelName" type="primary" effect="plain" size="small">
            {{ row.levelName }}
          </el-tag>
          <span v-else>-</span>
        </template>

        <!-- Points column custom render -->
        <template #points="{ row }">
          <span class="points-value">{{ row.points }}</span>
        </template>

        <!-- Total spent column custom render -->
        <template #totalSpent="{ row }">
          <span class="money-value">{{ formatMoney(row.totalSpent) }}</span>
        </template>

        <!-- Status column custom render -->
        <template #status="{ row }">
          <el-switch
            :model-value="row.status"
            :active-value="1"
            :inactive-value="0"
            :before-change="() => handleStatusBeforeChange(row)"
          />
        </template>

        <!-- Operation column -->
        <template #operation>
          <el-table-column :label="t('customer.detail')" width="220" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleDetail(row)">
                {{ t('customer.detail') }}
              </el-button>
              <el-button link type="primary" @click="handleEdit(row)">
                {{ t('customer.editCustomer') }}
              </el-button>
              <el-button link type="primary" @click="handleAdjustPoints(row)">
                {{ t('customer.adjustPoints') }}
              </el-button>
              <el-button link type="primary" @click="handleAdjustLevel(row)">
                {{ t('customer.adjustLevel') }}
              </el-button>
              <el-button link type="danger" @click="handleDelete(row)">
                {{ t('customer.deleteCustomer') }}
              </el-button>
            </template>
          </el-table-column>
        </template>
      </DraggableTable>
    </el-card>

    <!-- Customer form dialog -->
    <CustomerFormDialog
      v-model="dialogVisible"
      :customer-id="currentCustomerId"
      @success="handleSearch"
    />

    <!-- Points adjust dialog -->
    <PointsAdjustDialog
      v-model="pointsDialogVisible"
      :customer-id="currentCustomerId"
      :customer-points="currentCustomerPoints"
      @success="handleSearch"
    />

    <!-- Level adjust dialog -->
    <LevelAdjustDialog
      v-model="levelDialogVisible"
      :customer-id="currentCustomerId"
      :customer-level-id="currentCustomerLevelId"
      @success="handleSearch"
    />

    <!-- Customer detail dialog -->
    <CustomerDetailDialog
      v-model="detailDialogVisible"
      :customer-id="currentCustomerId"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Delete, Download, ArrowDown } from '@element-plus/icons-vue'
import {
  getCustomerList,
  deleteCustomer,
  updateCustomerStatus,
} from '@/api/buz/customerApi'
import { getMemberLevelList } from '@/api/buz/memberLevelApi'
import { useLocale } from '@/composables/useLocale'
import { exportToExcel } from '@/utils/export'
import type { Customer, MemberLevel } from '@/types'
import type { TableColumn } from '@/components/DraggableTable/index.vue'
import DraggableTable from '@/components/DraggableTable/index.vue'
import SearchForm from '@/components/SearchForm/index.vue'
import CustomerFormDialog from './components/CustomerFormDialog.vue'
import PointsAdjustDialog from './components/PointsAdjustDialog.vue'
import LevelAdjustDialog from './components/LevelAdjustDialog.vue'
import CustomerDetailDialog from './components/CustomerDetailDialog.vue'

const { t } = useLocale()

// Member level list
const memberLevelList = ref<MemberLevel[]>([])

// 搜索表单配置
const searchItems = computed(() => [
  { field: 'phone', label: t('customer.phone'), type: 'input' },
  { field: 'nickname', label: t('customer.nickname'), type: 'input' },
  { field: 'levelId', label: t('customer.level'), type: 'select', options: memberLevelList.value.map(l => ({ label: l.name, value: l.id })) },
  { field: 'status', label: t('customer.status'), type: 'select', options: [
    { label: t('customer.statusActive'), value: 1 },
    { label: t('customer.statusInactive'), value: 0 },
  ]},
])

// Table columns config
const tableColumns = ref<TableColumn[]>([
  { prop: 'phone', label: t('customer.phone'), minWidth: 120, sortable: true },
  { prop: 'nickname', label: t('customer.nickname'), minWidth: 100, sortable: true },
  { prop: 'email', label: t('customer.email'), minWidth: 150, sortable: true },
  { prop: 'levelName', label: t('customer.level'), minWidth: 100 },
  { prop: 'points', label: t('customer.points'), minWidth: 80, align: 'center' },
  { prop: 'totalSpent', label: t('customer.totalSpent'), minWidth: 100, align: 'right' },
  { prop: 'status', label: t('customer.status'), width: 100, align: 'center' },
  { prop: 'createTime', label: t('customer.registerTime'), width: 160, sortable: true },
])

// Current visible columns (for export)
const currentColumns = ref<TableColumn[]>([...tableColumns.value])

const draggableTableRef = ref()
const loading = ref(false)
const tableData = ref<Customer[]>([])
const total = ref(0)
const selectedRows = ref<Customer[]>([])

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  phone: '',
  nickname: '',
  levelId: '',
  status: undefined as 0 | 1 | undefined,
  sortField: '',
  sortOrder: '' as '' | 'ascending' | 'descending',
})

const dialogVisible = ref(false)
const pointsDialogVisible = ref(false)
const levelDialogVisible = ref(false)
const detailDialogVisible = ref(false)
const currentCustomerId = ref<string | undefined>(undefined)
const currentCustomerPoints = ref(0)
const currentCustomerLevelId = ref('')

onMounted(() => {
  loadMemberLevelList()
  handleSearch()
})

const loadMemberLevelList = async () => {
  try {
    const data = await getMemberLevelList()
    memberLevelList.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getCustomerList(queryParams)
    tableData.value = data.list
    total.value = data.total
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.pageIndex = 1
  queryParams.sortField = ''
  queryParams.sortOrder = ''
  handleSearch()
}

const handleSelectionChange = (rows: Customer[]) => {
  selectedRows.value = rows
}

const handleSortChange = ({ prop, order }: { prop: string; order: string | null }) => {
  queryParams.sortField = prop || ''
  queryParams.sortOrder = order as '' | 'ascending' | 'descending'
  handleSearch()
}

const handleRowOrderChange = (newData: Customer[]) => {
  tableData.value = newData
}

const handleColumnOrderChange = (newColumns: TableColumn[]) => {
  currentColumns.value = newColumns
}

const handleAdd = () => {
  currentCustomerId.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: Customer) => {
  currentCustomerId.value = row.id
  dialogVisible.value = true
}

const handleDetail = (row: Customer) => {
  currentCustomerId.value = row.id
  detailDialogVisible.value = true
}

const handleDelete = async (row: Customer) => {
  try {
    await ElMessageBox.confirm(
      t('customer.deleteCustomerConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteCustomer(row.id)
    ElMessage.success(t('customer.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleStatusBeforeChange = async (row: Customer): Promise<boolean> => {
  const newStatus = row.status === 1 ? 0 : 1
  try {
    await updateCustomerStatus(row.id, newStatus as 0 | 1)
    row.status = newStatus
    ElMessage.success(newStatus === 1 ? t('customer.enabled') : t('customer.disabled'))
    return true
  } catch (error) {
    return false
  }
}

const handleAdjustPoints = (row: Customer) => {
  currentCustomerId.value = row.id
  currentCustomerPoints.value = row.points
  pointsDialogVisible.value = true
}

const handleAdjustLevel = (row: Customer) => {
  currentCustomerId.value = row.id
  currentCustomerLevelId.value = row.levelId
  levelDialogVisible.value = true
}

const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  const idsToDelete = selectedRows.value.map(row => row.id)

  try {
    await ElMessageBox.confirm(
      t('customer.batchDeleteConfirm', { count: idsToDelete.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteCustomer(idsToDelete)
    ElMessage.success(t('customer.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

// ==================== Export ====================

const handleExport = async (type: string) => {
  let exportData: Customer[] = []

  switch (type) {
    case 'current':
      exportData = tableData.value
      break
    case 'all':
      try {
        const data = await getCustomerList({ ...queryParams, pageIndex: 1, pageSize: 9999 })
        exportData = data.list
      } catch (error) {
        ElMessage.error(t('common.export.exportFailed'))
        return
      }
      break
    case 'selected':
      exportData = selectedRows.value
      break
  }

  if (exportData.length === 0) {
    ElMessage.warning(t('common.export.noData'))
    return
  }

  // Transform status to readable text
  const transformedData = exportData.map(item => ({
    ...item,
    status: item.status === 1 ? t('customer.statusActive') : t('customer.statusInactive'),
    totalSpent: formatMoney(item.totalSpent),
  }))

  exportToExcel({
    columns: currentColumns.value.filter(c => c.prop !== 'status'),
    data: transformedData,
    fileName: `${t('customer.list')}_${new Date().toLocaleDateString().replace(/\//g, '-')}`,
    sheetName: t('customer.list'),
  })

  ElMessage.success(t('customer.exportSuccess'))
}

const formatMoney = (value: number) => {
  return value.toLocaleString('zh-CN', { style: 'currency', currency: 'CNY' })
}
</script>

<style scoped lang="scss">
.customer-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .header-buttons {
      display: flex;
      gap: 8px;
    }
  }

  .search-form {
    margin-bottom: 16px;
  }

  .points-value {
    color: #f56c6c;
    font-weight: 500;
  }

  .money-value {
    color: #67c23a;
    font-weight: 500;
  }
}
</style>