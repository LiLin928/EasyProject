// 大屏 Mock 数据

import type { MockMethod } from 'vite-plugin-mock'
import type { ScreenConfig, ScreenComponent, ScreenStyle, ScreenPermissions, DataSource, ScreenPublishRecord } from '@/types/screen'

/**
 * Mock 大屏类型（基于 ScreenConfig）
 */
type MockScreen = ScreenConfig

// ID 自增计数器
let screenIdCounter = 3

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

// 默认背景样式
const defaultBackground = 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)'

// 初始大屏数据
const screens: MockScreen[] = [
  {
    id: 1,
    name: '销售监控大屏',
    description: '实时监控销售数据，包含销售额、订单量、客户分布等关键指标',
    thumbnail: '',
    style: {
      background: defaultBackground,
      width: 1920,
      height: 1080,
    },
    components: [
      {
        id: 'line-chart-1',
        type: 'line-chart',
        position: { x: 50, y: 100 },
        size: { width: 600, height: 400 },
        dataSource: {
          type: 'static',
          data: [
            { date: '2024-01', value: 150000 },
            { date: '2024-02', value: 180000 },
            { date: '2024-03', value: 165000 },
            { date: '2024-04', value: 210000 },
            { date: '2024-05', value: 195000 },
            { date: '2024-06', value: 240000 },
          ],
        },
        config: {
          title: '销售趋势',
          xAxisField: 'date',
          yAxisField: 'value',
          smooth: true,
          showArea: true,
        },
      },
      {
        id: 'pie-chart-1',
        type: 'pie-chart',
        position: { x: 700, y: 100 },
        size: { width: 400, height: 400 },
        dataSource: {
          type: 'static',
          data: [
            { name: '电子产品', value: 45 },
            { name: '服装鞋帽', value: 28 },
            { name: '食品饮料', value: 18 },
            { name: '家居用品', value: 9 },
          ],
        },
        config: {
          title: '销售品类占比',
          nameField: 'name',
          valueField: 'value',
          showLegend: true,
        },
      },
      {
        id: 'number-card-1',
        type: 'number-card',
        position: { x: 50, y: 520 },
        size: { width: 300, height: 150 },
        dataSource: {
          type: 'static',
          data: [{ value: 1240567 }],
        },
        config: {
          title: '总销售额',
          unit: '元',
          prefix: '¥',
          showTrend: true,
          trendValue: 12.5,
        },
      },
      {
        id: 'number-card-2',
        type: 'number-card',
        position: { x: 380, y: 520 },
        size: { width: 300, height: 150 },
        dataSource: {
          type: 'static',
          data: [{ value: 8934 }],
        },
        config: {
          title: '订单数量',
          unit: '单',
          showTrend: true,
          trendValue: 8.3,
        },
      },
      {
        id: 'number-card-3',
        type: 'number-card',
        position: { x: 710, y: 520 },
        size: { width: 300, height: 150 },
        dataSource: {
          type: 'static',
          data: [{ value: 4562 }],
        },
        config: {
          title: '新增客户',
          unit: '人',
          showTrend: true,
          trendValue: -2.1,
        },
      },
    ],
    permissions: {
      sharedUsers: [],
      sharedRoles: [],
    },
    isPublic: 1,    // 1-公开，0-私有
    createdBy: 1,
    createTime: '2024-01-15 10:00:00',
    updateTime: '2024-01-15 10:00:00',
  },
  {
    id: 2,
    name: '生产监控大屏',
    description: '生产车间实时监控，展示产量、良品率、设备状态等数据',
    thumbnail: '',
    style: {
      background: defaultBackground,
      width: 1920,
      height: 1080,
    },
    components: [
      {
        id: 'bar-chart-1',
        type: 'bar-chart',
        position: { x: 50, y: 100 },
        size: { width: 600, height: 400 },
        dataSource: {
          type: 'static',
          data: [
            { name: '产线A', value: 5200 },
            { name: '产线B', value: 4800 },
            { name: '产线C', value: 6100 },
            { name: '产线D', value: 4300 },
            { name: '产线E', value: 5500 },
          ],
        },
        config: {
          title: '各产线产量',
          xAxisField: 'name',
          yAxisField: 'value',
          showLabel: true,
        },
      },
      {
        id: 'data-table-1',
        type: 'data-table',
        position: { x: 700, y: 100 },
        size: { width: 500, height: 400 },
        dataSource: {
          type: 'static',
          data: [
            { line: '产线A', output: 5200, rate: 98.5, status: '运行中' },
            { line: '产线B', output: 4800, rate: 97.2, status: '运行中' },
            { line: '产线C', output: 6100, rate: 99.1, status: '运行中' },
            { line: '产线D', output: 4300, rate: 96.8, status: '维护中' },
            { line: '产线E', output: 5500, rate: 98.9, status: '运行中' },
          ],
        },
        config: {
          title: '产线状态',
          columns: [
            { field: 'line', title: '产线名称', width: 100 },
            { field: 'output', title: '产量', width: 100 },
            { field: 'rate', title: '良品率(%)', width: 100 },
            { field: 'status', title: '状态', width: 100 },
          ],
        },
      },
      {
        id: 'number-card-4',
        type: 'number-card',
        position: { x: 50, y: 520 },
        size: { width: 280, height: 150 },
        dataSource: {
          type: 'static',
          data: [{ value: 25900 }],
        },
        config: {
          title: '今日总产量',
          unit: '件',
          showTrend: true,
          trendValue: 15.2,
        },
      },
      {
        id: 'number-card-5',
        type: 'number-card',
        position: { x: 360, y: 520 },
        size: { width: 280, height: 150 },
        dataSource: {
          type: 'static',
          data: [{ value: 98.1 }],
        },
        config: {
          title: '平均良品率',
          unit: '%',
          precision: 1,
          showTrend: true,
          trendValue: 0.5,
        },
      },
    ],
    permissions: {
      sharedUsers: [],
      sharedRoles: [],
    },
    isPublic: 0,    // 1-公开，0-私有
    createdBy: 2,
    createTime: '2024-01-14 09:30:00',
    updateTime: '2024-01-14 09:30:00',
  },
  {
    id: 3,
    name: '运维监控大屏',
    description: 'IT系统运维监控，展示服务器状态、网络流量、告警信息等',
    thumbnail: '',
    style: {
      background: defaultBackground,
      width: 1920,
      height: 1080,
    },
    components: [
      {
        id: 'line-chart-2',
        type: 'line-chart',
        position: { x: 50, y: 100 },
        size: { width: 800, height: 350 },
        dataSource: {
          type: 'static',
          data: [
            { time: '00:00', cpu: 45, memory: 62 },
            { time: '04:00', cpu: 38, memory: 58 },
            { time: '08:00', cpu: 72, memory: 75 },
            { time: '12:00', cpu: 85, memory: 82 },
            { time: '16:00', cpu: 78, memory: 79 },
            { time: '20:00', cpu: 55, memory: 68 },
          ],
        },
        config: {
          title: '系统资源使用率',
          xAxisField: 'time',
          series: [
            { field: 'cpu', name: 'CPU使用率', color: '#5470c6' },
            { field: 'memory', name: '内存使用率', color: '#91cc75' },
          ],
          smooth: true,
          showLegend: true,
        },
      },
      {
        id: 'line-chart-3',
        type: 'line-chart',
        position: { x: 50, y: 480 },
        size: { width: 800, height: 350 },
        dataSource: {
          type: 'static',
          data: [
            { time: '00:00', in: 120, out: 85 },
            { time: '04:00', in: 95, out: 62 },
            { time: '08:00', in: 280, out: 195 },
            { time: '12:00', in: 350, out: 245 },
            { time: '16:00', in: 320, out: 210 },
            { time: '20:00', in: 180, out: 125 },
          ],
        },
        config: {
          title: '网络流量(MB/s)',
          xAxisField: 'time',
          series: [
            { field: 'in', name: '入站流量', color: '#73c0de' },
            { field: 'out', name: '出站流量', color: '#fc8452' },
          ],
          smooth: true,
          showLegend: true,
        },
      },
    ],
    permissions: {
      sharedUsers: [],
      sharedRoles: [],
    },
    isPublic: 1,    // 1-公开，0-私有
    createdBy: 1,
    createTime: '2024-01-13 14:20:00',
    updateTime: '2024-01-13 14:20:00',
  },
]

