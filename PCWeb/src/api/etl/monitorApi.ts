/**
 * ETL 执行监控 API
 */
import { get, post } from '@/utils/request'
import type { PageResponse } from '@/types/response'
import type {
  Execution,
  ExecutionQueryParams,
  ExecutionStatistics,
} from '@/types/etl'

/**
 * 获取执行列表
 */
export function getExecutionList(params: ExecutionQueryParams) {
  return post<PageResponse<Execution>>('/api/etl/execution/list', params)
}

/**
 * 获取执行详情
 */
export function getExecutionDetail(id: string) {
  return get<Execution>(`/api/etl/execution/detail?id=${id}`)
}

/**
 * 获取执行统计（与后端 EtlExecutionStatisticsDto 对应）
 */
export function getExecutionStatistics(params?: { dateStart?: string; dateEnd?: string }) {
  return get<ExecutionStatistics>('/api/etl/execution/statistics', params)
}

/**
 * 取消执行
 */
export function cancelExecution(id: string) {
  return post<null>('/api/etl/monitor/execution/cancel', { id })
}

/**
 * 重试执行
 */
export function retryExecution(id: string) {
  return post<{ executionId: string }>('/api/etl/monitor/execution/retry', { id })
}

/**
 * 获取执行日志
 */
export function getExecutionLogs(id: string, params?: { nodeId?: string; level?: string }) {
  return get<{ logs: Array<{ time: string; level: string; message: string; nodeId?: string; nodeName?: string }> }>(
    `/api/etl/monitor/execution/logs?id=${id}`,
    params
  )
}

/**
 * 获取节点执行列表
 */
export function getNodeExecutions(executionId: string) {
  return get<Array<{
    nodeId: string
    nodeName: string
    nodeType?: string
    status: string
    startTime?: string
    endTime?: string
    duration?: number
    input?: any
    output?: any
    error?: string
  }>>(`/api/etl/monitor/execution/nodes?id=${executionId}`)
}

/**
 * 获取节点执行详情
 */
export function getNodeExecutionDetail(executionId: string, nodeId: string) {
  return get<{
    nodeId: string
    nodeName: string
    status: string
    startTime: string
    endTime?: string
    duration?: number
    input: any
    output: any
    error?: string
  }>(`/api/etl/monitor/execution/node?id=${executionId}&nodeId=${nodeId}`)
}

/**
 * 获取正在运行的任务流
 */
export function getRunningExecutions() {
  return post<Execution[]>('/api/etl/monitor/execution/running')
}

/**
 * 获取今日执行统计（与后端 EtlTodayStatisticsDto 对应）
 */
export function getTodayStatistics() {
  return get<{
    total: number
    running: number
    success: number
    failure: number
    pending: number
  }>('/api/etl/monitor/statistics/today')
}

/**
 * 获取实时执行状态
 */
export function getExecutionStatus(id: string) {
  return get<{
    id: string
    status: string
    progress: number
    currentNodeId?: string
    currentNodeName?: string
    completedNodes: number
    totalNodes: number
  }>(`/api/etl/monitor/execution/status?id=${id}`)
}