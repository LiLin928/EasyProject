// src/api/report/columnTemplateApi.ts

import { get, post, put } from '@/utils/request'
import type {
  ColumnTemplate,
  ColumnTemplateListParams,
  CreateColumnTemplateParams,
  UpdateColumnTemplateParams,
  FetchColumnsParams,
  DetectedColumn,
} from '@/types'

/**
 * 获取列配置模板列表
 */
export function getColumnTemplateList(params: ColumnTemplateListParams) {
  return post<{ list: ColumnTemplate[]; total: number }>('/api/column-template/list', params)
}

/**
 * 获取列配置模板详情
 */
export function getColumnTemplateDetail(id: string) {
  return get<ColumnTemplate>(`/api/column-template/detail/${id}`)
}

/**
 * 创建列配置模板
 */
export function createColumnTemplate(data: CreateColumnTemplateParams) {
  return post<{ id: string }>('/api/column-template/add', data)
}

/**
 * 更新列配置模板
 */
export function updateColumnTemplate(data: UpdateColumnTemplateParams) {
  return put<{ success: boolean }>('/api/column-template/update', data)
}

/**
 * 删除列配置模板
 */
export function deleteColumnTemplate(id: string) {
  return post<number>('/api/column-template/delete', id)
}

/**
 * 批量删除列配置模板
 * @param ids 模板ID列表 (GUID数组)
 */
export function deleteColumnTemplateBatch(ids: string[]) {
  return post<number>('/api/column-template/delete-batch', ids)
}

/**
 * 获取所有单列模板
 */
export function getSingleColumnTemplates() {
  return post<ColumnTemplate[]>('/api/column-template/single')
}

/**
 * 根据数据源和SQL获取列信息
 */
export function fetchColumnsFromSql(params: FetchColumnsParams) {
  return post<DetectedColumn[]>('/api/column-template/fetch-columns', params)
}