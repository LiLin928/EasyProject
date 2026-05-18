<!-- pages/product/list/index.vue -->
<template>
  <view class="product-list-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <input
        v-model="searchKeyword"
        class="search-input"
        type="text"
        placeholder="搜索商品名称"
        @confirm="handleSearch"
      />
      <view class="search-btn" @click="handleSearch">
        <text>搜索</text>
      </view>
    </view>

    <!-- 筛选栏 -->
    <view class="filter-bar">
      <view class="filter-item" @click="showCategoryPicker = true">
        <text class="filter-label">{{ selectedCategoryName || '全部分类' }}</text>
        <text class="filter-arrow">▼</text>
      </view>
      <view class="filter-item" @click="showStatusPicker = true">
        <text class="filter-label">{{ statusText }}</text>
        <text class="filter-arrow">▼</text>
      </view>
    </view>

    <!-- 商品列表 -->
    <scroll-view
      class="product-scroll"
      scroll-y
      @scrolltolower="loadMore"
    >
      <view class="product-list">
        <view
          v-for="item in productList"
          :key="item.id"
          class="product-card"
        >
          <image
            class="product-image"
            :src="item.image || '/static/logo.png'"
            mode="aspectFill"
          />
          <view class="product-info">
            <view class="product-header">
              <text class="product-name">{{ item.name }}</text>
              <view class="product-tags">
                <text v-if="item.isHot" class="tag tag-hot">热销</text>
                <text v-if="item.isNew" class="tag tag-new">新品</text>
                <text :class="['tag', item.status === 1 ? 'tag-on' : 'tag-off']">
                  {{ item.status === 1 ? '上架' : '下架' }}
                </text>
              </view>
            </view>
            <text class="product-sku">SKU: {{ item.skuCode }}</text>
            <view class="product-price">
              <text class="price-current">¥{{ item.price.toFixed(2) }}</text>
              <text v-if="item.originalPrice" class="price-original">¥{{ item.originalPrice.toFixed(2) }}</text>
            </view>
            <view class="product-footer">
              <text class="product-stock">库存: {{ item.stock }}</text>
              <view class="product-actions">
                <view class="action-btn action-edit" @click="handleEdit(item)">
                  <text>编辑</text>
                </view>
                <view
                  :class="['action-btn', item.status === 1 ? 'action-off' : 'action-on']"
                  @click="handleToggleStatus(item)"
                >
                  <text>{{ item.status === 1 ? '下架' : '上架' }}</text>
                </view>
              </view>
            </view>
          </view>
        </view>

        <!-- 加载状态 -->
        <view v-if="loading" class="loading-tip">
          <text>加载中...</text>
        </view>

        <!-- 加载完成 -->
        <view v-if="!hasMore && productList.length > 0" class="no-more-tip">
          <text>没有更多了</text>
        </view>

        <!-- 空状态 -->
        <view v-if="!loading && productList.length === 0" class="empty-state">
          <text class="empty-icon">📦</text>
          <text class="empty-text">暂无商品数据</text>
        </view>
      </view>
    </scroll-view>

    <!-- 新增按钮 -->
    <view class="add-btn" @click="handleAdd">
      <text>+</text>
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
            class="picker-item"
            :class="{ active: !queryParams.categoryId }"
            @click="selectCategory('')"
          >
            <text>全部分类</text>
          </view>
          <view
            v-for="cat in categoryList"
            :key="cat.id"
            class="picker-item"
            :class="{ active: queryParams.categoryId === cat.id }"
            @click="selectCategory(cat.id)"
          >
            <text>{{ cat.name }}</text>
          </view>
        </scroll-view>
      </view>
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
            :class="{ active: queryParams.status === 1 }"
            @click="selectStatus(1)"
          >
            <text>已上架</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.status === 0 }"
            @click="selectStatus(0)"
          >
            <text>已下架</text>
          </view>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getProductList, onShelfProduct, offShelfProduct } from '@/api/product/productApi'
import { getCategoryTree } from '@/api/product/categoryApi'
import type { Product, ProductCategory, ProductQueryParams, ProductStatus } from '@/types/product'

// 搜索关键字
const searchKeyword = ref('')

// 查询参数
const queryParams = ref<ProductQueryParams>({
  pageIndex: 1,
  pageSize: 10,
  name: undefined,
  categoryId: undefined,
  status: undefined,
})

// 商品列表
const productList = ref<Product[]>([])

// 分类列表
const categoryList = ref<ProductCategory[]>([])

// 加载状态
const loading = ref(false)
const hasMore = ref(true)

// 选择器显示状态
const showCategoryPicker = ref(false)
const showStatusPicker = ref(false)

// 当前选中的分类名称
const selectedCategoryName = computed(() => {
  if (!queryParams.value.categoryId) return ''
  const cat = findCategoryById(categoryList.value, queryParams.value.categoryId)
  return cat?.name || ''
})

