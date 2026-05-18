<!-- pages/screen/list/index.vue -->
<template>
  <view class="screen-list-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <input
        v-model="searchKeyword"
        class="search-input"
        type="text"
        placeholder="搜索大屏名称"
        @confirm="handleSearch"
      />
      <view class="search-btn" @click="handleSearch">
        <text>搜索</text>
      </view>
    </view>

    <!-- 筛选栏 -->
    <view class="filter-bar">
      <view class="filter-item" @click="showStatusPicker = true">
        <text class="filter-label">{{ statusText }}</text>
        <text class="filter-arrow">▼</text>
      </view>
    </view>

    <!-- 大屏列表 -->
    <scroll-view
      class="screen-scroll"
      scroll-y
      @scrolltolower="loadMore"
    >
      <view class="screen-list">
        <view
          v-for="item in screenList"
          :key="item.id"
          class="screen-card"
          @click="handleView(item)"
        >
          <image
            class="screen-thumbnail"
            :src="item.thumbnail || '/static/logo.png'"
            mode="aspectFit"
          />
          <view class="screen-info">
            <view class="screen-header">
              <text class="screen-name">{{ item.name }}</text>
              <view :class="['status-tag', getStatusClass(item.status)]">
                <text>{{ getStatusText(item.status) }}</text>
              </view>
            </view>
            <text v-if="item.description" class="screen-desc">{{ item.description }}</text>
            <view class="screen-footer">
              <text class="screen-time">{{ item.createTime }}</text>
            </view>
          </view>
        </view>

        <!-- 加载状态 -->
        <view v-if="loading" class="loading-tip">
          <text>加载中...</text>
        </view>

        <!-- 加载完成 -->
        <view v-if="!hasMore && screenList.length > 0" class="no-more-tip">
          <text>没有更多了</text>
        </view>

        <!-- 空状态 -->
        <view v-if="!loading && screenList.length === 0" class="empty-state">
          <text class="empty-icon">📺</text>
          <text class="empty-text">暂无大屏数据</text>
        </view>
      </view>

      <!-- 底部提示 -->
      <view v-if="screenList.length > 0" class="bottom-tip">
        <text>移动端仅支持查看，设计请前往PC端</text>
      </view>
    </scroll-view>

    <!-- 状态选择器 -->
    <view v-if="showStatusPicker" class="picker-mask" @click="showStatusPicker = false">
      <view class="picker-content" @click.stop>
        <view class="picker-header">
          <text class="picker-title">选择状态</text>
          <view class="picker-close" @click="showStatusPicker = false">
            <text>×</text>
          </view>
        </view>
        <view class="picker-list">
          <view
            class="picker-item"
            :class="{ active: queryParams.status === undefined }"
            @click="selectStatus(undefined)"
          >
            <text>全部状态</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.status === ScreenStatus.Published }"
            @click="selectStatus(ScreenStatus.Published)"
          >
            <text>已发布</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.status === ScreenStatus.Draft }"
            @click="selectStatus(ScreenStatus.Draft)"
          >
            <text>未发布</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.status === ScreenStatus.Archived }"
            @click="selectStatus(ScreenStatus.Archived)"
          >
            <text>已归档</text>
          </view>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getScreenList } from '@/api/screen/screenApi'
import type { Screen, ScreenQueryParams, ScreenStatus } from '@/types/screen'

// 状态枚举
const ScreenStatusEnum = {
  Draft: 0,      // 未发布
  Published: 1,  // 已发布
  Archived: 2,   // 已归档
}

// 搜索关键字
const searchKeyword = ref('')

// 搜索防抖定时器
let searchTimer: number | null = null

// 查询参数
const queryParams = ref<ScreenQueryParams>({
  pageIndex: 1,
  pageSize: 10,
  keyword: undefined,
  status: undefined,
})

// 大屏列表
const screenList = ref<Screen[]>([])

// 加载状态
const loading = ref(false)
const hasMore = ref(true)

// 选择器显示状态
const showStatusPicker = ref(false)

// 状态文本
const statusText = computed(() => {
  if (queryParams.value.status === undefined) return '全部状态'
  return getStatusText(queryParams.value.status)
})

// 获取状态文本
const getStatusText = (status: ScreenStatus | number): string => {
  const statusMap: Record<number, string> = {
    0: '未发布',
    1: '已发布',
    2: '已归档',
  }
  return statusMap[status] || '未知状态'
}

// 获取状态样式类
const getStatusClass = (status: ScreenStatus | number): string => {
  const classMap: Record<number, string> = {
    0: 'tag-draft',     // 未发布 - 灰色
    1: 'tag-published', // 已发布 - 蓝色
    2: 'tag-archived',  // 已归档 - 橙色
  }
  return classMap[status] || 'tag-draft'
}

