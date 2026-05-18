// api/basic/menuApi.ts

import { get, post, put } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Menu } from '@/types'

/** 获取菜单树 */
export function getMenuTree(): Promise<Menu[]> {
  return post<Menu[]>(API_PATHS.MENU_LIST)
}

/** 获取用户菜单 */
export function getUserMenus(): Promise<Menu[]> {
  return get<Menu[]>(API_PATHS.MENU_USER_MENU)
}

/** 获取菜单详情 */
export function getMenuDetail(id: string): Promise<Menu> {
  return get<Menu>(`${API_PATHS.MENU_DETAIL}/${id}`)
}

/** 创建菜单 */
export function createMenu(data: Partial<Menu>): Promise<{ id: string }> {
  return post<{ id: string }>('/api/menu/add', data)
}

/** 更新菜单 */
export function updateMenu(data: Menu): Promise<number> {
  return put<number>('/api/menu/update', data)
}

/** 删除菜单 */
export function deleteMenu(id: string): Promise<number> {
  return post<number>('/api/menu/delete', id)
}

/** 分配角色权限 */
export function assignRoles(menuId: string, roleIds: string[]): Promise<number> {
  return post<number>('/api/menu/assign-roles', { menuId, roleIds })
}