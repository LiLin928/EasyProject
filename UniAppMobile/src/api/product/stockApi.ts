// api/product/stockApi.ts

import { get } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { StockRecord, PageResponse } from '@/types'

/** 库存记录查询参数 */
export interface StockQueryParams {
  pageIndex: number
  pageSize: number
  productName?: string
}

/** 获取库存记录列表 */
export function getStockList(params: StockQueryParams): Promise<PageResponse<StockRecord>> {
  return get<PageResponse<StockRecord>>(API_PATHS.STOCK_LIST, params)
}