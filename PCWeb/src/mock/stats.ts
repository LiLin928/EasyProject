// 商品统计报表 Mock 数据

import type { MockMethod } from 'vite-plugin-mock'

/**
 * 格式化当前时间
 */
function formatNow(): string {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  const hours = String(now.getHours()).padStart(2, '0')
  const minutes = String(now.getMinutes()).padStart(2, '0')
  const seconds = String(now.getSeconds()).padStart(2, '0')
  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`
}

/**
 * 生成日期范围内的日期数组
 */
function generateDateRange(startDate: Date, days: number): string[] {
  const dates: string[] = []
  for (let i = 0; i < days; i++) {
    const date = new Date(startDate)
    date.setDate(date.getDate() + i)
    dates.push(date.toISOString().split('T')[0])
  }
  return dates
}

/**
 * 生成随机数
 */
function randomBetween(min: number, max: number): number {
  return Math.floor(Math.random() * (max - min + 1)) + min
}

export default [
  // 获取销量排行
  {
    url: '/api/product/stats/sales-ranking',
    method: 'get',
    response: ({ query }: { query: { startDate?: string; endDate?: string; categoryId?: string; top?: number | string } }) => {
      const { categoryId } = query
      const top = Number(query.top) || 10

      // 模拟销量数据
      const salesData = [
        { productId: '11111111-1111-1111-1111-111111111111', productName: '红玫瑰 99朵', productImage: '/static/images/products/rose-red.jpg', skuCode: 'SKU-ROSE-001', salesCount: 1256, salesAmount: 125600, orderCount: 980, avgPrice: 100 },
        { productId: '22222222-2222-2222-2222-222222222222', productName: '粉色康乃馨', productImage: '/static/images/products/carnation-pink.jpg', skuCode: 'SKU-CARN-001', salesCount: 892, salesAmount: 53520, orderCount: 756, avgPrice: 60 },
        { productId: '33333333-3333-3333-3333-333333333333', productName: '白百合花束', productImage: '/static/images/products/lily-white.jpg', skuCode: 'SKU-LILY-001', salesCount: 654, salesAmount: 45780, orderCount: 542, avgPrice: 70 },
        { productId: '44444444-4444-4444-4444-444444444444', productName: '向日葵花束', productImage: '/static/images/products/sunflower.jpg', skuCode: 'SKU-SUN-001', salesCount: 543, salesAmount: 32580, orderCount: 456, avgPrice: 60 },
        { productId: '55555555-5555-5555-5555-555555555555', productName: '蓝色妖姬', productImage: '/static/images/products/blue-rose.jpg', skuCode: 'SKU-BLUE-001', salesCount: 432, salesAmount: 51840, orderCount: 380, avgPrice: 120 },
        { productId: '66666666-6666-6666-6666-666666666666', productName: '满天星花束', productImage: '/static/images/products/babys-breath.jpg', skuCode: 'SKU-STAR-001', salesCount: 389, salesAmount: 19450, orderCount: 345, avgPrice: 50 },
        { productId: '77777777-7777-7777-7777-777777777777', productName: '郁金香花束', productImage: '/static/images/products/tulip.jpg', skuCode: 'SKU-TULIP-001', salesCount: 356, salesAmount: 28480, orderCount: 312, avgPrice: 80 },
        { productId: '88888888-8888-8888-8888-888888888888', productName: '香槟玫瑰', productImage: '/static/images/products/rose-champagne.jpg', skuCode: 'SKU-CHAMP-001', salesCount: 298, salesAmount: 35760, orderCount: 265, avgPrice: 120 },
        { productId: '99999999-9999-9999-9999-999999999999', productName: '紫罗兰花束', productImage: '/static/images/products/violet.jpg', skuCode: 'SKU-VIOLET-001', salesCount: 267, salesAmount: 16020, orderCount: 234, avgPrice: 60 },
        { productId: 'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', productName: '混搭花束', productImage: '/static/images/products/mixed.jpg', skuCode: 'SKU-MIXED-001', salesCount: 234, salesAmount: 21060, orderCount: 198, avgPrice: 90 },
      ]

      // 按销量排序并取前N条
      const result = salesData
        .sort((a, b) => b.salesCount - a.salesCount)
        .slice(0, top)

      return {
        code: 200,
        message: '成功',
        data: result,
      }
    },
  },

  // 获取销量趋势
  {
    url: '/api/product/stats/sales-trend',
    method: 'get',
    response: ({ query }: { query: { startDate?: string; endDate?: string; productId?: string } }) => {
      const { startDate, endDate } = query

      // 默认最近30天
      const end = endDate ? new Date(endDate) : new Date()
      const start = startDate ? new Date(startDate) : new Date(end.getTime() - 30 * 24 * 60 * 60 * 1000)
      const days = Math.ceil((end.getTime() - start.getTime()) / (24 * 60 * 60 * 1000)) + 1

      const dates = generateDateRange(start, Math.min(days, 30))
      const trendData = dates.map(date => ({
        date,
        salesCount: randomBetween(50, 200),
        salesAmount: randomBetween(3000, 15000),
        orderCount: randomBetween(20, 80),
      }))

      return {
        code: 200,
        message: '成功',
        data: trendData,
      }
    },
  },

  // 获取库存统计
  {
    url: '/api/product/stats/stock',
    method: 'get',
    response: () => {
      return {
        code: 200,
        message: '成功',
        data: {
          totalProducts: 156,
          totalStock: 12580,
          lowStockCount: 23,
          outOfStockCount: 5,
          totalValue: 856920,
        },
      }
    },
  },

  // 获取分类销售统计
  {
    url: '/api/product/stats/category-sales',
    method: 'get',
    response: () => {
      return {
        code: 200,
        message: '成功',
        data: [
          { categoryId: 'cat001', categoryName: '玫瑰', salesCount: 1890, salesAmount: 189000, percentage: 35.2 },
          { categoryId: 'cat002', categoryName: '康乃馨', salesCount: 1245, salesAmount: 74700, percentage: 23.2 },
          { categoryId: 'cat003', categoryName: '百合', salesCount: 876, salesAmount: 61320, percentage: 16.3 },
          { categoryId: 'cat004', categoryName: '向日葵', salesCount: 654, salesAmount: 39240, percentage: 12.2 },
          { categoryId: 'cat005', categoryName: '其他', salesCount: 712, salesAmount: 56960, percentage: 13.1 },
        ],
      }
    },
  },

  // 获取库存预警列表
  {
    url: '/api/product/stats/stock-alert',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10

      // 模拟低库存商品
      const lowStockProducts = [
        { productId: '11111111-1111-1111-1111-111111111111', productName: '红玫瑰 99朵', skuCode: 'SKU-ROSE-001', currentStock: 5, alertThreshold: 20, status: 'low' },
        { productId: '22222222-2222-2222-2222-222222222222', productName: '粉色康乃馨', skuCode: 'SKU-CARN-001', currentStock: 8, alertThreshold: 15, status: 'low' },
        { productId: '33333333-3333-3333-3333-333333333333', productName: '白百合花束', skuCode: 'SKU-LILY-001', currentStock: 0, alertThreshold: 10, status: 'out' },
        { productId: '44444444-4444-4444-4444-444444444444', productName: '向日葵花束', skuCode: 'SKU-SUN-001', currentStock: 3, alertThreshold: 15, status: 'low' },
        { productId: '55555555-5555-5555-5555-555555555555', productName: '蓝色妖姬', skuCode: 'SKU-BLUE-001', currentStock: 0, alertThreshold: 10, status: 'out' },
        { productId: '66666666-6666-6666-6666-666666666666', productName: '满天星花束', skuCode: 'SKU-STAR-001', currentStock: 12, alertThreshold: 20, status: 'low' },
      ]

      const total = lowStockProducts.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = lowStockProducts.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 获取概览数据
  {
    url: '/api/product/stats/overview',
    method: 'get',
    response: () => {
      return {
        code: 200,
        message: '成功',
        data: {
          // 今日数据
          today: {
            salesCount: 156,
            salesAmount: 15680,
            orderCount: 89,
            newProducts: 3,
          },
          // 本月数据
          month: {
            salesCount: 4523,
            salesAmount: 452300,
            orderCount: 2890,
            newProducts: 45,
          },
          // 同比增长
          growth: {
            salesCountGrowth: 12.5,
            salesAmountGrowth: 15.3,
            orderCountGrowth: 8.7,
          },
          // 评价相关
          review: {
            total: 1256,
            averageRating: 4.6,
            pendingCount: 12,
          },
          // 库存相关
          stock: {
            totalProducts: 156,
            lowStockCount: 23,
            outOfStockCount: 5,
          },
        },
      }
    },
  },
] as MockMethod[]