/**
 * ETL 数据源管理 API
 */
import { get, post } from '@/utils/request'
import type { PageResponse } from '@/types/response'
import type {
  DataSource,
  DataSourceListParams,
  CreateDataSourceParams,
  UpdateDataSourceParams,
  TestConnectionResult,
} from '@/types/etl'

/**
 * 获取数据源列表
 */
export function getDatasourceList(params: DataSourceListParams) {
  return post<PageResponse<DataSource>>('/api/etl/datasource/list', params)
}

/**
 * 获取数据源详情
 */
export function getDatasourceDetail(id: string) {
  return get<DataSource>(`/api/etl/datasource/detail?id=${id}`)
}

/**
 * 创建数据源
 */
export function createDatasource(data: CreateDataSourceParams) {
  return post<{ id: string }>('/api/etl/datasource/create', data)
}

/**
 * 更新数据源
 */
export function updateDatasource(data: UpdateDataSourceParams) {
  return post<number>('/api/etl/datasource/update', data)
}

/**
 * 删除数据源
 */
export function deleteDatasource(ids: string | string[]) {
  const idsArray = Array.isArray(ids) ? ids : [ids]
  return post<number>('/api/etl/datasource/delete', { ids: idsArray })
}

/**
 * 测试连接
 */
export function testDatasourceConnection(data: { id?: string }) {
  return post<TestConnectionResult>('/api/etl/datasource/test', data)
}

/**
 * 测试查询
 */
export function testDatasourceQuery(id: string, sql: string) {
  return post<{ success: boolean; data: any[]; columns: string[]; rowCount: number }>(
    '/api/etl/datasource/test-query',
    { id, sql }
  )
}