// 文件: PCWeb/src/api/basic/desktopWidgetApi.ts

import { get, post, del } from '@/utils/request'
import type {
  DesktopWidget,
  QueryDesktopWidgetParams,
  AddDesktopWidgetParams,
  UpdateDesktopWidgetParams,
} from '@/types'

/**
 * 获取组件列表(分页)
 */
export function getDesktopWidgetList(params: QueryDesktopWidgetParams) {
  return get<{ list: DesktopWidget[]; total: number; pageIndex: number; pageSize: number }>(
    '/api/desktop/widget/list',
    params
  )
}

/**
 * 获取组件详情
 * @param id 组件ID
 */
export function getDesktopWidgetDetail(id: string) {
  return get<DesktopWidget>(`/api/desktop/widget/detail/${id}`)
}

/**
 * 创建组件
 * @param data 组件数据
 */
export function addDesktopWidget(data: AddDesktopWidgetParams) {
  return post<string>('/api/desktop/widget/add', data)
}

/**
 * 更新组件
 * @param data 组件数据
 */
export function updateDesktopWidget(data: UpdateDesktopWidgetParams) {
  return post<boolean>('/api/desktop/widget/update', data)
}

/**
 * 删除组件
 * @param id 组件ID
 */
export function deleteDesktopWidget(id: string) {
  return del<boolean>(`/api/desktop/widget/delete/${id}`)
}

/**
 * 获取所有启用的组件列表
 */
export function getEnabledWidgetList() {
  return get<DesktopWidget[]>('/api/desktop/widget/enabled-list')
}