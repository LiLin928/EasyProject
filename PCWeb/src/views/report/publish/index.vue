<!-- src/views/report/publish/index.vue -->
<template>
  <div class="report-publish">
    <div v-loading="loading" class="publish-content">
      <!-- Header -->
      <div class="publish-header">
        <h1 class="report-title">{{ report?.name }}</h1>
        <div class="report-meta">
          <span>分类: {{ report?.category }}</span>
          <span>更新时间: {{ report?.updateTime }}</span>
        </div>
      </div>

      <!-- Chart Section -->
      <div v-if="report?.showChart && chartData.length > 0" class="chart-section">
        <div class="chart-container" ref="chartRef"></div>
      </div>

      <!-- Data Summary -->
      <div v-if="summary" class="summary-section">
        <div class="summary-item">
          <span class="label">总计:</span>
          <span class="value">{{ formatNumber(summary.total) }}</span>
        </div>
        <div class="summary-item">
          <span class="label">数量:</span>
          <span class="value">{{ summary.count }}</span>
        </div>
        <div v-if="summary.avg" class="summary-item">
          <span class="label">平均值:</span>
          <span class="value">{{ formatNumber(summary.avg) }}</span>
        </div>
      </div>

      <!-- Data Table -->
      <div v-if="report?.showTable && tableData.length > 0" class="table-section">
        <el-table :data="tableData" border stripe>
          <el-table-column
            v-for="col in tableColumns"
            :key="col.field"
            :prop="col.field"
            :label="col.label"
            :width="col.width"
            :align="col.align"
          >
            <template #default="{ row }">
              {{ formatValue(row[col.field], col) }}
            </template>
          </el-table-column>
        </el-table>
      </div>

      <!-- Error State -->
      <div v-if="error" class="error-section">
        <el-result icon="error" title="加载失败" :sub-title="error">
          <template #extra>
            <el-button type="primary" @click="loadData">重新加载</el-button>
          </template>
        </el-result>
      </div>

      <!-- Empty State -->
      <div v-if="!loading && !error && !report" class="empty-section">
        <el-result icon="info" title="报表不存在" sub-title="该报表可能已被删除或未发布" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick } from 'vue'
import { useRoute } from 'vue-router'
import * as echarts from 'echarts'
import { getReportPublishData } from '@/api/report/reportApi'
import type { Report, ColumnConfig } from '@/types'

const route = useRoute()

const loading = ref(false)
const error = ref('')
const report = ref<Report | null>(null)
const chartData = ref<{ name: string; value: number }[]>([])
const tableData = ref<Record<string, any>[]>([])
const tableColumns = ref<ColumnConfig[]>([])
const summary = ref<{ total: number; count: number; avg: number } | null>(null)

const chartRef = ref<HTMLElement | null>(null)
let chartInstance: echarts.ECharts | null = null

onMounted(async () => {
  await loadData()
})

onUnmounted(() => {
  if (chartInstance) {
    chartInstance.dispose()
    chartInstance = null
  }
})

const loadData = async () => {
  const id = route.params.id as string
  if (!id) {
    error.value = '报表ID无效'
    return
  }

  loading.value = true
  error.value = ''

  try {
    const data = await getReportPublishData(id)
    report.value = data.report
    chartData.value = data.chartData || []
    tableData.value = data.tableData || []
    tableColumns.value = data.columnConfigs || data.detectedColumns || []
    summary.value = data.summary

    // Initialize chart
    if (report.value?.showChart && chartData.value.length > 0) {
      await nextTick()
      initChart(report.value.chartType, chartData.value)
    }
  } catch (err: any) {
    error.value = err.message || '加载报表数据失败'
  } finally {
    loading.value = false
  }
}

const initChart = (chartType: string, data: { name: string; value: number }[]) => {
  if (!chartRef.value) return

  if (chartInstance) {
    chartInstance.dispose()
  }

  chartInstance = echarts.init(chartRef.value)

  const option = getChartOption(chartType, data)
  chartInstance.setOption(option)

  // Resize on window resize
  window.addEventListener('resize', handleResize)
}

const handleResize = () => {
  chartInstance?.resize()
}

const getChartOption = (chartType: string, data: { name: string; value: number }[]): echarts.EChartsOption => {
  const baseOption = {
    tooltip: { trigger: 'item' },
    legend: { bottom: 0 },
  }

  switch (chartType) {
    case 'bar':
      return {
        ...baseOption,
        xAxis: { type: 'category', data: data.map(d => d.name) },
        yAxis: { type: 'value' },
        series: [{
          type: 'bar',
          data: data.map(d => d.value),
          itemStyle: { color: '#409eff' },
        }],
      }
    case 'line':
      return {
        ...baseOption,
        xAxis: { type: 'category', data: data.map(d => d.name) },
        yAxis: { type: 'value' },
        series: [{
          type: 'line',
          data: data.map(d => d.value),
          smooth: true,
          itemStyle: { color: '#409eff' },
        }],
      }
    case 'pie':
      return {
        ...baseOption,
        series: [{
          type: 'pie',
          radius: ['40%', '70%'],
          data: data,
          emphasis: {
            itemStyle: {
              shadowBlur: 10,
              shadowOffsetX: 0,
              shadowColor: 'rgba(0, 0, 0, 0.5)',
            },
          },
        }],
      }
    default:
      return {}
  }
}

const formatNumber = (value: number): string => {
  if (!value) return '0'
  return value.toLocaleString()
}

const formatValue = (value: any, col: ColumnConfig): string => {
  if (value === null || value === undefined) return '-'

  switch (col.format) {
    case 'number':
      return Number(value).toLocaleString()
    case 'money':
      return '¥' + Number(value).toLocaleString()
    case 'date':
      return new Date(value).toLocaleDateString()
    case 'percent':
      return (Number(value) * 100).toFixed(2) + '%'
    default:
      return String(value)
  }
}
</script>

<style scoped lang="scss">
.report-publish {
  min-height: 100vh;
  background: #f5f7fa;
  padding: 20px;

  .publish-content {
    max-width: 1200px;
    margin: 0 auto;
    background: white;
    border-radius: 8px;
    padding: 24px;

    .publish-header {
      margin-bottom: 20px;
      padding-bottom: 16px;
      border-bottom: 1px solid #ebeef5;

      .report-title {
        font-size: 24px;
        margin: 0 0 8px;
        color: #303133;
      }

      .report-meta {
        font-size: 14px;
        color: #909399;
        display: flex;
        gap: 16px;
      }
    }

    .chart-section {
      margin-bottom: 20px;

      .chart-container {
        height: 400px;
        border: 1px solid #ebeef5;
        border-radius: 4px;
      }
    }

    .summary-section {
      display: flex;
      flex-wrap: nowrap;
      justify-content: center;
      align-items: center;
      gap: 32px;
      padding: 16px 24px;
      background: #f5f7fa;
      border-radius: 4px;
      margin-bottom: 20px;

      .summary-item {
        display: inline-flex;
        align-items: center;
        gap: 4px;

        .label {
          color: #606266;
          font-size: 14px;
        }

        .value {
          font-weight: bold;
          color: #409eff;
          font-size: 18px;
        }
      }
    }

    .table-section {
      margin-top: 20px;
    }

    .error-section,
    .empty-section {
      padding: 40px;
    }
  }
}
</style>