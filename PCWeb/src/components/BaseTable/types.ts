// BaseTable 类型定义

/**
 * 表格列配置
 */
export interface TableColumn {
  prop: string
  label: string
  minWidth?: number
  width?: number
  align?: 'left' | 'center' | 'right'
  sortable?: boolean
  fixed?: 'left' | 'right' | boolean
  showOverflowTooltip?: boolean
}