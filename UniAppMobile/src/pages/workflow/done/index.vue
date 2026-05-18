<!-- pages/workflow/done/index.vue -->
<template>
  <view class="done-page">
    <!-- 时间筛选 -->
    <view class="filter-bar">
      <text :class="['filter-btn', dateFilter === 'today' ? 'active' : '']" @click="filterByDate('today')">今天</text>
      <text :class="['filter-btn', dateFilter === 'week' ? 'active' : '']" @click="filterByDate('week')">本周</text>
      <text :class="['filter-btn', dateFilter === 'month' ? 'active' : '']" @click="filterByDate('month')">本月</text>
      <text :class="['filter-btn', dateFilter === 'all' ? 'active' : '']" @click="filterByDate('all')">全部</text>
    </view>

    <!-- 任务列表 -->
    <scroll-view scroll-y class="task-list" @scrolltolower="loadMore">
      <Loading :show="loading" />

      <CardItem
        v-for="item in taskList"
        :key="item.id"
        :title="item.title"
        :process-name="item.processName"
        :applicant-name="item.applicantName"
        :create-time="item.createTime"
        :status="item.status"
        @click="handleDetail(item.id)"
      />

      <Empty v-if="!loading && taskList.length === 0" text="暂无已办任务" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getDoneList } from '@/api/workflow/taskApi'
import type { TaskInfo } from '@/types'
import CardItem from '@/components/CardItem/index.vue'
import Loading from '@/components/Loading/index.vue'
import Empty from '@/components/Empty/index.vue'

const loading = ref(false)
const taskList = ref<TaskInfo[]>([])
const hasMore = ref(false)
const dateFilter = ref('all')

const queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
  startTime: '',
  endTime: '',
})

onMounted(() => {
  loadTasks()
})

const loadTasks = async () => {
  loading.value = true
  try {
    const res = await getDoneList(queryParams)
    if (queryParams.pageIndex === 1) {
      taskList.value = res.list
    } else {
      taskList.value.push(...res.list)
    }
    hasMore.value = taskList.value.length < res.total
  } finally {
    loading.value = false
  }
}

const filterByDate = (type: string) => {
  dateFilter.value = type
  queryParams.pageIndex = 1

  // 根据类型设置时间范围
  const now = new Date()
  if (type === 'today') {
    queryParams.startTime = now.toISOString().split('T')[0]
    queryParams.endTime = queryParams.startTime
  } else if (type === 'week') {
    const weekStart = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000)
    queryParams.startTime = weekStart.toISOString().split('T')[0]
    queryParams.endTime = now.toISOString().split('T')[0]
  } else if (type === 'month') {
    const monthStart = new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000)
    queryParams.startTime = monthStart.toISOString().split('T')[0]
    queryParams.endTime = now.toISOString().split('T')[0]
  } else {
    queryParams.startTime = ''
    queryParams.endTime = ''
  }

  loadTasks()
}

const loadMore = () => {
  if (hasMore.value && !loading.value) {
    queryParams.pageIndex++
    loadTasks()
  }
}

const handleDetail = (taskId: string) => {
  uni.navigateTo({ url: `/pages/workflow/detail/index?id=${taskId}` })
}
</script>

<style lang="scss" scoped>
.done-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: $u-bg-color;
}

.filter-bar {
  display: flex;
  gap: 16rpx;
  padding: 20rpx;
  background: #fff;

  .filter-btn {
    font-size: 26rpx;
    padding: 8rpx 24rpx;
    border-radius: $u-radius;
    background: $u-light-color;
    color: $u-content-color;

    &.active {
      background: $u-primary;
      color: #fff;
    }
  }
}

.task-list {
  flex: 1;
  padding: 20rpx;
}
</style>