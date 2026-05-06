// src/types/product.ts

/**
 * 商品分类
 */
export interface Category {
  id: string
  name: string
  icon?: string
  sort: number
  description?: string
}

/**
 * 商品信息
 */
export interface Product {
  id: string
  skuCode: string // SKU码，商品唯一标识
  name: string // 商品名称（可重复）
  description?: string
  price: number
  originalPrice?: number
  image: string
  images?: string[]
  categoryId: string
  categoryName?: string // 分类名称
  category?: Category
  stock: number
  sales?: number
  isHot?: boolean
  isNew?: boolean
  detail?: string
  alertThreshold?: number // 库存预警阈值，默认 10
  // 审核相关字段
  auditStatus?: number // 审核状态：0-待提交, 1-待审核, 2-已拒绝, 3-已通过, 4-已撤回
  workflowInstanceId?: string // 工作流实例ID
  auditPointCode?: string // 审核点编码
  auditTime?: string // 审核时间
  auditorId?: string // 审核人ID
  auditRemark?: string // 审核备注
  createTime?: string
  updateTime?: string
}

/**
 * 商品列表查询参数
 */
export interface ProductListParams {
  pageIndex?: number
  pageSize?: number
  name?: string
  categoryId?: string
  skuCode?: string // SKU码
  auditStatus?: number // 审核状态
  isHot?: boolean
  isNew?: boolean
}

/**
 * 创建商品参数
 */
export interface CreateProductParams {
  skuCode: string // SKU码（必填）
  name: string
  description?: string
  price: number
  originalPrice?: number
  image: string
  images?: string[]
  categoryId: string
  stock: number
  alertThreshold?: number
  isHot?: boolean
  isNew?: boolean
  detail?: string
}

/**
 * 更新商品参数
 */
export interface UpdateProductParams extends CreateProductParams {
  id: string
}

/**
 * 创建分类参数
 */
export interface CreateCategoryParams {
  name: string
  icon?: string
  sort: number
  description?: string
}

/**
 * 更新分类参数
 */
export interface UpdateCategoryParams extends CreateCategoryParams {
  id: string
}

// ===================== 库存管理类型 =====================

/**
 * 库存变动类型
 */
export type StockChangeType = 'in' | 'out' | 'adjust'

/**
 * 库存变动记录
 */
export interface StockRecord {
  id: string
  productId: string
  skuCode?: string // SKU码
  productName: string
  productImage?: string
  type: StockChangeType // in: 入库, out: 出库, adjust: 调整
  quantity: number // 变动数量（正数为增加，负数为减少）
  beforeStock: number // 变动前库存
  afterStock: number // 变动后库存
  supplierId?: string // 供应商ID（入库时）
  supplierName?: string // 供应商名称
  purchasePrice?: number // 采购价格（入库时）
  operator?: string // 操作人
  remark?: string // 备注
  createTime: string
}

/**
 * 库存查询参数
 */
export interface StockQueryParams {
  pageIndex?: number
  pageSize?: number
  name?: string
  categoryId?: string
  lowStock?: boolean // 只显示低库存
}

/**
 * 库存信息（库存列表返回，与后端 StockDto 对应）
 */
export interface Stock {
  productId: string // 商品ID
  skuCode: string // SKU码
  productName: string // 商品名称
  productImage?: string // 商品图片
  categoryName?: string // 分类名称
  stock: number // 库存数量
  alertThreshold: number // 库存预警阈值
  isLowStock: boolean // 是否低库存
  updateTime?: string // 更新时间
}

/**
 * 出入库记录查询参数
 */
export interface StockRecordQueryParams {
  pageIndex?: number
  pageSize?: number
  productId?: string
  productName?: string
  type?: StockChangeType
  startDate?: string
  endDate?: string
}

/**
 * 库存调整参数
 */
export interface StockAdjustParams {
  productId: string
  type: StockChangeType
  quantity: number
  supplierId?: string // 入库时必填
  purchasePrice?: number // 入库时必填
  remark?: string
}

// ===================== 供应商管理类型 =====================

/**
 * 供应商信息
 */
