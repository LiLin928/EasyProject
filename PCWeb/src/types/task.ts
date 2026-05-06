// PCWeb/src/types/task.ts

/**
 * 任务定义
 */
export interface TaskDefinition {
  id: string
  taskName: string
  taskGroup: string
  taskType: number
  taskTypeText: string
  scheduleType?: number
  cronExpression?: string
  executeTime?: string
  dayOfMonth?: number
  executeHour?: number
  executeMinute?: number
  executorType: number
  handlerType?: string
  handlerMethod?: string
  apiEndpoint?: string
  apiMethod?: string
  apiPayload?: string
  status: number
  statusText: string
  maxRetries: number
  timeoutSeconds: number
  description?: string
  nextExecuteTime?: string
  lastExecuteTime?: string
  createTime: string
  updateTime?: string
}

/**
 * 任务查询参数
 */
export interface QueryTaskParams {
  pageIndex: number
  pageSize: number
  taskName?: string
  taskType?: number
  status?: number
  taskGroup?: string
  startTime?: string
  endTime?: string
}

/**
 * 任务更新参数
 */
export interface UpdateTaskParams {
  id?: string
  taskName: string
  taskGroup: string
  taskType: number
  scheduleType?: number
  cronExpression?: string
  executeTime?: string
  dayOfMonth?: number
  executeHour?: number
  executeMinute?: number
  executorType: number
  handlerType?: string
  handlerMethod?: string
  apiEndpoint?: string
  apiMethod?: string
  apiPayload?: string
  maxRetries: number
  timeoutSeconds: number
  description?: string
}

/**
 * 任务执行日志
 */
export interface TaskExecutionLog {
  id: string
  jobName: string
  jobGroup: string
  status: number
  statusText: string
  startTime: string
  endTime?: string
  duration?: number
  resultMessage?: string
  exceptionMessage?: string
  exceptionStackTrace?: string
  triggerType: number
  instanceId?: string
  createTime: string
}

/**
 * 日志查询参数
 */
export interface QueryTaskLogParams {
  pageIndex: number
  pageSize: number
  jobName?: string
  status?: number
  triggerType?: number
  startTime?: string
  endTime?: string
}

/**
 * 任务统计
 */
export interface TaskStatistics {
  totalCount: number
  enabledCount: number
  pausedCount: number
  todayExecuted: number
  todaySuccess: number
  todayFailure: number
}

/**
 * 执行趋势数据点
 */
export interface TaskLogTrendPoint {
  date: string
  executeCount: number
  successCount: number
  successRate: number
}

/**
 * 执行趋势
 */
export interface TaskLogTrend {
  points: TaskLogTrendPoint[]
}

// 任务类型枚举
export const TaskType = {
  Cron: 0,
  Immediate: 1,
  Periodic: 2,
}

// 任务状态枚举
export const TaskStatus = {
  Pending: 0,
  Scheduled: 1,
  Paused: 2,
  Completed: 3,
  Failed: 4,
}

// 执行器类型枚举
export const ExecutorType = {
  Reflection: 0,
  Api: 1,
}

// 周期类型枚举
export const ScheduleType = {
  Daily: 0,
  Monthly: 1,
  Specific: 2,
}

// 日志状态枚举
export const LogStatus = {
  Running: 0,
  Success: 1,
  Failure: 2,
  Cancelled: 3,
}

// 触发类型枚举
export const TriggerType = {
  Cron: 0,
  Manual: 1,
}