<!-- src/views/buz/product/components/ProductSupplierDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="600px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('product.supplier.bindProduct') }}</span>
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
      <!-- Supplier selection -->
      <el-form-item :label="t('product.supplier.name')" prop="supplierId">
        <el-select
          v-model="formData.supplierId"
          :placeholder="t('product.stock.selectSupplier')"
          :loading="suppliersLoading"
          filterable
          style="width: 100%"
        >
          <el-option
            v-for="item in suppliers"
            :key="item.id"
            :label="item.name"
            :value="item.id"
          >
            <span>{{ item.name }}</span>
            <span style="float: right; color: var(--el-text-color-secondary)">
              {{ item.contact }} | {{ item.phone }}
            </span>
          </el-option>
        </el-select>
      </el-form-item>

      <!-- SKU Code -->
      <el-form-item :label="t('product.supplier.skuCode')" prop="skuCode">
        <el-input
          v-model="formData.skuCode"
          :placeholder="t('product.supplier.skuCodePlaceholder')"
          maxlength="50"
        >
          <template #append>
            <el-button @click="generateSkuCode">生成</el-button>
          </template>
        </el-input>
      </el-form-item>

      <!-- Purchase price -->
      <el-form-item :label="t('product.supplier.purchasePrice')" prop="purchasePrice">
        <el-input-number
          v-model="formData.purchasePrice"
          :min="0"
          :precision="2"
          :placeholder="t('product.stock.purchasePricePlaceholder')"
          style="width: 200px"
        />
        <span class="unit-label">元/件</span>
      </el-form-item>

      <!-- Min order quantity -->
      <el-form-item :label="t('product.supplier.minOrderQty')">
        <el-input-number
          v-model="formData.minOrderQty"
          :min="1"
          :precision="0"
          :placeholder="t('product.supplier.minOrderQtyPlaceholder')"
          style="width: 200px"
        />
        <span class="unit-label">件</span>
      </el-form-item>

      <!-- Default supplier -->
      <el-form-item :label="t('product.supplier.isDefault')">
        <el-switch
          v-model="formData.isDefault"
          :active-text="t('product.supplier.defaultSupplier')"
        />
      </el-form-item>

      <!-- Remark -->
      <el-form-item :label="t('product.supplier.remark')">
        <el-input
          v-model="formData.remark"
          type="textarea"
          :rows="2"
          :placeholder="t('product.supplier.remarkPlaceholder')"
          maxlength="200"
        />
      </el-form-item>
    </el-form>

    <!-- Existing bindings -->
    <el-divider content-position="left">
      {{ t('product.supplier.productList') }}
    </el-divider>

    <el-table v-loading="bindingsLoading" :data="existingBindings" border size="small">
      <el-table-column prop="skuCode" :label="t('product.supplier.skuCode')" width="150" />
      <el-table-column prop="supplierName" :label="t('product.supplier.name')" width="150">
        <template #default="{ row }">
          {{ row.supplier?.name || '-' }}
        </template>
      </el-table-column>
      <el-table-column prop="purchasePrice" :label="t('product.supplier.purchasePrice')" width="100">
        <template #default="{ row }">
          ¥{{ row.purchasePrice }}
        </template>
      </el-table-column>
      <el-table-column prop="minOrderQty" :label="t('product.supplier.minOrderQty')" width="100">
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
      <el-table-column :label="t('product.list.operation')" width="150">
        <template #default="{ row }">
          <el-button link type="primary" @click="handleSetDefault(row)" :disabled="row.isDefault">
            {{ t('product.supplier.setDefault') }}
          </el-button>
          <el-button link type="danger" @click="handleUnbind(row)">
            {{ t('common.button.delete') }}
          </el-button>
        </template>
      </el-table-column>
    </el-table>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import {
  getSupplierList,
  getProductSuppliers,
  bindProductSupplier,
  unbindProductSupplier,
  setDefaultSupplier,
} from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { Supplier, ProductSupplier } from '@/types/product'

const { t } = useLocale()

