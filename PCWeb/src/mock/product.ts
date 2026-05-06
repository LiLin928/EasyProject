// src/mock/product.ts

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * Mock 商品分类类型
 */
interface MockCategory {
  id: string
  name: string
  icon: string
  sort: number
  description?: string
  createTime: string
  updateTime: string
}

/**
 * Mock 商品类型
 */
interface MockProduct {
  id: string
  skuCode: string // SKU码
  name: string
  description: string
  price: number
  originalPrice: number
  image: string
  images: string[]
  categoryId: string
  stock: number
  sales: number
  isHot: boolean
  isNew: boolean
  detail: string
  alertThreshold: number // 库存预警阈值
  createTime: string
  updateTime: string
}

/**
 * 库存变动类型
 */
type StockChangeType = 'in' | 'out' | 'adjust'

/**
 * Mock 库存变动记录类型
 */
interface MockStockRecord {
  id: string
  productId: string
  skuCode?: string // SKU码
  productName: string
  productImage: string
  type: StockChangeType
  quantity: number
  beforeStock: number
  afterStock: number
  supplierId?: string
  supplierName?: string
  purchasePrice?: number
  operator: string
  remark: string
  createTime: string
}

/**
 * Mock 供应商类型
 */
interface MockSupplier {
  id: string
  name: string
  unifiedCode?: string // 三证合一码
  contact: string
  phone: string
  address: string
  remark: string
  status: 0 | 1
  createTime: string
  updateTime: string
}

/**
 * Mock 商品-供应商关联类型
 */
