/**
 * ETL 调度类型定义
 */

/**
 * 调度状态枚举
 */
export enum ScheduleStatus {
  INACTIVE = 0,  // 已禁用
  ACTIVE = 1,    // 已启用
}

/**
 * 调度类型枚举
 */
export enum ScheduleType {
  CRON = 'cron',
  MANUAL = 'manual',
  DEPENDENCY = 'dependency',
  EVENT = 'event',
}

/**
 * Cron 配置
 */
export interface CronConfig {
  expression: string
  timezone?: string
  startTime?: string
  endTime?: string
}

/**
 * 依赖配置
 */
export interface DependencyConfig {
  pipelineIds: string[]
  condition: 'all_success' | 'any_success' | 'all_complete'
}

/**
 * 事件配置
 */
export interface EventConfig {
  eventType: 'datasource_change' | 'api_callback' | 'file_arrived'
  datasourceId?: string
  callbackUrl?: string
  filePath?: string
}

/**
 * 告警配置
 */
export interface AlertConfig {
  onSuccess?: boolean
  onFailure?: boolean
  onTimeout?: boolean
  notificationId?: string
}

/**
 * 调度配置
 */
export interface ScheduleConfig {
  type: ScheduleType
  cron?: CronConfig
  dependency?: DependencyConfig
  event?: EventConfig
  concurrency?: number
  maxRetryTimes?: number
  retryInterval?: number
  timeout?: number
  alertConfig?: AlertConfig
}

/**
 * 调度任务接口
 */
export interface ScheduleTask {
  id: string
  name?: string
  description?: string
  pipelineId: string
  pipelineName?: string
  config: ScheduleConfig
  scheduleType?: string
  cronExpression?: string
  cronDescription?: string
  status: ScheduleStatus
  lastExecuteTime?: string
  nextExecuteTime?: string
  creatorId: string
  creatorName?: string
  createTime: string
  updateTime: string
}

/**
 * 调度任务别名
 */
export type Schedule = ScheduleTask

/**
 * 调度任务查询参数
 */
export interface ScheduleQueryParams {
  pageIndex: number
  pageSize: number
  name?: string
  pipelineId?: string
  pipelineName?: string
  status?: ScheduleStatus | string
}

/**
 * 创建调度参数
 */
export interface CreateScheduleParams {
  name: string
  pipelineId: string
  scheduleType?: string
  cronExpression?: string
  intervalSeconds?: number
  executeParams?: string
}

/**
 * 更新调度参数
 */
export interface UpdateScheduleParams {
  id: string
  name?: string
  scheduleType?: string
  cronExpression?: string
  intervalSeconds?: number
  executeParams?: string
}

/**
 * Cron 预览结果
 */
export interface CronPreviewResult {
  expression: string
  description: string
  nextExecutions: string[]
}