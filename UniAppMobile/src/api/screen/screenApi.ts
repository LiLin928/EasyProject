// api/screen/screenApi.ts

import { get } from '@/utils/request'
import { API_PATHS } from '@/config/api.config'
import type { Screen, ScreenQueryParams, PageResponse } from '@/types'

/** 获取大屏列表 */
export function getScreenList(params: ScreenQueryParams): Promise<PageResponse<Screen>> {
  return get<PageResponse<Screen>>(API_PATHS.SCREEN_LIST, params)
}

/** 获取大屏详情 */
export function getScreenDetail(id: string): Promise<Screen> {
  return get<Screen>(`${API_PATHS.SCREEN_DETAIL}/${id}`)
}