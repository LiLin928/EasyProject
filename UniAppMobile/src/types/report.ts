/**
 * 报表类型定义
 */

/**
 * 图表类型枚举
 */
export enum ChartType {
  Line = 1,      // 折线图
  Bar = 2,       // 柱状图
  Pie = 3,       // 饼图
  Table = 4,     // 数据表格
  Mixed = 5,     // 混合图表
}

/**
 * 时间类型枚举
 */
export enum TimeType {
  Today = 1,
  Yesterday = 2,
  Week = 3,
  Month = 4,
  Quarter = 5,
  Year = 6,
  Custom = 7,
}

/**
 * 报表信息
 */
export interface Report {
  id: string              // GUID 主键
  name: string            // 报表名称
  categoryId: string      // 分类ID
  categoryName?: string   // 分类名称
  description?: string    // 报表描述
  chartType: ChartType    // 图表类型
  dataSource?: string     // 数据源
  createTime?: string     // 创建时间
  updateTime?: string     // 更新时间
}

/**
 * 报表分类
 */
export interface ReportCategory {
  id: string
  name: string
  parentId?: string
  children?: ReportCategory[]
}

/**
 * 报表查询参数
 */
export interface ReportQueryParams {
  pageIndex?: number
  pageSize?: number
  keyword?: string
  categoryId?: string
  chartType?: ChartType
}

/**
 * 报表数据响应
 */
export interface ReportData {
  dimensions: string[]      // 维度字段
  values: number[]          // 数值字段
  timeRange?: {
    start: string
    end: string
  }
}

/**
 * 报表数据查询参数
 */
export interface ReportDataParams {
  reportId: string
  startTime?: string        // 开始时间
  endTime?: string          // 结束时间
  timeType?: TimeType       // 时间类型
}