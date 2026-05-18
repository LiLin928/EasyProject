<!-- pages/product/detail/index.vue -->
<template>
  <view class="product-detail-page">
    <!-- 图片轮播 -->
    <swiper
      class="image-swiper"
      indicator-dots
      autoplay
      circular
      :interval="3000"
    >
      <swiper-item v-for="(img, index) in imageList" :key="index">
        <image
          class="swiper-image"
          :src="img"
          mode="aspectFill"
          @click="previewImage(index)"
        />
      </swiper-item>
    </swiper>

    <!-- 商品信息 -->
    <view class="product-info-section">
      <!-- 名称和价格 -->
      <view class="product-header">
        <text class="product-name">{{ productDetail?.name || '' }}</text>
        <view class="product-tags">
          <text v-if="productDetail?.isHot" class="tag tag-hot">热销</text>
          <text v-if="productDetail?.isNew" class="tag tag-new">新品</text>
        </view>
      </view>

      <!-- 价格 -->
      <view class="price-section">
        <text class="price-current">¥{{ (productDetail?.price || 0).toFixed(2) }}</text>
        <text v-if="productDetail?.originalPrice" class="price-original">
          ¥{{ (productDetail?.originalPrice || 0).toFixed(2) }}
        </text>
      </view>

      <!-- 基本信息 -->
      <view class="info-card">
        <view class="info-row">
          <text class="info-label">SKU码</text>
          <text class="info-value">{{ productDetail?.skuCode || '-' }}</text>
        </view>
        <view class="info-row">
          <text class="info-label">分类</text>
          <text class="info-value">{{ productDetail?.categoryName || '-' }}</text>
        </view>
        <view class="info-row">
          <text class="info-label">库存</text>
          <text class="info-value">{{ productDetail?.stock || 0 }} 件</text>
        </view>
        <view class="info-row">
          <text class="info-label">状态</text>
          <view class="status-tag" :class="productDetail?.status === 1 ? 'status-on' : 'status-off'">
            <text>{{ productDetail?.status === 1 ? '已上架' : '已下架' }}</text>
          </view>
        </view>
      </view>

      <!-- 商品描述 -->
      <view class="description-section">
        <view class="section-title">
          <text>商品描述</text>
        </view>
        <view class="description-content">
          <rich-text
            v-if="productDetail?.description"
            :nodes="productDetail?.description"
          />
          <text v-else class="empty-text">暂无描述</text>
        </view>
      </view>
    </view>

    <!-- 加载状态 -->
    <view v-if="loading" class="loading-mask">
      <text>加载中...</text>
    </view>

    <!-- 空状态 -->
    <view v-if="!loading && !productDetail" class="empty-state">
      <text class="empty-icon">📦</text>
      <text class="empty-text">商品不存在或已删除</text>
    </view>

    <!-- 底部操作栏 -->
    <view class="bottom-actions" v-if="productDetail">
      <view class="action-btn action-edit" @click="handleEdit">
        <text>编辑</text>
      </view>
      <view
        class="action-btn action-shelf"
        :class="productDetail?.status === 1 ? 'action-off' : 'action-on'"
        @click="handleToggleStatus"
      >
        <text>{{ productDetail?.status === 1 ? '下架' : '上架' }}</text>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getProductDetail, onShelfProduct, offShelfProduct } from '@/api/product/productApi'
import type { Product } from '@/types/product'

// 商品ID
const productId = ref('')

// 商品详情
const productDetail = ref<Product | null>(null)

// 加载状态
const loading = ref(false)

// 图片列表
const imageList = computed(() => {
  if (!productDetail.value) return ['/static/logo.png']
  const images = productDetail.value.images || []
  if (images.length > 0) return images
  if (productDetail.value.image) return [productDetail.value.image]
  return ['/static/logo.png']
})

