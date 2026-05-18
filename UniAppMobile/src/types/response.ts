// types/response.ts

/** API 响应结构 */
export interface ApiResponse<T = any> {
  /** 状态码 */
  code: number
  /** 响应消息 */
  message: string
  /** 响应数据 */
  data: T
}

/** 分页响应 */
export interface PageResponse<T> {
  /** 数据列表 */
  list: T[]
  /** 总数量 */
  total: number
  /** 当前页码 */
  pageIndex: number
  /** 每页数量 */
  pageSize: number
}