interface MockProductSupplier {
  id: string
  productId: string
  supplierId: string
  skuCode: string // SKU码
  purchasePrice: number
  minOrderQty: number
  isDefault: boolean
  remark: string
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

// 初始商品分类数据
const categories: MockCategory[] = [
  {
    id: '550e8400-e29b-41d4-a716-446655440101',
    name: '玫瑰',
    icon: '/static/images/icons/rose.png',
    sort: 1,
    description: '玫瑰花卉分类',
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440102',
    name: '康乃馨',
    icon: '/static/images/icons/carnation.png',
    sort: 2,
    description: '康乃馨花卉分类',
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440103',
    name: '百合',
    icon: '/static/images/icons/lily.png',
    sort: 3,
    description: '百合花卉分类',
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440104',
    name: '向日葵',
    icon: '/static/images/icons/sunflower.png',
    sort: 4,
    description: '向日葵花卉分类',
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440105',
    name: '混合花束',
    icon: '/static/images/icons/mixed.png',
    sort: 5,
    description: '混合花束分类',
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
]

// 初始商品数据
const products: MockProduct[] = [
  {
    id: '550e8400-e29b-41d4-a716-446655440001',
    skuCode: 'SKU-ROSE-RED-001',
    name: '红玫瑰鲜花束',
    description: '11支红玫瑰，配满天星和绿叶',
    price: 199.00,
    originalPrice: 299.00,
    image: '/static/images/products/rose-red.jpg',
    images: ['/static/images/products/rose-red-1.jpg', '/static/images/products/rose-red-2.jpg'],
    categoryId: '550e8400-e29b-41d4-a716-446655440101',
    stock: 100,
    sales: 528,
    isHot: true,
    isNew: false,
    detail: '<p>11支精选红玫瑰，搭配满天星和绿叶，代表热烈的爱情与真挚的情感。</p>',
    alertThreshold: 20,
    createTime: '2024-01-15 10:00:00',
    updateTime: '2024-01-15 10:00:00',
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440002',
    skuCode: 'SKU-CARN-PINK-001',
    name: '粉色康乃馨花束',
    description: '20支粉色康乃馨，温馨祝福',
    price: 128.00,
    originalPrice: 168.00,
    image: '/static/images/products/carnation-pink.jpg',
    images: ['/static/images/products/carnation-pink-1.jpg'],
    categoryId: '550e8400-e29b-41d4-a716-446655440102',
    stock: 80,
    sales: 312,
    isHot: false,
    isNew: true,
    detail: '<p>20支粉色康乃馨，温馨典雅，适合送给母亲、老师等长辈，表达感恩与祝福。</p>',
    alertThreshold: 15,
    createTime: '2024-01-14 10:00:00',
    updateTime: '2024-01-14 10:00:00',
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440003',
    skuCode: 'SKU-LILY-WHITE-001',
    name: '白色百合花束',
    description: '6支白色百合，高雅纯洁',
    price: 158.00,
    originalPrice: 198.00,
    image: '/static/images/products/lily-white.jpg',
    images: ['/static/images/products/lily-white-1.jpg'],
    categoryId: '550e8400-e29b-41d4-a716-446655440103',
    stock: 50,
    sales: 186,
    isHot: true,
    isNew: false,
    detail: '<p>6支白色百合，象征纯洁与高雅，适合各种场合。</p>',
    alertThreshold: 10,
    createTime: '2024-01-13 10:00:00',
    updateTime: '2024-01-13 10:00:00',
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440004',
    skuCode: 'SKU-SUN-001',
    name: '向日葵花束',
    description: '5支向日葵，阳光活力',
    price: 98.00,
    originalPrice: 128.00,
    image: '/static/images/products/sunflower.jpg',
    images: ['/static/images/products/sunflower-1.jpg'],
    categoryId: '550e8400-e29b-41d4-a716-446655440104',
    stock: 60,
    sales: 234,
    isHot: false,
    isNew: true,
    detail: '<p>5支向日葵，代表阳光与活力，适合送给朋友或装饰空间。</p>',
    alertThreshold: 10,
    createTime: '2024-01-12 10:00:00',
    updateTime: '2024-01-12 10:00:00',
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440005',
    skuCode: 'SKU-MIXED-001',
    name: '混合鲜花束',
    description: '玫瑰、康乃馨、百合混合搭配',
    price: 268.00,
    originalPrice: 358.00,
    image: '/static/images/products/mixed.jpg',
    images: ['/static/images/products/mixed-1.jpg'],
    categoryId: '550e8400-e29b-41d4-a716-446655440105',
    stock: 8,
    sales: 89,
    isHot: true,
    isNew: true,
    detail: '<p>玫瑰、康乃馨、百合精选搭配，色彩丰富，适合各种节日和场合。</p>',
    alertThreshold: 10,
    createTime: '2024-01-11 10:00:00',
    updateTime: '2024-01-11 10:00:00',
  },
  {
    id: '550e8400-e29b-41d4-a716-446655440006',
    skuCode: 'SKU-BLUE-ROSE-001',
    name: '蓝色妖姬花束',
    description: '10支蓝色妖姬玫瑰，独特魅力',
    price: 388.00,
    originalPrice: 488.00,
    image: '/static/images/products/blue-rose.jpg',
    images: ['/static/images/products/blue-rose-1.jpg'],
    categoryId: '550e8400-e29b-41d4-a716-446655440101',
    stock: 5,
    sales: 156,
    isHot: false,
    isNew: false,
    detail: '<p>10支蓝色妖姬玫瑰，独特神秘，适合表达特殊的情感。</p>',
    alertThreshold: 5,
    createTime: '2024-01-10 10:00:00',
    updateTime: '2024-01-10 10:00:00',
  },
]

// 库存变动记录数据
const stockRecords: MockStockRecord[] = [
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440001',
    productName: '红玫瑰鲜花束',
    productImage: '/static/images/products/rose-red.jpg',
    type: 'in',
    quantity: 50,
    beforeStock: 50,
    afterStock: 100,
    operator: 'admin',
    remark: '采购入库',
    createTime: '2024-01-15 10:00:00',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440005',
    productName: '混合鲜花束',
    productImage: '/static/images/products/mixed.jpg',
    type: 'out',
    quantity: 22,
    beforeStock: 30,
    afterStock: 8,
    operator: 'admin',
    remark: '订单出库',
    createTime: '2024-01-16 14:30:00',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440006',
    productName: '蓝色妖姬花束',
    productImage: '/static/images/products/blue-rose.jpg',
    type: 'adjust',
    quantity: -15,
    beforeStock: 20,
    afterStock: 5,
    operator: 'admin',
    remark: '库存盘点调整',
    createTime: '2024-01-17 09:00:00',
  },
]

// 供应商数据
const suppliers: MockSupplier[] = [
  {
    id: '660e8400-e29b-41d4-a716-446655440001',
    name: '云南花卉基地',
    unifiedCode: '91530100MA6K7XYZ12',
    contact: '张经理',
    phone: '13800138001',
    address: '云南省昆明市呈贡区',
    remark: '主要供应玫瑰、百合',
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '660e8400-e29b-41d4-a716-446655440002',
    name: '广州鲜花批发',
    unifiedCode: '91440100MA5D2ABC34',
    contact: '李总',
    phone: '13800138002',
    address: '广东省广州市白云区',
    remark: '品种齐全，价格优惠',
    status: 1,
    createTime: '2024-01-02 10:00:00',
    updateTime: '2024-01-02 10:00:00',
  },
  {
    id: '660e8400-e29b-41d4-a716-446655440003',
    name: '上海花卉市场',
    unifiedCode: '91310000MA1K3DEF56',
    contact: '王经理',
    phone: '13800138003',
    address: '上海市浦东新区',
    remark: '高端花卉供应商',
    status: 1,
    createTime: '2024-01-03 10:00:00',
    updateTime: '2024-01-03 10:00:00',
  },
]

// 商品-供应商关联数据
const productSuppliers: MockProductSupplier[] = [
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440001',
    supplierId: '660e8400-e29b-41d4-a716-446655440001',
    skuCode: 'SKU-ROSE-RED-YN-001',
    purchasePrice: 120,
    minOrderQty: 10,
    isDefault: true,
    remark: '',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440001',
    supplierId: '660e8400-e29b-41d4-a716-446655440002',
    skuCode: 'SKU-ROSE-RED-GZ-001',
    purchasePrice: 115,
    minOrderQty: 20,
    isDefault: false,
    remark: '价格更低但起订量高',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440002',
    supplierId: '660e8400-e29b-41d4-a716-446655440001',
    skuCode: 'SKU-CARN-PINK-YN-001',
    purchasePrice: 80,
    minOrderQty: 15,
    isDefault: true,
    remark: '',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440003',
    supplierId: '660e8400-e29b-41d4-a716-446655440003',
    skuCode: 'SKU-LILY-WHITE-SH-001',
    purchasePrice: 100,
    minOrderQty: 5,
    isDefault: true,
    remark: '高端百合',
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440005',
    supplierId: '660e8400-e29b-41d4-a716-446655440002',
    skuCode: 'SKU-MIXED-GZ-001',
    purchasePrice: 180,
    minOrderQty: 10,
    isDefault: true,
    remark: '',
  },
]

