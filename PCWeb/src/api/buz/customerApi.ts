// src/api/buz/customerApi.ts

import { get, post, put } from '@/utils/request'
import type {
  Customer,
  CustomerListParams,
  CreateCustomerParams,
  UpdateCustomerParams,
  CustomerAddress,
  CreateAddressParams,
  UpdateAddressParams,
  CustomerCart,
  CustomerFavorite,
  FavoriteGroup,
  AdjustPointsParams,
  AdjustLevelParams,
} from '@/types'

// ===================== 客户管理 API =====================

/**
 * 获取客户列表
 */
export function getCustomerList(params: CustomerListParams) {
  return post<{ list: Customer[]; total: number }>('/api/customer/list', params)
}

/**
 * 获取客户详情
 */
export function getCustomerDetail(id: string) {
  return get<Customer>(`/api/customer/detail/${id}`)
}

/**
 * 创建客户
 */
export function createCustomer(data: CreateCustomerParams) {
  return post<{ id: string }>('/api/customer/create', data)
}

/**
 * 更新客户
 */
export function updateCustomer(data: UpdateCustomerParams) {
  return put<{ success: boolean }>('/api/customer/update', data)
}

/**
 * 删除客户
 */
export function deleteCustomer(ids: string | string[]) {
  const idsArray = Array.isArray(ids) ? ids : [ids]
  return post<number>('/api/customer/delete', idsArray)
}

/**
 * 更新客户状态
 */
export function updateCustomerStatus(id: string, status: number) {
  return put<{ success: boolean }>('/api/customer/update-status', { id, status })
}

/**
 * 调整客户积分
 */
export function adjustCustomerPoints(data: AdjustPointsParams) {
  return put<{ success: boolean }>('/api/customer/adjust-points', data)
}

/**
 * 调整客户等级
 */
export function adjustCustomerLevel(data: AdjustLevelParams) {
  return put<{ success: boolean }>('/api/customer/adjust-level', data)
}

// ===================== 收货地址 API =====================

/**
 * 获取客户地址列表
 */
export function getCustomerAddressList(userId: string) {
  return post<CustomerAddress[]>(`/api/customer/address/${userId}`)
}

/**
 * 创建客户地址
 */
export function createCustomerAddress(data: CreateAddressParams) {
  return post<{ id: string }>('/api/customer/address/create', data)
}

/**
 * 更新客户地址
 */
export function updateCustomerAddress(data: UpdateAddressParams) {
  return put<{ success: boolean }>('/api/customer/address/update', data)
}

/**
 * 删除客户地址
 */
export function deleteCustomerAddress(id: string) {
  return post<number>('/api/customer/address/delete', id)
}

/**
 * 设置默认地址
 */
export function setDefaultAddress(userId: string, addressId: string) {
  return put<{ success: boolean }>('/api/customer/address/set-default', { userId, addressId })
}

// ===================== 购物车 API =====================

/**
 * 获取客户购物车
 */
export function getCustomerCartList(userId: string) {
  return post<CustomerCart[]>(`/api/customer/cart/${userId}`)
}

// ===================== 收藏 API =====================

/**
 * 获取客户收藏列表
 */
export function getCustomerFavoriteList(userId: string, groupId?: string) {
  return get<CustomerFavorite[]>(`/api/customer/favorite/${userId}`, { groupId })
}

/**
 * 获取收藏分组列表
 */
export function getFavoriteGroupList(userId: string) {
  return get<FavoriteGroup[]>(`/api/customer/favorite-group/${userId}`)
}