<!-- PCWeb/src/views/ops/TaskLog.vue -->
<template>
  <div class="task-log-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('ops.taskLog.title') }}</span>
          <el-button type="danger" @click="handleClear">
            <el-icon><Delete /></el-icon>
            {{ t('ops.taskLog.clear') }}
          </el-button>
        </div>
      </template>

      <!-- 统计卡片 -->
      <el-row :gutter="16" class="stat-row">
        <el-col :span="6">
          <el-statistic :title="t('ops.taskLog.statistics.todayTotal')" :value="statistics.todayExecuted" />
        </el-col>
        <el-col :span="6">
          <el-statistic :title="t('ops.taskLog.statistics.successRate')" :value="successRate" suffix="%" />
        </el-col>
        <el-col :span="6">
          <el-statistic :title="t('ops.taskLog.statistics.avgDuration')" :value="avgDuration" suffix="ms" />
        </el-col>
        <el-col :span="6">
          <el-statistic :title="t('ops.taskLog.statistics.failureCount')" :value="statistics.todayFailure" />
        </el-col>
      </el-row>

      <!-- 执行趋势图表 -->
      <div class="chart-container">
        <v-chart :option="chartOption" autoresize style="height: 300px" />
      </div>

      <!-- 搜索栏 -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('ops.taskLog.search.timeRange')">
          <el-date-picker
            v-model="timeRange"
            type="datetimerange"
            range-separator="-"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 360px"
            @change="handleTimeRangeChange"
          />
        </el-form-item>
        <el-form-item :label="t('ops.taskLog.search.jobName')">
          <el-input v-model="queryParams.jobName" clearable style="width: 180px" placeholder="请输入任务名称" @keyup.enter="handleSearch" />
        </el-form-item>
        <el-form-item :label="t('ops.taskLog.search.status')">
          <el-select v-model="queryParams.status" clearable style="width: 120px" placeholder="请选择">
            <el-option :label="t('ops.taskLog.status.success')" :value="1" />
            <el-option :label="t('ops.taskLog.status.failure')" :value="2" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('ops.taskLog.search.triggerType')">
          <el-select v-model="queryParams.triggerType" clearable style="width: 120px" placeholder="请选择">
            <el-option :label="t('ops.taskLog.triggerType.cron')" :value="0" />
            <el-option :label="t('ops.taskLog.triggerType.manual')" :value="1" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">{{ t('common.button.search') }}</el-button>
          <el-button @click="handleReset">{{ t('common.button.reset') }}</el-button>
        </el-form-item>
      </el-form>

      <!-- 表格 -->
      <el-table v-loading="loading" :data="tableData" border>
        <el-table-column prop="startTime" :label="t('ops.taskLog.table.startTime')" width="160">
          <template #default="{ row }">{{ formatTime(row.startTime) }}</template>
        </el-table-column>
        <el-table-column prop="jobName" :label="t('ops.taskLog.table.jobName')" min-width="150" />
        <el-table-column prop="statusText" :label="t('ops.taskLog.table.status')" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status === 1 ? 'success' : 'danger'">{{ row.statusText }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="duration" :label="t('ops.taskLog.table.duration')" width="100" align="right">
          <template #default="{ row }">{{ row.duration ? `${row.duration}ms` : '-' }}</template>
        </el-table-column>
        <el-table-column prop="triggerType" :label="t('ops.taskLog.table.triggerType')" width="100" align="center">
          <template #default="{ row }">
            {{ row.triggerType === 0 ? t('ops.taskLog.triggerType.cron') : t('ops.taskLog.triggerType.manual') }}
          </template>
        </el-table-column>
        <el-table-column :label="t('ops.taskLog.table.operation')" width="100" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row)">{{ t('ops.taskLog.detailBtn') }}</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination
        v-model:current-page="queryParams.pageIndex"
        v-model:page-size="queryParams.pageSize"
        :total="total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next"
        @size-change="handleSearch"
        @current-change="handleSearch"
      />
    </el-card>

    <!-- 详情弹窗 -->
    <TaskLogDetailDialog v-model="detailVisible" :log-id="currentLogId" />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Delete } from '@element-plus/icons-vue'
import VChart from 'vue-echarts'
import { use } from 'echarts/core'
import { LineChart, BarChart } from 'echarts/charts'
import { GridComponent, TooltipComponent, LegendComponent } from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'
import { getTaskLogList, getTaskLogTrend, getTaskLogStatistics, clearTaskLog } from '@/api/ops/taskApi'
import { useLocale } from '@/composables/useLocale'
import type { TaskExecutionLog, QueryTaskLogParams, TaskStatistics, TaskLogTrend } from '@/types/task'
import TaskLogDetailDialog from './components/TaskLogDetailDialog.vue'

