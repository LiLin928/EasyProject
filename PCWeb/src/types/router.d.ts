import 'vue-router'

declare module 'vue-router' {
  interface RouteMeta {
    /** 路由标题 */
    title?: string
    /** 路由图标 */
    icon?: string
    /** 是否隐藏菜单 */
    hidden?: boolean
    /** 是否缓存页面 */
    keepAlive?: boolean
    /** 是否需要登录 */
    requiresAuth?: boolean
    /** 是否固定在标签栏 */
    affix?: boolean
  }
}