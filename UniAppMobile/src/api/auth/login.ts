// api/auth/login.ts

import { get, post } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { LoginParams, LoginResponse, UserInfo } from '@/types'

/** 登录 */
export function login(params: LoginParams): Promise<LoginResponse> {
  return post<LoginResponse>(API_PATHS.AUTH_LOGIN, params)
}

/** 退出登录 */
export function logout(): Promise<void> {
  return post<void>(API_PATHS.AUTH_LOGOUT)
}

/** 获取用户信息 */
export function getUserInfo(): Promise<UserInfo> {
  return get<UserInfo>(API_PATHS.USER_INFO)
}