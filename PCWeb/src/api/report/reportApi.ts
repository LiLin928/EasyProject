// src/api/report/reportApi.ts

import { get, post, put } from '@/utils/request'
import type {
  Report,
  ReportListParams,
  CreateReportParams,
  UpdateReportParams,
  PreviewParams,
  PreviewResult,
  ReportCategory,
} from '@/types'

/**
 * 获取报表列表
 */
export function getReportList(params: ReportListParams) {
  return post<{ list: Report[]; total: number }>('/api/report/list', params)
}

/**
 * 获取报表详情
 */
export function getReportDetail(id: string) {
  return get<Report>(`/api/report/detail/${id}`)
}

/**
 * 创建报表
 */
export function createReport(data: CreateReportParams) {
  return post<{ id: string }>('/api/report/add', data)
}

/**
 * 更新报表
 */
export function updateReport(data: UpdateReportParams) {
  return put<{ success: boolean }>('/api/report/update', data)
}

/**
 * 删除报表
 */
export function deleteReport(id: string) {
  return post<number>('/api/report/delete', id)
}

/**
 * 批量删除报表
 * @param ids 报表ID列表 (GUID数组)
 */
export function deleteReportBatch(ids: string[]) {
  return post<number>('/api/report/delete-batch', ids)
}

/**
 * 获取报表分类列表
 */
export function getReportCategories() {
  return get<ReportCategory[]>('/api/report/categories')
}

/**
 * 预览报表数据
 */
export function previewReport(data: PreviewParams) {
  return post<PreviewResult>('/api/report/preview', data)
}

/**
 * 执行报表查询
 */
export function executeReport(id: string) {
  return get<PreviewResult>(`/api/report/execute/${id}`)
}

/**
 * 获取发布报表数据（公开访问，无需登录）
 */
export function getReportPublishData(id: string) {
  return get<{
    report: Report
    chartData: { name: string; value: number }[]
    tableData: Record<string, any>[]
    columnConfigs: any[]
    detectedColumns: any[]
    summary: { total: number; count: number; avg: number }
  }>(`/api/report/publish/${id}`)
}