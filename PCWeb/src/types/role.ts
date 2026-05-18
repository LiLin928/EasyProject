import type { CommonStatus } from './enums'

/**
 * 角色列表查询参数
 */
export interface RoleListParams {
  pageIndex: number
  pageSize: number
  roleName?: string
  roleCode?: string
  status?: CommonStatus
}

/**
 * 角色信息
 */
export interface RoleInfo {
  id: string
  roleName: string
  roleCode: string
  description: string
  status: CommonStatus
  createTime: string
}

/**
 * 创建角色参数
 */
export interface CreateRoleParams {
  roleName: string
  roleCode?: string
  description?: string
}

/**
 * 更新角色参数
 */
export interface UpdateRoleParams {
  id: string
  roleName?: string
  roleCode?: string
  description?: string
  status?: CommonStatus
}