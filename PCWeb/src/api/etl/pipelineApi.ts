/**
 * ETL 任务流管理 API
 */
import { get, post } from '@/utils/request'
import type { PageResponse } from '@/types/response'
import type {
  Pipeline,
  PipelineQueryParams,
  CreatePipelineParams,
  UpdatePipelineParams,
} from '@/types/etl'

/**
 * 获取任务流列表
 */
export function getPipelineList(params: PipelineQueryParams) {
  return post<PageResponse<Pipeline>>('/api/etl/pipeline/list', params)
}

/**
 * 获取任务流详情
 */
export function getPipelineDetail(id: string) {
  return get<Pipeline>(`/api/etl/pipeline/detail?id=${id}`)
}

/**
 * 创建任务流
 */
export function createPipeline(data: CreatePipelineParams) {
  return post<Pipeline>('/api/etl/pipeline/create', data)
}

/**
 * 更新任务流
 */
export function updatePipeline(data: UpdatePipelineParams) {
  return post<Pipeline>('/api/etl/pipeline/update', data)
}

/**
 * 删除任务流
 */
export function deletePipeline(ids: string | string[]) {
  const idsArray = Array.isArray(ids) ? ids : [ids]
  return post<null>('/api/etl/pipeline/delete', { ids: idsArray })
}

/**
 * 发布任务流
 */
export function publishPipeline(id: string) {
  return post<Pipeline>('/api/etl/pipeline/publish', { id })
}

/**
 * 取消发布任务流
 */
export function unpublishPipeline(id: string) {
  return post<Pipeline>('/api/etl/pipeline/unpublish', { id })
}

/**
 * 复制任务流
 */
export function copyPipeline(id: string, name: string) {
  return post<Pipeline>('/api/etl/pipeline/copy', { id, name })
}

/**
 * 执行任务流
 * @returns 执行记录ID
 */
export function executePipeline(id: string, params?: Record<string, any>) {
  return post<string>('/api/etl/pipeline/execute', { id, params })
}

/**
 * 获取任务流 DAG 定义
 */
export function getPipelineDag(id: string) {
  return get<{ nodes: any[]; edges: any[] }>(`/api/etl/pipeline/dag?id=${id}`)
}

/**
 * 保存任务流 DAG 定义
 */
export function savePipelineDag(id: string, dag: { nodes: any[]; edges: any[] }) {
  return post<null>('/api/etl/pipeline/dag/save', { id, dag })
}