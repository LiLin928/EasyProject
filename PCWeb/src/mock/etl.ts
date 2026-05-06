/**
 * ETL Mock 数据
 */
import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'
import type {
  DataSource,
  DataSourceListParams,
  CreateDataSourceParams,
  UpdateDataSourceParams,
  DatasourceType,
  DatasourceStatus,
} from '@/types/etl'
import type {
  Pipeline,
  PipelineStatus,
  PipelineQueryParams,
  CreatePipelineParams,
  DagConfig,
} from '@/types/etl'
import type {
  ScheduleTask,
  ScheduleStatus,
  ScheduleType,
  ScheduleQueryParams,
  CreateScheduleParams,
} from '@/types/etl'
import type {
  ExecutionRecord,
  ExecutionStatus,
  ExecutionQueryParams,
} from '@/types/etl'

// ==================== 工具函数 ====================

function formatNow(): string {
  const now = new Date()
  const year = now.getFullYear()
  const month = String(now.getMonth() + 1).padStart(2, '0')
  const day = String(now.getDate()).padStart(2, '0')
  const hours = String(now.getHours()).padStart(2, '0')
  const minutes = String(now.getMinutes()).padStart(2, '0')
  const seconds = String(now.getSeconds()).padStart(2, '0')
  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`
}

function generateNewVersion(currentVersion: string): string {
  const match = currentVersion.match(/v(\d+)\.(\d+)\.(\d+)/)
  if (!match) return 'v1.0.0'
  const [, major, minor, patch] = match
  return `v${major}.${minor}.${Number.parseInt(patch, 10) + 1}`
}

// ==================== 预定义 ID ====================

const USER_IDS = {
  admin: 'user-00000000-0000-0000-0000-000000000001',
  zhangsan: 'user-00000000-0000-0000-0000-000000000002',
  lisi: 'user-00000000-0000-0000-0000-000000000003',
}

const DATASOURCE_IDS = {
  mysql_main: 'ds-aaaaaaaa-0000-0000-0000-000000000001',
  mysql_log: 'ds-aaaaaaaa-0000-0000-0000-000000000002',
  pg_analytics: 'ds-aaaaaaaa-0000-0000-0000-000000000003',
  api_external: 'ds-aaaaaaaa-0000-0000-0000-000000000004',
  ftp_backup: 'ds-aaaaaaaa-0000-0000-0000-000000000005',
}

const PIPELINE_IDS = {
  data_sync: 'pl-bbbbbbbb-0000-0000-0000-000000000001',
  daily_report: 'pl-bbbbbbbb-0000-0000-0000-000000000002',
  etl_process: 'pl-bbbbbbbb-0000-0000-0000-000000000003',
  data_archive: 'pl-bbbbbbbb-0000-0000-0000-000000000004',
}

const SCHEDULE_IDS = {
  schedule1: 'sc-cccccccc-0000-0000-0000-000000000001',
  schedule2: 'sc-cccccccc-0000-0000-0000-000000000002',
}

const EXECUTION_IDS = {
  exec1: 'ex-dddddddd-0000-0000-0000-000000000001',
  exec2: 'ex-dddddddd-0000-0000-0000-000000000002',
  exec3: 'ex-dddddddd-0000-0000-0000-000000000003',
}

// ==================== 数据源数据 ====================

const datasources: DataSource[] = [
  {
    id: DATASOURCE_IDS.mysql_main,
    name: '主数据库 MySQL',
    type: 'mysql' as DatasourceType,
    description: '生产环境主数据库',
    host: '192.168.1.100',
    port: 3306,
    database: 'production',
    username: 'admin',
    config: { password: '******', charset: 'utf8mb4' },
    status: 'connected' as DatasourceStatus,
    creatorId: USER_IDS.admin,
    creatorName: '管理员',
    createTime: '2024-01-10 08:00:00',
    updateTime: '2024-02-15 10:00:00',
  },
  {
    id: DATASOURCE_IDS.mysql_log,
    name: '日志数据库 MySQL',
    type: 'mysql' as DatasourceType,
    description: '日志分析数据库',
    host: '192.168.1.101',
    port: 3306,
    database: 'logs',
    username: 'log_user',
    config: { password: '******' },
    status: 'connected' as DatasourceStatus,
    creatorId: USER_IDS.zhangsan,
    creatorName: '张三',
    createTime: '2024-01-15 09:00:00',
    updateTime: '2024-01-15 09:00:00',
  },
  {
    id: DATASOURCE_IDS.pg_analytics,
    name: '分析数据库 PostgreSQL',
    type: 'postgresql' as DatasourceType,
    description: '数据分析专用数据库',
    host: '192.168.1.102',
    port: 5432,
    database: 'analytics',
    username: 'analyst',
    config: { password: '******' },
    status: 'connected' as DatasourceStatus,
    creatorId: USER_IDS.admin,
    creatorName: '管理员',
    createTime: '2024-02-01 10:00:00',
    updateTime: '2024-02-01 10:00:00',
  },
  {
    id: DATASOURCE_IDS.api_external,
    name: '外部 API 接口',
    type: 'http_api' as DatasourceType,
    description: '第三方数据接口',
    config: { apiUrl: 'https://api.example.com/v1', apiMethod: 'GET', apiAuthType: 'bearer', apiAuthConfig: { token: '******' } },
    status: 'connected' as DatasourceStatus,
    creatorId: USER_IDS.lisi,
    creatorName: '李四',
    createTime: '2024-02-10 14:00:00',
    updateTime: '2024-02-10 14:00:00',
  },
  {
    id: DATASOURCE_IDS.ftp_backup,
    name: '备份 FTP 服务器',
    type: 'ftp' as DatasourceType,
    description: '数据备份 FTP',
    config: { ftpHost: 'ftp.backup.com', ftpPort: 21, ftpUsername: 'backup_user', ftpPassword: '******', ftpPath: '/backup/data', ftpProtocol: 'ftp' },
    status: 'disconnected' as DatasourceStatus,
    creatorId: USER_IDS.admin,
    creatorName: '管理员',
    createTime: '2024-01-20 08:00:00',
    updateTime: '2024-03-01 09:00:00',
  },
]

// ==================== 任务流数据 ====================

const defaultDagConfig: DagConfig = { version: '1.0', nodes: [], edges: [] }

const pipelines: Pipeline[] = [
  { id: PIPELINE_IDS.data_sync, name: '用户数据同步', description: '每日同步用户数据到分析库', categoryCode: 'sync', categoryName: '数据同步', status: 1 as PipelineStatus, currentVersion: 'v1.0.0', dagConfig: defaultDagConfig, creatorId: USER_IDS.admin, creatorName: '管理员', createTime: '2024-02-01 08:00:00', updateTime: '2024-02-15 10:00:00' },
  { id: PIPELINE_IDS.daily_report, name: '日报生成', description: '每日生成运营报表', categoryCode: 'report', categoryName: '报表生成', status: 1 as PipelineStatus, currentVersion: 'v1.0.2', dagConfig: defaultDagConfig, creatorId: USER_IDS.zhangsan, creatorName: '张三', createTime: '2024-01-15 09:00:00', updateTime: '2024-02-20 14:00:00' },
  { id: PIPELINE_IDS.etl_process, name: 'ETL 数据处理', description: '主数据清洗转换流程', categoryCode: 'etl', categoryName: 'ETL处理', status: 0 as PipelineStatus, currentVersion: 'v0.1.0', dagConfig: defaultDagConfig, creatorId: USER_IDS.admin, creatorName: '管理员', createTime: '2024-03-01 10:00:00', updateTime: '2024-03-01 10:00:00' },
  { id: PIPELINE_IDS.data_archive, name: '数据归档', description: '历史数据归档处理', categoryCode: 'archive', categoryName: '数据归档', status: 1 as PipelineStatus, currentVersion: 'v2.0.0', dagConfig: defaultDagConfig, scheduleConfig: { type: 'cron' as ScheduleType, cron: { expression: '0 2 * * *' } }, creatorId: USER_IDS.lisi, creatorName: '李四', createTime: '2024-01-01 08:00:00', updateTime: '2024-02-01 09:00:00' },
]

// ==================== 调度数据 ====================

const schedules: ScheduleTask[] = [
  { id: SCHEDULE_IDS.schedule1, name: '数据同步调度', pipelineId: PIPELINE_IDS.data_sync, pipelineName: '用户数据同步', cronExpression: '0 6 * * *', cronDescription: '每天早上6点执行', config: { type: 'cron' as ScheduleType, cron: { expression: '0 6 * * *', timezone: 'Asia/Shanghai' }, concurrency: 1, maxRetryTimes: 3, retryInterval: 300 }, status: 1 as ScheduleStatus, lastExecuteTime: '2024-03-15 06:00:00', nextExecuteTime: '2024-03-16 06:00:00', creatorId: USER_IDS.admin, creatorName: '管理员', createTime: '2024-02-01 08:00:00', updateTime: '2024-02-01 08:00:00' },
  { id: SCHEDULE_IDS.schedule2, name: '日报生成调度', pipelineId: PIPELINE_IDS.daily_report, pipelineName: '日报生成', cronExpression: '0 8 * * *', cronDescription: '每天早上8点执行', config: { type: 'cron' as ScheduleType, cron: { expression: '0 8 * * *' }, concurrency: 1 }, status: 1 as ScheduleStatus, lastExecuteTime: '2024-03-15 08:00:00', nextExecuteTime: '2024-03-16 08:00:00', creatorId: USER_IDS.zhangsan, creatorName: '张三', createTime: '2024-01-15 09:00:00', updateTime: '2024-01-15 09:00:00' },
]

// ==================== 执行记录数据 ====================

const executions: ExecutionRecord[] = [
  { id: EXECUTION_IDS.exec1, executionNo: 'EXEC-20240315-001', pipelineId: PIPELINE_IDS.data_sync, pipelineName: '用户数据同步', scheduleId: SCHEDULE_IDS.schedule1, triggerType: 'schedule', status: 'success' as ExecutionStatus, startTime: '2024-03-15 06:00:00', endTime: '2024-03-15 06:05:30', duration: 330, progress: 100, dagSnapshot: defaultDagConfig, taskExecutions: [], createTime: '2024-03-15 06:00:00' },
  { id: EXECUTION_IDS.exec2, executionNo: 'EXEC-20240315-002', pipelineId: PIPELINE_IDS.daily_report, pipelineName: '日报生成', scheduleId: SCHEDULE_IDS.schedule2, triggerType: 'schedule', status: 'success' as ExecutionStatus, startTime: '2024-03-15 08:00:00', endTime: '2024-03-15 08:10:00', duration: 600, progress: 100, dagSnapshot: defaultDagConfig, taskExecutions: [], createTime: '2024-03-15 08:00:00' },
  { id: EXECUTION_IDS.exec3, executionNo: 'EXEC-20240315-003', pipelineId: PIPELINE_IDS.etl_process, pipelineName: 'ETL 数据处理', triggerType: 'manual', triggerUserId: USER_IDS.admin, triggerUserName: '管理员', status: 'failure' as ExecutionStatus, startTime: '2024-03-15 10:00:00', endTime: '2024-03-15 10:02:00', duration: 120, progress: 50, dagSnapshot: defaultDagConfig, taskExecutions: [], errorMessage: '数据库连接超时', createTime: '2024-03-15 10:00:00' },
]

// ==================== 数据源 API ====================

const datasourceApis: MockMethod[] = [
  { url: '/api/etl/datasource/list', method: 'get', response: ({ query }: { query: DataSourceListParams }) => {
    const { pageIndex = 1, pageSize = 10, name, type } = query
    let filtered = datasources.filter((ds) => {
      let match = true
      if (name && !ds.name.toLowerCase().includes(name.toLowerCase())) match = false
      if (type && ds.type !== type) match = false
      return match
    })
    filtered.sort((a, b) => new Date(b.updateTime || 0).getTime() - new Date(a.updateTime || 0).getTime())
    const total = filtered.length
    const list = filtered.slice((pageIndex - 1) * pageSize, pageIndex * pageSize)
    return { code: 200, message: '成功', data: { list, total, pageIndex, pageSize } }
  }},
  { url: '/api/etl/datasource/detail', method: 'get', response: ({ query }: { query: { id: string } }) => {
    const ds = datasources.find((d) => d.id === query.id)
    if (!ds) return { code: 404, message: '数据源不存在', data: null }
    return { code: 200, message: '成功', data: ds }
  }},
  { url: '/api/etl/datasource/create', method: 'post', response: ({ body }: { body: CreateDataSourceParams }) => {
    if (!body.name || !body.type) return { code: 400, message: '名称和类型不能为空', data: null }
    if (datasources.some((d) => d.name === body.name)) return { code: 400, message: '数据源名称已存在', data: null }
    const now = formatNow()
    const newDs: DataSource = {
      id: generateGuid(),
      name: body.name,
      type: body.type,
      description: body.description || '',
      host: body.host,
      port: body.port,
      database: body.database,
      username: body.username,
      status: 'connected' as DatasourceStatus,
      creatorId: USER_IDS.admin,
      creatorName: '管理员',
      createTime: now,
      updateTime: now,
    }
    datasources.push(newDs)
    return { code: 200, message: '创建成功', data: newDs }
  }},
  { url: '/api/etl/datasource/update', method: 'post', response: ({ body }: { body: UpdateDataSourceParams }) => {
    const idx = datasources.findIndex((d) => d.id === body.id)
    if (idx === -1) return { code: 404, message: '数据源不存在', data: null }
    const ds = datasources[idx]
    if (body.name && body.name !== ds.name && datasources.some((d) => d.name === body.name)) return { code: 400, message: '数据源名称已存在', data: null }
    if (body.name) ds.name = body.name
    if (body.description !== undefined) ds.description = body.description
    if (body.host) ds.host = body.host
    if (body.port) ds.port = body.port
    if (body.database) ds.database = body.database
    if (body.username) ds.username = body.username
    ds.updateTime = formatNow()
    return { code: 200, message: '更新成功', data: ds }
  }},
  { url: '/api/etl/datasource/delete', method: 'post', response: ({ body }: { body: { ids: string | string[] } }) => {
    const idsArray = Array.isArray(body.ids) ? body.ids : [body.ids]
    idsArray.forEach((id) => { const idx = datasources.findIndex((d) => d.id === id); if (idx !== -1) datasources.splice(idx, 1) })
    return { code: 200, message: '删除成功', data: null }
  }},
  { url: '/api/etl/datasource/test', method: 'post', response: () => ({ code: 200, message: '成功', data: { success: true, message: '连接成功', connectionTime: Math.floor(Math.random() * 50) + 10 } }) },
  { url: '/api/etl/datasource/test-query', method: 'post', response: () => ({ code: 200, message: '成功', data: { success: true, data: [{ id: 1, name: '示例数据1' }, { id: 2, name: '示例数据2' }], columns: ['id', 'name'], rowCount: 2 } }) },
]

// ==================== 任务流 API ====================

const pipelineApis: MockMethod[] = [
  { url: '/api/etl/pipeline/list', method: 'get', response: ({ query }: { query: PipelineQueryParams }) => {
    const { pageIndex = 1, pageSize = 10, name, status } = query
    const statusNum = status !== undefined && status !== '' ? Number(status) : undefined
    let filtered = pipelines.filter((p) => {
      let match = true
      if (name && !p.name.toLowerCase().includes(name.toLowerCase())) match = false
      if (statusNum !== undefined && p.status !== statusNum) match = false
      return match
    })
    filtered.sort((a, b) => new Date(b.updateTime || 0).getTime() - new Date(a.updateTime || 0).getTime())
    const total = filtered.length
    const list = filtered.slice((pageIndex - 1) * pageSize, pageIndex * pageSize)
    return { code: 200, message: '成功', data: { list, total, pageIndex, pageSize } }
  }},
  { url: '/api/etl/pipeline/detail', method: 'get', response: ({ query }: { query: { id: string } }) => {
    const p = pipelines.find((p) => p.id === query.id)
    if (!p) return { code: 404, message: '任务流不存在', data: null }
    return { code: 200, message: '成功', data: p }
  }},
  { url: '/api/etl/pipeline/create', method: 'post', response: ({ body }: { body: CreatePipelineParams }) => {
    if (!body.name) return { code: 400, message: '名称不能为空', data: null }
    if (pipelines.some((p) => p.name === body.name)) return { code: 400, message: '名称已存在', data: null }
    const now = formatNow()
    const newPipeline: Pipeline = { id: generateGuid(), name: body.name, description: body.description || '', categoryCode: body.categoryCode, status: 0 as PipelineStatus, currentVersion: 'v0.1.0', dagConfig: { version: '1.0', nodes: [], edges: [] }, creatorId: USER_IDS.admin, creatorName: '管理员', createTime: now, updateTime: now }
    pipelines.push(newPipeline)
    return { code: 200, message: '创建成功', data: newPipeline }
  }},
  { url: '/api/etl/pipeline/update', method: 'post', response: ({ body }: { body: { id: string; name?: string; description?: string; dagConfig?: DagConfig } }) => {
    const idx = pipelines.findIndex((p) => p.id === body.id)
    if (idx === -1) return { code: 404, message: '任务流不存在', data: null }
    const p = pipelines[idx]
    if (body.name) p.name = body.name
    if (body.description !== undefined) p.description = body.description
    if (body.dagConfig) p.dagConfig = body.dagConfig
    p.updateTime = formatNow()
    return { code: 200, message: '更新成功', data: p }
  }},
  { url: '/api/etl/pipeline/delete', method: 'post', response: ({ body }: { body: { ids: string | string[] } }) => {
    const idsArray = Array.isArray(body.ids) ? body.ids : [body.ids]
    idsArray.forEach((id) => { const idx = pipelines.findIndex((p) => p.id === id); if (idx !== -1) pipelines.splice(idx, 1) })
    return { code: 200, message: '删除成功', data: null }
  }},
  { url: '/api/etl/pipeline/publish', method: 'post', response: ({ body }: { body: { id: string } }) => {
    const idx = pipelines.findIndex((p) => p.id === body.id)
    if (idx === -1) return { code: 404, message: '任务流不存在', data: null }
    pipelines[idx].status = 1
    pipelines[idx].currentVersion = generateNewVersion(pipelines[idx].currentVersion)
    pipelines[idx].updateTime = formatNow()
    return { code: 200, message: '发布成功', data: null }
  }},
]

// ==================== 调度 API ====================

const scheduleApis: MockMethod[] = [
  { url: '/api/etl/schedule/list', method: 'get', response: ({ query }: { query: ScheduleQueryParams }) => {
    const { pageIndex = 1, pageSize = 10, name, status } = query
    let filtered = schedules.filter((s) => {
      let match = true
      if (name && !s.name?.toLowerCase().includes(name.toLowerCase()) && !s.pipelineName?.toLowerCase().includes(name.toLowerCase())) match = false
      if (status !== undefined && status !== '' && s.status !== Number(status)) match = false
      return match
    })
    const total = filtered.length
    const list = filtered.slice((pageIndex - 1) * pageSize, pageIndex * pageSize)
    return { code: 200, message: '成功', data: { list, total, pageIndex, pageSize } }
  }},
  { url: '/api/etl/schedule/detail', method: 'get', response: ({ query }: { query: { id: string } }) => {
    const s = schedules.find((s) => s.id === query.id)
    if (!s) return { code: 404, message: '调度不存在', data: null }
    return { code: 200, message: '成功', data: { ...s, scheduleType: 'cron', maxRetries: s.config.maxRetryTimes || 3, retryInterval: s.config.retryInterval || 60, timeout: s.config.timeout || 3600, notifyOnSuccess: false, notifyOnFailure: true, notifyEmails: [] } }
  }},
  { url: '/api/etl/schedule/create', method: 'post', response: ({ body }: { body: CreateScheduleParams }) => {
    const pipeline = pipelines.find((p) => p.id === body.pipelineId)
    const now = formatNow()
    const newSchedule: ScheduleTask = { id: generateGuid(), name: body.name, description: body.description, pipelineId: body.pipelineId, pipelineName: pipeline?.name, cronExpression: body.cronExpression, cronDescription: body.cronExpression === '0 6 * * *' ? '每天早上6点执行' : '自定义调度', config: { type: 'cron' as ScheduleType, cron: { expression: body.cronExpression || '0 0 * * *' }, maxRetryTimes: body.maxRetries, retryInterval: body.retryInterval, timeout: body.timeout }, status: 1 as ScheduleStatus, creatorId: USER_IDS.admin, creatorName: '管理员', createTime: now, updateTime: now }
    schedules.push(newSchedule)
    return { code: 200, message: '创建成功', data: newSchedule }
  }},
  { url: '/api/etl/schedule/update', method: 'post', response: ({ body }: { body: { id: string; name?: string; description?: string; cronExpression?: string; maxRetries?: number; retryInterval?: number; timeout?: number } }) => {
    const idx = schedules.findIndex((s) => s.id === body.id)
    if (idx === -1) return { code: 404, message: '调度不存在', data: null }
    const s = schedules[idx]
    if (body.name) s.name = body.name
    if (body.description !== undefined) s.description = body.description
    if (body.cronExpression) { s.cronExpression = body.cronExpression; s.config.cron = { expression: body.cronExpression } }
    if (body.maxRetries !== undefined) s.config.maxRetryTimes = body.maxRetries
    if (body.retryInterval !== undefined) s.config.retryInterval = body.retryInterval
    if (body.timeout !== undefined) s.config.timeout = body.timeout
    s.updateTime = formatNow()
    return { code: 200, message: '更新成功', data: s }
  }},
  { url: '/api/etl/schedule/delete', method: 'post', response: ({ body }: { body: { ids: string | string[] } }) => {
    const idsArray = Array.isArray(body.ids) ? body.ids : [body.ids]
    idsArray.forEach((id) => { const idx = schedules.findIndex((s) => s.id === id); if (idx !== -1) schedules.splice(idx, 1) })
    return { code: 200, message: '删除成功', data: null }
  }},
  { url: '/api/etl/schedule/enable', method: 'post', response: ({ body }: { body: { id: string } }) => {
    const s = schedules.find((s) => s.id === body.id)
    if (!s) return { code: 404, message: '调度不存在', data: null }
    s.status = 1 as ScheduleStatus
    s.updateTime = formatNow()
    return { code: 200, message: '启用成功', data: s }
  }},
  { url: '/api/etl/schedule/disable', method: 'post', response: ({ body }: { body: { id: string } }) => {
    const s = schedules.find((s) => s.id === body.id)
    if (!s) return { code: 404, message: '调度不存在', data: null }
    s.status = 0 as ScheduleStatus
    s.updateTime = formatNow()
    return { code: 200, message: '禁用成功', data: s }
  }},
  { url: '/api/etl/schedule/execute-now', method: 'post', response: ({ body }: { body: { id: string } }) => ({ code: 200, message: '已触发执行', data: { executionId: generateGuid() } }) },
  { url: '/api/etl/schedule/preview-cron', method: 'post', response: ({ body }: { body: { expression: string } }) => ({ code: 200, message: '成功', data: { expression: body.expression, description: '每天早上6点执行', nextExecutions: ['2024-03-16 06:00:00', '2024-03-17 06:00:00', '2024-03-18 06:00:00'] } }) },
]

// ==================== 执行监控 API ====================

const monitorApis: MockMethod[] = [
  { url: '/api/etl/execution/list', method: 'get', response: ({ query }: { query: ExecutionQueryParams }) => {
    const { pageIndex = 1, pageSize = 10, pipelineId, status } = query
    let filtered = executions.filter((e) => {
      let match = true
      if (pipelineId && e.pipelineId !== pipelineId) match = false
      if (status !== undefined && status !== '' && e.status !== status) match = false
      return match
    })
    filtered.sort((a, b) => new Date(b.createTime).getTime() - new Date(a.createTime).getTime())
    const total = filtered.length
    const list = filtered.slice((pageIndex - 1) * pageSize, pageIndex * pageSize)
    return { code: 200, message: '成功', data: { list, total, pageIndex, pageSize } }
  }},
  { url: '/api/etl/execution/detail', method: 'get', response: ({ query }: { query: { id: string } }) => {
    const e = executions.find((e) => e.id === query.id)
    if (!e) return { code: 404, message: '执行记录不存在', data: null }
    return { code: 200, message: '成功', data: e }
  }},
  { url: '/api/etl/execution/statistics', method: 'get', response: () => ({ code: 200, message: '成功', data: { total: 150, running: 5, success: 120, failure: 15, pending: 10, avgDuration: 450, successRate: 0.85 } }) },
]

export default [...datasourceApis, ...pipelineApis, ...scheduleApis, ...monitorApis] as MockMethod[]