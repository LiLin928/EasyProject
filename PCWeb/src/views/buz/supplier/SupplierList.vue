<!-- src/views/buz/supplier/SupplierList.vue -->
<template>
  <div class="supplier-list-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('product.supplier.title') }}</span>
          <div>
            <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleBatchDelete">
              {{ t('product.supplier.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('product.supplier.add') }}
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
      <BaseTable
        ref="baseTableRef"
        :data="tableData"
        :columns="tableColumns"
        :loading="loading"
        :total="total"
        :selection="true"
        :page-index="queryParams.pageIndex"
        :page-size="queryParams.pageSize"
        @update:page-index="queryParams.pageIndex = $event"
        @update:page-size="queryParams.pageSize = $event"
        @page-change="handleSearch"
        @selection-change="handleSelectionChange"
      >
        <!-- Status column -->
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
          <el-table-column :label="t('product.list.operation')" width="200" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleEdit(row)">
                {{ t('product.supplier.edit') }}
              </el-button>
              <el-button link type="primary" @click="handleViewProducts(row)">
                {{ t('product.supplier.productList') }}
              </el-button>
              <el-button link type="danger" @click="handleDelete(row)">
                {{ t('product.supplier.delete') }}
              </el-button>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>

    <!-- Supplier form dialog -->
    <SupplierFormDialog
      v-model="dialogVisible"
      :supplier-id="currentSupplierId"
      @success="handleSearch"
    />

    <!-- Supplier product dialog -->
    <SupplierProductDialog
      v-model="productDialogVisible"
      :supplier="currentSupplier"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { getSupplierList, deleteSupplier, deleteSupplierBatch, updateSupplier } from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { Supplier } from '@/types/product'
import type { TableColumn } from '@/components/BaseTable/index.vue'
import BaseTable from '@/components/BaseTable/index.vue'
import SearchForm from '@/components/SearchForm/index.vue'
import SupplierFormDialog from './components/SupplierFormDialog.vue'
import SupplierProductDialog from './components/SupplierProductDialog.vue'

const { t } = useLocale()

// 搜索表单配置
const searchItems = computed(() => [
  { field: 'name', label: t('product.supplier.name'), type: 'input' },
  { field: 'status', label: t('product.supplier.status'), type: 'select', options: [
    { label: t('product.supplier.enabled'), value: 1 },
    { label: t('product.supplier.disabled'), value: 0 },
  ]},
])

// Table columns
const tableColumns = ref<TableColumn[]>([
  { prop: 'name', label: t('product.supplier.name'), minWidth: 150 },
  { prop: 'unifiedCode', label: t('product.supplier.unifiedCode'), width: 180 },
  { prop: 'contact', label: t('product.supplier.contact'), width: 100 },
  { prop: 'phone', label: t('product.supplier.phone'), width: 130 },
  { prop: 'address', label: t('product.supplier.address'), minWidth: 200 },
  { prop: 'status', label: t('product.supplier.status'), width: 100, align: 'center' },
  { prop: 'createTime', label: t('product.list.updateTime'), width: 160 },
])

const loading = ref(false)
const tableData = ref<Supplier[]>([])
const selectedRows = ref<Supplier[]>([])
const total = ref(0)
const dialogVisible = ref(false)
const productDialogVisible = ref(false)
const currentSupplierId = ref<string | undefined>()
const currentSupplier = ref<Supplier | undefined>()
const baseTableRef = ref()

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  name: '',
  status: undefined as 0 | 1 | undefined,
})

onMounted(() => {
  handleSearch()
})

/**
 * Load suppliers
 */
const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getSupplierList(queryParams)
    tableData.value = data.list
    total.value = data.total
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

/**
 * Reset search form
 */
const handleReset = () => {
  queryParams.pageIndex = 1
  handleSearch()
}

/**
 * Open add dialog
 */
const handleAdd = () => {
  currentSupplierId.value = undefined
  dialogVisible.value = true
}

/**
 * Open edit dialog
 */
const handleEdit = (row: Supplier) => {
  currentSupplierId.value = row.id
  dialogVisible.value = true
}

/**
 * Handle status change with API call
 */
const handleStatusBeforeChange = async (row: Supplier): Promise<boolean> => {
  const newStatus = row.status === 1 ? 0 : 1
  try {
    await updateSupplier({ id: row.id, status: newStatus })
    row.status = newStatus
    ElMessage.success(newStatus === 1 ? t('product.supplier.enabled') : t('product.supplier.disabled'))
    return true
  } catch (error) {
    return false
  }
}

/**
 * View supplier's products
 */
const handleViewProducts = (row: Supplier) => {
  currentSupplier.value = row
  productDialogVisible.value = true
}

/**
 * Delete supplier
 */
const handleDelete = async (row: Supplier) => {
  try {
    await ElMessageBox.confirm(
      t('product.supplier.deleteConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteSupplier(row.id)
    ElMessage.success(t('product.supplier.deleteSuccess'))
    handleSearch()
  } catch (error: any) {
    if (error?.response?.data?.message) {
      ElMessage.error(error.response.data.message)
    }
  }
}

/**
 * Handle selection change
 */
const handleSelectionChange = (rows: Supplier[]) => {
  selectedRows.value = rows
}

/**
 * Batch delete suppliers
 */
const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  try {
    await ElMessageBox.confirm(
      t('product.supplier.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    const ids = selectedRows.value.map(row => row.id)
    await deleteSupplierBatch(ids)
    ElMessage.success(t('product.supplier.deleteSuccess'))
    baseTableRef.value?.clearSelection()
    handleSearch()
  } catch (error: any) {
    if (error?.response?.data?.message) {
      ElMessage.error(error.response.data.message)
    }
  }
}
</script>

<style scoped lang="scss">
.supplier-list-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .search-form {
    margin-bottom: 16px;
  }
}
</style>