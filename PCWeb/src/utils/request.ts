// Axios 请求封装

import axios, { type AxiosInstance, type AxiosResponse } from 'axios'
import { getToken, clearAuth } from './auth'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { ApiResponse } from '@/types'

// Token 失效标志（防止多次弹窗）
let isTokenExpired = false

// 创建 axios 实例
const service: AxiosInstance = axios.create({
  // 开发环境使用 Vite proxy，生产环境使用实际 API 地址
  baseURL: import.meta.env.MODE === 'production' ? import.meta.env.VITE_API_BASE_URL : '',
  timeout: 30000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// 请求拦截器
service.interceptors.request.use(
  (config) => {
    const token = getToken()
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// 响应拦截器
service.interceptors.response.use(
  (response: AxiosResponse<ApiResponse>) => {
    const { code, message, data } = response.data

    // 成功
    if (code === 200) {
      return data as any
    }

    // 未登录或 Token 过期
    if (code === 401) {
      // 防止多次弹窗
      if (!isTokenExpired) {
        isTokenExpired = true
        clearAuth()
        ElMessageBox.confirm('登录状态已过期，请重新登录', '提示', {
          confirmButtonText: '重新登录',
          cancelButtonText: '取消',
          type: 'warning',
        }).then(() => {
          window.location.href = '/#/login'
        }).catch(() => {
          // 用户取消，也跳转登录页
          window.location.href = '/#/login'
        })
      }
      return Promise.reject(new Error(message))
    }

    // 无权限
    if (code === 403) {
      ElMessage.error('没有操作权限')
      return Promise.reject(new Error(message))
    }

    // 其他错误
    ElMessage.error(message || '请求失败')
    return Promise.reject(new Error(message))
  },
  (error) => {
    // HTTP 401 未授权
    if (error.response?.status === 401) {
      if (!isTokenExpired) {
        isTokenExpired = true
        clearAuth()
        ElMessage.error('登录已过期，请重新登录')
        window.location.href = '/#/login'
      }
      return Promise.reject(error)
    }

    let message = '请求失败'
    if (error.code === 'ECONNABORTED') {
      message = '请求超时'
    } else if (error.response?.status === 500) {
      message = '服务器错误'
    } else if (error.response?.status === 404) {
      message = '资源不存在'
    } else if (error.response?.status === 403) {
      message = '没有操作权限'
    }
    ElMessage.error(message)
    return Promise.reject(error)
  }
)

// 请求方法封装
export function get<T = any>(url: string, params?: object): Promise<T> {
  return service.get(url, { params: params || {} })
}

export function post<T = any>(url: string, data?: object | any[] | string | number | null, config?: object): Promise<T> {
  // 原始值（字符串、数字）直接传递，不经过处理
  if (typeof data === 'string' || typeof data === 'number') {
    return service.post(url, data, config)
  }
  // null/undefined 传递空对象
  if (data === null || data === undefined) {
    return service.post(url, {}, config)
  }
  // 数组或对象直接传递
  return service.post(url, data, config)
}

export function put<T = any>(url: string, data?: object): Promise<T> {
  return service.put(url, data)
}

export function del<T = any>(url: string, data?: object): Promise<T> {
  return service.delete(url, { data })
}

export default service