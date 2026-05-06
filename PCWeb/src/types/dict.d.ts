import type { CommonStatus } from './enums'

// src/types/dict.d.ts

/**
 * 字典类型
 */
export interface DictType {
  id: string
  code: string
  name: string
  description: string
  status: CommonStatus
  version: number      // 版本号
  labelZh: string      // 中文名称
  labelEn?: string     // 英文名称
  createTime: string
  updateTime: string
}

/**
 * 字典数据
 */
export interface DictData {
  id: string
  typeCode: string
  label: string
  value: string
  sort: number
  status: CommonStatus
  labelZh: string      // 中文标签
  labelEn?: string     // 英文标签
  createTime: string
  updateTime: string
}

/**
 * 字典类型查询参数
 */
export interface DictTypeListParams {
  pageIndex: number
  pageSize: number
  code?: string
  name?: string
  status?: CommonStatus
}

/**
 * 字典数据查询参数
 */
export interface DictDataListParams {
  pageIndex: number
  pageSize: number
  typeCode: string
  status?: CommonStatus
}

/**
 * 创建字典类型参数
 */
export interface CreateDictTypeParams {
  code: string
  name: string
  description?: string
  status?: CommonStatus
}

/**
 * 更新字典类型参数
 */
export interface UpdateDictTypeParams {
  id: string
  name?: string
  description?: string
  status?: CommonStatus
}

/**
 * 创建字典数据参数
 */
export interface CreateDictDataParams {
  typeCode: string
  label: string
  value: string
  sort?: number
  status?: CommonStatus
}

/**
 * 更新字典数据参数
 */
export interface UpdateDictDataParams {
  id: string
  label?: string
  value?: string
  sort?: number
  status?: CommonStatus
}

/**
 * 字典数据项（用于缓存）
 */
export interface DictDataItem {
  value: string
  labelZh: string
  labelEn?: string
  sort: number
  status: CommonStatus
}

/**
 * 字典缓存项
 */
export interface DictCacheItem {
  code: string
  version: number
  items: DictDataItem[]
  updateTime: number
}

/**
 * 字典数据（含版本）
 */
export interface DictDataWithVersion {
  version: number
  items: DictDataItem[]
}

/**
 * 批量获取字典请求
 */
export interface BatchDictDataRequest {
  codes: string[]
}

/**
 * 版本检查请求
 */
export interface VersionCheckRequest {
  [code: string]: number
}

/**
 * 版本检查响应
 */
export interface VersionCheckResponse {
  needRefresh: string[]
  allVersions: Record<string, number>
}

/**
 * 字典选项（用于下拉框）
 */
export interface DictOption {
  value: string
  label: string
}