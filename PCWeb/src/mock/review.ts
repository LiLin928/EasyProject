// 商品评价 Mock 数据

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * Mock 评价类型
 */
interface MockReview {
  id: string
  productId: string
  productName: string
  productImage?: string
  orderId: string
  orderNo: string
  userId: string
  userName: string
  userAvatar?: string
  rating: number
  content: string
  images?: string[]
  reply?: string
  replyTime?: string
  status: 'pending' | 'approved' | 'rejected'
  isAnonymous: boolean
  createTime: string
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

// 评价示例数据
const reviews: MockReview[] = [
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440001',
    productName: '红玫瑰鲜花束',
    productImage: '/static/images/products/rose-red.jpg',
    orderId: generateGuid(),
    orderNo: 'ORD20240101001',
    userId: 'user001',
    userName: '小红',
    userAvatar: '/static/images/avatars/user1.jpg',
    rating: 5,
    content: '花很新鲜，包装精美，女朋友很喜欢！物流也很快，第二天就到了。客服态度也很好，有问必答。下次还会再来购买！',
    images: [
      '/static/images/reviews/review1-1.jpg',
      '/static/images/reviews/review1-2.jpg',
    ],
    status: 'approved',
    isAnonymous: false,
    createTime: '2024-01-15 14:30:00',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440001',
    productName: '红玫瑰鲜花束',
    productImage: '/static/images/products/rose-red.jpg',
    orderId: generateGuid(),
    orderNo: 'ORD20240102001',
    userId: 'user002',
    userName: '张先生',
    rating: 4,
    content: '整体还不错，就是配送时间稍微晚了点。花的质量很好，老婆很满意。',
    status: 'approved',
    isAnonymous: false,
    createTime: '2024-01-16 09:20:00',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440002',
    productName: '粉色康乃馨花束',
    productImage: '/static/images/products/carnation-pink.jpg',
    orderId: generateGuid(),
    orderNo: 'ORD20240103001',
    userId: 'user003',
    userName: '李女士',
    userAvatar: '/static/images/avatars/user3.jpg',
    rating: 5,
    content: '送给妈妈的，妈妈非常喜欢！花很新鲜，颜色也很正。包装很用心，卡片也写得很漂亮。',
    images: ['/static/images/reviews/review3-1.jpg'],
    reply: '感谢您的评价！我们会继续努力，为您提供更好的产品和服务！',
    replyTime: '2024-01-17 10:00:00',
    status: 'approved',
    isAnonymous: false,
    createTime: '2024-01-17 08:45:00',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440003',
    productName: '白色百合花束',
    productImage: '/static/images/products/lily-white.jpg',
    orderId: generateGuid(),
    orderNo: 'ORD20240104001',
    userId: 'user004',
    userName: '王先生',
    rating: 2,
    content: '花收到了，但是有几朵已经蔫了，不太满意。',
    status: 'pending',
    isAnonymous: false,
    createTime: '2024-01-18 16:30:00',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440004',
    productName: '向日葵花束',
    productImage: '/static/images/products/sunflower.jpg',
    orderId: generateGuid(),
    orderNo: 'ORD20240105001',
    userId: 'user005',
    userName: '匿名用户',
    rating: 3,
    content: '一般般吧，没有图片上看起来那么大。',
    status: 'rejected',
    isAnonymous: true,
    createTime: '2024-01-19 11:20:00',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440001',
    productName: '红玫瑰鲜花束',
    productImage: '/static/images/products/rose-red.jpg',
    orderId: generateGuid(),
    orderNo: 'ORD20240106001',
    userId: 'user006',
    userName: '陈女士',
    userAvatar: '/static/images/avatars/user6.jpg',
    rating: 5,
    content: '非常满意！已经是第三次购买了，每次都很满意。推荐给大家！',
    images: [
      '/static/images/reviews/review6-1.jpg',
      '/static/images/reviews/review6-2.jpg',
      '/static/images/reviews/review6-3.jpg',
    ],
    reply: '感谢您的支持和信任！我们会继续努力！',
    replyTime: '2024-01-20 09:30:00',
    status: 'approved',
    isAnonymous: false,
    createTime: '2024-01-20 08:00:00',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440006',
    productName: '蓝色妖姬花束',
    productImage: '/static/images/products/blue-rose.jpg',
    orderId: generateGuid(),
    orderNo: 'ORD20240107001',
    userId: 'user007',
    userName: '刘先生',
    rating: 4,
    content: '花很漂亮，女朋友很喜欢。就是价格稍微有点贵。',
    status: 'pending',
    isAnonymous: false,
    createTime: '2024-01-21 15:45:00',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440002',
    productName: '粉色康乃馨花束',
    productImage: '/static/images/products/carnation-pink.jpg',
    orderId: generateGuid(),
    orderNo: 'ORD20240108001',
    userId: 'user008',
    userName: '周女士',
    rating: 5,
    content: '第二次购买了，质量一如既往的好！',
    status: 'approved',
    isAnonymous: false,
    createTime: '2024-01-22 10:10:00',
  },
]

