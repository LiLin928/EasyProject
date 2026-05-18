<!-- pages/basic/role/index.vue -->
<template>
  <view class="role-list-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <input
        v-model="searchKeyword"
        class="search-input"
        type="text"
        placeholder="搜索角色名称/角色编码"
        @confirm="handleSearch"
      />
      <view class="search-btn" @click="handleSearch">
        <text>搜索</text>
      </view>
    </view>

    <!-- 状态筛选栏 -->
    <view class="filter-bar">
      <view class="filter-item" @click="showStatusPicker = true">
        <text class="filter-label">{{ statusText }}</text>
        <text class="filter-arrow">▼</text>
      </view>
    </view>

    <!-- 角色列表 -->
    <scroll-view
      class="role-scroll"
      scroll-y
      @scrolltolower="loadMore"
    >
      <view class="role-list">
        <view
          v-for="item in roleList"
          :key="item.id"
          class="role-card"
          @click="handleDetail(item)"
        >
          <view class="role-info">
            <view class="role-header">
              <text class="role-name">{{ item.name }}</text>
              <view class="role-tags">
                <text :class="['tag', item.status === RoleStatus.Enabled ? 'tag-enabled' : 'tag-disabled']">
                  {{ item.status === RoleStatus.Enabled ? '启用' : '禁用' }}
                </text>
              </view>
            </view>
            <view class="role-detail">
              <text class="detail-row">
                <text class="label">编码：</text>
                <text class="value">{{ item.code || '-' }}</text>
              </text>
              <text class="detail-row">
                <text class="label">描述：</text>
                <text class="value">{{ item.description || '-' }}</text>
              </text>
              <text class="detail-row">
                <text class="label">创建时间：</text>
                <text class="value">{{ item.createTime || '-' }}</text>
              </text>
            </view>
          </view>
          <view class="role-actions">
            <view
              :class="['action-btn', item.status === RoleStatus.Enabled ? 'action-disable' : 'action-enable']"
              @click.stop="handleToggleStatus(item)"
            >
              <text>{{ item.status === RoleStatus.Enabled ? '禁用' : '启用' }}</text>
            </view>
            <view
              class="action-btn action-delete"
              @click.stop="handleDelete(item)"
            >
              <text>删除</text>
            </view>
          </view>
        </view>

        <!-- 加载状态 -->
        <view v-if="loading" class="loading-tip">
          <text>加载中...</text>
        </view>

        <!-- 加载完成 -->
        <view v-if="!hasMore && roleList.length > 0" class="no-more-tip">
          <text>没有更多了</text>
        </view>

        <!-- 空状态 -->
        <view v-if="!loading && roleList.length === 0" class="empty-state">
          <text class="empty-icon">👥</text>
          <text class="empty-text">暂无角色数据</text>
        </view>
      </view>
    </scroll-view>

    <!-- 新增按钮 -->
    <view class="add-btn" @click="handleAdd">
      <text>+</text>
    </view>

    <!-- 状态选择器 -->
    <view v-if="showStatusPicker" class="picker-mask" @click="showStatusPicker = false">
      <view class="picker-content" @click.stop>
        <view class="picker-header">
          <text class="picker-title">选择状态</text>
          <view class="picker-close" @click="showStatusPicker = false">
            <text>×</text>
          </view>
        </view>
        <view class="picker-list">
          <view
            class="picker-item"
            :class="{ active: queryParams.status === undefined }"
            @click="selectStatus(undefined)"
          >
            <text>全部状态</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.status === RoleStatus.Enabled }"
            @click="selectStatus(RoleStatus.Enabled)"
          >
            <text>已启用</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.status === RoleStatus.Disabled }"
            @click="selectStatus(RoleStatus.Disabled)"
          >
            <text>已禁用</text>
          </view>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { getRoleList, enableRole, disableRole, deleteRole } from '@/api/basic/roleApi'
import type { Role, RoleQueryParams } from '@/types/basic'
import { RoleStatus } from '@/types/basic'

// 扩展查询参数以支持状态筛选
interface ExtendedRoleQueryParams extends RoleQueryParams {
  status?: RoleStatus
}

// 搜索关键字
const searchKeyword = ref('')

// 搜索防抖定时器
let searchTimer: number | null = null

// 查询参数
const queryParams = ref<ExtendedRoleQueryParams>({
  pageIndex: 1,
  pageSize: 10,
  keyword: undefined,
  status: undefined,
})

