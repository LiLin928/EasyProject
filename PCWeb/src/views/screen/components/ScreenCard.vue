<!-- 大屏卡片组件 -->
<template>
  <div class="screen-card" @click="handlePreview">
    <div class="screen-card-content">
      <div class="screen-name">{{ screen.name }}</div>
      <div class="screen-description">{{ screen.description || t('screen.screen.noDescription') }}</div>
      <div class="screen-meta">
        <span class="screen-status-tag" :class="screen.isPublic ? 'screen-status-tag--public' : 'screen-status-tag--private'">
          {{ screen.isPublic ? t('screen.screen.public') : t('screen.screen.private') }}
        </span>
        <span>{{ formatDate(screen.updateTime) }}</span>
        <span v-if="publishInfo?.published" class="publish-status published">已发布</span>
      </div>
    </div>
    <div class="screen-card-actions">
      <el-button link type="primary" @click.stop="handlePreview">
        <el-icon><View /></el-icon>
        {{ t('screen.screen.preview') }}
      </el-button>
      <el-button link type="primary" @click.stop="handleDesign">
        <el-icon><Edit /></el-icon>
        {{ t('screen.screen.design') }}
      </el-button>
      <el-button link type="primary" @click.stop="handlePublish">
        <el-icon><Promotion /></el-icon>
        {{ publishInfo?.published ? '更新发布' : '发布' }}
      </el-button>
      <el-button v-if="publishInfo?.published" link type="success" @click.stop="handleViewPublished">
        <el-icon><Link /></el-icon>
        查看
      </el-button>
      <el-button link type="primary" @click.stop="handleCopy">
        <el-icon><CopyDocument /></el-icon>
        {{ t('screen.screen.copy') }}
      </el-button>
      <el-button link type="danger" @click.stop="handleDelete">
        <el-icon><Delete /></el-icon>
        {{ t('screen.screen.delete') }}
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { View, Edit, CopyDocument, Delete, Promotion, Link } from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import { deleteScreen, copyScreen, publishScreen, getScreenPublishInfo } from '@/api/screen'
import type { ScreenConfig } from '@/types'

const props = defineProps<{
  screen: ScreenConfig
}>()

const emit = defineEmits<{
  (e: 'refresh'): void
}>()

const { t } = useLocale()

const publishInfo = ref<{ published: boolean; publishId?: string; publishUrl?: string } | null>(null)

const formatDate = (dateStr: string | undefined | null) => {
  if (!dateStr) return '-'
  const date = new Date(dateStr)
  if (isNaN(date.getTime())) return '-'
  return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`
}

const loadPublishInfo = async () => {
  try {
    publishInfo.value = await getScreenPublishInfo(props.screen.id)
  } catch (error) {
    // Ignore
  }
}

// 预览：在新窗口打开
const handlePreview = () => {
  window.open(`/#/screen/view/${props.screen.id}`, '_blank')
}

// 设计：在新窗口打开
const handleDesign = () => {
  window.open(`/#/screen/design/${props.screen.id}`, '_blank')
}

// 发布
const handlePublish = async () => {
  try {
    await ElMessageBox.confirm(
      publishInfo.value?.published
        ? '确定要更新发布吗？发布后的链接将保持不变。'
        : '确定要发布此大屏吗？发布后将生成一个可公开访问的链接。',
      '发布确认',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'info',
      }
    )
    const result = await publishScreen(props.screen.id)
    ElMessage.success(publishInfo.value?.published ? '更新发布成功' : '发布成功')
    await loadPublishInfo()

    // 显示发布链接
    if (result.publishUrl) {
      ElMessageBox.alert(
        `发布链接：${result.publishUrl}\n发布ID：${result.publishId}`,
        '发布成功',
        {
          confirmButtonText: '复制链接',
          type: 'success',
        }
      ).then(() => {
        navigator.clipboard.writeText(result.publishUrl)
        ElMessage.success('链接已复制到剪贴板')
      })
    }
  } catch (error) {
    // User cancelled
  }
}

// 查看发布后的大屏
const handleViewPublished = () => {
  if (publishInfo.value?.publishId) {
    window.open(`/#/screen/publish/${publishInfo.value.publishId}`, '_blank')
  }
}

const handleCopy = async () => {
  try {
    await copyScreen(props.screen.id)
    ElMessage.success(t('screen.screen.copySuccess'))
    emit('refresh')
  } catch (error) {
    // Error handled by interceptor
  }
}

const handleDelete = async () => {
  try {
    await ElMessageBox.confirm(
      t('screen.screen.deleteConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteScreen(props.screen.id)
    ElMessage.success(t('screen.screen.deleteSuccess'))
    emit('refresh')
  } catch (error) {
    // User cancelled or request failed
  }
}

onMounted(() => {
  loadPublishInfo()
})
</script>

<style scoped lang="scss">
.screen-card {
  cursor: pointer;
  background: #fff;
  border: 1px solid #e4e7ed;
  border-radius: 8px;
  transition: all 0.3s ease;

  &:hover {
    border-color: #409eff;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    transform: translateY(-2px);
  }
}

.screen-card-content {
  padding: 16px;

  .screen-name {
    font-size: 16px;
    font-weight: 600;
    color: #303133;
    margin-bottom: 8px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .screen-description {
    font-size: 14px;
    color: #909399;
    margin-bottom: 12px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .screen-meta {
    display: flex;
    align-items: center;
    gap: 8px;
    flex-wrap: wrap;
    font-size: 12px;
    color: #909399;
  }
}

.screen-status-tag {
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 12px;

  &--public {
    background: #f0f9eb;
    color: #67c23a;
  }

  &--private {
    background: #fdf6ec;
    color: #e6a23c;
  }
}

.publish-status {
  font-size: 12px;
  padding: 2px 6px;
  border-radius: 4px;

  &.published {
    background: #f0f9eb;
    color: #67c23a;
  }
}

.screen-card-actions {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 4px;
  padding: 12px;
  border-top: 1px solid #e4e7ed;
}
</style>