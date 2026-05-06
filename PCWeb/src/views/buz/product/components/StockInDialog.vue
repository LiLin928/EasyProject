<!-- src/views/buz/product/components/StockInDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="700px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('product.stock.stockIn') }}</span>
        <div class="dialog-actions">
          <el-button @click="handleClose">{{ t('common.button.cancel') }}</el-button>
          <el-button
            type="primary"
            :loading="saving"
            :disabled="!selectedProductSupplier && supplierProducts.length === 0"
            @click="handleSave"
          >
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
      <!-- Step 1: Supplier selection -->
      <el-form-item :label="t('product.stock.selectSupplier')" prop="supplierId">
        <el-select
          v-model="formData.supplierId"
          :placeholder="t('product.stock.selectSupplier')"
          :loading="suppliersLoading"
          filterable
          style="width: 100%"
          @change="handleSupplierChange"
        >
          <el-option
            v-for="item in suppliers"
            :key="item.id"
            :label="item.name"
            :value="item.id"
          >
            <span>{{ item.name }}</span>
            <span style="float: right; color: var(--el-text-color-secondary)">
              {{ item.unifiedCode || '-' }}
            </span>
          </el-option>
        </el-select>
      </el-form-item>

      <!-- Step 2: Product selection from supplier's bound products -->
      <el-form-item v-if="formData.supplierId" :label="t('product.stock.productName')" prop="productSupplierId">
        <el-select
          v-model="formData.productSupplierId"
          :placeholder="t('product.stock.selectProduct')"
          :loading="productsLoading"
          filterable
          style="width: 100%"
          @change="handleProductChange"
        >
          <el-option
            v-for="item in supplierProducts"
            :key="item.id"
            :label="item.product?.name"
            :value="item.id"
          >
            <div class="product-option">
              <span class="product-name">{{ item.product?.name }}</span>
              <span class="product-sku">{{ item.skuCode }}</span>
              <span class="product-price">¥{{ item.purchasePrice }}/件</span>
            </div>
          </el-option>
        </el-select>

        <!-- No products bound -->
        <div v-if="!productsLoading && supplierProducts.length === 0" class="no-product-tip">
          <el-icon><Warning /></el-icon>
          <span>该供应商暂无绑定商品</span>
          <el-button type="primary" link @click="showBindProduct = true">
            去绑定商品
          </el-button>
        </div>
      </el-form-item>

      <!-- Selected product info -->
      <template v-if="selectedProductSupplier">
        <el-form-item :label="t('product.supplier.skuCode')">
          <el-tag type="info">{{ selectedProductSupplier.skuCode }}</el-tag>
        </el-form-item>
        <el-form-item :label="t('product.stock.purchasePrice')">
          <span class="price">¥{{ selectedProductSupplier.purchasePrice?.toFixed(2) }}</span>
          <span class="unit-label">/件</span>
          <el-button link type="primary" style="margin-left: 16px" @click="showEditPrice = true">
            修改价格
          </el-button>
        </el-form-item>
        <el-form-item :label="t('product.stock.currentStock')">
          <span>{{ selectedProductSupplier.product?.stock || 0 }} 件</span>
        </el-form-item>
      </template>

      <el-divider />

      <!-- Quantity -->
      <el-form-item :label="t('product.stock.changeQuantity')" prop="quantity">
        <el-input-number
          v-model="formData.quantity"
          :min="1"
          :precision="0"
          placeholder="请输入入库数量"
          style="width: 200px"
        />
        <span class="unit-label">件</span>
      </el-form-item>

      <!-- Purchase price (can modify) -->
      <el-form-item v-if="showEditPrice" :label="t('product.stock.purchasePrice')" prop="purchasePrice">
        <el-input-number
          v-model="formData.purchasePrice"
          :min="0"
          :precision="2"
          placeholder="请输入采购价格"
          style="width: 200px"
        />
        <span class="unit-label">元/件</span>
        <el-button link @click="showEditPrice = false">取消修改</el-button>
      </el-form-item>

      <!-- Remark -->
      <el-form-item :label="t('product.stock.remark')">
        <el-input
          v-model="formData.remark"
          type="textarea"
          :rows="2"
          placeholder="请输入备注"
          maxlength="200"
        />
      </el-form-item>
    </el-form>

      </el-dialog>

  <!-- Bind product dialog -->
  <el-dialog
    v-model="showBindProduct"
    title="绑定新商品"
    width="600px"
    append-to-body
  >
    <el-form
      ref="bindFormRef"
      :model="bindFormData"
      :rules="bindFormRules"
      label-width="100px"
    >
      <!-- Select existing product or create new -->
      <el-radio-group v-model="bindType" style="margin-bottom: 16px">
        <el-radio value="existing">选择已有商品</el-radio>
        <el-radio value="new">新增商品</el-radio>
      </el-radio-group>

      <!-- Existing product -->
      <template v-if="bindType === 'existing'">
        <el-form-item label="选择商品" prop="productId">
          <el-select
            v-model="bindFormData.productId"
            placeholder="请选择商品"
            filterable
            style="width: 100%"
            @change="handleBindProductSelect"
          >
            <el-option
              v-for="item in allProducts"
              :key="item.id"
              :label="item.name"
              :value="item.id"
            >
              <span>{{ item.name }}</span>
              <span style="float: right; color: var(--el-text-color-secondary)">
                {{ item.skuCode }}
              </span>
            </el-option>
          </el-select>
        </el-form-item>
      </template>

      <!-- New product -->
      <template v-else>
        <el-form-item label="SKU码" prop="newSkuCode">
          <el-input v-model="bindFormData.newSkuCode" placeholder="请输入SKU码" maxlength="50">
            <template #append>
              <el-button @click="generateSkuCode">生成</el-button>
            </template>
          </el-input>
        </el-form-item>
        <el-form-item label="商品名称" prop="newProductName">
          <el-input v-model="bindFormData.newProductName" placeholder="请输入商品名称" maxlength="100" />
        </el-form-item>
        <el-form-item label="商品分类" prop="newCategoryId">
          <el-select v-model="bindFormData.newCategoryId" placeholder="请选择分类" style="width: 100%">
            <el-option v-for="cat in categories" :key="cat.id" :label="cat.name" :value="cat.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="销售价格">
          <el-input-number v-model="bindFormData.newPrice" :min="0" :precision="2" style="width: 200px" />
          <span class="unit-label">元</span>
        </el-form-item>
      </template>

      <el-divider />

      <!-- SKU Code for this binding -->
      <el-form-item :label="t('product.supplier.skuCode')" prop="bindSkuCode">
        <el-input v-model="bindFormData.bindSkuCode" placeholder="供应商商品SKU码" maxlength="50">
          <template #append>
            <el-button @click="generateBindSkuCode">生成</el-button>
          </template>
        </el-input>
      </el-form-item>

      <!-- Purchase price -->
      <el-form-item :label="t('product.stock.purchasePrice')" prop="bindPurchasePrice">
        <el-input-number
          v-model="bindFormData.bindPurchasePrice"
          :min="0"
          :precision="2"
          placeholder="采购价格"
          style="width: 200px"
        />
        <span class="unit-label">元/件</span>
      </el-form-item>

      <!-- Min order quantity -->
      <el-form-item :label="t('product.supplier.minOrderQty')">
        <el-input-number v-model="bindFormData.minOrderQty" :min="1" style="width: 200px" />
        <span class="unit-label">件</span>
      </el-form-item>
    </el-form>

    <template #footer>
      <el-button @click="showBindProduct = false">{{ t('common.button.cancel') }}</el-button>
      <el-button type="primary" :loading="bindSaving" @click="handleBindProduct">
        {{ t('common.button.confirm') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { Warning } from '@element-plus/icons-vue'
import type { FormInstance, FormRules } from 'element-plus'
import {
  getSupplierList,
  getProductList,
  getCategoryList,
  getSupplierProducts,
  bindProductSupplier,
  adjustStock,
  createProduct,
} from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { Supplier, Product, Category, ProductSupplier } from '@/types/product'

const { t } = useLocale()

const props = defineProps<{
  modelValue: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const formRef = ref<FormInstance>()
const bindFormRef = ref<FormInstance>()
const saving = ref(false)
const bindSaving = ref(false)
const suppliersLoading = ref(false)
const productsLoading = ref(false)
const showBindProduct = ref(false)
const showEditPrice = ref(false)
const bindType = ref<'existing' | 'new'>('existing')

const suppliers = ref<Supplier[]>([])
const supplierProducts = ref<ProductSupplier[]>([])
const allProducts = ref<Product[]>([])
const categories = ref<Category[]>([])

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const formData = reactive({
  supplierId: '',
  productSupplierId: '',
  quantity: 1,
  purchasePrice: 0,
  remark: '',
})

const bindFormData = reactive({
  productId: '',
  newSkuCode: '',
  newProductName: '',
  newCategoryId: '',
  newPrice: 0,
  bindSkuCode: '',
  bindPurchasePrice: 0,
  minOrderQty: 1,
})

const selectedProductSupplier = computed(() => {
  if (!formData.productSupplierId) return null
  return supplierProducts.value.find(ps => ps.id === formData.productSupplierId)
})

const formRules: FormRules = {
  supplierId: [
    { required: true, message: '请选择供应商', trigger: 'change' },
  ],
  productSupplierId: [
    { required: true, message: '请选择商品', trigger: 'change' },
  ],
  quantity: [
    { required: true, message: '请输入入库数量', trigger: 'blur' },
    { type: 'number', min: 1, message: '入库数量必须大于0', trigger: 'blur' },
  ],
}

const bindFormRules: FormRules = {
  productId: [
    {
      required: true,
      validator: (rule, value, callback) => {
        if (bindType.value === 'existing' && !value) {
          callback(new Error('请选择商品'))
        } else {
          callback()
        }
      },
      trigger: 'change',
    },
  ],
  newSkuCode: [
    {
      required: true,
      validator: (rule, value, callback) => {
        if (bindType.value === 'new' && !value) {
          callback(new Error('请输入SKU码'))
        } else {
          callback()
        }
      },
      trigger: 'blur',
    },
  ],
  newProductName: [
    {
      required: true,
      validator: (rule, value, callback) => {
        if (bindType.value === 'new' && !value) {
          callback(new Error('请输入商品名称'))
        } else {
          callback()
        }
      },
      trigger: 'blur',
    },
  ],
  newCategoryId: [
    {
      required: true,
      validator: (rule, value, callback) => {
        if (bindType.value === 'new' && !value) {
          callback(new Error('请选择分类'))
        } else {
          callback()
        }
      },
      trigger: 'change',
    },
  ],
  bindSkuCode: [
    { required: true, message: '请输入SKU码', trigger: 'blur' },
  ],
  bindPurchasePrice: [
    { required: true, message: '请输入采购价格', trigger: 'blur' },
    { type: 'number', min: 0.01, message: '采购价格必须大于0', trigger: 'blur' },
  ],
}

/**
 * Load suppliers
 */
const loadSuppliers = async () => {
  suppliersLoading.value = true
  try {
    const data = await getSupplierList({ status: 1, pageIndex: 1, pageSize: 1000 })
    suppliers.value = data.list || []
  } catch (error) {
    console.error('Failed to load suppliers:', error)
    suppliers.value = []
  } finally {
    suppliersLoading.value = false
  }
}

/**
 * Load all products for binding
 */
const loadAllProducts = async () => {
  try {
    const data = await getProductList({ pageIndex: 1, pageSize: 1000 })
    allProducts.value = data.list
  } catch (error) {
    // Error handled by interceptor
  }
}

/**
 * Load categories
 */
const loadCategories = async () => {
  try {
    const data = await getCategoryList()
    categories.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

/**
 * Load supplier's bound products
 */
const loadSupplierProducts = async (supplierId: string) => {
  productsLoading.value = true
  supplierProducts.value = []
  formData.productSupplierId = ''

  try {
    const data = await getSupplierProducts(supplierId)
    supplierProducts.value = data
  } catch (error) {
    // Error handled by interceptor
  } finally {
    productsLoading.value = false
  }
}

// Watch for dialog open
watch(visible, (val) => {
  if (val) {
    resetForm()
    loadSuppliers()
    loadAllProducts()
    loadCategories()
  }
})

/**
 * Reset form
 */
const resetForm = () => {
  formData.supplierId = ''
  formData.productSupplierId = ''
  formData.quantity = 1
  formData.purchasePrice = 0
  formData.remark = ''
  supplierProducts.value = []
  showEditPrice.value = false
}

/**
 * Handle supplier change
 */
const handleSupplierChange = (supplierId: string) => {
  loadSupplierProducts(supplierId)
  generateBindSkuCode()
}

/**
 * Handle product change
 */
const handleProductChange = (productSupplierId: string) => {
  const ps = supplierProducts.value.find(p => p.id === productSupplierId)
  if (ps) {
    formData.purchasePrice = ps.purchasePrice
  }
  showEditPrice.value = false
}

/**
 * Handle bind product select - auto fill SKU code
 */
const handleBindProductSelect = (productId: string) => {
  if (!productId) return
  const product = allProducts.value.find(p => p.id === productId)
  if (product && product.skuCode) {
    // Auto fill the bind SKU code with the product's SKU code
    bindFormData.bindSkuCode = product.skuCode
  }
}

/**
 * Generate SKU code for new product
 */
const generateSkuCode = () => {
  const timestamp = Date.now().toString(36).toUpperCase()
  const random = Math.random().toString(36).substring(2, 6).toUpperCase()
  bindFormData.newSkuCode = `SKU-${timestamp}-${random}`
}

/**
 * Generate SKU code for binding
 */
const generateBindSkuCode = () => {
  if (!formData.supplierId) return
  const supplier = suppliers.value.find(s => s.id === formData.supplierId)
  const prefix = supplier?.name?.substring(0, 3).toUpperCase() || 'SUP'
  const timestamp = Date.now().toString(36).toUpperCase()
  const random = Math.random().toString(36).substring(2, 5).toUpperCase()
  bindFormData.bindSkuCode = `${prefix}-${timestamp}-${random}`
}

/**
 * Handle close
 */
const handleClose = () => {
  visible.value = false
  formRef.value?.resetFields()
}

/**
 * Handle bind product
 */
const handleBindProduct = async () => {
  try {
    await bindFormRef.value?.validate()
  } catch {
    return
  }

  bindSaving.value = true
  try {
    let productId = bindFormData.productId

    // Create new product if needed
    if (bindType.value === 'new') {
      const newProduct = await createProduct({
        skuCode: bindFormData.newSkuCode,
        name: bindFormData.newProductName,
        categoryId: bindFormData.newCategoryId,
        price: bindFormData.newPrice || 0,
        stock: 0,
        image: '/static/images/products/default.jpg',
      })
      productId = newProduct.id
    }

    // Bind supplier to product
    await bindProductSupplier({
      productId,
      supplierId: formData.supplierId,
      skuCode: bindFormData.bindSkuCode,
      purchasePrice: bindFormData.bindPurchasePrice,
      minOrderQty: bindFormData.minOrderQty,
      isDefault: true,
    })

    ElMessage.success('绑定成功')
    showBindProduct.value = false

    // Reset bind form
    bindFormData.productId = ''
    bindFormData.newSkuCode = ''
    bindFormData.newProductName = ''
    bindFormData.newCategoryId = ''
    bindFormData.newPrice = 0
    bindFormData.bindSkuCode = ''
    bindFormData.bindPurchasePrice = 0
    bindFormData.minOrderQty = 1

    // Reload supplier products
    await loadSupplierProducts(formData.supplierId)
  } catch (error) {
    // Error handled by interceptor
  } finally {
    bindSaving.value = false
  }
}

/**
 * Handle save
 */
const handleSave = async () => {
  try {
    await formRef.value?.validate()
  } catch {
    return
  }

  if (!selectedProductSupplier.value) {
    ElMessage.warning('请选择商品')
    return
  }

  saving.value = true
  try {
    // Use modified price or original price
    const purchasePrice = showEditPrice.value && formData.purchasePrice > 0
      ? formData.purchasePrice
      : selectedProductSupplier.value.purchasePrice

    await adjustStock({
      productId: selectedProductSupplier.value.productId,
      type: 'in',
      quantity: formData.quantity,
      supplierId: formData.supplierId,
      purchasePrice,
      skuCode: selectedProductSupplier.value.skuCode,
      remark: formData.remark || '入库',
    })

    ElMessage.success('入库成功')
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

.unit-label {
  margin-left: 8px;
  color: var(--el-text-color-secondary);
}

.price {
  color: var(--el-color-danger);
  font-weight: 600;
  font-size: 16px;
}

.no-product-tip {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px;
  background: var(--el-fill-color-light);
  border-radius: 4px;
  margin-top: 8px;
  color: var(--el-text-color-secondary);
}

.product-option {
  display: flex;
  align-items: center;
  width: 100%;

  .product-name {
    flex: 1;
  }

  .product-sku {
    color: var(--el-text-color-secondary);
    font-size: 12px;
    margin: 0 16px;
  }

  .product-price {
    color: var(--el-color-danger);
    font-size: 12px;
  }
}
</style>