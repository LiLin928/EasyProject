// src/mock/points.ts

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * 积分变动类型
 */
type PointsChangeType = 'consume_gain' | 'exchange_use' | 'manual_adjust' | 'expire_clear'

/**
 * Mock 积分记录类型
 */
interface MockPointsLog {
  id: string
  customerId: string
  customerName: string
  customerPhone: string
  changeType: PointsChangeType
  changeAmount: number
  beforePoints: number
  afterPoints: number
  reason: string
  orderId?: string
  operatorId?: string
  operatorName?: string
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

// 初始积分记录数据
const pointsLogs: MockPointsLog[] = [
  {
    id: generateGuid(),
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    customerName: '小王',
    customerPhone: '13800138001',
    changeType: 'consume_gain',
    changeAmount: 85,
    beforePoints: 0,
    afterPoints: 85,
    reason: '订单消费获得积分',
    orderId: 'order-0001-aaaa-bbbb-cccc-0001',
    createTime: '2024-01-15 10:30:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    customerName: '小王',
    customerPhone: '13800138001',
    changeType: 'consume_gain',
    changeAmount: 120,
    beforePoints: 85,
    afterPoints: 205,
    reason: '订单消费获得积分',
    orderId: 'order-0002-aaaa-bbbb-cccc-0002',
    createTime: '2024-02-20 14:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    customerName: '小王',
    customerPhone: '13800138001',
    changeType: 'consume_gain',
    changeAmount: 315,
    beforePoints: 205,
    afterPoints: 520,
    reason: '金卡会员订单消费获得积分(1.5倍)',
    orderId: 'order-0003-aaaa-bbbb-cccc-0003',
    createTime: '2024-03-15 16:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0002-aaaa-bbbb-cccc-0002',
    customerName: '小李',
    customerPhone: '13800138002',
    changeType: 'consume_gain',
    changeAmount: 38,
    beforePoints: 0,
    afterPoints: 38,
    reason: '订单消费获得积分',
    orderId: 'order-0004-aaaa-bbbb-cccc-0004',
    createTime: '2024-01-20 11:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0002-aaaa-bbbb-cccc-0002',
    customerName: '小李',
    customerPhone: '13800138002',
    changeType: 'consume_gain',
    changeAmount: 96,
    beforePoints: 38,
    afterPoints: 134,
    reason: '银卡会员订单消费获得积分(1.2倍)',
    orderId: 'order-0005-aaaa-bbbb-cccc-0005',
    createTime: '2024-02-10 09:30:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0002-aaaa-bbbb-cccc-0002',
    customerName: '小李',
    customerPhone: '13800138002',
    changeType: 'consume_gain',
    changeAmount: 46,
    beforePoints: 134,
    afterPoints: 180,
    reason: '银卡会员订单消费获得积分(1.2倍)',
    orderId: 'order-0006-aaaa-bbbb-cccc-0006',
    createTime: '2024-03-10 12:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0003-aaaa-bbbb-cccc-0003',
    customerName: '小张',
    customerPhone: '13800138003',
    changeType: 'consume_gain',
    changeAmount: 50,
    beforePoints: 0,
    afterPoints: 50,
    reason: '订单消费获得积分',
    orderId: 'order-0007-aaaa-bbbb-cccc-0007',
    createTime: '2024-02-20 09:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0004-aaaa-bbbb-cccc-0004',
    customerName: '小陈',
    customerPhone: '13800138004',
    changeType: 'consume_gain',
    changeAmount: 200,
    beforePoints: 0,
    afterPoints: 200,
    reason: '钻石会员订单消费获得积分(2倍)',
    orderId: 'order-0008-aaaa-bbbb-cccc-0008',
    createTime: '2024-01-10 10:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0004-aaaa-bbbb-cccc-0004',
    customerName: '小陈',
    customerPhone: '13800138004',
    changeType: 'consume_gain',
    changeAmount: 500,
    beforePoints: 200,
    afterPoints: 700,
    reason: '钻石会员订单消费获得积分(2倍)',
    orderId: 'order-0009-aaaa-bbbb-cccc-0009',
    createTime: '2024-02-15 14:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0004-aaaa-bbbb-cccc-0004',
    customerName: '小陈',
    customerPhone: '13800138004',
    changeType: 'consume_gain',
    changeAmount: 1300,
    beforePoints: 700,
    afterPoints: 2000,
    reason: '钻石会员大额订单消费获得积分(2倍)',
    orderId: 'order-0010-aaaa-bbbb-cccc-0010',
    createTime: '2024-04-01 16:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    customerName: '小王',
    customerPhone: '13800138001',
    changeType: 'manual_adjust',
    changeAmount: 100,
    beforePoints: 205,
    afterPoints: 305,
    reason: '系统补偿积分',
    operatorId: 'admin-001',
    operatorName: '管理员',
    createTime: '2024-03-01 10:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0002-aaaa-bbbb-cccc-0002',
    customerName: '小李',
    customerPhone: '13800138002',
    changeType: 'exchange_use',
    changeAmount: -50,
    beforePoints: 134,
    afterPoints: 84,
    reason: '积分兑换礼品',
    orderId: 'exchange-0001-aaaa-bbbb-cccc-0001',
    createTime: '2024-02-25 15:00:00',
  },
]

export default [
  // ==================== 积分记录 API ====================

  // 获取积分记录列表
  {
    url: '/api/points/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; customerKeyword?: string; changeType?: PointsChangeType; startDate?: string; endDate?: string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { customerKeyword, changeType, startDate, endDate } = query

      let filteredLogs = pointsLogs.filter((log) => {
        let match = true
        if (customerKeyword) {
          const keyword = customerKeyword.toLowerCase()
          if (!log.customerName.toLowerCase().includes(keyword) && !log.customerPhone.includes(customerKeyword)) {
            match = false
          }
        }
        if (changeType && log.changeType !== changeType) {
          match = false
        }
        if (startDate && new Date(log.createTime) < new Date(startDate)) {
          match = false
        }
        if (endDate && new Date(log.createTime) > new Date(endDate + ' 23:59:59')) {
          match = false
        }
        return match
      })

      // 按时间倒序
      filteredLogs.sort((a, b) =>
        new Date(b.createTime).getTime() - new Date(a.createTime).getTime()
      )

      const total = filteredLogs.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredLogs.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },
] as MockMethod[]