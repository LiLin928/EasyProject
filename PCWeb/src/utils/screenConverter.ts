/**
 * 大屏数据格式转换工具
 * 用于前端嵌套对象格式与后端扁平字段格式之间的转换
 */

import type { ScreenConfig, ScreenComponent, ScreenStyle, ScreenPermissions } from '@/types'

/**
 * 后端返回的大屏详情格式
 */
export interface BackendScreenConfig {
  id: string
  name: string
  description?: string
  thumbnail?: string
  style: string | ScreenStyle  // 后端返回 JSON 字符串
  components: BackendScreenComponent[]
  permissions?: string | ScreenPermissions  // 后端返回 JSON 字符串
  isPublic: number
  createdBy?: string
  creator?: string
  createdAt?: string
  createTime?: string
  updatedAt?: string
  updateTime?: string
}

/**
 * 后端返回的组件格式（扁平字段）
 */
export interface BackendScreenComponent {
  id: string
  screenId?: string
  componentType: string
  type?: string  // 兼容前端格式
  positionX: number
  positionY: number
  width: number
  height: number
  rotation: number
  locked: number
  visible: number
  dataSource: string  // JSON 字符串
  config: string  // JSON 字符串
  styleConfig?: string  // JSON 字符串
  style?: string  // 兼容字段
  dataBinding: string  // JSON 字符串
  sortOrder?: number
  position?: { x: number; y: number }  // 兼容前端格式
  size?: { width: number; height: number }  // 兼容前端格式
}

/**
 * 将后端格式转换为前端格式
 */
export function convertScreenToFrontend(data: BackendScreenConfig): ScreenConfig {
  // 解析 style JSON 字符串
  let style: ScreenStyle
  if (typeof data.style === 'string') {
    try {
      style = JSON.parse(data.style)
    } catch (e) {
      style = { background: '#1a1a2e', width: 1920, height: 1080 }
    }
  } else {
    style = data.style
  }

  // 解析 permissions JSON 字符串
  let permissions: ScreenPermissions
  if (typeof data.permissions === 'string') {
    try {
      permissions = JSON.parse(data.permissions)
    } catch (e) {
      permissions = { sharedUsers: [], sharedRoles: [] }
    }
  } else {
    permissions = data.permissions || { sharedUsers: [], sharedRoles: [] }
  }

  // 转换组件格式
  const components = (data.components || []).map((comp) => convertComponentToFrontend(comp))

  return {
    id: data.id,
    name: data.name,
    description: data.description || '',
    thumbnail: data.thumbnail,
    style,
    components,
    permissions,
    isPublic: data.isPublic,
    createdBy: data.createdBy || data.creator || '',
    createdAt: data.createdAt || data.createTime || new Date().toISOString(),
    updatedAt: data.updatedAt || data.updateTime || new Date().toISOString(),
  }
}

/**
 * 将单个组件从后端格式转换为前端格式
 */
export function convertComponentToFrontend(comp: BackendScreenComponent): ScreenComponent {
  // 解析 dataSource JSON 字符串
  let dataSource: any
  if (typeof comp.dataSource === 'string') {
    try {
      dataSource = JSON.parse(comp.dataSource)
    } catch (e) {
      dataSource = { type: 'static', data: [] }
    }
  } else {
    dataSource = comp.dataSource || { type: 'static', data: [] }
  }

  // 解析 config JSON 字符串
  let config: Record<string, any>
  if (typeof comp.config === 'string') {
    try {
      config = JSON.parse(comp.config)
    } catch (e) {
      config = {}
    }
  } else {
    config = comp.config || {}
  }

  // 解析 styleConfig JSON 字符串
  let style: any
  const styleStr = comp.styleConfig || comp.style
  if (typeof styleStr === 'string') {
    try {
      style = JSON.parse(styleStr)
    } catch (e) {
      style = {}
    }
  } else {
    style = styleStr || {}
  }

  // 解析 dataBinding JSON 字符串
  let dataBinding: any
  if (typeof comp.dataBinding === 'string') {
    try {
      dataBinding = JSON.parse(comp.dataBinding)
    } catch (e) {
      dataBinding = {}
    }
  } else {
    dataBinding = comp.dataBinding || {}
  }

  return {
    id: comp.id,
    type: comp.componentType || comp.type,
    position: {
      x: comp.positionX ?? comp.position?.x ?? 0,
      y: comp.positionY ?? comp.position?.y ?? 0,
    },
    size: {
      width: comp.width ?? comp.size?.width ?? 400,
      height: comp.height ?? comp.size?.height ?? 300,
    },
    rotation: comp.rotation || 0,
    locked: comp.locked === 1 || comp.locked === true,
    visible: comp.visible !== 0 && comp.visible !== false,
    dataSource,
    config,
    style,
    dataBinding,
  }
}