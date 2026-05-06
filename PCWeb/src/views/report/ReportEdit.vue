<!-- src/views/report/ReportEdit.vue -->
<template>
  <div class="report-edit">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ isEdit ? t('report.edit.title') : t('report.edit.createTitle') }}</span>
          <el-button @click="goBack">
            {{ t('report.detail.backToList') }}
          </el-button>
        </div>
      </template>

      <div v-loading="loading" class="edit-content">
        <div class="form-section">
          <el-form
            ref="formRef"
            :model="formData"
            :rules="formRules"
            label-width="120px"
          >
            <!-- Basic Information -->
            <div class="section-title">{{ t('report.edit.basicInfo') }}</div>
            <el-form-item :label="t('report.edit.name')" prop="name">
              <el-input
                v-model="formData.name"
                :placeholder="t('report.edit.namePlaceholder')"
              />
            </el-form-item>
            <el-form-item :label="t('report.edit.category')" prop="category">
              <el-select
                v-model="formData.category"
                :placeholder="t('report.edit.categoryPlaceholder')"
                style="width: 100%"
              >
                <el-option
                  v-for="cat in categories"
                  :key="cat.id"
                  :label="cat.name"
                  :value="cat.name"
                />
              </el-select>
            </el-form-item>

            <!-- Data Configuration -->
            <div class="section-title">{{ t('report.edit.dataConfig') }}</div>
            <el-form-item :label="t('report.edit.dataSource')" prop="datasourceId">
              <el-select
                v-model="formData.datasourceId"
                :placeholder="t('report.edit.dataSourcePlaceholder')"
                style="width: 100%"
                @change="handleDataSourceChange"
              >
                <el-option
                  v-for="ds in dataSources"
                  :key="ds.id"
                  :label="ds.name"
                  :value="ds.id"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="t('report.edit.sqlQuery')" prop="sqlQuery">
              <el-input
                v-model="formData.sqlQuery"
                type="textarea"
                :rows="5"
                :placeholder="t('report.edit.sqlPlaceholder')"
              />
            </el-form-item>

            <!-- Display Settings -->
            <div class="section-title">{{ t('report.edit.displaySettings') }}</div>
            <el-form-item :label="t('report.edit.showChart')">
              <el-switch v-model="formData.showChart" />
            </el-form-item>
            <el-form-item :label="t('report.edit.showTable')">
              <el-switch v-model="formData.showTable" />
            </el-form-item>

            <!-- Chart Settings -->
            <div v-if="formData.showChart" class="section-title">{{ t('report.edit.chartSettings') }}</div>
            <template v-if="formData.showChart">
              <el-form-item :label="t('report.edit.chartType')" prop="chartType">
                <el-radio-group v-model="formData.chartType">
                  <el-radio-button value="bar">{{ t('report.edit.barChart') }}</el-radio-button>
                  <el-radio-button value="line">{{ t('report.edit.lineChart') }}</el-radio-button>
                  <el-radio-button value="pie">{{ t('report.edit.pieChart') }}</el-radio-button>
                </el-radio-group>
              </el-form-item>
              <el-form-item :label="t('report.edit.xAxisField')" prop="xAxisField">
                <el-select
                  v-model="formData.xAxisField"
                  :placeholder="t('report.edit.xAxisPlaceholder')"
                  style="width: 100%"
                >
                  <el-option label="date" value="date" />
                  <el-option label="product_name" value="product_name" />
                  <el-option label="category" value="category" />
                </el-select>
              </el-form-item>
              <el-form-item :label="t('report.edit.yAxisField')" prop="yAxisField">
                <el-select
                  v-model="formData.yAxisField"
                  :placeholder="t('report.edit.yAxisPlaceholder')"
                  style="width: 100%"
                >
                  <el-option label="amount" value="amount" />
                  <el-option label="quantity" value="quantity" />
                  <el-option label="total" value="total" />
                  <el-option label="count" value="count" />
                </el-select>
              </el-form-item>
              <el-form-item :label="t('report.edit.aggregation')" prop="aggregation">
                <el-select
                  v-model="formData.aggregation"
                  :placeholder="t('report.edit.aggregationPlaceholder')"
                  style="width: 100%"
                >
                  <el-option :label="t('report.edit.sum')" value="sum" />
                  <el-option :label="t('report.edit.count')" value="count" />
                  <el-option :label="t('report.edit.avg')" value="avg" />
                </el-select>
              </el-form-item>
            </template>

            <!-- Column Configuration Section -->
            <div v-if="formData.showTable" class="section-title">{{ t('report.edit.columnConfig') }}</div>
            <template v-if="formData.showTable">
            <el-form-item :label="t('report.edit.autoColumns')">
              <el-switch v-model="autoColumns" />
            </el-form-item>
            <el-form-item :label="t('report.edit.tableTemplate')">
              <el-select
                v-model="columnTemplateId"
                :placeholder="t('report.edit.selectTemplate')"
                clearable
                style="width: 100%"
                @change="handleTemplateChange"
              >
                <el-option
                  v-for="tpl in columnTemplates.filter(t => t.type === 'table')"
                  :key="tpl.id"
                  :label="tpl.name"
                  :value="tpl.id"
                />
              </el-select>
            </el-form-item>
            <el-form-item :label="t('report.edit.columnConfig')">
              <div class="column-config-table">
                <el-table :data="columnConfigs" border size="small">
                  <el-table-column prop="field" :label="t('columnTemplate.edit.fieldName')" width="120">
                    <template #default="{ row }">
                      <el-input v-model="row.field" size="small" :placeholder="t('columnTemplate.edit.fieldNamePlaceholder')" />
                    </template>
                  </el-table-column>
                  <el-table-column prop="label" :label="t('columnTemplate.edit.label')" width="120">
                    <template #default="{ row }">
                      <el-input v-model="row.label" size="small" :placeholder="t('columnTemplate.edit.labelPlaceholder')" />
                    </template>
                  </el-table-column>
                  <el-table-column prop="width" :label="t('columnTemplate.edit.width')" width="150">
                    <template #default="{ row }">
                      <el-input-number v-model="row.width" size="small" :min="50" :max="500" />
                    </template>
                  </el-table-column>
                  <el-table-column prop="format" :label="t('columnTemplate.edit.format')" width="120">
                    <template #default="{ row }">
                      <el-select v-model="row.format" size="small" clearable>
                        <el-option :label="t('columnTemplate.edit.formatNone')" value="" />
                        <el-option :label="t('columnTemplate.edit.formatNumber')" value="number" />
                        <el-option :label="t('columnTemplate.edit.formatMoney')" value="money" />
                        <el-option :label="t('columnTemplate.edit.formatDate')" value="date" />
                        <el-option :label="t('columnTemplate.edit.formatPercent')" value="percent" />
                      </el-select>
                    </template>
                  </el-table-column>
                  <el-table-column :label="t('columnTemplate.edit.drilldown')" width="120">
                    <template #default="{ row, $index }">
                      <el-button
                        v-if="row.drilldown?.enabled"
                        type="primary"
                        link
                        size="small"
                        @click="openDrilldownDialog($index)"
                      >
                        {{ t('columnTemplate.edit.drilldownEnabled') }}
                      </el-button>
                      <el-button
                        v-else
                        link
                        size="small"
                        @click="openDrilldownDialog($index)"
                      >
                        {{ t('columnTemplate.edit.drilldown') }}
                      </el-button>
                    </template>
                  </el-table-column>
                  <el-table-column :label="t('columnTemplate.list.operation')" width="80">
                    <template #default="{ $index }">
                      <el-button link type="danger" size="small" @click="removeColumn($index)">
                        {{ t('columnTemplate.edit.removeColumn') }}
                      </el-button>
                    </template>
                  </el-table-column>
                </el-table>
                <div class="column-actions">
                  <el-button size="small" @click="addColumn">{{ t('columnTemplate.edit.addColumn') }}</el-button>
                  <el-button size="small" @click="applySingleTemplate">{{ t('report.edit.applyTemplate') }}</el-button>
                </div>
              </div>
            </el-form-item>
            </template>

            <!-- Drilldown Config Dialog -->
            <DrilldownConfigDialog
              v-model="drilldownDialogVisible"
              :config="currentColumnIndex !== undefined ? columnConfigs[currentColumnIndex]?.drilldown : undefined"
              :available-fields="availableFields"
              @save="saveDrilldownConfig"
            />

            <!-- Actions -->
            <el-form-item>
              <el-button type="primary" @click="handleSave">
                <el-icon><Check /></el-icon>
                {{ t('report.edit.save') }}
              </el-button>
              <el-button @click="goBack">
                {{ t('report.edit.cancel') }}
              </el-button>
              <el-button @click="handlePreview">
                <el-icon><View /></el-icon>
                {{ t('report.edit.preview') }}
              </el-button>
            </el-form-item>
          </el-form>
        </div>

        <!-- Preview Panel -->
        <div class="preview-section" ref="previewRef">
          <div class="preview-title">{{ t('report.edit.preview') }}</div>
          <div v-if="formData.showChart" class="chart-preview">
            <div class="chart-container" ref="chartRef"></div>
          </div>
          <div v-if="formData.showTable" class="data-preview">
            <div class="preview-label">{{ t('report.edit.dataPreview') }}</div>
            <el-table :data="previewTableData" border size="small" max-height="200">
              <el-table-column prop="name" label="名称" width="150" />
              <el-table-column prop="value" label="数值" width="100" />
            </el-table>
          </div>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, onUnmounted, nextTick, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Check, View } from '@element-plus/icons-vue'
