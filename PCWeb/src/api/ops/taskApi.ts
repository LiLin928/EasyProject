// PCWeb/src/api/ops/taskApi.ts

import { get, post } from '@/utils/request'
import type { PageResponse } from '@/types/response'
import type {
  TaskDefinition,
  QueryTaskParams,
  UpdateTaskParams,
  TaskExecutionLog,
  QueryTaskLogParams,
  TaskStatistics,
  TaskLogTrend,
} from '@/types/task'

// ===================== 任务管理 API =====================

/**
 * 获取任务列表
 */
export function getTaskList(params: QueryTaskParams) {
  return post<PageResponse<TaskDefinition>>('/api/task/list', params)
}

/**
 * 获取任务详情
 */
export function getTaskDetail(id: string) {
  return get<TaskDefinition>(`/api/task/detail/${id}`)
}

/**
 * 创建任务
 */
export function createTask(data: UpdateTaskParams) {
  return post<string>('/api/task/create', data)
}

/**
 * 更新任务
 */
export function updateTask(data: UpdateTaskParams) {
  return post<number>('/api/task/update', data)
}

/**
 * 删除任务
 */
export function deleteTask(ids: string[]) {
  return post<number>('/api/task/delete', ids)
}

/**
 * 暂停任务
 */
export function pauseTask(id: string) {
  return post<number>('/api/task/pause', { id })
}

/**
 * 恢复任务
 */
export function resumeTask(id: string) {
  return post<number>('/api/task/resume', { id })
}

/**
 * 立即执行任务
 */
export function triggerTask(id: string) {
  return post<number>('/api/task/trigger', { id })
}

/**
 * 获取任务统计
 */
export function getTaskStatistics() {
  return get<TaskStatistics>('/api/task/statistics')
}

// ===================== 日志查询 API =====================

/**
 * 获取日志列表
 */
export function getTaskLogList(params: QueryTaskLogParams) {
  return post<PageResponse<TaskExecutionLog>>('/api/tasklog/list', params)
}

/**
 * 获取日志详情
 */
export function getTaskLogDetail(id: string) {
  return get<TaskExecutionLog>(`/api/tasklog/detail/${id}`)
}

/**
 * 清理日志
 */
export function clearTaskLog(retentionDays: number = 30) {
  return post<number>(`/api/tasklog/clear?retentionDays=${retentionDays}`, null)
}

/**
 * 获取执行趋势
 */
export function getTaskLogTrend(days: number = 7) {
  return get<TaskLogTrend>(`/api/tasklog/trend?days=${days}`)
}

/**
 * 获取日志统计
 */
export function getTaskLogStatistics() {
  return get<TaskStatistics>('/api/tasklog/statistics')
}