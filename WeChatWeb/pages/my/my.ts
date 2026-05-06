// pages/my/my.ts

import { authService, orderService } from '../../services/index';
import { userStore, orderStore } from '../../stores/index';
import { IUserInfo, OrderStatusEnum } from '../../types/index';

interface IMyPageData {
  userInfo: IUserInfo | null;
  isLoggedIn: boolean;
  orderCounts: { pending: number; paid: number; completed: number };
  chatUnread: number;
}

Page<IMyPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    userInfo: null,
    isLoggedIn: false,
    orderCounts: { pending: 0, paid: 0, completed: 0 },
    chatUnread: 0,
  },

  onShow() {
    this.loadUserInfo();
    this.loadOrderCounts();
  },

  loadUserInfo() {
    const state = userStore.getState();
    this.setData({
      userInfo: state.userInfo,
      isLoggedIn: state.isLoggedIn,
    });
  },

  async loadOrderCounts() {
    if (authService.checkLogin()) {
      await orderService.initOrders();
      const orders = orderStore.getState().orders;
      const counts = {
        pending: orders.filter(o => o.status === OrderStatusEnum.Pending).length,
        paid: orders.filter(o => o.status === OrderStatusEnum.Paid).length,
        completed: orders.filter(o => o.status === OrderStatusEnum.Completed).length,
      };
      this.setData({ orderCounts: counts });
    }
  },

  // 登录
  onLogin() {
    wx.navigateTo({ url: '/pages/login/login' });
  },

  // 退出登录
  onLogout() {
    wx.showModal({
      title: '提示',
      content: '确定要退出登录吗？',
      success: (res) => {
        if (res.confirm) {
          authService.logout();
          this.loadUserInfo();
        }
      },
    });
  },

  // 查看订单
  onViewOrder(e: WechatMiniprogram.TouchEvent) {
    const { status } = e.currentTarget.dataset;
    wx.navigateTo({ url: `/pages/order/list?status=${status || ''}` });
  },

  // 地址管理
  onAddressManage() {
    wx.navigateTo({ url: '/pages/address/list' });
  },

  // 客服聊天
  goToChat() {
    if (!authService.requireLogin()) {
      return;
    }
    wx.navigateTo({ url: '/pages/chat/chat' });
  },

  // 客服（旧方法保留）
  onContact() {
    wx.showModal({
      title: '客服电话',
      content: '400-123-4567',
      showCancel: false,
    });
  },
});