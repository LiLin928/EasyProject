import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import zhCnElement from 'element-plus/es/locale/lang/zh-cn'
import enElement from 'element-plus/es/locale/lang/en'
import type { Language } from 'element-plus/es/locale'

const ELEMENT_LOCALES: Record<string, Language> = {
  'zh-CN': zhCnElement,
  'en-US': enElement,
}

export type LocaleType = 'zh-CN' | 'en-US'

export interface LocaleOption {
  label: string
  value: LocaleType
  icon?: string
}

/**
 * 多语言切换 Hook
 * 提供语言切换、Element Plus 语言包适配等功能
 */
export function useLocale() {
  const i18n = useI18n()

  /**
   * 当前语言
   */
  const currentLocale = computed<LocaleType>(() => {
    return i18n.locale.value as LocaleType
  })

  /**
   * Element Plus 语言包
   */
  const elementLocale = computed<Language>(() => {
    return ELEMENT_LOCALES[currentLocale.value] || zhCnElement
  })

  /**
   * 语言选项列表
   */
  const localeOptions: LocaleOption[] = [
    {
      label: '简体中文',
      value: 'zh-CN',
      icon: '🇨🇳',
    },
    {
      label: 'English',
      value: 'en-US',
      icon: '🇺🇸',
    },
  ]

  /**
   * 切换语言
   * @param lang 目标语言
   */
  const changeLocale = (lang: LocaleType) => {
    if (currentLocale.value === lang) {
      return
    }

    i18n.locale.value = lang
    localStorage.setItem('locale', lang)

    // 更新 HTML lang 属性
    document.documentElement.setAttribute('lang', lang)

    // 重新加载页面以确保 Element Plus 组件也更新语言
    // 如果不想刷新页面，可以手动更新 Element Plus ConfigProvider 的 locale
    window.location.reload()
  }

  /**
   * 初始化语言
   * 从 localStorage 读取已保存的语言设置
   */
  const initLocale = () => {
    const savedLocale = localStorage.getItem('locale')
    if (savedLocale && ['zh-CN', 'en-US'].includes(savedLocale)) {
      i18n.locale.value = savedLocale
      document.documentElement.setAttribute('lang', savedLocale)
    }
  }

  return {
    t: i18n.t,
    currentLocale,
    elementLocale,
    changeLocale,
    initLocale,
    localeOptions,
  }
}