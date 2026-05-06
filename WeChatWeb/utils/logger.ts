// utils/logger.ts

/** 日志级别 */
type LogLevel = 'debug' | 'info' | 'warn' | 'error';

/** 日志工具类 */
export class Logger {
  private static level: LogLevel = 'info'; // 当前日志级别
  private static enabled: boolean = true; // 是否启用日志

  /** 设置日志级别 */
  static setLevel(level: LogLevel): void {
    this.level = level;
  }

  /** 启用/禁用日志 */
  static enable(enabled: boolean): void {
    this.enabled = enabled;
  }

  /** Debug 日志 */
  static debug(message: string, ...args: any[]): void {
    if (this.enabled && this.shouldLog('debug')) {
      console.log(`[DEBUG] ${message}`, ...args);
    }
  }

  /** Info 日志 */
  static info(message: string, ...args: any[]): void {
    if (this.enabled && this.shouldLog('info')) {
      console.log(`[INFO] ${message}`, ...args);
    }
  }

  /** Warn 日志 */
  static warn(message: string, ...args: any[]): void {
    if (this.enabled && this.shouldLog('warn')) {
      console.warn(`[WARN] ${message}`, ...args);
    }
  }

  /** Error 日志 */
  static error(message: string, ...args: any[]): void {
    if (this.enabled && this.shouldLog('error')) {
      console.error(`[ERROR] ${message}`, ...args);
    }
  }

  /** 判断是否应该输出日志 */
  private static shouldLog(level: LogLevel): boolean {
    const levels: LogLevel[] = ['debug', 'info', 'warn', 'error'];
    return levels.indexOf(level) >= levels.indexOf(this.level);
  }

  /** 记录 API 请求 */
  static apiRequest(url: string, method: string, data?: any): void {
    this.info(`API ${method} ${url}`, data ? { data } : '');
  }

  /** 记录 API 响应 */
  static apiResponse(url: string, response: any): void {
    this.debug(`API Response ${url}`, response);
  }

  /** 记录 API 错误 */
  static apiError(url: string, error: any): void {
    this.error(`API Error ${url}`, error);
  }
}