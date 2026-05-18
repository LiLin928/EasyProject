<!-- pages/product/stock/index.vue -->
<template>
  <view class="stock-list-page">
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

    <!-- 库存记录列表 -->
    <scroll-view
      class="stock-scroll"
      scroll-y
      @scrolltolower="loadMore"
    >
      <view class="stock-list">
        <view
          v-for="item in stockList"
          :key="item.id"
          class="stock-card"
        >
          <view class="stock-header">
            <view class="stock-info">
              <text class="stock-name">{{ item.productName }}</text>
              <text class="stock-sku">SKU: {{ item.skuCode }}</text>
            </view>
            <view :class="['stock-quantity', item.type === 1 ? 'quantity-in' : 'quantity-out']">
              <text>{{ item.type === 1 ? '+' : '-' }}{{ Math.abs(item.quantity) }}</text>
            </view>
          </view>

          <view class="stock-body">
            <view class="stock-row">
              <text class="stock-label">变动原因</text>
              <text class="stock-value">{{ item.reason }}</text>
            </view>
            <view class="stock-row">
              <text class="stock-label">操作人</text>
              <text class="stock-value">{{ item.operator }}</text>
            </view>
            <view class="stock-row">
              <text class="stock-label">操作时间</text>
              <text class="stock-value">{{ formatTime(item.operateTime) }}</text>
            </view>
          </view>

          <view class="stock-tag">
            <text :class="['tag-text', item.type === 1 ? 'tag-in' : 'tag-out']">
              {{ item.type === 1 ? '入库' : (item.type === 2 ? '出库' : '调整') }}
            </text>
          </view>
        </view>

        <!-- 加载状态 -->
        <view v-if="loading" class="loading-tip">
          <text>加载中...</text>
        </view>

        <!-- 加载完成 -->
        <view v-if="!hasMore && stockList.length > 0" class="no-more-tip">
          <text>没有更多了</text>
        </view>

        <!-- 空状态 -->
        <view v-if="!loading && stockList.length === 0" class="empty-state">
          <text class="empty-icon">📊</text>
          <text class="empty-text">暂无库存记录</text>
        </view>
      </view>
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getStockList } from '@/api/product/stockApi'
import type { StockRecord, StockQueryParams } from '@/types'

// 搜索关键字
const searchKeyword = ref('')

// 查询参数
const queryParams = ref<StockQueryParams>({
  pageIndex: 1,
  pageSize: 10,
  productName: undefined,
})

// 库存记录列表
const stockList = ref<StockRecord[]>([])

// 加载状态
const loading = ref(false)
const hasMore = ref(true)

// 格式化时间
const formatTime = (time: string) => {
  if (!time) return ''
  const date = new Date(time)
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')
  return `${year}-${month}-${day} ${hours}:${minutes}`
}

// 加载库存记录列表
const loadStockList = async (reset = false) => {
  if (loading.value) return
  if (!reset && !hasMore.value) return

  loading.value = true
  try {
    if (reset) {
      queryParams.value.pageIndex = 1
      stockList.value = []
      hasMore.value = true
    }

    const response = await getStockList(queryParams.value)
    if (reset) {
      stockList.value = response.list
    } else {
      stockList.value = [...stockList.value, ...response.list]
    }

    hasMore.value = stockList.value.length < response.total
    queryParams.value.pageIndex++
  } catch (error) {
    console.error('加载库存记录失败:', error)
    uni.showToast({ title: '加载失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  queryParams.value.productName = searchKeyword.value || undefined
  loadStockList(true)
}

// 加载更多
const loadMore = () => {
  if (!loading.value && hasMore.value) {
    loadStockList()
  }
}

onMounted(() => {
  loadStockList(true)
})
</script>

<style lang="scss" scoped>
.stock-list-page {
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

.stock-scroll {
  flex: 1;
  overflow: hidden;
}

.stock-list {
  padding: 20rpx;
}

.stock-card {
  background: #fff;
  border-radius: $u-radius-lg;
  padding: 24rpx;
  margin-bottom: 20rpx;

  .stock-header {
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    padding-bottom: 20rpx;
    border-bottom: 1rpx solid $u-border-color;
  }

  .stock-info {
    flex: 1;
  }

  .stock-name {
    font-size: 30rpx;
    font-weight: 600;
    color: $u-main-color;
    display: block;
  }

  .stock-sku {
    font-size: 24rpx;
    color: $u-tips-color;
    margin-top: 8rpx;
    display: block;
  }

  .stock-quantity {
    padding: 8rpx 20rpx;
    border-radius: $u-radius;
    font-size: 28rpx;
    font-weight: 600;
  }

  .quantity-in {
    background: rgba($u-success, 0.1);
    color: $u-success;
  }

  .quantity-out {
    background: rgba($u-error, 0.1);
    color: $u-error;
  }

  .stock-body {
    padding-top: 20rpx;
  }

  .stock-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 16rpx;

    &:last-child {
      margin-bottom: 0;
    }
  }

  .stock-label {
    font-size: 26rpx;
    color: $u-tips-color;
  }

  .stock-value {
    font-size: 26rpx;
    color: $u-main-color;
  }

  .stock-tag {
    display: flex;
    justify-content: flex-end;
    padding-top: 16rpx;

    .tag-text {
      font-size: 22rpx;
      padding: 4rpx 12rpx;
      border-radius: 4rpx;
    }

    .tag-in {
      background: rgba($u-success, 0.1);
      color: $u-success;
    }

    .tag-out {
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
</style>