// src/mock/banner.ts

import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

/**
 * Mock 轮播图类型
 */
interface MockBanner {
  id: string
  image: string
  linkType: 'none' | 'product' | 'category' | 'page'
  linkValue: string
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

// 初始轮播图数据
const banners: MockBanner[] = [
  {
    id: '11111111-aaaa-bbbb-cccc-dddddddddddd',
    image: '/static/images/banners/banner1.jpg',
    linkType: 'none',
    linkValue: '',
    sort: 1,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '22222222-aaaa-bbbb-cccc-dddddddddddd',
    image: '/static/images/banners/banner2.jpg',
    linkType: 'none',
    linkValue: '',
    sort: 2,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
  {
    id: '33333333-aaaa-bbbb-cccc-dddddddddddd',
    image: '/static/images/banners/banner3.jpg',
    linkType: 'none',
    linkValue: '',
    sort: 3,
    status: 1,
    createTime: '2024-01-01 10:00:00',
    updateTime: '2024-01-01 10:00:00',
  },
]

export default [
  // 获取轮播图列表（分页）
  {
    url: '/api/banner/list',
    method: 'get',
    response: ({ query }: { query: { pageIndex?: number; pageSize?: number; status?: 0 | 1 } }) => {
      const { pageIndex = 1, pageSize = 10, status } = query
      let filtered = banners
      if (status !== undefined) {
        filtered = filtered.filter((b) => b.status === status)
      }
      filtered.sort((a, b) => a.sort - b.sort)
      const total = filtered.length
      const start = (pageIndex - 1) * pageSize
      const list = filtered.slice(start, start + pageSize)
      return { code: 200, message: '成功', data: { list, total } }
    },
  },
  // 获取所有启用的轮播图
  {
    url: '/api/banner/all',
    method: 'get',
    response: () => {
      const activeBanners = banners.filter((b) => b.status === 1).sort((a, b) => a.sort - b.sort)
      return { code: 200, message: '成功', data: activeBanners }
    },
  },
  // 获取轮播图详情（路径参数）
  {
    url: /\/api\/banner\/detail\/[\w-]+/,
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const banner = banners.find((b) => b.id === id)
      if (!banner) {
        return { code: 404, message: '轮播图不存在', data: null }
      }
      return { code: 200, message: '成功', data: banner }
    },
  },
  // 创建轮播图
  {
    url: '/api/banner/add',
    method: 'post',
    response: ({ body }: { body: { image: string; linkType: string; linkValue: string; sort?: number; status?: 0 | 1 } }) => {
      const { image, linkType, linkValue, sort, status } = body
      if (!image) {
        return { code: 400, message: '图片地址不能为空', data: null }
      }
      const now = formatNow()
      const newBanner: MockBanner = {
        id: generateGuid(),
        image,
        linkType: linkType || 'none',
        linkValue: linkValue || '',
        sort: sort ?? banners.length + 1,
        status: status ?? 1,
        createTime: now,
        updateTime: now,
      }
      banners.push(newBanner)
      return { code: 200, message: '创建成功', data: { id: newBanner.id } }
    },
  },
  // 更新轮播图（PUT方法）
  {
    url: '/api/banner/update',
    method: 'put',
    response: ({ body }: { body: { id: string; image?: string; linkType?: string; linkValue?: string; sort?: number; status?: 0 | 1 } }) => {
      const { id, image, linkType, linkValue, sort, status } = body
      const banner = banners.find((b) => b.id === id)
      if (!banner) {
        return { code: 404, message: '轮播图不存在', data: null }
      }
      if (image !== undefined) banner.image = image
      if (linkType !== undefined) banner.linkType = linkType as MockBanner['linkType']
      if (linkValue !== undefined) banner.linkValue = linkValue
      if (sort !== undefined) banner.sort = sort
      if (status !== undefined) banner.status = status
      banner.updateTime = formatNow()
      return { code: 200, message: '更新成功', data: { success: true } }
    },
  },
  // 删除轮播图（DELETE方法，路径参数）
  {
    url: /\/api\/banner\/delete\/[\w-]+/,
    method: 'delete',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const index = banners.findIndex((b) => b.id === id)
      if (index !== -1) {
        banners.splice(index, 1)
      }
      return { code: 200, message: '删除成功', data: 1 }
    },
  },
] as MockMethod[]