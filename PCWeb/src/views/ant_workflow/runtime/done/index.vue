<!-- src/views/ant_workflow/runtime/done/index.vue -->
<template>
  <div class="ant-workflow-done">
    <el-card shadow="never">
      <template #header>
        <span>{{ t('antWorkflow.runtimeDone') }}</span>
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
        <el-form-item>
          <el-button type="primary" @click="handleSearch">{{ t('antWorkflow.search') }}</el-button>
          <el-button @click="handleReset">{{ t('antWorkflow.reset') }}</el-button>
        </el-form-item>
      </el-form>

      <!-- 表格 -->
      <el-table v-loading="loading" :data="tableData" border>
        <el-table-column prop="instanceTitle" :label="t('antWorkflow.taskTitle')" min-width="200" />
        <el-table-column prop="nodeName" :label="t('antWorkflow.nodeName')" width="150" />
        <el-table-column prop="entryTime" :label="t('antWorkflow.approveTime')" width="180" />
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
import { useLocale } from '@/composables/useLocale'
import { getDoneTasks } from '@/api/ant_workflow/taskApi'
import InstanceDetailDialog from '../components/InstanceDetailDialog.vue'
import type { AntWorkflowTaskDto } from '@/types/antWorkflow/runtime'

const { t } = useLocale()

const loading = ref(false)
const tableData = ref<AntWorkflowTaskDto[]>([])
const total = ref(0)

const queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  workflowName: '',
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
    const res = await getDoneTasks({
      pageIndex: queryParams.pageIndex,
      pageSize: queryParams.pageSize,
      title: queryParams.workflowName,
    })
    if (res) {
      tableData.value = res.list || []
      total.value = res.total || 0
    }
  } catch (error) {
    console.error('获取已办任务失败', error)
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.workflowName = ''
  queryParams.pageIndex = 1
  handleSearch()
}

const handleView = (row: AntWorkflowTaskDto) => {
  currentInstanceId.value = row.instanceId
  detailDialogVisible.value = true
}
</script>

<style scoped lang="scss">
.ant-workflow-done {
  padding: 20px;

  .search-form {
    margin-bottom: 16px;
  }
}
</style>