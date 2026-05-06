<!-- src/views/ops/LogQuery.vue -->
<template>
  <div class="log-query-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>日志查询</span>
          <div class="header-actions">
            <el-select
              v-model="currentEnvironment"
              placeholder="选择环境"
              style="width: 120px; margin-right: 12px"
              @change="handleEnvironmentChange"
            >
              <el-option
                v-for="env in environments"
                :key="env"
                :label="env"
                :value="env"
              />
            </el-select>
            <el-button type="primary" @click="handleExport">
              <el-icon><Download /></el-icon>
              导出
            </el-button>
          </div>
        </div>
      </template>

      <!-- Search form -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item label="时间范围">
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
        <el-form-item label="日志级别">
          <el-select
            v-model="queryParams.level"
            placeholder="全部级别"
            clearable
            style="width: 140px"
          >
            <el-option
              v-for="item in levelOptions"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="请求路径">
          <el-input
            v-model="queryParams.requestPath"
            placeholder="模糊匹配"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item label="消息关键字">
          <el-input
            v-model="queryParams.messageKeyword"
            placeholder="全文搜索"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item label="异常关键字">
          <el-input
            v-model="queryParams.exceptionKeyword"
            placeholder="全文搜索"
            clearable
            style="width: 200px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
        </el-form-item>
      </el-form>

      <!-- Table -->
      <BaseTable
        ref="baseTableRef"
        :data="tableData"
        :columns="tableColumns"
        :loading="loading"
        :total="total"
        :page-index="queryParams.pageIndex"
        :page-size="queryParams.pageSize"
        @update:page-index="queryParams.pageIndex = $event"
        @update:page-size="queryParams.pageSize = $event"
        @page-change="handleSearch"
      >
        <!-- Timestamp column -->
        <template #timestamp="{ row }">
          <span class="timestamp">{{ formatTimestamp(row.timestamp) }}</span>
        </template>

        <!-- Level column -->
        <template #level="{ row }">
          <el-tag :type="getLevelType(row.level)" :class="{ 'fatal-bold': row.level === 'Fatal' }">
            {{ row.level }}
          </el-tag>
        </template>

        <!-- Message column -->
        <template #message="{ row }">
          <el-tooltip :content="row.message" placement="top" :disabled="row.message.length <= 50">
            <span>{{ truncateText(row.message, 50) }}</span>
          </el-tooltip>
        </template>

        <!-- Duration column -->
        <template #duration="{ row }">
          <span v-if="row.duration !== undefined" :class="getDurationClass(row.duration)">
            {{ row.duration }}ms
          </span>
          <span v-else>-</span>
        </template>

        <!-- Exception column -->
        <template #exception="{ row }">
          <el-tooltip v-if="row.exception" :content="row.exception" placement="top" :disabled="row.exception.length <= 30">
            <span class="exception-text">{{ truncateText(row.exception, 30) }}</span>
          </el-tooltip>
          <span v-else>-</span>
        </template>

        <!-- Operation column -->
        <template #operation>
          <el-table-column label="操作" width="80" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleDetail(row)">
                详情
              </el-button>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>

    <!-- Log detail dialog -->
    <LogDetailDialog
      v-model:visible="detailVisible"
      :environment="currentEnvironment"
      :log-id="currentLogId"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Search, Refresh, Download } from '@element-plus/icons-vue'
import { queryLogs, getLogEnvironments, exportLogs } from '@/api/ops/logQueryApi'
import type { LogEntry, LogLevel } from '@/types/logQuery'
import type { TableColumn } from '@/components/BaseTable/index.vue'
import BaseTable from '@/components/BaseTable/index.vue'
import LogDetailDialog from './components/LogDetailDialog.vue'

// Level options for select
const levelOptions = [
  { value: 'Debug', label: 'Debug' },
  { value: 'Information', label: 'Information' },
  { value: 'Warning', label: 'Warning' },
  { value: 'Error', label: 'Error' },
  { value: 'Fatal', label: 'Fatal' },
]

// Table columns
const tableColumns = ref<TableColumn[]>([
  { prop: 'timestamp', label: '时间', width: 160 },
  { prop: 'level', label: '级别', width: 100, align: 'center' },
  { prop: 'message', label: '消息', minWidth: 200 },
  { prop: 'requestPath', label: '请求路径', width: 180 },
  { prop: 'method', label: '方法', width: 80, align: 'center' },
  { prop: 'userName', label: '用户', width: 100 },
  { prop: 'ipAddress', label: 'IP地址', width: 130 },
  { prop: 'duration', label: '耗时', width: 100, align: 'right' },
  { prop: 'machineName', label: '机器', width: 100 },
  { prop: 'exception', label: '异常', width: 150 },
])

const loading = ref(false)
const tableData = ref<LogEntry[]>([])
const total = ref(0)
const baseTableRef = ref()

// Environments
const environments = ref<string[]>([])
const currentEnvironment = ref('Production')

