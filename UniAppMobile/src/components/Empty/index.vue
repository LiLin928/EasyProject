<!-- components/Empty/index.vue -->
<template>
  <view class="empty-container">
    <text class="empty-icon">{{ icon }}</text>
    <text class="empty-text">{{ text }}</text>
    <button v-if="showButton" class="empty-btn" @click="$emit('action')">
      {{ buttonText }}
    </button>
  </view>
</template>

<script setup lang="ts">
defineProps({
  icon: {
    type: String,
    default: '📭',
  },
  text: {
    type: String,
    default: '暂无数据',
  },
  showButton: {
    type: Boolean,
    default: false,
  },
  buttonText: {
    type: String,
    default: '重新加载',
  },
})

defineEmits(['action'])
</script>

<style lang="scss" scoped>
.empty-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 80rpx 40rpx;

  .empty-icon {
    font-size: 80rpx;
  }

  .empty-text {
    font-size: 28rpx;
    color: $u-content-color;
    margin-top: 24rpx;
  }

  .empty-btn {
    margin-top: 32rpx;
    padding: 16rpx 32rpx;
    background: $u-primary;
    color: #fff;
    font-size: 26rpx;
    border-radius: $u-radius;
  }
}
</style>