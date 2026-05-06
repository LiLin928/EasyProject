// src/api/dict.ts

import { get, post, put } from '@/utils/request'
import type {
  DictType,
  DictData,
  DictTypeListParams,
  DictDataListParams,
  CreateDictTypeParams,
  UpdateDictTypeParams,
  CreateDictDataParams,
  UpdateDictDataParams,
  DictDataWithVersion,
  VersionCheckResponse,
} from '@/types/dict'

// ==================== 字典类型 API ====================

/**
 * 获取字典类型列表（分页）
 */
export function getDictTypeList(params: DictTypeListParams) {
  return post<{ list: DictType[]; total: number; pageIndex: number; pageSize: number }>('/api/dict/type/list', params)
}

/**
 * 获取所有字典类型列表（不分页）
 */
export function getAllDictTypes() {
  return post<DictType[]>('/api/dict/type/all')
}

/**
 * 获取字典类型详情
 */
export function getDictTypeDetail(id: string) {
  return get<DictType>(`/api/dict/type/detail/${id}`)
}

/**
 * 新增字典类型
 */
export function addDictType(data: CreateDictTypeParams) {
  return post<string>('/api/dict/type/add', data)
}

/**
 * 更新字典类型
 */
export function updateDictType(data: UpdateDictTypeParams) {
  return put<number>('/api/dict/type/update', data)
}

/**
 * 删除字典类型
 */
export function deleteDictType(id: string) {
  return post<number>('/api/dict/type/delete', id)
}

// ==================== 字典数据 API ====================

/**
 * 获取字典数据列表（分页）
 */
export function getDictDataList(params: DictDataListParams) {
  return post<{ list: DictData[]; total: number; pageIndex: number; pageSize: number }>('/api/dict/data/list', params)
}

/**
 * 根据字典类型编码获取字典数据列表（用于下拉选项）
 */
export function getDictDataByCode(code: string) {
  return post<DictData[]>(`/api/dict/data/by-code/${code}`)
}

/**
 * 获取字典数据详情
 */
export function getDictDataDetail(id: string) {
  return get<DictData>(`/api/dict/data/detail/${id}`)
}

/**
 * 新增字典数据
 */
export function addDictData(data: CreateDictDataParams) {
  return post<string>('/api/dict/data/add', data)
}

/**
 * 更新字典数据
 */
export function updateDictData(data: UpdateDictDataParams) {
  return put<number>('/api/dict/data/update', data)
}

/**
 * 删除字典数据
 */
export function deleteDictData(id: string) {
  return post<number>('/api/dict/data/delete', id)
}

/**
 * 批量删除字典数据
 */
export function deleteDictDataBatch(ids: string[]) {
  return post<number>('/api/dict/data/delete-batch', ids)
}

// ==================== 兼容旧接口（保持向后兼容） ====================

/** @deprecated 使用 addDictType 代替 */
export function createDictType(data: CreateDictTypeParams) {
  return addDictType(data)
}

/** @deprecated 使用 addDictData 代替 */
export function createDictData(data: CreateDictDataParams) {
  return addDictData(data)
}

// ==================== 批量获取和版本检查 ====================

/**
 * 批量获取字典数据
 */
export function getDictDataBatch(codes: string[]) {
  return post<Record<string, DictDataWithVersion>>('/api/dict/data/batch', { codes })
}

/**
 * 检查字典版本
 */
export function checkDictVersion(versions: Record<string, number>) {
  return post<VersionCheckResponse>('/api/dict/version/check', versions)
}

/**
 * 根据编码获取字典数据（含版本）
 */
export function getDictDataWithVersion(code: string) {
  return get<DictDataWithVersion>(`/api/dict/data/with-version/${code}`)
}