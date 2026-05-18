<!-- pages/message/list/index.vue -->
<template>
  <view class="message-page">
    <!-- 空状态 -->
    <view class="empty-state">
      <text class="empty-icon">📭</text>
      <text class="empty-text">暂无消息</text>
      <text class="empty-desc">消息功能正在开发中，敬请期待</text>
    </view>
  </view>
</template>

<script setup lang="ts">
// 消息通知页面占位
// 后续对接消息 API
</script>

<style lang="scss" scoped>
.message-page {
  padding: 20rpx;
  background: $u-bg-color;
  min-height: 100vh;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 100rpx 40rpx;

  .empty-icon {
    font-size: 80rpx;
    margin-bottom: 20rpx;
  }

  .empty-text {
    font-size: 32rpx;
    font-weight: 600;
    color: $u-main-color;
    margin-bottom: 12rpx;
  }

  .empty-desc {
    font-size: 26rpx;
    color: $u-content-color;
  }
}
</style>