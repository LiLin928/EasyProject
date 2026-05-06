<!-- src/views/basic/menu/index.vue -->
<template>
  <div class="menu-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('menu.menuManagement.title') }}</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>
            {{ t('menu.menuManagement.addMenu') }}
          </el-button>
        </div>
      </template>

      <!-- Search form -->
      <SearchForm
        :items="searchItems"
        v-model="queryParams"
        @search="handleSearch"
        @reset="handleReset"
      />

      <!-- Tree Table -->
      <el-table
        v-loading="loading"
        :data="filteredTableData"
        row-key="id"
        :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
        border
        default-expand-all
      >
        <el-table-column prop="menuName" :label="t('menu.menuManagement.menuName')" min-width="180" />
        <el-table-column prop="path" :label="t('menu.menuManagement.menuPath')" min-width="150" />
        <el-table-column prop="component" :label="t('menu.menuManagement.componentPath')" min-width="150">
          <template #default="{ row }">
            {{ row.component || '-' }}
          </template>
        </el-table-column>
        <el-table-column :label="t('menu.menuManagement.icon')" width="80" align="center">
          <template #default="{ row }">
            <el-icon v-if="row.icon" :size="18">
              <component :is="iconComponents[row.icon]" />
            </el-icon>
            <span v-else>-</span>
          </template>
        </el-table-column>
        <el-table-column prop="sort" :label="t('menu.menuManagement.sort')" width="80" align="center" />
        <el-table-column :label="t('menu.menuManagement.status')" width="100" align="center">
          <template #default="{ row }">
            <el-switch
              v-model="row.status"
              :active-value="CommonStatus.ENABLED"
              :inactive-value="CommonStatus.DISABLED"
              @change="handleStatusChange(row)"
            />
          </template>
        </el-table-column>
        <el-table-column :label="t('menu.menuManagement.visible')" width="100" align="center">
          <template #default="{ row }">
            <el-switch
              v-model="row.hidden"
              :active-value="MenuVisibility.VISIBLE"
              :inactive-value="MenuVisibility.HIDDEN"
              @change="handleVisibleChange(row)"
            />
          </template>
        </el-table-column>
        <el-table-column :label="t('menu.menuManagement.operation')" width="200" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleAddChild(row)">
              {{ t('menu.menuManagement.addChildMenu') }}
            </el-button>
            <el-button link type="primary" @click="handleEdit(row)">
              {{ t('menu.menuManagement.editMenu') }}
            </el-button>
            <el-button
              link
              type="danger"
              :disabled="row.children && row.children.length > 0"
              @click="handleDelete(row)"
            >
              {{ t('menu.menuManagement.deleteMenu') }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Menu Form Dialog -->
    <MenuFormDialog
      v-model="dialogVisible"
      :menu-id="currentMenuId"
      :parent-id="currentParentId"
      @success="loadMenuTree"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import * as Icons from '@element-plus/icons-vue'
import { getMenuTree, deleteMenu, updateMenu } from '@/api/menu'
import { useLocale } from '@/composables/useLocale'
import type { MockMenu } from '@/types/menu'
import { CommonStatus, MenuVisibility } from '@/types/enums'
import SearchForm from '@/components/SearchForm/index.vue'
import MenuFormDialog from './components/MenuFormDialog.vue'

const { t } = useLocale()

// Icon components map for rendering
const iconComponents = Icons

// 搜索表单配置
const searchItems = computed(() => [
  { field: 'name', label: t('menu.menuManagement.menuName'), type: 'input' },
  { field: 'status', label: t('menu.menuManagement.status'), type: 'select', options: [
    { label: t('menu.menuManagement.enabled'), value: CommonStatus.ENABLED },
    { label: t('menu.menuManagement.disabled'), value: CommonStatus.DISABLED },
  ]},
])

const loading = ref(false)
const tableData = ref<MockMenu[]>([])

let queryParams = reactive({
  name: '',
  status: undefined as CommonStatus | undefined,
})

// 过滤后的表格数据
const filteredTableData = computed(() => {
  if (!queryParams.name && queryParams.status === undefined) {
    return tableData.value
  }
  return filterMenuTree(tableData.value, queryParams)
})

// 过滤菜单树
const filterMenuTree = (menus: MockMenu[], params: { name?: string; status?: CommonStatus }): MockMenu[] => {
  return menus.filter(menu => {
    const nameMatch = !params.name || menu.menuName.includes(params.name)
    const statusMatch = params.status === undefined || menu.status === params.status
    if (menu.children && menu.children.length > 0) {
      const filteredChildren = filterMenuTree(menu.children, params)
      return (nameMatch && statusMatch) || filteredChildren.length > 0
    }
    return nameMatch && statusMatch
  }).map(menu => ({
    ...menu,
    children: menu.children ? filterMenuTree(menu.children, params) : undefined,
  }))
}

const dialogVisible = ref(false)
const currentMenuId = ref<string | undefined>(undefined)
const currentParentId = ref<string | null>(null)

onMounted(() => {
  loadMenuTree()
})

const loadMenuTree = async () => {
  loading.value = true
  try {
    const data = await getMenuTree()
    tableData.value = data
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  loadMenuTree()
}

const handleReset = () => {
  queryParams.name = ''
  queryParams.status = undefined
}

const handleAdd = () => {
  currentMenuId.value = undefined
  currentParentId.value = null
  dialogVisible.value = true
}

const handleAddChild = (row: MockMenu) => {
  currentMenuId.value = undefined
  currentParentId.value = row.id
  dialogVisible.value = true
}

const handleEdit = (row: MockMenu) => {
  currentMenuId.value = row.id
  currentParentId.value = null
  dialogVisible.value = true
}

const handleDelete = async (row: MockMenu) => {
  if (row.children && row.children.length > 0) {
    ElMessage.warning(t('menu.menuManagement.hasChildrenCannotDelete'))
    return
  }

  try {
    await ElMessageBox.confirm(
      t('menu.menuManagement.deleteMenuConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteMenu(row.id)
    ElMessage.success(t('menu.menuManagement.deleteMenuSuccess'))
    loadMenuTree()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleStatusChange = async (row: MockMenu) => {
  try {
    await updateMenu({
      id: row.id,
      status: row.status,
    })
    ElMessage.success(row.status === CommonStatus.ENABLED ? t('menu.menuManagement.enabled') : t('menu.menuManagement.disabled'))
  } catch (error) {
    // Restore original status
    row.status = row.status === CommonStatus.ENABLED ? CommonStatus.DISABLED : CommonStatus.ENABLED
  }
}

const handleVisibleChange = async (row: MockMenu) => {
  try {
    await updateMenu({
      id: row.id,
      hidden: row.hidden,
    })
    ElMessage.success(row.hidden === MenuVisibility.VISIBLE ? t('menu.menuManagement.shown') : t('menu.menuManagement.hidden'))
  } catch (error) {
    // Restore original value
    row.hidden = row.hidden === MenuVisibility.VISIBLE ? MenuVisibility.HIDDEN : MenuVisibility.VISIBLE
  }
}
</script>

<style scoped lang="scss">
.menu-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
}
</style>