// 角色列表
const roleList = ref<Role[]>([])

// 加载状态
const loading = ref(false)
const hasMore = ref(true)

// 选择器显示状态
const showStatusPicker = ref(false)

// 状态文本
const statusText = computed(() => {
  if (queryParams.value.status === undefined) return '全部状态'
  return queryParams.value.status === RoleStatus.Enabled ? '已启用' : '已禁用'
})

// 加载角色列表
const loadRoles = async (reset = false) => {
  if (loading.value) return
  if (!reset && !hasMore.value) return

  loading.value = true
  try {
    if (reset) {
      queryParams.value.pageIndex = 1
      roleList.value = []
      hasMore.value = true
    }

    const response = await getRoleList(queryParams.value)
    if (reset) {
      roleList.value = response.list
    } else {
      roleList.value = [...roleList.value, ...response.list]
    }

    hasMore.value = roleList.value.length < response.total
    queryParams.value.pageIndex++
  } catch (error: unknown) {
    console.error('加载角色列表失败:', error)
    const message = (error as { errMsg?: string; message?: string })?.errMsg
      || (error as { message?: string })?.message
      || '加载失败'
    uni.showToast({ title: message, icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 搜索（防抖处理）
const handleSearch = () => {
  if (searchTimer) clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    queryParams.value.keyword = searchKeyword.value || undefined
    loadRoles(true)
  }, 300) as unknown as number
}

// 加载更多
const loadMore = () => {
  if (!loading.value && hasMore.value) {
    loadRoles()
  }
}

// 选择状态
const selectStatus = (status: RoleStatus | undefined) => {
  queryParams.value.status = status
  showStatusPicker.value = false
  loadRoles(true)
}

// 新增角色
const handleAdd = () => {
  // TODO: 导航到新增页面
  uni.showToast({ title: '新增角色功能待实现', icon: 'none' })
}

// 查看详情
const handleDetail = (item: Role) => {
  // TODO: 导航到详情页面
  uni.showToast({ title: `查看角色: ${item.name}`, icon: 'none' })
}

// 切换启用/禁用状态
const handleToggleStatus = async (item: Role) => {
  const action = item.status === RoleStatus.Enabled ? '禁用' : '启用'

  // 添加确认对话框
  const res = await uni.showModal({
    title: '确认操作',
    content: `确定要${action}角色 "${item.name}" 吗？`,
  })

  if (!res.confirm) return

  try {
    uni.showLoading({ title: `${action}中...` })
    if (item.status === RoleStatus.Enabled) {
      await disableRole(item.id)
    } else {
      await enableRole(item.id)
    }
    uni.hideLoading()
    uni.showToast({ title: `${action}成功`, icon: 'success' })
    loadRoles(true)
  } catch (error: unknown) {
    uni.hideLoading()
    console.error(`${action}角色失败:`, error)
    const message = (error as { errMsg?: string; message?: string })?.errMsg
      || (error as { message?: string })?.message
      || `${action}失败`
    uni.showToast({ title: message, icon: 'none' })
  }
}

// 删除角色
const handleDelete = async (item: Role) => {
  // 添加确认对话框
  const res = await uni.showModal({
    title: '确认删除',
    content: `确定要删除角色 "${item.name}" 吗？此操作不可恢复。`,
  })

  if (!res.confirm) return

  try {
    uni.showLoading({ title: '删除中...' })
    await deleteRole(item.id)
    uni.hideLoading()
    uni.showToast({ title: '删除成功', icon: 'success' })
    loadRoles(true)
  } catch (error: unknown) {
    uni.hideLoading()
    console.error('删除角色失败:', error)
    const message = (error as { errMsg?: string; message?: string })?.errMsg
      || (error as { message?: string })?.message
      || '删除失败'
    uni.showToast({ title: message, icon: 'none' })
  }
}

onMounted(() => {
  loadRoles(true)
})

onUnmounted(() => {
  if (searchTimer) clearTimeout(searchTimer)
})
</script>

<style lang="scss" scoped>
.role-list-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: $u-bg-color;
}

.search-bar {
  display: flex;
  padding: 20rpx;
  background: #fff;
  gap: 16rpx;

  .search-input {
    flex: 1;
    height: 72rpx;
    padding: 0 24rpx;
    background: $u-light-color;
    border-radius: $u-radius;
    font-size: 28rpx;
  }

  .search-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 120rpx;
    height: 72rpx;
    background: $u-primary;
    border-radius: $u-radius;

    text {
      color: #fff;
      font-size: 28rpx;
    }
  }
}

