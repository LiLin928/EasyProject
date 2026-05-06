<template>
  <el-dialog
    v-model="dialogVisible"
    width="700px"
    destroy-on-close
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('refund.create.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="dialogVisible = false">
            {{ t('common.button.cancel') }}
          </el-button>
          <el-button type="primary" :loading="submitting" :disabled="selectedItems.length === 0" @click="handleSubmit">
            {{ t('common.button.submit') }}
          </el-button>
        </div>
      </div>
    </template>
    <div v-loading="loading" class="refund-create">
      <!-- 售后类型选择 -->
      <div class="section">
        <div class="section-title">{{ t('refund.create.typeInfo') }}</div>
        <el-form-item :label="t('refund.list.type')">
          <el-radio-group v-model="formData.type" @change="handleTypeChange">
            <el-radio-button value="refund_only">
              {{ t('refund.type.refund_only') }}
            </el-radio-button>
            <el-radio-button value="return_refund">
              {{ t('refund.type.return_refund') }}
            </el-radio-button>
            <el-radio-button value="exchange">
              {{ t('refund.type.exchange') }}
            </el-radio-button>
          </el-radio-group>
        </el-form-item>
        <div class="type-tip">
          {{ t(`refund.create.typeTip.${formData.type}`) }}
        </div>
      </div>

      <!-- 商品选择 -->
      <div class="section">
        <div class="section-title">{{ t('refund.create.selectItems') }}</div>
        <el-table
          ref="tableRef"
          :data="orderItems"
          border
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="55" />
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
          <el-table-column :label="t('refund.detail.productName')" prop="productName" min-width="180" />
          <el-table-column :label="t('refund.detail.price')" width="100">
            <template #default="{ row }">
              ¥{{ (row.price ?? 0).toFixed(2) }}
            </template>
          </el-table-column>
          <el-table-column :label="t('refund.create.maxQuantity')" width="100" align="center">
            <template #default="{ row }">
              {{ row.quantity }}
            </template>
          </el-table-column>
          <el-table-column :label="t('refund.detail.quantity')" width="120">
            <template #default="{ row }">
              <el-input-number
                v-model="row.refundQuantity"
                :min="1"
                :max="row.quantity"
                size="small"
                :disabled="!selectedItems.includes(row)"
              />
            </template>
          </el-table-column>
          <el-table-column :label="t('refund.detail.subtotal')" width="100">
            <template #default="{ row }">
              <span v-if="selectedItems.includes(row)">
                ¥{{ ((row.price ?? 0) * (row.refundQuantity || 1)).toFixed(2) }}
              </span>
              <span v-else>-</span>
            </template>
          </el-table-column>
        </el-table>
      </div>

      <!-- 退款原因 -->
      <div class="section">
        <div class="section-title">{{ t('refund.create.reasonInfo') }}</div>
        <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px">
          <el-form-item :label="t('refund.list.reason')" prop="reason">
            <el-select v-model="formData.reason" :placeholder="t('refund.create.reasonPlaceholder')" style="width: 100%">
              <el-option
                v-for="item in reasonOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
          </el-form-item>
          <el-form-item :label="t('refund.detail.description')">
            <el-input
              v-model="formData.description"
              type="textarea"
              :rows="3"
              :placeholder="t('refund.create.descriptionPlaceholder')"
              :maxlength="500"
              show-word-limit
            />
          </el-form-item>
        </el-form>
      </div>

      <!-- 退款金额汇总 - 仅退款/退货退款显示 -->
      <div v-if="formData.type !== 'exchange'" class="amount-summary">
        <span>{{ t('refund.create.refundAmount') }}：</span>
        <span class="amount">¥{{ totalRefundAmount.toFixed(2) }}</span>
      </div>

      <!-- 换货说明 -->
      <div v-if="formData.type === 'exchange'" class="exchange-tip">
        <el-alert type="info" :closable="false">
          换货商品将与原商品相同，商家将重新发货相同商品。
        </el-alert>
      </div>
    </div>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { getOrderDetail } from '@/api/buz/orderApi'
import { createRefund } from '@/api/buz/refundApi'
import type { Order, OrderItem, RefundType, ExchangeItem } from '@/types/order'

interface OrderItemWithRefund extends OrderItem {
  refundQuantity: number
}

const { t } = useI18n()

const props = defineProps<{
  visible: boolean
  orderId: string
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const loading = ref(false)
const submitting = ref(false)
const order = ref<Order | null>(null)
const orderItems = ref<OrderItemWithRefund[]>([])
const selectedItems = ref<OrderItemWithRefund[]>([])
const formRef = ref<FormInstance>()
const tableRef = ref()

const formData = ref({
  type: 'refund_only' as RefundType,
  reason: '',
  description: '',
})

const formRules: FormRules = {
  reason: [
    { required: true, message: t('refund.create.reasonRequired'), trigger: 'change' },
  ],
}

const dialogVisible = computed({
  get: () => props.visible,
  set: (value) => emit('update:visible', value),
})

const reasonOptions = computed(() => [
  { value: 'quality', label: t('refund.reasons.quality') },
  { value: 'damage', label: t('refund.reasons.damage') },
  { value: 'wrong', label: t('refund.reasons.wrong') },
  { value: 'notMatch', label: t('refund.reasons.notMatch') },
  { value: 'other', label: t('refund.reasons.other') },
])

const totalRefundAmount = computed(() => {
  return selectedItems.value.reduce((sum, item) => {
    return sum + (item.price ?? 0) * (item.refundQuantity || 1)
  }, 0)
})

const handleSelectionChange = (selection: OrderItemWithRefund[]) => {
  selectedItems.value = selection
  // 初始化退款数量为购买数量
  selection.forEach((item) => {
    if (!item.refundQuantity) {
      item.refundQuantity = item.quantity
    }
  })
}

const fetchOrderDetail = async () => {
  if (!props.orderId) return

  loading.value = true
  try {
    const res = await getOrderDetail(props.orderId)
    order.value = res
    // 初始化商品列表，设置默认退款数量为购买数量
    orderItems.value = (res.items || []).map((item) => ({
      ...item,
      refundQuantity: item.quantity,
    }))
  } catch (error) {
    console.error('Failed to fetch order detail:', error)
  } finally {
    loading.value = false
  }
}

// 换货时自动将选中的商品作为换货商品
const getExchangeItems = (): ExchangeItem[] => {
  return selectedItems.value.map((item) => ({
    productId: item.productId,
    productName: item.productName,
    productImage: item.productImage,
    price: item.price,
    quantity: item.refundQuantity,
  }))
}

const handleSubmit = async () => {
  if (!formRef.value) return

  await formRef.value.validate(async (valid) => {
    if (!valid) return

    if (selectedItems.value.length === 0) {
      ElMessage.warning(t('refund.create.selectItemsRequired'))
      return
    }

    submitting.value = true
    try {
      await createRefund({
        orderId: props.orderId,
        type: formData.value.type,
        items: selectedItems.value.map((item) => ({
          orderItemId: item.id,
          productId: item.productId,
          productName: item.productName,
          productImage: item.productImage,
          price: item.price,
          quantity: item.refundQuantity,
        })),
        reason: formData.value.reason,
        description: formData.value.description || undefined,
        refundAmount: formData.value.type !== 'exchange' ? totalRefundAmount.value : undefined,
        // 换货时自动使用选中的商品作为换货商品
        exchangeItems: formData.value.type === 'exchange' ? getExchangeItems() : undefined,
      })
      ElMessage.success(t('refund.create.success'))
      emit('success')
      dialogVisible.value = false
    } catch (error) {
      console.error('Failed to create refund:', error)
    } finally {
      submitting.value = false
    }
  })
}

watch(
  () => props.visible,
  (visible) => {
    if (visible && props.orderId) {
      formData.value = { type: 'refund_only', reason: '', description: '' }
      selectedItems.value = []
      fetchOrderDetail()
    }
  },
  { immediate: true }
)
</script>

<style scoped lang="less">
.refund-create {
  .section {
    margin-bottom: 20px;

    .section-title {
      font-size: 16px;
      font-weight: 600;
      color: #303133;
      margin-bottom: 12px;
      padding-left: 10px;
      border-left: 3px solid #409eff;
    }

    .type-tip {
      margin-top: 8px;
      padding: 8px 12px;
      background-color: #f4f4f5;
      border-radius: 4px;
      color: #909399;
      font-size: 14px;
    }
  }

  .product-image {
    width: 40px;
    height: 40px;
    border-radius: 4px;
  }

  .amount-summary {
    text-align: right;
    padding: 16px 0;
    font-size: 16px;

    .amount {
      color: #f56c6c;
      font-size: 20px;
      font-weight: 600;
    }
  }

  .exchange-tip {
    margin-bottom: 20px;
  }
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