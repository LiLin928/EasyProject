// types/common.types.ts

/** 分页查询参数 */
export interface IPageQuery {
  pageIndex: number;
  pageSize: number;
}

/** 前端分页结果（原有） */
export interface IPageResult<T> {
  list: T[];
  total: number;
  pageIndex: number;
  pageSize?: number;
}

/** 后端分页响应格式 */
export interface IPageResponse<T> {
  list: T[];
  total: number;
  pageIndex: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPrevPage: boolean;
}

/** 通用响应结构 */
export interface IApiResponse<T = any> {
  code: number;
  message: string;
  data: T;
  timestamp: number;
}

/** 时间戳类型 */
export type Timestamp = number;

/** 带时间戳的基础实体 */
export interface IBaseEntity {
  createdAt?: Timestamp;
  updatedAt?: Timestamp;
}

/** 转换后端分页响应为前端格式 */
export function transformPageResponse<T>(response: IPageResponse<T>): IPageResult<T> {
  return {
    list: response.list,
    total: response.total,
    pageIndex: response.pageIndex,
    pageSize: response.pageSize,
  };
}