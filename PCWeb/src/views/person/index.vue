<template>
  <div class="person-container">
    <el-card shadow="never" class="person-card">
      <template #header>
        <div class="card-header">
          <span>个人信息</span>
          <el-button type="primary" @click="handleEdit">
            <el-icon><Edit /></el-icon>
            编辑资料
          </el-button>
        </div>
      </template>

      <div class="person-content">
        <!-- 用户头像区域 -->
        <div class="avatar-section">
          <el-avatar :size="100" :src="userAvatar" class="user-avatar">
            <el-icon :size="50"><User /></el-icon>
          </el-avatar>
          <div class="user-basic">
            <h2 class="user-name">{{ userInfo?.nickname || userInfo?.username || '用户' }}</h2>
            <div class="user-roles">
              <el-tag
                v-for="role in userInfo?.roles"
                :key="role"
                type="primary"
                effect="plain"
                class="role-tag"
              >
                {{ role }}
              </el-tag>
            </div>
          </div>
        </div>

        <el-divider />

        <!-- 用户详细信息 -->
        <el-descriptions :column="2" border class="user-descriptions">
          <el-descriptions-item label="用户名">
            {{ userInfo?.username || '-' }}
          </el-descriptions-item>
          <el-descriptions-item label="昵称">
            {{ userInfo?.nickname || '-' }}
          </el-descriptions-item>
          <el-descriptions-item label="邮箱">
            {{ userInfo?.email || '-' }}
          </el-descriptions-item>
          <el-descriptions-item label="手机号">
            {{ userInfo?.phone || '-' }}
          </el-descriptions-item>
          <el-descriptions-item label="角色" :span="2">
            <el-tag
              v-for="role in userInfo?.roles"
              :key="role"
              type="primary"
              effect="plain"
              class="role-tag"
            >
              {{ role }}
            </el-tag>
            <span v-if="!userInfo?.roles?.length">-</span>
          </el-descriptions-item>
          <el-descriptions-item label="权限" :span="2">
            <el-tag
              v-for="permission in displayPermissions"
              :key="permission"
              type="success"
              effect="plain"
              class="permission-tag"
            >
              {{ permission }}
            </el-tag>
            <span v-if="!userInfo?.permissions?.length">-</span>
            <el-button
              v-if="userInfo?.permissions?.length > 5"
              link
              type="primary"
              @click="showAllPermissions = !showAllPermissions"
            >
              {{ showAllPermissions ? '收起' : `展开全部 (${userInfo?.permissions?.length})` }}
            </el-button>
          </el-descriptions-item>
        </el-descriptions>
      </div>
    </el-card>

    <!-- 安全设置卡片 -->
    <el-card shadow="never" class="security-card">
      <template #header>
        <span>安全设置</span>
      </template>

      <div class="security-content">
        <div class="security-item">
          <div class="security-left">
            <el-icon class="security-icon"><Lock /></el-icon>
            <div class="security-info">
              <div class="security-title">修改密码</div>
              <div class="security-desc">定期更换密码可以保护账号安全</div>
            </div>
          </div>
          <el-button type="primary" link @click="handleModifyPassword">修改</el-button>
        </div>

        <el-divider />

        <div class="security-item">
          <div class="security-left">
            <el-icon class="security-icon"><Picture /></el-icon>
            <div class="security-info">
              <div class="security-title">更换头像</div>
              <div class="security-desc">设置个性化头像展示</div>
            </div>
          </div>
          <el-button type="primary" link @click="handleChangeAvatar">更换</el-button>
        </div>

        <el-divider />

        <div class="security-item">
          <div class="security-left">
            <el-icon class="security-icon"><Message /></el-icon>
            <div class="security-info">
              <div class="security-title">邮箱绑定</div>
              <div class="security-desc">
                {{ userInfo?.email ? `已绑定：${userInfo.email}` : '未绑定邮箱' }}
              </div>
            </div>
          </div>
          <el-button type="primary" link @click="handleBindEmail">
            {{ userInfo?.email ? '更换' : '绑定' }}
          </el-button>
        </div>

        <el-divider />

        <div class="security-item">
          <div class="security-left">
            <el-icon class="security-icon"><Phone /></el-icon>
            <div class="security-info">
              <div class="security-title">手机绑定</div>
              <div class="security-desc">
                {{ userInfo?.phone ? `已绑定：${formatPhone(userInfo.phone)}` : '未绑定手机' }}
              </div>
            </div>
          </div>
          <el-button type="primary" link @click="handleBindPhone">
            {{ userInfo?.phone ? '更换' : '绑定' }}
          </el-button>
        </div>
      </div>
    </el-card>

    <!-- 弹窗组件 -->
    <EditProfileDialog v-model="dialogs.editProfile" @success="refreshUserInfo" />
    <ChangePasswordDialog v-model="dialogs.changePassword" />
    <ChangeAvatarDialog v-model="dialogs.changeAvatar" @success="refreshUserInfo" />
    <BindEmailDialog v-model="dialogs.bindEmail" @success="refreshUserInfo" />
    <BindPhoneDialog v-model="dialogs.bindPhone" @success="refreshUserInfo" />
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed } from 'vue'
import { Edit, User, Lock, Message, Phone, Picture } from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'
import EditProfileDialog from './components/EditProfileDialog.vue'
import ChangePasswordDialog from './components/ChangePasswordDialog.vue'
import ChangeAvatarDialog from './components/ChangeAvatarDialog.vue'
import BindEmailDialog from './components/BindEmailDialog.vue'
import BindPhoneDialog from './components/BindPhoneDialog.vue'

