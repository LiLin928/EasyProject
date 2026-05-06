<template>
  <div class="order-edit">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ isEdit ? t('order.edit.title') : t('order.create.title') }}</span>
          <el-button @click="handleCancel">
            {{ t('common.button.back') }}
          </el-button>
        </div>
      </template>

      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
        class="order-form"
      >
        <!-- 用户选择 -->
        <el-form-item :label="t('order.create.user')" prop="userId">
          <el-select
            v-model="formData.userId"
            :placeholder="t('order.create.userPlaceholder')"
            filterable
            style="width: 100%"
            @change="handleUserChange"
          >
            <el-option
              v-for="user in userList"
              :key="user.id"
              :label="`${user.realName || user.userName} (${user.userName})`"
              :value="String(user.id)"
            />
          </el-select>
        </el-form-item>

        <!-- 用户信息展示 -->
        <el-form-item :label="t('order.create.userPhone')">
          <el-input v-model="formData.userPhone" disabled />
        </el-form-item>

        <!-- 收货信息 -->
        <el-divider content-position="left">{{ t('order.create.receiverInfo') }}</el-divider>

        <el-form-item :label="t('order.create.selectAddress')">
          <el-select
            v-model="formData.addressId"
            :placeholder="t('order.create.selectAddressPlaceholder')"
            :loading="addressLoading"
            clearable
            style="width: 100%"
            @change="handleAddressChange"
          >
            <el-option
              v-for="addr in addressList"
              :key="addr.id"
              :label="addr.fullAddress || `${addr.province} ${addr.city} ${addr.district} ${addr.detail}`"
              :value="addr.id"
            >
              <div class="address-option">
                <span class="address-name">{{ addr.name }} ({{ addr.phone }})</span>
                <span class="address-full">{{ addr.fullAddress || `${addr.province} ${addr.city} ${addr.district}` }}</span>
                <el-tag v-if="addr.isDefault" type="success" size="small" style="margin-left: 8px">默认</el-tag>
              </div>
            </el-option>
          </el-select>
          <div v-if="addressLoading" class="address-tip">
            <el-icon class="is-loading"><Loading /></el-icon>
            <span>正在加载地址...</span>
          </div>
          <div v-if="!addressLoading && addressList.length === 0 && formData.userId" class="address-tip">
            <el-icon><Warning /></el-icon>
            <span>该用户暂无收货地址，请手动填写</span>
          </div>
        </el-form-item>

        <el-form-item :label="t('order.create.receiverName')" prop="receiverName">
          <el-input v-model="formData.receiverName" :placeholder="t('order.create.receiverNamePlaceholder')" />
        </el-form-item>

        <el-form-item :label="t('order.create.receiverPhone')" prop="receiverPhone">
          <el-input v-model="formData.receiverPhone" :placeholder="t('order.create.receiverPhonePlaceholder')" />
        </el-form-item>

        <el-form-item :label="t('order.create.country')" prop="country">
          <el-select
            v-model="formData.country"
            :placeholder="t('order.create.countryPlaceholder')"
            style="width: 100%"
            @change="handleCountryChange"
          >
            <el-option
              v-for="c in countries"
              :key="c.value"
              :label="c.label"
              :value="c.value"
            />
          </el-select>
        </el-form-item>

        <el-form-item v-if="formData.country === '中国'" :label="t('order.create.region')" prop="region">
          <el-cascader
            v-model="formData.region"
            :options="chinaRegions"
            :placeholder="t('order.create.regionPlaceholder')"
            :props="{ expandTrigger: 'hover' }"
            style="width: 100%"
            clearable
            @change="handleRegionChange"
          />
        </el-form-item>

        <el-form-item v-if="formData.country && formData.country !== '中国'" :label="t('order.create.provinceCity')">
          <el-input
            v-model="formData.foreignRegion"
            :placeholder="t('order.create.provinceCityPlaceholder')"
          />
        </el-form-item>

        <el-form-item :label="t('order.create.detailAddress')" prop="detailAddress">
          <el-input
            v-model="formData.detailAddress"
            type="textarea"
            :rows="2"
            :placeholder="t('order.create.detailAddressPlaceholder')"
          />
        </el-form-item>

        <!-- 商品选择 -->
        <el-divider content-position="left">{{ t('order.create.productList') }}</el-divider>

        <el-form-item>
          <el-button type="primary" @click="handleAddProduct">
            <el-icon><Plus /></el-icon>
            {{ t('order.create.addProduct') }}
          </el-button>
        </el-form-item>

        <el-table :data="formData.items" border class="product-table">
          <el-table-column :label="t('order.detail.productName')" prop="productName" min-width="200" />
          <el-table-column :label="t('order.detail.price')" width="120">
            <template #default="{ row }">
              ¥{{ row.price.toFixed(2) }}
            </template>
          </el-table-column>
          <el-table-column :label="t('order.detail.quantity')" width="150">
            <template #default="{ row, $index }">
              <el-input-number
                v-model="row.quantity"
                :min="1"
                :max="999"
                size="small"
                @change="handleQuantityChange($index)"
              />
            </template>
          </el-table-column>
          <el-table-column :label="t('order.detail.subtotal')" width="120">
            <template #default="{ row }">
              ¥{{ (row.price * row.quantity).toFixed(2) }}
            </template>
          </el-table-column>
          <el-table-column :label="t('order.create.operation')" width="80" align="center">
            <template #default="{ $index }">
              <el-button link type="danger" @click="handleRemoveProduct($index)">
                {{ t('common.button.delete') }}
              </el-button>
            </template>
          </el-table-column>
        </el-table>

        <!-- 金额汇总 -->
        <div class="amount-summary">
          <span>{{ t('order.create.totalAmount') }}：</span>
          <span class="amount">¥{{ totalAmount.toFixed(2) }}</span>
        </div>

        <!-- 备注 -->
        <el-form-item :label="t('order.create.remark')">
          <el-input
            v-model="formData.remark"
            type="textarea"
            :rows="2"
            :placeholder="t('order.create.remarkPlaceholder')"
          />
        </el-form-item>

        <!-- 操作按钮 -->
        <el-form-item>
          <el-button type="primary" :loading="submitting" @click="handleSubmit">
            {{ t('common.button.submit') }}
          </el-button>
          <el-button @click="handleCancel">
            {{ t('common.button.cancel') }}
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 商品选择对话框 -->
    <el-dialog
      v-model="productDialogVisible"
      :title="t('order.create.selectProduct')"
      width="800px"
      destroy-on-close
    >
      <el-form :model="productQuery" :inline="true" class="search-form">
        <el-form-item :label="t('product.list.name')">
          <el-input
            v-model="productQuery.name"
            :placeholder="t('product.list.namePlaceholder')"
            clearable
            @keyup.enter="handleProductSearch"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleProductSearch">
            {{ t('common.button.search') }}
          </el-button>
        </el-form-item>
      </el-form>

      <el-table
        ref="productTableRef"
        :data="productList"
        border
        @selection-change="handleProductSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column :label="t('product.list.name')" prop="name" min-width="180" />
        <el-table-column :label="t('product.list.skuCode')" prop="skuCode" width="140" />
        <el-table-column :label="t('product.list.price')" width="120">
          <template #default="{ row }">
            ¥{{ row.price.toFixed(2) }}
          </template>
        </el-table-column>
        <el-table-column :label="t('product.list.stock')" prop="stock" width="100" />
      </el-table>

      <div class="pagination-wrapper">
        <el-pagination
          v-model:current-page="productQuery.pageIndex"
          v-model:page-size="productQuery.pageSize"
          :total="productTotal"
          layout="total, prev, pager, next"
          @current-change="handleProductSearch"
        />
      </div>

      <template #footer>
        <el-button @click="productDialogVisible = false">
          {{ t('common.button.cancel') }}
        </el-button>
        <el-button type="primary" :disabled="selectedProducts.length === 0" @click="handleConfirmProducts">
          {{ t('common.button.confirm') }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Plus, Loading, Warning } from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import { getUserList } from '@/api/user'
