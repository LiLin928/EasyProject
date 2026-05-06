<!-- src/views/basic/dict/components/DictTypeFormDialog.vue -->
<template>
  <ModalForm
    v-model="visible"
    :title="isEdit ? t('dict.dict.editType') : t('dict.dict.addType')"
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
import { getDictTypeDetail, createDictType, updateDictType } from '@/api/dict'
import { useLocale } from '@/composables/useLocale'
import type { CreateDictTypeParams, UpdateDictTypeParams } from '@/types/dict'
import { CommonStatus } from '@/types/enums'
import ModalForm from '@/components/ModalForm/index.vue'
import type { ModalFormItem } from '@/components/ModalForm/types'

const props = defineProps<{
  modelValue: boolean
  typeId?: string
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

const isEdit = computed(() => !!props.typeId)

const loading = ref(false)

const formData = reactive({
  code: '',
  name: '',
  description: '',
  status: CommonStatus.ENABLED,
})

// 表单配置
const formItems = computed<ModalFormItem[]>(() => [
  {
    field: 'code',
    label: t('dict.dict.typeCode'),
    type: 'input',
    rules: [
      { required: true, message: t('dict.dict.typeCodeRequired'), trigger: 'blur' },
      { pattern: /^[a-zA-Z0-9_]+$/, message: t('dict.dict.typeCodeFormat'), trigger: 'blur' },
    ],
    props: { disabledInEdit: true },
  },
  {
    field: 'name',
    label: t('dict.dict.typeName'),
    type: 'input',
    rules: [
      { required: true, message: t('dict.dict.typeNameRequired'), trigger: 'blur' },
      { min: 2, max: 20, message: t('dict.dict.typeNameFormat'), trigger: 'blur' },
    ],
  },
  {
    field: 'description',
    label: t('dict.dict.typeDescription'),
    type: 'textarea',
    props: { rows: 3, maxlength: 100, showWordLimit: true },
  },
  {
    field: 'status',
    label: t('dict.dict.status'),
    type: 'switch',
    props: { activeValue: CommonStatus.ENABLED, inactiveValue: CommonStatus.DISABLED, activeText: t('dict.dict.enabled'), inactiveText: t('dict.dict.disabled') },
  },
])

const loadTypeDetail = async () => {
  if (!props.typeId) return
  try {
    const data = await getDictTypeDetail(props.typeId)
    formData.code = data.code
    formData.name = data.name
    formData.description = data.description || ''
    formData.status = data.status
  } catch (error) {
    // Error handled by interceptor
  }
}

watch(visible, (val) => {
  if (val) {
    if (props.typeId) {
      loadTypeDetail()
    } else {
      resetForm()
    }
  }
})

const resetForm = () => {
  formData.code = ''
  formData.name = ''
  formData.description = ''
  formData.status = CommonStatus.ENABLED
}

const handleSubmit = async (data: Record<string, any>) => {
  loading.value = true
  try {
    if (isEdit.value) {
      const params: UpdateDictTypeParams = {
        id: props.typeId!,
        name: data.name,
        description: data.description,
        status: data.status,
      }
      await updateDictType(params)
      ElMessage.success(t('dict.dict.updateTypeSuccess'))
    } else {
      const params: CreateDictTypeParams = {
        code: data.code,
        name: data.name,
        description: data.description,
        status: data.status,
      }
      await createDictType(params)
      ElMessage.success(t('dict.dict.createTypeSuccess'))
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