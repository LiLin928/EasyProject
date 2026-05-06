<!-- src/views/buz/product/components/StockAdjustDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('product.stock.adjustStockTitle') }}</span>
        <div class="dialog-actions">
          <el-button @click="handleClose">{{ t('common.button.cancel') }}</el-button>
          <el-button type="primary" :loading="saving" @click="handleSave">
            {{ t('common.button.confirm') }}
          </el-button>
        </div>
      </div>
    </template>
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="100px"
    >
      <!-- Product info -->
      <el-form-item :label="t('product.stock.productName')">
        <span>{{ product?.productName }}</span>
      </el-form-item>
      <el-form-item :label="t('product.stock.skuCode')">
        <span>{{ product?.skuCode }}</span>
      </el-form-item>
      <el-form-item :label="t('product.stock.currentStock')">
        <span :class="{ 'low-stock': isLowStock }">
          {{ product?.stock }}
          <el-tag v-if="isLowStock" type="danger" size="small" style="margin-left: 8px">
            {{ t('product.stock.lowStock') }}
          </el-tag>
        </span>
      </el-form-item>
      <el-form-item :label="t('product.stock.alertThreshold')">
        <span>{{ product?.alertThreshold || 10 }}</span>
      </el-form-item>

      <el-divider />

      <!-- Adjust form -->
      <el-form-item :label="t('product.stock.changeType')" prop="type">
        <el-radio-group v-model="formData.type">
          <el-radio value="in">{{ t('product.stock.stockIn') }}</el-radio>
          <el-radio value="out">{{ t('product.stock.stockOut') }}</el-radio>
          <el-radio value="adjust">{{ t('product.stock.stockAdjust') }}</el-radio>
        </el-radio-group>
      </el-form-item>

      <!-- Supplier selection (only for stock-in) -->
      <el-form-item
        v-if="formData.type === 'in'"
        :label="t('product.stock.selectSupplier')"
        prop="supplierId"
      >
        <el-select
          v-model="formData.supplierId"
          :placeholder="t('product.stock.selectSupplier')"
          :loading="suppliersLoading"
          style="width: 200px"
          @change="handleSupplierChange"
        >
          <el-option
            v-for="item in productSuppliers"
            :key="item.supplierId"
            :label="item.supplierName"
            :value="item.supplierId"
          >
            <span>{{ item.supplierName }}</span>
            <span style="float: right; color: var(--el-text-color-secondary); font-size: 12px">
              {{ item.skuCode }} | ¥{{ item.purchasePrice }}/件
              <el-tag v-if="item.isDefault" type="success" size="small" style="margin-left: 4px">
                {{ t('product.supplier.defaultSupplier') }}
              </el-tag>
            </span>
          </el-option>
        </el-select>
        <el-button
          v-if="productSuppliers.length === 0"
          link
          type="primary"
          @click="handleBindSupplier"
        >
          {{ t('product.stock.bindSupplier') }}
        </el-button>
      </el-form-item>

      <!-- SKU Code display (only for stock-in) -->
      <el-form-item
        v-if="formData.type === 'in' && selectedProductSupplier"
        :label="t('product.supplier.skuCode')"
      >
        <el-tag type="info">{{ selectedProductSupplier.skuCode }}</el-tag>
      </el-form-item>

      <!-- Purchase price (only for stock-in) -->
      <el-form-item
        v-if="formData.type === 'in'"
        :label="t('product.stock.purchasePrice')"
        prop="purchasePrice"
      >
        <el-input-number
          v-model="formData.purchasePrice"
          :min="0"
          :precision="2"
          :placeholder="t('product.stock.purchasePricePlaceholder')"
          style="width: 200px"
        />
        <span class="unit-label">元/件</span>
      </el-form-item>

      <el-form-item :label="t('product.stock.changeQuantity')" prop="quantity">
        <el-input-number
          v-model="formData.quantity"
          :min="formData.type === 'adjust' ? -99999 : 0"
          :max="formData.type === 'out' ? product?.stock || 99999 : 99999"
          :precision="0"
          :placeholder="t('product.stock.quantityPlaceholder')"
          style="width: 200px"
        />
        <span v-if="formData.type === 'in'" class="unit-label">件（入库数量）</span>
        <span v-else-if="formData.type === 'out'" class="unit-label">件（出库数量）</span>
        <span v-else class="unit-label">件（正数增加/负数减少）</span>
      </el-form-item>

      <el-form-item :label="t('product.stock.remark')">
        <el-input
          v-model="formData.remark"
          type="textarea"
          :rows="2"
          :placeholder="t('product.stock.remarkPlaceholder')"
          maxlength="200"
        />
      </el-form-item>
    </el-form>

      </el-dialog>

  <!-- Bind supplier dialog -->
  <ProductSupplierDialog
    v-model="bindDialogVisible"
    :product-id="product?.productId"
    @success="loadProductSuppliers"
  />
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { adjustStock, getProductSuppliers } from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { Stock, StockChangeType, ProductSupplier } from '@/types/product'
import ProductSupplierDialog from './ProductSupplierDialog.vue'

const { t } = useLocale()

