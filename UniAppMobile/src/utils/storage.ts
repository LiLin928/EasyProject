// utils/storage.ts

import { STORAGE_PREFIX } from '@/config/env'

/** 存储数据结构 */
interface StorageData<T> {
  value: T
  expire: number | null
}

/** 设置存储 */
export function setStorage<T>(key: string, value: T, expire?: number): void {
  const data: StorageData<T> = {
    value,
    expire: expire ? Date.now() + expire * 1000 : null,
  }
  uni.setStorageSync(STORAGE_PREFIX + key, JSON.stringify(data))
}

/** 获取存储 */
export function getStorage<T>(key: string): T | null {
  const dataStr = uni.getStorageSync(STORAGE_PREFIX + key)
  if (!dataStr) return null

  try {
    const data: StorageData<T> = JSON.parse(dataStr)
    // 检查过期
    if (data.expire && Date.now() > data.expire) {
      removeStorage(key)
      return null
    }
    return data.value
  } catch {
    return null
  }
}

/** 移除存储 */
export function removeStorage(key: string): void {
  uni.removeStorageSync(STORAGE_PREFIX + key)
}

/** 清除所有存储 */
export function clearStorage(): void {
  const res = uni.getStorageInfoSync()
  res.keys.forEach(k => {
    if (k.startsWith(STORAGE_PREFIX)) {
      uni.removeStorageSync(k)
    }
  })
}