import * as echarts from 'echarts'
import type { FormInstance, FormRules } from 'element-plus'
import {
  getReportDetail,
  createReport,
  updateReport,
  getReportCategories,
  previewReport,
} from '@/api/report/reportApi'
import { getDataSourceList } from '@/api/report/datasourceApi'
import { getColumnTemplateList, getSingleColumnTemplates } from '@/api/report/columnTemplateApi'
import DrilldownConfigDialog from './components/DrilldownConfigDialog.vue'
import { useLocale } from '@/composables/useLocale'
import type { Report, ReportCategory, DataSource, CreateReportParams, ChartType, AggregationType } from '@/types'
import type { ColumnConfig, DrilldownRule, ColumnTemplate, DetectedColumn } from '@/types'

const router = useRouter()
const route = useRoute()
const { t } = useLocale()

const loading = ref(false)
const isEdit = ref(false)
const formRef = ref<FormInstance | null>(null)
const categories = ref<ReportCategory[]>([])
const dataSources = ref<DataSource[]>([])

const chartRef = ref<HTMLElement | null>(null)
const previewRef = ref<HTMLElement | null>(null)
let chartInstance: echarts.ECharts | null = null
let scrollHandler: (() => void) | null = null

const formData = reactive<CreateReportParams>({
  name: '',
  category: '',
  datasourceId: '',
  sqlQuery: '',
  showChart: true,
  showTable: true,
  chartType: 'bar',
  xAxisField: 'date',
  yAxisField: 'amount',
  aggregation: 'sum',
})

