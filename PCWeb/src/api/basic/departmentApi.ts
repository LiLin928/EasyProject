import { get, post, del } from '@/utils/request'
import type { Department, AddDepartmentParams, UpdateDepartmentParams } from '@/types/department'
import type { UserInfo } from '@/types'

/**
 * 获取部门树形结构
 */
export function getDepartmentTree() {
  return get<Department[]>('/api/department/tree')
}

/**
 * 获取部门详情
 * @param id 部门ID (GUID)
 */
export function getDepartmentDetail(id: string) {
  return get<Department>(`/api/department/detail/${id}`)
}

/**
 * 获取部门成员列表
 * @param id 部门ID (GUID)
 */
export function getDepartmentUsers(id: string) {
  return get<UserInfo[]>(`/api/department/users/${id}`)
}

/**
 * 新增部门
 * @param data 部门数据
 */
export function createDepartment(data: AddDepartmentParams) {
  return post<{ id: string }>('/api/department/add', data)
}

/**
 * 更新部门
 * @param data 部门数据（含id）
 */
export function updateDepartment(data: UpdateDepartmentParams) {
  return post<number>('/api/department/update', data)
}

/**
 * 删除部门
 * @param id 部门ID (GUID)
 */
export function deleteDepartment(id: string) {
  return del<number>(`/api/department/delete/${id}`)
}