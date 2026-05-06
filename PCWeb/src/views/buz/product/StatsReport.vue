<!-- src/views/buz/product/StatsReport.vue -->
<template>
  <div class="stats-report-container">
    <!-- 概览卡片 -->
    <el-row :gutter="16" class="overview-row">
      <el-col :span="6">
        <el-card shadow="hover" class="overview-card">
          <div class="card-content">
            <div class="card-icon primary">
              <el-icon><Goods /></el-icon>
            </div>
            <div class="card-info">
              <div class="card-value">{{ overview.today?.salesCount || 0 }}</div>
              <div class="card-label">今日销量</div>
            </div>
          </div>
          <div class="card-footer">
            较昨日 <span :class="overview.growth?.salesCountGrowth >= 0 ? 'up' : 'down'">
              {{ overview.growth?.salesCountGrowth >= 0 ? '+' : '' }}{{ overview.growth?.salesCountGrowth || 0 }}%
            </span>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="overview-card">
          <div class="card-content">
            <div class="card-icon success">
              <el-icon><Money /></el-icon>
            </div>
            <div class="card-info">
              <div class="card-value">¥{{ formatNumber(overview.today?.salesAmount || 0) }}</div>
              <div class="card-label">今日销售额</div>
            </div>
          </div>
          <div class="card-footer">
            较昨日 <span :class="overview.growth?.salesAmountGrowth >= 0 ? 'up' : 'down'">
              {{ overview.growth?.salesAmountGrowth >= 0 ? '+' : '' }}{{ overview.growth?.salesAmountGrowth || 0 }}%
            </span>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="overview-card">
          <div class="card-content">
            <div class="card-icon warning">
              <el-icon><ShoppingCart /></el-icon>
            </div>
            <div class="card-info">
              <div class="card-value">{{ overview.today?.orderCount || 0 }}</div>
              <div class="card-label">今日订单</div>
            </div>
          </div>
          <div class="card-footer">
            较昨日 <span :class="overview.growth?.orderCountGrowth >= 0 ? 'up' : 'down'">
              {{ overview.growth?.orderCountGrowth >= 0 ? '+' : '' }}{{ overview.growth?.orderCountGrowth || 0 }}%
            </span>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card shadow="hover" class="overview-card">
          <div class="card-content">
            <div class="card-icon danger">
              <el-icon><Warning /></el-icon>
            </div>
            <div class="card-info">
              <div class="card-value">{{ overview.stock?.lowStockCount || 0 }}</div>
              <div class="card-label">库存预警</div>
            </div>
          </div>
          <div class="card-footer">
            缺货 <span class="danger">{{ overview.stock?.outOfStockCount || 0 }}</span> 件
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 销量趋势图表 -->
    <el-card shadow="never" class="chart-card">
      <template #header>
        <div class="card-header">
          <span>销量趋势</span>
          <el-radio-group v-model="trendDays" size="small" @change="loadSalesTrend">
            <el-radio-button :value="7">近7天</el-radio-button>
            <el-radio-button :value="30">近30天</el-radio-button>
          </el-radio-group>
        </div>
      </template>
      <div ref="trendChartRef" class="chart-container"></div>
    </el-card>

    <el-row :gutter="16">
      <!-- 销量排行 -->
      <el-col :span="12">
        <el-card shadow="never" class="ranking-card">
          <template #header>
            <div class="card-header">
              <span>销量排行 TOP 10</span>
              <el-button link type="primary" @click="exportRanking">导出</el-button>
            </div>
          </template>
          <el-table :data="salesRanking" max-height="400">
            <el-table-column type="index" label="排名" width="60" />
            <el-table-column label="商品" min-width="180">
              <template #default="{ row }">
                <div class="product-cell">
                  <el-image
                    :src="row.productImage || '/static/images/products/default.jpg'"
                    fit="cover"
                    class="product-img"
                  >
                    <template #error>
                      <div class="img-placeholder"><el-icon><Picture /></el-icon></div>
                    </template>
                  </el-image>
                  <div class="product-info">
                    <div class="product-name">{{ row.productName }}</div>
                    <div class="product-sku">{{ row.skuCode }}</div>
                  </div>
                </div>
              </template>
            </el-table-column>
            <el-table-column prop="salesCount" label="销量" width="80" align="right" />
            <el-table-column label="销售额" width="100" align="right">
              <template #default="{ row }">
                ¥{{ formatNumber(row.salesAmount) }}
              </template>
            </el-table-column>
          </el-table>
        </el-card>
      </el-col>

      <!-- 分类销售占比 -->
      <el-col :span="12">
        <el-card shadow="never" class="pie-card">
          <template #header>
            <span>分类销售占比</span>
          </template>
          <div ref="categoryChartRef" class="chart-container"></div>
        </el-card>
      </el-col>
    </el-row>

    <el-row :gutter="16" style="margin-top: 16px">
      <!-- 库存统计 -->
      <el-col :span="12">
        <el-card shadow="never">
          <template #header>
            <span>库存概览</span>
          </template>
          <div class="stock-stats">
            <div class="stat-item">
              <div class="stat-label">商品总数</div>
              <div class="stat-value">{{ stockStats.totalProducts }}</div>
            </div>
            <div class="stat-item">
              <div class="stat-label">库存总量</div>
              <div class="stat-value">{{ stockStats.totalStock }}</div>
            </div>
            <div class="stat-item">
              <div class="stat-label">库存总价值</div>
              <div class="stat-value">¥{{ formatNumber(stockStats.totalValue) }}</div>
            </div>
            <div class="stat-item warning">
              <div class="stat-label">低库存商品</div>
              <div class="stat-value">{{ stockStats.lowStockCount }}</div>
            </div>
            <div class="stat-item danger">
              <div class="stat-label">缺货商品</div>
              <div class="stat-value">{{ stockStats.outOfStockCount }}</div>
            </div>
          </div>
        </el-card>
      </el-col>

      <!-- 评价统计 -->
      <el-col :span="12">
        <el-card shadow="never">
          <template #header>
            <div class="card-header">
              <span>评价概览</span>
              <el-tag v-if="reviewStats.pendingCount > 0" type="warning">
                {{ reviewStats.pendingCount }} 条待审核
              </el-tag>
            </div>
          </template>
          <div class="review-stats">
            <div class="rating-overview">
              <div class="avg-rating">{{ reviewStats.averageRating }}</div>
              <el-rate :model-value="reviewStats.averageRating" disabled />
              <div class="total-count">{{ reviewStats.total }} 条评价</div>
            </div>
            <div class="rating-distribution">
              <div v-for="item in reviewStats.ratingDistribution" :key="item.rating" class="rating-item">
                <span class="rating-label">{{ item.rating }}星</span>
                <el-progress
                  :percentage="getRatingPercentage(item.count)"
                  :show-text="false"
                  :stroke-width="8"
                />
                <span class="rating-count">{{ item.count }}</span>
              </div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { Goods, Money, ShoppingCart, Warning, Picture } from '@element-plus/icons-vue'
