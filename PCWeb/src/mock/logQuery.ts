// src/mock/logQuery.ts

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * 日志级别
 */
type LogLevel = 'Debug' | 'Information' | 'Warning' | 'Error' | 'Fatal'

/**
 * HTTP 方法
 */
type HttpMethod = 'GET' | 'POST' | 'PUT' | 'DELETE'

/**
 * Mock 日志接口
 */
interface MockLog {
  id: string
  timestamp: string
  level: LogLevel
  message: string
  requestPath: string
  method: HttpMethod
  userId: string
  userName: string
  ipAddress: string
  duration: number
  machineName: string
  environment: 'Development' | 'Production'
  exception?: string
  stackTrace?: string
  properties?: Record<string, any>
}

/**
 * 模拟请求路径
 */
const REQUEST_PATHS: string[] = [
  '/api/user/list',
  '/api/user/detail',
  '/api/user/create',
  '/api/user/update',
  '/api/user/delete',
  '/api/order/list',
  '/api/order/detail',
  '/api/order/create',
  '/api/order/ship',
  '/api/product/list',
  '/api/product/detail',
  '/api/product/create',
  '/api/report/list',
  '/api/report/detail',
  '/api/workflow/list',
  '/api/workflow/start',
  '/api/workflow/approve',
  '/api/auth/login',
  '/api/auth/logout',
  '/api/auth/refresh',
  '/api/dict/list',
  '/api/menu/list',
  '/api/role/list',
  '/api/customer/list',
  '/api/banner/list',
  '/api/refund/list',
]

/**
 * 模拟机器名称
 */
const MACHINE_NAMES: string[] = ['server-01', 'server-02', 'server-03']

/**
 * 模拟用户
 */
const MOCK_USERS: { id: string; name: string }[] = [
  { id: '1', name: 'admin' },
  { id: '2', name: '张三' },
  { id: '3', name: '李四' },
  { id: '4', name: '王五' },
]

/**
 * 格式化日期时间
 * @param date - 日期对象
 * @returns 格式化后的日期时间字符串
 */
