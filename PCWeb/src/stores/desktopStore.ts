// 文件: PCWeb/src/stores/desktopStore.ts

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type {
  UserWidgetConfigDto,
  AvailableWidget,
  UserDesktopDto,
  UserWidgetConfigItem,
  RefreshState,
} from '@/types/desktopWidget'
import {
  getUserDesktop,
  saveUserWidgetConfig,
  resetUserDesktop,
  getWidgetData,
} from '@/api/basic/userWidgetConfigApi'
import { ElMessage } from 'element-plus'

export const useDesktopStore = defineStore('desktop', () => {
  // State
  const widgets = ref<UserWidgetConfigDto[]>([])
  const availableWidgets = ref<AvailableWidget[]>([])
  const loading = ref(false)
  const saving = ref(false)

  // 刷新状态 Map
  const refreshStates = ref<Map<string, RefreshState>>(new Map())
  // 组件数据 Map
  const widgetDataMap = ref<Map<string, any>>(new Map())
  // 定时器 Map
  const timers = new Map<string, ReturnType<typeof setInterval>>()

  // Getter - 启用的组件，按排序过滤
  const enabledWidgets = computed(() => {
    return widgets.value
      .filter(w => w.isEnabled)
      .sort((a, b) => a.sortOrder - b.sortOrder)
  })

  // Actions
  /**
   * 获取用户桌面配置
   */
  async function fetchDesktop() {
    loading.value = true
    try {
      const data: UserDesktopDto = await getUserDesktop()
      widgets.value = data.widgets || []
      availableWidgets.value = data.availableWidgets || []

      // 初始化自动刷新
      initAutoRefresh()
    } catch (error) {
      console.error('获取桌面配置失败:', error)
      ElMessage.error('获取桌面配置失败')
    } finally {
      loading.value = false
    }
  }

  /**
   * 保存用户配置
   * @param items 配置项列表
   */
  async function saveConfig(items: UserWidgetConfigItem[]) {
    saving.value = true
    try {
      await saveUserWidgetConfig({ widgets: items })
      ElMessage.success('保存成功')
      await fetchDesktop()
    } catch (error) {
      console.error('保存配置失败:', error)
      ElMessage.error('保存配置失败')
    } finally {
      saving.value = false
    }
  }

  /**
   * 重置为默认配置
   */
  async function resetToDefault() {
    saving.value = true
    try {
      await resetUserDesktop()
      ElMessage.success('已重置为默认配置')
      await fetchDesktop()
    } catch (error) {
      console.error('重置配置失败:', error)
      ElMessage.error('重置配置失败')
    } finally {
      saving.value = false
    }
  }

  /**
   * 初始化自动刷新
   */
  function initAutoRefresh() {
    // 清理旧的定时器
    stopAutoRefresh()

    // 初始化每个组件的刷新状态
    widgets.value.forEach((widget) => {
      refreshStates.value.set(widget.widgetId, {
        widgetId: widget.widgetId,
        loading: false,
        lastRefreshTime: undefined,
        error: undefined,
      })

      // 从数据源配置中提取刷新间隔
      const refreshInterval = extractRefreshInterval(widget.dataSourceConfig)

      // 如果有刷新间隔，启动定时器
      if (refreshInterval > 0) {
        const timer = setInterval(() => {
          refreshWidget(widget.widgetId)
        }, refreshInterval * 1000)
        timers.set(widget.widgetId, timer)
      }
    })

    // 立即刷新所有启用的组件
    refreshAllWidgets()
  }

  /**
   * 停止所有自动刷新
   */
  function stopAutoRefresh() {
    timers.forEach((timer) => clearInterval(timer))
    timers.clear()
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
   * 刷新所有启用的组件
   */
  async function refreshAllWidgets() {
    const enabledWidgetIds = enabledWidgets.value.map((w) => w.widgetId)
    await Promise.all(enabledWidgetIds.map((id) => refreshWidget(id)))
  }

  /**
   * 手动刷新单个组件
   */
  async function manualRefresh(widgetId: string) {
    await refreshWidget(widgetId)
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
  function getWidgetDataById(widgetId: string): any {
    return widgetDataMap.value.get(widgetId)
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

  // Reset
  function $reset() {
    widgets.value = []
    availableWidgets.value = []
    loading.value = false
    saving.value = false
    refreshStates.value.clear()
    widgetDataMap.value.clear()
    stopAutoRefresh()
  }

  return {
    // State
    widgets,
    availableWidgets,
    loading,
    saving,
    refreshStates,
    widgetDataMap,
    // Getter
    enabledWidgets,
    // Actions
    fetchDesktop,
    saveConfig,
    resetToDefault,
    initAutoRefresh,
    stopAutoRefresh,
    refreshWidget,
    refreshAllWidgets,
    manualRefresh,
    getRefreshState,
    getWidgetDataById,
    $reset,
  }
})