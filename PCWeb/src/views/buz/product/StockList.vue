<!-- src/views/buz/product/StockList.vue -->
<template>
  <div class="stock-list-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('product.stock.title') }}</span>
          <el-button type="primary" @click="stockInDialogVisible = true">
            <el-icon><Plus /></el-icon>
            新增入库
          </el-button>
        </div>
      </template>

      <!-- Search form -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('product.stock.productName')">
          <el-input
            v-model="queryParams.name"
            :placeholder="t('product.list.namePlaceholder')"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('product.list.category')">
          <el-select
            v-model="queryParams.categoryId"
            :placeholder="t('product.list.categoryPlaceholder')"
            clearable
            style="width: 150px"
          >
            <el-option
              v-for="cat in categories"
              :key="cat.id"
              :label="cat.name"
              :value="cat.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('product.stock.stockStatus')">
          <el-select
            v-model="queryParams.lowStock"
            :placeholder="t('common.button.select')"
            clearable
            style="width: 120px"
          >
            <el-option :label="t('product.stock.allProducts')" :value="undefined" />
            <el-option :label="t('product.stock.lowStockOnly')" :value="true" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            {{ t('product.list.search') }}
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            {{ t('common.button.reset') }}
          </el-button>
        </el-form-item>
      </el-form>

      <!-- Table -->
      <BaseTable
        :data="tableData"
        :columns="tableColumns"
        :loading="loading"
        :total="total"
        :page-index="queryParams.pageIndex"
        :page-size="queryParams.pageSize"
        @update:page-index="queryParams.pageIndex = $event"
        @update:page-size="queryParams.pageSize = $event"
        @page-change="handleSearch"
      >
        <!-- Image column -->
        <template #productImage="{ row }">
          <el-image
            v-if="row.productImage"
            :src="row.productImage"
            :preview-src-list="[row.productImage]"
            fit="cover"
            style="width: 60px; height: 60px; border-radius: 4px"
          />
          <span v-else>-</span>
        </template>

        <!-- Stock column with warning -->
        <template #stock="{ row }">
          <span :class="{ 'low-stock': row.isLowStock }">
            {{ row.stock }}
            <el-icon v-if="row.isLowStock" class="warning-icon"><Warning /></el-icon>
          </span>
        </template>

        <!-- Status column -->
        <template #status="{ row }">
          <el-tag v-if="row.isLowStock" type="danger" size="small">
            {{ t('product.stock.lowStock') }}
          </el-tag>
          <el-tag v-else type="success" size="small">
            {{ t('product.stock.normalStock') }}
          </el-tag>
        </template>

        <!-- Operation column -->
        <template #operation>
          <el-table-column :label="t('product.list.operation')" width="180" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleAdjustStock(row)">
                {{ t('product.stock.adjustStock') }}
              </el-button>
              <el-button link type="primary" @click="handleViewRecords(row)">
                {{ t('product.stock.viewRecords') }}
              </el-button>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>

    <!-- Stock adjust dialog -->
    <StockAdjustDialog
      v-model="adjustDialogVisible"
      :product="currentProduct"
      @success="handleSearch"
    />

    <!-- Stock in dialog -->
    <StockInDialog
      v-model="stockInDialogVisible"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { Plus, Search, Refresh, Warning } from '@element-plus/icons-vue'
import { getStockList, getCategoryList } from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { Stock, Category } from '@/types/product'
import type { TableColumn } from '@/components/BaseTable/index.vue'
import BaseTable from '@/components/BaseTable/index.vue'
import StockAdjustDialog from './components/StockAdjustDialog.vue'
import StockInDialog from './components/StockInDialog.vue'

const router = useRouter()
const { t } = useLocale()

const categories = ref<Category[]>([])
const loading = ref(false)
const tableData = ref<Stock[]>([])
const total = ref(0)
const adjustDialogVisible = ref(false)
const stockInDialogVisible = ref(false)
const currentProduct = ref<Stock | null>(null)

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  name: '',
  categoryId: '',
  lowStock: undefined as boolean | undefined,
})

// Table columns
const tableColumns = ref<TableColumn[]>([
  { prop: 'productImage', label: t('product.list.image'), width: 80, align: 'center' },
  { prop: 'productName', label: t('product.stock.productName'), minWidth: 150 },
  { prop: 'categoryName', label: t('product.list.category'), width: 120 },
  { prop: 'stock', label: t('product.stock.currentStock'), width: 100, align: 'center' },
  { prop: 'alertThreshold', label: t('product.stock.alertThreshold'), width: 100, align: 'center' },
  { prop: 'status', label: t('product.stock.stockStatus'), width: 100, align: 'center' },
  { prop: 'updateTime', label: t('product.list.updateTime'), width: 160 },
])

onMounted(() => {
  loadCategories()
  handleSearch()
})

/**
 * Load categories for select
 */
const loadCategories = async () => {
  try {
    const data = await getCategoryList()
    categories.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

/**
 * Load stock list
 */
const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getStockList(queryParams)
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
  queryParams.name = ''
  queryParams.categoryId = ''
  queryParams.lowStock = undefined
  queryParams.pageIndex = 1
  handleSearch()
}

/**
 * Open adjust stock dialog
 */
const handleAdjustStock = (row: Stock) => {
  currentProduct.value = row
  adjustDialogVisible.value = true
}

/**
 * View stock records for a product
 */
const handleViewRecords = (row: Stock) => {
  router.push({
    path: '/buz/product/stock-record',
    query: { productId: row.productId },
  })
}
</script>

<style scoped lang="scss">
.stock-list-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .search-form {
    margin-bottom: 16px;
  }

  .low-stock {
    color: var(--el-color-danger);
    font-weight: 600;

    .warning-icon {
      margin-left: 4px;
      vertical-align: middle;
    }
  }
}
</style>