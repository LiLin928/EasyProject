<!-- src/views/buz/product/CategoryList.vue -->
<template>
  <div class="category-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('product.category.listTitle') }}</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>
            {{ t('product.category.add') }}
          </el-button>
        </div>
      </template>

      <!-- Table -->
      <el-table v-loading="loading" :data="tableData" border>
        <el-table-column prop="name" :label="t('product.category.name')" min-width="150" />
        <el-table-column :label="t('product.category.icon')" width="100" align="center">
          <template #default="{ row }">
            <el-icon v-if="row.icon" :size="24">
              <component :is="iconComponents[row.icon]" />
            </el-icon>
            <span v-else class="no-icon">-</span>
          </template>
        </el-table-column>
        <el-table-column prop="sort" :label="t('product.category.sort')" width="100" align="center" />
        <el-table-column prop="description" :label="t('product.category.description')" min-width="200" show-overflow-tooltip />
        <el-table-column :label="t('product.list.operation')" width="150" fixed="right" align="center">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)">
              {{ t('product.category.edit') }}
            </el-button>
            <el-button link type="danger" @click="handleDelete(row)">
              {{ t('product.category.delete') }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>

    <!-- Form Dialog -->
    <CategoryFormDialog
      v-model="dialogVisible"
      :category-id="currentCategoryId"
      @success="loadCategories"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import * as Icons from '@element-plus/icons-vue'
import { getCategoryList, deleteCategory } from '@/api/buz/productApi'
import { useLocale } from '@/composables/useLocale'
import type { Category } from '@/types/product'
import CategoryFormDialog from './components/CategoryFormDialog.vue'

const { t } = useLocale()

// Icon components map for rendering
const iconComponents = Icons

const loading = ref(false)
const tableData = ref<Category[]>([])
const dialogVisible = ref(false)
const currentCategoryId = ref<string | undefined>(undefined)

onMounted(() => {
  loadCategories()
})

const loadCategories = async () => {
  loading.value = true
  try {
    const data = await getCategoryList()
    tableData.value = data
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleAdd = () => {
  currentCategoryId.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: Category) => {
  currentCategoryId.value = row.id
  dialogVisible.value = true
}

const handleDelete = async (row: Category) => {
  try {
    await ElMessageBox.confirm(
      t('product.category.deleteConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteCategory(row.id)
    ElMessage.success(t('product.category.deleteSuccess'))
    loadCategories()
  } catch (error) {
    // User cancelled or request failed
  }
}
</script>

<style scoped lang="scss">
.category-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .no-icon {
    color: var(--el-text-color-placeholder);
  }
}
</style>