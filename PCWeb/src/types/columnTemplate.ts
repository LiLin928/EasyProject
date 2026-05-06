// src/types/columnTemplate.ts

/**
 * 列配置模板类型
 */
export type ColumnTemplateType = 'single' | 'table'

/**
 * 钻取参数映射
 */
export interface DrilldownParam {
  sourceField: string // 来源字段（点击值用 '__clicked_value__'）
  targetParam: string // 目标参数名
}

/**
 * 钻取规则
 */
export interface DrilldownRule {
  enabled: boolean
  targetReportId: string
  params: DrilldownParam[]
}

/**
 * 列配置
 */
export interface ColumnConfig {
  field: string
  label: string
  width?: number
  align?: 'left' | 'center' | 'right'
  format?: string
  drilldown?: DrilldownRule
}

/**
 * 列配置模板
 */
export interface ColumnTemplate {
  id: string
  name: string
  type: ColumnTemplateType
  description?: string
  dataSourceId?: string      // 关联的数据源ID
  sqlQuery?: string          // SQL查询语句（用于自动获取列）
  columns: ColumnConfig[]
  createTime: string
  updateTime: string
}

/**
 * 创建列配置模板参数
 */
export interface CreateColumnTemplateParams {
  name: string
  type: ColumnTemplateType
  description?: string
  dataSourceId?: string
  sqlQuery?: string
  columns: ColumnConfig[]
}

/**
 * 更新列配置模板参数
 */
export interface UpdateColumnTemplateParams extends CreateColumnTemplateParams {
  id: string
}

/**
 * 列配置模板列表查询参数
 */
export interface ColumnTemplateListParams {
  pageIndex?: number
  pageSize?: number
  name?: string
  type?: ColumnTemplateType
  dataSourceId?: string
}

/**
 * 钻取路径
 */
export interface DrilldownPath {
  reportId: string
  reportName: string
  params: Record<string, any>
}

/**
 * 检测到的列信息
 */
export interface DetectedColumn {
  field: string
  type: string
}

/**
 * 获取列配置参数
 */
export interface FetchColumnsParams {
  dataSourceId: string
  sqlQuery: string
}