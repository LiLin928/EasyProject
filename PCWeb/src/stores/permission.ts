// 权限状态管理
// 管理动态路由和菜单数据

import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { RouteRecordRaw } from 'vue-router'
import { getMenuList } from '@/api'
import { generateRoutes } from '@/router/dynamic'
import type { MockMenu } from '@/types/menu'

export const usePermissionStore = defineStore('permission', () => {
  // 动态路由列表
  const routes = ref<RouteRecordRaw[]>([])

  // 菜单数据
  const menus = ref<MockMenu[]>([])

  // 是否已加载
  const isLoaded = ref(false)

  /**
   * 获取菜单并生成路由
   * @returns 生成的路由配置数组
   */
  async function generateRoutesAction(): Promise<RouteRecordRaw[]> {
    // 如果已加载，直接返回缓存的路由
    if (isLoaded.value) {
      return routes.value
    }

    try {
      // 获取菜单数据
      const menuList = await getMenuList()
      menus.value = menuList

      // 生成路由配置
      const generatedRoutes = generateRoutes(menuList)
      routes.value = generatedRoutes

      // 标记已加载
      isLoaded.value = true

      return generatedRoutes
    } catch (error) {
      console.error('[Permission Store] Failed to generate routes:', error)
      // 不抛出异常，返回空数组作为降级处理
      // 标记已加载，避免重复尝试
      isLoaded.value = true
      return []
    }
  }

  /**
   * 重置路由状态
   */
  function resetRoutes() {
    routes.value = []
    menus.value = []
    isLoaded.value = false
  }

  return {
    routes,
    menus,
    isLoaded,
    generateRoutesAction,
    resetRoutes,
  }
})