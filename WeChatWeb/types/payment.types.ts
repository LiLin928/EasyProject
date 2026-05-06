// types/payment.types.ts

/** 微信支付参数 */
export interface IWxPayParams {
  timeStamp: string;
  nonceStr: string;
  package: string;
  signType: 'RSA' | 'MD5';
  paySign: string;
}

/** 支付结果 */
export interface IPaymentResult {
  success: boolean;
  orderId: string;
  paymentId?: string;
  message?: string;
  /** 微信支付参数（真实微信支付时返回） */
  wxPayParams?: IWxPayParams;
}

/** 支付方式 */
export type PaymentMethod = 'wechat' | 'alipay' | 'balance';