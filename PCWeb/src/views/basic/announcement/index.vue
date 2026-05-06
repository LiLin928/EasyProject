<!-- src/views/basic/announcement/index.vue -->
<template>
  <div class="announcement-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('announcement.list.title') }}</span>
          <el-button type="primary" @click="handleCreate">
            <el-icon><Plus /></el-icon>
            {{ t('announcement.list.create') }}
          </el-button>
        </div>
      </template>

      <!-- 搜索栏 -->
      <SearchForm
        :items="searchItems"
        v-model="queryParams"
        :loading="loading"
        @search="handleSearch"
        @reset="handleReset"
      />

      <!-- 表格 -->
      <BaseTable
        :data="tableData"
        :columns="columns"
        :loading="loading"
        :total="total"
        :page-index="queryParams.pageIndex"
        :page-size="queryParams.pageSize"
        @update:page-index="queryParams.pageIndex = $event"
        @update:page-size="queryParams.pageSize = $event"
        @page-change="handleSearch"
      >
        <!-- 类型列 -->
        <template #type="{ row }">
          <el-tag :type="row.type === 1 ? 'success' : 'warning'" size="small">
            {{ row.type === 1 ? t('announcement.type.all') : t('announcement.type.targeted') }}
          </el-tag>
        </template>

        <!-- 级别列 -->
        <template #level="{ row }">
          <el-tag :type="getLevelType(row.level)" size="small">
            {{ getLevelText(row.level) }}
          </el-tag>
        </template>

        <!-- 状态列 -->
        <template #status="{ row }">
          <el-tag :type="getStatusType(row.status)" size="small">
            {{ getStatusText(row.status) }}
          </el-tag>
        </template>

        <!-- 置顶列 -->
        <template #isTop="{ row }">
          <el-tag v-if="row.isTop" type="danger" size="small">{{ t('announcement.list.top') }}</el-tag>
          <span v-else>-</span>
        </template>

        <!-- 操作列 -->
        <template #operation>
          <el-table-column :label="t('common.table.operation')" width="280" fixed="right">
            <template #default="{ row }">
              <!-- 草稿状态 -->
              <template v-if="row.status === 0">
                <el-button link type="primary" @click="handleView(row)">
                  {{ t('common.button.view') }}
                </el-button>
                <el-button link type="primary" @click="handleEdit(row)">
                  {{ t('common.button.edit') }}
                </el-button>
                <el-button link type="success" @click="handlePublish(row)">
                  {{ t('announcement.list.publish') }}
                </el-button>
                <el-button link type="danger" @click="handleDelete(row)">
                  {{ t('common.button.delete') }}
                </el-button>
              </template>
              <!-- 已发布状态 -->
              <template v-if="row.status === 1">
                <el-button link type="primary" @click="handleView(row)">
                  {{ t('common.button.view') }}
                </el-button>
                <el-button link type="warning" @click="handleRecall(row)">
                  {{ t('announcement.list.recall') }}
                </el-button>
                <el-button link :type="row.isTop ? 'info' : 'success'" @click="handleToggleTop(row)">
                  {{ row.isTop ? t('announcement.list.unTop') : t('announcement.list.top') }}
                </el-button>
                <el-button link type="primary" @click="handleReadStats(row)">
                  {{ t('announcement.list.readStats') }}
                </el-button>
              </template>
              <!-- 已撤回状态 -->
              <template v-if="row.status === 2">
                <el-button link type="primary" @click="handleView(row)">
                  {{ t('common.button.view') }}
                </el-button>
                <el-button link type="primary" @click="handleEdit(row)">
                  {{ t('common.button.edit') }}
                </el-button>
                <el-button link type="success" @click="handlePublish(row)">
                  {{ t('announcement.list.republish') }}
                </el-button>
                <el-button link type="danger" @click="handleDelete(row)">
                  {{ t('common.button.delete') }}
                </el-button>
              </template>
            </template>
          </el-table-column>
        </template>
      </BaseTable>
    </el-card>

    <!-- 阅读详情弹窗 -->
    <ReadDetailDialog
      v-model="readDetailVisible"
      :id="currentId"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import BaseTable from '@/components/BaseTable/index.vue'
import SearchForm from '@/components/SearchForm/index.vue'
import type { SearchFormItem } from '@/components/SearchForm/types'
import {
  getAnnouncementList,
  deleteAnnouncement,
  publishAnnouncement,
  recallAnnouncement,
  toggleTopAnnouncement
} from '@/api/announcement/announcementApi'
import ReadDetailDialog from './components/ReadDetailDialog.vue'
import { useLocale } from '@/composables/useLocale'
import type { Announcement, QueryAnnouncementParams } from '@/types'
import type { TableColumn } from '@/components/BaseTable/index.vue'

