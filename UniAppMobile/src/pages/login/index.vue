<template>
  <view class="login-container">
    <view class="login-header">
      <text class="title">EasyProject</text>
      <text class="subtitle">移动端管理系统</text>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// Placeholder for login page
</script>

<style scoped lang="scss">
.login-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.login-header {
  text-align: center;
  margin-bottom: 60rpx;
}

.title {
  display: block;
  font-size: 56rpx;
  font-weight: bold;
  color: #ffffff;
  margin-bottom: 16rpx;
}

.subtitle {
  display: block;
  font-size: 28rpx;
  color: rgba(255, 255, 255, 0.8);
}
</style>