export default [
  // ==================== 商品 API ====================

  // 获取商品列表
  {
    url: '/api/product/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; name?: string; skuCode?: string; categoryId?: string; isHot?: boolean | string; isNew?: boolean | string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { name, skuCode, categoryId } = query
      const isHot = query.isHot !== undefined ? query.isHot === 'true' || query.isHot === true : undefined
      const isNew = query.isNew !== undefined ? query.isNew === 'true' || query.isNew === true : undefined

      let filteredProducts = products.filter((product) => {
        let match = true
        if (name && !product.name.toLowerCase().includes(name.toLowerCase())) {
          match = false
        }
        if (skuCode && !product.skuCode.toLowerCase().includes(skuCode.toLowerCase())) {
          match = false
        }
        if (categoryId && product.categoryId !== categoryId) {
          match = false
        }
        if (isHot !== undefined && product.isHot !== isHot) {
          match = false
        }
        if (isNew !== undefined && product.isNew !== isNew) {
          match = false
        }
        return match
      })

      // 默认按更新时间倒序
      filteredProducts.sort((a, b) =>
        new Date(b.updateTime).getTime() - new Date(a.updateTime).getTime()
      )

      const total = filteredProducts.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredProducts.slice(start, end).map(product => ({
        ...product,
        category: categories.find(c => c.id === product.categoryId),
      }))

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 获取商品详情（路径参数）
  {
    url: /\/api\/product\/detail\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const product = products.find((p) => p.id === id)

      if (!product) {
        return { code: 404, message: '商品不存在', data: null }
      }

      return {
        code: 200,
        message: '成功',
        data: {
          ...product,
          category: categories.find(c => c.id === product.categoryId),
        },
      }
    },
  },

  // 创建商品
  {
    url: '/api/product/add',
    method: 'post',
    response: ({ body }: { body: Omit<MockProduct, 'id' | 'createTime' | 'updateTime' | 'sales'> }) => {
      const { skuCode, name, description, price, originalPrice, image, images, categoryId, stock, isHot, isNew, detail, alertThreshold } = body

      if (!name) {
        return { code: 400, message: '商品名称不能为空', data: null }
      }

      if (!skuCode) {
        return { code: 400, message: 'SKU码不能为空', data: null }
      }

      if (!categoryId) {
        return { code: 400, message: '请选择商品分类', data: null }
      }

      if (products.some((p) => p.skuCode === skuCode)) {
        return { code: 400, message: 'SKU码已存在', data: null }
      }

      const now = formatNow()
      const newProduct: MockProduct = {
        id: generateGuid(),
        skuCode,
        name,
        description: description || '',
        price: price || 0,
        originalPrice: originalPrice || price || 0,
        image: image || '/static/images/products/default.jpg',
        images: images || [],
        categoryId,
        stock: stock || 0,
        sales: 0,
        isHot: isHot ?? false,
        isNew: isNew ?? false,
        detail: detail || '',
        alertThreshold: alertThreshold || 10,
        createTime: now,
        updateTime: now,
      }

      products.push(newProduct)

      return { code: 200, message: '创建成功', data: { id: newProduct.id } }
    },
  },

  // 更新商品（PUT方法）
  {
    url: '/api/product/update',
    method: 'put',
    response: ({ body }: { body: Partial<MockProduct> & { id: string } }) => {
      const { id, skuCode, name, description, price, originalPrice, image, images, categoryId, stock, isHot, isNew, detail, alertThreshold } = body

      const productIndex = products.findIndex((p) => p.id === id)

      if (productIndex === -1) {
        return { code: 404, message: '商品不存在', data: null }
      }

      const product = products[productIndex]

      // SKU码唯一性检查
      if (skuCode && skuCode !== product.skuCode) {
        if (products.some((p) => p.skuCode === skuCode)) {
          return { code: 400, message: 'SKU码已存在', data: null }
        }
        product.skuCode = skuCode
      }

      if (name !== undefined) product.name = name
      if (description !== undefined) product.description = description
      if (price !== undefined) product.price = price
      if (originalPrice !== undefined) product.originalPrice = originalPrice
      if (image !== undefined) product.image = image
      if (images !== undefined) product.images = images
      if (categoryId !== undefined) product.categoryId = categoryId
      if (stock !== undefined) product.stock = stock
      if (isHot !== undefined) product.isHot = isHot
      if (isNew !== undefined) product.isNew = isNew
      if (detail !== undefined) product.detail = detail
      if (alertThreshold !== undefined) product.alertThreshold = alertThreshold
      product.updateTime = formatNow()

      products[productIndex] = product

      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },

  // 删除商品（DELETE方法，路径参数）
  {
    url: /\/api\/product\/delete\/[\w-]+/,
    method: 'delete',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()

      const index = products.findIndex((p) => p.id === id)
      if (index !== -1) {
        products.splice(index, 1)
      }

      return { code: 200, message: '删除成功', data: 1 }
    },
  },

  // ==================== 分类 API ====================

  // 获取分类列表（不分页，直接返回数组）
  {
    url: '/api/product/category/list',
    method: 'get',
    response: ({ query }: { query: { name?: string } }) => {
      const { name } = query

      let filteredCategories = categories.filter((category) => {
        let match = true
        if (name && !category.name.toLowerCase().includes(name.toLowerCase())) {
          match = false
        }
        return match
      })

      // 按排序号升序
      filteredCategories.sort((a, b) => a.sort - b.sort)

      return {
        code: 200,
        message: '成功',
        data: filteredCategories,
      }
    },
  },

  // 创建分类
  {
    url: '/api/product/category/add',
    method: 'post',
    response: ({ body }: { body: Omit<MockCategory, 'id' | 'createTime' | 'updateTime'> }) => {
      const { name, icon, sort, description } = body

      if (!name) {
        return { code: 400, message: '分类名称不能为空', data: null }
      }

      if (categories.some((c) => c.name === name)) {
        return { code: 400, message: '分类名称已存在', data: null }
      }

      const now = formatNow()
      const newCategory: MockCategory = {
        id: generateGuid(),
        name,
        icon: icon || '/static/images/icons/default.png',
        sort: sort ?? categories.length + 1,
        description: description || '',
        createTime: now,
        updateTime: now,
      }

      categories.push(newCategory)

      return { code: 200, message: '创建成功', data: { id: newCategory.id } }
    },
  },

  // 更新分类（PUT方法）
  {
    url: '/api/product/category/update',
    method: 'put',
    response: ({ body }: { body: Partial<MockCategory> & { id: string } }) => {
      const { id, name, icon, sort, description } = body

      const categoryIndex = categories.findIndex((c) => c.id === id)

      if (categoryIndex === -1) {
        return { code: 404, message: '分类不存在', data: null }
      }

      const category = categories[categoryIndex]

      if (name && name !== category.name) {
        if (categories.some((c) => c.name === name)) {
          return { code: 400, message: '分类名称已存在', data: null }
        }
        category.name = name
      }

      if (icon !== undefined) category.icon = icon
      if (sort !== undefined) category.sort = sort
      if (description !== undefined) category.description = description
      category.updateTime = formatNow()

      categories[categoryIndex] = category

      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },

  // 删除分类（DELETE方法，路径参数）
  {
    url: /\/api\/product\/category\/delete\/[\w-]+/,
    method: 'delete',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()

      // 检查是否有商品关联该分类
      const productCount = products.filter(p => p.categoryId === id).length
      if (productCount > 0) {
        const category = categories.find(c => c.id === id)
        return {
          code: 400,
          message: `分类"${category?.name}"下还有${productCount}个商品，无法删除`,
          data: null,
        }
      }

      const index = categories.findIndex((c) => c.id === id)
      if (index !== -1) {
        categories.splice(index, 1)
      }

      return { code: 200, message: '删除成功', data: 1 }
    },
  },

  // 获取分类详情（路径参数）
  {
    url: /\/api\/product\/category\/detail\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()

      const category = categories.find(c => c.id === id)

      if (!category) {
        return { code: 404, message: '分类不存在', data: null }
      }

      return { code: 200, message: '成功', data: category }
    },
  },

  // ==================== 批量操作 API ====================

  // 批量修改分类
  {
    url: '/api/product/batch/category',
    method: 'post',
    response: ({ body }: { body: { ids: string[]; categoryId: string } }) => {
      const { ids, categoryId } = body

      if (!ids || ids.length === 0) {
        return { code: 400, message: '请选择要修改的商品', data: null }
      }

      if (!categoryId) {
        return { code: 400, message: '请选择分类', data: null }
      }

      // 检查分类是否存在
      const category = categories.find(c => c.id === categoryId)
      if (!category) {
        return { code: 400, message: '分类不存在', data: null }
      }

      const now = formatNow()
      let count = 0
      ids.forEach((id) => {
        const product = products.find(p => p.id === id)
        if (product) {
          product.categoryId = categoryId
          product.updateTime = now
          count++
        }
      })

      return { code: 200, message: '修改成功', data: { success: true, count } }
    },
  },

  // 批量修改标签
  {
    url: '/api/product/batch/tag',
    method: 'post',
    response: ({ body }: { body: { ids: string[]; isHot?: boolean; isNew?: boolean } }) => {
      const { ids, isHot, isNew } = body

      if (!ids || ids.length === 0) {
        return { code: 400, message: '请选择要修改的商品', data: null }
      }

      const now = formatNow()
      let count = 0
      ids.forEach((id) => {
        const product = products.find(p => p.id === id)
        if (product) {
          if (isHot !== undefined) product.isHot = isHot
          if (isNew !== undefined) product.isNew = isNew
          product.updateTime = now
          count++
        }
      })

      return { code: 200, message: '修改成功', data: { success: true, count } }
    },
  },

  // 批量创建商品（导入）
  {
    url: '/api/product/batch/create',
    method: 'post',
    response: ({ body }: { body: { products: Omit<MockProduct, 'id' | 'createTime' | 'updateTime' | 'sales'>[] } }) => {
      const { products: productList } = body

      if (!productList || productList.length === 0) {
        return { code: 400, message: '没有要导入的商品', data: null }
      }

      const now = formatNow()
      const createdIds: string[] = []
      const errors: string[] = []

      productList.forEach((product, index) => {
        // 验证必填字段
        if (!product.name) {
          errors.push(`第${index + 1}行: 商品名称不能为空`)
          return
        }
        if (!product.categoryId) {
          errors.push(`第${index + 1}行: 分类不能为空`)
          return
        }

        // 检查分类是否存在
        const category = categories.find(c => c.id === product.categoryId)
        if (!category) {
          errors.push(`第${index + 1}行: 分类不存在`)
          return
        }

        // 创建商品
        const newProduct: MockProduct = {
          id: generateGuid(),
          name: product.name,
          description: product.description || '',
          price: product.price || 0,
          originalPrice: product.originalPrice || product.price || 0,
          image: product.image || '/static/images/products/default.jpg',
          images: product.images || [],
          categoryId: product.categoryId,
          stock: product.stock || 0,
          sales: 0,
          isHot: product.isHot ?? false,
          isNew: product.isNew ?? false,
          detail: product.detail || '',
          createTime: now,
          updateTime: now,
        }

        products.push(newProduct)
        createdIds.push(newProduct.id)
      })

      if (errors.length > 0) {
        return {
          code: 207, // Multi-Status
          message: `导入完成，成功 ${createdIds.length} 条，失败 ${errors.length} 条`,
          data: {
            success: createdIds.length,
            failed: errors.length,
            errors,
            ids: createdIds,
          },
        }
      }

      return {
        code: 200,
        message: `导入成功，共导入 ${createdIds.length} 个商品`,
        data: { ids: createdIds, count: createdIds.length },
      }
    },
  },

  // ==================== 库存管理 API ====================

  // 获取库存列表
  {
    url: '/api/product/stock/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; name?: string; categoryId?: string; lowStock?: boolean | string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { name, categoryId } = query
      const lowStock = query.lowStock === 'true' || query.lowStock === true

      let filteredProducts = products.filter((product) => {
        let match = true
        if (name && !product.name.toLowerCase().includes(name.toLowerCase())) {
          match = false
        }
        if (categoryId && product.categoryId !== categoryId) {
          match = false
        }
        // 低库存筛选：库存 < 预警阈值
        if (lowStock && product.stock >= (product.alertThreshold || 10)) {
          match = false
        }
        return match
      })

      // 按更新时间倒序
      filteredProducts.sort((a, b) =>
        new Date(b.updateTime).getTime() - new Date(a.updateTime).getTime()
      )

      const total = filteredProducts.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredProducts.slice(start, end).map((p) => ({
        productId: p.id,
        skuCode: p.skuCode,
        productName: p.name,
        productImage: p.image,
        categoryName: categories.find((c) => c.id === p.categoryId)?.name || '-',
        stock: p.stock,
        alertThreshold: p.alertThreshold || 10,
        isLowStock: p.stock < (p.alertThreshold || 10),
        updateTime: p.updateTime,
      }))

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 获取出入库记录
  {
    url: '/api/product/stock/record',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; productId?: string; type?: StockChangeType; startDate?: string; endDate?: string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { productId, type, startDate, endDate } = query

      let filteredRecords = stockRecords.filter((record) => {
        let match = true
        if (productId && record.productId !== productId) {
          match = false
        }
        if (type && record.type !== type) {
          match = false
        }
        if (startDate && new Date(record.createTime) < new Date(startDate)) {
          match = false
        }
        if (endDate && new Date(record.createTime) > new Date(endDate + ' 23:59:59')) {
          match = false
        }
        return match
      })

      // 按时间倒序
      filteredRecords.sort((a, b) =>
        new Date(b.createTime).getTime() - new Date(a.createTime).getTime()
      )

      const total = filteredRecords.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredRecords.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 库存调整
  {
    url: '/api/product/stock/adjust',
    method: 'post',
    response: ({ body }: { body: { productId: string; type: StockChangeType; quantity: number; supplierId?: string; purchasePrice?: number; skuCode?: string; remark?: string } }) => {
      const { productId, type, quantity, supplierId, purchasePrice, skuCode, remark } = body

      if (!productId) {
        return { code: 400, message: '请选择商品', data: null }
      }

      const product = products.find(p => p.id === productId)
      if (!product) {
        return { code: 404, message: '商品不存在', data: null }
      }

      const beforeStock = product.stock
      let afterStock = beforeStock
      let actualQuantity = quantity

      // 根据类型计算库存变化
      if (type === 'in') {
        if (quantity <= 0) {
          return { code: 400, message: '入库数量必须大于 0', data: null }
        }
        if (!supplierId) {
          return { code: 400, message: '入库时请选择供应商', data: null }
        }
        if (!purchasePrice || purchasePrice <= 0) {
          return { code: 400, message: '入库时请输入采购价格', data: null }
        }
        afterStock = beforeStock + quantity
      } else if (type === 'out') {
        if (quantity <= 0) {
          return { code: 400, message: '出库数量必须大于 0', data: null }
        }
        if (beforeStock < quantity) {
          return { code: 400, message: `库存不足，当前库存 ${beforeStock}`, data: null }
        }
        afterStock = beforeStock - quantity
        actualQuantity = -quantity
      } else {
        // 调整模式：直接设置库存
        if (quantity < 0) {
          return { code: 400, message: '库存不能为负数', data: null }
        }
        afterStock = quantity
        actualQuantity = afterStock - beforeStock
      }

      // 更新商品库存
      product.stock = afterStock
      product.updateTime = formatNow()

      // 获取供应商信息和SKU码
      const supplier = supplierId ? suppliers.find(s => s.id === supplierId) : null
      const productSupplier = supplierId && productId ? productSuppliers.find(ps => ps.supplierId === supplierId && ps.productId === productId) : null
      const recordSkuCode = skuCode || productSupplier?.skuCode || product.skuCode

      // 创建库存变动记录
      const newRecord: MockStockRecord = {
        id: generateGuid(),
        productId: product.id,
        skuCode: recordSkuCode,
        productName: product.name,
        productImage: product.image,
        type,
        quantity: actualQuantity,
        beforeStock,
        afterStock,
        supplierId: type === 'in' ? supplierId : undefined,
        supplierName: type === 'in' ? supplier?.name : undefined,
        purchasePrice: type === 'in' ? purchasePrice : undefined,
        operator: 'admin',
        remark: remark || '',
        createTime: formatNow(),
      }
      stockRecords.unshift(newRecord)

      return {
        code: 200,
        message: '库存调整成功',
        data: { success: true, record: newRecord },
      }
    },
  },

  // 批量设置预警阈值
  {
    url: '/api/product/stock/alert-threshold',
    method: 'post',
    response: ({ body }: { body: { ids: string[]; threshold: number } }) => {
      const { ids, threshold } = body

      if (!ids || ids.length === 0) {
        return { code: 400, message: '请选择商品', data: null }
      }

      if (threshold < 0) {
        return { code: 400, message: '预警阈值不能为负数', data: null }
      }

      const now = formatNow()
      let count = 0
      ids.forEach((id) => {
        const product = products.find(p => p.id === id)
        if (product) {
          product.alertThreshold = threshold
          product.updateTime = now
          count++
        }
      })

      return { code: 200, message: '设置成功', data: { success: true, count } }
    },
  },

  // ==================== 供应商管理 API ====================

  // 获取供应商列表
  {
    url: '/api/supplier/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; name?: string; status?: number | string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { name } = query
      const status = query.status !== undefined ? Number(query.status) : undefined

      let filteredSuppliers = suppliers.filter((supplier) => {
        let match = true
        if (name && !supplier.name.toLowerCase().includes(name.toLowerCase())) {
          match = false
        }
        if (status !== undefined && supplier.status !== status) {
          match = false
        }
        return match
      })

      // 按创建时间倒序
      filteredSuppliers.sort((a, b) =>
        new Date(b.createTime).getTime() - new Date(a.createTime).getTime()
      )

      const total = filteredSuppliers.length
      const start = (pageIndex - 1) * pageSize
      const end = start + pageSize
      const list = filteredSuppliers.slice(start, end)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 获取供应商详情
  {
    url: '/api/supplier/detail',
    method: 'get',
    response: ({ query }: { query: { id: string } }) => {
      const { id } = query
      const supplier = suppliers.find(s => s.id === id)

      if (!supplier) {
        return { code: 404, message: '供应商不存在', data: null }
      }

      return { code: 200, message: '成功', data: supplier }
    },
  },

  // 创建供应商
  {
    url: '/api/supplier/create',
    method: 'post',
    response: ({ body }: { body: Omit<MockSupplier, 'id' | 'createTime' | 'updateTime'> }) => {
      const { name, unifiedCode, contact, phone, address, remark, status } = body

      if (!name) {
        return { code: 400, message: '供应商名称不能为空', data: null }
      }

      if (suppliers.some(s => s.name === name)) {
        return { code: 400, message: '供应商名称已存在', data: null }
      }

      const now = formatNow()
      const newSupplier: MockSupplier = {
        id: generateGuid(),
        name,
        unifiedCode: unifiedCode || '',
        contact: contact || '',
        phone: phone || '',
        address: address || '',
        remark: remark || '',
        status: status ?? 1,
        createTime: now,
        updateTime: now,
      }

      suppliers.push(newSupplier)

      return { code: 200, message: '创建成功', data: { id: newSupplier.id } }
    },
  },

  // 更新供应商
  {
    url: '/api/supplier/update',
    method: 'post',
    response: ({ body }: { body: Partial<MockSupplier> & { id: string } }) => {
      const { id, name, unifiedCode, contact, phone, address, remark, status } = body

      const supplierIndex = suppliers.findIndex(s => s.id === id)

      if (supplierIndex === -1) {
        return { code: 404, message: '供应商不存在', data: null }
      }

      const supplier = suppliers[supplierIndex]

      if (name && name !== supplier.name) {
        if (suppliers.some(s => s.name === name)) {
          return { code: 400, message: '供应商名称已存在', data: null }
        }
        supplier.name = name
      }

      if (unifiedCode !== undefined) supplier.unifiedCode = unifiedCode
      if (contact !== undefined) supplier.contact = contact
      if (phone !== undefined) supplier.phone = phone
      if (address !== undefined) supplier.address = address
      if (remark !== undefined) supplier.remark = remark
      if (status !== undefined) supplier.status = status
      supplier.updateTime = formatNow()

      suppliers[supplierIndex] = supplier

      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },

  // 删除供应商
  {
    url: '/api/supplier/delete',
    method: 'post',
    response: ({ body }: { body: { id: string } }) => {
      const { id } = body

      // 检查是否有关联商品
      const hasProducts = productSuppliers.some(ps => ps.supplierId === id)
      if (hasProducts) {
        return { code: 400, message: '该供应商关联了商品，无法删除', data: null }
      }

      const index = suppliers.findIndex(s => s.id === id)
      if (index !== -1) {
        suppliers.splice(index, 1)
      }

      return { code: 200, message: '删除成功', data: { success: true } }
    },
  },

  // ==================== 商品-供应商关联 API ====================

  // 获取商品的供应商列表
  {
    url: '/api/product/supplier/list',
    method: 'get',
    response: ({ query }: { query: { productId: string } }) => {
      const { productId } = query

      if (!productId) {
        return { code: 400, message: '商品ID不能为空', data: null }
      }

      const list = productSuppliers
        .filter(ps => ps.productId === productId)
        .map(ps => ({
          ...ps,
          supplier: suppliers.find(s => s.id === ps.supplierId),
        }))

      return { code: 200, message: '成功', data: list }
    },
  },

  // 绑定商品供应商
  {
    url: '/api/product/supplier/bind',
    method: 'post',
    response: ({ body }: { body: { productId: string; supplierId: string; skuCode: string; purchasePrice: number; minOrderQty?: number; isDefault?: boolean; remark?: string } }) => {
      const { productId, supplierId, skuCode, purchasePrice, minOrderQty, isDefault, remark } = body

      if (!productId || !supplierId) {
        return { code: 400, message: '商品ID和供应商ID不能为空', data: null }
      }

      if (!skuCode) {
        return { code: 400, message: 'SKU码不能为空', data: null }
      }

      // 检查是否已绑定
      if (productSuppliers.some(ps => ps.productId === productId && ps.supplierId === supplierId)) {
        return { code: 400, message: '该供应商已绑定', data: null }
      }

      // 检查SKU码是否已存在
      if (productSuppliers.some(ps => ps.skuCode === skuCode)) {
        return { code: 400, message: 'SKU码已存在', data: null }
      }

      // 如果设置为默认，取消其他默认
      if (isDefault) {
        productSuppliers.forEach(ps => {
          if (ps.productId === productId) {
            ps.isDefault = false
          }
        })
      }

      const newProductSupplier: MockProductSupplier = {
        id: generateGuid(),
        productId,
        supplierId,
        skuCode,
        purchasePrice,
        minOrderQty: minOrderQty || 1,
        isDefault: isDefault ?? productSuppliers.filter(ps => ps.productId === productId).length === 0,
        remark: remark || '',
      }

      productSuppliers.push(newProductSupplier)

      return { code: 200, message: '绑定成功', data: { id: newProductSupplier.id } }
    },
  },

  // 解绑商品供应商
  {
    url: '/api/product/supplier/unbind',
    method: 'post',
    response: ({ body }: { body: { id: string } }) => {
      const { id } = body

      const index = productSuppliers.findIndex(ps => ps.id === id)
      if (index !== -1) {
        productSuppliers.splice(index, 1)
      }

      return { code: 200, message: '解绑成功', data: { success: true } }
    },
  },

  // 设置默认供应商
  {
    url: '/api/product/supplier/default',
    method: 'post',
    response: ({ body }: { body: { id: string } }) => {
      const { id } = body

      const ps = productSuppliers.find(ps => ps.id === id)
      if (!ps) {
        return { code: 404, message: '绑定关系不存在', data: null }
      }

      // 取消其他默认
      productSuppliers.forEach(p => {
        if (p.productId === ps.productId) {
          p.isDefault = p.id === id
        }
      })

      return { code: 200, message: '设置成功', data: { success: true } }
    },
  },

  // 获取供应商的商品列表
  {
    url: '/api/supplier/products',
    method: 'get',
    response: ({ query }: { query: { supplierId: string } }) => {
      const { supplierId } = query

      if (!supplierId) {
        return { code: 400, message: '供应商ID不能为空', data: null }
      }

      const list = productSuppliers
        .filter(ps => ps.supplierId === supplierId)
        .map(ps => ({
          ...ps,
          product: {
            ...products.find(p => p.id === ps.productId),
          },
        }))

      return { code: 200, message: '成功', data: list }
    },
  },

  // ==================== 商品评价 API ====================

  // 获取评价列表
  {
    url: /\/api\/product\/review\/list/,
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number | string; pageSize?: number | string; productId?: string; status?: string; rating?: number | string } }) => {
      const pageIndex = Number(query.pageIndex) || 1
      const pageSize = Number(query.pageSize) || 10
      const { productId, status, rating } = query

      // 生成模拟评价数据
      const mockReviews = [
        {
          id: 'rev-001',
          productId: '550e8400-e29b-41d4-a716-446655440001',
          productName: '红玫瑰鲜花束',
          productImage: '/static/images/products/rose-red.jpg',
          orderNo: 'ORD202401150001',
          userId: 'user-001',
          userName: '小明',
          userAvatar: '/static/images/avatars/user1.jpg',
          rating: 5,
          content: '花很新鲜，包装也很精美，女朋友很喜欢！',
          images: ['/static/images/reviews/rev1-1.jpg', '/static/images/reviews/rev1-2.jpg'],
          reply: null,
          replyTime: null,
          status: 'approved',
          isAnonymous: false,
          createTime: '2024-01-15 14:30:00',
        },
        {
          id: 'rev-002',
          productId: '550e8400-e29b-41d4-a716-446655440001',
          productName: '红玫瑰鲜花束',
          productImage: '/static/images/products/rose-red.jpg',
          orderNo: 'ORD202401140002',
          userId: 'user-002',
          userName: '花艺爱好者',
          userAvatar: '/static/images/avatars/user2.jpg',
          rating: 4,
          content: '整体不错，就是配送时间稍微长了一点。',
          images: [],
          reply: '感谢您的反馈，我们会努力改进配送服务！',
          replyTime: '2024-01-14 16:00:00',
          status: 'approved',
          isAnonymous: false,
          createTime: '2024-01-14 15:00:00',
        },
        {
          id: 'rev-003',
          productId: '550e8400-e29b-41d4-a716-446655440002',
          productName: '粉色康乃馨花束',
          productImage: '/static/images/products/carnation-pink.jpg',
          orderNo: 'ORD202401130003',
          userId: 'user-003',
          userName: '匿名用户',
          userAvatar: null,
          rating: 5,
          content: '送给妈妈的，她非常喜欢，花很新鲜！',
          images: ['/static/images/reviews/rev3-1.jpg'],
          reply: null,
          replyTime: null,
          status: 'pending',
          isAnonymous: true,
          createTime: '2024-01-13 10:00:00',
        },
      ]

      let filteredReviews = mockReviews.filter((review) => {
        let match = true
        if (productId && review.productId !== productId) {
          match = false
        }
        if (status && review.status !== status) {
          match = false
        }
        if (rating && review.rating !== Number(rating)) {
          match = false
        }
        return match
      })

      const total = filteredReviews.length
      const start = (pageIndex - 1) * pageSize
      const list = filteredReviews.slice(start, start + pageSize)

      return {
        code: 200,
        message: '成功',
        data: { list, total, pageIndex, pageSize },
      }
    },
  },

  // 获取评价详情
  {
    url: /\/api\/product\/review\/detail\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()

      const mockReview = {
        id,
        productId: '550e8400-e29b-41d4-a716-446655440001',
        productName: '红玫瑰鲜花束',
        productImage: '/static/images/products/rose-red.jpg',
        orderNo: 'ORD202401150001',
        userId: 'user-001',
        userName: '小明',
        userAvatar: '/static/images/avatars/user1.jpg',
        rating: 5,
        content: '花很新鲜，包装也很精美，女朋友很喜欢！',
        images: ['/static/images/reviews/rev1-1.jpg', '/static/images/reviews/rev1-2.jpg'],
        reply: null,
        replyTime: null,
        status: 'approved',
        isAnonymous: false,
        createTime: '2024-01-15 14:30:00',
      }

      return { code: 200, message: '成功', data: mockReview }
    },
  },

  // 回复评价
  {
    url: '/api/product/review/reply',
    method: 'post',
    response: ({ body }: { body: { id: string; reply: string } }) => {
      const { id, reply } = body

      if (!id || !reply) {
        return { code: 400, message: '参数错误', data: null }
      }

      return { code: 200, message: '回复成功', data: 1 }
    },
  },

  // 审核评价
  {
    url: '/api/product/review/audit',
    method: 'put',
    response: ({ body }: { body: { id: string; status: string } }) => {
      const { id, status } = body

      if (!id || !status) {
        return { code: 400, message: '参数错误', data: null }
      }

      return { code: 200, message: '审核成功', data: 1 }
    },
  },

  // 隐藏评价
  {
    url: /\/api\/product\/review\/hide\/[\w-]+/,
    method: 'put',
    response: () => {
      return { code: 200, message: '隐藏成功', data: 1 }
    },
  },

  // 获取评价统计
  {
    url: '/api/product/review/stats',
    method: 'get',
    response: ({ query }: { query: { productId?: string } }) => {
      const { productId } = query

      return {
        code: 200,
        message: '成功',
        data: {
          totalCount: 156,
          avgRating: 4.5,
          fiveStarCount: 89,
          fourStarCount: 42,
          threeStarCount: 18,
          twoStarCount: 5,
          oneStarCount: 2,
          pendingCount: 12,
          repliedCount: 68,
        },
      }
    },
  },

  // ==================== 商品统计 API ====================

  // 获取销量统计
  {
    url: '/api/product/stats/sales',
    method: 'get',
    response: ({ query }: { query: { startDate?: string; endDate?: string; top?: number | string } }) => {
      const top = Number(query.top) || 10

      const salesStats = products
        .sort((a, b) => b.sales - a.sales)
        .slice(0, top)
        .map((p) => ({
          productId: p.id,
          productName: p.name,
          productImage: p.image,
          skuCode: p.skuCode,
          salesCount: p.sales,
          salesAmount: p.sales * p.price,
          orderCount: Math.floor(p.sales / 2),
          avgPrice: p.price,
        }))

      return { code: 200, message: '成功', data: salesStats }
    },
  },

  // 获取销量趋势
  {
    url: '/api/product/stats/trend',
    method: 'get',
    response: ({ query }: { query: { startDate?: string; endDate?: string } }) => {
      const trend = []
      for (let i = 6; i >= 0; i--) {
        const date = new Date()
        date.setDate(date.getDate() - i)
        trend.push({
          date: date.toISOString().split('T')[0],
          salesCount: Math.floor(Math.random() * 100) + 50,
          salesAmount: Math.floor(Math.random() * 10000) + 5000,
          orderCount: Math.floor(Math.random() * 50) + 20,
        })
      }

      return { code: 200, message: '成功', data: trend }
    },
  },

  // 获取库存统计
  {
    url: '/api/product/stats/stock',
    method: 'get',
    response: () => {
      const totalProducts = products.length
      const totalStock = products.reduce((sum, p) => sum + p.stock, 0)
      const lowStockCount = products.filter((p) => p.stock <= p.alertThreshold && p.stock > 0).length
      const outOfStockCount = products.filter((p) => p.stock === 0).length
      const totalValue = products.reduce((sum, p) => sum + p.stock * p.price, 0)

      return {
        code: 200,
        message: '成功',
        data: {
          totalProducts,
          totalStock,
          lowStockCount,
          outOfStockCount,
          totalValue,
        },
      }
    },
  },

  // 获取库存预警列表
  {
    url: '/api/product/stock/alert',
    method: 'get',
    response: () => {
      const lowStockProducts = products
        .filter((p) => p.stock <= p.alertThreshold)
        .map((p) => ({
          productId: p.id,
          skuCode: p.skuCode,
          productName: p.name,
          productImage: p.image,
          categoryName: categories.find((c) => c.id === p.categoryId)?.name || '',
          stock: p.stock,
          alertThreshold: p.alertThreshold,
          isLowStock: true,
        }))

      return { code: 200, message: '成功', data: lowStockProducts }
    },
  },

  // 获取概览统计
  {
    url: '/api/product/stats/overview',
    method: 'get',
    response: () => {
      const totalSales = products.reduce((sum, p) => sum + p.sales, 0)
      const totalAmount = products.reduce((sum, p) => sum + p.sales * p.price, 0)
      const todaySales = Math.floor(totalSales * 0.1)
      const todayAmount = Math.floor(totalAmount * 0.1)

      return {
        code: 200,
        message: '成功',
        data: {
          today: {
            salesCount: todaySales,
            salesAmount: todayAmount,
            orderCount: Math.floor(todaySales / 2),
          },
          month: {
            salesCount: totalSales,
            salesAmount: totalAmount,
            orderCount: Math.floor(totalSales / 2),
          },
          growth: {
            salesCountGrowth: Math.floor(Math.random() * 20) - 10,
            salesAmountGrowth: Math.floor(Math.random() * 20) - 10,
            orderCountGrowth: Math.floor(Math.random() * 20) - 10,
          },
          stock: {
            totalProducts: products.length,
            lowStockCount: products.filter((p) => p.stock <= p.alertThreshold && p.stock > 0).length,
            outOfStockCount: products.filter((p) => p.stock === 0).length,
          },
        },
      }
    },
  },

  // 获取销量排行
  {
    url: '/api/product/stats/ranking',
    method: 'get',
    response: ({ query }: { query: { limit?: number | string; sortBy?: string } }) => {
      const limit = Number(query.limit) || 10
      const sortBy = query.sortBy || 'sales'

      const ranking = products
        .sort((a, b) => sortBy === 'amount' ? (b.sales * b.price) - (a.sales * a.price) : b.sales - a.sales)
        .slice(0, limit)
        .map((p) => ({
          productId: p.id,
          productName: p.name,
          productImage: p.image,
          skuCode: p.skuCode,
          salesCount: p.sales,
          salesAmount: p.sales * p.price,
        }))

      return { code: 200, message: '成功', data: ranking }
    },
  },

  // 获取分类销量统计
  {
    url: '/api/product/stats/category',
    method: 'get',
    response: () => {
      const categoryStats = categories.map((cat) => {
        const categoryProducts = products.filter((p) => p.categoryId === cat.id)
        const salesCount = categoryProducts.reduce((sum, p) => sum + p.sales, 0)
        const salesAmount = categoryProducts.reduce((sum, p) => sum + p.sales * p.price, 0)

        return {
          categoryId: cat.id,
          categoryName: cat.name,
          salesCount,
          salesAmount,
        }
      })

      return { code: 200, message: '成功', data: categoryStats }
    },
  },

  // 获取评价统计（统计报表用）
  {
    url: '/api/product/review/statistics',
    method: 'get',
    response: () => {
      return {
        code: 200,
        message: '成功',
        data: {
          total: 156,
          averageRating: 4.5,
          ratingDistribution: [
            { rating: 5, count: 89 },
            { rating: 4, count: 42 },
            { rating: 3, count: 18 },
            { rating: 2, count: 5 },
            { rating: 1, count: 2 },
          ],
          pendingCount: 12,
          approvedCount: 130,
          rejectedCount: 14,
        },
      }
    },
  },
] as MockMethod[]