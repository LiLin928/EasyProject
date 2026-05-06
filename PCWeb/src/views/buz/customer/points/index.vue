<!-- src/views/buz/customer/points/index.vue -->
<template>
  <div class="points-log-container">
    <el-card shadow="never">
      <template #header>
        <div class="card-header">
          <span>{{ t('customer.pointsLog') }}</span>
        </div>
      </template>

      <!-- Search form -->
      <el-form :model="queryParams" :inline="true" class="search-form">
        <el-form-item :label="t('customer.phone')">
          <el-input
            v-model="queryParams.userKeyword"
            :placeholder="t('customer.phonePlaceholder')"
            clearable
            @keyup.enter="handleSearch"
          />
        </el-form-item>
        <el-form-item :label="t('customer.changeType')">
          <el-select
            v-model="queryParams.type"
            :placeholder="t('customer.changeTypePlaceholder')"
            clearable
            style="width: 150px"
          >
            <el-option :label="t('customer.changeTypeAll')" value="" />
            <el-option :label="t('customer.changeTypeReview')" value="review" />
            <el-option :label="t('customer.changeTypeOrder')" value="order" />
            <el-option :label="t('customer.changeTypeExchange')" value="exchange" />
            <el-option :label="t('customer.changeTypeRefund')" value="refund" />
            <el-option :label="t('customer.changeTypeSystem')" value="system" />
          </el-select>
        </el-form-item>
        <el-form-item :label="t('customer.changeTimeRange')">
          <el-date-picker
            v-model="dateRange"
            type="daterange"
            :start-placeholder="t('common.date.startPlaceholder')"
            :end-placeholder="t('common.date.endPlaceholder')"
            format="YYYY-MM-DD"
            value-format="YYYY-MM-DD"
            style="width: 240px"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            {{ t('customer.search') }}
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            {{ t('customer.reset') }}
          </el-button>
        </el-form-item>
      </el-form>

      <!-- Table -->
      <el-table v-loading="loading" :data="tableData" border>
        <el-table-column prop="userName" :label="t('customer.nickname')" min-width="100" />
        <el-table-column prop="userPhone" :label="t('customer.phone')" min-width="120" />
        <el-table-column prop="typeText" :label="t('customer.changeType')" width="120" align="center">
          <template #default="{ row }">
            <el-tag :type="getChangeTypeTag(row.type)" size="small">
              {{ row.typeText }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="points" :label="t('customer.changeAmount')" width="120" align="center">
          <template #default="{ row }">
            <span :class="row.points > 0 ? 'positive' : 'negative'">
              {{ row.points > 0 ? '+' : '' }}{{ row.points }}
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="balance" :label="t('customer.afterPoints')" width="100" align="center" />
        <el-table-column prop="reason" :label="t('customer.changeReason')" min-width="200" show-overflow-tooltip />
        <el-table-column prop="createTime" :label="t('customer.changeTime')" width="160" />
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
      />
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, watch } from 'vue'
import { Search, Refresh } from '@element-plus/icons-vue'
import { getPointsLogList } from '@/api/buz/pointsApi'
import { useLocale } from '@/composables/useLocale'
import type { PointsLog, PointsChangeType } from '@/types'

const { t } = useLocale()

const loading = ref(false)
const tableData = ref<PointsLog[]>([])
const total = ref(0)
const dateRange = ref<[string, string] | null>(null)

const queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  userKeyword: '',
  type: '' as PointsChangeType | '',
  startTime: '',
  endTime: '',
})

// Watch date range change
watch(dateRange, (val) => {
  if (val) {
    queryParams.startTime = val[0]
    queryParams.endTime = val[1]
  } else {
    queryParams.startTime = ''
    queryParams.endTime = ''
  }
})

onMounted(() => {
  handleSearch()
})

const handleSearch = async () => {
  loading.value = true
  try {
    const params: any = {
      pageIndex: queryParams.pageIndex,
      pageSize: queryParams.pageSize,
    }
    if (queryParams.userKeyword) {
      params.userKeyword = queryParams.userKeyword
    }
    if (queryParams.type) {
      params.type = queryParams.type
    }
    if (queryParams.startTime) {
      params.startTime = queryParams.startTime
    }
    if (queryParams.endTime) {
      params.endTime = queryParams.endTime
    }

    const data = await getPointsLogList(params)
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
  queryParams.userKeyword = ''
  queryParams.type = ''
  queryParams.startTime = ''
  queryParams.endTime = ''
  dateRange.value = null
  handleSearch()
}

const getChangeTypeTag = (type: PointsChangeType) => {
  const tags: Record<PointsChangeType, string> = {
    review: 'success',
    order: 'success',
    exchange: 'warning',
    refund: 'danger',
    system: 'info',
  }
  return tags[type] || 'info'
}
</script>

<style scoped lang="scss">
.points-log-container {
  padding: 20px;

  .card-header {
    font-size: 16px;
    font-weight: 500;
  }

  .search-form {
    margin-bottom: 16px;
  }

  .positive {
    color: #67c23a;
    font-weight: 500;
  }

  .negative {
    color: #f56c6c;
    font-weight: 500;
  }

  .el-pagination {
    margin-top: 16px;
    justify-content: flex-end;
  }
}
</style>