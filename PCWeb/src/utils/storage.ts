// 本地存储封装

const PREFIX = 'EP_'

export interface StorageOptions {
  expire?: number // 过期时间（毫秒）
}

interface StorageData<T> {
  value: T
  expire: number | null
}

/**
 * 设置存储
 */
export function setStorage(key: string, value: any, options?: StorageOptions): void {
  const data: StorageData<any> = {
    value,
    expire: options?.expire ? Date.now() + options.expire : null,
  }
  localStorage.setItem(PREFIX + key, JSON.stringify(data))
}

/**
 * 获取存储
 */
export function getStorage<T = any>(key: string): T | null {
  const dataStr = localStorage.getItem(PREFIX + key)
  if (!dataStr) return null

  try {
    const data: StorageData<T> = JSON.parse(dataStr)
    // 检查过期时间
    if (data.expire && Date.now() > data.expire) {
      removeStorage(key)
      return null
    }
    return data.value
  } catch {
    return null
  }
}

/**
 * 移除存储
 */
export function removeStorage(key: string): void {
  localStorage.removeItem(PREFIX + key)
}

/**
 * 清除所有存储
 */
export function clearStorage(): void {
  const keys = Object.keys(localStorage)
  keys.forEach(key => {
    if (key.startsWith(PREFIX)) {
      localStorage.removeItem(key)
    }
  })
}