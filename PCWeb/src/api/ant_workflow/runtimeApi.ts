import { get, post } from '@/utils/request'
import type {
  StartAntWorkflowParams,
  AntWorkflowInstanceDto,
  AntWorkflowInstanceDetailDto,
  AntMyInstanceQueryParams,
  AntExecutionLog,
} from '@/types/antWorkflow'

/**
 * 发起流程
 * 后端 API: /api/ant-workflow/runtime/start
 */
export function startWorkflow(params: StartAntWorkflowParams) {
  return post<{ instanceId: string }>('/api/ant-workflow/runtime/start', params)
}

/**
 * 获取我发起的流程列表
 * 后端 API: /api/ant-workflow/runtime/my-instances
 */
export function getMyInstances(params: AntMyInstanceQueryParams) {
  return post<{ list: AntWorkflowInstanceDto[]; total: number }>('/api/ant-workflow/runtime/my-instances', params)
}

/**
 * 获取实例详情
 * 后端 API: /api/ant-workflow/runtime/detail/{id}
 */
export function getInstanceDetail(instanceId: string) {
  return get<AntWorkflowInstanceDetailDto>(`/api/ant-workflow/runtime/detail/${instanceId}`)
}

/**
 * 取消实例（撤回）
 * 后端 API: /api/ant-workflow/runtime/cancel/{id}?reason=xxx
 */
export function cancelInstance(instanceId: string, reason?: string) {
  const url = reason
    ? `/api/ant-workflow/runtime/cancel/${instanceId}?reason=${encodeURIComponent(reason)}`
    : `/api/ant-workflow/runtime/cancel/${instanceId}`
  return post<{ success: boolean }>(url)
}

/**
 * 获取执行日志
 * 后端 API: /api/ant-workflow/runtime/logs/{instanceId}
 */
export function getInstanceLogs(instanceId: string) {
  return post<AntExecutionLog[]>(`/api/ant-workflow/runtime/logs/${instanceId}`)
}