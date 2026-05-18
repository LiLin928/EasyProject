// stores/app.ts

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const useAppStore = defineStore('app', () => {
  // State
  const loading = ref(false)
  const networkType = ref<string>('')
  const systemInfo = ref<UniApp.GetSystemInfoResult | null>(null)
  const statusBarHeight = ref<number>(0)
  const safeAreaInsets = ref<UniApp.SafeAreaInsets | null>(null)

  // Getter
  const isOnline = computed(() => networkType.value && networkType.value !== 'none')

  const isIOS = computed(() => {
    if (!systemInfo.value) return false
    return systemInfo.value.platform === 'ios'
  })

  const isAndroid = computed(() => {
    if (!systemInfo.value) return false
    return systemInfo.value.platform === 'android'
  })

  const screenWidth = computed(() => systemInfo.value?.screenWidth || 375)

  const screenHeight = computed(() => systemInfo.value?.screenHeight || 667)

  // Actions
  /** 设置加载状态 */
  function setLoading(val: boolean): void {
    loading.value = val
  }

  /** 显示加载 */
  function showLoading(): void {
    loading.value = true
  }

  /** 隐藏加载 */
  function hideLoading(): void {
    loading.value = false
  }

  /** 获取网络类型 */
  async function getNetworkType(): Promise<string> {
    try {
      const res = await uni.getNetworkType()
      networkType.value = res.networkType
      return res.networkType
    } catch (error) {
      console.error('获取网络类型失败:', error)
      networkType.value = 'unknown'
      return 'unknown'
    }
  }

  /** 获取系统信息 */
  function getSystemInfo(): UniApp.GetSystemInfoResult {
    if (!systemInfo.value) {
      systemInfo.value = uni.getSystemInfoSync()
      statusBarHeight.value = systemInfo.value.statusBarHeight || 0
      safeAreaInsets.value = systemInfo.value.safeAreaInsets || null
    }
    return systemInfo.value
  }

  /** 初始化系统信息 */
  function initSystemInfo(): void {
    getSystemInfo()
    getNetworkType()
  }

  /** 显示 Toast */
  function showToast(
    title: string,
    options?: Partial<UniApp.ShowToastOptions>
  ): void {
    uni.showToast({
      title,
      icon: 'none',
      duration: 2000,
      ...options,
    })
  }

  /** 显示成功 Toast */
  function showSuccess(title: string): void {
    showToast(title, { icon: 'success' })
  }

  /** 显示错误 Toast */
  function showError(title: string): void {
    showToast(title, { icon: 'error' })
  }

  /** 显示加载中 */
  function showLoadingToast(title: string = '加载中...'): void {
    uni.showLoading({
      title,
      mask: true,
    })
  }

  /** 隐藏加载中 */
  function hideLoadingToast(): void {
    uni.hideLoading()
  }

  /** 重置状态 */
  function $reset(): void {
    loading.value = false
    networkType.value = ''
    systemInfo.value = null
    statusBarHeight.value = 0
    safeAreaInsets.value = null
  }

  return {
    // State
    loading,
    networkType,
    systemInfo,
    statusBarHeight,
    safeAreaInsets,
    // Getter
    isOnline,
    isIOS,
    isAndroid,
    screenWidth,
    screenHeight,
    // Actions
    setLoading,
    showLoading,
    hideLoading,
    getNetworkType,
    getSystemInfo,
    initSystemInfo,
    showToast,
    showSuccess,
    showError,
    showLoadingToast,
    hideLoadingToast,
    $reset,
  }
})