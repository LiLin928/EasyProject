<!-- 分享配置弹窗 -->
<template>
  <el-dialog
    :model-value="modelValue"
    width="500px"
    class="screen-dialog"
    @update:model-value="emit('update:modelValue', $event)"
    @open="handleOpen"
  >
    <template #header>
      <div class="dialog-header">
        <span class="dialog-title">{{ t('screen.screen.share.title') }}</span>
        <div class="dialog-actions">
          <el-button @click="emit('update:modelValue', false)">
            {{ t('common.button.cancel') }}
          </el-button>
          <el-button type="primary" :loading="saving" @click="handleSave">
            {{ t('common.button.confirm') }}
          </el-button>
        </div>
      </div>
    </template>
    <el-form label-width="100px">
      <!-- 访问权限 -->
      <el-form-item :label="t('screen.screen.share.accessMode')">
        <el-radio-group v-model="accessMode">
          <el-radio value="private">{{ t('screen.screen.share.private') }}</el-radio>
          <el-radio value="shared">{{ t('screen.screen.share.shared') }}</el-radio>
          <el-radio value="public">{{ t('screen.screen.share.public') }}</el-radio>
        </el-radio-group>
      </el-form-item>

      <!-- 分享给用户 -->
      <el-form-item v-if="accessMode === 'shared'" :label="t('screen.screen.share.sharedUsers')">
        <el-select
          v-model="selectedUsers"
          multiple
          filterable
          :placeholder="t('screen.screen.share.selectUser')"
          style="width: 100%"
        >
          <el-option
            v-for="user in userList"
            :key="user.id"
            :label="user.name"
            :value="user.id"
          />
        </el-select>
      </el-form-item>

      <!-- 已分享用户列表 -->
      <el-form-item v-if="accessMode === 'shared' && selectedUsers.length > 0" :label="' '">
        <div class="tag-list">
          <el-tag
            v-for="userId in selectedUsers"
            :key="userId"
            closable
            @close="removeUser(userId)"
          >
            {{ getUserName(userId) }}
          </el-tag>
        </div>
      </el-form-item>

      <!-- 分享给角色 -->
      <el-form-item v-if="accessMode === 'shared'" :label="t('screen.screen.share.sharedRoles')">
        <el-select
          v-model="selectedRoles"
          multiple
          filterable
          :placeholder="t('screen.screen.share.selectRole')"
          style="width: 100%"
        >
          <el-option
            v-for="role in roleList"
            :key="role.id"
            :label="role.name"
            :value="role.id"
          />
        </el-select>
      </el-form-item>

      <!-- 已分享角色列表 -->
      <el-form-item v-if="accessMode === 'shared' && selectedRoles.length > 0" :label="' '">
        <div class="tag-list">
          <el-tag
            v-for="roleId in selectedRoles"
            :key="roleId"
            type="success"
            closable
            @close="removeRole(roleId)"
          >
            {{ getRoleName(roleId) }}
          </el-tag>
        </div>
      </el-form-item>
    </el-form>

    </el-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { useLocale } from '@/composables/useLocale'
import { getShareableUsers, getShareableRoles, updateScreenShare } from '@/api/screen'
import type { ScreenConfig, ScreenPermissions } from '@/types'

const { t } = useLocale()

const props = defineProps<{
  modelValue: boolean
  screen: ScreenConfig | null
}>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}>()

// 状态
const saving = ref(false)
const accessMode = ref<'private' | 'shared' | 'public'>('private')
const selectedUsers = ref<string[]>([])
const selectedRoles = ref<string[]>([])
const userList = ref<{ id: string; name: string; avatar?: string }[]>([])
const roleList = ref<{ id: string; name: string }[]>([])

// 初始化
const handleOpen = async () => {
  if (!props.screen) return

  // 加载用户和角色列表
  try {
    const [usersRes, rolesRes] = await Promise.all([
      getShareableUsers(props.screen.id),
      getShareableRoles(props.screen.id),
    ])
    userList.value = usersRes.list
    roleList.value = rolesRes.list
  } catch (error) {
    console.error('加载用户/角色列表失败:', error)
  }

  // 初始化权限配置
  initPermissions()
}

// 初始化权限配置
const initPermissions = () => {
  if (!props.screen) return

  const permissions = props.screen.permissions
  if (props.screen.isPublic === 1) {
    accessMode.value = 'public'
  } else if (permissions.sharedUsers?.length || permissions.sharedRoles?.length) {
    accessMode.value = 'shared'
    selectedUsers.value = permissions.sharedUsers || []
    selectedRoles.value = permissions.sharedRoles || []
  } else {
    accessMode.value = 'private'
  }
}

// 监听 screen 变化初始化权限
watch(
  () => props.screen,
  () => {
    if (props.modelValue) {
      initPermissions()
    }
  },
  { immediate: true }
)

// 获取用户名
const getUserName = (userId: string) => {
  return userList.value.find(u => u.id === userId)?.name || userId
}

// 获取角色名
const getRoleName = (roleId: string) => {
  return roleList.value.find(r => r.id === roleId)?.name || roleId
}

// 移除用户
const removeUser = (userId: string) => {
  selectedUsers.value = selectedUsers.value.filter(id => id !== userId)
}

// 移除角色
const removeRole = (roleId: string) => {
  selectedRoles.value = selectedRoles.value.filter(id => id !== roleId)
}

// 保存
const handleSave = async () => {
  if (!props.screen) return

  saving.value = true
  try {
    const permissions: ScreenPermissions = {
      isPublic: accessMode.value === 'public',
      sharedUsers: accessMode.value === 'shared' ? selectedUsers.value : [],
      sharedRoles: accessMode.value === 'shared' ? selectedRoles.value : [],
    }

    await updateScreenShare({
      id: props.screen.id,
      permissions,
    })

    ElMessage.success(t('screen.screen.updateSuccess'))
    emit('success')
    emit('update:modelValue', false)
  } catch (error) {
    console.error('保存分享配置失败:', error)
    ElMessage.error(t('screen.screen.share.saveFailed') || '保存失败')
  } finally {
    saving.value = false
  }
}
</script>

<style scoped lang="scss">
.dialog-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;

  .dialog-title {
    font-size: 18px;
    font-weight: 500;
    color: #303133;
  }

  .dialog-actions {
    display: flex;
    gap: 8px;
    margin-right: 30px;
  }
}

.tag-list {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
}
</style>