export interface Supplier {
  id: string
  name: string // 供应商名称
  unifiedCode?: string // 三证合一码
  contact?: string // 联系人
  phone?: string // 联系电话
  address?: string // 地址
  remark?: string // 备注
  status: 0 | 1 // 状态：0-禁用, 1-启用
  createTime?: string
  updateTime?: string
}

/**
 * 供应商查询参数
 */
export interface SupplierQueryParams {
  pageIndex?: number
  pageSize?: number
  name?: string
  status?: 0 | 1
}

/**
 * 创建供应商参数
 */
export interface CreateSupplierParams {
  name: string
  unifiedCode?: string // 三证合一码
  contact?: string
  phone?: string
  address?: string
  remark?: string
  status?: 0 | 1
}

/**
 * 更新供应商参数
 */
export interface UpdateSupplierParams extends CreateSupplierParams {
  id: string
}

// ===================== 商品-供应商关联类型 =====================

/**
 * 商品-供应商关联
 */
export interface ProductSupplier {
  id: string
  productId: string
  supplierId: string
  supplierName?: string // 供应商名称
  supplier?: Supplier // 可选的供应商详情
  product?: Product // 商品信息
  skuCode: string // SKU码
  purchasePrice: number // 采购价格
  minOrderQty?: number // 最小起订量
  isDefault: boolean // 是否默认供应商
  remark?: string
}

/**
 * 绑定商品供应商参数
 */
export interface BindProductSupplierParams {
  productId: string
  supplierId: string
  skuCode: string // SKU码（必填）
  purchasePrice: number
  minOrderQty?: number
  isDefault?: boolean
  remark?: string
}

// ===================== 商品评价管理类型 =====================

/**
 * 评价状态
 */
export type ReviewStatus = 'pending' | 'approved' | 'rejected'

/**
 * 商品评价
 */
export interface ProductReview {
  id: string
  productId: string
  productName: string
  productImage?: string
  orderId: string
  orderNo: string
  userId: string
  userName: string
  userAvatar?: string
  rating: number // 评分 1-5
  content: string // 评价内容
  images?: string[] // 评价图片
  reply?: string // 商家回复
  replyTime?: string // 回复时间
  status: ReviewStatus // 状态：pending-待审核, approved-已通过, rejected-已拒绝
  isAnonymous: boolean // 是否匿名
  createTime: string
}

/**
 * 评价查询参数
 */
export interface ReviewQueryParams {
  pageIndex?: number
  pageSize?: number
  productId?: string
  productName?: string
  userName?: string
  rating?: number
  status?: ReviewStatus
  startDate?: string
  endDate?: string
}

/**
 * 回复评价参数
 */
export interface ReplyReviewParams {
  id: string
  reply: string
}

/**
 * 审核评价参数
 */
export interface AuditReviewParams {
  id: string
  status: ReviewStatus
}

/**
 * 评价统计
 */
export interface ReviewStatistics {
  total: number // 总评价数
  averageRating: number // 平均评分
  ratingDistribution: {
    rating: number
    count: number
  }[]
  pendingCount: number // 待审核数
  approvedCount: number // 已通过数
  rejectedCount: number // 已拒绝数
}

// ===================== 商品统计报表类型 =====================

/**
 * 商品销量统计
 */
export interface ProductSalesStats {
  productId: string
  productName: string
  productImage?: string
  skuCode: string
  salesCount: number // 销量
  salesAmount: number // 销售额
  orderCount: number // 订单数
  avgPrice: number // 平均单价
}

/**
 * 销量趋势数据
 */
export interface SalesTrend {
  date: string
  salesCount: number
  salesAmount: number
  orderCount: number
}

/**
 * 库存统计
 */
export interface StockStatistics {
  totalProducts: number // 商品总数
  totalStock: number // 库存总量
  lowStockCount: number // 低库存商品数
  outOfStockCount: number // 缺货商品数
  totalValue: number // 库存总价值
}

/**
 * 商品统计查询参数
 */
export interface ProductStatsParams {
  startDate?: string
  endDate?: string
  categoryId?: string
  top?: number // 排名前N
}