// utils/storage.ts

/** 本地存储工具类 */
export class StorageUtil {
  private static prefix = 'wechat_mall_'; // 存储前缀

  /** 设置存储 */
  static set<T>(key: string, value: T): void {
    const fullKey = this.prefix + key;
    try {
      wx.setStorageSync(fullKey, JSON.stringify(value));
    } catch (error) {
      console.error('Storage set error:', error);
    }
  }

  /** 获取存储 */
  static get<T>(key: string): T | null {
    const fullKey = this.prefix + key;
    try {
      const value = wx.getStorageSync(fullKey);
      if (value) {
        return JSON.parse(value) as T;
      }
    } catch (error) {
      console.error('Storage get error:', error);
    }
    return null;
  }

  /** 删除存储 */
  static remove(key: string): void {
    const fullKey = this.prefix + key;
    try {
      wx.removeStorageSync(fullKey);
    } catch (error) {
      console.error('Storage remove error:', error);
    }
  }

  /** 清空所有存储 */
  static clear(): void {
    try {
      wx.clearStorageSync();
    } catch (error) {
      console.error('Storage clear error:', error);
    }
  }

  /** 获取所有存储键 */
  static keys(): string[] {
    try {
      const info = wx.getStorageInfoSync();
      return info.keys.filter(k => k.startsWith(this.prefix));
    } catch (error) {
      console.error('Storage keys error:', error);
      return [];
    }
  }

  /** 获取存储大小 */
  static size(): number {
    try {
      const info = wx.getStorageInfoSync();
      return info.currentSize;
    } catch (error) {
      return 0;
    }
  }
}