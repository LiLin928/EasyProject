import { computed } from 'vue'
import { getModuleConfig, getDictCode } from '@/config/dict.config'
import { getLocalEnumLabel, getLocalEnumOptions } from '@/config/enumLabels'
import { useDictCacheStore } from '@/stores/dictCacheStore'
import { useLocale } from '@/composables/useLocale'
import type { DictOption } from '@/types/dict'

/**
 * 字典核心 composable
 * @param module 模块名称（如 'workflow', 'etl'）
 */
export function useDict(module: string) {
  const { currentLocale } = useLocale()
  const dictCacheStore = useDictCacheStore()
  const config = getModuleConfig(module)

  /**
   * 获取标签
   * @param enumName 枚举名称
   * @param value 枚举值
   */
  function getLabel(enumName: string, value: number | string): string {
    if (config?.IsUseDic) {
      const dictCode = getDictCode(module, enumName)
      return getDictLabel(dictCode, String(value), currentLocale.value)
    } else {
      return getLocalEnumLabel(enumName, value, currentLocale.value)
    }
  }

  /**
   * 获取字典项列表（用于下拉选项）
   * @param enumName 枚举名称
   */
  function getOptions(enumName: string): DictOption[] {
    if (config?.IsUseDic) {
      const dictCode = getDictCode(module, enumName)
      return getDictOptions(dictCode, currentLocale.value)
    } else {
      return getLocalEnumOptions(enumName, currentLocale.value)
    }
  }

  /**
   * 根据标签获取值
   */
  function getValueByLabel(enumName: string, label: string): string | undefined {
    const options = getOptions(enumName)
    return options.find(opt => opt.label === label)?.value
  }

  /**
   * 是否使用字典
   */
  const isUseDict = computed(() => config?.IsUseDic ?? false)

  return {
    getLabel,
    getOptions,
    getValueByLabel,
    isUseDict
  }
}

/**
 * 从缓存获取字典标签
 */
function getDictLabel(code: string, value: string, locale: string): string {
  const dictCacheStore = useDictCacheStore()
  const cacheItem = dictCacheStore.getCacheItem(code)

  if (!cacheItem) {
    // 缓存中没有，返回原值
    return value
  }

  const item = cacheItem.items.find(i => i.value === value)
  if (!item) return value

  return locale === 'zh-CN' ? item.labelZh : (item.labelEn || item.labelZh)
}

/**
 * 从缓存获取字典选项列表
 */
function getDictOptions(code: string, locale: string): DictOption[] {
  const dictCacheStore = useDictCacheStore()
  const cacheItem = dictCacheStore.getCacheItem(code)

  if (!cacheItem) return []

  return cacheItem.items
    .filter(item => item.status === 1)
    .sort((a, b) => a.sort - b.sort)
    .map(item => ({
      value: item.value,
      label: locale === 'zh-CN' ? item.labelZh : (item.labelEn || item.labelZh)
    }))
}