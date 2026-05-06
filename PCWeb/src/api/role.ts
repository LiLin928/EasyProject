// src/api/role.ts
import { get, post, put } from '@/utils/request'
import type { RoleInfo, RoleListParams, CreateRoleParams, UpdateRoleParams } from '@/types/role'

/**
 * 获取角色列表
 */
export function getRoleList(params: RoleListParams) {
  return post<{ list: RoleInfo[]; total: number }>('/api/role/list', params)
}

/**
 * 获取角色详情
 * @param id 角色ID (GUID)
 */
export function getRoleDetail(id: string) {
  return get<RoleInfo>(`/api/role/detail/${id}`)
}

/**
 * 创建角色
 * @returns 新创建的角色ID
 */
export function createRole(data: CreateRoleParams) {
  return post<string>('/api/role/add', data)
}

/**
 * 更新角色
 */
export function updateRole(data: UpdateRoleParams) {
  return put<RoleInfo>('/api/role/update', data)
}

/**
 * 删除角色
 * @param id 角色ID (GUID)
 */
export function deleteRole(id: string) {
  return post<number>('/api/role/delete', id)
}

/**
 * 批量删除角色
 * @param ids 角色ID列表 (GUID数组)
 */
export function deleteRoleBatch(ids: string[]) {
  return post<number>('/api/role/delete-batch', ids)
}

/**
 * 获取角色菜单权限ID列表
 * @param id 角色ID (GUID)
 */
export function getRoleMenuIds(id: string) {
  return post<string[]>(`/api/role/menu/${id}`)
}

/**
 * 更新角色菜单权限
 */
export function updateRoleMenu(id: string, menuIds: string[]) {
  return post<{ success: boolean }>('/api/role/updateMenu', { id, menuIds })
}