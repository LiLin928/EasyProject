<!-- pages/product/edit/index.vue -->
<template>
  <view class="product-edit-page">
    <!-- 表单区域 -->
    <scroll-view class="form-scroll" scroll-y>
      <view class="form-section">
        <!-- SKU码 -->
        <view class="form-item">
          <view class="form-label">
            <text class="label-text">SKU码</text>
            <text class="label-required">*</text>
          </view>
          <input
            v-model="formData.skuCode"
            class="form-input"
            type="text"
            placeholder="请输入SKU码"
            maxlength="50"
          />
        </view>

        <!-- 商品名称 -->
        <view class="form-item">
          <view class="form-label">
            <text class="label-text">商品名称</text>
            <text class="label-required">*</text>
          </view>
          <input
            v-model="formData.name"
            class="form-input"
            type="text"
            placeholder="请输入商品名称"
            maxlength="100"
          />
        </view>

        <!-- 售价 -->
        <view class="form-item">
          <view class="form-label">
            <text class="label-text">售价</text>
            <text class="label-required">*</text>
          </view>
          <view class="form-input-wrapper">
            <input
              v-model="formData.price"
              class="form-input"
              type="digit"
              placeholder="请输入售价"
            />
            <text class="input-unit">元</text>
          </view>
        </view>

        <!-- 原价 -->
        <view class="form-item">
          <view class="form-label">
            <text class="label-text">原价</text>
          </view>
          <view class="form-input-wrapper">
            <input
              v-model="formData.originalPrice"
              class="form-input"
              type="digit"
              placeholder="请输入原价（可选）"
            />
            <text class="input-unit">元</text>
          </view>
        </view>

        <!-- 分类 -->
        <view class="form-item">
          <view class="form-label">
            <text class="label-text">商品分类</text>
            <text class="label-required">*</text>
          </view>
          <view class="form-picker" @click="showCategoryPicker = true">
            <text class="picker-value" :class="{ placeholder: !selectedCategoryName }">
              {{ selectedCategoryName || '请选择分类' }}
            </text>
            <text class="picker-arrow">▼</text>
          </view>
        </view>

        <!-- 库存 -->
        <view class="form-item">
          <view class="form-label">
            <text class="label-text">库存</text>
            <text class="label-required">*</text>
          </view>
          <view class="form-input-wrapper">
            <input
              v-model="formData.stock"
              class="form-input"
              type="number"
              placeholder="请输入库存数量"
            />
            <text class="input-unit">件</text>
          </view>
        </view>

        <!-- 主图上传 -->
        <view class="form-item">
          <view class="form-label">
            <text class="label-text">主图</text>
            <text class="label-required">*</text>
          </view>
          <view class="image-upload-area">
            <view
              v-if="formData.image"
              class="image-item"
            >
              <image
                class="upload-image"
                :src="formData.image"
                mode="aspectFill"
              />
              <view class="image-delete" @click="deleteMainImage">
                <text>×</text>
              </view>
            </view>
            <view
              v-else
              class="image-upload-btn"
              @click="chooseMainImage"
            >
              <text class="upload-icon">+</text>
              <text class="upload-text">上传主图</text>
            </view>
          </view>
        </view>

        <!-- 商品图片 -->
        <view class="form-item">
          <view class="form-label">
            <text class="label-text">商品图片</text>
          </view>
          <view class="image-upload-area multi">
            <view
              v-for="(img, index) in formData.images"
              :key="index"
              class="image-item"
            >
              <image
                class="upload-image"
                :src="img"
                mode="aspectFill"
              />
              <view class="image-delete" @click="deleteImage(index)">
                <text>×</text>
              </view>
            </view>
            <view
              v-if="formData.images.length < 9"
              class="image-upload-btn"
              @click="chooseImages"
            >
              <text class="upload-icon">+</text>
              <text class="upload-text">添加图片</text>
            </view>
          </view>
          <text class="image-tip">最多上传9张图片</text>
        </view>

        <!-- 商品描述 -->
        <view class="form-item">
          <view class="form-label">
            <text class="label-text">商品描述</text>
          </view>
          <textarea
            v-model="formData.description"
            class="form-textarea"
            placeholder="请输入商品描述"
            maxlength="2000"
          />
        </view>

        <!-- 标签设置 -->
        <view class="form-item">
          <view class="form-label">
            <text class="label-text">商品标签</text>
          </view>
          <view class="checkbox-group">
            <view
              class="checkbox-item"
              :class="{ active: formData.isHot }"
              @click="formData.isHot = !formData.isHot"
            >
              <view class="checkbox-icon">
                <text v-if="formData.isHot">✓</text>
              </view>
              <text class="checkbox-label">热销商品</text>
            </view>
            <view
              class="checkbox-item"
              :class="{ active: formData.isNew }"
              @click="formData.isNew = !formData.isNew"
            >
              <view class="checkbox-icon">
                <text v-if="formData.isNew">✓</text>
              </view>
              <text class="checkbox-label">新品上架</text>
            </view>
          </view>
        </view>
      </view>
    </scroll-view>

    <!-- 底部操作栏 -->
    <view class="bottom-actions">
      <view class="action-btn action-cancel" @click="handleCancel">
        <text>取消</text>
      </view>
      <view class="action-btn action-save" @click="handleSave">
        <text>保存</text>
      </view>
    </view>

    <!-- 分类选择器 -->
    <view v-if="showCategoryPicker" class="picker-mask" @click="showCategoryPicker = false">
      <view class="picker-content" @click.stop>
        <view class="picker-header">
          <text class="picker-title">选择分类</text>
          <view class="picker-close" @click="showCategoryPicker = false">
            <text>×</text>
          </view>
        </view>
        <scroll-view class="picker-list" scroll-y>
          <view
            v-for="cat in flatCategoryList"
            :key="cat.id"
            class="picker-item"
            :class="{ active: formData.categoryId === cat.id }"
            @click="selectCategory(cat)"
          >
            <text :style="{ paddingLeft: cat.level * 20 + 'rpx' }">{{ cat.name }}</text>
          </view>
        </scroll-view>
      </view>
    </view>

    <!-- 加载状态 -->
    <view v-if="loading" class="loading-mask">
      <text>加载中...</text>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getProductDetail, createProduct, updateProduct } from '@/api/product/productApi'
