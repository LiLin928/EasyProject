import type { UserInfo } from './user'

// API 响应类型
export interface ApiResponse<T = any> {
  code: number
  message: string
  data: T
}

// 分页响应
export interface PageResponse<T = any> {
  list: T[]
  total: number
  pageIndex: number
  pageSize: number
}

// 分页请求参数
export interface PageParams {
  pageIndex: number
  pageSize: number
}

// 登录响应（与后端 LoginResultDto 对应）
export interface LoginResponse {
  accessToken: string
  refreshToken: string
  expiresIn: number
  tokenType: string
}

// 状态码常量
export const SUCCESS_CODE = 200
export const ERROR_CODE = 500
export const UNAUTHORIZED_CODE = 401
export const FORBIDDEN_CODE = 403