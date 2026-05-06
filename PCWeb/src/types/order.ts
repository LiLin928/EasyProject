// src/types/order.ts

/**
 * 订单状态
 */
export type OrderStatus = 'pending' | 'paid' | 'shipped' | 'completed' | 'cancelled' | 'refunded'

/**
 * 订单商品项
 */
export interface OrderItem {
  id: string
  orderId: string
  productId: string
  productName: string
  productImage: string
  price: number
  quantity: number
  amount: number
}

/**
 * 订单信息（扩展）
 */
export interface Order {
  id: string
  orderNo: string
  userId: string
  userName?: string
  userPhone?: string
  receiverName?: string
  receiverPhone?: string
  receiverAddress?: string
  items?: OrderItem[]
  totalAmount: number
  payAmount?: number
  status: number | OrderStatus  // 支持数字或字符串状态
  payTime?: string
  payMethod?: string
  // 物流信息（后端字段名）
  deliveryTime?: string        // 发货时间
  logisticsCompany?: string    // 物流公司
  logisticsNumber?: string     // 物流单号
  // 物流信息（兼容旧字段名）
  shipTime?: string
  shipCompany?: string
  shipNo?: string
  createTime: string
  updateTime?: string
  remark?: string

  // 售后关联字段
  refundAmount?: number     // 已退款金额
  refundCount?: number      // 售后申请次数
  hasRefund?: boolean       // 是否有售后记录

  // 确认收货相关字段
  confirmTime?: string      // 确认收货时间
  autoConfirmDays?: number  // 自动确认天数（根据商品类型计算）
  autoConfirmTime?: string  // 自动确认时间（预计）

  // 评价相关字段
  hasReview?: boolean       // 是否已评价
  reviewId?: string         // 评价ID
  reviewTime?: string       // 评价时间

  // 积分相关字段
  pointsEarned?: number     // 订单获得的积分
  pointsStatus?: 'pending' | 'earned' | 'deducted'  // 积分状态
}

/**
 * 订单列表查询参数
 */
export interface OrderListParams {
  pageIndex?: number
  pageSize?: number
  orderNo?: string
  userId?: string
  userKeyword?: string
  status?: OrderStatus
  startTime?: string
  endTime?: string
}

/**
 * 创建订单商品项参数
 */
export interface CreateOrderItemParams {
  productId: string
  productName: string
  productImage: string
  price: number
  quantity: number
}

/**
 * 创建订单参数
 */
export interface CreateOrderParams {
  userId: string
  userName: string
  userPhone: string
  receiverName: string
  receiverPhone: string
  receiverAddress: string
  items: CreateOrderItemParams[]
  remark?: string
}

/**
 * 更新订单参数
 */
export interface UpdateOrderParams {
  id: string
  receiverName?: string
  receiverPhone?: string
  receiverAddress?: string
  remark?: string
}

/**
 * 发货参数
 */
export interface ShipParams {
  orderId: string
  logisticsCompany: string
  logisticsNumber: string
}

/**
 * 创建售后申请参数（扩展）
 */
export interface CreateRefundParams {
  orderId: string
  type: RefundType  // 新增
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
  // 换货商品（新增）
  exchangeItems?: ExchangeItem[]
}

/**
 * 售后类型
 */
export type RefundType = 'refund_only' | 'return_refund' | 'exchange'

/**
 * 售后状态（扩展）
 */
export type RefundStatus =
  | 'pending'    // 待审核
  | 'approved'   // 已通过
  | 'returning'  // 退货中
  | 'received'   // 已收货
  | 'shipped'    // 已发货（换货）
  | 'refunded'   // 已退款
  | 'completed'  // 已完成
  | 'rejected'   // 已拒绝

/**
 * 售后商品项
 */
export interface RefundItem {
  id: string
  refundId: string
  orderItemId: string
  productId: string
  productName: string
  productImage: string
  price: number
  quantity: number
  amount: number
}

/**
 * 换货商品项（新增）
 */
export interface ExchangeItem {
  productId: string
  productName: string
  productImage: string
  price: number
  quantity: number
}

/**
 * 售后信息（扩展）
 */
export interface Refund {
  id: string
  refundNo: string
  orderId: string
  orderNo: string

  // 售后类型（新增）
  type: RefundType

  // 商品信息
  items: RefundItem[]

  // 换货商品（新增，仅换货类型使用）
  exchangeItems?: ExchangeItem[]

  // 金额
  refundAmount: number

  // 原因
  reason: string
  description?: string

  // 状态
  status: RefundStatus