use([LineChart, BarChart, GridComponent, TooltipComponent, LegendComponent, CanvasRenderer])

const { t } = useLocale()

const loading = ref(false)
const tableData = ref<TaskExecutionLog[]>([])
const total = ref(0)
const timeRange = ref<[string, string] | null>(null)
const statistics = ref<TaskStatistics>({
  totalCount: 0,
  enabledCount: 0,
  pausedCount: 0,
  todayExecuted: 0,
  todaySuccess: 0,
  todayFailure: 0,
})
const trendData = ref<TaskLogTrend>({ points: [] })
const detailVisible = ref(false)
const currentLogId = ref('')

const queryParams = reactive<QueryTaskLogParams>({
  pageIndex: 1,
  pageSize: 20,
  jobName: '',
  status: undefined,
  triggerType: undefined,
  startTime: '',
  endTime: '',
})

const successRate = computed(() => {
  const total = statistics.value.todayExecuted
  if (total === 0) return 0
  return Math.round((statistics.value.todaySuccess / total) * 100)
})

const avgDuration = computed(() => {
  if (tableData.value.length === 0) return 0
  const durations = tableData.value.filter(x => x.duration).map(x => x.duration!)
  if (durations.length === 0) return 0
  return Math.round(durations.reduce((a, b) => a + b, 0) / durations.length)
})

const chartOption = computed(() => ({
  tooltip: { trigger: 'axis' },
  legend: { data: [t('ops.taskLog.chart.executeCount'), t('ops.taskLog.chart.successRate')] },
  xAxis: { type: 'category', data: trendData.value.points.map(p => p.date) },
  yAxis: [
    { type: 'value', name: t('ops.taskLog.chart.count') },
    { type: 'value', name: '%', max: 100 },
  ],
  series: [
    { name: t('ops.taskLog.chart.executeCount'), type: 'bar', data: trendData.value.points.map(p => p.executeCount) },
    { name: t('ops.taskLog.chart.successRate'), type: 'line', yAxisIndex: 1, data: trendData.value.points.map(p => p.successRate) },
  ],
}))

onMounted(() => {
  loadStatistics()
  loadTrend()
  handleSearch()
})

const loadStatistics = async () => {
  try {
    statistics.value = await getTaskLogStatistics()
  } catch (error) {
    // error handled
  }
}

const loadTrend = async () => {
  try {
    trendData.value = await getTaskLogTrend(7)
  } catch (error) {
    // error handled
  }
}

const handleTimeRangeChange = (value: [string, string] | null) => {
  if (value) {
    queryParams.startTime = value[0]
    queryParams.endTime = value[1]
  } else {
    queryParams.startTime = ''
    queryParams.endTime = ''
  }
}

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getTaskLogList(queryParams)
    tableData.value = data.list
    total.value = data.total
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.jobName = ''
  queryParams.status = undefined
  queryParams.triggerType = undefined
  queryParams.startTime = ''
  queryParams.endTime = ''
  queryParams.pageIndex = 1
  timeRange.value = null
  handleSearch()
}

const handleDetail = (row: TaskExecutionLog) => {
  currentLogId.value = row.id
  detailVisible.value = true
}

const handleClear = async () => {
  try {
    const { value } = await ElMessageBox.prompt(
      t('ops.taskLog.clearPrompt'),
      t('ops.taskLog.clear'),
      {
        inputPattern: /^[1-9]\d*$/,
        inputErrorMessage: t('ops.taskLog.clearError'),
        inputValue: '30',
      }
    )
    const days = parseInt(value, 10)
    const result = await clearTaskLog(days)
    ElMessage.success(t('ops.taskLog.clearSuccess', { count: result }))
    loadStatistics()
    loadTrend()
    handleSearch()
  } catch (error) {
    // cancelled
  }
}

const formatTime = (time?: string) => {
  if (!time) return '-'
  return time.replace('T', ' ').substring(0, 19)
}
</script>

<style scoped lang="scss">
.task-log-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .stat-row {
    margin-bottom: 20px;
  }

  .chart-container {
    margin-bottom: 20px;
  }

  .search-form {
    margin-bottom: 16px;
  }

  .el-pagination {
    margin-top: 16px;
    justify-content: flex-end;
  }
}
</style>