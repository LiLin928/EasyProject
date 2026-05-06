<!-- src/views/report/ReportDetail.vue -->
<template>
  <div class="report-detail">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span class="report-title">{{ report?.name }}</span>
          <div class="header-right">
            <el-button type="primary" @click="handleEdit">
              <el-icon><Edit /></el-icon>
              {{ t('report.detail.edit') }}
            </el-button>
            <el-button type="success" @click="handleExport">
              <el-icon><Download /></el-icon>
              {{ t('report.detail.export') }}
            </el-button>
            <el-button @click="goBack">
              {{ t('report.detail.backToList') }}
            </el-button>
          </div>
        </div>
      </template>

      <div v-loading="loading" class="detail-content">
        <!-- Breadcrumb Navigation -->
        <BreadcrumbNav
          :path="drilldownPath"
          @navigate="handleBreadcrumbNavigate"
        />

        <!-- Metadata -->
        <div class="metadata">
          <span>{{ t('report.list.category') }}: {{ report?.category }}</span>
          <span>{{ t('report.list.creator') }}: {{ report?.creator }}</span>
          <span>{{ t('report.list.updateTime') }}: {{ report?.updateTime }}</span>
        </div>

        <!-- Main content area -->
        <div v-if="report?.showChart" class="main-content">
          <!-- Chart area -->
          <div class="chart-section">
            <div class="chart-container" ref="chartRef"></div>
          </div>

          <!-- Summary sidebar -->
          <div class="summary-section">
            <div class="summary-title">{{ t('report.detail.dataSummary') }}</div>
            <div class="summary-item">
              <span class="label">{{ t('report.detail.total') }}:</span>
              <span class="value">{{ summary?.total || 0 }}</span>
            </div>
            <div class="summary-item">
              <span class="label">{{ t('report.detail.count') }}:</span>
              <span class="value">{{ summary?.count || 0 }}</span>
            </div>
            <div class="summary-item">
              <span class="label">{{ t('report.detail.avg') }}:</span>
              <span class="value">{{ summary?.avg || 0 }}</span>
            </div>
          </div>
        </div>

        <!-- Data table -->
        <div v-if="report?.showTable" class="table-section">
          <div class="table-title">{{ t('report.detail.dataTable') }}</div>
          <BaseTable
            :data="tableData"
            :columns="dynamicColumns.length > 0 ? dynamicColumns : tableColumns"
            :loading="tableLoading"
            :total="tableTotal"
            :page-index="tablePageIndex"
            :page-size="tablePageSize"
            @update:page-index="tablePageIndex = $event"
            @update:page-size="tablePageSize = $event"
            @page-change="loadTableData"
          >
            <!-- Dynamic slots for drilldown columns -->
            <template v-for="col in (dynamicColumns.length > 0 ? dynamicColumns : tableColumns)" :key="col.prop" #[col.prop]="{ row }">
              <span
                v-if="isColumnDrilldownable(col.prop)"
                class="drilldown-link"
                @click="handleDrilldownClick(col.prop, row)"
              >
                {{ formatCellValue(row[col.prop], col.prop) }}
              </span>
              <span v-else>
                {{ formatCellValue(row[col.prop], col.prop) }}
              </span>
            </template>
          </BaseTable>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Edit, Download } from '@element-plus/icons-vue'
import * as echarts from 'echarts'
import { getReportDetail, previewReport } from '@/api/report/reportApi'
import { useLocale } from '@/composables/useLocale'
import { useDrilldown } from '@/composables/useDrilldown'
import { exportToExcel, exportChartAndDataToExcel } from '@/utils/export'
import type { Report, PreviewResult, ColumnConfig, DrilldownPath } from '@/types'
import type { TableColumn } from '@/components/DraggableTable/index.vue'
import BaseTable from '@/components/BaseTable/index.vue'
import BreadcrumbNav from './components/BreadcrumbNav.vue'

const router = useRouter()
const route = useRoute()
const { t } = useLocale()

