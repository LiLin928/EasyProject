import type { CommonStatus, MenuVisibility } from './enums'

// 菜单项（与后端 MenuDto 字段对应）
export interface MenuItem {
  id: string
  parentId: string | null
  menuName: string          // 显示标题
  menuCode: string | null   // 路由名称
  path: string | null
  component: string | null
  icon: string | null
  sort: number
  type: number              // 1-菜单，2-按钮
  status: CommonStatus
  hidden: number            // 0-显示，1-隐藏
  affix: number             // 0-可关闭，1-固定
  createTime: string
  updateTime: string | null
  children?: MenuItem[]
}

// Mock 菜单类型（兼容旧代码，字段名与后端一致）
export interface MockMenu {
  id: string
  parentId: string | null
  menuName: string          // 显示标题
  menuCode: string | null   // 路由名称
  path: string | null
  component: string | null
  icon: string | null
  sort: number
  type: number
  status: CommonStatus
  hidden: number
  affix: number
  createTime: string
  updateTime: string | null
  children?: MockMenu[]
}

// 路由元信息
export interface RouteMeta {
  title: string
  icon?: string
  hidden?: boolean
  keepAlive?: boolean
  requiresAuth?: boolean
  affix?: boolean // 固定标签页（不可关闭）
}

/**
 * 菜单列表查询参数
 */
export interface MenuListParams {
  name?: string
  status?: CommonStatus
}

/**
 * 创建菜单参数
 */
export interface CreateMenuParams {
  parentId: string | null
  menuName: string
  menuCode?: string | null
  path?: string | null
  component?: string | null
  icon?: string | null
  sort?: number
  type?: number
  status?: CommonStatus
  hidden?: number
  affix?: number
}

/**
 * 更新菜单参数
 */
export interface UpdateMenuParams {
  id: string
  parentId?: string | null
  menuName?: string
  menuCode?: string | null
  path?: string | null
  component?: string | null
  icon?: string | null
  sort?: number
  status?: CommonStatus
  hidden?: number
  affix?: number
}