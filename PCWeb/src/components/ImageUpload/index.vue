<!-- src/components/ImageUpload/index.vue -->
<template>
  <div class="image-upload">
    <!-- 单图上传 -->
    <div v-if="!multiple" class="single-upload">
      <el-upload
        class="upload-trigger"
        :action="uploadUrl"
        :headers="uploadHeaders"
        :show-file-list="false"
        :on-success="handleSuccess"
        :on-error="handleError"
        :before-upload="beforeUpload"
        accept="image/*"
      >
        <div v-if="imageUrl" class="image-preview">
          <el-image :src="imageUrl" fit="cover" />
          <div class="image-actions">
            <el-icon class="action-icon" @click.stop="handleRemove"><Delete /></el-icon>
            <el-icon class="action-icon" @click.stop="handlePreview"><View /></el-icon>
          </div>
        </div>
        <div v-else class="upload-placeholder">
          <el-icon class="upload-icon"><Plus /></el-icon>
          <span class="upload-text">{{ placeholder || t('common.upload') }}</span>
        </div>
      </el-upload>
      <!-- URL 输入框作为备选 -->
      <div class="url-input-wrapper">
        <el-input
          v-model="urlInput"
          :placeholder="urlPlaceholder || t('common.imageUrlPlaceholder')"
          @change="handleUrlChange"
        >
          <template #prepend>URL</template>
        </el-input>
      </div>
    </div>

    <!-- 多图上传 -->
    <div v-if="multiple" class="multiple-upload">
      <div class="images-list">
        <div v-for="(img, index) in imageList" :key="index" class="image-item">
          <el-image :src="img" fit="cover" class="preview-image" />
          <div class="image-item-actions">
            <el-icon class="action-icon" @click="handleRemoveItem(index)"><Delete /></el-icon>
            <el-icon class="action-icon" @click="handlePreviewItem(index)"><View /></el-icon>
          </div>
        </div>
        <el-upload
          class="upload-trigger-small"
          :action="uploadUrl"
          :headers="uploadHeaders"
          :show-file-list="false"
          :on-success="handleMultipleSuccess"
          :on-error="handleError"
          :before-upload="beforeUpload"
          accept="image/*"
        >
          <div class="upload-placeholder-small">
            <el-icon><Plus /></el-icon>
          </div>
        </el-upload>
      </div>
      <!-- URL 添加 -->
      <div class="url-add-wrapper">
        <el-input
          v-model="urlInput"
          :placeholder="urlPlaceholder || t('common.imageUrlPlaceholder')"
          @change="handleUrlAdd"
        >
          <template #prepend>URL</template>
          <template #append>
            <el-button @click="handleUrlAdd">{{ t('common.add') }}</el-button>
          </template>
        </el-input>
      </div>
    </div>

    <!-- 图片预览对话框 -->
    <el-dialog v-model="previewVisible" title="图片预览" width="600px">
      <el-image :src="previewUrl" fit="contain" style="width: 100%" />
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus, Delete, View } from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import { getToken } from '@/utils/auth'

interface Props {
  modelValue: string | string[]
  multiple?: boolean
  limit?: number
  maxSize?: number // KB
  placeholder?: string
  urlPlaceholder?: string
}

const props = withDefaults(defineProps<Props>(), {
  multiple: false,
  limit: 5,
  maxSize: 2048, // 2MB
  placeholder: '',
  urlPlaceholder: '',
})

const emit = defineEmits<{
  (e: 'update:modelValue', value: string | string[]): void
  (e: 'change', value: string | string[]): void
}>()

const { t } = useLocale()

// 上传地址
const uploadUrl = '/api/file/upload'

// 上传请求头（添加 Authorization）
const uploadHeaders = computed(() => {
  const token = getToken()
  return token ? { Authorization: `Bearer ${token}` } : {}
})

// URL 输入
const urlInput = ref('')

// 预览
const previewVisible = ref(false)
const previewUrl = ref('')

// 单图模式
const imageUrl = computed({
  get: () => props.modelValue as string,
  set: (val) => emit('update:modelValue', val),
})

// 多图模式
const imageList = computed({
  get: () => props.modelValue as string[],
  set: (val) => emit('update:modelValue', val),
})

// 上传前校验
const beforeUpload = (file: File) => {
  const isImage = file.type.startsWith('image/')
  if (!isImage) {
    ElMessage.error(t('common.uploadImageOnly'))
    return false
  }
  const isLtMaxSize = file.size / 1024 < props.maxSize
  if (!isLtMaxSize) {
    ElMessage.error(t('common.uploadSizeLimit', { size: props.maxSize }))
    return false
  }
  if (props.multiple && imageList.value.length >= props.limit) {
    ElMessage.error(t('common.uploadLimit', { limit: props.limit }))
    return false
  }
  return true
}

