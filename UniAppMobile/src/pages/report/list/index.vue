<!-- pages/report/list/index.vue -->
<template>
  <view class="report-list-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <input
        v-model="searchKeyword"
        class="search-input"
        type="text"
        placeholder="搜索报表名称"
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
      <view class="filter-item" @click="showChartTypePicker = true">
        <text class="filter-label">{{ chartTypeText }}</text>
        <text class="filter-arrow">▼</text>
      </view>
    </view>

    <!-- 报表列表 -->
    <scroll-view
      class="report-scroll"
      scroll-y
      @scrolltolower="loadMore"
    >
      <view class="report-list">
        <view
          v-for="item in reportList"
          :key="item.id"
          class="report-card"
          @click="handleView(item)"
        >
          <view class="report-header">
            <text class="report-name">{{ item.name }}</text>
            <view :class="['chart-type-tag', getChartTypeClass(item.chartType)]">
              <text>{{ getChartTypeText(item.chartType) }}</text>
            </view>
          </view>
          <view class="report-info">
            <text class="report-category">分类: {{ item.categoryName || '未分类' }}</text>
            <text v-if="item.description" class="report-desc">{{ item.description }}</text>
          </view>
          <view class="report-footer">
            <text class="report-time">{{ item.createTime }}</text>
          </view>
        </view>

        <!-- 加载状态 -->
        <view v-if="loading" class="loading-tip">
          <text>加载中...</text>
        </view>

        <!-- 加载完成 -->
        <view v-if="!hasMore && reportList.length > 0" class="no-more-tip">
          <text>没有更多了</text>
        </view>

        <!-- 空状态 -->
        <view v-if="!loading && reportList.length === 0" class="empty-state">
          <text class="empty-icon">📊</text>
          <text class="empty-text">暂无报表数据</text>
        </view>
      </view>
    </scroll-view>

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

    <!-- 图表类型选择器 -->
    <view v-if="showChartTypePicker" class="picker-mask" @click="showChartTypePicker = false">
      <view class="picker-content" @click.stop>
        <view class="picker-header">
          <text class="picker-title">选择图表类型</text>
          <view class="picker-close" @click="showChartTypePicker = false">
            <text>×</text>
          </view>
        </view>
        <view class="picker-list">
          <view
            class="picker-item"
            :class="{ active: queryParams.chartType === undefined }"
            @click="selectChartType(undefined)"
          >
            <text>全部类型</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.chartType === ChartType.Line }"
            @click="selectChartType(ChartType.Line)"
          >
            <text>折线图</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.chartType === ChartType.Bar }"
            @click="selectChartType(ChartType.Bar)"
          >
            <text>柱状图</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.chartType === ChartType.Pie }"
            @click="selectChartType(ChartType.Pie)"
          >
            <text>饼图</text>
          </view>
          <view
            class="picker-item"
            :class="{ active: queryParams.chartType === ChartType.Table }"
            @click="selectChartType(ChartType.Table)"
          >
            <text>表格</text>
          </view>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getReportList, getReportCategories } from '@/api/report/reportApi'
import type { Report, ReportCategory, ReportQueryParams, ChartType } from '@/types/report'

// 导入枚举
const ChartTypeEnum = {
  Line: 1,
  Bar: 2,
  Pie: 3,
  Table: 4,
  Mixed: 5,
}

// 搜索关键字
const searchKeyword = ref('')

// 搜索防抖定时器
let searchTimer: number | null = null

// 查询参数
const queryParams = ref<ReportQueryParams>({
  pageIndex: 1,
  pageSize: 10,
  keyword: undefined,
  categoryId: undefined,
  chartType: undefined,
})

// 报表列表
const reportList = ref<Report[]>([])

// 分类列表
const categoryList = ref<ReportCategory[]>([])

// 加载状态
const loading = ref(false)
const hasMore = ref(true)

// 选择器显示状态
const showCategoryPicker = ref(false)
const showChartTypePicker = ref(false)

// 当前选中的分类名称
const selectedCategoryName = computed(() => {
  if (!queryParams.value.categoryId) return ''
  const cat = findCategoryById(categoryList.value, queryParams.value.categoryId)
  return cat?.name || ''
})

// 图表类型文本
const chartTypeText = computed(() => {
  if (queryParams.value.chartType === undefined) return '全部类型'
  return getChartTypeText(queryParams.value.chartType)
})

