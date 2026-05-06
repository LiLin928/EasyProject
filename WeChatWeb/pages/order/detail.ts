// pages/order/detail.ts

import { orderService, paymentService } from '../../services/index';
import { IOrder } from '../../types/index';

interface IDetailPageData {
  order: IOrder | null;
  loading: boolean;
  statusText: string;
  statusColor: string;
}

Page<IDetailPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    order: null,
    loading: true,
    statusText: '',
    statusColor: '',
  },

  async onLoad(options: { id: string }) {
    await this.loadOrder(options.id);
  },

  async loadOrder(id: string) {
    this.setData({ loading: true });
    try {
      const order = await orderService.getOrderDetail(id);
      // 使用数字状态值匹配后端 OrderStatus
      const statusMap: Record<number, { text: string; color: string }> = {
        0: { text: '待付款', color: '#ff976a' },
        1: { text: '待发货', color: '#1989fa' },
        2: { text: '待收货', color: '#07c160' },
        3: { text: '已完成', color: '#07c160' },
        4: { text: '已取消', color: '#969799' },
        5: { text: '已退款', color: '#969799' },
      };
      const statusInfo = statusMap[order.status] || { text: '未知', color: '#969799' };

      this.setData({
        order,
        statusText: statusInfo.text,
        statusColor: statusInfo.color,
        loading: false,
      });
    } catch (error) {
      console.error('加载订单详情失败:', error);
      this.setData({ loading: false });
    }
  },

  // 复制订单号
  onCopyOrderNo() {
    const { order } = this.data;
    if (order) {
      wx.setClipboardData({
        data: order.orderNo,
        success: () => wx.showToast({ title: '已复制', icon: 'success' }),
      });
    }
  },

  // 取消订单
  async onCancelOrder() {
    const { order } = this.data;
    if (!order) return;

    wx.showModal({
      title: '提示',
      content: '确定要取消这个订单吗？',
      success: async (res) => {
        if (res.confirm) {
          await orderService.cancelOrder(order.id);
          this.loadOrder(order.id);
        }
      },
    });
  },

  // 去支付
  async onPayOrder() {
    const { order } = this.data;
    if (!order) return;

    wx.redirectTo({ url: `/pages/pay/result?orderId=${order.id}` });
  },
});