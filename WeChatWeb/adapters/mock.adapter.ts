// adapters/mock.adapter.ts

import { IDataAdapter } from './adapter.interface';
import { StorageUtil } from '../utils/storage';
import { ApiConfig } from '../config/api.config';
import { MockProducts, MockUser, MockAddresses, MockCoupons, MockReviews } from '../mock-data/index';
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
  IOrderItem,
  OrderStatus,
  OrderStatusEnum,
  IProductReview,
  ICoupon,
  IReviewQueryParams,
  IPhoneLoginParams,
  IWxPhoneLoginParams,
} from '../types/index';

/** 生成 GUID */
function generateGuid(): string {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0;
    const v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

export class MockAdapter implements IDataAdapter {

  // ========== 用户认证 ==========
  async wxLogin(code: string): Promise<IUser> {
    const user = { ...MockUser.defaultUser, code };
    StorageUtil.set('userInfo', user.userInfo);
    StorageUtil.set('token', user.token);
    return user;
  }

  async phoneLogin(params: IPhoneLoginParams): Promise<IUser> {
    const mockUser: IUser = {
      id: generateGuid(),
      token: 'mock_token_' + generateGuid(),
      userInfo: {
        id: generateGuid(),
        openId: 'mock_openid',
        nickname: 'Mock用户',
        avatarUrl: '/static/images/default-avatar.png',
        phone: params.phone,
        gender: 0,
      },
    };

    StorageUtil.set(ApiConfig.tokenKey, mockUser.token);
    StorageUtil.set(ApiConfig.userInfoKey, mockUser.userInfo);

    return mockUser;
  }

  async wxPhoneLogin(params: IWxPhoneLoginParams): Promise<IUser> {
    const mockUser: IUser = {
      id: generateGuid(),
      token: 'mock_token_' + generateGuid(),
      userInfo: {
        id: generateGuid(),
        openId: 'mock_openid',
        nickname: '微信用户',
        avatarUrl: '/static/images/default-avatar.png',
        phone: '13800138000',
        gender: 0,
      },
    };

    StorageUtil.set(ApiConfig.tokenKey, mockUser.token);
    StorageUtil.set(ApiConfig.userInfoKey, mockUser.userInfo);

    return mockUser;
  }

  async getUserInfo(): Promise<IUserInfo> {
    return StorageUtil.get<IUserInfo>('userInfo') || MockUser.defaultUserInfo;
  }

  async updateUserInfo(data: Partial<IUserInfo>): Promise<IUserInfo> {
    const current = await this.getUserInfo();
    const updated = { ...current, ...data, updatedAt: Date.now() };
    StorageUtil.set('userInfo', updated);
    return updated;
  }

  // ========== 商品相关 ==========
  async getProductList(params: IPageQuery): Promise<IPageResult<IProduct>> {
    const allProducts = MockProducts.list;
    const start = (params.pageIndex - 1) * params.pageSize;
    const list = allProducts.slice(start, start + params.pageSize);
    return { list, total: allProducts.length, pageIndex: params.pageIndex };
  }

  async getProductDetail(id: string): Promise<IProductDetail> {
    const detail = MockProducts.getProductDetail(id);
    if (!detail) {
      throw new Error('商品不存在');
    }
    return detail;
  }

  async getProductCategories(): Promise<ICategory[]> {
    return MockProducts.categories;
  }

  async searchProducts(keyword: string): Promise<IProduct[]> {
    const keywordLower = keyword.toLowerCase();
    return MockProducts.list.filter(p =>
      p.name.toLowerCase().includes(keywordLower) ||
      (p.description && p.description.toLowerCase().includes(keywordLower))
    );
  }

  async getProductReviews(params: IReviewQueryParams): Promise<IPageResult<IProductReview>> {
    const result = MockProducts.getProductReviews(params.productId, params.pageIndex, params.pageSize);
    return {
      list: result.list,
      total: result.total,
      pageIndex: params.pageIndex,
    };
  }

  async getAvailableCoupons(productId?: string): Promise<ICoupon[]> {
    // 返回可用优惠券
    return MockCoupons.filter(c => c.status === 'available');
  }

  async claimCoupon(couponId: string): Promise<boolean> {
    // 模拟领取优惠券
    const coupon = MockCoupons.find(c => c.id === couponId);
    if (coupon && coupon.status === 'available') {
      // 这里可以保存到用户优惠券列表
      return true;
    }
    return false;
  }

  // ========== 购物车相关 ==========
  async getCartList(): Promise<ICartItem[]> {
    return StorageUtil.get<ICartItem[]>('cartList') || [];
  }

  async addToCart(productId: string, count: number): Promise<ICartItem> {
    const cartList = StorageUtil.get<ICartItem[]>('cartList') || [];
    const product = MockProducts.list.find(p => p.id === productId);

    if (!product) {
      throw new Error('商品不存在');
    }

    const existItem = cartList.find(item => item.productId === productId);
    if (existItem) {
      existItem.count += count;
      existItem.updatedAt = Date.now();
      StorageUtil.set('cartList', cartList);
      return existItem;
    }

    const newItem: ICartItem = {
      id: generateGuid(),
      productId,
      product,
      count,
      createdAt: Date.now(),
      updatedAt: Date.now(),
    };
    cartList.push(newItem);
    StorageUtil.set('cartList', cartList);
    return newItem;
  }

  async updateCartItem(cartItemId: string, count: number): Promise<ICartItem> {
    const cartList = StorageUtil.get<ICartItem[]>('cartList') || [];
    const item = cartList.find(i => i.id === cartItemId);
    if (item) {
      item.count = count;
      item.updatedAt = Date.now();
      StorageUtil.set('cartList', cartList);
    }
    return item!;
  }

  async removeFromCart(cartItemId: string): Promise<void> {
    const cartList = StorageUtil.get<ICartItem[]>('cartList') || [];
    const filtered = cartList.filter(i => i.id !== cartItemId);
    StorageUtil.set('cartList', filtered);
  }

  async clearCart(): Promise<void> {
    StorageUtil.remove('cartList');
  }

  // ========== 订单相关 ==========
  async createOrder(data: ICreateOrderParams): Promise<IOrder> {
    const cartList = await this.getCartList();
    const selectedItems = cartList.filter(item => data.cartItemIds.includes(item.id));

    // 根据 addressId 查找地址信息
    const addresses = await this.getAddressList();
    const address = addresses.find(a => a.id === data.addressId);
    if (!address) {
      throw new Error('地址不存在');
    }

    const orderItems: IOrderItem[] = selectedItems.map(item => ({
      id: generateGuid(),
      productId: item.productId,
      productName: item.product.name,
      productImage: item.product.image,
      price: item.product.price,
      count: item.count,
      subtotal: item.product.price * item.count,
    }));

    const totalAmount = selectedItems.reduce((sum, item) => sum + item.product.price * item.count, 0);

    const order: IOrder = {
      id: generateGuid(),
      orderNo: 'NO' + Date.now(),
      status: OrderStatusEnum.Pending as OrderStatus, // 待支付
      items: orderItems,
      totalAmount,
      address,
      addressId: data.addressId,
      remark: data.remark,
      createdAt: Date.now(),
      updatedAt: Date.now(),
    };

    const orderList = StorageUtil.get<IOrder[]>('orderList') || [];
    orderList.push(order);
    StorageUtil.set('orderList', orderList);

    // 清空已下单的购物车商品
    const remainingCart = cartList.filter(item => !data.cartItemIds.includes(item.id));
    StorageUtil.set('cartList', remainingCart);

    return order;
  }

  async getOrderList(params: IPageQuery): Promise<IPageResult<IOrder>> {
    const allOrders = StorageUtil.get<IOrder[]>('orderList') || [];
    const start = (params.pageIndex - 1) * params.pageSize;
    const list = allOrders.slice(start, start + params.pageSize);
    return { list, total: allOrders.length, pageIndex: params.pageIndex };
  }

  async getOrderDetail(id: string): Promise<IOrder> {
    const orderList = StorageUtil.get<IOrder[]>('orderList') || [];
    return orderList.find(o => o.id === id)!;
  }

  async cancelOrder(id: string): Promise<IOrder> {
    const orderList = StorageUtil.get<IOrder[]>('orderList') || [];
    const order = orderList.find(o => o.id === id);
    if (order && order.status === OrderStatusEnum.Pending) {
      order.status = OrderStatusEnum.Cancelled as OrderStatus;
      order.updatedAt = Date.now();
      StorageUtil.set('orderList', orderList);
    }
    return order!;
  }

  // ========== 地址相关 ==========
  async getAddressList(): Promise<IAddress[]> {
    return StorageUtil.get<IAddress[]>('addressList') || MockAddresses.list;
  }

  async getAddressDetail(id: string): Promise<IAddress> {
    const addresses = await this.getAddressList();
    return addresses.find(a => a.id === id)!;
  }

  async saveAddress(data: IAddress): Promise<IAddress> {
    const addressList = StorageUtil.get<IAddress[]>('addressList') || MockAddresses.list;

    if (data.id) {
      const index = addressList.findIndex(a => a.id === data.id);
      if (index !== -1) {
        addressList[index] = { ...data, updatedAt: Date.now() };
      }
    } else {
      const newAddress: IAddress = {
        ...data,
        id: generateGuid(),
        createdAt: Date.now(),
        updatedAt: Date.now(),
      };
      addressList.push(newAddress);

      if (addressList.length === 1 || data.isDefault) {
        addressList.forEach(a => a.isDefault = a.id === newAddress.id);
      }
      data = newAddress;
    }

    StorageUtil.set('addressList', addressList);
    return data;
  }

  async deleteAddress(id: string): Promise<void> {
    const addressList = StorageUtil.get<IAddress[]>('addressList') || [];
    const filtered = addressList.filter(a => a.id !== id);
    StorageUtil.set('addressList', filtered);
  }

  // ========== 支付相关 ==========
  async createPayment(orderId: string): Promise<IPaymentResult> {
    const orderList = StorageUtil.get<IOrder[]>('orderList') || [];
    const order = orderList.find(o => o.id === orderId);
    if (order) {
      order.status = OrderStatusEnum.Paid as OrderStatus; // 已支付
      order.paymentTime = Date.now();
      order.updatedAt = Date.now();
      StorageUtil.set('orderList', orderList);
    }

    return {
      success: true,
      orderId,
      paymentId: generateGuid(),
      message: '支付成功',
    };
  }
}