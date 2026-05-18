<!-- pages/report/view/index.vue -->
<template>
  <view class="report-view-page">
    <Loading :show="loading" />

    <!-- 报表基本信息 -->
    <view v-if="reportDetail" class="report-header">
      <view class="header-content">
        <text class="report-name">{{ reportDetail.name }}</text>
        <text v-if="reportDetail.description" class="report-desc">{{ reportDetail.description }}</text>
        <view class="report-meta">
          <text class="meta-item">分类: {{ reportDetail.categoryName || '未分类' }}</text>
          <text class="meta-item">数据源: {{ reportDetail.dataSource || '默认' }}</text>
        </view>
      </view>
      <view class="refresh-btn" @click="handleRefresh">
        <text class="refresh-icon">🔄</text>
        <text class="refresh-text">刷新</text>
      </view>
    </view>

    <!-- 时间筛选栏 -->
    <view class="time-filter-bar">
      <view class="quick-options">
        <view
          v-for="opt in quickTimeOptions"
          :key="opt.value"
          :class="['quick-option', { active: selectedTimeType === opt.value }]"
          @click="selectQuickTime(opt.value)"
        >
          <text>{{ opt.label }}</text>
        </view>
      </view>
      <view class="custom-range" @click="showDatePicker = true">
        <text class="range-label">{{ dateRangeText }}</text>
        <text class="range-arrow">▼</text>
      </view>
    </view>

    <!-- 图表展示区域 -->
    <view v-if="reportData" class="chart-section">
      <view class="section-title">
        <text>数据图表</text>
      </view>
      <view class="chart-container">
        <!-- 使用 canvas 简化图表展示 -->
        <canvas
          canvas-id="reportChart"
          id="reportChart"
          class="chart-canvas"
          @click="handleChartClick"
        />
        <!-- 简化图表提示 -->
        <view v-if="chartDataReady" class="chart-tip">
          <text>图表数据已加载</text>
        </view>
      </view>
    </view>

    <!-- 数据表格区域 -->
    <view v-if="reportData" class="table-section">
      <view class="section-title">
        <text>数据明细</text>
        <view class="sort-btn" @click="toggleSort">
          <text>{{ sortAsc ? '↑ 升序' : '↓ 降序' }}</text>
        </view>
      </view>
      <scroll-view class="table-scroll" scroll-x>
        <view class="table-container">
          <!-- 表头 -->
          <view class="table-header">
            <view class="table-cell header-cell">
              <text>维度</text>
            </view>
            <view class="table-cell header-cell">
              <text>数值</text>
            </view>
          </view>
          <!-- 表体 -->
          <view class="table-body">
            <view v-for="(item, index) in sortedTableData" :key="index" class="table-row">
              <view class="table-cell">
                <text>{{ item.dimension }}</text>
              </view>
              <view class="table-cell">
                <text class="value-text">{{ formatValue(item.value) }}</text>
              </view>
            </view>
          </view>
        </view>
      </scroll-view>
    </view>

    <!-- 空状态 -->
    <view v-if="!loading && !reportData" class="empty-state">
      <text class="empty-icon">📊</text>
      <text class="empty-text">暂无报表数据</text>
    </view>

    <!-- 日期选择器 -->
    <view v-if="showDatePicker" class="picker-mask" @click="showDatePicker = false">
      <view class="picker-content" @click.stop>
        <view class="picker-header">
          <text class="picker-title">选择时间范围</text>
          <view class="picker-close" @click="showDatePicker = false">
            <text>×</text>
          </view>
        </view>
        <view class="picker-body">
          <view class="date-input-group">
            <text class="date-label">开始时间</text>
            <picker mode="date" :value="customStartDate" @change="onStartDateChange">
              <view class="date-picker-btn">
                <text>{{ customStartDate || '请选择' }}</text>
              </view>
            </picker>
          </view>
          <view class="date-input-group">
            <text class="date-label">结束时间</text>
            <picker mode="date" :value="customEndDate" @change="onEndDateChange">
              <view class="date-picker-btn">
                <text>{{ customEndDate || '请选择' }}</text>
              </view>
            </picker>
          </view>
        </view>
        <view class="picker-footer">
          <view class="picker-btn cancel" @click="showDatePicker = false">
            <text>取消</text>
          </view>
          <view class="picker-btn confirm" @click="applyCustomDate">
            <text>确定</text>
          </view>
        </view>
      </view>
    </view>

    <!-- 底部提示 -->
    <view class="bottom-tip">
      <text>移动端仅支持查看，设计请前往PC端</text>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { getReportDetail, getReportData } from '@/api/report/reportApi'
