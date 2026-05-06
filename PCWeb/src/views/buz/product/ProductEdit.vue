<!-- src/views/buz/product/ProductEdit.vue -->
<template>
  <div class="product-edit">
    <!-- Breadcrumb -->
    <el-breadcrumb separator="/">
      <el-breadcrumb-item :to="{ path: '/buz/product/list' }">
        {{ t('product.list.title') }}
      </el-breadcrumb-item>
      <el-breadcrumb-item>
        {{ isEdit ? t('product.edit.editTitle') : t('product.edit.title') }}
      </el-breadcrumb-item>
    </el-breadcrumb>

    <el-card shadow="never" class="edit-card">
      <template #header>
        <div class="card-header">
          <span>{{ isEdit ? t('product.edit.editTitle') : t('product.edit.title') }}</span>
          <div class="header-buttons">
            <!-- 审核按钮区域 -->
            <template v-if="isEdit && productData">
              <!-- 待提交/已拒绝/已撤回状态：显示提交审核按钮 -->
              <template v-if="showSubmitAuditButton">
                <el-button type="warning" @click="handleShowSubmitAudit">
                  <el-icon><Upload /></el-icon>
                  {{ t('product.audit.submitAudit') }}
                </el-button>
              </template>
              <!-- 待审核状态：显示撤回审核按钮 -->
              <template v-if="productData.auditStatus === ProductAuditStatus.Pending">
                <el-button type="info" @click="handleWithdrawAudit">
                  <el-icon><Download /></el-icon>
                  {{ t('product.audit.withdrawAudit') }}
                </el-button>
              </template>
              <!-- 已通过状态：显示已通过标签 -->
              <template v-if="productData.auditStatus === ProductAuditStatus.Passed">
                <el-tag type="success" size="large">
                  <el-icon><CircleCheck /></el-icon>
                  {{ t('product.audit.auditPassed') }}
                </el-tag>
              </template>
              <!-- 已拒绝状态：显示驳回原因标签 -->
              <template v-if="productData.auditStatus === ProductAuditStatus.Rejected && productData.auditRemark">
                <el-tooltip :content="productData.auditRemark" placement="top">
                  <el-tag type="danger" size="large">
                    <el-icon><CircleClose /></el-icon>
                    {{ t('product.audit.auditRejected') }}: {{ productData.auditRemark }}
                  </el-tag>
                </el-tooltip>
              </template>
            </template>
            <el-button type="primary" :loading="saving" @click="handleSave">
              <el-icon><Check /></el-icon>
              {{ t('product.edit.save') }}
            </el-button>
            <el-button @click="handleCancel">
              {{ t('product.edit.backToList') }}
            </el-button>
          </div>
        </div>
      </template>
      <div v-loading="loading" class="form-container">
        <el-form
          ref="formRef"
          :model="formData"
          :rules="formRules"
          label-width="120px"
        >
          <!-- Basic Information -->
          <div class="section-title">{{ t('product.edit.basicInfo') }}</div>
          <el-form-item :label="t('product.edit.skuCode')" prop="skuCode">
            <el-input
              v-model="formData.skuCode"
              :placeholder="t('product.edit.skuCodePlaceholder')"
              maxlength="50"
              show-word-limit
            >
              <template v-if="!isEdit" #append>
                <el-button @click="generateSkuCode">{{ t('product.edit.skuCodeAutoGenerate') }}</el-button>
              </template>
            </el-input>
          </el-form-item>
          <el-form-item :label="t('product.edit.name')" prop="name">
            <el-input
              v-model="formData.name"
              :placeholder="t('product.edit.namePlaceholder')"
              maxlength="100"
              show-word-limit
            />
          </el-form-item>
          <el-form-item :label="t('product.edit.description')" prop="description">
            <el-input
              v-model="formData.description"
              type="textarea"
              :rows="3"
              :placeholder="t('product.edit.descriptionPlaceholder')"
              maxlength="500"
              show-word-limit
            />
          </el-form-item>
          <el-form-item :label="t('product.edit.category')" prop="categoryId">
            <el-select
              v-model="formData.categoryId"
              :placeholder="t('product.edit.categoryPlaceholder')"
              style="width: 100%"
              clearable
            >
              <el-option
                v-for="cat in categories"
                :key="cat.id"
                :label="cat.name"
                :value="cat.id"
              />
            </el-select>
          </el-form-item>

          <!-- Price & Stock -->
          <div class="section-title">{{ t('product.edit.priceStock') }}</div>
          <el-form-item :label="t('product.edit.price')" prop="price">
            <el-input-number
              v-model="formData.price"
              :min="0"
              :precision="2"
              :placeholder="t('product.edit.pricePlaceholder')"
              style="width: 200px"
            />
            <span class="unit-label">元</span>
          </el-form-item>
          <el-form-item :label="t('product.edit.originalPrice')" prop="originalPrice">
            <el-input-number
              v-model="formData.originalPrice"
              :min="0"
              :precision="2"
              :placeholder="t('product.edit.originalPricePlaceholder')"
              style="width: 200px"
            />
            <span class="unit-label">元</span>
          </el-form-item>
          <el-form-item :label="t('product.edit.stock')" prop="stock">
            <el-input-number
              v-model="formData.stock"
              :min="0"
              :precision="0"
              :placeholder="t('product.edit.stockPlaceholder')"
              style="width: 200px"
            />
            <span class="unit-label">件</span>
          </el-form-item>
          <el-form-item :label="t('product.edit.alertThreshold')" prop="alertThreshold">
            <el-input-number
              v-model="formData.alertThreshold"
              :min="0"
              :precision="0"
              :placeholder="t('product.edit.alertThresholdPlaceholder')"
              style="width: 200px"
            />
            <span class="unit-label">件</span>
            <div class="form-tip">{{ t('product.edit.alertThresholdTip') }}</div>
          </el-form-item>

          <!-- Supplier (only for new product) -->
          <template v-if="!isEdit">
            <div class="section-title">{{ t('product.supplier.title') }}</div>
            <el-form-item :label="t('product.stock.selectSupplier')">
              <el-select
                v-model="supplierData.supplierId"
                :placeholder="t('product.stock.selectSupplier')"
                :loading="suppliersLoading"
                filterable
                clearable
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
            <el-form-item v-if="supplierData.supplierId" :label="t('product.supplier.skuCode')">
              <el-input
                v-model="supplierData.skuCode"
                :placeholder="t('product.supplier.skuCodePlaceholder')"
                maxlength="50"
              >
                <template #append>
                  <el-button @click="generateSupplierSkuCode">生成</el-button>
                </template>
              </el-input>
            </el-form-item>
            <el-form-item v-if="supplierData.supplierId" :label="t('product.stock.purchasePrice')">
              <el-input-number
                v-model="supplierData.purchasePrice"
                :min="0"
                :precision="2"
                :placeholder="t('product.stock.purchasePricePlaceholder')"
                style="width: 200px"
              />
              <span class="unit-label">元/件</span>
            </el-form-item>
            <el-form-item v-if="supplierData.supplierId" :label="t('product.supplier.minOrderQty')">
              <el-input-number
                v-model="supplierData.minOrderQty"
                :min="1"
                :precision="0"
                :placeholder="t('product.supplier.minOrderQtyPlaceholder')"
                style="width: 200px"
              />
              <span class="unit-label">件</span>
            </el-form-item>
          </template>

          <!-- Product Images -->
          <div class="section-title">{{ t('product.edit.productImage') }}</div>
          <el-form-item :label="t('product.edit.mainImage')" prop="image">
            <ImageUpload
              v-model="formData.image"
              :placeholder="t('product.edit.mainImagePlaceholder')"
            />
          </el-form-item>
          <el-form-item :label="t('product.edit.detailImages')" prop="images">
            <ImageUpload
              v-model="formData.images"
              multiple
              :limit="5"
            />
          </el-form-item>

          <!-- Product Tags -->
          <div class="section-title">{{ t('product.edit.productTag') }}</div>
          <el-form-item :label="t('product.edit.isHot')">
            <el-switch v-model="formData.isHot" />
          </el-form-item>
          <el-form-item :label="t('product.edit.isNew')">
            <el-switch v-model="formData.isNew" />
          </el-form-item>

          <!-- Product Detail -->
          <div class="section-title">{{ t('product.edit.productDetail') }}</div>
          <el-form-item :label="t('product.edit.detail')" prop="detail">
            <WangEditor
              v-model="formData.detail"
              :height="400"
              :placeholder="t('product.edit.detailPlaceholder')"
            />
          </el-form-item>
        </el-form>
      </div>
    </el-card>

    <!-- 提交审核弹窗 -->
    <SubmitAuditDialog
      v-model="submitAuditDialogVisible"
      :product-id="productData?.id || ''"
      @success="handleSubmitAuditSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Check, Upload, Download, CircleCheck, CircleClose } from '@element-plus/icons-vue'
