import type { CommonStatus } from './enums'

// 用户信息（与后端 UserDto 对应）
export interface UserInfo {
  id: string
  userName: string
  realName?: string
  avatar?: string
  email?: string
  phone?: string
  status: number
  createTime?: string
  roles?: string[]
  roleIds?: string[]
  // 前端扩展字段
  permissions?: string[]
}

// 兼容旧字段名的别名（可选）
export interface UserInfoLegacy {
  id: string
  username: string      // 兼容 userName
  nickname: string      // 兼容 realName
  avatar: string
  email: string
  phone: string
  roles: string[]
  permissions: string[]
  status?: CommonStatus
  createTime?: string
  updateTime?: string
}

// 登录请求参数
export interface LoginParams {
  username: string
  password: string
  rememberMe?: boolean
}

// 用户列表查询参数
export interface UserListParams {
  pageIndex: number
  pageSize: number
  keyword?: string      // 改为 keyword 与后端一致
  status?: CommonStatus
}

// 创建用户参数
export interface CreateUserParams {
  userName: string
  password: string
  realName?: string
  email?: string
  phone?: string
  avatar?: string
  status?: number
  roleIds?: string[]
}

// 更新用户参数
export interface UpdateUserParams {
  id: string
  realName?: string
  email?: string
  phone?: string
  avatar?: string
  status?: number
  roleIds?: string[]
}

// Re-export RoleInfo from role.d.ts
export type { RoleInfo } from './role'