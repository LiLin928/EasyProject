<!-- 文件: PCWeb/src/views/desktop/index.vue -->

<template>
  <div class="desktop-container">
    <!-- 欢迎卡片 -->
    <el-card shadow="never" class="welcome-card">
      <div class="welcome-content">
        <el-avatar :size="64" :src="userAvatar" class="avatar">
          <el-icon :size="32"><User /></el-icon>
        </el-avatar>
        <div class="welcome-info">
          <h2 class="greeting">{{ greeting }}</h2>
          <p class="user-name">{{ userInfo?.nickname || userInfo?.username || '用户' }}</p>
          <p class="welcome-text">欢迎使用 EasyProject 管理系统</p>
        </div>
        <div class="welcome-actions">
          <el-button type="primary" @click="handleOpenLayoutSetting">
            <el-icon><Setting /></el-icon>
            布局设置
          </el-button>
          <el-button @click="handleRefresh">
            <el-icon><Refresh /></el-icon>
            刷新
          </el-button>
        </div>
      </div>
    </el-card>

    <!-- 组件网格区域 -->
    <div v-if="enabledWidgets.length > 0" class="widgets-grid" v-loading="loading">
      <div
        v-for="widget in enabledWidgets"
        :key="widget.id"
        class="widget-item"
        :style="{ width: `${(widget.width / 12) * 100}%` }"
      >
        <WidgetContainer :widget="widget" />
      </div>
    </div>

    <!-- 空状态 -->
    <el-empty
      v-else-if="!loading"
      description="暂无桌面组件，请配置布局"
      :image-size="120"
    >
      <el-button type="primary" @click="handleOpenLayoutSetting">
        配置布局
      </el-button>
    </el-empty>

    <!-- 布局设置弹窗 -->
    <LayoutSettingDialog
      v-model="layoutDialogVisible"
      @success="handleLayoutSuccess"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { User, Setting, Refresh } from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'
import { useDesktopStore } from '@/stores/desktopStore'
import WidgetContainer from './components/WidgetContainer.vue'
import LayoutSettingDialog from './components/LayoutSettingDialog.vue'

// 用户状态
const userStore = useUserStore()
const userInfo = computed(() => userStore.userInfo)

// 桌面状态
const desktopStore = useDesktopStore()
const loading = computed(() => desktopStore.loading)
const enabledWidgets = computed(() => desktopStore.enabledWidgets)

// 布局设置弹窗
const layoutDialogVisible = ref(false)

// 用户头像
const userAvatar = computed(() => {
  return userInfo.value?.avatar || ''
})

// 根据时间生成问候语
const greeting = computed(() => {
  const hour = new Date().getHours()
  if (hour < 6) return '凌晨好'
  if (hour < 9) return '早上好'
  if (hour < 12) return '上午好'
  if (hour < 14) return '中午好'
  if (hour < 17) return '下午好'
  if (hour < 19) return '傍晚好'
  if (hour < 22) return '晚上好'
  return '夜深了'
})

// 打开布局设置
function handleOpenLayoutSetting() {
  layoutDialogVisible.value = true
}

// 刷新桌面
async function handleRefresh() {
  await desktopStore.fetchDesktop()
}

// 布局设置成功回调
function handleLayoutSuccess() {
  // 刷新桌面配置
  desktopStore.fetchDesktop()
}

// 生命周期
onMounted(() => {
  desktopStore.fetchDesktop()
})

onUnmounted(() => {
  // 停止自动刷新
  desktopStore.stopAutoRefresh()
})
</script>

<style scoped lang="scss">
.desktop-container {
  padding: 20px;
  background-color: #f5f7fa;
  min-height: calc(100vh - 60px);

  .welcome-card {
    margin-bottom: 20px;

    .welcome-content {
      display: flex;
      align-items: center;
      gap: 20px;

      .avatar {
        flex-shrink: 0;
        background-color: #409eff;
      }

      .welcome-info {
        flex: 1;

        .greeting {
          margin: 0;
          font-size: 20px;
          font-weight: 600;
          color: #303133;
        }

        .user-name {
          margin: 8px 0;
          font-size: 14px;
          color: #606266;
        }

        .welcome-text {
          margin: 0;
          font-size: 14px;
          color: #909399;
        }
      }

      .welcome-actions {
        display: flex;
        gap: 12px;

        .el-button {
          .el-icon {
            margin-right: 4px;
          }
        }
      }
    }
  }

  .widgets-grid {
    display: flex;
    flex-wrap: wrap;
    gap: 20px;

    .widget-item {
      flex-shrink: 0;
      min-height: 200px;
    }
  }
}
</style>