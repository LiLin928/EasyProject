// types/basic.ts

/** 用户信息 */
export interface User {
  id: string
  userName: string
  realName?: string
  avatar?: string
  email?: string
  phone?: string
  status: UserStatus
  createTime?: string
  roles?: string[]
  roleIds?: string[]
  departmentId?: string
  departmentName?: string
}

/** 用户状态 */
export enum UserStatus {
  Disabled = 0,
  Enabled = 1,
}

/** 用户查询参数 */
export interface UserQueryParams {
  pageIndex: number
  pageSize: number
  keyword?: string
  status?: UserStatus
}

/** 角色信息 */
export interface Role {
  id: string
  name: string
  code: string
  description?: string
  status: RoleStatus
  createTime?: string
  permissions?: string[]
}

/** 角色状态 */
export enum RoleStatus {
  Disabled = 0,
  Enabled = 1,
}

/** 角色查询参数 */
export interface RoleQueryParams {
  pageIndex: number
  pageSize: number
  keyword?: string
}

/** 部门信息 */
export interface Department {
  id: string
  name: string
  parentId?: string
  children?: Department[]
  level: number
  sort: number
  leader?: string
  phone?: string
}

/** 菜单信息 */
export interface Menu {
  id: string
  name: string
  parentId?: string
  children?: Menu[]
  type: MenuType
  path?: string
  component?: string
  perms?: string
  icon?: string
  sort: number
  status: MenuStatus
}

/** 菜单类型 */
export enum MenuType {
  Directory = 0,
  Menu = 1,
  Button = 2,
}

/** 菜单状态 */
export enum MenuStatus {
  Hidden = 0,
  Visible = 1,
}

/** 字典类型 */
export interface DictType {
  id: string
  name: string
  code: string
  description?: string
  status: DictStatus
}

/** 字典数据 */
export interface DictData {
  id: string
  dictTypeId: string
  label: string
  value: string
  sort: number
  status: DictStatus
}

/** 字典状态 */
export enum DictStatus {
  Disabled = 0,
  Enabled = 1,
}