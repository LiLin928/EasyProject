<!-- src/views/buz/order/OrderList.vue -->
<template>
  <div class="order-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('order.list.title') }}</span>
          <el-button type="primary" @click="handleCreate">
            <el-icon><Plus /></el-icon>
            {{ t('order.list.create') }}
          </el-button>
        </div>
      </template>

      <!-- Search form -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('order.list.orderNo')">
          <el-input
            v-model="queryParams.orderNo"
            :placeholder="t('order.list.orderNoPlaceholder')"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('order.list.user')">
          <el-input
            v-model="queryParams.userKeyword"
            :placeholder="t('order.list.userPlaceholder')"
            clearable
            style="width: 180px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('order.list.status')">
          <el-select
            v-model="queryParams.status"
            :placeholder="t('order.list.statusPlaceholder')"
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
        <el-form-item :label="t('order.list.createTime')">
          <el-date-picker
            v-model="timeRange"
            type="daterange"
            range-separator="-"
            :start-placeholder="t('common.date.startPlaceholder')"
            :end-placeholder="t('common.date.endPlaceholder')"
            value-format="YYYY-MM-DD"
            style="width: 240px"
            teleported
            :popper-options="{
              modifiers: [
                { name: 'flip', enabled: false },
                { name: 'preventOverflow', enabled: true, options: { padding: 8 } }
              ]
            }"
            @change="handleTimeRangeChange"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            {{ t('order.list.search') }}
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            {{ t('order.list.reset') }}
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
        <!-- User column -->
        <template #user="{ row }">
          <div class="user-info">
            <span>{{ row.userName }}</span>
            <span class="phone">{{ row.userPhone }}</span>
          </div>
        </template>

        <!-- Amount column -->
        <template #amount="{ row }">
          <span class="amount">{{ formatAmount(row.totalAmount) }}</span>
        </template>

        <!-- Status column -->
        <template #status="{ row }">
          <el-tag :type="getStatusType(row.status)">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>

        <!-- Operation column -->
        <template #operation>
          <el-table-column :label="t('order.list.operation')" width="180" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleDetail(row)">
                {{ t('order.list.detail') }}
              </el-button>
              <el-button
                v-if="row.status === 1"
                link
                type="success"
                @click="handleShip(row)"
              >
                {{ t('order.list.ship') }}
              </el-button>
              <el-button
                v-if="row.status === 0"
                link
                type="danger"
                @click="handleCancel(row)"
              >
                {{ t('order.list.cancel') }}
              </el-button>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>

    <!-- Order detail dialog -->
    <OrderDetailDialog
      v-model:visible="detailVisible"
      :order-id="currentOrderId"
      @success="handleSearch"
    />

    <!-- Ship dialog -->
    <ShipDialog
      v-model:visible="shipVisible"
      :order-id="currentOrderId"
      @success="handleShipSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Refresh, Plus } from '@element-plus/icons-vue'
import { getOrderList, cancelOrder } from '@/api/buz/orderApi'
import { useLocale } from '@/composables/useLocale'
import type { Order, OrderStatus } from '@/types/order'
import type { TableColumn } from '@/components/BaseTable/index.vue'
import BaseTable from '@/components/BaseTable/index.vue'
import OrderDetailDialog from './components/OrderDetailDialog.vue'
import ShipDialog from './components/ShipDialog.vue'

const { t } = useLocale()
const router = useRouter()

// 状态映射：数字 -> 字符串
const STATUS_MAP: Record<number, OrderStatus> = {
  0: 'pending',
  1: 'paid',
  2: 'shipped',
  3: 'completed',
  4: 'cancelled',
}

// 状态映射：字符串 -> 数字
const STATUS_VALUE_MAP: Record<OrderStatus, number> = {
  pending: 0,
  paid: 1,
  shipped: 2,
  completed: 3,
  cancelled: 4,
}

// Status options for select
const statusOptions = computed(() => [
  { value: 'pending', label: t('order.status.pending') },
  { value: 'paid', label: t('order.status.paid') },
  { value: 'shipped', label: t('order.status.shipped') },
  { value: 'completed', label: t('order.status.completed') },
  { value: 'cancelled', label: t('order.status.cancelled') },
])

