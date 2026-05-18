<!-- pages/basic/menu/index.vue -->
<template>
  <view class="menu-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <input
        v-model="searchKeyword"
        class="search-input"
        type="text"
        placeholder="搜索菜单名称"
        @confirm="handleSearch"
      />
      <view class="search-btn" @click="handleSearch">
        <text>搜索</text>
      </view>
    </view>

    <!-- 菜单树形列表 -->
    <scroll-view class="menu-scroll" scroll-y>
      <view class="menu-list">
        <!-- 递归渲染菜单树 -->
        <view v-for="menu in filteredMenuList" :key="menu.id" class="menu-group">
          <MenuNode
            :menu="menu"
            :level="0"
            :expanded-keys="expandedKeys"
            @edit="handleEdit"
            @toggle="handleToggleExpand"
          />
        </view>

        <!-- 空状态 -->
        <view v-if="!loading && filteredMenuList.length === 0" class="empty-state">
          <text class="empty-icon">📋</text>
          <text class="empty-text">暂无菜单数据</text>
          <text class="empty-tip">点击下方按钮添加菜单</text>
        </view>

        <!-- 加载状态 -->
        <view v-if="loading" class="loading-state">
          <text>加载中...</text>
        </view>
      </view>
    </scroll-view>

    <!-- 新增按钮 -->
    <view class="add-btn" @click="handleAddRoot">
      <text>+</text>
    </view>

    <!-- 编辑弹窗 -->
    <view v-if="showEditPopup" class="popup-mask" @click="closePopup">
      <view class="popup-content" @click.stop>
        <view class="popup-header">
          <text class="popup-title">{{ isEdit ? '编辑菜单' : '新增菜单' }}</text>
          <view class="popup-close" @click="closePopup">
            <text>×</text>
          </view>
        </view>

        <view class="popup-body">
          <!-- 菜单名称 -->
          <view class="form-item">
            <view class="form-label">
              <text class="label-text">菜单名称</text>
              <text class="label-required">*</text>
            </view>
            <input
              v-model="formData.name"
              class="form-input"
              type="text"
              placeholder="请输入菜单名称"
              maxlength="50"
            />
          </view>

          <!-- 菜单类型 -->
          <view class="form-item">
            <view class="form-label">
              <text class="label-text">菜单类型</text>
              <text class="label-required">*</text>
            </view>
            <view class="type-switch">
              <view
                class="switch-item"
                :class="{ active: formData.type === MenuType.Directory }"
                @click="formData.type = MenuType.Directory"
              >
                <text>目录</text>
              </view>
              <view
                class="switch-item"
                :class="{ active: formData.type === MenuType.Menu }"
                @click="formData.type = MenuType.Menu"
              >
                <text>菜单</text>
              </view>
              <view
                class="switch-item"
                :class="{ active: formData.type === MenuType.Button }"
                @click="formData.type = MenuType.Button"
              >
                <text>按钮</text>
              </view>
            </view>
          </view>

          <!-- 上级菜单 -->
          <view class="form-item">
            <view class="form-label">
              <text class="label-text">上级菜单</text>
            </view>
            <view class="form-picker" @click="showParentPicker = true">
              <text class="picker-value" :class="{ placeholder: !selectedParentName }">
                {{ selectedParentName || '请选择上级菜单（不选则为根菜单）' }}
              </text>
              <text class="picker-arrow">▼</text>
            </view>
          </view>

          <!-- 路由路径 -->
          <view v-if="formData.type === MenuType.Menu" class="form-item">
            <view class="form-label">
              <text class="label-text">路由路径</text>
            </view>
            <input
              v-model="formData.path"
              class="form-input"
              type="text"
              placeholder="请输入路由路径"
              maxlength="100"
            />
          </view>

          <!-- 图标 -->
          <view v-if="formData.type !== MenuType.Button" class="form-item">
            <view class="form-label">
              <text class="label-text">图标</text>
            </view>
            <input
              v-model="formData.icon"
              class="form-input"
              type="text"
              placeholder="请输入图标名称"
              maxlength="50"
            />
          </view>

          <!-- 排序 -->
          <view class="form-item">
            <view class="form-label">
              <text class="label-text">排序</text>
            </view>
            <input
              v-model="formData.sort"
              class="form-input"
              type="number"
              placeholder="请输入排序值（默认为0）"
            />
          </view>

          <!-- 状态 -->
          <view class="form-item">
            <view class="form-label">
              <text class="label-text">状态</text>
            </view>
            <view class="status-switch">
              <view
                class="switch-item"
                :class="{ active: formData.status === MenuStatus.Visible }"
                @click="formData.status = MenuStatus.Visible"
              >
                <text>显示</text>
              </view>
              <view
                class="switch-item"
                :class="{ active: formData.status === MenuStatus.Hidden }"
                @click="formData.status = MenuStatus.Hidden"
              >
                <text>隐藏</text>
              </view>
            </view>
          </view>
        </view>

        <view class="popup-footer">
          <view v-if="isEdit" class="action-btn action-delete" @click="handleDelete">
            <text>删除</text>
          </view>
          <view class="action-btn action-cancel" @click="closePopup">
            <text>取消</text>
          </view>
          <view class="action-btn action-save" @click="handleSave">
            <text>保存</text>
          </view>
        </view>
      </view>
    </view>

    <!-- 上级菜单选择器 -->
    <view v-if="showParentPicker" class="picker-mask" @click="showParentPicker = false">
      <view class="picker-content" @click.stop>
        <view class="picker-header">
          <text class="picker-title">选择上级菜单</text>
          <view class="picker-close" @click="showParentPicker = false">
            <text>×</text>
          </view>
        </view>
        <scroll-view class="picker-list" scroll-y>
          <!-- 无上级菜单选项 -->
          <view
            class="picker-item"
            :class="{ active: !formData.parentId }"
            @click="selectParent('')"
          >
            <text>无上级菜单（作为根菜单）</text>
          </view>
          <!-- 菜单列表（扁平化） -->
          <view
            v-for="menu in flatMenuList"
            :key="menu.id"
            class="picker-item"
            :class="{ active: formData.parentId === menu.id, disabled: editingMenuId === menu.id }"
            @click="selectParent(menu.id)"
          >
            <text>{{ menu.name }}</text>
          </view>
        </scroll-view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import {
  getMenuTree,
  createMenu,
  updateMenu,
  deleteMenu,
} from '@/api/basic/menuApi'
import type { Menu } from '@/types/basic'
import { MenuType, MenuStatus } from '@/types/basic'
import MenuNode from '@/components/MenuNode.vue'

