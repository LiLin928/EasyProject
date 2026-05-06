import { get, post } from '@/utils/request'
import type {
  AntWorkflowTaskDto,
  AntWorkflowCcDto,
  AntTaskDetail,
  AntTodoTaskQueryParams,
  AntDoneTaskQueryParams,
  AntApproveParams,
  AntRejectParams,
  AntTransferParams,
  AntAddSignerParams,
} from '@/types/antWorkflow'

/**
 * 获取待办任务列表
 * 后端 API: /api/ant-workflow/task/todo
 */
export function getTodoTasks(params: AntTodoTaskQueryParams) {
  // 后端参数名: WorkflowName, BusinessType
  return post<{ list: AntWorkflowTaskDto[]; total: number }>('/api/ant-workflow/task/todo', {
    pageIndex: params.pageIndex,
    pageSize: params.pageSize,
    workflowName: params.title, // 前端用title，后端用workflowName
    businessType: params.businessType,
  })
}

/**
 * 获取已办任务列表
 * 后端 API: /api/ant-workflow/task/done
 */
export function getDoneTasks(params: AntDoneTaskQueryParams) {
  // 后端参数名: WorkflowName, StartTime, EndTime
  return post<{ list: AntWorkflowTaskDto[]; total: number }>('/api/ant-workflow/task/done', {
    pageIndex: params.pageIndex,
    pageSize: params.pageSize,
    workflowName: params.title, // 前端用title，后端用workflowName
    startTime: params.startTime,
    endTime: params.endTime,
  })
}

/**
 * 获取抄送任务列表
 * 后端 API: /api/ant-workflow/task/cc
 */
export function getCcTasks(params: { pageIndex?: number; pageSize?: number; workflowName?: string; isRead?: boolean }) {
  // 后端参数名: WorkflowName, IsRead (int类型)
  return post<{ list: AntWorkflowCcDto[]; total: number }>('/api/ant-workflow/task/cc', {
    pageIndex: params.pageIndex,
    pageSize: params.pageSize,
    workflowName: params.workflowName,
    isRead: params.isRead !== undefined ? (params.isRead ? 1 : 0) : undefined, // 后端用int类型
  })
}

/**
 * 获取任务详情
 * 后端 API: /api/ant-workflow/task/detail/{taskId}
 */
export function getTaskDetail(taskId: string) {
  return get<AntTaskDetail>(`/api/ant-workflow/task/detail/${taskId}`)
}

/**
 * 审批通过
 * 后端 API: /api/ant-workflow/task/approve
 */
export function approveTask(taskId: string, params: Omit<AntApproveParams, 'taskId'>) {
  return post<{ success: boolean }>('/api/ant-workflow/task/approve', {
    taskId,
    approveDesc: params.comment, // 后端用approveDesc
    ccUserIds: params.ccUserIds,
  })
}

/**
 * 审批拒绝
 * 后端 API: /api/ant-workflow/task/reject
 */
export function rejectTask(taskId: string, params: Omit<AntRejectParams, 'taskId'>) {
  return post<{ success: boolean }>('/api/ant-workflow/task/reject', {
    taskId,
    rejectReason: params.comment, // 后端用rejectReason
  })
}

/**
 * 转办任务
 * 后端 API: /api/ant-workflow/task/transfer
 */
export function transferTask(taskId: string, params: Omit<AntTransferParams, 'taskId'>) {
  return post<{ success: boolean }>('/api/ant-workflow/task/transfer', {
    taskId,
    transferToUserId: params.toUserId, // 后端用transferToUserId
    transferReason: params.comment, // 后端用transferReason
  })
}

/**
 * 加签
 * 后端 API: /api/ant-workflow/task/add-signer
 */
export function addSigner(taskId: string, params: Omit<AntAddSignerParams, 'taskId'>) {
  return post<{ success: boolean }>('/api/ant-workflow/task/add-signer', {
    taskId,
    signerIds: params.userIds, // 后端用signerIds
    reason: params.comment,
    signerType: 'after', // 默认后加签
  })
}

/**
 * 标记抄送已读
 * 后端 API: /api/ant-workflow/task/cc-read
 */
export function markCcRead(ids: string[]) {
  return post<{ success: boolean }>('/api/ant-workflow/task/cc-read', ids)
}