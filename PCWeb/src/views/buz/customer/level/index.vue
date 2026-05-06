<!-- src/views/buz/customer/level/index.vue -->
<template>
  <div class="member-level-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('customer.levelManage') }}</span>
          <div>
            <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleBatchDelete">
              {{ t('customer.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('customer.addLevel') }}
            </el-button>
          </div>
        </div>
      </template>

      <!-- Table -->
      <el-table v-loading="loading" :data="tableData" border @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="50" align="center" />
        <el-table-column prop="name" :label="t('customer.levelName')" min-width="120" />
        <el-table-column prop="minSpent" :label="t('customer.minSpent')" min-width="140">
          <template #default="{ row }">
            {{ formatMoney(row.minSpent) }}
          </template>
        </el-table-column>
        <el-table-column prop="discount" :label="t('customer.discount')" min-width="100" align="center">
          <template #default="{ row }">
            {{ row.discount }}%
          </template>
        </el-table-column>
        <el-table-column prop="pointsRate" :label="t('customer.pointsRate')" min-width="100" align="center">
          <template #default="{ row }">
            x{{ row.pointsRate }}
          </template>
        </el-table-column>
        <el-table-column prop="sort" :label="t('customer.levelSort')" width="80" align="center" />
        <el-table-column :label="t('customer.status')" width="100" align="center">
          <template #default="{ row }">
            <el-switch
              v-model="row.status"
              :active-value="1"
              :inactive-value="0"
              @change="handleStatusChange(row)"
            />
          </template>
        </el-table-column>
        <el-table-column :label="t('customer.detail')" width="150" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)">
              {{ t('customer.editLevel') }}
            </el-button>
            <el-button link type="danger" @click="handleDelete(row)">
              {{ t('customer.deleteLevel') }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Level form dialog -->
    <LevelFormDialog
      v-model="dialogVisible"
      :level-id="currentLevelId"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { getMemberLevelList, deleteMemberLevel, deleteMemberLevelBatch, updateMemberLevel } from '@/api/buz/memberLevelApi'
import { useLocale } from '@/composables/useLocale'
import type { MemberLevel } from '@/types'
import LevelFormDialog from './components/LevelFormDialog.vue'

const { t } = useLocale()

const loading = ref(false)
const tableData = ref<MemberLevel[]>([])
const selectedRows = ref<MemberLevel[]>([])
const dialogVisible = ref(false)
const currentLevelId = ref<string | undefined>(undefined)

onMounted(() => {
  handleSearch()
})

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getMemberLevelList()
    tableData.value = data
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  currentLevelId.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: MemberLevel) => {
  currentLevelId.value = row.id
  dialogVisible.value = true
}

const handleDelete = async (row: MemberLevel) => {
  try {
    await ElMessageBox.confirm(
      t('customer.deleteLevelConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteMemberLevel(row.id)
    ElMessage.success(t('customer.deleteLevelSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleStatusChange = async (row: MemberLevel) => {
  try {
    await updateMemberLevel({
      id: row.id,
      name: row.name,
      minSpent: row.minSpent,
      discount: row.discount,
      pointsRate: row.pointsRate,
      icon: row.icon,
      sort: row.sort,
      status: row.status,
    })
    ElMessage.success(row.status === 1 ? t('customer.enabled') : t('customer.disabled'))
  } catch (error) {
    row.status = row.status === 1 ? 0 : 1
  }
}

const handleSelectionChange = (rows: MemberLevel[]) => {
  selectedRows.value = rows
}

const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  try {
    await ElMessageBox.confirm(
      t('customer.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    const ids = selectedRows.value.map(row => row.id)
    await deleteMemberLevelBatch(ids)
    ElMessage.success(t('customer.deleteLevelSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const formatMoney = (value: number) => {
  return value.toLocaleString('zh-CN', { style: 'currency', currency: 'CNY' })
}
</script>

<style scoped lang="scss">
.member-level-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
}
</style>