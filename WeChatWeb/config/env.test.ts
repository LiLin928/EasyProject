// config/env.test.ts

/** 测试环境配置 */
export const envConfig = {
  /** 环境标识 */
  env: 'test',

  /** API 基础地址 */
  baseUrl: 'https://test.example.com/api/wechat',

  /** 请求超时时间（毫秒） */
  timeout: 30000,

  /** Token 存储键名 */
  tokenKey: 'token',

  /** 用户信息存储键名 */
  userInfoKey: 'userInfo',
};