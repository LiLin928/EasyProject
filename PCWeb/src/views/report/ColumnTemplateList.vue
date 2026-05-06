<!-- src/views/report/ColumnTemplateList.vue -->
<template>
  <div class="column-template-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('columnTemplate.list.title') }}</span>
          <div>
            <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleBatchDelete">
              {{ t('columnTemplate.list.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('columnTemplate.list.add') }}
            </el-button>
          </div>
        </div>
      </template>

      <!-- Search form -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('columnTemplate.list.name')">
          <el-input
            v-model="queryParams.name"
            :placeholder="t('columnTemplate.list.namePlaceholder')"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('columnTemplate.list.type')">
          <el-select
            v-model="queryParams.type"
            :placeholder="t('columnTemplate.list.typePlaceholder')"
            clearable
            style="width: 150px"
          >
            <el-option
              value="single"
              :label="t('columnTemplate.list.typeSingle')"
            />
            <el-option
              value="table"
              :label="t('columnTemplate.list.typeTable')"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            {{ t('columnTemplate.list.search') }}
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            {{ t('common.button.reset') }}
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
        :selection="true"
        :page-index="queryParams.pageIndex"
        :page-size="queryParams.pageSize"
        @update:page-index="queryParams.pageIndex = $event"
        @update:page-size="queryParams.pageSize = $event"
        @page-change="handleSearch"
        @selection-change="handleSelectionChange"
      >
        <!-- Type column -->
        <template #type="{ row }">
          <el-tag :type="row.type === 'single' ? 'primary' : 'success'" size="small">
            {{ row.type === 'single' ? t('columnTemplate.list.typeSingle') : t('columnTemplate.list.typeTable') }}
          </el-tag>
        </template>

        <!-- Columns count column -->
        <template #columns="{ row }">
          {{ row.columns?.length || 0 }}
        </template>

        <!-- Operation column -->
        <template #operation>
          <el-table-column :label="t('columnTemplate.list.operation')" width="150" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleEdit(row)">
                {{ t('columnTemplate.list.edit') }}
              </el-button>
              <el-button link type="danger" @click="handleDelete(row)">
                {{ t('columnTemplate.list.delete') }}
              </el-button>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search, Refresh } from '@element-plus/icons-vue'
import { getColumnTemplateList, deleteColumnTemplate, deleteColumnTemplateBatch } from '@/api/report/columnTemplateApi'
import { useLocale } from '@/composables/useLocale'
import type { ColumnTemplate } from '@/types'
import type { TableColumn } from '@/components/BaseTable/index.vue'
import BaseTable from '@/components/BaseTable/index.vue'

const router = useRouter()
const { t } = useLocale()

// Table columns
const tableColumns = ref<TableColumn[]>([
  { prop: 'name', label: t('columnTemplate.list.name'), minWidth: 150 },
  { prop: 'type', label: t('columnTemplate.list.type'), width: 120, align: 'center' },
  { prop: 'description', label: t('columnTemplate.list.description'), minWidth: 200 },
  { prop: 'columns', label: t('columnTemplate.list.columns'), width: 100, align: 'center' },
  { prop: 'createTime', label: t('columnTemplate.list.createTime'), width: 160 },
])

const loading = ref(false)
const tableData = ref<ColumnTemplate[]>([])
const selectedRows = ref<ColumnTemplate[]>([])
const total = ref(0)
const baseTableRef = ref()

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  name: '',
  type: '',
})

onMounted(() => {
  handleSearch()
})

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getColumnTemplateList(queryParams)
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
  router.push('/report/column-template/create')
}

const handleEdit = (row: ColumnTemplate) => {
  router.push(`/report/column-template/edit/${row.id}`)
}

const handleDelete = async (row: ColumnTemplate) => {
  try {
    await ElMessageBox.confirm(
      t('columnTemplate.list.deleteConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteColumnTemplate(row.id)
    ElMessage.success(t('columnTemplate.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleSelectionChange = (rows: ColumnTemplate[]) => {
  selectedRows.value = rows
}

const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  try {
    await ElMessageBox.confirm(
      t('columnTemplate.list.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    const ids = selectedRows.value.map(row => row.id)
    await deleteColumnTemplateBatch(ids)
    ElMessage.success(t('columnTemplate.list.deleteSuccess'))
    baseTableRef.value?.clearSelection()
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}
</script>

<style scoped lang="scss">
.column-template-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .search-form {
    margin-bottom: 16px;
  }
}
</style>