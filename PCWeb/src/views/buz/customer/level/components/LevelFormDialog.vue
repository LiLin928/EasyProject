<!-- src/views/buz/customer/level/components/LevelFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ isEdit ? t('customer.editLevel') : t('customer.addLevel') }}</span>
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
      label-width="120px"
    >
      <el-form-item :label="t('customer.levelName')" prop="name">
        <el-input v-model="formData.name" :placeholder="t('customer.levelNamePlaceholder')" />
      </el-form-item>
      <el-form-item :label="t('customer.minSpent')" prop="minSpent">
        <el-input-number
          v-model="formData.minSpent"
          :min="0"
          :precision="2"
          :placeholder="t('customer.minSpentPlaceholder')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item :label="t('customer.discount')" prop="discount">
        <el-input-number
          v-model="formData.discount"
          :min="0"
          :max="100"
          :precision="0"
          :placeholder="t('customer.discountPlaceholder')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item :label="t('customer.pointsRate')" prop="pointsRate">
        <el-input-number
          v-model="formData.pointsRate"
          :min="0"
          :precision="1"
          :step="0.1"
          :placeholder="t('customer.pointsRatePlaceholder')"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item :label="t('customer.levelSort')" prop="sort">
        <el-input-number
          v-model="formData.sort"
          :min="0"
          :placeholder="t('customer.levelSortPlaceholder')"
          style="width: 100%"
        />
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
import { getMemberLevelDetail, createMemberLevel, updateMemberLevel } from '@/api/buz/memberLevelApi'
import { useLocale } from '@/composables/useLocale'

const props = defineProps<{
  modelValue: boolean
  levelId?: string
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

const isEdit = computed(() => !!props.levelId)

const formRef = ref<FormInstance>()
const loading = ref(false)

const formData = reactive({
  name: '',
  minSpent: 0,
  discount: 100,
  pointsRate: 1,
  sort: 0,
  status: 1 as 0 | 1,
})

const formRules = computed<FormRules>(() => ({
  name: [
    { required: true, message: t('customer.levelNameRequired'), trigger: 'blur' },
  ],
  minSpent: [
    { required: true, message: t('customer.minSpentRequired'), trigger: 'blur' },
  ],
  discount: [
    { type: 'number', min: 0, max: 100, message: t('customer.discountRange'), trigger: 'blur' },
  ],
  pointsRate: [
    { type: 'number', min: 0, message: t('customer.pointsRatePositive'), trigger: 'blur' },
  ],
}))

// Load level detail
const loadLevelDetail = async () => {
  if (!props.levelId) return
  try {
    const data = await getMemberLevelDetail(props.levelId)
    formData.name = data.name
    formData.minSpent = data.minSpent
    formData.discount = data.discount
    formData.pointsRate = data.pointsRate
    formData.sort = data.sort
    formData.status = data.status ?? 1
  } catch (error) {
    // Error handled by interceptor
  }
}

// Watch dialog open
watch(visible, (val) => {
  if (val) {
    if (props.levelId) {
      loadLevelDetail()
    } else {
      resetForm()
    }
  }
})

const resetForm = () => {
  formData.name = ''
  formData.minSpent = 0
  formData.discount = 100
  formData.pointsRate = 1
  formData.sort = 0
  formData.status = 1
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    if (isEdit.value) {
      await updateMemberLevel({
        id: props.levelId!,
        name: formData.name,
        minSpent: formData.minSpent,
        discount: formData.discount,
        pointsRate: formData.pointsRate,
        sort: formData.sort,
        status: formData.status,
      })
      ElMessage.success(t('customer.updateLevelSuccess'))
    } else {
      await createMemberLevel({
        name: formData.name,
        minSpent: formData.minSpent,
        discount: formData.discount,
        pointsRate: formData.pointsRate,
        sort: formData.sort,
        status: formData.status,
      })
      ElMessage.success(t('customer.createLevelSuccess'))
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