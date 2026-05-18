<!-- pages/log/detail/index.vue -->
<template>
  <view class="log-detail-page">
    <!-- 加载状态 -->
    <view v-if="loading" class="loading-state">
      <text>加载中...</text>
    </view>

    <!-- 错误状态 -->
    <view v-if="error && !loading" class="error-state">
      <text class="error-icon">⚠</text>
      <text class="error-text">{{ error }}</text>
      <view class="retry-btn" @click="loadDetail">
        <text>重新加载</text>
      </view>
    </view>

    <!-- 详情内容 -->
    <view v-if="logDetail && !loading" class="detail-content">
      <!-- 基本信息 -->
      <view class="detail-card">
        <view class="card-header">
          <text class="card-title">基本信息</text>
        </view>
        <view class="card-body">
          <view class="info-row">
            <text class="info-label">操作模块</text>
            <text class="info-value">{{ logDetail.module }}</text>
          </view>
          <view class="info-row">
            <text class="info-label">操作类型</text>
            <text class="info-value">{{ logDetail.action }}</text>
          </view>
          <view class="info-row">
            <text class="info-label">日志类型</text>
            <view :class="['type-tag', getLogTypeClass(logDetail.logType)]">
              <text>{{ getLogTypeText(logDetail.logType) }}</text>
            </view>
          </view>
        </view>
      </view>

      <!-- 操作信息 -->
      <view class="detail-card">
        <view class="card-header">
          <text class="card-title">操作信息</text>
        </view>
        <view class="card-body">
          <view class="info-row">
            <text class="info-label">操作人</text>
            <text class="info-value">{{ logDetail.operator }}</text>
          </view>
          <view class="info-row">
            <text class="info-label">操作时间</text>
            <text class="info-value">{{ formatTime(logDetail.createTime) }}</text>
          </view>
          <view class="info-row">
            <text class="info-label">操作IP</text>
            <text class="info-value">{{ logDetail.ip || '-' }}</text>
          </view>
        </view>
      </view>

      <!-- 目标对象 -->
      <view v-if="logDetail.targetId || logDetail.targetName" class="detail-card">
        <view class="card-header">
          <text class="card-title">目标对象</text>
        </view>
        <view class="card-body">
          <view v-if="logDetail.targetId" class="info-row">
            <text class="info-label">对象ID</text>
            <text class="info-value id-value">{{ logDetail.targetId }}</text>
          </view>
          <view v-if="logDetail.targetName" class="info-row">
            <text class="info-label">对象名称</text>
            <text class="info-value">{{ logDetail.targetName }}</text>
          </view>
          <view v-if="logDetail.targetId" class="info-row link-row">
            <view class="link-btn" @click="handleViewTarget">
              <text>查看对象详情</text>
              <text class="link-arrow">→</text>
            </view>
          </view>
        </view>
      </view>

      <!-- 操作内容 -->
      <view v-if="logDetail.content" class="detail-card">
        <view class="card-header">
          <text class="card-title">操作内容</text>
        </view>
        <view class="card-body">
          <view class="content-box">
            <text class="content-text">{{ logDetail.content }}</text>
          </view>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getLogDetail } from '@/api/log/logApi'
import type { Log } from '@/types'
import { LogType } from '@/types'

// 日志ID
const logId = ref('')

// 日志详情
const logDetail = ref<Log | null>(null)

// 加载状态
const loading = ref(false)

// 错误信息
const error = ref('')

// 获取日志类型样式类
const getLogTypeClass = (type: LogType) => {
  switch (type) {
    case LogType.Login:
      return 'type-login'
    case LogType.Operation:
      return 'type-operation'
    case LogType.Exception:
      return 'type-exception'
    case LogType.System:
      return 'type-system'
    default:
      return 'type-default'
  }
}

// 获取日志类型文本
const getLogTypeText = (type: LogType) => {
  switch (type) {
    case LogType.Login:
      return '登录日志'
    case LogType.Operation:
      return '操作日志'
    case LogType.Exception:
      return '异常日志'
    case LogType.System:
      return '系统日志'
    default:
      return '未知'
  }
}

// 格式化时间
const formatTime = (time: string) => {
  if (!time) return '-'
  const date = new Date(time)
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hour = String(date.getHours()).padStart(2, '0')
  const minute = String(date.getMinutes()).padStart(2, '0')
  const second = String(date.getSeconds()).padStart(2, '0')
  return `${year}-${month}-${day} ${hour}:${minute}:${second}`
}

