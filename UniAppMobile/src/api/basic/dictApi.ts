// api/basic/dictApi.ts

import { get, post, put } from '@/utils/request'
import type { DictType, DictData } from '@/types'

const BASE_URL = '/api/dict'

/** 获取字典类型列表（分页） */
export function getDictTypeList(params?: { pageIndex: number; pageSize: number }): Promise<{ list: DictType[]; total: number }> {
  return post<{ list: DictType[]; total: number }>(`${BASE_URL}/type/list`, params || { pageIndex: 1, pageSize: 20 })
}

/** 获取所有字典类型列表 */
export function getAllDictTypes(): Promise<DictType[]> {
  return post<DictType[]>(`${BASE_URL}/type/all`)
}

/** 获取字典类型详情 */
export function getDictTypeDetail(id: string): Promise<DictType> {
  return get<DictType>(`${BASE_URL}/type/detail/${id}`)
}

/** 创建字典类型 */
export function createDictType(data: Partial<DictType>): Promise<{ id: string }> {
  return post<{ id: string }>(`${BASE_URL}/type/add`, data)
}

/** 更新字典类型 */
export function updateDictType(data: DictType): Promise<number> {
  return put<number>(`${BASE_URL}/type/update`, data)
}

/** 删除字典类型 */
export function deleteDictType(id: string): Promise<number> {
  return post<number>(`${BASE_URL}/type/delete`, id)
}

/** 获取字典数据列表（分页） */
export function getDictDataList(params?: { pageIndex: number; pageSize: number; dictTypeId?: string }): Promise<{ list: DictData[]; total: number }> {
  return post<{ list: DictData[]; total: number }>(`${BASE_URL}/data/list`, params || { pageIndex: 1, pageSize: 20 })
}

/** 获取字典数据详情 */
export function getDictDataDetail(id: string): Promise<DictData> {
  return get<DictData>(`${BASE_URL}/data/detail/${id}`)
}

/** 根据字典编码获取数据 */
export function getDictDataByCode(code: string): Promise<DictData[]> {
  return post<DictData[]>(`${BASE_URL}/data/by-code/${code}`)
}

/** 创建字典数据 */
export function createDictData(data: Partial<DictData>): Promise<{ id: string }> {
  return post<{ id: string }>(`${BASE_URL}/data/add`, data)
}

/** 更新字典数据 */
export function updateDictData(data: DictData): Promise<number> {
  return put<number>(`${BASE_URL}/data/update`, data)
}

/** 删除字典数据 */
export function deleteDictData(id: string): Promise<number> {
  return post<number>(`${BASE_URL}/data/delete`, id)
}

/** 批量删除字典数据 */
export function deleteDictDataBatch(ids: string[]): Promise<number> {
  return post<number>(`${BASE_URL}/data/delete-batch`, ids)
}

/** 批量获取字典数据 */
export function getDictDataBatch(codes: string[]): Promise<Record<string, DictData[]>> {
  return post<Record<string, DictData[]>>(`${BASE_URL}/data/batch`, { codes })
}