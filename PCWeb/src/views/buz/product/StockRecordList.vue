<!-- src/views/buz/product/StockRecordList.vue -->
<template>
  <div class="stock-record-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('product.stock.recordTitle') }}</span>
        </div>
      </template>

      <!-- Search form -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('product.stock.productName')">
          <el-input
            v-model="queryParams.productName"
            :placeholder="t('product.stock.productNamePlaceholder')"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item :label="t('product.stock.changeType')">
          <el-select
            v-model="queryParams.type"
            :placeholder="t('product.stock.selectType')"
            clearable
            style="width: 120px"
          >
            <el-option :label="t('product.stockType.in')" value="in" />
            <el-option :label="t('product.stockType.out')" value="out" />
            <el-option :label="t('product.stockType.adjust')" value="adjust" />
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
        <!-- Type column -->
        <template #type="{ row }">
          <el-tag :type="getTypeTagType(row.type)" size="small">
            {{ t(`product.stockType.${row.type}`) }}
          </el-tag>
        </template>

        <!-- Quantity column -->
        <template #quantity="{ row }">
          <span :class="row.quantity >= 0 ? 'quantity-in' : 'quantity-out'">
            {{ row.quantity >= 0 ? '+' : '' }}{{ row.quantity }}
          </span>
        </template>

        <!-- Purchase price column -->
        <template #purchasePrice="{ row }">
          <span v-if="row.purchasePrice">¥{{ row.purchasePrice.toFixed(2) }}</span>
          <span v-else>-</span>
        </template>
      </BaseTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { Search, Refresh } from '@element-plus/icons-vue'
import { getStockRecords } from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { StockRecord, StockChangeType } from '@/types/product'
import type { TableColumn } from '@/components/BaseTable/index.vue'
import BaseTable from '@/components/BaseTable/index.vue'

const route = useRoute()
const { t } = useLocale()

const loading = ref(false)
const tableData = ref<StockRecord[]>([])
const total = ref(0)

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  productId: route.query.productId as string || '',
  productName: '',
  type: undefined as StockChangeType | undefined,
})

// Table columns
const tableColumns = ref<TableColumn[]>([
  { prop: 'productName', label: t('product.stock.productName'), minWidth: 150 },
  { prop: 'skuCode', label: t('product.stock.skuCode'), width: 140 },
  { prop: 'type', label: t('product.stock.changeType'), width: 100, align: 'center' },
  { prop: 'quantity', label: t('product.stock.changeQuantity'), width: 100, align: 'center' },
  { prop: 'beforeStock', label: t('product.stock.beforeStock'), width: 100, align: 'center' },
  { prop: 'afterStock', label: t('product.stock.afterStock'), width: 100, align: 'center' },
  { prop: 'supplierName', label: t('product.stock.selectSupplier'), width: 120 },
  { prop: 'purchasePrice', label: t('product.stock.purchasePrice'), width: 100, align: 'right' },
  { prop: 'operator', label: t('product.stock.operator'), width: 100 },
  { prop: 'createTime', label: t('product.stock.operateTime'), width: 160 },
  { prop: 'remark', label: t('product.stock.remark'), minWidth: 150 },
])

onMounted(() => {
  handleSearch()
})

/**
 * Load stock records
 */
const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getStockRecords(queryParams)
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
  queryParams.productId = ''
  queryParams.productName = ''
  queryParams.type = undefined
  queryParams.pageIndex = 1
  handleSearch()
}

/**
 * Get tag type for stock change type
 */
const getTypeTagType = (type: StockChangeType) => {
  switch (type) {
    case 'in':
      return 'success'
    case 'out':
      return 'danger'
    case 'adjust':
      return 'warning'
    default:
      return ''
  }
}
</script>

<style scoped lang="scss">
.stock-record-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .search-form {
    margin-bottom: 16px;
  }

  .quantity-in {
    color: var(--el-color-success);
    font-weight: 600;
  }

  .quantity-out {
    color: var(--el-color-danger);
    font-weight: 600;
  }
}
</style>