// src/api/ops/logQueryApi.ts

import { get, post } from '@/utils/request'
import type { LogQueryParams, LogQueryResponse, LogDetail } from '@/types/logQuery'

/**
 * 获取可用环境列表
 */
export function getLogEnvironments() {
  return post<string[]>('/api/logquery/environments')
}

/**
 * 查询日志列表
 */
export function queryLogs(params: LogQueryParams) {
  return post<LogQueryResponse>('/api/logquery/query', params)
}

/**
 * 获取日志详情
 */
export function getLogDetail(environment: string, id: string) {
  return get<LogDetail>(`/api/logquery/detail/${environment}/${id}`)
}

/**
 * 导出日志
 */
export function exportLogs(params: LogQueryParams) {
  return post('/api/logquery/export', params, { responseType: 'blob' })
}