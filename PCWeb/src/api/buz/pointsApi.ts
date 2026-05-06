// src/api/buz/pointsApi.ts

import { get, post } from '@/utils/request'
import type { PointsLog, PointsLogListParams } from '@/types'

/**
 * 获取积分记录列表
 */
export function getPointsLogList(params: PointsLogListParams) {
  return post<{ list: PointsLog[]; total: number }>('/api/points/list', params)
}

/**
 * 删除积分记录
 */
export function deletePointsLog(id: string) {
  return post<number>('/api/points/delete', id)
}

/**
 * 批量删除积分记录
 * @param ids 积分记录ID列表 (GUID数组)
 */
export function deletePointsLogBatch(ids: string[]) {
  return post<number>('/api/points/delete-batch', ids)
}