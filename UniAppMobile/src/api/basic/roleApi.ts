// api/basic/roleApi.ts

import { get, post, put } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Role, RoleQueryParams, PageResponse } from '@/types'

/** 获取角色列表 */
export function getRoleList(params: RoleQueryParams): Promise<PageResponse<Role>> {
  return post<PageResponse<Role>>(API_PATHS.ROLE_LIST, params)
}

/** 获取角色详情 */
export function getRoleDetail(id: string): Promise<Role> {
  return get<Role>(`${API_PATHS.ROLE_DETAIL}/${id}`)
}

/** 创建角色 */
export function createRole(data: Partial<Role>): Promise<{ id: string }> {
  return post<{ id: string }>(API_PATHS.ROLE_ADD, data)
}

/** 更新角色 */
export function updateRole(data: Role): Promise<number> {
  return put<number>(API_PATHS.ROLE_UPDATE, data)
}

/** 删除角色 */
export function deleteRole(id: string): Promise<number> {
  return post<number>(API_PATHS.ROLE_DELETE, id)
}

/** 批量删除角色 */
export function deleteRoleBatch(ids: string[]): Promise<number> {
  return post<number>('/api/role/delete-batch', ids)
}

/** 更新角色状态 */
export function updateRoleStatus(id: string, status: number): Promise<number> {
  return post<number>('/api/role/updateStatus', { id, status })
}

/** 启用角色 */
export function enableRole(id: string): Promise<number> {
  return updateRoleStatus(id, 1)
}

/** 禁用角色 */
export function disableRole(id: string): Promise<number> {
  return updateRoleStatus(id, 0)
}

/** 分配用户到角色 */
export function assignUsers(roleId: string, userIds: string[]): Promise<number> {
  return post<number>('/api/role/assign-users', { roleId, userIds })
}

/** 获取角色的菜单ID列表 */
export function getRoleMenuIds(id: string): Promise<string[]> {
  return post<string[]>(`/api/role/menu/${id}`)
}

/** 更新角色的菜单权限 */
export function updateRoleMenu(id: string, menuIds: string[]): Promise<number> {
  return post<number>('/api/role/updateMenu', { id, menuIds })
}