const props = defineProps<{
  modelValue: boolean
  product: Stock | null
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const formRef = ref<FormInstance>()
const saving = ref(false)
const suppliersLoading = ref(false)
const productSuppliers = ref<ProductSupplier[]>([])
const bindDialogVisible = ref(false)

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const isLowStock = computed(() => {
  if (!props.product) return false
  return props.product.stock < (props.product.alertThreshold || 10)
})

const formData = reactive({
  type: 'in' as StockChangeType,
  quantity: 0,
  remark: '',
  supplierId: '',
  purchasePrice: 0,
})

const selectedProductSupplier = computed(() => {
  if (!formData.supplierId) return null
  return productSuppliers.value.find((s) => s.supplierId === formData.supplierId)
})

const formRules: FormRules = {
  type: [{ required: true, message: t('product.stock.selectType'), trigger: 'change' }],
  quantity: [
    { required: true, message: t('product.stock.quantityPlaceholder'), trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (formData.type === 'adjust') {
          // 调整类型允许负数，但调整后库存不能为负
          const afterStock = (props.product?.stock || 0) + value
          if (afterStock < 0) {
            callback(new Error('调整后库存不能为负数'))
          } else {
            callback()
          }
        } else if (value < 0) {
          // 入库和出库不允许负数
          callback(new Error('数量不能为负数'))
        } else {
          callback()
        }
      },
      trigger: 'blur',
    },
  ],
  supplierId: [
    {
      required: true,
      validator: (rule, value, callback) => {
        if (formData.type === 'in' && !value) {
          callback(new Error(t('product.stock.supplierRequired')))
        } else {
          callback()
        }
      },
      trigger: 'change',
    },
  ],
  purchasePrice: [
    {
      required: true,
      validator: (rule, value, callback) => {
        if (formData.type === 'in' && (!value || value <= 0)) {
          callback(new Error(t('product.stock.purchasePriceRequired')))
        } else {
          callback()
        }
      },
      trigger: 'blur',
    },
  ],
}

/**
 * Load product suppliers
 */
const loadProductSuppliers = async () => {
  if (!props.product?.productId) return

  suppliersLoading.value = true
  try {
    const data = await getProductSuppliers(props.product.productId)
    productSuppliers.value = data

    // Auto-select default supplier
    const defaultSupplier = data.find((s) => s.isDefault)
    if (defaultSupplier) {
      formData.supplierId = defaultSupplier.supplierId
      formData.purchasePrice = defaultSupplier.purchasePrice
    } else if (data.length > 0) {
      formData.supplierId = data[0].supplierId
      formData.purchasePrice = data[0].purchasePrice
    }
  } catch (error) {
    // Error handled by interceptor
  } finally {
    suppliersLoading.value = false
  }
}

/**
 * Handle supplier change
 */
const handleSupplierChange = (supplierId: string) => {
  const supplier = productSuppliers.value.find((s) => s.supplierId === supplierId)
  if (supplier) {
    formData.purchasePrice = supplier.purchasePrice
  }
}

/**
 * Open bind supplier dialog
 */
const handleBindSupplier = () => {
  bindDialogVisible.value = true
}

// Reset form when dialog opens
watch(visible, (val) => {
  if (val) {
    formData.type = 'in'
    formData.quantity = 0
    formData.remark = ''
    formData.supplierId = ''
    formData.purchasePrice = 0

    if (formData.type === 'in') {
      loadProductSuppliers()
    }
  }
})

// Load suppliers when type changes to 'in'
watch(
  () => formData.type,
  (type) => {
    if (type === 'in' && visible.value) {
      loadProductSuppliers()
    }
  }
)

const handleClose = () => {
  visible.value = false
  formRef.value?.resetFields()
  productSuppliers.value = []
}

const handleSave = async () => {
  if (!props.product) return

  try {
    await formRef.value?.validate()
  } catch {
    return
  }

  // Validate quantity based on type
  if (formData.type === 'out' && formData.quantity > props.product.stock) {
    ElMessage.error(t('product.stock.stockNotEnough', { stock: props.product.stock }))
    return
  }

  // Validate adjust result
  if (formData.type === 'adjust') {
    const afterStock = props.product.stock + formData.quantity
    if (afterStock < 0) {
      ElMessage.error('调整后库存不能为负数')
      return
    }
  }

  // Validate stock-in has supplier
  if (formData.type === 'in' && productSuppliers.value.length === 0) {
    ElMessage.warning(t('product.stock.noSupplierBound'))
    return
  }

  saving.value = true
  try {
    // Get SKU code from selected supplier
    const selectedSupplier = productSuppliers.value.find((s) => s.supplierId === formData.supplierId)
    const skuCode = formData.type === 'in' ? selectedSupplier?.skuCode : undefined

    await adjustStock({
      productId: props.product.productId,
      type: formData.type,
      quantity: formData.quantity,
      supplierId: formData.type === 'in' ? formData.supplierId : undefined,
      purchasePrice: formData.type === 'in' ? formData.purchasePrice : undefined,
      skuCode,
      remark: formData.remark,
    })
    ElMessage.success(t('product.stock.adjustSuccess'))
    emit('success')
    handleClose()
  } catch (error) {
    // Error handled by interceptor
  } finally {
    saving.value = false
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

.low-stock {
  color: var(--el-color-danger);
  font-weight: 600;
}

.unit-label {
  margin-left: 8px;
  color: var(--el-text-color-secondary);
}
</style>