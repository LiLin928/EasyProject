<!-- src/views/basic/user/index.vue -->
<template>
  <div class="user-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('user.user.title') }}</span>
          <div class="header-buttons">
            <el-dropdown trigger="click" @command="handleExport">
              <el-button type="success">
                <el-icon><Download /></el-icon>
                {{ t('user.user.export') }}
                <el-icon class="el-icon--right"><ArrowDown /></el-icon>
              </el-button>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item command="current">
                    {{ t('user.user.exportCurrentPage') }}
                  </el-dropdown-item>
                  <el-dropdown-item command="all">
                    {{ t('user.user.exportAll') }}
                  </el-dropdown-item>
                  <el-dropdown-item command="selected" :disabled="selectedRows.length === 0">
                    {{ t('user.user.exportSelected') }}
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
            <el-button
              type="danger"
              :disabled="selectedRows.length === 0"
              @click="handleBatchDelete"
            >
              <el-icon><Delete /></el-icon>
              {{ t('user.user.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('user.user.addUser') }}
            </el-button>
          </div>
        </div>
      </template>

      <!-- Search form -->
      <SearchForm
        :items="searchItems"
        v-model="queryParams"
        :loading="loading"
        @search="handleSearch"
        @reset="handleReset"
      />

      <!-- 可拖拽表格 -->
      <DraggableTable
        ref="draggableTableRef"
        :data="tableData"
        :columns="tableColumns"
        :loading="loading"
        :selection="true"
        :total="total"
        :page-index="queryParams.pageIndex"
        :page-size="queryParams.pageSize"
        :tips-text="t('user.user.dragRowTip') + ' | ' + t('user.user.dragColumnTip')"
        @update:page-index="queryParams.pageIndex = $event"
        @update:page-size="queryParams.pageSize = $event"
        @selection-change="handleSelectionChange"
        @sort-change="handleSortChange"
        @page-change="handleSearch"
        @row-order-change="handleRowOrderChange"
        @column-order-change="handleColumnOrderChange"
      >
        <!-- 角色列自定义渲染 -->
        <template #roles="{ row }">
          <el-tag
            v-for="role in row.roles"
            :key="role"
            type="primary"
            effect="plain"
            size="small"
            style="margin-right: 4px"
          >
            {{ role }}
          </el-tag>
          <span v-if="!row.roles?.length">-</span>
        </template>

        <!-- 状态列自定义渲染 -->
        <template #status="{ row }">
          <el-switch
            :model-value="row.status"
            :active-value="1"
            :inactive-value="0"
            :disabled="row.userName === 'admin'"
            :before-change="() => handleStatusBeforeChange(row)"
          />
        </template>

        <!-- 操作列 -->
        <template #operation>
          <el-table-column :label="t('user.user.operation')" width="200" fixed="right">
            <template #default="{ row }">
              <el-button link type="primary" @click="handleEdit(row)">
                {{ t('user.user.editUser') }}
              </el-button>
              <el-button link type="primary" @click="handleResetPassword(row)">
                {{ t('user.user.resetPassword') }}
              </el-button>
              <el-button
                link
                type="danger"
                :disabled="row.userName === 'admin'"
                @click="handleDelete(row)"
              >
                {{ t('user.user.deleteUser') }}
              </el-button>
            </template>
          </el-table-column>
        </template>
      </DraggableTable>
    </el-card>

    <!-- User form dialog -->
    <UserFormDialog
      v-model="dialogVisible"
      :user-id="currentUserId"
      @success="handleSearch"
    />

    <!-- Reset password dialog -->
    <ResetPasswordDialog
      v-model="resetPasswordVisible"
      :user-id="currentUserId"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Delete, Download, ArrowDown } from '@element-plus/icons-vue'
import { getUserList, deleteUser, deleteUserBatch, updateUserStatus } from '@/api/user'
import { useLocale } from '@/composables/useLocale'
import { exportToExcel } from '@/utils/export'
import type { UserInfo } from '@/types'
import { CommonStatus } from '@/types/enums'
import type { TableColumn } from '@/components/DraggableTable/index.vue'
import DraggableTable from '@/components/DraggableTable/index.vue'
import SearchForm from '@/components/SearchForm/index.vue'
import UserFormDialog from './components/UserFormDialog.vue'
import ResetPasswordDialog from './components/ResetPasswordDialog.vue'

const { t } = useLocale()

// 搜索表单配置
const searchItems = computed(() => [
  { field: 'userName', label: t('user.user.username'), type: 'input' },
  { field: 'realName', label: t('user.user.nickname'), type: 'input' },
  { field: 'status', label: t('user.user.status'), type: 'select', options: [
    { label: t('user.user.enabled'), value: CommonStatus.ENABLED },
    { label: t('user.user.disabled'), value: CommonStatus.DISABLED },
  ], props: { style: { width: '120px' } } },
])

// 表格列配置
const tableColumns = ref<TableColumn[]>([
  { prop: 'userName', label: t('user.user.username'), minWidth: 100, sortable: true },
  { prop: 'realName', label: t('user.user.nickname'), minWidth: 100, sortable: true },
  { prop: 'email', label: t('user.user.email'), minWidth: 150, sortable: true },
  { prop: 'phone', label: t('user.user.phone'), minWidth: 120, sortable: true },
  { prop: 'roles', label: t('user.user.roles'), minWidth: 120 },
  { prop: 'status', label: t('user.user.status'), width: 100, align: 'center' },
  { prop: 'createTime', label: t('user.user.createTime'), width: 160, sortable: true },
])

// 当前显示的列配置（用于导出）
const currentColumns = ref<TableColumn[]>([...tableColumns.value])

const draggableTableRef = ref()
const loading = ref(false)
const tableData = ref<UserInfo[]>([])
const total = ref(0)
const selectedRows = ref<UserInfo[]>([])

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  userName: '',
  realName: '',
  status: undefined as CommonStatus | undefined,
  sortField: '',
  sortOrder: '' as '' | 'ascending' | 'descending',
})

