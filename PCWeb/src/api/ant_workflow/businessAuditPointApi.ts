import { get, post, del } from '@/utils/request'
import type {
  BusinessAuditPoint,
  BusinessAuditPointQueryParams,
  CreateBusinessAuditPointParams,
  UpdateBusinessAuditPointParams,
} from '@/types/businessAuditPoint'

/**
 * 获取业务审核点列表
 * @param params 查询参数
 * @returns 分页业务审核点列表
 */
export function getBusinessAuditPointList(params: BusinessAuditPointQueryParams) {
  return post<{ list: BusinessAuditPoint[]; total: number }>('/api/business-audit-point/list', params)
}

/**
 * 获取业务审核点详情
 * @param id 业务审核点ID (GUID)
 */
export function getBusinessAuditPointDetail(id: string) {
  return get<BusinessAuditPoint>(`/api/business-audit-point/detail/${id}`)
}

/**
 * 创建业务审核点
 * @param data 业务审核点数据
 */
export function createBusinessAuditPoint(data: CreateBusinessAuditPointParams) {
  return post<{ id: string }>('/api/business-audit-point/add', data)
}

/**
 * 更新业务审核点
 * @param data 业务审核点数据（含id）
 */
export function updateBusinessAuditPoint(data: UpdateBusinessAuditPointParams) {
  return post<number>('/api/business-audit-point/update', data)
}

/**
 * 删除业务审核点
 * @param id 业务审核点ID (GUID)
 */
export function deleteBusinessAuditPoint(id: string) {
  return del<number>(`/api/business-audit-point/delete/${id}`)
}

/**
 * 根据编码获取业务审核点
 * @param code 审核点编码
 */
export function getBusinessAuditPointByCode(code: string) {
  return get<BusinessAuditPoint>(`/api/business-audit-point/by-code/${code}`)
}

/**
 * 根据数据表名获取业务审核点列表
 * @param tableName 数据表名
 */
export function getBusinessAuditPointByTable(tableName: string) {
  return get<BusinessAuditPoint[]>(`/api/business-audit-point/by-table/${tableName}`)
}