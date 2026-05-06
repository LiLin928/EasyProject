// src/mock/order.ts

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * 订单状态
 */
type OrderStatus = 'pending' | 'paid' | 'shipped' | 'completed' | 'cancelled'

/**
 * 售后类型
 */
type RefundType = 'refund_only' | 'return_refund' | 'exchange'

/**
 * 退款状态（扩展）
 */
type RefundStatus = 'pending' | 'approved' | 'returning' | 'received' | 'shipped' | 'refunded' | 'completed' | 'rejected'

/**
 * 换货商品项
 */
interface MockExchangeItem {
  productId: string
  productName: string
  productImage: string
  price: number
  quantity: number
}

/**
 * Mock 订单商品项
 */
interface MockOrderItem {
  id: string
  orderId: string
  productId: string
  productName: string
  productImage: string
  price: number
  quantity: number
  amount: number
}

/**
 * Mock 订单
 */
interface MockOrder {
  id: string
  orderNo: string
  userId: string
  userName: string
  userPhone: string
  receiverName: string
  receiverPhone: string
  receiverAddress: string
  items: MockOrderItem[]
  totalAmount: number
  payAmount: number
  status: OrderStatus
  payTime?: string
  payMethod?: string
  shipTime?: string
  shipCompany?: string
  shipNo?: string
  createTime: string
  updateTime: string
  remark?: string
}

/**
 * Mock 退款商品项
 */
interface MockRefundItem {
  id: string
  refundId: string
  orderItemId: string
  productId: string
  productName: string
  productImage: string
  price: number
  quantity: number
  amount: number
}

/**
 * Mock 退款
 */
interface MockRefund {
  id: string
  refundNo: string
  orderId: string
  orderNo: string
  // 售后类型
  type: RefundType
  items: MockRefundItem[]
  // 换货商品（仅换货类型）
  exchangeItems?: MockExchangeItem[]
  refundAmount: number
  reason: string
  description?: string
  status: RefundStatus
  // 用户退货快递信息
  returnShipCompany?: string
  returnShipNo?: string
  returnShipTime?: string
  // 商家换货发货快递信息
  exchangeShipCompany?: string
  exchangeShipNo?: string
  exchangeShipTime?: string
  // 审核信息
  approver?: string
  approveTime?: string
  approveRemark?: string
  // 审核信息
  approver?: string
  approveTime?: string
  approveRemark?: string
  // 完成时间
  refundTime?: string
  completeTime?: string
  createTime: string
  updateTime: string
}

/**
 * 评价状态
 */
type ReviewStatus = 'normal' | 'hidden' | 'deleted'

/**
 * Mock 评价
 */
interface MockReview {
  id: string
  orderId: string
  orderNo: string
  productId: string
  productName: string
  productImage: string
  userId: string
  userName: string
  userAvatar: string
  // 商品维度评分
  productQuality: number
  descriptionMatch: number
  costPerformance: number
  // 服务维度评分
  shippingSpeed: number
  logisticsService: number
  customerService: number
  // 综合评分
  overallRating: number
  // 评价内容
  content: string
  images: string[]
  videos: string[]
  tags: string[]
  isAnonymous: boolean
  // 积分奖励
  pointsEarned: number
  pointsReason: string
  // 状态
  status: ReviewStatus
  // 时间戳
  createTime: string
  updateTime: string
}

/**
 * 积分类型
 */
type PointsType = 'review' | 'order' | 'exchange' | 'refund' | 'system'

/**
 * Mock 积分记录
 */
interface MockPointsRecord {
  id: string
  userId: string
  points: number
  balance: number
  type: PointsType
  sourceId: string
  reason: string
  createTime: string
}

/**
 * 创建评价参数（Mock用）
 */
interface CreateReviewParamsBody {
  orderId: string
  productId: string
  productQuality: number
  descriptionMatch: number
  costPerformance: number
  shippingSpeed: number
  logisticsService: number
  customerService: number
  content: string
  images?: string[]
  videos?: string[]
  tags?: string[]
  isAnonymous?: boolean
}

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
 * 生成订单编号
 */
function generateOrderNo(): string {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  const random = String(Math.floor(Math.random() * 10000)).padStart(4, '0')
  return `${year}${month}${day}${random}`
}

/**
 * 生成退款编号
 */
function generateRefundNo(): string {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  const random = String(Math.floor(Math.random() * 10000)).padStart(4, '0')
  return `RF${year}${month}${day}${random}`
}

