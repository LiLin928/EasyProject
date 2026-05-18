// api/report/reportApi.ts

import { get, post } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Report, ReportCategory, ReportQueryParams, ReportData, ReportDataParams, PageResponse } from '@/types'

/** 获取报表列表 */
export function getReportList(params: ReportQueryParams): Promise<PageResponse<Report>> {
  return get<PageResponse<Report>>(API_PATHS.REPORT_LIST, params)
}

/** 获取报表分类列表 */
export function getReportCategories(): Promise<ReportCategory[]> {
  return get<ReportCategory[]>(API_PATHS.REPORT_CATEGORIES)
}

/** 获取报表详情 */
export function getReportDetail(id: string): Promise<Report> {
  return get<Report>(`${API_PATHS.REPORT_DETAIL}/${id}`)
}

/** 获取报表数据 */
export function getReportData(params: ReportDataParams): Promise<ReportData> {
  return post<ReportData>(API_PATHS.REPORT_DATA, params)
}