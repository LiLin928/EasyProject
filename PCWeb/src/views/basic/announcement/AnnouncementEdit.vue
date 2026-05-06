<!-- src/views/basic/announcement/AnnouncementEdit.vue -->
<template>
  <div class="announcement-edit-container">
    <!-- 顶部标题栏 -->
    <div class="header-bar">
      <!-- 面包屑 -->
      <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/basic/announcement/list' }">
          {{ t('announcement.list.title') }}
        </el-breadcrumb-item>
        <el-breadcrumb-item>
          {{ isEdit ? t('announcement.edit.title') : t('announcement.create.title') }}
        </el-breadcrumb-item>
      </el-breadcrumb>

      <!-- 操作按钮 -->
      <div class="action-buttons">
        <el-button v-if="isEdit" type="info" @click="handlePreview">
          <el-icon><View /></el-icon>
          {{ t('common.button.preview') }}
        </el-button>
        <el-button type="primary" :loading="saving" @click="handleSaveDraft">
          {{ t('announcement.edit.saveDraft') }}
        </el-button>
        <el-button type="success" :loading="publishing" @click="handlePublish">
          {{ t('announcement.edit.publish') }}
        </el-button>
        <el-button @click="handleCancel">
          {{ t('common.button.cancel') }}
        </el-button>
      </div>
    </div>

    <el-card shadow="never" class="edit-card">
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
        class="form-container"
      >
        <!-- 基础信息 -->
        <div class="section-title">{{ t('announcement.edit.basicInfo') }}</div>

        <el-form-item :label="t('announcement.edit.titleLabel')" prop="title">
          <el-input v-model="formData.title" maxlength="200" show-word-limit />
        </el-form-item>

        <el-form-item :label="t('announcement.edit.typeLabel')" prop="type">
          <el-radio-group v-model="formData.type">
            <el-radio :value="1">{{ t('announcement.type.all') }}</el-radio>
            <el-radio :value="2">{{ t('announcement.type.targeted') }}</el-radio>
          </el-radio-group>
        </el-form-item>

        <el-form-item :label="t('announcement.edit.levelLabel')" prop="level">
          <el-radio-group v-model="formData.level">
            <el-radio :value="1">{{ t('announcement.level.normal') }}</el-radio>
            <el-radio :value="2">{{ t('announcement.level.important') }}</el-radio>
            <el-radio :value="3">{{ t('announcement.level.urgent') }}</el-radio>
          </el-radio-group>
        </el-form-item>

        <!-- 定向公告时显示角色选择 -->
        <el-form-item
          v-if="formData.type === 2"
          :label="t('announcement.edit.targetRoles')"
          prop="targetRoleIds"
        >
          <el-select
            v-model="formData.targetRoleIds"
            multiple
            :placeholder="t('announcement.edit.selectRoles')"
            style="width: 100%"
          >
            <el-option
              v-for="role in roleList"
              :key="role.id"
              :label="role.roleName"
              :value="role.id"
            />
          </el-select>
        </el-form-item>

        <!-- 公告内容 -->
        <div class="section-title">{{ t('announcement.edit.contentSection') }}</div>

        <el-form-item :label="t('announcement.edit.contentLabel')" prop="content">
          <WangEditor
            v-model="formData.content"
            :height="400"
            :placeholder="t('announcement.edit.contentPlaceholder')"
            :business-id="announcementId"
            @upload-success="handleEditorUploadSuccess"
          />
        </el-form-item>

        <!-- 附件上传 -->
        <div class="section-title">{{ t('announcement.edit.attachmentSection') }}</div>

        <el-form-item :label="t('announcement.edit.attachmentsLabel')">
          <el-upload
            ref="uploadRef"
            :action="uploadUrl"
            :headers="uploadHeaders"
            :on-success="handleUploadSuccess"
            :on-error="handleUploadError"
            :before-upload="beforeUpload"
            :file-list="fileList"
            :show-file-list="false"
            multiple
          >
            <el-button type="primary">
              <el-icon><Upload /></el-icon>
              {{ t('announcement.edit.uploadButton') }}
            </el-button>
            <template #tip>
              <div class="el-upload__tip">
                {{ t('announcement.edit.uploadTip') }}
              </div>
            </template>
          </el-upload>

          <!-- 已上传附件列表 -->
          <div v-if="fileList.length > 0" class="attachment-list">
            <div class="list-header">
              <span>已上传文件</span>
              <span class="count">{{ fileList.length }} 个文件</span>
            </div>
            <div
              v-for="file in fileList"
              :key="file.uid"
              class="attachment-item"
            >
              <div class="file-info">
                <el-icon class="file-icon"><Document /></el-icon>
                <span class="file-name">{{ file.name }}</span>
                <span v-if="file.size" class="file-size">{{ formatFileSize(file.size) }}</span>
              </div>
              <div class="file-actions">
                <el-button link type="primary" size="small" @click="handlePreviewAttachment(file)">
                  <el-icon><View /></el-icon>
                  预览
                </el-button>
                <el-button link type="danger" size="small" @click="handleRemoveFile(file)">
                  <el-icon><Delete /></el-icon>
                  删除
                </el-button>
              </div>
            </div>
          </div>
        </el-form-item>
      </el-form>
    </el-card>

    <!-- 附件预览弹窗 -->
    <el-dialog
      v-model="previewDialogVisible"
      :title="previewAttachment?.name"
      width="80%"
      top="5vh"
      destroy-on-close
    >
      <!-- 图片预览 -->
      <div v-if="previewType === 'image'" class="preview-container">
        <img :src="previewAttachment?.url" style="max-width: 100%; max-height: 70vh;" />
      </div>

      <!-- PDF 预览 -->
      <div v-else-if="previewType === 'pdf'" class="preview-container">
        <iframe :src="previewAttachment?.url" style="width: 100%; height: 70vh; border: none;" />
      </div>

      <!-- Word 文档预览 -->
      <div v-else-if="previewType === 'word'" class="preview-container">
        <VueOfficeDocx
          :src="previewAttachment?.url"
          class="office-viewer"
          :request-config="{
            headers: {
              'Cache-Control': 'no-cache'
            }
          }"
        />
      </div>

      <!-- Excel 文档预览 -->
      <div v-else-if="previewType === 'excel'" class="preview-container">
        <VueOfficeExcel
          :src="previewAttachment?.url"
          class="office-viewer"
          :request-config="{
            headers: {
              'Cache-Control': 'no-cache'
            }
          }"
        />
      </div>

      <!-- 其他文件提示下载 -->
      <div v-else class="preview-container download-tip">
        <el-icon :size="48"><Document /></el-icon>
        <p>此文件类型不支持在线预览</p>
        <el-button type="primary" @click="handleDownloadAttachment">
          点击下载
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Upload, View, Document, Delete } from '@element-plus/icons-vue'
import type { FormInstance, FormRules, UploadFile } from 'element-plus'
import WangEditor from '@/components/WangEditor/index.vue'
// vue-office 样式和组件
import '@vue-office/docx/lib/index.css'
import '@vue-office/excel/lib/index.css'
import VueOfficeDocx from '@vue-office/docx'
import VueOfficeExcel from '@vue-office/excel'
import { getAnnouncementDetail, addAnnouncement, updateAnnouncement, publishAnnouncement, updateFileBusinessId, deleteFile } from '@/api/announcement/announcementApi'
import { getRoleList } from '@/api/user'
import { useLocale } from '@/composables/useLocale'
import { getToken } from '@/utils/auth'
import type { RoleInfo, AddAnnouncementParams, UpdateAnnouncementParams, FileAttachment } from '@/types'

