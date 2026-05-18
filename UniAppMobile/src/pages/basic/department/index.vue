<!-- pages/basic/department/index.vue -->
<template>
  <view class="department-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <input
        v-model="searchKeyword"
        class="search-input"
        type="text"
        placeholder="搜索部门名称"
        @confirm="handleSearch"
      />
      <view class="search-btn" @click="handleSearch">
        <text>搜索</text>
      </view>
    </view>

    <!-- 部门树形列表 -->
    <scroll-view class="department-scroll" scroll-y>
      <view class="department-list">
        <!-- 递归渲染部门树 -->
        <view v-for="dept in filteredDepartmentList" :key="dept.id" class="department-group">
          <DepartmentNode
            :department="dept"
            :level="0"
            :expanded-keys="expandedKeys"
            @edit="handleEdit"
            @toggle="handleToggleExpand"
          />
        </view>

        <!-- 空状态 -->
        <view v-if="!loading && filteredDepartmentList.length === 0" class="empty-state">
          <text class="empty-icon">🏢</text>
          <text class="empty-text">暂无部门数据</text>
          <text class="empty-tip">点击下方按钮添加部门</text>
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
          <text class="popup-title">{{ isEdit ? '编辑部门' : '新增部门' }}</text>
          <view class="popup-close" @click="closePopup">
            <text>×</text>
          </view>
        </view>

        <view class="popup-body">
          <!-- 部门名称 -->
          <view class="form-item">
            <view class="form-label">
              <text class="label-text">部门名称</text>
              <text class="label-required">*</text>
            </view>
            <input
              v-model="formData.name"
              class="form-input"
              type="text"
              placeholder="请输入部门名称"
              maxlength="50"
            />
          </view>

          <!-- 部门编码 -->
          <view class="form-item">
            <view class="form-label">
              <text class="label-text">部门编码</text>
            </view>
            <input
              v-model="formData.code"
              class="form-input"
              type="text"
              placeholder="请输入部门编码"
              maxlength="50"
            />
          </view>

          <!-- 上级部门 -->
          <view class="form-item">
            <view class="form-label">
              <text class="label-text">上级部门</text>
            </view>
            <view class="form-picker" @click="showParentPicker = true">
              <text class="picker-value" :class="{ placeholder: !selectedParentName }">
                {{ selectedParentName || '请选择上级部门（不选则为根部门）' }}
              </text>
              <text class="picker-arrow">▼</text>
            </view>
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
                :class="{ active: formData.status === DepartmentStatus.Enabled }"
                @click="formData.status = DepartmentStatus.Enabled"
              >
                <text>启用</text>
              </view>
              <view
                class="switch-item"
                :class="{ active: formData.status === DepartmentStatus.Disabled }"
                @click="formData.status = DepartmentStatus.Disabled"
              >
                <text>禁用</text>
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

    <!-- 上级部门选择器 -->
    <view v-if="showParentPicker" class="picker-mask" @click="showParentPicker = false">
      <view class="picker-content" @click.stop>
        <view class="picker-header">
          <text class="picker-title">选择上级部门</text>
          <view class="picker-close" @click="showParentPicker = false">
            <text>×</text>
          </view>
        </view>
        <scroll-view class="picker-list" scroll-y>
          <!-- 无上级部门选项 -->
          <view
            class="picker-item"
            :class="{ active: !formData.parentId }"
            @click="selectParent('')"
          >
            <text>无上级部门（作为根部门）</text>
          </view>
          <!-- 部门列表（扁平化） -->
          <view
            v-for="dept in flatDepartmentList"
            :key="dept.id"
            class="picker-item"
            :class="{ active: formData.parentId === dept.id, disabled: editingDepartmentId === dept.id }"
            @click="selectParent(dept.id)"
          >
            <text>{{ dept.name }}</text>
          </view>
        </scroll-view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  getDepartmentTree,
  createDepartment,
  updateDepartment,
  deleteDepartment,
} from '@/api/basic/departmentApi'
import type { Department } from '@/types/basic'
import { DepartmentStatus } from '@/types/basic'
import DepartmentNode from '@/components/DepartmentNode.vue'

// 部门树数据
const departmentList = ref<Department[]>([])

// 加载状态
const loading = ref(false)

// 搜索关键词
const searchKeyword = ref('')

// 弹窗显示状态
const showEditPopup = ref(false)
const showParentPicker = ref(false)

// 编辑中的部门ID
const editingDepartmentId = ref('')

// 是否编辑模式
const isEdit = computed(() => !!editingDepartmentId.value)

// 表单数据
const formData = ref({
  name: '',
  code: '',
  parentId: '',
  sort: 0,
  status: DepartmentStatus.Enabled,
})

// 展开的节点ID集合
const expandedKeys = ref<Set<string>>(new Set())

// 过滤后的部门列表
const filteredDepartmentList = computed(() => {
  if (!searchKeyword.value.trim()) {
    return departmentList.value
  }
  return filterDepartments(departmentList.value, searchKeyword.value.trim())
})

// 扁平化部门列表（用于上级部门选择）
const flatDepartmentList = computed(() => {
  return flattenDepartments(departmentList.value)
})

// 选中的上级部门名称
const selectedParentName = computed(() => {
  if (!formData.value.parentId) return ''
  const parent = flatDepartmentList.value.find(dept => dept.id === formData.value.parentId)
  return parent?.name || ''
})

