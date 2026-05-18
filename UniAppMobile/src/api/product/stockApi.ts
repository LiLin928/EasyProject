// api/product/stockApi.ts

import { get, post } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { StockRecord, Stock, PageResponse } from '@/types'

/** 库存查询参数 */
export interface StockQueryParams {
  pageIndex: number
  pageSize: number
  productId?: string
  productName?: string
}

/** 库存记录查询参数 */
export interface StockRecordQueryParams {
  pageIndex: number
  pageSize: number
  productId?: string
}

/** 库存调整参数 */
export interface StockAdjustParams {
  productId: string
  quantity: number
  reason?: string
}

/** 获取库存列表 */
export function getStockList(params: StockQueryParams): Promise<PageResponse<Stock>> {
  return post<PageResponse<Stock>>(`${API_PATHS.STOCK_LIST}`, params)
}

/** 库存调整 */
export function adjustStock(params: StockAdjustParams): Promise<number> {
  return post<number>('/api/product/stock/adjust', params)
}

/** 获取库存记录 */
export function getStockRecord(params: StockRecordQueryParams): Promise<PageResponse<StockRecord>> {
  return post<PageResponse<StockRecord>>('/api/product/stock/record', params)
}

/** 获取库存预警列表 */
export function getStockAlert(): Promise<Stock[]> {
  return post<Stock[]>('/api/product/stock/alert')
}

/** 库存入库 */
export function stockIn(params: { productId: string; quantity: number; supplierId?: string }): Promise<number> {
  return post<number>('/api/product/stock/in', params)
}

/** 获取库存统计 */
export function getStockStatistics(): Promise<{ total: number; lowStock: number; outOfStock: number }> {
  return post<{ total: number; lowStock: number; outOfStock: number }>('/api/product/stats/stock')
}