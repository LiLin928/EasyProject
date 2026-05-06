// src/mock/columnTemplate.ts

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'
import type { ColumnConfig, DetectedColumn } from '@/types/columnTemplate'

interface MockColumnTemplate {
  id: string
  name: string
  type: 'single' | 'table'
  description: string
  dataSourceId?: string
  sqlQuery?: string
  columns: ColumnConfig[]
  createTime: string
  updateTime: string
}

function formatNow(): string {
  const now = new Date()
  return `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(now.getDate()).padStart(2, '0')} ${String(now.getHours()).padStart(2, '0')}:${String(now.getMinutes()).padStart(2, '0')}:${String(now.getSeconds()).padStart(2, '0')}`
}

const templates: MockColumnTemplate[] = [
  {
    id: '11111111-aaaa-bbbb-cccc-dddddddddddd',
    name: '区域列',
    type: 'single',
    description: '区域字段列配置，支持钻取',
    dataSourceId: 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee',
    sqlQuery: 'SELECT region FROM sales GROUP BY region',
    columns: [{
      field: 'region',
      label: '区域',
      width: 120,
      align: 'left',
      drilldown: { enabled: true, targetReportId: '22222222-3333-4444-5555-666666666666', params: [{ sourceField: '__clicked_value__', targetParam: 'region' }] }
    }],
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '22222222-bbbb-cccc-dddd-eeeeeeeeeeee',
    name: '金额列',
    type: 'single',
    description: '金额字段列配置，货币格式化',
    dataSourceId: 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee',
    sqlQuery: 'SELECT amount FROM sales',
    columns: [{
      field: 'amount',
      label: '金额',
      width: 120,
      align: 'right',
      format: 'money'
    }],
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '33333333-cccc-dddd-eeee-ffffffffffff',
    name: '销售报表表格模板',
    type: 'table',
    description: '销售报表完整表格配置',
    dataSourceId: 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee',
    sqlQuery: 'SELECT region, SUM(amount) as amount, COUNT(*) as count, AVG(amount) as avg FROM sales GROUP BY region',
    columns: [
      { field: 'region', label: '区域', width: 120, drilldown: { enabled: true, targetReportId: '22222222-3333-4444-5555-666666666666', params: [{ sourceField: '__clicked_value__', targetParam: 'region' }] } },
      { field: 'amount', label: '销售额', width: 100, align: 'right', format: 'money' },
      { field: 'count', label: '订单数', width: 80, align: 'right', format: 'number' },
      { field: 'avg', label: '客单价', width: 100, align: 'right', format: 'money' }
    ],
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '44444444-dddd-eeee-ffff-000000000000',
    name: '日期列',
    type: 'single',
    description: '日期字段列配置',
    columns: [{
      field: 'date',
      label: '日期',
      width: 120,
      format: 'date'
    }],
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
]

/**
 * 模拟根据SQL获取列信息
 */
function mockFetchColumns(sqlQuery: string): DetectedColumn[] {
  // 根据SQL模拟返回不同的列
  const sqlLower = sqlQuery.toLowerCase()

  if (sqlLower.includes('region')) {
    const cols: DetectedColumn[] = [{ field: 'region', type: 'string' }]
    if (sqlLower.includes('amount')) cols.push({ field: 'amount', type: 'number' })
    if (sqlLower.includes('count')) cols.push({ field: 'count', type: 'number' })
    if (sqlLower.includes('avg')) cols.push({ field: 'avg', type: 'number' })
    return cols
  }

  if (sqlLower.includes('amount')) {
    return [{ field: 'amount', type: 'number' }]
  }

  if (sqlLower.includes('date')) {
    const cols: DetectedColumn[] = [{ field: 'date', type: 'string' }]
    if (sqlLower.includes('amount')) cols.push({ field: 'amount', type: 'number' })
    if (sqlLower.includes('orders')) cols.push({ field: 'orders', type: 'number' })
    return cols
  }

  // 默认返回一些通用列
  return [
    { field: 'id', type: 'number' },
    { field: 'name', type: 'string' },
    { field: 'create_time', type: 'datetime' }
  ]
}

export default [
  // 获取模板列表
  {
    url: '/api/column-template/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number; pageSize?: number; name?: string; type?: string; dataSourceId?: string } }) => {
      const { pageIndex = 1, pageSize = 10, name, type, dataSourceId } = query

      let filteredTemplates = templates.filter((t) => {
        let match = true
        if (name && !t.name.toLowerCase().includes(name.toLowerCase())) {
          match = false
        }
        if (type && t.type !== type) {
          match = false
        }
        if (dataSourceId && t.dataSourceId !== dataSourceId) {
          match = false
        }
        return match
      })

      const total = filteredTemplates.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredTemplates.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 获取模板详情
  {
    url: '/api/column-template/detail',
    method: 'get',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query
      const template = templates.find((t) => t.id === id)

      if (!template) {
        return { code: 404, message: '模板不存在', data: null }
      }

      return { code: 200, message: '成功', data: template }
    },
  },

  // 创建模板
  {
    url: '/api/column-template/create',
    method: 'post',
    response: ({ body }: { body: Omit<MockColumnTemplate, 'id' | 'createTime' | 'updateTime'> }) => {
      const { name, type, description, dataSourceId, sqlQuery, columns } = body

      if (!name) {
        return { code: 400, message: '模板名称不能为空', data: null }
      }

      if (templates.some((t) => t.name === name)) {
        return { code: 400, message: '模板名称已存在', data: null }
      }

      const now = formatNow()
      const newTemplate: MockColumnTemplate = {
        id: generateGuid(),
        name,
        type,
        description,
        dataSourceId,
        sqlQuery,
        columns,
        createTime: now,
        updateTime: now,
      }

      templates.push(newTemplate)

      return { code: 200, message: '创建成功', data: { id: newTemplate.id } }
    },
  },

  // 更新模板
  {
    url: '/api/column-template/update',
    method: 'post',
    response: ({ body }: { body: MockColumnTemplate }) => {
      const { id, name, type, description, dataSourceId, sqlQuery, columns } = body

      const templateIndex = templates.findIndex((t) => t.id === id)

      if (templateIndex === -1) {
        return { code: 404, message: '模板不存在', data: null }
      }

      const template = templates[templateIndex]

      if (name && name !== template.name) {
        if (templates.some((t) => t.name === name)) {
          return { code: 400, message: '模板名称已存在', data: null }
        }
        template.name = name
      }

      template.type = type
      template.description = description
      template.dataSourceId = dataSourceId
      template.sqlQuery = sqlQuery
      template.columns = columns
      template.updateTime = formatNow()

      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },

  // 删除模板
  {
    url: '/api/column-template/delete',
    method: 'delete',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query
      const index = templates.findIndex((t) => t.id === id)

      if (index === -1) {
        return { code: 404, message: '模板不存在', data: null }
      }

      templates.splice(index, 1)

      return { code: 200, message: '删除成功', data: { success: true } }
    },
  },

  // 根据数据源和SQL获取列信息
  {
    url: '/api/column-template/fetch-columns',
    method: 'post',
    response: ({ body }: { body: { dataSourceId: string; sqlQuery: string } }) => {
      const { sqlQuery } = body

      if (!sqlQuery) {
        return { code: 400, message: 'SQL查询语句不能为空', data: null }
      }

      // 模拟解析SQL并返回列信息
      const detectedColumns = mockFetchColumns(sqlQuery)

      // 转换为列配置格式
      const columns: ColumnConfig[] = detectedColumns.map(col => ({
        field: col.field,
        label: col.field,
        width: 100,
        align: col.type === 'number' ? 'right' : 'left',
      }))

      return {
        code: 200,
        message: '成功',
        data: {
          detectedColumns,
          columns,
        },
      }
    },
  },
] as MockMethod[]