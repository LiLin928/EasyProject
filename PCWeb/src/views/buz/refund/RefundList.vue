<!-- src/views/buz/refund/RefundList.vue -->
<template>
  <div class="refund-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('refund.list.title') }}</span>
        </div>
      </template>

      <!-- Search form -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('refund.list.refundNo')">
          <el-input
            v-model="queryParams.refundNo"
            :placeholder="t('refund.list.refundNoPlaceholder')"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('refund.list.orderNo')">
          <el-input
            v-model="queryParams.orderNo"
            :placeholder="t('refund.list.orderNoPlaceholder')"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('refund.list.type')">
          <el-select
            v-model="queryParams.type"
            :placeholder="t('refund.list.typePlaceholder')"
            clearable
            style="width: 140px"
          >
            <el-option
              v-for="item in typeOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('refund.list.status')">
          <el-select
            v-model="queryParams.status"
            :placeholder="t('refund.list.statusPlaceholder')"
            clearable
            style="width: 140px"
          >
            <el-option
              v-for="item in statusOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('refund.list.createTime')">
          <el-date-picker
            v-model="timeRange"
            type="daterange"
            range-separator="-"
            :start-placeholder="t('common.date.startDate')"
            :end-placeholder="t('common.date.endDate')"
            value-format="YYYY-MM-DD"
            style="width: 240px"
            @change="handleTimeRangeChange"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            {{ t('refund.list.search') }}
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            {{ t('refund.list.reset') }}
          </el-button>
        </el-form-item>
      </el-form>

      <!-- Table -->
      <BaseTable
        ref="baseTableRef"
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
          <el-tag :type="getTypeTagType(row.type)">
            {{ t(`refund.type.${row.type}`) }}
          </el-tag>
        </template>

        <!-- Amount column -->
        <template #refundAmount="{ row }">
          <span v-if="row.type === 'exchange'">-</span>
          <span v-else class="refund-amount">{{ formatAmount(row.refundAmount) }}</span>
        </template>

        <!-- Return ship column -->
        <template #returnShip="{ row }">
          <span v-if="row.returnShipNo">
            {{ getShipCompanyName(row.returnShipCompany) }} {{ row.returnShipNo }}
          </span>
          <span v-else>-</span>
        </template>

        <!-- Reason column -->
        <template #reason="{ row }">
          <span>{{ getReasonLabel(row.reason) }}</span>
        </template>

        <!-- Status column -->
        <template #status="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ t(`refund.status.${row.status}`) }}
          </el-tag>
        </template>

        <!-- Operation column -->
        <template #operation>
          <el-table-column :label="t('refund.list.operation')" width="120" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleDetail(row)">
                {{ t('refund.list.detail') }}
              </el-button>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>

    <!-- Refund detail dialog -->
    <RefundDetailDialog
      v-model:visible="detailVisible"
      :refund-id="currentRefundId"
      @success="handleDetailSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { Search, Refresh } from '@element-plus/icons-vue'
import { getRefundList } from '@/api/buz/refundApi'
import { useLocale } from '@/composables/useLocale'
import { SHIP_COMPANIES } from '@/types/order'
import type { Refund, RefundStatus, RefundType } from '@/types/order'
import type { TableColumn } from '@/components/BaseTable/index.vue'
import BaseTable from '@/components/BaseTable/index.vue'
import RefundDetailDialog from './components/RefundDetailDialog.vue'

const { t } = useLocale()

// Type options for select
const typeOptions = computed(() => [
  { value: 'refund_only', label: t('refund.type.refund_only') },
  { value: 'return_refund', label: t('refund.type.return_refund') },
  { value: 'exchange', label: t('refund.type.exchange') },
])

// Status options for select (扩展)
const statusOptions = computed(() => [
  { value: 'pending', label: t('refund.status.pending') },
  { value: 'approved', label: t('refund.status.approved') },
  { value: 'returning', label: t('refund.status.returning') },
  { value: 'received', label: t('refund.status.received') },
  { value: 'shipped', label: t('refund.status.shipped') },
  { value: 'refunded', label: t('refund.status.refunded') },
  { value: 'completed', label: t('refund.status.completed') },
  { value: 'rejected', label: t('refund.status.rejected') },
])

