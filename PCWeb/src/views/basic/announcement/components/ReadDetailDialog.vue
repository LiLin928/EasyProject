<!-- src/views/basic/announcement/components/ReadDetailDialog.vue -->
<template>
  <el-dialog
    v-model="visible"
    :title="t('announcement.readDetail.title')"
    width="800px"
    :close-on-click-modal="false"
    destroy-on-close
  >
    <div v-loading="loading" class="read-detail-content">
      <!-- 阅读统计 -->
      <div class="stats-section">
        <div class="stats-item">
          <div class="stats-value">{{ stats?.totalCount || 0 }}</div>
          <div class="stats-label">{{ t('announcement.readDetail.total') }}</div>
        </div>
        <div class="stats-item read">
          <div class="stats-value">{{ stats?.readCount || 0 }}</div>
          <div class="stats-label">{{ t('announcement.readDetail.read') }}</div>
        </div>
        <div class="stats-item unread">
          <div class="stats-value">{{ stats?.unreadCount || 0 }}</div>
          <div class="stats-label">{{ t('announcement.readDetail.unread') }}</div>
        </div>
        <div class="stats-progress">
          <el-progress
            :percentage="readPercentage"
            :stroke-width="10"
            :text-inside="true"
            :color="progressColor"
          />
        </div>
      </div>

      <!-- 筛选 -->
      <div class="filter-section">
        <el-radio-group v-model="filterIsRead" @change="handleFilterChange">
          <el-radio-button :value="undefined">{{ t('announcement.readDetail.all') }}</el-radio-button>
          <el-radio-button :value="true">{{ t('announcement.readDetail.readOnly') }}</el-radio-button>
          <el-radio-button :value="false">{{ t('announcement.readDetail.unreadOnly') }}</el-radio-button>
        </el-radio-group>
      </div>

      <!-- 阅读详情表格 -->
      <el-table :data="tableData" stripe>
        <el-table-column prop="userName" :label="t('announcement.readDetail.userName')" width="150" />
        <el-table-column prop="realName" :label="t('announcement.readDetail.realName')" width="150" />
        <el-table-column prop="isRead" :label="t('announcement.readDetail.status')" width="100" align="center">
          <template #default="{ row }">
            <el-tag :type="row.isRead ? 'success' : 'warning'" size="small">
              {{ row.isRead ? t('announcement.readDetail.read') : t('announcement.readDetail.unread') }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="readTime" :label="t('announcement.readDetail.readTime')" width="180">
          <template #default="{ row }">
            {{ row.readTime || '-' }}
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination
        v-model:current-page="queryParams.pageIndex"
        v-model:page-size="queryParams.pageSize"
        :total="total"
        :page-sizes="[10, 20, 50]"
        layout="total, sizes, prev, pager, next"
        @change="loadReadDetail"
      />
    </div>

    <template #footer>
      <el-button @click="visible = false">
        {{ t('common.button.close') }}
      </el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, reactive, watch } from 'vue'
import { getReadStats, getReadDetailList } from '@/api/announcement/announcementApi'
import { useLocale } from '@/composables/useLocale'
import type { ReadStats, ReadDetail, ReadDetailApiParams } from '@/types'

const props = defineProps<{
  modelValue: boolean
  id: string
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
}>()

const { t } = useLocale()

const visible = computed({
  get: () => props.modelValue,
  set: (val) => emit('update:modelValue', val)
})

const loading = ref(false)
const stats = ref<ReadStats | null>(null)
const tableData = ref<ReadDetail[]>([])
const total = ref(0)
const filterIsRead = ref<boolean | undefined>(undefined)

const queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  isRead: undefined as boolean | undefined
})

// 阅读百分比
const readPercentage = computed(() => {
  if (!stats.value || stats.value.totalCount === 0) return 0
  return Math.round((stats.value.readCount / stats.value.totalCount) * 100)
})

// 进度条颜色
const progressColor = computed(() => {
  if (readPercentage.value < 50) return '#e6a23c'
  if (readPercentage.value < 80) return '#409eff'
  return '#67c23a'
})

// 加载阅读统计
const loadReadStats = async () => {
  if (!props.id) return
  try {
    stats.value = await getReadStats(props.id)
  } catch {
    // Error handled
  }
}

// 加载阅读详情
const loadReadDetail = async () => {
  if (!props.id) return
  loading.value = true
  try {
    // 转换 isRead 参数：前端 boolean -> 后端 int (0/1)
    const params: ReadDetailApiParams = {
      pageIndex: queryParams.pageIndex,
      pageSize: queryParams.pageSize,
      isRead: queryParams.isRead === true ? 1 : queryParams.isRead === false ? 0 : undefined
    }
    const data = await getReadDetailList(props.id, params)
    tableData.value = data.list
    total.value = data.total
  } catch {
    // Error handled
  } finally {
    loading.value = false
  }
}

// 筛选变化
const handleFilterChange = () => {
  queryParams.isRead = filterIsRead.value
  queryParams.pageIndex = 1
  loadReadDetail()
}

// 监听弹窗打开
watch(visible, (val) => {
  if (val && props.id) {
    queryParams.pageIndex = 1
    queryParams.pageSize = 10
    filterIsRead.value = undefined
    queryParams.isRead = undefined
    loadReadStats()
    loadReadDetail()
  }
})
</script>

<style scoped lang="scss">
.read-detail-content {
  .stats-section {
    display: flex;
    align-items: center;
    gap: 24px;
    margin-bottom: 24px;
    padding: 16px;
    background: #f5f7fa;
    border-radius: 8px;

    .stats-item {
      text-align: center;

      .stats-value {
        font-size: 28px;
        font-weight: 600;
        color: #303133;
      }

      .stats-label {
        font-size: 14px;
        color: #909399;
        margin-top: 4px;
      }

      &.read .stats-value {
        color: #67c23a;
      }

      &.unread .stats-value {
        color: #e6a23c;
      }
    }

    .stats-progress {
      flex: 1;
      max-width: 300px;
    }
  }

  .filter-section {
    margin-bottom: 16px;
  }
}
</style>