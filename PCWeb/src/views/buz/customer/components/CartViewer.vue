<!-- src/views/buz/customer/components/CartViewer.vue -->
<template>
  <div class="cart-viewer">
    <div v-loading="loading" class="cart-content">
      <template v-if="cartList.length > 0">
        <div class="cart-header">
          <span>{{ t('customer.cartTotal', { count: cartList.length }) }}</span>
        </div>

        <div class="cart-list">
          <div
            v-for="item in cartList"
            :key="item.id"
            class="cart-item"
          >
            <div class="product-image">
              <el-image
                :src="item.productImage"
                fit="cover"
                style="width: 80px; height: 80px"
              >
                <template #error>
                  <div class="image-placeholder">
                    <el-icon><Picture /></el-icon>
                  </div>
                </template>
              </el-image>
            </div>
            <div class="product-info">
              <div class="product-name">{{ item.productName }}</div>
              <div class="sku-name">{{ item.skuName }}</div>
              <div class="add-time">{{ t('customer.addToCartTime') }}: {{ item.createTime }}</div>
            </div>
            <div class="product-price">
              <span class="price">¥{{ item.price.toFixed(2) }}</span>
              <span class="quantity">x{{ item.quantity }}</span>
            </div>
            <div class="product-total">
              ¥{{ (item.price * item.quantity).toFixed(2) }}
            </div>
          </div>
        </div>

        <div class="cart-footer">
          <span>{{ t('customer.cartTotal', { count: selectedCount }) }}</span>
          <span class="total-price">{{ t('customer.price') }}: ¥{{ totalPrice.toFixed(2) }}</span>
        </div>
      </template>
      <el-empty v-else :description="t('customer.cartEmpty')" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Picture } from '@element-plus/icons-vue'
import { getCustomerCartList } from '@/api/buz/customerApi'
import { useLocale } from '@/composables/useLocale'
import type { CustomerCart } from '@/types'

const props = defineProps<{
  customerId: string
}>()

const { t } = useLocale()

const loading = ref(false)
const cartList = ref<CustomerCart[]>([])
const totalPrice = ref(0)
const selectedCount = ref(0)

// Load cart list
const loadCartList = async () => {
  if (!props.customerId) return
  loading.value = true
  try {
    const data = await getCustomerCartList(props.customerId)
    cartList.value = data.list || []
    totalPrice.value = data.totalPrice || 0
    selectedCount.value = data.selectedCount || 0
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

// Watch customer id change
watch(() => props.customerId, (val) => {
  if (val) {
    loadCartList()
  }
}, { immediate: true })
</script>

<style scoped lang="scss">
.cart-viewer {
  .cart-content {
    min-height: 200px;
  }

  .cart-header {
    padding: 12px 0;
    font-size: 14px;
    color: #606266;
    border-bottom: 1px solid #e4e7ed;
  }

  .cart-list {
    .cart-item {
      display: flex;
      align-items: center;
      padding: 16px 0;
      border-bottom: 1px solid #f0f0f0;

      .product-image {
        flex-shrink: 0;
        margin-right: 12px;

        .image-placeholder {
          display: flex;
          align-items: center;
          justify-content: center;
          width: 80px;
          height: 80px;
          background: #f5f7fa;
          color: #909399;
        }
      }

      .product-info {
        flex: 1;
        min-width: 0;

        .product-name {
          font-size: 14px;
          color: #303133;
          margin-bottom: 4px;
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
        }

        .sku-name {
          font-size: 12px;
          color: #909399;
          margin-bottom: 4px;
        }

        .add-time {
          font-size: 12px;
          color: #c0c4cc;
        }
      }

      .product-price {
        flex-shrink: 0;
        width: 100px;
        text-align: right;
        margin-right: 20px;

        .price {
          font-size: 14px;
          color: #303133;
        }

        .quantity {
          font-size: 12px;
          color: #909399;
          margin-left: 8px;
        }
      }

      .product-total {
        flex-shrink: 0;
        width: 80px;
        text-align: right;
        font-size: 14px;
        color: #f56c6c;
        font-weight: 500;
      }
    }
  }

  .cart-footer {
    padding: 16px 0;
    display: flex;
    justify-content: space-between;
    align-items: center;
    font-size: 14px;
    color: #606266;

    .total-price {
      color: #f56c6c;
      font-weight: 500;
      font-size: 16px;
    }
  }
}
</style>