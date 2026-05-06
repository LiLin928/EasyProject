// src/types/operateLog.ts

/**
 * 操作日志实体（与后端 OperateLogDto 对应）
 */
export interface OperateLog {
  /** 日志ID */
  id: string
  /** 操作用户ID */
  userId?: string
  /** 操作用户名 */
  userName?: string
  /** 操作模块 */
  module?: string
  /** 操作动作 */
  action?: string
  /** 请求方法 */
  method?: string
  /** 请求地址 */
  url?: string
  /** IP地址 */
  ip?: string
  /** 操作地点 */
  location?: string
  /** 请求参数 */
  params?: string
  /** 操作结果 */
  result?: string
  /** 状态：1=成功，0=失败 */
  status: number
  /** 错误信息 */
  errorMsg?: string
  /** 执行时长（毫秒） */
  duration?: number
  /** 操作时间 */
  createTime?: string
}

/**
 * 查询操作日志参数（与后端 QueryOperateLogDto 对应）
 */
export interface QueryOperateLogParams {
  /** 页码 */
  pageIndex: number
  /** 每页数量 */
  pageSize: number
  /** 操作用户ID */
  userId?: string
  /** 操作用户名 */
  userName?: string
  /** 操作模块 */
  module?: string
  /** 操作动作 */
  action?: string
  /** 状态 */
  status?: number
  /** 开始时间 */
  startTime?: string
  /** 结束时间 */
  endTime?: string
  /** IP地址 */
  ip?: string
}

/**
 * 操作日志统计
 */
export interface OperateLogStatistics {
  /** 总数 */
  total: number
  /** 成功数 */
  successCount: number
  /** 失败数 */
  failureCount: number
  /** 今日总数 */
  todayTotal: number
}