const loading = ref(false)
const tableLoading = ref(false)
const report = ref<Report | null>(null)
const previewData = ref<PreviewResult | null>(null)
const summary = ref<{ total: number; count: number; avg: number } | null>(null)

const chartRef = ref<HTMLElement | null>(null)
let chartInstance: echarts.ECharts | null = null

// Table data
const tableData = ref<Record<string, any>[]>([])
const tableTotal = ref(0)
const tablePageIndex = ref(1)
const tablePageSize = ref(10)
const tableColumns = ref<TableColumn[]>([
  { prop: 'date', label: '日期', width: 120 },
  { prop: 'amount', label: '金额', width: 120 },
  { prop: 'orders', label: '订单数', width: 100 },
  { prop: 'avg', label: '客单价', width: 100 },
])

// Drilldown functionality
const {
  drilldownPath,
  initDrilldownPath,
  executeDrilldown,
  goBack: drilldownGoBack,
  isDrilldownable,
} = useDrilldown()

// Column configuration
const columnConfigs = ref<ColumnConfig[]>([])
const dynamicColumns = ref<TableColumn[]>([])

onMounted(async () => {
  await loadReport()
})

onUnmounted(() => {
  if (chartInstance) {
    chartInstance.dispose()
    chartInstance = null
  }
})

const loadReport = async () => {
  loading.value = true
  try {
    const id = route.params.id as string
    const data = await getReportDetail(id)
    report.value = data

    // Initialize drilldown path
    initDrilldownPath(data.id, data.name)

    // Set column configuration
    if (data.columnConfigs && data.columnConfigs.length > 0) {
      columnConfigs.value = data.columnConfigs
      dynamicColumns.value = data.columnConfigs.map(col => ({
        prop: col.field,
        label: col.label,
        width: col.width,
        align: col.align,
      }))
    } else {
      // Use default columns
      dynamicColumns.value = []
    }

    // Get drilldown parameters
    const drillParams = { ...route.query }

    // Load preview data
    const preview = await previewReport({
      datasourceId: data.datasourceId,
      sqlQuery: data.sqlQuery,
      chartType: data.chartType,
      xAxisField: data.xAxisField,
      yAxisField: data.yAxisField,
      aggregation: data.aggregation,
    })
    previewData.value = preview
    summary.value = preview.summary

    // Initialize chart if showChart is true
    if (data.showChart) {
      await nextTick()
      initChart(data.chartType, preview.chartData)
    }

    // Load table data if showTable is true
    if (data.showTable) {
      tableData.value = preview.tableData
      tableTotal.value = preview.tableData.length
    }
  } catch (error) {
    ElMessage.error('加载报表失败')
  } finally {
    loading.value = false
  }
}

const initChart = (chartType: string, chartData: { name: string; value: number }[]) => {
  if (!chartRef.value) return

  if (chartInstance) {
    chartInstance.dispose()
  }

  chartInstance = echarts.init(chartRef.value)

  const option: echarts.EChartsOption = getChartOption(chartType, chartData)
  chartInstance.setOption(option)
}

