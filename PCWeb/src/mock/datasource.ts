// src/mock/datasource.ts

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * Mock 数据源类型
 */
interface MockDataSource {
  id: string
  name: string
  dbType: string
  host: string
  port: number
  database: string
  username: string
  password: string
  status: 0 | 1
  lastCheckTime: string
  reportCount: number
  createTime: string
  updateTime: string
}

/**
 * 格式化当前时间
 */
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

// SqlSugar 支持的数据库类型
const dbTypes = [
  { value: 'MySql', label: 'MySQL', defaultPort: 3306 },
  { value: 'SqlServer', label: 'SQL Server', defaultPort: 1433 },
  { value: 'PostgreSQL', label: 'PostgreSQL', defaultPort: 5432 },
  { value: 'Sqlite', label: 'SQLite', defaultPort: 0 },
  { value: 'Oracle', label: 'Oracle', defaultPort: 1521 },
  { value: 'ClickHouse', label: 'ClickHouse', defaultPort: 8123 },
  { value: 'Dm', label: '达梦', defaultPort: 5236 },
  { value: 'Kd', label: '人大金仓', defaultPort: 54321 },
  { value: 'Shentong', label: '神通', defaultPort: 2003 },
  { value: 'Hg', label: '瀚高', defaultPort: 5866 },
  { value: 'GaussDB', label: '华为GaussDB', defaultPort: 8000 },
  { value: 'Firebird', label: 'Firebird', defaultPort: 3050 },
  { value: 'DB2', label: 'DB2', defaultPort: 50000 },
  { value: 'Informix', label: 'Informix', defaultPort: 9088 },
  { value: 'Oscar', label: '神舟通用', defaultPort: 2003 },
  { value: 'QuestDB', label: 'QuestDB', defaultPort: 8812 },
]

// 初始数据源数据
const dataSources: MockDataSource[] = [
  {
    id: 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee',
    name: '销售数据库',
    dbType: 'MySql',
    host: '192.168.1.100',
    port: 3306,
    database: 'sales_db',
    username: 'sales_user',
    password: '******',
    status: 1,
    lastCheckTime: '2024-01-15 10:00:00',
    reportCount: 3,
    createTime: '2024-01-10 10:00:00',
    updateTime: '2024-01-15 10:00:00',
  },
  {
    id: 'bbbbbbbb-cccc-dddd-eeee-ffffffffffff',
    name: '库存数据库',
    dbType: 'PostgreSQL',
    host: '192.168.1.101',
    port: 5432,
    database: 'inventory_db',
    username: 'inventory_user',
    password: '******',
    status: 1,
    lastCheckTime: '2024-01-15 09:30:00',
    reportCount: 1,
    createTime: '2024-01-11 10:00:00',
    updateTime: '2024-01-15 09:30:00',
  },
  {
    id: 'cccccccc-dddd-eeee-ffff-000000000000',
    name: '财务数据库',
    dbType: 'SqlServer',
    host: '192.168.1.102',
    port: 1433,
    database: 'finance_db',
    username: 'finance_user',
    password: '******',
    status: 0,
    lastCheckTime: '2024-01-14 16:00:00',
    reportCount: 1,
    createTime: '2024-01-12 10:00:00',
    updateTime: '2024-01-14 16:00:00',
  },
  {
    id: 'dddddddd-eeee-ffff-0000-111111111111',
    name: '分析数据库',
    dbType: 'ClickHouse',
    host: '192.168.1.103',
    port: 8123,
    database: 'analytics_db',
    username: 'analytics_user',
    password: '******',
    status: 1,
    lastCheckTime: '2024-01-15 08:00:00',
    reportCount: 1,
    createTime: '2024-01-13 10:00:00',
    updateTime: '2024-01-15 08:00:00',
  },
]

