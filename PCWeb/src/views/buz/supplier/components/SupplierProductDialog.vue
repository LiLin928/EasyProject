<!-- src/views/buz/supplier/components/SupplierProductDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="800px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ `${supplier?.name || ''} - ${t('product.supplier.productList')}` }}</span>
        <div class="dialog-actions">
          <el-button @click="handleClose">{{ t('common.button.cancel') }}</el-button>
        </div>
      </div>
    </template>
    <el-table v-loading="loading" :data="tableData" border>
      <el-table-column prop="skuCode" :label="t('product.supplier.skuCode')" width="150" />
      <el-table-column :label="t('product.stock.productName')" min-width="150">
        <template #default="{ row }">
          <div class="product-info">
            <el-image
              v-if="row.product?.image"
              :src="row.product.image"
              fit="cover"
              style="width: 40px; height: 40px; border-radius: 4px; margin-right: 8px"
            />
            <div>
              <div>{{ row.product?.name || '-' }}</div>
              <div class="product-sku">{{ row.product?.skuCode }}</div>
            </div>
          </div>
        </template>
      </el-table-column>
      <el-table-column :label="t('product.list.category')" width="120">
        <template #default="{ row }">
          {{ row.product?.category?.name || '-' }}
        </template>
      </el-table-column>
      <el-table-column prop="purchasePrice" :label="t('product.supplier.purchasePrice')" width="100" align="right">
        <template #default="{ row }">
          ¥{{ row.purchasePrice?.toFixed(2) }}
        </template>
      </el-table-column>
      <el-table-column prop="minOrderQty" :label="t('product.supplier.minOrderQty')" width="100" align="center">
        <template #default="{ row }">
          {{ row.minOrderQty || '-' }}
        </template>
      </el-table-column>
      <el-table-column prop="isDefault" :label="t('product.supplier.isDefault')" width="80" align="center">
        <template #default="{ row }">
          <el-tag v-if="row.isDefault" type="success" size="small">
            {{ t('product.supplier.defaultSupplier') }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="t('product.list.operation')" width="100" fixed="right">
        <template #default="{ row }">
          <el-button link type="primary" @click="handleSetDefault(row)" :disabled="row.isDefault">
            {{ t('product.supplier.setDefault') }}
          </el-button>
        </template>
      </el-table-column>
    </el-table>

      </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { getSupplierProducts, setDefaultSupplier } from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { Supplier, ProductSupplier } from '@/types/product'

const { t } = useLocale()

const props = defineProps<{
  modelValue: boolean
  supplier?: Supplier
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>()

const loading = ref(false)
const tableData = ref<ProductSupplier[]>([])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

/**
 * Load supplier products
 */
const loadSupplierProducts = async () => {
  if (!props.supplier?.id) return

  loading.value = true
  try {
    const data = await getSupplierProducts(props.supplier.id)
    tableData.value = data
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

// Watch for dialog open
watch(visible, (val) => {
  if (val) {
    loadSupplierProducts()
  }
})

/**
 * Handle close
 */
const handleClose = () => {
  visible.value = false
}

/**
 * Set default supplier
 */
const handleSetDefault = async (row: ProductSupplier) => {
  try {
    await setDefaultSupplier(row.id)
    ElMessage.success(t('product.supplier.setDefaultSuccess'))
    loadSupplierProducts()
  } catch (error) {
    // Error handled by interceptor
  }
}
</script>

<style scoped lang="scss">
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

.product-info {
  display: flex;
  align-items: center;

  .product-sku {
    font-size: 12px;
    color: var(--el-text-color-secondary);
    margin-top: 2px;
  }
}
</style>