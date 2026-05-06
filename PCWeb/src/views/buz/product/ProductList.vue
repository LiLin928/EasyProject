<!-- src/views/buz/product/ProductList.vue -->
<template>
  <div class="product-container">
    <!-- Left sidebar: Category tree -->
    <div class="sidebar">
      <CategoryTree ref="categoryTreeRef" @select="handleCategorySelect" @loaded="handleCategoryLoaded" />
    </div>

    <!-- Right main content -->
    <div class="main-content">
      <el-card shadow="never">
        <template #header>
          <div class="card-header">
            <span>{{ t('product.list.title') }}</span>
            <div class="header-buttons">
              <!-- Export dropdown -->
              <el-dropdown trigger="click" @command="handleExport">
                <el-button type="success">
                  <el-icon><Download /></el-icon>
                  {{ t('product.list.export') }}
                  <el-icon class="el-icon--right"><ArrowDown /></el-icon>
                </el-button>
                <template #dropdown>
                  <el-dropdown-menu>
                    <el-dropdown-item command="current">
                      {{ t('product.list.exportCurrent') }}
                    </el-dropdown-item>
                    <el-dropdown-item command="all">
                      {{ t('product.list.exportAll') }}
                    </el-dropdown-item>
                    <el-dropdown-item command="selected" :disabled="selectedRows.length === 0">
                      {{ t('product.list.exportSelected') }}
                    </el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>
              <!-- Import button -->
              <el-button type="info" @click="importVisible = true">
                <el-icon><Upload /></el-icon>
                {{ t('product.list.import') }}
              </el-button>
              <!-- Batch operation dropdown -->
              <el-dropdown trigger="click" @command="handleBatchAction">
                <el-button type="warning">
                  {{ t('product.list.batchOperation') }}
                  <el-icon class="el-icon--right"><ArrowDown /></el-icon>
                </el-button>
                <template #dropdown>
                  <el-dropdown-menu>
                    <el-dropdown-item command="delete" :disabled="selectedRows.length === 0">
                      {{ t('product.list.batchDelete') }}
                    </el-dropdown-item>
                    <el-dropdown-item command="category" :disabled="selectedRows.length === 0">
                      {{ t('product.list.batchCategory') }}
                    </el-dropdown-item>
                    <el-dropdown-item command="hot" :disabled="selectedRows.length === 0">
                      {{ t('product.list.batchHot') }}
                    </el-dropdown-item>
                    <el-dropdown-item command="new" :disabled="selectedRows.length === 0">
                      {{ t('product.list.batchNew') }}
                    </el-dropdown-item>
                    <el-dropdown-item command="cancelHot" :disabled="selectedRows.length === 0">
                      {{ t('product.list.batchCancelHot') }}
                    </el-dropdown-item>
                    <el-dropdown-item command="cancelNew" :disabled="selectedRows.length === 0">
                      {{ t('product.list.batchCancelNew') }}
                    </el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>
              <!-- Add button -->
              <el-button type="primary" @click="handleAdd">
                <el-icon><Plus /></el-icon>
                {{ t('product.list.add') }}
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
          <!-- Image column -->
          <template #image="{ row }">
            <el-image
              v-if="row.image"
              :src="row.image"
              :preview-src-list="[row.image]"
              fit="cover"
              style="width: 60px; height: 60px; border-radius: 4px"
            />
            <span v-else>-</span>
          </template>

          <!-- Price column -->
          <template #price="{ row }">
            <span class="price">{{ formatPrice(row.price) }}</span>
          </template>

          <!-- Audit status column -->
          <template #auditStatus="{ row }">
            <el-tag :type="getAuditStatusType(row.auditStatus)" size="small">
              {{ getAuditStatusLabel(row.auditStatus) }}
            </el-tag>
          </template>

          <!-- Tags column -->
          <template #tags="{ row }">
            <el-tag v-if="row.isHot" type="danger" size="small" style="margin-right: 4px">
              {{ t('product.list.isHot') }}
            </el-tag>
            <el-tag v-if="row.isNew" type="success" size="small">
              {{ t('product.list.isNew') }}
            </el-tag>
          </template>

          <!-- Operation column -->
          <template #operation>
            <el-table-column :label="t('product.list.operation')" width="200" fixed="right">
              <template #default="{ row }">
                <el-button link type="primary" @click="handleEdit(row)">
                  {{ t('product.list.edit') }}
                </el-button>
                <el-button link type="success" @click="handleReview(row)">
                  {{ t('product.review.title') }}
                </el-button>
                <el-button link type="danger" @click="handleDelete(row)">
                  {{ t('product.list.delete') }}
                </el-button>
              </template>
            </el-table-column>
          </template>
        </BaseTable>
      </el-card>
    </div>

    <!-- Batch category dialog -->
    <el-dialog
      v-model="batchCategoryVisible"
      :title="t('product.list.batchCategoryTitle')"
      width="400px"
    >
      <el-form label-width="80px">
        <el-form-item :label="t('product.list.category')">
          <el-select
            v-model="batchCategoryId"
            :placeholder="t('product.list.selectCategory')"
            style="width: 100%"
          >
            <el-option
              v-for="cat in categories"
              :key="cat.id"
              :label="cat.name"
              :value="cat.id"
            />
          </el-select>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="batchCategoryVisible = false">
          {{ t('common.button.cancel') }}
        </el-button>
        <el-button type="primary" :loading="batchLoading" @click="handleBatchCategoryConfirm">
          {{ t('common.button.confirm') }}
        </el-button>
      </template>
    </el-dialog>

    <!-- Import dialog -->
    <el-dialog
      v-model="importVisible"
      :title="t('product.list.importTitle')"
      width="500px"
    >
      <el-upload
        ref="uploadRef"
        drag
        accept=".xlsx,.xls"
        :auto-upload="false"
        :limit="1"
        :on-change="handleFileChange"
        :on-exceed="handleExceed"
      >
        <el-icon class="el-icon--upload"><UploadFilled /></el-icon>
        <div class="el-upload__text">
          {{ t('product.list.dragUploadTip') }}<em>{{ t('product.list.clickUpload') }}</em>
        </div>
        <template #tip>
          <div class="el-upload__tip">
            {{ t('product.list.uploadTip') }}
            <el-button type="primary" link @click="downloadTemplate">
              {{ t('product.list.downloadTemplate') }}
            </el-button>
          </div>
        </template>
      </el-upload>
      <template #footer>
        <el-button @click="importVisible = false">
          {{ t('common.button.cancel') }}
        </el-button>
        <el-button type="primary" :loading="importing" @click="handleImport">
          {{ t('common.button.confirm') }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Download, Upload, ArrowDown, UploadFilled } from '@element-plus/icons-vue'
