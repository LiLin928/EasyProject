<!-- src/views/basic/dict/components/DictDataFormDialog.vue -->
<template>
  <ModalForm
    v-model="visible"
    :title="isEdit ? t('dict.dict.editData') : t('dict.dict.addData')"
    :items="formItems"
    :form-data="formData"
    :mode="isEdit ? 'edit' : 'create'"
    :loading="loading"
    @submit="handleSubmit"
  />
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { createDictData, updateDictData } from '@/api/dict'
import { useLocale } from '@/composables/useLocale'
import type { CreateDictDataParams, UpdateDictDataParams, DictData } from '@/types/dict'
import { CommonStatus } from '@/types/enums'
import ModalForm from '@/components/ModalForm/index.vue'
import type { ModalFormItem } from '@/components/ModalForm/types'

const props = defineProps<{
  modelValue: boolean
  dataId?: string
  typeCode: string
  currentData?: DictData
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

const isEdit = computed(() => !!props.dataId)

const loading = ref(false)

const formData = reactive({
  label: '',
  value: '',
  sort: 0,
  status: CommonStatus.ENABLED,
})

// 表单配置
const formItems = computed<ModalFormItem[]>(() => [
  {
    field: 'label',
    label: t('dict.dict.dataLabel'),
    type: 'input',
    rules: [
      { required: true, message: t('dict.dict.dataLabelRequired'), trigger: 'blur' },
      { min: 2, max: 50, message: t('dict.dict.dataLabelFormat'), trigger: 'blur' },
    ],
  },
  {
    field: 'value',
    label: t('dict.dict.dataValue'),
    type: 'input',
    rules: [
      { required: true, message: t('dict.dict.dataValueRequired'), trigger: 'blur' },
    ],
  },
  {
    field: 'sort',
    label: t('dict.dict.dataSort'),
    type: 'number',
    props: { min: 0, max: 999 },
  },
  {
    field: 'status',
    label: t('dict.dict.status'),
    type: 'switch',
    props: { activeValue: CommonStatus.ENABLED, inactiveValue: CommonStatus.DISABLED, activeText: t('dict.dict.enabled'), inactiveText: t('dict.dict.disabled') },
  },
])

watch(visible, (val) => {
  if (val) {
    if (props.dataId && props.currentData) {
      formData.label = props.currentData.label
      formData.value = props.currentData.value
      formData.sort = props.currentData.sort
      formData.status = props.currentData.status
    } else {
      resetForm()
    }
  }
})

const resetForm = () => {
  formData.label = ''
  formData.value = ''
  formData.sort = 0
  formData.status = CommonStatus.ENABLED
}

const handleSubmit = async (data: Record<string, any>) => {
  loading.value = true
  try {
    if (isEdit.value) {
      const params: UpdateDictDataParams = {
        id: props.dataId!,
        label: data.label,
        value: data.value,
        sort: data.sort,
        status: data.status,
      }
      await updateDictData(params)
      ElMessage.success(t('dict.dict.updateDataSuccess'))
    } else {
      const params: CreateDictDataParams = {
        typeCode: props.typeCode,
        label: data.label,
        value: data.value,
        sort: data.sort,
        status: data.status,
      }
      await createDictData(params)
      ElMessage.success(t('dict.dict.createDataSuccess'))
    }
    visible.value = false
    emit('success')
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}
</script>