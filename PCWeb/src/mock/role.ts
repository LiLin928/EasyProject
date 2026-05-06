// 角色管理 Mock

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * Mock 角色类型
 */
interface MockRole {
  id: string // GUID 类型
  roleName: string
  roleCode: string
  description: string
  status: 0 | 1 // 0: 禁用, 1: 启用
  createTime: string
}

// 初始角色数据（使用 GUID）
const roles: MockRole[] = [
  {
    id: '11111111-2222-3333-4444-555555555001',
    roleName: '超级管理员',
    roleCode: 'admin',
    description: '拥有系统所有权限',
    status: 1,
    createTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555002',
    roleName: '管理员',
    roleCode: 'manager',
    description: '拥有基础管理权限',
    status: 1,
    createTime: '2024-01-02 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555003',
    roleName: '普通用户',
    roleCode: 'user',
    description: '拥有工作台权限',
    status: 1,
    createTime: '2024-01-03 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555004',
    roleName: '游客',
    roleCode: 'guest',
    description: '仅查看权限',
    status: 1,
    createTime: '2024-01-04 10:00:00',
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

export default [
  // 角色列表（分页、筛选）
  {
    url: '/api/role/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number; pageSize?: number; roleName?: string; roleCode?: string; status?: string } }) => {
      const { pageIndex = 1, pageSize = 10, roleName, roleCode, status } = query

      // 筛选
      let filteredRoles = roles.filter((role) => {
        let match = true
        if (roleName && !role.roleName.includes(roleName)) {
          match = false
        }
        if (roleCode && !role.roleCode.toLowerCase().includes(roleCode.toLowerCase())) {
          match = false
        }
        if (status !== undefined && status !== '') {
          const statusNum = Number.parseInt(status, 10)
          if (!Number.isNaN(statusNum) && role.status !== statusNum) {
            match = false
          }
        }
        return match
      })

      // 排序（按创建时间倒序）
      filteredRoles = filteredRoles.sort((a, b) =>
        new Date(b.createTime).getTime() - new Date(a.createTime).getTime()
      )

      // 分页
      const total = filteredRoles.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredRoles.slice(start, end)

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

  // 角色详情（路径参数）
  {
    url: /\/api\/role\/detail\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const role = roles.find((r) => r.id === id)

      if (!role) {
        return {
          code: 404,
          message: '角色不存在',
          data: null,
        }
      }

      return {
        code: 200,
        message: '成功',
        data: role,
      }
    },
  },

  // 新增角色
  {
    url: '/api/role/add',
    method: 'post',
    response: ({ body }: { body: Partial<MockRole> }) => {
      const { roleName, roleCode, description, status } = body

      // 检查必填字段
      if (!roleName) {
        return {
          code: 400,
          message: '角色名称不能为空',
          data: null,
        }
      }

      // 检查编码唯一
      if (roleCode && roles.some((r) => r.roleCode === roleCode)) {
        return {
          code: 400,
          message: '角色编码已存在',
          data: null,
        }
      }

      // 创建新角色（使用 GUID）
      const now = formatNow()
      const newRole: MockRole = {
        id: generateGuid(),
        roleName,
        roleCode: roleCode || '',
        description: description || '',
        status: status ?? 1,
        createTime: now,
      }

      roles.push(newRole)

      return {
        code: 200,
        message: '创建成功',
        data: newRole.id,
      }
    },
  },

  // 编辑角色（PUT方法）
  {
    url: '/api/role/update',
    method: 'put',
    response: ({ body }: { body: Partial<MockRole> & { id: string } }) => {
      const { id, roleName, roleCode, description, status } = body

      const roleIndex = roles.findIndex((r) => r.id === id)

      if (roleIndex === -1) {
        return {
          code: 404,
          message: '角色不存在',
          data: null,
        }
      }

      const role = roles[roleIndex]

      // 检查编码唯一（如果修改了编码）
      if (roleCode && roleCode !== role.roleCode) {
        if (roles.some((r) => r.roleCode === roleCode)) {
          return {
            code: 400,
            message: '角色编码已存在',
            data: null,
          }
        }
        role.roleCode = roleCode
      }

      // 更新字段
      if (roleName !== undefined) role.roleName = roleName
      if (description !== undefined) role.description = description
      if (status !== undefined) role.status = status

      roles[roleIndex] = role

      return {
        code: 200,
        message: '更新成功',
        data: 1,
      }
    },
  },

  // 删除角色（单个，DELETE方法）
  {
    url: /\/api\/role\/delete\/[\w-]+/,
    method: 'delete',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()

      // 检查是否包含 admin 角色
      const adminRoleId = '11111111-2222-3333-4444-555555555001'
      if (id === adminRoleId) {
        return {
          code: 400,
          message: '不能删除超级管理员角色',
          data: null,
        }
      }

      const index = roles.findIndex((r) => r.id === id)
      if (index !== -1) {
        roles.splice(index, 1)
      }

      return {
        code: 200,
        message: '删除成功',
        data: 1,
      }
    },
  },

  // 分配用户到角色
  {
    url: '/api/role/assign-users',
    method: 'post',
    response: ({ body }: { body: { roleId: string; userIds: string[] } }) => {
      const { roleId, userIds } = body

      if (!roleId) {
        return {
          code: 400,
          message: '角色ID不能为空',
          data: null,
        }
      }

      const role = roles.find((r) => r.id === roleId)
      if (!role) {
        return {
          code: 404,
          message: '角色不存在',
          data: null,
        }
      }

      return {
        code: 200,
        message: '分配成功',
        data: userIds.length,
      }
    },
  },
] as MockMethod[]