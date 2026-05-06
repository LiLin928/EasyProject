<!-- src/views/buz/customer/components/LevelAdjustDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="450px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('customer.levelAdjustTitle') }}</span>
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
      <el-form-item :label="t('customer.currentLevel')">
        <el-tag v-if="currentLevelName" type="primary" effect="plain">
          {{ currentLevelName }}
        </el-tag>
        <span v-else>-</span>
      </el-form-item>
      <el-form-item :label="t('customer.selectLevel')" prop="levelId">
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
            :disabled="level.id === customerLevelId"
          >
            <div class="level-option">
              <span>{{ level.name }}</span>
              <span class="level-threshold">{{ t('customer.levelThreshold') }}: {{ formatMoney(level.minSpent) }}</span>
            </div>
          </el-option>
        </el-select>
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
import { adjustCustomerLevel } from '@/api/buz/customerApi'
import { getMemberLevelList } from '@/api/buz/memberLevelApi'
import { useLocale } from '@/composables/useLocale'
import type { MemberLevel } from '@/types'

const props = defineProps<{
  modelValue: boolean
  customerId?: string
  customerLevelId: string
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
const memberLevelList = ref<MemberLevel[]>([])

const formData = reactive({
  levelId: '',
  reason: '',
})

const formRules = computed<FormRules>(() => ({
  levelId: [
    { required: true, message: t('customer.selectLevelRequired'), trigger: 'change' },
  ],
  reason: [
    { required: true, message: t('customer.adjustReasonRequired'), trigger: 'blur' },
  ],
}))

// Get current level name
const currentLevelName = computed(() => {
  const level = memberLevelList.value.find(l => l.id === props.customerLevelId)
  return level?.name || ''
})

// Load member level list
const loadMemberLevelList = async () => {
  try {
    const data = await getMemberLevelList()
    memberLevelList.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

// Watch dialog open
watch(visible, (val) => {
  if (val) {
    loadMemberLevelList()
    resetForm()
  }
})

const resetForm = () => {
  formData.levelId = ''
  formData.reason = ''
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  if (!props.customerId) return

  loading.value = true
  try {
    await adjustCustomerLevel({
      userId: props.customerId,
      levelId: formData.levelId,
      reason: formData.reason,
    })
    ElMessage.success(t('customer.levelAdjustSuccess'))
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

const formatMoney = (value: number) => {
  return value.toLocaleString('zh-CN', { style: 'currency', currency: 'CNY' })
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

.level-option {
  display: flex;
  justify-content: space-between;
  align-items: center;

  .level-threshold {
    font-size: 12px;
    color: #909399;
  }
}
</style>