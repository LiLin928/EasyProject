// adapters/adapter.config.ts

import { IDataAdapter } from './adapter.interface';
import { MockAdapter } from './mock.adapter';
import { ApiAdapter } from './api.adapter';

/** 当前使用的适配器模式
 * true: 使用 Mock 数据（开发阶段）
 * false: 使用真实 API（上线前切换）
 */
const USE_MOCK = false;  // 使用真实 API 模式

// 调试日志：确认当前模式
console.log('[Adapter Config] USE_MOCK:', USE_MOCK, '| API模式:', !USE_MOCK ? '真实API' : 'Mock数据');

/** 获取当前适配器实例 */
export function getAdapter(): IDataAdapter {
  if (USE_MOCK) {
    console.log('[Adapter Config] 返回 MockAdapter');
    return new MockAdapter();
  }
  console.log('[Adapter Config] 返回 ApiAdapter');
  return new ApiAdapter();
}

/** 单例适配器实例 */
export const adapter = getAdapter();