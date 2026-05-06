<!-- src/views/buz/banner/components/BannerFormDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ isEdit ? t('banner.list.edit') : t('banner.list.add') }}</span>
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
      <el-form-item :label="t('banner.list.imageUrl')" prop="image">
        <ImageUpload
          v-model="formData.image"
          :placeholder="t('banner.list.imagePlaceholder')"
        />
      </el-form-item>
      <el-form-item :label="t('banner.list.linkType')" prop="linkType">
        <el-select v-model="formData.linkType" style="width: 100%">
          <el-option :label="t('banner.linkType.none')" value="none" />
          <el-option :label="t('banner.linkType.product')" value="product" />
          <el-option :label="t('banner.linkType.category')" value="category" />
          <el-option :label="t('banner.linkType.page')" value="page" />
        </el-select>
      </el-form-item>
      <el-form-item
        v-if="formData.linkType !== 'none'"
        :label="t('banner.list.linkValue')"
        prop="linkValue"
      >
        <el-input
          v-if="formData.linkType === 'page'"
          v-model="formData.linkValue"
          :placeholder="t('banner.list.pagePathPlaceholder')"
        />
        <el-input
          v-else-if="formData.linkType === 'product'"
          v-model="formData.linkValue"
          :placeholder="t('banner.list.productIdPlaceholder')"
        />
        <el-input
          v-else-if="formData.linkType === 'category'"
          v-model="formData.linkValue"
          :placeholder="t('banner.list.categoryIdPlaceholder')"
        />
      </el-form-item>
      <el-form-item :label="t('banner.list.sort')" prop="sort">
        <el-input-number
          v-model="formData.sort"
          :min="1"
          :max="999"
          style="width: 100%"
        />
      </el-form-item>
      <el-form-item :label="t('banner.list.status')">
        <el-switch
          v-model="formData.status"
          :active-value="1"
          :inactive-value="0"
        />
      </el-form-item>
    </el-form>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { getBannerDetail, createBanner, updateBanner } from '@/api/buz/bannerApi'
import { useLocale } from '@/composables/useLocale'
import ImageUpload from '@/components/ImageUpload/index.vue'
import type { BannerLinkType } from '@/types'

const props = defineProps<{
  modelValue: boolean
  bannerId?: string
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

const isEdit = computed(() => !!props.bannerId)

const formRef = ref<FormInstance>()
const loading = ref(false)

const formData = reactive({
  image: '',
  linkType: 'none' as BannerLinkType,
  linkValue: '',
  sort: 1,
  status: 1 as 0 | 1,
})

const formRules = computed<FormRules>(() => ({
  image: [
    { required: true, message: t('banner.list.imagePlaceholder'), trigger: 'blur' },
  ],
  linkType: [
    { required: true, message: t('banner.list.linkType'), trigger: 'change' },
  ],
  linkValue: [
    {
      required: formData.linkType !== 'none',
      message: t('banner.list.linkValuePlaceholder'),
      trigger: 'blur',
    },
  ],
}))

// Load banner detail for edit
const loadBannerDetail = async () => {
  if (!props.bannerId) return
  try {
    const data = await getBannerDetail(props.bannerId)
    formData.image = data.image
    formData.linkType = data.linkType
    formData.linkValue = data.linkValue
    formData.sort = data.sort
    formData.status = data.status
  } catch (error) {
    ElMessage.error(t('banner.list.loadFailed'))
  }
}

// Watch dialog open
watch(visible, (val) => {
  if (val) {
    if (props.bannerId) {
      loadBannerDetail()
    } else {
      resetForm()
    }
  }
})

// Clear linkValue when linkType changes to 'none'
watch(() => formData.linkType, (newVal) => {
  if (newVal === 'none') {
    formData.linkValue = ''
  }
})

const resetForm = () => {
  formData.image = ''
  formData.linkType = 'none'
  formData.linkValue = ''
  formData.sort = 1
  formData.status = 1
}

const handleSubmit = async () => {
  const valid = await formRef.value?.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    if (isEdit.value) {
      await updateBanner({
        id: props.bannerId!,
        image: formData.image,
        linkType: formData.linkType,
        linkValue: formData.linkValue,
        sort: formData.sort,
        status: formData.status,
      })
      ElMessage.success(t('banner.list.updateSuccess'))
    } else {
      await createBanner({
        image: formData.image,
        linkType: formData.linkType,
        linkValue: formData.linkValue,
        sort: formData.sort,
        status: formData.status,
      })
      ElMessage.success(t('banner.list.createSuccess'))
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
  formRef.value?.clearValidate()
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