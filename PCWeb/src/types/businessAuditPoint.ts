// src/types/businessAuditPoint.ts

// ===================== 业务审核点类型 =====================

/**
 * 业务审核点信息
 */
export interface BusinessAuditPoint {
  id: string // GUID 主键
  code: string // 审核点编码
  name: string // 审核点名称
  category?: string // 审核点分类（可选）
  workflowId: string // 关联工作流ID
  workflowName?: string // 工作流名称
  tableName: string // 关联数据表名
  primaryKeyField: string // 主键字段名（默认 "Id"）
  statusField: string // 状态字段名
  auditStatusValue: number // 待审核状态值
  passStatusValue: number // 审核通过状态值
  rejectStatusValue: number // 审核拒绝状态值
  withdrawStatusValue: number // 撤回状态值
  auditPageUrl?: string // 审核页面URL
  titleTemplate?: string // 标题模板
  codeField?: string // 编码字段名
  passCallbackApi?: string // 通过回调API
  rejectCallbackApi?: string // 拒绝回调API
  status: 0 | 1 // 状态：0-禁用, 1-启用
  sort: number // 排序
  remark?: string // 备注
  createTime?: string // 创建时间
  updateTime?: string // 更新时间
}

/**
 * 业务审核点查询参数
 */
export interface BusinessAuditPointQueryParams {
  pageIndex?: number
  pageSize?: number
  code?: string
  name?: string
  category?: string
  tableName?: string
  status?: 0 | 1
}

/**
 * 创建业务审核点参数
 */
export interface CreateBusinessAuditPointParams {
  code: string // 审核点编码
  name: string // 审核点名称
  category?: string // 审核点分类（可选）
  workflowId: string // 关联工作流ID
  tableName: string // 关联数据表名
  primaryKeyField?: string // 主键字段名（默认 "Id"）
  statusField: string // 状态字段名
  auditStatusValue: number // 待审核状态值
  passStatusValue: number // 审核通过状态值
  rejectStatusValue: number // 审核拒绝状态值
  withdrawStatusValue: number // 撤回状态值
  auditPageUrl?: string // 审核页面URL
  titleTemplate?: string // 标题模板
  codeField?: string // 编码字段名
  passCallbackApi?: string // 通过回调API
  rejectCallbackApi?: string // 拒绝回调API
  status?: 0 | 1 // 状态
  sort?: number // 排序
  remark?: string // 备注
}

/**
 * 更新业务审核点参数
 */
export interface UpdateBusinessAuditPointParams extends CreateBusinessAuditPointParams {
  id: string // GUID 主键
}

// ===================== 商品审核状态 =====================

/**
 * 商品审核状态枚举
 */
export enum ProductAuditStatus {
  /** 待提交 */
  WaitSubmit = 0,
  /** 待审核 */
  Pending = 1,
  /** 已拒绝 */
  Rejected = 2,
  /** 已通过 */
  Passed = 3,
  /** 已撤回 */
  Withdrawn = 4,
}

/**
 * 商品审核状态标签映射
 */
export const ProductAuditStatusLabel: Record<ProductAuditStatus, string> = {
  [ProductAuditStatus.WaitSubmit]: '待提交',
  [ProductAuditStatus.Pending]: '待审核',
  [ProductAuditStatus.Rejected]: '已拒绝',
  [ProductAuditStatus.Passed]: '已通过',
  [ProductAuditStatus.Withdrawn]: '已撤回',
}

/**
 * 商品审核状态对应的 Element Plus Tag 类型
 */
export const ProductAuditStatusType: Record<ProductAuditStatus, string> = {
  [ProductAuditStatus.WaitSubmit]: 'info',
  [ProductAuditStatus.Pending]: 'warning',
  [ProductAuditStatus.Rejected]: 'danger',
  [ProductAuditStatus.Passed]: 'success',
  [ProductAuditStatus.Withdrawn]: 'info',
}

// ===================== 审核操作参数 =====================

/**
 * 提交审核参数
 */
export interface SubmitAuditParams {
  productId: string // 商品ID (GUID)
  auditPointCode: string // 审核点编码
}

/**
 * 审核拒绝参数
 */
export interface AuditRejectParams {
  rejectReason: string // 拒绝原因
}