import type { UploadFile } from 'element-plus'
import * as XLSX from 'xlsx'
import {
  getProductList,
  deleteProduct,
  deleteProductBatch,
  batchUpdateCategory,
  batchUpdateTag,
  batchCreateProducts,
} from '@/api/buz/productApi'
import { exportToExcel } from '@/utils/export'
import { useLocale } from '@/composables/useLocale'
import type { Product, Category, CreateProductParams } from '@/types/product'
import { ProductAuditStatus, ProductAuditStatusLabel, ProductAuditStatusType } from '@/types/businessAuditPoint'
import type { TableColumn } from '@/components/BaseTable/index.vue'
import BaseTable from '@/components/BaseTable/index.vue'
import SearchForm from '@/components/SearchForm/index.vue'
import CategoryTree from './components/CategoryTree.vue'

const router = useRouter()
const { t } = useLocale()

// Category tree ref
const categoryTreeRef = ref<InstanceType<typeof CategoryTree>>()
const baseTableRef = ref()
const uploadRef = ref()
const categories = ref<Category[]>([])

// 搜索表单配置
const searchItems = computed(() => [
  { field: 'skuCode', label: t('product.list.skuCode'), type: 'input' },
  { field: 'name', label: t('product.list.name'), type: 'input' },
  { field: 'auditStatus', label: t('product.audit.auditStatus'), type: 'select', options: [
    { label: t('product.audit.waitSubmit'), value: ProductAuditStatus.WaitSubmit },
    { label: t('product.audit.pending'), value: ProductAuditStatus.Pending },
    { label: t('product.audit.rejected'), value: ProductAuditStatus.Rejected },
    { label: t('product.audit.passed'), value: ProductAuditStatus.Passed },
    { label: t('product.audit.withdrawn'), value: ProductAuditStatus.Withdrawn },
  ]},
  { field: 'isHot', label: t('product.list.isHot'), type: 'select', options: [
    { label: t('common.button.yes'), value: true },
    { label: t('common.button.no'), value: false },
  ]},
  { field: 'isNew', label: t('product.list.isNew'), type: 'select', options: [
    { label: t('common.button.yes'), value: true },
    { label: t('common.button.no'), value: false },
  ]},
])

