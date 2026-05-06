// src/api/buz/bannerApi.ts

import { get, post, put } from '@/utils/request'
import type {
  Banner,
  CreateBannerParams,
  UpdateBannerParams,
  BannerQueryParams,
} from '@/types'

/**
 * 获取轮播图列表（分页）
 */
export function getBannerList(params?: BannerQueryParams) {
  return post<{ list: Banner[]; total: number }>('/api/banner/list', params)
}

/**
 * 获取所有启用的轮播图（供小程序调用）
 */
export function getActiveBanners() {
  return post<Banner[]>('/api/banner/active')
}

/**
 * 获取轮播图详情
 */
export function getBannerDetail(id: string) {
  return get<Banner>(`/api/banner/detail/${id}`)
}

/**
 * 创建轮播图
 */
export function createBanner(data: CreateBannerParams) {
  return post<{ id: string }>('/api/banner/add', data)
}

/**
 * 更新轮播图
 */
export function updateBanner(data: UpdateBannerParams) {
  return put<{ success: boolean }>('/api/banner/update', data)
}

/**
 * 删除轮播图
 */
export function deleteBanner(id: string) {
  return post<number>('/api/banner/delete', id)
}

/**
 * 批量删除轮播图
 * @param ids 轮播图ID列表 (GUID数组)
 */
export function deleteBannerBatch(ids: string[]) {
  return post<number>('/api/banner/delete-batch', ids)
}

/**
 * 更新轮播图状态
 */
export function updateBannerStatus(id: string, status: number) {
  return put<{ success: boolean }>(`/api/banner/status/${id}?status=${status}`)
}

/**
 * 批量更新排序
 */
export function batchSortBanner(items: Array<{ id: string; sort: number }>) {
  return put<{ success: boolean }>('/api/banner/sort', { items })
}