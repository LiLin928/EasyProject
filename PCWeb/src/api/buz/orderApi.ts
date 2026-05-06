// src/api/buz/orderApi.ts

import { get, post, put } from '@/utils/request'
import type {
  Order,
  OrderListParams,
  OrderStatistics,
  ShipParams,
  CreateOrderParams,
  UpdateOrderParams,
  ShipTrackResponse,
} from '@/types'

// ==================== 订单管理 API ====================

/**
 * 获取订单列表
 */
export function getOrderList(params: OrderListParams) {
  return post<{ list: Order[]; total: number }>('/api/order/list', params)
}

/**
 * 获取订单详情
 */
export function getOrderDetail(id: string) {
  return get<Order>(`/api/order/detail/${id}`)
}

/**
 * 创建订单（后台代客下单）
 */
export function createOrder(data: CreateOrderParams) {
  return post<{ id: string }>('/api/order/create', data)
}

/**
 * 更新订单
 */
export function updateOrder(data: UpdateOrderParams) {
  return put<{ success: boolean }>('/api/order/update', data)
}

/**
 * 订单发货
 */
export function shipOrder(data: ShipParams) {
  return post<{ success: boolean }>('/api/order/ship', data)
}

/**
 * 取消订单
 */
export function cancelOrder(id: string) {
  return put<{ success: boolean }>(`/api/order/cancel/${id}`)
}

/**
 * 确认收货
 */
export function confirmReceive(id: string) {
  return put<{ success: boolean }>(`/api/order/confirm/${id}`)
}

/**
 * 获取物流轨迹
 */
export function getShipTrack(id: string) {
  return get<ShipTrackResponse>(`/api/order/track/${id}`)
}

/**
 * 获取订单统计
 */
export function getOrderStatistics() {
  return post<OrderStatistics>('/api/order/statistics')
}

/**
 * 导出订单
 */
export function exportOrders(params: OrderListParams) {
  return post('/api/order/export', params, { responseType: 'blob' })
}

// ==================== 评价 API ====================

/**
 * 获取评价列表（管理后台）
 */
export function getReviewList(params: { pageIndex: number; pageSize: number; orderNo?: string; productId?: string; status?: string }) {
  return post<{ list: any[]; total: number }>('/api/order/review/list', params)
}

/**
 * 获取评价详情
 */
export function getReviewDetail(id: string) {
  return get<any>(`/api/order/review/detail/${id}`)
}

/**
 * 创建评价回复
 */
export function createReview(data: {
  orderId: string
  productId: string
  rating: number
  content: string
  images?: string[]
}) {
  return post<{ id: string }>('/api/order/review/create', data)
}