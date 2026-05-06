import { get, post, put } from '@/utils/request'
import type { UserInfo, UserListParams, CreateUserParams, UpdateUserParams, RoleInfo } from '@/types'
import type { CommonStatus } from '@/types/enums'

/**
 * 更新用户资料参数
 */
export interface UpdateProfileParams {
  nickname?: string
  email?: string
  phone?: string
}

/**
 * 修改密码参数
 */
export interface ChangePasswordParams {
  oldPassword: string
  newPassword: string
  confirmPassword: string
}

/**
 * 绑定邮箱参数
 */
export interface BindEmailParams {
  email: string
  password: string
}

/**
 * 绑定手机参数
 */
export interface BindPhoneParams {
  phone: string
  password: string
}

/**
 * 更新用户资料
 */
export function updateProfile(data: UpdateProfileParams) {
  return post<UserInfo>('/api/user/updateProfile', data)
}

/**
 * 修改密码
 */
export function changePassword(data: ChangePasswordParams) {
  return post<{ success: boolean; message: string }>('/api/user/changePassword', data)
}

/**
 * 上传头像
 */
export function uploadAvatar(file: File) {
  const formData = new FormData()
  formData.append('file', file)
  return post<{ url: string }>('/api/user/uploadAvatar', formData)
}

/**
 * 绑定邮箱
 */
export function bindEmail(data: BindEmailParams) {
  return post<{ success: boolean; message: string }>('/api/user/bindEmail', data)
}

/**
 * 绑定手机
 */
export function bindPhone(data: BindPhoneParams) {
  return post<{ success: boolean; message: string }>('/api/user/bindPhone', data)
}

// ==================== 用户管理 API ====================

/**
 * 获取用户列表
 */
export function getUserList(params: UserListParams) {
  return post<{ list: UserInfo[]; total: number }>('/api/user/list', params)
}

/**
 * 获取用户详情
 * @param id 用户ID (GUID)
 */
export function getUserDetail(id: string) {
  return get<UserInfo>(`/api/user/detail/${id}`)
}

/**
 * 创建用户
 */
export function createUser(data: CreateUserParams) {
  return post<{ id: string }>('/api/user/add', data)
}

/**
 * 更新用户
 */
export function updateUser(data: UpdateUserParams) {
  return put<{ success: boolean }>('/api/user/update', data)
}

/**
 * 删除用户（POST 方式）
 * @param id 用户ID (GUID)
 */
export function deleteUser(id: string) {
  return post<number>('/api/user/delete', id)
}

/**
 * 批量删除用户
 * @param ids 用户ID列表 (GUID)
 */
export function deleteUserBatch(ids: string[]) {
  return post<number>('/api/user/deleteBatch', ids)
}

/**
 * 重置用户密码
 */
export function resetUserPassword(id: string, newPassword: string) {
  return post<{ success: boolean }>('/api/user/resetPassword', { id, newPassword })
}

/**
 * 更新用户状态
 */
export function updateUserStatus(id: string, status: CommonStatus) {
  return post<{ success: boolean }>('/api/user/updateStatus', { id, status })
}

/**
 * 获取角色列表
 */
export function getRoleList(params?: { pageIndex?: number; pageSize?: number; roleName?: string; status?: CommonStatus }) {
  return post<{ list: RoleInfo[]; total: number }>('/api/role/list', params)
}