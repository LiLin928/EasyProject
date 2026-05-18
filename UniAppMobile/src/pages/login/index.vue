<template>
  <view class="login-page">
    <view class="login-header">
      <image class="logo" src="/static/logo.png" mode="aspectFit" />
      <text class="title">EasyProject</text>
      <text class="subtitle">移动端管理平台</text>
    </view>

    <view class="login-form">
      <view class="form-item">
        <view class="input-wrapper">
          <text class="input-icon">👤</text>
          <input
            class="input"
            v-model="formData.username"
            placeholder="请输入用户名"
            type="text"
            placeholder-class="input-placeholder"
          />
        </view>
      </view>

      <view class="form-item">
        <view class="input-wrapper">
          <text class="input-icon">🔒</text>
          <input
            class="input"
            v-model="formData.password"
            placeholder="请输入密码"
            :password="!showPassword"
            placeholder-class="input-placeholder"
          />
          <view class="password-toggle" @click="showPassword = !showPassword">
            <text class="toggle-icon">{{ showPassword ? '🙈' : '👁️' }}</text>
          </view>
        </view>
      </view>

      <button class="login-btn" :loading="loading" :disabled="loading" @click="handleLogin">
        登录
      </button>

      <view class="login-footer">
        <text class="footer-text">首次登录请联系管理员获取账号</text>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { useUserStore } from '@/stores/user'

const userStore = useUserStore()
const loading = ref(false)
const showPassword = ref(false)

const formData = reactive({
  username: '',
  password: '',
})

const handleLogin = async () => {
  // 表单验证
  if (!formData.username.trim()) {
    uni.showToast({ title: '请输入用户名', icon: 'none' })
    return
  }

  if (!formData.password.trim()) {
    uni.showToast({ title: '请输入密码', icon: 'none' })
    return
  }

  loading.value = true
  try {
    await userStore.loginAction({
      username: formData.username.trim(),
      password: formData.password,
    })
    uni.showToast({ title: '登录成功', icon: 'success' })
    // 登录成功后跳转到首页
    setTimeout(() => {
      uni.switchTab({ url: '/pages/index/index' })
    }, 500)
  } catch (error: any) {
    const message = error?.message || '登录失败，请重试'
    uni.showToast({ title: message, icon: 'none' })
  } finally {
    loading.value = false
  }
}
</script>

<style lang="scss" scoped>
.login-page {
  display: flex;
  flex-direction: column;
  align-items: center;
  min-height: 100vh;
  padding: 120rpx 40rpx 40rpx;
  background: linear-gradient(180deg, #2563EB 0%, #1E40AF 50%, #F8FAFC 100%);
}

.login-header {
  text-align: center;
  margin-bottom: 80rpx;

  .logo {
    width: 160rpx;
    height: 160rpx;
    margin-bottom: 24rpx;
  }

  .title {
    display: block;
    font-size: 48rpx;
    font-weight: 600;
    color: #fff;
    margin-bottom: 12rpx;
  }

  .subtitle {
    display: block;
    font-size: 28rpx;
    color: rgba(255, 255, 255, 0.8);
  }
}

.login-form {
  width: 100%;
  padding: 48rpx 32rpx;
  background: #fff;
  border-radius: 24rpx;
  box-shadow: 0 8rpx 32rpx rgba(0, 0, 0, 0.1);

  .form-item {
    margin-bottom: 32rpx;

    .input-wrapper {
      display: flex;
      align-items: center;
      height: 96rpx;
      padding: 0 24rpx;
      background: #F8FAFC;
      border: 2rpx solid #E2E8F0;
      border-radius: 12rpx;
      transition: border-color 0.3s;

      &:focus-within {
        border-color: #2563EB;
        background: #fff;
      }

      .input-icon {
        font-size: 36rpx;
        margin-right: 16rpx;
      }

      .input {
        flex: 1;
        height: 100%;
        font-size: 28rpx;
        color: #1E293B;
      }

      .input-placeholder {
        color: #94A3B8;
      }

      .password-toggle {
        padding: 16rpx;
        margin-right: -16rpx;

        .toggle-icon {
          font-size: 32rpx;
        }
      }
    }
  }

  .login-btn {
    width: 100%;
    height: 96rpx;
    margin-top: 48rpx;
    font-size: 32rpx;
    font-weight: 500;
    color: #fff;
    background: linear-gradient(135deg, #2563EB 0%, #1D4ED8 100%);
    border: none;
    border-radius: 12rpx;
    transition: opacity 0.3s;

    &:active {
      opacity: 0.9;
    }

    &[disabled] {
      opacity: 0.6;
    }
  }

  .login-footer {
    margin-top: 32rpx;
    text-align: center;

    .footer-text {
      font-size: 24rpx;
      color: #94A3B8;
    }
  }
}
</style>