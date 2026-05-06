<!-- src/views/person/components/ChangeAvatarDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    width="500px"
    :close-on-click-modal="false"
    @closed="handleClose"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">更换头像</span>
        <div class="dialog-actions">
          <el-button @click="visible = false">取消</el-button>
          <el-button type="primary" :loading="loading" :disabled="!previewUrl" @click="handleSubmit">
            确定
          </el-button>
        </div>
      </div>
    </template>
    <div class="avatar-upload">
      <!-- 预览区域 -->
      <div class="avatar-preview">
        <el-avatar :size="150" :src="previewUrl" class="preview-avatar">
          <el-icon :size="60"><User /></el-icon>
        </el-avatar>
        <p class="preview-tip">头像预览</p>
      </div>

      <!-- 上传区域 -->
      <div class="upload-area">
        <el-upload
          ref="uploadRef"
          class="avatar-uploader"
          :show-file-list="false"
          :before-upload="beforeUpload"
          :http-request="handleUpload"
          accept="image/jpeg,image/png"
        >
          <el-button type="primary">
            <el-icon><Upload /></el-icon>
            选择图片
          </el-button>
        </el-upload>
        <p class="upload-tip">
          支持 JPG、PNG 格式，大小不超过 2MB
        </p>
      </div>
    </div>

    </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { User, Upload } from '@element-plus/icons-vue'
import { uploadAvatar } from '@/api/user'
import { useUserStore } from '@/stores/user'
import { useLocale } from '@/composables/useLocale'

const props = defineProps<{
  modelValue: boolean
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()
const userStore = useUserStore()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val),
})

const loading = ref(false)
const previewUrl = ref('')
const selectedFile = ref<File | null>(null)

// 监听 previewUrl 变化，释放旧的 blob URL 防止内存泄漏
watch(previewUrl, (newUrl, oldUrl) => {
  if (oldUrl && oldUrl !== newUrl) {
    URL.revokeObjectURL(oldUrl)
  }
})

// 文件上传前校验
const beforeUpload = (file: File) => {
  const isJPG = file.type === 'image/jpeg'
  const isPNG = file.type === 'image/png'
  const isLt2M = file.size / 1024 / 1024 < 2

  if (!isJPG && !isPNG) {
    ElMessage.error(t('user.profile.avatarFormatLimit'))
    return false
  }
  if (!isLt2M) {
    ElMessage.error(t('user.profile.avatarSizeLimit'))
    return false
  }

  // 生成预览
  previewUrl.value = URL.createObjectURL(file)
  selectedFile.value = file
  return false // 阻止自动上传
}

// 自定义上传
const handleUpload = () => {
  // 由 handleSubmit 处理
}

const handleSubmit = async () => {
  if (!selectedFile.value) {
    ElMessage.warning(t('user.profile.selectImageFirst'))
    return
  }

  loading.value = true
  try {
    const data = await uploadAvatar(selectedFile.value)
    // 更新用户信息
    if (userStore.userInfo) {
      userStore.setUserInfoAction({
        ...userStore.userInfo,
        avatar: data.url,
      })
    }
    ElMessage.success(t('user.profile.avatarChangeSuccess'))
    visible.value = false
    emit('success')
  } catch (error) {
    // 错误已由拦截器处理
  } finally {
    loading.value = false
  }
}

const handleClose = () => {
  if (previewUrl.value) {
    URL.revokeObjectURL(previewUrl.value)
  }
  previewUrl.value = ''
  selectedFile.value = null
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

.avatar-upload {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 24px;
}

.avatar-preview {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;

  .preview-avatar {
    background-color: #e4e7ed;
  }

  .preview-tip {
    font-size: 14px;
    color: #909399;
    margin: 0;
  }
}

.upload-area {
  text-align: center;

  .upload-tip {
    font-size: 12px;
    color: #909399;
    margin-top: 8px;
  }
}
</style>