import type { Report, ReportData, ChartType, TimeType } from '@/types/report'
import Loading from '@/components/Loading/index.vue'

// 快捷时间选项
const quickTimeOptions = [
  { label: '今日', value: 1 },
  { label: '昨日', value: 2 },
  { label: '本周', value: 3 },
  { label: '本月', value: 4 },
  { label: '本季', value: 5 },
  { label: '本年', value: 6 },
]

// 报表ID
const reportId = ref('')

// 加载状态
const loading = ref(false)

// 报表详情
const reportDetail = ref<Report | null>(null)

// 报表数据
const reportData = ref<ReportData | null>(null)

// 图表数据是否准备好
const chartDataReady = ref(false)

// 选中的时间类型
const selectedTimeType = ref<TimeType>(4) // 默认本月

// 日期选择器显示
const showDatePicker = ref(false)

// 自定义开始日期
const customStartDate = ref('')

// 自定义结束日期
const customEndDate = ref('')

// 排序方向
const sortAsc = ref(true)

// 日期范围文本
const dateRangeText = computed(() => {
  if (selectedTimeType.value === 7) {
    if (customStartDate.value && customEndDate.value) {
      return `${customStartDate.value} ~ ${customEndDate.value}`
    }
    return '自定义'
  }
  const opt = quickTimeOptions.find(o => o.value === selectedTimeType.value)
  return opt?.label || '本月'
})

// 排序后的表格数据
const sortedTableData = computed(() => {
  if (!reportData.value) return []

  const data = reportData.value.dimensions.map((dim, index) => ({
    dimension: dim,
    value: reportData.value!.values[index] || 0,
  }))

  return data.sort((a, b) => {
    if (sortAsc.value) {
      return a.value - b.value
    }
    return b.value - a.value
  })
})

// 格式化数值
const formatValue = (value: number): string => {
  if (value >= 10000) {
    return `${(value / 10000).toFixed(2)}万`
  }
  return value.toFixed(2)
}

