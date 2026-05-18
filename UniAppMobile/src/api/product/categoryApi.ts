// api/product/categoryApi.ts

import { get, post, put } from '@/utils/request'
import type { ProductCategory } from '@/types/product'

const BASE_URL = '/api/product/category'

/** 获取分类树形列表 */
export function getCategoryTree(): Promise<ProductCategory[]> {
  return post<ProductCategory[]>(`${BASE_URL}/list`)
}

/** 获取分类详情 */
export function getCategoryDetail(id: string): Promise<ProductCategory> {
  return get<ProductCategory>(`${BASE_URL}/detail/${id}`)
}

/** 创建分类 */
export function createCategory(data: Partial<ProductCategory>): Promise<{ id: string }> {
  return post<{ id: string }>(`${BASE_URL}/add`, data)
}

/** 更新分类 */
export function updateCategory(data: ProductCategory): Promise<number> {
  return put<number>(`${BASE_URL}/update`, data)
}

/** 删除分类 */
export function deleteCategory(id: string): Promise<number> {
  return post<number>(`${BASE_URL}/delete`, id)
}