const { t } = useLocale()
const router = useRouter()
const route = useRoute()

const loading = ref(false)
const saving = ref(false)
const publishing = ref(false)
const formRef = ref<FormInstance | null>(null)
const uploadRef = ref()
const roleList = ref<RoleInfo[]>([])
const fileList = ref<UploadFile[]>([])
// 当前公告 ID（优先使用本地变量，因为 router.replace 后 computed 可能不立即更新）
const currentAnnouncementId = ref<string>('')
const announcementId = computed(() => currentAnnouncementId.value || (route.params.id as string) || '')

const isEdit = computed(() => !!route.params.id)

// 上传配置
const uploadUrl = computed(() => {
  let url = '/api/file/upload'
  if (announcementId.value) {
    url += `?businessId=${announcementId.value}`
  }
  return url
})
const uploadHeaders = computed(() => ({
  Authorization: `Bearer ${getToken()}`
}))

// 存储上传成功的文件ID列表
const uploadedFileIds = ref<string[]>([])
// 存储编辑器上传的图片ID列表
const editorImageIds = ref<string[]>([])
// 附件预览弹窗
const previewDialogVisible = ref(false)
const previewAttachment = ref<UploadFile | null>(null)
const previewType = ref<'image' | 'pdf' | 'word' | 'excel' | 'other'>('other')