const previewTableData = ref<{ name: string; value: number }[]>([])

// Column configuration
const autoColumns = ref(true)
const columnTemplateId = ref<string | undefined>()
const columnTemplates = ref<ColumnTemplate[]>([])
const columnConfigs = ref<ColumnConfig[]>([])
const detectedColumns = ref<DetectedColumn[]>([])

// Drilldown config dialog
const drilldownDialogVisible = ref(false)
const currentColumnIndex = ref<number | undefined>()
const availableFields = ref<string[]>([])

const formRules: FormRules = {
  name: [{ required: true, message: t('report.edit.validation.nameRequired'), trigger: 'blur' }],
  category: [{ required: true, message: t('report.edit.validation.categoryRequired'), trigger: 'change' }],
  datasourceId: [{ required: true, message: t('report.edit.validation.dataSourceRequired'), trigger: 'change' }],
  sqlQuery: [{ required: true, message: t('report.edit.validation.sqlRequired'), trigger: 'blur' }],
}

// 滚动监听 - 实现右侧预览跟随滚动
const setupScrollListener = () => {
  // 找到滚动容器 .app-main
  const appMain = document.querySelector('.app-main')
  if (!appMain || !previewRef.value) return

  scrollHandler = () => {
    const scrollTop = appMain.scrollTop
    const previewEl = previewRef.value
    if (!previewEl) return

    // 设置 top 值，使预览区域保持在可视区域顶部
    previewEl.style.position = 'relative'
    previewEl.style.top = `${scrollTop + 10}px`
  }

  appMain.addEventListener('scroll', scrollHandler)
}

