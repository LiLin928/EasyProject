<!-- src/components/ant_workflow/DagDesigner/Toolbar.vue -->
<template>
  <div class="dag-toolbar">
    <!-- 工具按钮组 -->
    <div class="tool-group">
      <el-button :disabled="!canUndo" @click="handleUndo">
        <el-icon><RefreshLeft /></el-icon>
        {{ t('antWorkflow.designer.undo') }}
      </el-button>
      <el-button :disabled="!canRedo" @click="handleRedo">
        <el-icon><RefreshRight /></el-icon>
        {{ t('antWorkflow.designer.redo') }}
      </el-button>
      <el-button @click="handleAutoLayout">
        <el-icon><Grid /></el-icon>
        {{ t('antWorkflow.designer.autoLayout') }}
      </el-button>
    </div>

    <!-- 流程名称 -->
    <div class="workflow-title">
      <span class="workflow-name">{{ workflowName }}</span>
      <el-tag v-if="workflowStatus === 2" size="small" type="success">
        {{ t('antWorkflow.published') }}
      </el-tag>
      <el-tag v-else-if="workflowStatus === 0" size="small" type="info">
        {{ t('antWorkflow.draft') }}
      </el-tag>
    </div>

    <!-- 右侧操作按钮 -->
    <div class="action-group">
      <el-button v-if="!readonly" @click="handleSave" :loading="saveLoading">
        <el-icon><DocumentChecked /></el-icon>
        {{ t('antWorkflow.designer.save') }}
      </el-button>
      <el-button v-if="!readonly" type="primary" @click="handlePublish" :loading="publishLoading">
        <el-icon><Promotion /></el-icon>
        {{ t('antWorkflow.publish') }}
      </el-button>
      <el-button @click="handlePreview">
        <el-icon><View /></el-icon>
        {{ t('antWorkflow.preview') }}
      </el-button>
      <!-- 返回按钮 -->
      <el-button @click="handleBack">
        {{ t('antWorkflow.designer.back') }}
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  RefreshLeft,
  RefreshRight,
  Grid,
  DocumentChecked,
  Promotion,
  View,
} from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import { WorkflowStatus } from '@/types/antWorkflow'

const { t } = useLocale()

// Props
const props = defineProps<{
  canUndo: boolean
  canRedo: boolean
  workflowName: string
  workflowStatus: WorkflowStatus
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
  (e: 'preview'): void
}>()

// 加载状态
const saveLoading = ref(false)
const publishLoading = ref(false)

// ========== 工具栏操作 ==========

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
  saveLoading.value = true
  emit('save')
  // 保存完成后重置加载状态（由父组件控制）
  setTimeout(() => {
    saveLoading.value = false
  }, 500)
}

const handlePublish = () => {
  publishLoading.value = true
  emit('publish')
  setTimeout(() => {
    publishLoading.value = false
  }, 500)
}

const handlePreview = () => {
  emit('preview')
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

  .tool-group {
    display: flex;
    gap: 8px;
  }

  .workflow-title {
    display: flex;
    align-items: center;
    gap: 8px;
    margin-left: 16px;

    .workflow-name {
      font-size: 16px;
      font-weight: 500;
      color: #303133;
    }
  }

  .action-group {
    display: flex;
    gap: 8px;
    margin-left: auto;
  }
}
</style>