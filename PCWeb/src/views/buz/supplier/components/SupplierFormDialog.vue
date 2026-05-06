<!-- src/views/buz/supplier/components/SupplierFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ isEdit ? t('product.supplier.editTitle') : t('product.supplier.createTitle') }}</span>
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
      <el-form-item :label="t('product.supplier.name')" prop="name">
        <el-input
          v-model="formData.name"
          :placeholder="t('product.supplier.namePlaceholder')"
          maxlength="100"
        />
      </el-form-item>

      <el-form-item :label="t('product.supplier.unifiedCode')">
        <el-input
          v-model="formData.unifiedCode"
          :placeholder="t('product.supplier.unifiedCodePlaceholder')"
          maxlength="50"
        />
      </el-form-item>

      <el-form-item :label="t('product.supplier.contact')" prop="contact">
        <el-input
          v-model="formData.contact"
          :placeholder="t('product.supplier.contactPlaceholder')"
          maxlength="50"
        />
      </el-form-item>

      <el-form-item :label="t('product.supplier.phone')" prop="phone">
        <el-input
          v-model="formData.phone"
          :placeholder="t('product.supplier.phonePlaceholder')"
          maxlength="20"
        />
      </el-form-item>

      <!-- 地址：国家选择 -->
      <el-form-item :label="t('order.create.country')" prop="country">
        <el-select
          v-model="formData.country"
          :placeholder="t('order.create.countryPlaceholder')"
          style="width: 100%"
          @change="handleCountryChange"
        >
          <el-option
            v-for="country in countries"
            :key="country.value"
            :label="country.label"
            :value="country.value"
          />
        </el-select>
      </el-form-item>

      <!-- 中国地址：省市区级联选择 -->
      <el-form-item v-if="formData.country === '中国'" :label="t('order.create.provinceCity')">
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

      <!-- 非中国地址：省/城市文本输入 -->
      <el-form-item v-if="formData.country && formData.country !== '中国'" :label="t('order.create.provinceCity')">
        <el-input
          v-model="formData.foreignRegion"
          :placeholder="t('order.create.provinceCityPlaceholder')"
        />
      </el-form-item>

      <!-- 详细地址 -->
      <el-form-item :label="t('order.create.detailAddress')" prop="detailAddress">
        <el-input
          v-model="formData.detailAddress"
          type="textarea"
          :rows="2"
          :placeholder="t('order.create.detailAddressPlaceholder')"
        />
      </el-form-item>

      <el-form-item :label="t('product.supplier.remark')">
        <el-input
          v-model="formData.remark"
          type="textarea"
          :rows="3"
          :placeholder="t('product.supplier.remarkPlaceholder')"
          maxlength="500"
        />
      </el-form-item>

      <el-form-item :label="t('product.supplier.status')">
        <el-switch
          v-model="formData.status"
          :active-value="1"
          :inactive-value="0"
          :active-text="t('product.supplier.enabled')"
          :inactive-text="t('product.supplier.disabled')"
        />
      </el-form-item>
    </el-form>
  </el-dialog>
</template>

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
</style>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { getSupplierDetail, createSupplier, updateSupplier } from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import { chinaRegions, countries } from '@/data/chinaRegions'
import type { CreateSupplierParams } from '@/types/product'

const { t } = useLocale()

