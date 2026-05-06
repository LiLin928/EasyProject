// 字典管理 Mock

import type { MockMethod } from 'vite-plugin-mock'

/**
 * Mock 字典类型
 */
interface MockDictType {
  id: string
  code: string
  name: string
  description: string
  status: 0 | 1 // 0: 禁用, 1: 启用
  createTime: string
  updateTime: string | null
}

/**
 * Mock 字典数据
 */
interface MockDictData {
  id: string
  typeCode: string
  label: string
  value: string
  sort: number
  status: 0 | 1 // 0: 禁用, 1: 启用
  createTime: string
  updateTime: string | null
}

// 初始字典类型数据
const dictTypes: MockDictType[] = [
  {
    id: '10000000-0000-0000-0000-000000000001',
    code: 'status',
    name: '状态',
    description: '通用状态字典',
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '10000000-0000-0000-0000-000000000002',
    code: 'gender',
    name: '性别',
    description: '性别字典',
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '10000000-0000-0000-0000-000000000003',
    code: 'userType',
    name: '用户类型',
    description: '用户类型字典',
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '10000000-0000-0000-0000-000000000004',
    code: 'workflow_category',
    name: '流程分类',
    description: '工作流流程分类',
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
]

// 初始字典数据
const dictDataList: MockDictData[] = [
  {
    id: '20000000-0000-0000-0000-000000000001',
    typeCode: 'status',
    label: '启用',
    value: '1',
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000002',
    typeCode: 'status',
    label: '禁用',
    value: '0',
    sort: 2,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000003',
    typeCode: 'gender',
    label: '男',
    value: 'male',
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000004',
    typeCode: 'gender',
    label: '女',
    value: 'female',
    sort: 2,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000005',
    typeCode: 'gender',
    label: '未知',
    value: 'unknown',
    sort: 3,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000006',
    typeCode: 'userType',
    label: '管理员',
    value: 'admin',
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000007',
    typeCode: 'userType',
    label: '普通用户',
    value: 'user',
    sort: 2,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000008',
    typeCode: 'userType',
    label: '游客',
    value: 'guest',
    sort: 3,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  // 流程分类
  {
    id: '20000000-0000-0000-0000-000000000009',
    typeCode: 'workflow_category',
    label: '请假管理',
    value: 'leave',
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000010',
    typeCode: 'workflow_category',
    label: '费用管理',
    value: 'expense',
    sort: 2,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000011',
    typeCode: 'workflow_category',
    label: '合同管理',
    value: 'contract',
    sort: 3,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000012',
    typeCode: 'workflow_category',
    label: '人事管理',
    value: 'hr',
    sort: 4,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000013',
    typeCode: 'workflow_category',
    label: '行政管理',
    value: 'admin',
    sort: 5,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
  {
    id: '20000000-0000-0000-0000-000000000014',
    typeCode: 'workflow_category',
    label: '采购管理',
    value: 'purchase',
    sort: 6,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: null,
  },
]

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

/**
 * 生成 GUID
 */
function generateGuid(): string {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
    const r = (Math.random() * 16) | 0
    const v = c === 'x' ? r : (r & 0x3) | 0x8
    return v.toString(16)
  })
}

export default [
  // ==================== 字典类型接口 ====================

  // 字典类型列表（分页）
  {
    url: '/api/dict/type/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number; pageSize?: number; code?: string; name?: string; status?: string } }) => {
      const { pageIndex = 1, pageSize = 10, code, name, status } = query

      // 筛选
      let filteredList = dictTypes.filter((item) => {
        let match = true
        if (code && !item.code.toLowerCase().includes(code.toLowerCase())) {
          match = false
        }
        if (name && !item.name.includes(name)) {
          match = false
        }
        if (status !== undefined && status !== '') {
          const statusNum = Number.parseInt(status, 10)
          if (!Number.isNaN(statusNum) && item.status !== statusNum) {
            match = false
          }
        }
        return match
      })

      // 排序（按创建时间倒序）
      filteredList = filteredList.sort(
        (a, b) => new Date(b.createTime).getTime() - new Date(a.createTime).getTime()
      )

      // 分页
      const total = filteredList.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredList.slice(start, end)

      return {
        code: 200,
        message: '操作成功',
        data: {
          list,
          total,
          pageIndex,
          pageSize,
        },
      }
    },
  },

  // 获取所有字典类型（不分页）
  {
    url: '/api/dict/type/all',
    method: 'get',
    response: () => {
      const list = dictTypes
        .filter((item) => item.status === 1)
        .sort((a, b) => a.code.localeCompare(b.code))

      return {
        code: 200,
        message: '操作成功',
        data: list,
      }
    },
  },

  // 字典类型详情
  {
    url: '/api/dict/type/detail/:id',
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const dictType = dictTypes.find((item) => item.id === id)

      if (!dictType) {
        return {
          code: 404,
          message: '字典类型不存在',
          data: null,
        }
      }

      return {
        code: 200,
        message: '操作成功',
        data: dictType,
      }
    },
  },

  // 新增字典类型
  {
    url: '/api/dict/type/add',
    method: 'post',
    response: ({ body }: { body: { code: string; name: string; description?: string; status?: number } }) => {
      const { code, name, description, status } = body

      // 检查必填字段
      if (!code || !name) {
        return {
          code: 400,
          message: '编码和名称不能为空',
          data: null,
        }
      }

      // 检查编码唯一
      if (dictTypes.some((item) => item.code === code)) {
        return {
          code: 400,
          message: '字典编码已存在',
          data: null,
        }
      }

      // 创建新字典类型
      const now = formatNow()
      const newDictType: MockDictType = {
        id: generateGuid(),
        code,
        name,
        description: description || '',
        status: (status as 0 | 1) ?? 1,
        createTime: now,
        updateTime: null,
      }

      dictTypes.push(newDictType)

      return {
        code: 200,
        message: '添加成功',
        data: newDictType.id,
      }
    },
  },

  // 更新字典类型
  {
    url: '/api/dict/type/update',
    method: 'put',
    response: ({ body }: { body: { id: string; name?: string; description?: string; status?: number } }) => {
      const { id, name, description, status } = body

      const index = dictTypes.findIndex((item) => item.id === id)

      if (index === -1) {
        return {
          code: 404,
          message: '字典类型不存在',
          data: 0,
        }
      }

      const dictType = dictTypes[index]

      // 更新字段
      if (name !== undefined) dictType.name = name
      if (description !== undefined) dictType.description = description
      if (status !== undefined) dictType.status = status as 0 | 1
      dictType.updateTime = formatNow()

      dictTypes[index] = dictType

      return {
        code: 200,
        message: '更新成功',
        data: 1,
      }
    },
  },

  // 删除字典类型
  {
    url: '/api/dict/type/delete/:id',
    method: 'delete',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const index = dictTypes.findIndex((item) => item.id === id)

      if (index === -1) {
        return {
          code: 200,
          message: '删除成功',
          data: 0,
        }
      }

      const code = dictTypes[index].code
      // 删除关联的字典数据
      for (let i = dictDataList.length - 1; i >= 0; i--) {
        if (dictDataList[i].typeCode === code) {
          dictDataList.splice(i, 1)
        }
      }
      // 删除字典类型
      dictTypes.splice(index, 1)

      return {
        code: 200,
        message: '删除成功',
        data: 1,
      }
    },
  },

  // ==================== 字典数据接口 ====================

  // 字典数据列表（分页）
  {
    url: '/api/dict/data/list',
    method: 'get',
    response: ({
      query,
    }: {
      query: { pageIndex?: number; pageSize?: number; typeCode: string; status?: string }
    }) => {
      const { pageIndex = 1, pageSize = 10, typeCode, status } = query

      // 检查必填参数
      if (!typeCode) {
        return {
          code: 400,
          message: '字典类型编码不能为空',
          data: null,
        }
      }

      // 筛选
      let filteredList = dictDataList.filter((item) => {
        let match = item.typeCode === typeCode
        if (status !== undefined && status !== '') {
          const statusNum = Number.parseInt(status, 10)
          if (!Number.isNaN(statusNum) && item.status !== statusNum) {
            match = false
          }
        }
        return match
      })

      // 排序（按 sort 升序）
      filteredList = filteredList.sort((a, b) => a.sort - b.sort)

      // 分页
      const total = filteredList.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredList.slice(start, end)

      return {
        code: 200,
        message: '操作成功',
        data: {
          list,
          total,
          pageIndex,
          pageSize,
        },
      }
    },
  },

  // 根据字典类型编码获取字典数据列表（用于下拉选项）
  {
    url: '/api/dict/data/by-code/:code',
    method: 'get',
    response: (config: { url: string }) => {
      const code = config.url.split('/').pop()

      const list = dictDataList
        .filter((item) => item.typeCode === code && item.status === 1)
        .sort((a, b) => a.sort - b.sort)

      return {
        code: 200,
        message: '操作成功',
        data: list,
      }
    },
  },

  // 字典数据详情
  {
    url: '/api/dict/data/detail/:id',
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const dictData = dictDataList.find((item) => item.id === id)

      if (!dictData) {
        return {
          code: 404,
          message: '字典数据不存在',
          data: null,
        }
      }

      return {
        code: 200,
        message: '操作成功',
        data: dictData,
      }
    },
  },

  // 新增字典数据
  {
    url: '/api/dict/data/add',
    method: 'post',
    response: ({
      body,
    }: {
      body: { typeCode: string; label: string; value: string; sort?: number; status?: number }
    }) => {
      const { typeCode, label, value, sort, status } = body

      // 检查必填字段
      if (!typeCode || !label || !value) {
        return {
          code: 400,
          message: '字典类型编码、标签和值不能为空',
          data: null,
        }
      }

      // 检查字典类型是否存在
      if (!dictTypes.some((item) => item.code === typeCode)) {
        return {
          code: 400,
          message: '字典类型不存在',
          data: null,
        }
      }

      // 创建新字典数据
      const now = formatNow()
      const newDictData: MockDictData = {
        id: generateGuid(),
        typeCode,
        label,
        value,
        sort: sort ?? 0,
        status: (status as 0 | 1) ?? 1,
        createTime: now,
        updateTime: null,
      }

      dictDataList.push(newDictData)

      return {
        code: 200,
        message: '添加成功',
        data: newDictData.id,
      }
    },
  },

  // 更新字典数据
  {
    url: '/api/dict/data/update',
    method: 'put',
    response: ({
      body,
    }: {
      body: { id: string; label?: string; value?: string; sort?: number; status?: number }
    }) => {
      const { id, label, value, sort, status } = body

      const index = dictDataList.findIndex((item) => item.id === id)

      if (index === -1) {
        return {
          code: 404,
          message: '字典数据不存在',
          data: 0,
        }
      }

      const dictData = dictDataList[index]

      // 更新字段
      if (label !== undefined) dictData.label = label
      if (value !== undefined) dictData.value = value
      if (sort !== undefined) dictData.sort = sort
      if (status !== undefined) dictData.status = status as 0 | 1
      dictData.updateTime = formatNow()

      dictDataList[index] = dictData

      return {
        code: 200,
        message: '更新成功',
        data: 1,
      }
    },
  },

  // 删除字典数据
  {
    url: '/api/dict/data/delete/:id',
    method: 'delete',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const index = dictDataList.findIndex((item) => item.id === id)

      if (index !== -1) {
        dictDataList.splice(index, 1)
      }

      return {
        code: 200,
        message: '删除成功',
        data: 1,
      }
    },
  },
] as MockMethod[]