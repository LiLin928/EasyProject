// types/log.ts

/** 日志类型枚举 */
export enum LogType {
  Login = 1,      // 登录日志
  Operation = 2,  // 操作日志
  Exception = 3,  // 异常日志
  System = 4,     // 系统日志
}

/** 操作日志 */
export interface Log {
  /** GUID 主键 */
  id: string
  /** 操作模块 */
  module: string
  /** 操作类型 */
  action: string
  /** 操作人 */
  operator: string
  /** 操作人ID */
  operatorId: string
  /** 目标对象ID */
  targetId?: string
  /** 目标对象名称 */
  targetName?: string
  /** 操作内容 */
  content?: string
  /** 操作IP */
  ip?: string
  /** 日志类型 */
  logType: LogType
  /** 操作时间 */
  createTime: string
}

/** 日志查询参数 */
export interface LogQueryParams {
  pageIndex?: number
  pageSize?: number
  /** 关键词搜索（操作人/模块） */
  keyword?: string
  /** 日志类型筛选 */
  logType?: LogType
  /** 开始时间 */
  startTime?: string
  /** 结束时间 */
  endTime?: string
}