const removeScrollListener = () => {
  const appMain = document.querySelector('.app-main')
  if (appMain && scrollHandler) {
    appMain.removeEventListener('scroll', scrollHandler)
    scrollHandler = null
  }
}

onMounted(async () => {
  await loadOptions()
  await loadColumnTemplates()
  const id = route.params.id
  if (id) {
    isEdit.value = true
    await loadReport(id as string)
  }
  // 设置滚动监听，实现右侧预览跟随滚动
  setupScrollListener()
})

onUnmounted(() => {
  if (chartInstance) {
    chartInstance.dispose()
    chartInstance = null
  }
  // 移除滚动监听
  removeScrollListener()
})

// Watch for chartType changes to update preview
watch(() => formData.chartType, () => {
  handlePreview()
})

// Watch for showChart/showTable changes
watch([() => formData.showChart, () => formData.showTable], () => {
  handlePreview()
})

const loadOptions = async () => {
  try {
    const cats = await getReportCategories()
    categories.value = cats

    const dsData = await getDataSourceList({ pageIndex: 1, pageSize: 100 })
    dataSources.value = dsData.list
  } catch (error) {
    // Error handled by interceptor
  }
}

// Load column configuration templates
const loadColumnTemplates = async () => {
  try {
    const data = await getColumnTemplateList({ pageIndex: 1, pageSize: 100 })
    columnTemplates.value = data.list
  } catch (error) {
    // Error handled
  }
}

// Handle template selection change
const handleTemplateChange = (templateId: string | undefined) => {
  if (!templateId) return
  const template = columnTemplates.value.find(t => t.id === templateId)
  if (template && template.type === 'table') {
    columnConfigs.value = JSON.parse(JSON.stringify(template.columns))
    updateAvailableFields()
  }
}

// Add column
const addColumn = () => {
  columnConfigs.value.push({
    field: '',
    label: '',
    width: 100,
    align: 'left',
  })
}

// Remove column
const removeColumn = (index: number) => {
  columnConfigs.value.splice(index, 1)
}

// Apply single column template
const applySingleTemplate = async () => {
  try {
    const templates = await getSingleColumnTemplates()
    if (templates.length > 0) {
      // Simplified: add the first template's column
      columnConfigs.value.push(JSON.parse(JSON.stringify(templates[0].columns[0])))
    }
  } catch (error) {
    // Error handled
  }
}

// Open drilldown config dialog
const openDrilldownDialog = (index: number) => {
  currentColumnIndex.value = index
  drilldownDialogVisible.value = true
}

// Save drilldown config
const saveDrilldownConfig = (config: DrilldownRule) => {
  if (currentColumnIndex.value !== undefined) {
    columnConfigs.value[currentColumnIndex.value].drilldown = config
  }
}

// Update available fields list
const updateAvailableFields = () => {
  availableFields.value = columnConfigs.value.map(c => c.field).filter(Boolean)
}

const loadReport = async (id: string) => {
  loading.value = true
  try {
    const data = await getReportDetail(id)
    formData.name = data.name
    formData.category = data.category
    formData.datasourceId = data.datasourceId
    formData.sqlQuery = data.sqlQuery
    formData.showChart = data.showChart ?? true
    formData.showTable = data.showTable ?? true
    formData.chartType = data.chartType
    formData.xAxisField = data.xAxisField
    formData.yAxisField = data.yAxisField
    formData.aggregation = data.aggregation

    // Load column configuration
    autoColumns.value = data.autoColumns ?? true
    columnTemplateId.value = data.columnTemplateId ?? undefined
    columnConfigs.value = data.columnConfigs ?? []
    updateAvailableFields()

    // Load preview
    await handlePreview()
  } catch (error) {
    ElMessage.error('加载报表失败')
  } finally {
    loading.value = false
  }
}

const handleDataSourceChange = () => {
  // Could update available fields based on data source
}

