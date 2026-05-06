// src/mock/customer.ts

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * Mock 客户类型
 */
interface MockCustomer {
  id: string
  username: string
  nickname: string
  phone: string
  email: string
  avatar: string
  status: 0 | 1
  levelId: string
  levelName: string
  points: number
  totalSpent: number
  createTime: string
  updateTime: string
}

/**
 * Mock 客户收货地址类型
 */
interface MockCustomerAddress {
  id: string
  customerId: string
  name: string
  phone: string
  province: string
  city: string
  district: string
  address: string
  isDefault: boolean
  createTime: string
  updateTime: string
}

/**
 * Mock 客户购物车项类型
 */
interface MockCustomerCart {
  id: string
  customerId: string
  productId: string
  productName: string
  productImage: string
  skuId: string
  skuName: string
  quantity: number
  price: number
  selected: boolean
  createTime: string
  updateTime: string
}

/**
 * Mock 收藏分组类型
 */
interface MockFavoriteGroup {
  id: string
  customerId: string
  name: string
  sort: number
  createTime: string
  updateTime: string
}

/**
 * Mock 客户收藏项类型
 */
interface MockCustomerFavorite {
  id: string
  customerId: string
  productId: string
  productName: string
  productImage: string
  groupId: string
  groupName: string
  createTime: string
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

// 初始会员等级 ID
const LEVEL_IDS = {
  normal: 'level-0001-aaaa-bbbb-cccc-0001',
  silver: 'level-0002-aaaa-bbbb-cccc-0002',
  gold: 'level-0003-aaaa-bbbb-cccc-0003',
  diamond: 'level-0004-aaaa-bbbb-cccc-0004',
}

// 初始客户数据
const customers: MockCustomer[] = [
  {
    id: 'cust-0001-aaaa-bbbb-cccc-0001',
    username: 'xiaowang',
    nickname: '小王',
    phone: '13800138001',
    email: 'xiaowang@example.com',
    avatar: '/static/images/avatars/avatar1.png',
    status: 1,
    levelId: LEVEL_IDS.gold,
    levelName: '金卡会员',
    points: 520,
    totalSpent: 8500,
    createTime: '2024-01-05 10:00:00',
    updateTime: '2024-03-15 14:30:00',
  },
  {
    id: 'cust-0002-aaaa-bbbb-cccc-0002',
    username: 'xiaoli',
    nickname: '小李',
    phone: '13800138002',
    email: 'xiaoli@example.com',
    avatar: '/static/images/avatars/avatar2.png',
    status: 1,
    levelId: LEVEL_IDS.silver,
    levelName: '银卡会员',
    points: 180,
    totalSpent: 3200,
    createTime: '2024-01-10 10:00:00',
    updateTime: '2024-03-10 12:00:00',
  },
  {
    id: 'cust-0003-aaaa-bbbb-cccc-0003',
    username: 'xiaozhang',
    nickname: '小张',
    phone: '13800138003',
    email: 'xiaozhang@example.com',
    avatar: '/static/images/avatars/avatar3.png',
    status: 1,
    levelId: LEVEL_IDS.normal,
    levelName: '普通会员',
    points: 50,
    totalSpent: 800,
    createTime: '2024-02-01 10:00:00',
    updateTime: '2024-02-20 09:00:00',
  },
  {
    id: 'cust-0004-aaaa-bbbb-cccc-0004',
    username: 'xiaochen',
    nickname: '小陈',
    phone: '13800138004',
    email: 'xiaochen@example.com',
    avatar: '/static/images/avatars/avatar4.png',
    status: 1,
    levelId: LEVEL_IDS.diamond,
    levelName: '钻石会员',
    points: 2000,
    totalSpent: 15000,
    createTime: '2024-01-02 10:00:00',
    updateTime: '2024-04-01 16:00:00',
  },
  {
    id: 'cust-0005-aaaa-bbbb-cccc-0005',
    username: 'xiaoliu',
    nickname: '小刘',
    phone: '13800138005',
    email: 'xiaoliu@example.com',
    avatar: '/static/images/avatars/avatar5.png',
    status: 0,
    levelId: LEVEL_IDS.normal,
    levelName: '普通会员',
    points: 0,
    totalSpent: 0,
    createTime: '2024-03-01 10:00:00',
    updateTime: '2024-03-01 10:00:00',
  },
]

// 初始客户收货地址数据
const customerAddresses: MockCustomerAddress[] = [
  {
    id: generateGuid(),
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    name: '小王',
    phone: '13800138001',
    province: '广东省',
    city: '广州市',
    district: '天河区',
    address: '中山大道100号',
    isDefault: true,
    createTime: '2024-01-05 10:00:00',
    updateTime: '2024-01-05 10:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    name: '小王(公司)',
    phone: '13800138001',
    province: '广东省',
    city: '广州市',
    district: '番禺区',
    address: '大学城南一路50号',
    isDefault: false,
    createTime: '2024-02-01 10:00:00',
    updateTime: '2024-02-01 10:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0002-aaaa-bbbb-cccc-0002',
    name: '小李',
    phone: '13800138002',
    province: '北京市',
    city: '北京市',
    district: '朝阳区',
    address: '建国路88号',
    isDefault: true,
    createTime: '2024-01-10 10:00:00',
    updateTime: '2024-01-10 10:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0004-aaaa-bbbb-cccc-0004',
    name: '小陈',
    phone: '13800138004',
    province: '上海市',
    city: '上海市',
    district: '浦东新区',
    address: '陆家嘴金融中心',
    isDefault: true,
    createTime: '2024-01-02 10:00:00',
    updateTime: '2024-01-02 10:00:00',
  },
]

// 初始客户购物车数据
const customerCarts: MockCustomerCart[] = [
  {
    id: generateGuid(),
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    productId: '550e8400-e29b-41d4-a716-446655440001',
    productName: '红玫瑰鲜花束',
    productImage: '/static/images/products/rose-red.jpg',
    skuId: 'sku-rose-red-001',
    skuName: '红玫瑰鲜花束-标准版',
    quantity: 1,
    price: 199,
    selected: true,
    createTime: '2024-03-20 10:00:00',
    updateTime: '2024-03-20 10:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    productId: '550e8400-e29b-41d4-a716-446655440003',
    productName: '白色百合花束',
    productImage: '/static/images/products/lily-white.jpg',
    skuId: 'sku-lily-white-001',
    skuName: '白色百合花束-标准版',
    quantity: 2,
    price: 158,
    selected: true,
    createTime: '2024-03-21 14:00:00',
    updateTime: '2024-03-21 14:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0002-aaaa-bbbb-cccc-0002',
    productId: '550e8400-e29b-41d4-a716-446655440002',
    productName: '粉色康乃馨花束',
    productImage: '/static/images/products/carnation-pink.jpg',
    skuId: 'sku-carn-pink-001',
    skuName: '粉色康乃馨花束-标准版',
    quantity: 1,
    price: 128,
    selected: true,
    createTime: '2024-03-15 10:00:00',
    updateTime: '2024-03-15 10:00:00',
  },
]

// 初始收藏分组数据
const favoriteGroups: MockFavoriteGroup[] = [
  {
    id: 'fav-group-0001-aaaa-bbbb-cccc-0001',
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    name: '喜欢的花卉',
    sort: 1,
    createTime: '2024-02-01 10:00:00',
    updateTime: '2024-02-01 10:00:00',
  },
  {
    id: 'fav-group-0002-aaaa-bbbb-cccc-0002',
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    name: '送给朋友',
    sort: 2,
    createTime: '2024-02-15 10:00:00',
    updateTime: '2024-02-15 10:00:00',
  },
  {
    id: 'fav-group-0003-aaaa-bbbb-cccc-0003',
    customerId: 'cust-0002-aaaa-bbbb-cccc-0002',
    name: '母亲节礼物',
    sort: 1,
    createTime: '2024-03-01 10:00:00',
    updateTime: '2024-03-01 10:00:00',
  },
]

// 初始客户收藏数据
const customerFavorites: MockCustomerFavorite[] = [
  {
    id: generateGuid(),
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    productId: '550e8400-e29b-41d4-a716-446655440001',
    productName: '红玫瑰鲜花束',
    productImage: '/static/images/products/rose-red.jpg',
    groupId: 'fav-group-0001-aaaa-bbbb-cccc-0001',
    groupName: '喜欢的花卉',
    createTime: '2024-02-10 10:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    productId: '550e8400-e29b-41d4-a716-446655440003',
    productName: '白色百合花束',
    productImage: '/static/images/products/lily-white.jpg',
    groupId: 'fav-group-0001-aaaa-bbbb-cccc-0001',
    groupName: '喜欢的花卉',
    createTime: '2024-02-12 10:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0001-aaaa-bbbb-cccc-0001',
    productId: '550e8400-e29b-41d4-a716-446655440004',
    productName: '向日葵花束',
    productImage: '/static/images/products/sunflower.jpg',
    groupId: 'fav-group-0002-aaaa-bbbb-cccc-0002',
    groupName: '送给朋友',
    createTime: '2024-03-01 10:00:00',
  },
  {
    id: generateGuid(),
    customerId: 'cust-0002-aaaa-bbbb-cccc-0002',
    productId: '550e8400-e29b-41d4-a716-446655440002',
    productName: '粉色康乃馨花束',
    productImage: '/static/images/products/carnation-pink.jpg',
    groupId: 'fav-group-0003-aaaa-bbbb-cccc-0003',
    groupName: '母亲节礼物',
    createTime: '2024-03-05 10:00:00',
  },
]

export default [
  // ==================== 客户 API ====================

  // 获取客户列表
  {
    url: '/api/customer/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; phone?: string; nickname?: string; levelId?: string; status?: number | string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { phone, nickname, levelId } = query
      const status = query.status !== undefined ? Number(query.status) : undefined

      let filteredCustomers = customers.filter((customer) => {
        let match = true
        if (phone && !customer.phone.includes(phone)) {
          match = false
        }
        if (nickname && !customer.nickname.toLowerCase().includes(nickname.toLowerCase())) {
          match = false
        }
        if (levelId && customer.levelId !== levelId) {
          match = false
        }
        if (status !== undefined && customer.status !== status) {
          match = false
        }
        return match
      })

      // 默认按更新时间倒序
      filteredCustomers.sort((a, b) =>
        new Date(b.updateTime).getTime() - new Date(a.updateTime).getTime()
      )

      const total = filteredCustomers.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredCustomers.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 获取客户详情（路径参数）
  {
    url: /\/api\/customer\/detail\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const customer = customers.find((c) => c.id === id)

      if (!customer) {
        return { code: 404, message: '客户不存在', data: null }
      }

      // 获取客户的地址列表
      const addresses = customerAddresses.filter(a => a.customerId === id)
      // 获取客户的购物车数量
      const cartCount = customerCarts.filter(c => c.customerId === id).length
      // 获取客户的收藏数量
      const favoriteCount = customerFavorites.filter(f => f.customerId === id).length

      return {
        code: 200,
        message: '成功',
        data: {
          ...customer,
          addresses,
          cartCount,
          favoriteCount,
        },
      }
    },
  },

  // 创建客户
  {
    url: '/api/customer/create',
    method: 'post',
    response: ({ body }: { body: { nickname: string; phone: string; email?: string; avatar?: string; status?: 0 | 1; levelId?: string } }) => {
      const { nickname, phone, email, avatar, status, levelId } = body

      if (!nickname) {
        return { code: 400, message: '客户昵称不能为空', data: null }
      }

      if (!phone) {
        return { code: 400, message: '手机号不能为空', data: null }
      }

      if (customers.some((c) => c.phone === phone)) {
        return { code: 400, message: '手机号已存在', data: null }
      }

      const now = formatNow()
      const defaultLevelId = 'level-0001-aaaa-bbbb-cccc-0001'
      const defaultLevelName = '普通会员'

      const newCustomer: MockCustomer = {
        id: generateGuid(),
        username: phone, // 默认用户名为手机号
        nickname,
        phone,
        email: email || '',
        avatar: avatar || '/static/images/avatars/default.png',
        status: status ?? 1,
        levelId: levelId || defaultLevelId,
        levelName: levelId ? (levelId === LEVEL_IDS.silver ? '银卡会员' : levelId === LEVEL_IDS.gold ? '金卡会员' : levelId === LEVEL_IDS.diamond ? '钻石会员' : '普通会员') : defaultLevelName,
        points: 0,
        totalSpent: 0,
        createTime: now,
        updateTime: now,
      }

      customers.push(newCustomer)

      return { code: 200, message: '创建成功', data: { id: newCustomer.id } }
    },
  },

  // 更新客户
  {
    url: '/api/customer/update',
    method: 'put',
    response: ({ body }: { body: { id: string; nickname?: string; phone?: string; email?: string; avatar?: string; status?: 0 | 1; levelId?: string } }) => {
      const { id, nickname, phone, email, avatar, status, levelId } = body

      const customerIndex = customers.findIndex((c) => c.id === id)

      if (customerIndex === -1) {
        return { code: 404, message: '客户不存在', data: null }
      }

      const customer = customers[customerIndex]

      // 手机号唯一性检查
      if (phone && phone !== customer.phone) {
        if (customers.some((c) => c.phone === phone)) {
          return { code: 400, message: '手机号已存在', data: null }
        }
        customer.phone = phone
      }

      if (nickname !== undefined) customer.nickname = nickname
      if (email !== undefined) customer.email = email
      if (avatar !== undefined) customer.avatar = avatar
      if (status !== undefined) customer.status = status

      // 更新等级
      if (levelId !== undefined && levelId !== customer.levelId) {
        customer.levelId = levelId
        customer.levelName = levelId === LEVEL_IDS.silver ? '银卡会员' : levelId === LEVEL_IDS.gold ? '金卡会员' : levelId === LEVEL_IDS.diamond ? '钻石会员' : '普通会员'
      }

      customer.updateTime = formatNow()

      customers[customerIndex] = customer

      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },

  // 删除客户（支持批量）
  {
    url: '/api/customer/delete',
    method: 'delete',
    response: ({ body }: { body: { ids: string[] } }) => {
      const { ids } = body

      if (!ids || ids.length === 0) {
        return { code: 400, message: '请选择要删除的客户', data: null }
      }

      ids.forEach((id) => {
        const index = customers.findIndex((c) => c.id === id)
        if (index !== -1) {
          customers.splice(index, 1)
          // 同时删除相关数据
          customerAddresses.splice(0, customerAddresses.length, ...customerAddresses.filter(a => a.customerId !== id))
          customerCarts.splice(0, customerCarts.length, ...customerCarts.filter(c => c.customerId !== id))
          customerFavorites.splice(0, customerFavorites.length, ...customerFavorites.filter(f => f.customerId !== id))
          favoriteGroups.splice(0, favoriteGroups.length, ...favoriteGroups.filter(g => g.customerId !== id))
        }
      })

      return { code: 200, message: '删除成功', data: { success: true } }
    },
  },

  // 更新客户状态
  {
    url: '/api/customer/update-status',
    method: 'put',
    response: ({ body }: { body: { id: string; status: 0 | 1 } }) => {
      const { id, status } = body

      if (!id) {
        return { code: 400, message: '客户ID不能为空', data: null }
      }

      const customer = customers.find((c) => c.id === id)

      if (!customer) {
        return { code: 404, message: '客户不存在', data: null }
      }

      customer.status = status
      customer.updateTime = formatNow()

      return { code: 200, message: '状态更新成功', data: { success: true } }
    },
  },

  // 调整客户积分
  {
    url: '/api/customer/adjust-points',
    method: 'put',
    response: ({ body }: { body: { id: string; type: 'add' | 'subtract'; amount: number; reason: string } }) => {
      const { id, type, amount, reason } = body

      if (!id) {
        return { code: 400, message: '客户ID不能为空', data: null }
      }

      if (!amount || amount <= 0) {
        return { code: 400, message: '积分数量必须大于0', data: null }
      }

      if (!reason) {
        return { code: 400, message: '调整原因不能为空', data: null }
      }

      const customer = customers.find((c) => c.id === id)

      if (!customer) {
        return { code: 404, message: '客户不存在', data: null }
      }

      const beforePoints = customer.points
      let afterPoints = beforePoints

      if (type === 'add') {
        afterPoints = beforePoints + amount
      } else {
        if (beforePoints < amount) {
          return { code: 400, message: `积分不足，当前积分 ${beforePoints}`, data: null }
        }
        afterPoints = beforePoints - amount
      }

      customer.points = afterPoints
      customer.updateTime = formatNow()

      return {
        code: 200,
        message: '积分调整成功',
        data: {
          success: true,
          beforePoints,
          afterPoints,
          changeAmount: type === 'add' ? amount : -amount,
        },
      }
    },
  },

  // 调整客户等级
  {
    url: '/api/customer/adjust-level',
    method: 'put',
    response: ({ body }: { body: { id: string; levelId: string; reason: string } }) => {
      const { id, levelId, reason } = body

      if (!id) {
        return { code: 400, message: '客户ID不能为空', data: null }
      }

      if (!levelId) {
        return { code: 400, message: '会员等级不能为空', data: null }
      }

      if (!reason) {
        return { code: 400, message: '调整原因不能为空', data: null }
      }

      const customer = customers.find((c) => c.id === id)

      if (!customer) {
        return { code: 404, message: '客户不存在', data: null }
      }

      const beforeLevelId = customer.levelId
      const beforeLevelName = customer.levelName

      customer.levelId = levelId
      customer.levelName = levelId === LEVEL_IDS.silver ? '银卡会员' : levelId === LEVEL_IDS.gold ? '金卡会员' : levelId === LEVEL_IDS.diamond ? '钻石会员' : '普通会员'
      customer.updateTime = formatNow()

      return {
        code: 200,
        message: '等级调整成功',
        data: {
          success: true,
          beforeLevelId,
          beforeLevelName,
          afterLevelId: levelId,
          afterLevelName: customer.levelName,
        },
      }
    },
  },

  // ==================== 客户收货地址 API ====================

  // 获取客户地址列表（路径参数）
  {
    url: /\/api\/customer\/address\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const userId = config.url.split('/').pop()

      const list = customerAddresses.filter(a => a.customerId === userId)

      // 默认地址排在前面
      list.sort((a, b) => {
        if (a.isDefault && !b.isDefault) return -1
        if (!a.isDefault && b.isDefault) return 1
        return new Date(b.updateTime).getTime() - new Date(a.updateTime).getTime()
      })

      return {
        code: 200,
        message: '成功',
        data: list,
      }
    },
  },

  // 创建客户地址
  {
    url: '/api/customer/address/create',
    method: 'post',
    response: ({ body }: { body: { customerId: string; name: string; phone: string; province: string; city: string; district: string; address: string; isDefault?: boolean } }) => {
      const { customerId, name, phone, province, city, district, address, isDefault } = body

      if (!customerId) {
        return { code: 400, message: '客户ID不能为空', data: null }
      }

      if (!name) {
        return { code: 400, message: '收货人不能为空', data: null }
      }

      if (!phone) {
        return { code: 400, message: '手机号不能为空', data: null }
      }

      if (!province || !city || !district || !address) {
        return { code: 400, message: '地址信息不完整', data: null }
      }

      const customer = customers.find(c => c.id === customerId)
      if (!customer) {
        return { code: 404, message: '客户不存在', data: null }
      }

      const now = formatNow()

      // 如果设置为默认，取消其他默认地址
      if (isDefault) {
        customerAddresses.forEach(a => {
          if (a.customerId === customerId) {
            a.isDefault = false
          }
        })
      }

      const newAddress: MockCustomerAddress = {
        id: generateGuid(),
        customerId,
        name,
        phone,
        province,
        city,
        district,
        address,
        isDefault: isDefault ?? customerAddresses.filter(a => a.customerId === customerId).length === 0,
        createTime: now,
        updateTime: now,
      }

      customerAddresses.push(newAddress)

      return { code: 200, message: '创建成功', data: { id: newAddress.id } }
    },
  },

  // 更新客户地址
  {
    url: '/api/customer/address/update',
    method: 'put',
    response: ({ body }: { body: { id: string; name?: string; phone?: string; province?: string; city?: string; district?: string; address?: string; isDefault?: boolean } }) => {
      const { id, name, phone, province, city, district, address, isDefault } = body

      const addressIndex = customerAddresses.findIndex((a) => a.id === id)

      if (addressIndex === -1) {
        return { code: 404, message: '地址不存在', data: null }
      }

      const addr = customerAddresses[addressIndex]

      // 如果设置为默认，取消其他默认地址
      if (isDefault) {
        customerAddresses.forEach(a => {
          if (a.customerId === addr.customerId) {
            a.isDefault = false
          }
        })
      }

      if (name !== undefined) addr.name = name
      if (phone !== undefined) addr.phone = phone
      if (province !== undefined) addr.province = province
      if (city !== undefined) addr.city = city
      if (district !== undefined) addr.district = district
      if (address !== undefined) addr.address = address
      if (isDefault !== undefined) addr.isDefault = isDefault
      addr.updateTime = formatNow()

      customerAddresses[addressIndex] = addr

      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },

  // 删除客户地址
  {
    url: '/api/customer/address/delete',
    method: 'delete',
    response: ({ body }: { body: { id: string } }) => {
      const { id } = body

      const index = customerAddresses.findIndex((a) => a.id === id)
      if (index !== -1) {
        customerAddresses.splice(index, 1)
      }

      return { code: 200, message: '删除成功', data: { success: true } }
    },
  },

  // 设置默认地址
  {
    url: '/api/customer/address/set-default',
    method: 'put',
    response: ({ body }: { body: { userId: string; addressId: string } }) => {
      const { userId, addressId } = body

      const addr = customerAddresses.find((a) => a.id === addressId && a.customerId === userId)
      if (!addr) {
        return { code: 404, message: '地址不存在', data: null }
      }

      // 取消其他默认
      customerAddresses.forEach(a => {
        if (a.customerId === userId) {
          a.isDefault = a.id === addressId
        }
      })

      addr.updateTime = formatNow()

      return { code: 200, message: '设置成功', data: { success: true } }
    },
  },

  // ==================== 客户购物车 API ====================

  // 获取客户购物车列表（路径参数）
  {
    url: /\/api\/customer\/cart\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const userId = config.url.split('/').pop()

      const list = customerCarts.filter(c => c.customerId === userId)

      // 按更新时间倒序
      list.sort((a, b) =>
        new Date(b.updateTime).getTime() - new Date(a.updateTime).getTime()
      )

      // 计算选中商品的总价
      const totalPrice = list
        .filter(c => c.selected)
        .reduce((sum, c) => sum + c.price * c.quantity, 0)

      return {
        code: 200,
        message: '成功',
        data: {
          list,
          totalPrice,
          totalCount: list.length,
          selectedCount: list.filter(c => c.selected).length,
        },
      }
    },
  },

  // ==================== 客户收藏 API ====================

  // 获取客户收藏列表（路径参数）
  {
    url: /\/api\/customer\/favorite\/[\w-]+/,
    method: 'get',
    response: (config: { url: string; query?: { groupId?: string } }) => {
      const userId = config.url.split('/').pop()
      const { groupId } = config.query || {}

      let list = customerFavorites.filter(f => f.customerId === userId)

      if (groupId) {
        list = list.filter(f => f.groupId === groupId)
      }

      // 按收藏时间倒序
      list.sort((a, b) =>
        new Date(b.createTime).getTime() - new Date(a.createTime).getTime()
      )

      return {
        code: 200,
        message: '成功',
        data: list,
      }
    },
  },

  // 获取收藏分组列表
  {
    url: '/api/customer/favorite-group/list',
    method: 'get',
    response: ({ query }: { query: { userId: string } }) => {
      const { userId } = query

      if (!userId) {
        return { code: 400, message: '客户ID不能为空', data: null }
      }

      const list = favoriteGroups.filter(g => g.customerId === userId)

      // 按排序升序
      list.sort((a, b) => a.sort - b.sort)

      // 添加每个分组的收藏数量
      const result = list.map(g => ({
        ...g,
        favoriteCount: customerFavorites.filter(f => f.groupId === g.id).length,
      }))

      return {
        code: 200,
        message: '成功',
        data: result,
      }
    },
  },
] as MockMethod[]