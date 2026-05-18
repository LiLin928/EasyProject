// api/workflow/taskApi.ts

import { get, post } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { TaskInfo, TaskDetail, TaskQueryParams, PageResponse } from '@/types'

/** 获取待办任务列表 */
export function getTodoList(params: TaskQueryParams): Promise<PageResponse<TaskInfo>> {
  return post<PageResponse<TaskInfo>>(API_PATHS.WORKFLOW_TODO_LIST, params)
}

/** 获取已办任务列表 */
export function getDoneList(params: TaskQueryParams): Promise<PageResponse<TaskInfo>> {
  return post<PageResponse<TaskInfo>>(API_PATHS.WORKFLOW_DONE_LIST, params)
}

/** 获取抄送任务列表 */
export function getCCList(params: TaskQueryParams): Promise<PageResponse<TaskInfo>> {
  return post<PageResponse<TaskInfo>>(API_PATHS.WORKFLOW_CC_LIST, params)
}

/** 获取任务详情 */
export function getTaskDetail(taskId: string): Promise<TaskDetail> {
  return get<TaskDetail>(`${API_PATHS.WORKFLOW_TASK_DETAIL}/${taskId}`)
}