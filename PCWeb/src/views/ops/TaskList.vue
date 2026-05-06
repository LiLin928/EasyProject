<!-- PCWeb/src/views/ops/TaskList.vue -->
<template>
  <div class="task-list-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('ops.task.title') }}</span>
          <el-button type="primary" @click="handleCreate">
            <el-icon><Plus /></el-icon>
            {{ t('ops.task.create') }}
          </el-button>
        </div>
      </template>

      <!-- 统计卡片 -->
      <el-row :gutter="16" class="stat-row">
        <el-col :span="6">
          <el-statistic :title="t('ops.task.statistics.totalCount')" :value="statistics.totalCount" />
        </el-col>
        <el-col :span="6">
          <el-statistic :title="t('ops.task.statistics.enabledCount')" :value="statistics.enabledCount" />
        </el-col>
        <el-col :span="6">
          <el-statistic :title="t('ops.task.statistics.pausedCount')" :value="statistics.pausedCount" />
        </el-col>
        <el-col :span="6">
          <el-statistic :title="t('ops.task.statistics.todayExecuted')" :value="statistics.todayExecuted" />
        </el-col>
      </el-row>

      <!-- 搜索栏 -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('ops.task.search.taskName')">
          <el-input v-model="queryParams.taskName" clearable style="width: 200px" placeholder="请输入任务名称" @keyup.enter="handleSearch" />
        </el-form-item>
        <el-form-item :label="t('ops.task.search.taskType')">
          <el-select v-model="queryParams.taskType" clearable style="width: 150px" placeholder="请选择">
            <el-option :label="t('ops.task.type.cron')" :value="0" />
            <el-option :label="t('ops.task.type.periodic')" :value="2" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('ops.task.search.status')">
          <el-select v-model="queryParams.status" clearable style="width: 150px" placeholder="请选择">
            <el-option :label="t('ops.task.status.scheduled')" :value="1" />
            <el-option :label="t('ops.task.status.paused')" :value="2" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">{{ t('common.button.search') }}</el-button>
          <el-button @click="handleReset">{{ t('common.button.reset') }}</el-button>
        </el-form-item>
      </el-form>

      <!-- 表格 -->
      <el-table v-loading="loading" :data="tableData" border>
        <el-table-column prop="taskName" :label="t('ops.task.form.taskName')" min-width="150" />
        <el-table-column prop="taskTypeText" :label="t('ops.task.form.taskType')" width="100" align="center" />
        <el-table-column :label="t('ops.task.schedule')" min-width="150">
          <template #default="{ row }">
            <span v-if="row.taskType === 0">{{ row.cronExpression }}</span>
            <span v-else-if="row.taskType === 2">
              {{ row.scheduleType === 0 ? t('ops.task.scheduleType.daily') : t('ops.task.scheduleType.monthly') }}
              {{ row.executeHour }}:{{ String(row.executeMinute).padStart(2, '0') }}
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="statusText" :label="t('ops.task.search.status')" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">{{ row.statusText }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="nextExecuteTime" :label="t('ops.task.nextExecuteTime')" width="160">
          <template #default="{ row }">
            {{ formatTime(row.nextExecuteTime) }}
          </template>
        </el-table-column>
        <el-table-column :label="t('common.table.operation')" width="200" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)">{{ t('common.button.edit') }}</el-button>
            <el-button v-if="row.status === 1" link type="warning" @click="handlePause(row)">
              {{ t('ops.task.pause') }}
            </el-button>
            <el-button v-if="row.status === 2" link type="success" @click="handleResume(row)">
              {{ t('ops.task.resume') }}
            </el-button>
            <el-button link type="primary" @click="handleTrigger(row)">{{ t('ops.task.trigger') }}</el-button>
            <el-button link type="danger" @click="handleDelete(row)">{{ t('common.button.delete') }}</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination
        v-model:current-page="queryParams.pageIndex"
        v-model:page-size="queryParams.pageSize"
        :total="total"
        :page-sizes="[10, 20, 50]"
        layout="total, sizes, prev, pager, next"
        @size-change="handleSearch"
        @current-change="handleSearch"
      />
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import {
  getTaskList,
  deleteTask,
  pauseTask,
  resumeTask,
  triggerTask,
  getTaskStatistics,
} from '@/api/ops/taskApi'
import { useLocale } from '@/composables/useLocale'
import type { TaskDefinition, QueryTaskParams, TaskStatistics } from '@/types/task'

const router = useRouter()
const { t } = useLocale()

const loading = ref(false)
const tableData = ref<TaskDefinition[]>([])
const total = ref(0)
const statistics = ref<TaskStatistics>({
  totalCount: 0,
  enabledCount: 0,
  pausedCount: 0,
  todayExecuted: 0,
  todaySuccess: 0,
  todayFailure: 0,
})

const queryParams = reactive<QueryTaskParams>({
  pageIndex: 1,
  pageSize: 10,
  taskName: '',
  taskType: undefined,
  status: undefined,
})

onMounted(() => {
  loadStatistics()
  handleSearch()
})

const loadStatistics = async () => {
  try {
    statistics.value = await getTaskStatistics()
  } catch (error) {
    // error handled
  }
}

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getTaskList(queryParams)
    tableData.value = data.list
    total.value = data.total
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.taskName = ''
  queryParams.taskType = undefined
  queryParams.status = undefined
  queryParams.pageIndex = 1
  handleSearch()
}

const handleCreate = () => {
  router.push('/ops/task/edit')
}

const handleEdit = (row: TaskDefinition) => {
  router.push(`/ops/task/edit/${row.id}`)
}

const handlePause = async (row: TaskDefinition) => {
  try {
    await pauseTask(row.id)
    ElMessage.success(t('ops.task.pauseSuccess'))
    loadStatistics()
    handleSearch()
  } catch (error) {
    // error handled
  }
}

const handleResume = async (row: TaskDefinition) => {
  try {
    await resumeTask(row.id)
    ElMessage.success(t('ops.task.resumeSuccess'))
    loadStatistics()
    handleSearch()
  } catch (error) {
    // error handled
  }
}

const handleTrigger = async (row: TaskDefinition) => {
  try {
    await triggerTask(row.id)
    ElMessage.success(t('ops.task.triggerSuccess'))
    loadStatistics()
    handleSearch()  // 刷新列表
  } catch (error) {
    // error handled
  }
}

const handleDelete = async (row: TaskDefinition) => {
  try {
    await ElMessageBox.confirm(t('ops.task.deleteConfirm'), t('common.message.warning'), { type: 'warning' })
    await deleteTask([row.id])
    ElMessage.success(t('common.message.deleteSuccess'))
    loadStatistics()
    handleSearch()
  } catch (error) {
    // cancelled or error
  }
}

const getStatusType = (status: number): 'success' | 'warning' | 'info' | 'danger' => {
  const types: Record<number, 'success' | 'warning' | 'info' | 'danger'> = {
    0: 'info',
    1: 'success',
    2: 'warning',
    3: 'info',
    4: 'danger',
  }
  return types[status] || 'info'
}

const formatTime = (time?: string) => {
  if (!time) return '-'
  return time.replace('T', ' ').substring(0, 19)
}
</script>

<style scoped lang="scss">
.task-list-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .stat-row {
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