import * as echarts from 'echarts'
import {
  getOverviewStats,
  getSalesTrend,
  getSalesRanking,
  getCategorySalesStats,
  getStockStatistics,
  getReviewStatistics,
} from '@/api/buz/productApi'
import type { StockStatistics, ReviewStatistics } from '@/types/product'

// 概览数据
const overview = ref({
  today: { salesCount: 0, salesAmount: 0, orderCount: 0, newProducts: 0 },
  month: { salesCount: 0, salesAmount: 0, orderCount: 0, newProducts: 0 },
  growth: { salesCountGrowth: 0, salesAmountGrowth: 0, orderCountGrowth: 0 },
  review: { total: 0, averageRating: 0, pendingCount: 0 },
  stock: { totalProducts: 0, lowStockCount: 0, outOfStockCount: 0 },
})

// 销量趋势
const trendDays = ref(30)
const trendChartRef = ref<HTMLElement>()
let trendChart: echarts.ECharts | null = null

// 销量排行
const salesRanking = ref<any[]>([])

// 分类图表
const categoryChartRef = ref<HTMLElement>()
let categoryChart: echarts.ECharts | null = null

// 库存统计
const stockStats = ref<StockStatistics>({
  totalProducts: 0,
  totalStock: 0,
  lowStockCount: 0,
  outOfStockCount: 0,
  totalValue: 0,
})

// 评价统计
const reviewStats = ref<ReviewStatistics>({
  total: 0,
  averageRating: 0,
  ratingDistribution: [],
  pendingCount: 0,
  approvedCount: 0,
  rejectedCount: 0,
})

