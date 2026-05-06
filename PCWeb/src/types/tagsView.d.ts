/**
 * 标签页信息
 */
export interface TagView {
  path: string
  name: string
  title: string
  icon?: string
  affix?: boolean
  query?: Record<string, string>
  fullPath?: string
}

/**
 * 右键菜单项
 */
export interface ContextMenuItem {
  label: string
  icon?: string
  divider?: boolean
  disabled?: boolean
  handler: () => void
}