<!-- src/views/buz/customer/components/CustomerDetailDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    :title="t('customer.customerDetail')"
    width="700px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <div v-loading="loading" class="dialog-content">
      <template v-if="customerDetail">
        <!-- Basic Info Section -->
        <div class="section">
          <div class="section-title">{{ t('customer.basicInfo') }}</div>
          <div class="info-grid">
            <div class="info-item">
              <label>{{ t('customer.phone') }}</label>
              <span>{{ customerDetail.phone }}</span>
            </div>
            <div class="info-item">
              <label>{{ t('customer.nickname') }}</label>
              <span>{{ customerDetail.nickname }}</span>
            </div>
            <div class="info-item">
              <label>{{ t('customer.email') }}</label>
              <span>{{ customerDetail.email || '-' }}</span>
            </div>
            <div class="info-item">
              <label>{{ t('customer.level') }}</label>
              <el-tag v-if="customerDetail.levelName" type="primary" effect="plain" size="small">
                {{ customerDetail.levelName }}
              </el-tag>
              <span v-else>-</span>
            </div>
            <div class="info-item">
              <label>{{ t('customer.points') }}</label>
              <span class="points-value">{{ customerDetail.points }}</span>
            </div>
            <div class="info-item">
              <label>{{ t('customer.totalSpent') }}</label>
              <span class="money-value">¥{{ customerDetail.totalSpent.toLocaleString() }}</span>
            </div>
            <div class="info-item">
              <label>{{ t('customer.status') }}</label>
              <el-tag :type="customerDetail.status === 1 ? 'success' : 'danger'" size="small">
                {{ customerDetail.status === 1 ? t('customer.enabled') : t('customer.disabled') }}
              </el-tag>
            </div>
            <div class="info-item">
              <label>{{ t('customer.registerTime') }}</label>
              <span>{{ customerDetail.createTime }}</span>
            </div>
          </div>
        </div>

        <!-- Tabs Section -->
        <div class="tabs-section">
          <el-tabs v-model="activeTab">
            <el-tab-pane :label="t('customer.addressTab')" name="address">
              <AddressManager :customer-id="customerId!" @success="loadCustomerDetail" />
            </el-tab-pane>
            <el-tab-pane :label="t('customer.cartTab')" name="cart">
              <CartViewer :customer-id="customerId!" />
            </el-tab-pane>
            <el-tab-pane :label="t('customer.favoriteTab')" name="favorite">
              <FavoriteViewer :customer-id="customerId!" />
            </el-tab-pane>
          </el-tabs>
        </div>
      </template>
    </div>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { getCustomerDetail } from '@/api/buz/customerApi'
import { useLocale } from '@/composables/useLocale'
import type { Customer } from '@/types'
import AddressManager from './AddressManager.vue'
import CartViewer from './CartViewer.vue'
import FavoriteViewer from './FavoriteViewer.vue'

const props = defineProps<{
  modelValue: boolean
  customerId?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const loading = ref(false)
const customerDetail = ref<Customer | null>(null)
const activeTab = ref('address')

// Load customer detail
const loadCustomerDetail = async () => {
  if (!props.customerId) return
  loading.value = true
  try {
    const data = await getCustomerDetail(props.customerId)
    customerDetail.value = data
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

// Watch dialog open
watch(visible, (val) => {
  if (val && props.customerId) {
    loadCustomerDetail()
    activeTab.value = 'address'
  }
})

const handleClose = () => {
  customerDetail.value = null
  activeTab.value = 'address'
}
</script>

<style scoped lang="scss">
.dialog-content {
  max-height: 60vh;
  overflow-y: auto;
  padding: 0 10px;

  .section {
    margin-bottom: 24px;

    .section-title {
      font-size: 16px;
      font-weight: 500;
      color: #303133;
      margin-bottom: 16px;
      padding-bottom: 8px;
      border-bottom: 1px solid #e4e7ed;
    }

    .info-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 16px;

      .info-item {
        display: flex;
        align-items: center;

        label {
          flex-shrink: 0;
          width: 80px;
          color: #909399;
          font-size: 14px;
        }

        span {
          color: #303133;
          font-size: 14px;
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
    }
  }

  .tabs-section {
    .el-tabs__content {
      padding: 16px 0;
    }
  }
}
</style>