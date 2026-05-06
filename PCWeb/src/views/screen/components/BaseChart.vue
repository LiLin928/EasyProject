<!-- ECharts 基础图表组件 -->
<template>
  <div ref="chartRef" class="chart-container"></div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, nextTick, computed } from 'vue'
import * as echarts from 'echarts'
import type { ScreenComponent, ComponentStyleConfig, DataBindingConfig } from '@/types'

const props = defineProps<{
  component: ScreenComponent
  designMode?: boolean
}>()

const chartRef = ref<HTMLDivElement>()
let chartInstance: echarts.ECharts | null = null

// 默认颜色
const defaultColors = ['#5470c6', '#91cc75', '#fac858', '#ee6666', '#73c0de', '#3ba272', '#fc8452', '#9a60b4']

// 获取颜色
const getColors = computed(() => {
  return props.component.style?.colors || defaultColors
})

// 获取样式配置
const getStyleConfig = computed(() => {
  return props.component.style
})

// 获取数据绑定配置
const getDataBinding = computed(() => {
  return props.component.dataBinding
})

// 格式化数值
const formatValue = (value: number): string => {
  const formatter = getDataBinding.value?.formatter?.number
  if (!formatter) return String(value)

  let result = value
  const { decimals = 2, prefix = '', suffix = '', unit = 'none' } = formatter

  // 单位换算
  switch (unit) {
    case 'thousand':
      result = value / 1000
      break
    case 'million':
      result = value / 1000000
      break
    case 'billion':
      result = value / 100000000
      break
    case 'percent':
      result = value * 100
      break
  }

  return `${prefix}${result.toFixed(decimals)}${suffix}`
}

// 根据系列字段分组数据
const groupDataBySeries = (data: any[], seriesField?: string) => {
  if (!seriesField || !data.length) {
    return { seriesNames: [], groupedData: {} }
  }

  const seriesNames = [...new Set(data.map(d => d[seriesField]))]
  const groupedData: Record<string, any[]> = {}

  seriesNames.forEach(name => {
    groupedData[name] = data.filter(d => d[seriesField] === name)
  })

  return { seriesNames, groupedData }
}

// 生成系列配置
const generateSeries = (
  type: 'line' | 'bar',
  data: any[],
  config: Record<string, any>,
  styleConfig?: ComponentStyleConfig,
  dataBinding?: DataBindingConfig
) => {
  const colors = styleConfig?.colors || defaultColors
  const seriesField = dataBinding?.fieldMapping?.series
  const valueFields = dataBinding?.fieldMapping?.value
  const dimensionField = dataBinding?.fieldMapping?.dimension || config.xField || 'x'

  // 获取维度数据
  const categories = [...new Set(data.map(d => d[dimensionField]))]

  // 如果有系列字段，按系列分组
  if (seriesField) {
    const { seriesNames, groupedData } = groupDataBySeries(data, seriesField)

    return seriesNames.map((name, index) => {
      const seriesData = groupedData[name]
      const valueField = Array.isArray(valueFields) ? (valueFields[0] || config.yField || 'y') : (valueFields || config.yField || 'y')

      // 按维度排序
      const sortedData = categories.map(cat => {
        const item = seriesData.find(d => d[dimensionField] === cat)
        return item ? item[valueField] : 0
      })

      const color = colors[index % colors.length]

      if (type === 'line') {
        return {
          name,
          type: 'line',
          data: sortedData,
          smooth: config.smooth !== false,
          lineStyle: { color, width: 2 },
          areaStyle: {
            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
              { offset: 0, color: `${color}80` },
              { offset: 1, color: `${color}20` },
            ]),
          },
          itemStyle: { color },
        }
      } else {
        return {
          name,
          type: 'bar',
          data: sortedData,
          itemStyle: {
            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
              { offset: 0, color },
              { offset: 1, color: `${color}60` },
            ]),
            borderRadius: [4, 4, 0, 0],
          },
        }
      }
    })
  }

  // 如果有多个值字段
  if (Array.isArray(valueFields) && valueFields.length > 1) {
    return valueFields.map((field, index) => {
      const seriesData = data.map(d => d[field])
      const color = colors[index % colors.length]

      if (type === 'line') {
        return {
          name: field,
          type: 'line',
          data: seriesData,
          smooth: config.smooth !== false,
          lineStyle: { color, width: 2 },
          areaStyle: {
            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
              { offset: 0, color: `${color}80` },
              { offset: 1, color: `${color}20` },
            ]),
          },
          itemStyle: { color },
        }
      } else {
        return {
          name: field,
          type: 'bar',
          data: seriesData,
          itemStyle: {
            color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
              { offset: 0, color },
              { offset: 1, color: `${color}60` },
            ]),
            borderRadius: [4, 4, 0, 0],
          },
        }
      }
    })
  }

  // 单系列
  const valueField = Array.isArray(valueFields) ? (valueFields[0] || 'y') : (valueFields || config.yField || 'y')
  const color = colors[0]

  if (type === 'line') {
    return [{
      type: 'line',
      data: data.map(d => d[valueField]),
      smooth: config.smooth !== false,
      lineStyle: { color, width: 2 },
      areaStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
          { offset: 0, color: `${color}80` },
          { offset: 1, color: `${color}20` },
        ]),
      },
      itemStyle: { color },
    }]
  } else {
    return [{
      type: 'bar',
      data: data.map(d => d[valueField]),
      itemStyle: {
        color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
          { offset: 0, color },
          { offset: 1, color: `${color}60` },
        ]),
        borderRadius: [4, 4, 0, 0],
      },
    }]
  }
}