import type { FormInstance, FormRules } from 'element-plus'
import WangEditor from '@/components/WangEditor/index.vue'
import ImageUpload from '@/components/ImageUpload/index.vue'
import SubmitAuditDialog from './components/SubmitAuditDialog.vue'
import {
  getProductDetail,
  createProduct,
  updateProduct,
  getCategoryList,
  getSupplierList,
  bindProductSupplier,
  withdrawProductAudit,
} from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { Category, Supplier, CreateProductParams, UpdateProductParams, Product } from '@/types'
import { ProductAuditStatus } from '@/types/businessAuditPoint'

const router = useRouter()
const route = useRoute()
const { t } = useLocale()

const loading = ref(false)
const saving = ref(false)
const isEdit = ref(false)
const formRef = ref<FormInstance | null>(null)
const categories = ref<Category[]>([])
const suppliers = ref<Supplier[]>([])
const suppliersLoading = ref(false)
const productData = ref<Product | null>(null)
const submitAuditDialogVisible = ref(false)

const formData = reactive<CreateProductParams>({
  skuCode: '',
  name: '',
  description: '',
  price: 0,
  originalPrice: undefined,
  image: '',
  images: [],
  categoryId: '',
  stock: 0,
  alertThreshold: 10,
  isHot: false,
  isNew: false,
  detail: '',
})

