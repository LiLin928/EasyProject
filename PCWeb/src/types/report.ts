// src/types/report.ts

import type { ColumnConfig, DetectedColumn } from './columnTemplate'

/**
 * 图表类型
 */
export type ChartType = 'bar' | 'line' | 'pie'

/**
 * 聚合类型
 */
export type AggregationType = 'sum' | 'count' | 'avg'

/**
 * 报表信息
 */
export interface Report {
  id: string
  name: string
  category: string
  datasourceId: string
  datasourceName?: string
  sqlQuery: string
  // 显示配置
  showChart: boolean
  showTable: boolean
  chartType: ChartType
  xAxisField?: string
  yAxisField?: string
  aggregation: AggregationType
  // 列配置
  autoColumns?: boolean
  columnTemplateId?: string
  columnConfigs?: ColumnConfig[]
  // 审计信息
  creator?: string
  createTime?: string
  updateTime?: string
}

/**
 * 报表分类
 */
export interface ReportCategory {
  id: string
  name: string
  icon?: string
}

/**
 * 报表列表查询参数
 */
export interface ReportListParams {
  pageIndex?: number
  pageSize?: number
  name?: string
  category?: string
}

/**
 * 创建报表参数
 */
export interface CreateReportParams {
  name: string
  category: string
  datasourceId: string
  sqlQuery: string
  showChart?: boolean
  showTable?: boolean
  chartType?: ChartType
  xAxisField?: string
  yAxisField?: string
  aggregation?: AggregationType
  autoColumns?: boolean
  columnTemplateId?: string
  columnConfigs?: ColumnConfig[]
}

/**
 * 更新报表参数
 */
export interface UpdateReportParams {
  id: string
  name?: string
  category?: string
  datasourceId?: string
  sqlQuery?: string
  showChart?: boolean
  showTable?: boolean
  chartType?: string
  xAxisField?: string
  yAxisField?: string
  aggregation?: string
  autoColumns?: boolean
  columnTemplateId?: string
  columnConfigs?: ColumnConfig[]
}

/**
 * 报表预览参数
 */
export interface PreviewParams {
  datasourceId: string
  sqlQuery: string
  chartType?: ChartType
  xAxisField?: string
  yAxisField?: string
  aggregation?: AggregationType
}

/**
 * 图表数据项
 */
export interface ChartDataItem {
  name: string
  value: number
}

/**
 * 报表统计摘要
 */
export interface ReportSummary {
  total: number
  count: number
  avg: number
}

/**
 * 报表预览结果
 */
export interface PreviewResult {
  chartData: ChartDataItem[]
  tableData: Record<string, any>[]
  detectedColumns: DetectedColumn[]
  summary: ReportSummary
}