// 生成坐标轴配置
const generateAxisConfig = (axisConfig: any, isXAxis: boolean) => {
  const styleConfig = getStyleConfig.value
  const defaultAxisConfig = isXAxis ? styleConfig?.axis?.xAxis : styleConfig?.axis?.yAxis

  return {
    show: defaultAxisConfig?.show !== false,
    name: defaultAxisConfig?.name || '',
    nameTextStyle: {
      color: defaultAxisConfig?.nameColor || '#ffffff',
      fontSize: defaultAxisConfig?.nameFontSize || 12,
    },
    axisLine: {
      lineStyle: {
        color: defaultAxisConfig?.lineColor || 'rgba(255,255,255,0.3)',
      },
    },
    axisLabel: {
      color: defaultAxisConfig?.labelColor || 'rgba(255,255,255,0.7)',
      fontSize: defaultAxisConfig?.labelFontSize || 12,
    },
    splitLine: isXAxis ? undefined : {
      lineStyle: {
        color: defaultAxisConfig?.splitLineColor || 'rgba(255,255,255,0.1)',
      },
    },
    min: isXAxis ? undefined : defaultAxisConfig?.min,
    max: isXAxis ? undefined : defaultAxisConfig?.max,
    ...axisConfig,
  }
}

const initChart = () => {
  if (!chartRef.value) return

  if (chartInstance) {
    chartInstance.dispose()
  }

  chartInstance = echarts.init(chartRef.value)
  const option = generateOption()
  chartInstance.setOption(option)
}

