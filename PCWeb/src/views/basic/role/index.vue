<!-- src/views/basic/role/index.vue -->
<template>
  <div class="role-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('role.role.title') }}</span>
          <div>
            <el-button
              type="danger"
              :disabled="selectedRows.length === 0"
              @click="handleBatchDelete"
            >
              <el-icon><Delete /></el-icon>
              {{ t('role.role.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('role.role.addRole') }}
            </el-button>
          </div>
        </div>
      </template>

      <!-- Search form -->
      <SearchForm
        :items="searchItems"
        v-model="queryParams"
        @search="handleSearch"
        @reset="handleReset"
      />

      <!-- Data table -->
      <el-table
        v-loading="loading"
        :data="tableData"
        border
        @selection-change="handleSelectionChange"
      >
        <el-table-column type="selection" width="50" align="center" />
        <el-table-column prop="roleName" :label="t('role.role.roleName')" min-width="120" />
        <el-table-column prop="roleCode" :label="t('role.role.roleCode')" min-width="120" />
        <el-table-column prop="description" :label="t('role.role.description')" min-width="150" show-overflow-tooltip />
        <el-table-column :label="t('role.role.status')" width="100" align="center">
          <template #default="{ row }">
            <el-switch
              v-model="row.status"
              :active-value="1"
              :inactive-value="0"
              :disabled="row.roleCode === 'admin'"
              @change="handleStatusChange(row)"
            />
          </template>
        </el-table-column>
        <el-table-column prop="createTime" :label="t('role.role.createTime')" width="160" />
        <el-table-column :label="t('role.role.operation')" width="150" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)">
              {{ t('role.role.editRole') }}
            </el-button>
            <el-button
              link
              type="danger"
              :disabled="row.roleCode === 'admin'"
              @click="handleDelete(row)"
            >
              {{ t('role.role.deleteRole') }}
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
        style="margin-top: 16px; justify-content: flex-end"
        @size-change="handleSearch"
        @current-change="handleSearch"
      />
    </el-card>

    <!-- Role form dialog -->
    <RoleFormDialog
      v-model="dialogVisible"
      :role-id="currentRoleId"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Delete } from '@element-plus/icons-vue'
import { getRoleList, deleteRole, deleteRoleBatch, updateRole } from '@/api/role'
import { useLocale } from '@/composables/useLocale'
import type { RoleInfo } from '@/types/role'
import { CommonStatus } from '@/types/enums'
import SearchForm from '@/components/SearchForm/index.vue'
import RoleFormDialog from './components/RoleFormDialog.vue'

const { t } = useLocale()

// 搜索表单配置
const searchItems = computed(() => [
  { field: 'roleName', label: t('role.role.roleName'), type: 'input' },
  { field: 'roleCode', label: t('role.role.roleCode'), type: 'input' },
  { field: 'status', label: t('role.role.status'), type: 'select', options: [
    { label: t('role.role.enabled'), value: CommonStatus.ENABLED },
    { label: t('role.role.disabled'), value: CommonStatus.DISABLED },
  ]},
])

const loading = ref(false)
const tableData = ref<RoleInfo[]>([])
const total = ref(0)
const selectedRows = ref<RoleInfo[]>([])

let queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  roleName: '',
  roleCode: '',
  status: undefined as CommonStatus | undefined,
})

const dialogVisible = ref(false)
const currentRoleId = ref<string | undefined>(undefined)

onMounted(() => {
  handleSearch()
})

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getRoleList(queryParams)
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
  handleSearch()
}

const handleSelectionChange = (rows: RoleInfo[]) => {
  selectedRows.value = rows
}

const handleAdd = () => {
  currentRoleId.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: RoleInfo) => {
  currentRoleId.value = row.id
  dialogVisible.value = true
}

const handleDelete = async (row: RoleInfo) => {
  try {
    await ElMessageBox.confirm(
      t('role.role.deleteRoleConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteRole(row.id)
    ElMessage.success(t('role.role.deleteRoleSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleStatusChange = async (row: RoleInfo) => {
  try {
    await updateRole({
      id: row.id,
      status: row.status,
    })
    ElMessage.success(row.status === 1 ? t('role.role.enabled') : t('role.role.disabled'))
  } catch (error) {
    // Restore original status
    row.status = row.status === 1 ? 0 : 1
  }
}

const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  const idsToDelete = selectedRows.value
    .filter(row => row.roleCode !== 'admin')
    .map(row => row.id)

  if (idsToDelete.length === 0) {
    ElMessage.warning(t('role.role.cannotDeleteAdmin'))
    return
  }

  try {
    await ElMessageBox.confirm(
      t('role.role.deleteRolesConfirm', { count: idsToDelete.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteRoleBatch(idsToDelete)
    ElMessage.success(t('role.role.deleteRoleSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}
</script>

<style scoped lang="scss">
.role-container {
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