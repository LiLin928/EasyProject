<!-- src/views/ant_workflow/list/index.vue -->
<template>
  <div class="ant-workflow-list">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('antWorkflow.list') }}</span>
          <div>
            <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleBatchDelete">
              {{ t('antWorkflow.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleCreate">
              <el-icon><Plus /></el-icon> {{ t('antWorkflow.create') }}
            </el-button>
          </div>
        </div>
      </template>
      <el-form :model="queryParams" :inline="true">
        <el-form-item :label="t('antWorkflow.name')">
          <el-input v-model="queryParams.name" :placeholder="t('antWorkflow.namePlaceholder')" clearable />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.status')">
          <el-select v-model="queryParams.status" :placeholder="t('antWorkflow.statusPlaceholder')" clearable style="width: 120px">
            <el-option :label="t('antWorkflow.draft')" :value="0" />
            <el-option :label="t('antWorkflow.published')" :value="2" />
            <el-option :label="t('antWorkflow.disabled')" :value="4" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">{{ t('antWorkflow.search') }}</el-button>
          <el-button @click="handleReset">{{ t('antWorkflow.reset') }}</el-button>
        </el-form-item>
      </el-form>
      <el-table v-loading="loading" :data="tableData" border @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="50" align="center" />
        <el-table-column prop="name" :label="t('antWorkflow.name')" />
        <el-table-column prop="categoryName" :label="t('antWorkflow.category')" />
        <el-table-column prop="status" :label="t('antWorkflow.status')">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">{{ getStatusLabel(row.status) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="creatorName" :label="t('antWorkflow.creator')" />
        <el-table-column prop="createTime" :label="t('antWorkflow.createTime')" />
        <el-table-column :label="t('antWorkflow.operation')" width="200">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleDesign(row)">{{ t('antWorkflow.designAction') }}</el-button>
            <el-button link type="primary" @click="handleEdit(row)">{{ t('antWorkflow.edit') }}</el-button>
            <el-button link type="danger" @click="handleDelete(row)">{{ t('antWorkflow.delete') }}</el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-pagination v-model:current-page="queryParams.pageIndex" v-model:page-size="queryParams.pageSize" :total="total" @current-change="handleSearch" />
    </el-card>

    <!-- 新建流程对话框（基本信息） -->
    <el-dialog v-model="createDialogVisible" :title="t('antWorkflow.create')" width="500px" :close-on-click-modal="false">
      <el-form ref="createFormRef" :model="createForm" :rules="createRules" label-width="100px">
        <el-form-item :label="t('antWorkflow.name')" prop="name">
          <el-input v-model="createForm.name" :placeholder="t('antWorkflow.namePlaceholder')" maxlength="50" show-word-limit />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.category')" prop="categoryCode">
          <el-select v-model="createForm.categoryCode" :placeholder="t('antWorkflow.categoryPlaceholder')" clearable style="width: 100%">
            <el-option
              v-for="cat in categories"
              :key="cat.value"
              :label="cat.labelZh || cat.label"
              :value="cat.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('antWorkflow.description')">
          <el-input v-model="createForm.description" type="textarea" :rows="3" :placeholder="t('antWorkflow.descriptionPlaceholder')" maxlength="200" show-word-limit />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="createDialogVisible = false">{{ t('antWorkflow.cancel') }}</el-button>
        <el-button type="primary" :loading="creating" @click="handleCreateSubmit">
          {{ t('antWorkflow.createAndDesign') }}
        </el-button>
      </template>
    </el-dialog>

    <!-- 编辑弹窗 -->
    <el-dialog v-model="editDialogVisible" :title="t('antWorkflow.edit')" width="500px">
      <el-form :model="editForm" label-width="100px">
        <el-form-item :label="t('antWorkflow.name')">
          <el-input v-model="editForm.name" :placeholder="t('antWorkflow.namePlaceholder')" />
        </el-form-item>
        <el-form-item :label="t('antWorkflow.category')">
          <el-select v-model="editForm.categoryCode" :placeholder="t('antWorkflow.categoryPlaceholder')" clearable style="width: 100%">
            <el-option
              v-for="cat in categories"
              :key="cat.value"
              :label="cat.labelZh || cat.label"
              :value="cat.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('antWorkflow.description')">
          <el-input v-model="editForm.description" type="textarea" :rows="3" :placeholder="t('antWorkflow.descriptionPlaceholder')" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="editDialogVisible = false">{{ t('antWorkflow.cancel') }}</el-button>
        <el-button type="primary" @click="handleEditSubmit">{{ t('antWorkflow.save') }}</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import type { FormInstance, FormRules } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { WorkflowStatus } from '@/types/antWorkflow'
import { getWorkflowList, createWorkflow, deleteWorkflow, updateWorkflow } from '@/api/ant_workflow/workflowApi'
import { getDictDataByCode } from '@/api/dict'
import type { DictData } from '@/types/dict'

const { t } = useLocale()
const router = useRouter()

const loading = ref(false)
const tableData = ref<any[]>([])
const selectedRows = ref<any[]>([])
const total = ref(0)
const queryParams = ref({ pageIndex: 1, pageSize: 10, name: '', status: undefined as WorkflowStatus | undefined })

// 分类列表
const categories = ref<DictData[]>([])

// 新建流程对话框
const createDialogVisible = ref(false)
const createFormRef = ref<FormInstance | null>(null)
const createForm = ref({ name: '', categoryCode: '', description: '' })
const creating = ref(false)
const createRules: FormRules = {
  name: [
    { required: true, message: t('antWorkflow.validation.nameRequired'), trigger: 'blur' },
    { min: 2, max: 50, message: t('antWorkflow.validation.nameLength'), trigger: 'blur' },
  ],
}

// 编辑弹窗
const editDialogVisible = ref(false)
const editForm = ref({ id: '', name: '', categoryName: '', categoryCode: '', description: '' })

onMounted(() => {
  loadCategories()
  handleSearch()
})

const loadCategories = async () => {
  try {
    const data = await getDictDataByCode('workflow_category')
    categories.value = data.filter(item => item.status === 1)
  } catch (error) {
    // Error handled by interceptor
  }
}

const handleSearch = async () => {
  loading.value = true
  try { const res = await getWorkflowList(queryParams.value); tableData.value = res.list || []; total.value = res.total || 0 } finally { loading.value = false }
}

const handleReset = () => { queryParams.value = { pageIndex: 1, pageSize: 10, name: '', status: undefined }; handleSearch() }

const getStatusType = (status: WorkflowStatus) => status === WorkflowStatus.PUBLISHED ? 'success' : status === WorkflowStatus.DRAFT ? 'info' : 'danger'
const getStatusLabel = (status: WorkflowStatus) => status === WorkflowStatus.PUBLISHED ? t('antWorkflow.published') : status === WorkflowStatus.DRAFT ? t('antWorkflow.draft') : t('antWorkflow.disabled')

// 新建流程 - 先弹出基本信息对话框
const handleCreate = () => {
  createForm.value = { name: '', categoryCode: '', description: '' }
  createDialogVisible.value = true
}

// 创建并进入设计页面
const handleCreateSubmit = async () => {
  if (!createFormRef.value) return

  try {
    await createFormRef.value.validate()
  } catch {
    return
  }

  creating.value = true
  try {
    const workflowId = await createWorkflow({
      name: createForm.value.name,
      code: 'wf-' + Date.now(),
      categoryCode: createForm.value.categoryCode,
      description: createForm.value.description,
    })
    ElMessage.success(t('antWorkflow.messages.createSuccess'))
    createDialogVisible.value = false
    // 创建成功后跳转到设计页面（workflowId 是后端返回的 GUID 字符串）
    router.push(`/workflow-ant-design/${workflowId}`)
  } catch (error) {
    ElMessage.error(t('antWorkflow.messages.createFailed'))
  } finally {
    creating.value = false
  }
}

const handleDesign = (row: any) => router.push(`/workflow-ant-design/${row.id}`)
const handleEdit = (row: any) => {
  editForm.value = {
    id: row.id,
    name: row.name,
    categoryName: row.categoryName || '',
    categoryCode: row.categoryCode || '',
    description: row.description || ''
  }
  editDialogVisible.value = true
}
const handleEditSubmit = async () => {
  await updateWorkflow(editForm.value)
  ElMessage.success(t('antWorkflow.messages.updateSuccess'))
  editDialogVisible.value = false
  handleSearch()
}
const handleDelete = async (row: any) => {
  await ElMessageBox.confirm(t('antWorkflow.messages.deleteConfirm'), t('antWorkflow.delete'), { type: 'warning' })
  await deleteWorkflow(row.id)
  ElMessage.success(t('antWorkflow.messages.deleteSuccess'))
  handleSearch()
}

const handleSelectionChange = (rows: any[]) => {
  selectedRows.value = rows
}

const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  try {
    await ElMessageBox.confirm(
      t('antWorkflow.messages.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('antWorkflow.delete'),
      { type: 'warning' }
    )
    const ids = selectedRows.value.map(row => row.id)
    await deleteWorkflow(ids)
    ElMessage.success(t('antWorkflow.messages.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}
</script>

<style scoped lang="scss">.ant-workflow-list { padding: 20px; .card-header { display: flex; justify-content: space-between; align-items: center; } }</style>