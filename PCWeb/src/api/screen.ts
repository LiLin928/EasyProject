// 大屏管理 API

import { get, post } from '@/utils/request'
import type { ScreenConfig, ScreenListParams, CreateScreenParams, UpdateScreenParams, ShareScreenParams, DatasourceOption, ScreenPublishRecord } from '@/types'

/**
 * 获取大屏列表
 */
export function getScreenList(params: ScreenListParams) {
  return post<{ list: ScreenConfig[]; total: number }>('/api/screen/list', params)
}

/**
 * 获取大屏详情
 */
export function getScreenDetail(id: string) {
  return get<ScreenConfig>(`/api/screen/detail/${id}`)
}

/**
 * 创建大屏
 */
export function createScreen(data: CreateScreenParams) {
  return post<{ id: string }>('/api/screen/create', data)
}

/**
 * 更新大屏
 */
export function updateScreen(data: UpdateScreenParams) {
  return post<{ success: boolean }>('/api/screen/update', data)
}

/**
 * 删除大屏
 */
export function deleteScreen(ids: string | string[]) {
  const idsArray = Array.isArray(ids) ? ids : [ids]
  return post<{ success: boolean }>('/api/screen/delete', { ids: idsArray })
}

/**
 * 复制大屏
 */
export function copyScreen(id: string) {
  return post<ScreenConfig>(`/api/screen/copy/${id}`)
}

/**
 * 更新分享配置
 */
export function updateScreenShare(data: ShareScreenParams) {
  return post<{ success: boolean }>('/api/screen/share', data)
}

/**
 * 获取可分享的用户列表
 */
export function getShareableUsers(id: string) {
  return post<{ list: { id: string; name: string; avatar?: string }[] }>(`/api/screen/shareable-users/${id}`)
}

/**
 * 获取可分享的角色列表
 */
export function getShareableRoles(id: string) {
  return post<{ list: { id: string; name: string }[] }>(`/api/screen/shareable-roles/${id}`)
}

// ===================== 数据源相关 API =====================

/**
 * 获取可用的数据源列表（用于组件数据源选择）
 */
export function getAvailableDatasources() {
  return post<DatasourceOption[]>('/api/screen/datasources')
}

/**
 * 执行SQL查询（预览数据）
 */
export function executeSqlQuery(data: {
  datasourceId: string
  sql: string
  params?: Record<string, any>
}) {
  return post<{ data: any[]; columns: { name: string; type: string }[] }>('/api/screen/execute-sql', data)
}

/**
 * 验证SQL语法
 */
export function validateSql(data: { datasourceId: string; sql: string }) {
  return post<{ valid: boolean; message?: string; columns?: { name: string; type: string }[] }>('/api/screen/validate-sql', data)
}

// ===================== 发布相关 API =====================

/**
 * 发布大屏
 */
export function publishScreen(id: string) {
  return post<{ publishId: string; publishUrl: string }>('/api/screen/publish', { screenId: id })
}

/**
 * 获取发布后的大屏数据
 */
export function getPublishedScreen(publishId: string) {
  return get<ScreenConfig>(`/api/screen/published/${publishId}`)
}

/**
 * 取消发布
 */
export function unpublishScreen(id: string) {
  return post<{ success: boolean }>('/api/screen/unpublish', { screenId: id })
}

/**
 * 获取大屏发布信息
 */
export function getScreenPublishInfo(id: string) {
  return get<{ published: boolean; publishId?: string; publishUrl?: string; publishedAt?: string; viewCount?: number }>(`/api/screen/publish-info/${id}`)
}

/**
 * 获取大屏发布列表
 */
export function getScreenPublishList(params: { pageIndex: number; pageSize: number; name?: string }) {
  return post<{ list: ScreenPublishRecord[]; total: number }>('/api/screen/publish-list', params)
}

/**
 * 下架发布的大屏（通过发布ID）
 */
export function unpublishScreenByPublishId(publishId: string) {
  return post<{ success: boolean }>('/api/screen/unpublish', { publishId })
}

/**
 * 上架发布的大屏
 */
export function republishScreen(publishId: string) {
  return post<{ success: boolean }>('/api/screen/republish', { publishId })
}

// ===================== 验证相关 API =====================

/**
 * 验证大屏配置
 */
export function validateScreenConfig(components: string) {
  return post<{ valid: boolean; errors: string[] }>('/api/screen/validate-config', { components })
}

/**
 * 验证单个组件
 */
export function validateScreenComponent(component: object) {
  return post<{ valid: boolean; errors: string[] }>('/api/screen/validate-component', { component })
}

// ===================== 组件管理 API =====================

/**
 * 组件创建参数
 */
export interface CreateComponentParams {
  componentType: string
  name: string
  positionX: number
  positionY: number
  width: number
  height: number
  config?: object
  dataSource?: object
  styleConfig?: object
  dataBinding?: object
}

/**
 * 组件更新参数
 */
export interface UpdateComponentParams {
  id: string
  name?: string
  config?: object
  dataSource?: object
  styleConfig?: object
  dataBinding?: object
  positionX?: number
  positionY?: number
  width?: number
  height?: number
}

/**
 * 添加组件到大屏
 */
export function addScreenComponent(screenId: string, data: CreateComponentParams) {
  return post<{ id: string }>('/api/screen/component/add', { screenId, ...data })
}

/**
 * 更新大屏组件
 */
export function updateScreenComponent(data: UpdateComponentParams) {
  return post<{ success: boolean }>('/api/screen/component/update', data)
}

/**
 * 删除大屏组件
 */
export function deleteScreenComponent(id: string) {
  return post<{ success: boolean }>(`/api/screen/component/delete/${id}`)
}