const formData = reactive<AddAnnouncementParams>({
  title: '',
  content: '',
  type: 1,
  level: 1,
  targetRoleIds: []
})

const formRules: FormRules = {
  title: [
    { required: true, message: t('announcement.edit.titleRequired'), trigger: 'blur' },
    { min: 2, max: 200, message: t('announcement.edit.titleLength'), trigger: 'blur' },
  ],
  content: [
    { required: true, message: t('announcement.edit.contentRequired'), trigger: 'change' },
  ],
  type: [
    { required: true, message: t('announcement.edit.typeRequired'), trigger: 'change' },
  ],
  level: [
    { required: true, message: t('announcement.edit.levelRequired'), trigger: 'change' },
  ],
  targetRoleIds: [
    {
      validator: (_rule, value, callback) => {
        if (formData.type === 2 && (!value || value.length === 0)) {
          callback(new Error(t('announcement.edit.rolesRequired')))
        } else {
          callback()
        }
      },
      trigger: 'change'
    }
  ],
}

// 加载角色列表
const loadRoleList = async () => {
  try {
    const data = await getRoleList()
    roleList.value = data.list
  } catch {
    // Error handled
  }
}

// 加载公告详情
const loadDetail = async (id: string) => {
  loading.value = true
  try {
    const data = await getAnnouncementDetail(id)

    formData.title = data.title
    formData.content = data.content
    formData.type = data.type
    formData.level = data.level
    formData.targetRoleIds = data.targetRoleIds || []

    // 加载附件
    if (data.attachments && data.attachments.length > 0) {
      fileList.value = data.attachments.map((att: FileAttachment) => ({
        name: att.fileName,
        url: att.url || '',
        uid: att.id,
        size: att.fileSize,
        status: 'success'
      })) as UploadFile[]
    } else {
      fileList.value = []
    }
  } catch {
    ElMessage.error(t('common.message.loadFailed'))
    router.push('/basic/announcement/list')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadRoleList()
  // 初始化公告 ID
  if (route.params.id) {
    currentAnnouncementId.value = route.params.id as string
    loadDetail(currentAnnouncementId.value)
  }
})

// 上传前校验
const beforeUpload = (file: File) => {
  const maxSize = 20 * 1024 * 1024 // 20MB
  if (file.size > maxSize) {
    ElMessage.error(t('announcement.edit.fileTooLarge'))
    return false
  }
  return true
}

