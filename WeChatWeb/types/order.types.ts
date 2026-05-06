// types/order.types.ts

import { IAddress } from './address.types';
import { IBaseEntity } from './common.types';

/** 订单状态枚举 - 匹配后端 OrderDto 状态值 */
export type OrderStatus = 0 | 1 | 2 | 3 | 4 | 5;

/** 订单状态常量 */
export const OrderStatusEnum = {
  Pending: 0,      // 待付款
  Paid: 1,         // 待发货（已支付）
  Shipped: 2,      // 待收货（已发货）
  Completed: 3,    // 已完成
  Cancelled: 4,    // 已取消
  Refunded: 5,     // 已退款
} as const;

/** 订单状态映射（用于显示） */
export const OrderStatusMap: Record<number, { text: string; color: string }> = {
  0: { text: '待付款', color: '#ff976a' },
  1: { text: '待发货', color: '#1989fa' },
  2: { text: '待收货', color: '#07c160' },
  3: { text: '已完成', color: '#07c160' },
  4: { text: '已取消', color: '#999' },
  5: { text: '已退款', color: '#999' },
};

/** 获取订单状态文本 */
export function getOrderStatusText(status: number): string {
  return OrderStatusMap[status]?.text || '未知';
}

/** 订单商品项 */
export interface IOrderItem {
  id: string;
  productId: string;
  productName: string;
  productImage: string;
  price: number;  // 元
  count: number;     // 商品数量
  subtotal: number;  // 小计金额（元）
}

/** 订单信息 */
export interface IOrder extends IBaseEntity {
  id: string;
  orderNo: string;
  status: OrderStatus;
  statusText?: string;
  items: IOrderItem[];
  totalAmount: number;  // 元
  address?: IAddress;
  addressId?: string;
  paymentTime?: number;
  deliveryTime?: number;
  completeTime?: number;
  remark?: string;
  logistics?: {
    company: string;
    number: string;
    status: string;
  };
}

/** 创建订单参数 */
export interface ICreateOrderParams {
  cartItemIds: string[];
  addressId: string;
  remark?: string;
}

/** 订单查询参数 */
export interface IOrderQueryParams {
  pageIndex: number;
  pageSize: number;
  status?: number;
}