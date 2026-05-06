// src/mock/task.ts

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * 任务定义 Mock
 */
interface MockTask {
  id: string
  taskName: string
  taskGroup: string
  taskType: number
  taskTypeText: string
  scheduleType?: number
  cronExpression?: string
  executeHour?: number
  executeMinute?: number
  dayOfMonth?: number
  executorType: number
  handlerType?: string
  handlerMethod?: string
  apiEndpoint?: string
  apiMethod?: string
  apiPayload?: string
  status: number
  statusText: string
  maxRetries: number
  timeoutSeconds: number
  description?: string
  nextExecuteTime?: string
  lastExecuteTime?: string
  createTime: string
}

/**
 * 任务执行日志 Mock
 */
interface MockTaskLog {
  id: string
  jobName: string
  jobGroup: string
  status: number
  statusText: string
  startTime: string
  endTime?: string
  duration?: number
  resultMessage?: string
  exceptionMessage?: string
  exceptionStackTrace?: string
  triggerType: number
  createTime: string
}

// 格式化日期时间
function formatDateTime(date: Date): string {
  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  const hours = String(date.getHours()).padStart(2, '0')
  const minutes = String(date.getMinutes()).padStart(2, '0')
  const seconds = String(date.getSeconds()).padStart(2, '0')
  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`
}

// 任务类型文本
function getTaskTypeText(type: number): string {
  const texts = { 0: 'Cron定时', 1: '即时执行', 2: '周期任务' }
  return texts[type] || '未知'
}

// 任务状态文本
function getStatusText(status: number): string {
  const texts = { 0: '待调度', 1: '已调度', 2: '已暂停', 3: '已完成', 4: '失败' }
  return texts[status] || '未知'
}

// 日志状态文本
function getLogStatusText(status: number): string {
  const texts = { 0: '执行中', 1: '成功', 2: '失败', 3: '取消' }
  return texts[status] || '未知'
}

// 生成测试任务数据
function generateMockTasks(): MockTask[] {
  const now = new Date()

  const tasks: MockTask[] = [
    {
      id: generateGuid(),
      taskName: '日志清理任务',
      taskGroup: 'Default',
      taskType: 0,
      taskTypeText: 'Cron定时',
      cronExpression: '0 0 2 * * ?',
      executorType: 0,
      handlerType: 'ClearLogJob',
      handlerMethod: 'ExecuteAsync',
      status: 1,
      statusText: '已调度',
      maxRetries: 3,
      timeoutSeconds: 300,
      description: '每天凌晨2点清理30天前的操作日志',
      nextExecuteTime: formatDateTime(new Date(now.getTime() + 24 * 60 * 60 * 1000)),
      createTime: formatDateTime(new Date(now.getTime() - 30 * 24 * 60 * 60 * 1000)),
    },
    {
      id: generateGuid(),
      taskName: '数据同步任务',
      taskGroup: 'Default',
      taskType: 2,
      taskTypeText: '周期任务',
      scheduleType: 0,
      executeHour: 8,
      executeMinute: 0,
      executorType: 1,
      apiEndpoint: 'http://localhost:7600/api/sync/data',
      apiMethod: 'POST',
      apiPayload: '{"source": "erp"}',
      status: 1,
      statusText: '已调度',
      maxRetries: 5,
      timeoutSeconds: 600,
      description: '每天早上8点同步ERP数据',
      nextExecuteTime: formatDateTime(new Date(now.getTime() + 12 * 60 * 60 * 1000)),
      createTime: formatDateTime(new Date(now.getTime() - 15 * 24 * 60 * 60 * 1000)),
    },
    {
      id: generateGuid(),
      taskName: '报表生成任务',
      taskGroup: 'Default',
      taskType: 0,
      taskTypeText: 'Cron定时',
      cronExpression: '0 30 6 ? * MON',  // Quartz 6-field: 每周一 6:30
      executorType: 0,
      handlerType: 'ReportGeneratorJob',
      handlerMethod: 'GenerateWeeklyReport',
      status: 1,
      statusText: '已调度',
      maxRetries: 3,
      timeoutSeconds: 1800,
      description: '每周一早上6:30生成周报表',
      nextExecuteTime: formatDateTime(new Date(now.getTime() + 3 * 24 * 60 * 60 * 1000)),
      createTime: formatDateTime(new Date(now.getTime() - 20 * 24 * 60 * 60 * 1000)),
    },
    {
      id: generateGuid(),
      taskName: '库存预警检查',
      taskGroup: 'Default',
      taskType: 2,
      taskTypeText: '周期任务',
      scheduleType: 0,
      executeHour: 10,
      executeMinute: 30,
      executorType: 0,
      handlerType: 'StockAlertJob',
      handlerMethod: 'CheckStockLevel',
      status: 2,
      statusText: '已暂停',
      maxRetries: 3,
      timeoutSeconds: 120,
      description: '每天10:30检查库存预警',
      createTime: formatDateTime(new Date(now.getTime() - 10 * 24 * 60 * 60 * 1000)),
    },
    {
      id: generateGuid(),
      taskName: '会员积分结算',
      taskGroup: 'Default',
      taskType: 2,
      taskTypeText: '周期任务',
      scheduleType: 1,
      executeHour: 0,
      executeMinute: 0,
      dayOfMonth: 1,
      executorType: 1,
      apiEndpoint: 'http://localhost:7600/api/member/settle-points',
      apiMethod: 'POST',
      status: 1,
      statusText: '已调度',
      maxRetries: 5,
      timeoutSeconds: 3600,
      description: '每月1日凌晨结算会员积分',
      nextExecuteTime: formatDateTime(new Date(now.getFullYear(), now.getMonth() + 1, 1)),
      createTime: formatDateTime(new Date(now.getTime() - 60 * 24 * 60 * 60 * 1000)),
    },
    {
      id: generateGuid(),
      taskName: '订单超时取消',
      taskGroup: 'Default',
      taskType: 0,
      taskTypeText: 'Cron定时',
      cronExpression: '0 */30 * * * ?',
      executorType: 0,
      handlerType: 'OrderTimeoutJob',
      handlerMethod: 'CancelTimeoutOrders',
      status: 1,
      statusText: '已调度',
      maxRetries: 3,
      timeoutSeconds: 60,
      description: '每30分钟检查超时未支付订单',
      nextExecuteTime: formatDateTime(new Date(now.getTime() + 30 * 60 * 1000)),
      createTime: formatDateTime(new Date(now.getTime() - 5 * 24 * 60 * 60 * 1000)),
    },
    {
      id: generateGuid(),
      taskName: '优惠券过期提醒',
      taskGroup: 'Default',
      taskType: 0,
      taskTypeText: 'Cron定时',
      cronExpression: '0 0 9 * * ?',
      executorType: 1,
      apiEndpoint: 'http://localhost:7600/api/coupon/expire-notice',
      apiMethod: 'POST',
      status: 2,
      statusText: '已暂停',
      maxRetries: 2,
      timeoutSeconds: 300,
      description: '每天早上9点发送优惠券过期提醒',
      createTime: formatDateTime(new Date(now.getTime() - 8 * 24 * 60 * 60 * 1000)),
    },
    {
      id: generateGuid(),
      taskName: '系统健康检查',
      taskGroup: 'System',
      taskType: 0,
      taskTypeText: 'Cron定时',
      cronExpression: '0 */5 * * * ?',
      executorType: 0,
      handlerType: 'HealthCheckJob',
      handlerMethod: 'CheckSystemHealth',
      status: 1,
      statusText: '已调度',
      maxRetries: 1,
      timeoutSeconds: 30,
      description: '每5分钟检查系统健康状态',
      nextExecuteTime: formatDateTime(new Date(now.getTime() + 5 * 60 * 1000)),
      createTime: formatDateTime(new Date(now.getTime() - 2 * 24 * 60 * 60 * 1000)),
    },
  ]

  return tasks
}

// 生成测试日志数据
function generateMockTaskLogs(): MockTaskLog[] {
  const now = new Date()
  const logs: MockTaskLog[] = []

  const taskNames = [
    '日志清理任务',
    '数据同步任务',
    '报表生成任务',
    '库存预警检查',
    '会员积分结算',
    '订单超时取消',
    '优惠券过期提醒',
    '系统健康检查',
  ]

  // 生成最近7天的执行日志
  for (let day = 0; day < 7; day++) {
    const dayDate = new Date(now.getTime() - day * 24 * 60 * 60 * 1000)

    // 每天生成 5-15 条日志
    const logCount = Math.floor(Math.random() * 10) + 5

    for (let i = 0; i < logCount; i++) {
      const hour = Math.floor(Math.random() * 24)
      const minute = Math.floor(Math.random() * 60)
      const second = Math.floor(Math.random() * 60)

      const startTime = new Date(dayDate.getFullYear(), dayDate.getMonth(), dayDate.getDate(), hour, minute, second)
      const isSuccess = Math.random() > 0.1 // 90% 成功率
      const duration = Math.floor(Math.random() * 2000) + 50 // 50-2050ms

      const endTime = new Date(startTime.getTime() + duration)

      const log: MockTaskLog = {
        id: generateGuid(),
        jobName: taskNames[Math.floor(Math.random() * taskNames.length)],
        jobGroup: 'Default',
        status: isSuccess ? 1 : 2,
        statusText: getLogStatusText(isSuccess ? 1 : 2),
        startTime: formatDateTime(startTime),
        endTime: formatDateTime(endTime),
        duration,
        triggerType: Math.random() > 0.3 ? 0 : 1, // 70% Cron触发
        resultMessage: isSuccess ? '执行成功' : undefined,
        exceptionMessage: isSuccess ? undefined : '连接超时：远程服务器响应时间超过限制',
        exceptionStackTrace: isSuccess ? undefined :
          'System.TimeoutException: The operation has timed out.\n' +
          '   at TaskExecutor.ExecuteAsync() in /src/TaskExecutor.cs:line 45\n' +
          '   at TaskScheduler.RunTask() in /src/TaskScheduler.cs:line 28',
        createTime: formatDateTime(startTime),
      }

      logs.push(log)
    }
  }

  // 按时间倒序排列
  logs.sort((a, b) => new Date(b.startTime).getTime() - new Date(a.startTime).getTime())

  return logs
}

// 初始化数据
const mockTasks = generateMockTasks()
const mockTaskLogs = generateMockTaskLogs()

export default [
  // ==================== 任务管理 API ====================

  // 获取任务列表
  {
    url: '/api/task/list',
    method: 'post',
    response: ({ body }: { body: {
      pageIndex?: number
      pageSize?: number
      taskName?: string
      taskType?: number
      status?: number
    } }) => {
      const { pageIndex = 1, pageSize = 10, taskName, taskType, status } = body

      let filtered = mockTasks.filter(task => {
        if (taskName && !task.taskName.includes(taskName)) return false
        if (taskType !== undefined && task.taskType !== taskType) return false
        if (status !== undefined && task.status !== status) return false
        return true
      })

      const total = filtered.length
      const start = (pageIndex - 1) * pageSize
      const list = filtered.slice(start, start + pageSize)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize }
      }
    }
  },

  // 获取任务详情
  {
    url: '/api/task/detail/:id',
    method: 'get',
    response: ({ query }: { query: { id: string } }) => {
      const task = mockTasks.find(t => t.id === query.id)
      if (!task) {
        return { code: 404, message: '任务不存在', data: null }
      }
      return { code: 200, message: '成功', data: task }
    }
  },

  // 创建任务
  {
    url: '/api/task/create',
    method: 'post',
    response: () => {
      const newId = generateGuid()
      return { code: 200, message: '创建成功', data: newId }
    }
  },

  // 更新任务
  {
    url: '/api/task/update',
    method: 'post',
    response: () => {
      return { code: 200, message: '更新成功', data: 1 }
    }
  },

  // 删除任务
  {
    url: '/api/task/delete',
    method: 'post',
    response: () => {
      return { code: 200, message: '删除成功', data: 1 }
    }
  },

  // 暂停任务
  {
    url: '/api/task/pause',
    method: 'post',
    response: () => {
      return { code: 200, message: '任务已暂停', data: 1 }
    }
  },

  // 恢复任务
  {
    url: '/api/task/resume',
    method: 'post',
    response: () => {
      return { code: 200, message: '任务已恢复', data: 1 }
    }
  },

  // 立即执行
  {
    url: '/api/task/trigger',
    method: 'post',
    response: () => {
      return { code: 200, message: '任务已触发执行', data: 1 }
    }
  },

  // 获取任务统计
  {
    url: '/api/task/statistics',
    method: 'get',
    response: () => {
      const todayLogs = mockTaskLogs.filter(log => {
        const logDate = new Date(log.startTime)
        const today = new Date()
        return logDate.toDateString() === today.toDateString()
      })

      return {
        code: 200,
        message: '成功',
        data: {
          totalCount: mockTasks.length,
          enabledCount: mockTasks.filter(t => t.status === 1).length,
          pausedCount: mockTasks.filter(t => t.status === 2).length,
          todayExecuted: todayLogs.length,
          todaySuccess: todayLogs.filter(l => l.status === 1).length,
          todayFailure: todayLogs.filter(l => l.status === 2).length,
        }
      }
    }
  },

  // ==================== 日志查询 API ====================

  // 获取日志列表
  {
    url: '/api/tasklog/list',
    method: 'post',
    response: ({ body }: { body: {
      pageIndex?: number
      pageSize?: number
      jobName?: string
      status?: number
      triggerType?: number
      startTime?: string
      endTime?: string
    } }) => {
      const { pageIndex = 1, pageSize = 20, jobName, status, triggerType, startTime, endTime } = body

      let filtered = mockTaskLogs.filter(log => {
        if (jobName && !log.jobName.includes(jobName)) return false
        if (status !== undefined && log.status !== status) return false
        if (triggerType !== undefined && log.triggerType !== triggerType) return false
        if (startTime && new Date(log.startTime) < new Date(startTime)) return false
        if (endTime && new Date(log.startTime) > new Date(endTime)) return false
        return true
      })

      const total = filtered.length
      const start = (pageIndex - 1) * pageSize
      const list = filtered.slice(start, start + pageSize)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize }
      }
    }
  },

  // 获取日志详情
  {
    url: '/api/tasklog/detail/:id',
    method: 'get',
    response: ({ query }: { query: { id: string } }) => {
      const log = mockTaskLogs.find(l => l.id === query.id)
      if (!log) {
        return { code: 404, message: '日志不存在', data: null }
      }
      return { code: 200, message: '成功', data: log }
    }
  },

  // 清理日志
  {
    url: '/api/tasklog/clear',
    method: 'post',
    response: () => {
      return { code: 200, message: '清理完成', data: 10 }
    }
  },

  // 获取执行趋势
  {
    url: '/api/tasklog/trend',
    method: 'get',
    response: ({ query }: { query: { days?: string } }) => {
      const days = parseInt(query.days || '7', 10)
      const now = new Date()
      const points = []

      for (let i = days - 1; i >= 0; i--) {
        const date = new Date(now.getTime() - i * 24 * 60 * 60 * 1000)
        const dateStr = `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`

        const dayLogs = mockTaskLogs.filter(log => {
          const logDate = new Date(log.startTime)
          return logDate.toDateString() === date.toDateString()
        })

        const executeCount = dayLogs.length
        const successCount = dayLogs.filter(l => l.status === 1).length
        const successRate = executeCount > 0 ? Math.round((successCount / executeCount) * 100) : 0

        points.push({ date: dateStr, executeCount, successCount, successRate })
      }

      return { code: 200, message: '成功', data: { points } }
    }
  },

  // 获取日志统计
  {
    url: '/api/tasklog/statistics',
    method: 'get',
    response: () => {
      const today = new Date()
      const todayLogs = mockTaskLogs.filter(log => {
        const logDate = new Date(log.startTime)
        return logDate.toDateString() === today.toDateString()
      })

      return {
        code: 200,
        message: '成功',
        data: {
          totalCount: 0,
          enabledCount: 0,
          pausedCount: 0,
          todayExecuted: todayLogs.length,
          todaySuccess: todayLogs.filter(l => l.status === 1).length,
          todayFailure: todayLogs.filter(l => l.status === 2).length,
        }
      }
    }
  },
] as MockMethod[]