// 菜单树数据
const menuList = ref<Menu[]>([])

// 加载状态
const loading = ref(false)

// 搜索关键词
const searchKeyword = ref('')

// 搜索防抖定时器
let searchTimer: ReturnType<typeof setTimeout> | null = null

// 弹窗显示状态
const showEditPopup = ref(false)
const showParentPicker = ref(false)

// 编辑中的菜单ID
const editingMenuId = ref('')

// 是否编辑模式
const isEdit = computed(() => !!editingMenuId.value)

// 表单数据
const formData = ref({
  name: '',
  type: MenuType.Menu,
  parentId: '',
  path: '',
  icon: '',
  sort: 0,
  status: MenuStatus.Visible,
})

// 展开的节点ID集合
const expandedKeys = ref<Set<string>>(new Set())

// 过滤后的菜单列表
const filteredMenuList = computed(() => {
  if (!searchKeyword.value.trim()) {
    return menuList.value
  }
  return filterMenus(menuList.value, searchKeyword.value.trim())
})

// 扁平化菜单列表（用于上级菜单选择）
const flatMenuList = computed(() => {
  return flattenMenus(menuList.value)
})

// 选中的上级菜单名称
const selectedParentName = computed(() => {
  if (!formData.value.parentId) return ''
  const parent = flatMenuList.value.find(menu => menu.id === formData.value.parentId)
  return parent?.name || ''
})

// 递归过滤菜单
const filterMenus = (menus: Menu[], keyword: string): Menu[] => {
  const result: Menu[] = []
  for (const menu of menus) {
    const matches = menu.name.toLowerCase().includes(keyword.toLowerCase())
    const childrenMatches = menu.children ? filterMenus(menu.children, keyword) : []

    if (matches || childrenMatches.length > 0) {
      result.push({
        ...menu,
        children: childrenMatches.length > 0 ? childrenMatches : menu.children,
      })
    }
  }
  return result
}

// 扁平化菜单列表
const flattenMenus = (menus: Menu[], result: Menu[] = []): Menu[] => {
  for (const menu of menus) {
    result.push(menu)
    if (menu.children) {
      flattenMenus(menu.children, result)
    }
  }
  return result
}

