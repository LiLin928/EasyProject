// stores/user.ts

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import {
  getToken,
  setToken,
  removeToken,
  getUserInfo as getStoredUserInfo,
  setUserInfo,
  removeUserInfo,
  getRefreshToken,
  setRefreshToken,
  removeRefreshToken,
} from '@/utils/auth'
import { login, logout, getUserInfo as fetchUserInfo } from '@/api/auth/login'
import type { UserInfo, LoginParams, LoginResponse } from '@/types'

export const useUserStore = defineStore('user', () => {
  // State
  const token = ref<string | null>(getToken())
  const refreshToken = ref<string | null>(getRefreshToken())
  const userInfo = ref<UserInfo | null>(getStoredUserInfo())

  // Getter
  const isLoggedIn = computed(() => !!token.value)

  const userName = computed(() => userInfo.value?.userName || '')

  const realName = computed(() => userInfo.value?.realName || userInfo.value?.userName || '')

  const avatar = computed(() => userInfo.value?.avatar || '')

  const permissions = computed(() => userInfo.value?.permissions || [])

  const roles = computed(() => userInfo.value?.roles || [])

  // Actions
  /** 登录 */
  async function loginAction(params: LoginParams): Promise<LoginResponse> {
    const data = await login(params)

    // 保存 token
    token.value = data.accessToken
    refreshToken.value = data.refreshToken
    setToken(data.accessToken)
    setRefreshToken(data.refreshToken)

    // 登录成功后单独获取用户信息
    const info = await fetchUserInfo()
    userInfo.value = info
    setUserInfo(info)

    return data
  }

  /** 退出登录 */
  async function logoutAction(): Promise<void> {
    try {
      await logout()
    } catch (error) {
      console.error('退出登录失败:', error)
    } finally {
      // 无论接口是否成功，都清除本地状态
      token.value = null
      refreshToken.value = null
      userInfo.value = null
      removeToken()
      removeRefreshToken()
      removeUserInfo()
    }
  }

  /** 刷新用户信息 */
  async function refreshUserInfo(): Promise<UserInfo> {
    const info = await fetchUserInfo()
    userInfo.value = info
    setUserInfo(info)
    return info
  }

  /** 检查是否已登录 */
  function checkLogin(): boolean {
    return !!token.value
  }

  /** 检查是否有权限 */
  function hasPermission(permission: string): boolean {
    return permissions.value.includes(permission)
  }

  /** 检查是否有角色 */
  function hasRole(role: string): boolean {
    return roles.value.includes(role)
  }

  /** 重置状态 */
  function $reset(): void {
    token.value = null
    refreshToken.value = null
    userInfo.value = null
  }

  return {
    // State
    token,
    refreshToken,
    userInfo,
    // Getter
    isLoggedIn,
    userName,
    realName,
    avatar,
    permissions,
    roles,
    // Actions
    loginAction,
    logoutAction,
    refreshUserInfo,
    checkLogin,
    hasPermission,
    hasRole,
    $reset,
  }
})