<template>
  <el-dialog
    v-model="dialogVisible"
    width="500px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('order.ship.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="handleClose">{{ t('common.button.cancel') }}</el-button>
          <el-button type="primary" :loading="loading" @click="handleSubmit">
            {{ t('order.ship.confirm') }}
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
      <el-form-item
        :label="t('order.ship.company')"
        prop="shipCompany"
      >
        <el-select
          v-model="formData.shipCompany"
          :placeholder="t('order.ship.companyPlaceholder')"
          style="width: 100%"
        >
          <el-option
            v-for="item in SHIP_COMPANIES"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>

      <el-form-item
        :label="t('order.ship.trackingNo')"
        prop="shipNo"
      >
        <el-input
          v-model="formData.shipNo"
          :placeholder="t('order.ship.trackingNoPlaceholder')"
        />
      </el-form-item>
    </el-form>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { SHIP_COMPANIES } from '@/types/order'
import { shipOrder } from '@/api/buz/orderApi'

const { t } = useI18n()

interface Props {
  visible: boolean
  orderId: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'update:visible', value: boolean): void
  (e: 'success'): void
}>()

const formRef = ref<FormInstance>()
const loading = ref(false)

const dialogVisible = computed({
  get: () => props.visible,
  set: (value) => emit('update:visible', value),
})

const formData = ref({
  shipCompany: '',
  shipNo: '',
})

const formRules: FormRules = {
  shipCompany: [
    { required: true, message: t('order.ship.companyRequired'), trigger: 'change' },
  ],
  shipNo: [
    { required: true, message: t('order.ship.trackingNoRequired'), trigger: 'blur' },
  ],
}

// Reset form when dialog opens
watch(() => props.visible, (val) => {
  if (val) {
    formData.value = {
      shipCompany: '',
      shipNo: '',
    }
    formRef.value?.resetFields()
  }
})

const handleClose = () => {
  emit('update:visible', false)
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate()
  if (!valid) return

  loading.value = true
  try {
    await shipOrder({
      orderId: props.orderId,
      logisticsCompany: formData.value.shipCompany,
      logisticsNumber: formData.value.shipNo,
    })
    ElMessage.success(t('order.list.shipSuccess'))
    emit('success')
    handleClose()
  } catch (error) {
    // Error handled by request interceptor
  } finally {
    loading.value = false
  }
}
</script>

<style scoped lang="less">
.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}

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