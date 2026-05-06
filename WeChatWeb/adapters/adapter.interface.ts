// adapters/adapter.interface.ts

import {
  IUser,
  IUserInfo,
  IProduct,
  IProductDetail,
  ICategory,
  IPageQuery,
  IPageResult,
  ICartItem,
  IOrder,
  ICreateOrderParams,
  IAddress,
  IPaymentResult,
  IProductReview,
  ICoupon,
  IReviewQueryParams,
  IPhoneLoginParams,
  IWxPhoneLoginParams,
} from '../types/index';

/** 数据适配器接口 - Mock 和 API 共享 */
export interface IDataAdapter {
  // ========== 用户认证 ==========
  wxLogin(code: string): Promise<IUser>;
  phoneLogin(params: IPhoneLoginParams): Promise<IUser>;
  wxPhoneLogin(params: IWxPhoneLoginParams): Promise<IUser>;
  getUserInfo(): Promise<IUserInfo>;
  updateUserInfo(data: Partial<IUserInfo>): Promise<IUserInfo>;

  // ========== 商品相关 ==========
  getProductList(params: IPageQuery): Promise<IPageResult<IProduct>>;
  getProductDetail(id: string): Promise<IProductDetail>;
  getProductCategories(): Promise<ICategory[]>;
  searchProducts(keyword: string): Promise<IProduct[]>;
  getProductReviews(params: IReviewQueryParams): Promise<IPageResult<IProductReview>>;
  getAvailableCoupons(productId?: string): Promise<ICoupon[]>;
  claimCoupon(couponId: string): Promise<boolean>;

  // ========== 购物车相关 ==========
  getCartList(): Promise<ICartItem[]>;
  addToCart(productId: string, count: number): Promise<ICartItem>;
  updateCartItem(cartItemId: string, count: number): Promise<ICartItem>;
  removeFromCart(cartItemId: string): Promise<void>;
  clearCart(): Promise<void>;

  // ========== 订单相关 ==========
  createOrder(data: ICreateOrderParams): Promise<IOrder>;
  getOrderList(params: IPageQuery): Promise<IPageResult<IOrder>>;
  getOrderDetail(id: string): Promise<IOrder>;
  cancelOrder(id: string): Promise<IOrder>;

  // ========== 地址相关 ==========
  getAddressList(): Promise<IAddress[]>;
  getAddressDetail(id: string): Promise<IAddress>;
  saveAddress(data: IAddress): Promise<IAddress>;
  deleteAddress(id: string): Promise<void>;

  // ========== 支付相关 ==========
  createPayment(orderId: string): Promise<IPaymentResult>;
}