// 上传成功
const handleUploadSuccess = async (response: any, file: UploadFile) => {
  if (response && response.code === 200) {
    const fileId = response.data.id
    const url = response.data.url

    // 更新文件的 url 和 uid
    file.url = url
    file.uid = fileId || file.uid

    // 将文件添加到列表中显示
    fileList.value.push(file)

    // 如果当前已有公告 ID，立即关联文件
    if (currentAnnouncementId.value && fileId) {
      try {
        await updateFileBusinessId([fileId], currentAnnouncementId.value)
      } catch {
        // 关联失败也不影响编辑，保存时会再次尝试关联
        uploadedFileIds.value.push(fileId)
      }
    } else if (fileId) {
      // 新建模式，记录文件 ID，等待保存后关联
      uploadedFileIds.value.push(fileId)
    }

    ElMessage.success(t('announcement.edit.uploadSuccess'))
  } else {
    ElMessage.error(response.message || t('announcement.edit.uploadFailed'))
  }
}

// 上传失败
const handleUploadError = () => {
  ElMessage.error(t('announcement.edit.uploadFailed'))
}

// 编辑器图片上传成功
const handleEditorUploadSuccess = async (fileId: string, url: string) => {
  // 如果当前已有公告 ID，立即关联文件
  if (currentAnnouncementId.value) {
    try {
      await updateFileBusinessId([fileId], currentAnnouncementId.value)
    } catch {
      // 关联失败也不影响编辑，保存时会再次尝试关联
      editorImageIds.value.push(fileId)
    }
  } else {
    // 新建模式，记录文件 ID，等待保存后关联
    editorImageIds.value.push(fileId)
  }
}

// 保存草稿
const handleSaveDraft = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch {
    return
  }

  saving.value = true
  try {
    let id = announcementId.value

    // 如果是新建模式，先保存获得 ID
    if (!isEdit.value || !currentAnnouncementId.value) {
      id = await addAnnouncement(formData)
      // 更新本地 ID，让后续上传能带上正确的 businessId
      currentAnnouncementId.value = id
      // 更新路由参数
      router.replace(`/basic/announcement/edit/${id}`)
    } else {
      // 编辑模式，直接更新
      const params: UpdateAnnouncementParams = {
        id: currentAnnouncementId.value,
        ...formData
      }
      await updateAnnouncement(params)
    }

    // 如果有新上传的附件，关联到公告
    const allFileIds = [...uploadedFileIds.value, ...editorImageIds.value]
    if (allFileIds.length > 0 && id) {
      await updateFileBusinessId(allFileIds, id)
      uploadedFileIds.value = []
      editorImageIds.value = []
    }

    ElMessage.success(t('announcement.edit.saveDraftSuccess'))
    // 不立即跳转，让用户可以继续编辑
  } catch {
    ElMessage.error(t('common.message.saveFailed'))
  } finally {
    saving.value = false
  }
}

// 发布
const handlePublish = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch {
    return
  }

  publishing.value = true
  try {
    let id = announcementId.value

    // 如果是新建模式，先保存获得 ID
    if (!isEdit.value || !currentAnnouncementId.value) {
      id = await addAnnouncement(formData)
      // 更新本地 ID
      currentAnnouncementId.value = id
      // 更新路由参数
      router.replace(`/basic/announcement/edit/${id}`)
    } else {
      // 编辑模式，先更新
      const params: UpdateAnnouncementParams = {
        id: currentAnnouncementId.value,
        ...formData
      }
      await updateAnnouncement(params)
    }

    // 如果有新上传的附件，关联到公告
    const allFileIds = [...uploadedFileIds.value, ...editorImageIds.value]
    if (allFileIds.length > 0 && id) {
      await updateFileBusinessId(allFileIds, id)
      uploadedFileIds.value = []
      editorImageIds.value = []
    }

    // 发布
    await publishAnnouncement(id)
    ElMessage.success(t('announcement.message.publishSuccess'))
    handleCancel()
  } catch {
    ElMessage.error(t('announcement.message.publishFailed'))
  } finally {
    publishing.value = false
  }
}

