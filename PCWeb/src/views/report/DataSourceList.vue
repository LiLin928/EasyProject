<!-- src/views/report/DataSourceList.vue -->
<template>
  <div class="datasource-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('datasource.list.title') }}</span>
          <div class="header-buttons">
            <el-button @click="handleTestAll" :loading="testingAll">
              <el-icon><Connection /></el-icon>
              {{ t('datasource.list.testAll') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('datasource.list.add') }}
            </el-button>
          </div>
        </div>
      </template>

      <!-- Search form -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('datasource.list.name')">
          <el-input
            v-model="queryParams.name"
            :placeholder="t('datasource.list.namePlaceholder')"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('datasource.list.dbType')">
          <el-select
            v-model="queryParams.type"
            :placeholder="t('datasource.list.dbTypePlaceholder')"
            clearable
            style="width: 150px"
          >
            <el-option
              v-for="db in dbTypes"
              :key="db.value"
              :label="db.label"
              :value="db.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            {{ t('datasource.list.search') }}
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            {{ t('common.button.reset') }}
          </el-button>
        </el-form-item>
      </el-form>

      <!-- Table -->
      <BaseTable
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
        <!-- Status column -->
        <template #status="{ row }">
          <el-tag :type="row.status === 'connected' ? 'success' : 'danger'" size="small">
            {{ row.status === 'connected' ? t('datasource.list.statusNormal') : t('datasource.list.statusError') }}
          </el-tag>
        </template>

        <!-- Address column -->
        <template #address="{ row }">
          {{ row.host }}:{{ row.port }}/{{ row.database }}
        </template>

        <!-- Operation column -->
        <template #operation>
          <el-table-column :label="t('datasource.list.operation')" width="200" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleTest(row)">
                {{ t('datasource.list.testConnection') }}
              </el-button>
              <el-button link type="primary" @click="handleEdit(row)">
                {{ t('datasource.list.edit') }}
              </el-button>
              <el-button link type="danger" @click="handleDelete(row)">
                {{ t('datasource.list.delete') }}
              </el-button>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>

    <!-- Form Dialog -->
    <DataSourceFormDialog
      v-model="dialogVisible"
      :data-source-id="currentDataSourceId"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search, Refresh, Connection } from '@element-plus/icons-vue'
import {
  getDataSourceList,
  deleteDataSource,
  getDbTypes,
  testConnectionById,
  testAllConnections,
} from '@/api/report/datasourceApi'
import { useLocale } from '@/composables/useLocale'
import type { DataSource, DbTypeInfo } from '@/types'
import type { TableColumn } from '@/components/BaseTable/index.vue'
import BaseTable from '@/components/BaseTable/index.vue'
import DataSourceFormDialog from './components/DataSourceFormDialog.vue'

const { t } = useLocale()

// Table columns
const tableColumns = ref<TableColumn[]>([
  { prop: 'name', label: t('datasource.list.name'), minWidth: 150 },
  { prop: 'type', label: t('datasource.list.dbType'), width: 120 },
  { prop: 'address', label: t('datasource.list.address'), minWidth: 200 },
  { prop: 'status', label: t('datasource.list.status'), width: 100, align: 'center' },
  { prop: 'reportCount', label: t('datasource.list.reportCount'), width: 100, align: 'center' },
  { prop: 'lastCheckTime', label: t('datasource.list.lastCheckTime'), width: 160 },
])

const loading = ref(false)
const testingAll = ref(false)
const tableData = ref<DataSource[]>([])
const total = ref(0)
const dbTypes = ref<DbTypeInfo[]>([])

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  name: '',
  type: '',
})

const dialogVisible = ref(false)
const currentDataSourceId = ref<string | undefined>(undefined)

onMounted(() => {
  loadDbTypes()
  handleSearch()
})

const loadDbTypes = async () => {
  try {
    const data = await getDbTypes()
    dbTypes.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getDataSourceList(queryParams)
    tableData.value = data.list
    total.value = data.total
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.name = ''
  queryParams.type = ''
  queryParams.pageIndex = 1
  handleSearch()
}

const handleAdd = () => {
  currentDataSourceId.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: DataSource) => {
  currentDataSourceId.value = row.id
  dialogVisible.value = true
}

const handleTest = async (row: DataSource) => {
  try {
    const result = await testConnectionById(row.id)
    if (result.success) {
      ElMessage.success(t('datasource.list.testSuccess'))
      // 更新该行的状态
      row.status = 'connected'
      row.lastConnectionTime = new Date().toISOString()
    } else {
      ElMessage.error(result.message || t('datasource.list.testFailed'))
      // 更新该行的状态
      row.status = 'error'
    }
  } catch (error) {
    ElMessage.error(t('datasource.list.testFailed'))
    row.status = 'error'
  }
}

const handleTestAll = async () => {
  testingAll.value = true
  try {
    await testAllConnections()
    ElMessage.success(t('datasource.list.testAllSuccess'))
    handleSearch() // 刷新列表以更新状态
  } catch (error) {
    // Error handled by interceptor
  } finally {
    testingAll.value = false
  }
}

const handleDelete = async (row: DataSource) => {
  if (row.reportCount > 0) {
    ElMessage.warning(t('datasource.list.cannotDelete', { count: row.reportCount }))
    return
  }

  try {
    await ElMessageBox.confirm(
      t('datasource.list.deleteConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteDataSource(row.id)
    ElMessage.success(t('datasource.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
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

  .search-form {
    margin-bottom: 16px;
  }
}
</style>