<!-- src/views/person/components/BindPhoneDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">绑定手机</span>
        <div class="dialog-actions">
          <el-button @click="visible = false">取消</el-button>
          <el-button type="primary" :loading="loading" @click="handleSubmit">
            确定
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
      <el-form-item label="新手机号" prop="phone">
        <el-input v-model="formData.phone" placeholder="请输入新手机号" />
      </el-form-item>
      <el-form-item label="当前密码" prop="password">
        <el-input
          v-model="formData.password"
          type="password"
          placeholder="请输入当前密码验证身份"
          show-password
        />
      </el-form-item>
    </el-form>

    </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { bindPhone } from '@/api/user'
import { useUserStore } from '@/stores/user'
import { useLocale } from '@/composables/useLocale'

const props = defineProps<{
  modelValue: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()
const userStore = useUserStore()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const formRef = ref<FormInstance>()
const loading = ref(false)

const formData = reactive({
  phone: '',
  password: '',
})

const formRules: FormRules = {
  phone: [
    { required: true, message: '请输入手机号', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号', trigger: 'blur' },
  ],
  password: [
    { required: true, message: '请输入当前密码', trigger: 'blur' },
  ],
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    await bindPhone(formData)
    // 更新用户信息
    if (userStore.userInfo) {
      userStore.setUserInfoAction({
        ...userStore.userInfo,
        phone: formData.phone,
      })
    }
    ElMessage.success(t('user.profile.phoneBindSuccess'))
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