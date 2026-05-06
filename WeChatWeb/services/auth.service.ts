// services/auth.service.ts

import { adapter } from '../adapters/adapter.config';
import { userStore } from '../stores/index';
import { IUser, IUserInfo } from '../types/index';

export class AuthService {
  private static instance: AuthService;

  static getInstance(): AuthService {
    if (!AuthService.instance) {
      AuthService.instance = new AuthService();
    }
    return AuthService.instance;
  }

  /** 微信登录 */
  async wxLogin(): Promise<IUser> {
    try {
      const { code } = await wx.login();
      const user = await adapter.wxLogin(code);
      userStore.setUser(user);
      return user;
    } catch (error) {
      console.error('微信登录失败:', error);
      throw error;
    }
  }

  /** 手机号密码登录 */
  async phoneLogin(phone: string, password: string): Promise<IUser> {
    try {
      const user = await adapter.phoneLogin({ phone, password });
      userStore.setUser(user);
      return user;
    } catch (error) {
      console.error('手机号登录失败:', error);
      throw error;
    }
  }

  /** 微信手机号快速验证登录 */
  async wxPhoneLogin(encryptedData: string, iv: string): Promise<IUser> {
    try {
      const { code } = await wx.login();
      const user = await adapter.wxPhoneLogin({ code, encryptedData, iv });
      userStore.setUser(user);
      return user;
    } catch (error) {
      console.error('微信手机号登录失败:', error);
      throw error;
    }
  }

  /** 获取用户信息 */
  async getUserInfo(): Promise<IUserInfo> {
    const userInfo = await adapter.getUserInfo();
    userStore.updateUserInfo(userInfo);
    return userInfo;
  }

  /** 更新用户信息 */
  async updateUserInfo(data: Partial<IUserInfo>): Promise<IUserInfo> {
    const userInfo = await adapter.updateUserInfo(data);
    userStore.updateUserInfo(userInfo);
    return userInfo;
  }

  /** 检查登录状态 */
  checkLogin(): boolean {
    return userStore.getState().isLoggedIn;
  }

  /** 退出登录 */
  logout(): void {
    userStore.logout();
    wx.switchTab({ url: '/pages/index/index' });
  }

  /** 登录拦截（未登录跳转） */
  requireLogin(): boolean {
    if (!this.checkLogin()) {
      wx.showModal({
        title: '提示',
        content: '请先登录后再操作',
        confirmText: '去登录',
        success: (res) => {
          if (res.confirm) {
            wx.navigateTo({ url: '/pages/login/login' });
          }
        },
      });
      return false;
    }
    return true;
  }
}

export const authService = AuthService.getInstance();