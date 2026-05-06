// 应用状态管理

import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useAppStore = defineStore('app', () => {
  // 侧边菜单折叠状态
  const sidebarCollapsed = ref(false)

  // 切换菜单折叠
  function toggleSidebar() {
    sidebarCollapsed.value = !sidebarCollapsed.value
  }

  // 设置菜单折叠状态
  function setSidebarCollapsed(collapsed: boolean) {
    sidebarCollapsed.value = collapsed
  }

  return {
    sidebarCollapsed,
    toggleSidebar,
    setSidebarCollapsed,
  }
})