// 加载菜单树
const loadMenus = async () => {
  loading.value = true
  try {
    const response = await getMenuTree()
    menuList.value = response
    // 默认展开所有节点
    expandAllNodes(response)
  } catch (error) {
    console.error('加载菜单失败:', error)
    uni.showToast({ title: '加载菜单失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 展开所有节点
const expandAllNodes = (menus: Menu[]) => {
  const newSet = new Set<string>()
  for (const menu of menus) {
    if (menu.children && menu.children.length > 0) {
      newSet.add(menu.id)
      expandAllNodesRecursive(menu.children, newSet)
    }
  }
  expandedKeys.value = newSet
}

// 递归收集所有节点ID
const expandAllNodesRecursive = (menus: Menu[], set: Set<string>) => {
  for (const menu of menus) {
    if (menu.children && menu.children.length > 0) {
      set.add(menu.id)
      expandAllNodesRecursive(menu.children, set)
    }
  }
}

// 搜索菜单
const handleSearch = () => {
  // 搜索时展开所有匹配的节点
  if (searchKeyword.value.trim()) {
    const newSet = new Set<string>()
    expandMatchingNodes(filteredMenuList.value, newSet)
    expandedKeys.value = newSet
  }
}

// 展开匹配的节点
const expandMatchingNodes = (menus: Menu[], set: Set<string>) => {
  for (const menu of menus) {
    if (menu.children && menu.children.length > 0) {
      set.add(menu.id)
      expandMatchingNodes(menu.children, set)
    }
  }
}

// 新增根菜单
const handleAddRoot = () => {
  editingMenuId.value = ''
  formData.value = {
    name: '',
    type: MenuType.Menu,
    parentId: '',
    path: '',
    icon: '',
    sort: 0,
    status: MenuStatus.Visible,
  }
  showEditPopup.value = true
}

// 编辑菜单
const handleEdit = (menu: Menu) => {
  editingMenuId.value = menu.id
  formData.value = {
    name: menu.name,
    type: menu.type,
    parentId: menu.parentId || '',
    path: menu.path || '',
    icon: menu.icon || '',
    sort: menu.sort,
    status: menu.status,
  }
  showEditPopup.value = true
}

// 关闭弹窗
const closePopup = () => {
  showEditPopup.value = false
  showParentPicker.value = false
}

// 选择上级菜单
const selectParent = (id: string) => {
  // 不能选择自己作为上级菜单
  if (id === editingMenuId.value) {
    uni.showToast({ title: '不能选择自己作为上级菜单', icon: 'none' })
    return
  }
  formData.value.parentId = id
  showParentPicker.value = false
}

// 展开/折叠节点
const handleToggleExpand = (id: string) => {
  const newSet = new Set(expandedKeys.value)
  if (newSet.has(id)) {
    newSet.delete(id)
  } else {
    newSet.add(id)
  }
  expandedKeys.value = newSet  // 重新赋值触发响应式更新
}

// 表单验证
const validateForm = (): boolean => {
  if (!formData.value.name.trim()) {
    uni.showToast({ title: '请输入菜单名称', icon: 'none' })
    return false
  }
  return true
}

// 保存菜单
const handleSave = async () => {
  if (!validateForm()) return

  loading.value = true
  try {
    uni.showLoading({ title: '保存中...' })

    const submitData = {
      id: editingMenuId.value,
      name: formData.value.name.trim(),
      type: formData.value.type,
      parentId: formData.value.parentId || undefined,
      path: formData.value.type === MenuType.Menu ? formData.value.path.trim() : undefined,
      icon: formData.value.type !== MenuType.Button ? formData.value.icon.trim() : undefined,
      sort: Number(formData.value.sort) || 0,
      status: formData.value.status,
    }

    if (isEdit.value) {
      await updateMenu({
        ...submitData,
        id: editingMenuId.value,
      } as Menu)
    } else {
      await createMenu(submitData)
    }

    uni.hideLoading()
    uni.showToast({ title: '保存成功', icon: 'success' })
    closePopup()

    // 重新加载菜单列表
    await loadMenus()
  } catch (error) {
    uni.hideLoading()
    console.error('保存菜单失败:', error)
    uni.showToast({ title: '保存失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 删除菜单
const handleDelete = async () => {
  // 检查是否有子菜单
  const menu = flatMenuList.value.find(m => m.id === editingMenuId.value)
  if (menu?.children && menu.children.length > 0) {
    uni.showToast({ title: '该菜单下有子菜单，无法删除', icon: 'none' })
    return
  }

  // 确认删除
  uni.showModal({
    title: '确认删除',
    content: `确定要删除菜单"${formData.value.name}"吗？`,
    success: async (res) => {
      if (res.confirm) {
        loading.value = true
        try {
          uni.showLoading({ title: '删除中...' })
          await deleteMenu(editingMenuId.value)
          uni.hideLoading()
          uni.showToast({ title: '删除成功', icon: 'success' })
          closePopup()

          // 重新加载菜单列表
          await loadMenus()
        } catch (error) {
          uni.hideLoading()
          console.error('删除菜单失败:', error)
          uni.showToast({ title: '删除失败', icon: 'none' })
        } finally {
          loading.value = false
        }
      }
    },
  })
}

onMounted(() => {
  loadMenus()
})

// 监听搜索关键词变化（防抖实时搜索）
watch(searchKeyword, (newVal) => {
  if (searchTimer) clearTimeout(searchTimer)
  searchTimer = setTimeout(() => {
    handleSearch()
  }, 300) as unknown as ReturnType<typeof setTimeout>
})

// 清理定时器
onUnmounted(() => {
  if (searchTimer) clearTimeout(searchTimer)
})
</script>

<style lang="scss" scoped>
.menu-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: $u-bg-color;
}

.search-bar {
  display: flex;
  padding: 20rpx;
  gap: 16rpx;
}

.search-input {
  flex: 1;
  height: 72rpx;
  padding: 0 20rpx;
  background: #fff;
  border-radius: $u-radius;
  font-size: 28rpx;
  color: $u-main-color;
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
    font-size: 28rpx;
    color: #fff;
  }
}

.menu-scroll {
  flex: 1;
  overflow: hidden;
}

.menu-list {
  padding: 20rpx;
}

.menu-group {
  background: #fff;
  border-radius: $u-radius-lg;
  margin-bottom: 16rpx;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 120rpx 0;

  .empty-icon {
    font-size: 80rpx;
  }

  .empty-text {
    font-size: 28rpx;
    color: $u-tips-color;
    margin-top: 20rpx;
  }

  .empty-tip {
    font-size: 24rpx;
    color: $u-tips-color;
    margin-top: 12rpx;
  }
}

.loading-state {
  display: flex;
  justify-content: center;
  padding: 60rpx 0;

  text {
    font-size: 28rpx;
    color: $u-tips-color;
  }
}

.add-btn {
  position: fixed;
  right: 40rpx;
  bottom: 120rpx;
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

.popup-mask {
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

.popup-content {
  width: 100%;
  background: #fff;
  border-radius: $u-radius-lg $u-radius-lg 0 0;
}

.popup-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 30rpx;
  border-bottom: 1rpx solid $u-border-color;

  .popup-title {
    font-size: 32rpx;
    font-weight: 600;
    color: $u-main-color;
  }

  .popup-close {
    font-size: 40rpx;
    color: $u-content-color;
    line-height: 1;
  }
}

.popup-body {
  padding: 24rpx;
}

.form-item {
  margin-bottom: 24rpx;
}

.form-label {
  display: flex;
  align-items: center;
  margin-bottom: 16rpx;

  .label-text {
    font-size: 28rpx;
    font-weight: 600;
    color: $u-main-color;
  }

  .label-required {
    font-size: 28rpx;
    color: $u-error;
    margin-left: 8rpx;
  }
}

.form-input {
  width: 100%;
  height: 72rpx;
  padding: 0 20rpx;
  background: $u-light-color;
  border-radius: $u-radius;
  font-size: 28rpx;
  color: $u-main-color;
}

.form-picker {
  display: flex;
  justify-content: space-between;
  align-items: center;
  height: 72rpx;
  padding: 0 20rpx;
  background: $u-light-color;
  border-radius: $u-radius;

  .picker-value {
    font-size: 28rpx;
    color: $u-main-color;

    &.placeholder {
      color: $u-tips-color;
    }
  }

  .picker-arrow {
    font-size: 20rpx;
    color: $u-content-color;
  }
}

.type-switch {
  display: flex;
  gap: 12rpx;

  .switch-item {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 72rpx;
    background: $u-light-color;
    border-radius: $u-radius;

    text {
      font-size: 28rpx;
      color: $u-content-color;
    }

    &.active {
      background: rgba($u-primary, 0.1);

      text {
        color: $u-primary;
        font-weight: 600;
      }
    }
  }
}

.status-switch {
  display: flex;
  gap: 16rpx;

  .switch-item {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 72rpx;
    background: $u-light-color;
    border-radius: $u-radius;

    text {
      font-size: 28rpx;
      color: $u-content-color;
    }

    &.active {
      background: rgba($u-primary, 0.1);

      text {
        color: $u-primary;
        font-weight: 600;
      }
    }
  }
}

.popup-footer {
  display: flex;
  padding: 20rpx 24rpx;
  gap: 16rpx;
  border-top: 1rpx solid $u-border-color;

  .action-btn {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 88rpx;
    border-radius: $u-radius;
    font-size: 30rpx;
  }

  .action-delete {
    background: rgba($u-error, 0.1);
    color: $u-error;
  }

  .action-cancel {
    background: $u-light-color;
    color: $u-content-color;
  }

  .action-save {
    background: $u-primary;
    color: #fff;
    font-weight: 600;
  }
}

.picker-mask {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  z-index: 1001;
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

  &.disabled {
    opacity: 0.5;
  }
}
</style>