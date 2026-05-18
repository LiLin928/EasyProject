<!-- 文件: PCWeb/src/views/desktop/components/renderers/ChartWidget.vue -->

<template>
  <div class="chart-widget">
    <div ref="chartRef" class="chart-container" :style="{ height: chartHeight }"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue'
import * as echarts from 'echarts'
import type { UserWidgetConfigDto, WidgetDataResponse, ChartDataConfig } from '@/types/desktopWidget'

// Props
const props = defineProps<{
  widget: UserWidgetConfigDto
  data?: WidgetDataResponse
}>()

// 图表容器引用
const chartRef = ref<HTMLElement | null>(null)

// 图表实例
let chartInstance: echarts.ECharts | null = null

// 图表高度
const chartHeight = computed(() => {
  const height = props.widget.defaultHeight || 200
  return `${height}px`
})

// 图表数据
const chartData = computed<ChartDataConfig | undefined>(() => {
  return props.data?.chartData
})

// 初始化图表
function initChart() {
  if (!chartRef.value) return

  // 销毁旧实例
  if (chartInstance) {
    chartInstance.dispose()
  }

  // 创建新实例
  chartInstance = echarts.init(chartRef.value)

  // 配置图表选项
  const option = getChartOption()

  if (option) {
    chartInstance.setOption(option)
  }
}

// 根据图表类型获取配置
function getChartOption(): echarts.EChartsOption | null {
  if (!chartData.value) return null

  const config = chartData.value

  switch (config.type) {
    case 'bar':
      return {
        title: {
          text: config.title,
          left: 'center',
          textStyle: {
            fontSize: 14,
            fontWeight: 'normal',
          },
        },
        tooltip: {
          trigger: 'axis',
        },
        xAxis: {
          type: 'category',
          data: config.xAxis || [],
          axisLabel: {
            fontSize: 10,
          },
        },
        yAxis: {
          type: 'value',
          axisLabel: {
            fontSize: 10,
          },
        },
        series: [
          {
            type: 'bar',
            data: config.yAxis || [],
            itemStyle: {
              color: '#409eff',
            },
          },
        ],
        grid: {
          top: 40,
          right: 20,
          bottom: 30,
          left: 40,
        },
      }

    case 'line':
      return {
        title: {
          text: config.title,
          left: 'center',
          textStyle: {
            fontSize: 14,
            fontWeight: 'normal',
          },
        },
        tooltip: {
          trigger: 'axis',
        },
        xAxis: {
          type: 'category',
          data: config.xAxis || [],
          axisLabel: {
            fontSize: 10,
          },
        },
        yAxis: {
          type: 'value',
          axisLabel: {
            fontSize: 10,
          },
        },
        series: [
          {
            type: 'line',
            data: config.yAxis || [],
            smooth: true,
            itemStyle: {
              color: '#409eff',
            },
            lineStyle: {
              width: 2,
            },
            areaStyle: {
              color: 'rgba(64, 158, 255, 0.2)',
            },
          },
        ],
        grid: {
          top: 40,
          right: 20,
          bottom: 30,
          left: 40,
        },
      }

    case 'pie':
      return {
        title: {
          text: config.title,
          left: 'center',
          textStyle: {
            fontSize: 14,
            fontWeight: 'normal',
          },
        },
        tooltip: {
          trigger: 'item',
          formatter: '{b}: {c} ({d}%)',
        },
        legend: {
          bottom: 10,
          left: 'center',
          itemWidth: 10,
          itemHeight: 10,
          textStyle: {
            fontSize: 10,
          },
        },
        series: [
          {
            type: 'pie',
            radius: ['40%', '70%'],
            center: ['50%', '45%'],
            avoidLabelOverlap: false,
            itemStyle: {
              borderRadius: 4,
              borderColor: '#fff',
              borderWidth: 2,
            },
            label: {
              show: false,
            },
            data: config.pieData || [],
            color: ['#409eff', '#67c23a', '#e6a23c', '#f56c6c', '#909399'],
          },
        ],
      }

    default:
      return null
  }
}

// 处理窗口大小变化
function handleResize() {
  if (chartInstance) {
    chartInstance.resize()
  }
}

// 监听数据变化
watch(chartData, () => {
  nextTick(() => {
    initChart()
  })
})

// 生命周期
onMounted(() => {
  initChart()
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  if (chartInstance) {
    chartInstance.dispose()
    chartInstance = null
  }
  window.removeEventListener('resize', handleResize)
})
</script>

<style scoped lang="scss">
.chart-widget {
  .chart-container {
    width: 100%;
    min-height: 200px;
  }
}
</style>