<!-- src/views/buz/customer/components/PointsAdjustDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="450px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('customer.pointsAdjustTitle') }}</span>
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
      <el-form-item :label="t('customer.currentPoints')">
        <span class="current-points">{{ customerPoints }}</span>
      </el-form-item>
      <el-form-item :label="t('customer.adjustType')" prop="type">
        <el-radio-group v-model="formData.type">
          <el-radio value="add">{{ t('customer.pointsAdd') }}</el-radio>
          <el-radio value="subtract">{{ t('customer.pointsSubtract') }}</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item :label="t('customer.adjustAmount')" prop="amount">
        <el-input-number
          v-model="formData.amount"
          :min="1"
          :max="formData.type === 'subtract' ? customerPoints : 99999"
          :placeholder="t('customer.adjustAmountPlaceholder')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item :label="t('customer.adjustReason')" prop="reason">
        <el-input
          v-model="formData.reason"
          type="textarea"
          :rows="3"
          :placeholder="t('customer.adjustReasonPlaceholder')"
        />
      </el-form-item>
    </el-form>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { adjustCustomerPoints } from '@/api/buz/customerApi'
import { useLocale } from '@/composables/useLocale'

const props = defineProps<{
  modelValue: boolean
  customerId?: string
  customerPoints: number
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
  type: 'add' as 'add' | 'subtract',
  amount: 1,
  reason: '',
})

const formRules = computed<FormRules>(() => ({
  type: [
    { required: true, message: t('customer.adjustAmountRequired'), trigger: 'change' },
  ],
  amount: [
    { required: true, message: t('customer.adjustAmountRequired'), trigger: 'blur' },
    { type: 'number', min: 1, message: t('customer.adjustAmountPositive'), trigger: 'blur' },
  ],
  reason: [
    { required: true, message: t('customer.adjustReasonRequired'), trigger: 'blur' },
  ],
}))

// Watch dialog open
watch(visible, (val) => {
  if (val) {
    resetForm()
  }
})

const resetForm = () => {
  formData.type = 'add'
  formData.amount = 1
  formData.reason = ''
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  if (!props.customerId) return

  loading.value = true
  try {
    await adjustCustomerPoints({
      userId: props.customerId,
      type: formData.type,
      amount: formData.amount,
      reason: formData.reason,
    })
    ElMessage.success(t('customer.adjustSuccess'))
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

.current-points {
  font-size: 18px;
  font-weight: 500;
  color: #f56c6c;
}
</style>