// 初始订单数据
const orders: MockOrder[] = [
  {
    id: generateGuid(),
    orderNo: '202604080001',
    userId: '2',
    userName: '张三',
    userPhone: '13800138001',
    receiverName: '张三',
    receiverPhone: '13800138001',
    receiverAddress: '北京市朝阳区望京街道XX小区1号楼101室',
    items: [
      {
        id: generateGuid(),
        orderId: '',
        productId: '550e8400-e29b-41d4-a716-446655440001',
        productName: '红玫瑰鲜花束',
        productImage: '/static/images/products/rose-red.jpg',
        price: 199.00,
        quantity: 1,
        amount: 199.00,
      },
      {
        id: generateGuid(),
        orderId: '',
        productId: '550e8400-e29b-41d4-a716-446655440002',
        productName: '粉色康乃馨花束',
        productImage: '/static/images/products/carnation-pink.jpg',
        price: 128.00,
        quantity: 2,
        amount: 256.00,
      },
    ],
    totalAmount: 455.00,
    payAmount: 455.00,
    status: 'completed',
    payTime: '2026-04-08 09:30:00',
    payMethod: 'wechat',
    shipTime: '2026-04-08 10:00:00',
    shipCompany: 'sf',
    shipNo: 'SF1234567890',
    createTime: '2026-04-08 09:25:00',
    updateTime: '2026-04-08 14:00:00',
    remark: '请尽快发货',
  },
  {
    id: generateGuid(),
    orderNo: '202604080002',
    userId: '3',
    userName: '李四',
    userPhone: '13800138002',
    receiverName: '王五',
    receiverPhone: '13800138003',
    receiverAddress: '上海市浦东新区陆家嘴金融中心A座2001室',
    items: [
      {
        id: generateGuid(),
        orderId: '',
        productId: '550e8400-e29b-41d4-a716-446655440003',
        productName: '白色百合花束',
        productImage: '/static/images/products/lily-white.jpg',
        price: 158.00,
        quantity: 1,
        amount: 158.00,
      },
    ],
    totalAmount: 158.00,
    payAmount: 158.00,
    status: 'shipped',
    payTime: '2026-04-08 10:15:00',
    payMethod: 'alipay',
    shipTime: '2026-04-08 11:00:00',
    shipCompany: 'yt',
    shipNo: 'YT9876543210',
    createTime: '2026-04-08 10:10:00',
    updateTime: '2026-04-08 11:00:00',
  },
  {
    id: generateGuid(),
    orderNo: '202604080003',
    userId: '5',
    userName: '赵六',
    userPhone: '13800138004',
    receiverName: '赵六',
    receiverPhone: '13800138004',
    receiverAddress: '广州市天河区珠江新城华夏路XX号',
    items: [
      {
        id: generateGuid(),
        orderId: '',
        productId: '550e8400-e29b-41d4-a716-446655440004',
        productName: '向日葵花束',
        productImage: '/static/images/products/sunflower.jpg',
        price: 98.00,
        quantity: 3,
        amount: 294.00,
      },
    ],
    totalAmount: 294.00,
    payAmount: 294.00,
    status: 'paid',
    payTime: '2026-04-08 11:30:00',
    payMethod: 'wechat',
    createTime: '2026-04-08 11:25:00',
    updateTime: '2026-04-08 11:30:00',
  },
  {
    id: generateGuid(),
    orderNo: '202604080004',
    userId: '2',
    userName: '张三',
    userPhone: '13800138001',
    receiverName: '李四',
    receiverPhone: '13800138002',
    receiverAddress: '深圳市南山区科技园南区XX大厦',
    items: [
      {
        id: generateGuid(),
        orderId: '',
        productId: '550e8400-e29b-41d4-a716-446655440005',
        productName: '混合鲜花束',
        productImage: '/static/images/products/mixed.jpg',
        price: 268.00,
        quantity: 1,
        amount: 268.00,
      },
      {
        id: generateGuid(),
        orderId: '',
        productId: '550e8400-e29b-41d4-a716-446655440006',
        productName: '蓝色妖姬花束',
        productImage: '/static/images/products/blue-rose.jpg',
        price: 388.00,
        quantity: 1,
        amount: 388.00,
      },
    ],
    totalAmount: 656.00,
    payAmount: 656.00,
    status: 'pending',
    createTime: '2026-04-08 12:00:00',
    updateTime: '2026-04-08 12:00:00',
    remark: '生日礼物，包装精美一点',
  },
  {
    id: generateGuid(),
    orderNo: '202604070001',
    userId: '3',
    userName: '李四',
    userPhone: '13800138002',
    receiverName: '张三',
    receiverPhone: '13800138001',
    receiverAddress: '北京市朝阳区望京街道XX小区1号楼101室',
    items: [
      {
        id: generateGuid(),
        orderId: '',
        productId: '550e8400-e29b-41d4-a716-446655440001',
        productName: '红玫瑰鲜花束',
        productImage: '/static/images/products/rose-red.jpg',
        price: 199.00,
        quantity: 2,
        amount: 398.00,
      },
    ],
    totalAmount: 398.00,
    payAmount: 398.00,
    status: 'cancelled',
    createTime: '2026-04-07 15:00:00',
    updateTime: '2026-04-07 16:00:00',
    remark: '取消订单',
  },
]

// 更新订单ID关联
orders.forEach(order => {
  order.items.forEach(item => {
    item.orderId = order.id
  })
})

