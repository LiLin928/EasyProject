<!-- 大屏预览页面 -->
<template>
  <div class="screen-preview" :class="{ 'screen-preview--fullscreen': isFullscreen }" :style="screenStyle">
    <!-- 工具栏 -->
    <div class="screen-toolbar">
      <el-button @click="handleClose">
        <el-icon><Close /></el-icon>
        关闭
      </el-button>
      <el-button @click="toggleFullscreen">
        <el-icon v-if="isFullscreen"><Close /></el-icon>
        <el-icon v-else><FullScreen /></el-icon>
        {{ isFullscreen ? t('screen.screen.exitFullscreen') : t('screen.screen.fullscreen') }}
      </el-button>
      <el-button @click="handleRefresh">
        <el-icon><Refresh /></el-icon>
        {{ t('screen.screen.refresh') }}
      </el-button>
    </div>

    <!-- 大屏内容 -->
    <div class="screen-content" ref="contentRef">
      <template v-for="component in screen?.components" :key="component.id">
        <!-- 标题组件 -->
        <div
          v-if="component.type === 'title'"
          class="screen-preview-component screen-title-component"
          :style="{ left: component.position.x + 'px', top: component.position.y + 'px', width: component.size.width + 'px', height: component.size.height + 'px' }"
        >
          {{ component.config.text }}
        </div>

        <!-- 数字卡片组件 -->
        <div
          v-else-if="component.type === 'number-card'"
          class="screen-preview-component screen-number-card"
          :style="{ left: component.position.x + 'px', top: component.position.y + 'px', width: component.size.width + 'px', height: component.size.height + 'px' }"
        >
          <div class="card-value" :style="{ color: component.config.color || '#409eff' }">
            {{ formatNumber(component.dataSource.data?.[0]?.value) }}
          </div>
          <div class="card-label">{{ component.dataSource.data?.[0]?.label || component.config.title }}</div>
        </div>

        <!-- 图表组件 -->
        <div
          v-else-if="isChartType(component.type)"
          class="screen-preview-component"
          :style="{ left: component.position.x + 'px', top: component.position.y + 'px', width: component.size.width + 'px', height: component.size.height + 'px' }"
        >
          <BaseChart :component="component" />
        </div>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'
import { FullScreen, Close, Refresh } from '@element-plus/icons-vue'
import { useLocale } from '@/composables/useLocale'
import { getScreenDetail } from '@/api/screen'
import { convertScreenToFrontend } from '@/utils/screenConverter'
import type { ScreenConfig } from '@/types'
import BaseChart from '../components/BaseChart.vue'

const route = useRoute()
const { t } = useLocale()

const screen = ref<ScreenConfig | null>(null)
const isFullscreen = ref(false)
const contentRef = ref<HTMLDivElement>()

const screenStyle = computed(() => ({
  background: screen.value?.style?.background || 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
  width: screen.value?.style?.width ? `${screen.value.style.width}px` : '1920px',
  height: screen.value?.style?.height ? `${screen.value.style.height}px` : '1080px',
}))

const isChartType = (type: string) => ['line-chart', 'bar-chart', 'pie-chart', 'radar-chart', 'funnel-chart', 'heatmap-chart'].includes(type)

const formatNumber = (num: number | undefined) => {
  if (num === undefined) return '0'
  if (num >= 10000) {
    return (num / 10000).toFixed(2) + '万'
  }
  return num.toLocaleString()
}

const loadScreen = async () => {
  const id = route.params.id as string
  if (!id) return

  try {
    const data = await getScreenDetail(id)
    // 转换后端数据格式为前端格式
    screen.value = convertScreenToFrontend(data as any)
  } catch (error) {
    // Error handled by interceptor
  }
}

const handleClose = () => {
  window.close()
}

const toggleFullscreen = () => {
  if (!document.fullscreenElement) {
    document.documentElement.requestFullscreen()
    isFullscreen.value = true
  } else {
    document.exitFullscreen()
    isFullscreen.value = false
  }
}

const handleRefresh = () => {
  loadScreen()
}

const handleFullscreenChange = () => {
  isFullscreen.value = Boolean(document.fullscreenElement)
}

onMounted(() => {
  loadScreen()
  document.addEventListener('fullscreenchange', handleFullscreenChange)
})

onUnmounted(() => {
  document.removeEventListener('fullscreenchange', handleFullscreenChange)
})
</script>

<style scoped lang="scss">
@use '@/assets/styles/screen.scss' as *;

.screen-preview {
  position: relative;
  margin: 0 auto;
  overflow: auto;
}

.screen-content {
  position: relative;
  width: 100%;
  height: 100%;
}

.screen-preview-component {
  position: absolute;
  box-sizing: border-box;
}

.screen-title-component {
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 36px;
}

.screen-number-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;

  .card-value {
    font-size: 32px;
    font-weight: 700;
  }

  .card-label {
    font-size: 14px;
    color: rgba(255, 255, 255, 0.7);
  }
}
</style>