// src/types/etl.ts

/**
 * 数据源类型枚举
 */
export enum DatasourceType {
  MYSQL = 'mysql',
  POSTGRESQL = 'postgresql',
  ORACLE = 'oracle',
  SQLSERVER = 'sqlserver',
  CLICKHOUSE = 'clickhouse',
}

/**
 * 数据源状态枚举（ETL专用）
 */
export enum EtlDatasourceStatus {
  ACTIVE = 'connected',
  INACTIVE = 'disconnected',
  ERROR = 'error',
}

/**
 * 管道状态枚举（字符串类型，与后端一致）
 */
export enum PipelineStatus {
  DRAFT = 'draft',
  PUBLISHED = 'published',
  ARCHIVED = 'archived',
}

/**
 * 调度状态枚举（字符串类型，与后端一致）
 */
export enum ScheduleStatus {
  ACTIVE = 'active',
  PAUSED = 'paused',
  STOPPED = 'stopped',
  ENABLED = 'active',   // 别名，与 ACTIVE 相同
  DISABLED = 'paused',  // 别名，与 PAUSED 相同
}

/**
 * 执行状态枚举（字符串类型，与后端一致）
 */
export enum ExecutionStatus {
  PENDING = 'pending',
  RUNNING = 'running',
  SUCCESS = 'success',
  FAILED = 'failure',
  FAILURE = 'failure',   // 别名，与 FAILED 相同
  CANCELLED = 'cancelled',
}

/**
 * 触发类型枚举（字符串类型，与后端一致）
 */
export enum TriggerType {
  MANUAL = 'manual',
  SCHEDULE = 'schedule',
  API = 'api',
}

/**
 * 数据源信息（与后端 DatasourceDto 对应）
 */
export interface DataSource {
  id: string
  name: string
  type: DatasourceType | string
  host: string
  port: number
  database: string
  username: string
  status: EtlDatasourceStatus | string
  lastConnectionTime?: string
  description?: string
  createTime?: string
  updateTime?: string
  config?: Record<string, any>
}

/**
 * 数据源列表查询参数
 */
export interface DataSourceListParams {
  pageIndex?: number
  pageSize?: number
  name?: string
  type?: DatasourceType | string
  status?: EtlDatasourceStatus | string
}

/**
 * 创建数据源参数
 */
export interface CreateDataSourceParams {
  name: string
  type: DatasourceType | string
  host: string
  port: number
  database: string
  username: string
  password: string
  description?: string
}

/**
 * 更新数据源参数
 */
export interface UpdateDataSourceParams {
  id: string
  name?: string
  host?: string
  port?: number
  database?: string
  username?: string
  password?: string
  description?: string
}

/**
 * 测试连接结果
 */
export interface TestConnectionResult {
  success: boolean
  message: string
  serverVersion?: string
  connectionTime?: number
}

/**
 * 数据库类型信息
 */
export interface DbTypeInfo {
  code: string
  name: string
  defaultPort: number
}

/**
 * 测试查询结果
 */
export interface TestQueryResult {
  success: boolean
  error?: string
  columns: string[]
  data: Record<string, any>[]
  rowCount: number
}

/**
 * 管道信息
 */
export interface Pipeline {
  id: string
  name: string
  description?: string
  status: PipelineStatus | string
  dagConfig?: DagConfig | string
  version?: number
  currentVersion?: string
  categoryCode?: string
  categoryName?: string
  creatorId?: string
  creatorName?: string
  createTime?: string
  updateTime?: string
  publishTime?: string
}

/**
 * 管道列表查询参数
 */
export interface PipelineQueryParams {
  pageIndex?: number
  pageSize?: number
  name?: string
  status?: PipelineStatus | string
}

/**
 * 创建管道参数
 */
export interface CreatePipelineParams {
  name: string
  description?: string
  dagConfig?: string
  categoryCode?: string
}

/**
 * 更新管道参数
 */
export interface UpdatePipelineParams {
  id: string
  name?: string
  description?: string
  dagConfig?: string
}

/**
 * DAG 配置
 */
export interface DagConfig {
  version?: string
  nodes: DagNode[]
  edges: DagEdge[]
}

/**
 * DAG 节点
 */
