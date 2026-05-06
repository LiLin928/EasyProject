// config/env.dev.ts

/** 开发环境配置 */
export const envConfig = {
  /** 环境标识 */
  env: 'dev',

  /** API 基础地址 */
  baseUrl: 'http://localhost:7600/api/wechat',

  /** 文件服务器地址 */
  fileBaseUrl: 'http://localhost:7600/api/file',

  /** 请求超时时间（毫秒） */
  timeout: 30000,

  /** Token 存储键名 */
  tokenKey: 'token',

  /** 用户信息存储键名 */
  userInfoKey: 'userInfo',
};