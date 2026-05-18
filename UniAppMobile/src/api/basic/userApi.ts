// api/basic/userApi.ts

import { get, post, del } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { User, UserQueryParams, PageResponse } from '@/types'

/** 获取用户列表 */
export function getUserList(params: UserQueryParams): Promise<PageResponse<User>> {
  return get<PageResponse<User>>(API_PATHS.USER_LIST, params)
}

/** 获取用户详情 */
export function getUserDetail(id: string): Promise<User> {
  return get<User>(`/api/user/detail/${id}`)
}

/** 创建用户 */
export function createUser(data: Partial<User>): Promise<{ id: string }> {
  return post<{ id: string }>('/api/user/add', data)
}

/** 更新用户 */
export function updateUser(data: User): Promise<number> {
  return post<number>('/api/user/update', data)
}

/** 删除用户 */
export function deleteUser(id: string): Promise<number> {
  return del<number>(`/api/user/delete/${id}`)
}

/** 启用用户 */
export function enableUser(id: string): Promise<number> {
  return post<number>(`/api/user/enable/${id}`)
}

/** 禁用用户 */
export function disableUser(id: string): Promise<number> {
  return post<number>(`/api/user/disable/${id}`)
}

/** 重置密码 */
export function resetPassword(id: string): Promise<number> {
  return post<number>(`/api/user/reset-password/${id}`)
}