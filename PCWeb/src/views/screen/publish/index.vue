<!-- 发布后的大屏查看页面 -->
<template>
  <div class="screen-publish" :style="screenStyle">
    <!-- 加载中 -->
    <div v-if="loading" class="screen-loading">
      <el-icon class="loading-icon"><Loading /></el-icon>
      <span>加载中...</span>
    </div>

    <!-- 错误提示 -->
    <div v-else-if="error" class="screen-error">
      <el-icon class="error-icon"><WarningFilled /></el-icon>
      <span>{{ error }}</span>
    </div>

    <!-- 大屏内容 -->
    <div v-else class="screen-content" ref="contentRef">
      <template v-for="component in screen?.components" :key="component.id">
        <!-- 标题组件 -->
        <div
          v-if="component.type === 'title'"
          class="screen-publish-component screen-title-component"
          :style="getComponentStyle(component)"
        >
          {{ component.config.text }}
        </div>

        <!-- 数字卡片组件 -->
        <div
          v-else-if="component.type === 'number-card'"
          class="screen-publish-component screen-number-card"
          :style="getComponentStyle(component)"
        >
          <div class="card-value" :style="{ color: component.config.color || '#409eff' }">
            {{ formatNumber(component.dataSource.data?.[0]?.value) }}
          </div>
          <div class="card-label">{{ component.dataSource.data?.[0]?.label || component.config.title }}</div>
        </div>

        <!-- 图表组件 -->
        <div
          v-else-if="isChartType(component.type)"
          class="screen-publish-component"
          :style="getComponentStyle(component)"
        >
          <BaseChart :component="component" />
        </div>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { Loading, WarningFilled } from '@element-plus/icons-vue'
import { getPublishedScreen } from '@/api/screen'
import type { ScreenConfig, ScreenComponent } from '@/types'
import BaseChart from '../components/BaseChart.vue'

const route = useRoute()

const loading = ref(true)
const error = ref('')
const screen = ref<ScreenConfig | null>(null)
const contentRef = ref<HTMLDivElement>()

const screenStyle = computed(() => ({
  background: screen.value?.style?.background || 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
  width: '100vw',
  height: '100vh',
  overflow: 'hidden',
}))

const getComponentStyle = (component: ScreenComponent) => ({
  left: component.position.x + 'px',
  top: component.position.y + 'px',
  width: component.size.width + 'px',
  height: component.size.height + 'px',
})

const isChartType = (type: string) => ['line-chart', 'bar-chart', 'pie-chart', 'radar-chart', 'funnel-chart', 'heatmap-chart'].includes(type)

const formatNumber = (num: number | undefined) => {
  if (num === undefined) return '0'
  if (num >= 10000) {
    return (num / 10000).toFixed(2) + '万'
  }
  return num.toLocaleString()
}

const loadScreen = async () => {
  const publishId = route.params.publishId as string
  if (!publishId) {
    error.value = '缺少发布ID'
    loading.value = false
    return
  }

  try {
    screen.value = await getPublishedScreen(publishId)
  } catch (err: any) {
    error.value = err.message || '加载失败'
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadScreen()
})
</script>

<style scoped lang="scss">
.screen-publish {
  position: relative;
  margin: 0;
  padding: 0;
}

.screen-loading,
.screen-error {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #fff;
  font-size: 16px;
  gap: 12px;
}

.loading-icon,
.error-icon {
  font-size: 48px;
}

.error-icon {
  color: #f56c6c;
}

.screen-content {
  position: relative;
  width: 100%;
  height: 100%;
}

.screen-publish-component {
  position: absolute;
  box-sizing: border-box;
}

.screen-title-component {
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 36px;
  color: #fff;
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