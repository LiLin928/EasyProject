<!-- pages/product/category/index.vue -->
<template>
  <view class="category-page">
    <!-- 分类列表 -->
    <scroll-view class="category-scroll" scroll-y>
      <view class="category-list">
        <!-- 遍历一级分类 -->
        <view v-for="parent in categoryList" :key="parent.id" class="category-group">
          <!-- 一级分类 -->
          <view class="category-item level-1" @click="handleEdit(parent)">
            <view class="category-content">
              <text class="category-name">{{ parent.name }}</text>
              <text class="category-sort">排序: {{ parent.sort }}</text>
            </view>
            <text class="category-arrow">›</text>
          </view>

          <!-- 二级分类 -->
          <view v-if="parent.children && parent.children.length > 0" class="category-children">
            <view
              v-for="child in parent.children"
              :key="child.id"
              class="category-item level-2"
              @click="handleEdit(child)"
            >
              <view class="category-content">
                <text class="category-name">{{ child.name }}</text>
                <text class="category-sort">排序: {{ child.sort }}</text>
              </view>
              <text class="category-arrow">›</text>
            </view>
          </view>
        </view>

        <!-- 空状态 -->
        <view v-if="!loading && categoryList.length === 0" class="empty-state">
          <text class="empty-icon">📁</text>
          <text class="empty-text">暂无分类数据</text>
          <text class="empty-tip">点击下方按钮添加分类</text>
        </view>

        <!-- 加载状态 -->
        <view v-if="loading" class="loading-state">
          <text>加载中...</text>
        </view>
      </view>
    </scroll-view>

    <!-- 新增按钮 -->
    <view class="add-btn" @click="handleAdd">
      <text>+</text>
    </view>

    <!-- 编辑弹窗 -->
    <view v-if="showEditPopup" class="popup-mask" @click="closePopup">
      <view class="popup-content" @click.stop>
        <view class="popup-header">
          <text class="popup-title">{{ isEdit ? '编辑分类' : '新增分类' }}</text>
          <view class="popup-close" @click="closePopup">
            <text>×</text>
          </view>
        </view>

        <view class="popup-body">
          <!-- 分类名称 -->
          <view class="form-item">
            <view class="form-label">
              <text class="label-text">分类名称</text>
              <text class="label-required">*</text>
            </view>
            <input
              v-model="formData.name"
              class="form-input"
              type="text"
              placeholder="请输入分类名称"
              maxlength="50"
            />
          </view>

          <!-- 上级分类 -->
          <view class="form-item">
            <view class="form-label">
              <text class="label-text">上级分类</text>
            </view>
            <view class="form-picker" @click="showParentPicker = true">
              <text class="picker-value" :class="{ placeholder: !selectedParentName }">
                {{ selectedParentName || '请选择上级分类（不选则为一级分类）' }}
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

    <!-- 上级分类选择器 -->
    <view v-if="showParentPicker" class="picker-mask" @click="showParentPicker = false">
      <view class="picker-content" @click.stop>
        <view class="picker-header">
          <text class="picker-title">选择上级分类</text>
          <view class="picker-close" @click="showParentPicker = false">
            <text>×</text>
          </view>
        </view>
        <scroll-view class="picker-list" scroll-y>
          <!-- 无上级分类选项 -->
          <view
            class="picker-item"
            :class="{ active: !formData.parentId }"
            @click="selectParent('')"
          >
            <text>无上级分类（作为一级分类）</text>
          </view>
          <!-- 一级分类列表 -->
          <view
            v-for="cat in level1Categories"
            :key="cat.id"
            class="picker-item"
            :class="{ active: formData.parentId === cat.id, disabled: editingCategoryId === cat.id }"
            @click="selectParent(cat.id)"
          >
            <text>{{ cat.name }}</text>
          </view>
        </scroll-view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getCategoryTree, createCategory, updateCategory, deleteCategory } from '@/api/product/categoryApi'
import type { ProductCategory } from '@/types/product'

// 分类列表
const categoryList = ref<ProductCategory[]>([])

// 加载状态
const loading = ref(false)

// 弹窗显示状态
const showEditPopup = ref(false)
const showParentPicker = ref(false)

// 编辑中的分类ID
const editingCategoryId = ref('')

// 是否编辑模式
const isEdit = computed(() => !!editingCategoryId.value)

// 表单数据
const formData = ref({
  name: '',
  parentId: '',
  sort: 0,
})

// 一级分类列表（用于上级分类选择）
const level1Categories = computed(() => {
  return categoryList.value.filter(cat => cat.level === 0)
})