export interface DagNode {
  id: string
  type: string
  name?: string
  position: { x: number; y: number }
  config?: Record<string, any> | string
  // 扩展属性
  retryTimes?: number
  retryInterval?: number
  timeout?: number
  skipOnFailure?: boolean
}

/**
 * DAG 边
 */
export interface DagEdge {
  id: string
  source?: string
  target?: string
  sourceNodeId?: string
  targetNodeId?: string
  sourcePort?: string
  targetPort?: string
  condition?: EtlEdgeCondition
}

/**
 * 调度信息
 */
export interface Schedule {
  id: string
  name?: string
  pipelineId: string
  pipelineName?: string
  cronExpression: string
  cronDescription?: string
  status: ScheduleStatus | string
  lastExecutionTime?: string
  nextExecutionTime?: string
  description?: string
  config?: {
    type: ScheduleType
    cron?: { expression: string; timezone?: string }
    concurrency?: number
    maxRetryTimes?: number
    retryInterval?: number
    timeout?: number
  }
  createTime?: string
  updateTime?: string
}

/**
 * 调度列表查询参数
 */
export interface ScheduleQueryParams {
  pageIndex?: number
  pageSize?: number
  name?: string
  pipelineId?: string
  status?: ScheduleStatus | string
}

/**
 * 创建调度参数
 */
export interface CreateScheduleParams {
  name: string
  pipelineId: string
  scheduleType: string
  cronExpression?: string
  intervalSeconds?: number
  executeParams?: string
  enabled?: boolean
  description?: string
  maxRetries?: number
  retryInterval?: number
  timeout?: number
}

/**
 * 更新调度参数
 */
export interface UpdateScheduleParams {
  id: string
  name?: string
  cronExpression?: string
  intervalSeconds?: number
  executeParams?: string
  enabled?: boolean
}

/**
 * 执行记录（与后端 EtlExecutionDto 对应）
 */
export interface Execution {
  id: string
  pipelineId: string
  pipelineName?: string
  scheduleId?: string
  executionNo?: string
  status: ExecutionStatus | string
  triggerType: TriggerType | string
  triggerUserName?: string
  startTime?: string
  endTime?: string
  duration?: number
  progress?: number
  completedNodes?: number
  totalNodes?: number
  currentNodeId?: string
  currentNodeName?: string
  errorMessage?: string
  executeParams?: string
  result?: string
  nodes?: NodeExecution[]
  createTime?: string
}

/**
 * 节点执行记录
 */
export interface NodeExecution {
  nodeId: string
  nodeName: string
  nodeType: string
  status: string
  startTime?: string
  endTime?: string
  duration?: number
  input?: any
  output?: any
  error?: string
}

/**
 * 执行列表查询参数
 */
/**
 * 执行列表查询参数
 */
export interface ExecutionQueryParams {
  pageIndex?: number
  pageSize?: number
  pipelineId?: string
  status?: ExecutionStatus | string
  triggerType?: TriggerType | string
  startTime?: string
  endTime?: string
  dateStart?: string
  dateEnd?: string
}

/**
 * 调度类型（类型别名）
 */
export type ScheduleType = 'cron' | 'interval' | 'once'

/**
 * 调度任务（类型别名，与 Schedule 相同）
 */
export type ScheduleTask = Schedule

/**
 * 执行记录（类型别名，与 Execution 相同）
 */
export type ExecutionRecord = Execution

/**
 * 执行统计（与后端 EtlExecutionStatisticsDto 对应）
 */
export interface ExecutionStatistics {
  totalCount: number
  successCount: number
  failureCount: number
  runningCount: number
  pendingCount: number
  avgDuration?: number
  successRate?: number
}

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
  CONDITION = 'condition',
  PARALLEL = 'parallel',
  SCRIPT = 'script',
  SUBFLOW = 'subflow',
  NOTIFICATION = 'notification',
}

/**
 * 边条件（ETL专用）
 */
export interface EtlEdgeCondition {
  field?: string
  operator?: string
  value?: any
  // 组件使用的扩展属性
  expression?: string
  rules?: Array<{
    field: string
    operator: string
    value: any
  }>
}

/**
 * 数据源节点配置
 */
export interface DataSourceNodeConfig {
  datasourceId: string
  queryType?: 'table' | 'sql'
  tableName?: string
  sql?: string
  columns?: string[]
  whereClause?: string
  limit?: number
  outputVariable?: string
}

