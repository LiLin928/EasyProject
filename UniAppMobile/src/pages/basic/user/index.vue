<!-- pages/basic/user/index.vue -->
<template>
  <view class="user-list-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <input
        v-model="searchKeyword"
        class="search-input"
        type="text"
        placeholder="搜索用户名/姓名"
        @confirm="handleSearch"
      />
      <view class="search-btn" @click="handleSearch">
        <text>搜索</text>
      </view>
    </view>

    <!-- 状态筛选栏 -->
    <view class="filter-bar">
      <view class="filter-item" @click="showStatusPicker = true">
        <text class="filter-label">{{ statusText }}</text>
        <text class="filter-arrow">▼</text>
      </view>
    </view>

    <!-- 用户列表 -->
    <scroll-view
      class="user-scroll"
      scroll-y
      @scrolltolower="loadMore"
    >
      <view class="user-list">
        <view
          v-for="item in userList"
          :key="item.id"
          class="user-card"
          @click="handleDetail(item)"
        >
          <view class="user-info">
            <view class="user-header">
              <text class="user-name">{{ item.userName }}</text>
              <view class="user-tags">
                <text :class="['tag', item.status === UserStatus.Enabled ? 'tag-enabled' : 'tag-disabled']">
                  {{ item.status === UserStatus.Enabled ? '启用' : '禁用' }}
                </text>
              </view>
            </view>
            <view class="user-detail">
              <text class="detail-row">
                <text class="label">姓名：</text>
                <text class="value">{{ item.realName || '-' }}</text>
              </text>
              <text class="detail-row">
                <text class="label">手机：</text>
                <text class="value">{{ item.phone || '-' }}</text>
              </text>
              <text class="detail-row">
                <text class="label">部门：</text>
                <text class="value">{{ item.departmentName || '-' }}</text>
              </text>
            </view>
          </view>
          <view class="user-actions">
            <view
              :class="['action-btn', item.status === UserStatus.Enabled ? 'action-disable' : 'action-enable']"
              @click.stop="handleToggleStatus(item)"
            >
              <text>{{ item.status === UserStatus.Enabled ? '禁用' : '启用' }}</text>
            </view>
          </view>
        </view>

        <!-- 加载状态 -->
        <view v-if="loading" class="loading-tip">
          <text>加载中...</text>
        </view>

        <!-- 加载完成 -->
        <view v-if="!hasMore && userList.length > 0" class="no-more-tip">
          <text>没有更多了</text>
        </view>

        <!-- 空状态 -->
        <view v-if="!loading && userList.length === 0" class="empty-state">
          <text class="empty-icon">👤</text>
          <text class="empty-text">暂无用户数据</text>
        </view>
      </view>
    </scroll-view>

    <!-- 新增按钮 -->
    <view class="add-btn" @click="handleAdd">
      <text>+</text>
    </view>

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
            :class="{ active: queryParams.status === UserStatus.Enabled }"
            @click="selectStatus(UserStatus.Enabled)"
          >
            <text>已启用</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.status === UserStatus.Disabled }"
            @click="selectStatus(UserStatus.Disabled)"
          >
            <text>已禁用</text>
          </view>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getUserList, enableUser, disableUser } from '@/api/basic/userApi'
import type { User, UserQueryParams } from '@/types/basic'
import { UserStatus } from '@/types/basic'

// 搜索关键字
const searchKeyword = ref('')

// 查询参数
const queryParams = ref<UserQueryParams>({
  pageIndex: 1,
  pageSize: 10,
  keyword: undefined,
  status: undefined,
})

// 用户列表
const userList = ref<User[]>([])

// 加载状态
const loading = ref(false)
const hasMore = ref(true)

// 选择器显示状态
const showStatusPicker = ref(false)

// 状态文本
const statusText = computed(() => {
  if (queryParams.value.status === undefined) return '全部状态'
  return queryParams.value.status === UserStatus.Enabled ? '已启用' : '已禁用'
})

