// 文件: PCWeb/src/composables/useWidgetRefresh.ts

import { ref, onUnmounted } from 'vue'
import type { RefreshState } from '@/types/desktopWidget'
import { getWidgetData } from '@/api/basic/userWidgetConfigApi'

/**
 * 单个组件刷新管理
 * @param widgetId 组件ID
 * @param refreshInterval 刷新间隔（秒），0表示手动刷新
 */
export function useWidgetRefresh(widgetId: string, refreshInterval: number = 0) {
  const refreshState = ref<RefreshState>({
    widgetId,
    loading: false,
    lastRefreshTime: undefined,
    error: undefined,
  })

  const widgetData = ref<any>(null)
  let timer: ReturnType<typeof setInterval> | null = null

  /**
   * 执行刷新
   */
  async function doRefresh() {
    refreshState.value.loading = true
    refreshState.value.error = undefined

    try {
      const data = await getWidgetData(widgetId)
      widgetData.value = data
      refreshState.value.lastRefreshTime = new Date()
    } catch (err: any) {
      refreshState.value.error = err.message || '获取数据失败'
      console.error(`组件 ${widgetId} 数据刷新失败:`, err)
    } finally {
      refreshState.value.loading = false
    }
  }

  /**
   * 启动定时刷新
   */
  function startAutoRefresh() {
    if (timer) {
      clearInterval(timer)
      timer = null
    }

    if (refreshInterval > 0) {
      timer = setInterval(() => {
        doRefresh()
      }, refreshInterval * 1000)
    }
  }

  /**
   * 停止定时刷新
   */
  function stopAutoRefresh() {
    if (timer) {
      clearInterval(timer)
      timer = null
    }
  }

  /**
   * 手动刷新（立即执行一次）
   */
  async function manualRefresh() {
    await doRefresh()
  }

  // 组件卸载时清理定时器
  onUnmounted(() => {
    stopAutoRefresh()
  })

  return {
    refreshState,
    widgetData,
    doRefresh,
    startAutoRefresh,
    stopAutoRefresh,
    manualRefresh,
  }
}

/**
 * 批量组件刷新管理器
 */
export function useBatchRefresh() {
  const refreshStates = ref<Map<string, RefreshState>>(new Map())
  const widgetDataMap = ref<Map<string, any>>(new Map())
  const timers = new Map<string, ReturnType<typeof setInterval>>()

  /**
   * 初始化组件刷新
   * @param widgetConfigs 组件配置列表
   */
  function initRefresh(widgetConfigs: Array<{ id: string; dataSourceConfig?: string }>) {
    // 清理旧的定时器
    clearAllTimers()

    // 初始化每个组件
    widgetConfigs.forEach((config) => {
      const refreshInterval = extractRefreshInterval(config.dataSourceConfig)

      refreshStates.value.set(config.id, {
        widgetId: config.id,
        loading: false,
        lastRefreshTime: undefined,
        error: undefined,
      })

      // 如果有刷新间隔，启动定时器
      if (refreshInterval > 0) {
        const timer = setInterval(() => {
          refreshWidget(config.id)
        }, refreshInterval * 1000)
        timers.set(config.id, timer)
      }
    })

    // 立即刷新所有组件
    refreshAllWidgets(widgetConfigs.map((c) => c.id))
  }

  /**
   * 刷新单个组件
   */
  async function refreshWidget(widgetId: string) {
    const state = refreshStates.value.get(widgetId)
    if (!state) return

    state.loading = true
    state.error = undefined

    try {
      const data = await getWidgetData(widgetId)
      widgetDataMap.value.set(widgetId, data)
      state.lastRefreshTime = new Date()
      refreshStates.value.set(widgetId, state)
    } catch (err: any) {
      state.error = err.message || '获取数据失败'
      refreshStates.value.set(widgetId, state)
      console.error(`组件 ${widgetId} 数据刷新失败:`, err)
    } finally {
      state.loading = false
      refreshStates.value.set(widgetId, state)
    }
  }

  /**
   * 刷新所有组件
   */
  async function refreshAllWidgets(widgetIds: string[]) {
    await Promise.all(widgetIds.map((id) => refreshWidget(id)))
  }

  /**
   * 手动刷新单个组件
   */
  async function manualRefreshWidget(widgetId: string) {
    await refreshWidget(widgetId)
  }

  /**
   * 停止所有自动刷新
   */
  function stopAllAutoRefresh() {
    clearAllTimers()
  }

  /**
   * 获取组件刷新状态
   */
  function getRefreshState(widgetId: string): RefreshState | undefined {
    return refreshStates.value.get(widgetId)
  }

  /**
   * 获取组件数据
   */
  function getWidgetData(widgetId: string): any {
    return widgetDataMap.value.get(widgetId)
  }

  /**
   * 清理所有定时器
   */
  function clearAllTimers() {
    timers.forEach((timer) => clearInterval(timer))
    timers.clear()
  }

  /**
   * 从数据源配置中提取刷新间隔
   */
  function extractRefreshInterval(dataSourceConfig?: string): number {
    if (!dataSourceConfig) return 0

    try {
      const config = JSON.parse(dataSourceConfig)
      return config.refreshInterval || 0
    } catch {
      return 0
    }
  }

  // 组件卸载时清理
  onUnmounted(() => {
    clearAllTimers()
  })

  return {
    refreshStates,
    widgetDataMap,
    initRefresh,
    refreshWidget,
    refreshAllWidgets,
    manualRefreshWidget,
    stopAllAutoRefresh,
    getRefreshState,
    getWidgetData,
  }
}