// Time range
const timeRange = ref<[string, string] | null>(null)

// Query params
const queryParams = reactive({
  environment: 'Production',
  startTime: '',
  endTime: '',
  level: '' as LogLevel | '',
  requestPath: '',
  messageKeyword: '',
  exceptionKeyword: '',
  pageIndex: 1,
  pageSize: 10,
})

// Dialog state
const detailVisible = ref(false)
const currentLogId = ref('')

onMounted(async () => {
  await loadEnvironments()
  handleSearch()
})

/**
 * Load available environments
 */
const loadEnvironments = async () => {
  try {
    const data = await getLogEnvironments()
    environments.value = data
    if (data.length > 0 && !data.includes(currentEnvironment.value)) {
      currentEnvironment.value = data[0]
      queryParams.environment = data[0]
    }
  } catch (error) {
    // Error handled by interceptor
  }
}

/**
 * Format timestamp for display
 */
const formatTimestamp = (timestamp: string) => {
  if (!timestamp) return '-'
  // Convert to local time format: YYYY-MM-DD HH:mm:ss
  const date = new Date(timestamp)
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')
  const seconds = String(date.getSeconds()).padStart(2, '0')
  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`
}

/**
 * Get level tag type
 */
const getLevelType = (level: LogLevel): 'success' | 'warning' | 'info' | 'danger' => {
  const types: Record<LogLevel, 'success' | 'warning' | 'info' | 'danger'> = {
    Debug: 'info',
    Information: 'success',
    Warning: 'warning',
    Error: 'danger',
    Fatal: 'danger',
  }
  return types[level] || 'info'
}

/**
 * Truncate text with ellipsis
 */
const truncateText = (text: string, maxLength: number) => {
  if (!text) return '-'
  return text.length > maxLength ? text.substring(0, maxLength) + '...' : text
}

/**
 * Get duration CSS class
 */
const getDurationClass = (duration: number) => {
  if (duration < 200) return 'duration-fast'
  if (duration < 1000) return 'duration-normal'
  return 'duration-slow'
}

/**
 * Handle time range change
 */
const handleTimeRangeChange = (value: [string, string] | null) => {
  if (value) {
    queryParams.startTime = value[0]
    queryParams.endTime = value[1]
  } else {
    queryParams.startTime = ''
    queryParams.endTime = ''
  }
}

/**
 * Handle environment change
 */
const handleEnvironmentChange = (value: string) => {
  queryParams.environment = value
  queryParams.pageIndex = 1
  handleSearch()
}

/**
 * Load logs from API
 */
const handleSearch = async () => {
  loading.value = true
  try {
    const params = {
      environment: queryParams.environment,
      startTime: queryParams.startTime || undefined,
      endTime: queryParams.endTime || undefined,
      level: queryParams.level || undefined,
      requestPath: queryParams.requestPath || undefined,
      messageKeyword: queryParams.messageKeyword || undefined,
      exceptionKeyword: queryParams.exceptionKeyword || undefined,
      pageIndex: queryParams.pageIndex,
      pageSize: queryParams.pageSize,
    }
    const data = await queryLogs(params)
    tableData.value = data.items
    total.value = data.total
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

/**
 * Reset search form
 */
const handleReset = () => {
  queryParams.startTime = ''
  queryParams.endTime = ''
  queryParams.level = ''
  queryParams.requestPath = ''
  queryParams.messageKeyword = ''
  queryParams.exceptionKeyword = ''
  timeRange.value = null
  queryParams.pageIndex = 1
  handleSearch()
}

/**
 * View log detail
 */
const handleDetail = (row: LogEntry) => {
  currentLogId.value = row.id
  detailVisible.value = true
}

/**
 * Export logs
 */
const handleExport = async () => {
  try {
    const params = {
      environment: queryParams.environment,
      startTime: queryParams.startTime || undefined,
      endTime: queryParams.endTime || undefined,
      level: queryParams.level || undefined,
      requestPath: queryParams.requestPath || undefined,
      messageKeyword: queryParams.messageKeyword || undefined,
      exceptionKeyword: queryParams.exceptionKeyword || undefined,
      pageIndex: 1,
      pageSize: 10000, // Export all
    }
    const blob = await exportLogs(params)
    // Create download link
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `logs_${queryParams.environment}_${new Date().toISOString().slice(0, 10)}.xlsx`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
    ElMessage.success('导出成功')
  } catch (error) {
    ElMessage.error('导出失败')
  }
}
</script>

<style scoped lang="scss">
.log-query-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .header-actions {
      display: flex;
      align-items: center;
    }
  }

  .search-form {
    margin-bottom: 16px;
  }

  .timestamp {
    color: #606266;
  }

  .fatal-bold {
    font-weight: bold;
  }

  .duration-fast {
    color: #67c23a;
  }

  .duration-normal {
    color: #606266;
  }

  .duration-slow {
    color: #f56c6c;
  }

  .exception-text {
    color: #f56c6c;
  }
}
</style>