const supplierData = reactive({
  supplierId: '',
  skuCode: '',
  purchasePrice: 0,
  minOrderQty: 1,
})

/**
 * 是否显示提交审核按钮
 * 状态 0（待提交）、2（已拒绝）、4（已撤回）时显示
 */
const showSubmitAuditButton = computed(() => {
  if (!productData.value) return false
  const status = productData.value.auditStatus
  return status === ProductAuditStatus.WaitSubmit ||
         status === ProductAuditStatus.Rejected ||
         status === ProductAuditStatus.Withdrawn ||
         status === undefined
})

const formRules: FormRules = {
  skuCode: [
    { required: true, message: '请输入SKU码', trigger: 'blur' },
    { min: 2, max: 50, message: 'SKU码长度应在 2-50 个字符之间', trigger: 'blur' },
  ],
  name: [
    { required: true, message: '请输入商品名称', trigger: 'blur' },
    { min: 2, max: 100, message: '商品名称长度应在 2-100 个字符之间', trigger: 'blur' },
  ],
  categoryId: [
    { required: true, message: '请选择商品分类', trigger: 'change' },
  ],
  price: [
    { required: true, message: '请输入商品价格', trigger: 'blur' },
    { type: 'number', min: 0, message: '价格不能小于 0', trigger: 'blur' },
  ],
  stock: [
    { required: true, message: '请输入库存数量', trigger: 'blur' },
    { type: 'number', min: 0, message: '库存不能小于 0', trigger: 'blur' },
  ],
  image: [
    { required: true, message: '请上传商品主图', trigger: 'change' },
  ],
}

onMounted(async () => {
  await loadCategories()
  await loadSuppliers()
  const id = route.params.id as string
  if (id) {
    isEdit.value = true
    await loadProduct(id)
  }
})

/**
 * Load categories for select
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
 * Load suppliers for select
 */
const loadSuppliers = async () => {
  suppliersLoading.value = true
  try {
    const data = await getSupplierList({ status: 1, pageIndex: 1, pageSize: 1000 })
    suppliers.value = data.list || []
  } catch (error) {
    // Error handled by interceptor
  } finally {
    suppliersLoading.value = false
  }
}

/**
 * Load product detail for edit mode
 */
const loadProduct = async (id: string) => {
  loading.value = true
  try {
    const data = await getProductDetail(id)
    // 保存完整的商品数据用于审核状态判断
    productData.value = data
    formData.skuCode = data.skuCode
    formData.name = data.name
    formData.description = data.description || ''
    formData.price = data.price
    formData.originalPrice = data.originalPrice
    formData.image = data.image
    formData.images = data.images || []
    formData.categoryId = data.categoryId
    formData.stock = data.stock
    formData.alertThreshold = data.alertThreshold || 10
    formData.isHot = data.isHot || false
    formData.isNew = data.isNew || false
    formData.detail = data.detail || ''
  } catch (error) {
    ElMessage.error('加载商品详情失败')
    router.push('/buz/product/list')
  } finally {
    loading.value = false
  }
}

