/**
 * 选择器类型定义
 * 用于用户角色选择器组件
 */

/**
 * 选择器目标类型枚举
 */
export enum SelectorTargetType {
  USER = 'user',           // 用户
  ROLE = 'role',           // 角色
  DEPT = 'dept',           // 部门
  POST = 'post',           // 岗位
  FORM_FIELD = 'formField', // 表单字段
}

/**
 * 选择器目标项（统一结构）
 */
export interface SelectorTarget {
  /** 目标ID */
  id: string
  /** 显示名称 */
  name: string
  /** 目标类型 */
  type: SelectorTargetType
  /** 用户头像URL（可选） */
  avatar?: string
  /** 所属部门名称（可选） */
  deptName?: string
  /** 岗位ID（可选） */
  postId?: string
}

/**
 * 选择器组件 Props
 */
export interface UserRoleSelectorProps {
  /** 已选择的项 */
  selected?: SelectorTarget[]
  /** 选择模式，默认 multiple */
  mode?: 'single' | 'multiple'
  /** 允许的类型，默认全部 */
  allowedTypes?: SelectorTargetType[]
  /** 占位符 */
  placeholder?: string
  /** 禁用状态 */
  disabled?: boolean
}