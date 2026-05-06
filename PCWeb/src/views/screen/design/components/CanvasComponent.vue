<!-- 画布上的可拖拽组件包装器 -->
<template>
  <div
    class="canvas-component"
    :class="{
      'canvas-component--selected': isSelected,
      'canvas-component--locked': component.locked,
    }"
    :style="componentStyle"
    @mousedown.stop="handleMouseDown"
    @dblclick="handleDoubleClick"
  >
    <!-- 组件内容 -->
    <div class="component-content">
      <!-- 标题组件 -->
      <div v-if="component.type === 'title'" class="component-title">
        {{ component.config.text }}
      </div>

      <!-- 数字卡片 -->
      <div v-else-if="component.type === 'number-card'" class="component-number-card">
        <div class="card-value" :style="{ color: component.config.color || '#409eff' }">
          {{ formatNumber(component.dataSource.data?.[0]?.value) }}
        </div>
        <div class="card-label">{{ component.dataSource.data?.[0]?.label || component.config.title }}</div>
      </div>

      <!-- 图表组件 -->
      <BaseChart
        v-else-if="isChartType(component.type)"
        :component="component"
        :design-mode="true"
      />

      <!-- 图片组件 -->
      <div v-else-if="component.type === 'image'" class="component-image">
        <img
          v-if="component.config.url"
          :src="component.config.url"
          :style="imageStyle"
          alt="image"
        />
        <div v-else class="placeholder">
          <el-icon><Picture /></el-icon>
          <span>图片占位</span>
        </div>
      </div>

      <!-- 视频组件 -->
      <div v-else-if="component.type === 'video'" class="component-video">
        <video
          v-if="component.config.url"
          :src="component.config.url"
          :autoplay="component.config.autoplay"
          :loop="component.config.loop"
          :muted="component.config.muted"
          :controls="component.config.controls"
          style="width: 100%; height: 100%; object-fit: cover;"
        />
        <div v-else class="placeholder">
          <el-icon><VideoCamera /></el-icon>
          <span>视频占位</span>
        </div>
      </div>

      <!-- 边框组件 -->
      <div v-else-if="component.type === 'border'" class="component-border">
        <!-- 空边框，仅显示边框样式 -->
      </div>

      <!-- 数据表格 -->
      <div v-else-if="component.type === 'data-table'" class="component-data-table">
        <el-table :data="component.dataSource.data" size="small" style="width: 100%">
          <el-table-column
            v-for="col in component.config.columns"
            :key="col.field"
            :prop="col.field"
            :label="col.title"
            :width="col.width"
          />
        </el-table>
      </div>
    </div>

    <!-- 选中时显示调整点 -->
    <template v-if="isSelected && !component.locked">
      <div
        v-for="handle in resizeHandles"
        :key="handle.position"
        class="resize-handle"
        :class="`resize-handle--${handle.position}`"
        @mousedown.stop="handleResizeStart($event, handle.position)"
      />
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { Picture, VideoCamera } from '@element-plus/icons-vue'
import type { ScreenComponent, ComponentType } from '@/types'
import BaseChart from '@/views/screen/components/BaseChart.vue'

const props = defineProps<{
  component: ScreenComponent
  isSelected: boolean
  zoom: number
}>()

const emit = defineEmits<{
  (e: 'select'): void
  (e: 'move', position: { x: number; y: number }): void
  (e: 'resize', size: { width: number; height: number }): void
  (e: 'dblclick'): void
}>()

// 组件样式
const componentStyle = computed(() => ({
  left: `${props.component.position.x}px`,
  top: `${props.component.position.y}px`,
  width: `${props.component.size.width}px`,
  height: `${props.component.size.height}px`,
  transform: `scale(${props.zoom})`,
  transformOrigin: 'top left',
}))

// 图片样式
const imageStyle = computed(() => ({
  objectFit: props.component.config.fit || 'cover',
  opacity: (props.component.config.opacity ?? 100) / 100,
}))

// 是否为图表类型
const isChartType = (type: ComponentType) =>
  ['line-chart', 'bar-chart', 'pie-chart', 'radar-chart', 'funnel-chart', 'heatmap-chart'].includes(type)

// 格式化数字
const formatNumber = (num: number | undefined) => {
  if (num === undefined) return '0'
  if (num >= 10000) {
    return (num / 10000).toFixed(2) + '万'
  }
  return num.toLocaleString()
}

// 调整点定义
const resizeHandles = [
  { position: 'nw', cursor: 'nwse-resize' },
  { position: 'ne', cursor: 'nesw-resize' },
  { position: 'sw', cursor: 'nesw-resize' },
  { position: 'se', cursor: 'nwse-resize' },
  { position: 'n', cursor: 'ns-resize' },
  { position: 's', cursor: 'ns-resize' },
  { position: 'w', cursor: 'ew-resize' },
  { position: 'e', cursor: 'ew-resize' },
]

// 拖拽状态
const isDragging = ref(false)
const dragStartPos = ref({ x: 0, y: 0 })
const componentStartPos = ref({ x: 0, y: 0 })

// 缩放状态
const isResizing = ref(false)
const resizeHandle = ref('')
const resizeStartSize = ref({ width: 0, height: 0 })
const resizeStartPos2 = ref({ x: 0, y: 0 })

