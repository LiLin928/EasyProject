// src/api/ops/operateLogApi.ts

import { get, post, del } from '@/utils/request'
import type { PageResponse } from '@/types/response'
import type { OperateLog, QueryOperateLogParams } from '@/types/operateLog'

/**
 * 获取操作日志列表
 */
export function getOperateLogList(params: QueryOperateLogParams) {
  return post<PageResponse<OperateLog>>('/api/operatelog/list', params)
}

/**
 * 获取操作日志详情
 * @param id 操作日志ID
 */
export function getOperateLogDetail(id: string) {
  // 后端路由: [HttpGet("detail/{id}")]
  return get<OperateLog>(`/api/operatelog/detail/${id}`)
}

/**
 * 删除操作日志
 * @param id 操作日志ID
 */
export function deleteOperateLog(id: string) {
  // 后端接收 [FromBody] Guid，需要发送 JSON 字符串格式
  return post<number>('/api/operatelog/delete', JSON.stringify(id))
}

/**
 * 清理过期操作日志
 * @param retentionDays 保留天数
 */
export function clearOperateLog(retentionDays: number = 30) {
  // 后端接收 [FromQuery] int，参数在 URL 中，body 为空
  return post<number>(`/api/operatelog/clear?retentionDays=${retentionDays}`, null)
}

/**
 * 按月份查询操作日志
 * @param year 年份
 * @param month 月份
 * @param pageIndex 页码
 * @param pageSize 每页数量
 */
export function getOperateLogByMonth(
  year: number,
  month: number,
  pageIndex: number = 1,
  pageSize: number = 20
) {
  return get<PageResponse<OperateLog>>(
    `/api/operatelog/month`,
    { year, month, pageIndex, pageSize }
  )
}