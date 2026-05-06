<!-- src/components/etl/DagDesigner/Toolbar.vue -->
<template>
  <div class="dag-toolbar">
    <!-- 撤销/重做 -->
    <el-button-group class="history-buttons">
      <el-button :disabled="!canUndo" @click="handleUndo">
        <el-icon><RefreshLeft /></el-icon>
        撤销
      </el-button>
      <el-button :disabled="!canRedo" @click="handleRedo">
        <el-icon><RefreshRight /></el-icon>
        重做
      </el-button>
    </el-button-group>

    <!-- 自动布局 -->
    <el-button v-if="!readonly" @click="handleAutoLayout">
      <el-icon><Grid /></el-icon>
      自动布局
    </el-button>

    <!-- 任务流名称和状态 -->
    <div class="toolbar-title">
      <span class="pipeline-name">{{ pipelineName }}</span>
      <el-tag
        :type="getStatusTagType(pipelineStatus)"
        size="small"
      >
        {{ getStatusText(pipelineStatus) }}
      </el-tag>
    </div>

    <!-- 右侧操作按钮 -->
    <div class="toolbar-actions">
      <el-button v-if="!readonly" @click="handleSave">
        <el-icon><DocumentChecked /></el-icon>
        保存
      </el-button>
      <el-button
        v-if="!readonly"
        type="primary"
        @click="handlePublish"
      >
        <el-icon><Promotion /></el-icon>
        发布
      </el-button>
      <el-button type="success" @click="handleRun">
        <el-icon><VideoPlay /></el-icon>
        运行
      </el-button>
      <!-- 返回按钮 -->
      <el-button @click="handleBack">
        返回
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { RefreshLeft, RefreshRight, Grid, DocumentChecked, Promotion, VideoPlay } from '@element-plus/icons-vue'
import { PipelineStatus } from '@/types/etl'

// Props
defineProps<{
  canUndo: boolean
  canRedo: boolean
  pipelineName: string
  pipelineStatus: PipelineStatus
  readonly: boolean
}>()

// Emits
const emit = defineEmits<{
  (e: 'back'): void
  (e: 'undo'): void
  (e: 'redo'): void
  (e: 'auto-layout'): void
  (e: 'save'): void
  (e: 'publish'): void
  (e: 'run'): void
}>()

// ========== 操作处理 ==========

const handleBack = () => {
  emit('back')
}

const handleUndo = () => {
  emit('undo')
}

const handleRedo = () => {
  emit('redo')
}

const handleAutoLayout = () => {
  emit('auto-layout')
}

const handleSave = () => {
  emit('save')
}

const handlePublish = () => {
  emit('publish')
}

const handleRun = () => {
  emit('run')
}

// ========== 状态显示 ==========

const getStatusTagType = (status: PipelineStatus | string) => {
  switch (status) {
    case PipelineStatus.DRAFT:
    case 'draft':
      return 'info'
    case PipelineStatus.PUBLISHED:
    case 'published':
      return 'success'
    case PipelineStatus.ARCHIVED:
    case 'archived':
      return 'warning'
    default:
      return 'info'
  }
}

const getStatusText = (status: PipelineStatus | string) => {
  switch (status) {
    case PipelineStatus.DRAFT:
    case 'draft':
      return '草稿'
    case PipelineStatus.PUBLISHED:
    case 'published':
      return '已发布'
    case PipelineStatus.ARCHIVED:
    case 'archived':
      return '已归档'
    default:
      return '未知'
  }
}
</script>

<style scoped lang="scss">
$toolbar-height: 56px;
$toolbar-bg: #fff;
$toolbar-border: #e4e7ed;

.dag-toolbar {
  height: $toolbar-height;
  display: flex;
  align-items: center;
  padding: 0 20px;
  background: $toolbar-bg;
  border-bottom: 1px solid $toolbar-border;
  gap: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);

  .history-buttons {
    margin-left: 8px;
  }

  .toolbar-title {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-left: 16px;

    .pipeline-name {
      font-size: 16px;
      font-weight: 500;
      color: #303133;
    }
  }

  .toolbar-actions {
    display: flex;
    gap: 8px;
    margin-left: auto;
  }
}
</style>