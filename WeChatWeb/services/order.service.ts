// services/order.service.ts

import { adapter } from '../adapters/adapter.config';
import { orderStore, cartStore } from '../stores/index';
import { IOrder, ICreateOrderParams, IPageResult, IPageQuery, OrderStatus, OrderStatusEnum } from '../types/index';
import { authService } from './auth.service';

export class OrderService {
  private static instance: OrderService;

  static getInstance(): OrderService {
    if (!OrderService.instance) {
      OrderService.instance = new OrderService();
    }
    return OrderService.instance;
  }

  /** 初始化订单列表 */
  async initOrders(): Promise<void> {
    orderStore.setLoading(true);
    const result = await adapter.getOrderList({ pageIndex: 1, pageSize: 50 });
    orderStore.setOrders(result.list);
    orderStore.setLoading(false);
  }

  /** 创建订单 */
  async createOrder(addressId: string, remark?: string): Promise<IOrder> {
    if (!authService.requireLogin()) {
      throw new Error('请先登录');
    }

    const selectedItems = cartStore.getSelectedItems();
    if (selectedItems.length === 0) {
      wx.showToast({ title: '请选择商品', icon: 'none' });
      throw new Error('请选择商品');
    }

    // 获取地址
    const addresses = await adapter.getAddressList();
    const address = addresses.find(a => a.id === addressId);
    if (!address) {
      wx.showToast({ title: '请选择收货地址', icon: 'none' });
      throw new Error('请选择收货地址');
    }

    const params: ICreateOrderParams = {
      cartItemIds: selectedItems.map(item => item.id),
      addressId,
      remark,
    };

    const order = await adapter.createOrder(params);
    orderStore.addOrder(order);

    // 清空已选购物车
    cartStore.clearSelected();

    return order;
  }

  /** 获取订单列表 */
  async getOrderList(params: IPageQuery): Promise<IPageResult<IOrder>> {
    orderStore.setLoading(true);
    const result = await adapter.getOrderList(params);
    orderStore.setOrders(result.list);
    orderStore.setLoading(false);
    return result;
  }

  /** 获取订单详情 */
  async getOrderDetail(id: string): Promise<IOrder> {
    const order = await adapter.getOrderDetail(id);
    orderStore.setCurrentOrder(order);
    return order;
  }

  /** 取消订单 */
  async cancelOrder(id: string): Promise<IOrder> {
    const order = await adapter.cancelOrder(id);
    orderStore.updateOrderStatus(id, OrderStatusEnum.Cancelled);
    wx.showToast({ title: '订单已取消', icon: 'success' });
    return order;
  }

  /** 按状态获取订单 */
  getOrdersByStatus(status?: OrderStatus): IOrder[] {
    return orderStore.getOrdersByStatus(status);
  }

  /** 刷新订单列表 */
  async refreshOrders(): Promise<void> {
    await this.initOrders();
  }
}

export const orderService = OrderService.getInstance();