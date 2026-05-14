<!-- 文件: PCWeb/src/views/basic/desktop/widget/components/WidgetFormDialog.vue -->
<template>
  <ModalForm
    ref="modalFormRef"
    v-model="visible"
    :title="isEdit ? '编辑组件' : '新建组件'"
    :items="formItems"
    :form-data="formData"
    :mode="isEdit ? 'edit' : 'create'"
    :loading="loading"
    width="700px"
    label-width="120px"
    @submit="handleSubmit"
  >
    <!-- 数据源配置插槽 -->
    <template #dataSourceConfig="{ formData }">
      <el-input
        v-model="formData.dataSourceConfig"
        type="textarea"
        :rows="5"
        placeholder="请输入数据源配置（JSON格式）"
      />
    </template>

    <!-- 交互配置插槽 -->
    <template #interactionConfig="{ formData }">
      <el-input
        v-model="formData.interactionConfig"
        type="textarea"
        :rows="5"
        placeholder="请输入交互配置（JSON格式）"
      />
    </template>
  </ModalForm>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import ModalForm from '@/components/ModalForm/index.vue'
import type { ModalFormItem } from '@/components/ModalForm/types'
import { getDesktopWidgetDetail, addDesktopWidget, updateDesktopWidget } from '@/api/basic/desktopWidgetApi'
import {
  WidgetType,
  DataSourceType,
  widgetTypeLabels,
  dataSourceTypeLabels,
  gridWidthOptions,
} from '@/types'

const props = defineProps<{
  modelValue: boolean
  id?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const isEdit = computed(() => !!props.id)

const loading = ref(false)
const modalFormRef = ref<InstanceType<typeof ModalForm> | null>(null)

const formData = reactive({
  name: '',
  type: WidgetType.Card,
  icon: '',
  defaultWidth: 3,
  defaultHeight: 200,
  dataSourceType: DataSourceType.Static,
  dataSourceConfig: '',
  interactionConfig: '',
  status: 1,
})

// 表单配置
const formItems = computed<ModalFormItem[]>(() => [
  {
    field: 'name',
    label: '组件名称',
    type: 'input',
    rules: [
      { required: true, message: '请输入组件名称', trigger: 'blur' },
      { min: 2, max: 50, message: '名称长度为2-50个字符', trigger: 'blur' },
    ],
  },
  {
    field: 'type',
    label: '组件类型',
    type: 'select',
    rules: [{ required: true, message: '请选择组件类型', trigger: 'change' }],
    options: Object.entries(widgetTypeLabels).map(([value, label]) => ({
      label,
      value: Number(value),
    })),
  },
  {
    field: 'icon',
    label: '组件图标',
    type: 'input',
    props: { placeholder: '请输入图标名称（如：el-icon-data-line）' },
  },
  {
    field: 'defaultWidth',
    label: '默认宽度',
    type: 'select',
    rules: [{ required: true, message: '请选择默认宽度', trigger: 'change' }],
    options: gridWidthOptions,
  },
  {
    field: 'defaultHeight',
    label: '默认高度',
    type: 'number',
    rules: [{ required: true, message: '请输入默认高度', trigger: 'blur' }],
    props: { min: 100, max: 800, step: 50 },
  },
  {
    field: 'dataSourceType',
    label: '数据源类型',
    type: 'select',
    rules: [{ required: true, message: '请选择数据源类型', trigger: 'change' }],
    options: Object.entries(dataSourceTypeLabels).map(([value, label]) => ({
      label,
      value: Number(value),
    })),
  },
  {
    field: 'dataSourceConfig',
    label: '数据源配置',
    type: 'slot',
  },
  {
    field: 'interactionConfig',
    label: '交互配置',
    type: 'slot',
  },
  {
    field: 'status',
    label: '状态',
    type: 'switch',
    props: {
      activeValue: 1,
      inactiveValue: 0,
      activeText: '启用',
      inactiveText: '禁用',
    },
  },
])

// 加载详情
const loadDetail = async () => {
  if (!props.id) return
  loading.value = true
  try {
    const data = await getDesktopWidgetDetail(props.id)
    formData.name = data.name
    formData.type = data.type
    formData.icon = data.icon || ''
    formData.defaultWidth = data.defaultWidth
    formData.defaultHeight = data.defaultHeight
    formData.dataSourceType = data.dataSourceType
    formData.dataSourceConfig = data.dataSourceConfig || ''
    formData.interactionConfig = data.interactionConfig || ''
    formData.status = data.status
  } catch (error) {
    // 错误已处理
  } finally {
    loading.value = false
  }
}

// 监听弹窗打开
watch(visible, (val) => {
  if (val) {
    if (props.id) {
      loadDetail()
    } else {
      resetForm()
    }
  }
})

const resetForm = () => {
  formData.name = ''
  formData.type = WidgetType.Card
  formData.icon = ''
  formData.defaultWidth = 3
  formData.defaultHeight = 200
  formData.dataSourceType = DataSourceType.Static
  formData.dataSourceConfig = ''
  formData.interactionConfig = ''
  formData.status = 1
}

const handleSubmit = async (data: Record<string, any>) => {
  loading.value = true
  try {
    if (isEdit.value) {
      await updateDesktopWidget({
        id: props.id!,
        name: data.name,
        type: data.type,
        icon: data.icon || undefined,
        defaultWidth: data.defaultWidth,
        defaultHeight: data.defaultHeight,
        dataSourceType: data.dataSourceType,
        dataSourceConfig: data.dataSourceConfig || undefined,
        interactionConfig: data.interactionConfig || undefined,
        status: data.status,
      })
      ElMessage.success('更新成功')
    } else {
      await addDesktopWidget({
        name: data.name,
        type: data.type,
        icon: data.icon || undefined,
        defaultWidth: data.defaultWidth,
        defaultHeight: data.defaultHeight,
        dataSourceType: data.dataSourceType,
        dataSourceConfig: data.dataSourceConfig || undefined,
        interactionConfig: data.interactionConfig || undefined,
        status: data.status,
      })
      ElMessage.success('创建成功')
    }
    visible.value = false
    emit('success')
  } catch (error) {
    // 错误已处理
  } finally {
    loading.value = false
  }
}
</script>