// 文件: PCWeb/src/types/desktopWidget.ts

/**
 * 组件类型枚举
 */
export enum WidgetType {
  Card = 1,      // 统计卡片
  List = 2,      // 数据列表
  Image = 3,     // 图片展示
  Chart = 4      // 图表统计
}

/**
 * 数据源类型枚举
 */
export enum DataSourceType {
  Api = 1,       // API接口
  Static = 2,    // 静态配置
  Statistics = 3 // 实时统计
}

/**
 * 组件类型标签映射
 */
export const widgetTypeLabels: Record<WidgetType, string> = {
  [WidgetType.Card]: '统计卡片',
  [WidgetType.List]: '数据列表',
  [WidgetType.Image]: '图片展示',
  [WidgetType.Chart]: '图表统计',
}

/**
 * 数据源类型标签映射
 */
export const dataSourceTypeLabels: Record<DataSourceType, string> = {
  [DataSourceType.Api]: 'API接口',
  [DataSourceType.Static]: '静态配置',
  [DataSourceType.Statistics]: '实时统计',
}

/**
 * 桌面组件信息
 */
export interface DesktopWidget {
  id: string
  name: string
  type: WidgetType
  icon?: string
  defaultWidth: number
  defaultHeight: number
  dataSourceType: DataSourceType
  dataSourceConfig?: string
  interactionConfig?: string
  status: number
  createTime: string
  updateTime?: string
}

/**
 * 组件列表查询参数
 */
export interface QueryDesktopWidgetParams {
  pageIndex?: number
  pageSize?: number
  name?: string
  type?: WidgetType
  status?: number
}

/**
 * 新增组件参数
 */
export interface AddDesktopWidgetParams {
  name: string
  type: WidgetType
  icon?: string
  defaultWidth: number
  defaultHeight: number
  dataSourceType: DataSourceType
  dataSourceConfig?: string
  interactionConfig?: string
  status: number
}

/**
 * 更新组件参数
 */
export interface UpdateDesktopWidgetParams extends AddDesktopWidgetParams {
  id: string
}

/**
 * 栅格宽度选项
 */
export const gridWidthOptions = [
  { value: 1, label: '1栅格 (8.33%)' },
  { value: 2, label: '2栅格 (16.67%)' },
  { value: 3, label: '3栅格 (25%)' },
  { value: 4, label: '4栅格 (33.33%)' },
  { value: 6, label: '6栅格 (50%)' },
  { value: 8, label: '8栅格 (66.67%)' },
  { value: 12, label: '12栅格 (100%)' },
]

/**
 * 刷新频率选项
 */
export const refreshIntervalOptions = [
  { value: 0, label: '手动刷新' },
  { value: 30, label: '30秒' },
  { value: 60, label: '1分钟' },
  { value: 300, label: '5分钟' },
  { value: 600, label: '10分钟' },
]