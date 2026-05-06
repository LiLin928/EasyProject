<!-- src/views/buz/product/components/SubmitAuditDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    :title="t('product.audit.submitAudit')"
    width="500px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="100px"
    >
      <el-form-item :label="t('product.audit.auditScene')" prop="auditPointCode">
        <el-select
          v-model="formData.auditPointCode"
          :placeholder="t('product.audit.selectAuditScene')"
          :loading="loading"
          style="width: 100%"
        >
          <el-option
            v-for="item in auditPoints"
            :key="item.code"
            :label="item.name"
            :value="item.code"
          >
            <span>{{ item.name }}</span>
            <span style="float: right; color: var(--el-text-color-secondary)">
              {{ item.category }}
            </span>
          </el-option>
        </el-select>
      </el-form-item>
      <el-form-item :label="t('product.audit.remark')" prop="remark">
        <el-input
          v-model="formData.remark"
          type="textarea"
          :rows="4"
          :placeholder="t('product.audit.remarkPlaceholder')"
          maxlength="500"
          show-word-limit
        />
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="handleClose">
        {{ t('common.button.cancel') }}
      </el-button>
      <el-button type="primary" :loading="submitting" @click="handleSubmit">
        {{ t('product.audit.submit') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { getBusinessAuditPointByTable } from '@/api/ant_workflow/businessAuditPointApi'
import { submitProductAudit } from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { BusinessAuditPoint } from '@/types'

const props = defineProps<{
  modelValue: boolean
  productId: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

const visible = ref(props.modelValue)
const loading = ref(false)
const submitting = ref(false)
const formRef = ref<FormInstance | null>(null)
const auditPoints = ref<BusinessAuditPoint[]>([])

const formData = reactive({
  auditPointCode: '',
  remark: '',
})

const formRules: FormRules = {
  auditPointCode: [
    { required: true, message: t('product.audit.selectAuditSceneRequired'), trigger: 'change' },
  ],
}

// 同步 visible
watch(
  () => props.modelValue,
  (val) => {
    visible.value = val
    if (val) {
      loadAuditPoints()
    }
  }
)

watch(visible, (val) => {
  emit('update:modelValue', val)
})

/**
 * 加载审核点列表
 */
const loadAuditPoints = async () => {
  loading.value = true
  try {
    const data = await getBusinessAuditPointByTable('Product')
    auditPoints.value = data || []
    // 如果只有一个审核点，自动选中
    if (auditPoints.value.length === 1) {
      formData.auditPointCode = auditPoints.value[0].code
    }
  } catch (error) {
    ElMessage.error(t('product.audit.loadAuditPointsFailed'))
  } finally {
    loading.value = false
  }
}

/**
 * 提交审核
 */
const handleSubmit = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch {
    return
  }

  submitting.value = true
  try {
    await submitProductAudit({
      productId: props.productId,
      auditPointCode: formData.auditPointCode,
    })
    ElMessage.success(t('product.audit.submitSuccess'))
    emit('success')
    handleClose()
  } catch (error) {
    ElMessage.error(t('product.audit.submitFailed'))
  } finally {
    submitting.value = false
  }
}

/**
 * 关闭弹窗
 */
const handleClose = () => {
  formData.auditPointCode = ''
  formData.remark = ''
  formRef.value?.resetFields()
  visible.value = false
}
</script>