// 单图上传成功
const handleSuccess = (response: any) => {
  if (response.code === 200 && response.data?.success) {
    // 优先使用预签名URL（支持私有存储桶），如果没有则使用普通URL
    imageUrl.value = response.data.presignedUrl || response.data.url
    emit('change', response.data.presignedUrl || response.data.url)
    ElMessage.success(t('common.message.uploadSuccess'))
  } else {
    ElMessage.error(response.message || response.data?.errorMessage || t('common.message.uploadFailed'))
  }
}

// 多图上传成功
const handleMultipleSuccess = (response: any) => {
  if (response.code === 200 && response.data?.success) {
    // 优先使用预签名URL（支持私有存储桶），如果没有则使用普通URL
    const url = response.data.presignedUrl || response.data.url
    imageList.value = [...imageList.value, url]
    emit('change', imageList.value)
    ElMessage.success(t('common.message.uploadSuccess'))
  } else {
    ElMessage.error(response.message || response.data?.errorMessage || t('common.message.uploadFailed'))
  }
}

// 上传失败
const handleError = () => {
  ElMessage.error(t('common.message.uploadFailed'))
}

// URL 输入处理 (单图)
const handleUrlChange = () => {
  if (urlInput.value) {
    imageUrl.value = urlInput.value
    emit('change', urlInput.value)
    urlInput.value = ''
  }
}

// URL 添加处理 (多图)
const handleUrlAdd = () => {
  if (urlInput.value) {
    if (imageList.value.length >= props.limit) {
      ElMessage.error(t('common.uploadLimit', { limit: props.limit }))
      return
    }
    imageList.value = [...imageList.value, urlInput.value]
    emit('change', imageList.value)
    urlInput.value = ''
  }
}

// 删除图片 (单图)
const handleRemove = () => {
  imageUrl.value = ''
  emit('change', '')
}

// 删除图片 (多图)
const handleRemoveItem = (index: number) => {
  const newList = [...imageList.value]
  newList.splice(index, 1)
  imageList.value = newList
  emit('change', newList)
}

// 预览图片 (单图)
const handlePreview = () => {
  previewUrl.value = imageUrl.value
  previewVisible.value = true
}

// 预览图片 (多图)
const handlePreviewItem = (index: number) => {
  previewUrl.value = imageList.value[index]
  previewVisible.value = true
}
</script>

<style scoped lang="scss">
.image-upload {
  width: 100%;
}

.single-upload {
  .upload-trigger {
    width: 100%;
  }

  .image-preview {
    width: 120px;
    height: 120px;
    border-radius: 4px;
    overflow: hidden;
    position: relative;

    .el-image {
      width: 100%;
      height: 100%;
    }

    .image-actions {
      position: absolute;
      bottom: 0;
      left: 0;
      right: 0;
      height: 32px;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 16px;

      .action-icon {
        color: #fff;
        font-size: 16px;
        cursor: pointer;

        &:hover {
          color: var(--el-color-primary);
        }
      }
    }
  }

  .upload-placeholder {
    width: 120px;
    height: 120px;
    border: 1px dashed var(--el-border-color);
    border-radius: 4px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    cursor: pointer;
    transition: border-color 0.2s;

    &:hover {
      border-color: var(--el-color-primary);
    }

    .upload-icon {
      font-size: 24px;
      color: var(--el-text-color-placeholder);
    }

    .upload-text {
      font-size: 12px;
      color: var(--el-text-color-placeholder);
      margin-top: 8px;
    }
  }

  .url-input-wrapper {
    margin-top: 8px;
  }
}

.multiple-upload {
  .images-list {
    display: flex;
    flex-wrap: wrap;
    gap: 8px;
  }

  .image-item {
    width: 80px;
    height: 80px;
    border-radius: 4px;
    overflow: hidden;
    position: relative;

    .preview-image {
      width: 100%;
      height: 100%;
    }

    .image-item-actions {
      position: absolute;
      bottom: 0;
      left: 0;
      right: 0;
      height: 28px;
      background: rgba(0, 0, 0, 0.5);
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 12px;

      .action-icon {
        color: #fff;
        font-size: 14px;
        cursor: pointer;

        &:hover {
          color: var(--el-color-primary);
        }
      }
    }
  }

  .upload-trigger-small {
    .upload-placeholder-small {
      width: 80px;
      height: 80px;
      border: 1px dashed var(--el-border-color);
      border-radius: 4px;
      display: flex;
      justify-content: center;
      align-items: center;
      cursor: pointer;
      transition: border-color 0.2s;

      &:hover {
        border-color: var(--el-color-primary);
      }

      .el-icon {
        font-size: 20px;
        color: var(--el-text-color-placeholder);
      }
    }
  }

  .url-add-wrapper {
    margin-top: 8px;
  }
}
</style>