// 递归过滤部门
const filterDepartments = (departments: Department[], keyword: string): Department[] => {
  const result: Department[] = []
  for (const dept of departments) {
    const matches = dept.name.toLowerCase().includes(keyword.toLowerCase())
    const childrenMatches = dept.children ? filterDepartments(dept.children, keyword) : []

    if (matches || childrenMatches.length > 0) {
      result.push({
        ...dept,
        children: childrenMatches.length > 0 ? childrenMatches : dept.children,
      })
    }
  }
  return result
}

// 扁平化部门列表
const flattenDepartments = (departments: Department[], result: Department[] = []): Department[] => {
  for (const dept of departments) {
    result.push(dept)
    if (dept.children) {
      flattenDepartments(dept.children, result)
    }
  }
  return result
}

// 加载部门树
const loadDepartments = async () => {
  loading.value = true
  try {
    const response = await getDepartmentTree()
    departmentList.value = response
    // 默认展开所有节点
    expandAllNodes(response)
  } catch (error) {
    console.error('加载部门失败:', error)
    uni.showToast({ title: '加载部门失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 展开所有节点
const expandAllNodes = (departments: Department[]) => {
  for (const dept of departments) {
    if (dept.children && dept.children.length > 0) {
      expandedKeys.value.add(dept.id)
      expandAllNodes(dept.children)
    }
  }
}

// 搜索部门
const handleSearch = () => {
  // 搜索时展开所有匹配的节点
  if (searchKeyword.value.trim()) {
    expandedKeys.value.clear()
    expandMatchingNodes(filteredDepartmentList.value)
  }
}

// 展开匹配的节点
const expandMatchingNodes = (departments: Department[]) => {
  for (const dept of departments) {
    if (dept.children && dept.children.length > 0) {
      expandedKeys.value.add(dept.id)
      expandMatchingNodes(dept.children)
    }
  }
}

// 新增根部门
const handleAddRoot = () => {
  editingDepartmentId.value = ''
  formData.value = {
    name: '',
    code: '',
    parentId: '',
    sort: 0,
    status: DepartmentStatus.Enabled,
  }
  showEditPopup.value = true
}

// 编辑部门
const handleEdit = (department: Department) => {
  editingDepartmentId.value = department.id
  formData.value = {
    name: department.name,
    code: department.code || '',
    parentId: department.parentId || '',
    sort: department.sort,
    status: department.status,
  }
  showEditPopup.value = true
}

// 关闭弹窗
const closePopup = () => {
  showEditPopup.value = false
  showParentPicker.value = false
}

// 选择上级部门
const selectParent = (id: string) => {
  // 不能选择自己作为上级部门
  if (id === editingDepartmentId.value) {
    uni.showToast({ title: '不能选择自己作为上级部门', icon: 'none' })
    return
  }
  formData.value.parentId = id
  showParentPicker.value = false
}

// 展开/折叠节点
const handleToggleExpand = (id: string) => {
  if (expandedKeys.value.has(id)) {
    expandedKeys.value.delete(id)
  } else {
    expandedKeys.value.add(id)
  }
}

// 表单验证
const validateForm = (): boolean => {
  if (!formData.value.name.trim()) {
    uni.showToast({ title: '请输入部门名称', icon: 'none' })
    return false
  }
  return true
}

// 保存部门
const handleSave = async () => {
  if (!validateForm()) return

  loading.value = true
  try {
    uni.showLoading({ title: '保存中...' })

    const submitData = {
      id: editingDepartmentId.value,
      name: formData.value.name.trim(),
      code: formData.value.code.trim(),
      parentId: formData.value.parentId || undefined,
      sort: Number(formData.value.sort) || 0,
      status: formData.value.status,
    }

    if (isEdit.value) {
      await updateDepartment({
        ...submitData,
        id: editingDepartmentId.value,
      } as Department)
    } else {
      await createDepartment(submitData)
    }

    uni.hideLoading()
    uni.showToast({ title: '保存成功', icon: 'success' })
    closePopup()

    // 重新加载部门列表
    await loadDepartments()
  } catch (error) {
    uni.hideLoading()
    console.error('保存部门失败:', error)
    uni.showToast({ title: '保存失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 删除部门
const handleDelete = async () => {
  // 检查是否有子部门
  const department = flatDepartmentList.value.find(dept => dept.id === editingDepartmentId.value)
  if (department?.children && department.children.length > 0) {
    uni.showToast({ title: '该部门下有子部门，无法删除', icon: 'none' })
    return
  }

  // 确认删除
  uni.showModal({
    title: '确认删除',
    content: `确定要删除部门"${formData.value.name}"吗？`,
    success: async (res) => {
      if (res.confirm) {
        loading.value = true
        try {
          uni.showLoading({ title: '删除中...' })
          await deleteDepartment(editingDepartmentId.value)
          uni.hideLoading()
          uni.showToast({ title: '删除成功', icon: 'success' })
          closePopup()

          // 重新加载部门列表
          await loadDepartments()
        } catch (error) {
          uni.hideLoading()
          console.error('删除部门失败:', error)
          uni.showToast({ title: '删除失败', icon: 'none' })
        } finally {
          loading.value = false
        }
      }
    },
  })
}

onMounted(() => {
  loadDepartments()
})
</script>

<style lang="scss" scoped>
.department-page {
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

.department-scroll {
  flex: 1;
  overflow: hidden;
}

.department-list {
  padding: 20rpx;
}

.department-group {
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