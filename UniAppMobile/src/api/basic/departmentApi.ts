// api/basic/departmentApi.ts

import { get, post, del } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Department, User } from '@/types'

/** 获取部门树 */
export function getDepartmentTree(): Promise<Department[]> {
  return get<Department[]>(API_PATHS.DEPARTMENT_TREE)
}

/** 获取部门详情 */
export function getDepartmentDetail(id: string): Promise<Department> {
  return get<Department>(`${API_PATHS.DEPARTMENT_DETAIL}/${id}`)
}

/** 创建部门 */
export function createDepartment(data: Partial<Department>): Promise<{ id: string }> {
  return post<{ id: string }>(API_PATHS.DEPARTMENT_ADD, data)
}

/** 更新部门 */
export function updateDepartment(data: Department): Promise<number> {
  return post<number>(API_PATHS.DEPARTMENT_UPDATE, data)
}

/** 删除部门 */
export function deleteDepartment(id: string): Promise<number> {
  return del<number>(`${API_PATHS.DEPARTMENT_DELETE}/${id}`)
}

/** 获取部门用户列表 */
export function getDepartmentUsers(id: string): Promise<User[]> {
  return get<User[]>(`/api/department/users/${id}`)
}