const props = defineProps<{
  modelValue: boolean
  supplierId?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const formRef = ref<FormInstance>()
const saving = ref(false)

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const isEdit = computed(() => !!props.supplierId)

const formData = reactive({
  id: undefined as string | undefined,
  name: '',
  unifiedCode: '',
  contact: '',
  phone: '',
  country: '中国',
  region: [] as string[],
  foreignRegion: '',
  detailAddress: '',
  address: '', // 完整地址用于提交
  remark: '',
  status: 1,
})

const formRules: FormRules = {
  name: [
    { required: true, message: t('product.supplier.namePlaceholder'), trigger: 'blur' },
    { min: 2, max: 100, message: '名称长度应在 2-100 个字符之间', trigger: 'blur' },
  ],
  country: [
    { required: true, message: t('order.create.countryRequired'), trigger: 'change' },
  ],
}

// Watch for dialog open
watch(visible, async (val) => {
  if (val) {
    if (isEdit.value && props.supplierId) {
      await loadSupplier(props.supplierId)
    } else {
      resetForm()
    }
  }
})

/**
 * Load supplier detail
 */
const loadSupplier = async (id: string) => {
  try {
    const data = await getSupplierDetail(id)
    formData.id = data.id
    formData.name = data.name
    formData.unifiedCode = data.unifiedCode || ''
    formData.contact = data.contact || ''
    formData.phone = data.phone || ''
    formData.remark = data.remark || ''
    formData.status = data.status
    // 解析地址
    parseAddress(data.address || '')
  } catch (error) {
    // Error handled by interceptor
  }
}

/**
 * Parse address string to region fields
 */
const parseAddress = (address: string) => {
  if (!address) {
    formData.country = '中国'
    formData.region = []
    formData.foreignRegion = ''
    formData.detailAddress = ''
    formData.address = ''
    return
  }

  // Try to match country
  const countryMatch = countries.find(c => address.startsWith(c.value))
  if (countryMatch && countryMatch.value !== '中国') {
    formData.country = countryMatch.value
    const remaining = address.slice(countryMatch.value.length).trim()
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
    // Chinese address
    formData.country = '中国'
    const provinceMatch = chinaRegions.find(p => address.includes(p.value))
    if (provinceMatch) {
      const province = provinceMatch.value
      const afterProvince = address.slice(address.indexOf(province) + province.length).trim()

      const cityMatch = provinceMatch.children?.find(c => afterProvince.includes(c.value))
      if (cityMatch) {
        const city = cityMatch.value
        const afterCity = afterProvince.slice(afterProvince.indexOf(city) + city.length).trim()

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
      formData.region = []
      formData.detailAddress = address
    }
    formData.foreignRegion = ''
  }
  formData.address = address
}

/**
 * Update full address
 */
const updateFullAddress = () => {
  if (formData.country === '中国' && formData.region.length > 0) {
    formData.address = `${formData.region.join(' ')} ${formData.detailAddress}`
  } else if (formData.country && formData.country !== '中国') {
    formData.address = `${formData.country} ${formData.foreignRegion} ${formData.detailAddress}`
  } else {
    formData.address = formData.detailAddress
  }
}

/**
 * Handle country change
 */
const handleCountryChange = () => {
  formData.region = []
  formData.foreignRegion = ''
  updateFullAddress()
}

/**
 * Handle region change
 */
const handleRegionChange = () => {
  updateFullAddress()
}

/**
 * Reset form
 */
const resetForm = () => {
  formData.id = undefined
  formData.name = ''
  formData.unifiedCode = ''
  formData.contact = ''
  formData.phone = ''
  formData.country = '中国'
  formData.region = []
  formData.foreignRegion = ''
  formData.detailAddress = ''
  formData.address = ''
  formData.remark = ''
  formData.status = 1
}

/**
 * Handle close
 */
const handleClose = () => {
  visible.value = false
  formRef.value?.resetFields()
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

  // Update address before saving
  updateFullAddress()

  saving.value = true
  try {
    if (isEdit.value) {
      await updateSupplier({
        id: formData.id!,
        name: formData.name,
        unifiedCode: formData.unifiedCode,
        contact: formData.contact,
        phone: formData.phone,
        address: formData.address,
        remark: formData.remark,
        status: formData.status,
      })
    } else {
      await createSupplier({
        name: formData.name,
        unifiedCode: formData.unifiedCode,
        contact: formData.contact,
        phone: formData.phone,
        address: formData.address,
        remark: formData.remark,
        status: formData.status,
      })
    }
    ElMessage.success(t('product.supplier.saveSuccess'))
    emit('success')
    handleClose()
  } catch (error) {
    // Error handled by interceptor
  } finally {
    saving.value = false
  }
}
</script>