const getChartOption = (chartType: string, chartData: { name: string; value: number }[]): echarts.EChartsOption => {
  const baseOption = {
    tooltip: { trigger: 'item' },
    legend: { bottom: 0 },
  }

  switch (chartType) {
    case 'bar':
      return {
        ...baseOption,
        xAxis: { type: 'category', data: chartData.map(d => d.name) },
        yAxis: { type: 'value' },
        series: [{
          type: 'bar',
          data: chartData.map(d => d.value),
          itemStyle: { color: '#409eff' },
        }],
      }
    case 'line':
      return {
        ...baseOption,
        xAxis: { type: 'category', data: chartData.map(d => d.name) },
        yAxis: { type: 'value' },
        series: [{
          type: 'line',
          data: chartData.map(d => d.value),
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
          data: chartData,
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

const loadTableData = () => {
  if (!previewData.value) return
  const start = (tablePageIndex.value - 1) * tablePageSize.value
  const end = start + tablePageSize.value
  tableData.value = previewData.value.tableData.slice(start, end)
}

// Check if column is drilldownable
const isColumnDrilldownable = (field: string): boolean => {
  const config = columnConfigs.value.find(c => c.field === field)
  return config ? isDrilldownable(config) : false
}

// Handle drilldown click
const handleDrilldownClick = async (field: string, row: Record<string, any>) => {
  const config = columnConfigs.value.find(c => c.field === field)
  if (!config?.drilldown) return

  // Get target report name
  const targetReport = await getReportDetail(config.drilldown.targetReportId)

  executeDrilldown(
    config.drilldown,
    row,
    row[field],
    targetReport.name
  )
}

// Breadcrumb navigation
const handleBreadcrumbNavigate = (index: number) => {
  drilldownGoBack(index)
}

// Format cell value
const formatCellValue = (value: any, field: string): string => {
  if (value === null || value === undefined) return '-'

  const config = columnConfigs.value.find(c => c.field === field)
  if (!config?.format) return String(value)

  switch (config.format) {
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

const goBack = () => {
  router.push('/report/list')
}

const handleEdit = () => {
  router.push(`/report/edit/${report.value?.id}`)
}

const handleExport = async () => {
  if (!previewData.value?.tableData?.length) {
    ElMessage.warning('暂无数据可导出')
    return
  }

  try {
    // 获取图表图片（base64）
    let chartImage: string | null = null
    if (chartInstance && report.value?.showChart) {
      // getDataURL 返回带前缀的 base64，需要去掉前缀
      const dataUrl = chartInstance.getDataURL({
        type: 'png',
        pixelRatio: 2,
        backgroundColor: '#fff',
      })
      // 去掉 "data:image/png;base64," 前缀
      chartImage = dataUrl.replace(/^data:image\/png;base64,/, '')
    }

    // 确定要导出的列
    const exportColumns = dynamicColumns.value.length > 0
      ? dynamicColumns.value
      : tableColumns.value

    await exportChartAndDataToExcel({
      chartImage,
      chartTitle: `${report.value?.name || '报表'} - 图表`,
      columns: exportColumns,
      data: previewData.value.tableData,
      summary: summary.value,
      fileName: `${report.value?.name || '报表'}_${new Date().toLocaleDateString().replace(/\//g, '-')}`,
      sheetName: '报表数据',
    })

    ElMessage.success('导出成功')
  } catch (error) {
    console.error('导出失败:', error)
    ElMessage.error('导出失败')
  }
}
</script>

<style scoped lang="scss">
.report-detail {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .header-left {
      display: flex;
      align-items: center;
      gap: 16px;

      .report-title {
        font-size: 16px;
        font-weight: bold;
      }
    }

    .header-right {
      display: flex;
      gap: 8px;
    }
  }

  .detail-content {
    .metadata {
      margin-bottom: 16px;
      padding: 12px;
      background: #f5f7fa;
      border-radius: 4px;
      display: flex;
      gap: 24px;
      color: #606266;
      font-size: 14px;
    }

    .main-content {
      display: flex;
      gap: 16px;
      margin-bottom: 20px;

      .chart-section {
        flex: 1;

        .chart-container {
          height: 300px;
          border: 1px solid #ebeef5;
          border-radius: 4px;
        }
      }

      .summary-section {
        width: 200px;
        padding: 16px;
        border: 1px solid #ebeef5;
        border-radius: 4px;

        .summary-title {
          font-weight: bold;
          margin-bottom: 12px;
          border-bottom: 1px solid #ebeef5;
          padding-bottom: 8px;
        }

        .summary-item {
          display: flex;
          justify-content: space-between;
          margin-bottom: 8px;

          .label {
            color: #606266;
          }

          .value {
            font-weight: bold;
            color: #409eff;
          }
        }
      }
    }

    .table-section {
      .table-title {
        font-weight: bold;
        margin-bottom: 12px;
      }

      .drilldown-link {
        color: #409eff;
        text-decoration: underline;
        cursor: pointer;

        &:hover {
          color: #66b1ff;
        }
      }
    }
  }
}
</style>