  // 用户退货快递信息（新增）
  returnShipCompany?: string
  returnShipNo?: string
  returnShipTime?: string

  // 商家换货发货快递信息（新增）
  exchangeShipCompany?: string
  exchangeShipNo?: string
  exchangeShipTime?: string

  // 审核信息
  approver?: string
  approveTime?: string
  approveRemark?: string

  // 完成时间
  refundTime?: string
  completeTime?: string

  // 时间戳
  createTime: string
  updateTime: string
}

/**
 * 售后列表查询参数（扩展）
 */
export interface RefundListParams {
  pageIndex?: number
  pageSize?: number
  refundNo?: string
  orderNo?: string
  orderId?: string      // 新增：按订单ID查询
  type?: RefundType
  status?: RefundStatus
  startTime?: string
  endTime?: string
}

/**
 * 审核退款参数
 */
export interface ApproveRefundParams {
  id: string
  remark?: string
}

/**
 * 拒绝退款参数
 */
export interface RejectRefundParams {
  id: string
  remark: string
}

/**
 * 用户填写退货快递参数（新增）
 */
export interface FillReturnShipParams {
  id: string
  shipCompany: string
  shipNo: string
}

/**
 * 商家确认收货参数（新增）
 */
export interface ConfirmReceiveParams {
  id: string
  remark?: string
}

/**
 * 商家换货发货参数（新增）
 */
export interface ExchangeShipParams {
  id: string
  shipCompany: string
  shipNo: string
}

/**
 * 物流轨迹项（新增）
 */
export interface ShipTrack {
  time: string
  status: string
  location?: string
  description: string
}

/**
 * 订单详情中的售后记录（新增）
 */
export interface OrderRefundRecord {
  id: string
  refundNo: string
  type: RefundType
  status: RefundStatus
  refundAmount: number
  createTime: string
}

/**
 * 物流公司常量
 */
export const SHIP_COMPANIES = [
  { value: 'sf', label: '顺丰速运' },
  { value: 'yt', label: '圆通速递' },
  { value: 'zt', label: '中通快递' },
  { value: 'yd', label: '韵达快递' },
  { value: 'ems', label: 'EMS' },
  { value: 'jd', label: '京东物流' },
  { value: 'db', label: '德邦快递' },
  { value: 'other', label: '其他' },
]

/**
 * 售后类型选项（新增）
 */
export const REFUND_TYPES = [
  { value: 'refund_only', label: '仅退款' },
  { value: 'return_refund', label: '退货退款' },
  { value: 'exchange', label: '换货' },
]

/**
 * 售后原因选项（新增）
 */
export const REFUND_REASONS = [
  { value: 'quality', label: '商品质量问题' },
  { value: 'damage', label: '商品破损' },
  { value: 'wrong', label: '发错商品' },
  { value: 'notMatch', label: '商品与描述不符' },
  { value: 'other', label: '其他原因' },
]

// ==================== 评价相关类型 ====================

/**
 * 评价状态
 */
export type ReviewStatus = 'normal' | 'hidden' | 'deleted'

/**
 * 订单评价
 */
export interface OrderReview {
  id: string                      // 评价ID（GUID）
  orderId: string                 // 订单ID
  orderNo: string                 // 订单编号
  productId: string               // 商品ID
  productName: string             // 商品名称（冗余）
  productImage: string            // 商品图片（冗余）
  userId: string                  // 用户ID
  userName: string                // 用户昵称
  userAvatar: string              // 用户头像

  // 商品维度评分
  productQuality: number          // 商品质量 1-5
  descriptionMatch: number        // 描述相符 1-5
  costPerformance: number         // 性价比 1-5

  // 服务维度评分
  shippingSpeed: number           // 发货速度 1-5
  logisticsService: number        // 物流服务 1-5
  customerService: number         // 客服态度 1-5

  // 综合评分（自动计算）
  overallRating: number           // 综合评分 1-5（保留1位小数）

  // 评价内容
  content: string                 // 评价文字
  images: string[]                // 图片URL数组
  videos: string[]                // 视频URL数组
  tags: string[]                  // 标签数组
  isAnonymous: boolean            // 是否匿名

  // 积分奖励
  pointsEarned: number            // 获得积分
  pointsReason: string            // 积分原因描述

  // 状态
  status: ReviewStatus            // 评价状态

  // 时间戳
  createTime: string              // 创建时间
  updateTime: string              // 更新时间
}

/**
 * 评价列表查询参数
 */
