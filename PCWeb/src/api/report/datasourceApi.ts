// src/api/report/datasourceApi.ts

import { get, post, put } from '@/utils/request'
import type {
  DataSource,
  DataSourceListParams,
  CreateDataSourceParams,
  UpdateDataSourceParams,
  DbTypeInfo,
  TestConnectionResult,
} from '@/types'

/**
 * 获取数据源列表
 */
export function getDataSourceList(params: DataSourceListParams) {
  return post<{ list: DataSource[]; total: number }>('/api/datasource/list', params)
}

/**
 * 获取所有数据源（下拉选择用）
 */
export function getDataSourceAll() {
  return post<DataSource[]>('/api/datasource/all')
}

/**
 * 获取数据源详情
 */
export function getDataSourceDetail(id: string) {
  return get<DataSource>(`/api/datasource/detail/${id}`)
}

/**
 * 创建数据源
 */
export function createDataSource(data: CreateDataSourceParams) {
  return post<{ id: string }>('/api/datasource/add', data)
}

/**
 * 更新数据源
 */
export function updateDataSource(data: UpdateDataSourceParams) {
  return put<{ success: boolean }>('/api/datasource/update', data)
}

/**
 * 删除数据源
 */
export function deleteDataSource(id: string) {
  return post<number>('/api/datasource/delete', id)
}

/**
 * 获取支持的数据库类型
 */
export function getDbTypes() {
  return post<DbTypeInfo[]>('/api/datasource/dbtypes')
}

/**
 * 测试连接（按 ID，用于已保存的数据源）
 */
export function testConnectionById(id: string) {
  return post<TestConnectionResult>(`/api/datasource/test/${id}`)
}

/**
 * 测试连接（按配置，用于未保存的数据源）
 */
export function testConnection(data: CreateDataSourceParams) {
  return post<TestConnectionResult>('/api/datasource/test-config', data)
}

/**
 * 测试所有连接
 */
export function testAllConnections() {
  return post<TestConnectionResult[]>('/api/datasource/test-all')
}