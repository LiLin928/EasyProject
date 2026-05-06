// pages/order/list.ts

import { orderService, authService } from '../../services/index';
import { orderStore } from '../../stores/index';
import { IOrder, OrderStatus, OrderStatusEnum, getOrderStatusText } from '../../types/index';

interface IListPageData {
  orders: IOrder[];
  loading: boolean;
  currentTab: number;
  tabs: { key: string; name: string; status?: OrderStatus }[];
}

Page<IListPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    orders: [],
    loading: true,
    currentTab: 0,
    tabs: [
      { key: 'all', name: '全部' },
      { key: 'pending', name: '待付款', status: OrderStatusEnum.Pending },
      { key: 'paid', name: '待发货', status: OrderStatusEnum.Paid },
      { key: 'completed', name: '已完成', status: OrderStatusEnum.Completed },
    ],
  },

  unsubscribe: null as (() => void) | null,

  onLoad() {
    if (!authService.requireLogin()) return;

    this.unsubscribe = orderStore.subscribe(() => {
      this.filterOrders();
    });
  },

  onShow() {
    this.loadOrders();
  },

  onUnload() {
    if (this.unsubscribe) this.unsubscribe();
  },

  async loadOrders() {
    this.setData({ loading: true });
    await orderService.initOrders();
    this.filterOrders();
    this.setData({ loading: false });
  },

  // 筛选订单
  filterOrders() {
    const { currentTab, tabs } = this.data;
    const tab = tabs[currentTab];
    let orders = orderStore.getState().orders;

    // 添加状态文本
    orders = orders.map(order => ({
      ...order,
      statusText: getOrderStatusText(order.status),
    }));

    // 按状态筛选
    if (tab.status) {
      orders = orders.filter(order => order.status === tab.status);
    }

    this.setData({ orders });
  },

  // 切换 Tab
  onTabChange(e: WechatMiniprogram.TouchEvent) {
    const { index } = e.currentTarget.dataset;
    this.setData({ currentTab: index });
    this.filterOrders();
  },

  // 查看订单详情
  onViewDetail(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    wx.navigateTo({ url: `/pages/order/detail?id=${id}` });
  },

  // 取消订单
  async onCancelOrder(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    wx.showModal({
      title: '提示',
      content: '确定要取消这个订单吗？',
      success: async (res) => {
        if (res.confirm) {
          await orderService.cancelOrder(id);
          this.loadOrders();
        }
      },
    });
  },

  // 去支付
  async onPayOrder(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    wx.redirectTo({ url: `/pages/pay/result?orderId=${id}` });
  },

  // 下拉刷新
  onPullDownRefresh() {
    this.loadOrders().then(() => wx.stopPullDownRefresh());
  },
});