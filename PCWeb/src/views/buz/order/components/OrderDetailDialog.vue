<template>
  <el-dialog
    v-model="dialogVisible"
    width="800px"
    destroy-on-close
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('order.detail.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="dialogVisible = false">{{ t('common.button.close') }}</el-button>
          <!-- 确认收货按钮 -->
          <el-button
            v-if="canConfirmReceive"
            type="success"
            @click="handleConfirmReceive"
          >
            <el-icon><Check /></el-icon>
            {{ t('order.detail.confirmReceive') }}
          </el-button>
          <!-- 评价按钮 -->
          <el-button
            v-if="canReview"
            type="primary"
            @click="handleReview"
          >
            <el-icon><Edit /></el-icon>
            {{ t('order.detail.review') }}
          </el-button>
          <!-- 申请售后按钮 -->
          <el-button
            v-if="canCreateRefund"
            type="warning"
            :disabled="allItemsInRefund"
            @click="handleCreateRefund"
          >
            {{ t('order.detail.createRefund') }}
            <span v-if="allItemsInRefund" style="font-size: 12px; margin-left: 4px;">
              (全部已申请)
            </span>
          </el-button>
        </div>
      </div>
    </template>
    <div v-loading="loading" class="order-detail">
      <!-- 基本信息 -->
      <div class="section">
        <div class="section-title">{{ t('order.detail.basicInfo') }}</div>
        <el-descriptions :column="2" border>
          <el-descriptions-item :label="t('order.detail.orderNo')">
            {{ order?.orderNo }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('order.detail.createTime')">
            {{ order?.createTime }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('order.detail.payTime')">
            {{ order?.payTime || '-' }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('order.detail.payMethod')">
            {{ getPayMethodLabel(order?.payMethod) }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('order.list.status')">
            <el-tag :type="getStatusType(getStatusString(order?.status))">
              {{ t(`order.status.${getStatusString(order?.status)}`) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item v-if="order?.hasRefund" label="售后状态">
            <el-tag type="warning">有售后记录</el-tag>
          </el-descriptions-item>
          <!-- 确认收货时间 -->
          <el-descriptions-item v-if="order?.confirmTime" :label="t('order.detail.confirmTime')">
            {{ order?.confirmTime }}
          </el-descriptions-item>
          <!-- 评价状态 -->
          <el-descriptions-item v-if="order?.hasReview" :label="t('order.detail.reviewStatus')">
            <el-tag type="success">{{ t('order.detail.reviewed') }}</el-tag>
          </el-descriptions-item>
          <!-- 积分信息 -->
          <el-descriptions-item v-if="order?.pointsEarned" :label="t('order.detail.pointsEarned')">
            <span class="points-amount">+{{ order?.pointsEarned }} {{ t('order.points.unit') }}</span>
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <!-- 用户信息 -->
      <div class="section">
        <div class="section-title">{{ t('order.detail.userInfo') }}</div>
        <el-descriptions :column="2" border>
          <el-descriptions-item :label="t('order.detail.userName')">
            {{ order?.userName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('order.detail.userPhone')">
            {{ order?.userPhone }}
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <!-- 收货信息 -->
      <div class="section">
        <div class="section-title">{{ t('order.detail.receiverInfo') }}</div>
        <el-descriptions :column="2" border>
          <el-descriptions-item :label="t('order.detail.receiverName')">
            {{ order?.receiverName }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('order.detail.receiverPhone')">
            {{ order?.receiverPhone }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('order.detail.receiverAddress')" :span="2">
            {{ order?.receiverAddress }}
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <!-- 商品列表 -->
      <div class="section">
        <div class="section-title">{{ t('order.detail.productList') }}</div>
        <el-table :data="order?.items || []" border>
          <el-table-column width="80">
            <template #default="{ row }">
              <el-image
                :src="row.productImage"
                :preview-src-list="[row.productImage]"
                fit="cover"
                class="product-image"
              />
            </template>
          </el-table-column>
          <el-table-column :label="t('order.detail.productName')" prop="productName" />
          <el-table-column :label="t('order.detail.price')" width="120">
            <template #default="{ row }">
              ¥{{ (row.price ?? 0).toFixed(2) }}
            </template>
          </el-table-column>
          <el-table-column :label="t('order.detail.quantity')" prop="quantity" width="80" />
          <el-table-column :label="t('order.detail.subtotal')" width="120">
            <template #default="{ row }">
              ¥{{ (row.amount ?? 0).toFixed(2) }}
            </template>
          </el-table-column>
          <el-table-column label="售后状态" width="100">
            <template #default="{ row }">
              <el-tag v-if="isItemInRefund(row.id)" type="warning" size="small">
                已申请
              </el-tag>
              <span v-else>-</span>
            </template>
          </el-table-column>
        </el-table>
      </div>

      <!-- 金额信息 -->
      <div class="section">
        <div class="section-title">{{ t('order.detail.amountInfo') }}</div>
        <el-descriptions :column="2" border>
          <el-descriptions-item :label="t('order.detail.totalAmount')">
            ¥{{ (order?.totalAmount ?? 0).toFixed(2) }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('order.detail.payAmount')">
            <span class="pay-amount">¥{{ (order?.payAmount ?? order?.totalAmount ?? 0).toFixed(2) }}</span>
          </el-descriptions-item>
          <el-descriptions-item v-if="order?.refundAmount" label="已退款金额">
            <span class="refund-amount">¥{{ (order?.refundAmount ?? 0).toFixed(2) }}</span>
          </el-descriptions-item>
          <el-descriptions-item v-if="order?.refundCount" label="售后次数">
            {{ order?.refundCount }} 次
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <!-- 物流信息 - 已发货/已完成时显示 -->
      <div v-if="showLogistics" class="section">
        <div class="section-title">{{ t('order.detail.logistics') }}</div>
        <el-descriptions :column="3" border>
          <el-descriptions-item :label="t('order.detail.shipTime')">
            {{ order?.deliveryTime }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('order.detail.shipCompany')">
            {{ getShipCompanyLabel(order?.logisticsCompany) }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('order.detail.shipNo')">
            {{ order?.logisticsNumber }}
          </el-descriptions-item>
          <!-- 自动确认时间 -->
          <el-descriptions-item v-if="order?.autoConfirmTime" :label="t('order.detail.autoConfirmTime')">
            {{ order?.autoConfirmTime }}
          </el-descriptions-item>
        </el-descriptions>
        <!-- 查看物流轨迹按钮 -->
        <div class="logistics-action">
          <el-button type="primary" link @click="handleViewShipTrack">
            <el-icon><Location /></el-icon>
            {{ t('order.detail.viewShipTrack') }}
          </el-button>
        </div>
      </div>

      <!-- 售后记录 - 有售后记录时显示 -->
      <div v-if="refundRecords.length > 0" class="section">
        <div class="section-title">售后记录</div>
        <el-table :data="refundRecords" border size="small">
          <el-table-column label="售后编号" prop="refundNo" width="180" />
          <el-table-column label="售后类型" width="100">
            <template #default="{ row }">
              <el-tag :type="getRefundTypeTagType(row.type)" size="small">
                {{ t(`refund.type.${row.type}`) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="状态" width="100">
            <template #default="{ row }">
              <el-tag :type="getRefundStatusTagType(row.status)" size="small">
                {{ t(`refund.status.${row.status}`) }}
              </el-tag>
            </template>
          </el-table-column>
          <el-table-column label="退款金额" width="120">
            <template #default="{ row }">
              <span v-if="row.type === 'exchange'">-</span>
              <span v-else class="refund-amount">¥{{ (row.refundAmount ?? 0).toFixed(2) }}</span>
            </template>
          </el-table-column>
          <el-table-column label="申请时间" prop="createTime" width="160" />
          <el-table-column label="操作" width="80">
            <template #default="{ row }">
              <el-button link type="primary" size="small" @click="handleViewRefundDetail(row)">
                查看
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>
  </el-dialog>

  <!-- 退款申请对话框 -->
  <RefundCreateDialog
    v-model:visible="refundCreateVisible"
    :order-id="orderId"
    @success="handleRefundSuccess"
  />

  <!-- 售后详情弹窗 -->
  <RefundDetailDialog
    v-model:visible="refundDetailVisible"
    :refund-id="currentRefundId"
    @success="handleRefundDetailSuccess"
  />

  <!-- 评价弹窗 -->
  <ReviewDialog
    v-model:visible="reviewVisible"
    :order-id="orderId"
    @success="handleReviewSuccess"
  />

  <!-- 物流轨迹弹窗 -->
  <ShipTrackDialog
    v-model:visible="shipTrackVisible"
    :order-id="orderId"
  />
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Check, Edit, Location } from '@element-plus/icons-vue'
import { getOrderDetail, confirmReceive } from '@/api/buz/orderApi'
import { getRefundList } from '@/api/buz/refundApi'
import type { Order, OrderStatus, RefundType, RefundStatus, Refund } from '@/types/order'
import { SHIP_COMPANIES } from '@/types/order'
import RefundCreateDialog from './RefundCreateDialog.vue'
import RefundDetailDialog from '@/views/buz/refund/components/RefundDetailDialog.vue'
import ReviewDialog from './ReviewDialog.vue'
import ShipTrackDialog from './ShipTrackDialog.vue'

interface RefundRecord {
  id: string
  refundNo: string
  type: RefundType
  status: RefundStatus
  refundAmount: number
  createTime: string
}

const { t } = useI18n()

// 状态映射：数字 -> 字符串
const STATUS_MAP: Record<number, OrderStatus> = {
  0: 'pending',
  1: 'paid',
  2: 'shipped',
  3: 'completed',
  4: 'cancelled',
  5: 'refunded',
}

const props = defineProps<{
  visible: boolean
  orderId: string
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const loading = ref(false)
const order = ref<Order | null>(null)
const refundRecords = ref<RefundRecord[]>([])
const refundCreateVisible = ref(false)
const refundDetailVisible = ref(false)
const currentRefundId = ref('')
const reviewVisible = ref(false)
const shipTrackVisible = ref(false)
// 记录已申请售后的商品项 ID
const refundedItemIds = ref<Set<string>>(new Set())

const dialogVisible = computed({
  get: () => props.visible,
  set: (value) => emit('update:visible', value),
})

// 获取字符串状态（兼容数字和字符串）
const getStatusString = (status: number | OrderStatus): OrderStatus => {
  if (typeof status === 'number') {
    return STATUS_MAP[status] || 'pending'
  }
  return status
}

const showLogistics = computed(() => {
  if (!order.value) return false
  const status = getStatusString(order.value.status)
  return status === 'shipped' || status === 'completed'
})

const canConfirmReceive = computed(() => {
  if (!order.value) return false
  return getStatusString(order.value.status) === 'shipped'
})

const canReview = computed(() => {
  if (!order.value) return false
  return getStatusString(order.value.status) === 'completed' && !order.value.hasReview
})

const canCreateRefund = computed(() => {
  if (!order.value) return false
  const status = getStatusString(order.value.status)
  return ['paid', 'shipped', 'completed'].includes(status)
})

const allItemsInRefund = computed(() => {
  // 所有商品都已申请售后
  if (!order.value || order.value.items.length === 0) return false
  return order.value.items.every(item => refundedItemIds.value.has(item.id))
})

const getStatusType = (status?: OrderStatus) => {
  if (!status) return 'info'
  const types: Record<OrderStatus, string> = {
    pending: 'warning',
    paid: 'success',
    shipped: 'primary',
    completed: 'info',
    cancelled: 'danger',
    refunded: 'warning',
  }
  return types[status] || 'info'
}

const getRefundTypeTagType = (type: RefundType) => {
  const types: Record<RefundType, string> = {
    refund_only: 'info',
    return_refund: 'warning',
    exchange: 'success',
  }
  return types[type] || 'info'
}

const getRefundStatusTagType = (status: RefundStatus) => {
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

const payMethodMap: Record<string, string> = {
  wechat: 'order.payMethod.wechat',
  alipay: 'order.payMethod.alipay',
  bank: 'order.payMethod.bank',
  other: 'order.payMethod.other',
}

const getPayMethodLabel = (payMethod?: string) => {
  if (!payMethod) return '-'
  const key = payMethodMap[payMethod]
  return key ? t(key) : payMethod
}

const getShipCompanyLabel = (shipCompany?: string) => {
  if (!shipCompany) return '-'
  const company = SHIP_COMPANIES.find((c) => c.value === shipCompany)
  return company ? company.label : shipCompany
}

const isItemInRefund = (itemId: string) => {
  return refundedItemIds.value.has(itemId)
}

const fetchOrderDetail = async () => {
  if (!props.orderId) return

  loading.value = true
  try {
    // 获取订单详情
    const res = await getOrderDetail(props.orderId)
    order.value = res

    // 获取该订单的售后记录
    const refundRes = await getRefundList({
      orderId: props.orderId,
      pageIndex: 1,
      pageSize: 100,
    })

    // 处理售后记录
    const refunds = refundRes.list as Refund[]
    refundRecords.value = refunds.map(r => ({
      id: r.id,
      refundNo: r.refundNo,
      type: r.type,
      status: r.status,
      refundAmount: r.refundAmount,
      createTime: r.createTime,
    }))

    // 收集已申请售后的商品项 ID（排除已拒绝的）
    const itemIds = new Set<string>()
    refunds.forEach(refund => {
      // 已拒绝的不算
      if (refund.status === 'rejected') return
      refund.items.forEach(item => {
        itemIds.add(item.orderItemId)
      })
    })
    refundedItemIds.value = itemIds
  } catch (error) {
    console.error('Failed to fetch order detail:', error)
  } finally {
    loading.value = false
  }
}

const handleConfirmReceive = async () => {
  if (!order.value) return

  try {
    await ElMessageBox.confirm(
      t('order.detail.confirmReceiveConfirm'),
      t('common.message.warning'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )

    const res = await confirmReceive(order.value.id)
    ElMessage.success(t('order.detail.confirmReceiveSuccess', { points: res.pointsEarned }))
    emit('success')
    dialogVisible.value = false
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleReview = () => {
  reviewVisible.value = true
}

const handleReviewSuccess = (pointsEarned: number) => {
  ElMessage.success(t('order.review.success', { points: pointsEarned }))
  emit('success')
  dialogVisible.value = false
}

const handleViewShipTrack = () => {
  shipTrackVisible.value = true
}

const handleCreateRefund = () => {
  refundCreateVisible.value = true
}

const handleRefundSuccess = () => {
  emit('success')
  dialogVisible.value = false
}

const handleViewRefundDetail = (row: RefundRecord) => {
  currentRefundId.value = row.id
  refundDetailVisible.value = true
}

const handleRefundDetailSuccess = () => {
  fetchOrderDetail()
}

watch(
  () => props.visible,
  (visible) => {
    if (visible && props.orderId) {
      refundRecords.value = []
      refundedItemIds.value = new Set()
      fetchOrderDetail()
    }
  },
  { immediate: true }
)
</script>

<style scoped lang="less">
.order-detail {
  .section {
    margin-bottom: 20px;

    &:last-child {
      margin-bottom: 0;
    }

    .section-title {
      font-size: 16px;
      font-weight: 600;
      color: #303133;
      margin-bottom: 12px;
      padding-left: 10px;
      border-left: 3px solid #409eff;
    }

    .logistics-action {
      margin-top: 12px;
    }
  }

  .product-image {
    width: 40px;
    height: 40px;
    border-radius: 4px;
    cursor: pointer;
  }

  .pay-amount {
    color: #f56c6c;
    font-weight: 600;
  }

  .refund-amount {
    color: #e6a23c;
    font-weight: 500;
  }

  .points-amount {
    color: #67c23a;
    font-weight: 500;
  }
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

.dialog-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;

  .dialog-title {
    font-size: 18px;
    font-weight: 500;
    color: #303133;
  }

  .dialog-actions {
    display: flex;
    gap: 8px;
    margin-right: 30px;
  }
}
</style>