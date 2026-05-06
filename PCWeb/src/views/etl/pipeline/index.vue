<!-- src/views/etl/pipeline/index.vue -->
<template>
  <div class="pipeline-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('etl.pipeline.list.title') }}</span>
          <div class="header-buttons">
            <el-button
              type="danger"
              :disabled="selectedRows.length === 0"
              @click="handleBatchDelete"
            >
              <el-icon><Delete /></el-icon>
              {{ t('etl.pipeline.list.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('etl.pipeline.list.add') }}
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
        <el-table-column prop="name" :label="t('etl.pipeline.list.name')" min-width="180" />
        <el-table-column prop="description" :label="t('etl.pipeline.list.description')" min-width="200" show-overflow-tooltip />
        <el-table-column prop="status" :label="t('etl.pipeline.list.status')" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusTagType(row.status)">
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="scheduleCount" :label="t('etl.pipeline.list.scheduleCount')" width="100" align="center">
          <template #default="{ row }">
            <el-tag type="info" effect="plain">
              {{ row.scheduleCount || 0 }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="creatorName" :label="t('etl.pipeline.list.creator')" width="120" />
        <el-table-column prop="createTime" :label="t('etl.pipeline.list.createTime')" width="160" />
        <el-table-column :label="t('etl.pipeline.list.operation')" width="280" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDesign(row)">
              {{ t('etl.pipeline.list.design') }}
            </el-button>
            <el-button link type="success" @click="handleExecute(row)">
              {{ t('etl.pipeline.list.execute') }}
            </el-button>
            <el-button link type="primary" @click="handleEdit(row)">
              {{ t('etl.pipeline.list.edit') }}
            </el-button>
            <el-button link type="primary" @click="handleCopy(row)">
              {{ t('etl.pipeline.list.copy') }}
            </el-button>
            <el-button link type="danger" @click="handleDelete(row)">
              {{ t('etl.pipeline.list.delete') }}
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
    <PipelineFormDialog
      v-model="dialogVisible"
      :pipeline-id="currentPipelineId"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Delete } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'
import {
  getPipelineList,
  deletePipeline,
  copyPipeline,
  executePipeline,
} from '@/api/etl/pipelineApi'
import { useLocale } from '@/composables/useLocale'
import type { Pipeline, PipelineQueryParams } from '@/types/etl'
import { PipelineStatus } from '@/types/etl'
import SearchForm from '@/components/SearchForm/index.vue'
import PipelineFormDialog from './components/PipelineFormDialog.vue'

const router = useRouter()
const { t } = useLocale()

// 搜索表单配置
const searchItems = computed(() => [
  {
    field: 'name',
    label: t('etl.pipeline.list.name'),
    type: 'input',
  },
  {
    field: 'status',
    label: t('etl.pipeline.list.status'),
    type: 'select',
    options: [
      { label: t('etl.pipeline.list.statusDraft'), value: PipelineStatus.DRAFT },
      { label: t('etl.pipeline.list.statusPublished'), value: PipelineStatus.PUBLISHED },
      { label: t('etl.pipeline.list.statusArchived'), value: PipelineStatus.ARCHIVED },
    ],
  },
])

const loading = ref(false)
const tableData = ref<Pipeline[]>([])
const total = ref(0)
const selectedRows = ref<Pipeline[]>([])

const dialogVisible = ref(false)
const currentPipelineId = ref<string | undefined>(undefined)

let queryParams = reactive<PipelineQueryParams>({
  pageIndex: 1,
  pageSize: 10,
  name: undefined,
  status: undefined,
})

onMounted(() => {
  handleSearch()
})

const handleSearch = async () => {
  loading.value = true
  try {
    const { list, total: totalCount } = await getPipelineList(queryParams)
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
  queryParams.status = undefined
  handleSearch()
}

const handleSelectionChange = (rows: Pipeline[]) => {
  selectedRows.value = rows
}

const handleAdd = () => {
  currentPipelineId.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: Pipeline) => {
  currentPipelineId.value = row.id
  dialogVisible.value = true
}

const handleDesign = (row: Pipeline) => {
  router.push(`/etl/design?id=${row.id}`)
}

const handleExecute = async (row: Pipeline) => {
  try {
    await ElMessageBox.confirm(
      t('etl.pipeline.list.executeConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'info',
      }
    )
    // executePipeline 返回的是 GUID 字符串，不是对象
    const executionId = await executePipeline(row.id)
    ElMessage.success(`执行已触发，执行ID: ${executionId}`)
    router.push(`/etl/monitor?executionId=${executionId}`)
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleCopy = async (row: Pipeline) => {
  try {
    const { value } = await ElMessageBox.prompt(
      t('etl.pipeline.list.copyNamePlaceholder'),
      t('etl.pipeline.list.copyTitle'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        inputValue: `${row.name} (副本)`,
      }
    )
    await copyPipeline(row.id, value)
    ElMessage.success(t('etl.pipeline.list.copySuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleDelete = async (row: Pipeline) => {
  try {
    await ElMessageBox.confirm(
      t('etl.pipeline.list.deleteConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deletePipeline(row.id)
    ElMessage.success(t('etl.pipeline.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  try {
    await ElMessageBox.confirm(
      t('etl.pipeline.list.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    const ids = selectedRows.value.map(row => row.id)
    await deletePipeline(ids)
    ElMessage.success(t('etl.pipeline.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const getStatusTagType = (status: PipelineStatus | string) => {
  switch (status) {
    case PipelineStatus.DRAFT:
    case 'draft':
      return 'info'
    case PipelineStatus.PUBLISHED:
    case 'published':
      return 'success'
    case PipelineStatus.ARCHIVED:
    case 'archived':
      return 'warning'
    default:
      return 'info'
  }
}

const getStatusText = (status: PipelineStatus | string) => {
  switch (status) {
    case PipelineStatus.DRAFT:
    case 'draft':
      return t('etl.pipeline.list.statusDraft')
    case PipelineStatus.PUBLISHED:
    case 'published':
      return t('etl.pipeline.list.statusPublished')
    case PipelineStatus.ARCHIVED:
    case 'archived':
      return t('etl.pipeline.list.statusArchived')
    default:
      return '-'
  }
}
</script>

<style scoped lang="scss">
.pipeline-container {
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