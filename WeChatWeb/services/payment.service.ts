// services/payment.service.ts

import { adapter } from '../adapters/adapter.config';
import { orderStore } from '../stores/index';
import { IPaymentResult, OrderStatusEnum } from '../types/index';
import { authService } from './auth.service';

export class PaymentService {
  private static instance: PaymentService;

  static getInstance(): PaymentService {
    if (!PaymentService.instance) {
      PaymentService.instance = new PaymentService();
    }
    return PaymentService.instance;
  }

  /** 创建支付 */
  async createPayment(orderId: string): Promise<IPaymentResult> {
    if (!authService.requireLogin()) {
      throw new Error('请先登录');
    }

    const result = await adapter.createPayment(orderId);

    if (result.success) {
      orderStore.updateOrderStatus(orderId, OrderStatusEnum.Paid);
      wx.showToast({ title: '支付成功', icon: 'success' });
    } else {
      wx.showToast({ title: result.message || '支付失败', icon: 'none' });
    }

    return result;
  }

  /** 模拟微信支付流程 */
  async wxPay(orderId: string): Promise<IPaymentResult> {
    return await this.createPayment(orderId);
  }
}

export const paymentService = PaymentService.getInstance();