// src/api/buz/memberLevelApi.ts

import { get, post, put } from '@/utils/request'
import type {
  MemberLevel,
  CreateMemberLevelParams,
  UpdateMemberLevelParams,
} from '@/types'

/**
 * 获取会员等级列表
 */
export function getMemberLevelList() {
  return post<MemberLevel[]>('/api/memberlevel/list')
}

/**
 * 获取会员等级详情
 */
export function getMemberLevelDetail(id: string) {
  return get<MemberLevel>(`/api/memberlevel/detail/${id}`)
}

/**
 * 创建会员等级
 */
export function createMemberLevel(data: CreateMemberLevelParams) {
  return post<{ id: string }>('/api/memberlevel/create', data)
}

/**
 * 更新会员等级
 */
export function updateMemberLevel(data: UpdateMemberLevelParams) {
  return put<{ success: boolean }>('/api/memberlevel/update', data)
}

/**
 * 删除会员等级
 */
export function deleteMemberLevel(id: string) {
  return post<number>('/api/memberlevel/delete', id)
}

/**
 * 批量删除会员等级
 * @param ids 会员等级ID列表 (GUID数组)
 */
export function deleteMemberLevelBatch(ids: string[]) {
  return post<number>('/api/memberlevel/delete-batch', ids)
}