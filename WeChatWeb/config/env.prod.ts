// config/env.prod.ts

/** 生产环境配置 */
export const envConfig = {
  /** 环境标识 */
  env: 'prod',

  /** API 基础地址 */
  baseUrl: 'https://api.example.com/api/wechat',

  /** 请求超时时间（毫秒） */
  timeout: 30000,

  /** Token 存储键名 */
  tokenKey: 'token',

  /** 用户信息存储键名 */
  userInfoKey: 'userInfo',
};