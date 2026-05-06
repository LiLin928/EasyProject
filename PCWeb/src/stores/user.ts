// 用户状态管理

import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getToken, setToken, removeToken, getUserInfo as getStoredUserInfo, setUserInfo, removeUserInfo, getRefreshToken, setRefreshToken, removeRefreshToken } from '@/utils/auth'
import { login, logout, getUserInfo as fetchUserInfo } from '@/api'
import type { UserInfo, LoginParams } from '@/types'
import router from '@/router'

export const useUserStore = defineStore('user', () => {
  // 状态
  const token = ref<string | null>(getToken())
  const userInfo = ref<UserInfo | null>(getStoredUserInfo())

  // 登录
  async function loginAction(params: LoginParams) {
    const data = await login(params)

    // 存储 Token
    token.value = data.accessToken
    setToken(data.accessToken)
    setRefreshToken(data.refreshToken)

    // 登录成功后获取用户信息
    const info = await fetchUserInfo()
    userInfo.value = info
    setUserInfo(info)

    return data
  }

  // 退出登录
  async function logoutAction() {
    await logout()

    // 清除用户状态
    token.value = null
    userInfo.value = null
    removeToken()
    removeRefreshToken()
    removeUserInfo()

    // 清除路由状态
    const { usePermissionStore } = await import('./permission')
    const permissionStore = usePermissionStore()
    permissionStore.resetRoutes()

    router.push('/login')
  }

  // 获取用户信息
  async function getUserInfoAction() {
    const data = await fetchUserInfo()
    userInfo.value = data
    setUserInfo(data)
    return data
  }

  // 设置用户信息（用于更新用户资料后同步状态）
  function setUserInfoAction(info: UserInfo) {
    userInfo.value = info
    setUserInfo(info)
  }

  return {
    token,
    userInfo,
    loginAction,
    logoutAction,
    getUserInfoAction,
    setUserInfoAction,
  }
})