import { getCategoryTree } from '@/api/product/categoryApi'
import type { Product, ProductCategory, ProductFormParams } from '@/types/product'

// 商品ID（编辑模式）
const productId = ref('')

// 是否编辑模式
const isEdit = computed(() => !!productId.value)

// 加载状态
const loading = ref(false)

// 分类选择器
const showCategoryPicker = ref(false)
const categoryList = ref<ProductCategory[]>([])

// 表单数据
const formData = ref<ProductFormParams>({
  skuCode: '',
  name: '',
  price: 0,
  originalPrice: undefined,
  image: '',
  images: [],
  categoryId: '',
  stock: 0,
  isHot: false,
  isNew: false,
  description: '',
})

// 扁平化分类列表（用于选择器）
const flatCategoryList = computed(() => {
  const result: Array<ProductCategory & { level: number }> = []
  const flatten = (categories: ProductCategory[], level: number) => {
    for (const cat of categories) {
      result.push({ ...cat, level })
      if (cat.children && cat.children.length > 0) {
        flatten(cat.children, level + 1)
      }
    }
  }
  flatten(categoryList.value, 0)
  return result
})

// 选中的分类名称
const selectedCategoryName = computed(() => {
  if (!formData.value.categoryId) return ''
  const cat = flatCategoryList.value.find(c => c.id === formData.value.categoryId)
  return cat?.name || ''
})