// 加载大屏列表
const loadScreens = async (reset = false) => {
  if (loading.value) return
  if (!reset && !hasMore.value) return

  loading.value = true
  try {
    if (reset) {
      queryParams.value.pageIndex = 1
      screenList.value = []
      hasMore.value = true
    }

    const response = await getScreenList(queryParams.value)
    if (reset) {
      screenList.value = response.list
    } else {
      screenList.value = [...screenList.value, ...response.list]
    }

    hasMore.value = screenList.value.length < response.total
    queryParams.value.pageIndex++
  } catch (error) {
    console.error('加载大屏列表失败:', error)
    uni.showToast({ title: '加载失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 搜索（带防抖）
const handleSearch = () => {
  if (searchTimer) {
    clearTimeout(searchTimer)
  }
  searchTimer = setTimeout(() => {
    queryParams.value.keyword = searchKeyword.value || undefined
    loadScreens(true)
  }, 300)
}

// 加载更多
const loadMore = () => {
  if (!loading.value && hasMore.value) {
    loadScreens()
  }
}

// 选择状态
const selectStatus = (status: ScreenStatus | undefined) => {
  queryParams.value.status = status
  showStatusPicker.value = false
  loadScreens(true)
}

// 查看大屏
const handleView = (item: Screen) => {
  uni.navigateTo({ url: `/pages/screen/view/index?id=${item.id}` })
}

onMounted(() => {
  loadScreens(true)
})
</script>

<style lang="scss" scoped>
.screen-list-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: $u-bg-color;
}

.search-bar {
  display: flex;
  padding: 20rpx;
  background: #fff;
  gap: 16rpx;

  .search-input {
    flex: 1;
    height: 72rpx;
    padding: 0 24rpx;
    background: $u-light-color;
    border-radius: $u-radius;
    font-size: 28rpx;
  }

  .search-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 120rpx;
    height: 72rpx;
    background: $u-primary;
    border-radius: $u-radius;

    text {
      color: #fff;
      font-size: 28rpx;
    }
  }
}

.filter-bar {
  display: flex;
  padding: 16rpx 20rpx;
  background: #fff;
  border-top: 1rpx solid $u-border-color;
  gap: 20rpx;

  .filter-item {
    display: flex;
    align-items: center;
    padding: 12rpx 20rpx;
    background: $u-light-color;
    border-radius: $u-radius;

    .filter-label {
      font-size: 26rpx;
      color: $u-main-color;
    }

    .filter-arrow {
      font-size: 20rpx;
      color: $u-content-color;
      margin-left: 8rpx;
    }
  }
}

.screen-scroll {
  flex: 1;
  overflow: hidden;
}

.screen-list {
  padding: 20rpx;
}

.screen-card {
  background: #fff;
  border-radius: $u-radius-lg;
  padding: 20rpx;
  margin-bottom: 20rpx;
  display: flex;
  gap: 20rpx;

  .screen-thumbnail {
    width: 200rpx;
    height: 150rpx;
    border-radius: $u-radius;
    background: $u-light-color;
    flex-shrink: 0;
  }

  .screen-info {
    flex: 1;
    display: flex;
    flex-direction: column;
  }

  .screen-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 12rpx;
  }

  .screen-name {
    font-size: 30rpx;
    font-weight: 600;
    color: $u-main-color;
    flex: 1;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .status-tag {
    font-size: 20rpx;
    padding: 6rpx 16rpx;
    border-radius: 4rpx;
    flex-shrink: 0;

    text {
      color: #fff;
    }
  }

  .tag-draft {
    background: $u-content-color;  // 灰色 - 未发布
  }

  .tag-published {
    background: $u-primary;  // 蓝色 - 已发布
  }

  .tag-archived {
    background: $u-warning;  // 橙色 - 已归档
  }

  .screen-desc {
    font-size: 24rpx;
    color: $u-tips-color;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    margin-bottom: 12rpx;
  }

  .screen-footer {
    margin-top: auto;
  }

  .screen-time {
    font-size: 22rpx;
    color: $u-tips-color;
  }
}

.loading-tip,
.no-more-tip {
  text-align: center;
  padding: 30rpx 0;

  text {
    font-size: 26rpx;
    color: $u-tips-color;
  }
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 100rpx 0;

  .empty-icon {
    font-size: 80rpx;
  }

  .empty-text {
    font-size: 28rpx;
    color: $u-tips-color;
    margin-top: 20rpx;
  }
}

.bottom-tip {
  text-align: center;
  padding: 20rpx 30rpx 40rpx;
  background: $u-light-color;
  margin: 0 20rpx 20rpx;
  border-radius: $u-radius;

  text {
    font-size: 24rpx;
    color: $u-content-color;
  }
}

.picker-mask {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 1000;
  display: flex;
  align-items: flex-end;
}

.picker-content {
  width: 100%;
  background: #fff;
  border-radius: $u-radius-lg $u-radius-lg 0 0;
  max-height: 70vh;
}

.picker-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 30rpx;
  border-bottom: 1rpx solid $u-border-color;

  .picker-title {
    font-size: 32rpx;
    font-weight: 600;
    color: $u-main-color;
  }

  .picker-close {
    font-size: 40rpx;
    color: $u-content-color;
    line-height: 1;
  }
}

.picker-list {
  max-height: 60vh;
}

.picker-item {
  padding: 30rpx;
  border-bottom: 1rpx solid $u-border-color;

  text {
    font-size: 28rpx;
    color: $u-main-color;
  }

  &.active {
    background: rgba($u-primary, 0.1);

    text {
      color: $u-primary;
      font-weight: 600;
    }
  }
}
</style>