const { t } = useLocale()
const router = useRouter()

// 搜索表单配置
const searchItems = computed<SearchFormItem[]>(() => [
  { field: 'title', label: t('announcement.list.title'), type: 'input' },
  { field: 'status', label: t('announcement.list.status'), type: 'select', options: [
    { label: t('announcement.status.draft'), value: 0 },
    { label: t('announcement.status.published'), value: 1 },
    { label: t('announcement.status.recalled'), value: 2 },
  ] },
])

// 表格列配置
const columns = ref<TableColumn[]>([
  { prop: 'title', label: t('announcement.list.title'), minWidth: 200 },
  { prop: 'type', label: t('announcement.list.type'), width: 100, align: 'center' },
  { prop: 'level', label: t('announcement.list.level'), width: 100, align: 'center' },
  { prop: 'status', label: t('announcement.list.status'), width: 100, align: 'center' },
  { prop: 'isTop', label: t('announcement.list.top'), width: 80, align: 'center' },
  { prop: 'publishTime', label: t('announcement.list.publishTime'), width: 180 },
  { prop: 'creatorName', label: t('announcement.list.creator'), width: 120 },
])

const loading = ref(false)
const tableData = ref<Announcement[]>([])
const total = ref(0)
const readDetailVisible = ref(false)
const currentId = ref<string>('')

const queryParams = reactive<QueryAnnouncementParams>({
  pageIndex: 1,
  pageSize: 10,
  title: '',
  status: undefined
})

onMounted(() => {
  handleSearch()
})

const handleSearch = async () => {
  loading.value = true
  try {
    const data = await getAnnouncementList(queryParams)
    tableData.value = data.list
    total.value = data.total
  } catch (error) {
    // Error handled by interceptor
  } finally {
    loading.value = false
  }
}

const handleReset = () => {
  queryParams.title = ''
  queryParams.status = undefined
  queryParams.pageIndex = 1
  handleSearch()
}

const handleCreate = () => {
  router.push('/basic/announcement/edit')
}

const handleEdit = (row: Announcement) => {
  router.push(`/basic/announcement/edit/${row.id}`)
}

const handleView = (row: Announcement) => {
  router.push(`/basic/announcement/preview/${row.id}`)
}

const handlePublish = async (row: Announcement) => {
  try {
    await ElMessageBox.confirm(t('announcement.message.publishConfirm'), t('common.message.info'), { type: 'warning' })
    await publishAnnouncement(row.id)
    ElMessage.success(t('announcement.message.publishSuccess'))
    handleSearch()
  } catch {
    // User cancelled
  }
}

const handleRecall = async (row: Announcement) => {
  try {
    await ElMessageBox.confirm(t('announcement.message.recallConfirm'), t('common.message.info'), { type: 'warning' })
    await recallAnnouncement(row.id)
    ElMessage.success(t('announcement.message.recallSuccess'))
    handleSearch()
  } catch {
    // User cancelled
  }
}

const handleDelete = async (row: Announcement) => {
  try {
    await ElMessageBox.confirm(t('announcement.message.deleteConfirm'), t('common.message.info'), { type: 'warning' })
    await deleteAnnouncement(row.id)
    ElMessage.success(t('common.message.deleteSuccess'))
    handleSearch()
  } catch {
    // User cancelled
  }
}

const handleToggleTop = async (row: Announcement) => {
  try {
    await toggleTopAnnouncement(row.id)
    ElMessage.success(row.isTop ? t('announcement.message.unTopSuccess') : t('announcement.message.topSuccess'))
    handleSearch()
  } catch {
    // Error handled
  }
}

const handleReadStats = (row: Announcement) => {
  currentId.value = row.id
  readDetailVisible.value = true
}

const getLevelType = (level: number): 'info' | 'warning' | 'danger' => {
  const map: Record<number, 'info' | 'warning' | 'danger'> = { 1: 'info', 2: 'warning', 3: 'danger' }
  return map[level] || 'info'
}

const getLevelText = (level: number): string => {
  const map: Record<number, string> = { 1: t('announcement.level.normal'), 2: t('announcement.level.important'), 3: t('announcement.level.urgent') }
  return map[level] || t('announcement.level.normal')
}

const getStatusType = (status: number): 'info' | 'success' | 'warning' => {
  const map: Record<number, 'info' | 'success' | 'warning'> = { 0: 'info', 1: 'success', 2: 'warning' }
  return map[status] || 'info'
}

const getStatusText = (status: number): string => {
  const map: Record<number, string> = { 0: t('announcement.status.draft'), 1: t('announcement.status.published'), 2: t('announcement.status.recalled') }
  return map[status] || t('announcement.status.draft')
}
</script>

<style scoped lang="scss">
.announcement-container {
  padding: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
}
</style>