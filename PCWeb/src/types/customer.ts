// src/types/customer.ts

/**
 * 会员等级
 */
export interface MemberLevel {
  id: string
  name: string
  minSpent: number
  discount: number
  pointsRate: number
  icon: string
  sort: number
  status: 0 | 1
  createTime: string
  updateTime: string
}

/**
 * 客户信息
 */
export interface Customer {
  id: string
  username?: string
  nickname?: string
  phone?: string
  email?: string
  avatar?: string
  status: 0 | 1
  levelId?: string
  levelName?: string
  points: number
  totalSpent: number
  createTime: string
  updateTime?: string
}

/**
 * 客户列表查询参数
 */
export interface CustomerListParams {
  pageIndex?: number
  pageSize?: number
  phone?: string
  nickname?: string
  levelId?: string
  status?: 0 | 1
}

/**
 * 创建客户参数
 */
export interface CreateCustomerParams {
  username?: string
  nickname: string
  phone: string
  email?: string
  avatar?: string
  password?: string
  status?: 0 | 1
  levelId?: string
}

/**
 * 更新客户参数
 */
export interface UpdateCustomerParams {
  id: string
  username?: string
  nickname?: string
  phone?: string
  email?: string
  avatar?: string
  status?: 0 | 1
  levelId?: string
}

/**
 * 客户收货地址
 */
export interface CustomerAddress {
  id: string
  customerId?: string
  name: string
  phone: string
  province: string
  city: string
  district: string
  detail: string // 详细地址（与后端 Detail 对应）
  isDefault: boolean
  fullAddress?: string // 完整地址
  createTime?: string
  updateTime?: string
}

/**
 * 创建地址参数
 */
export interface CreateAddressParams {
  customerId: string
  name: string
  phone: string
  province: string
  city: string
  district: string
  address: string
  isDefault?: boolean
}

/**
 * 更新地址参数
 */
export interface UpdateAddressParams extends CreateAddressParams {
  id: string
}

/**
 * 客户购物车项
 */
export interface CustomerCart {
  id: string
  customerId: string
  productId: string
  productName: string
  productImage: string
  skuId: string
  skuName: string
  quantity: number
  price: number
  selected: boolean
  createTime: string
  updateTime: string
}

/**
 * 收藏分组
 */
export interface FavoriteGroup {
  id: string
  customerId: string
  name: string
  sort: number
  createTime: string
  updateTime: string
}

/**
 * 客户收藏项
 */
export interface CustomerFavorite {
  id: string
  customerId: string
  productId: string
  productName: string
  productImage: string
  groupId: string
  groupName: string
  createTime: string
}

/**
 * 积分变动类型（与后端对应）
 */
export type PointsChangeType = 'review' | 'order' | 'exchange' | 'refund' | 'system'

/**
 * 积分记录（与后端 PointsRecordDto 对应）
 */
export interface PointsLog {
  id: string
  userId: string
  userName: string
  userPhone: string
  points: number // 积分变动
  balance: number // 变动后余额
  type: PointsChangeType
  typeText: string // 变动类型文本
  reason: string
  createTime: string
}

/**
 * 积分记录查询参数（与后端 QueryPointsRecordDto 对应）
 */
export interface PointsLogListParams {
  pageIndex?: number
  pageSize?: number
  userId?: string
  userKeyword?: string
  type?: PointsChangeType
  startTime?: string
  endTime?: string
}

/**
 * 积分调整参数
 */
export interface AdjustPointsParams {
  userId: string
  type: 'add' | 'subtract'
  amount: number
  reason: string
}

/**
 * 等级调整参数
 */
export interface AdjustLevelParams {
  userId: string
  levelId: string
  reason: string
}

/**
 * 创建会员等级参数
 */
export interface CreateMemberLevelParams {
  name: string
  minSpent: number
  discount: number
  pointsRate: number
  icon?: string
  sort?: number
  status?: 0 | 1
}

/**
 * 更新会员等级参数
 */
export interface UpdateMemberLevelParams extends CreateMemberLevelParams {
  id: string
}