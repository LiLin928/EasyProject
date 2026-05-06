// 路由入口

import { createRouter, createWebHashHistory } from 'vue-router'
import { routes } from './routes'
import { setupRouterGuards } from './guards'

const router = createRouter({
  history: createWebHashHistory(),
  routes,
  scrollBehavior: () => ({ top: 0 }),
})

// 设置路由守卫
setupRouterGuards(router)

export default router