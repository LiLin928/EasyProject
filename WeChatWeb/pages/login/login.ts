// pages/login/login.ts

import { authService } from '../../services/index';

interface ILoginPageData {
  activeTab: 'quick' | 'password';
  phone: string;
  password: string;
  loading: boolean;
}

Page<ILoginPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    activeTab: 'quick',
    phone: '',
    password: '',
    loading: false,
  },

  onLoad() {
    const token = wx.getStorageSync('wechat_mall_token');
    if (token) {
      console.log('登录页：已有 token，返回首页');
      wx.switchTab({ url: '/pages/index/index' });
    }
  },

  onTabChange(e: WechatMiniprogram.TouchEvent) {
    const { tab } = e.currentTarget.dataset;
    this.setData({ activeTab: tab as 'quick' | 'password' });
  },

  async onGetPhoneNumber(e: WechatMiniprogram.TouchEvent) {
    if (this.data.loading) return;

    if (e.detail.errMsg !== 'getPhoneNumber:ok') {
      wx.showToast({ title: '请授权手机号以完成登录', icon: 'none' });
      return;
    }

    this.setData({ loading: true });

    try {
      await authService.wxPhoneLogin(e.detail.encryptedData, e.detail.iv);
      wx.showToast({ title: '登录成功', icon: 'success' });

      setTimeout(() => {
        wx.switchTab({ url: '/pages/index/index' });
      }, 1500);
    } catch (error) {
      console.error('快捷登录失败:', error);
      wx.showToast({ title: '登录失败', icon: 'none' });
    } finally {
      this.setData({ loading: false });
    }
  },

  async onPasswordLogin() {
    if (this.data.loading) return;

    const { phone, password } = this.data;

    if (!/^1[3-9]\d{9}$/.test(phone)) {
      wx.showToast({ title: '请输入正确的手机号', icon: 'none' });
      return;
    }

    if (!password) {
      wx.showToast({ title: '请输入密码', icon: 'none' });
      return;
    }

    this.setData({ loading: true });

    try {
      await authService.phoneLogin(phone, password);
      wx.showToast({ title: '登录成功', icon: 'success' });

      setTimeout(() => {
        wx.switchTab({ url: '/pages/index/index' });
      }, 1500);
    } catch (error) {
      console.error('密码登录失败:', error);
      wx.showToast({ title: '手机号或密码错误', icon: 'none' });
    } finally {
      this.setData({ loading: false });
    }
  },

  onPhoneInput(e: WechatMiniprogram.Input) {
    this.setData({ phone: e.detail.value });
  },

  onPasswordInput(e: WechatMiniprogram.Input) {
    this.setData({ password: e.detail.value });
  },
});