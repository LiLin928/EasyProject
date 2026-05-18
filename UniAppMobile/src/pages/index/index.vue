<!-- pages/index/index.vue -->
<template>
  <view class="index-page">
    <!-- 用户信息 -->
    <view class="user-section">
      <image
        class="avatar"
        :src="userInfo?.avatar || '/static/logo.png'"
        mode="aspectFill"
      />
      <view class="user-info">
        <text class="name">{{ userInfo?.realName || userInfo?.userName || '未登录' }}</text>
        <text class="role">{{ userInfo?.roles?.join('、') || '普通用户' }}</text>
      </view>
      <view class="logout-btn" @click="handleLogout">
        <text>退出</text>
      </view>
    </view>

    <!-- 数据概览 -->
    <view class="stats-section">
      <view class="stat-card">
        <text class="stat-value">{{ stats.todoCount }}</text>
        <text class="stat-label">待办任务</text>
      </view>
      <view class="stat-card">
        <text class="stat-value">{{ stats.productCount }}</text>
        <text class="stat-label">商品总数</text>
      </view>
      <view class="stat-card">
        <text class="stat-value">{{ stats.userCount }}</text>
        <text class="stat-label">用户总数</text>
      </view>
    </view>

    <!-- 快捷入口（8宫格） -->
    <view class="quick-entry">
      <text class="section-title">快捷入口</text>
      <view class="entry-grid">
        <view
          v-for="item in quickEntries"
          :key="item.path"
          class="entry-item"
          @click="navigateTo(item.path)"
        >
          <text class="entry-icon">{{ item.icon }}</text>
          <text class="entry-text">{{ item.name }}</text>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'

const userStore = useUserStore()
const userInfo = computed(() => userStore.userInfo)

const stats = ref({
  todoCount: 12,
  productCount: 156,
  userCount: 89,
})

const quickEntries = [
  { name: '报表', icon: '📊', path: '/pages/report/list/index' },
  { name: '大屏', icon: '🖥️', path: '/pages/screen/list/index' },
  { name: '日志', icon: '📝', path: '/pages/log/list/index' },
  { name: '用户', icon: '👤', path: '/pages/basic/user/index' },
  { name: '角色', icon: '👥', path: '/pages/basic/role/index' },
  { name: '部门', icon: '🏢', path: '/pages/basic/department/index' },
  { name: '菜单', icon: '📋', path: '/pages/basic/menu/index' },
  { name: '字典', icon: '📖', path: '/pages/basic/dict/index' },
]

onMounted(() => {
  // 可在此处加载真实统计数据
})

const navigateTo = (url: string) => {
  uni.navigateTo({ url })
}

const handleLogout = async () => {
  try {
    await userStore.logoutAction()
    uni.showToast({ title: '已退出登录', icon: 'success' })
    uni.navigateTo({ url: '/pages/login/index' })
  } catch (error) {
    uni.showToast({ title: '退出失败', icon: 'none' })
  }
}
</script>

<style lang="scss" scoped>
.index-page {
  padding: 20rpx;
  background: $u-bg-color;
  min-height: 100vh;
}

.user-section {
  display: flex;
  align-items: center;
  padding: 24rpx;
  background: #fff;
  border-radius: $u-radius-lg;
  margin-bottom: 20rpx;

  .avatar {
    width: 80rpx;
    height: 80rpx;
    border-radius: 50%;
  }

  .user-info {
    flex: 1;
    margin-left: 20rpx;

    .name {
      font-size: 32rpx;
      font-weight: 600;
      color: $u-main-color;
    }

    .role {
      font-size: 24rpx;
      color: $u-content-color;
      margin-top: 8rpx;
      display: block;
    }
  }

  .logout-btn {
    padding: 8rpx 16rpx;
    background: $u-light-color;
    border-radius: $u-radius;

    text {
      font-size: 24rpx;
      color: $u-content-color;
    }
  }
}

.stats-section {
  display: flex;
  gap: 16rpx;
  margin-bottom: 20rpx;

  .stat-card {
    flex: 1;
    padding: 24rpx;
    background: #fff;
    border-radius: $u-radius;
    text-align: center;

    .stat-value {
      font-size: 40rpx;
      font-weight: 600;
      color: $u-primary;
    }

    .stat-label {
      font-size: 24rpx;
      color: $u-content-color;
      margin-top: 8rpx;
      display: block;
    }
  }
}

.quick-entry {
  background: #fff;
  border-radius: $u-radius-lg;
  padding: 24rpx;

  .section-title {
    font-size: 28rpx;
    font-weight: 600;
    color: $u-main-color;
    margin-bottom: 20rpx;
    display: block;
  }

  .entry-grid {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 24rpx;

    .entry-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 20rpx;

      .entry-icon {
        font-size: 48rpx;
      }

      .entry-text {
        font-size: 24rpx;
        color: $u-main-color;
        margin-top: 12rpx;
      }
    }
  }
}
</style>