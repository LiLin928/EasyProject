// 路由守卫

import type { Router } from 'vue-router'
import { getToken } from '@/utils/auth'
import { usePermissionStore } from '@/stores/permission'
import { useUserStore } from '@/stores/user'
import { useTagsViewStore } from '@/stores/tagsView'
import { notFoundRoute } from './routes'
import NProgress from 'nprogress'
import 'nprogress/nprogress.css'

// 配置 NProgress
NProgress.configure({ showSpinner: false })

// 白名单路由（无需登录）
const whiteList = ['/login', '/404']

// 公开页面路由前缀（不需要登录，可以公开访问）
const publicPrefixes = ['/report/publish/']

// 独立页面路由前缀（不需要 Layout，全屏展示，需要登录）
const standalonePrefixes = ['/screen/view/', '/screen/design/', '/screen/publish/', '/etl/design/', '/workflow/design', '/workflow-ant-design/']

// 检查是否为公开页面
function isPublicPage(path: string): boolean {
  return publicPrefixes.some(prefix => path.startsWith(prefix))
}

// 检查是否为独立页面
function isStandalonePage(path: string): boolean {
  return standalonePrefixes.some(prefix => path.startsWith(prefix))
}

export function setupRouterGuards(router: Router) {
  // 前置守卫
  router.beforeEach(async (to, from, next) => {
    NProgress.start()

    const token = getToken()

    // 白名单直接放行
    if (whiteList.includes(to.path)) {
      next()
      return
    }

    // 公开页面直接放行（不需要登录）
    if (isPublicPage(to.path)) {
      next()
      return
    }

    // 独立页面直接放行（需要登录但不需要动态路由）
    if (isStandalonePage(to.path)) {
      // 无 token 跳转登录页
      if (!token) {
        next({
          path: '/login',
          query: { redirect: to.fullPath },
        })
        return
      }
      next()
      return
    }

    // 无 token 跳转登录页
    if (!token) {
      next({
        path: '/login',
        query: { redirect: to.fullPath },
      })
      return
    }

    const permissionStore = usePermissionStore()

    // 动态路由未加载
    if (!permissionStore.isLoaded) {
      try {
        // 生成动态路由
        const routes = await permissionStore.generateRoutesAction()

        // 注册每个路由到 Layout 下（作为 children）
        routes.forEach(route => router.addRoute('Layout', route))

        // 添加 404 兜底路由
        router.addRoute(notFoundRoute)

        // 重新导航
        next({ ...to, replace: true })
      } catch (error) {
        console.error('[Router Guards] Failed to load dynamic routes:', error)

        // 加载失败，清除登录状态并跳转登录页
        const userStore = useUserStore()
        await userStore.logoutAction()

        next({
          path: '/login',
          query: { redirect: to.fullPath },
        })
      }
      return
    }

    next()
  })

  // 后置守卫
  router.afterEach((to) => {
    NProgress.done()

    // 独立页面不添加标签页
    if (isStandalonePage(to.path)) {
      return
    }

    // 添加标签页
    const tagsViewStore = useTagsViewStore()

    if (to.name && !to.meta.hidden) {
      tagsViewStore.addTag({
        path: to.path,
        name: to.name as string,
        title: to.meta.title as string || 'no-name',
        icon: to.meta.icon as string,
        affix: to.meta.affix as boolean,
        query: to.query as Record<string, string>,
        fullPath: to.fullPath,
      })
    }
  })
}