// 选中的上级分类名称
const selectedParentName = computed(() => {
  if (!formData.value.parentId) return ''
  const parent = level1Categories.value.find(cat => cat.id === formData.value.parentId)
  return parent?.name || ''
})

// 加载分类树
const loadCategories = async () => {
  loading.value = true
  try {
    const response = await getCategoryTree()
    categoryList.value = response
  } catch (error) {
    console.error('加载分类失败:', error)
    uni.showToast({ title: '加载分类失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 新增分类
const handleAdd = () => {
  editingCategoryId.value = ''
  formData.value = {
    name: '',
    parentId: '',
    sort: 0,
  }
  showEditPopup.value = true
}

// 编辑分类
const handleEdit = (category: ProductCategory) => {
  editingCategoryId.value = category.id
  formData.value = {
    name: category.name,
    parentId: category.parentId || '',
    sort: category.sort,
  }
  showEditPopup.value = true
}

// 关闭弹窗
const closePopup = () => {
  showEditPopup.value = false
  showParentPicker.value = false
}

// 选择上级分类
const selectParent = (id: string) => {
  // 不能选择自己作为上级分类
  if (id === editingCategoryId.value) {
    uni.showToast({ title: '不能选择自己作为上级分类', icon: 'none' })
    return
  }
  formData.value.parentId = id
  showParentPicker.value = false
}

// 表单验证
const validateForm = (): boolean => {
  if (!formData.value.name.trim()) {
    uni.showToast({ title: '请输入分类名称', icon: 'none' })
    return false
  }
  return true
}

// 保存分类
const handleSave = async () => {
  if (!validateForm()) return

  loading.value = true
  try {
    uni.showLoading({ title: '保存中...' })

    const submitData = {
      id: editingCategoryId.value,
      name: formData.value.name.trim(),
      parentId: formData.value.parentId || undefined,
      sort: Number(formData.value.sort) || 0,
      level: formData.value.parentId ? 1 : 0,
    }

    if (isEdit.value) {
      await updateCategory({
        ...submitData,
        id: editingCategoryId.value,
      } as ProductCategory)
    } else {
      await createCategory(submitData)
    }

    uni.hideLoading()
    uni.showToast({ title: '保存成功', icon: 'success' })
    closePopup()

    // 重新加载分类列表
    await loadCategories()
  } catch (error) {
    uni.hideLoading()
    console.error('保存分类失败:', error)
    uni.showToast({ title: '保存失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 删除分类
const handleDelete = async () => {
  // 检查是否有子分类
  const category = categoryList.value.find(cat => cat.id === editingCategoryId.value)
  if (category?.children && category.children.length > 0) {
    uni.showToast({ title: '该分类下有子分类，无法删除', icon: 'none' })
    return
  }

  // 确认删除
  uni.showModal({
    title: '确认删除',
    content: `确定要删除分类"${formData.value.name}"吗？`,
    success: async (res) => {
      if (res.confirm) {
        loading.value = true
        try {
          uni.showLoading({ title: '删除中...' })
          await deleteCategory(editingCategoryId.value)
          uni.hideLoading()
          uni.showToast({ title: '删除成功', icon: 'success' })
          closePopup()

          // 重新加载分类列表
          await loadCategories()
        } catch (error) {
          uni.hideLoading()
          console.error('删除分类失败:', error)
          uni.showToast({ title: '删除失败', icon: 'none' })
        } finally {
          loading.value = false
        }
      }
    },
  })
}

onMounted(() => {
  loadCategories()
})
</script>

<style lang="scss" scoped>
.category-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: $u-bg-color;
}

.category-scroll {
  flex: 1;
  overflow: hidden;
}

.category-list {
  padding: 20rpx;
}

.category-group {
  background: #fff;
  border-radius: $u-radius-lg;
  margin-bottom: 16rpx;
}

.category-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 30rpx 24rpx;

  &.level-1 {
    border-bottom: 1rpx solid $u-border-color;

    .category-name {
      font-size: 32rpx;
      font-weight: 600;
      color: $u-main-color;
    }
  }

  &.level-2 {
    padding-left: 48rpx;
    background: $u-light-color;

    .category-name {
      font-size: 28rpx;
      color: $u-content-color;
    }
  }
}

.category-content {
  display: flex;
  flex-direction: column;
  gap: 8rpx;
}

.category-name {
  font-size: 28rpx;
  color: $u-main-color;
}

.category-sort {
  font-size: 24rpx;
  color: $u-tips-color;
}

.category-arrow {
  font-size: 32rpx;
  color: $u-content-color;
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