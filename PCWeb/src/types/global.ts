// 全局类型声明

// 扩展 Window
declare global {
  interface Window {
    __APP_ENV__: string
  }
}

export {}