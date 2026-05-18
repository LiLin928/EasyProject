// api/log/logApi.ts

import { get, del } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Log, LogQueryParams, PageResponse } from '@/types'

/** 获取日志列表 */
export function getLogList(params: LogQueryParams): Promise<PageResponse<Log>> {
  return get<PageResponse<Log>>(API_PATHS.LOG_LIST, params)
}

/** 获取日志详情 */
export function getLogDetail(id: string): Promise<Log> {
  return get<Log>(`${API_PATHS.LOG_DETAIL}/${id}`)
}

/** 清理日志
 * @param beforeTime 清理此时间之前的日志
 */
export function clearLogs(beforeTime: string): Promise<number> {
  return del<number>(API_PATHS.LOG_CLEAR, { beforeTime })
}