// 取消
const handleCancel = () => {
  router.push('/basic/announcement/list')
}

// 预览公告
const handlePreview = () => {
  if (announcementId.value) {
    router.push(`/basic/announcement/preview/${announcementId.value}`)
  }
}

// 预览附件
const handlePreviewAttachment = (file: UploadFile) => {
  previewAttachment.value = file
  const name = file.name.toLowerCase()
  const ext = name.split('.').pop() || ''

  if (['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'].includes(ext)) {
    previewType.value = 'image'
  } else if (ext === 'pdf') {
    previewType.value = 'pdf'
  } else if (['doc', 'docx'].includes(ext)) {
    previewType.value = 'word'
  } else if (['xls', 'xlsx'].includes(ext)) {
    previewType.value = 'excel'
  } else {
    previewType.value = 'other'
  }

  previewDialogVisible.value = true
}

// 下载附件
const handleDownloadAttachment = () => {
  if (!previewAttachment.value) return
  const link = document.createElement('a')
  link.href = previewAttachment.value.url || ''
  link.download = previewAttachment.value.name
  link.target = '_blank'
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}

// 删除附件
const handleRemoveFile = async (file: UploadFile) => {
  try {
    // 从列表中移除
    const index = fileList.value.findIndex(f => f.uid === file.uid)
    if (index > -1) {
      fileList.value.splice(index, 1)
    }

    // 调用后端 API 删除文件记录
    const fileId = file.uid as string
    if (fileId && typeof fileId === 'string' && fileId.includes('-')) {
      await deleteFile(fileId)
      ElMessage.success('附件删除成功')
    }
  } catch {
    ElMessage.error('附件删除失败')
  }
}

// 格式化文件大小
const formatFileSize = (size: number): string => {
  if (!size) return ''
  if (size < 1024) return `${size} B`
  if (size < 1024 * 1024) return `${(size / 1024).toFixed(2)} KB`
  return `${(size / (1024 * 1024)).toFixed(2)} MB`
}

// 监听类型变化，清空角色选择
watch(() => formData.type, (val) => {
  if (val === 1) {
    formData.targetRoleIds = []
  }
})
</script>

<style scoped lang="scss">
.announcement-edit-container {
  padding: 20px;

  .header-bar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 16px;

    .action-buttons {
      display: flex;
      gap: 12px;
    }
  }

  .edit-card {
    margin-top: 0;
  }

  .form-container {
    max-width: 900px;

    .section-title {
      font-size: 16px;
      font-weight: 600;
      margin: 20px 0 16px;
      padding-bottom: 8px;
      border-bottom: 1px solid #ebeef5;
    }

    .attachment-list {
      margin-top: 16px;
      border: 1px solid #ebeef5;
      border-radius: 8px;

      .list-header {
        padding: 12px 16px;
        background: #f5f7fa;
        border-bottom: 1px solid #ebeef5;
        display: flex;
        justify-content: space-between;
        align-items: center;
        font-weight: 500;

        .count {
          color: #909399;
          font-size: 12px;
        }
      }

      .attachment-item {
        padding: 12px 16px;
        display: flex;
        justify-content: space-between;
        align-items: center;
        border-bottom: 1px solid #ebeef5;

        &:last-child {
          border-bottom: none;
        }

        .file-info {
          display: flex;
          align-items: center;
          gap: 8px;
          flex: 1;

          .file-icon {
            color: #409eff;
          }

          .file-name {
            color: #303133;
          }

          .file-size {
            color: #909399;
            font-size: 12px;
          }
        }

        .file-actions {
          display: flex;
          gap: 8px;
        }
      }
    }
  }
}

.preview-container {
  width: 100%;
  min-height: 400px;

  .office-viewer {
    width: 100%;
    height: 70vh;
    overflow: auto;
  }
}

.download-tip {
  text-align: center;
  color: #909399;

  p {
    margin: 16px 0;
  }
}
</style>