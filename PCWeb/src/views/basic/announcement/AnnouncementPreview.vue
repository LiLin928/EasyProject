<!-- src/views/basic/announcement/AnnouncementPreview.vue -->
<template>
  <div class="announcement-preview-container">
    <!-- 顶部标题栏 -->
    <div class="header-bar">
      <!-- 面包屑 -->
      <el-breadcrumb separator="/">
        <el-breadcrumb-item :to="{ path: '/basic/announcement/list' }">
          {{ t('announcement.list.title') }}
        </el-breadcrumb-item>
        <el-breadcrumb-item>
          {{ t('announcement.detail.title') }}
        </el-breadcrumb-item>
      </el-breadcrumb>

      <!-- 操作按钮 -->
      <div class="action-buttons">
        <el-button v-if="detail?.status === 0 || detail?.status === 2" type="primary" @click="handleEdit">
          {{ t('common.button.edit') }}
        </el-button>
        <el-button @click="handleBack">
          {{ t('common.button.back') }}
        </el-button>
      </div>
    </div>

    <el-card v-loading="loading" shadow="never" class="preview-card">
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
        <el-button type="primary" :loading="markingRead" @click="handleMarkRead" style="margin-top: 12px;">
          {{ t('announcement.detail.markRead') }}
        </el-button>
      </div>
    </el-card>

    <!-- 附件预览弹窗 -->
    <el-dialog
      v-model="previewDialogVisible"
      :title="previewFile?.fileName"
      width="80%"
      top="5vh"
      destroy-on-close
    >
      <!-- 图片预览 -->
      <div v-if="previewType === 'image'" class="preview-container">
        <img :src="previewFile?.url" style="max-width: 100%; max-height: 70vh;" />
      </div>

      <!-- PDF 预览 -->
      <div v-else-if="previewType === 'pdf'" class="preview-container">
        <iframe :src="previewFile?.url" style="width: 100%; height: 70vh; border: none;" />
      </div>

      <!-- Word 文档预览 -->
      <div v-else-if="previewType === 'word'" class="preview-container">
        <VueOfficeDocx
          :src="previewFile?.url"
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
          :src="previewFile?.url"
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
        <el-button type="primary" @click="handleDownload">
          点击下载
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
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

const { t } = useLocale()
const router = useRouter()
const route = useRoute()

const loading = ref(false)
const markingRead = ref(false)
const detail = ref<Announcement | null>(null)
const previewDialogVisible = ref(false)
const previewFile = ref<FileAttachment | null>(null)
const previewType = ref<'image' | 'pdf' | 'word' | 'excel' | 'other'>('other')

const announcementId = computed(() => route.params.id as string)

// 加载详情
const loadDetail = async () => {
  if (!announcementId.value) return
  loading.value = true
  try {
    detail.value = await getAnnouncementDetail(announcementId.value)
  } catch {
    ElMessage.error(t('common.message.loadFailed'))
    router.push('/basic/announcement/list')
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadDetail()
})

// 预览附件
const handlePreview = (file: FileAttachment) => {
  const ext = file.fileExt.toLowerCase()
  previewFile.value = file

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

// 下载文件
const handleDownload = () => {
  if (!previewFile.value) return
  const link = document.createElement('a')
  link.href = previewFile.value.url || ''
  link.download = previewFile.value.fileName
  link.target = '_blank'
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}

// 标记已读
const handleMarkRead = async () => {
  if (!announcementId.value) return
  markingRead.value = true
  try {
    await markReadAnnouncement(announcementId.value)
    ElMessage.success(t('announcement.detail.markReadSuccess'))
    detail.value = { ...detail.value!, isRead: 1 }
  } catch {
    // Error handled
  } finally {
    markingRead.value = false
  }
}

// 编辑
const handleEdit = () => {
  router.push(`/basic/announcement/edit/${announcementId.value}`)
}

// 返回
const handleBack = () => {
  router.push('/basic/announcement/list')
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
.announcement-preview-container {
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

  .preview-card {
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
      margin-top: 24px;
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