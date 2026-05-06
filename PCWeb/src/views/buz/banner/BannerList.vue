<!-- src/views/buz/banner/BannerList.vue -->
<template>
  <div class="banner-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('banner.list.title') }}</span>
          <div>
            <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleBatchDelete">
              {{ t('banner.list.batchDelete') }}
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ t('banner.list.add') }}
            </el-button>
          </div>
        </div>
      </template>

      <!-- Search Bar -->
      <SearchForm
        :items="searchItems"
        v-model="queryParams"
        @search="handleSearch"
        @reset="handleReset"
      />

      <!-- Table -->
      <el-table v-loading="loading" :data="tableData" border @selection-change="handleSelectionChange">
        <el-table-column type="selection" width="50" align="center" />
        <el-table-column prop="image" :label="t('banner.list.image')" width="100" align="center">
          <template #default="{ row }">
            <el-image
              :src="row.image"
              :preview-src-list="[row.image]"
              fit="cover"
              style="width: 60px; height: 60px; cursor: pointer"
            />
          </template>
        </el-table-column>
        <el-table-column prop="linkType" :label="t('banner.list.linkType')" width="120" align="center">
          <template #default="{ row }">
            <el-tag :type="getLinkTypeTagType(row.linkType)">
              {{ t(`banner.linkType.${row.linkType}`) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="linkValue" :label="t('banner.list.linkValue')" min-width="200">
          <template #default="{ row }">
            <span v-if="row.linkType === 'none'" class="text-muted">-</span>
            <span v-else>{{ row.linkValue }}</span>
          </template>
        </el-table-column>
        <el-table-column prop="sort" :label="t('banner.list.sort')" width="120" align="center">
          <template #default="{ row }">
            <el-input-number
              v-model="row.sort"
              :min="1"
              :max="999"
              size="small"
              controls-position="right"
              style="width: 80px"
              @change="handleSortChange(row)"
            />
          </template>
        </el-table-column>
        <el-table-column :label="t('banner.list.status')" width="100" align="center">
          <template #default="{ row }">
            <el-switch
              v-model="row.status"
              :active-value="1"
              :inactive-value="0"
              @change="handleStatusChange(row)"
            />
          </template>
        </el-table-column>
        <el-table-column prop="createTime" :label="t('banner.list.createTime')" width="160" />
        <el-table-column :label="t('banner.list.operation')" width="150" fixed="right">
          <template #default="{ row }">
            <el-button link type="primary" @click="handleEdit(row)">
              {{ t('banner.list.edit') }}
            </el-button>
            <el-button link type="danger" @click="handleDelete(row)">
              {{ t('banner.list.delete') }}
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
        @size-change="handleSearch"
        @current-change="handleSearch"
        style="margin-top: 20px; justify-content: flex-end"
      />
    </el-card>

    <!-- Form Dialog -->
    <BannerFormDialog
      v-model="dialogVisible"
      :banner-id="currentBannerId"
      @success="handleSearch"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { getBannerList, deleteBanner, deleteBannerBatch, updateBanner } from '@/api/buz/bannerApi'
import { useLocale } from '@/composables/useLocale'
import type { Banner, BannerQueryParams } from '@/types'
import SearchForm from '@/components/SearchForm/index.vue'
import BannerFormDialog from './components/BannerFormDialog.vue'

const { t } = useLocale()

// 搜索表单配置
const searchItems = computed(() => [
  { field: 'status', label: t('banner.list.statusFilter'), type: 'select', options: [
    { label: t('banner.list.enabled'), value: 1 },
    { label: t('banner.list.disabled'), value: 0 },
  ]},
])

const loading = ref(false)
const tableData = ref<Banner[]>([])
const selectedRows = ref<Banner[]>([])
const total = ref(0)
const dialogVisible = ref(false)
const currentBannerId = ref<string | undefined>(undefined)

let queryParams = reactive<BannerQueryParams>({
  pageIndex: 1,
  pageSize: 10,
  status: undefined,
})

onMounted(() => {
  handleSearch()
})

const handleSearch = async () => {
  loading.value = true
  try {
    const { list, total: totalCount } = await getBannerList(queryParams)
    tableData.value = list
    total.value = totalCount
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

const handleAdd = () => {
  currentBannerId.value = undefined
  dialogVisible.value = true
}

const handleEdit = (row: Banner) => {
  currentBannerId.value = row.id
  dialogVisible.value = true
}

const handleDelete = async (row: Banner) => {
  try {
    await ElMessageBox.confirm(
      t('banner.list.deleteConfirm'),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await deleteBanner(row.id)
    ElMessage.success(t('banner.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}

const handleStatusChange = async (row: Banner) => {
  try {
    await updateBanner({
      id: row.id,
      status: row.status,
    })
    ElMessage.success(row.status === 1 ? t('banner.list.enabled') : t('banner.list.disabled'))
  } catch (error) {
    // Restore original status on failure
    row.status = row.status === 1 ? 0 : 1
  }
}

const handleSortChange = async (row: Banner) => {
  try {
    await updateBanner({
      id: row.id,
      sort: row.sort,
    })
    ElMessage.success(t('banner.list.updateSuccess'))
  } catch (error) {
    // Error handled by interceptor
  }
}

const getLinkTypeTagType = (linkType: string) => {
  const typeMap: Record<string, string> = {
    none: 'info',
    product: 'success',
    category: 'warning',
    page: 'primary',
  }
  return typeMap[linkType] || 'info'
}

const handleSelectionChange = (rows: Banner[]) => {
  selectedRows.value = rows
}

const handleBatchDelete = async () => {
  if (selectedRows.value.length === 0) return

  try {
    await ElMessageBox.confirm(
      t('banner.list.batchDeleteConfirm', { count: selectedRows.value.length }),
      t('common.message.info'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    const ids = selectedRows.value.map(row => row.id)
    await deleteBannerBatch(ids)
    ElMessage.success(t('banner.list.deleteSuccess'))
    handleSearch()
  } catch (error) {
    // User cancelled or request failed
  }
}
</script>

<style scoped lang="scss">
.banner-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .search-form {
    margin-bottom: 20px;
  }

  .text-muted {
    color: #909399;
  }
}
</style>