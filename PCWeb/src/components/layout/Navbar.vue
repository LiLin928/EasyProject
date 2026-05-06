<!-- 顶部导航栏 -->
<template>
  <div class="navbar">
    <!-- 左侧 Logo -->
    <div class="navbar-left">
      <el-icon class="logo-icon"><Monitor /></el-icon>
      <span class="logo-text">EasyProject</span>
    </div>

    <!-- 右侧功能区 -->
    <div class="navbar-right">
      <!-- 消息通知 -->
      <el-badge :value="3" :max="99" class="nav-item">
        <el-icon class="nav-icon"><Bell /></el-icon>
      </el-badge>

      <!-- 语言切换 -->
      <el-dropdown trigger="click" @command="handleLocale">
        <div class="nav-item">
          <el-icon class="nav-icon"><Connection /></el-icon>
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="zh-CN">中文</el-dropdown-item>
            <el-dropdown-item command="en-US">English</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>

      <!-- 全屏 -->
      <div class="nav-item" @click="toggleFullscreen">
        <el-icon class="nav-icon"><FullScreen /></el-icon>
      </div>

      <!-- 用户头像 -->
      <el-dropdown trigger="click" @command="handleCommand">
        <div class="user-info">
          <el-avatar :size="32" :src="userAvatar">
            <el-icon><UserFilled /></el-icon>
          </el-avatar>
          <span class="username">{{ username }}</span>
          <el-icon class="dropdown-icon"><ArrowDown /></el-icon>
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="profile">
              <el-icon><User /></el-icon>
              <span>{{ $t('user.navbar.profile') }}</span>
            </el-dropdown-item>
            <el-dropdown-item divided command="logout">
              <el-icon><SwitchButton /></el-icon>
              <span>{{ $t('user.navbar.logout') }}</span>
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessageBox, ElMessage } from 'element-plus'
import {
  Monitor,
  UserFilled,
  ArrowDown,
  User,
  SwitchButton,
  Bell,
  Connection,
  FullScreen,
} from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'
import { useLocale } from '@/composables/useLocale'

const router = useRouter()
const userStore = useUserStore()
const { t, changeLocale } = useLocale()

const collapsed = defineModel<boolean>('collapsed', { default: false })

const username = computed(() => userStore.userInfo?.nickname || userStore.userInfo?.username || '用户')
const userAvatar = computed(() => userStore.userInfo?.avatar || '')

// 切换全屏 - 添加错误处理
const toggleFullscreen = async () => {
  try {
    if (!document.fullscreenElement) {
      await document.documentElement.requestFullscreen()
    } else {
      await document.exitFullscreen()
    }
  } catch (error) {
    console.warn('Fullscreen not supported or denied:', error)
  }
}

// 语言切换 - 使用 i18n 提示
const handleLocale = (locale: string) => {
  changeLocale(locale as 'zh-CN' | 'en-US')
  ElMessage.success(t('common.localeChanged'))
}

// 处理下拉菜单命令
const handleCommand = (command: string) => {
  switch (command) {
    case 'profile':
      router.push('/person')
      break
    case 'logout':
      handleLogout()
      break
  }
}

// 退出登录
const handleLogout = async () => {
  try {
    await ElMessageBox.confirm(
      t('user.navbar.logoutConfirm'),
      t('user.navbar.logoutTitle'),
      {
        confirmButtonText: t('common.button.confirm'),
        cancelButtonText: t('common.button.cancel'),
        type: 'warning',
      }
    )
    await userStore.logoutAction()
  } catch {
    // 用户取消
  }
}
</script>

<style scoped lang="scss">
.navbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  height: 60px;
  padding: 0 20px;
  background: linear-gradient(135deg, #409eff 0%, #66b1ff 100%);
  box-sizing: border-box;
}

.navbar-left {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-shrink: 0;

  .logo-icon {
    font-size: 28px;
    color: #fff;
  }

  .logo-text {
    font-size: 18px;
    font-weight: bold;
    color: #fff;
  }
}

.navbar-right {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  gap: 8px;
  flex: 1;
}

.nav-item {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s;

  &:hover {
    background-color: rgba(255, 255, 255, 0.2);
  }
}

.nav-icon {
  font-size: 20px;
  color: #fff;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 5px 12px;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s;

  &:hover {
    background-color: rgba(255, 255, 255, 0.2);
  }

  .username {
    font-size: 14px;
    color: #fff;
  }

  .dropdown-icon {
    font-size: 12px;
    color: rgba(255, 255, 255, 0.8);
  }
}

:deep(.el-badge__content) {
  background-color: #f56c6c;
}

:deep(.el-dropdown-menu__item) {
  display: flex;
  align-items: center;
  gap: 8px;
}
</style>