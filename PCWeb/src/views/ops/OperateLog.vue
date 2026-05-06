<!-- src/views/ops/OperateLog.vue -->
<template>
  <div class="operate-log-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('ops.operateLog.title') }}</span>
          <div class="header-actions">
            <el-button type="danger" @click="handleClear">
              <el-icon><Delete /></el-icon>
              {{ t('ops.operateLog.clear') }}
            </el-button>
          </div>
        </div>
      </template>

      <!-- Search form -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('ops.operateLog.search.timeRange')">
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
        <el-form-item :label="t('ops.operateLog.search.userName')">
          <el-input
            v-model="queryParams.userName"
            :placeholder="t('common.placeholder.input')"
            clearable
            style="width: 150px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('ops.operateLog.search.module')">
          <el-input
            v-model="queryParams.module"
            :placeholder="t('common.placeholder.input')"
            clearable
            style="width: 150px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('ops.operateLog.search.action')">
          <el-input
            v-model="queryParams.action"
            :placeholder="t('common.placeholder.input')"
            clearable
            style="width: 150px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('ops.operateLog.search.status')">
          <el-select
            v-model="queryParams.status"
            :placeholder="t('common.placeholder.select')"
            clearable
            style="width: 120px"
          >
            <el-option :label="t('ops.operateLog.status.success')" :value="1" />
            <el-option :label="t('ops.operateLog.status.failure')" :value="0" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('ops.operateLog.search.ip')">
          <el-input
            v-model="queryParams.ip"
            :placeholder="t('common.placeholder.input')"
            clearable
            style="width: 150px"
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            {{ t('common.button.search') }}
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            {{ t('common.button.reset') }}
          </el-button>
        </el-form-item>
      </el-form>

      <!-- Table -->
      <el-table
        v-loading="loading"
        :data="tableData"
        border
        style="width: 100%"
      >
        <el-table-column prop="createTime" :label="t('ops.operateLog.table.createTime')" width="160">
          <template #default="{ row }">
            {{ formatTime(row.createTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="userName" :label="t('ops.operateLog.table.userName')" width="100" />
        <el-table-column prop="module" :label="t('ops.operateLog.table.module')" width="120" />
        <el-table-column prop="action" :label="t('ops.operateLog.table.action')" width="120">
          <template #default="{ row }">
            <el-tooltip :content="row.action" placement="top" :disabled="!row.action || row.action.length <= 10">
              <span>{{ truncateText(row.action, 10) }}</span>
            </el-tooltip>
          </template>
        </el-table-column>
        <el-table-column prop="method" :label="t('ops.operateLog.table.method')" width="80" align="center">
          <template #default="{ row }">
            <el-tag v-if="row.method" :type="getMethodType(row.method)" size="small">
              {{ row.method }}
            </el-tag>
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column prop="url" :label="t('ops.operateLog.table.url')" min-width="180">
          <template #default="{ row }">
            <el-tooltip :content="row.url" placement="top" :disabled="!row.url || row.url.length <= 40">
              <span>{{ truncateText(row.url, 40) }}</span>
            </el-tooltip>
          </template>
        </el-table-column>
        <el-table-column prop="ip" :label="t('ops.operateLog.table.ip')" width="130" />
        <el-table-column prop="status" :label="t('ops.operateLog.table.status')" width="80" align="center">
          <template #default="{ row }">
            <el-tag :type="row.status === 1 ? 'success' : 'danger'" size="small">
              {{ row.status === 1 ? t('ops.operateLog.status.success') : t('ops.operateLog.status.failure') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="duration" :label="t('ops.operateLog.table.duration')" width="100" align="right">
          <template #default="{ row }">
            <span :class="getDurationClass(row.duration)">
              {{ row.duration ? `${row.duration}ms` : '-' }}
            </span>
          </template>
        </el-table-column>
        <el-table-column :label="t('ops.operateLog.table.operation')" width="120" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDetail(row)">
              {{ t('ops.operateLog.detailBtn') }}
            </el-button>
            <el-button link type="danger" @click="handleDelete(row)">
              {{ t('common.button.delete') }}
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

    <!-- Detail Dialog -->
    <OperateLogDetailDialog
      v-model="detailVisible"
      :log-id="currentLogId"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Search, Refresh, Delete } from '@element-plus/icons-vue'
import { getOperateLogList, deleteOperateLog, clearOperateLog } from '@/api/ops/operateLogApi'
import { useLocale } from '@/composables/useLocale'
import type { OperateLog, QueryOperateLogParams } from '@/types/operateLog'
import OperateLogDetailDialog from './components/OperateLogDetailDialog.vue'

const { t } = useLocale()

const loading = ref(false)
const tableData = ref<OperateLog[]>([])
const total = ref(0)

// Time range
const timeRange = ref<[string, string] | null>(null)

// Query params
const queryParams = reactive<QueryOperateLogParams>({
  pageIndex: 1,
  pageSize: 10,
  userName: '',
  module: '',
  action: '',
  status: undefined,
  startTime: '',
  endTime: '',
  ip: '',
})

// Dialog state
const detailVisible = ref(false)
const currentLogId = ref('')

onMounted(() => {
  handleSearch()
})

/**
 * Format time for display
 */
const formatTime = (time?: string) => {
  if (!time) return '-'
  return time.replace('T', ' ').substring(0, 19)
}

/**
 * Truncate text with ellipsis
 */
const truncateText = (text?: string, maxLength: number) => {
  if (!text) return '-'
  return text.length > maxLength ? text.substring(0, maxLength) + '...' : text
}

/**
 * Get method tag type
 */
const getMethodType = (method: string): 'success' | 'warning' | 'info' | 'danger' => {
  const types: Record<string, 'success' | 'warning' | 'info' | 'danger'> = {
    GET: 'success',
    POST: 'warning',
    PUT: 'info',
    DELETE: 'danger',
    PATCH: 'info',
  }
  return types[method.toUpperCase()] || 'info'
}

/**
 * Get duration CSS class
 */
const getDurationClass = (duration?: number) => {
  if (!duration) return ''
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
 * Load logs from API
 */
const handleSearch = async () => {
  loading.value = true
  try {
    const params: QueryOperateLogParams = {
      pageIndex: queryParams.pageIndex,
      pageSize: queryParams.pageSize,
      userName: queryParams.userName || undefined,
      module: queryParams.module || undefined,
      action: queryParams.action || undefined,
      status: queryParams.status,
      startTime: queryParams.startTime || undefined,
      endTime: queryParams.endTime || undefined,
      ip: queryParams.ip || undefined,
    }
    const data = await getOperateLogList(params)
    tableData.value = data.list
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
  queryParams.userName = ''
  queryParams.module = ''
  queryParams.action = ''
  queryParams.status = undefined
  queryParams.startTime = ''
  queryParams.endTime = ''
  queryParams.ip = ''
  queryParams.pageIndex = 1
  timeRange.value = null
  handleSearch()
}

/**
 * View log detail
 */
const handleDetail = (row: OperateLog) => {
  currentLogId.value = row.id
  detailVisible.value = true
}

/**
 * Delete log
 */
const handleDelete = async (row: OperateLog) => {
  try {
    await ElMessageBox.confirm(
      t('ops.operateLog.deleteConfirm'),
      t('common.message.warning'),
      { type: 'warning' }
    )
    await deleteOperateLog(row.id)
    ElMessage.success(t('common.message.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

/**
 * Clear old logs
 */
const handleClear = async () => {
  try {
    const { value } = await ElMessageBox.prompt(
      t('ops.operateLog.clearPrompt'),
      t('ops.operateLog.clear'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        inputPattern: /^[1-9]\d*$/,
        inputErrorMessage: t('ops.operateLog.clearDaysError'),
        inputValue: '30',
      }
    )
    const days = parseInt(value, 10)
    const result = await clearOperateLog(days)
    ElMessage.success(t('ops.operateLog.clearSuccess', { count: result }))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}
</script>

<style scoped lang="scss">
.operate-log-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .header-actions {
      display: flex;
      gap: 12px;
    }
  }

  .search-form {
    margin-bottom: 16px;
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
}
</style>