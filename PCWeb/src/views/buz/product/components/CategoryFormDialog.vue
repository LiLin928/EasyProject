<!-- src/views/buz/product/components/CategoryFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ isEdit ? t('product.category.editTitle') : t('product.category.createTitle') }}</span>
        <div class="dialog-actions">
          <el-button @click="visible = false">{{ t('product.category.cancel') }}</el-button>
          <el-button type="primary" :loading="loading" @click="handleSubmit">
            {{ t('product.category.save') }}
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
      <el-form-item :label="t('product.category.name')" prop="name">
        <el-input
          v-model="formData.name"
          :placeholder="t('product.category.namePlaceholder')"
          maxlength="50"
          show-word-limit
        />
      </el-form-item>
      <el-form-item :label="t('product.category.icon')" prop="icon">
        <el-input
          v-model="formData.icon"
          :placeholder="t('product.category.iconPlaceholder')"
          clearable
        >
          <template #append>
            <el-button @click="iconDialogVisible = true">
              <el-icon><EditPen /></el-icon>
            </el-button>
          </template>
        </el-input>
        <!-- Icon preview -->
        <div v-if="formData.icon" class="icon-preview">
          <el-icon :size="32">
            <component :is="iconComponents[formData.icon]" />
          </el-icon>
          <span class="icon-name">{{ formData.icon }}</span>
        </div>
      </el-form-item>
      <el-form-item :label="t('product.category.sort')" prop="sort">
        <el-input-number
          v-model="formData.sort"
          :min="0"
          :max="9999"
          controls-position="right"
          style="width: 150px"
        />
      </el-form-item>
      <el-form-item :label="t('product.category.description')" prop="description">
        <el-input
          v-model="formData.description"
          type="textarea"
          :rows="3"
          :placeholder="t('product.category.descriptionPlaceholder')"
          maxlength="200"
          show-word-limit
        />
      </el-form-item>
    </el-form>

    <!-- Icon Select Dialog -->
    <IconSelectDialog
      v-model="iconDialogVisible"
      :current-icon="formData.icon"
      @select="handleIconSelect"
    />
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { EditPen } from '@element-plus/icons-vue'
import * as Icons from '@element-plus/icons-vue'
import { getCategoryDetail, createCategory, updateCategory } from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import IconSelectDialog from '@/views/basic/menu/components/IconSelectDialog.vue'
import type { CreateCategoryParams, UpdateCategoryParams } from '@/types/product'

const props = defineProps<{
  modelValue: boolean
  categoryId?: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

// Icon components map for rendering
const iconComponents = Icons

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const isEdit = computed(() => !!props.categoryId)

const formRef = ref<FormInstance>()
const loading = ref(false)
const iconDialogVisible = ref(false)

const formData = reactive({
  name: '',
  icon: '',
  sort: 0,
  description: '',
})

const formRules: FormRules = {
  name: [
    { required: true, message: t('product.category.namePlaceholder'), trigger: 'blur' },
    { min: 1, max: 50, message: t('common.validate.maxLength', { max: 50 }), trigger: 'blur' },
  ],
}

const loadCategoryDetail = async () => {
  if (!props.categoryId) return
  try {
    const data = await getCategoryDetail(props.categoryId)
    formData.name = data.name
    formData.icon = data.icon || ''
    formData.sort = data.sort
    formData.description = data.description || ''
  } catch (error) {
    // Error handled by interceptor
  }
}

watch(visible, (val) => {
  if (val) {
    if (props.categoryId) {
      loadCategoryDetail()
    } else {
      resetForm()
    }
  }
})

const resetForm = () => {
  formData.name = ''
  formData.icon = ''
  formData.sort = 0
  formData.description = ''
}

const handleIconSelect = (icon: string) => {
  formData.icon = icon
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    if (isEdit.value) {
      const params: UpdateCategoryParams = {
        id: props.categoryId!,
        name: formData.name,
        icon: formData.icon,
        sort: formData.sort,
        description: formData.description,
      }
      await updateCategory(params)
      ElMessage.success(t('product.category.saveSuccess'))
    } else {
      const params: CreateCategoryParams = {
        name: formData.name,
        icon: formData.icon,
        sort: formData.sort,
        description: formData.description,
      }
      await createCategory(params)
      ElMessage.success(t('product.category.saveSuccess'))
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

.icon-preview {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-top: 8px;
  padding: 8px 12px;
  background-color: var(--el-fill-color-light);
  border-radius: 4px;

  .icon-name {
    font-size: 14px;
    color: var(--el-text-color-secondary);
  }
}
</style>