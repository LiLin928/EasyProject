<!-- src/views/buz/customer/components/CustomerFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ isEdit ? t('customer.editCustomer') : t('customer.addCustomer') }}</span>
        <div class="dialog-actions">
          <el-button @click="visible = false">{{ t('common.button.cancel') }}</el-button>
          <el-button type="primary" :loading="loading" @click="handleSubmit">
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
      <el-form-item :label="t('customer.username')" prop="username">
        <el-input
          v-model="formData.username"
          :placeholder="t('customer.usernamePlaceholder')"
        />
      </el-form-item>
      <el-form-item :label="t('customer.phone')" prop="phone">
        <el-input
          v-model="formData.phone"
          :placeholder="t('customer.phonePlaceholder')"
          :disabled="isEdit"
        />
      </el-form-item>
      <el-form-item :label="t('customer.nickname')" prop="nickname">
        <el-input v-model="formData.nickname" :placeholder="t('customer.nicknamePlaceholder')" />
      </el-form-item>
      <el-form-item :label="t('customer.email')" prop="email">
        <el-input v-model="formData.email" :placeholder="t('customer.email')" />
      </el-form-item>
      <el-form-item v-if="!isEdit" :label="t('customer.password')" prop="password">
        <el-input
          v-model="formData.password"
          type="password"
          :placeholder="t('customer.passwordMinLength')"
          show-password
        />
      </el-form-item>
      <el-form-item :label="t('customer.level')" prop="levelId">
        <el-select
          v-model="formData.levelId"
          :placeholder="t('customer.levelPlaceholder')"
          style="width: 100%"
        >
          <el-option
            v-for="level in memberLevelList"
            :key="level.id"
            :label="level.name"
            :value="level.id"
          />
        </el-select>
      </el-form-item>
      <el-form-item :label="t('customer.status')">
        <el-switch
          v-model="formData.status"
          :active-value="1"
          :inactive-value="0"
          :active-text="t('customer.enabled')"
          :inactive-text="t('customer.disabled')"
        />
      </el-form-item>
    </el-form>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { getCustomerDetail, createCustomer, updateCustomer } from '@/api/buz/customerApi'
import { getMemberLevelList } from '@/api/buz/memberLevelApi'
import { useLocale } from '@/composables/useLocale'
import type { MemberLevel } from '@/types'

const props = defineProps<{
  modelValue: boolean
  customerId?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const isEdit = computed(() => !!props.customerId)

const formRef = ref<FormInstance>()
const loading = ref(false)
const memberLevelList = ref<MemberLevel[]>([])

const formData = reactive({
  username: '',
  phone: '',
  nickname: '',
  email: '',
  password: '',
  levelId: '',
  status: 1 as 0 | 1,
})

const formRules = computed<FormRules>(() => ({
  username: [
    { min: 3, max: 20, message: t('customer.usernameLength'), trigger: 'blur' },
  ],
  phone: [
    { required: true, message: t('customer.phoneRequired'), trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: t('customer.phoneFormat'), trigger: 'blur' },
  ],
  nickname: [
    { required: true, message: t('customer.nicknameRequired'), trigger: 'blur' },
  ],
  email: [
    { type: 'email', message: t('customer.emailFormat'), trigger: 'blur' },
  ],
  password: isEdit.value ? [] : [
    { min: 6, message: t('customer.passwordMinLength'), trigger: 'blur' },
  ],
}))

// Load member level list
const loadMemberLevelList = async () => {
  try {
    const data = await getMemberLevelList()
    memberLevelList.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

// Load customer detail
const loadCustomerDetail = async () => {
  if (!props.customerId) return
  try {
    const data = await getCustomerDetail(props.customerId)
    formData.username = data.username || ''
    formData.phone = data.phone
    formData.nickname = data.nickname
    formData.email = data.email || ''
    formData.levelId = data.levelId || ''
    formData.status = data.status ?? 1
  } catch (error) {
    // Error handled by interceptor
  }
}

// Watch dialog open
watch(visible, (val) => {
  if (val) {
    loadMemberLevelList()
    if (props.customerId) {
      loadCustomerDetail()
    } else {
      resetForm()
    }
  }
})

const resetForm = () => {
  formData.username = ''
  formData.phone = ''
  formData.nickname = ''
  formData.email = ''
  formData.password = ''
  formData.levelId = ''
  formData.status = 1
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    if (isEdit.value) {
      await updateCustomer({
        id: props.customerId!,
        username: formData.username || undefined,
        nickname: formData.nickname,
        email: formData.email || undefined,
        levelId: formData.levelId || undefined,
        status: formData.status,
      })
      ElMessage.success(t('customer.updateCustomerSuccess'))
    } else {
      await createCustomer({
        username: formData.username || undefined,
        phone: formData.phone,
        nickname: formData.nickname,
        email: formData.email || undefined,
        password: formData.password || undefined,
        levelId: formData.levelId || undefined,
        status: formData.status,
      })
      ElMessage.success(t('customer.createCustomerSuccess'))
    }
    visible.value = false
    emit('success')
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
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
</style>