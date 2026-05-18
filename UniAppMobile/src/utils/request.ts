// utils/request.ts

import { getToken, clearAuth } from './auth'
import { API_BASE_URL } from '@/config/env'
import type { ApiResponse } from '@/types/response'

/** Token 失效标志 */
let isTokenExpired = false

/** 敏感字段列表 - 用于日志过滤 */
const SENSITIVE_FIELDS = ['password', 'token', 'secret', 'accessToken', 'refreshToken', 'apiKey']

/** 请求配置 */
interface RequestConfig {
  url: string
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  data?: any
  params?: any
  header?: Record<string, string>
  timeout?: number
}

/** 生成请求签名（简化版） */
const generateSign = (timestamp: number, nonce: string): string => {
  // 实际项目应使用更安全的签名算法（如 HMAC-SHA256）
  // 这里使用简化版本：timestamp + nonce 的 hash
  const signStr = `${timestamp}:${nonce}`
  // 简单的字符转换作为签名
  let hash = 0
  for (let i = 0; i < signStr.length; i++) {
    const char = signStr.charCodeAt(i)
    hash = ((hash << 5) - hash) + char
    hash = hash & hash // Convert to 32bit integer
  }
  return Math.abs(hash).toString(16)
}

/** 生成随机 nonce */
const generateNonce = (): string => {
  return Math.random().toString(36).substring(2, 15) +
         Math.random().toString(36).substring(2, 15)
}

/** 过滤敏感数据（用于日志打印） */
const filterSensitiveData = (data: any): any => {
  if (typeof data !== 'object' || data === null) return data

  const filtered = { ...data }
  for (const field of SENSITIVE_FIELDS) {
    if (filtered[field]) {
      filtered[field] = '[PROTECTED]'
    }
  }
  return filtered
}

/** 安全日志输出 */
const safeLog = (message: string, data?: any) => {
  if (data) {
    const filteredData = filterSensitiveData(data)
    console.log(message, filteredData)
  } else {
    console.log(message)
  }
}

/** 通用请求方法 */
function request<T>(config: RequestConfig): Promise<T> {
  return new Promise((resolve, reject) => {
    const token = getToken()

    // 生成请求安全参数
    const timestamp = Date.now()
    const nonce = generateNonce()
    const sign = generateSign(timestamp, nonce)

    // 构建请求头
    const header: Record<string, string> = {
      'Content-Type': 'application/json',
      'Authorization': token ? `Bearer ${token}` : '',
      // 安全签名参数
      'X-Timestamp': timestamp.toString(),
      'X-Nonce': nonce,
      'X-Sign': sign,
      ...config.header,
    }

    // 开发模式下打印安全日志
    if (process.env.NODE_ENV === 'development') {
      safeLog('Request:', {
        url: config.url,
        method: config.method,
        data: config.data,
        params: config.params,
      })
    }

    uni.request({
      url: API_BASE_URL + config.url,
      method: config.method || 'GET',
      data: config.method === 'GET' ? config.params : config.data,
      header,
      timeout: config.timeout || 30000,
      success: (res) => {
        const response = res.data as ApiResponse

        // 开发模式下打印安全日志
        if (process.env.NODE_ENV === 'development') {
          safeLog('Response:', {
            url: config.url,
            code: response.code,
            message: response.message,
          })
        }

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
        console.error('Request failed:', err)
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

/** 上传文件 */
export function upload<T>(
  url: string,
  filePath: string,
  formData?: Record<string, any>
): Promise<T> {
  return new Promise((resolve, reject) => {
    const token = getToken()

    const timestamp = Date.now()
    const nonce = generateNonce()
    const sign = generateSign(timestamp, nonce)

    uni.uploadFile({
      url: API_BASE_URL + url,
      filePath,
      name: 'file',
      formData,
      header: {
        'Authorization': token ? `Bearer ${token}` : '',
        'X-Timestamp': timestamp.toString(),
        'X-Nonce': nonce,
        'X-Sign': sign,
      },
      success: (res) => {
        try {
          const response = JSON.parse(res.data) as ApiResponse
          if (response.code === 200) {
            resolve(response.data as T)
          } else {
            uni.showToast({ title: response.message || '上传失败', icon: 'none' })
            reject(new Error(response.message))
          }
        } catch {
          reject(new Error('上传响应解析失败'))
        }
      },
      fail: (err) => {
        console.error('Upload failed:', err)
        uni.showToast({ title: '上传失败', icon: 'none' })
        reject(err)
      },
    })
  })
}

/** 下载文件 */
export function download(
  url: string
): Promise<string> {
  return new Promise((resolve, reject) => {
    const token = getToken()

    uni.downloadFile({
      url: API_BASE_URL + url,
      header: {
        'Authorization': token ? `Bearer ${token}` : '',
      },
      success: (res) => {
        if (res.statusCode === 200) {
          resolve(res.tempFilePath)
        } else {
          reject(new Error('下载失败'))
        }
      },
      fail: (err) => {
        console.error('Download failed:', err)
        reject(err)
      },
    })
  })
}