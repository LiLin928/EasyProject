// src/types/logQuery.ts

/**
 * 日志级别
 */
export type LogLevel = 'Debug' | 'Information' | 'Warning' | 'Error' | 'Fatal'

/**
 * HTTP 方法
 */
export type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH'

/**
 * 日志条目（列表展示）
 */
export interface LogEntry {
  /** ES 文档 ID */
  id: string
  /** 日志时间戳 */
  timestamp: string
  /** 日志级别 */
  level: LogLevel
  /** 日志消息 */
  message: string
  /** 请求路径 */
  requestPath?: string
  /** HTTP 方法 */
  method?: HttpMethod
  /** 用户ID */
  userId?: string
  /** 用户名 */
  userName?: string
  /** IP 地址 */
  ipAddress?: string
  /** 执行时长（毫秒） */
  duration?: number
  /** 来源机器名 */
  machineName?: string
  /** 环境名称 */
  environment?: string
  /** 异常信息摘要 */
  exception?: string
}

/**
 * 日志详情（完整信息）
 */
export interface LogDetail {
  /** ES 文档 ID */
  id: string
  /** 日志时间戳 */
  timestamp: string
  /** 日志级别 */
  level: LogLevel
  /** 完整消息 */
  message: string
  requestPath?: string
  method?: HttpMethod
  userId?: string
  userName?: string
  ipAddress?: string
  duration?: number
  machineName?: string
  environment?: string
  /** 完整异常信息 */
  exception?: string
  /** 异常堆栈 */
  stackTrace?: string
  /** 所有附加属性 */
  properties?: Record<string, unknown>
}

/**
 * 查询请求参数
 */
export interface LogQueryParams {
  /** 环境标识 */
  environment: string
  /** 开始时间 */
  startTime?: string
  /** 结束时间 */
  endTime?: string
  /** 日志级别 */
  level?: LogLevel
  /** 请求路径（模糊匹配） */
  requestPath?: string
  /** 消息关键字（全文搜索） */
  messageKeyword?: string
  /** 异常关键字（全文搜索） */
  exceptionKeyword?: string
  /** 页码 */
  pageIndex: number
  /** 每页数量 */
  pageSize: number
}

/**
 * 查询响应结果
 */
export interface LogQueryResponse {
  total: number
  items: LogEntry[]
  /** 实际查询的索引列表（调试用） */
  queriedIndices?: string[]
}