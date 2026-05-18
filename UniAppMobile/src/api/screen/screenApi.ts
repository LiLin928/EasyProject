// api/screen/screenApi.ts

import { get, post } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Screen, ScreenQueryParams, PageResponse } from '@/types'

/** 获取大屏列表 */
export function getScreenList(params: ScreenQueryParams): Promise<PageResponse<Screen>> {
  return post<PageResponse<Screen>>(API_PATHS.SCREEN_LIST, params)
}

/** 获取大屏详情 */
export function getScreenDetail(id: string): Promise<Screen> {
  return get<Screen>(`${API_PATHS.SCREEN_DETAIL}/${id}`)
}

/** 创建大屏 */
export function createScreen(data: Partial<Screen>): Promise<{ id: string }> {
  return post<{ id: string }>('/api/screen/create', data)
}

/** 更新大屏 */
export function updateScreen(data: Screen): Promise<number> {
  return post<number>('/api/screen/update', data)
}

/** 删除大屏 */
export function deleteScreen(ids: string[]): Promise<number> {
  return post<number>('/api/screen/delete', { ids })
}

/** 复制大屏 */
export function copyScreen(id: string): Promise<Screen> {
  return post<Screen>(`/api/screen/copy/${id}`)
}

/** 发布大屏 */
export function publishScreen(screenId: string): Promise<{ publishId: string; url: string }> {
  return post<{ publishId: string; url: string }>('/api/screen/publish', { screenId })
}

/** 下架大屏 */
export function unpublishScreen(screenId: string): Promise<number> {
  return post<number>('/api/screen/unpublish', { screenId })
}

/** 获取发布大屏配置 */
export function getPublishedScreen(publishId: string): Promise<Screen> {
  return get<Screen>(`/api/screen/published/${publishId}`)
}