export default [
  // 获取数据源列表
  {
    url: '/api/datasource/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number; pageSize?: number; name?: string; dbType?: string } }) => {
      const { pageIndex = 1, pageSize = 10, name, dbType } = query

      let filteredDataSources = dataSources.filter((ds) => {
        let match = true
        if (name && !ds.name.toLowerCase().includes(name.toLowerCase())) {
          match = false
        }
        if (dbType && ds.dbType !== dbType) {
          match = false
        }
        return match
      })

      const total = filteredDataSources.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredDataSources.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 获取数据源详情
  {
    url: '/api/datasource/detail',
    method: 'get',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query
      const ds = dataSources.find((d) => d.id === id)

      if (!ds) {
        return { code: 404, message: '数据源不存在', data: null }
      }

      return { code: 200, message: '成功', data: ds }
    },
  },

  // 创建数据源
  {
    url: '/api/datasource/create',
    method: 'post',
    response: ({ body }: { body: Omit<MockDataSource, 'id' | 'status' | 'lastCheckTime' | 'reportCount' | 'createTime' | 'updateTime'> }) => {
      const { name, dbType, host, port, database, username, password } = body

      if (!name) {
        return { code: 400, message: '数据源名称不能为空', data: null }
      }

      if (dataSources.some((d) => d.name === name)) {
        return { code: 400, message: '数据源名称已存在', data: null }
      }

      const now = formatNow()
      const newDataSource: MockDataSource = {
        id: generateGuid(),
        name,
        dbType,
        host,
        port,
        database,
        username,
        password: '******',
        status: 1,
        lastCheckTime: now,
        reportCount: 0,
        createTime: now,
        updateTime: now,
      }

      dataSources.push(newDataSource)

      return { code: 200, message: '创建成功', data: { id: newDataSource.id } }
    },
  },

  // 更新数据源
  {
    url: '/api/datasource/update',
    method: 'post',
    response: ({ body }: { body: MockDataSource }) => {
      const { id, name, dbType, host, port, database, username, password } = body

      const dsIndex = dataSources.findIndex((d) => d.id === id)

      if (dsIndex === -1) {
        return { code: 404, message: '数据源不存在', data: null }
      }

      const ds = dataSources[dsIndex]

      if (name && name !== ds.name) {
        if (dataSources.some((d) => d.name === name)) {
          return { code: 400, message: '数据源名称已存在', data: null }
        }
        ds.name = name
      }

      if (dbType !== undefined) ds.dbType = dbType
      if (host !== undefined) ds.host = host
      if (port !== undefined) ds.port = port
      if (database !== undefined) ds.database = database
      if (username !== undefined) ds.username = username
      if (password !== undefined) ds.password = '******'
      ds.updateTime = formatNow()

      dataSources[dsIndex] = ds

      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },

  // 删除数据源
  {
    url: '/api/datasource/delete',
    method: 'delete',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query
      const ds = dataSources.find((d) => d.id === id)

      if (!ds) {
        return { code: 404, message: '数据源不存在', data: null }
      }

      if (ds.reportCount > 0) {
        return { code: 400, message: `该数据源被 ${ds.reportCount} 个报表使用，无法删除`, data: null }
      }

      const index = dataSources.findIndex((d) => d.id === id)
      if (index !== -1) {
        dataSources.splice(index, 1)
      }

      return { code: 200, message: '删除成功', data: { success: true } }
    },
  },

  // 获取数据库类型列表
  {
    url: '/api/datasource/dbtypes',
    method: 'get',
    response: () => {
      return { code: 200, message: '成功', data: dbTypes }
    },
  },

  // 测试连接
  {
    url: '/api/datasource/test',
    method: 'post',
    response: ({ body }: { body: { dbType: string; host: string; port: number; database: string; username: string; password: string } }) => {
      // 模拟测试连接（随机成功或失败）
      const success = Math.random() > 0.2

      if (success) {
        return {
          code: 200,
          message: '成功',
          data: {
            success: true,
            message: '连接成功',
          },
        }
      } else {
        return {
          code: 200,
          message: '成功',
          data: {
            success: false,
            message: '连接失败：无法访问数据库服务器',
          },
        }
      }
    },
  },

  // 批量测试所有连接
  {
    url: '/api/datasource/testAll',
    method: 'post',
    response: () => {
      const results = dataSources.map((ds) => {
        const success = Math.random() > 0.2
        ds.status = success ? 1 : 0
        ds.lastCheckTime = formatNow()
        return {
          id: ds.id,
          name: ds.name,
          success,
          message: success ? '连接成功' : '连接失败',
        }
      })

      return {
        code: 200,
        message: '成功',
        data: { results },
      }
    },
  },
] as MockMethod[]