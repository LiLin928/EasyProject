/**
 * 大屏类型定义
 */

/**
 * 大屏状态枚举
 */
export enum ScreenStatus {
  Draft = 0,      // 未发布
  Published = 1,  // 已发布
  Archived = 2,   // 已归档
}

/**
 * 大屏信息
 */
export interface Screen {
  id: string              // GUID 主键
  name: string            // 大屏名称
  description?: string    // 大屏描述
  publishUrl?: string     // 发布URL
  thumbnail?: string      // 缩略图
  status: ScreenStatus    // 发布状态
  createTime?: string     // 创建时间
  updateTime?: string     // 更新时间
}

/**
 * 大屏查询参数
 */
export interface ScreenQueryParams {
  pageIndex?: number
  pageSize?: number
  keyword?: string
  status?: ScreenStatus
}