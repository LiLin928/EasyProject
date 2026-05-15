// 文件: PCWeb/src/api/basic/userWidgetConfigApi.ts

import { get, post } from '@/utils/request'
import type {
  UserDesktopDto,
  SaveUserWidgetConfigParams,
  WidgetDataResponse,
} from '@/types/desktopWidget'

/**
 * 获取用户桌面配置
 */
export function getUserDesktop() {
  return get<UserDesktopDto>('/api/desktop/user-config/my')
}

/**
 * 保存用户桌面配置
 * @param data 配置数据
 */
export function saveUserWidgetConfig(data: SaveUserWidgetConfigParams) {
  return post<boolean>('/api/desktop/user-config/save', data)
}

/**
 * 重置用户桌面配置（恢复为角色默认配置）
 */
export function resetUserDesktop() {
  return post<boolean>('/api/desktop/user-config/reset')
}

/**
 * 获取组件数据
 * @param widgetId 组件ID
 */
export function getWidgetData(widgetId: string) {
  return get<WidgetDataResponse>(`/api/desktop/user-config/data/${widgetId}`)
}