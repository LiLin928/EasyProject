// api/workflow/runtimeApi.ts

import { get, post } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { ApproveParams, RejectParams, TransferParams, AddSignerParams } from '@/types'

/** 审批通过 */
export function approveTask(params: ApproveParams): Promise<void> {
  return post<void>(API_PATHS.WORKFLOW_APPROVE, params)
}

/** 驳回任务 */
export function rejectTask(params: RejectParams): Promise<void> {
  return post<void>(API_PATHS.WORKFLOW_REJECT, params)
}

/** 转办任务 */
export function transferTask(params: TransferParams): Promise<void> {
  return post<void>(API_PATHS.WORKFLOW_TRANSFER, params)
}

/** 加签 */
export function addSign(params: AddSignerParams): Promise<void> {
  return post<void>(API_PATHS.WORKFLOW_ADD_SIGNER, params)
}

/** 标记抄送已读 */
export function markCcRead(ids: string[]): Promise<void> {
  return post<void>(API_PATHS.WORKFLOW_CC_READ, ids)
}

/** 启动流程 */
export function startWorkflow(params: { workflowId: string; formData?: Record<string, any> }): Promise<string> {
  return post<string>(API_PATHS.WORKFLOW_START, params)
}

/** 获取我的流程实例 */
export function getMyInstances(params: { pageIndex: number; pageSize: number; status?: number }): Promise<any> {
  return post<any>(API_PATHS.WORKFLOW_MY_INSTANCES, params)
}

/** 获取流程实例详情 */
export function getInstanceDetail(instanceId: string): Promise<any> {
  return get<any>(`${API_PATHS.WORKFLOW_INSTANCE_DETAIL}/${instanceId}`)
}

/** 撤回流程 */
export function cancelWorkflow(instanceId: string, reason?: string): Promise<void> {
  return post<void>(`${API_PATHS.WORKFLOW_CANCEL}/${instanceId}?reason=${reason || ''}`)
}

/** 获取审批日志 */
export function getWorkflowLogs(instanceId: string): Promise<any[]> {
  return post<any[]>(`${API_PATHS.WORKFLOW_LOGS}/${instanceId}`)
}