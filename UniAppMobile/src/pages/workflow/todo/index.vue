<!-- pages/workflow/todo/index.vue -->
<template>
  <view class="todo-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <input
        class="search-input"
        v-model="keyword"
        placeholder="搜索任务"
        @confirm="handleSearch"
      />
      <view class="filter-bar">
        <text :class="['filter-item', urgentOnly ? 'active' : '']" @click="toggleUrgent">
          仅紧急
        </text>
      </view>
    </view>

    <!-- 任务列表 -->
    <scroll-view
      scroll-y
      class="task-list"
      @scrolltolower="loadMore"
    >
      <Loading :show="loading" />

      <CardItem
        v-for="item in taskList"
        :key="item.id"
        :title="item.title"
        :process-name="item.processName"
        :applicant-name="item.applicantName"
        :create-time="item.createTime"
        :status="item.status"
        :urgent="item.urgent"
        @click="handleDetail(item.id)"
      />

      <Empty v-if="!loading && taskList.length === 0" text="暂无待办任务" />

      <view v-if="hasMore && !loading" class="load-more">
        <text>加载更多...</text>
      </view>
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getTodoList } from '@/api/workflow/taskApi'
import type { TaskInfo, TaskQueryParams } from '@/types'
import CardItem from '@/components/CardItem/index.vue'
import Loading from '@/components/Loading/index.vue'
import Empty from '@/components/Empty/index.vue'

const loading = ref(false)
const keyword = ref('')
const urgentOnly = ref(false)
const taskList = ref<TaskInfo[]>([])
const hasMore = ref(false)

const queryParams = reactive<TaskQueryParams>({
  pageIndex: 1,
  pageSize: 10,
})

onMounted(() => {
  loadTasks()
})

const loadTasks = async () => {
  loading.value = true
  try {
    const res = await getTodoList({
      ...queryParams,
      keyword: keyword.value,
      urgent: urgentOnly.value,
    })
    if (queryParams.pageIndex === 1) {
      taskList.value = res.list
    } else {
      taskList.value.push(...res.list)
    }
    hasMore.value = taskList.value.length < res.total
  } catch (error) {
    // 错误已在 request.ts 处理
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  queryParams.pageIndex = 1
  loadTasks()
}

const toggleUrgent = () => {
  urgentOnly.value = !urgentOnly.value
  handleSearch()
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
.todo-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: $u-bg-color;
}

.search-bar {
  padding: 20rpx;
  background: #fff;

  .search-input {
    width: 100%;
    height: 72rpx;
    padding: 0 20rpx;
    border: 1rpx solid $u-border-color;
    border-radius: $u-radius;
    font-size: 28rpx;
  }

  .filter-bar {
    margin-top: 16rpx;

    .filter-item {
      font-size: 26rpx;
      padding: 8rpx 16rpx;
      border-radius: $u-radius;
      background: $u-light-color;
      color: $u-content-color;

      &.active {
        background: $u-primary;
        color: #fff;
      }
    }
  }
}

.task-list {
  flex: 1;
  padding: 20rpx;
}

.load-more {
  text-align: center;
  padding: 20rpx;
  color: $u-content-color;
}
</style>