import { getProductList } from '@/api/buz/productApi'
import { createOrder, updateOrder, getOrderDetail } from '@/api/buz/orderApi'
import { getCustomerAddressList } from '@/api/buz/customerApi'
import { chinaRegions, countries } from '@/data/chinaRegions'
import type { FormInstance, FormRules } from 'element-plus'
import type { UserInfo, CustomerAddress } from '@/types'
import type { Product } from '@/types'

const { t } = useLocale()
const router = useRouter()
const route = useRoute()

// 判断是否编辑模式
const isEdit = computed(() => !!route.params.id)
const orderId = computed(() => route.params.id as string)

// 表单引用
const formRef = ref<FormInstance>()
const submitting = ref(false)

// 用户列表
const userList = ref<UserInfo[]>([])

// 地址列表
const addressList = ref<CustomerAddress[]>([])
const addressLoading = ref(false)

// 表单数据
const formData = reactive({
  userId: '',
  userName: '',
  userPhone: '',
  addressId: '',
  receiverName: '',
  receiverPhone: '',
  country: '中国',
  region: [] as string[], // 联级选择: [省份, 城市, 区县]
  foreignRegion: '', // 非中国地区的省/城市
  detailAddress: '', // 详细地址
  receiverAddress: '', // 完整地址（用于提交）
  items: [] as { productId: string; productName: string; productImage: string; price: number; quantity: number }[],
  remark: '',
})

