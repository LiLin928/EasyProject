/**
 * 大屏配置类型（与后端 ScreenConfigDto 结构一致）
 */
import type { CommonStatus, DatasourceStatus, ScreenPublishStatus } from './enums'

export interface ScreenConfig {
  id: string
  name: string
  description: string
  thumbnail?: string
  style: ScreenStyle
  components: ScreenComponent[]
  permissions: ScreenPermissions
  isPublic: number             // 是否公开（0-私有，1-公开），与后端一致
  createdBy: string
  createTime: string           // 与后端一致
  updateTime: string           // 与后端一致
}

// 大屏样式配置
export interface ScreenStyle {
  background: string  // 渐变背景 CSS
  width: number       // 默认 1920
  height: number      // 默认 1080
}

// 大屏组件
export interface ScreenComponent {
  id: string           // 组件唯一标识
  type: ComponentType  // 组件类型
  position: { x: number; y: number }
  size: { width: number; height: number }
  rotation?: number    // 旋转角度
  locked?: boolean     // 是否锁定
  visible?: boolean    // 是否可见
  dataSource: DataSource
  config: Record<string, any>  // 组件配置
  style?: ComponentStyleConfig  // 组件样式配置（新增）
  dataBinding?: DataBindingConfig  // 数据绑定配置（新增）
}

// 组件类型枚举（第二阶段扩展）
export type ComponentType =
  | 'line-chart'    // 折线图
  | 'bar-chart'     // 柱状图
  | 'pie-chart'     // 饼图
  | 'radar-chart'   // 雷达图（新增）
  | 'funnel-chart'  // 漏斗图（新增）
  | 'heatmap-chart' // 热力图（新增）
  | 'number-card'   // 数字卡片
  | 'data-table'    // 数据表格
  | 'title'         // 标题
  | 'border'        // 边框
  | 'image'         // 图片（新增）
  | 'video'         // 视频（新增）

// 数据源类型（支持多种数据源）
export type DataSourceType = 'static' | 'media' | 'api' | 'dataset'

// 数据源配置
export interface DataSource {
  type: DataSourceType
  // static 类型使用
  data?: any[]
  // media 类型使用
  url?: string
  // api 类型使用
  apiConfig?: ApiDataSourceConfig
  // dataset 类型使用（报表数据源SQL查询）
  datasetConfig?: DatasetSourceConfig
}

// API 数据源配置
export interface ApiDataSourceConfig {
  url: string                    // API 地址
  method: 'GET' | 'POST'         // 请求方法
  headers?: Record<string, string>  // 请求头
  params?: Record<string, any>   // 请求参数（GET）或请求体（POST）
  dataPath?: string              // 数据路径，如 'data.list'
  refreshInterval?: number       // 自动刷新间隔（秒），0 表示不刷新
}

// 报表数据源配置（SQL查询）
export interface DatasetSourceConfig {
  datasourceId: string           // 数据源ID
  sql: string                    // SQL 查询语句
  params?: SqlQueryParam[]       // SQL 参数
  refreshInterval?: number       // 自动刷新间隔（秒）
}

// SQL 查询参数
export interface SqlQueryParam {
  name: string                   // 参数名
  type: 'string' | 'number' | 'date' | 'datetime'  // 参数类型
  defaultValue?: any             // 默认值
  required?: boolean             // 是否必填
}

// 数据源选项（用于下拉选择）
export interface DatasourceOption {
  id: string
  name: string
  type: string                   // mysql, postgresql, oracle 等
}

// 权限配置（第二阶段完整权限）
export interface ScreenPermissions {
  sharedUsers: string[]       // 分享的用户 ID 列表（新增）
  sharedRoles: string[]       // 分享的角色 ID 列表（新增）
}

// 大屏列表查询参数
export interface ScreenListParams {
  pageIndex: number
  pageSize: number
  name?: string
  isPublic?: number  // 0=私有, 1=公开, 不传=全部
}

// 创建大屏参数（与后端 CreateScreenDto 结构一致）
export interface CreateScreenParams {
  name: string
  description?: string
  thumbnail?: string
  style?: string          // JSON 字符串
  permissions?: string    // JSON 字符串
  isPublic?: number       // 0=私有，1=公开
}

// 更新大屏参数（转换为后端格式）
export interface UpdateScreenParams {
  id: string
  name?: string
  description?: string
  style?: string | Partial<ScreenStyle>  // JSON字符串或对象
  components?: ConvertedScreenComponent[]  // 转换后的组件格式
  permissions?: string | ScreenPermissions  // JSON字符串或对象
}

// 转换后的组件格式（用于后端 API）
export interface ConvertedScreenComponent {
  id: string
  screenId: string
  componentType: string
  positionX: number
  positionY: number
  width: number
  height: number
  rotation: number
  locked: number
  visible: number
  dataSource: string  // JSON字符串
  config: string  // JSON字符串
  styleConfig: string  // JSON字符串
  dataBinding: string  // JSON字符串
  sortOrder: number
}