// 加载分类列表
const loadCategories = async () => {
  try {
    const response = await getCategoryTree()
    categoryList.value = response
  } catch (error) {
    console.error('加载分类失败:', error)
    uni.showToast({ title: '加载分类失败', icon: 'none' })
  }
}

// 加载商品详情（编辑模式）
const loadProductDetail = async () => {
  if (!productId.value) return

  loading.value = true
  try {
    const response = await getProductDetail(productId.value)
    // 填充表单数据
    formData.value = {
      id: response.id,
      skuCode: response.skuCode,
      name: response.name,
      price: response.price,
      originalPrice: response.originalPrice,
      image: response.image,
      images: response.images || [],
      categoryId: response.categoryId,
      stock: response.stock,
      isHot: response.isHot,
      isNew: response.isNew,
      description: response.description,
    }
  } catch (error) {
    console.error('加载商品详情失败:', error)
    uni.showToast({ title: '加载商品详情失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 选择分类
const selectCategory = (cat: ProductCategory) => {
  formData.value.categoryId = cat.id
  showCategoryPicker.value = false
}

// 上传主图
const chooseMainImage = () => {
  uni.chooseImage({
    count: 1,
    sizeType: ['compressed'],
    sourceType: ['album', 'camera'],
    success: (res) => {
      formData.value.image = res.tempFilePaths[0]
    },
    fail: (err) => {
      console.error('选择图片失败:', err)
    },
  })
}

// 删除主图
const deleteMainImage = () => {
  formData.value.image = ''
}

// 上传商品图片
const chooseImages = () => {
  const remaining = 9 - formData.value.images.length
  uni.chooseImage({
    count: remaining,
    sizeType: ['compressed'],
    sourceType: ['album', 'camera'],
    success: (res) => {
      formData.value.images = [...formData.value.images, ...res.tempFilePaths]
    },
    fail: (err) => {
      console.error('选择图片失败:', err)
    },
  })
}

// 删除商品图片
const deleteImage = (index: number) => {
  formData.value.images.splice(index, 1)
}

// 表单验证
const validateForm = (): boolean => {
  if (!formData.value.skuCode.trim()) {
    uni.showToast({ title: '请输入SKU码', icon: 'none' })
    return false
  }
  if (!formData.value.name.trim()) {
    uni.showToast({ title: '请输入商品名称', icon: 'none' })
    return false
  }
  if (!formData.value.price || formData.value.price <= 0) {
    uni.showToast({ title: '请输入正确的售价', icon: 'none' })
    return false
  }
  if (!formData.value.categoryId) {
    uni.showToast({ title: '请选择商品分类', icon: 'none' })
    return false
  }
  if (!formData.value.stock || formData.value.stock < 0) {
    uni.showToast({ title: '请输入正确的库存数量', icon: 'none' })
    return false
  }
  if (!formData.value.image) {
    uni.showToast({ title: '请上传主图', icon: 'none' })
    return false
  }
  return true
}

// 保存商品
const handleSave = async () => {
  if (!validateForm()) return

  loading.value = true
  try {
    uni.showLoading({ title: '保存中...' })

    // 准备提交数据
    const submitData: ProductFormParams = {
      ...formData.value,
      price: Number(formData.value.price),
      originalPrice: formData.value.originalPrice ? Number(formData.value.originalPrice) : undefined,
      stock: Number(formData.value.stock),
    }

    if (isEdit.value) {
      await updateProduct(submitData)
    } else {
      await createProduct(submitData)
    }

    uni.hideLoading()
    uni.showToast({ title: '保存成功', icon: 'success' })

    // 返回上一页
    setTimeout(() => {
      uni.navigateBack()
    }, 1500)
  } catch (error) {
    uni.hideLoading()
    console.error('保存商品失败:', error)
    uni.showToast({ title: '保存失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 取消编辑
const handleCancel = () => {
  uni.navigateBack()
}

onMounted(() => {
  // 从路由参数获取商品ID
  const pages = getCurrentPages()
  const currentPage = pages[pages.length - 1]
  const options = (currentPage as any).options || {}
  productId.value = options.id || ''

  // 加载分类列表
  loadCategories()

  // 编辑模式加载商品详情
  if (productId.value) {
    uni.setNavigationBarTitle({ title: '编辑商品' })
    loadProductDetail()
  } else {
    uni.setNavigationBarTitle({ title: '新增商品' })
  }
})
</script>

<style lang="scss" scoped>
.product-edit-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: $u-bg-color;
}

.form-scroll {
  flex: 1;
  overflow: hidden;
  padding-bottom: 120rpx;
}

.form-section {
  padding: 20rpx;
}

.form-item {
  background: #fff;
  padding: 24rpx;
  margin-bottom: 16rpx;
  border-radius: $u-radius-lg;
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

.form-input-wrapper {
  display: flex;
  align-items: center;
  background: $u-light-color;
  border-radius: $u-radius;
  height: 72rpx;

  .form-input {
    flex: 1;
    background: transparent;
    height: 100%;
  }

  .input-unit {
    font-size: 28rpx;
    color: $u-content-color;
    padding-right: 20rpx;
  }
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

.form-textarea {
  width: 100%;
  height: 200rpx;
  padding: 20rpx;
  background: $u-light-color;
  border-radius: $u-radius;
  font-size: 28rpx;
  color: $u-main-color;
  line-height: 1.5;
}

.image-upload-area {
  display: flex;
  flex-wrap: wrap;
  gap: 16rpx;

  &.multi {
    min-height: 160rpx;
  }
}

.image-item {
  position: relative;
  width: 160rpx;
  height: 160rpx;

  .upload-image {
    width: 100%;
    height: 100%;
    border-radius: $u-radius;
  }

  .image-delete {
    position: absolute;
    top: -8rpx;
    right: -8rpx;
    width: 32rpx;
    height: 32rpx;
    background: rgba(0, 0, 0, 0.6);
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;

    text {
      font-size: 24rpx;
      color: #fff;
      line-height: 1;
    }
  }
}

.image-upload-btn {
  width: 160rpx;
  height: 160rpx;
  background: $u-light-color;
  border-radius: $u-radius;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;

  .upload-icon {
    font-size: 48rpx;
    color: $u-content-color;
    line-height: 1;
  }

  .upload-text {
    font-size: 24rpx;
    color: $u-tips-color;
    margin-top: 8rpx;
  }
}

.image-tip {
  font-size: 24rpx;
  color: $u-tips-color;
  margin-top: 12rpx;
}

.checkbox-group {
  display: flex;
  gap: 24rpx;
}

.checkbox-item {
  display: flex;
  align-items: center;
  padding: 16rpx 24rpx;
  background: $u-light-color;
  border-radius: $u-radius;
  border: 2rpx solid transparent;

  &.active {
    background: rgba($u-primary, 0.1);
    border-color: $u-primary;

    .checkbox-icon {
      background: $u-primary;
      border-color: $u-primary;

      text {
        color: #fff;
      }
    }

    .checkbox-label {
      color: $u-primary;
    }
  }

  .checkbox-icon {
    width: 36rpx;
    height: 36rpx;
    border: 2rpx solid $u-border-color;
    border-radius: 4rpx;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-right: 12rpx;

    text {
      font-size: 24rpx;
      color: transparent;
      line-height: 1;
    }
  }

  .checkbox-label {
    font-size: 28rpx;
    color: $u-content-color;
  }
}

.bottom-actions {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  display: flex;
  padding: 20rpx;
  background: #fff;
  box-shadow: 0 -2rpx 10rpx rgba(0, 0, 0, 0.05);
  gap: 20rpx;

  .action-btn {
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
    height: 88rpx;
    border-radius: $u-radius;
    font-size: 32rpx;
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

.loading-mask {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255, 255, 255, 0.9);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 100;

  text {
    font-size: 28rpx;
    color: $u-tips-color;
  }
}
</style>