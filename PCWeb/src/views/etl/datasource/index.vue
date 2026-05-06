<!-- src/views/etl/datasource/index.vue -->
<template>
  <div class="datasource-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('etl.datasource.list.title') }}</span>
          <div class="header-buttons">
            <el-button
              type="danger"
              :disabled="selectedRows.length === 0"
              @click="handleBatchDelete"
            >
              <el-icon><Delete /></el-icon>
              {{ t('etl.datasource.list.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('etl.datasource.list.add') }}
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
        <el-table-column prop="name" :label="t('etl.datasource.list.name')" min-width="150" />
        <el-table-column prop="type" :label="t('etl.datasource.list.type')" width="120" align="center">
          <template #default="{ row }">
            <el-tag type="primary" effect="plain">
              {{ getTypeLabel(row.type) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="host" :label="t('etl.datasource.list.host')" min-width="150">
          <template #default="{ row }">
            {{ row.host }}:{{ row.port }}/{{ row.database }}
          </template>
        </el-table-column>
        <el-table-column prop="status" :label="t('etl.datasource.list.status')" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="getStatusTagType(row.status)">
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createTime" :label="t('etl.datasource.list.createTime')" width="160" />
        <el-table-column :label="t('etl.datasource.list.operation')" width="200" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleTestConnection(row)">
              {{ t('etl.datasource.list.testConnection') }}
            </el-button>
            <el-button link type="primary" @click="handleEdit(row)">
              {{ t('etl.datasource.list.edit') }}
            </el-button>
            <el-button link type="danger" @click="handleDelete(row)">
              {{ t('etl.datasource.list.delete') }}
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
    <DatasourceFormDialog
      v-model="dialogVisible"
      :datasource-id="currentDatasourceId"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Delete } from '@element-plus/icons-vue'
import {
  getDatasourceList,
  deleteDatasource,
  testDatasourceConnection,
} from '@/api/etl/datasourceApi'
import { useLocale } from '@/composables/useLocale'
import type { DataSource, DataSourceListParams } from '@/types/etl'
import SearchForm from '@/components/SearchForm/index.vue'
import DatasourceFormDialog from './components/DatasourceFormDialog.vue'

const { t } = useLocale()

// 搜索表单配置
const searchItems = computed(() => [
  {
    field: 'name',
    label: t('etl.datasource.list.name'),
    type: 'input',
  },
  {
    field: 'type',
    label: t('etl.datasource.list.type'),
    type: 'select',
    options: [
      { label: 'MySQL', value: 'mysql' },
      { label: 'PostgreSQL', value: 'postgresql' },
      { label: 'Oracle', value: 'oracle' },
      { label: 'SQL Server', value: 'sqlserver' },
      { label: 'ClickHouse', value: 'clickhouse' },
    ],
  },
  {
    field: 'status',
    label: t('etl.datasource.list.status'),
    type: 'select',
    options: [
      { label: t('etl.datasource.list.statusActive'), value: 'connected' },
      { label: t('etl.datasource.list.statusInactive'), value: 'disconnected' },
      { label: t('etl.datasource.list.statusError'), value: 'error' },
    ],
  },
])

const loading = ref(false)
const tableData = ref<DataSource[]>([])
const total = ref(0)
const selectedRows = ref<DataSource[]>([])

const dialogVisible = ref(false)
const currentDatasourceId = ref<string | undefined>(undefined)

let queryParams = reactive<DataSourceListParams>({
  pageIndex: 1,
  pageSize: 10,
  name: undefined,
  type: undefined,
  status: undefined,
})

onMounted(() => {
  handleSearch()
})

const handleSearch = async () => {
  loading.value = true
  try {
    const { list, total: totalCount } = await getDatasourceList(queryParams)
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
  queryParams.type = undefined
  queryParams.status = undefined
  handleSearch()
}

const handleSelectionChange = (rows: DataSource[]) => {
  selectedRows.value = rows
}

const handleAdd = () => {
  currentDatasourceId.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: DataSource) => {
  currentDatasourceId.value = row.id
  dialogVisible.value = true
}

const handleDelete = async (row: DataSource) => {
  try {
    await ElMessageBox.confirm(
      t('etl.datasource.list.deleteConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteDatasource(row.id)
    ElMessage.success(t('etl.datasource.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  try {
    await ElMessageBox.confirm(
      t('etl.datasource.list.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    const ids = selectedRows.value.map(row => row.id)
    await deleteDatasource(ids)
    ElMessage.success(t('etl.datasource.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleTestConnection = async (row: DataSource) => {
  loading.value = true
  try {
    const result = await testDatasourceConnection({ id: row.id })
    if (result.success) {
      ElMessage.success(t('etl.datasource.list.testSuccess'))
      // 更新状态
      row.status = 'connected'
    } else {
      ElMessage.error(result.message || t('etl.datasource.list.testFailed'))
      row.status = 'error'
    }
  } catch (error) {
    ElMessage.error(t('etl.datasource.list.testFailed'))
    row.status = 'error'
  } finally {
    loading.value = false
  }
}

const getTypeLabel = (type: string) => {
  const labels: Record<string, string> = {
    mysql: 'MySQL',
    postgresql: 'PostgreSQL',
    oracle: 'Oracle',
    sqlserver: 'SQL Server',
    clickhouse: 'ClickHouse',
  }
  return labels[type] || type
}

const getStatusTagType = (status: string) => {
  switch (status) {
    case 'connected':
      return 'success'
    case 'disconnected':
      return 'info'
    case 'error':
      return 'danger'
    default:
      return 'info'
  }
}

const getStatusText = (status: string) => {
  switch (status) {
    case 'connected':
      return t('etl.datasource.list.statusActive')
    case 'disconnected':
      return t('etl.datasource.list.statusInactive')
    case 'error':
      return t('etl.datasource.list.statusError')
    default:
      return '-'
  }
}
</script>

<style scoped lang="scss">
.datasource-container {
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