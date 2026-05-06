// src/api/buz/refundApi.ts

import { get, post, put } from '@/utils/request'
import type {
  Refund,
  RefundListParams,
  ApproveRefundParams,
  RejectRefundParams,
  ExchangeShipParams,
  RefundType,
  ExchangeItem,
} from '@/types'

// ==================== 售后管理 API ====================

/**
 * 获取售后列表
 */
export function getRefundList(params: RefundListParams) {
  return post<{ list: Refund[]; total: number }>('/api/refund/list', params)
}

/**
 * 获取售后详情
 */
export function getRefundDetail(id: string) {
  return get<Refund>(`/api/refund/detail/${id}`)
}

/**
 * 通过审核
 */
export function approveRefund(data: ApproveRefundParams) {
  return put<{ success: boolean }>('/api/refund/approve', data)
}

/**
 * 拒绝审核
 */
export function rejectRefund(data: RejectRefundParams) {
  return put<{ success: boolean }>('/api/refund/reject', data)
}

/**
 * 确认收到退货
 */
export function confirmReceive(id: string, remark?: string) {
  return put<{ success: boolean }>(`/api/refund/confirm-receive/${id}`, null, { params: { remark } })
}

/**
 * 换货发货
 */
export function exchangeShip(data: ExchangeShipParams) {
  return post<{ success: boolean }>('/api/refund/exchange-ship', data)
}

/**
 * 完成售后
 */
export function completeRefund(id: string) {
  return put<{ success: boolean }>(`/api/refund/complete/${id}`)
}

/**
 * 创建售后申请
 */
export function createRefund(data: {
  orderId: string
  type: RefundType
  items: {
    orderItemId: string
    productId: string
    productName: string
    productImage: string
    price: number
    quantity: number
  }[]
  reason: string
  description?: string
  refundAmount?: number
  exchangeItems?: ExchangeItem[]
}) {
  return post<{ id: string }>('/api/refund/create', data)
}

/**
 * 获取换货物流详情
 */
export function getShipDetail(id: string) {
  return get<{
    shipNo: string
    shipCompany: string
    shipStatus: string
    tracks: { time: string; status: string; location: string }[]
  }>(`/api/refund/ship-detail/${id}`)
}