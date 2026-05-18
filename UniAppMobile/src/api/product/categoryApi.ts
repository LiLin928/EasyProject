// api/product/categoryApi.ts

import { get, post, del } from '@/utils/request'
import type { ProductCategory } from '@/types/product'

const BASE_URL = '/api/product/category'

/** 获取分类树 */
export function getCategoryTree(): Promise<ProductCategory[]> {
  return get<ProductCategory[]>(`${BASE_URL}/tree`)
}

/** 创建分类 */
export function createCategory(data: Partial<ProductCategory>): Promise<{ id: string }> {
  return post<{ id: string }>(BASE_URL, data)
}

/** 更新分类 */
export function updateCategory(data: ProductCategory): Promise<number> {
  return post<number>(BASE_URL, data)
}

/** 删除分类 */
export function deleteCategory(id: string): Promise<number> {
  return del<number>(`${BASE_URL}/${id}`)
}