export interface ReviewListParams {
  pageIndex?: number
  pageSize?: number
  orderNo?: string
  productId?: string
  userId?: string
  status?: ReviewStatus
  minRating?: number              // 最低评分筛选
  maxRating?: number              // 最高评分筛选
  startTime?: string
  endTime?: string
}

/**
 * 创建评价参数
 */
export interface CreateReviewParams {
  orderId: string
  productId: string
  // 商品维度评分
  productQuality: number
  descriptionMatch: number
  costPerformance: number
  // 服务维度评分
  shippingSpeed: number
  logisticsService: number
  customerService: number
  // 评价内容
  content: string
  images?: string[]
  videos?: string[]
  tags?: string[]
  isAnonymous?: boolean
}

/**
 * 好评标签
 */
export const GOOD_TAGS = [
  { value: 'quality_good', label: '质量好' },
  { value: 'logistics_fast', label: '物流快' },
  { value: 'service_good', label: '服务好' },
  { value: 'cost_effective', label: '性价比高' },
  { value: 'package_good', label: '包装完好' },
]

/**
 * 中评标签
 */
export const NEUTRAL_TAGS = [
  { value: 'logistics_normal', label: '物流一般' },
  { value: 'need_improve', label: '有待改进' },
]

/**
 * 差评标签
 */
export const BAD_TAGS = [
  { value: 'quality_issue', label: '质量问题' },
  { value: 'description_mismatch', label: '描述不符' },
  { value: 'logistics_slow', label: '物流慢' },
  { value: 'service_bad', label: '服务差' },
]

// ==================== 积分相关类型 ====================

/**
 * 积分类型
 */
export type PointsType = 'review' | 'order' | 'exchange' | 'refund' | 'system'

/**
 * 用户积分汇总
 */
export interface UserPoints {
  id: string                      // 记录ID（GUID）
  userId: string                  // 用户ID
  totalPoints: number             // 累计积分
  availablePoints: number         // 可用积分
  usedPoints: number              // 已使用积分
  createTime: string              // 创建时间
  updateTime: string              // 更新时间
}

/**
 * 用户积分记录
 */
export interface UserPointsRecord {
  id: string                      // 记录ID（GUID）
  userId: string                  // 用户ID
  points: number                  // 积分变动（正数为获得，负数为消耗）
  balance: number                 // 变动后余额
  type: PointsType                // 积分类型
  sourceId: string                // 来源ID（订单ID、评价ID等）
  reason: string                  // 原因描述
  createTime: string              // 创建时间
}

/**
 * 积分记录查询参数
 */
export interface PointsRecordParams {
  pageIndex?: number
  pageSize?: number
  userId?: string
  type?: PointsType
  startTime?: string
  endTime?: string
}

/**
 * 积分配置
 */
export interface PointsConfig {
  // 订单积分比例（例如 0.01 表示订单金额的 1%）
  orderPointsRate: number
  // 评价积分规则
  reviewTextPoints: number        // 纯文字评价积分
  reviewImagePoints: number       // 含图片评价积分
  reviewVideoPoints: number       // 含视频评价积分
  reviewFirstPoints: number       // 首次评价额外积分
  reviewImageCountPoints: number  // 晒图奖励（3张以上）
  // 积分上限
  orderPointsMax: number          // 单订单积分上限
  dailyPointsMax: number          // 每日积分上限
}

// ==================== 物流轨迹相关类型 ====================

/**
 * 物流轨迹项
 */
export interface ShipTrackItem {
  time: string                    // 时间
  status: string                  // 状态描述
  location?: string               // 地点
  description: string             // 详细描述
}

/**
 * 物流轨迹响应
 */
export interface ShipTrackResponse {
  company: string                 // 快递公司
  shipNo: string                  // 快递单号
  tracks: ShipTrackItem[]         // 轨迹列表
  isSigned: boolean               // 是否已签收
}

// ==================== 确认收货相关类型 ====================

/**
 * 商品类型自动确认天数配置
 */
export interface ProductTypeAutoConfirm {
  productType: string             // 商品类型
  autoConfirmDays: number         // 自动确认天数
  description: string             // 说明
}

/**
 * 商品类型自动确认天数配置列表
 */
export const PRODUCT_TYPE_AUTO_CONFIRM: ProductTypeAutoConfirm[] = [
  { productType: 'flower', autoConfirmDays: 3, description: '鲜花类商品' },
  { productType: 'gift', autoConfirmDays: 5, description: '礼品类商品' },
  { productType: 'general', autoConfirmDays: 7, description: '一般商品' },
  { productType: 'digital', autoConfirmDays: 15, description: '数码类商品' },
]