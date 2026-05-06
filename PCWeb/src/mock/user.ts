// 用户信息 Mock

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * Mock 用户类型
 */
interface MockUser {
  id: string // GUID 类型
  userName: string
  realName: string
  password: string
  email: string
  phone: string
  avatar: string
  status: 0 | 1 // 0: 禁用, 1: 启用
  roles: string[]
  roleIds: string[]
  createTime: string
  updateTime: string
}

// 初始用户数据（使用 GUID）
const users: MockUser[] = [
  {
    id: '11111111-2222-3333-4444-555555555551',
    userName: 'admin',
    realName: '管理员',
    password: '123456',
    email: 'admin@example.com',
    phone: '13800138000',
    avatar: 'https://cube.elemecdn.com/0/88/03b0d39583f48206768a7534e55bcpng.png',
    status: 1,
    roles: ['admin'],
    roleIds: ['11111111-2222-3333-4444-555555555556'],
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555552',
    userName: 'zhangsan',
    realName: '张三',
    password: '123456',
    email: 'zhangsan@example.com',
    phone: '13800138001',
    avatar: '',
    status: 1,
    roles: ['user'],
    roleIds: ['11111111-2222-3333-4444-555555555557'],
    createTime: '2024-01-02 10:00:00',
    updateTime: '2024-01-02 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555553',
    userName: 'lisi',
    realName: '李四',
    password: '123456',
    email: 'lisi@example.com',
    phone: '13800138002',
    avatar: '',
    status: 1,
    roles: ['user'],
    createTime: '2024-01-03 10:00:00',
    updateTime: '2024-01-03 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555554',
    userName: 'wangwu',
    realName: '王五',
    password: '123456',
    email: 'wangwu@example.com',
    phone: '13800138003',
    avatar: '',
    status: 0,
    roles: ['user'],
    createTime: '2024-01-04 10:00:00',
    updateTime: '2024-01-04 10:00:00',
  },
  {
    id: '11111111-2222-3333-4444-555555555555',
    userName: 'zhaoliu',
    realName: '赵六',
    password: '123456',
    email: 'zhaoliu@example.com',
    phone: '13800138004',
    avatar: '',
    status: 1,
    roles: ['user'],
    createTime: '2024-01-05 10:00:00',
    updateTime: '2024-01-05 10:00:00',
  },
]

/**
 * 排除密码字段
 */