// Table columns
const tableColumns = ref<TableColumn[]>([
  { prop: 'image', label: t('product.list.image'), width: 80, align: 'center' },
  { prop: 'skuCode', label: t('product.list.skuCode'), width: 140 },
  { prop: 'name', label: t('product.list.name'), minWidth: 150 },
  { prop: 'categoryName', label: t('product.list.category'), width: 120 },
  { prop: 'auditStatus', label: t('product.audit.auditStatus'), width: 120, align: 'center' },
  { prop: 'price', label: t('product.list.price'), width: 100, align: 'right' },
  { prop: 'stock', label: t('product.list.stock'), width: 80, align: 'center' },
  { prop: 'sales', label: t('product.list.sales'), width: 80, align: 'center' },
  { prop: 'tags', label: t('product.list.isHot') + '/' + t('product.list.isNew'), width: 100, align: 'center' },
  { prop: 'updateTime', label: t('product.list.updateTime'), width: 160 },
])

// Export columns
const exportColumns: TableColumn[] = [
  { prop: 'skuCode', label: t('product.list.skuCode') },
  { prop: 'name', label: t('product.list.name') },
  { prop: 'categoryName', label: t('product.list.category') },
  { prop: 'price', label: t('product.list.price') },
  { prop: 'originalPrice', label: t('product.list.originalPrice') },
  { prop: 'stock', label: t('product.list.stock') },
  { prop: 'sales', label: t('product.list.sales') },
  { prop: 'isHot', label: t('product.list.isHot') },
  { prop: 'isNew', label: t('product.list.isNew') },
  { prop: 'updateTime', label: t('product.list.updateTime') },
]

const loading = ref(false)
const tableData = ref<Product[]>([])
const total = ref(0)
const selectedRows = ref<Product[]>([])

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  name: '',
  skuCode: '',
  categoryId: '' as string | undefined,
  auditStatus: undefined as number | undefined,
  isHot: undefined as boolean | undefined,
  isNew: undefined as boolean | undefined,
})

// Batch operation state
const batchCategoryVisible = ref(false)
const batchCategoryId = ref('')
const batchLoading = ref(false)

// Import state
const importVisible = ref(false)
const importing = ref(false)
const importFile = ref<File | null>(null)

onMounted(() => {
  handleSearch()
})

/**
 * Handle categories loaded from CategoryTree
 */
const handleCategoryLoaded = (data: Category[]) => {
  categories.value = data
}

/**
 * Format price to currency string
 */
const formatPrice = (price: number) => {
  return `¥${price.toFixed(2)}`
}

/**
 * Get audit status label
 */
const getAuditStatusLabel = (status: number | undefined) => {
  if (status === undefined) return ProductAuditStatusLabel[ProductAuditStatus.WaitSubmit]
  return ProductAuditStatusLabel[status] || '未知'
}

/**
 * Get audit status type for el-tag
 */
const getAuditStatusType = (status: number | undefined) => {
  if (status === undefined) return ProductAuditStatusType[ProductAuditStatus.WaitSubmit]
  return ProductAuditStatusType[status] || 'info'
}

/**
 * Load products from API
 */
const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getProductList(queryParams)
    // 直接使用后端返回的 categoryName
    tableData.value = data.list.map(product => ({
      ...product,
      categoryName: product.categoryName || '-',
    }))
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
  queryParams.categoryId = undefined
  queryParams.pageIndex = 1
  handleSearch()
}

/**
 * Handle category selection from sidebar
 */