const userStore = useUserStore()
const userInfo = computed(() => userStore.userInfo)

// 是否展开全部权限
const showAllPermissions = ref(false)

// 弹窗状态管理
const dialogs = reactive({
  editProfile: false,
  changePassword: false,
  changeAvatar: false,
  bindEmail: false,
  bindPhone: false,
})

// 用户头像
const userAvatar = computed(() => {
  return userInfo.value?.avatar || ''
})

// 显示的权限列表（最多显示5个）
const displayPermissions = computed(() => {
  const permissions = userInfo.value?.permissions || []
  if (showAllPermissions.value || permissions.length <= 5) {
    return permissions
  }
  return permissions.slice(0, 5)
})

// 格式化手机号（隐藏中间4位）
function formatPhone(phone: string) {
  if (!phone || phone.length !== 11) return phone
  return phone.replace(/(\d{3})\d{4}(\d{4})/, '$1****$2')
}

// 打开弹窗
function openDialog(dialog: keyof typeof dialogs) {
  dialogs[dialog] = true
}

// 刷新用户信息
function refreshUserInfo() {
  // 弹窗组件会直接更新 store，这里可以留空或添加额外逻辑
}

// 编辑资料
function handleEdit() {
  openDialog('editProfile')
}

// 修改密码
function handleModifyPassword() {
  openDialog('changePassword')
}

// 更换头像
function handleChangeAvatar() {
  openDialog('changeAvatar')
}

// 绑定邮箱
function handleBindEmail() {
  openDialog('bindEmail')
}

// 绑定手机
function handleBindPhone() {
  openDialog('bindPhone')
}
</script>

<style scoped lang="scss">
.person-container {
  padding: 20px;
  background-color: #f5f7fa;
  min-height: calc(100vh - 60px);
}

.person-card {
  margin-bottom: 20px;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  .person-content {
    .avatar-section {
      display: flex;
      align-items: center;
      gap: 24px;
      padding: 20px 0;

      .user-avatar {
        background-color: #409eff;
        flex-shrink: 0;
      }

      .user-basic {
        .user-name {
          margin: 0 0 12px;
          font-size: 24px;
          font-weight: 600;
          color: #303133;
        }

        .user-roles {
          display: flex;
          gap: 8px;
          flex-wrap: wrap;

          .role-tag {
            margin: 0;
          }
        }
      }
    }

    .user-descriptions {
      .role-tag,
      .permission-tag {
        margin-right: 8px;
        margin-bottom: 4px;
      }
    }
  }
}

.security-card {
  .security-content {
    .security-item {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px 0;

      .security-left {
        display: flex;
        align-items: center;
        gap: 16px;

        .security-icon {
          font-size: 32px;
          color: #409eff;
        }

        .security-info {
          .security-title {
            font-size: 16px;
            font-weight: 500;
            color: #303133;
            margin-bottom: 4px;
          }

          .security-desc {
            font-size: 14px;
            color: #909399;
          }
        }
      }
    }
  }
}

@media (max-width: 768px) {
  .person-card {
    .person-content {
      .avatar-section {
        flex-direction: column;
        text-align: center;

        .user-basic {
          .user-name {
            font-size: 20px;
          }

          .user-roles {
            justify-content: center;
          }
        }
      }

      .user-descriptions {
        :deep(.el-descriptions) {
          .el-descriptions__label {
            width: 80px !important;
          }
        }
      }
    }
  }

  .security-card {
    .security-content {
      .security-item {
        flex-direction: column;
        align-items: flex-start;
        gap: 12px;

        .security-left {
          width: 100%;
        }
      }
    }
  }
}
</style>