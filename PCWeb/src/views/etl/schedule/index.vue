<!-- src/views/etl/schedule/index.vue -->
<template>
  <div class="schedule-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('etl.schedule.list.title') }}</span>
          <div class="header-buttons">
            <el-button
              type="danger"
              :disabled="selectedRows.length === 0"
              @click="handleBatchDelete"
            >
              <el-icon><Delete /></el-icon>
              {{ t('etl.schedule.list.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('etl.schedule.list.add') }}
            </el-button>
          </div>
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
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="50" align="center" />
        <el-table-column prop="name" :label="t('etl.schedule.list.name')" min-width="150" />
        <el-table-column prop="pipelineName" :label="t('etl.schedule.list.pipelineName')" min-width="180" />
        <el-table-column prop="cronExpression" :label="t('etl.schedule.list.cronExpression')" width="150" />
        <el-table-column prop="cronDescription" :label="t('etl.schedule.list.cronDescription')" min-width="200" show-overflow-tooltip />
        <el-table-column prop="status" :label="t('etl.schedule.list.status')" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusTagType(row.status)">
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="lastExecuteTime" :label="t('etl.schedule.list.lastExecutionTime')" width="160" />
        <el-table-column prop="nextExecuteTime" :label="t('etl.schedule.list.nextExecutionTime')" width="160" />
        <el-table-column :label="t('etl.schedule.list.operation')" width="200" fixed="right">
          <template #default="{ row }">
            <el-button link type="success" @click="handleExecuteNow(row)">
              {{ t('etl.schedule.list.executeNow') }}
            </el-button>
            <el-button
              link
              :type="row.status === ScheduleStatus.ACTIVE ? 'warning' : 'success'"
              @click="handleToggleStatus(row)"
            >
              {{ row.status === ScheduleStatus.ACTIVE ? t('etl.schedule.list.disable') : t('etl.schedule.list.enable') }}
            </el-button>
            <el-button link type="primary" @click="handleEdit(row)">
              {{ t('etl.schedule.list.edit') }}
            </el-button>
            <el-button link type="danger" @click="handleDelete(row)">
              {{ t('etl.schedule.list.delete') }}
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

    <!-- Form Dialog -->
    <ScheduleFormDialog
      v-model="dialogVisible"
      :schedule-id="currentScheduleId"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Delete } from '@element-plus/icons-vue'
import {
  getScheduleList,
  deleteSchedule,
  enableSchedule,
  disableSchedule,
  executeScheduleNow,
} from '@/api/etl/scheduleApi'
import { useLocale } from '@/composables/useLocale'
import type { Schedule, ScheduleQueryParams } from '@/types/etl'
import { ScheduleStatus } from '@/types/etl'
import SearchForm from '@/components/SearchForm/index.vue'
import ScheduleFormDialog from './components/ScheduleFormDialog.vue'

const { t } = useLocale()

// 搜索表单配置
const searchItems = computed(() => [
  {
    field: 'name',
    label: t('etl.schedule.list.name'),
    type: 'input',
  },
  {
    field: 'pipelineId',
    label: t('etl.schedule.list.pipelineName'),
    type: 'select',
    options: pipelineOptions.value,
  },
  {
    field: 'status',
    label: t('etl.schedule.list.status'),
    type: 'select',
    options: [
      { label: t('etl.schedule.list.statusEnabled'), value: ScheduleStatus.ENABLED },
      { label: t('etl.schedule.list.statusDisabled'), value: ScheduleStatus.DISABLED },
    ],
  },
])

const loading = ref(false)
const tableData = ref<Schedule[]>([])
const total = ref(0)
const selectedRows = ref<Schedule[]>([])
const pipelineOptions = ref<Array<{ label: string; value: string }>>([])

const dialogVisible = ref(false)
const currentScheduleId = ref<string | undefined>(undefined)

let queryParams = reactive<ScheduleQueryParams>({
  pageIndex: 1,
  pageSize: 10,
  name: undefined,
  pipelineId: undefined,
  status: undefined,
})

onMounted(() => {
  handleSearch()
  loadPipelineOptions()
})

const loadPipelineOptions = async () => {
  // Load pipeline options for filter
  // This would normally call an API to get pipeline list
  pipelineOptions.value = [
    { label: '数据同步任务流', value: 'pipeline-1' },
    { label: '报表生成任务流', value: 'pipeline-2' },
  ]
}

const handleSearch = async () => {
  loading.value = true
  try {
    const { list, total: totalCount } = await getScheduleList(queryParams)
    tableData.value = list
    total.value = totalCount
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.pageIndex = 1
  queryParams.name = undefined
  queryParams.pipelineId = undefined
  queryParams.status = undefined
  handleSearch()
}

const handleSelectionChange = (rows: Schedule[]) => {
  selectedRows.value = rows
}

const handleAdd = () => {
  currentScheduleId.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: Schedule) => {
  currentScheduleId.value = row.id
  dialogVisible.value = true
}

const handleExecuteNow = async (row: Schedule) => {
  try {
    await ElMessageBox.confirm(
      t('etl.schedule.list.executeNowConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'info',
      }
    )
    const result = await executeScheduleNow(row.id)
    ElMessage.success(t('etl.schedule.list.executeNowSuccess', { executionId: result.executionId }))
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleToggleStatus = async (row: Schedule) => {
  try {
    if (row.status === ScheduleStatus.ACTIVE) {
      await disableSchedule(row.id)
      ElMessage.success(t('etl.schedule.list.disableSuccess'))
    } else {
      await enableSchedule(row.id)
      ElMessage.success(t('etl.schedule.list.enableSuccess'))
    }
    handleSearch()
  } catch (error) {
    // Error handled by interceptor
  }
}

const handleDelete = async (row: Schedule) => {
  try {
    await ElMessageBox.confirm(
      t('etl.schedule.list.deleteConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteSchedule(row.id)
    ElMessage.success(t('etl.schedule.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  try {
    await ElMessageBox.confirm(
      t('etl.schedule.list.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    const ids = selectedRows.value.map(row => row.id)
    await deleteSchedule(ids)
    ElMessage.success(t('etl.schedule.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const getStatusTagType = (status: ScheduleStatus | string) => {
  switch (status) {
    case ScheduleStatus.ACTIVE:
    case ScheduleStatus.ENABLED:
    case 'active':
      return 'success'
    case ScheduleStatus.PAUSED:
    case ScheduleStatus.DISABLED:
    case 'paused':
      return 'info'
    case ScheduleStatus.STOPPED:
    case 'stopped':
      return 'warning'
    default:
      return 'info'
  }
}

const getStatusText = (status: ScheduleStatus | string) => {
  switch (status) {
    case ScheduleStatus.ACTIVE:
    case ScheduleStatus.ENABLED:
    case 'active':
      return t('etl.schedule.list.statusEnabled')
    case ScheduleStatus.PAUSED:
    case ScheduleStatus.DISABLED:
    case 'paused':
      return t('etl.schedule.list.statusDisabled')
    case ScheduleStatus.STOPPED:
    case 'stopped':
      return t('etl.schedule.list.statusStopped')
    default:
      return '-'
  }
}
</script>

<style scoped lang="scss">
.schedule-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;

    .header-buttons {
      display: flex;
      gap: 8px;
    }
  }
}
</style>