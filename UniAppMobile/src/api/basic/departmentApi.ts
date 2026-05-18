// api/basic/departmentApi.ts

import { get, post, del } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Department } from '@/types'

/** 获取部门树 */
export function getDepartmentTree(): Promise<Department[]> {
  return get<Department[]>(API_PATHS.DEPARTMENT_LIST + '/tree')
}

/** 获取部门详情 */
export function getDepartmentDetail(id: string): Promise<Department> {
  return get<Department>(`${API_PATHS.DEPARTMENT_LIST}/${id}`)
}

/** 创建部门 */
export function createDepartment(data: Partial<Department>): Promise<{ id: string }> {
  return post<{ id: string }>(API_PATHS.DEPARTMENT_LIST, data)
}

/** 更新部门 */
export function updateDepartment(data: Department): Promise<number> {
  return post<number>(API_PATHS.DEPARTMENT_LIST, data)
}

/** 删除部门 */
export function deleteDepartment(id: string): Promise<number> {
  return del<number>(`${API_PATHS.DEPARTMENT_LIST}/${id}`)
}

/** 获取部门用户列表 */
export function getDepartmentUsers(id: string): Promise<{ id: string; userName: string; realName: string }[]> {
  return get<{ id: string; userName: string; realName: string }[]>(`${API_PATHS.DEPARTMENT_LIST}/${id}/users`)
}