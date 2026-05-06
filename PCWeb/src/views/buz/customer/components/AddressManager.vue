<!-- src/views/buz/customer/components/AddressManager.vue -->
<template>
  <div class="address-manager">
    <div class="header">
      <el-button type="primary" size="small" @click="handleAdd">
        <el-icon><Plus /></el-icon>
        {{ t('customer.addAddress') }}
      </el-button>
    </div>

    <div v-loading="loading" class="address-list">
      <template v-if="addressList.length > 0">
        <div
          v-for="address in addressList"
          :key="address.id"
          class="address-item"
          :class="{ 'is-default': address.isDefault }"
        >
          <div class="address-info">
            <div class="address-header">
              <span class="name">{{ address.name }}</span>
              <span class="phone">{{ address.phone }}</span>
              <el-tag v-if="address.isDefault" type="success" size="small">
                {{ t('customer.defaultAddress') }}
              </el-tag>
            </div>
            <div class="address-detail">
              {{ address.province }} {{ address.city }} {{ address.district }} {{ address.address }}
            </div>
          </div>
          <div class="address-actions">
            <el-button link type="primary" size="small" @click="handleEdit(address)">
              {{ t('customer.editAddress') }}
            </el-button>
            <el-button
              v-if="!address.isDefault"
              link
              type="primary"
              size="small"
              @click="handleSetDefault(address)"
            >
              {{ t('customer.setDefault') }}
            </el-button>
            <el-button link type="danger" size="small" @click="handleDelete(address)">
              {{ t('customer.deleteAddress') }}
            </el-button>
          </div>
        </div>
      </template>
      <el-empty v-else :description="t('customer.noAddress')" />
    </div>

    <!-- Address Form Dialog -->
    <el-dialog
      v-model="dialogVisible"
      width="500px"
      :close-on-click-modal="false"
      @closed="handleClose"
    >
      <template #header>
        <div class="dialog-header">
          <span class="dialog-title">{{ isEdit ? t('customer.editAddress') : t('customer.addAddress') }}</span>
          <div class="dialog-actions">
            <el-button @click="dialogVisible = false">{{ t('common.button.cancel') }}</el-button>
            <el-button type="primary" :loading="submitLoading" @click="handleSubmit">
              {{ t('common.button.ok') }}
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
        <el-form-item :label="t('customer.addressName')" prop="name">
          <el-input v-model="formData.name" :placeholder="t('customer.addressNamePlaceholder')" />
        </el-form-item>
        <el-form-item :label="t('customer.addressPhone')" prop="phone">
          <el-input v-model="formData.phone" :placeholder="t('customer.addressPhonePlaceholder')" />
        </el-form-item>
        <el-form-item :label="t('customer.province')" prop="province">
          <el-select
            v-model="formData.province"
            :placeholder="t('customer.provincePlaceholder')"
            style="width: 100%"
            @change="handleProvinceChange"
          >
            <el-option
              v-for="province in provinces"
              :key="province"
              :label="province"
              :value="province"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('customer.city')" prop="city">
          <el-select
            v-model="formData.city"
            :placeholder="t('customer.cityPlaceholder')"
            style="width: 100%"
            :disabled="!formData.province"
            @change="handleCityChange"
          >
            <el-option
              v-for="city in cities"
              :key="city"
              :label="city"
              :value="city"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('customer.district')" prop="district">
          <el-select
            v-model="formData.district"
            :placeholder="t('customer.districtPlaceholder')"
            style="width: 100%"
            :disabled="!formData.city"
          >
            <el-option
              v-for="district in districts"
              :key="district"
              :label="district"
              :value="district"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('customer.detailAddress')" prop="address">
          <el-input
            v-model="formData.address"
            type="textarea"
            :rows="2"
            :placeholder="t('customer.detailAddressPlaceholder')"
          />
        </el-form-item>
        <el-form-item :label="t('customer.defaultAddress')">
          <el-switch v-model="formData.isDefault" />
        </el-form-item>
      </el-form>

      </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import {
  getCustomerAddressList,
  createCustomerAddress,
  updateCustomerAddress,
  deleteCustomerAddress,
  setDefaultAddress,
} from '@/api/buz/customerApi'
import { useLocale } from '@/composables/useLocale'
import type { CustomerAddress } from '@/types'

const props = defineProps<{
  customerId: string
}>()

const emit = defineEmits<{
  (e: 'success'): void
}>()

const { t } = useLocale()