const dialogVisible = ref(false)
const resetPasswordVisible = ref(false)
const currentUserId = ref<string | undefined>(undefined)

onMounted(() => {
  handleSearch()
})

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getUserList(queryParams)
    tableData.value = data.list
    total.value = data.total
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.pageIndex = 1
  queryParams.sortField = ''
  queryParams.sortOrder = ''
  handleSearch()
}

const handleSelectionChange = (rows: UserInfo[]) => {
  selectedRows.value = rows
}

const handleSortChange = ({ prop, order }: { prop: string; order: string | null }) => {
  queryParams.sortField = prop || ''
  queryParams.sortOrder = order as '' | 'ascending' | 'descending'
  handleSearch()
}

const handleRowOrderChange = (newData: UserInfo[]) => {
  tableData.value = newData
}

const handleColumnOrderChange = (newColumns: TableColumn[]) => {
  currentColumns.value = newColumns
}

const handleAdd = () => {
  currentUserId.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: UserInfo) => {
  currentUserId.value = row.id
  dialogVisible.value = true
}

const handleDelete = async (row: UserInfo) => {
  try {
    await ElMessageBox.confirm(
      t('user.user.deleteUserConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteUser(row.id)
    ElMessage.success(t('user.user.deleteUserSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleStatusBeforeChange = async (row: UserInfo): Promise<boolean> => {
  const newStatus = row.status === 1 ? 0 : 1
  try {
    await updateUserStatus(row.id, newStatus as CommonStatus)
    row.status = newStatus
    ElMessage.success(newStatus === 1 ? t('user.user.enabled') : t('user.user.disabled'))
    return true
  } catch (error) {
    return false
  }
}

const handleResetPassword = (row: UserInfo) => {
  currentUserId.value = row.id
  resetPasswordVisible.value = true
}

const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  const idsToDelete = selectedRows.value
    .filter(row => row.userName !== 'admin')
    .map(row => row.id)

  if (idsToDelete.length === 0) {
    ElMessage.warning(t('user.user.cannotDeleteAdmin'))
    return
  }

  try {
    await ElMessageBox.confirm(
      t('user.user.deleteUsersConfirm', { count: idsToDelete.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteUserBatch(idsToDelete)
    ElMessage.success(t('user.user.deleteUserSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

// ==================== 导出功能 ====================

const handleExport = async (type: string) => {
  let exportData: UserInfo[] = []

  switch (type) {
    case 'current':
      exportData = tableData.value
      break
    case 'all':
      try {
        const data = await getUserList({ ...queryParams, pageIndex: 1, pageSize: 9999 })
        exportData = data.list
      } catch (error) {
        ElMessage.error(t('user.user.exportFailed'))
        return
      }
      break
    case 'selected':
      exportData = selectedRows.value
      break
  }

  if (exportData.length === 0) {
    ElMessage.warning('暂无数据可导出')
    return
  }

  exportToExcel({
    columns: currentColumns.value.filter(c => c.prop !== 'status'), // 排除状态列
    data: exportData,
    fileName: `用户列表_${new Date().toLocaleDateString().replace(/\//g, '-')}`,
    sheetName: '用户列表',
  })

  ElMessage.success(t('user.user.exportSuccess'))
}
</script>

<style scoped lang="scss">
.user-container {
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