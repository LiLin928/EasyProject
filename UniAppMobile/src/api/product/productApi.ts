// api/product/productApi.ts

import { get, post, put } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Product, ProductQueryParams, ProductFormParams, PageResponse } from '@/types'

/** 获取商品列表 */
export function getProductList(params: ProductQueryParams): Promise<PageResponse<Product>> {
  return post<PageResponse<Product>>(API_PATHS.PRODUCT_LIST, params)
}

/** 获取商品详情 */
export function getProductDetail(id: string): Promise<Product> {
  return get<Product>(`${API_PATHS.PRODUCT_DETAIL}/${id}`)
}

/** 创建商品 */
export function createProduct(data: ProductFormParams): Promise<{ id: string }> {
  return post<{ id: string }>(API_PATHS.PRODUCT_ADD, data)
}

/** 更新商品 */
export function updateProduct(data: ProductFormParams): Promise<number> {
  return put<number>(API_PATHS.PRODUCT_UPDATE, data)
}

/** 删除商品 */
export function deleteProduct(id: string): Promise<number> {
  return post<number>(API_PATHS.PRODUCT_DELETE, id)
}

/** 批量删除商品 */
export function deleteProductBatch(ids: string[]): Promise<number> {
  return post<number>(`${API_PATHS.PRODUCT_DELETE}-batch`, ids)
}

/** 上架商品 */
export function onShelfProduct(id: string): Promise<number> {
  return post<number>('/api/product/batch-status', { ids: [id], status: 1 })
}

/** 下架商品 */
export function offShelfProduct(id: string): Promise<number> {
  return post<number>('/api/product/batch-status', { ids: [id], status: 0 })
}