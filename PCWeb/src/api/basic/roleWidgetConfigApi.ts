// 文件: PCWeb/src/api/basic/roleWidgetConfigApi.ts

import { get, post } from '@/utils/request'
import type {
  RoleWidgetConfig,
  SaveRoleWidgetConfigParams,
  AvailableWidget,
} from '@/types'

/**
 * 获取角色的组件配置列表
 * @param roleId 角色ID
 */
export function getRoleWidgetConfigList(roleId: string) {
  return get<RoleWidgetConfig[]>(`/api/desktop/role-config/list/${roleId}`)
}

/**
 * 保存角色组件配置
 * @param data 配置数据
 */
export function saveRoleWidgetConfig(data: SaveRoleWidgetConfigParams) {
  return post<boolean>('/api/desktop/role-config/save', data)
}

/**
 * 获取角色可用的组件列表
 * @param roleId 角色ID
 */
export function getAvailableWidgets(roleId: string) {
  return get<AvailableWidget[]>(`/api/desktop/role-config/available-widgets/${roleId}`)
}