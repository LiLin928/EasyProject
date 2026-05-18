// types/product.ts

/** 商品信息 */
export interface Product {
  id: string
  skuCode: string
  name: string
  description?: string
  price: number
  originalPrice?: number
  image: string
  images?: string[]
  categoryId: string
  categoryName?: string
  stock: number
  status: ProductStatus
  isHot: boolean
  isNew: boolean
  createTime?: string
  updateTime?: string
}

/** 商品状态 */
export enum ProductStatus {
  OffShelf = 0,    // 下架
  OnShelf = 1,     // 上架
}

/** 商品分类 */
export interface ProductCategory {
  id: string
  name: string
  parentId?: string
  children?: ProductCategory[]
  level: number
  sort: number
}

/** 商品列表查询参数 */
export interface ProductQueryParams {
  pageIndex: number
  pageSize: number
  name?: string
  categoryId?: string
  status?: ProductStatus
}

/** 创建/更新商品参数 */
export interface ProductFormParams {
  id?: string
  skuCode: string
  name: string
  description?: string
  price: number
  originalPrice?: number
  image: string
  images?: string[]
  categoryId: string
  stock: number
  status?: ProductStatus
  isHot?: boolean
  isNew?: boolean
}

/** 库存记录 */
export interface StockRecord {
  id: string
  productId: string
  productName: string
  skuCode: string
  quantity: number
  type: StockType
  reason: string
  operator: string
  operateTime: string
}

/** 库存变动类型 */
export enum StockType {
  In = 1,      // 入库
  Out = 2,     // 出库
  Adjust = 3,  // 调整
}