// 表单验证规则
const formRules: FormRules = {
  userId: [
    { required: true, message: t('order.create.userRequired'), trigger: 'change' },
  ],
  receiverName: [
    { required: true, message: t('order.create.receiverNameRequired'), trigger: 'blur' },
  ],
  receiverPhone: [
    { required: true, message: t('order.create.receiverPhoneRequired'), trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: t('common.validation.phone'), trigger: 'blur' },
  ],
  country: [
    { required: true, message: t('order.create.countryRequired'), trigger: 'change' },
  ],
  detailAddress: [
    { required: true, message: t('order.create.detailAddressRequired'), trigger: 'blur' },
  ],
}

// 计算总金额
const totalAmount = computed(() => {
  return formData.items.reduce((sum, item) => sum + item.price * item.quantity, 0)
})

// 商品选择对话框
const productDialogVisible = ref(false)
const productTableRef = ref()
const productQuery = reactive({
  pageIndex: 1,
  pageSize: 10,
  name: '',
})
const productList = ref<Product[]>([])
const productTotal = ref(0)
const selectedProducts = ref<Product[]>([])

// 加载用户列表
const loadUserList = async () => {
  try {
    const res = await getUserList({ pageIndex: 1, pageSize: 1000, status: 1 })
    userList.value = res.list
  } catch (error) {
    console.error('Failed to load users:', error)
  }
}

// 用户选择变更
const handleUserChange = (userId: string) => {
  const user = userList.value.find((u) => String(u.id) === userId)
  if (user) {
    formData.userName = user.realName || user.userName
    formData.userPhone = user.phone || ''
  }
  // 清空地址相关
  formData.addressId = ''
  formData.receiverName = ''
  formData.receiverPhone = ''
  formData.country = '中国'
  formData.region = []
  formData.foreignRegion = ''
  formData.detailAddress = ''
  formData.receiverAddress = ''
  addressList.value = []
  // 加载用户地址列表
  loadAddressList(userId)
}

// 加载用户地址列表
const loadAddressList = async (userId: string) => {
  if (!userId) return
  addressLoading.value = true
  try {
    const res = await getCustomerAddressList(userId)
    addressList.value = res || []
    // 如果有默认地址，自动选中
    const defaultAddr = addressList.value.find((a) => a.isDefault)
    if (defaultAddr) {
      formData.addressId = defaultAddr.id
      fillAddressInfo(defaultAddr)
    }
  } catch (error) {
    console.error('Failed to load addresses:', error)
    addressList.value = []
  } finally {
    addressLoading.value = false
  }
}

// 地址选择变更
const handleAddressChange = (addressId: string) => {
  if (!addressId) {
    formData.receiverName = ''
    formData.receiverPhone = ''
    formData.country = '中国'
    formData.region = []
    formData.foreignRegion = ''
    formData.detailAddress = ''
    formData.receiverAddress = ''
    return
  }
  const addr = addressList.value.find((a) => a.id === addressId)
  if (addr) {
    fillAddressInfo(addr)
  }
}

