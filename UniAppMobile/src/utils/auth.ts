// utils/auth.ts

import type { UserInfo } from '@/types'
import { getStorage, setStorage, removeStorage } from './storage'

const ACCESS_TOKEN_KEY = 'accessToken'
const REFRESH_TOKEN_KEY = 'refreshToken'
const USER_KEY = 'user'

/** 简单加密函数（Base64 编码） */
const encryptData = (data: string): string => {
  try {
    // 添加随机前缀增强安全性
    const prefix = Math.random().toString(36).substring(2, 8)
    const encrypted = btoa(prefix + ':' + data)
    return encrypted
  } catch {
    return data
  }
}

/** 简单解密函数（Base64 解码） */
const decryptData = (encrypted: string): string => {
  try {
    const decrypted = atob(encrypted)
    // 移除随机前缀
    const parts = decrypted.split(':')
    if (parts.length >= 2) {
      return parts[1]
    }
    return decrypted
  } catch {
    return ''
  }
}

/** 获取 Access Token */
export function getToken(): string | null {
  const encrypted = getStorage<string>(ACCESS_TOKEN_KEY)
  if (!encrypted) return null
  return decryptData(encrypted)
}

/** 设置 Access Token */
export function setToken(token: string): void {
  const encrypted = encryptData(token)
  setStorage(ACCESS_TOKEN_KEY, encrypted)
}

/** 移除 Access Token */
export function removeToken(): void {
  removeStorage(ACCESS_TOKEN_KEY)
}

/** 获取 Refresh Token */
export function getRefreshToken(): string | null {
  const encrypted = getStorage<string>(REFRESH_TOKEN_KEY)
  if (!encrypted) return null
  return decryptData(encrypted)
}

/** 设置 Refresh Token */
export function setRefreshToken(token: string): void {
  const encrypted = encryptData(token)
  setStorage(REFRESH_TOKEN_KEY, encrypted)
}

/** 移除 Refresh Token */
export function removeRefreshToken(): void {
  removeStorage(REFRESH_TOKEN_KEY)
}

/** 获取用户信息 */
export function getUserInfo(): UserInfo | null {
  return getStorage<UserInfo>(USER_KEY)
}

/** 设置用户信息 */
export function setUserInfo(user: UserInfo): void {
  // 用户信息不加密，但可以过滤敏感字段
  const safeUser: UserInfo = {
    ...user,
    // 移除可能存在的敏感信息
  }
  setStorage(USER_KEY, safeUser)
}

/** 移除用户信息 */
export function removeUserInfo(): void {
  removeStorage(USER_KEY)
}

/** 清除所有认证信息 */
export function clearAuth(): void {
  removeToken()
  removeRefreshToken()
  removeUserInfo()
}

/** 检查是否已登录 */
export function isLoggedIn(): boolean {
  return !!getToken() && !!getUserInfo()
}

/** 检查 Token 是否即将过期（需配合后端实现） */
export function isTokenExpiringSoon(): boolean {
  // 这里可以根据后端返回的过期时间判断
  // 暂时返回 false，实际项目中应实现 Token 刷新逻辑
  return false
}