const loading = ref(false)
const submitLoading = ref(false)
const addressList = ref<CustomerAddress[]>([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const currentAddressId = ref<string | undefined>(undefined)

const formRef = ref<FormInstance>()
const formData = reactive({
  name: '',
  phone: '',
  province: '',
  city: '',
  district: '',
  address: '',
  isDefault: false,
})

const formRules: FormRules = {
  name: [
    { required: true, message: t('customer.addressNameRequired'), trigger: 'blur' },
  ],
  phone: [
    { required: true, message: t('customer.addressPhoneRequired'), trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: t('customer.addressPhoneFormat'), trigger: 'blur' },
  ],
  province: [
    { required: true, message: t('customer.provinceRequired'), trigger: 'change' },
  ],
  city: [
    { required: true, message: t('customer.cityRequired'), trigger: 'change' },
  ],
  district: [
    { required: true, message: t('customer.districtRequired'), trigger: 'change' },
  ],
  address: [
    { required: true, message: t('customer.detailAddressRequired'), trigger: 'blur' },
  ],
}

// Mock province/city/district data
const provinces = ['广东省', '北京市', '上海市', '浙江省', '江苏省', '四川省']
const cityMap: Record<string, string[]> = {
  '广东省': ['广州市', '深圳市', '东莞市', '佛山市', '中山市'],
  '北京市': ['北京市'],
  '上海市': ['上海市'],
  '浙江省': ['杭州市', '宁波市', '温州市'],
  '江苏省': ['南京市', '苏州市', '无锡市'],
  '四川省': ['成都市', '绵阳市', '德阳市'],
}
const districtMap: Record<string, string[]> = {
  '广州市': ['天河区', '番禺区', '白云区', '黄埔区', '越秀区'],
  '深圳市': ['福田区', '南山区', '罗湖区', '宝安区', '龙岗区'],
  '东莞市': ['莞城区', '南城区', '东城区', '万江区'],
  '佛山市': ['禅城区', '南海区', '顺德区', '三水区'],
  '中山市': ['石岐区', '东区', '西区', '南区'],
  '北京市': ['朝阳区', '海淀区', '东城区', '西城区', '丰台区'],
  '上海市': ['浦东新区', '黄浦区', '徐汇区', '静安区', '长宁区'],
  '杭州市': ['西湖区', '上城区', '下城区', '江干区', '拱墅区'],
  '宁波市': ['海曙区', '江北区', '鄞州区', '镇海区'],
  '温州市': ['鹿城区', '龙湾区', '瓯海区'],
  '南京市': ['玄武区', '秦淮区', '建邺区', '鼓楼区', '栖霞区'],
  '苏州市': ['姑苏区', '吴中区', '相城区', '虎丘区'],
  '无锡市': ['梁溪区', '锡山区', '惠山区', '滨湖区'],
  '成都市': ['锦江区', '青羊区', '金牛区', '武侯区', '成华区'],
  '绵阳市': ['涪城区', '游仙区', '安州区'],
  '德阳市': ['旌阳区', '罗江区'],
}

const cities = computed(() => {
  return cityMap[formData.province] || []
})

const districts = computed(() => {
  return districtMap[formData.city] || []
})

// Load address list
const loadAddressList = async () => {
  if (!props.customerId) return
  loading.value = true
  try {
    const data = await getCustomerAddressList(props.customerId)
    addressList.value = data
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

// Watch customer id change
watch(() => props.customerId, (val) => {
  if (val) {
    loadAddressList()
  }
}, { immediate: true })

const handleProvinceChange = () => {
  formData.city = ''
  formData.district = ''
}

const handleCityChange = () => {
  formData.district = ''
}

const handleAdd = () => {
  isEdit.value = false
  currentAddressId.value = undefined
  resetForm()
  dialogVisible.value = true
}

const handleEdit = (address: CustomerAddress) => {
  isEdit.value = true
  currentAddressId.value = address.id
  formData.name = address.name
  formData.phone = address.phone
  formData.province = address.province
  formData.city = address.city
  formData.district = address.district
  formData.address = address.address
  formData.isDefault = address.isDefault
  dialogVisible.value = true
}

const handleDelete = async (address: CustomerAddress) => {
  try {
    await ElMessageBox.confirm(
      t('customer.deleteAddressConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteCustomerAddress(address.id)
    ElMessage.success(t('customer.deleteAddressSuccess'))
    loadAddressList()
    emit('success')
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleSetDefault = async (address: CustomerAddress) => {
  try {
    await setDefaultAddress(address.id)
    ElMessage.success(t('customer.setDefaultAddressSuccess'))
    loadAddressList()
    emit('success')
  } catch (error) {
    // Error handled by interceptor
  }
}

const resetForm = () => {
  formData.name = ''
  formData.phone = ''
  formData.province = ''
  formData.city = ''
  formData.district = ''
  formData.address = ''
  formData.isDefault = false
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  submitLoading.value = true
  try {
    if (isEdit.value) {
      await updateCustomerAddress({
        id: currentAddressId.value!,
        customerId: props.customerId,
        name: formData.name,
        phone: formData.phone,
        province: formData.province,
        city: formData.city,
        district: formData.district,
        address: formData.address,
        isDefault: formData.isDefault,
      })
      ElMessage.success(t('customer.updateAddressSuccess'))
    } else {
      await createCustomerAddress({
        customerId: props.customerId,
        name: formData.name,
        phone: formData.phone,
        province: formData.province,
        city: formData.city,
        district: formData.district,
        address: formData.address,
        isDefault: formData.isDefault,
      })
      ElMessage.success(t('customer.createAddressSuccess'))
    }
    dialogVisible.value = false
    loadAddressList()
    emit('success')
  } catch (error) {
    // Error handled by interceptor
  } finally {
    submitLoading.value = false
  }
}

const handleClose = () => {
  formRef.value?.resetFields()
  resetForm()
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

.address-manager {
  .header {
    margin-bottom: 16px;
  }

  .address-list {
    min-height: 200px;
  }

  .address-item {
    padding: 16px;
    border: 1px solid #e4e7ed;
    border-radius: 4px;
    margin-bottom: 12px;
    background: #fff;

    &:hover {
      border-color: #409eff;
    }

    &.is-default {
      border-color: #67c23a;
      background: #f0f9eb;
    }

    .address-info {
      .address-header {
        display: flex;
        align-items: center;
        gap: 12px;
        margin-bottom: 8px;

        .name {
          font-weight: 500;
          font-size: 14px;
        }

        .phone {
          color: #606266;
        }
      }

      .address-detail {
        color: #909399;
        font-size: 13px;
      }
    }

    .address-actions {
      margin-top: 12px;
      display: flex;
      gap: 8px;
    }
  }
}
</style>