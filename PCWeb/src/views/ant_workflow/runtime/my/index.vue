<!-- src/views/ant_workflow/runtime/my/index.vue -->
<template>
  <div class="ant-workflow-my">
    <el-card shadow="never">
      <template #header>
        <span>{{ t('antWorkflow.runtimeMy') }}</span>
      </template>

      <!-- 搜索栏 -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('antWorkflow.workflowName')">
          <el-input
            v-model="queryParams.workflowName"
            :placeholder="t('antWorkflow.namePlaceholder')"
            clearable
            style="width: 200px"
          />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.status')">
          <el-select v-model="queryParams.status" :placeholder="t('antWorkflow.statusPlaceholder')" clearable style="width: 120px">
            <el-option :label="t('antWorkflow.all')" value="" />
            <el-option :label="t('antWorkflow.instanceStatusLabels.approving')" :value="1" />
            <el-option :label="t('antWorkflow.instanceStatusLabels.passed')" :value="2" />
            <el-option :label="t('antWorkflow.instanceStatusLabels.rejected')" :value="3" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">{{ t('antWorkflow.search') }}</el-button>
          <el-button @click="handleReset">{{ t('antWorkflow.reset') }}</el-button>
        </el-form-item>
      </el-form>

      <!-- 表格 -->
      <el-table v-loading="loading" :data="tableData" border>
        <el-table-column prop="title" :label="t('antWorkflow.taskTitle')" min-width="200" />
        <el-table-column prop="currentNodeName" :label="t('antWorkflow.currentNode')" width="150" />
        <el-table-column prop="status" :label="t('antWorkflow.status')" width="100">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              {{ getStatusLabel(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="startTime" :label="t('antWorkflow.entryTime')" width="180" />
        <el-table-column prop="finishTime" :label="t('antWorkflow.approveTime')" width="180" />
        <el-table-column :label="t('antWorkflow.operation')" width="150" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleView(row)">{{ t('antWorkflow.view') }}</el-button>
            <el-button v-if="row.status === 1" link type="warning" @click="handleWithdraw(row)">{{ t('antWorkflow.withdraw') }}</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination
        v-model:current-page="queryParams.pageIndex"
        v-model:page-size="queryParams.pageSize"
        :total="total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSearch"
        @current-change="handleSearch"
      />
    </el-card>

    <!-- 详情弹框 -->
    <InstanceDetailDialog
      v-model="detailDialogVisible"
      :instance-id="currentInstanceId"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { AntWorkflowInstanceStatus } from '@/types/antWorkflow/runtime'
import { getMyInstances, cancelInstance } from '@/api/ant_workflow/runtimeApi'
import InstanceDetailDialog from '../components/InstanceDetailDialog.vue'
import type { AntWorkflowInstanceDto } from '@/types/antWorkflow/runtime'

const { t } = useLocale()

const loading = ref(false)
const tableData = ref<AntWorkflowInstanceDto[]>([])
const total = ref(0)

const queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  workflowName: '',
  status: '' as string | number,
})

// 详情弹框
const detailDialogVisible = ref(false)
const currentInstanceId = ref<string | null>(null)

onMounted(() => {
  handleSearch()
})

const handleSearch = async () => {
  loading.value = true
  try {
    const res = await getMyInstances({
      pageIndex: queryParams.pageIndex,
      pageSize: queryParams.pageSize,
      workflowName: queryParams.workflowName || undefined,
      status: queryParams.status !== '' ? Number(queryParams.status) : undefined,
    })
    if (res) {
      tableData.value = res.list || []
      total.value = res.total || 0
    }
  } catch (error) {
    console.error('获取我发起的流程失败', error)
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.workflowName = ''
  queryParams.status = ''
  queryParams.pageIndex = 1
  handleSearch()
}

const getStatusType = (status: number) => {
  switch (status) {
    case AntWorkflowInstanceStatus.APPROVING: return 'warning'
    case AntWorkflowInstanceStatus.PASSED: return 'success'
    case AntWorkflowInstanceStatus.REJECTED: return 'danger'
    case AntWorkflowInstanceStatus.WITHDRAWN: return 'info'
    default: return 'info'
  }
}

const getStatusLabel = (status: number) => {
  switch (status) {
    case AntWorkflowInstanceStatus.WAIT_SUBMIT: return t('antWorkflow.instanceStatusLabels.waitSubmit')
    case AntWorkflowInstanceStatus.APPROVING: return t('antWorkflow.instanceStatusLabels.approving')
    case AntWorkflowInstanceStatus.PASSED: return t('antWorkflow.instanceStatusLabels.passed')
    case AntWorkflowInstanceStatus.REJECTED: return t('antWorkflow.instanceStatusLabels.rejected')
    case AntWorkflowInstanceStatus.WITHDRAWN: return t('antWorkflow.instanceStatusLabels.withdrawn')
    case AntWorkflowInstanceStatus.TERMINATED: return t('antWorkflow.instanceStatusLabels.terminated')
    default: return ''
  }
}

const handleView = (row: AntWorkflowInstanceDto) => {
  currentInstanceId.value = row.id
  detailDialogVisible.value = true
}

const handleWithdraw = async (row: AntWorkflowInstanceDto) => {
  try {
    await ElMessageBox.confirm(
      t('antWorkflow.confirmWithdraw'),
      t('antWorkflow.warning'),
      {
        confirmButtonText: t('antWorkflow.confirm'),
        cancelButtonText: t('antWorkflow.cancel'),
        type: 'warning',
      }
    )

    await cancelInstance(row.id)
    ElMessage.success(t('antWorkflow.messages.runtimeWithdrawSuccess'))
    handleSearch()
  } catch {
    // 用户取消
  }
}
</script>

<style scoped lang="scss">
.ant-workflow-my {
  padding: 20px;

  .search-form {
    margin-bottom: 16px;
  }
}
</style>