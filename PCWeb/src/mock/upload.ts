// src/mock/upload.ts
import { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'

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

// 模拟图片 URL 列表
const mockImages = [
  'https://picsum.photos/200/200?random=1',
  'https://picsum.photos/200/200?random=2',
  'https://picsum.photos/200/200?random=3',
  'https://picsum.photos/200/200?random=4',
  'https://picsum.photos/200/200?random=5',
  'https://picsum.photos/400/400?random=6',
  'https://picsum.photos/400/400?random=7',
]

export default [
  // 图片上传
  {
    url: '/api/upload/image',
    method: 'post',
    response: () => {
      // 模拟上传成功，返回随机图片 URL
      const randomIndex = Math.floor(Math.random() * mockImages.length)
      return {
        success: true,
        code: 200,
        message: '上传成功',
        data: {
          id: generateGuid(),
          url: mockImages[randomIndex],
          name: `image_${formatNow().replace(/[: ]/g, '_')}.jpg`,
          size: 1024 * 100, // 100KB
          createTime: formatNow(),
        },
      }
    },
  },
] as MockMethod[]