// 加载用户列表
const loadUsers = async (reset = false) => {
  if (loading.value) return
  if (!reset && !hasMore.value) return

  loading.value = true
  try {
    if (reset) {
      queryParams.value.pageIndex = 1
      userList.value = []
      hasMore.value = true
    }

    const response = await getUserList(queryParams.value)
    if (reset) {
      userList.value = response.list
    } else {
      userList.value = [...userList.value, ...response.list]
    }

    hasMore.value = userList.value.length < response.total
    queryParams.value.pageIndex++
  } catch (error) {
    console.error('加载用户列表失败:', error)
    uni.showToast({ title: '加载失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  queryParams.value.keyword = searchKeyword.value || undefined
  loadUsers(true)
}

// 加载更多
const loadMore = () => {
  if (!loading.value && hasMore.value) {
    loadUsers()
  }
}

// 选择状态
const selectStatus = (status: UserStatus | undefined) => {
  queryParams.value.status = status
  showStatusPicker.value = false
  loadUsers(true)
}

// 新增用户
const handleAdd = () => {
  // TODO: 导航到新增页面
  uni.showToast({ title: '新增用户功能待实现', icon: 'none' })
}

// 查看详情
const handleDetail = (item: User) => {
  // TODO: 导航到详情页面
  uni.showToast({ title: `查看用户: ${item.userName}`, icon: 'none' })
}

// 切换启用/禁用状态
const handleToggleStatus = async (item: User) => {
  const action = item.status === UserStatus.Enabled ? '禁用' : '启用'
  try {
    uni.showLoading({ title: `${action}中...` })
    if (item.status === UserStatus.Enabled) {
      await disableUser(item.id)
    } else {
      await enableUser(item.id)
    }
    uni.hideLoading()
    uni.showToast({ title: `${action}成功`, icon: 'success' })
    loadUsers(true)
  } catch (error) {
    uni.hideLoading()
    uni.showToast({ title: `${action}失败`, icon: 'none' })
  }
}

onMounted(() => {
  loadUsers(true)
})
</script>

<style lang="scss" scoped>
.user-list-page {
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

.user-scroll {
  flex: 1;
  overflow: hidden;
}

.user-list {
  padding: 20rpx;
}

.user-card {
  display: flex;
  justify-content: space-between;
  background: #fff;
  border-radius: $u-radius-lg;
  padding: 24rpx;
  margin-bottom: 20rpx;

  .user-info {
    flex: 1;
    display: flex;
    flex-direction: column;
  }

  .user-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 16rpx;
  }

  .user-name {
    font-size: 30rpx;
    font-weight: 600;
    color: $u-main-color;
    flex: 1;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .user-tags {
    display: flex;
    gap: 8rpx;
    flex-shrink: 0;

    .tag {
      font-size: 22rpx;
      padding: 4rpx 12rpx;
      border-radius: 4rpx;
    }

    .tag-enabled {
      background: rgba($u-primary, 0.1);
      color: $u-primary;
    }

    .tag-disabled {
      background: rgba($u-content-color, 0.1);
      color: $u-content-color;
    }
  }

  .user-detail {
    display: flex;
    flex-direction: column;
    gap: 8rpx;

    .detail-row {
      display: flex;
      align-items: center;

      .label {
        font-size: 24rpx;
        color: $u-tips-color;
        width: 100rpx;
      }

      .value {
        font-size: 24rpx;
        color: $u-main-color;
      }
    }
  }

  .user-actions {
    display: flex;
    align-items: center;
    padding-left: 20rpx;

    .action-btn {
      padding: 12rpx 24rpx;
      border-radius: $u-radius;
      font-size: 24rpx;
    }

    .action-enable {
      background: rgba($u-success, 0.1);
      color: $u-success;
    }

    .action-disable {
      background: rgba($u-warning, 0.1);
      color: $u-warning;
    }
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

.add-btn {
  position: fixed;
  right: 40rpx;
  bottom: 200rpx;
  width: 100rpx;
  height: 100rpx;
  background: $u-primary;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4rpx 16rpx rgba($u-primary, 0.4);

  text {
    font-size: 48rpx;
    color: #fff;
    line-height: 1;
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