.filter-bar {
  display: flex;
  padding: 16rpx 20rpx;
  background: #fff;
  border-top: 1rpx solid $u-border-color;
  gap: 20rpx;

  .filter-item {
    display: flex;
    align-items: center;
    padding: 12rpx 20rpx;
    background: $u-light-color;
    border-radius: $u-radius;

    .filter-label {
      font-size: 26rpx;
      color: $u-main-color;
    }

    .filter-arrow {
      font-size: 20rpx;
      color: $u-content-color;
      margin-left: 8rpx;
    }
  }
}

.role-scroll {
  flex: 1;
  overflow: hidden;
}

.role-list {
  padding: 20rpx;
}

.role-card {
  display: flex;
  justify-content: space-between;
  background: #fff;
  border-radius: $u-radius-lg;
  padding: 24rpx;
  margin-bottom: 20rpx;

  .role-info {
    flex: 1;
    display: flex;
    flex-direction: column;
  }

  .role-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    margin-bottom: 16rpx;
  }

  .role-name {
    font-size: 30rpx;
    font-weight: 600;
    color: $u-main-color;
    flex: 1;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .role-tags {
    display: flex;
    gap: 8rpx;
    flex-shrink: 0;

    .tag {
      font-size: 22rpx;
      padding: 4rpx 12rpx;
      border-radius: 4rpx;
    }

    .tag-enabled {
      background: rgba($u-primary, 0.1);
      color: $u-primary;
    }

    .tag-disabled {
      background: rgba($u-content-color, 0.1);
      color: $u-content-color;
    }
  }

  .role-detail {
    display: flex;
    flex-direction: column;
    gap: 8rpx;

    .detail-row {
      display: flex;
      align-items: center;

      .label {
        font-size: 24rpx;
        color: $u-tips-color;
        width: 120rpx;
      }

      .value {
        font-size: 24rpx;
        color: $u-main-color;
        flex: 1;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }
    }
  }

  .role-actions {
    display: flex;
    flex-direction: column;
    align-items: center;
    padding-left: 20rpx;
    gap: 12rpx;

    .action-btn {
      padding: 12rpx 24rpx;
      border-radius: $u-radius;
      font-size: 24rpx;
    }

    .action-enable {
      background: rgba($u-success, 0.1);
      color: $u-success;
    }

    .action-disable {
      background: rgba($u-warning, 0.1);
      color: $u-warning;
    }

    .action-delete {
      background: rgba($u-error, 0.1);
      color: $u-error;
    }
  }
}

.loading-tip,
.no-more-tip {
  text-align: center;
  padding: 30rpx 0;

  text {
    font-size: 26rpx;
    color: $u-tips-color;
  }
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 100rpx 0;

  .empty-icon {
    font-size: 80rpx;
  }

  .empty-text {
    font-size: 28rpx;
    color: $u-tips-color;
    margin-top: 20rpx;
  }
}

.add-btn {
  position: fixed;
  right: 40rpx;
  bottom: 200rpx;
  width: 100rpx;
  height: 100rpx;
  background: $u-primary;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4rpx 16rpx rgba($u-primary, 0.4);

  text {
    font-size: 48rpx;
    color: #fff;
    line-height: 1;
  }
}

.picker-mask {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 1000;
  display: flex;
  align-items: flex-end;
}

.picker-content {
  width: 100%;
  background: #fff;
  border-radius: $u-radius-lg $u-radius-lg 0 0;
  max-height: 70vh;
}

.picker-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 30rpx;
  border-bottom: 1rpx solid $u-border-color;

  .picker-title {
    font-size: 32rpx;
    font-weight: 600;
    color: $u-main-color;
  }

  .picker-close {
    font-size: 40rpx;
    color: $u-content-color;
    line-height: 1;
  }
}

.picker-list {
  max-height: 60vh;
}

.picker-item {
  padding: 30rpx;
  border-bottom: 1rpx solid $u-border-color;

  text {
    font-size: 28rpx;
    color: $u-main-color;
  }

  &.active {
    background: rgba($u-primary, 0.1);

    text {
      color: $u-primary;
      font-weight: 600;
    }
  }
}
</style>