// 加载报表详情和数据
const loadReport = async () => {
  if (!reportId.value) return

  loading.value = true
  chartDataReady.value = false

  try {
    // 加载报表详情
    reportDetail.value = await getReportDetail(reportId.value)

    // 加载报表数据
    const params = {
      reportId: reportId.value,
      timeType: selectedTimeType.value,
      startTime: selectedTimeType.value === 7 ? customStartDate.value : undefined,
      endTime: selectedTimeType.value === 7 ? customEndDate.value : undefined,
    }

    reportData.value = await getReportData(params)

    // 绘制图表
    drawChart()
  } catch (error) {
    console.error('加载报表失败:', error)
    uni.showToast({ title: '加载失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}

// 绘制图表（简化版）
const drawChart = () => {
  if (!reportData.value || !reportDetail.value) return

  const ctx = uni.createCanvasContext('reportChart')
  const chartType = reportDetail.value.chartType

  // 清空画布
  ctx.clearRect(0, 0, 700, 400)

  // 设置背景
  ctx.setFillStyle('#ffffff')
  ctx.fillRect(0, 0, 700, 400)

  // 根据图表类型绘制
  switch (chartType) {
    case 1: // 折线图
      drawLineChart(ctx)
      break
    case 2: // 柱状图
      drawBarChart(ctx)
      break
    case 3: // 饼图
      drawPieChart(ctx)
      break
    default: // 默认柱状图
      drawBarChart(ctx)
  }

  ctx.draw()
  chartDataReady.value = true
}

// 绘制折线图
const drawLineChart = (ctx: any) => {
  const data = reportData.value!
  const maxVal = Math.max(...data.values) || 100
  const stepX = 700 / (data.dimensions.length + 1)

  ctx.setStrokeStyle('#2563eb')
  ctx.setLineWidth(3)

  // 绘制线条
  data.values.forEach((val, index) => {
    const x = stepX * (index + 1)
    const y = 400 - (val / maxVal) * 350 - 20

    if (index === 0) {
      ctx.moveTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }

    // 绘制点
    ctx.beginPath()
    ctx.arc(x, y, 5, 0, 2 * Math.PI)
    ctx.setFillStyle('#2563eb')
    ctx.fill()
    ctx.closePath()
  })

  ctx.stroke()
}

// 绘制柱状图
const drawBarChart = (ctx: any) => {
  const data = reportData.value!
  const maxVal = Math.max(...data.values) || 100
  const barWidth = 700 / data.dimensions.length - 20

  data.values.forEach((val, index) => {
    const x = (700 / data.dimensions.length) * index + 10
    const height = (val / maxVal) * 350
    const y = 400 - height - 20

    // 绘制柱子
    ctx.setFillStyle('#2563eb')
    ctx.fillRect(x, y, barWidth, height)

    // 绘制数值标签
    ctx.setFillStyle('#333333')
    ctx.setFontSize(12)
    ctx.fillText(formatValue(val), x + barWidth / 2 - 20, y - 10)
  })
}

// 绘制饼图
const drawPieChart = (ctx: any) => {
  const data = reportData.value!
  const total = data.values.reduce((sum, val) => sum + val, 0)
  const centerX = 350
  const centerY = 200
  const radius = 150

  const colors = ['#2563eb', '#10b981', '#f59e0b', '#ef4444', '#8b5cf6', '#ec4899']

  let startAngle = 0

  data.values.forEach((val, index) => {
    const angle = (val / total) * 2 * Math.PI

    ctx.beginPath()
    ctx.moveTo(centerX, centerY)
    ctx.arc(centerX, centerY, radius, startAngle, startAngle + angle)
    ctx.closePath()

    ctx.setFillStyle(colors[index % colors.length])
    ctx.fill()

    startAngle += angle
  })
}

// 选择快捷时间
const selectQuickTime = (type: TimeType) => {
  selectedTimeType.value = type
  loadReport()
}

// 开始日期变更
const onStartDateChange = (e: any) => {
  customStartDate.value = e.detail.value
}

// 结束日期变更
const onEndDateChange = (e: any) => {
  customEndDate.value = e.detail.value
}

// 应用自定义日期
const applyCustomDate = () => {
  if (!customStartDate.value || !customEndDate.value) {
    uni.showToast({ title: '请选择完整时间范围', icon: 'none' })
    return
  }

  if (customStartDate.value > customEndDate.value) {
    uni.showToast({ title: '开始时间不能大于结束时间', icon: 'none' })
    return
  }

  selectedTimeType.value = 7 // Custom
  showDatePicker.value = false
  loadReport()
}

// 切换排序
const toggleSort = () => {
  sortAsc.value = !sortAsc.value
}

// 刷新数据
const handleRefresh = () => {
  loadReport()
}

// 图表点击事件
const handleChartClick = () => {
  uni.showToast({ title: '图表详情请前往PC端查看', icon: 'none' })
}

onMounted(() => {
  // 从路由参数获取报表ID
  const pages = getCurrentPages()
  const currentPage = pages[pages.length - 1] as any
  reportId.value = currentPage.options?.id || ''

  if (reportId.value) {
    loadReport()
  } else {
    uni.showToast({ title: '缺少报表ID参数', icon: 'none' })
  }
})
</script>

<style lang="scss" scoped>
.report-view-page {
  min-height: 100vh;
  background: $u-bg-color;
  padding-bottom: 80rpx;
}

.report-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  padding: 20rpx;
  background: #fff;
  margin-bottom: 20rpx;

  .header-content {
    flex: 1;

    .report-name {
      font-size: 32rpx;
      font-weight: 600;
      color: $u-main-color;
      display: block;
    }

    .report-desc {
      font-size: 24rpx;
      color: $u-tips-color;
      margin-top: 8rpx;
      display: block;
    }

    .report-meta {
      display: flex;
      gap: 20rpx;
      margin-top: 12rpx;

      .meta-item {
        font-size: 22rpx;
        color: $u-content-color;
      }
    }
  }

  .refresh-btn {
    display: flex;
    align-items: center;
    padding: 12rpx 20rpx;
    background: $u-light-color;
    border-radius: $u-radius;
    gap: 8rpx;

    .refresh-icon {
      font-size: 24rpx;
    }

    .refresh-text {
      font-size: 24rpx;
      color: $u-content-color;
    }
  }
}

.time-filter-bar {
  display: flex;
  flex-direction: column;
  padding: 16rpx 20rpx;
  background: #fff;
  margin-bottom: 20rpx;

  .quick-options {
    display: flex;
    gap: 12rpx;
    flex-wrap: wrap;

    .quick-option {
      padding: 12rpx 20rpx;
      background: $u-light-color;
      border-radius: $u-radius;
      font-size: 24rpx;
      color: $u-content-color;

      &.active {
        background: $u-primary;
        color: #fff;
      }
    }
  }

  .custom-range {
    display: flex;
    align-items: center;
    justify-content: center;
    margin-top: 16rpx;
    padding: 12rpx 20rpx;
    background: $u-light-color;
    border-radius: $u-radius;

    .range-label {
      font-size: 24rpx;
      color: $u-main-color;
    }

    .range-arrow {
      font-size: 20rpx;
      color: $u-content-color;
      margin-left: 8rpx;
    }
  }
}

.chart-section {
  margin: 0 20rpx 20rpx;
  padding: 20rpx;
  background: #fff;
  border-radius: $u-radius;

  .section-title {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 16rpx;

    text {
      font-size: 28rpx;
      font-weight: 600;
      color: $u-main-color;
    }
  }

  .chart-container {
    position: relative;
    width: 100%;
    height: 400rpx;

    .chart-canvas {
      width: 100%;
      height: 400rpx;
    }

    .chart-tip {
      position: absolute;
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
      background: rgba($u-primary, 0.1);
      padding: 12rpx 24rpx;
      border-radius: $u-radius;

      text {
        font-size: 24rpx;
        color: $u-primary;
      }
    }
  }
}

.table-section {
  margin: 0 20rpx 20rpx;
  padding: 20rpx;
  background: #fff;
  border-radius: $u-radius;

  .section-title {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 16rpx;

    text {
      font-size: 28rpx;
      font-weight: 600;
      color: $u-main-color;
    }

    .sort-btn {
      padding: 8rpx 16rpx;
      background: $u-light-color;
      border-radius: $u-radius;

      text {
        font-size: 22rpx;
        color: $u-content-color;
      }
    }
  }

  .table-scroll {
    width: 100%;
  }

  .table-container {
    min-width: 100%;
  }

  .table-header {
    display: flex;
    background: $u-light-color;
  }

  .table-body {
    .table-row {
      display: flex;
      border-bottom: 1rpx solid $u-border-color;
    }
  }

  .table-cell {
    flex: 1;
    padding: 16rpx 20rpx;
    min-width: 200rpx;

    text {
      font-size: 26rpx;
      color: $u-main-color;
    }

    &.header-cell {
      text {
        font-weight: 600;
        color: $u-content-color;
      }
    }

    .value-text {
      color: $u-primary;
      font-weight: 600;
    }
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

.bottom-tip {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  text-align: center;
  padding: 20rpx 30rpx;
  background: $u-light-color;

  text {
    font-size: 24rpx;
    color: $u-content-color;
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

.picker-body {
  padding: 30rpx;

  .date-input-group {
    margin-bottom: 20rpx;

    .date-label {
      font-size: 28rpx;
      color: $u-main-color;
      display: block;
      margin-bottom: 12rpx;
    }

    .date-picker-btn {
      padding: 16rpx 20rpx;
      background: $u-light-color;
      border-radius: $u-radius;

      text {
        font-size: 26rpx;
        color: $u-content-color;
      }
    }
  }
}

.picker-footer {
  display: flex;
  gap: 20rpx;
  padding: 30rpx;
  border-top: 1rpx solid $u-border-color;

  .picker-btn {
    flex: 1;
    text-align: center;
    padding: 16rpx 0;
    border-radius: $u-radius;

    text {
      font-size: 28rpx;
    }

    &.cancel {
      background: $u-light-color;
      text {
        color: $u-content-color;
      }
    }

    &.confirm {
      background: $u-primary;
      text {
        color: #fff;
      }
    }
  }
}
</style>