// 递归查找分类
const findCategoryById = (categories: ReportCategory[], id: string): ReportCategory | undefined => {
  for (const cat of categories) {
    if (cat.id === id) return cat
    if (cat.children) {
      const found = findCategoryById(cat.children, id)
      if (found) return found
    }
  }
  return undefined
}

// 获取图表类型文本
const getChartTypeText = (chartType: ChartType | number): string => {
  const typeMap: Record<number, string> = {
    1: '折线图',
    2: '柱状图',
    3: '饼图',
    4: '表格',
    5: '混合图表',
  }
  return typeMap[chartType] || '未知类型'
}

// 获取图表类型样式类
const getChartTypeClass = (chartType: ChartType | number): string => {
  const classMap: Record<number, string> = {
    1: 'tag-line',    // 折线图 - 蓝色
    2: 'tag-bar',     // 柱状图 - 绿色
    3: 'tag-pie',     // 饼图 - 橙色
    4: 'tag-table',   // 表格 - 灰色
    5: 'tag-mixed',   // 混合 - 紫色
  }
  return classMap[chartType] || 'tag-table'
}

// 加载报表列表
const loadReports = async (reset = false) => {
  if (loading.value) return
  if (!reset && !hasMore.value) return

  loading.value = true
  try {
    if (reset) {
      queryParams.value.pageIndex = 1
      reportList.value = []
      hasMore.value = true
    }

    const response = await getReportList(queryParams.value)
    if (reset) {
      reportList.value = response.list
    } else {
      reportList.value = [...reportList.value, ...response.list]
    }

    hasMore.value = reportList.value.length < response.total
    queryParams.value.pageIndex++
  } catch (error) {
    console.error('加载报表列表失败:', error)
    uni.showToast({ title: '加载失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 加载分类列表
const loadCategories = async () => {
  try {
    const response = await getReportCategories()
    categoryList.value = response
  } catch (error) {
    console.error('加载分类失败:', error)
  }
}

// 搜索（带防抖）
const handleSearch = () => {
  if (searchTimer) {
    clearTimeout(searchTimer)
  }
  searchTimer = setTimeout(() => {
    queryParams.value.keyword = searchKeyword.value || undefined
    loadReports(true)
  }, 300)
}

// 加载更多
const loadMore = () => {
  if (!loading.value && hasMore.value) {
    loadReports()
  }
}

// 选择分类
const selectCategory = (id: string) => {
  queryParams.value.categoryId = id || undefined
  showCategoryPicker.value = false
  loadReports(true)
}

// 选择图表类型
const selectChartType = (chartType: ChartType | undefined) => {
  queryParams.value.chartType = chartType
  showChartTypePicker.value = false
  loadReports(true)
}

// 查看报表
const handleView = (item: Report) => {
  uni.navigateTo({ url: `/pages/report/detail/index?id=${item.id}` })
}

onMounted(() => {
  loadReports(true)
  loadCategories()
})
</script>

<style lang="scss" scoped>
.report-list-page {
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

.report-scroll {
  flex: 1;
  overflow: hidden;
}

.report-list {
  padding: 20rpx;
}

.report-card {
  background: #fff;
  border-radius: $u-radius-lg;
  padding: 24rpx;
  margin-bottom: 20rpx;

  .report-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 16rpx;
  }

  .report-name {
    font-size: 30rpx;
    font-weight: 600;
    color: $u-main-color;
    flex: 1;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .chart-type-tag {
    font-size: 20rpx;
    padding: 6rpx 16rpx;
    border-radius: 4rpx;
    flex-shrink: 0;

    text {
      color: #fff;
    }
  }

  .tag-line {
    background: $u-primary;  // 蓝色
  }

  .tag-bar {
    background: $u-success;  // 绿色
  }

  .tag-pie {
    background: $u-warning;  // 橙色
  }

  .tag-table {
    background: $u-content-color;  // 灰色
  }

  .tag-mixed {
    background: #9333ea;  // 紫色
  }

  .report-info {
    display: flex;
    flex-direction: column;
    gap: 8rpx;
  }

  .report-category {
    font-size: 24rpx;
    color: $u-content-color;
  }

  .report-desc {
    font-size: 24rpx;
    color: $u-tips-color;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  .report-footer {
    margin-top: 16rpx;
    padding-top: 16rpx;
    border-top: 1rpx solid $u-border-color;
  }

  .report-time {
    font-size: 22rpx;
    color: $u-tips-color;
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