onMounted(async () => {
  await Promise.all([
    loadOverview(),
    loadSalesTrend(),
    loadSalesRanking(),
    loadCategorySales(),
    loadStockStats(),
    loadReviewStats(),
  ])

  // 监听窗口变化
  window.addEventListener('resize', handleResize)
})

onUnmounted(() => {
  window.removeEventListener('resize', handleResize)
  trendChart?.dispose()
  categoryChart?.dispose()
})

const handleResize = () => {
  trendChart?.resize()
  categoryChart?.resize()
}

const formatNumber = (num: number) => {
  if (num >= 10000) {
    return (num / 10000).toFixed(1) + '万'
  }
  return num.toLocaleString()
}

const loadOverview = async () => {
  try {
    const data = await getOverviewStats()
    overview.value = data
  } catch (error) {
    // Error handled
  }
}

const loadSalesTrend = async () => {
  try {
    const endDate = new Date()
    const startDate = new Date()
    startDate.setDate(startDate.getDate() - trendDays.value)

    const data = await getSalesTrend({
      startDate: startDate.toISOString().split('T')[0],
      endDate: endDate.toISOString().split('T')[0],
    })

    renderTrendChart(data)
  } catch (error) {
    // Error handled
  }
}

const renderTrendChart = (data: any[]) => {
  if (!trendChartRef.value) return

  if (!trendChart) {
    trendChart = echarts.init(trendChartRef.value)
  }

  const option = {
    tooltip: {
      trigger: 'axis',
      axisPointer: { type: 'cross' },
    },
    legend: {
      data: ['销量', '销售额'],
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true,
    },
    xAxis: {
      type: 'category',
      boundaryGap: false,
      data: data.map(d => d.date.slice(5)), // 只显示月-日
    },
    yAxis: [
      {
        type: 'value',
        name: '销量',
        position: 'left',
      },
      {
        type: 'value',
        name: '销售额',
        position: 'right',
        axisLabel: {
          formatter: (val: number) => (val >= 1000 ? val / 1000 + 'k' : val),
        },
      },
    ],
    series: [
      {
        name: '销量',
        type: 'line',
        smooth: true,
        data: data.map(d => d.salesCount),
        areaStyle: { opacity: 0.3 },
      },
      {
        name: '销售额',
        type: 'line',
        smooth: true,
        yAxisIndex: 1,
        data: data.map(d => d.salesAmount),
        areaStyle: { opacity: 0.3 },
      },
    ],
  }

  trendChart.setOption(option)
}

const loadSalesRanking = async () => {
  try {
    const data = await getSalesRanking({ top: 10 })
    salesRanking.value = data
  } catch (error) {
    // Error handled
  }
}

const loadCategorySales = async () => {
  try {
    const data = await getCategorySalesStats()
    renderCategoryChart(data)
  } catch (error) {
    // Error handled
  }
}

const renderCategoryChart = (data: any[]) => {
  if (!categoryChartRef.value) return

  if (!categoryChart) {
    categoryChart = echarts.init(categoryChartRef.value)
  }

  const option = {
    tooltip: {
      trigger: 'item',
      formatter: '{a} <br/>{b}: {c} ({d}%)',
    },
    legend: {
      orient: 'vertical',
      right: 10,
      top: 'center',
    },
    series: [
      {
        name: '销售占比',
        type: 'pie',
        radius: ['40%', '70%'],
        center: ['40%', '50%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 10,
          borderColor: '#fff',
          borderWidth: 2,
        },
        label: {
          show: false,
          position: 'center',
        },
        emphasis: {
          label: {
            show: true,
            fontSize: 16,
            fontWeight: 'bold',
          },
        },
        labelLine: {
          show: false,
        },
        data: data.map(d => ({
          value: d.salesAmount,
          name: d.categoryName,
        })),
      },
    ],
  }

  categoryChart.setOption(option)
}

const loadStockStats = async () => {
  try {
    const data = await getStockStatistics()
    stockStats.value = data
  } catch (error) {
    // Error handled
  }
}

const loadReviewStats = async () => {
  try {
    const data = await getReviewStatistics()
    reviewStats.value = data
  } catch (error) {
    // Error handled
  }
}

