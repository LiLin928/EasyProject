// utils/http.ts

import { ApiConfig } from '../config/api.config';
import { StorageUtil } from './storage';

interface RequestOptions {
  url: string;
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE';
  data?: any;
  header?: Record<string, string>;
  showLoading?: boolean;
  showError?: boolean;
}

interface HttpResponse<T = any> {
  code: number;
  message: string;
  data: T;
  timestamp: number;
}

interface HttpMethods {
  request<T>(options: RequestOptions): Promise<T>;
  get<T>(url: string, data?: any, options?: Partial<RequestOptions>): Promise<T>;
  post<T>(url: string, data?: any, options?: Partial<RequestOptions>): Promise<T>;
  put<T>(url: string, data?: any, options?: Partial<RequestOptions>): Promise<T>;
  delete<T>(url: string, data?: any, options?: Partial<RequestOptions>): Promise<T>;
}

/** HTTP 请求封装 */
export const Http: HttpMethods = {
  request<T>(options: RequestOptions): Promise<T> {
    const { url, method = 'GET', data, header = {}, showLoading = false, showError = true } = options;

    // 调试日志：显示 baseUrl 和完整请求 URL
    console.log('[HTTP] baseUrl:', ApiConfig.baseUrl, '| path:', url, '| full:', ApiConfig.baseUrl + url);

    const token = StorageUtil.get<string>(ApiConfig.tokenKey);
    if (token) {
      header['Authorization'] = `Bearer ${token}`;
    }
    header['Content-Type'] = 'application/json';

    if (showLoading) {
      wx.showLoading({ title: '加载中...', mask: true });
    }

    return new Promise((resolve, reject) => {
      wx.request({
        url: ApiConfig.baseUrl + url,
        method,
        data,
        header,
        timeout: ApiConfig.timeout,
        success: (res) => {
          if (showLoading) wx.hideLoading();

          const response = res.data as HttpResponse<T>;

          if (response.code === 200) {
            resolve(response.data);
            return;
          }

          if (response.code === 401) {
            StorageUtil.remove(ApiConfig.tokenKey);
            StorageUtil.remove(ApiConfig.userInfoKey);
            wx.navigateTo({ url: '/pages/login/login' });
            reject(new Error('登录已过期'));
            return;
          }

          const error = new Error(response.message || '请求失败');
          if (showError) {
            wx.showToast({ title: response.message || '请求失败', icon: 'none' });
          }
          reject(error);
        },
        fail: (err) => {
          if (showLoading) wx.hideLoading();
          if (showError) {
            wx.showToast({ title: '网络请求失败', icon: 'none' });
          }
          reject(new Error('网络请求失败'));
        },
      });
    });
  },

  get<T>(url: string, data?: any, options?: Partial<RequestOptions>): Promise<T> {
    return Http.request<T>({ url, method: 'GET', data, ...options });
  },

  post<T>(url: string, data?: any, options?: Partial<RequestOptions>): Promise<T> {
    return Http.request<T>({ url, method: 'POST', data, ...options });
  },

  put<T>(url: string, data?: any, options?: Partial<RequestOptions>): Promise<T> {
    return Http.request<T>({ url, method: 'PUT', data, ...options });
  },

  delete<T>(url: string, data?: any, options?: Partial<RequestOptions>): Promise<T> {
    return Http.request<T>({ url, method: 'DELETE', data, ...options });
  },
};