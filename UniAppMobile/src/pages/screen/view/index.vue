<!-- pages/screen/view/index.vue -->
<template>
  <view class="screen-view-page">
    <!-- WebView 嵌入区域 -->
    <view class="webview-container" :class="{ fullscreen: isFullscreen }">
      <!-- 加载进度 -->
      <view v-if="loadingProgress > 0 && loadingProgress < 100" class="loading-progress">
        <view class="progress-bar" :style="{ width: loadingProgress + '%' }" />
        <text class="progress-text">{{ loadingProgress }}%</text>
      </view>

      <!-- 预加载提示 -->
      <view v-if="isPreloading && !screenUrl" class="preload-state">
        <text class="preload-icon">⏳</text>
        <text class="preload-text">正在预加载大屏资源...</text>
      </view>

      <!-- WebView -->
      <web-view
        v-if="screenUrl"
        :src="screenUrl"
        :style="{ transform: `scale(${scaleValue})`, transformOrigin: 'top left' }"
        @message="onWebViewMessage"
      />

      <!-- 加载失败提示 -->
      <view v-if="loadError" class="error-state">
        <text class="error-icon">!</text>
        <text class="error-text">大屏加载失败</text>
        <view class="retry-btn" @click="handleRefresh">
          <text>重新加载</text>
        </view>
      </view>
    </view>

    <!-- 操作按钮栏（非全屏时显示） -->
    <view v-if="!isFullscreen" class="action-bar">
      <view class="action-btn" @click="handleRefresh">
        <text class="btn-icon">🔄</text>
        <text class="btn-text">刷新</text>
      </view>
      <view class="action-btn" @click="handleFullscreen">
        <text class="btn-icon">⛶</text>
        <text class="btn-text">全屏</text>
      </view>
      <view class="action-btn" @click="showScalePicker = true">
        <text class="btn-icon">🔍</text>
        <text class="btn-text">{{ scaleText }}</text>
      </view>
    </view>

    <!-- 全屏退出按钮 -->
    <view v-if="isFullscreen" class="fullscreen-exit" @click="exitFullscreen">
      <text class="exit-icon">✕</text>
    </view>

    <!-- 缩放选择器 -->
    <view v-if="showScalePicker" class="picker-mask" @click="showScalePicker = false">
      <view class="picker-content" @click.stop>
        <view class="picker-header">
          <text class="picker-title">选择缩放比例</text>
          <view class="picker-close" @click="showScalePicker = false">
            <text>×</text>
          </view>
        </view>
        <view class="picker-list">
          <view
            v-for="opt in scaleOptions"
            :key="opt.value"
            :class="['picker-item', { active: currentScale === opt.value }]"
            @click="selectScale(opt.value)"
          >
            <text>{{ opt.label }}</text>
          </view>
        </view>
      </view>
    </view>

    <!-- 底部提示 -->
    <view v-if="!isFullscreen" class="bottom-tip">
      <text>移动端仅支持查看，设计请前往PC端</text>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { getScreenDetail } from '@/api/screen/screenApi'
import { setCache, getCache, removeCache } from '@/utils/storage'
import type { Screen } from '@/types/screen'

// 缩放选项
const scaleOptions = [
  { label: '50%', value: 0.5 },
  { label: '75%', value: 0.75 },
  { label: '100%', value: 1 },
  { label: '125%', value: 1.25 },
]

// 预加载缓存过期时间（5分钟）
const PRELOAD_CACHE_EXPIRE = 300

// 大屏ID
const screenId = ref('')

// 大屏详情
const screenDetail = ref<Screen | null>(null)

// 大屏URL
const screenUrl = ref('')

// 加载进度
const loadingProgress = ref(0)

// 加载错误
const loadError = ref(false)

// 全屏状态
const isFullscreen = ref(false)

// 当前缩放比例
const currentScale = ref(1)

// 显示缩放选择器
const showScalePicker = ref(false)

// 预加载状态
const isPreloading = ref(false)

// 缩放文本
const scaleText = computed(() => {
  const opt = scaleOptions.find(o => o.value === currentScale.value)
  return opt?.label || '100%'
})

// 缩放值（用于样式）
const scaleValue = computed(() => {
  return currentScale.value
})

/** 预加载大屏资源 */
const preloadScreen = async (id: string) => {
  isPreloading.value = true
  loadingProgress.value = 10

  // 尝试从缓存获取
  const cachedUrl = getCache<string>(`screen_url_${id}`)
  if (cachedUrl) {
    screenUrl.value = cachedUrl
    loadingProgress.value = 80
    isPreloading.value = false
    // 后台更新缓存
    updateScreenCache(id)
    return
  }

  // 预加载大屏详情
  try {
    const detail = await getScreenDetail(id)
    loadingProgress.value = 50

    if (detail.publishUrl) {
      // 缓存 URL
      setCache(`screen_url_${id}`, detail.publishUrl, PRELOAD_CACHE_EXPIRE)
      screenUrl.value = detail.publishUrl
      screenDetail.value = detail
      loadingProgress.value = 80
    } else {
      loadError.value = true
      loadingProgress.value = 0
    }
  } catch (error) {
    console.error('预加载失败:', error)
    // 预加载失败不显示错误，等待用户手动刷新
  } finally {
    isPreloading.value = false
  }
}