function excludePassword(user: MockUser): Omit<MockUser, 'password'> {
  const { password: _, ...rest } = user
  return rest
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

export default [
  // 获取用户信息
  {
    url: '/api/user/info',
    method: 'get',
    response: () => {
      return {
        code: 200,
        message: '成功',
        data: {
          id: '11111111-2222-3333-4444-555555555551',
          userName: 'admin',
          realName: '管理员',
          avatar: 'https://cube.elemecdn.com/0/88/03b0d39583f48206768a7534e55bcpng.png',
          email: 'admin@example.com',
          phone: '13800138000',
          roles: ['admin'],
          roleIds: ['11111111-2222-3333-4444-555555555556'],
          permissions: ['*'],
          status: 1,
        },
      }
    },
  },

  // 用户列表（分页、筛选、排序）
  {
    url: '/api/user/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number; pageSize?: number; userName?: string; realName?: string; status?: string; sortField?: string; sortOrder?: string } }) => {
      const { pageIndex = 1, pageSize = 10, userName, realName, status, sortField, sortOrder } = query

      // 筛选
      let filteredUsers = users.filter((user) => {
        let match = true
        if (userName && !user.userName.toLowerCase().includes(userName.toLowerCase())) {
          match = false
        }
        if (realName && !user.realName.includes(realName)) {
          match = false
        }
        if (status !== undefined && status !== '' && status !== undefined) {
          const statusNum = Number.parseInt(status, 10)
          if (!Number.isNaN(statusNum) && user.status !== statusNum) {
            match = false
          }
        }
        return match
      })

      // 排序
      if (sortField && sortOrder) {
        filteredUsers.sort((a: any, b: any) => {
          const aVal = a[sortField]
          const bVal = b[sortField]
          const modifier = sortOrder === 'ascending' ? 1 : -1

          if (aVal === bVal) return 0
          if (aVal === null || aVal === undefined) return 1
          if (bVal === null || bVal === undefined) return -1

          // 字符串比较
          if (typeof aVal === 'string') {
            return aVal.localeCompare(bVal) * modifier
          }
          // 数字比较
          return (aVal - bVal) * modifier
        })
      } else {
        // 默认按创建时间倒序
        filteredUsers.sort((a, b) =>
          new Date(b.createTime).getTime() - new Date(a.createTime).getTime()
        )
      }

      // 分页
      const total = filteredUsers.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredUsers.slice(start, end).map(excludePassword)

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

  // 用户详情（路径参数）
  {
    url: /\/api\/user\/detail\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const user = users.find((u) => u.id === id)

      if (!user) {
        return {
          code: 404,
          message: '用户不存在',
          data: null,
        }
      }

      return {
        code: 200,
        message: '成功',
        data: excludePassword(user),
      }
    },
  },

  // 新增用户
  {
    url: '/api/user/add',
    method: 'post',
    response: ({ body }: { body: Partial<MockUser> }) => {
      const { userName, realName, password, email, phone, avatar, status, roles } = body

      // 检查必填字段
      if (!userName || !realName) {
        return {
          code: 400,
          message: '用户名和昵称不能为空',
          data: null,
        }
      }

      // 检查用户名唯一
      if (users.some((u) => u.userName === userName)) {
        return {
          code: 400,
          message: '用户名已存在',
          data: null,
        }
      }

      // 创建新用户（使用 GUID）
      const now = formatNow()
      const newUser: MockUser = {
        id: generateGuid(),
        userName,
        realName,
        password: password || '123456',
        email: email || '',
        phone: phone || '',
        avatar: avatar || '',
        status: status ?? 1,
        roles: roles || ['user'],
        createTime: now,
        updateTime: now,
      }

      users.push(newUser)

      return {
        code: 200,
        message: '创建成功',
        data: excludePassword(newUser),
      }
    },
  },

  // 编辑用户（PUT方法）
  {
    url: '/api/user/update',
    method: 'put',
    response: ({ body }: { body: Partial<MockUser> & { id: string } }) => {
      const { id, userName, realName, email, phone, avatar, status, roles } = body

      const userIndex = users.findIndex((u) => u.id === id)

      if (userIndex === -1) {
        return {
          code: 404,
          message: '用户不存在',
          data: null,
        }
      }

      const user = users[userIndex]

      // 检查用户名唯一（如果修改了用户名）
      if (userName && userName !== user.userName) {
        if (users.some((u) => u.userName === userName)) {
          return {
            code: 400,
            message: '用户名已存在',
            data: null,
          }
        }
        user.userName = userName
      }

      // 更新字段
      if (realName !== undefined) user.realName = realName
      if (email !== undefined) user.email = email
      if (phone !== undefined) user.phone = phone
      if (avatar !== undefined) user.avatar = avatar
      if (status !== undefined) user.status = status
      if (roles !== undefined) user.roles = roles
      user.updateTime = formatNow()

      users[userIndex] = user

      return {
        code: 200,
        message: '更新成功',
        data: excludePassword(user),
      }
    },
  },

  // 删除用户（单个，DELETE方法）
  {
    url: /\/api\/user\/delete\/[\w-]+/,
    method: 'delete',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()

      // 检查是否包含 admin（GUID）
      if (id === '11111111-2222-3333-4444-555555555551') {
        return {
          code: 400,
          message: '不能删除管理员账号',
          data: null,
        }
      }

      const index = users.findIndex((u) => u.id === id)
      if (index !== -1) {
        users.splice(index, 1)
      }

      return {
        code: 200,
        message: '删除成功',
        data: 1,
      }
    },
  },

  // 重置密码
  {
    url: '/api/user/reset-password',
    method: 'post',
    response: ({ body }: { body: { id: string; password?: string } }) => {
      const { id, password = '123456' } = body

      const userIndex = users.findIndex((u) => u.id === id)

      if (userIndex === -1) {
        return {
          code: 404,
          message: '用户不存在',
          data: null,
        }
      }

      users[userIndex].password = password
      users[userIndex].updateTime = formatNow()

      return {
        code: 200,
        message: '密码重置成功',
        data: null,
      }
    },
  },

  // 更新状态（PUT方法）
  {
    url: '/api/user/update-status',
    method: 'put',
    response: ({ body }: { body: { id: string; status: 0 | 1 } }) => {
      const { id, status } = body

      // 不能禁用 admin（GUID）
      if (id === '11111111-2222-3333-4444-555555555551' && status === 0) {
        return {
          code: 400,
          message: '不能禁用管理员账号',
          data: null,
        }
      }

      const userIndex = users.findIndex((u) => u.id === id)

      if (userIndex === -1) {
        return {
          code: 404,
          message: '用户不存在',
          data: null,
        }
      }

      users[userIndex].status = status
      users[userIndex].updateTime = formatNow()

      return {
        code: 200,
        message: status === 1 ? '启用成功' : '禁用成功',
        data: null,
      }
    },
  },

  // 更新用户资料（PUT方法）
  {
    url: '/api/user/update-profile',
    method: 'put',
    response: ({ body }: { body: { realName?: string; email?: string; phone?: string } }) => {
      const { realName, email, phone } = body

      const userIndex = users.findIndex((u) => u.id === '11111111-2222-3333-4444-555555555551')

      if (userIndex === -1) {
        return {
          code: 404,
          message: '用户不存在',
          data: null,
        }
      }

      const user = users[userIndex]
      if (realName !== undefined) user.realName = realName
      if (email !== undefined) user.email = email
      if (phone !== undefined) user.phone = phone
      user.updateTime = formatNow()

      return {
        code: 200,
        message: '更新成功',
        data: excludePassword(user),
      }
    },
  },

  // 修改密码
  {
    url: '/api/user/change-password',
    method: 'post',
    response: ({ body }: { body: { oldPassword: string; newPassword: string; confirmPassword: string } }) => {
      const { oldPassword, newPassword, confirmPassword } = body

      const user = users.find((u) => u.id === '11111111-2222-3333-4444-555555555551')

      if (!user) {
        return {
          code: 404,
          message: '用户不存在',
          data: null,
        }
      }

      // 验证旧密码
      if (user.password !== oldPassword) {
        return {
          code: 400,
          message: '旧密码错误',
          data: null,
        }
      }

      // 验证新密码一致性
      if (newPassword !== confirmPassword) {
        return {
          code: 400,
          message: '两次密码输入不一致',
          data: null,
        }
      }

      // 更新密码
      user.password = newPassword
      user.updateTime = formatNow()

      return {
        code: 200,
        message: '密码修改成功',
        data: { success: true, message: '密码修改成功' },
      }
    },
  },

  // 上传头像（模拟，返回随机头像 URL）
  {
    url: '/api/user/upload-avatar',
    method: 'post',
    response: () => {
      // 模拟上传，返回随机头像
      const avatarUrls = [
        'https://cube.elemecdn.com/0/88/03b0d39583f48206768a7534e55bcpng.png',
        'https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png',
        'https://cube.elemecdn.com/9/c2/f0ee8a3c7c9638a54940382568c9dpng.png',
      ]
      const randomAvatar = avatarUrls[Math.floor(Math.random() * avatarUrls.length)]

      // 更新用户头像
      const user = users.find((u) => u.id === '11111111-2222-3333-4444-555555555551')
      if (user) {
        user.avatar = randomAvatar
        user.updateTime = formatNow()
      }

      return {
        code: 200,
        message: '上传成功',
        data: { url: randomAvatar },
      }
    },
  },

  // 绑定邮箱
  {
    url: '/api/user/bind-email',
    method: 'post',
    response: ({ body }: { body: { email: string; password: string } }) => {
      const { email, password } = body

      const user = users.find((u) => u.id === '11111111-2222-3333-4444-555555555551')

      if (!user) {
        return {
          code: 404,
          message: '用户不存在',
          data: null,
        }
      }

      // 验证密码
      if (user.password !== password) {
        return {
          code: 400,
          message: '密码错误',
          data: null,
        }
      }

      // 更新邮箱
      user.email = email
      user.updateTime = formatNow()

      return {
        code: 200,
        message: '邮箱绑定成功',
        data: { success: true, message: '邮箱绑定成功' },
      }
    },
  },

  // 绑定手机
  {
    url: '/api/user/bind-phone',
    method: 'post',
    response: ({ body }: { body: { phone: string; password: string } }) => {
      const { phone, password } = body

      const user = users.find((u) => u.id === '11111111-2222-3333-4444-555555555551')

      if (!user) {
        return {
          code: 404,
          message: '用户不存在',
          data: null,
        }
      }

      // 验证密码
      if (user.password !== password) {
        return {
          code: 400,
          message: '密码错误',
          data: null,
        }
      }

      // 更新手机
      user.phone = phone
      user.updateTime = formatNow()

      return {
        code: 200,
        message: '手机绑定成功',
        data: { success: true, message: '手机绑定成功' },
      }
    },
  },
] as MockMethod[]