<!-- 画布工具栏 -->
<template>
  <div class="designer-toolbar">
    <!-- 左侧：撤销/重做 -->
    <div class="toolbar-left">
      <el-tooltip :content="t('screen.designer.undo')" placement="bottom">
        <el-button
          :disabled="!canUndo"
          @click="handleUndo"
        >
          <el-icon><RefreshLeft /></el-icon>
        </el-button>
      </el-tooltip>

      <el-tooltip :content="t('screen.designer.redo')" placement="bottom">
        <el-button
          :disabled="!canRedo"
          @click="handleRedo"
        >
          <el-icon><RefreshRight /></el-icon>
        </el-button>
      </el-tooltip>

      <el-divider direction="vertical" />

      <!-- 网格显示切换 -->
      <el-tooltip :content="showGrid ? '隐藏网格' : '显示网格'" placement="bottom">
        <el-button
          :type="showGrid ? 'primary' : 'default'"
          @click="handleToggleGrid"
        >
          <el-icon><Grid /></el-icon>
        </el-button>
      </el-tooltip>
    </div>

    <!-- 中间：缩放控制 -->
    <div class="toolbar-center">
      <el-select
        v-model="zoomValue"
        size="small"
        style="width: 100px"
        @change="handleZoomChange"
      >
        <el-option
          v-for="option in zoomOptions"
          :key="option.value"
          :label="option.label"
          :value="option.value"
        />
      </el-select>

      <el-button-group style="margin-left: 8px">
        <el-button size="small" @click="handleZoomOut">
          <el-icon><ZoomOut /></el-icon>
        </el-button>
        <el-button size="small" @click="handleZoomIn">
          <el-icon><ZoomIn /></el-icon>
        </el-button>
        <el-button size="small" @click="handleFitWindow">
          {{ t('screen.designer.fitWindow') }}
        </el-button>
      </el-button-group>
    </div>

    <!-- 右侧：预览/保存/关闭 -->
    <div class="toolbar-right">
      <el-button @click="handlePreview">
        <el-icon><View /></el-icon>
        {{ t('screen.designer.preview') }}
      </el-button>

      <el-button type="primary" :loading="saving" @click="handleSave">
        <el-icon><FolderChecked /></el-icon>
        {{ t('screen.designer.save') }}
      </el-button>

      <el-divider direction="vertical" />

      <el-tooltip :content="t('screen.designer.close')" placement="bottom">
        <el-button @click="handleClose">
          <el-icon><Close /></el-icon>
        </el-button>
      </el-tooltip>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import {
  RefreshLeft,
  RefreshRight,
  Grid,
  ZoomIn,
  ZoomOut,
  View,
  FolderChecked,
  Close,
} from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import { useScreenStore } from '@/stores/screen'
import { storeToRefs } from 'pinia'

const { t } = useLocale()
const router = useRouter()
const store = useScreenStore()

const { canvasZoom, canUndo, canRedo, isDirty, currentScreen } = storeToRefs(store)

const props = defineProps<{
  showGrid: boolean
}>()

const emit = defineEmits<{
  (e: 'toggle-grid'): void
  (e: 'preview'): void
}>()

// 状态
const saving = ref(false)
const zoomValue = ref(100)

// 缩放选项
const zoomOptions = [
  { label: '50%', value: 50 },
  { label: '75%', value: 75 },
  { label: '100%', value: 100 },
  { label: '125%', value: 125 },
  { label: '150%', value: 150 },
  { label: '200%', value: 200 },
]

// 监听缩放值变化
watch(canvasZoom, (val) => {
  zoomValue.value = Math.round(val * 100)
}, { immediate: true })

// 撤销
const handleUndo = () => {
  store.undo()
}

// 重做
const handleRedo = () => {
  store.redo()
}

// 切换网格
const handleToggleGrid = () => {
  emit('toggle-grid')
}

// 缩放选择变化
const handleZoomChange = (value: number) => {
  store.setCanvasZoom(value / 100)
}

// 放大
const handleZoomIn = () => {
  const newZoom = Math.min(2, canvasZoom.value + 0.25)
  store.setCanvasZoom(newZoom)
}

// 缩小
const handleZoomOut = () => {
  const newZoom = Math.max(0.5, canvasZoom.value - 0.25)
  store.setCanvasZoom(newZoom)
}

// 适应窗口
const handleFitWindow = () => {
  // TODO: 计算适应窗口的缩放比例
  store.setCanvasZoom(1)
}

// 预览
const handlePreview = () => {
  emit('preview')
}

// 保存
const handleSave = async () => {
  saving.value = true
  try {
    const success = await store.saveScreen()
    if (success) {
      ElMessage.success(t('screen.designer.saveSuccess'))
    } else {
      ElMessage.error(t('screen.designer.saveFailed'))
    }
  } finally {
    saving.value = false
  }
}

// 关闭/返回
const handleClose = () => {
  router.push('/screen/list')
}

// 暴露方法
defineExpose({
  hasUnsavedChanges: () => isDirty.value,
})
</script>

<style scoped lang="scss">
.designer-toolbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 8px 16px;
  background: rgba(255, 255, 255, 0.1);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);

  :deep(.el-button) {
    background: rgba(255, 255, 255, 0.1);
    border-color: rgba(255, 255, 255, 0.2);
    color: #fff;

    &:hover {
      background: rgba(255, 255, 255, 0.2);
    }

    &.is-disabled {
      opacity: 0.5;
    }
  }

  :deep(.el-select) {
    .el-input__wrapper {
      background: rgba(255, 255, 255, 0.1);
      box-shadow: none;
    }

    .el-input__inner {
      color: #fff;
    }
  }

  :deep(.el-divider) {
    border-color: rgba(255, 255, 255, 0.2);
  }
}

.toolbar-left,
.toolbar-center,
.toolbar-right {
  display: flex;
  align-items: center;
  gap: 8px;
}
</style>