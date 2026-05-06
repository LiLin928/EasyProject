/**
 * ETL 任务节点类型定义
 */

/**
 * 任务节点类型枚举
 */
export enum TaskNodeType {
  DATASOURCE = 'datasource',
  SQL = 'sql',
  TRANSFORM = 'transform',
  OUTPUT = 'output',
  API = 'api',
  FILE = 'file',
  SCRIPT = 'script',
  CONDITION = 'condition',
  PARALLEL = 'parallel',
  NOTIFICATION = 'notification',
  SUBFLOW = 'subflow',
}

/**
 * 任务节点基础配置接口
 */
export interface TaskNodeConfig {
  // 由具体节点类型扩展
}

/**
 * SQL 参数
 */
export interface SqlParameter {
  name: string
  type: 'string' | 'number' | 'date' | 'variable'
  value: string
}

/**
 * 字段映射项
 */
export interface FieldMappingItem {
  sourceField: string
  targetField: string
  transform?: string
  dataType?: string
}

/**
 * 聚合项
 */
export interface AggregateItem {
  field: string
  function: 'sum' | 'avg' | 'count' | 'max' | 'min'
  alias?: string
}

/**
 * 条件分支
 */
export interface ConditionBranch {
  id: string
  name: string
  isDefault?: boolean
}

/**
 * 并行分支
 */
export interface ParallelBranch {
  id: string
  name: string
}

/**
 * 数据源节点配置
 */
export interface DataSourceNodeConfig extends TaskNodeConfig {
  datasourceId: string
  queryType: 'table' | 'sql'
  tableName?: string
  sql?: string
  columns?: string[]
  whereClause?: string
  limit?: number
  outputVariable: string
}

/**
 * SQL 节点配置
 */
export interface SqlNodeConfig extends TaskNodeConfig {
  datasourceId: string
  sqlType: 'query' | 'insert' | 'update' | 'delete' | 'ddl'
  sql: string
  parameters?: SqlParameter[]
  outputVariable?: string
}

/**
 * 数据转换节点配置
 */
export interface TransformNodeConfig extends TaskNodeConfig {
  transformType: 'mapping' | 'filter' | 'aggregate' | 'script'
  inputVariable: string
  outputVariable: string
  fieldMapping?: FieldMappingItem[]
  filterExpression?: string
  aggregateConfig?: {
    groupBy?: string[]
    aggregations: AggregateItem[]
  }
  script?: string
  scriptLanguage?: 'javascript' | 'python' | 'sql'
}

/**
 * 数据输出节点配置
 */
export interface OutputNodeConfig extends TaskNodeConfig {
  datasourceId: string
  outputType: 'insert' | 'update' | 'merge' | 'truncate_insert'
  tableName: string
  inputVariable: string
  fieldMapping?: FieldMappingItem[]
  batchSize?: number
  onConflict?: 'skip' | 'update' | 'error'
}

/**
 * API 节点配置
 */
export interface ApiNodeConfig extends TaskNodeConfig {
  apiUrl: string
  apiMethod: 'GET' | 'POST' | 'PUT' | 'DELETE'
  apiHeaders?: Record<string, string>
  apiBody?: string
  apiBodyType?: 'json' | 'form' | 'raw'
  timeout?: number
  retryOnFailure?: boolean
  outputVariable?: string
  responseMapping?: {
    fields: FieldMappingItem[]
  }
}

/**
 * 文件节点配置
 */
export interface FileNodeConfig extends TaskNodeConfig {
  fileOperation: 'read' | 'write' | 'delete' | 'move' | 'copy'
  fileType: 'csv' | 'excel' | 'json' | 'xml' | 'txt'
  datasourceId?: string
  filePath?: string
  localPath?: string
  sheetName?: string
  delimiter?: string
  encoding?: string
  outputVariable?: string
  inputVariable?: string
  includeHeader?: boolean
}

/**
 * 脚本节点配置
 */
export interface ScriptNodeConfig extends TaskNodeConfig {
  scriptType: 'shell' | 'python' | 'javascript'
  script: string
  scriptPath?: string
  parameters?: Record<string, string>
  envVariables?: Record<string, string>
  workingDirectory?: string
  outputVariable?: string
}

/**
 * 条件节点配置
 */
export interface ConditionNodeConfig extends TaskNodeConfig {
  conditionType: 'simple' | 'expression'
  inputVariable?: string
  conditions?: {
    rules: ConditionRule[]
    logic: 'and' | 'or'
  }
  expression?: string
  branches: ConditionBranch[]
}

/**
 * 并行节点配置
 */
export interface ParallelNodeConfig extends TaskNodeConfig {
  parallelType: 'fixed' | 'dynamic'
  branches?: ParallelBranch[]
  dynamicSource?: string
  waitMode?: 'all' | 'any'
}

/**
 * 通知节点配置
 */
export interface NotificationNodeConfig extends TaskNodeConfig {
  notificationType: 'email' | 'sms' | 'webhook' | 'message'
  emailConfig?: {
    recipients: string[]
    subject: string
    body: string
    attachments?: string[]
  }
  webhookConfig?: {
    url: string
    method: 'GET' | 'POST'
    headers?: Record<string, string>
    body?: string
  }
  messageConfig?: {
    recipients: string[]
    title: string
    content: string
  }
  triggerOn: 'always' | 'success' | 'failure'
}

/**
 * 子流程节点配置
 */
export interface SubflowNodeConfig extends TaskNodeConfig {
  pipelineId: string
  inputMapping?: FieldMappingItem[]
  outputMapping?: FieldMappingItem[]
  async?: boolean
}

/**
 * 条件规则（用于条件节点）
 */
export interface ConditionRule {
  field: string
  operator: 'eq' | 'ne' | 'gt' | 'lt' | 'gte' | 'lte' | 'contains' | 'regex'
  value: string
}