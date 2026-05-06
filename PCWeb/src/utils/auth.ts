// Token 管理

import { getStorage, setStorage, removeStorage } from './storage'
import type { UserInfo } from '@/types'

const ACCESS_TOKEN_KEY = 'accessToken'
const REFRESH_TOKEN_KEY = 'refreshToken'
const USER_KEY = 'user'

/**
 * 获取 Access Token
 */
export function getToken(): string | null {
  return getStorage<string>(ACCESS_TOKEN_KEY)
}

/**
 * 设置 Access Token
 */
export function setToken(token: string): void {
  setStorage(ACCESS_TOKEN_KEY, token)
}

/**
 * 移除 Access Token
 */
export function removeToken(): void {
  removeStorage(ACCESS_TOKEN_KEY)
}

/**
 * 获取 Refresh Token
 */
export function getRefreshToken(): string | null {
  return getStorage<string>(REFRESH_TOKEN_KEY)
}

/**
 * 设置 Refresh Token
 */
export function setRefreshToken(token: string): void {
  setStorage(REFRESH_TOKEN_KEY, token)
}

/**
 * 移除 Refresh Token
 */
export function removeRefreshToken(): void {
  removeStorage(REFRESH_TOKEN_KEY)
}

/**
 * 获取用户信息
 */
export function getUserInfo(): UserInfo | null {
  return getStorage<UserInfo>(USER_KEY)
}

/**
 * 设置用户信息
 */
export function setUserInfo(user: UserInfo): void {
  setStorage(USER_KEY, user)
}

/**
 * 移除用户信息
 */
export function removeUserInfo(): void {
  removeStorage(USER_KEY)
}

/**
 * 清除所有认证信息
 */
export function clearAuth(): void {
  removeToken()
  removeRefreshToken()
  removeUserInfo()
}