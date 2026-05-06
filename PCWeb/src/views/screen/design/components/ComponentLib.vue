<!-- 组件库面板 -->
<template>
  <div class="component-lib">
    <!-- 搜索框 -->
    <div class="component-search">
      <el-input
        v-model="searchKeyword"
        :placeholder="t('common.button.search')"
        clearable
        size="small"
      >
        <template #prefix>
          <el-icon><Search /></el-icon>
        </template>
      </el-input>
    </div>

    <!-- 组件分类列表 -->
    <el-collapse v-model="activeCategories" class="component-collapse">
      <!-- 基础图表 -->
      <el-collapse-item name="basic-charts">
        <template #title>
          <span class="category-title">{{ t('screen.componentCategory.basicCharts') }}</span>
        </template>
        <div class="component-grid">
          <div
            v-for="comp in filteredBasicCharts"
            :key="comp.type"
            class="component-item"
            draggable="true"
            @dragstart="handleDragStart($event, comp)"
          >
            <el-icon class="component-icon"><component :is="comp.icon" /></el-icon>
            <span class="component-name">{{ comp.name }}</span>
          </div>
        </div>
      </el-collapse-item>

      <!-- 高级图表 -->
      <el-collapse-item name="advanced-charts">
        <template #title>
          <span class="category-title">{{ t('screen.componentCategory.advancedCharts') }}</span>
        </template>
        <div class="component-grid">
          <div
            v-for="comp in filteredAdvancedCharts"
            :key="comp.type"
            class="component-item"
            draggable="true"
            @dragstart="handleDragStart($event, comp)"
          >
            <el-icon class="component-icon"><component :is="comp.icon" /></el-icon>
            <span class="component-name">{{ comp.name }}</span>
          </div>
        </div>
      </el-collapse-item>

      <!-- 信息组件 -->
      <el-collapse-item name="info-components">
        <template #title>
          <span class="category-title">{{ t('screen.componentCategory.infoComponents') }}</span>
        </template>
        <div class="component-grid">
          <div
            v-for="comp in filteredInfoComponents"
            :key="comp.type"
            class="component-item"
            draggable="true"
            @dragstart="handleDragStart($event, comp)"
          >
            <el-icon class="component-icon"><component :is="comp.icon" /></el-icon>
            <span class="component-name">{{ comp.name }}</span>
          </div>
        </div>
      </el-collapse-item>

      <!-- 装饰组件 -->
      <el-collapse-item name="decor-components">
        <template #title>
          <span class="category-title">{{ t('screen.componentCategory.decorComponents') }}</span>
        </template>
        <div class="component-grid">
          <div
            v-for="comp in filteredDecorComponents"
            :key="comp.type"
            class="component-item"
            draggable="true"
            @dragstart="handleDragStart($event, comp)"
          >
            <el-icon class="component-icon"><component :is="comp.icon" /></el-icon>
            <span class="component-name">{{ comp.name }}</span>
          </div>
        </div>
      </el-collapse-item>

      <!-- 媒体组件 -->
      <el-collapse-item name="media-components">
        <template #title>
          <span class="category-title">{{ t('screen.componentCategory.mediaComponents') }}</span>
        </template>
        <div class="component-grid">
          <div
            v-for="comp in filteredMediaComponents"
            :key="comp.type"
            class="component-item"
            draggable="true"
            @dragstart="handleDragStart($event, comp)"
          >
            <el-icon class="component-icon"><component :is="comp.icon" /></el-icon>
            <span class="component-name">{{ comp.name }}</span>
          </div>
        </div>
      </el-collapse-item>
    </el-collapse>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  Search,
  TrendCharts,
  Histogram,
  PieChart,
  DataAnalysis,
  DataLine,
  Grid,
  Calendar,
  Tickets,
  Document,
  Picture,
  VideoCamera,
} from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import type { ComponentType } from '@/types'

const { t } = useLocale()

const searchKeyword = ref('')
const activeCategories = ref(['basic-charts', 'advanced-charts', 'info-components', 'decor-components', 'media-components'])

// 组件定义
interface ComponentItem {
  type: ComponentType
  name: string
  icon: any
  category: string
}