const getRatingPercentage = (count: number) => {
  if (reviewStats.value.total === 0) return 0
  return Math.round((count / reviewStats.value.total) * 100)
}

const exportRanking = () => {
  // 简单的CSV导出
  const headers = ['排名,商品名称,SKU码,销量,销售额\n']
  const rows = salesRanking.value.map((item, index) =>
    `${index + 1},${item.productName},${item.skuCode},${item.salesCount},${item.salesAmount}\n`
  )

  const csv = headers.concat(rows).join('')
  const blob = new Blob(['\ufeff' + csv], { type: 'text/csv;charset=utf-8' })
  const url = URL.createObjectURL(blob)
  const link = document.createElement('a')
  link.href = url
  link.download = `销量排行_${new Date().toISOString().split('T')[0]}.csv`
  link.click()
  URL.revokeObjectURL(url)
}
</script>

<style scoped lang="scss">
.stats-report-container {
  padding: 20px;

  .overview-row {
    margin-bottom: 16px;

    .overview-card {
      .card-content {
        display: flex;
        align-items: center;
        gap: 16px;

        .card-icon {
          width: 56px;
          height: 56px;
          border-radius: 12px;
          display: flex;
          align-items: center;
          justify-content: center;
          font-size: 24px;
          color: #fff;

          &.primary { background: linear-gradient(135deg, #409eff, #79bbff); }
          &.success { background: linear-gradient(135deg, #67c23a, #95d475); }
          &.warning { background: linear-gradient(135deg, #e6a23c, #f0c78a); }
          &.danger { background: linear-gradient(135deg, #f56c6c, #fab6b6); }
        }

        .card-info {
          .card-value {
            font-size: 24px;
            font-weight: 600;
            color: #303133;
          }

          .card-label {
            font-size: 14px;
            color: #909399;
            margin-top: 4px;
          }
        }
      }

      .card-footer {
        margin-top: 12px;
        padding-top: 12px;
        border-top: 1px solid #ebeef5;
        font-size: 12px;
        color: #909399;

        .up { color: #67c23a; }
        .down { color: #f56c6c; }
        .danger { color: #f56c6c; }
      }
    }
  }

  .chart-card {
    margin-bottom: 16px;

    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .chart-container {
      height: 300px;
    }
  }

  .ranking-card, .pie-card {
    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .product-cell {
      display: flex;
      align-items: center;
      gap: 10px;

      .product-img {
        width: 40px;
        height: 40px;
        border-radius: 4px;
      }

      .img-placeholder {
        width: 40px;
        height: 40px;
        display: flex;
        align-items: center;
        justify-content: center;
        background: #f5f7fa;
        color: #c0c4cc;
      }

      .product-info {
        .product-name {
          font-weight: 500;
        }

        .product-sku {
          font-size: 12px;
          color: #909399;
        }
      }
    }
  }

  .stock-stats {
    display: flex;
    flex-wrap: wrap;
    gap: 20px;

    .stat-item {
      flex: 1;
      min-width: 100px;
      text-align: center;
      padding: 16px;
      background: #f5f7fa;
      border-radius: 8px;

      .stat-label {
        font-size: 12px;
        color: #909399;
        margin-bottom: 8px;
      }

      .stat-value {
        font-size: 20px;
        font-weight: 600;
        color: #303133;
      }

      &.warning .stat-value { color: #e6a23c; }
      &.danger .stat-value { color: #f56c6c; }
    }
  }

  .review-stats {
    display: flex;
    gap: 40px;

    .rating-overview {
      text-align: center;
      padding: 20px;
      background: #f5f7fa;
      border-radius: 8px;

      .avg-rating {
        font-size: 48px;
        font-weight: 600;
        color: #e6a23c;
      }

      .total-count {
        margin-top: 8px;
        font-size: 12px;
        color: #909399;
      }
    }

    .rating-distribution {
      flex: 1;

      .rating-item {
        display: flex;
        align-items: center;
        gap: 12px;
        margin-bottom: 12px;

        .rating-label {
          width: 30px;
          font-size: 12px;
          color: #909399;
        }

        .el-progress {
          flex: 1;
        }

        .rating-count {
          width: 40px;
          font-size: 12px;
          color: #606266;
          text-align: right;
        }
      }
    }
  }
}
</style>