// src/api/buz/productApi.ts

import { get, post, put } from '@/utils/request'
import type {
  Product,
  Category,
  ProductListParams,
  CreateProductParams,
  UpdateProductParams,
  CreateCategoryParams,
  UpdateCategoryParams,
  StockRecord,
  StockQueryParams,
  StockRecordQueryParams,
  StockAdjustParams,
  Supplier,
  SupplierQueryParams,
  CreateSupplierParams,
  UpdateSupplierParams,
  ProductSupplier,
  BindProductSupplierParams,
  ProductReview,
  ReviewQueryParams,
  ReplyReviewParams,
  AuditReviewParams,
  ReviewStatistics,
  ProductSalesStats,
  SalesTrend,
  StockStatistics,
  ProductStatsParams,
  Stock,
  SubmitAuditParams,
} from '@/types'

// ===================== 商品 API =====================

/**
 * 获取商品列表
 */
export function getProductList(params: ProductListParams) {
  return post<{ list: Product[]; total: number; pageIndex: number; pageSize: number }>('/api/product/list', params)
}

/**
 * 获取商品详情
 */
export function getProductDetail(id: string) {
  return get<Product>(`/api/product/detail/${id}`)
}

/**
 * 创建商品
 */
export function createProduct(data: CreateProductParams) {
  return post<{ id: string }>('/api/product/add', data)
}

/**
 * 更新商品
 */
export function updateProduct(data: UpdateProductParams) {
  return put<number>('/api/product/update', data)
}

/**
 * 删除商品
 */
export function deleteProduct(id: string) {
  return post<number>('/api/product/delete', id)
}

/**
 * 批量删除商品
 * @param ids 商品ID列表 (GUID数组)
 */
export function deleteProductBatch(ids: string[]) {
  return post<number>('/api/product/delete-batch', ids)
}

/**
 * 批量更新商品状态
 */
export function batchUpdateProductStatus(ids: string[], status: number) {
  return put<{ count: number }>('/api/product/batch-status', { ids, status })
}

/**
 * 批量更新商品分类
 */
export function batchUpdateCategory(ids: string[], categoryId: string) {
  return put<{ count: number }>('/api/product/batch-category', { ids, categoryId })
}

/**
 * 批量更新商品标签
 */
export function batchUpdateTag(ids: string[], tags: { isHot?: boolean; isNew?: boolean }) {
  return put<{ count: number }>('/api/product/batch-tag', { ids, isHot: tags.isHot, isNew: tags.isNew })
}

/**
 * 批量创建商品
 */
export function batchCreateProducts(products: CreateProductParams[]) {
  return post<{ count: number }>('/api/product/batch-create', { products })
}

// ===================== 分类 API =====================

/**
 * 获取分类树形列表
 */
export function getCategoryList(params?: { name?: string }) {
  return post<Category[]>('/api/product/category/list', params)
}

/**
 * 获取分类详情
 */
export function getCategoryDetail(id: string) {
  return get<Category>(`/api/product/category/detail/${id}`)
}

/**
 * 创建分类
 */
export function createCategory(data: CreateCategoryParams) {
  return post<{ id: string }>('/api/product/category/add', data)
}

/**
 * 更新分类
 */
export function updateCategory(data: UpdateCategoryParams) {
  return put<number>('/api/product/category/update', data)
}

/**
 * 删除分类
 */
export function deleteCategory(id: string) {
  return post<number>('/api/product/category/delete', id)
}

// ===================== 库存管理 API =====================

/**
 * 获取库存列表
 */
export function getStockList(params: StockQueryParams) {
  return post<{ list: Stock[]; total: number; pageIndex: number; pageSize: number }>('/api/product/stock/list', params)
}

/**
 * 库存调整
 */
export function adjustStock(data: StockAdjustParams) {
  return post<number>('/api/product/stock/adjust', data)
}

/**
 * 获取库存变动记录
 */
export function getStockRecords(params: StockRecordQueryParams) {
  return post<{ list: StockRecord[]; total: number; pageIndex: number; pageSize: number }>('/api/product/stock/record', params)
}

/**
 * 获取库存预警列表
 */
export function getStockAlert() {
  return post<Stock[]>('/api/product/stock/alert')
}

// ===================== 供应商管理 API =====================

/**
 * 获取供应商列表
 */
export function getSupplierList(params?: SupplierQueryParams) {
  return post<{ list: Supplier[]; total: number; pageIndex: number; pageSize: number }>('/api/supplier/list', params)
}

/**
 * 获取供应商详情
 */
export function getSupplierDetail(id: string) {
  return get<Supplier>(`/api/supplier/detail/${id}`)
}

/**
 * 创建供应商
 */
export function createSupplier(data: CreateSupplierParams) {
  return post<{ id: string }>('/api/supplier/add', data)
}