const allComponents: ComponentItem[] = [
  // 基础图表
  { type: 'line-chart', name: '折线图', icon: TrendCharts, category: 'basic-charts' },
  { type: 'bar-chart', name: '柱状图', icon: Histogram, category: 'basic-charts' },
  { type: 'pie-chart', name: '饼图', icon: PieChart, category: 'basic-charts' },
  // 高级图表
  { type: 'radar-chart', name: '雷达图', icon: DataAnalysis, category: 'advanced-charts' },
  { type: 'funnel-chart', name: '漏斗图', icon: DataLine, category: 'advanced-charts' },
  { type: 'heatmap-chart', name: '热力图', icon: Grid, category: 'advanced-charts' },
  // 信息组件
  { type: 'number-card', name: '数字卡片', icon: Calendar, category: 'info-components' },
  { type: 'data-table', name: '数据表格', icon: Tickets, category: 'info-components' },
  // 装饰组件
  { type: 'title', name: '标题', icon: Document, category: 'decor-components' },
  { type: 'border', name: '边框', icon: Document, category: 'decor-components' },
  // 媒体组件
  { type: 'image', name: '图片', icon: Picture, category: 'media-components' },
  { type: 'video', name: '视频', icon: VideoCamera, category: 'media-components' },
]

// 过滤后的组件列表
const filterComponents = (category: string) => {
  return allComponents.filter(
    (comp) =>
      comp.category === category &&
      (comp.name.toLowerCase().includes(searchKeyword.value.toLowerCase()) ||
        comp.type.toLowerCase().includes(searchKeyword.value.toLowerCase()))
  )
}

const filteredBasicCharts = computed(() => filterComponents('basic-charts'))
const filteredAdvancedCharts = computed(() => filterComponents('advanced-charts'))
const filteredInfoComponents = computed(() => filterComponents('info-components'))
const filteredDecorComponents = computed(() => filterComponents('decor-components'))
const filteredMediaComponents = computed(() => filterComponents('media-components'))

// 拖拽开始事件
const emit = defineEmits<{
  (e: 'drag-start', event: DragEvent, componentType: ComponentType): void
}>()

const handleDragStart = (event: DragEvent, comp: ComponentItem) => {
  // 设置拖拽效果
  if (event.dataTransfer) {
    event.dataTransfer.effectAllowed = 'copy'
    event.dataTransfer.setData('componentType', comp.type)
    event.dataTransfer.setData('text/plain', comp.type) // 备用
  }

  // 设置拖拽图像
  const dragImage = document.createElement('div')
  dragImage.className = 'drag-preview'
  dragImage.innerHTML = `<span>${comp.name}</span>`
  document.body.appendChild(dragImage)
  event.dataTransfer?.setDragImage(dragImage, 50, 20)
  setTimeout(() => dragImage.remove(), 0)

  console.log('Drag start:', comp.type)
  emit('drag-start', event, comp.type)
}
</script>

<style scoped lang="scss">
.component-lib {
  height: 100%;
  display: flex;
  flex-direction: column;
  background: rgba(255, 255, 255, 0.05);
  border-right: 1px solid rgba(255, 255, 255, 0.1);
}

.component-search {
  padding: 12px;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);

  :deep(.el-input__wrapper) {
    background: rgba(255, 255, 255, 0.1);
    box-shadow: none;
  }

  :deep(.el-input__inner) {
    color: #fff;

    &::placeholder {
      color: rgba(255, 255, 255, 0.5);
    }
  }
}

.component-collapse {
  flex: 1;
  overflow-y: auto;
  border: none;

  // 自定义滚动条
  &::-webkit-scrollbar {
    width: 6px;
  }

  &::-webkit-scrollbar-track {
    background: rgba(255, 255, 255, 0.05);
    border-radius: 3px;
  }

  &::-webkit-scrollbar-thumb {
    background: rgba(255, 255, 255, 0.2);
    border-radius: 3px;

    &:hover {
      background: rgba(255, 255, 255, 0.3);
    }
  }

  :deep(.el-collapse-item__header) {
    background: transparent;
    color: #fff;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  }

  :deep(.el-collapse-item__wrap) {
    background: transparent;
    border-bottom: none;
  }

  :deep(.el-collapse-item__content) {
    padding: 8px;
  }
}

.category-title {
  font-weight: 600;
  font-size: 13px;
}

.component-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 8px;
}

.component-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 12px 8px;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 6px;
  cursor: grab;
  transition: all 0.2s;

  &:hover {
    background: rgba(64, 158, 255, 0.3);
    border-color: rgba(64, 158, 255, 0.5);
  }

  &:active {
    cursor: grabbing;
  }
}

.component-icon {
  font-size: 24px;
  color: #409eff;
  margin-bottom: 4px;
}

.component-name {
  font-size: 12px;
  color: rgba(255, 255, 255, 0.8);
}
</style>

<style lang="scss">
// 全局拖拽预览样式
.drag-preview {
  position: fixed;
  top: -100px;
  left: -100px;
  padding: 8px 16px;
  background: rgba(64, 158, 255, 0.9);
  color: #fff;
  border-radius: 4px;
  font-size: 12px;
  pointer-events: none;
  z-index: 9999;
}
</style>