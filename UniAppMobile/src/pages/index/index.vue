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

    <!-- 消息通知入口 -->
    <view class="message-section" @click="navigateTo('/pages/message/list/index')">
      <view class="message-icon">🔔</view>
      <view class="message-content">
        <text class="message-title">消息通知</text>
        <text class="message-desc">{{ messageCount > 0 ? `${messageCount}条未读消息` : '暂无新消息' }}</text>
      </view>
      <view v-if="messageCount > 0" class="message-badge">
        <text>{{ messageCount > 99 ? '99+' : messageCount }}</text>
      </view>
      <text class="message-arrow">›</text>
    </view>

    <!-- 数据概览 -->
    <view class="stats-section">
      <view class="stat-card">
        <text class="stat-value">{{ loading ? '...' : stats.todoCount }}</text>
        <text class="stat-label">待办任务</text>
      </view>
      <view class="stat-card">
        <text class="stat-value">{{ loading ? '...' : stats.productCount }}</text>
        <text class="stat-label">商品总数</text>
      </view>
      <view class="stat-card">
        <text class="stat-value">{{ loading ? '...' : stats.userCount }}</text>
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

    <!-- 版本信息 -->
    <view class="version-section">
      <text class="version-text">当前版本 v{{ currentVersion }}</text>
      <text v-if="hasNewVersion" class="version-update" @click="showUpdateModal">
        有新版本可用
      </text>
    </view>
  </view>
</template>

<script setup lang="ts">
import { computed, ref, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import { getTodoList } from '@/api/workflow/taskApi'
import { getProductList } from '@/api/product/productApi'
import { getUserList } from '@/api/basic/userApi'

const userStore = useUserStore()
const userInfo = computed(() => userStore.userInfo)

// 统计数据
const stats = ref({
  todoCount: 0,
  productCount: 0,
  userCount: 0,
})

// 加载状态
const loading = ref(false)

// 消息数量（暂时使用静态数据，后续对接消息API）
const messageCount = ref(0)

// 版本相关
const currentVersion = ref('1.0.0')
const hasNewVersion = ref(false)
const newVersionInfo = ref({ version: '', description: '' })

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
  loadStats()
  checkVersion()
})

// 加载统计数据
const loadStats = async () => {
  loading.value = true
  try {
    // 并行请求所有统计数据
    const [todoRes, productRes, userRes] = await Promise.all([
      getTodoList({ pageIndex: 1, pageSize: 1 }).catch(() => ({ total: 0, list: [] })),
      getProductList({ pageIndex: 1, pageSize: 1 }).catch(() => ({ total: 0, list: [] })),
      getUserList({ pageIndex: 1, pageSize: 1 }).catch(() => ({ total: 0, list: [] })),
    ])

    stats.value.todoCount = todoRes.total || 0
    stats.value.productCount = productRes.total || 0
    stats.value.userCount = userRes.total || 0
  } catch (error) {
    console.error('加载统计数据失败:', error)
  } finally {
    loading.value = false
  }
}

// 版本检查
const checkVersion = async () => {
  try {
    // 从服务器获取最新版本信息（模拟）
    // 实际项目中应该调用版本检查 API
    // const res = await getVersionInfo()
    // const latestVersion = res.version

    // 模拟版本检查（当前版本与服务器版本一致，无更新）
    const latestVersion = '1.0.0'

    if (latestVersion !== currentVersion.value) {
      hasNewVersion.value = true
      newVersionInfo.value = {
        version: latestVersion,
        description: '有新版本可用，请前往应用商店更新',
      }
    }
  } catch (error) {
    console.error('版本检查失败:', error)
  }
}

// 显示更新弹窗
const showUpdateModal = () => {
  uni.showModal({
    title: '版本更新',
    content: `发现新版本 ${newVersionInfo.value.version}，${newVersionInfo.value.description}`,
    showCancel: true,
    confirmText: '更新',
    cancelText: '稍后',
    success: (res) => {
      if (res.confirm) {
        // 跳转到应用商店或下载页面
        uni.showToast({ title: '请前往应用商店更新', icon: 'none' })
      }
    },
  })
}

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

.message-section {
  display: flex;
  align-items: center;
  padding: 24rpx;
  background: #fff;
  border-radius: $u-radius-lg;
  margin-bottom: 20rpx;

  .message-icon {
    font-size: 40rpx;
  }

  .message-content {
    flex: 1;
    margin-left: 16rpx;

    .message-title {
      font-size: 28rpx;
      font-weight: 600;
      color: $u-main-color;
    }

    .message-desc {
      font-size: 24rpx;
      color: $u-content-color;
      margin-top: 4rpx;
      display: block;
    }
  }

  .message-badge {
    padding: 4rpx 12rpx;
    background: $u-error;
    border-radius: 20rpx;

    text {
      font-size: 22rpx;
      color: #fff;
    }
  }

  .message-arrow {
    font-size: 32rpx;
    color: $u-content-color;
    margin-left: 8rpx;
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

.version-section {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 32rpx 0;
  gap: 16rpx;

  .version-text {
    font-size: 24rpx;
    color: $u-content-color;
  }

  .version-update {
    font-size: 24rpx;
    color: $u-primary;
    text-decoration: underline;
  }
}
</style>