// 初始退款数据
const refunds: MockRefund[] = [
  {
    id: generateGuid(),
    refundNo: 'RF202604080001',
    orderId: orders[0].id,
    orderNo: orders[0].orderNo,
    type: 'refund_only',
    items: [
      {
        id: generateGuid(),
        refundId: '',
        orderItemId: orders[0].items[1].id,
        productId: orders[0].items[1].productId,
        productName: orders[0].items[1].productName,
        productImage: orders[0].items[1].productImage,
        price: orders[0].items[1].price,
        quantity: 1,
        amount: 128.00,
      },
    ],
    refundAmount: 128.00,
    reason: 'quality',
    description: '收到时花瓣已经枯萎了',
    status: 'refunded',
    approver: 'admin',
    approveTime: '2026-04-08 15:00:00',
    approveRemark: '同意退款',
    refundTime: '2026-04-08 15:30:00',
    createTime: '2026-04-08 14:00:00',
    updateTime: '2026-04-08 15:30:00',
  },
  {
    id: generateGuid(),
    refundNo: 'RF202604080002',
    orderId: orders[2].id,
    orderNo: orders[2].orderNo,
    type: 'refund_only',
    items: orders[2].items.map(item => ({
      id: generateGuid(),
      refundId: '',
      orderItemId: item.id,
      productId: item.productId,
      productName: item.productName,
      productImage: item.productImage,
      price: item.price,
      quantity: item.quantity,
      amount: item.amount,
    })),
    refundAmount: 294.00,
    reason: 'other',
    description: '不需要了',
    status: 'pending',
    createTime: '2026-04-08 16:00:00',
    updateTime: '2026-04-08 16:00:00',
  },
  {
    id: generateGuid(),
    refundNo: 'RF202604070001',
    orderId: orders[4].id,
    orderNo: orders[4].orderNo,
    type: 'refund_only',
    items: orders[4].items.map(item => ({
      id: generateGuid(),
      refundId: '',
      orderItemId: item.id,
      productId: item.productId,
      productName: item.productName,
      productImage: item.productImage,
      price: item.price,
      quantity: item.quantity,
      amount: item.amount,
    })),
    refundAmount: 398.00,
    reason: 'wrong',
    description: '发错货了',
    status: 'rejected',
    approver: 'admin',
    approveTime: '2026-04-07 17:00:00',
    approveRemark: '请提供发错货的照片证据',
    createTime: '2026-04-07 16:30:00',
    updateTime: '2026-04-07 17:00:00',
  },
  // 退货退款示例 - 已审核等待用户退货
  {
    id: generateGuid(),
    refundNo: 'RF202604080003',
    orderId: orders[1].id,
    orderNo: orders[1].orderNo,
    type: 'return_refund',
    items: [
      {
        id: generateGuid(),
        refundId: '',
        orderItemId: orders[1].items[0].id,
        productId: orders[1].items[0].productId,
        productName: orders[1].items[0].productName,
        productImage: orders[1].items[0].productImage,
        price: orders[1].items[0].price,
        quantity: 1,
        amount: 158.00,
      },
    ],
    refundAmount: 158.00,
    reason: 'damage',
    description: '花束包装破损严重',
    status: 'approved',
    approver: 'admin',
    approveTime: '2026-04-08 13:00:00',
    approveRemark: '请退回商品后退款',
    createTime: '2026-04-08 12:00:00',
    updateTime: '2026-04-08 13:00:00',
  },
  // 退货退款示例 - 用户已发货
  {
    id: generateGuid(),
    refundNo: 'RF202604070002',
    orderId: orders[0].id,
    orderNo: orders[0].orderNo,
    type: 'return_refund',
    items: [
      {
        id: generateGuid(),
        refundId: '',
        orderItemId: orders[0].items[0].id,
        productId: orders[0].items[0].productId,
        productName: orders[0].items[0].productName,
        productImage: orders[0].items[0].productImage,
        price: orders[0].items[0].price,
        quantity: 1,
        amount: 199.00,
      },
    ],
    refundAmount: 199.00,
    reason: 'notMatch',
    description: '花朵数量与描述不符',
    status: 'returning',
    returnShipCompany: 'sf',
    returnShipNo: 'SF1234567891',
    returnShipTime: '2026-04-07 14:00:00',
    approver: 'admin',
    approveTime: '2026-04-07 12:00:00',
    approveRemark: '同意退货退款',
    createTime: '2026-04-07 10:00:00',
    updateTime: '2026-04-07 14:00:00',
  },
  // 换货示例 - 已审核等待用户退货
  {
    id: generateGuid(),
    refundNo: 'RF202604080004',
    orderId: orders[2].id,
    orderNo: orders[2].orderNo,
    type: 'exchange',
    items: [
      {
        id: generateGuid(),
        refundId: '',
        orderItemId: orders[2].items[0].id,
        productId: orders[2].items[0].productId,
        productName: orders[2].items[0].productName,
        productImage: orders[2].items[0].productImage,
        price: orders[2].items[0].price,
        quantity: 1,
        amount: 98.00,
      },
    ],
    exchangeItems: [
      {
        productId: '550e8400-e29b-41d4-a716-446655440001',
        productName: '红玫瑰鲜花束',
        productImage: '/static/images/products/rose-red.jpg',
        price: 199.00,
        quantity: 1,
      },
    ],
    refundAmount: 0,
    reason: 'wrong',
    description: '颜色发错了，需要换成红色',
    status: 'approved',
    approver: 'admin',
    approveTime: '2026-04-08 14:00:00',
    approveRemark: '同意换货，请退回旧商品',
    createTime: '2026-04-08 13:30:00',
    updateTime: '2026-04-08 14:00:00',
  },
  // 换货示例 - 商家已发货完成
  {
    id: generateGuid(),
    refundNo: 'RF202604060001',
    orderId: orders[0].id,
    orderNo: orders[0].orderNo,
    type: 'exchange',
    items: [
      {
        id: generateGuid(),
        refundId: '',
        orderItemId: orders[0].items[0].id,
        productId: orders[0].items[0].productId,
        productName: orders[0].items[0].productName,
        productImage: orders[0].items[0].productImage,
        price: orders[0].items[0].price,
        quantity: 1,
        amount: 199.00,
      },
    ],
    exchangeItems: [
      {
        productId: '550e8400-e29b-41d4-a716-446655440003',
        productName: '白色百合花束',
        productImage: '/static/images/products/lily-white.jpg',
        price: 158.00,
        quantity: 1,
      },
    ],
    refundAmount: 0,
    reason: 'other',
    description: '想换成百合花束',
    status: 'completed',
    returnShipCompany: 'yt',
    returnShipNo: 'YT1111111111',
    returnShipTime: '2026-04-06 12:00:00',
    exchangeShipCompany: 'sf',
    exchangeShipNo: 'SF2222222222',
    exchangeShipTime: '2026-04-06 16:00:00',
    approver: 'admin',
    approveTime: '2026-04-06 10:00:00',
    approveRemark: '同意换货',
    completeTime: '2026-04-06 18:00:00',
    createTime: '2026-04-06 09:00:00',
    updateTime: '2026-04-06 18:00:00',
  },
]

// 更新退款ID关联
refunds.forEach(refund => {
  refund.items.forEach(item => {
    item.refundId = refund.id
  })
})

