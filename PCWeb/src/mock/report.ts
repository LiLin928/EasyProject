// src/mock/report.ts

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * Mock 列配置类型
 */
interface MockColumnConfig {
  field: string
  label: string
  width?: number
  align?: 'left' | 'center' | 'right'
  format?: 'number' | 'money' | 'percent' | 'date'
  drilldown?: {
    enabled: boolean
    targetReportId: string
    params: { sourceField: string; targetParam: string }[]
  }
}

/**
 * Mock 检测到的列类型
 */
interface MockDetectedColumn {
  field: string
  type: 'string' | 'number' | 'date' | 'boolean'
}

/**
 * Mock 报表类型
 */
interface MockReport {
  id: string
  name: string
  category: string
  creator: string
  createTime: string
  updateTime: string
  dataSource: string
  sqlQuery: string
  // 显示配置
  showChart: boolean
  showTable: boolean
  chartType: 'bar' | 'line' | 'pie'
  xAxisField: string
  yAxisField: string
  aggregation: 'sum' | 'count' | 'avg'
  // 列配置相关
  autoColumns: boolean
  columnTemplateId: string | null
  columnConfigs: MockColumnConfig[]
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

// 报表分类数据
const categories = [
  { id: 'a1b2c3d4-e5f6-4a5b-8c9d-0e1f2a3b4c5d', name: '销售报表', icon: 'DataAnalysis' },
  { id: 'b2c3d4e5-f6a7-4b5c-9d0e-1f2a3b4c5d6e', name: '库存报表', icon: 'Box' },
  { id: 'c3d4e5f6-a7b8-4c5d-0e1f-2a3b4c5d6e7f', name: '财务报表', icon: 'Money' },
  { id: 'd4e5f6a7-b8c9-4d5e-1f2a-3b4c5d6e7f8a', name: '分析报表', icon: 'TrendCharts' },
]

// 数据源数据
const dataSources = [
  { id: 'e5f6a7b8-c9d0-4e5f-2a3b-4c5d6e7f8a9b', name: 'SalesDB', type: 'MySQL' },
  { id: 'f6a7b8c9-d0e1-4f5a-3b4c-5d6e7f8a9b0c', name: 'InventoryDB', type: 'MySQL' },
  { id: 'a7b8c9d0-e1f2-4a5b-4c5d-6e7f8a9b0c1d', name: 'FinanceDB', type: 'PostgreSQL' },
  { id: 'b8c9d0e1-f2a3-4b5c-5d6e-7f8a9b0c1d2e', name: 'AnalyticsDB', type: 'ClickHouse' },
]

// 初始报表数据
const reports: MockReport[] = [
  {
    id: '11111111-2222-3333-4444-555555555555',
    name: '销售日报',
    category: '销售报表',
    creator: '张三',
    createTime: '2024-01-15 10:00:00',
    updateTime: '2024-01-15 10:00:00',
    dataSource: 'SalesDB',
    sqlQuery: 'SELECT date, SUM(amount) as total FROM sales WHERE date = CURDATE() GROUP BY date',
    showChart: true,
    showTable: true,
    chartType: 'bar',
    xAxisField: 'date',
    yAxisField: 'total',
    aggregation: 'sum',
    autoColumns: true,
    columnTemplateId: null,
    columnConfigs: [
      { field: 'date', label: '日期', width: 120 },
      { field: 'total', label: '销售总额', width: 120, align: 'right', format: 'money' },
    ],
  },
  {
    id: '22222222-3333-4444-5555-666666666666',
    name: '库存统计',
    category: '库存报表',
    creator: '李四',
    createTime: '2024-01-14 10:00:00',
    updateTime: '2024-01-14 10:00:00',
    dataSource: 'InventoryDB',
    sqlQuery: 'SELECT product_name, quantity FROM inventory ORDER BY quantity DESC LIMIT 10',
    showChart: false,
    showTable: true,
    chartType: 'bar',
    xAxisField: 'product_name',
    yAxisField: 'quantity',
    aggregation: 'sum',
    autoColumns: true,
    columnTemplateId: null,
    columnConfigs: [
      { field: 'product_name', label: '产品名称', width: 200 },
      { field: 'quantity', label: '库存数量', width: 120, align: 'right', format: 'number' },
    ],
  },
  {
    id: '33333333-4444-5555-6666-777777777777',
    name: '财务月报',
    category: '财务报表',
    creator: '王五',
    createTime: '2024-01-13 10:00:00',
    updateTime: '2024-01-13 10:00:00',
    dataSource: 'FinanceDB',
    sqlQuery: 'SELECT category, SUM(amount) as total FROM finance WHERE MONTH(date) = MONTH(CURDATE()) GROUP BY category',
    showChart: true,
    showTable: true,
    chartType: 'pie',
    xAxisField: 'category',
    yAxisField: 'total',
    aggregation: 'sum',
    autoColumns: true,
    columnTemplateId: null,
    columnConfigs: [
      { field: 'category', label: '分类', width: 150 },
      { field: 'total', label: '金额', width: 120, align: 'right', format: 'money' },
    ],
  },
  {
    id: '44444444-5555-6666-7777-888888888888',
    name: '销售趋势分析',
    category: '分析报表',
    creator: '张三',
    createTime: '2024-01-12 10:00:00',
    updateTime: '2024-01-12 10:00:00',
    dataSource: 'SalesDB',
    sqlQuery: 'SELECT date, SUM(amount) as total FROM sales WHERE date >= DATE_SUB(CURDATE(), INTERVAL 30 DAY) GROUP BY date ORDER BY date',
    showChart: true,
    showTable: false,
    chartType: 'line',
    xAxisField: 'date',
    yAxisField: 'total',
    aggregation: 'sum',
    autoColumns: true,
    columnTemplateId: null,
    columnConfigs: [
      { field: 'date', label: '日期', width: 120 },
      { field: 'total', label: '销售额', width: 120, align: 'right', format: 'money' },
    ],
  },
  {
    id: '55555555-6666-7777-8888-999999999999',
    name: '产品销量排行',
    category: '销售报表',
    creator: '李四',
    createTime: '2024-01-11 10:00:00',
    updateTime: '2024-01-11 10:00:00',
    dataSource: 'SalesDB',
    sqlQuery: 'SELECT product_name, COUNT(*) as sales_count FROM sales GROUP BY product_name ORDER BY sales_count DESC LIMIT 20',
    showChart: true,
    showTable: true,
    chartType: 'bar',
    xAxisField: 'product_name',
    yAxisField: 'sales_count',
    aggregation: 'count',
    autoColumns: true,
    columnTemplateId: null,
    columnConfigs: [
      { field: 'product_name', label: '产品名称', width: 180 },
      { field: 'sales_count', label: '销量', width: 100, align: 'right', format: 'number' },
    ],
  },
]

/**
 * 生成模拟图表数据
 */
function generateChartData(chartType: string): { name: string; value: number }[] {
  const data: { name: string; value: number }[] = []
  const dateCategories = ['2024-01-01', '2024-01-02', '2024-01-03', '2024-01-04', '2024-01-05']

  if (chartType === 'pie') {
    return [
      { name: '电子产品', value: 45000 },
      { name: '服装', value: 32000 },
      { name: '食品', value: 28000 },
      { name: '家居', value: 20000 },
    ]
  }

  dateCategories.forEach(cat => {
    data.push({
      name: cat,
      value: Math.floor(Math.random() * 50000) + 10000,
    })
  })

  return data
}

/**
 * 生成模拟表格数据
 */
function generateTableData(): Record<string, any>[] {
  return [
    { date: '2024-01-15', amount: 32000, orders: 320, avg: 100 },
    { date: '2024-01-14', amount: 28500, orders: 285, avg: 100 },
    { date: '2024-01-13', amount: 35000, orders: 350, avg: 100 },
    { date: '2024-01-12', amount: 29800, orders: 298, avg: 100 },
    { date: '2024-01-11', amount: 31200, orders: 312, avg: 100 },
  ]
}

export default [
  // 获取报表列表
  {
    url: '/api/report/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number; pageSize?: number; name?: string; category?: string } }) => {
      const { pageIndex = 1, pageSize = 10, name, category } = query

      let filteredReports = reports.filter((report) => {
        let match = true
        if (name && !report.name.toLowerCase().includes(name.toLowerCase())) {
          match = false
        }
        if (category && report.category !== category) {
          match = false
        }
        return match
      })

      // 默认按更新时间倒序
      filteredReports.sort((a, b) =>
        new Date(b.updateTime).getTime() - new Date(a.updateTime).getTime()
      )

      const total = filteredReports.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredReports.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 获取报表详情
  {
    url: '/api/report/detail',
    method: 'get',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query
      const report = reports.find((r) => r.id === id)

      if (!report) {
        return { code: 404, message: '报表不存在', data: null }
      }

      return { code: 200, message: '成功', data: report }
    },
  },

  // 创建报表
  {
    url: '/api/report/create',
    method: 'post',
    response: ({ body }: { body: Omit<MockReport, 'id' | 'createTime' | 'updateTime' | 'creator'> }) => {
      const { name, category, dataSource, sqlQuery, showChart, showTable, chartType, xAxisField, yAxisField, aggregation, autoColumns, columnTemplateId, columnConfigs } = body

      if (!name) {
        return { code: 400, message: '报表名称不能为空', data: null }
      }

      if (reports.some((r) => r.name === name)) {
        return { code: 400, message: '报表名称已存在', data: null }
      }

      const now = formatNow()
      const newReport: MockReport = {
        id: generateGuid(),
        name,
        category,
        creator: '张三',
        createTime: now,
        updateTime: now,
        dataSource,
        sqlQuery,
        showChart: showChart ?? true,
        showTable: showTable ?? true,
        chartType,
        xAxisField,
        yAxisField,
        aggregation,
        autoColumns: autoColumns ?? true,
        columnTemplateId: columnTemplateId ?? null,
        columnConfigs: columnConfigs ?? [],
      }

      reports.push(newReport)

      return { code: 200, message: '创建成功', data: { id: newReport.id } }
    },
  },

  // 更新报表
  {
    url: '/api/report/update',
    method: 'post',
    response: ({ body }: { body: MockReport }) => {
      const { id, name, category, dataSource, sqlQuery, showChart, showTable, chartType, xAxisField, yAxisField, aggregation, autoColumns, columnTemplateId, columnConfigs } = body

      const reportIndex = reports.findIndex((r) => r.id === id)

      if (reportIndex === -1) {
        return { code: 404, message: '报表不存在', data: null }
      }

      const report = reports[reportIndex]

      if (name && name !== report.name) {
        if (reports.some((r) => r.name === name)) {
          return { code: 400, message: '报表名称已存在', data: null }
        }
        report.name = name
      }

      if (category !== undefined) report.category = category
      if (dataSource !== undefined) report.dataSource = dataSource
      if (sqlQuery !== undefined) report.sqlQuery = sqlQuery
      if (showChart !== undefined) report.showChart = showChart
      if (showTable !== undefined) report.showTable = showTable
      if (chartType !== undefined) report.chartType = chartType
      if (xAxisField !== undefined) report.xAxisField = xAxisField
      if (yAxisField !== undefined) report.yAxisField = yAxisField
      if (aggregation !== undefined) report.aggregation = aggregation
      if (autoColumns !== undefined) report.autoColumns = autoColumns
      if (columnTemplateId !== undefined) report.columnTemplateId = columnTemplateId
      if (columnConfigs !== undefined) report.columnConfigs = columnConfigs
      report.updateTime = formatNow()

      reports[reportIndex] = report

      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },

  // 删除报表
  {
    url: '/api/report/delete',
    method: 'post',
    response: ({ body }: { body: { ids: string[] } }) => {
      const { ids } = body

      ids.forEach((id) => {
        const index = reports.findIndex((r) => r.id === id)
        if (index !== -1) {
          reports.splice(index, 1)
        }
      })

      return { code: 200, message: '删除成功', data: { success: true } }
    },
  },

  // 获取报表分类
  {
    url: '/api/report/categories',
    method: 'get',
    response: () => {
      return { code: 200, message: '成功', data: categories }
    },
  },

  // 获取数据源列表
  {
    url: '/api/report/datasources',
    method: 'get',
    response: () => {
      return { code: 200, message: '成功', data: dataSources }
    },
  },

  // 预览报表
  {
    url: '/api/report/preview',
    method: 'post',
    response: ({ body }: { body: { chartType: string } }) => {
      const { chartType } = body

      const chartData = generateChartData(chartType)
      const tableData = generateTableData()

      const total = chartData.reduce((sum, item) => sum + item.value, 0)
      const count = chartData.length
      const avg = Math.floor(total / count)

      // 检测到的列信息（模拟 SQL 分析结果）
      const detectedColumns: MockDetectedColumn[] = [
        { field: 'date', type: 'string' },
        { field: 'amount', type: 'number' },
        { field: 'orders', type: 'number' },
        { field: 'avg', type: 'number' },
      ]

      return {
        code: 200,
        message: '成功',
        data: {
          chartData,
          tableData,
          summary: { total, count, avg },
          detectedColumns,
        },
      }
    },
  },
] as MockMethod[]