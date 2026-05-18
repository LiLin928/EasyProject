<!-- pages/workflow/cc/index.vue -->
<template>
  <view class="cc-page">
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

      <Empty v-if="!loading && taskList.length === 0" text="暂无抄送任务" />
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { getCCList } from '@/api/workflow/taskApi'
import type { TaskInfo } from '@/types'
import CardItem from '@/components/CardItem/index.vue'
import Loading from '@/components/Loading/index.vue'
import Empty from '@/components/Empty/index.vue'

const loading = ref(false)
const taskList = ref<TaskInfo[]>([])
const hasMore = ref(false)

const queryParams = reactive({
  pageIndex: 1,
  pageSize: 10,
})

onMounted(() => {
  loadTasks()
})

const loadTasks = async () => {
  loading.value = true
  try {
    const res = await getCCList(queryParams)
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
.cc-page {
  height: 100vh;
  background: $u-bg-color;
}

.task-list {
  height: 100%;
  padding: 20rpx;
}
</style>