/** 后台更新缓存 */
const updateScreenCache = async (id: string) => {
  try {
    const detail = await getScreenDetail(id)
    if (detail.publishUrl) {
      setCache(`screen_url_${id}`, detail.publishUrl, PRELOAD_CACHE_EXPIRE)
      screenDetail.value = detail
    }
  } catch (error) {
    console.error('更新缓存失败:', error)
  }
}

/** 加载大屏详情 */
const loadScreen = async () => {
  if (!screenId.value) return

  loadingProgress.value = 10
  loadError.value = false
  isPreloading.value = true

  try {
    // 加载大屏详情
    screenDetail.value = await getScreenDetail(screenId.value)
    loadingProgress.value = 50

    // 获取发布URL
    if (screenDetail.value.publishUrl) {
      screenUrl.value = screenDetail.value.publishUrl
      // 缓存 URL
      setCache(`screen_url_${screenId.value}`, screenUrl.value, PRELOAD_CACHE_EXPIRE)
      loadingProgress.value = 80
    } else {
      // 如果没有发布URL，显示错误
      loadError.value = true
      loadingProgress.value = 0
      uni.showToast({ title: '大屏未发布', icon: 'none' })
    }
  } catch (error) {
    console.error('加载大屏失败:', error)
    loadError.value = true
    loadingProgress.value = 0
    uni.showToast({ title: '加载失败', icon: 'none' })
  } finally {
    isPreloading.value = false
  }
}

/** WebView 消息回调 */
const onWebViewMessage = (e: any) => {
  // WebView 加载完成
  if (e.detail?.data?.loaded) {
    loadingProgress.value = 100
  }
}

/** 刷新大屏 */
const handleRefresh = () => {
  // 清除缓存
  removeCache(`screen_url_${screenId.value}`)
  loadError.value = false
  loadingProgress.value = 0
  screenUrl.value = ''
  loadScreen()
}

/** 全屏显示 */
const handleFullscreen = () => {
  isFullscreen.value = true
  // 隐藏导航栏（如果可能）
  // uni.setNavigationBarColor({ frontColor: '#000000', backgroundColor: '#000000' })
}

/** 退出全屏 */
const exitFullscreen = () => {
  isFullscreen.value = false
  // 恢复导航栏
  // uni.setNavigationBarColor({ frontColor: '#000000', backgroundColor: '#F8FAFC' })
}

/** 选择缩放比例 */
const selectScale = (scale: number) => {
  currentScale.value = scale
  showScalePicker.value = false
}

/** 监听 screenId 变化 */
watch(screenId, (newId) => {
  if (newId) {
    preloadScreen(newId)
  }
})

onMounted(() => {
  // 从路由参数获取大屏ID
  const pages = getCurrentPages()
  const currentPage = pages[pages.length - 1] as any
  screenId.value = currentPage.options?.id || ''

  if (!screenId.value) {
    uni.showToast({ title: '缺少大屏ID参数', icon: 'none' })
  }

  // 模拟加载进度
  setTimeout(() => {
    if (loadingProgress.value > 0 && loadingProgress.value < 100) {
      loadingProgress.value = 100
    }
  }, 3000)
})
</script>

<style lang="scss" scoped>
.screen-view-page {
  min-height: 100vh;
  background: #000;
  display: flex;
  flex-direction: column;
}

.webview-container {
  flex: 1;
  position: relative;
  overflow: hidden;

  &.fullscreen {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    z-index: 999;
  }
}

.loading-progress {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 60rpx;
  background: rgba(0, 0, 0, 0.8);
  display: flex;
  align-items: center;
  padding: 0 20rpx;
  z-index: 10;

  .progress-bar {
    height: 20rpx;
    background: $u-primary;
    border-radius: 10rpx;
    transition: width 0.3s;
  }

  .progress-text {
    font-size: 24rpx;
    color: #fff;
    margin-left: 20rpx;
  }
}

.preload-state {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  flex-direction: column;
  align-items: center;

  .preload-icon {
    font-size: 60rpx;
    animation: spin 1s linear infinite;
  }

  .preload-text {
    font-size: 28rpx;
    color: rgba(255, 255, 255, 0.7);
    margin-top: 20rpx;
  }
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.error-state {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  flex-direction: column;
  align-items: center;

  .error-icon {
    font-size: 60rpx;
    font-weight: bold;
    color: #ff4d4f;
  }

  .error-text {
    font-size: 28rpx;
    color: #fff;
    margin-top: 20rpx;
  }

  .retry-btn {
    margin-top: 30rpx;
    padding: 16rpx 40rpx;
    background: $u-primary;
    border-radius: $u-radius;

    text {
      font-size: 26rpx;
      color: #fff;
    }
  }
}

.action-bar {
  display: flex;
  justify-content: space-around;
  padding: 20rpx;
  background: rgba(0, 0, 0, 0.9);

  .action-btn {
    display: flex;
    flex-direction: column;
    align-items: center;
    gap: 8rpx;

    .btn-icon {
      font-size: 32rpx;
    }

    .btn-text {
      font-size: 24rpx;
      color: #fff;
    }
  }
}

.fullscreen-exit {
  position: fixed;
  top: 20rpx;
  right: 20rpx;
  width: 80rpx;
  height: 80rpx;
  background: rgba(255, 255, 255, 0.3);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;

  .exit-icon {
    font-size: 40rpx;
    color: #fff;
  }
}

.bottom-tip {
  text-align: center;
  padding: 16rpx 30rpx;
  background: rgba(0, 0, 0, 0.9);

  text {
    font-size: 24rpx;
    color: rgba(255, 255, 255, 0.7);
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
}
</style>