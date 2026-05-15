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

/**
 * 角色组件配置项
 */
export interface RoleWidgetConfigItem {
  widgetId: string
  sortOrder: number
  isEnabled: boolean
}

/**
 * 角色组件配置
 */
export interface RoleWidgetConfig {
  id: string
  roleId: string
  widgetId: string
  widgetName: string
  widgetType: WidgetType
  defaultWidth: number
  defaultHeight: number
  sortOrder: number
  isEnabled: boolean
}

/**
 * 保存角色配置参数
 */
export interface SaveRoleWidgetConfigParams {
  roleId: string
  widgets: RoleWidgetConfigItem[]
}

/**
 * 可用组件DTO
 */
export interface AvailableWidget {
  id: string
  name: string
  type: number
  icon?: string
  defaultWidth: number
  defaultHeight: number
  isUserEnabled: boolean
}

/**
 * 用户组件配置项
 */
export interface UserWidgetConfigItem {
  widgetId: string
  width: number
  isEnabled: boolean
  sortOrder: number
}

/**
 * 用户组件配置DTO（完整配置，包含组件详情）
 */
export interface UserWidgetConfigDto {
  id: string
  widgetId: string
  widgetName: string
  widgetType: WidgetType
  icon?: string
  width: number
  height: number
  sortOrder: number
  isEnabled: boolean
  dataSourceType: DataSourceType
  dataSourceConfig?: string
}

/**
 * 保存用户配置参数
 */
export interface SaveUserWidgetConfigParams {
  widgets: UserWidgetConfigItem[]
}

/**
 * 用户桌面DTO
 */
export interface UserDesktopDto {
  widgets: UserWidgetConfigDto[]
  availableWidgets: AvailableWidget[]
}

/**
 * 组件数据响应（用于不同类型组件的数据）
 */
export interface WidgetDataResponse {
  /** 卡片类型：显示的数值 */
  value?: number
  /** 卡片类型：显示的标签 */
  label?: string
  /** 列表类型：列表数据 */
  list?: WidgetListItem[]
  /** 图表类型：图表数据 */
  chartData?: ChartDataConfig
  /** 图片类型：图片列表 */
  images?: string[]
}

/**
 * 组件列表项
 */
export interface WidgetListItem {
  id: string
  name: string
  status?: number
  statusLabel?: string
  time?: string
}

/**
 * 图表数据配置
 */
export interface ChartDataConfig {
  /** 图表类型：bar/line/pie */
  type: 'bar' | 'line' | 'pie'
  /** 图表标题 */
  title?: string
  /** X轴数据（柱状图、折线图） */
  xAxis?: string[]
  /** Y轴数据（柱状图、折线图） */
  yAxis?: number[]
  /** 饼图数据 */
  pieData?: PieDataItem[]
}

/**
 * 饼图数据项
 */
export interface PieDataItem {
  name: string
  value: number
}

/**
 * 组件刷新状态
 */
export interface RefreshState {
  /** 组件ID */
  widgetId: string
  /** 是否正在刷新 */
  loading: boolean
  /** 上次刷新时间 */
  lastRefreshTime?: Date
  /** 刷新错误信息 */
  error?: string
}

/**
 * 扩展的组件数据响应（包含趋势数据）
 */
export interface ExtendedWidgetDataResponse extends WidgetDataResponse {
  /** 趋势方向：up/down/flat */
  trend?: 'up' | 'down' | 'flat'
  /** 趋势值 */
  trendValue?: number
  /** 图表系列数据格式 */
  seriesData?: ChartSeriesItem[]
}

/**
 * 图表系列数据项
 */
export interface ChartSeriesItem {
  /** 系列名称 */
  name: string
  /** 数据值列表 */
  data: number[]
  /** 系列类型 */
  type?: 'bar' | 'line'
}