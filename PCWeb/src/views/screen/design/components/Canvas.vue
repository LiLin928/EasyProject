<!-- 画布区域 -->
<template>
  <div
    class="canvas-container"
    ref="containerRef"
    @dragover.prevent="handleDragOver"
    @drop="handleDrop"
    @dragleave="handleDragLeave"
  >
    <!-- 工具栏插槽 -->
    <div class="canvas-toolbar-slot">
      <slot name="toolbar"></slot>
    </div>

    <!-- 画布包装器（可滚动） -->
    <div class="canvas-wrapper" ref="wrapperRef">
      <!-- 占位容器，确保滚动区域足够大 -->
      <div class="canvas-spacer" :style="spacerStyle">
        <!-- 画布 -->
        <div
          class="canvas"
          ref="canvasRef"
          :style="canvasStyle"
          @click="handleCanvasClick"
        >
          <!-- 网格背景 -->
          <div v-if="showGrid" class="canvas-grid" :style="gridStyle"></div>

          <!-- 组件列表 -->
          <CanvasComponent
            v-for="component in screen?.components"
            :key="component.id"
            :component="component"
            :is-selected="selectedComponentId === component.id"
            :zoom="zoom"
            @select="handleSelectComponent(component.id)"
            @move="handleMoveComponent(component.id, $event)"
            @resize="handleResizeComponent(component.id, $event)"
            @dblclick="handleComponentDblClick(component)"
          />

          <!-- 拖拽放置提示 -->
          <div
            v-if="isDragOver"
            class="drop-indicator"
            :style="dropIndicatorStyle"
          >
            <el-icon><Plus /></el-icon>
            <span>放置组件</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Plus } from '@element-plus/icons-vue'
import type { ScreenConfig, ScreenComponent, ComponentType } from '@/types'
import { useScreenStore } from '@/stores/screen'
import { storeToRefs } from 'pinia'
import CanvasComponent from './CanvasComponent.vue'

const store = useScreenStore()
const { currentScreen, selectedComponentId, canvasZoom } = storeToRefs(store)

const props = defineProps<{
  showGrid?: boolean
}>()

const emit = defineEmits<{
  (e: 'component-dblclick', component: ScreenComponent): void
}>()

// 引用
const containerRef = ref<HTMLDivElement>()
const wrapperRef = ref<HTMLDivElement>()
const canvasRef = ref<HTMLDivElement>()

// 状态
const isDragOver = ref(false)
const dropPosition = ref({ x: 0, y: 0 })

// 计算属性
const screen = computed(() => currentScreen.value)
const zoom = computed(() => canvasZoom.value)

const canvasStyle = computed(() => ({
  width: `${screen.value?.style.width || 1920}px`,
  height: `${screen.value?.style.height || 1080}px`,
  background: screen.value?.style.background || 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
  transform: `scale(${zoom.value})`,
  transformOrigin: 'top left',
}))

const gridStyle = computed(() => ({
  backgroundSize: '20px 20px',
  backgroundImage: `
    linear-gradient(to right, rgba(255, 255, 255, 0.05) 1px, transparent 1px),
    linear-gradient(to bottom, rgba(255, 255, 255, 0.05) 1px, transparent 1px)
  `,
}))

// 占位容器样式，比画布大一些以确保有滚动空间
const spacerStyle = computed(() => ({
  width: `${(screen.value?.style.width || 1920) * zoom.value + 40}px`,
  height: `${(screen.value?.style.height || 1080) * zoom.value + 40}px`,
}))

const dropIndicatorStyle = computed(() => ({
  left: `${dropPosition.value.x}px`,
  top: `${dropPosition.value.y}px`,
}))

// 拖拽悬停事件
const handleDragOver = (event: DragEvent) => {
  event.preventDefault()
  isDragOver.value = true

  // 计算放置位置（相对于画布）
  const rect = canvasRef.value?.getBoundingClientRect()
  if (rect) {
    dropPosition.value = {
      x: (event.clientX - rect.left) / zoom.value,
      y: (event.clientY - rect.top) / zoom.value,
    }
  }
}

// 放置事件
const handleDrop = (event: DragEvent) => {
  event.preventDefault()
  isDragOver.value = false

  const componentType = event.dataTransfer?.getData('componentType') as ComponentType
  console.log('Drop event, componentType:', componentType)

  if (!componentType) return

  // 计算放置位置（相对于画布）
  const rect = canvasRef.value?.getBoundingClientRect()
  if (rect) {
    const position = {
      x: (event.clientX - rect.left) / zoom.value,
      y: (event.clientY - rect.top) / zoom.value,
    }
    console.log('Adding component at:', position)
    store.addComponent(componentType, position)
  }
}

// 拖拽离开
const handleDragLeave = (event: DragEvent) => {
  // 只有当离开整个容器时才设置为 false
  if (event.target === containerRef.value) {
    isDragOver.value = false
  }
}

// 点击画布空白区域
const handleCanvasClick = (event: MouseEvent) => {
  if (event.target === canvasRef.value) {
    store.selectComponent(null)
  }
}

// 选择组件
const handleSelectComponent = (id: string) => {
  store.selectComponent(id)
}

// 移动组件
const handleMoveComponent = (id: string, position: { x: number; y: number }) => {
  store.moveComponent(id, position)
}

// 缩放组件
const handleResizeComponent = (id: string, size: { width: number; height: number }) => {
  store.resizeComponent(id, size)
}

// 双击组件
const handleComponentDblClick = (component: ScreenComponent) => {
  emit('component-dblclick', component)
}

// 暴露方法供父组件调用
defineExpose({
  getCanvasElement: () => canvasRef.value,
})
</script>

<style scoped lang="scss">
.canvas-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  background: #1a1a2e;
}

.canvas-toolbar-slot {
  flex-shrink: 0;
}

.canvas-wrapper {
  flex: 1;
  overflow: auto;
}

.canvas-spacer {
  position: relative;
  // 画布放在左上角，有20px边距
  padding: 20px;
}

.canvas {
  position: relative;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.5);
}

.canvas-grid {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  pointer-events: none;
}

.drop-indicator {
  position: absolute;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 120px;
  height: 80px;
  background: rgba(64, 158, 255, 0.3);
  border: 2px dashed #409eff;
  border-radius: 8px;
  color: #fff;
  font-size: 12px;
  pointer-events: none;
  z-index: 100;

  .el-icon {
    font-size: 24px;
    margin-bottom: 4px;
  }
}
</style>