import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getDictDataBatch, checkDictVersion, getDictDataWithVersion, getDictDataByCode } from '@/api/dict'
import type { DictCacheItem, DictDataItem } from '@/types/dict'
import axios from 'axios'

const CACHE_KEY = 'pcweb_dict_cache'

export const useDictCacheStore = defineStore('dictCache', () => {
  const cache = ref<Record<string, DictCacheItem>>({})

  /**
   * 初始化缓存（从 localStorage 读取）
   */
  function init() {
    try {
      const stored = localStorage.getItem(CACHE_KEY)
      if (stored) {
        cache.value = JSON.parse(stored)
      }
    } catch (e) {
      console.warn('Failed to load dict cache from localStorage:', e)
      cache.value = {}
    }
  }

  /**
   * 获取缓存项
   */
  function getCacheItem(code: string): DictCacheItem | undefined {
    return cache.value[code]
  }

  /**
   * 检查并更新缓存
   */
  async function checkAndUpdate(codes: string[]): Promise<void> {
    if (codes.length === 0) return

    // 获取本地缓存的版本信息
    const localVersions: Record<string, number> = {}
    codes.forEach(code => {
      const item = cache.value[code]
      if (item) {
        localVersions[code] = item.version
      }
    })

    try {
      // 请求版本检查
      const result = await checkDictVersion(localVersions)
      const needRefresh = result.needRefresh || []

      // 如果有需要更新的，批量获取
      if (needRefresh.length > 0) {
        const batchResult = await getDictDataBatch(needRefresh)

        // 更新缓存
        Object.entries(batchResult).forEach(([code, data]) => {
          cache.value[code] = {
            code,
            version: data.version,
            items: data.items,
            updateTime: Date.now()
          }
        })

        // 保存到 localStorage
        saveToLocalStorage()
      }
    } catch (e) {
      // 如果是 404 错误（接口不存在），使用备用方案逐个获取
      if (axios.isAxiosError(e) && e.response?.status === 404) {
        console.warn('Dict version check API not available, using fallback method')
        // 备用方案：逐个获取字典数据
        for (const code of codes) {
          try {
            const items = await getDictDataByCode(code)
            cache.value[code] = {
              code,
              version: Date.now(), // 使用时间戳作为版本
              items: items,
              updateTime: Date.now()
            }
          } catch (itemError) {
            // 单个字典获取失败也静默处理
            if (axios.isAxiosError(itemError) && itemError.response?.status === 404) {
              console.warn(`Dict API not available for code: ${code}`)
            } else {
              console.warn(`Failed to get dict data for ${code}:`, itemError)
            }
          }
        }
        saveToLocalStorage()
      } else {
        // 其他错误静默处理，不阻塞应用
        console.warn('Failed to check and update dict cache:', e)
      }
    }
  }

  /**
   * 强制刷新指定字典
   */
  async function refreshDict(code: string): Promise<void> {
    try {
      const result = await getDictDataWithVersion(code)
      cache.value[code] = {
        code,
        version: result.version,
        items: result.items,
        updateTime: Date.now()
      }
      saveToLocalStorage()
    } catch (e) {
      // 如果是 404 错误，使用备用方案
      if (axios.isAxiosError(e) && e.response?.status === 404) {
        try {
          const items = await getDictDataByCode(code)
          cache.value[code] = {
            code,
            version: Date.now(),
            items: items,
            updateTime: Date.now()
          }
          saveToLocalStorage()
        } catch (fallbackError) {
          console.warn(`Failed to refresh dict ${code}:`, fallbackError)
        }
      } else {
        console.warn(`Failed to refresh dict ${code}:`, e)
      }
    }
  }

  /**
   * 清除缓存
   */
  function clearCache(): void {
    cache.value = {}
    localStorage.removeItem(CACHE_KEY)
  }

  /**
   * 保存到 localStorage
   */
  function saveToLocalStorage(): void {
    try {
      localStorage.setItem(CACHE_KEY, JSON.stringify(cache.value))
    } catch (e) {
      console.warn('Failed to save dict cache to localStorage:', e)
    }
  }

  return {
    cache,
    init,
    getCacheItem,
    checkAndUpdate,
    refreshDict,
    clearCache
  }
})