// 鼠标按下事件（开始拖拽）
const handleMouseDown = (event: MouseEvent) => {
  if (props.component.locked) return

  emit('select')

  isDragging.value = true
  dragStartPos.value = { x: event.clientX, y: event.clientY }
  componentStartPos.value = { ...props.component.position }

  document.addEventListener('mousemove', handleMouseMove)
  document.addEventListener('mouseup', handleMouseUp)
}

// 鼠标移动事件
const handleMouseMove = (event: MouseEvent) => {
  if (isDragging.value) {
    const deltaX = event.clientX - dragStartPos.value.x
    const deltaY = event.clientY - dragStartPos.value.y

    emit('move', {
      x: componentStartPos.value.x + deltaX / props.zoom,
      y: componentStartPos.value.y + deltaY / props.zoom,
    })
  }

  if (isResizing.value) {
    const deltaX = event.clientX - resizeStartPos2.value.x
    const deltaY = event.clientY - resizeStartPos2.value.y

    let newWidth = resizeStartSize.value.width
    let newHeight = resizeStartSize.value.height

    // 根据调整点位置计算新尺寸
    if (resizeHandle.value.includes('e')) {
      newWidth = Math.max(100, resizeStartSize.value.width + deltaX / props.zoom)
    }
    if (resizeHandle.value.includes('w')) {
      newWidth = Math.max(100, resizeStartSize.value.width - deltaX / props.zoom)
    }
    if (resizeHandle.value.includes('s')) {
      newHeight = Math.max(60, resizeStartSize.value.height + deltaY / props.zoom)
    }
    if (resizeHandle.value.includes('n')) {
      newHeight = Math.max(60, resizeStartSize.value.height - deltaY / props.zoom)
    }

    emit('resize', { width: newWidth, height: newHeight })
  }
}

// 鼠标释放事件
const handleMouseUp = () => {
  isDragging.value = false
  isResizing.value = false

  document.removeEventListener('mousemove', handleMouseMove)
  document.removeEventListener('mouseup', handleMouseUp)
}

// 开始缩放
const handleResizeStart = (event: MouseEvent, position: string) => {
  isResizing.value = true
  resizeHandle.value = position
  resizeStartSize.value = { ...props.component.size }
  resizeStartPos2.value = { x: event.clientX, y: event.clientY }

  emit('select')

  document.addEventListener('mousemove', handleMouseMove)
  document.addEventListener('mouseup', handleMouseUp)
}

// 双击事件
const handleDoubleClick = () => {
  emit('dblclick')
}
</script>

<style scoped lang="scss">
.canvas-component {
  position: absolute;
  box-sizing: border-box;
  cursor: move;
  transition: box-shadow 0.2s;

  &:hover:not(.canvas-component--locked) {
    box-shadow: 0 0 0 1px rgba(64, 158, 255, 0.5);
  }

  &--selected {
    box-shadow: 0 0 0 2px #409eff;
    z-index: 10;
  }

  &--locked {
    cursor: not-allowed;
    opacity: 0.7;
  }
}

.component-content {
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.component-title {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  color: #fff;
  font-weight: 600;
  font-size: 24px;
}

.component-number-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  width: 100%;
  height: 100%;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 8px;

  .card-value {
    font-size: 28px;
    font-weight: 700;
  }

  .card-label {
    font-size: 12px;
    color: rgba(255, 255, 255, 0.7);
    margin-top: 4px;
  }
}

.component-image,
.component-video {
  width: 100%;
  height: 100%;

  .placeholder {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 100%;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 4px;
    color: rgba(255, 255, 255, 0.5);

    .el-icon {
      font-size: 32px;
      margin-bottom: 8px;
    }
  }
}

.component-border {
  width: 100%;
  height: 100%;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-radius: 8px;
  box-sizing: border-box;
}

.component-data-table {
  width: 100%;
  height: 100%;
  padding: 8px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 4px;

  :deep(.el-table) {
    background: transparent;
    --el-table-bg-color: transparent;
    --el-table-tr-bg-color: transparent;
    --el-table-header-bg-color: rgba(255, 255, 255, 0.1);
    --el-table-text-color: rgba(255, 255, 255, 0.8);
    --el-table-border-color: rgba(255, 255, 255, 0.1);
  }
}

// 调整点样式
.resize-handle {
  position: absolute;
  width: 8px;
  height: 8px;
  background: #409eff;
  border: 1px solid #fff;
  border-radius: 2px;
  z-index: 20;

  &--nw { top: -4px; left: -4px; cursor: nwse-resize; }
  &--ne { top: -4px; right: -4px; cursor: nesw-resize; }
  &--sw { bottom: -4px; left: -4px; cursor: nesw-resize; }
  &--se { bottom: -4px; right: -4px; cursor: nwse-resize; }
  &--n { top: -4px; left: 50%; transform: translateX(-50%); cursor: ns-resize; }
  &--s { bottom: -4px; left: 50%; transform: translateX(-50%); cursor: ns-resize; }
  &--w { left: -4px; top: 50%; transform: translateY(-50%); cursor: ew-resize; }
  &--e { right: -4px; top: 50%; transform: translateY(-50%); cursor: ew-resize; }
}
</style>