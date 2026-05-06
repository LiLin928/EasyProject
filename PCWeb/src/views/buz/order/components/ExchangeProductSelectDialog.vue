<template>
  <el-dialog
    v-model="dialogVisible"
    width="600px"
    destroy-on-close
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('refund.exchange.selectProduct') }}</span>
        <div class="dialog-actions">
          <el-button @click="dialogVisible = false">
            {{ t('common.button.close') }}
          </el-button>
        </div>
      </div>
    </template>
    <div v-loading="loading" class="product-select">
      <!-- 搜索 -->
      <el-input
        v-model="searchKeyword"
        :placeholder="t('product.list.namePlaceholder')"
        clearable
        @keyup.enter="handleSearch"
      >
        <template #prefix>
          <el-icon><Search /></el-icon>
        </template>
      </el-input>

      <!-- 商品列表 -->
      <div class="product-list">
        <div
          v-for="product in products"
          :key="product.id"
          class="product-item"
          @click="handleSelect(product)"
        >
          <el-image
            :src="product.image"
            fit="cover"
            class="product-image"
          />
          <div class="product-info">
            <div class="product-name">{{ product.name }}</div>
            <div class="product-price">¥{{ product.price.toFixed(2) }}</div>
          </div>
        </div>
        <div v-if="products.length === 0" class="empty-tip">
          暂无商品
        </div>
      </div>
    </div>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Search } from '@element-plus/icons-vue'
import { useI18n } from 'vue-i18n'
import { getProductList } from '@/api/buz/productApi'
import type { Product } from '@/types/product'

const { t } = useI18n()

const props = defineProps<{
  visible: boolean
}>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'select', product: { id: string; name: string; image: string; price: number }): void
}>()

const loading = ref(false)
const searchKeyword = ref('')
const products = ref<Product[]>([])

const dialogVisible = computed({
  get: () => props.visible,
  set: (value) => emit('update:visible', value),
})

const handleSearch = async () => {
  loading.value = true
  try {
    const res = await getProductList({
      pageIndex: 1,
      pageSize: 20,
      name: searchKeyword.value || undefined,
      status: 'active',
    })
    products.value = res.data.list
  } catch (error) {
    console.error('Failed to fetch products:', error)
  } finally {
    loading.value = false
  }
}

const handleSelect = (product: Product) => {
  emit('select', {
    id: product.id,
    name: product.name,
    image: product.image,
    price: product.price,
  })
  dialogVisible.value = false
}

watch(
  () => props.visible,
  (visible) => {
    if (visible) {
      searchKeyword.value = ''
      handleSearch()
    }
  },
  { immediate: true }
)
</script>

<style scoped lang="less">
.product-select {
  .product-list {
    margin-top: 16px;
    max-height: 400px;
    overflow-y: auto;

    .product-item {
      display: flex;
      align-items: center;
      padding: 12px;
      border: 1px solid #ebeef5;
      border-radius: 4px;
      margin-bottom: 8px;
      cursor: pointer;
      transition: all 0.2s;

      &:hover {
        border-color: #409eff;
        background-color: #f5f7fa;
      }

      .product-image {
        width: 60px;
        height: 60px;
        border-radius: 4px;
      }

      .product-info {
        margin-left: 12px;
        flex: 1;

        .product-name {
          font-size: 14px;
          color: #303133;
          margin-bottom: 4px;
        }

        .product-price {
          font-size: 16px;
          color: #f56c6c;
          font-weight: 500;
        }
      }
    }

    .empty-tip {
      text-align: center;
      padding: 20px;
      color: #909399;
    }
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