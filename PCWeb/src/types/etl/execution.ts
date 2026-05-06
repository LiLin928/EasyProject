/**
 * ETL 执行记录类型定义
 */
import type { DagConfig } from './pipeline'
import type { TaskNodeType } from './taskNode'

/**
 * 执行状态枚举
 */
export enum ExecutionStatus {
  PENDING = 'pending',
  RUNNING = 'running',
  SUCCESS = 'success',
  FAILURE = 'failure',
  TIMEOUT = 'timeout',
  CANCELLED = 'cancelled',
  RETRYING = 'retrying',
}

/**
 * 触发类型枚举
 */
export enum TriggerType {
  MANUAL = 'manual',
  SCHEDULE = 'schedule',
  DEPENDENCY = 'dependency',
  EVENT = 'event',
  API = 'api',
}

/**
 * 单个任务节点执行详情
 */
export interface TaskExecution {
  nodeId: string
  nodeName: string
  nodeType: TaskNodeType
  status: ExecutionStatus
  startTime?: string
  endTime?: string
  duration?: number
  retryCount?: number
  inputVariables?: Record<string, any>
  outputVariables?: Record<string, any>
  logs?: string
  errorMessage?: string
}

/**
 * 任务执行记录
 */
export interface ExecutionRecord {
  id: string
  executionNo?: string
  pipelineId: string
  pipelineName?: string
  scheduleId?: string
  triggerType: TriggerType | string
  triggerUserId?: string
  triggerUserName?: string
  status: ExecutionStatus | string
  startTime?: string
  endTime?: string
  duration?: number
  progress?: number
  dagSnapshot: DagConfig
  taskExecutions: TaskExecution[]
  errorMessage?: string
  logs?: string
  createTime: string
}

/**
 * 执行统计
 */
export interface ExecutionStatistics {
  total: number
  running: number
  success: number
  failure: number
  pending: number
  avgDuration?: number
  successRate?: number
}

/**
 * 执行记录别名
 */
export type Execution = ExecutionRecord

/**
 * 执行记录查询参数
 */
export interface ExecutionQueryParams {
  pageIndex: number
  pageSize: number
  pipelineId?: string
  pipelineName?: string
  status?: ExecutionStatus | string
  triggerType?: TriggerType | string
  dateStart?: string
  dateEnd?: string
}