// 加载商品详情
const loadProductDetail = async () => {
  if (!productId.value) {
    uni.showToast({ title: '商品ID不存在', icon: 'none' })
    return
  }

  loading.value = true
  try {
    const response = await getProductDetail(productId.value)
    productDetail.value = response
  } catch (error) {
    console.error('加载商品详情失败:', error)
    uni.showToast({ title: '加载失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 预览图片
const previewImage = (index: number) => {
  uni.previewImage({
    current: index,
    urls: imageList.value,
  })
}

// 编辑商品
const handleEdit = () => {
  if (!productDetail.value) return
  uni.navigateTo({ url: `/pages/product/edit/index?id=${productDetail.value.id}` })
}

// 切换上架状态
const handleToggleStatus = async () => {
  if (!productDetail.value) return

  const action = productDetail.value.status === 1 ? '下架' : '上架'
  try {
    uni.showLoading({ title: `${action}中...` })
    if (productDetail.value.status === 1) {
      await offShelfProduct(productDetail.value.id)
    } else {
      await onShelfProduct(productDetail.value.id)
    }
    uni.hideLoading()
    uni.showToast({ title: `${action}成功`, icon: 'success' })
    // 更新本地状态
    productDetail.value.status = productDetail.value.status === 1 ? 0 : 1
  } catch (error) {
    uni.hideLoading()
    uni.showToast({ title: `${action}失败`, icon: 'none' })
  }
}

onMounted(() => {
  // 从路由参数获取商品ID
  const pages = getCurrentPages()
  const currentPage = pages[pages.length - 1]
  const options = (currentPage as any).options || {}
  productId.value = options.id || ''
  loadProductDetail()
})
</script>

<style lang="scss" scoped>
.product-detail-page {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background: $u-bg-color;
  padding-bottom: 120rpx;
}

.image-swiper {
  width: 100%;
  height: 500rpx;
  background: #fff;
}

.swiper-image {
  width: 100%;
  height: 100%;
}

.product-info-section {
  padding: 20rpx;
}

.product-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  background: #fff;
  padding: 24rpx;
  border-radius: $u-radius-lg;
}

.product-name {
  font-size: 36rpx;
  font-weight: 600;
  color: $u-main-color;
  flex: 1;
}

.product-tags {
  display: flex;
  gap: 8rpx;
  flex-shrink: 0;

  .tag {
    font-size: 22rpx;
    padding: 6rpx 14rpx;
    border-radius: 4rpx;
  }

  .tag-hot {
    background: rgba($u-error, 0.1);
    color: $u-error;
  }

  .tag-new {
    background: rgba($u-success, 0.1);
    color: $u-success;
  }
}

.price-section {
  display: flex;
  align-items: baseline;
  gap: 16rpx;
  background: #fff;
  padding: 24rpx;
  margin-top: 16rpx;
  border-radius: $u-radius-lg;

  .price-current {
    font-size: 40rpx;
    font-weight: 700;
    color: $u-error;
  }

  .price-original {
    font-size: 26rpx;
    color: $u-tips-color;
    text-decoration: line-through;
  }
}

.info-card {
  background: #fff;
  padding: 24rpx;
  margin-top: 16rpx;
  border-radius: $u-radius-lg;
}

.info-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 16rpx 0;
  border-bottom: 1rpx solid $u-border-color;

  &:last-child {
    border-bottom: none;
  }

  .info-label {
    font-size: 28rpx;
    color: $u-tips-color;
  }

  .info-value {
    font-size: 28rpx;
    color: $u-main-color;
  }
}

.status-tag {
  padding: 6rpx 16rpx;
  border-radius: 4rpx;
  font-size: 24rpx;

  &.status-on {
    background: rgba($u-primary, 0.1);
    color: $u-primary;
  }

  &.status-off {
    background: rgba($u-content-color, 0.1);
    color: $u-content-color;
  }
}

.description-section {
  background: #fff;
  margin-top: 16rpx;
  border-radius: $u-radius-lg;
  overflow: hidden;
}

.section-title {
  padding: 24rpx;
  border-bottom: 1rpx solid $u-border-color;

  text {
    font-size: 32rpx;
    font-weight: 600;
    color: $u-main-color;
  }
}

.description-content {
  padding: 24rpx;
  font-size: 28rpx;
  color: $u-content-color;
  line-height: 1.6;
}

.empty-text {
  color: $u-tips-color;
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

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 100rpx 0;

  .empty-icon {
    font-size: 100rpx;
  }

  .empty-text {
    font-size: 28rpx;
    color: $u-tips-color;
    margin-top: 24rpx;
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

  .action-edit {
    background: $u-light-color;
    color: $u-content-color;
  }

  .action-shelf {
    font-weight: 600;
  }

  .action-on {
    background: $u-success;
    color: #fff;
  }

  .action-off {
    background: $u-warning;
    color: #fff;
  }
}
</style>