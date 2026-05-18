// utils/platform.ts

/** 平台类型 */
type Platform = 'mp-weixin' | 'h5' | 'app' | 'app-plus' | 'mp-alipay' | 'mp-baidu'

/** 获取当前平台 */
export function getPlatform(): Platform {
  // @ts-ignore
  return uni.getSystemInfoSync().platform || 'h5'
}

/** 是否微信小程序 */
export function isWeixin(): boolean {
  return getPlatform() === 'mp-weixin'
}

/** 是否 H5 */
export function isH5(): boolean {
  return getPlatform() === 'h5'
}

/** 是否 App */
export function isApp(): boolean {
  return getPlatform() === 'app' || getPlatform() === 'app-plus'
}

/** 平台工具集 */
export const platformUtils = {
  /** 扫码 */
  scanCode(): Promise<string> {
    return new Promise((resolve, reject) => {
      uni.scanCode({
        success: (res) => resolve(res.result),
        fail: (err) => reject(err),
      })
    })
  },

  /** 获取设备信息 */
  getDeviceInfo() {
    const info = uni.getSystemInfoSync()
    return {
      platform: info.platform,
      model: info.model,
      system: info.system,
      brand: info.brand,
    }
  },
}