// 设计器状态类型
export interface ScreenDesignState {
  currentScreen: ScreenConfig | null       // 当前编辑的大屏配置
  selectedComponentId: string | null       // 选中的组件 ID
  canvasZoom: number                       // 画布缩放比例 (0.5 - 2)
  canvasOffset: { x: number; y: number }   // 画布平移偏移
  history: ScreenConfig[]                  // 操作历史（撤销栈）
  historyIndex: number                     // 当前历史位置
  isDirty: boolean                         // 是否有未保存的修改
}

// 组件默认配置
export interface ComponentDefaultConfig {
  type: ComponentType
  defaultSize: { width: number; height: number }
  defaultData: any[]
  defaultConfig: Record<string, any>
}

// 分享配置参数
export interface ShareScreenParams {
  id: string
  permissions: ScreenPermissions
}

// 分享用户/角色查询参数
export interface ShareQueryParams {
  id: string
}

// 组件样式配置（新增）
export interface ComponentStyleConfig {
  // 标题样式
  title?: {
    show?: boolean           // 是否显示标题
    text?: string            // 标题文本
    fontSize?: number        // 字体大小
    fontFamily?: string      // 字体
    color?: string           // 字体颜色
    fontWeight?: 'normal' | 'bold'  // 字体粗细
    position?: 'top' | 'bottom' | 'left' | 'right'  // 标题位置
    align?: 'left' | 'center' | 'right'  // 对齐方式
  }
  // 组件背景
  background?: {
    color?: string           // 背景颜色
    image?: string           // 背景图片
    opacity?: number         // 透明度 (0-100)
  }
  // 边框样式
  border?: {
    show?: boolean           // 是否显示边框
    color?: string           // 边框颜色
    width?: number           // 边框宽度
    style?: 'solid' | 'dashed' | 'dotted'  // 边框样式
    radius?: number          // 圆角大小
  }
  // 图表配色
  colors?: string[]          // 系列颜色列表
  // 图例样式
  legend?: {
    show?: boolean           // 是否显示图例
    position?: 'top' | 'bottom' | 'left' | 'right'  // 图例位置
    fontSize?: number        // 字体大小
    color?: string           // 字体颜色
  }
  // 标签样式
  label?: {
    show?: boolean           // 是否显示标签
    position?: 'inside' | 'outside' | 'top' | 'bottom' | 'left' | 'right'  // 标签位置
    fontSize?: number        // 字体大小
    color?: string           // 字体颜色
    formatter?: string       // 格式化模板，如 '{value}%'
  }
  // 坐标轴样式（折线图/柱状图）
  axis?: {
    xAxis?: AxisConfig
    yAxis?: AxisConfig
  }
}

// 坐标轴配置
export interface AxisConfig {
  show?: boolean           // 是否显示坐标轴
  name?: string            // 坐标轴名称
  nameColor?: string       // 名称颜色
  nameFontSize?: number    // 名称字体大小
  labelColor?: string      // 标签颜色
  labelFontSize?: number   // 标签字体大小
  lineColor?: string       // 轴线颜色
  splitLineColor?: string  // 分隔线颜色
  min?: number             // 最小值
  max?: number             // 最大值
}

// 数据绑定配置（新增）
export interface DataBindingConfig {
  // 字段映射
  fieldMapping?: {
    // 维度字段（X轴/名称）
    dimension?: string
    // 数值字段（Y轴/值）
    value?: string | string[]  // 单值或多值（多条折线）
    // 额外字段
    series?: string           // 系列字段（多条折线/多系列柱状图）
  }
  // 数据过滤
  filter?: {
    field?: string            // 过滤字段
    operator?: 'eq' | 'ne' | 'gt' | 'lt' | 'gte' | 'lte' | 'contains' | 'between'  // 操作符
    value?: any               // 过滤值
  }[]
  // 数据排序
  sort?: {
    field?: string            // 排序字段
    order?: 'asc' | 'desc'    // 排序方向
  }
  // 数据格式化
  formatter?: {
    number?: {
      decimals?: number       // 小数位数
      prefix?: string         // 前缀
      suffix?: string         // 后缀
      unit?: 'none' | 'thousand' | 'million' | 'billion' | 'percent'  // 单位换算
    }
    date?: {
      format?: string         // 日期格式，如 'YYYY-MM-DD'
    }
  }
}

// 发布记录类型
export interface ScreenPublishRecord {
  publishId: string              // 发布ID
  screenId: string               // 原大屏ID
  screenName: string             // 大屏名称
  screenDescription: string      // 大屏描述
  screenData: ScreenConfig       // 大屏快照数据（完整副本）
  publishUrl: string             // 发布访问URL
  publishedAt: string            // 发布时间
  publishedBy: string            // 发布人ID
  viewCount: number              // 访问次数
  status: ScreenPublishStatus    // 状态
}