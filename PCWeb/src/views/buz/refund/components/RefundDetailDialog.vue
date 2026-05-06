<template>
  <el-dialog
    v-model="dialogVisible"
    width="750px"
    destroy-on-close
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('refund.detail.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="dialogVisible = false">
            {{ t('common.button.close') }}
          </el-button>
          <!-- 状态为 returning 时显示确认收货按钮 -->
          <el-button
            v-if="isReturning"
            type="success"
            :loading="confirmReceiveLoading"
            @click="handleConfirmReceive"
          >
            {{ t('refund.action.confirmReceive') }}
          </el-button>
          <!-- 状态为 received 且是换货类型时显示换货发货按钮 -->
          <el-button
            v-if="isReceivedAndExchange"
            type="primary"
            :loading="exchangeShipLoading"
            @click="handleExchangeShip"
          >
            {{ t('refund.action.exchangeShip') }}
          </el-button>
          <!-- 状态为 pending 时显示审核按钮 -->
          <template v-if="isPending">
            <el-button
              type="danger"
              :loading="rejectLoading"
              @click="handleReject"
            >
              {{ t('refund.detail.reject') }}
            </el-button>
            <el-button
              type="success"
              :loading="approveLoading"
              @click="handleApprove"
            >
              {{ t('refund.detail.approve') }}
            </el-button>
          </template>
        </div>
      </div>
    </template>
    <div v-loading="loading" class="refund-detail">
      <!-- 基本信息 -->
      <div class="section">
        <div class="section-title">{{ t('refund.detail.basicInfo') }}</div>
        <el-descriptions :column="2" border>
          <el-descriptions-item :label="t('refund.detail.refundNo')">
            {{ refund?.refundNo }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('refund.detail.createTime')">
            {{ refund?.createTime }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('refund.list.orderNo')">
            {{ refund?.orderNo }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('refund.detail.type')">
            <el-tag :type="getTypeTagType(refund?.type)">
              {{ t(`refund.type.${refund?.type}`) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item :label="t('refund.detail.refundAmount')" v-if="refund?.type !== 'exchange'">
            <span class="refund-amount">¥{{ refund?.refundAmount.toFixed(2) }}</span>
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <!-- 退款原因 -->
      <div class="section">
        <div class="section-title">{{ t('refund.detail.reason') }}</div>
        <el-descriptions :column="1" border>
          <el-descriptions-item :label="t('refund.list.reason')">
            {{ getReasonLabel(refund?.reason) }}
          </el-descriptions-item>
          <el-descriptions-item
            v-if="refund?.description"
            :label="t('refund.detail.description')"
          >
            {{ refund?.description }}
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <!-- 商品列表 -->
      <div class="section">
        <div class="section-title">{{ t('refund.detail.productList') }}</div>
        <el-table :data="refund?.items || []" border>
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
          <el-table-column :label="t('refund.detail.productName')" prop="productName" />
          <el-table-column :label="t('refund.detail.price')" width="120">
            <template #default="{ row }">
              ¥{{ row.price.toFixed(2) }}
            </template>
          </el-table-column>
          <el-table-column :label="t('refund.detail.quantity')" prop="quantity" width="80" />
          <el-table-column :label="t('refund.detail.subtotal')" width="120">
            <template #default="{ row }">
              ¥{{ row.amount.toFixed(2) }}
            </template>
          </el-table-column>
        </el-table>
      </div>

      <!-- 换货商品 - 仅换货类型显示 -->
      <div v-if="refund?.type === 'exchange' && refund?.exchangeItems?.length" class="section">
        <div class="section-title">{{ t('refund.exchange.title') }}</div>
        <el-table :data="refund?.exchangeItems || []" border>
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
          <el-table-column :label="t('refund.detail.productName')" prop="productName" />
          <el-table-column :label="t('refund.detail.price')" width="120">
            <template #default="{ row }">
              ¥{{ row.price.toFixed(2) }}
            </template>
          </el-table-column>
          <el-table-column :label="t('refund.exchange.quantity')" prop="quantity" width="80" />
        </el-table>
      </div>

      <!-- 退货快递信息 - 退货退款/换货类型显示 -->
      <div v-if="refund?.type !== 'refund_only'" class="section">
        <div class="section-title">{{ t('refund.ship.returnTitle') }}</div>
        <!-- 已填写快递信息 -->
        <template v-if="refund?.returnShipNo">
          <el-descriptions :column="3" border>
            <el-descriptions-item :label="t('refund.ship.returnCompany')">
              {{ getShipCompanyName(refund?.returnShipCompany) }}
            </el-descriptions-item>
            <el-descriptions-item :label="t('refund.ship.returnNo')">
              {{ refund?.returnShipNo }}
            </el-descriptions-item>
            <el-descriptions-item :label="t('refund.ship.returnTime')">
              {{ refund?.returnShipTime }}
            </el-descriptions-item>
          </el-descriptions>
          <div class="ship-actions">
            <el-button type="primary" link @click="showShipTrack('return')">
              <el-icon><Location /></el-icon>
              {{ t('refund.ship.viewTrack') }}
            </el-button>
          </div>
        </template>
        <!-- 未填写快递信息 - 状态为 approved 时显示填写表单 -->
        <template v-else-if="refund?.status === 'approved'">
          <el-alert type="info" :closable="false" show-icon>
            {{ t('refund.action.waitReturn') }}
          </el-alert>
        </template>
        <template v-else>
          <el-alert type="info" :closable="false">
            {{ t('refund.ship.noShipInfo') }}
          </el-alert>
        </template>
      </div>

      <!-- 换货发货信息 - 仅换货类型显示 -->
      <div v-if="refund?.type === 'exchange'" class="section">
        <div class="section-title">{{ t('refund.ship.exchangeTitle') }}</div>
        <!-- 已填写发货信息 -->
        <template v-if="refund?.exchangeShipNo">
          <el-descriptions :column="3" border>
            <el-descriptions-item :label="t('refund.ship.returnCompany')">
              {{ getShipCompanyName(refund?.exchangeShipCompany) }}
            </el-descriptions-item>
            <el-descriptions-item :label="t('refund.ship.returnNo')">
              {{ refund?.exchangeShipNo }}
            </el-descriptions-item>
            <el-descriptions-item :label="t('refund.ship.returnTime')">
              {{ refund?.exchangeShipTime }}
            </el-descriptions-item>
          </el-descriptions>
          <div class="ship-actions">
            <el-button type="primary" link @click="showShipTrack('exchange')">
              <el-icon><Location /></el-icon>
              {{ t('refund.ship.viewTrack') }}
            </el-button>
          </div>
        </template>
        <!-- 未填写发货信息 - 状态为 received 时显示填写表单 -->
        <template v-else-if="refund?.status === 'received'">
          <el-form ref="exchangeShipFormRef" :model="exchangeShipForm" :rules="shipRules" label-width="100px">
            <el-form-item :label="t('refund.ship.returnCompany')" prop="shipCompany">
              <el-select v-model="exchangeShipForm.shipCompany" :placeholder="t('refund.ship.companyPlaceholder')" style="width: 200px">
                <el-option
                  v-for="item in shipCompanyOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="t('refund.ship.returnNo')" prop="shipNo">
              <el-input v-model="exchangeShipForm.shipNo" :placeholder="t('refund.ship.noPlaceholder')" style="width: 200px" />
            </el-form-item>
          </el-form>
        </template>
        <template v-else>
          <el-alert type="info" :closable="false">
            {{ t('refund.ship.noShipInfo') }}
          </el-alert>
        </template>
      </div>

      <!-- 审核信息 - 已审核时显示 -->
      <div v-if="showApproveInfo" class="section">
        <div class="section-title">{{ t('refund.detail.approveRemark') }}</div>
        <el-descriptions :column="2" border>
          <el-descriptions-item :label="t('refund.detail.approver')">
            {{ refund?.approver || '-' }}
          </el-descriptions-item>
          <el-descriptions-item :label="t('refund.detail.approveTime')">
            {{ refund?.approveTime || '-' }}
          </el-descriptions-item>
          <el-descriptions-item
            v-if="refund?.approveRemark"
            :label="t('refund.detail.approveRemark')"
            :span="2"
          >
            {{ refund?.approveRemark }}
          </el-descriptions-item>
          <el-descriptions-item
            v-if="refund?.refundTime"
            :label="t('refund.detail.refundTime')"
            :span="2"
          >
            {{ refund?.refundTime }}
          </el-descriptions-item>
          <el-descriptions-item
            v-if="refund?.completeTime"
            :label="t('refund.detail.completeTime')"
            :span="2"
          >
            {{ refund?.completeTime }}
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <!-- 审核表单 - pending 时显示 -->
      <div v-if="isPending" class="section">
        <div class="section-title">{{ t('refund.detail.approveRemark') }}</div>
        <el-form ref="formRef" :model="form" :rules="rules" label-position="top">
          <el-form-item :label="t('refund.detail.approveRemark')" prop="remark">
            <el-input
              v-model="form.remark"
              type="textarea"
              :rows="3"
              :placeholder="t('refund.detail.remarkPlaceholder')"
              :maxlength="200"
              show-word-limit
            />
          </el-form-item>
        </el-form>
      </div>
    </div>

    
    <!-- 物流轨迹弹窗 -->
    <el-dialog
      v-model="shipTrackVisible"
      :title="t('refund.ship.trackTitle')"
      width="500px"
      append-to-body
    >
      <div v-loading="shipTrackLoading" class="ship-track">
        <el-timeline v-if="shipTracks.length > 0">
          <el-timeline-item
            v-for="track in shipTracks"
            :key="track.time"
            :timestamp="track.time"
            placement="top"
          >
            <div class="track-content">
              <div class="track-status">{{ track.status }}</div>
              <div class="track-location" v-if="track.location">{{ track.location }}</div>
              <div class="track-description">{{ track.description }}</div>
            </div>
          </el-timeline-item>
        </el-timeline>
        <el-empty v-else description="暂无物流信息" />
      </div>
    </el-dialog>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Location } from '@element-plus/icons-vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import {
  getRefundDetail,
  approveRefund,
  rejectRefund,
  confirmReceive,
  exchangeShip,
  getShipDetail,
} from '@/api/buz/refundApi'
import { SHIP_COMPANIES } from '@/types/order'
import type { Refund, RefundType, ShipTrack } from '@/types/order'

const { t } = useI18n()

const props = defineProps<{
  visible: boolean
  refundId: string
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const loading = ref(false)
const approveLoading = ref(false)
const rejectLoading = ref(false)
const confirmReceiveLoading = ref(false)
const exchangeShipLoading = ref(false)
const refund = ref<Refund | null>(null)
const formRef = ref<FormInstance>()
const exchangeShipFormRef = ref<FormInstance>()

const form = ref({
  remark: '',
})

const exchangeShipForm = ref({
  shipCompany: '',
  shipNo: '',
})

const rules: FormRules = {
  remark: [],
}

const shipRules: FormRules = {
  shipCompany: [
    { required: true, message: t('refund.ship.companyPlaceholder'), trigger: 'change' },
  ],
  shipNo: [
    { required: true, message: t('refund.ship.noPlaceholder'), trigger: 'blur' },
  ],
}

const shipCompanyOptions = computed(() => SHIP_COMPANIES)

// 物流轨迹
const shipTrackVisible = ref(false)
const shipTrackLoading = ref(false)
const shipTracks = ref<ShipTrack[]>([])
const currentShipType = ref<'return' | 'exchange'>('return')

const dialogVisible = computed({
  get: () => props.visible,
  set: (value) => emit('update:visible', value),
})

// 状态判断
const isPending = computed(() => refund.value?.status === 'pending')
const isReturning = computed(() => refund.value?.status === 'returning')
const isReceivedAndExchange = computed(() => refund.value?.status === 'received' && refund.value?.type === 'exchange')

const showApproveInfo = computed(() => {
  return refund.value && refund.value.status !== 'pending'
})

const getTypeTagType = (type?: RefundType) => {
  if (!type) return 'info'
  const types: Record<RefundType, string> = {
    refund_only: 'info',
    return_refund: 'warning',
    exchange: 'success',
  }
  return types[type] || 'info'
}

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

const getShipCompanyName = (company?: string) => {
  if (!company) return ''
  const found = SHIP_COMPANIES.find(c => c.value === company)
  return found ? found.label : company
}

const fetchRefundDetail = async () => {
  if (!props.refundId) return

  loading.value = true
  try {
    const res = await getRefundDetail(props.refundId)
    refund.value = res
  } catch (error) {
    console.error('Failed to fetch refund detail:', error)
  } finally {
    loading.value = false
  }
}

const showShipTrack = async (type: 'return' | 'exchange') => {
  currentShipType.value = type
  const shipCompany = type === 'return' ? refund.value?.returnShipCompany : refund.value?.exchangeShipCompany
  const shipNo = type === 'return' ? refund.value?.returnShipNo : refund.value?.exchangeShipNo

  if (!shipCompany || !shipNo) {
    ElMessage.warning(t('refund.ship.noShipInfo'))
    return
  }

  shipTrackVisible.value = true
  shipTrackLoading.value = true
  try {
    const res = await getShipDetail(shipCompany, shipNo)
    shipTracks.value = res.tracks
  } catch (error) {
    console.error('Failed to fetch ship track:', error)
    shipTracks.value = []
  } finally {
    shipTrackLoading.value = false
  }
}

const handleApprove = async () => {
  if (!props.refundId) return

  approveLoading.value = true
  try {
    await approveRefund({
      id: props.refundId,
      remark: form.value.remark || undefined,
    })
    ElMessage.success(t('refund.detail.approveSuccess'))
    emit('success')
    dialogVisible.value = false
  } catch (error) {
    console.error('Failed to approve refund:', error)
  } finally {
    approveLoading.value = false
  }
}

const handleReject = async () => {
  if (!props.refundId) return

  // 拒绝时必须填写审核备注
  if (!form.value.remark.trim()) {
    ElMessage.warning(t('refund.detail.remarkRequired'))
    return
  }

  rejectLoading.value = true
  try {
    await rejectRefund({
      id: props.refundId,
      remark: form.value.remark,
    })
    ElMessage.success(t('refund.detail.rejectSuccess'))
    emit('success')
    dialogVisible.value = false
  } catch (error) {
    console.error('Failed to reject refund:', error)
  } finally {
    rejectLoading.value = false
  }
}

const handleConfirmReceive = async () => {
  if (!props.refundId) return

  confirmReceiveLoading.value = true
  try {
    await confirmReceive({
      id: props.refundId,
    })
    ElMessage.success(t('refund.action.confirmReceiveSuccess'))
    emit('success')
    fetchRefundDetail()
  } catch (error) {
    console.error('Failed to confirm receive:', error)
  } finally {
    confirmReceiveLoading.value = false
  }
}

const handleExchangeShip = async () => {
  if (!props.refundId) return
  if (!exchangeShipFormRef.value) return

  await exchangeShipFormRef.value.validate(async (valid) => {
    if (!valid) return

    exchangeShipLoading.value = true
    try {
      await exchangeShip({
        id: props.refundId,
        shipCompany: exchangeShipForm.value.shipCompany,
        shipNo: exchangeShipForm.value.shipNo,
      })
      ElMessage.success(t('refund.ship.fillExchangeSuccess'))
      emit('success')
      fetchRefundDetail()
    } catch (error) {
      console.error('Failed to exchange ship:', error)
    } finally {
      exchangeShipLoading.value = false
    }
  })
}

watch(
  () => props.visible,
  (visible) => {
    if (visible && props.refundId) {
      form.value.remark = ''
      exchangeShipForm.value = { shipCompany: '', shipNo: '' }
      shipTracks.value = []
      fetchRefundDetail()
    }
  },
  { immediate: true }
)
</script>

<style scoped lang="less">
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

.refund-detail {
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

    .ship-actions {
      margin-top: 8px;
      padding: 8px 12px;
    }
  }

  .product-image {
    width: 40px;
    height: 40px;
    border-radius: 4px;
    cursor: pointer;
  }

  .refund-amount {
    color: #f56c6c;
    font-weight: 600;
  }
}

.ship-track {
  .track-content {
    .track-status {
      font-weight: 600;
      color: #303133;
    }

    .track-location {
      font-size: 12px;
      color: #909399;
      margin-top: 4px;
    }

    .track-description {
      font-size: 14px;
      color: #606266;
      margin-top: 4px;
    }
  }
}
</style>