// 初始评价数据
const reviews: MockReview[] = [
  {
    id: generateGuid(),
    orderId: orders[0].id,
    orderNo: orders[0].orderNo,
    productId: orders[0].items[0].productId,
    productName: orders[0].items[0].productName,
    productImage: orders[0].items[0].productImage,
    userId: orders[0].userId,
    userName: orders[0].userName,
    userAvatar: '',
    productQuality: 5,
    descriptionMatch: 5,
    costPerformance: 4,
    shippingSpeed: 5,
    logisticsService: 5,
    customerService: 5,
    overallRating: 4.8,
    content: '鲜花质量很好，包装精美，物流也很快，非常满意！',
    images: [],
    videos: [],
    tags: ['quality_good', 'logistics_fast', 'package_good'],
    isAnonymous: false,
    pointsEarned: 15,
    pointsReason: '评价奖励',
    status: 'normal',
    createTime: '2026-04-08 16:00:00',
    updateTime: '2026-04-08 16:00:00',
  },
]

// 初始积分记录数据
const pointsRecords: MockPointsRecord[] = [
  {
    id: generateGuid(),
    userId: orders[0].userId,
    points: 5,
    balance: 5,
    type: 'order',
    sourceId: orders[0].id,
    reason: `订单 ${orders[0].orderNo} 完成奖励`,
    createTime: '2026-04-08 14:00:00',
  },
  {
    id: generateGuid(),
    userId: orders[0].userId,
    points: 15,
    balance: 20,
    type: 'review',
    sourceId: reviews[0].id,
    reason: `订单 ${orders[0].orderNo} 评价奖励`,
    createTime: '2026-04-08 16:00:00',
  },
]