// Table columns
const tableColumns = ref<TableColumn[]>([
  { prop: 'orderNo', label: t('order.list.orderNo'), width: 180 },
  { prop: 'user', label: t('order.list.user'), width: 150 },
  { prop: 'receiverName', label: t('order.list.receiver'), width: 100 },
  { prop: 'itemCount', label: t('order.list.itemCount'), width: 100, align: 'center' },
  { prop: 'amount', label: t('order.list.amount'), width: 120, align: 'right' },
  { prop: 'status', label: t('order.list.status'), width: 100, align: 'center' },
  { prop: 'createTime', label: t('order.list.createTime'), width: 160 },
])

const loading = ref(false)
const tableData = ref<Order[]>([])
const total = ref(0)
const baseTableRef = ref()

// Time range
const timeRange = ref<[string, string] | null>(null)

// Query params
let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  orderNo: '',
  userKeyword: '',
  status: '' as OrderStatus | '',
  startTime: '',
  endTime: '',
})

// Dialog state
const detailVisible = ref(false)
const shipVisible = ref(false)
const currentOrderId = ref('')

onMounted(() => {
  handleSearch()
})

/**
 * Format amount to currency string
 */
const formatAmount = (amount: number) => {
  return `¥${amount.toFixed(2)}`
}

/**
 * 获取状态标签类型
 */
const getStatusType = (status: number | OrderStatus) => {
  // 转换数字状态为字符串
  const statusKey = typeof status === 'number' ? STATUS_MAP[status] : status
  const types: Record<OrderStatus, string> = {
    pending: 'warning',
    paid: 'success',
    shipped: 'primary',
    completed: 'info',
    cancelled: 'danger',
  }
  return types[statusKey] || 'info'
}

/**
 * 获取状态文本
 */
const getStatusText = (status: number | OrderStatus) => {
  // 转换数字状态为字符串
  const statusKey = typeof status === 'number' ? STATUS_MAP[status] : status
  return t(`order.status.${statusKey}`)
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
 * Load orders from API
 */
const handleSearch = async () => {
  loading.value = true
  try {
    // 将字符串状态转换为数字状态
    let statusValue: number | undefined = undefined
    if (queryParams.status) {
      statusValue = STATUS_VALUE_MAP[queryParams.status]
    }
    const params = {
      pageIndex: queryParams.pageIndex,
      pageSize: queryParams.pageSize,
      orderNo: queryParams.orderNo || undefined,
      userKeyword: queryParams.userKeyword || undefined,
      status: statusValue,
      startTime: queryParams.startTime || undefined,
      endTime: queryParams.endTime || undefined,
    }
    const data = await getOrderList(params)
    // Add itemCount for each order
    tableData.value = data.list.map(order => ({
      ...order,
      itemCount: order.items?.length || 0,
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
  queryParams.orderNo = ''
  queryParams.userKeyword = ''
  queryParams.status = ''
  queryParams.startTime = ''
  queryParams.endTime = ''
  timeRange.value = null
  queryParams.pageIndex = 1
  handleSearch()
}

/**
 * View order detail
 */
const handleDetail = (row: Order) => {
  currentOrderId.value = row.id
  detailVisible.value = true
}

/**
 * Ship order
 */
const handleShip = (row: Order) => {
  currentOrderId.value = row.id
  shipVisible.value = true
}

/**
 * Handle ship success
 */
const handleShipSuccess = () => {
  ElMessage.success(t('order.list.shipSuccess'))
  handleSearch()
}

/**
 * Cancel order
 */
const handleCancel = async (row: Order) => {
  try {
    await ElMessageBox.confirm(
      t('order.list.cancelConfirm'),
      t('common.message.warning'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await cancelOrder(row.id)
    ElMessage.success(t('order.list.cancelSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

/**
 * Create new order
 */
const handleCreate = () => {
  router.push('/order/create')
}
</script>

<style scoped lang="scss">
.order-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .search-form {
    margin-bottom: 16px;
  }

  .user-info {
    display: flex;
    flex-direction: column;
    line-height: 1.4;

    .phone {
      font-size: 12px;
      color: #909399;
    }
  }

  .amount {
    color: #f56c6c;
    font-weight: 500;
  }
}
</style>