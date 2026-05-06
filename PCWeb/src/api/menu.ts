// 菜单相关 API

import { get, post, put } from '@/utils/request'
import type { MockMenu, CreateMenuParams, UpdateMenuParams } from '@/types/menu'

/**
 * 获取菜单树（全部）
 */
export function getMenuTree() {
  return post<MockMenu[]>('/api/menu/list')
}

/**
 * 获取菜单详情
 * @param id 菜单ID (GUID)
 */
export function getMenuDetail(id: string) {
  return get<MockMenu>(`/api/menu/detail/${id}`)
}

/**
 * 新增菜单
 */
export function createMenu(data: CreateMenuParams) {
  return post<string>('/api/menu/add', data)
}

/**
 * 编辑菜单
 */
export function updateMenu(data: UpdateMenuParams) {
  return put<number>('/api/menu/update', data)
}

/**
 * 删除菜单
 * @param id 菜单ID (GUID)
 */
export function deleteMenu(id: string) {
  return post<number>('/api/menu/delete', id)
}

// 别名导出，保持向后兼容
export const getMenuList = getMenuTree