import { get, post, put } from '@/utils/request'
import type {
  AntWorkflowDefinition,
  AntWorkflowQueryParams,
  CreateAntWorkflowParams,
  UpdateAntWorkflowParams,
  AntWorkflowVersion,
} from '@/types/antWorkflow'

/**
 * 获取流程列表
 * 后端 API: /api/ant-workflow/workflow/list
 */
export function getWorkflowList(params: AntWorkflowQueryParams) {
  return post<{ list: AntWorkflowDefinition[]; total: number }>('/api/ant-workflow/workflow/list', params)
}

/**
 * 获取流程详情
 * 后端 API: /api/ant-workflow/workflow/detail/{id}
 */
export function getWorkflowDetail(id: string) {
  return get<AntWorkflowDefinition>(`/api/ant-workflow/workflow/detail/${id}`)
}

/**
 * 创建流程
 * 后端 API: /api/ant-workflow/workflow/create
 * 返回创建的流程 ID
 */
export function createWorkflow(data: CreateAntWorkflowParams) {
  return post<string>('/api/ant-workflow/workflow/create', data)
}

/**
 * 更新流程
 * 后端 API: /api/ant-workflow/workflow/update
 * 返回影响的行数
 */
export function updateWorkflow(data: UpdateAntWorkflowParams) {
  return put<number>('/api/ant-workflow/workflow/update', data)
}

/**
 * 删除流程
 * 后端 API: /api/ant-workflow/workflow/delete
 */
export function deleteWorkflow(ids: string | string[]) {
  return post<number>('/api/ant-workflow/workflow/delete', Array.isArray(ids) ? ids : [ids])
}

/**
 * 发布流程
 * 后端 API: /api/ant-workflow/workflow/publish/{id}
 */
export function publishWorkflow(id: string) {
  return post<{ success: boolean }>(`/api/ant-workflow/workflow/publish/${id}`)
}

/**
 * 停用流程
 * 后端 API: /api/ant-workflow/workflow/disable/{id}
 */
export function disableWorkflow(id: string) {
  return put<{ success: boolean }>(`/api/ant-workflow/workflow/disable/${id}`)
}

/**
 * 启用流程
 * 后端 API: /api/ant-workflow/workflow/enable/{id}
 */
export function enableWorkflow(id: string) {
  return put<{ success: boolean }>(`/api/ant-workflow/workflow/enable/${id}`)
}

/**
 * 复制流程
 * 后端 API: /api/ant-workflow/workflow/copy
 */
export function copyWorkflow(id: string, name: string) {
  return post<{ id: string }>('/api/ant-workflow/workflow/copy', { id, name })
}

/**
 * 获取版本历史
 * 后端 API: /api/ant-workflow/workflow/versions/{workflowId}
 */
export function getWorkflowVersions(workflowId: string) {
  return post<AntWorkflowVersion[]>(`/api/ant-workflow/workflow/versions/${workflowId}`)
}