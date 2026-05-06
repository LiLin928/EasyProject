/**
 * ETL 调度管理 API
 */
import { get, post } from '@/utils/request'
import type { PageResponse } from '@/types/response'
import type {
  Schedule,
  ScheduleQueryParams,
  CreateScheduleParams,
  UpdateScheduleParams,
} from '@/types/etl'

/**
 * 获取调度列表
 */
export function getScheduleList(params: ScheduleQueryParams) {
  return post<PageResponse<Schedule>>('/api/etl/schedule/list', params)
}

/**
 * 获取调度详情
 */
export function getScheduleDetail(id: string) {
  return get<Schedule>(`/api/etl/schedule/detail?id=${id}`)
}

/**
 * 创建调度
 */
export function createSchedule(data: CreateScheduleParams) {
  return post<Schedule>('/api/etl/schedule/create', data)
}

/**
 * 更新调度
 */
export function updateSchedule(data: UpdateScheduleParams) {
  return post<Schedule>('/api/etl/schedule/update', data)
}

/**
 * 删除调度（确保 ids 始终是数组）
 */
export function deleteSchedule(ids: string | string[]) {
  const idsArray = Array.isArray(ids) ? ids : [ids]
  return post<null>('/api/etl/schedule/delete', { ids: idsArray })
}

/**
 * 启用调度
 */
export function enableSchedule(id: string) {
  return post<Schedule>('/api/etl/schedule/enable', { id })
}

/**
 * 禁用调度
 */
export function disableSchedule(id: string) {
  return post<Schedule>('/api/etl/schedule/disable', { id })
}

/**
 * 立即执行调度
 */
export function executeScheduleNow(id: string) {
  return post<{ executionId: string }>('/api/etl/schedule/execute-now', { id })
}

/**
 * 获取调度执行历史
 */
export function getScheduleExecutionHistory(id: string, params?: { pageIndex?: number; pageSize?: number }) {
  return get<PageResponse<any>>(`/api/etl/schedule/history?id=${id}`, params)
}

/**
 * 获取调度统计信息
 */
export function getScheduleStatistics(id: string) {
  return get<{
    totalExecutions: number
    successCount: number
    failureCount: number
    avgDuration: number
    lastExecutionTime: string
  }>(`/api/etl/schedule/statistics?id=${id}`)
}