// 国家选择变更
const handleCountryChange = (country: string) => {
  formData.region = []
  formData.foreignRegion = ''
  formData.detailAddress = ''
  updateReceiverAddress()
}

// 地区选择变更
const handleRegionChange = (region: string[]) => {
  updateReceiverAddress()
}

// 更新完整地址
const updateReceiverAddress = () => {
  if (formData.country === '中国' && formData.region.length > 0) {
    formData.receiverAddress = `${formData.region.join(' ')} ${formData.detailAddress}`
  } else if (formData.country && formData.country !== '中国') {
    formData.receiverAddress = `${formData.country} ${formData.foreignRegion} ${formData.detailAddress}`
  } else {
    formData.receiverAddress = formData.detailAddress
  }
}

// 填充地址信息（从已有地址选择）
const fillAddressInfo = (addr: CustomerAddress) => {
  formData.receiverName = addr.name
  formData.receiverPhone = addr.phone

  // 解析地址为地区字段
  // 地址格式：省份 城市 区县 详细地址
  const fullAddr = addr.fullAddress || ''
  const detail = addr.detail || ''

  // 尝试匹配中国地址格式
  formData.country = '中国'

  // 根据地址中的省市区信息匹配
  if (addr.province && addr.city && addr.district) {
    formData.region = [addr.province, addr.city, addr.district]
  } else if (addr.province && addr.city) {
    formData.region = [addr.province, addr.city]
  } else {
    // 从完整地址尝试解析
    const parts = fullAddr.split(' ').filter(p => p)
    if (parts.length >= 3) {
      formData.region = parts.slice(0, 3)
    } else if (parts.length >= 2) {
      formData.region = parts.slice(0, 2)
    } else {
      formData.region = []
    }
  }

  formData.foreignRegion = ''
  formData.detailAddress = detail
  formData.receiverAddress = fullAddr || `${formData.region.join(' ')} ${formData.detailAddress}`
}

// 解析地址字符串为地区字段
const parseAddress = (address: string) => {
  if (!address) {
    formData.country = '中国'
    formData.region = []
    formData.foreignRegion = ''
    formData.detailAddress = ''
    return
  }

  // 尝试匹配国家
  const countryMatch = countries.find(c => address.startsWith(c.value))
  if (countryMatch && countryMatch.value !== '中国') {
    formData.country = countryMatch.value
    const remaining = address.slice(countryMatch.value.length).trim()
    // 简单分割为省/城市和详细地址
    const parts = remaining.split(' ')
    if (parts.length > 1) {
      formData.foreignRegion = parts[0]
      formData.detailAddress = parts.slice(1).join(' ')
    } else {
      formData.foreignRegion = ''
      formData.detailAddress = remaining
    }
    formData.region = []
  } else {
    // 中国地址，尝试匹配省份
    formData.country = '中国'
    const provinceMatch = chinaRegions.find(p => address.includes(p.value))
    if (provinceMatch) {
      const province = provinceMatch.value
      const afterProvince = address.slice(address.indexOf(province) + province.length).trim()

      // 尝试匹配城市
      const cityMatch = provinceMatch.children?.find(c => afterProvince.includes(c.value))
      if (cityMatch) {
        const city = cityMatch.value
        const afterCity = afterProvince.slice(afterProvince.indexOf(city) + city.length).trim()

        // 尝试匹配区县
        const districtMatch = cityMatch.children?.find(d => afterCity.startsWith(d.value))
        if (districtMatch) {
          formData.region = [province, city, districtMatch.value]
          formData.detailAddress = afterCity.slice(districtMatch.value.length).trim()
        } else {
          formData.region = [province, city]
          formData.detailAddress = afterCity
        }
      } else {
        formData.region = [province]
        formData.detailAddress = afterProvince
      }
    } else {
      // 无法匹配，将整个地址作为详细地址
      formData.region = []
      formData.detailAddress = address
    }
    formData.foreignRegion = ''
  }
  formData.receiverAddress = address
}