/**
 * SQL节点配置
 */
export interface SqlNodeConfig {
  datasourceId: string
  sql: string
  parameters?: SqlParameter[]
  // 组件使用的扩展属性
  sqlType?: 'query' | 'execute' | 'script'
  outputVariable?: string
}

/**
 * SQL参数
 */
export interface SqlParameter {
  name: string
  type: string
  value?: any
  defaultValue?: any
}

/**
 * 转换节点配置
 */
export interface TransformNodeConfig {
  transformType: string
  fieldMappings?: FieldMappingItem[]
  script?: string
  // 扩展属性
  inputVariable?: string
  outputVariable?: string
}

/**
 * 输出节点配置
 */
export interface OutputNodeConfig {
  outputType: string
  datasourceId?: string
  tableName?: string
  fieldMappings?: FieldMappingItem[]
  // 组件使用的扩展属性
  inputVariable?: string
}

/**
 * API节点配置
 */
export interface ApiNodeConfig {
  url: string
  method: string
  headers?: Record<string, string>
  body?: any
  fieldMappings?: FieldMappingItem[]
  // 组件使用的别名属性
  apiUrl?: string
  apiMethod?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  apiHeaders?: Record<string, string>
  apiBody?: string
  apiBodyType?: 'json' | 'form-data' | 'x-www-form-urlencoded'
  timeout?: number
  retryOnFailure?: boolean
  outputVariable?: string
  responseMapping?: {
    fieldPath?: string
    variableName?: string
    fields?: { sourceField: string; targetField: string }[]
  }
}

/**
 * 文件节点配置
 */
export interface FileNodeConfig {
  fileType: string
  path: string
  encoding?: string
  delimiter?: string
  fieldMappings?: FieldMappingItem[]
  // 组件使用的扩展属性
  fileOperation?: 'read' | 'write' | 'move' | 'copy' | 'delete'
  outputVariable?: string
  inputVariable?: string
  includeHeader?: boolean
  localPath?: string
  filePath?: string
  datasourceId?: string
  sheetName?: string
}

/**
 * 条件节点配置
 */
/**
 * 条件节点配置（ETL专用）
 */
export interface EtlConditionNodeConfig {
  conditions: EtlConditionRule[]
  branches: EtlConditionBranch[]
}

/**
 * 条件规则（ETL专用）
 */
export interface EtlConditionRule {
  field: string
  operator: string
  value: any
}

/**
 * 条件分支（ETL专用）
 */
export interface EtlConditionBranch {
  name: string
  condition?: EtlConditionRule[]
  // 组件使用的扩展属性
  id?: string
  isDefault?: boolean
}

/**
 * 并行节点配置（ETL专用）
 */
export interface EtlParallelNodeConfig {
  branches: EtlParallelBranch[]
  // 组件使用的扩展属性
  waitMode?: 'all' | 'any' | 'none'
}

/**
 * 并行分支（ETL专用）
 */
export interface EtlParallelBranch {
  name: string
  nodes?: string[]
  // 组件使用的扩展属性
  id?: string
}

/**
 * 脚本节点配置
 */
export interface ScriptNodeConfig {
  language: string
  script: string
  // 组件使用的扩展属性
  scriptType?: 'inline' | 'file'
  scriptPath?: string
  outputVariable?: string
}

/**
 * 子流程节点配置（ETL专用）
 */
export interface EtlSubflowNodeConfig {
  pipelineId: string
  fieldMappings?: FieldMappingItem[]
  // 扩展属性
  async?: boolean
}

/**
 * 通知节点配置（ETL专用）
 */
export interface EtlNotificationNodeConfig {
  notificationType: string
  recipients: string[]
  subject?: string
  template?: string
  // 组件使用的扩展属性
  triggerOn?: 'success' | 'failure' | 'always'
  emailConfig?: {
    subject?: string
    body?: string
    isHtml?: boolean
    attachments?: string[]
    recipients?: string[]
  }
  webhookConfig?: {
    url?: string
    method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
    headers?: Record<string, string>
    body?: string
  }
  messageConfig?: {
    recipients?: string[]
    title?: string
    content?: string
  }
  smsConfig?: {
    recipients?: string[]
    content?: string
  }
}

/**
 * 字段映射项
 */
export interface FieldMappingItem {
  sourceField: string
  targetField: string
  transform?: string
}