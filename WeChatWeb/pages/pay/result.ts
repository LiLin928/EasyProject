// pages/pay/result.ts

import { paymentService, orderService } from '../../services/index';
import { IOrder } from '../../types/index';

interface IResultPageData {
  orderId: string;
  order: IOrder | null;
  status: 'pending' | 'success' | 'failed';
  loading: boolean;
}

Page<IResultPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    orderId: '',
    order: null,
    status: 'pending',
    loading: true,
  },

  async onLoad(options: { orderId: string }) {
    const orderId = options.orderId;
    this.setData({ orderId });

    await this.loadOrder(orderId);
  },

  async loadOrder(orderId: string) {
    try {
      const order = await orderService.getOrderDetail(orderId);
      this.setData({ order, status: 'pending', loading: false });
    } catch (error) {
      console.error('加载订单失败:', error);
      this.setData({ status: 'failed', loading: false });
    }
  },

  // 立即支付
  async onPayNow() {
    const { orderId, loading } = this.data;
    if (loading) return;

    this.setData({ loading: true });

    try {
      const result = await paymentService.createPayment(orderId);
      if (result.success) {
        this.setData({ status: 'success' });
      }
    } catch (error) {
      console.error('支付失败:', error);
      wx.showToast({ title: '支付失败', icon: 'none' });
    } finally {
      this.setData({ loading: false });
    }
  },

  // 查看订单
  onViewOrder() {
    const { orderId } = this.data;
    wx.redirectTo({ url: `/pages/order/detail?id=${orderId}` });
  },

  // 返回首页
  onBackHome() {
    wx.switchTab({ url: '/pages/index/index' });
  },
});