function formatDateTime(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')
  const seconds = String(date.getSeconds()).padStart(2, '0')
  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`
}

/**
 * 根据日志级别生成消息
 * @param level - 日志级别
 * @param path - 请求路径
 * @returns 日志消息
 */
function generateMessage(level: LogLevel, path: string): string {
  const messagesByLevel: Record<LogLevel, string[]> = {
    Debug: [
      `Processing request for ${path}`,
      `Entering method handler for ${path}`,
      `Parameter validation completed for ${path}`,
      `Database query initiated for ${path}`,
      `Cache lookup for ${path}`,
    ],
    Information: [
      `Request completed successfully: ${path}`,
      `User authenticated for ${path}`,
      `Data retrieved from ${path}`,
      `Operation executed: ${path}`,
      `Session created for ${path}`,
    ],
    Warning: [
      `Slow response time detected for ${path}`,
      `Cache miss for ${path}`,
      `Rate limit approaching for ${path}`,
      `Deprecated API usage: ${path}`,
      `Resource utilization high for ${path}`,
    ],
    Error: [
      `Database connection failed for ${path}`,
      `Authentication error for ${path}`,
      `Validation failed for ${path}`,
      `External service timeout for ${path}`,
      `Permission denied for ${path}`,
    ],
    Fatal: [
      `Application crashed during ${path}`,
      `Critical database failure for ${path}`,
      `Memory exhaustion during ${path}`,
      `Unrecoverable error for ${path}`,
      `System failure for ${path}`,
    ],
  }

  const messages = messagesByLevel[level]
  return messages[Math.floor(Math.random() * messages.length)]
}

/**
 * 生成异常信息
 * @returns 异常消息
 */
function generateException(): string {
  const exceptions: string[] = [
    'System.NullReferenceException: Object reference not set to an instance of an object.',
    'System.Data.SqlClient.SqlException: Connection timeout expired.',
    'System.UnauthorizedAccessException: Access to the path is denied.',
    'System.ArgumentException: Invalid argument provided.',
    'System.InvalidOperationException: Operation is not valid due to the current state of the object.',
    'System.TimeoutException: The operation has timed out.',
    'System.IO.IOException: An I/O error occurred.',
    'System.OutOfMemoryException: Insufficient memory to continue the execution of the program.',
  ]
  return exceptions[Math.floor(Math.random() * exceptions.length)]
}

/**
 * 生成堆栈跟踪
 * @returns 堆栈跟踪字符串
 */
function generateStackTrace(): string {
  const stackFrames: string[] = [
    '   at Application.Controllers.UserController.GetUser(String id) in C:\\src\\Controllers\\UserController.cs:line 45',
    '   at Application.Services.UserService.GetById(String id) in C:\\src\\Services\\UserService.cs:line 28',
    '   at Application.Data.Repositories.UserRepository.Find(String id) in C:\\src\\Data\\Repositories\\UserRepository.cs:line 15',
    '   at Application.Controllers.OrderController.ProcessOrder(OrderDto order) in C:\\src\\Controllers\\OrderController.cs:line 67',
    '   at Application.Services.OrderService.ValidateOrder(OrderDto order) in C:\\src\\Services\\OrderService.cs:line 42',
    '   at Application.Controllers.ReportController.GenerateReport(ReportRequest request) in C:\\src\\Controllers\\ReportController.cs:line 89',
    '   at Application.Services.ReportService.ExecuteQuery(String sql) in C:\\src\\Services\\ReportService.cs:line 56',
    '   at Application.Data.DbContext.OpenConnection() in C:\\src\\Data\\DbContext.cs:line 12',
    '   at System.Threading.Tasks.Task.InnerInvoke()',
    '   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)',
  ]

  // 随机选择 3-6 个堆栈帧
  const count = Math.floor(Math.random() * 4) + 3
  const shuffled = stackFrames.sort(() => 0.5 - Math.random())
  return shuffled.slice(0, count).join('\n')
}

/**
 * 生成模拟 IP 地址
 * @returns IP 地址字符串
 */
function generateIpAddress(): string {
  const prefix = Math.floor(Math.random() * 3) + 1 // 1-3
  const parts = [
    Math.floor(Math.random() * 256),
    Math.floor(Math.random() * 256),
    Math.floor(Math.random() * 256),
  ]
  return `${192}.${168}.${prefix}.${parts[0]}`
}

/**
 * 生成模拟日志数据
 * @returns Mock 日志数组
 */
function generateMockLogs(): MockLog[] {
  const logs: MockLog[] = []
  const now = new Date()

  // 日志级别分布权重: Debug=10, Information=60, Warning=15, Error=10, Fatal=5
  const levelWeights: { level: LogLevel; weight: number }[] = [
    { level: 'Debug', weight: 10 },
    { level: 'Information', weight: 60 },
    { level: 'Warning', weight: 15 },
    { level: 'Error', weight: 10 },
    { level: 'Fatal', weight: 5 },
  ]

  // 根据权重随机选择级别
  const totalWeight = levelWeights.reduce((sum, w) => sum + w.weight, 0)
  const getRandomLevel = (): LogLevel => {
    const random = Math.random() * totalWeight
    let cumulative = 0
    for (const { level, weight } of levelWeights) {
      cumulative += weight
      if (random <= cumulative) return level
    }
    return 'Information'
  }

  // HTTP 方法分布
  const getMethodByPath = (path: string): HttpMethod => {
    if (path.includes('/create') || path.includes('/start')) return 'POST'
    if (path.includes('/update') || path.includes('/approve') || path.includes('/ship')) return 'PUT'
    if (path.includes('/delete')) return 'DELETE'
    return 'GET'
  }

  // 生成大约 50 条日志，分布在过去 7 天
  const logCount = 50

  for (let i = 0; i < logCount; i++) {
    // 随机时间：过去 7 天内
    const daysOffset = Math.floor(Math.random() * 7)
    const hoursOffset = Math.floor(Math.random() * 24)
    const minutesOffset = Math.floor(Math.random() * 60)
    const logTime = new Date(now.getTime() - daysOffset * 24 * 60 * 60 * 1000 - hoursOffset * 60 * 60 * 1000 - minutesOffset * 60 * 1000)

    const level = getRandomLevel()
    const path = REQUEST_PATHS[Math.floor(Math.random() * REQUEST_PATHS.length)]
    const method = getMethodByPath(path)
    const user = MOCK_USERS[Math.floor(Math.random() * MOCK_USERS.length)]
    const machine = MACHINE_NAMES[Math.floor(Math.random() * MACHINE_NAMES.length)]
    const environment: 'Development' | 'Production' = Math.random() > 0.3 ? 'Production' : 'Development'

    // 请求持续时间：根据级别调整
    let duration: number
    if (level === 'Debug') {
      duration = Math.floor(Math.random() * 100) + 10 // 10-110ms
    } else if (level === 'Information') {
      duration = Math.floor(Math.random() * 500) + 50 // 50-550ms
    } else if (level === 'Warning') {
      duration = Math.floor(Math.random() * 2000) + 500 // 500-2500ms (慢请求)
    } else if (level === 'Error') {
      duration = Math.floor(Math.random() * 5000) + 1000 // 1-6s (错误请求可能较慢)
    } else {
      duration = Math.floor(Math.random() * 10000) + 5000 // 5-15s (致命错误)
    }

    const log: MockLog = {
      id: generateGuid(),
      timestamp: formatDateTime(logTime),
      level,
      message: generateMessage(level, path),
      requestPath: path,
      method,
      userId: user.id,
      userName: user.name,
      ipAddress: generateIpAddress(),
      duration,
      machineName: machine,
      environment,
    }

    // Error 和 Fatal 级别添加异常和堆栈跟踪
    if (level === 'Error' || level === 'Fatal') {
      log.exception = generateException()
      log.stackTrace = generateStackTrace()
    }

    // 添加一些额外属性
    log.properties = {
      requestId: generateGuid().substring(0, 8),
      userAgent: 'Mozilla/5.0 (Windows NT 10.0; Win64; x64)',
      correlationId: generateGuid().substring(0, 8),
    }

    logs.push(log)
  }

  // 按时间倒序排列
  logs.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime())

  return logs
}

// 初始化日志数据
const mockLogs: MockLog[] = generateMockLogs()

/**
 * 生成 CSV 内容
 * @param logs - 日志数组
 * @returns CSV 字符串
 */
function generateCsvContent(logs: MockLog[]): string {
  const headers = [
    'ID',
    'Timestamp',
    'Level',
    'Message',
    'RequestPath',
    'Method',
    'UserId',
    'UserName',
    'IpAddress',
    'Duration',
    'MachineName',
    'Environment',
    'Exception',
    'StackTrace',
  ]

  const rows = logs.map((log) => [
    log.id,
    log.timestamp,
    log.level,
    `"${log.message.replace(/"/g, '""')}"`, // CSV 字符串转义
    log.requestPath,
    log.method,
    log.userId,
    log.userName,
    log.ipAddress,
    log.duration,
    log.machineName,
    log.environment,
    log.exception ? `"${log.exception.replace(/"/g, '""')}"` : '',
    log.stackTrace ? `"${log.stackTrace.replace(/"/g, '""').replace(/\n/g, '\\n')}"` : '',
  ])

  return [headers.join(','), ...rows.map((row) => row.join(','))].join('\n')
}

