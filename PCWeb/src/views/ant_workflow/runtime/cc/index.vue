<!-- src/views/ant_workflow/runtime/cc/index.vue -->
<template>
  <div class="ant-workflow-cc">
    <el-card shadow="never">
      <template #header>
        <span>{{ t('antWorkflow.runtimeCc') }}</span>
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
        <el-form-item :label="t('antWorkflow.isRead')">
          <el-select v-model="queryParams.isRead" :placeholder="t('antWorkflow.selectReadStatus')" clearable style="width: 120px">
            <el-option :label="t('antWorkflow.readStatus.all')" value="" />
            <el-option :label="t('antWorkflow.readStatus.unread')" :value="0" />
            <el-option :label="t('antWorkflow.readStatus.read')" :value="1" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">{{ t('antWorkflow.search') }}</el-button>
          <el-button @click="handleReset">{{ t('antWorkflow.reset') }}</el-button>
          <el-button type="success" @click="handleBatchMarkRead">{{ t('antWorkflow.batchMarkRead') }}</el-button>
        </el-form-item>
      </el-form>

      <!-- 表格 -->
      <el-table
        v-loading="loading"
        :data="tableData"
        border
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="instanceTitle" :label="t('antWorkflow.taskTitle')" min-width="200" />
        <el-table-column prop="fromUserName" :label="t('antWorkflow.initiator')" width="120" />
        <el-table-column prop="sendTime" :label="t('antWorkflow.ccTime')" width="180" />
        <el-table-column prop="isRead" :label="t('antWorkflow.isRead')" width="100">
          <template #default="{ row }">
            <el-tag :type="row.isRead === 1 ? 'success' : 'warning'">
              {{ row.isRead === 1 ? t('antWorkflow.readStatus.read') : t('antWorkflow.readStatus.unread') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column :label="t('antWorkflow.operation')" width="100" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleView(row)">{{ t('antWorkflow.view') }}</el-button>
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
import { getCcTasks, markCcRead } from '@/api/ant_workflow/taskApi'
import InstanceDetailDialog from '../components/InstanceDetailDialog.vue'
import type { AntWorkflowCcDto } from '@/types/antWorkflow/runtime'

const { t } = useLocale()

const loading = ref(false)
const tableData = ref<AntWorkflowCcDto[]>([])
const total = ref(0)
const selectedRows = ref<AntWorkflowCcDto[]>([])

const queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  workflowName: '',
  isRead: '' as string | number,
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
    const res = await getCcTasks({
      pageIndex: queryParams.pageIndex,
      pageSize: queryParams.pageSize,
      workflowName: queryParams.workflowName || undefined,
      isRead: queryParams.isRead !== '' ? Boolean(queryParams.isRead) : undefined,
    })
    if (res) {
      tableData.value = res.list || []
      total.value = res.total || 0
    }
  } catch (error) {
    console.error('获取抄送列表失败', error)
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.workflowName = ''
  queryParams.isRead = ''
  queryParams.pageIndex = 1
  handleSearch()
}

const handleSelectionChange = (rows: AntWorkflowCcDto[]) => {
  selectedRows.value = rows
}

const handleBatchMarkRead = () => {
  if (selectedRows.value.length === 0) {
    ElMessage.warning(t('antWorkflow.messages.selectReadRecords'))
    return
  }

  ElMessageBox.confirm(
    t('antWorkflow.confirmBatchRead', { count: selectedRows.value.length }),
    t('antWorkflow.warning'),
    {
      confirmButtonText: t('antWorkflow.confirm'),
      cancelButtonText: t('antWorkflow.cancel'),
      type: 'warning',
    }
  ).then(async () => {
    try {
      await markCcRead(selectedRows.value.map(r => r.id))
      ElMessage.success(t('antWorkflow.messages.markReadSuccess'))
      handleSearch()
    } catch (error) {
      ElMessage.error(t('antWorkflow.messages.markFailed'))
    }
  }).catch(() => {})
}

const handleView = (row: AntWorkflowCcDto) => {
  currentInstanceId.value = row.instanceId
  detailDialogVisible.value = true
}
</script>

<style scoped lang="scss">
.ant-workflow-cc {
  padding: 20px;

  .search-form {
    margin-bottom: 16px;
  }
}
</style>