<!-- src/views/workflow/businessAuditPoint/index.vue -->
<template>
  <div class="business-audit-point-list">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('antWorkflow.businessAuditPoint.list') }}</span>
          <div class="header-buttons">
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon>
              {{ t('antWorkflow.businessAuditPoint.create') }}
            </el-button>
          </div>
        </div>
      </template>

      <!-- 搜索栏 -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('antWorkflow.businessAuditPoint.code')">
          <el-input
            v-model="queryParams.code"
            :placeholder="t('antWorkflow.businessAuditPoint.codePlaceholder')"
            clearable
            style="width: 150px"
          />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.businessAuditPoint.name')">
          <el-input
            v-model="queryParams.name"
            :placeholder="t('antWorkflow.businessAuditPoint.namePlaceholder')"
            clearable
            style="width: 150px"
          />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.businessAuditPoint.tableName')">
          <el-input
            v-model="queryParams.tableName"
            :placeholder="t('antWorkflow.businessAuditPoint.tableNamePlaceholder')"
            clearable
            style="width: 150px"
          />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.businessAuditPoint.status')">
          <el-select
            v-model="queryParams.status"
            :placeholder="t('antWorkflow.businessAuditPoint.statusPlaceholder')"
            clearable
            style="width: 120px"
          >
            <el-option :label="t('antWorkflow.businessAuditPoint.enabled')" :value="1" />
            <el-option :label="t('antWorkflow.businessAuditPoint.disabled')" :value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            {{ t('antWorkflow.businessAuditPoint.search') }}
          </el-button>
          <el-button @click="handleReset">
            {{ t('antWorkflow.businessAuditPoint.reset') }}
          </el-button>
        </el-form-item>
      </el-form>

      <!-- 表格 -->
      <BaseTable
        ref="baseTableRef"
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
        <!-- 分类列 -->
        <template #category="{ row }">
          <el-tag>{{ row.category }}</el-tag>
        </template>

        <!-- 关联流程列 -->
        <template #workflowName="{ row }">
          <span v-if="row.workflowName">{{ row.workflowName }}</span>
          <span v-else class="text-muted">-</span>
        </template>

        <!-- 状态列 -->
        <template #status="{ row }">
          <el-tag :type="row.status === 1 ? 'success' : 'danger'">
            {{ row.status === 1 ? t('antWorkflow.businessAuditPoint.enabled') : t('antWorkflow.businessAuditPoint.disabled') }}
          </el-tag>
        </template>

        <!-- 操作列 -->
        <template #operation>
          <el-table-column :label="t('antWorkflow.businessAuditPoint.operation')" width="150" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleEdit(row)">
                {{ t('antWorkflow.businessAuditPoint.edit') }}
              </el-button>
              <el-button link type="danger" @click="handleDelete(row)">
                {{ t('antWorkflow.businessAuditPoint.delete') }}
              </el-button>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>

    <!-- 编辑弹窗 -->
    <EditDialog
      v-model="dialogVisible"
      :id="currentId"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
// 1. Vue 核心
import { ref, reactive, onMounted, computed } from 'vue'

// 2. 第三方库
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'

// 3. 组件
import BaseTable from '@/components/BaseTable/index.vue'
import EditDialog from './components/EditDialog.vue'

// 4. API
import { getBusinessAuditPointList, deleteBusinessAuditPoint } from '@/api/ant_workflow/businessAuditPointApi'

// 5. Composables
import { useLocale } from '@/composables/useLocale'

// 6. 类型
import type { BusinessAuditPoint, BusinessAuditPointQueryParams } from '@/types/businessAuditPoint'
import type { TableColumn } from '@/components/BaseTable/index.vue'

const { t } = useLocale()

// 表格列配置
const tableColumns = computed<TableColumn[]>(() => [
  { prop: 'code', label: t('antWorkflow.businessAuditPoint.code'), width: 150 },
  { prop: 'name', label: t('antWorkflow.businessAuditPoint.name'), minWidth: 150 },
  { prop: 'category', label: t('antWorkflow.businessAuditPoint.category'), width: 120, align: 'center' },
  { prop: 'workflowName', label: t('antWorkflow.businessAuditPoint.workflowName'), minWidth: 150 },
  { prop: 'tableName', label: t('antWorkflow.businessAuditPoint.tableName'), width: 150 },
  { prop: 'status', label: t('antWorkflow.businessAuditPoint.status'), width: 100, align: 'center' },
  { prop: 'createTime', label: t('antWorkflow.businessAuditPoint.createTime'), width: 180 },
])

// 响应式数据
const loading = ref(false)
const tableData = ref<BusinessAuditPoint[]>([])
const total = ref(0)
const baseTableRef = ref()
const dialogVisible = ref(false)
const currentId = ref<string | undefined>(undefined)

const queryParams = reactive<BusinessAuditPointQueryParams>({
  pageIndex: 1,
  pageSize: 10,
  code: '',
  name: '',
  tableName: '',
  status: undefined,
})

// 生命周期
onMounted(() => {
  handleSearch()
})

/**
 * 加载审核点列表
 */
const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getBusinessAuditPointList(queryParams)
    tableData.value = data.list || []
    total.value = data.total || 0
  } catch (error) {
    // 错误已在拦截器处理
  } finally {
    loading.value = false
  }
}

/**
 * 重置搜索
 */
const handleReset = () => {
  queryParams.code = ''
  queryParams.name = ''
  queryParams.tableName = ''
  queryParams.status = undefined
  queryParams.pageIndex = 1
  handleSearch()
}

/**
 * 新增审核点
 */
const handleCreate = () => {
  currentId.value = undefined
  dialogVisible.value = true
}

/**
 * 编辑审核点
 */
const handleEdit = (row: BusinessAuditPoint) => {
  currentId.value = row.id
  dialogVisible.value = true
}

/**
 * 删除审核点
 */
const handleDelete = async (row: BusinessAuditPoint) => {
  try {
    await ElMessageBox.confirm(
      t('antWorkflow.businessAuditPoint.messages.deleteConfirm'),
      t('antWorkflow.businessAuditPoint.warning'),
      {
        confirmButtonText: t('antWorkflow.businessAuditPoint.confirm'),
        cancelButtonText: t('antWorkflow.businessAuditPoint.cancel'),
        type: 'warning',
      }
    )
    await deleteBusinessAuditPoint(row.id)
    ElMessage.success(t('antWorkflow.businessAuditPoint.messages.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // 用户取消或请求失败
  }
}
</script>

<style scoped lang="scss">
.business-audit-point-list {
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

  .text-muted {
    color: #909399;
  }
}
</style>