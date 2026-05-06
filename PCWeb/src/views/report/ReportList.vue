<!-- src/views/report/ReportList.vue -->
<template>
  <div class="report-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('report.list.title') }}</span>
          <div class="header-buttons">
            <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleBatchDelete">
              {{ t('report.list.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('report.list.add') }}
            </el-button>
          </div>
        </div>
      </template>

      <!-- Search form -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('report.list.name')">
          <el-input
            v-model="queryParams.name"
            :placeholder="t('report.list.namePlaceholder')"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('report.list.category')">
          <el-select
            v-model="queryParams.category"
            :placeholder="t('report.list.categoryPlaceholder')"
            clearable
            style="width: 150px"
          >
            <el-option
              v-for="cat in categories"
              :key="cat.id"
              :label="cat.name"
              :value="cat.name"
            />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            {{ t('report.list.search') }}
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
        <!-- Operation column -->
        <template #operation>
          <el-table-column :label="t('report.list.operation')" width="260" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handlePreview(row)">
                {{ t('report.list.preview') }}
              </el-button>
              <el-button link type="success" @click="handlePublish(row)">
                {{ t('report.list.publish') }}
              </el-button>
              <el-button link type="primary" @click="handleEdit(row)">
                {{ t('report.list.edit') }}
              </el-button>
              <el-button link type="danger" @click="handleDelete(row)">
                {{ t('report.list.delete') }}
              </el-button>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>

    <!-- Publish Link Dialog -->
    <el-dialog
      v-model="publishDialogVisible"
      :title="t('report.list.publishDialogTitle')"
      width="500px"
    >
      <div class="publish-dialog-content">
        <div class="publish-info">
          <span class="label">{{ t('report.list.reportName') }}:</span>
          <span class="value">{{ currentReport?.name }}</span>
        </div>
        <div class="publish-link-section">
          <div class="label">{{ t('report.list.publishLink') }}:</div>
          <el-input v-model="publishLink" readonly>
            <template #append>
              <el-button @click="copyPublishLink">
                <el-icon><CopyDocument /></el-icon>
                {{ t('report.list.copyLink') }}
              </el-button>
            </template>
          </el-input>
        </div>
        <div class="publish-tip">
          {{ t('report.list.publishTip') }}
        </div>
      </div>
      <template #footer>
        <el-button @click="publishDialogVisible = false">
          {{ t('common.button.close') }}
        </el-button>
        <el-button type="primary" @click="openPublishLink">
          {{ t('report.list.openLink') }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Search, Refresh, CopyDocument } from '@element-plus/icons-vue'
import { getReportList, deleteReport, deleteReportBatch, getReportCategories } from '@/api/report/reportApi'
import { useLocale } from '@/composables/useLocale'
import type { Report, ReportCategory } from '@/types'
import type { TableColumn } from '@/components/BaseTable/index.vue'
import BaseTable from '@/components/BaseTable/index.vue'

const router = useRouter()
const { t } = useLocale()

// Publish dialog
const publishDialogVisible = ref(false)
const currentReport = ref<Report | null>(null)
const publishLink = computed(() => {
  if (!currentReport.value) return ''
  const baseUrl = window.location.origin
  return `${baseUrl}/report/publish/${currentReport.value.id}`
})

// Table columns
const tableColumns = ref<TableColumn[]>([
  { prop: 'name', label: t('report.list.name'), minWidth: 150 },
  { prop: 'category', label: t('report.list.category'), width: 120 },
  { prop: 'creator', label: t('report.list.creator'), width: 100 },
  { prop: 'updateTime', label: t('report.list.updateTime'), width: 160 },
])

const loading = ref(false)
const tableData = ref<Report[]>([])
const selectedRows = ref<Report[]>([])
const total = ref(0)
const categories = ref<ReportCategory[]>([])
const baseTableRef = ref()

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  name: '',
  category: '',
})

onMounted(() => {
  loadCategories()
  handleSearch()
})

const loadCategories = async () => {
  try {
    const data = await getReportCategories()
    categories.value = data
  } catch (error) {
    // Error handled by interceptor
  }
}

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getReportList(queryParams)
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
  queryParams.category = ''
  queryParams.pageIndex = 1
  handleSearch()
}

const handleAdd = () => {
  router.push('/report/edit')
}

const handleEdit = (row: Report) => {
  router.push(`/report/edit/${row.id}`)
}

const handlePreview = (row: Report) => {
  router.push(`/report/detail/${row.id}`)
}

const handleDelete = async (row: Report) => {
  try {
    await ElMessageBox.confirm(
      t('report.list.deleteConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteReport(row.id)
    ElMessage.success(t('report.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleSelectionChange = (rows: Report[]) => {
  selectedRows.value = rows
}

const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  try {
    await ElMessageBox.confirm(
      t('report.list.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    const ids = selectedRows.value.map(row => row.id)
    await deleteReportBatch(ids)
    ElMessage.success(t('report.list.deleteSuccess'))
    baseTableRef.value?.clearSelection()
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

// Publish handlers
const handlePublish = (row: Report) => {
  currentReport.value = row
  publishDialogVisible.value = true
}

const copyPublishLink = async () => {
  try {
    await navigator.clipboard.writeText(publishLink.value)
    ElMessage.success(t('report.list.copySuccess'))
  } catch (error) {
    // Fallback for older browsers
    const textArea = document.createElement('textarea')
    textArea.value = publishLink.value
    document.body.appendChild(textArea)
    textArea.select()
    document.execCommand('copy')
    document.body.removeChild(textArea)
    ElMessage.success(t('report.list.copySuccess'))
  }
}

const openPublishLink = () => {
  window.open(publishLink.value, '_blank')
}
</script>

<style scoped lang="scss">
.report-container {
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

  .publish-dialog-content {
    .publish-info {
      margin-bottom: 16px;
      display: flex;
      gap: 8px;

      .label {
        color: #606266;
      }

      .value {
        font-weight: bold;
      }
    }

    .publish-link-section {
      margin-bottom: 16px;

      .label {
        color: #606266;
        margin-bottom: 8px;
      }
    }

    .publish-tip {
      font-size: 12px;
      color: #909399;
      line-height: 1.5;
    }
  }
}
</style>