export default [
  // ==================== 订单 API ====================

  // 获取订单列表
  {
    url: '/api/order/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; orderNo?: string; userKeyword?: string; status?: OrderStatus; startTime?: string; endTime?: string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { orderNo, userKeyword, status, startTime, endTime } = query

      let filteredOrders = orders.filter((order) => {
        let match = true
        if (orderNo && !order.orderNo.includes(orderNo)) {
          match = false
        }
        if (userKeyword) {
          const keyword = userKeyword.toLowerCase()
          const matchUser = order.userName.toLowerCase().includes(keyword) ||
            order.userPhone.includes(keyword)
          if (!matchUser) {
            match = false
          }
        }
        if (status && order.status !== status) {
          match = false
        }
        if (startTime && new Date(order.createTime) < new Date(startTime)) {
          match = false
        }
        if (endTime && new Date(order.createTime) > new Date(endTime + ' 23:59:59')) {
          match = false
        }
        return match
      })

      // 按创建时间倒序
      filteredOrders.sort((a, b) =>
        new Date(b.createTime).getTime() - new Date(a.createTime).getTime()
      )

      const total = filteredOrders.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredOrders.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 获取订单详情
  {
    url: '/api/order/detail/:id',
    method: 'get',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query
      const order = orders.find((o) => o.id === id)

      if (!order) {
        return { code: 404, message: '订单不存在', data: null }
      }

      // 计算售后关联字段
      const orderRefunds = refunds.filter(r => r.orderId === id && r.status !== 'rejected')
      const refundAmount = orderRefunds.reduce((sum, r) => sum + r.refundAmount, 0)
      const refundCount = orderRefunds.length
      const hasRefund = refundCount > 0

      // 计算评价关联字段
      const orderReview = reviews.find(r => r.orderId === id && r.status === 'normal')
      const hasReview = !!orderReview
      const reviewId = orderReview?.id
      const reviewTime = orderReview?.createTime

      // 计算积分（订单金额的 1%）
      const pointsEarned = order.status === 'completed' ? Math.floor(order.payAmount * 0.01) : 0
      const pointsStatus = order.status === 'completed' ? 'earned' :
        (order.status === 'cancelled' ? 'deducted' : 'pending')

      return {
        code: 200,
        message: '成功',
        data: {
          ...order,
          refundAmount,
          refundCount,
          hasRefund,
          hasReview,
          reviewId,
          reviewTime,
          pointsEarned,
          pointsStatus,
        },
      }
    },
  },

  // 发货
  {
    url: '/api/order/ship',
    method: 'post',
    response: ({ body }: { body: { orderId: string; logisticsCompany: string; logisticsNumber: string } }) => {
      const { orderId, logisticsCompany, logisticsNumber } = body

      const order = orders.find((o) => o.id === orderId)

      if (!order) {
        return { code: 404, message: '订单不存在', data: null }
      }

      if (order.status !== 'paid') {
        return { code: 400, message: '订单状态不允许发货', data: null }
      }

      order.status = 'shipped'
      order.shipCompany = logisticsCompany
      order.shipNo = logisticsNumber
      order.shipTime = formatNow()
      // 计算自动确认天数（取商品类型最短的）
      order.autoConfirmDays = 3  // Mock 默认3天
      order.autoConfirmTime = new Date(Date.now() + 3 * 24 * 60 * 60 * 1000).toISOString().replace('T', ' ').substring(0, 19)
      order.updateTime = formatNow()

      return { code: 200, message: '发货成功', data: { success: true } }
    },
  },

  // 取消订单
  {
    url: '/api/order/cancel/:id',
    method: 'put',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query

      const order = orders.find((o) => o.id === id)

      if (!order) {
        return { code: 404, message: '订单不存在', data: null }
      }

      if (order.status !== 'pending' && order.status !== 'paid') {
        return { code: 400, message: '订单状态不允许取消', data: null }
      }

      order.status = 'cancelled'
      order.updateTime = formatNow()

      return { code: 200, message: '取消成功', data: { success: true } }
    },
  },

  // 导出订单
  {
    url: '/api/order/export',
    method: 'post',
    response: () => {
      return { code: 200, message: '导出成功', data: { success: true } }
    },
  },

  // 创建订单
  {
    url: '/api/order/create',
    method: 'post',
    response: ({ body }: { body: { userId: string; userName: string; userPhone: string; receiverName: string; receiverPhone: string; receiverAddress: string; items: { productId: string; productName: string; productImage: string; price: number; quantity: number }[]; remark?: string } }) => {
      const { userId, userName, userPhone, receiverName, receiverPhone, receiverAddress, items, remark } = body

      if (!userId || !userName) {
        return { code: 400, message: '请选择用户', data: null }
      }

      if (!receiverName || !receiverPhone || !receiverAddress) {
        return { code: 400, message: '请填写完整的收货信息', data: null }
      }

      if (!items || items.length === 0) {
        return { code: 400, message: '请添加商品', data: null }
      }

      // 计算总金额
      const totalAmount = items.reduce((sum, item) => sum + item.price * item.quantity, 0)

      const now = formatNow()
      const orderId = generateGuid()
      const orderNo = generateOrderNo()

      // 创建订单商品项
      const orderItems = items.map((item) => ({
        id: generateGuid(),
        orderId,
        productId: item.productId,
        productName: item.productName,
        productImage: item.productImage,
        price: item.price,
        quantity: item.quantity,
        amount: item.price * item.quantity,
      }))

      const newOrder: MockOrder = {
        id: orderId,
        orderNo,
        userId,
        userName,
        userPhone,
        receiverName,
        receiverPhone,
        receiverAddress,
        items: orderItems,
        totalAmount,
        payAmount: totalAmount,
        status: 'pending',
        createTime: now,
        updateTime: now,
        remark,
      }

      orders.unshift(newOrder)

      return { code: 200, message: '创建成功', data: { id: orderId } }
    },
  },

  // 更新订单（PUT方法）
  {
    url: '/api/order/update',
    method: 'put',
    response: ({ body }: { body: { id: string; receiverName?: string; receiverPhone?: string; receiverAddress?: string; remark?: string } }) => {
      const { id, receiverName, receiverPhone, receiverAddress, remark } = body

      const order = orders.find((o) => o.id === id)

      if (!order) {
        return { code: 404, message: '订单不存在', data: null }
      }

      if (order.status !== 'pending') {
        return { code: 400, message: '只有待支付的订单可以修改', data: null }
      }

      if (receiverName !== undefined) order.receiverName = receiverName
      if (receiverPhone !== undefined) order.receiverPhone = receiverPhone
      if (receiverAddress !== undefined) order.receiverAddress = receiverAddress
      if (remark !== undefined) order.remark = remark
      order.updateTime = formatNow()

      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },

  // ==================== 确认收货 API ====================

  // 确认收货（手动）
  {
    url: '/api/order/confirm/:id',
    method: 'put',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query

      const order = orders.find((o) => o.id === id)

      if (!order) {
        return { code: 404, message: '订单不存在', data: null }
      }

      if (order.status !== 'shipped') {
        return { code: 400, message: '订单状态不允许确认收货', data: null }
      }

      order.status = 'completed'
      order.confirmTime = formatNow()
      order.updateTime = formatNow()

      // 计算积分（订单金额的 1%）
      const pointsEarned = Math.floor(order.payAmount * 0.01)

      // 添加积分记录
      const pointsRecord: MockPointsRecord = {
        id: generateGuid(),
        userId: order.userId,
        points: pointsEarned,
        balance: pointsEarned,  // Mock 简化处理
        type: 'order',
        sourceId: order.id,
        reason: `订单 ${order.orderNo} 完成奖励`,
        createTime: formatNow(),
      }
      pointsRecords.unshift(pointsRecord)

      return { code: 200, message: '确认收货成功', data: { success: true, pointsEarned } }
    },
  },

  // ==================== 物流轨迹 API ====================

  // 获取物流轨迹
  {
    url: '/api/order/track/:id',
    method: 'get',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query

      const order = orders.find((o) => o.id === id)

      if (!order) {
        return { code: 404, message: '订单不存在', data: null }
      }

      if (!order.shipCompany || !order.shipNo) {
        return { code: 400, message: '订单未发货', data: null }
      }

      // Mock 物流轨迹数据
      const now = formatNow()
      const tracks = [
        {
          time: now,
          status: '已签收',
          location: order.receiverAddress?.substring(0, 20) || '北京市朝阳区',
          description: '您的包裹已签收，签收人：本人，如有疑问请联系派送员：张师傅（13800138000）',
        },
        {
          time: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString().replace('T', ' ').substring(0, 19),
          status: '派送中',
          location: order.receiverAddress?.substring(0, 20) || '北京市朝阳区',
          description: '您的包裹正在派送中，派送员：张师傅（13800138000）',
        },
        {
          time: new Date(Date.now() - 6 * 60 * 60 * 1000).toISOString().replace('T', ' ').substring(0, 19),
          status: '到达目的地',
          location: order.receiverAddress?.substring(0, 20) || '北京市朝阳区',
          description: '您的包裹已到达北京市朝阳区站点',
        },
        {
          time: new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString().replace('T', ' ').substring(0, 19),
          status: '运输中',
          location: '北京市',
          description: '您的包裹已到达北京市分拨中心',
        },
        {
          time: order.shipTime || new Date(Date.now() - 48 * 60 * 60 * 1000).toISOString().replace('T', ' ').substring(0, 19),
          status: '已揽收',
          location: '上海市浦东新区',
          description: '您的包裹已被揽收，揽收员：李师傅（13900139000）',
        },
      ]

      return {
        code: 200,
        message: '成功',
        data: {
          company: order.shipCompany,
          shipNo: order.shipNo,
          tracks,
          isSigned: order.status === 'completed',
        },
      }
    },
  },

  // ==================== 评价 API ====================

  // 创建评价
  {
    url: '/api/order/review/create',
    method: 'post',
    response: ({ body }: { body: CreateReviewParamsBody }) => {
      const { orderId, productId, productQuality, descriptionMatch, costPerformance, shippingSpeed, logisticsService, customerService, content, images, videos, tags, isAnonymous } = body

      const order = orders.find((o) => o.id === orderId)

      if (!order) {
        return { code: 404, message: '订单不存在', data: null }
      }

      if (order.status !== 'completed') {
        return { code: 400, message: '只有已完成的订单可以评价', data: null }
      }

      // 检查是否已评价
      const existingReview = reviews.find((r) => r.orderId === orderId && r.productId === productId && r.status === 'normal')
      if (existingReview) {
        return { code: 400, message: '该商品已评价', data: null }
      }

      // 计算综合评分
      const overallRating = (productQuality + descriptionMatch + costPerformance + shippingSpeed + logisticsService + customerService) / 6

      // 计算积分
      let pointsEarned = 10  // 纯文字基础积分
      if (images && images.length > 0) {
        pointsEarned = 20  // 含图片
        if (images.length >= 3) {
          pointsEarned += 5  // 晒图奖励
        }
      }
      if (videos && videos.length > 0) {
        pointsEarned = 30  // 含视频
      }
      // 首次评价奖励
      const userReviews = reviews.filter((r) => r.userId === order.userId && r.status === 'normal')
      if (userReviews.length === 0) {
        pointsEarned += 5
      }
      // 积分上限
      pointsEarned = Math.min(pointsEarned, 50)

      const now = formatNow()
      const reviewId = generateGuid()

      const newReview: MockReview = {
        id: reviewId,
        orderId,
        orderNo: order.orderNo,
        productId,
        productName: order.items.find(i => i.productId === productId)?.productName || '',
        productImage: order.items.find(i => i.productId === productId)?.productImage || '',
        userId: order.userId,
        userName: order.userName,
        userAvatar: '',
        productQuality,
        descriptionMatch,
        costPerformance,
        shippingSpeed,
        logisticsService,
        customerService,
        overallRating: Math.round(overallRating * 10) / 10,
        content,
        images: images || [],
        videos: videos || [],
        tags: tags || [],
        isAnonymous: isAnonymous || false,
        pointsEarned,
        pointsReason: '评价奖励',
        status: 'normal',
        createTime: now,
        updateTime: now,
      }

      reviews.unshift(newReview)

      // 添加积分记录
      const pointsRecord: MockPointsRecord = {
        id: generateGuid(),
        userId: order.userId,
        points: pointsEarned,
        balance: pointsEarned,
        type: 'review',
        sourceId: reviewId,
        reason: `订单 ${order.orderNo} 评价奖励`,
        createTime: now,
      }
      pointsRecords.unshift(pointsRecord)

      return { code: 200, message: '评价成功', data: { id: reviewId, pointsEarned } }
    },
  },

  // 获取评价详情（路径参数）
  {
    url: /\/api\/order\/review\/detail\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const review = reviews.find((r) => r.id === id)

      if (!review) {
        return { code: 404, message: '评价不存在', data: null }
      }

      return { code: 200, message: '成功', data: review }
    },
  },

  // 获取评价列表
  {
    url: '/api/order/review/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; orderNo?: string; productId?: string; userId?: string; status?: string; minRating?: number | string; maxRating?: number | string; startTime?: string; endTime?: string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { orderNo, productId, userId, status, minRating, maxRating, startTime, endTime } = query

      let filteredReviews = reviews.filter((review) => {
        let match = true
        if (orderNo && !review.orderNo.includes(orderNo)) {
          match = false
        }
        if (productId && review.productId !== productId) {
          match = false
        }
        if (userId && review.userId !== userId) {
          match = false
        }
        if (status && review.status !== status) {
          match = false
        }
        if (minRating && review.overallRating < Number(minRating)) {
          match = false
        }
        if (maxRating && review.overallRating > Number(maxRating)) {
          match = false
        }
        if (startTime && new Date(review.createTime) < new Date(startTime)) {
          match = false
        }
        if (endTime && new Date(review.createTime) > new Date(endTime + ' 23:59:59')) {
          match = false
        }
        return match
      })

      // 按创建时间倒序
      filteredReviews.sort((a, b) =>
        new Date(b.createTime).getTime() - new Date(a.createTime).getTime()
      )

      const total = filteredReviews.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredReviews.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 检查订单是否可评价
  {
    url: '/api/order/review/check',
    method: 'get',
    response: ({ query }: { query: { orderId: string } }) => {
      const { orderId } = query

      const order = orders.find((o) => o.id === orderId)

      if (!order) {
        return { code: 404, message: '订单不存在', data: null }
      }

      if (order.status !== 'completed') {
        return {
          code: 200,
          message: '成功',
          data: { canReview: false, reason: '只有已完成的订单可以评价' },
        }
      }

      // 检查是否已全部评价
      const reviewedProductIds = reviews
        .filter(r => r.orderId === orderId && r.status === 'normal')
        .map(r => r.productId)

      const allReviewed = order.items.every(item => reviewedProductIds.includes(item.productId))

      if (allReviewed) {
        return {
          code: 200,
          message: '成功',
          data: { canReview: false, reason: '该订单已全部评价' },
        }
      }

      return {
        code: 200,
        message: '成功',
        data: { canReview: true },
      }
    },
  },

  // 获取待评价订单
  {
    url: '/api/order/review/pendingList',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; userId?: string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { userId } = query

      // 状态为 completed 且有未评价商品的订单
      let pendingOrders = orders.filter((order) => {
        if (order.status !== 'completed') return false
        if (userId && order.userId !== userId) return false

        // 检查是否有未评价商品
        const reviewedProductIds = reviews
          .filter(r => r.orderId === order.id && r.status === 'normal')
          .map(r => r.productId)

        return !order.items.every(item => reviewedProductIds.includes(item.productId))
      })

      const total = pendingOrders.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = pendingOrders.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // ==================== 积分 API ====================

  // 获取用户积分信息
  {
    url: '/api/points/info',
    method: 'get',
    response: ({ query }: { query: { userId: string } }) => {
      const { userId } = query

      // 计算用户积分
      const userPointsRecords = pointsRecords.filter(r => r.userId === userId)
      const totalPoints = userPointsRecords.reduce((sum, r) => sum + (r.points > 0 ? r.points : 0), 0)
      const usedPoints = userPointsRecords.reduce((sum, r) => sum + (r.points < 0 ? Math.abs(r.points) : 0), 0)
      const availablePoints = totalPoints - usedPoints

      return {
        code: 200,
        message: '成功',
        data: {
          id: generateGuid(),
          userId,
          totalPoints,
          availablePoints,
          usedPoints,
          createTime: formatNow(),
          updateTime: formatNow(),
        },
      }
    },
  },

  // 获取用户积分记录
  {
    url: '/api/points/records',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; userId?: string; type?: string; startTime?: string; endTime?: string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { userId, type, startTime, endTime } = query

      let filteredRecords = pointsRecords.filter((record) => {
        let match = true
        if (userId && record.userId !== userId) {
          match = false
        }
        if (type && record.type !== type) {
          match = false
        }
        if (startTime && new Date(record.createTime) < new Date(startTime)) {
          match = false
        }
        if (endTime && new Date(record.createTime) > new Date(endTime + ' 23:59:59')) {
          match = false
        }
        return match
      })

      // 按创建时间倒序
      filteredRecords.sort((a, b) =>
        new Date(b.createTime).getTime() - new Date(a.createTime).getTime()
      )

      const total = filteredRecords.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredRecords.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // ==================== 退款 API ====================

  // 获取退款列表
  {
    url: '/api/refund/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; refundNo?: string; orderNo?: string; orderId?: string; type?: RefundType; status?: RefundStatus; startTime?: string; endTime?: string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { refundNo, orderNo, orderId, type, status, startTime, endTime } = query

      let filteredRefunds = refunds.filter((refund) => {
        let match = true
        if (refundNo && !refund.refundNo.includes(refundNo)) {
          match = false
        }
        if (orderNo && !refund.orderNo.includes(orderNo)) {
          match = false
        }
        if (orderId && refund.orderId !== orderId) {
          match = false
        }
        if (type && refund.type !== type) {
          match = false
        }
        if (status && refund.status !== status) {
          match = false
        }
        if (startTime && new Date(refund.createTime) < new Date(startTime)) {
          match = false
        }
        if (endTime && new Date(refund.createTime) > new Date(endTime + ' 23:59:59')) {
          match = false
        }
        return match
      })

      // 按创建时间倒序
      filteredRefunds.sort((a, b) =>
        new Date(b.createTime).getTime() - new Date(a.createTime).getTime()
      )

      const total = filteredRefunds.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredRefunds.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 获取退款详情
  {
    url: '/api/refund/detail/:id',
    method: 'get',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query
      const refund = refunds.find((r) => r.id === id)

      if (!refund) {
        return { code: 404, message: '退款记录不存在', data: null }
      }

      return {
        code: 200,
        message: '成功',
        data: refund,
      }
    },
  },

  // 审核退款（通过）
  {
    url: '/api/refund/approve',
    method: 'put',
    response: ({ body }: { body: { id: string; remark?: string } }) => {
      const { id, remark } = body

      const refund = refunds.find((r) => r.id === id)

      if (!refund) {
        return { code: 404, message: '售后记录不存在', data: null }
      }

      if (refund.status !== 'pending') {
        return { code: 400, message: '售后状态不允许审核', data: null }
      }

      refund.approver = 'admin'
      refund.approveTime = formatNow()
      refund.approveRemark = remark || ''
      refund.updateTime = formatNow()

      // 根据售后类型设置状态
      if (refund.type === 'refund_only') {
        // 仅退款：直接退款完成
        refund.status = 'refunded'
        refund.refundTime = formatNow()
      } else {
        // 退货退款/换货：等待用户退货
        refund.status = 'approved'
      }

      return { code: 200, message: '审核通过', data: { success: true } }
    },
  },

  // 审核退款（拒绝）
  {
    url: '/api/refund/reject',
    method: 'put',
    response: ({ body }: { body: { id: string; remark: string } }) => {
      const { id, remark } = body

      const refund = refunds.find((r) => r.id === id)

      if (!refund) {
        return { code: 404, message: '退款记录不存在', data: null }
      }

      if (refund.status !== 'pending') {
        return { code: 400, message: '退款状态不允许审核', data: null }
      }

      if (!remark) {
        return { code: 400, message: '拒绝原因不能为空', data: null }
      }

      refund.status = 'rejected'
      refund.approver = 'admin'
      refund.approveTime = formatNow()
      refund.approveRemark = remark
      refund.updateTime = formatNow()

      return { code: 200, message: '已拒绝退款', data: { success: true } }
    },
  },

  // 创建退款申请
  {
    url: '/api/refund/create',
    method: 'post',
    response: ({ body }: { body: { orderId: string; type: RefundType; items: { orderItemId: string; productId: string; productName: string; productImage: string; price: number; quantity: number }[]; reason: string; description?: string; exchangeItems?: MockExchangeItem[] } }) => {
      const { orderId, type, items, reason, description, exchangeItems } = body

      const order = orders.find((o) => o.id === orderId)

      if (!order) {
        return { code: 404, message: '订单不存在', data: null }
      }

      if (!items || items.length === 0) {
        return { code: 400, message: '请选择售后商品', data: null }
      }

      if (!reason) {
        return { code: 400, message: '请选择售后原因', data: null }
      }

      if (!type) {
        return { code: 400, message: '请选择售后类型', data: null }
      }

      // 换货类型必须选择换货商品
      if (type === 'exchange' && (!exchangeItems || exchangeItems.length === 0)) {
        return { code: 400, message: '换货类型需选择换货商品', data: null }
      }

      // 计算退款金额（换货类型为 0）
      const refundAmount = type === 'exchange' ? 0 : items.reduce((sum, item) => sum + item.price * item.quantity, 0)

      const now = formatNow()
      const refundId = generateGuid()
      const refundNo = generateRefundNo()

      // 创建退款商品项
      const refundItems = items.map((item) => ({
        id: generateGuid(),
        refundId,
        orderItemId: item.orderItemId,
        productId: item.productId,
        productName: item.productName,
        productImage: item.productImage,
        price: item.price,
        quantity: item.quantity,
        amount: item.price * item.quantity,
      }))

      const newRefund: MockRefund = {
        id: refundId,
        refundNo,
        orderId,
        orderNo: order.orderNo,
        type,
        items: refundItems,
        exchangeItems: type === 'exchange' ? exchangeItems : undefined,
        refundAmount,
        reason,
        description,
        status: 'pending',
        createTime: now,
        updateTime: now,
      }

      refunds.unshift(newRefund)

      return { code: 200, message: '售后申请已提交', data: { id: refundId } }
    },
  },

  // 用户填写退货快递信息
  {
    url: '/api/refund/fillReturnShip',
    method: 'post',
    response: ({ body }: { body: { id: string; shipCompany: string; shipNo: string } }) => {
      const { id, shipCompany, shipNo } = body

      const refund = refunds.find((r) => r.id === id)

      if (!refund) {
        return { code: 404, message: '售后记录不存在', data: null }
      }

      if (refund.status !== 'approved') {
        return { code: 400, message: '当前状态不允许填写退货快递', data: null }
      }

      if (refund.type === 'refund_only') {
        return { code: 400, message: '仅退款类型无需填写退货快递', data: null }
      }

      refund.status = 'returning'
      refund.returnShipCompany = shipCompany
      refund.returnShipNo = shipNo
      refund.returnShipTime = formatNow()
      refund.updateTime = formatNow()

      return { code: 200, message: '退货快递信息已填写', data: { success: true } }
    },
  },

  // 商家确认收货
  {
    url: '/api/refund/confirm-receive/:id',
    method: 'put',
    response: ({ query }: { query: { id: string; remark?: string } }) => {
      const { id, remark } = query

      const refund = refunds.find((r) => r.id === id)

      if (!refund) {
        return { code: 404, message: '售后记录不存在', data: null }
      }

      if (refund.status !== 'returning') {
        return { code: 400, message: '当前状态不允许确认收货', data: null }
      }

      refund.status = refund.type === 'exchange' ? 'exchanging' : 'refunding'
      refund.updateTime = formatNow()

      return { code: 200, message: '已确认收货', data: { success: true } }
    },
  },

  // 商家换货发货
  {
    url: '/api/refund/exchange-ship',
    method: 'post',
    response: ({ body }: { body: { id: string; shipCompany: string; shipNo: string } }) => {
      const { id, shipCompany, shipNo } = body

      const refund = refunds.find((r) => r.id === id)

      if (!refund) {
        return { code: 404, message: '售后记录不存在', data: null }
      }

      if (refund.status !== 'exchanging') {
        return { code: 400, message: '当前状态不允许换货发货', data: null }
      }

      if (refund.type !== 'exchange') {
        return { code: 400, message: '仅换货类型可以换货发货', data: null }
      }

      refund.status = 'completing'
      refund.exchangeShipCompany = shipCompany
      refund.exchangeShipNo = shipNo
      refund.exchangeShipTime = formatNow()
      refund.updateTime = formatNow()

      return { code: 200, message: '换货发货成功', data: { success: true } }
    },
  },

  // 完成售后
  {
    url: '/api/refund/complete/:id',
    method: 'put',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query

      const refund = refunds.find((r) => r.id === id)

      if (!refund) {
        return { code: 404, message: '售后记录不存在', data: null }
      }

      if (refund.status !== 'refunding' && refund.status !== 'completing') {
        return { code: 400, message: '售后状态不允许完成', data: null }
      }

      refund.status = 'completed'
      refund.completeTime = formatNow()
      refund.updateTime = formatNow()

      return { code: 200, message: '售后已完成', data: { success: true } }
    },
  },

  // 获取物流轨迹
  {
    url: '/api/refund/shipDetail',
    method: 'get',
    response: ({ query }: { query: { company: string; shipNo: string } }) => {
      const { company, shipNo } = query

      if (!company || !shipNo) {
        return { code: 400, message: '请提供快递公司和快递单号', data: null }
      }

      // 模拟物流轨迹数据
      const tracks = [
        {
          time: formatNow(),
          status: '已签收',
          location: '北京市朝阳区',
          description: '您的包裹已签收，签收人：本人，如有疑问请联系派送员：张师傅（13800138000）',
        },
        {
          time: formatNow(),
          status: '派送中',
          location: '北京市朝阳区',
          description: '您的包裹正在派送中，派送员：张师傅（13800138000）',
        },
        {
          time: formatNow(),
          status: '到达目的地',
          location: '北京市朝阳区',
          description: '您的包裹已到达北京市朝阳区站点',
        },
        {
          time: formatNow(),
          status: '运输中',
          location: '北京市',
          description: '您的包裹已到达北京市分拨中心',
        },
        {
          time: formatNow(),
          status: '已揽收',
          location: '上海市浦东新区',
          description: '您的包裹已被揽收，揽收员：李师傅（13900139000）',
        },
      ]

      return {
        code: 200,
        message: '成功',
        data: { tracks },
      }
    },
  },
] as MockMethod[]