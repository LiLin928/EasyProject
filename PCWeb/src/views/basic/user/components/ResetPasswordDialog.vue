<!-- src/views/basic/user/components/ResetPasswordDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="400px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('user.user.resetPassword') }}</span>
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
      <el-form-item :label="t('user.user.newPassword')" prop="password">
        <el-input
          v-model="formData.password"
          type="password"
          :placeholder="t('user.user.newPasswordPlaceholder')"
          show-password
        />
      </el-form-item>
      <el-form-item :label="t('user.user.confirmPassword')" prop="confirmPassword">
        <el-input
          v-model="formData.confirmPassword"
          type="password"
          :placeholder="t('user.user.confirmPasswordPlaceholder')"
          show-password
        />
      </el-form-item>
    </el-form>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { resetUserPassword } from '@/api/user'
import { useLocale } from '@/composables/useLocale'

const props = defineProps<{
  modelValue: boolean
  userId?: string
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

const formRef = ref<FormInstance>()
const loading = ref(false)

const formData = reactive({
  password: '',
  confirmPassword: '',
})

// 密码确认校验
const validateConfirmPassword = (rule: any, value: string, callback: any) => {
  if (value !== formData.password) {
    callback(new Error(t('user.user.passwordNotMatch')))
  } else {
    callback()
  }
}

const formRules: FormRules = {
  password: [
    { required: true, message: t('user.user.passwordRequired'), trigger: 'blur' },
    { min: 6, max: 20, message: t('user.user.passwordFormat'), trigger: 'blur' },
  ],
  confirmPassword: [
    { required: true, message: t('user.user.confirmPasswordRequired'), trigger: 'blur' },
    { validator: validateConfirmPassword, trigger: 'blur' },
  ],
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  if (!props.userId) return

  loading.value = true
  try {
    await resetUserPassword(props.userId, formData.password)
    ElMessage.success(t('user.user.resetPasswordSuccess'))
    visible.value = false
    emit('success')
  } catch (error) {
    // 错误已由拦截器处理
  } finally {
    loading.value = false
  }
}

const handleClose = () => {
  formRef.value?.resetFields()
  formData.password = ''
  formData.confirmPassword = ''
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