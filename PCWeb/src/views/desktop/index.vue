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
      </div>
    </el-card>

    <!-- 快捷入口 -->
    <div class="section-title">快捷入口</div>
    <el-row :gutter="20" class="quick-entry">
      <el-col :xs="24" :sm="12" :md="8" :lg="6">
        <el-card shadow="hover" class="entry-card" @click="handleQuickEntry('/system/user')">
          <div class="entry-content">
            <el-icon class="entry-icon" :size="40"><User /></el-icon>
            <span class="entry-title">用户管理</span>
          </div>
        </el-card>
      </el-col>
      <el-col :xs="24" :sm="12" :md="8" :lg="6">
        <el-card shadow="hover" class="entry-card" @click="handleQuickEntry('/system/role')">
          <div class="entry-content">
            <el-icon class="entry-icon" :size="40"><Key /></el-icon>
            <span class="entry-title">角色管理</span>
          </div>
        </el-card>
      </el-col>
      <el-col :xs="24" :sm="12" :md="8" :lg="6">
        <el-card shadow="hover" class="entry-card" @click="handleQuickEntry('/system/menu')">
          <div class="entry-content">
            <el-icon class="entry-icon" :size="40"><Menu /></el-icon>
            <span class="entry-title">菜单管理</span>
          </div>
        </el-card>
      </el-col>
      <el-col :xs="24" :sm="12" :md="8" :lg="6">
        <el-card shadow="hover" class="entry-card" @click="handleQuickEntry('/system/config')">
          <div class="entry-content">
            <el-icon class="entry-icon" :size="40"><Setting /></el-icon>
            <span class="entry-title">系统设置</span>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 待办事项 -->
    <div class="section-title">待办事项</div>
    <el-card shadow="never" class="pending-card">
      <div v-if="pendingItems.length > 0" class="pending-list">
        <div v-for="item in pendingItems" :key="item.id" class="pending-item">
          <div class="pending-left">
            <el-tag :type="getPendingTagType(item.type)" size="small">{{ item.typeName }}</el-tag>
            <span class="pending-title">{{ item.title }}</span>
          </div>
          <div class="pending-right">
            <span class="pending-time">{{ item.time }}</span>
            <el-button link type="primary" @click="handlePending(item)">处理</el-button>
          </div>
        </div>
      </div>
      <el-empty v-else description="暂无待办事项" :image-size="80" />
    </el-card>

    <!-- 公告通知 -->
    <div class="section-title">公告通知</div>
    <el-card shadow="never" class="announcement-card" v-loading="loading">
      <div v-if="announcements.length > 0" class="announcement-list">
        <div
          v-for="item in announcements"
          :key="item.id"
          class="announcement-item"
          @click="handleAnnouncementClick(item)"
        >
          <div class="announcement-header">
            <el-tag type="danger" v-if="item.isTop" size="small">置顶</el-tag>
            <el-tag
              :type="item.level === 3 ? 'danger' : item.level === 2 ? 'warning' : 'info'"
              size="small"
            >
              {{ item.level === 3 ? '紧急' : item.level === 2 ? '重要' : '普通' }}
            </el-tag>
            <span class="announcement-title">{{ item.title }}</span>
          </div>
          <div class="announcement-content">{{ item.content }}</div>
          <div class="announcement-footer">
            <span class="announcement-author">{{ item.creatorName || '管理员' }}</span>
            <span class="announcement-time">{{ item.publishTime || item.createTime }}</span>
          </div>
        </div>
      </div>
      <el-empty v-else description="暂无公告通知" :image-size="80" />
    </el-card>

    <!-- 公告详情弹窗 -->
    <AnnouncementDetailDialog
      v-model="detailDialogVisible"
      :id="currentAnnouncement?.id || ''"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { User, Key, Menu, Setting } from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'
import { getUnreadAnnouncementList } from '@/api/announcement/announcementApi'
import { markReadAnnouncement } from '@/api/announcement/announcementApi'
import AnnouncementDetailDialog from '@/views/basic/announcement/components/AnnouncementDetailDialog.vue'
import type { Announcement } from '@/types'