// 加载详情
const loadDetail = async () => {
  if (!logId.value) {
    error.value = '缺少日志ID参数'
    return
  }

  loading.value = true
  error.value = ''

  try {
    const detail = await getLogDetail(logId.value)
    logDetail.value = detail
  } catch (err: unknown) {
    console.error('加载日志详情失败:', err)
    const message = (err as { errMsg?: string; message?: string })?.errMsg
      || (err as { message?: string })?.message
      || '加载失败'
    error.value = message
  } finally {
    loading.value = false
  }
}

// 查看目标对象
const handleViewTarget = () => {
  if (!logDetail.value?.targetId) return

  // 根据模块跳转到不同的详情页面
  const module = logDetail.value.module.toLowerCase()
  const targetId = logDetail.value.targetId

  let url = ''

  // 根据模块判断跳转路径
  if (module.includes('商品') || module.includes('product')) {
    url = `/pages/product/detail/index?id=${targetId}`
  } else if (module.includes('用户') || module.includes('user')) {
    // TODO: 用户详情页面
    uni.showToast({ title: '用户详情页面待实现', icon: 'none' })
    return
  } else if (module.includes('订单') || module.includes('order')) {
    // TODO: 订单详情页面
    uni.showToast({ title: '订单详情页面待实现', icon: 'none' })
    return
  } else {
    uni.showToast({ title: '暂不支持查看此类型对象', icon: 'none' })
    return
  }

  if (url) {
    uni.navigateTo({ url })
  }
}

onMounted(() => {
  // 获取URL参数
  const pages = getCurrentPages()
  const currentPage = pages[pages.length - 1] as { options: { id?: string } }
  const options = currentPage.options || {}

  if (options.id) {
    logId.value = options.id
    loadDetail()
  } else {
    error.value = '缺少日志ID参数'
  }
})
</script>

<style lang="scss" scoped>
.log-detail-page {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background: $u-bg-color;
  padding: 20rpx;
}

.loading-state {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;

  text {
    font-size: 28rpx;
    color: $u-tips-color;
  }
}

.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  padding: 40rpx;

  .error-icon {
    font-size: 80rpx;
    color: $u-error;
  }

  .error-text {
    font-size: 28rpx;
    color: $u-content-color;
    margin-top: 20rpx;
  }

  .retry-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 200rpx;
    height: 72rpx;
    background: $u-primary;
    border-radius: $u-radius;
    margin-top: 30rpx;

    text {
      color: #fff;
      font-size: 28rpx;
    }
  }
}

.detail-content {
  display: flex;
  flex-direction: column;
  gap: 20rpx;
}

.detail-card {
  background: #fff;
  border-radius: $u-radius-lg;
  overflow: hidden;

  .card-header {
    padding: 24rpx;
    background: $u-light-color;
    border-bottom: 1rpx solid $u-border-color;

    .card-title {
      font-size: 30rpx;
      font-weight: 600;
      color: $u-main-color;
    }
  }

  .card-body {
    padding: 24rpx;
  }
}

.info-row {
  display: flex;
  align-items: flex-start;
  padding: 16rpx 0;

  &:not(:last-child) {
    border-bottom: 1rpx solid $u-border-color;
  }

  .info-label {
    font-size: 28rpx;
    color: $u-tips-color;
    width: 160rpx;
    flex-shrink: 0;
  }

  .info-value {
    font-size: 28rpx;
    color: $u-main-color;
    flex: 1;
    word-break: break-all;
  }

  .id-value {
    font-size: 24rpx;
    color: $u-content-color;
  }

  .type-tag {
    font-size: 24rpx;
    padding: 6rpx 16rpx;
    border-radius: 4rpx;

    text {
      color: #fff;
    }
  }

  .type-login {
    background: $u-primary;
  }

  .type-operation {
    background: $u-success;
  }

  .type-exception {
    background: $u-error;
  }

  .type-system {
    background: $u-content-color;
  }

  .type-default {
    background: $u-tips-color;
  }
}

.link-row {
  border-bottom: none;
  padding-top: 24rpx;

  .link-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 72rpx;
    background: rgba($u-primary, 0.1);
    border-radius: $u-radius;
    gap: 8rpx;

    text {
      font-size: 28rpx;
      color: $u-primary;
    }

    .link-arrow {
      font-size: 28rpx;
    }
  }
}

.content-box {
  background: $u-light-color;
  border-radius: $u-radius;
  padding: 24rpx;
  min-height: 120rpx;

  .content-text {
    font-size: 28rpx;
    color: $u-main-color;
    line-height: 1.6;
    word-break: break-all;
    white-space: pre-wrap;
  }
}
</style>