// 发布信息存储（用于大屏列表显示发布状态）
const publishMap: Map<number, { publishId: string; publishUrl: string; publishedAt: string }> = new Map()

// 发布记录存储（独立于大屏数据，存储完整快照）
const publishRecords: Map<string, ScreenPublishRecord> = new Map()

// 生成发布ID
function generatePublishId(): string {
  return 'pub_' + Math.random().toString(36).substring(2, 10) + Date.now().toString(36)
}

export default [
  // 获取大屏列表
  {
    url: '/api/screen/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number; pageSize?: number; name?: string; isPublic?: string } }) => {
      const { pageIndex = 1, pageSize = 10, name, isPublic } = query

      // 筛选
      let filteredScreens = screens.filter((screen) => {
        let match = true
        if (name && !screen.name.toLowerCase().includes(name.toLowerCase())) {
          match = false
        }
        if (isPublic !== undefined && isPublic !== '' && isPublic !== undefined) {
          const isPublicBool = isPublic === 'true'
          if (screen.isPublic !== (isPublicBool ? 1 : 0)) {
            match = false
          }
        }
        return match
      })

      // 默认按更新时间倒序
      filteredScreens.sort((a, b) =>
        new Date(b.updateTime).getTime() - new Date(a.updateTime).getTime()
      )

      // 分页
      const total = filteredScreens.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredScreens.slice(start, end)

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

  // 获取大屏详情
  {
    url: '/api/screen/detail',
    method: 'get',
    response: ({ query }: { query: { id: string | number } }) => {
      const id = Number(query.id)
      const screen = screens.find((s) => s.id === id)

      if (!screen) {
        return {
          code: 404,
          message: '大屏不存在',
          data: null,
        }
      }

      return {
        code: 200,
        message: '成功',
        data: screen,
      }
    },
  },

  // 创建大屏
  {
    url: '/api/screen/create',
    method: 'post',
    response: (config: any) => {
      const body = config.body || {}
      const { name, description, style, permissions, isPublic } = body

      // 检查必填字段
      if (!name) {
        return {
          code: 400,
          message: '大屏名称不能为空',
          data: null,
        }
      }

      // 检查名称唯一
      if (screens.some((s) => s.name === name)) {
        return {
          code: 400,
          message: '大屏名称已存在',
          data: null,
        }
      }

      // 解析 JSON 字符串
      let styleObj = { background: defaultBackground, width: 1920, height: 1080 }
      if (style) {
        try {
          styleObj = JSON.parse(style)
        } catch {
          // 使用默认值
        }
      }

      let permissionsObj = { sharedUsers: [], sharedRoles: [] }
      if (permissions) {
        try {
          permissionsObj = JSON.parse(permissions)
        } catch {
          // 使用默认值
        }
      }

      // 创建新大屏
      screenIdCounter++
      const now = formatNow()
      const newScreen: MockScreen = {
        id: screenIdCounter,
        name,
        description: description || '',
        thumbnail: '',
        style: styleObj,
        components: [],
        permissions: permissionsObj,
        isPublic: isPublic || 0,
        createdBy: 1, // Mock 当前用户ID
        createTime: now,
        updateTime: now,
      }

      screens.push(newScreen)

      return {
        code: 200,
        message: '创建成功',
        data: { id: newScreen.id },
      }
    },
  },

  // 更新大屏
  {
    url: '/api/screen/update',
    method: 'post',
    response: (config: any) => {
      const body = config.body || {}
      const { id, name, description, style, components, permissions } = body

      const screenIndex = screens.findIndex((s) => s.id === id)

      if (screenIndex === -1) {
        return {
          code: 404,
          message: '大屏不存在',
          data: null,
        }
      }

      const screen = screens[screenIndex]

      // 检查名称唯一（如果修改了名称）
      if (name && name !== screen.name) {
        if (screens.some((s) => s.name === name)) {
          return {
            code: 400,
            message: '大屏名称已存在',
            data: null,
          }
        }
        screen.name = name
      }

      // 更新字段
      if (description !== undefined) screen.description = description
      if (style !== undefined) {
        screen.style = {
          background: style.background || screen.style.background,
          width: style.width || screen.style.width,
          height: style.height || screen.style.height,
        }
      }
      if (components !== undefined) screen.components = components
      if (permissions !== undefined) screen.permissions = permissions
      screen.updateTime = formatNow()

      screens[screenIndex] = screen

      return {
        code: 200,
        message: '更新成功',
        data: { success: true },
      }
    },
  },

  // 删除大屏
  {
    url: '/api/screen/delete',
    method: 'post',
    response: (config: any) => {
      const body = config.body || {}
      const { ids } = body
      const idsArray = Array.isArray(ids) ? ids : [ids]

      // 删除大屏
      idsArray.forEach((id) => {
        const index = screens.findIndex((s) => s.id === id)
        if (index !== -1) {
          screens.splice(index, 1)
        }
      })

      return {
        code: 200,
        message: '删除成功',
        data: null,
      }
    },
  },

  // 复制大屏
  {
    url: '/api/screen/copy',
    method: 'post',
    response: (config: any) => {
      const body = config.body || {}
      const { id } = body

      const screen = screens.find((s) => s.id === id)

      if (!screen) {
        return {
          code: 404,
          message: '大屏不存在',
          data: null,
        }
      }

      // 创建副本
      screenIdCounter++
      const now = formatNow()
      const newScreen: MockScreen = {
        ...screen,
        id: screenIdCounter,
        name: `${screen.name} - 副本`,
        createTime: now,
        updateTime: now,
      }

      // 深拷贝组件
      newScreen.components = screen.components.map((comp) => ({
        ...comp,
        id: `${comp.id}-copy`,
      }))

      screens.push(newScreen)

      return {
        code: 200,
        message: '复制成功',
        data: { id: newScreen.id },
      }
    },
  },

  // 更新分享配置
  {
    url: '/api/screen/share',
    method: 'post',
    response: (config: any) => {
      const body = config.body || {}
      const { id, permissions } = body

      const screenIndex = screens.findIndex((s) => s.id === id)

      if (screenIndex === -1) {
        return { code: 404, message: '大屏不存在', data: null }
      }

      screens[screenIndex].isPublic = permissions.isPublic ? 1 : 0
      screens[screenIndex].permissions = {
        sharedUsers: permissions.sharedUsers || [],
        sharedRoles: permissions.sharedRoles || [],
      }
      screens[screenIndex].updateTime = formatNow()

      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },

  // 获取可分享的用户列表
  {
    url: '/api/screen/shared-users',
    method: 'get',
    response: () => {
      // Mock 用户列表
      const users = [
        { id: 1, name: '管理员', avatar: '' },
        { id: 2, name: '张三', avatar: '' },
        { id: 3, name: '李四', avatar: '' },
        { id: 4, name: '王五', avatar: '' },
      ]

      return { code: 200, message: '成功', data: { list: users } }
    },
  },

  // 获取可分享的角色列表
  {
    url: '/api/screen/shared-roles',
    method: 'get',
    response: () => {
      // Mock 角色列表
      const roles = [
        { id: 1, name: '管理员' },
        { id: 2, name: '编辑' },
        { id: 3, name: '查看者' },
      ]

      return { code: 200, message: '成功', data: { list: roles } }
    },
  },

  // ===================== 数据源相关接口 =====================

  // 获取可用的数据源列表
  {
    url: '/api/screen/datasources',
    method: 'get',
    response: () => {
      // Mock 数据源列表
      const datasources = [
        { id: 1, name: '生产数据库', type: 'mysql', status: 1 },
        { id: 2, name: '报表数据库', type: 'postgresql', status: 1 },
        { id: 3, name: '历史数据库', type: 'oracle', status: 1 },
        { id: 4, name: '日志数据库', type: 'clickhouse', status: 0 },
      ]

      return { code: 200, message: '成功', data: { list: datasources } }
    },
  },

  // 执行SQL查询
  {
    url: '/api/screen/execute-sql',
    method: 'post',
    response: (config: any) => {
      const body = config.body || {}
      const { datasourceId, sql } = body

      // 验证数据源是否存在
      if (![1, 2, 3, 4].includes(datasourceId)) {
        return { code: 404, message: '数据源不存在', data: null }
      }

      // Mock 根据SQL返回不同的数据
      let mockData: any[] = []
      let columns: { name: string; type: string }[] = []

      const sqlLower = sql.toLowerCase()

      if (sqlLower.includes('sales') || sqlLower.includes('销售')) {
        mockData = [
          { month: '1月', value: 1200, growth: 15.2 },
          { month: '2月', value: 1500, growth: 25.0 },
          { month: '3月', value: 1800, growth: 20.0 },
          { month: '4月', value: 2100, growth: 16.7 },
          { month: '5月', value: 2500, growth: 19.0 },
          { month: '6月', value: 2800, growth: 12.0 },
        ]
        columns = [
          { name: 'month', type: 'string' },
          { name: 'value', type: 'number' },
          { name: 'growth', type: 'number' },
        ]
      } else if (sqlLower.includes('category') || sqlLower.includes('分类') || sqlLower.includes('产品')) {
        mockData = [
          { name: '电子产品', value: 4500, percent: 35 },
          { name: '服装鞋帽', value: 2800, percent: 22 },
          { name: '食品饮料', value: 2200, percent: 17 },
          { name: '家居用品', value: 1800, percent: 14 },
          { name: '其他', value: 1500, percent: 12 },
        ]
        columns = [
          { name: 'name', type: 'string' },
          { name: 'value', type: 'number' },
          { name: 'percent', type: 'number' },
        ]
      } else if (sqlLower.includes('region') || sqlLower.includes('区域') || sqlLower.includes('地区')) {
        mockData = [
          { region: '华东', sales: 5200, orders: 1250 },
          { region: '华南', sales: 3800, orders: 890 },
          { region: '华北', sales: 3200, orders: 720 },
          { region: '华中', sales: 2600, orders: 580 },
          { region: '西南', sales: 1800, orders: 420 },
        ]
        columns = [
          { name: 'region', type: 'string' },
          { name: 'sales', type: 'number' },
          { name: 'orders', type: 'number' },
        ]
      } else {
        // 默认返回模拟数据
        mockData = [
          { id: 1, name: '数据1', value: 100 },
          { id: 2, name: '数据2', value: 200 },
          { id: 3, name: '数据3', value: 300 },
        ]
        columns = [
          { name: 'id', type: 'number' },
          { name: 'name', type: 'string' },
          { name: 'value', type: 'number' },
        ]
      }

      return {
        code: 200,
        message: '成功',
        data: { data: mockData, columns },
      }
    },
  },

  // 验证SQL语法
  {
    url: '/api/screen/validate-sql',
    method: 'post',
    response: (config: any) => {
      const body = config.body || {}
      const { sql } = body

      // 简单验证：检查是否包含SELECT
      const sqlLower = sql.toLowerCase().trim()

      if (!sqlLower.startsWith('select')) {
        return {
          code: 200,
          message: '成功',
          data: { valid: false, message: 'SQL语句必须以SELECT开头' },
        }
      }

      // 检查是否包含危险的SQL操作
      const dangerousKeywords = ['insert', 'update', 'delete', 'drop', 'truncate', 'alter', 'create']
      for (const keyword of dangerousKeywords) {
        if (sqlLower.includes(keyword)) {
          return {
            code: 200,
            message: '成功',
            data: { valid: false, message: `SQL语句不能包含${keyword.toUpperCase()}操作` },
          }
        }
      }

      // 模拟返回列信息
      const columns = [
        { name: 'id', type: 'number' },
        { name: 'name', type: 'string' },
        { name: 'value', type: 'number' },
      ]

      return {
        code: 200,
        message: '成功',
        data: { valid: true, columns },
      }
    },
  },

  // ===================== 发布相关接口 =====================

  // 发布大屏
  {
    url: '/api/screen/publish',
    method: 'post',
    response: (config: any) => {
      const body = config.body || {}
      const id = Number(body.id)

      const screen = screens.find((s) => s.id === id)

      if (!screen) {
        return { code: 404, message: '大屏不存在', data: null }
      }

      // 检查是否已发布过，如果已发布则更新
      let existingRecord: ScreenPublishRecord | undefined
      for (const record of publishRecords.values()) {
        if (record.screenId === id && record.status === 1) {
          existingRecord = record
          break
        }
      }

      let publishId: string
      let publishUrl: string
      const publishedAt = formatNow()

      // 获取 origin（Mock 环境下使用默认值，因为 vite-plugin-mock 在 Node.js 环境运行）
      const origin = 'http://localhost:3005'

      if (existingRecord) {
        // 更新已存在的发布记录
        publishId = existingRecord.publishId
        publishUrl = existingRecord.publishUrl
        existingRecord.screenData = JSON.parse(JSON.stringify(screen))
        existingRecord.screenName = screen.name
        existingRecord.screenDescription = screen.description
        existingRecord.publishedAt = publishedAt
      } else {
        // 创建新的发布记录
        publishId = generatePublishId()
        publishUrl = `${origin}/#/screen/publish/${publishId}`

        const publishRecord: ScreenPublishRecord = {
          publishId,
          screenId: screen.id,
          screenName: screen.name,
          screenDescription: screen.description,
          screenData: JSON.parse(JSON.stringify(screen)), // 深拷贝完整快照
          publishUrl,
          publishedAt,
          publishedBy: 1,
          viewCount: 0,
          status: 1,
        }

        publishRecords.set(publishId, publishRecord)
      }

      // 更新大屏的发布状态
      publishMap.set(id, { publishId, publishUrl, publishedAt })

      return {
        code: 200,
        message: existingRecord ? '更新发布成功' : '发布成功',
        data: { publishId, publishUrl },
      }
    },
  },

  // 获取发布后的大屏数据
  {
    url: '/api/screen/published',
    method: 'get',
    response: ({ query }: { query: { publishId: string } }) => {
      const { publishId } = query

      if (!publishId) {
        return { code: 400, message: '缺少发布ID', data: null }
      }

      const record = publishRecords.get(publishId)

      if (!record || record.status !== 1) {
        return { code: 404, message: '发布的大屏不存在或已下架', data: null }
      }

      // 增加访问计数
      record.viewCount++

      return { code: 200, message: '成功', data: record.screenData }
    },
  },

  // 获取发布列表
  {
    url: '/api/screen/publish-list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number; pageSize?: number; name?: string; status?: number } }) => {
      const { pageIndex = 1, pageSize = 12, name, status } = query

      // 获取所有发布记录
      let records = Array.from(publishRecords.values())

      // 按状态筛选
      if (status !== undefined && status !== null) {
        records = records.filter(r => r.status === status)
      }

      // 按名称筛选
      if (name) {
        records = records.filter(r => r.screenName.toLowerCase().includes(name.toLowerCase()))
      }

      // 按发布时间倒序
      records.sort((a, b) => new Date(b.publishedAt).getTime() - new Date(a.publishedAt).getTime())

      // 分页
      const total = records.length
      const start = (pageIndex - 1) * pageSize
      const list = records.slice(start, start + pageSize)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 取消发布/下架发布
  {
    url: '/api/screen/unpublish',
    method: 'post',
    response: (config: any) => {
      const body = config.body || {}
      const { id, publishId } = body

      // 支持通过大屏ID或发布ID下架
      if (publishId) {
        const record = publishRecords.get(publishId)
        if (!record) {
          return { code: 404, message: '发布记录不存在', data: null }
        }
        record.status = 0
        // 同时更新发布状态
        publishMap.delete(record.screenId)
        return { code: 200, message: '下架成功', data: { success: true } }
      }

      if (id) {
        const info = publishMap.get(Number(id))
        if (!info) {
          return { code: 404, message: '该大屏未发布', data: null }
        }
        // 更新发布记录状态
        const record = publishRecords.get(info.publishId)
        if (record) {
          record.status = 0
        }
        publishMap.delete(Number(id))
        return { code: 200, message: '下架成功', data: { success: true } }
      }

      return { code: 400, message: '缺少参数', data: null }
    },
  },

  // 上架发布
  {
    url: '/api/screen/republish',
    method: 'post',
    response: (config: any) => {
      const body = config.body || {}
      const { publishId } = body

      if (!publishId) {
        return { code: 400, message: '缺少发布ID', data: null }
      }

      const record = publishRecords.get(publishId)
      if (!record) {
        return { code: 404, message: '发布记录不存在', data: null }
      }

      record.status = 1
      // 恢复发布状态
      publishMap.set(record.screenId, {
        publishId: record.publishId,
        publishUrl: record.publishUrl,
        publishedAt: record.publishedAt,
      })

      return { code: 200, message: '上架成功', data: { success: true } }
    },
  },

  // 获取大屏发布信息
  {
    url: '/api/screen/publish-info',
    method: 'get',
    response: ({ query }: { query: { id: string | number } }) => {
      const id = Number(query.id)
      const info = publishMap.get(id)

      if (!info) {
        return {
          code: 200,
          message: '成功',
          data: { published: false },
        }
      }

      const record = publishRecords.get(info.publishId)

      return {
        code: 200,
        message: '成功',
        data: {
          published: true,
          publishId: info.publishId,
          publishUrl: info.publishUrl,
          publishedAt: info.publishedAt,
          viewCount: record?.viewCount || 0,
        },
      }
    },
  },
] as MockMethod[]