const handlePreview = async () => {
  if (!formData.datasourceId || !formData.sqlQuery) return

  try {
    const preview = await previewReport({
      datasourceId: formData.datasourceId,
      sqlQuery: formData.sqlQuery,
      chartType: formData.chartType,
      xAxisField: formData.xAxisField,
      yAxisField: formData.yAxisField,
      aggregation: formData.aggregation,
    })

    previewTableData.value = preview.chartData

    // Handle detected columns
    if (formData.showTable) {
      detectedColumns.value = preview.detectedColumns || []
      if (autoColumns.value && detectedColumns.value.length > 0) {
        columnConfigs.value = detectedColumns.value.map(col => ({
          field: col.field,
          label: col.field,
          width: 100,
          align: col.type === 'number' ? 'right' : 'left',
        }))
      }
      updateAvailableFields()
    }

    // Update chart
    if (formData.showChart) {
      await nextTick()
      initChart(formData.chartType, preview.chartData)
    }
  } catch (error) {
    // Error handled by interceptor
  }
}

const initChart = (chartType: ChartType, chartData: { name: string; value: number }[]) => {
  if (!chartRef.value) return

  if (chartInstance) {
    chartInstance.dispose()
  }

  chartInstance = echarts.init(chartRef.value)

  const option: echarts.EChartsOption = getChartOption(chartType, chartData)
  chartInstance.setOption(option)
}

const getChartOption = (chartType: ChartType, chartData: { name: string; value: number }[]): echarts.EChartsOption => {
  const baseOption = {
    tooltip: { trigger: 'item' as const },
    legend: { bottom: 0 },
  }

  switch (chartType) {
    case 'bar':
      return {
        ...baseOption,
        xAxis: { type: 'category' as const, data: chartData.map(d => d.name) },
        yAxis: { type: 'value' as const },
        series: [{
          type: 'bar' as const,
          data: chartData.map(d => d.value),
          itemStyle: { color: '#409eff' },
        }],
      }
    case 'line':
      return {
        ...baseOption,
        xAxis: { type: 'category' as const, data: chartData.map(d => d.name) },
        yAxis: { type: 'value' as const },
        series: [{
          type: 'line' as const,
          data: chartData.map(d => d.value),
          smooth: true,
          itemStyle: { color: '#409eff' },
        }],
      }
    case 'pie':
      return {
        ...baseOption,
        series: [{
          type: 'pie' as const,
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
    case 'table':
      return {}
    default:
      return {}
  }
}

const handleSave = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
  } catch {
    return
  }

  loading.value = true
  try {
    const params = {
      ...formData,
      autoColumns: autoColumns.value,
      columnTemplateId: columnTemplateId.value,
      columnConfigs: columnConfigs.value,
    }

    if (isEdit.value) {
      const id = route.params.id as string
      await updateReport({ id, ...params })
    } else {
      await createReport(params)
    }
    ElMessage.success(t('report.edit.saveSuccess'))
    router.push('/report/list')
  } catch (error) {
    ElMessage.error(t('report.edit.saveFailed'))
  } finally {
    loading.value = false
  }
}

const goBack = () => {
  router.push('/report/list')
}
</script>

<style scoped lang="scss">
.report-edit {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .edit-content {
    display: flex;
    gap: 24px;
    align-items: flex-start;

    .form-section {
      flex: 1;
      min-width: 0;

      .section-title {
        font-weight: bold;
        margin: 16px 0 12px;
        padding-bottom: 8px;
        border-bottom: 1px solid #ebeef5;
      }

      .column-config-table {
        width: 100%;

        .column-actions {
          margin-top: 8px;
          display: flex;
          gap: 8px;
        }
      }
    }

    .preview-section {
      width: 350px;
      padding: 16px;
      border: 1px solid #ebeef5;
      border-radius: 4px;
      background: #f5f7fa;
      flex-shrink: 0;

      .preview-title {
        font-weight: bold;
        margin-bottom: 12px;
      }

      .chart-preview {
        margin-bottom: 12px;

        .chart-container {
          height: 200px;
          background: white;
          border-radius: 4px;
        }
      }

      .data-preview {
        .preview-label {
          font-size: 12px;
          color: #606266;
          margin-bottom: 8px;
        }
      }
    }
  }
}
</style>