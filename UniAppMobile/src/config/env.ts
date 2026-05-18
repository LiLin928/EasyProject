// config/env.ts

/** 环境类型 */
type EnvType = 'development' | 'production'

/** 当前环境 */
export const ENV: EnvType = process.env.NODE_ENV === 'production' ? 'production' : 'development'

/** API 基础地址 */
export const API_BASE_URL = ENV === 'production'
  ? 'https://api.yourdomain.com'
  : ''  // 开发环境使用 proxy

/** Token 过期时间（秒） */
export const TOKEN_EXPIRE_TIME = 60 * 60 * 24  // 24小时

/** 存储前缀 */
export const STORAGE_PREFIX = 'EP_MOBILE_'