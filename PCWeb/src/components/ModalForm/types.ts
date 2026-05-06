// src/components/ModalForm/types.ts

import type { FormItemRule } from 'element-plus'

/**
 * 表单项配置
 */
export interface ModalFormItem {
  field: string
  label: string
  type: 'input' | 'textarea' | 'select' | 'radio' | 'checkbox' | 'switch' | 'date' | 'number' | 'slot'

  // 类型相关配置
  placeholder?: string
  options?: Array<{ label: string; value: any }>
  props?: Record<string, any>

  // 显示控制
  span?: number           // 栅格占位
  hideInCreate?: boolean  // 新增时隐藏
  hideInEdit?: boolean    // 编辑时隐藏
  disabled?: boolean

  // 验证规则
  rules?: FormItemRule[]
}

/**
 * ModalForm Props
 */
export interface ModalFormProps {
  modelValue: boolean
  title?: string
  items: ModalFormItem[]
  formData: Record<string, any>
  mode: 'create' | 'edit'
  width?: string
  loading?: boolean
  labelWidth?: string

  // 自定义提交按钮文字
  submitText?: string
  cancelText?: string
}

/**
 * ModalForm Emits
 */
export interface ModalFormEmits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'submit', formData: Record<string, any>): void
  (e: 'cancel'): void
}