<!-- src/views/etl/monitor/index.vue -->
<template>
  <div class="monitor-container">
    <!-- Statistics Cards -->
    <el-row :gutter="20" class="statistics-row">
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-content">
            <div class="stat-icon running">
              <el-icon><Loading /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-value">{{ statistics.runningCount }}</div>
              <div class="stat-label">{{ t('etl.monitor.statistics.running') }}</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-content">
            <div class="stat-icon success">
              <el-icon><SuccessFilled /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-value">{{ statistics.successCount }}</div>
              <div class="stat-label">{{ t('etl.monitor.statistics.success') }}</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-content">
            <div class="stat-icon failure">
              <el-icon><CircleCloseFilled /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-value">{{ statistics.failureCount }}</div>
              <div class="stat-label">{{ t('etl.monitor.statistics.failure') }}</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="stat-card">
          <div class="stat-content">
            <div class="stat-icon total">
              <el-icon><DataAnalysis /></el-icon>
            </div>
            <div class="stat-info">
              <div class="stat-value">{{ statistics.totalCount }}</div>
              <div class="stat-label">{{ t('etl.monitor.statistics.total') }}</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- Execution List -->
    <el-card shadow="never" class="execution-card">
      <template #header>
        <div class="card-header">
          <span>{{ t('etl.monitor.list.title') }}</span>
        </div>
      </template>

      <!-- Search Form -->
      <SearchForm
        :items="searchItems"
        v-model="queryParams"
        :loading="loading"
        @search="handleSearch"
        @reset="handleReset"
      />

      <!-- Table -->
      <el-table
        v-loading="loading"
        :data="tableData"
        border
      >
        <el-table-column prop="pipelineName" :label="t('etl.monitor.list.pipelineName')" min-width="180" />
        <el-table-column prop="executionNo" :label="t('etl.monitor.list.executionNo')" width="180" />
        <el-table-column prop="status" :label="t('etl.monitor.list.status')" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusTagType(row.status)">
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="triggerType" :label="t('etl.monitor.list.triggerType')" width="100" align="center">
          <template #default="{ row }">
            <el-tag type="info" effect="plain">
              {{ getTriggerTypeText(row.triggerType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="startTime" :label="t('etl.monitor.list.startTime')" width="160" />
        <el-table-column prop="endTime" :label="t('etl.monitor.list.endTime')" width="160" />
        <el-table-column prop="duration" :label="t('etl.monitor.list.duration')" width="100">
          <template #default="{ row }">
            {{ formatDuration(row.duration) }}
          </template>
        </el-table-column>
        <el-table-column prop="progress" :label="t('etl.monitor.list.progress')" width="120" align="center">
          <template #default="{ row }">
            <el-progress
              :percentage="row.progress || 0"
              :status="row.status === 'success' ? 'success' : row.status === 'failure' ? 'exception' : undefined"
              :stroke-width="6"
            />
          </template>
        </el-table-column>
        <el-table-column :label="t('etl.monitor.list.operation')" width="180" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row)">
              {{ t('etl.monitor.list.detail') }}
            </el-button>
            <el-button
              v-if="row.status === 'running'"
              link
              type="warning"
              @click="handleCancel(row)"
            >
              {{ t('etl.monitor.list.cancel') }}
            </el-button>
            <el-button
              v-if="row.status === 'failure'"
              link
              type="success"
              @click="handleRetry(row)"
            >
              {{ t('etl.monitor.list.retry') }}
            </el-button>
            <el-button link type="primary" @click="handleLogs(row)">
              {{ t('etl.monitor.list.logs') }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- Pagination -->
      <el-pagination
        v-model:current-page="queryParams.pageIndex"
        v-model:page-size="queryParams.pageSize"
        :total="total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSearch"
        @current-change="handleSearch"
        style="margin-top: 20px; justify-content: flex-end"
      />
    </el-card>

    <!-- Execution Detail Dialog -->
    <ExecutionDetailDialog
      v-model="detailDialogVisible"
      :execution-id="currentExecutionId"
    />

    <!-- Execution Logs Dialog -->
    <ExecutionLogsDialog
      v-model="logsDialogVisible"
      :execution-id="currentExecutionId"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Loading, SuccessFilled, CircleCloseFilled, DataAnalysis } from '@element-plus/icons-vue'
import {
  getExecutionList,
  getExecutionStatistics,
  cancelExecution,
  retryExecution,
} from '@/api/etl/monitorApi'
import { useLocale } from '@/composables/useLocale'
import type { Execution, ExecutionQueryParams, ExecutionStatistics } from '@/types/etl'
import { ExecutionStatus, TriggerType } from '@/types/etl'
import SearchForm from '@/components/SearchForm/index.vue'
import ExecutionDetailDialog from './components/ExecutionDetailDialog.vue'
import ExecutionLogsDialog from './components/ExecutionLogsDialog.vue'

const { t } = useLocale()

// Statistics
const statistics = ref<ExecutionStatistics>({
  totalCount: 0,
  runningCount: 0,
  successCount: 0,
  failureCount: 0,
  pendingCount: 0,
  avgDuration: 0,
  successRate: 0,
})

// Search form config
const searchItems = computed(() => [
  {
    field: 'pipelineId',
    label: t('etl.monitor.list.pipelineName'),
    type: 'select',
    options: pipelineOptions.value,
  },
  {
    field: 'status',
    label: t('etl.monitor.list.status'),
    type: 'select',
    options: [
      { label: t('etl.monitor.list.statusRunning'), value: ExecutionStatus.RUNNING },
      { label: t('etl.monitor.list.statusSuccess'), value: ExecutionStatus.SUCCESS },
      { label: t('etl.monitor.list.statusFailure'), value: ExecutionStatus.FAILURE },
      { label: t('etl.monitor.list.statusCancelled'), value: ExecutionStatus.CANCELLED },
      { label: t('etl.monitor.list.statusPending'), value: ExecutionStatus.PENDING },
    ],
  },
  {
    field: 'dateRange',
    label: t('etl.monitor.list.dateRange'),
    type: 'dateRange',
    collapse: true,
  },
])

const loading = ref(false)
const tableData = ref<Execution[]>([])
const total = ref(0)
const pipelineOptions = ref<Array<{ label: string; value: string }>>([])

const detailDialogVisible = ref(false)
const logsDialogVisible = ref(false)
const currentExecutionId = ref<string | undefined>(undefined)

let queryParams = reactive<ExecutionQueryParams & { dateRange?: [string, string] }>({
  pageIndex: 1,
  pageSize: 10,
  pipelineId: undefined,
  status: undefined,
  dateStart: undefined,
  dateEnd: undefined,
  dateRange: undefined,
})

// Auto refresh timer
let refreshTimer: number | null = null

onMounted(() => {
  handleSearch()
  loadStatistics()
  loadPipelineOptions()
  // Auto refresh every 30 seconds
  refreshTimer = window.setInterval(() => {
    handleSearch(true)
    loadStatistics()
  }, 30000)
})

onUnmounted(() => {
  if (refreshTimer) {
    clearInterval(refreshTimer)
  }
})

const loadStatistics = async () => {
  try {
    const data = await getExecutionStatistics()
    statistics.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

const loadPipelineOptions = async () => {
  // Load pipeline options for filter
  pipelineOptions.value = [
    { label: '数据同步任务流', value: 'pipeline-1' },
    { label: '报表生成任务流', value: 'pipeline-2' },
  ]
}

const handleSearch = async (silent = false) => {
  if (!silent) loading.value = true
  try {
    // 将 dateRange 数组转换为 dateStart 和 dateEnd
    if (queryParams.dateRange && queryParams.dateRange.length === 2) {
      queryParams.dateStart = queryParams.dateRange[0]
      queryParams.dateEnd = queryParams.dateRange[1]
    } else {
      queryParams.dateStart = undefined
      queryParams.dateEnd = undefined
    }
    const { list, total: totalCount } = await getExecutionList(queryParams)
    tableData.value = list
    total.value = totalCount
  } catch (error) {
    // Error handled by interceptor
  } finally {
    if (!silent) loading.value = false
  }
}

const handleReset = () => {
  queryParams.pageIndex = 1
  queryParams.pipelineId = undefined
  queryParams.status = undefined
  queryParams.dateStart = undefined
  queryParams.dateEnd = undefined
  queryParams.dateRange = undefined
  handleSearch()
}

const handleDetail = (row: Execution) => {
  currentExecutionId.value = row.id
  detailDialogVisible.value = true
}

const handleLogs = (row: Execution) => {
  currentExecutionId.value = row.id
  logsDialogVisible.value = true
}

const handleCancel = async (row: Execution) => {
  try {
    await ElMessageBox.confirm(
      t('etl.monitor.list.cancelConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await cancelExecution(row.id)
    ElMessage.success(t('etl.monitor.list.cancelSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleRetry = async (row: Execution) => {
  try {
    await ElMessageBox.confirm(
      t('etl.monitor.list.retryConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'info',
      }
    )
    const result = await retryExecution(row.id)
    ElMessage.success(t('etl.monitor.list.retrySuccess', { executionId: result.executionId }))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const getStatusTagType = (status: string) => {
  switch (status) {
    case ExecutionStatus.RUNNING:
    case 'running':
      return 'primary'
    case ExecutionStatus.SUCCESS:
    case 'success':
      return 'success'
    case ExecutionStatus.FAILURE:
    case 'failure':
      return 'danger'
    case ExecutionStatus.CANCELLED:
    case 'cancelled':
      return 'warning'
    case ExecutionStatus.PENDING:
    case 'pending':
      return 'info'
    default:
      return 'info'
  }
}

const getStatusText = (status: string) => {
  switch (status) {
    case ExecutionStatus.RUNNING:
    case 'running':
      return t('etl.monitor.list.statusRunning')
    case ExecutionStatus.SUCCESS:
    case 'success':
      return t('etl.monitor.list.statusSuccess')
    case ExecutionStatus.FAILURE:
    case 'failure':
      return t('etl.monitor.list.statusFailure')
    case ExecutionStatus.CANCELLED:
    case 'cancelled':
      return t('etl.monitor.list.statusCancelled')
    case ExecutionStatus.PENDING:
    case 'pending':
      return t('etl.monitor.list.statusPending')
    default:
      return '-'
  }
}

const getTriggerTypeText = (triggerType: string) => {
  switch (triggerType) {
    case TriggerType.MANUAL:
    case 'manual':
      return t('etl.monitor.list.triggerManual')
    case TriggerType.SCHEDULE:
    case 'schedule':
      return t('etl.monitor.list.triggerSchedule')
    case TriggerType.API:
    case 'api':
      return t('etl.monitor.list.triggerApi')
    default:
      return '-'
  }
}

const formatDuration = (duration?: number) => {
  if (!duration) return '-'
  if (duration < 1000) return `${duration}ms`
  if (duration < 60000) return `${(duration / 1000).toFixed(1)}s`
  if (duration < 3600000) return `${(duration / 60000).toFixed(1)}m`
  return `${(duration / 3600000).toFixed(1)}h`
}
</script>

<style scoped lang="scss">
.monitor-container {
  padding: 20px;

  .statistics-row {
    margin-bottom: 20px;

    .stat-card {
      .stat-content {
        display: flex;
        align-items: center;
        gap: 16px;

        .stat-icon {
          width: 48px;
          height: 48px;
          border-radius: 8px;
          display: flex;
          align-items: center;
          justify-content: center;
          font-size: 24px;

          &.running {
            background: #e6f7ff;
            color: #1890ff;
          }

          &.success {
            background: #f6ffed;
            color: #52c41a;
          }

          &.failure {
            background: #fff2f0;
            color: #ff4d4f;
          }

          &.total {
            background: #f0f5ff;
            color: #2f54eb;
          }
        }

        .stat-info {
          .stat-value {
            font-size: 24px;
            font-weight: 600;
            color: #303133;
          }

          .stat-label {
            font-size: 14px;
            color: #909399;
          }
        }
      }
    }
  }

  .execution-card {
    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }
  }
}
</style>