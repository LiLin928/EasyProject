// api/basic/menuApi.ts

import { get, post, del } from '@/utils/request'
import type { Menu } from '@/types'

const BASE_URL = '/api/menu'

/** 获取菜单树 */
export function getMenuTree(): Promise<Menu[]> {
  return get<Menu[]>(`${BASE_URL}/tree`)
}

/** 获取用户菜单 */
export function getUserMenus(): Promise<Menu[]> {
  return get<Menu[]>(`${BASE_URL}/user`)
}

/** 获取菜单详情 */
export function getMenuDetail(id: string): Promise<Menu> {
  return get<Menu>(`${BASE_URL}/${id}`)
}

/** 创建菜单 */
export function createMenu(data: Partial<Menu>): Promise<{ id: string }> {
  return post<{ id: string }>(BASE_URL, data)
}

/** 更新菜单 */
export function updateMenu(data: Menu): Promise<number> {
  return post<number>(BASE_URL, data)
}

/** 删除菜单 */
export function deleteMenu(id: string): Promise<number> {
  return del<number>(`${BASE_URL}/${id}`)
}