// 文件: PCWeb/src/stores/desktopStore.ts

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type {
  UserWidgetConfigDto,
  AvailableWidget,
  UserDesktopDto,
  UserWidgetConfigItem,
} from '@/types/desktopWidget'
import {
  getUserDesktop,
  saveUserWidgetConfig,
  resetUserDesktop,
} from '@/api/basic/userWidgetConfigApi'
import { ElMessage } from 'element-plus'

export const useDesktopStore = defineStore('desktop', () => {
  // State
  const widgets = ref<UserWidgetConfigDto[]>([])
  const availableWidgets = ref<AvailableWidget[]>([])
  const loading = ref(false)
  const saving = ref(false)

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

  // Reset
  function $reset() {
    widgets.value = []
    availableWidgets.value = []
    loading.value = false
    saving.value = false
  }

  return {
    // State
    widgets,
    availableWidgets,
    loading,
    saving,
    // Getter
    enabledWidgets,
    // Actions
    fetchDesktop,
    saveConfig,
    resetToDefault,
    $reset,
  }
})