const handleCategorySelect = (categoryId: string | null) => {
  queryParams.categoryId = categoryId || undefined
  queryParams.pageIndex = 1
  handleSearch()
}

/**
 * Handle table selection change
 */
const handleSelectionChange = (rows: Product[]) => {
  selectedRows.value = rows
}

/**
 * Navigate to add product page
 */
const handleAdd = () => {
  router.push('/product/edit')
}

/**
 * Navigate to edit product page
 */
const handleEdit = (row: Product) => {
  router.push(`/product/edit/${row.id}`)
}

/**
 * Navigate to review page for this product
 */
const handleReview = (row: Product) => {
  router.push({ path: '/product/review', query: { productId: row.id, productName: row.name } })
}

/**
 * Delete product with confirmation
 */
const handleDelete = async (row: Product) => {
  try {
    await ElMessageBox.confirm(
      t('product.list.deleteConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteProduct(row.id)
    ElMessage.success(t('product.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

// ==================== Batch Operations ====================

/**
 * Handle batch action command
 */
const handleBatchAction = async (command: string) => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning(t('product.list.noSelection'))
    return
  }

  const ids = selectedRows.value.map(row => row.id)

  switch (command) {
    case 'delete':
      await handleBatchDelete(ids)
      break
    case 'category':
      batchCategoryVisible.value = true
      break
    case 'hot':
      await handleBatchTagUpdate(ids, { isHot: true })
      break
    case 'new':
      await handleBatchTagUpdate(ids, { isNew: true })
      break
    case 'cancelHot':
      await handleBatchTagUpdate(ids, { isHot: false })
      break
    case 'cancelNew':
      await handleBatchTagUpdate(ids, { isNew: false })
      break
  }
}

/**
 * Batch delete products
 */
const handleBatchDelete = async (ids: string[]) => {
  try {
    await ElMessageBox.confirm(
      t('product.list.batchDeleteConfirm', { count: ids.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteProductBatch(ids)
    ElMessage.success(t('product.list.deleteSuccess'))
    baseTableRef.value?.clearSelection()
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

/**
 * Confirm batch category update
 */
const handleBatchCategoryConfirm = async () => {
  if (!batchCategoryId.value) {
    ElMessage.warning(t('product.list.selectCategory'))
    return
  }

  const ids = selectedRows.value.map(row => row.id)
  batchLoading.value = true

  try {
    const result = await batchUpdateCategory(ids, batchCategoryId.value)
    ElMessage.success(t('product.list.batchCategorySuccess', { count: result.count }))
    batchCategoryVisible.value = false
    batchCategoryId.value = ''
    baseTableRef.value?.clearSelection()
    handleSearch()
  } catch (error) {
    // Error handled by interceptor
  } finally {
    batchLoading.value = false
  }
}

/**
 * Batch update product tags
 */
const handleBatchTagUpdate = async (ids: string[], tags: { isHot?: boolean; isNew?: boolean }) => {
  try {
    const result = await batchUpdateTag(ids, tags)
    ElMessage.success(t('product.list.batchTagSuccess', { count: result.count }))
    baseTableRef.value?.clearSelection()
    handleSearch()
  } catch (error) {
    // Error handled by interceptor
  }
}

// ==================== Export ====================

/**
 * Handle export command
 */
const handleExport = async (type: string) => {
  let exportData: Product[] = []

  switch (type) {
    case 'current':
      exportData = tableData.value
      break
    case 'all':
      try {
        loading.value = true
        const data = await getProductList({ pageIndex: 1, pageSize: 9999 })
        exportData = data.list.map(product => ({
          ...product,
          categoryName: product.categoryName || '-',
        }))
      } catch (error) {
        ElMessage.error(t('product.list.exportFailed'))
        return
      } finally {
        loading.value = false
      }
      break
    case 'selected':
      exportData = selectedRows.value
      break
  }

  if (exportData.length === 0) {
    ElMessage.warning(t('product.list.exportNoData'))
    return
  }

  // Transform data for export
  const dataToExport = exportData.map(item => ({
    ...item,
    isHot: item.isHot ? t('common.button.yes') : t('common.button.no'),
    isNew: item.isNew ? t('common.button.yes') : t('common.button.no'),
  }))

  exportToExcel({
    columns: exportColumns,
    data: dataToExport,
    fileName: `商品列表_${new Date().toLocaleDateString().replace(/\//g, '-')}`,
    sheetName: '商品列表',
  })

  ElMessage.success(t('product.list.exportSuccess'))
}

// ==================== Import ====================

/**
 * Handle file change
 */
const handleFileChange = (file: UploadFile) => {
  if (file.raw) {
    importFile.value = file.raw
  }
}

/**
 * Handle exceed limit
 */
const handleExceed = () => {
  ElMessage.warning('只能上传一个文件')
}

/**
 * Download import template
 */
const downloadTemplate = () => {
  const templateData = [
    {
      '商品名称': '示例商品',
      '分类名称': '玫瑰',
      '价格': 100,
      '原价': 120,
      '库存': 50,
      '热销': '是',
      '新品': '否',
      '商品简介': '这是一个示例商品',
    },
  ]
  const ws = XLSX.utils.json_to_sheet(templateData)
  const wb = XLSX.utils.book_new()
  XLSX.utils.book_append_sheet(wb, ws, '商品导入模板')
  XLSX.writeFile(wb, '商品导入模板.xlsx')
}

/**
 * Handle import
 */
const handleImport = async () => {
  if (!importFile.value) {
    ElMessage.warning('请选择要导入的文件')
    return
  }

  importing.value = true

  try {
    const reader = new FileReader()
    reader.onload = async (e) => {
      try {
        const data = new Uint8Array(e.target?.result as ArrayBuffer)
        const workbook = XLSX.read(data, { type: 'array' })
        const sheet = workbook.Sheets[workbook.SheetNames[0]]
        const rows = XLSX.utils.sheet_to_json(sheet) as Record<string, any>[]

        if (rows.length === 0) {
          ElMessage.warning('文件中没有数据')
          importing.value = false
          return
        }

        // Build category name to id map
        const categoryNameToId = new Map<string, string>()
        categories.value.forEach(cat => {
          categoryNameToId.set(cat.name, cat.id)
        })

        // Transform rows to products
        const products: CreateProductParams[] = []
        const errors: string[] = []

        rows.forEach((row, index) => {
          const productName = row['商品名称']
          const categoryName = row['分类名称']

          if (!productName) {
            errors.push(`第 ${index + 1} 行: 商品名称不能为空`)
            return
          }

          const categoryId = categoryNameToId.get(categoryName)
          if (!categoryId) {
            errors.push(`第 ${index + 1} 行: 分类 "${categoryName}" 不存在`)
            return
          }

          products.push({
            name: String(productName),
            description: row['商品简介'] ? String(row['商品简介']) : '',
            price: Number(row['价格']) || 0,
            originalPrice: row['原价'] ? Number(row['原价']) : undefined,
            image: '/static/images/products/default.jpg',
            images: [],
            categoryId,
            stock: Number(row['库存']) || 0,
            isHot: row['热销'] === '是',
            isNew: row['新品'] === '是',
            detail: '',
          })
        })

        if (errors.length > 0) {
          ElMessage.warning(errors.slice(0, 5).join('\n'))
          importing.value = false
          return
        }

        // Batch create products
        const result = await batchCreateProducts(products)
        ElMessage.success(t('product.list.importSuccess', { count: result.count }))
        importVisible.value = false
        importFile.value = null
        uploadRef.value?.clearFiles()
        handleSearch()
      } catch (error) {
        ElMessage.error(t('product.list.importError'))
      } finally {
        importing.value = false
      }
    }
    reader.readAsArrayBuffer(importFile.value)
  } catch (error) {
    ElMessage.error(t('product.list.importFailed'))
    importing.value = false
  }
}
</script>

<style scoped lang="scss">
.product-container {
  display: flex;
  height: calc(100vh - 100px);
  padding: 20px;
  gap: 16px;

  .sidebar {
    width: 200px;
    flex-shrink: 0;
    overflow-y: auto;
  }

  .main-content {
    flex: 1;
    min-width: 0;
    overflow-y: auto;

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

    .price {
      color: #f56c6c;
      font-weight: 500;
    }
  }
}
</style>