export default [
  // ==================== 日志查询 API ====================

  // 获取环境列表
  {
    url: '/api/logquery/environments',
    method: 'get',
    response: () => {
      return {
        code: 200,
        message: '成功',
        data: ['Development', 'Production'],
      }
    },
  },

  // 查询日志列表
  {
    url: '/api/logquery/query',
    method: 'post',
    response: ({ body }: { body: {
      pageIndex?: number
      pageSize?: number
      environment?: string
      level?: LogLevel
      keyword?: string
      startTime?: string
      endTime?: string
      machineName?: string
      userId?: string
      requestPath?: string
      method?: HttpMethod
    } }) => {
      const {
        pageIndex = 1,
        pageSize = 20,
        environment,
        level,
        keyword,
        startTime,
        endTime,
        machineName,
        userId,
        requestPath,
        method,
      } = body

      // 过滤日志
      let filteredLogs = mockLogs.filter((log) => {
        let match = true

        if (environment && log.environment !== environment) {
          match = false
        }
        if (level && log.level !== level) {
          match = false
        }
        if (keyword && !log.message.toLowerCase().includes(keyword.toLowerCase()) &&
            !log.requestPath.toLowerCase().includes(keyword.toLowerCase())) {
          match = false
        }
        if (startTime && new Date(log.timestamp) < new Date(startTime)) {
          match = false
        }
        if (endTime && new Date(log.timestamp) > new Date(endTime + ' 23:59:59')) {
          match = false
        }
        if (machineName && log.machineName !== machineName) {
          match = false
        }
        if (userId && log.userId !== userId) {
          match = false
        }
        if (requestPath && !log.requestPath.includes(requestPath)) {
          match = false
        }
        if (method && log.method !== method) {
          match = false
        }

        return match
      })

      // 分页
      const total = filteredLogs.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredLogs.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: {
          list,
          total,
          pageIndex,
          pageSize,
        },
      }
    },
  },

  // 获取日志详情
  {
    url: '/api/logquery/detail/:environment/:id',
    method: 'get',
    response: ({ query }: { query: { environment: string; id: string } }) => {
      const { environment, id } = query

      const log = mockLogs.find((l) => l.id === id && l.environment === environment)

      if (!log) {
        return {
          code: 404,
          message: '日志记录不存在',
          data: null,
        }
      }

      return {
        code: 200,
        message: '成功',
        data: log,
      }
    },
  },

  // 导出日志
  {
    url: '/api/logquery/export',
    method: 'post',
    response: ({ body }: { body: {
      environment?: string
      level?: LogLevel
      keyword?: string
      startTime?: string
      endTime?: string
      machineName?: string
      userId?: string
      requestPath?: string
      method?: HttpMethod
    } }) => {
      const {
        environment,
        level,
        keyword,
        startTime,
        endTime,
        machineName,
        userId,
        requestPath,
        method,
      } = body

      // 过滤日志
      let filteredLogs = mockLogs.filter((log) => {
        let match = true

        if (environment && log.environment !== environment) {
          match = false
        }
        if (level && log.level !== level) {
          match = false
        }
        if (keyword && !log.message.toLowerCase().includes(keyword.toLowerCase()) &&
            !log.requestPath.toLowerCase().includes(keyword.toLowerCase())) {
          match = false
        }
        if (startTime && new Date(log.timestamp) < new Date(startTime)) {
          match = false
        }
        if (endTime && new Date(log.timestamp) > new Date(endTime + ' 23:59:59')) {
          match = false
        }
        if (machineName && log.machineName !== machineName) {
          match = false
        }
        if (userId && log.userId !== userId) {
          match = false
        }
        if (requestPath && !log.requestPath.includes(requestPath)) {
          match = false
        }
        if (method && log.method !== method) {
          match = false
        }

        return match
      })

      // 生成 CSV 内容
      const csvContent = generateCsvContent(filteredLogs)

      return {
        code: 200,
        message: '成功',
        data: {
          content: csvContent,
          fileName: `logs_export_${formatDateTime(new Date()).replace(/[: ]/g, '_')}.csv`,
          recordCount: filteredLogs.length,
        },
      }
    },
  },
] as MockMethod[]