// Table columns (扩展)
const tableColumns = ref<TableColumn[]>([
  { prop: 'refundNo', label: t('refund.list.refundNo'), width: 180 },
  { prop: 'orderNo', label: t('refund.list.orderNo'), width: 180 },
  { prop: 'type', label: t('refund.list.type'), width: 100, align: 'center' },
  { prop: 'refundAmount', label: t('refund.list.amount'), width: 120, align: 'right' },
  { prop: 'reason', label: t('refund.list.reason'), minWidth: 150 },
  { prop: 'returnShip', label: t('refund.list.returnShip'), width: 180 },
  { prop: 'status', label: t('refund.list.status'), width: 100, align: 'center' },
  { prop: 'createTime', label: t('refund.list.createTime'), width: 160 },
])

const loading = ref(false)
const tableData = ref<Refund[]>([])
const total = ref(0)
const baseTableRef = ref()

// Time range
const timeRange = ref<[string, string] | null>(null)

// Query params (扩展)
let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  refundNo: '',
  orderNo: '',
  type: '' as RefundType | '',
  status: '' as RefundStatus | '',
  startTime: '',
  endTime: '',
})

// Dialog state
const detailVisible = ref(false)
const currentRefundId = ref('')

onMounted(() => {
  handleSearch()
})

/**
 * Format amount to currency string
 */
const formatAmount = (amount: number) => {
  return `\u00A5${amount.toFixed(2)}`
}

/**
 * Get type tag type
 */
const getTypeTagType = (type: RefundType) => {
  const types: Record<RefundType, string> = {
    refund_only: 'info',
    return_refund: 'warning',
    exchange: 'success',
  }
  return types[type] || 'info'
}

/**
 * Get status tag type (扩展)
 */
const getStatusType = (status: RefundStatus) => {
  const types: Record<RefundStatus, string> = {
    pending: 'warning',
    approved: 'primary',
    returning: 'primary',
    received: 'primary',
    shipped: 'success',
    refunded: 'info',
    completed: 'success',
    rejected: 'danger',
  }
  return types[status] || 'info'
}

/**
 * Get ship company name
 */
const getShipCompanyName = (company?: string) => {
  if (!company) return ''
  const found = SHIP_COMPANIES.find(c => c.value === company)
  return found ? found.label : company
}

/**
 * Get reason label
 */
const reasonMap: Record<string, string> = {
  quality: 'refund.reasons.quality',
  damage: 'refund.reasons.damage',
  wrong: 'refund.reasons.wrong',
  notMatch: 'refund.reasons.notMatch',
  other: 'refund.reasons.other',
}

const getReasonLabel = (reason?: string) => {
  if (!reason) return '-'
  const key = reasonMap[reason]
  return key ? t(key) : reason
}

/**
 * Handle time range change
 */
const handleTimeRangeChange = (value: [string, string] | null) => {
  if (value) {
    queryParams.startTime = value[0]
    queryParams.endTime = value[1]
  } else {
    queryParams.startTime = ''
    queryParams.endTime = ''
  }
}

/**
 * Load refunds from API
 */
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      pageIndex: queryParams.pageIndex,
      pageSize: queryParams.pageSize,
      refundNo: queryParams.refundNo || undefined,
      orderNo: queryParams.orderNo || undefined,
      type: queryParams.type || undefined,
      status: queryParams.status || undefined,
      startTime: queryParams.startTime || undefined,
      endTime: queryParams.endTime || undefined,
    }
    const data = await getRefundList(params)
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
  queryParams.refundNo = ''
  queryParams.orderNo = ''
  queryParams.type = ''
  queryParams.status = ''
  queryParams.startTime = ''
  queryParams.endTime = ''
  timeRange.value = null
  queryParams.pageIndex = 1
  handleSearch()
}

/**
 * View refund detail
 */
const handleDetail = (row: Refund) => {
  currentRefundId.value = row.id
  detailVisible.value = true
}

/**
 * Handle detail dialog success
 */
const handleDetailSuccess = () => {
  handleSearch()
}
</script>

<style scoped lang="scss">
.refund-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .search-form {
    margin-bottom: 16px;
  }

  .refund-amount {
    color: #f56c6c;
    font-weight: 500;
  }
}
</style>