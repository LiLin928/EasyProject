// src/mock/memberLevel.ts

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * Mock 会员等级类型
 */
interface MockMemberLevel {
  id: string
  name: string
  minSpent: number
  discount: number
  pointsRate: number
  icon: string
  sort: number
  status: 0 | 1
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

// 初始会员等级数据
const memberLevels: MockMemberLevel[] = [
  {
    id: 'level-0001-aaaa-bbbb-cccc-0001',
    name: '普通会员',
    minSpent: 0,
    discount: 100,
    pointsRate: 1.0,
    icon: '/static/images/levels/normal.png',
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: 'level-0002-aaaa-bbbb-cccc-0002',
    name: '银卡会员',
    minSpent: 2000,
    discount: 98,
    pointsRate: 1.2,
    icon: '/static/images/levels/silver.png',
    sort: 2,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: 'level-0003-aaaa-bbbb-cccc-0003',
    name: '金卡会员',
    minSpent: 5000,
    discount: 95,
    pointsRate: 1.5,
    icon: '/static/images/levels/gold.png',
    sort: 3,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: 'level-0004-aaaa-bbbb-cccc-0004',
    name: '钻石会员',
    minSpent: 10000,
    discount: 90,
    pointsRate: 2.0,
    icon: '/static/images/levels/diamond.png',
    sort: 4,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
]

export default [
  // ==================== 会员等级 API ====================

  // 获取会员等级列表
  {
    url: '/api/memberlevel/list',
    method: 'get',
    response: ({ query }: { query: { name?: string; status?: number | string } }) => {
      const { name } = query
      const status = query.status !== undefined ? Number(query.status) : undefined

      let filteredLevels = memberLevels.filter((level) => {
        let match = true
        if (name && !level.name.toLowerCase().includes(name.toLowerCase())) {
          match = false
        }
        if (status !== undefined && level.status !== status) {
          match = false
        }
        return match
      })

      // 按排序号升序
      filteredLevels.sort((a, b) => a.sort - b.sort)

      return {
        code: 200,
        message: '成功',
        data: filteredLevels,
      }
    },
  },

  // 获取会员等级详情（路径参数）
  {
    url: /\/api\/memberlevel\/detail\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const level = memberLevels.find((l) => l.id === id)

      if (!level) {
        return { code: 404, message: '会员等级不存在', data: null }
      }

      return {
        code: 200,
        message: '成功',
        data: level,
      }
    },
  },

  // 创建会员等级
  {
    url: '/api/memberlevel/add',
    method: 'post',
    response: ({ body }: { body: Omit<MockMemberLevel, 'id' | 'createTime' | 'updateTime'> }) => {
      const { name, minSpent, discount, pointsRate, icon, sort, status } = body

      if (!name) {
        return { code: 400, message: '等级名称不能为空', data: null }
      }

      if (memberLevels.some((l) => l.name === name)) {
        return { code: 400, message: '等级名称已存在', data: null }
      }

      const now = formatNow()
      const newLevel: MockMemberLevel = {
        id: generateGuid(),
        name,
        minSpent: minSpent ?? 0,
        discount: discount ?? 100,
        pointsRate: pointsRate ?? 1.0,
        icon: icon || '/static/images/levels/default.png',
        sort: sort ?? memberLevels.length + 1,
        status: status ?? 1,
        createTime: now,
        updateTime: now,
      }

      memberLevels.push(newLevel)

      return { code: 200, message: '创建成功', data: { id: newLevel.id } }
    },
  },

  // 更新会员等级
  {
    url: '/api/memberlevel/update',
    method: 'put',
    response: ({ body }: { body: Partial<MockMemberLevel> & { id: string } }) => {
      const { id, name, minSpent, discount, pointsRate, icon, sort, status } = body

      const levelIndex = memberLevels.findIndex((l) => l.id === id)

      if (levelIndex === -1) {
        return { code: 404, message: '会员等级不存在', data: null }
      }

      const level = memberLevels[levelIndex]

      if (name && name !== level.name) {
        if (memberLevels.some((l) => l.name === name)) {
          return { code: 400, message: '等级名称已存在', data: null }
        }
        level.name = name
      }

      if (minSpent !== undefined) level.minSpent = minSpent
      if (discount !== undefined) level.discount = discount
      if (pointsRate !== undefined) level.pointsRate = pointsRate
      if (icon !== undefined) level.icon = icon
      if (sort !== undefined) level.sort = sort
      if (status !== undefined) level.status = status
      level.updateTime = formatNow()

      memberLevels[levelIndex] = level

      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },

  // 删除会员等级（DELETE方法，路径参数）
  {
    url: /\/api\/memberlevel\/delete\/[\w-]+/,
    method: 'delete',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()

      const index = memberLevels.findIndex((l) => l.id === id)
      if (index !== -1) {
        memberLevels.splice(index, 1)
      }

      return { code: 200, message: '删除成功', data: 1 }
    },
  },
] as MockMethod[]