/**
 * Generate SKU code automatically
 */
const generateSkuCode = () => {
  const timestamp = Date.now().toString(36).toUpperCase()
  const random = Math.random().toString(36).substring(2, 6).toUpperCase()
  formData.skuCode = `SKU-${timestamp}-${random}`
}

/**
 * Handle supplier change
 */
const handleSupplierChange = (supplierId: string) => {
  if (supplierId) {
    generateSupplierSkuCode()
    supplierData.purchasePrice = 0
    supplierData.minOrderQty = 1
  } else {
    supplierData.skuCode = ''
    supplierData.purchasePrice = 0
    supplierData.minOrderQty = 1
  }
}

/**
 * Generate supplier SKU code
 */
const generateSupplierSkuCode = () => {
  if (!supplierData.supplierId) return
  const supplier = suppliers.value.find(s => s.id === supplierData.supplierId)
  const prefix = supplier?.name?.substring(0, 3).toUpperCase() || 'SUP'
  const timestamp = Date.now().toString(36).toUpperCase()
  const random = Math.random().toString(36).substring(2, 5).toUpperCase()
  supplierData.skuCode = `${prefix}-${timestamp}-${random}`
}

/**
 * Handle save
 */
const handleSave = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch {
    return
  }

  saving.value = true
  try {
    // Filter out empty images
    const params: CreateProductParams = {
      ...formData,
      images: formData.images.filter(img => img && img.trim()),
    }

    let productId: string

    if (isEdit.value) {
      const id = route.params.id as string
      productId = id
      const updateParams: UpdateProductParams = { id, ...params }
      await updateProduct(updateParams)
    } else {
      const result = await createProduct(params)
      productId = result.id

      // Bind supplier if selected
      if (supplierData.supplierId && supplierData.skuCode && supplierData.purchasePrice > 0) {
        await bindProductSupplier({
          productId,
          supplierId: supplierData.supplierId,
          skuCode: supplierData.skuCode,
          purchasePrice: supplierData.purchasePrice,
          minOrderQty: supplierData.minOrderQty,
          isDefault: true,
        })
      }
    }

    ElMessage.success(t('product.edit.saveSuccess'))
    router.push('/buz/product/list')
  } catch (error) {
    ElMessage.error('保存失败')
  } finally {
    saving.value = false
  }
}

/**
 * Handle cancel - go back to previous page
 */
const handleCancel = () => {
  router.back()
}

/**
 * 显示提交审核弹窗
 */
const handleShowSubmitAudit = () => {
  submitAuditDialogVisible.value = true
}

/**
 * 撤回审核
 */
const handleWithdrawAudit = async () => {
  if (!productData.value) return

  try {
    await ElMessageBox.confirm(
      t('product.audit.withdrawConfirm'),
      t('common.message.warning'),
      { type: 'warning' }
    )
    await withdrawProductAudit(productData.value.id)
    ElMessage.success(t('product.audit.withdrawSuccess'))
    // 重新加载商品数据更新状态
    await loadProduct(productData.value.id)
  } catch (error) {
    // 用户取消或请求失败
  }
}

/**
 * 提交审核成功后回调
 */
const handleSubmitAuditSuccess = async () => {
  if (productData.value) {
    await loadProduct(productData.value.id)
  }
}
</script>

<style scoped lang="scss">
.product-edit {
  padding: 20px;

  .edit-card {
    margin-top: 16px;

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;

      .header-buttons {
        display: flex;
        gap: 12px;
      }
    }
  }

  .form-container {
    max-width: 800px;

    .section-title {
      font-size: 16px;
      font-weight: 600;
      color: #303133;
      margin: 20px 0 16px;
      padding-bottom: 8px;
      border-bottom: 1px solid #ebeef5;

      &:first-of-type {
        margin-top: 0;
      }
    }

    .unit-label {
      margin-left: 8px;
      color: #606266;
    }

    .form-tip {
      font-size: 12px;
      color: var(--el-text-color-placeholder);
      margin-top: 4px;
    }
  }
}
</style>