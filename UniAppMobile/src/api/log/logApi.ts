// api/log/logApi.ts

import { get, post } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Log, LogQueryParams, PageResponse } from '@/types'

/** 查询日志列表 */
export function getLogList(params: LogQueryParams): Promise<PageResponse<Log>> {
  return post<PageResponse<Log>>(API_PATHS.LOG_QUERY, params)
}

/** 获取日志详情 */
export function getLogDetail(environment: string, id: string): Promise<Log> {
  return get<Log>(`${API_PATHS.LOG_DETAIL}/${environment}/${id}`)
}

/** 获取可用环境列表 */
export function getLogEnvironments(): Promise<string[]> {
  return post<string[]>(API_PATHS.LOG_ENVIRONMENTS)
}

/** 清理日志 */
export function clearLogs(beforeTime: string): Promise<number> {
  return post<number>('/api/log/clear', { beforeTime })
}