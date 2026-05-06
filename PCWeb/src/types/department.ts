/**
 * 部门信息类型定义
 */

/**
 * 部门信息接口
 */
export interface Department {
  /** 部门ID (GUID) */
  id: string
  /** 上级部门ID */
  parentId: string | null
  /** 部门名称 */
  name: string
  /** 部门编码 */
  code: string | null
  /** 部门完整路径 */
  fullPath: string | null
  /** 部门层级 */
  level: number
  /** 部门主管ID */
  managerId: string | null
  /** 部门负责人姓名 */
  leaderName: string | null
  /** 联系电话 */
  phone: string | null
  /** 部门邮箱 */
  email: string | null
  /** 部门描述 */
  description: string | null
  /** 排序 */
  sort: number
  /** 状态：1-正常，0-禁用 */
  status: number
  /** 成员数量 */
  memberCount: number
  /** 创建时间 */
  createTime: string
  /** 更新时间 */
  updateTime: string | null
  /** 子部门列表 */
  children?: Department[]
}

/**
 * 新增部门参数
 */
export interface AddDepartmentParams {
  /** 上级部门ID */
  parentId?: string | null
  /** 部门名称 */
  name: string
  /** 部门编码 */
  code?: string
  /** 部门主管ID */
  managerId?: string | null
  /** 联系电话 */
  phone?: string
  /** 部门邮箱 */
  email?: string
  /** 部门描述 */
  description?: string
  /** 排序 */
  sort?: number
  /** 状态 */
  status?: number
}

/**
 * 更新部门参数
 */
export interface UpdateDepartmentParams extends AddDepartmentParams {
  /** 部门ID */
  id: string
  /** 上级部门ID */
  parentId?: string | null
  /** 部门名称 */
  name: string
  /** 排序 */
  sort: number
  /** 状态 */
  status: number
}