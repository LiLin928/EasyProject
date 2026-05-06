// stores/order.store.ts

import { BaseStore } from './base.store';
import { IOrder, OrderStatus } from '../types/index';
import { StorageUtil } from '../utils/storage';

interface IOrderState {
  orders: IOrder[];
  currentOrder: IOrder | null;
  loading: boolean;
}

export class OrderStore extends BaseStore<IOrderState> {
  private static instance: OrderStore;

  static getInstance(): OrderStore {
    if (!OrderStore.instance) {
      OrderStore.instance = new OrderStore();
    }
    return OrderStore.instance;
  }

  protected getInitialState(): IOrderState {
    const orders = StorageUtil.get<IOrder[]>('orderList') || [];
    return {
      orders,
      currentOrder: null,
      loading: false,
    };
  }

  /** 设置订单列表 */
  setOrders(orders: IOrder[]): void {
    StorageUtil.set('orderList', orders);
    this.setState({ orders });
  }

  /** 添加订单 */
  addOrder(order: IOrder): void {
    const orders = [...this.state.orders, order];
    StorageUtil.set('orderList', orders);
    this.setState({ orders, currentOrder: order });
  }

  /** 更新订单状态 */
  updateOrderStatus(orderId: string, status: OrderStatus): void {
    const orders = this.state.orders.map(order =>
      order.id === orderId ? { ...order, status, updatedAt: Date.now() } : order
    );
    StorageUtil.set('orderList', orders);
    this.setState({ orders });
  }

  /** 设置当前查看的订单 */
  setCurrentOrder(order: IOrder | null): void {
    this.setState({ currentOrder: order });
  }

  /** 设置加载状态 */
  setLoading(loading: boolean): void {
    this.setState({ loading });
  }

  /** 按状态筛选订单 */
  getOrdersByStatus(status?: OrderStatus): IOrder[] {
    if (!status) return this.state.orders;
    return this.state.orders.filter(order => order.status === status);
  }
}

/** 导出单例实例 */
export const orderStore = OrderStore.getInstance();