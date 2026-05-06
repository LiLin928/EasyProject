// 动态路由生成模块
// 将后端菜单数据转换为 Vue Router 路由配置

import type { RouteRecordRaw } from 'vue-router'
import type { MockMenu } from '@/types/menu'

// 使用 Vite glob import 获取所有视图组件
const modules = import.meta.glob('@/views/**/*.vue')

/**
 * 根据菜单数组生成路由配置
 * @param menus - 菜单数组
 * @returns 路由配置数组
 */
export function generateRoutes(menus: MockMenu[]): RouteRecordRaw[] {
  return menus.filter(m => m.status === 1).map(convertMenuToRoute).filter(Boolean) as RouteRecordRaw[]
}

/**
 * 将单个菜单转换为路由配置
 * @param menu - 菜单项
 * @param parentPath - 父菜单路径（用于路径转换）
 * @returns 路由配置对象或 null
 */
function convertMenuToRoute(menu: MockMenu, parentPath?: string): RouteRecordRaw | null {
  // 过滤禁用的菜单
  if (menu.status !== 1) {
    return null
  }

  // 构建路由元信息（menuName 作为 title，menuCode 作为 name）
  const meta = {
    title: menu.menuName,
    icon: menu.icon,
    hidden: menu.hidden === 1,
    keepAlive: true,
    affix: menu.affix === 1, // 固定标签页
  }

  // 计算当前路由的路径
  // 如果有父路径且当前路径以父路径开头，转换为相对路径
  let currentPath = menu.path || ''
  if (parentPath && currentPath.startsWith(parentPath + '/')) {
    currentPath = currentPath.slice(parentPath.length + 1)
  }

  // 基础路由配置（menuCode 作为路由 name）
  const route: RouteRecordRaw = {
    path: currentPath,
    name: menu.menuCode || undefined,
    meta,
  }

  // 处理组件加载
  route.component = loadComponent(menu.component, Boolean(menu.children && menu.children.length > 0))

  // 递归处理子菜单（传入当前菜单的完整路径作为子菜单的父路径）
  if (menu.children && menu.children.length > 0) {
    route.children = menu.children
      .filter(child => child.status === 1)
      .map(child => convertMenuToRoute(child, menu.path || undefined))
      .filter(Boolean) as RouteRecordRaw[]
  }

  return route
}

/**
 * 动态加载组件
 * @param path - 组件路径（相对于 views 目录）
 * @param hasChildren - 是否有子菜单
 * @returns 组件加载函数
 */
function loadComponent(path: string | null, hasChildren: boolean = false) {
  // 有子菜单的父路由不需要加载组件，返回空的 router-view 容器
  if (hasChildren) {
    return () => import('@/views/layout/EmptyContainer.vue')
  }

  // 无组件路径，返回 404 页面
  if (!path) {
    return () => import('@/views/error/404.vue')
  }

  // 标准化路径格式
  // 支持多种格式: 'system/user', '/system/user', 'views/system/user', '/views/system/user'
  let normalizedPath = path

  // 移除开头的斜杠
  if (normalizedPath.startsWith('/')) {
    normalizedPath = normalizedPath.slice(1)
  }

  // 移除 views/ 前缀（支持后端返回带 views/ 的路径）
  if (normalizedPath.startsWith('views/')) {
    normalizedPath = normalizedPath.slice(6)
  }

  // 移除结尾的 .vue 后缀
  if (normalizedPath.endsWith('.vue')) {
    normalizedPath = normalizedPath.slice(0, -4)
  }

  // 尝试多种路径格式匹配
  const possiblePaths = [
    `/src/views/${normalizedPath}.vue`,
    `/src/views/${normalizedPath}/index.vue`,
  ]

  // 查找匹配的组件
  for (const possiblePath of possiblePaths) {
    if (modules[possiblePath]) {
      return modules[possiblePath]
    }
  }

  // 如果直接路径没找到，尝试模糊匹配
  const moduleKeys = Object.keys(modules)
  for (const key of moduleKeys) {
    // 匹配路径结尾
    if (key.endsWith(`/${normalizedPath}.vue`) || key.endsWith(`/${normalizedPath}/index.vue`)) {
      return modules[key]
    }
  }

  // 找不到组件时，返回 404 页面
  console.warn(`[Dynamic Router] Component not found: ${path}`)
  return () => import('@/views/error/404.vue')
}

/**
 * 扁平化路由（将嵌套路由转为一级路由）
 * @param routes - 路由配置数组
 * @returns 扁平化后的路由数组
 */
export function flattenRoutes(routes: RouteRecordRaw[]): RouteRecordRaw[] {
  const result: RouteRecordRaw[] = []

  function flatten(routeList: RouteRecordRaw[], parentPath = '') {
    routeList.forEach(route => {
      const currentPath = parentPath + route.path

      if (route.children && route.children.length > 0) {
        // 有子路由时，递归处理
        flatten(route.children, currentPath)
      } else {
        // 无子路由，添加到结果
        result.push({
          ...route,
          path: currentPath,
        })
      }
    })
  }

  flatten(routes)
  return result
}