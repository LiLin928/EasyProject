// api/basic/userApi.ts

import { get, post, put } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { User, UserQueryParams, PageResponse } from '@/types'

/** 获取用户列表 */
export function getUserList(params: UserQueryParams): Promise<PageResponse<User>> {
  return post<PageResponse<User>>(API_PATHS.USER_LIST, params)
}

/** 获取用户详情 */
export function getUserDetail(id: string): Promise<User> {
  return get<User>(`${API_PATHS.USER_DETAIL}/${id}`)
}

/** 创建用户 */
export function createUser(data: Partial<User>): Promise<{ id: string }> {
  return post<{ id: string }>(API_PATHS.USER_ADD, data)
}

/** 更新用户 */
export function updateUser(data: User): Promise<number> {
  return put<number>(API_PATHS.USER_UPDATE, data)
}

/** 删除用户 */
export function deleteUser(id: string): Promise<number> {
  return post<number>(API_PATHS.USER_DELETE, id)
}

/** 批量删除用户 */
export function deleteUserBatch(ids: string[]): Promise<number> {
  return post<number>('/api/user/deleteBatch', ids)
}

/** 更新用户状态 */
export function updateUserStatus(id: string, status: number): Promise<number> {
  return post<number>('/api/user/updateStatus', { id, status })
}

/** 启用用户 */
export function enableUser(id: string): Promise<number> {
  return updateUserStatus(id, 1)
}

/** 禁用用户 */
export function disableUser(id: string): Promise<number> {
  return updateUserStatus(id, 0)
}

/** 重置密码 */
export function resetPassword(id: string, newPassword: string): Promise<number> {
  return post<number>('/api/user/resetPassword', { id, newPassword })
}