const props = defineProps<{
  modelValue: boolean
  productId?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const formRef = ref<FormInstance>()
const saving = ref(false)
const suppliersLoading = ref(false)
const bindingsLoading = ref(false)

const suppliers = ref<Supplier[]>([])
const existingBindings = ref<ProductSupplier[]>([])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const formData = reactive({
  supplierId: '',
  skuCode: '',
  purchasePrice: 0,
  minOrderQty: 1,
  isDefault: false,
  remark: '',
})

const formRules: FormRules = {
  supplierId: [
    { required: true, message: t('product.stock.selectSupplier'), trigger: 'change' },
  ],
  skuCode: [
    { required: true, message: t('product.supplier.skuCodeRequired'), trigger: 'blur' },
  ],
  purchasePrice: [
    { required: true, message: t('product.stock.purchasePriceRequired'), trigger: 'blur' },
    { type: 'number', min: 0.01, message: '采购价格必须大于0', trigger: 'blur' },
  ],
}

/**
 * Load enabled suppliers
 */
const loadSuppliers = async () => {
  suppliersLoading.value = true
  try {
    const data = await getSupplierList({ status: 1 })
    suppliers.value = data.list
  } catch (error) {
    // Error handled by interceptor
  } finally {
    suppliersLoading.value = false
  }
}

/**
 * Load existing bindings
 */
const loadBindings = async () => {
  if (!props.productId) return

  bindingsLoading.value = true
  try {
    const data = await getProductSuppliers(props.productId)
    existingBindings.value = data
  } catch (error) {
    // Error handled by interceptor
  } finally {
    bindingsLoading.value = false
  }
}

// Watch for dialog open
watch(visible, (val) => {
  if (val) {
    resetForm()
    loadSuppliers()
    loadBindings()
  }
})

/**
 * Reset form
 */
const resetForm = () => {
  formData.supplierId = ''
  formData.skuCode = ''
  formData.purchasePrice = 0
  formData.minOrderQty = 1
  formData.isDefault = existingBindings.value.length === 0
  formData.remark = ''
}

/**
 * Generate SKU code
 */
const generateSkuCode = () => {
  if (!formData.supplierId) {
    ElMessage.warning('请先选择供应商')
    return
  }
  const supplier = suppliers.value.find(s => s.id === formData.supplierId)
  const prefix = supplier?.name?.substring(0, 3).toUpperCase() || 'SUP'
  const timestamp = Date.now().toString(36).toUpperCase()
  const random = Math.random().toString(36).substring(2, 5).toUpperCase()
  formData.skuCode = `${prefix}-${timestamp}-${random}`
}

/**
 * Handle close
 */
const handleClose = () => {
  visible.value = false
  formRef.value?.resetFields()
}

/**
 * Handle save (bind supplier)
 */
const handleSave = async () => {
  if (!props.productId) return

  try {
    await formRef.value?.validate()
  } catch {
    return
  }

  // Check if already bound
  const exists = existingBindings.value.find((b) => b.supplierId === formData.supplierId)
  if (exists) {
    ElMessage.warning('该供应商已绑定此商品')
    return
  }

  saving.value = true
  try {
    await bindProductSupplier({
      productId: props.productId,
      supplierId: formData.supplierId,
      skuCode: formData.skuCode,
      purchasePrice: formData.purchasePrice,
      minOrderQty: formData.minOrderQty,
      isDefault: formData.isDefault,
      remark: formData.remark,
    })
    ElMessage.success(t('product.supplier.bindSuccess'))
    emit('success')
    loadBindings()
    resetForm()
  } catch (error) {
    // Error handled by interceptor
  } finally {
    saving.value = false
  }
}

/**
 * Set default supplier
 */
const handleSetDefault = async (row: ProductSupplier) => {
  try {
    await setDefaultSupplier(row.id)
    ElMessage.success(t('product.supplier.setDefaultSuccess'))
    loadBindings()
  } catch (error) {
    // Error handled by interceptor
  }
}

/**
 * Unbind supplier
 */
const handleUnbind = async (row: ProductSupplier) => {
  try {
    await ElMessageBox.confirm(
      t('product.supplier.unbindConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await unbindProductSupplier(row.id)
    ElMessage.success(t('product.supplier.unbindSuccess'))
    loadBindings()
  } catch (error: any) {
    if (error !== 'cancel' && error?.response?.data?.message) {
      ElMessage.error(error.response.data.message)
    }
  }
}
</script>

<style scoped lang="scss">
.unit-label {
  margin-left: 8px;
  color: var(--el-text-color-secondary);
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