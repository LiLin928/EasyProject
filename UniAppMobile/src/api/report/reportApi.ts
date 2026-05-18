// api/report/reportApi.ts

import { get, post, put } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Report, ReportCategory, ReportQueryParams, ReportData, ReportDataParams, PageResponse } from '@/types'

/** 获取报表列表 */
export function getReportList(params: ReportQueryParams): Promise<PageResponse<Report>> {
  return post<PageResponse<Report>>(API_PATHS.REPORT_LIST, params)
}

/** 获取报表分类列表 */
export function getReportCategories(): Promise<ReportCategory[]> {
  return get<ReportCategory[]>(API_PATHS.REPORT_CATEGORIES)
}

/** 获取报表详情 */
export function getReportDetail(id: string): Promise<Report> {
  return get<Report>(`${API_PATHS.REPORT_DETAIL}/${id}`)
}

/** 创建报表 */
export function createReport(data: Partial<Report>): Promise<{ id: string }> {
  return post<{ id: string }>('/api/report/add', data)
}

/** 更新报表 */
export function updateReport(data: Report): Promise<number> {
  return put<number>('/api/report/update', data)
}

/** 删除报表 */
export function deleteReport(id: string): Promise<number> {
  return post<number>('/api/report/delete', id)
}

/** 获取报表数据 */
export function getReportData(params: ReportDataParams): Promise<ReportData> {
  return post<ReportData>(API_PATHS.REPORT_DATA, params)
}

/** 执行报表 */
export function executeReport(id: string): Promise<ReportData> {
  return get<ReportData>(`/api/report/execute/${id}`)
}

/** 预览报表 */
export function previewReport(data: Partial<Report>): Promise<ReportData> {
  return post<ReportData>('/api/report/preview', data)
}

/** 获取发布报表数据 */
export function getPublishReport(id: string): Promise<Report> {
  return get<Report>(`/api/report/publish/${id}`)
}