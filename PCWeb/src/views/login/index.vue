<!-- src/views/login/index.vue -->
<template>
  <div class="login-container">
    <div class="login-card">
      <div class="login-header">
        <h2 class="login-title">EasyProject</h2>
        <p class="login-subtitle">{{ $t('user.login.subtitle') }}</p>
      </div>

      <el-form
        ref="loginFormRef"
        :model="loginForm"
        :rules="loginRules"
        class="login-form"
        size="large"
      >
        <el-form-item prop="username">
          <el-input
            v-model="loginForm.username"
            :placeholder="$t('user.login.usernamePlaceholder')"
            prefix-icon="User"
          />
        </el-form-item>

        <el-form-item prop="password">
          <el-input
            v-model="loginForm.password"
            type="password"
            :placeholder="$t('user.login.passwordPlaceholder')"
            prefix-icon="Lock"
            show-password
          />
        </el-form-item>

        <el-form-item>
          <div class="login-options">
            <el-checkbox v-model="loginForm.rememberMe">{{ $t('user.login.rememberMe') }}</el-checkbox>
          </div>
        </el-form-item>

        <el-form-item>
          <el-button
            type="primary"
            class="login-button"
            :loading="loading"
            @click="handleLogin"
          >
            {{ $t('user.login.loginButton') }}
          </el-button>
        </el-form-item>
      </el-form>

      <div class="login-footer">
        <p>测试账号：admin / 123456</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { useUserStore } from '@/stores'
import type { LoginParams } from '@/types'
import { useLocale } from '@/composables/useLocale'

const router = useRouter()
const route = useRoute()
const userStore = useUserStore()
const { t } = useLocale()

// 表单引用
const loginFormRef = ref<FormInstance>()

// 加载状态
const loading = ref(false)

// 表单数据 - 仅开发环境填充默认值
const loginForm = reactive<LoginParams>({
  username: import.meta.env.DEV ? 'admin' : '',
  password: import.meta.env.DEV ? '123456' : '',
  rememberMe: false,
})

// 表单验证规则 - 使用 computed 确保响应式
const loginRules = computed<FormRules>(() => ({
  username: [
    { required: true, message: t('user.login.usernameRequired'), trigger: 'blur' },
    { min: 2, max: 20, message: t('common.validation.minLength', { min: 2 }), trigger: 'blur' },
  ],
  password: [
    { required: true, message: t('user.login.passwordRequired'), trigger: 'blur' },
    { min: 6, max: 20, message: t('common.validation.minLength', { min: 6 }), trigger: 'blur' },
  ],
}))

// 登录处理
async function handleLogin() {
  const valid = await loginFormRef.value?.validate().catch(() => false)
  if (!valid) return

  loading.value = true
  try {
    await userStore.loginAction(loginForm)
    ElMessage.success(t('user.login.loginSuccess'))

    // 获取重定向路径
    const redirect = (route.query.redirect as string) || '/'
    router.push(redirect)
  } catch {
    // Login failed - handled by store/global interceptor
  } finally {
    loading.value = false
  }
}
</script>

<style scoped lang="scss">
@use '@/assets/styles/variables.scss' as *;

.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: linear-gradient(135deg, #409eff 0%, #667eea 100%);
}

.login-card {
  width: 400px;
  padding: 40px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
}

.login-title {
  font-size: 28px;
  color: $primary-color;
  margin-bottom: 8px;
}

.login-subtitle {
  font-size: 14px;
  color: $text-secondary;
}

.login-form {
  .el-form-item {
    margin-bottom: 20px;
  }
}

.login-options {
  display: flex;
  justify-content: space-between;
  width: 100%;
}

.login-button {
  width: 100%;
}

.login-footer {
  margin-top: 20px;
  text-align: center;
  font-size: 12px;
  color: $text-secondary;
}
</style>