export default [
  // 获取评价列表
  {
    url: '/api/product/review/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; productId?: string; productName?: string; userName?: string; rating?: number | string; status?: string; startDate?: string; endDate?: string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { productId, productName, userName, status, startDate, endDate } = query
      const rating = query.rating !== undefined ? Number(query.rating) : undefined

      let filteredReviews = reviews.filter((review) => {
        let match = true
        if (productId && review.productId !== productId) {
          match = false
        }
        if (productName && !review.productName.toLowerCase().includes(productName.toLowerCase())) {
          match = false
        }
        if (userName && !review.userName.toLowerCase().includes(userName.toLowerCase())) {
          match = false
        }
        if (rating !== undefined && review.rating !== rating) {
          match = false
        }
        if (status && review.status !== status) {
          match = false
        }
        if (startDate) {
          const reviewDate = new Date(review.createTime)
          const start = new Date(startDate)
          if (reviewDate < start) {
            match = false
          }
        }
        if (endDate) {
          const reviewDate = new Date(review.createTime)
          const end = new Date(endDate)
          end.setHours(23, 59, 59)
          if (reviewDate > end) {
            match = false
          }
        }
        return match
      })

      // 按时间倒序
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

  // 获取评价详情
  {
    url: '/api/product/review/detail',
    method: 'get',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query
      const review = reviews.find(r => r.id === id)

      if (!review) {
        return {
          code: 404,
          message: '评价不存在',
          data: null,
        }
      }

      return {
        code: 200,
        message: '成功',
        data: review,
      }
    },
  },

  // 审核评价
  {
    url: '/api/product/review/audit',
    method: 'post',
    response: ({ body }: { body: { id: string; status: 'pending' | 'approved' | 'rejected' } }) => {
      const { id, status } = body
      const review = reviews.find(r => r.id === id)

      if (!review) {
        return {
          code: 404,
          message: '评价不存在',
          data: null,
        }
      }

      review.status = status
      return {
        code: 200,
        message: '审核成功',
        data: review,
      }
    },
  },

  // 回复评价
  {
    url: '/api/product/review/reply',
    method: 'post',
    response: ({ body }: { body: { id: string; reply: string } }) => {
      const { id, reply } = body
      const review = reviews.find(r => r.id === id)

      if (!review) {
        return {
          code: 404,
          message: '评价不存在',
          data: null,
        }
      }

      review.reply = reply
      review.replyTime = formatNow()

      return {
        code: 200,
        message: '回复成功',
        data: review,
      }
    },
  },

  // 删除评价
  {
    url: '/api/product/review/delete',
    method: 'post',
    response: ({ body }: { body: { ids: string | string[] } }) => {
      const { ids } = body
      const idsArray = Array.isArray(ids) ? ids : [ids]

      idsArray.forEach((id) => {
        const index = reviews.findIndex(r => r.id === id)
        if (index !== -1) {
          reviews.splice(index, 1)
        }
      })

      return {
        code: 200,
        message: '删除成功',
        data: null,
      }
    },
  },

  // 获取评价统计
  {
    url: '/api/product/review/statistics',
    method: 'get',
    response: ({ query }: { query: { productId?: string; productName?: string; userName?: string; rating?: number | string; status?: string; startDate?: string; endDate?: string } }) => {
      const { productId, productName, userName, status, startDate, endDate } = query
      const rating = query.rating !== undefined ? Number(query.rating) : undefined

      let filteredReviews = reviews.filter((review) => {
        let match = true
        if (productId && review.productId !== productId) {
          match = false
        }
        if (productName && !review.productName.toLowerCase().includes(productName.toLowerCase())) {
          match = false
        }
        if (userName && !review.userName.toLowerCase().includes(userName.toLowerCase())) {
          match = false
        }
        if (rating !== undefined && review.rating !== rating) {
          match = false
        }
        if (status && review.status !== status) {
          match = false
        }
        if (startDate) {
          const reviewDate = new Date(review.createTime)
          const start = new Date(startDate)
          if (reviewDate < start) {
            match = false
          }
        }
        if (endDate) {
          const reviewDate = new Date(review.createTime)
          const end = new Date(endDate)
          end.setHours(23, 59, 59)
          if (reviewDate > end) {
            match = false
          }
        }
        return match
      })

      const total = filteredReviews.length
      const averageRating = total > 0
        ? filteredReviews.reduce((sum, r) => sum + r.rating, 0) / total
        : 0

      // 评分分布
      const ratingDistribution = [1, 2, 3, 4, 5].map(rating => ({
        rating,
        count: filteredReviews.filter(r => r.rating === rating).length,
      }))

      const pendingCount = filteredReviews.filter(r => r.status === 'pending').length
      const approvedCount = filteredReviews.filter(r => r.status === 'approved').length
      const rejectedCount = filteredReviews.filter(r => r.status === 'rejected').length

      return {
        code: 200,
        message: '成功',
        data: {
          total,
          averageRating: Number(averageRating.toFixed(1)),
          ratingDistribution,
          pendingCount,
          approvedCount,
          rejectedCount,
        },
      }
    },
  },

  // 批量审核评价
  {
    url: '/api/product/review/batch-audit',
    method: 'post',
    response: ({ body }: { body: { ids: string[]; status: 'pending' | 'approved' | 'rejected' } }) => {
      const { ids, status } = body

      ids.forEach(id => {
        const review = reviews.find(r => r.id === id)
        if (review) {
          review.status = status
        }
      })

      return {
        code: 200,
        message: '批量审核成功',
        data: { count: ids.length },
      }
    },
  },
] as MockMethod[]