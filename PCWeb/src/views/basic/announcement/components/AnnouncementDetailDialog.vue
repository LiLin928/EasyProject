<!-- src/views/basic/announcement/components/AnnouncementDetailDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    :title="t('announcement.detail.title')"
    width="700px"
    :close-on-click-modal="false"
    destroy-on-close
  >
    <div v-loading="loading" class="detail-content">
      <!-- 标题和级别 -->
      <div class="header-section">
        <h2 class="title">{{ detail?.title }}</h2>
        <el-tag :type="getLevelType(detail?.level)" size="default" class="level-tag">
          {{ getLevelText(detail?.level) }}
        </el-tag>
      </div>

      <!-- 基本信息 -->
      <div class="info-section">
        <span class="info-item">
          <el-icon><User /></el-icon>
          {{ detail?.creatorName }}
        </span>
        <span class="info-item">
          <el-icon><Clock /></el-icon>
          {{ detail?.publishTime }}
        </span>
        <el-tag v-if="detail?.type === 2" type="warning" size="small">
          {{ t('announcement.type.targeted') }}: {{ detail?.targetRoleNames?.join(', ') }}
        </el-tag>
      </div>

      <!-- 内容 -->
      <div class="content-section">
        <div class="html-content" v-html="detail?.content"></div>
      </div>

      <!-- 附件 -->
      <div v-if="detail?.attachments && detail.attachments.length > 0" class="attachment-section">
        <div class="section-label">{{ t('announcement.detail.attachments') }}</div>
        <div class="attachment-list">
          <div
            v-for="file in detail.attachments"
            :key="file.id"
            class="attachment-item"
            @click="handlePreview(file)"
          >
            <el-icon><Document /></el-icon>
            <span class="file-name">{{ file.fileName }}</span>
            <span class="file-size">{{ file.fileSizeFormat || formatFileSize(file.fileSize) }}</span>
            <el-icon class="preview-icon"><View /></el-icon>
          </div>
        </div>
      </div>

      <!-- 未读提示 -->
      <div v-if="detail?.isRead === 0" class="unread-tip">
        <el-alert type="info" :closable="false">
          {{ t('announcement.detail.unreadTip') }}
        </el-alert>
      </div>
    </div>

    <!-- 图片预览 -->
    <el-image
      v-if="imagePreviewVisible"
      :src="previewImageUrl"
      :preview-src-list="[previewImageUrl]"
      :initial-index="0"
      fit="contain"
      class="preview-image-hidden"
      @close="imagePreviewVisible = false"
    />

    <!-- 文件预览弹窗 -->
    <el-dialog
      v-model="filePreviewVisible"
      :title="previewFileName"
      width="80%"
      top="5vh"
      append-to-body
      destroy-on-close
    >
      <!-- 图片预览 -->
      <div v-if="previewType === 'image'" class="preview-container">
        <img :src="previewFileUrl" style="max-width: 100%; max-height: 70vh;" />
      </div>

      <!-- PDF 预览 -->
      <div v-else-if="previewType === 'pdf'" class="preview-container">
        <iframe
          :src="previewFileUrl"
          style="width: 100%; height: 70vh; border: none;"
        />
      </div>

      <!-- Word 文档预览 -->
      <div v-else-if="previewType === 'word'" class="preview-container office-preview">
        <VueOfficeDocx :src="previewFileUrl" />
      </div>

      <!-- Excel 文档预览 -->
      <div v-else-if="previewType === 'excel'" class="preview-container office-preview">
        <VueOfficeExcel :src="previewFileUrl" />
      </div>

      <!-- 其他文件提示下载 -->
      <div v-else class="preview-container download-tip">
        <el-icon :size="48"><Document /></el-icon>
        <p>此文件类型不支持在线预览</p>
        <el-button type="primary" @click="handleDownload(previewFileUrl, previewFileName)">
          点击下载
        </el-button>
      </div>
    </el-dialog>

    <template #footer>
      <el-button v-if="detail?.isRead === 0" type="primary" :loading="markingRead" @click="handleMarkRead">
        {{ t('announcement.detail.markRead') }}
      </el-button>
      <el-button @click="visible = false">
        {{ t('common.button.close') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { User, Clock, Document, View } from '@element-plus/icons-vue'
// vue-office 样式和组件
import '@vue-office/docx/lib/index.css'
import '@vue-office/excel/lib/index.css'
import VueOfficeDocx from '@vue-office/docx'
import VueOfficeExcel from '@vue-office/excel'
import { getAnnouncementDetail, markReadAnnouncement } from '@/api/announcement/announcementApi'
import { useLocale } from '@/composables/useLocale'
import type { Announcement, FileAttachment } from '@/types'

const props = defineProps<{
  modelValue: boolean
  id: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const loading = ref(false)
const markingRead = ref(false)
const detail = ref<Announcement | null>(null)
const imagePreviewVisible = ref(false)
const previewImageUrl = ref('')
const filePreviewVisible = ref(false)
const previewFileName = ref('')
const previewFileUrl = ref('')
const previewType = ref<'image' | 'pdf' | 'word' | 'excel' | 'other'>('other')

// 加载详情
const loadDetail = async () => {
  if (!props.id) return
  loading.value = true
  try {
    detail.value = await getAnnouncementDetail(props.id)
  } catch {
    // Error handled
  } finally {
    loading.value = false
  }
}

// 监听弹窗打开
watch(visible, (val) => {
  if (val && props.id) {
    loadDetail()
  }
})

// 标记已读
const handleMarkRead = async () => {
  if (!props.id) return
  markingRead.value = true
  try {
    await markReadAnnouncement(props.id)
    ElMessage.success(t('announcement.detail.markReadSuccess'))
    detail.value = { ...detail.value!, isRead: 1 }
    emit('success')
  } catch {
    // Error handled
  } finally {
    markingRead.value = false
  }
}

// 预览附件
const handlePreview = (file: FileAttachment) => {
  const ext = file.fileExt.toLowerCase()
  const url = file.url || ''

  previewFileName.value = file.fileName
  previewFileUrl.value = url

  if (['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp', 'svg'].includes(ext)) {
    // 图片预览
    previewType.value = 'image'
    filePreviewVisible.value = true
  } else if (ext === 'pdf') {
    // PDF 预览
    previewType.value = 'pdf'
    filePreviewVisible.value = true
  } else if (['doc', 'docx'].includes(ext)) {
    // Word 文档预览（使用 vue-office）
    previewType.value = 'word'
    filePreviewVisible.value = true
  } else if (['xls', 'xlsx'].includes(ext)) {
    // Excel 文档预览（使用 vue-office）
    previewType.value = 'excel'
    filePreviewVisible.value = true
  } else if (['ppt', 'pptx'].includes(ext)) {
    // PPT 暂不支持，提示下载
    previewType.value = 'other'
    filePreviewVisible.value = true
  } else {
    // 其他文件不支持预览
    previewType.value = 'other'
    filePreviewVisible.value = true
  }
}

// 下载文件
const handleDownload = (url: string, fileName: string) => {
  const link = document.createElement('a')
  link.href = url
  link.download = fileName
  link.target = '_blank'
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}

// 格式化文件大小
const formatFileSize = (size: number): string => {
  if (size < 1024) return `${size} B`
  if (size < 1024 * 1024) return `${(size / 1024).toFixed(2)} KB`
  return `${(size / (1024 * 1024)).toFixed(2)} MB`
}

const getLevelType = (level?: number): 'info' | 'warning' | 'danger' => {
  if (!level) return 'info'
  const map: Record<number, 'info' | 'warning' | 'danger'> = { 1: 'info', 2: 'warning', 3: 'danger' }
  return map[level] || 'info'
}

const getLevelText = (level?: number): string => {
  if (!level) return ''
  const map: Record<number, string> = { 1: t('announcement.level.normal'), 2: t('announcement.level.important'), 3: t('announcement.level.urgent') }
  return map[level] || ''
}
</script>

<style scoped lang="scss">
.detail-content {
  .header-section {
    display: flex;
    align-items: center;
    gap: 12px;
    margin-bottom: 16px;

    .title {
      font-size: 20px;
      font-weight: 600;
      margin: 0;
      flex: 1;
    }

    .level-tag {
      flex-shrink: 0;
    }
  }

  .info-section {
    display: flex;
    align-items: center;
    gap: 16px;
    margin-bottom: 24px;
    color: #666;
    font-size: 14px;

    .info-item {
      display: flex;
      align-items: center;
      gap: 4px;
    }
  }

  .content-section {
    margin-bottom: 24px;
    padding: 16px;
    background: #f5f7fa;
    border-radius: 8px;

    .html-content {
      line-height: 1.6;

      img {
        max-width: 100%;
        height: auto;
      }
    }
  }

  .attachment-section {
    .section-label {
      font-size: 14px;
      font-weight: 500;
      margin-bottom: 8px;
      color: #303133;
    }

    .attachment-list {
      display: flex;
      flex-direction: column;
      gap: 8px;

      .attachment-item {
        display: flex;
        align-items: center;
        gap: 8px;
        padding: 8px 12px;
        background: #f5f7fa;
        border-radius: 4px;
        cursor: pointer;
        transition: background-color 0.2s;

        &:hover {
          background: #e6e8eb;
        }

        .file-name {
          flex: 1;
          color: #303133;
        }

        .file-size {
          color: #909399;
          font-size: 12px;
        }

        .preview-icon {
          color: #409eff;
        }
      }
    }
  }

  .unread-tip {
    margin-top: 16px;
  }
}

.preview-image-hidden {
  position: fixed;
  top: -9999px;
  left: -9999px;
  opacity: 0;
  pointer-events: none;
}

.preview-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 400px;
}

.office-preview {
  width: 100%;
  height: 70vh;
  overflow: auto;
}

.download-tip {
  text-align: center;
  color: #909399;

  p {
    margin: 16px 0;
  }
}
</style>