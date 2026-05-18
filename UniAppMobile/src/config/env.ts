// config/env.ts

/** 环境类型 */
type EnvType = 'development' | 'production'

/** 当前环境 */
export const ENV: EnvType = process.env.NODE_ENV === 'production' ? 'production' : 'development'

/** API 基础地址 - 从 Vite 环境变量读取 */
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || ''

/** 应用标题 */
export const APP_TITLE = import.meta.env.VITE_APP_TITLE || 'EasyProject'

/** 是否启用 Mock */
export const MOCK_ENABLED = import.meta.env.VITE_MOCK_ENABLED === 'true'

/** Token 过期时间（秒） */
export const TOKEN_EXPIRE_TIME = 60 * 60 * 24  // 24小时

/** 存储前缀 */
export const STORAGE_PREFIX = 'EP_MOBILE_'