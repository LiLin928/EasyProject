<!-- components/Loading/index.vue -->
<template>
  <view v-if="show" class="loading-container">
    <view class="loading-spinner">
      <view class="spinner-item" />
      <view class="spinner-item" />
      <view class="spinner-item" />
    </view>
    <text class="loading-text">{{ text }}</text>
  </view>
</template>

<script setup lang="ts">
defineProps({
  show: {
    type: Boolean,
    default: false,
  },
  text: {
    type: String,
    default: '加载中...',
  },
})
</script>

<style lang="scss" scoped>
.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 40rpx;

  .loading-spinner {
    display: flex;
    gap: 8rpx;

    .spinner-item {
      width: 16rpx;
      height: 16rpx;
      background: $u-primary;
      border-radius: 50%;
      animation: bounce 0.6s infinite ease-in-out both;

      &:nth-child(1) { animation-delay: -0.32s; }
      &:nth-child(2) { animation-delay: -0.16s; }
      &:nth-child(3) { animation-delay: 0s; }
    }
  }

  .loading-text {
    font-size: 24rpx;
    color: $u-content-color;
    margin-top: 16rpx;
  }
}

@keyframes bounce {
  0%, 80%, 100% {
    transform: scale(0);
  }
  40% {
    transform: scale(1);
  }
}
</style>