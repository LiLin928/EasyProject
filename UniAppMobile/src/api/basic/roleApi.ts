// api/basic/roleApi.ts

import { get, post, del } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Role, RoleQueryParams, PageResponse } from '@/types'

/** 获取角色列表 */
export function getRoleList(params: RoleQueryParams): Promise<PageResponse<Role>> {
  return get<PageResponse<Role>>(API_PATHS.ROLE_LIST, params)
}

/** 获取角色详情 */
export function getRoleDetail(id: string): Promise<Role> {
  return get<Role>(`/api/role/detail/${id}`)
}

/** 创建角色 */
export function createRole(data: Partial<Role>): Promise<{ id: string }> {
  return post<{ id: string }>('/api/role/add', data)
}

/** 更新角色 */
export function updateRole(data: Role): Promise<number> {
  return post<number>('/api/role/update', data)
}

/** 删除角色 */
export function deleteRole(id: string): Promise<number> {
  return del<number>(`/api/role/delete/${id}`)
}

/** 获取角色的权限列表 */
export function getRolePermissions(id: string): Promise<string[]> {
  return get<string[]>(`/api/role/permissions/${id}`)
}

/** 分配权限给角色 */
export function assignPermissions(id: string, permissionIds: string[]): Promise<number> {
  return post<number>(`/api/role/permissions/${id}`, { permissionIds })
}

/** 获取角色的用户列表 */
export function getRoleUsers(id: string): Promise<{ id: string; userName: string; realName: string }[]> {
  return get<{ id: string; userName: string; realName: string }[]>(`/api/role/users/${id}`)
}