const generateOption = (): echarts.EChartsOption => {
  const { type, dataSource, config, style, dataBinding } = props.component
  const data = dataSource.data || []
  const styleConfig = style
  const dataBindingConfig = dataBinding

  // 标题配置
  const titleConfig = {
    show: styleConfig?.title?.show !== false,
    text: styleConfig?.title?.text || config.title || '',
    textStyle: {
      color: styleConfig?.title?.color || '#ffffff',
      fontSize: styleConfig?.title?.fontSize || 16,
      fontWeight: styleConfig?.title?.fontWeight || 'normal',
    },
    left: styleConfig?.title?.align || 'center',
    [styleConfig?.title?.position || 'top']: styleConfig?.title?.position === 'bottom' ? '5%' : undefined,
  }

  // 图例配置
  const legendConfig = {
    show: styleConfig?.legend?.show !== false,
    orient: 'horizontal' as const,
    [styleConfig?.legend?.position || 'top']: styleConfig?.legend?.position === 'bottom' ? '5%' : '15%',
    textStyle: {
      color: styleConfig?.legend?.color || 'rgba(255,255,255,0.7)',
      fontSize: styleConfig?.legend?.fontSize || 12,
    },
  }

  // 标签配置
  const labelConfig = {
    show: styleConfig?.label?.show !== false,
    position: styleConfig?.label?.position || 'top',
    color: styleConfig?.label?.color || '#ffffff',
    fontSize: styleConfig?.label?.fontSize || 12,
    formatter: styleConfig?.label?.formatter ? (params: any) => {
      const template = styleConfig.label!.formatter!
      return template.replace('{value}', formatValue(params.value))
    } : undefined,
  }

  // 维度字段
  const dimensionField = dataBindingConfig?.fieldMapping?.dimension || config.xField || 'x'

  switch (type) {
    case 'line-chart': {
      const categories = [...new Set(data.map((d: any) => d[dimensionField]))]
      const series = generateSeries('line', data, config, styleConfig, dataBindingConfig)

      return {
        title: titleConfig,
        tooltip: { trigger: 'axis' },
        legend: legendConfig,
        grid: { left: '10%', right: '10%', top: '20%', bottom: '15%' },
        xAxis: generateAxisConfig({
          type: 'category',
          data: categories,
        }, true),
        yAxis: generateAxisConfig({
          type: 'value',
        }, false),
        series,
      }
    }

    case 'bar-chart': {
      const categories = [...new Set(data.map((d: any) => d[dimensionField]))]
      const series = generateSeries('bar', data, config, styleConfig, dataBindingConfig)

      return {
        title: titleConfig,
        tooltip: { trigger: 'axis' },
        legend: legendConfig,
        grid: { left: '10%', right: '10%', top: '20%', bottom: '15%' },
        xAxis: generateAxisConfig({
          type: 'category',
          data: categories,
        }, true),
        yAxis: generateAxisConfig({
          type: 'value',
        }, false),
        series,
      }
    }

    case 'pie-chart': {
      const nameField = dataBindingConfig?.fieldMapping?.dimension || config.nameField || 'name'
      const valueField = Array.isArray(dataBindingConfig?.fieldMapping?.value)
        ? dataBindingConfig.fieldMapping.value[0]
        : (dataBindingConfig?.fieldMapping?.value || config.valueField || 'value')

      return {
        title: titleConfig,
        tooltip: { trigger: 'item', formatter: '{b}: {c} ({d}%)' },
        legend: legendConfig,
        series: [{
          type: 'pie',
          radius: ['40%', '70%'],
          center: ['50%', '50%'],
          data: data.map((d: any, index: number) => ({
            name: d[nameField],
            value: d[valueField],
            itemStyle: {
              color: getColors.value[index % getColors.value.length],
            },
          })),
          itemStyle: {
            borderRadius: 6,
            borderColor: 'rgba(255,255,255,0.3)',
          },
          label: labelConfig,
        }],
      }
    }

    case 'radar-chart': {
      const dimensionField = dataBindingConfig?.fieldMapping?.dimension || config.dimensionField || 'dimension'
      const valueField = Array.isArray(dataBindingConfig?.fieldMapping?.value)
        ? dataBindingConfig.fieldMapping.value[0]
        : (dataBindingConfig?.fieldMapping?.value || config.valueField || 'value')

      return {
        title: titleConfig,
        tooltip: { trigger: 'item' },
        radar: {
          indicator: data.map((d: any) => ({
            name: d[dimensionField],
            max: config.maxValues?.[d[dimensionField]] || 100,
          })),
          axisLine: { lineStyle: { color: 'rgba(255,255,255,0.3)' } },
          splitLine: { lineStyle: { color: 'rgba(255,255,255,0.1)' } },
          splitArea: { areaStyle: { color: ['rgba(255,255,255,0.05)', 'rgba(255,255,255,0.1)'] } },
        },
        series: [{
          type: 'radar',
          data: [{
            value: data.map((d: any) => d[valueField]),
            name: config.title || '',
            areaStyle: {
              color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                { offset: 0, color: `${getColors.value[0]}99` },
                { offset: 1, color: `${getColors.value[0]}33` },
              ]),
            },
            lineStyle: { color: getColors.value[0], width: 2 },
            itemStyle: { color: getColors.value[0] },
          }],
        }],
      }
    }

    case 'funnel-chart': {
      const nameField = dataBindingConfig?.fieldMapping?.dimension || config.nameField || 'name'
      const valueField = Array.isArray(dataBindingConfig?.fieldMapping?.value)
        ? dataBindingConfig.fieldMapping.value[0]
        : (dataBindingConfig?.fieldMapping?.value || config.valueField || 'value')

      return {
        title: titleConfig,
        tooltip: { trigger: 'item', formatter: '{b}: {c} ({d}%)' },
        series: [{
          type: 'funnel',
          left: '10%',
          top: '15%',
          bottom: '10%',
          width: '80%',
          min: 0,
          max: 100,
          minSize: '0%',
          maxSize: '100%',
          sort: config.sort || 'descending',
          gap: 2,
          label: labelConfig,
          itemStyle: { borderWidth: 0 },
          emphasis: { label: { fontSize: 14 } },
          data: data.map((d: any, index: number) => ({
            name: d[nameField],
            value: d[valueField],
            itemStyle: {
              color: new echarts.graphic.LinearGradient(0, 0, 1, 0, [
                { offset: 0, color: `${getColors.value[index % getColors.value.length]}cc` },
                { offset: 1, color: `${getColors.value[index % getColors.value.length]}66` },
              ]),
            },
          })),
        }],
      }
    }

    case 'heatmap-chart': {
      const xField = dataBindingConfig?.fieldMapping?.dimension || config.xField || 'x'
      const yField = config.yField || 'y'
      const valueField = Array.isArray(dataBindingConfig?.fieldMapping?.value)
        ? dataBindingConfig.fieldMapping.value[0]
        : (dataBindingConfig?.fieldMapping?.value || config.valueField || 'value')

      const xData = [...new Set(data.map((d: any) => d[xField]))]
      const yData = [...new Set(data.map((d: any) => d[yField]))]

      const heatmapData: [number, number, number][] = data.map((d: any) => {
        const xIndex = xData.indexOf(d[xField])
        const yIndex = yData.indexOf(d[yField])
        return [xIndex, yIndex, d[valueField]]
      })

      return {
        title: titleConfig,
        tooltip: {
          position: 'top',
          formatter: (params: any) => {
            const [xIndex, yIndex, value] = params.data
            return `${xData[xIndex]} - ${yData[yIndex]}: ${formatValue(value)}`
          },
        },
        grid: { left: '10%', right: '10%', top: '15%', bottom: '15%' },
        xAxis: generateAxisConfig({
          type: 'category',
          data: xData,
          splitArea: { show: true },
        }, true),
        yAxis: generateAxisConfig({
          type: 'category',
          data: yData,
          splitArea: { show: true },
        }, false),
        visualMap: {
          min: Math.min(...data.map((d: any) => d[valueField])),
          max: Math.max(...data.map((d: any) => d[valueField])),
          calculable: true,
          orient: 'horizontal',
          left: 'center',
          bottom: '0%',
          inRange: { color: getColors.value.slice(0, 4) },
          textStyle: { color: '#fff' },
        },
        series: [{
          type: 'heatmap',
          data: heatmapData,
          label: { show: false },
          emphasis: {
            itemStyle: { shadowBlur: 10, shadowColor: 'rgba(0, 0, 0, 0.5)' },
          },
        }],
      }
    }

    default:
      return {}
  }
}

const resizeChart = () => {
  chartInstance?.resize()
}

onMounted(() => {
  nextTick(() => {
    initChart()
  })
  window.addEventListener('resize', resizeChart)
})

onUnmounted(() => {
  chartInstance?.dispose()
  chartInstance = null
  window.removeEventListener('resize', resizeChart)
})

watch(() => props.component, () => {
  nextTick(() => {
    initChart()
  })
}, { deep: true })
</script>

<style scoped lang="scss">
.chart-container {
  width: 100%;
  height: 100%;
}
</style>