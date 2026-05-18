// api/workflow/runtimeApi.ts

import { post } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { ApproveParams } from '@/types'

/** 审批任务 */
export function approveTask(params: ApproveParams): Promise<void> {
  return post<void>(API_PATHS.WORKFLOW_APPROVE, params)
}

/** 转办任务 */
export function transferTask(taskId: string, transferTo: string, comment?: string): Promise<void> {
  return post<void>(`${API_PATHS.WORKFLOW_APPROVE}/transfer`, {
    taskId,
    transferTo,
    comment,
  })
}

/** 加签 */
export function addSign(taskId: string, userIds: string[], comment?: string): Promise<void> {
  return post<void>(`${API_PATHS.WORKFLOW_APPROVE}/addSign`, {
    taskId,
    userIds,
    comment,
  })
}