const router = useRouter()
const userStore = useUserStore()
const userInfo = computed(() => userStore.userInfo)

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

// 待办事项（模拟数据）
const pendingItems = ref([
  { id: 1, title: '张三申请加入项目组', type: 'approval', typeName: '审批', time: '10分钟前' },
  { id: 2, title: '李四提交了日报待审核', type: 'review', typeName: '审核', time: '30分钟前' },
  { id: 3, title: '系统升级通知需确认', type: 'notice', typeName: '通知', time: '1小时前' },
])

// 公告通知（从API获取）
const announcements = ref<Announcement[]>([])
const loading = ref(false)

// 公告详情弹窗
const detailDialogVisible = ref(false)
const currentAnnouncement = ref<Announcement | null>(null)

// 获取待办标签类型
function getPendingTagType(type: string) {
  const typeMap: Record<string, string> = {
    approval: 'warning',
    review: 'success',
    notice: 'info',
  }
  return typeMap[type] || 'info'
}

// 快捷入口点击
function handleQuickEntry(path: string) {
  router.push(path)
}

// 处理待办事项
function handlePending(item: any) {
  console.log('处理待办:', item)
}

// 加载未读公告
async function loadUnreadAnnouncements() {
  loading.value = true
  try {
    const data = await getUnreadAnnouncementList()
    announcements.value = data
  } catch (error) {
    console.error('加载未读公告失败:', error)
  } finally {
    loading.value = false
  }
}

// 点击公告查看详情
async function handleAnnouncementClick(item: Announcement) {
  currentAnnouncement.value = item
  detailDialogVisible.value = true

  // 标记已读
  if (!item.isRead) {
    try {
      await markReadAnnouncement(item.id)
      item.isRead = true
    } catch (error) {
      console.error('标记已读失败:', error)
    }
  }
}

// 生命周期
onMounted(() => {
  loadUnreadAnnouncements()
})
</script>

<style scoped lang="scss">
.desktop-container {
  padding: 20px;
  background-color: #f5f7fa;
  min-height: calc(100vh - 60px);
}

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
  }
}

.section-title {
  font-size: 16px;
  font-weight: 600;
  color: #303133;
  margin-bottom: 12px;
  padding-left: 10px;
  border-left: 3px solid #409eff;
}

.quick-entry {
  margin-bottom: 20px;

  .entry-card {
    cursor: pointer;
    margin-bottom: 12px;
    transition: all 0.3s;

    &:hover {
      transform: translateY(-2px);
    }

    .entry-content {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 20px 0;

      .entry-icon {
        color: #409eff;
        margin-bottom: 12px;
      }

      .entry-title {
        font-size: 14px;
        color: #606266;
      }
    }
  }
}

.pending-card {
  margin-bottom: 20px;

  .pending-list {
    .pending-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 12px 0;
      border-bottom: 1px solid #ebeef5;

      &:last-child {
        border-bottom: none;
      }

      .pending-left {
        display: flex;
        align-items: center;
        gap: 12px;

        .pending-title {
          font-size: 14px;
          color: #303133;
        }
      }

      .pending-right {
        display: flex;
        align-items: center;
        gap: 12px;

        .pending-time {
          font-size: 12px;
          color: #909399;
        }
      }
    }
  }
}

.announcement-card {
  .announcement-list {
    .announcement-item {
      padding: 16px 0;
      border-bottom: 1px solid #ebeef5;
      cursor: pointer;
      transition: background-color 0.2s;

      &:hover {
        background-color: #f5f7fa;
      }

      &:last-child {
        border-bottom: none;
      }

      .announcement-header {
        display: flex;
        align-items: center;
        gap: 8px;
        margin-bottom: 8px;

        .announcement-title {
          font-size: 16px;
          font-weight: 500;
          color: #303133;
        }
      }

      .announcement-content {
        font-size: 14px;
        color: #606266;
        line-height: 1.6;
        margin-bottom: 8px;
        overflow: hidden;
        text-overflow: ellipsis;
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
      }

      .announcement-footer {
        display: flex;
        gap: 16px;
        font-size: 12px;
        color: #909399;
      }
    }
  }
}
</style>