// 添加商品
const handleAddProduct = () => {
  productDialogVisible.value = true
  handleProductSearch()
}

// 商品搜索
const handleProductSearch = async () => {
  try {
    const res = await getProductList({
      pageIndex: productQuery.pageIndex,
      pageSize: productQuery.pageSize,
      name: productQuery.name || undefined,
    })
    productList.value = res.list
    productTotal.value = res.total
  } catch (error) {
    console.error('Failed to load products:', error)
  }
}

// 商品选择变更
const handleProductSelectionChange = (selection: Product[]) => {
  selectedProducts.value = selection
}

// 确认选择商品
const handleConfirmProducts = () => {
  for (const product of selectedProducts.value) {
    // 检查是否已添加
    if (formData.items.some((item) => item.productId === product.id)) {
      continue
    }
    formData.items.push({
      productId: product.id,
      productName: product.name,
      productImage: product.image,
      price: product.price,
      quantity: 1,
    })
  }
  productDialogVisible.value = false
  selectedProducts.value = []
}

// 移除商品
const handleRemoveProduct = (index: number) => {
  formData.items.splice(index, 1)
}

// 数量变更
const handleQuantityChange = (index: number) => {
  // 触发重新计算
}

// 监听详细地址变化，自动更新完整地址
watch([() => formData.detailAddress, () => formData.foreignRegion], () => {
  updateReceiverAddress()
})

// 加载订单详情
const loadOrderDetail = async () => {
  if (!orderId.value) return

  try {
    const order = await getOrderDetail(orderId.value)
    formData.userId = order.userId
    formData.userName = order.userName
    formData.userPhone = order.userPhone
    formData.receiverName = order.receiverName
    formData.receiverPhone = order.receiverPhone

    // 解析地址
    parseAddress(order.receiverAddress || '')

    formData.remark = order.remark || ''
    formData.items = order.items.map((item) => ({
      productId: item.productId,
      productName: item.productName,
      productImage: item.productImage,
      price: item.price,
      quantity: item.quantity,
    }))
  } catch (error) {
    console.error('Failed to load order:', error)
    ElMessage.error(t('common.message.error'))
  }
}

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return

  await formRef.value.validate(async (valid) => {
    if (!valid) return

    if (formData.items.length === 0) {
      ElMessage.warning(t('order.create.productRequired'))
      return
    }

    submitting.value = true
    try {
      if (isEdit.value) {
        await updateOrder({
          id: orderId.value,
          receiverName: formData.receiverName,
          receiverPhone: formData.receiverPhone,
          receiverAddress: formData.receiverAddress,
          remark: formData.remark,
        })
        ElMessage.success(t('common.message.updateSuccess'))
      } else {
        await createOrder({
          userId: formData.userId,
          userName: formData.userName,
          userPhone: formData.userPhone,
          receiverName: formData.receiverName,
          receiverPhone: formData.receiverPhone,
          receiverAddress: formData.receiverAddress,
          items: formData.items,
          remark: formData.remark,
        })
        ElMessage.success(t('common.message.createSuccess'))
      }
      router.push('/buz/order')
    } catch (error) {
      console.error('Failed to submit:', error)
    } finally {
      submitting.value = false
    }
  })
}

// 取消
const handleCancel = () => {
  router.back()
}

onMounted(() => {
  loadUserList()
  if (isEdit.value) {
    loadOrderDetail()
  }
})
</script>

<style scoped lang="scss">
.order-edit {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .order-form {
    max-width: 800px;
  }

  .product-table {
    margin-bottom: 20px;
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

  .search-form {
    margin-bottom: 16px;
  }

  .pagination-wrapper {
    margin-top: 16px;
    display: flex;
    justify-content: flex-end;
  }

  .address-option {
    display: flex;
    align-items: center;
    width: 100%;

    .address-name {
      font-weight: 500;
      min-width: 120px;
    }

    .address-full {
      color: var(--el-text-color-secondary);
      margin-left: 8px;
    }
  }

  .address-tip {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-top: 8px;
    color: var(--el-text-color-secondary);
    font-size: 13px;
  }

  .region-row {
    display: flex;
    gap: 16px;
    margin-bottom: 16px;
  }
}
</style>