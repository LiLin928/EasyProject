// 登录认证 API

import { get, post } from '@/utils/request'
import type { LoginParams, LoginResponse, UserInfo } from '@/types'

/**
 * 登录
 */
export function login(params: LoginParams) {
  return post<LoginResponse>('/api/auth/login', params)
}

/**
 * 退出登录
 */
export function logout() {
  return post('/api/auth/logout')
}

/**
 * 获取用户信息
 */
export function getUserInfo() {
  return get<UserInfo>('/api/user/info')
}