// src/components/SearchForm/types.ts

/**
 * 搜索表单项配置
 */
export interface SearchFormItem {
  field: string
  label: string
  type: 'input' | 'select' | 'date' | 'dateRange' | 'cascader' | 'number'

  // 类型相关配置
  placeholder?: string
  options?: Array<{ label: string; value: any }>
  props?: Record<string, any>  // 传递给组件的额外属性

  // 显示控制
  span?: number           // 栅格占位
  defaultValue?: any      // 默认值

  // 是否在折叠区域
  collapse?: boolean
}

/**
 * SearchForm Props
 */
export interface SearchFormProps {
  items: SearchFormItem[]
  modelValue: Record<string, any>
  collapsed?: boolean      // 是否折叠
  showCollapse?: boolean   // 是否显示折叠按钮
  collapseCount?: number   // 折叠时显示的表单项数量
  labelWidth?: string
  loading?: boolean
}

/**
 * SearchForm Emits
 */
export interface SearchFormEmits {
  (e: 'update:modelValue', value: Record<string, any>): void
  (e: 'search'): void
  (e: 'reset'): void
}