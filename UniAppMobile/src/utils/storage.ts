// utils/storage.ts

import { STORAGE_PREFIX } from '@/config/env'

/** 缓存版本号 - 用于版本更新时清理旧缓存 */
const CACHE_VERSION = 'v1.0.0'

/** 存储数据结构 */
interface StorageData<T> {
  /** 数据版本 */
  version: string
  /** 存储值 */
  value: T
  /** 过期时间（毫秒时间戳） */
  expire: number | null
}

/** 获取完整存储键名 */
function getFullKey(key: string): string {
  return STORAGE_PREFIX + key
}

/** 设置存储 */
export function setStorage<T>(key: string, value: T, expire?: number): void {
  const data: StorageData<T> = {
    version: CACHE_VERSION,
    value,
    expire: expire ? Date.now() + expire * 1000 : null,
  }
  uni.setStorageSync(getFullKey(key), JSON.stringify(data))
}

/** 获取存储 */
export function getStorage<T>(key: string): T | null {
  const dataStr = uni.getStorageSync(getFullKey(key))
  if (!dataStr) return null

  try {
    const data: StorageData<T> = JSON.parse(dataStr)

    // 版本检查 - 版本不一致则清除旧缓存
    if (data.version !== CACHE_VERSION) {
      removeStorage(key)
      return null
    }

    // 过期检查
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
  uni.removeStorageSync(getFullKey(key))
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

/** 清除过期缓存 */
export function clearExpiredCache(): void {
  const res = uni.getStorageInfoSync()
  res.keys.forEach(k => {
    if (k.startsWith(STORAGE_PREFIX)) {
      const dataStr = uni.getStorageSync(k)
      if (dataStr) {
        try {
          const data = JSON.parse(dataStr)
          // 版本不一致或已过期则清除
          if (data.version !== CACHE_VERSION ||
              (data.expire && Date.now() > data.expire)) {
            uni.removeStorageSync(k)
          }
        } catch {
          // 解析失败的也清除
          uni.removeStorageSync(k)
        }
      }
    }
  })
}

/** 设置带过期时间的缓存 */
export function setCache<T>(key: string, value: T, expireSeconds?: number): void {
  setStorage(key, value, expireSeconds)
}

/** 获取缓存 */
export function getCache<T>(key: string): T | null {
  return getStorage<T>(key)
}

/** 移除缓存 */
export function removeCache(key: string): void {
  removeStorage(key)
}

/** 获取存储信息 */
export function getStorageInfo(): UniApp.GetStorageInfoSyncRes {
  return uni.getStorageInfoSync()
}

/** 检查存储是否可用 */
export function isStorageAvailable(): boolean {
  try {
    uni.setStorageSync('__test__', 'test')
    uni.removeStorageSync('__test__')
    return true
  } catch {
    return false
  }
}