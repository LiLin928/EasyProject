// utils/request.ts

import { getToken, clearAuth } from './auth'
import { API_BASE_URL } from '@/config/env'
import type { ApiResponse } from '@/types/response'

/** Token 失效标志 */
let isTokenExpired = false

/** 请求配置 */
interface RequestConfig {
  url: string
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  data?: any
  params?: any
  header?: Record<string, string>
  timeout?: number
}

/** 通用请求方法 */
function request<T>(config: RequestConfig): Promise<T> {
  return new Promise((resolve, reject) => {
    const token = getToken()

    uni.request({
      url: API_BASE_URL + config.url,
      method: config.method || 'GET',
      data: config.method === 'GET' ? config.params : config.data,
      header: {
        'Content-Type': 'application/json',
        'Authorization': token ? `Bearer ${token}` : '',
        ...config.header,
      },
      timeout: config.timeout || 30000,
      success: (res) => {
        const response = res.data as ApiResponse

        // 成功
        if (response.code === 200) {
          resolve(response.data as T)
          return
        }

        // Token 过期
        if (response.code === 401) {
          if (!isTokenExpired) {
            isTokenExpired = true
            clearAuth()
            uni.showToast({ title: '登录已过期', icon: 'none' })
            uni.navigateTo({ url: '/pages/login/index' })
          }
          reject(new Error(response.message))
          return
        }

        // 无权限
        if (response.code === 403) {
          uni.showToast({ title: '没有操作权限', icon: 'none' })
          reject(new Error(response.message))
          return
        }

        // 其他错误
        uni.showToast({ title: response.message || '请求失败', icon: 'none' })
        reject(new Error(response.message))
      },
      fail: (err) => {
        uni.showToast({ title: '网络错误', icon: 'none' })
        reject(err)
      },
    })
  })
}

/** GET 请求 */
export function get<T>(url: string, params?: object): Promise<T> {
  return request<T>({ url, method: 'GET', params })
}

/** POST 请求 */
export function post<T>(url: string, data?: object): Promise<T> {
  return request<T>({ url, method: 'POST', data })
}

/** PUT 请求 */
export function put<T>(url: string, data?: object): Promise<T> {
  return request<T>({ url, method: 'PUT', data })
}

/** DELETE 请求 */
export function del<T>(url: string, data?: object): Promise<T> {
  return request<T>({ url, method: 'DELETE', data })
}