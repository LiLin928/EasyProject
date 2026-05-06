// 登录认证 Mock

import type { MockMethod } from 'vite-plugin-mock'
import type { LoginParams, LoginResponse, UserInfo } from '@/types'

// Mock 用户数据
const users: Record<string, { password: string; userInfo: UserInfo }> = {
  admin: {
    password: '123456',
    userInfo: {
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
  },
  user: {
    password: '123456',
    userInfo: {
      id: '11111111-2222-3333-4444-555555555552',
      userName: 'zhangsan',
      realName: '张三',
      avatar: '',
      email: 'zhangsan@example.com',
      phone: '13800138001',
      roles: ['user'],
      roleIds: ['11111111-2222-3333-4444-555555555557'],
      permissions: ['user:view', 'user:edit'],
      status: 1,
    },
  },
}

export default [
  // 登录接口
  {
    url: '/api/auth/login',
    method: 'post',
    response: ({ body }: { body: LoginParams }) => {
      const { username, password } = body
      const user = users[username]

      if (!user) {
        return {
          code: 500,
          message: '用户不存在',
          data: null,
        }
      }

      if (user.password !== password) {
        return {
          code: 500,
          message: '密码错误',
          data: null,
        }
      }

      return {
        code: 200,
        message: '登录成功',
        data: {
          token: `mock_token_${username}_${Date.now()}`,
          userInfo: user.userInfo,
        } as LoginResponse,
      }
    },
  },
  // 退出登录
  {
    url: '/api/auth/logout',
    method: 'post',
    response: () => {
      return {
        code: 200,
        message: '退出成功',
        data: null,
      }
    },
  },
  // Token刷新
  {
    url: '/api/auth/refresh',
    method: 'post',
    response: ({ body }: { body: { refreshToken: string } }) => {
      const { refreshToken } = body

      if (!refreshToken) {
        return {
          code: 401,
          message: '刷新令牌无效或已过期',
          data: null,
        }
      }

      // Mock 返回新的 token
      return {
        code: 200,
        message: '刷新成功',
        data: {
          token: `mock_token_refreshed_${Date.now()}`,
          refreshToken: `mock_refresh_token_${Date.now()}`,
          userInfo: users.admin.userInfo,
        } as LoginResponse,
      }
    },
  },
] as MockMethod[]