/**
 * 更新供应商
 */
export function updateSupplier(data: UpdateSupplierParams) {
  return put<number>('/api/supplier/update', data)
}

/**
 * 删除供应商
 */
export function deleteSupplier(id: string) {
  return post<number>('/api/supplier/delete', id)
}

/**
 * 批量删除供应商
 * @param ids 供应商ID列表 (GUID数组)
 */
export function deleteSupplierBatch(ids: string[]) {
  return post<number>('/api/supplier/delete-batch', ids)
}

// ===================== 商品-供应商关联 API =====================

/**
 * 获取商品的供应商列表
 */
export function getProductSuppliers(productId: string) {
  return post<ProductSupplier[]>(`/api/supplier/product/${productId}`)
}

/**
 * 绑定商品供应商
 */
export function bindProductSupplier(data: BindProductSupplierParams) {
  return post<{ id: string }>('/api/supplier/bind', data)
}

/**
 * 解绑商品供应商
 */
export function unbindProductSupplier(id: string) {
  return post<number>('/api/supplier/unbind', id)
}

/**
 * 设置默认供应商
 */
export function setDefaultSupplier(id: string) {
  return put<number>(`/api/supplier/set-default/${id}`)
}

/**
 * 获取供应商的商品列表（含绑定信息）
 */
export function getSupplierProducts(supplierId: string) {
  return post<ProductSupplier[]>(`/api/supplier/products/${supplierId}`)
}

// ===================== 商品评价管理 API =====================

/**
 * 获取评价列表
 */
export function getReviewList(params: ReviewQueryParams) {
  return post<{ list: ProductReview[]; total: number; pageIndex: number; pageSize: number }>('/api/product/review/list', params)
}

/**
 * 获取评价详情
 */
export function getReviewDetail(id: string) {
  return get<ProductReview>(`/api/product/review/detail/${id}`)
}

/**
 * 回复评价
 */
export function replyReview(data: ReplyReviewParams) {
  return post<number>('/api/product/review/reply', data)
}

/**
 * 审核评价
 */
export function auditReview(params: { id: string; status: string }) {
  return put<number>('/api/product/review/audit', params)
}

/**
 * 隐藏评价
 */
export function hideReview(id: string) {
  return put<number>(`/api/product/review/hide/${id}`)
}

/**
 * 获取评价统计
 */
export function getReviewStatistics(params?: {
  productId?: string
  productName?: string
  userName?: string
  rating?: number
  status?: string
  startDate?: string
  endDate?: string
}) {
  return post<ReviewStatistics>('/api/product/review/stats', params || {})
}

/**
 * 删除评价
 */
export function deleteReview(id: string) {
  return post<number>('/api/product/review/delete', id)
}

/**
 * 批量审核评价
 */
export function batchAuditReview(ids: string[], status: string) {
  return post<number>('/api/product/review/batch-audit', { ids, status })
}

// ===================== 商品统计报表 API =====================

/**
 * 获取销量统计
 */
export function getSalesStats(params?: ProductStatsParams) {
  return post<ProductSalesStats[]>('/api/product/stats/sales', params)
}

/**
 * 获取销量趋势
 */
export function getSalesTrend(params?: { startDate?: string; endDate?: string; productId?: string }) {
  return post<SalesTrend[]>('/api/product/stats/trend', params)
}

/**
 * 获取库存统计
 */
export function getStockStatistics() {
  return post<StockStatistics>('/api/product/stats/stock')
}

/**
 * 获取概览统计
 */
export function getOverviewStats() {
  return post<{
    today: { salesCount: number; salesAmount: number; orderCount: number }
    growth: { salesCountGrowth: number; salesAmountGrowth: number; orderCountGrowth: number }
  }>('/api/product/stats/overview')
}

/**
 * 获取销量排行
 */
export function getSalesRanking(params?: { limit?: number; sortBy?: 'sales' | 'amount' }) {
  return post<ProductSalesStats[]>('/api/product/stats/ranking', params)
}

/**
 * 获取分类销量统计
 */
export function getCategorySalesStats(params?: { startDate?: string; endDate?: string }) {
  return post<{ categoryName: string; salesCount: number; salesAmount: number }[]>('/api/product/stats/category', params)
}

// ===================== 商品审核 API =====================

/**
 * 提交商品审核
 * @param data 提交审核参数
 */
export function submitProductAudit(data: SubmitAuditParams) {
  return post<{ success: boolean }>('/api/product/submit-audit', data)
}

/**
 * 撤回商品审核
 * @param id 商品ID (GUID)
 */
export function withdrawProductAudit(id: string, reason?: string) {
  return post<{ success: boolean }>('/api/product/withdraw-audit', { productId: id, reason })
}