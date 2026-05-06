<!-- src/components/ModalForm/index.vue -->
<template>
  <el-dialog
    v-model="visible"
    :title="dialogTitle"
    :width="width || '600px'"
    :close-on-click-modal="false"
    @closed="handleClosed"
  >
    <!-- 顶部按钮区 -->
    <template #header="{ close }">
      <div class="dialog-header">
        <span class="dialog-title">{{ dialogTitle }}</span>
        <div class="dialog-actions">
          <el-button @click="handleCancel">
            {{ cancelText || t('common.button.cancel') }}
          </el-button>
          <el-button type="primary" :loading="loading" @click="handleSubmit">
            {{ submitText || t('common.button.confirm') }}
          </el-button>
        </div>
      </div>
    </template>

    <el-form
      ref="formRef"
      :model="localFormData"
      :rules="formRules"
      :label-width="labelWidth || '100px'"
    >
      <el-row :gutter="20">
        <template v-for="item in visibleItems" :key="item.field">
          <el-col :span="item.span || 24">
            <el-form-item :label="item.label" :prop="item.field">
              <!-- Input -->
              <el-input
                v-if="item.type === 'input'"
                v-model="localFormData[item.field]"
                :placeholder="item.placeholder || t('common.placeholder.input', { label: item.label })"
                :disabled="getDisabled(item)"
                v-bind="item.props"
              />

              <!-- Textarea -->
              <el-input
                v-else-if="item.type === 'textarea'"
                v-model="localFormData[item.field]"
                type="textarea"
                :rows="item.props?.rows || 3"
                :placeholder="item.placeholder || t('common.placeholder.input', { label: item.label })"
                v-bind="item.props"
              />

              <!-- Select -->
              <el-select
                v-else-if="item.type === 'select'"
                v-model="localFormData[item.field]"
                :placeholder="item.placeholder || t('common.placeholder.select', { label: item.label })"
                :disabled="getDisabled(item)"
                style="width: 100%"
                v-bind="item.props"
              >
                <el-option
                  v-for="option in item.options"
                  :key="option.value"
                  :label="option.label"
                  :value="option.value"
                />
              </el-select>

              <!-- Radio -->
              <el-radio-group
                v-else-if="item.type === 'radio'"
                v-model="localFormData[item.field]"
                :disabled="getDisabled(item)"
                v-bind="item.props"
              >
                <el-radio
                  v-for="option in item.options"
                  :key="option.value"
                  :label="option.value"
                >
                  {{ option.label }}
                </el-radio>
              </el-radio-group>

              <!-- Checkbox -->
              <el-checkbox-group
                v-else-if="item.type === 'checkbox'"
                v-model="localFormData[item.field]"
                :disabled="getDisabled(item)"
                v-bind="item.props"
              >
                <el-checkbox
                  v-for="option in item.options"
                  :key="option.value"
                  :label="option.value"
                >
                  {{ option.label }}
                </el-checkbox>
              </el-checkbox-group>

              <!-- Switch -->
              <el-switch
                v-else-if="item.type === 'switch'"
                v-model="localFormData[item.field]"
                v-bind="item.props"
              />

              <!-- Date -->
              <el-date-picker
                v-else-if="item.type === 'date'"
                v-model="localFormData[item.field]"
                :placeholder="item.placeholder || t('common.placeholder.select', { label: item.label })"
                :disabled="getDisabled(item)"
                style="width: 100%"
                v-bind="item.props"
              />

              <!-- Number -->
              <el-input-number
                v-else-if="item.type === 'number'"
                v-model="localFormData[item.field]"
                :placeholder="item.placeholder"
                style="width: 100%"
                v-bind="item.props"
              />

              <!-- Slot: 自定义渲染 -->
              <slot
                v-else-if="item.type === 'slot'"
                :name="item.field"
                :form-data="localFormData"
              />
            </el-form-item>
          </el-col>
        </template>
      </el-row>
    </el-form>
  </el-dialog>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import type { ModalFormItem, ModalFormProps, ModalFormEmits } from './types'

const props = withDefaults(defineProps<ModalFormProps>(), {
  width: '600px',
  loading: false,
  labelWidth: '100px',
})

const emit = defineEmits<ModalFormEmits>()

const { t } = useLocale()

// 双向绑定
const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

// 对话框标题
const dialogTitle = computed(() => {
  if (props.title) return props.title
  return props.mode === 'create' ? t('common.button.add') : t('common.button.edit')
})

// 表单引用
const formRef = ref<FormInstance>()

// 本地表单数据
const localFormData = ref<Record<string, any>>({ ...props.formData })

// 监听外部数据变化，更新本地数据
watch(() => props.formData, (val) => {
  localFormData.value = { ...val }
}, { deep: true, immediate: true })

// 监听本地数据变化，同步回外部（用于外部验证函数访问最新值）
watch(localFormData, (val) => {
  // 仅同步值，不替换整个对象，保持外部 reactive 的引用
  Object.keys(val).forEach(key => {
    if (props.formData[key] !== val[key]) {
      props.formData[key] = val[key]
    }
  })
}, { deep: true })

// 可见表单项
const visibleItems = computed(() => {
  return props.items.filter(item => {
    if (props.mode === 'create' && item.hideInCreate) return false
    if (props.mode === 'edit' && item.hideInEdit) return false
    return true
  })
})

// 表单验证规则
const formRules = computed<FormRules>(() => {
  const rules: FormRules = {}
  props.items.forEach(item => {
    if (item.rules) {
      rules[item.field] = item.rules
    }
  })
  return rules
})

// 获取禁用状态
const getDisabled = (item: ModalFormItem) => {
  if (item.disabled) return true
  if (props.mode === 'edit' && item.props?.disabledInEdit) return true
  return false
}

// 提交表单
const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return
  emit('submit', { ...localFormData.value })
}

// 取消
const handleCancel = () => {
  visible.value = false
  emit('cancel')
}

// 关闭时重置
const handleClosed = () => {
  formRef.value?.resetFields()
}

// 暴露方法
defineExpose({
  formRef,
  validate: () => formRef.value?.validate(),
  validateField: (prop: string) => formRef.value?.validateField(prop),
  clearValidate: (prop?: string) => formRef.value?.clearValidate(prop),
  resetFields: () => formRef.value?.resetFields(),
})
</script>

<style scoped lang="scss">
:deep(.el-dialog__body) {
  padding: 20px;
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
    margin-right: 30px; // 为关闭按钮留出空间
  }
}
</style>