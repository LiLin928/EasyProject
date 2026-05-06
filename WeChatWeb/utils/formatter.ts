// utils/formatter.ts

import { envConfig } from '../config/env.dev';

/** 格式化工具类 */
export class Formatter {

  /** 格式化价格（保留两位小数） */
  static price(value: number): string {
    return (value / 100).toFixed(2); // 假设价格单位为分
  }

  /** 格式化价格显示（带¥符号） */
  static priceDisplay(value: number): string {
    return '¥' + this.price(value);
  }

  /** 格式化图片 URL */
  static imageUrl(path: string | undefined | null): string {
    if (!path) {
      return '';
    }

    // 如果已经是完整 URL（http/https），直接返回
    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }

    // 如果是本地静态资源（/static/），直接返回
    if (path.startsWith('/static/')) {
      return path;
    }

    // 其他路径，拼接为文件服务器 API 地址
    // 去掉开头的斜杠
    const cleanPath = path.startsWith('/') ? path.slice(1) : path;
    return `${envConfig.fileBaseUrl}/image?path=${cleanPath}`;
  }

  /** 格式化多个图片 URL */
  static imageUrls(paths: string[] | undefined | null): string[] {
    if (!paths || !Array.isArray(paths)) {
      return [];
    }
    return paths.map(p => this.imageUrl(p));
  }

  /** 格式化时间戳为日期 */
  static date(timestamp: number, format: string = 'YYYY-MM-DD'): string {
    const date = new Date(timestamp);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hour = String(date.getHours()).padStart(2, '0');
    const minute = String(date.getMinutes()).padStart(2, '0');
    const second = String(date.getSeconds()).padStart(2, '0');

    return format
      .replace('YYYY', String(year))
      .replace('MM', month)
      .replace('DD', day)
      .replace('HH', hour)
      .replace('mm', minute)
      .replace('ss', second);
  }

  /** 格式化时间戳为友好显示 */
  static timeAgo(timestamp: number): string {
    const now = Date.now();
    const diff = now - timestamp;

    const minute = 60 * 1000;
    const hour = 60 * minute;
    const day = 24 * hour;
    const week = 7 * day;
    const month = 30 * day;

    if (diff < minute) return '刚刚';
    if (diff < hour) return Math.floor(diff / minute) + '分钟前';
    if (diff < day) return Math.floor(diff / hour) + '小时前';
    if (diff < week) return Math.floor(diff / day) + '天前';
    if (diff < month) return Math.floor(diff / week) + '周前';

    return this.date(timestamp, 'YYYY-MM-DD');
  }

  /** 格式化手机号（隐藏中间四位） */
  static phone(phone: string): string {
    if (phone.length === 11) {
      return phone.replace(/(\d{3})\d{4}(\d{4})/, '$1****$2');
    }
    return phone;
  }

  /** 格式化订单编号 */
  static orderNo(orderNo: string): string {
    if (orderNo.length > 10) {
      return orderNo.slice(0, 4) + '...' + orderNo.slice(-4);
    }
    return orderNo;
  }
}