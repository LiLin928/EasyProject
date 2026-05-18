// api/basic/dictApi.ts

import { get, post, del } from '@/utils/request'
import type { DictType, DictData } from '@/types'

const BASE_URL = '/api/dict'

/** 获取字典类型列表 */
export function getDictTypeList(): Promise<DictType[]> {
  return get<DictType[]>(`${BASE_URL}/type/list`)
}

/** 获取字典类型详情 */
export function getDictTypeDetail(id: string): Promise<DictType> {
  return get<DictType>(`${BASE_URL}/type/${id}`)
}

/** 创建字典类型 */
export function createDictType(data: Partial<DictType>): Promise<{ id: string }> {
  return post<{ id: string }>(`${BASE_URL}/type`, data)
}

/** 更新字典类型 */
export function updateDictType(data: DictType): Promise<number> {
  return post<number>(`${BASE_URL}/type`, data)
}

/** 删除字典类型 */
export function deleteDictType(id: string): Promise<number> {
  return del<number>(`${BASE_URL}/type/${id}`)
}

/** 获取字典数据列表 */
export function getDictDataList(dictTypeId: string): Promise<DictData[]> {
  return get<DictData[]>(`${BASE_URL}/data/list/${dictTypeId}`)
}

/** 根据字典编码获取数据 */
export function getDictDataByCode(code: string): Promise<DictData[]> {
  return get<DictData[]>(`${BASE_URL}/data/code/${code}`)
}

/** 创建字典数据 */
export function createDictData(data: Partial<DictData>): Promise<{ id: string }> {
  return post<{ id: string }>(`${BASE_URL}/data`, data)
}

/** 更新字典数据 */
export function updateDictData(data: DictData): Promise<number> {
  return post<number>(`${BASE_URL}/data`, data)
}

/** 删除字典数据 */
export function deleteDictData(id: string): Promise<number> {
  return del<number>(`${BASE_URL}/data/${id}`)
}