// 状态文本
const statusText = computed(() => {
  if (queryParams.value.status === undefined) return '全部状态'
  return queryParams.value.status === 1 ? '已上架' : '已下架'
})

// 递归查找分类
const findCategoryById = (categories: ProductCategory[], id: string): ProductCategory | undefined => {
  for (const cat of categories) {
    if (cat.id === id) return cat
    if (cat.children) {
      const found = findCategoryById(cat.children, id)
      if (found) return found
    }
  }
  return undefined
}

// 加载商品列表
const loadProducts = async (reset = false) => {
  if (loading.value) return
  if (!reset && !hasMore.value) return

  loading.value = true
  try {
    if (reset) {
      queryParams.value.pageIndex = 1
      productList.value = []
      hasMore.value = true
    }

    const response = await getProductList(queryParams.value)
    if (reset) {
      productList.value = response.list
    } else {
      productList.value = [...productList.value, ...response.list]
    }

    hasMore.value = productList.value.length < response.total
    queryParams.value.pageIndex++
  } catch (error) {
    console.error('加载商品列表失败:', error)
    uni.showToast({ title: '加载失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 加载分类列表
const loadCategories = async () => {
  try {
    const response = await getCategoryTree()
    categoryList.value = response
  } catch (error) {
    console.error('加载分类失败:', error)
  }
}

// 搜索
const handleSearch = () => {
  queryParams.value.name = searchKeyword.value || undefined
  loadProducts(true)
}

// 加载更多
const loadMore = () => {
  if (!loading.value && hasMore.value) {
    loadProducts()
  }
}

// 选择分类
const selectCategory = (id: string) => {
  queryParams.value.categoryId = id || undefined
  showCategoryPicker.value = false
  loadProducts(true)
}

// 选择状态
const selectStatus = (status: ProductStatus | undefined) => {
  queryParams.value.status = status
  showStatusPicker.value = false
  loadProducts(true)
}

// 新增商品
const handleAdd = () => {
  uni.navigateTo({ url: '/pages/product/edit/index' })
}

// 编辑商品
const handleEdit = (item: Product) => {
  uni.navigateTo({ url: `/pages/product/edit/index?id=${item.id}` })
}

// 切换上架状态
const handleToggleStatus = async (item: Product) => {
  const action = item.status === 1 ? '下架' : '上架'
  try {
    uni.showLoading({ title: `${action}中...` })
    if (item.status === 1) {
      await offShelfProduct(item.id)
    } else {
      await onShelfProduct(item.id)
    }
    uni.hideLoading()
    uni.showToast({ title: `${action}成功`, icon: 'success' })
    loadProducts(true)
  } catch (error) {
    uni.hideLoading()
    uni.showToast({ title: `${action}失败`, icon: 'none' })
  }
}

onMounted(() => {
  loadProducts(true)
  loadCategories()
})
</script>

<style lang="scss" scoped>
.product-list-page {
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

.product-scroll {
  flex: 1;
  overflow: hidden;
}

.product-list {
  padding: 20rpx;
}

.product-card {
  display: flex;
  background: #fff;
  border-radius: $u-radius-lg;
  padding: 20rpx;
  margin-bottom: 20rpx;

  .product-image {
    width: 160rpx;
    height: 160rpx;
    border-radius: $u-radius;
    flex-shrink: 0;
  }

  .product-info {
    flex: 1;
    margin-left: 20rpx;
    display: flex;
    flex-direction: column;
  }

  .product-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
  }

  .product-name {
    font-size: 30rpx;
    font-weight: 600;
    color: $u-main-color;
    flex: 1;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .product-tags {
    display: flex;
    gap: 8rpx;
    flex-shrink: 0;

    .tag {
      font-size: 20rpx;
      padding: 4rpx 12rpx;
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

    .tag-on {
      background: rgba($u-primary, 0.1);
      color: $u-primary;
    }

    .tag-off {
      background: rgba($u-content-color, 0.1);
      color: $u-content-color;
    }
  }

  .product-sku {
    font-size: 24rpx;
    color: $u-tips-color;
    margin-top: 8rpx;
  }

  .product-price {
    display: flex;
    align-items: baseline;
    gap: 12rpx;
    margin-top: 12rpx;

    .price-current {
      font-size: 32rpx;
      font-weight: 600;
      color: $u-error;
    }

    .price-original {
      font-size: 24rpx;
      color: $u-tips-color;
      text-decoration: line-through;
    }
  }

  .product-footer {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: auto;
    padding-top: 16rpx;
  }

  .product-stock {
    font-size: 24rpx;
    color: $u-content-color;
  }

  .product-actions {
    display: flex;
    gap: 16rpx;

    .action-btn {
      padding: 8rpx 20rpx;
      border-radius: $u-radius;
      font-size: 24rpx;
    }

    .action-edit {
      background: $u-light-color;
      color: $u-content-color;
    }

    .action-on {
      background: rgba($u-success, 0.1);
      color: $u-success;
    }

    .action-off {
      background: rgba($u-warning, 0.1);
      color: $u-warning;
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