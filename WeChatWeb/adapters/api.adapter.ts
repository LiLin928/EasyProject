// adapters/api.adapter.ts

import { IDataAdapter } from './adapter.interface';
import { Http } from '../utils/http';
import { StorageUtil } from '../utils/storage';
import { ApiConfig, ApiPaths } from '../config/api.config';
import { Formatter } from '../utils/formatter';
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
  IWxPayParams,
  IOrderQueryParams,
  IProductQueryParams,
  IProductReview,
  ICoupon,
  IReviewQueryParams,
  transformPageResponse,
  IPageResponse,
  IPhoneLoginParams,
  IWxPhoneLoginParams,
  OrderStatus,
} from '../types/index';

/** 后端购物车状态响应格式 */
interface ICartStateResponse {
  items: ICartItem[];
  totalCount: number;
  totalPrice: number;
  selectedCount: number;
  selectedPrice: number;
}

/** 后端登录响应格式 */
interface ILoginResponse {
  id: string;
  token: string;
  userInfo: {
    id: string;
    openId: string;
    nickname: string;
    avatarUrl: string;
    phone: string;
    gender: 0 | 1 | 2;
  };
}

/** 后端支付响应格式 */
interface IPaymentResponse {
  success: boolean;
  orderId: string;
  paymentId?: string;
  message?: string;
  /** 微信支付参数 */
  wxPayParams?: IWxPayParams;
}

/** 后端订单商品项响应格式 */
interface IOrderItemResponse {
  id: string;
  productId: string;
  productName: string;
  productImage: string;
  price: number;
  quantity: number;  // 后端数量字段名
  amount: number;    // 后端小计字段名
}

/** 后端订单响应格式 */
interface IOrderResponse {
  id: string;
  orderNo: string;
  status: number;
  items: IOrderItemResponse[];
  totalAmount: number;
  addressId: string;
  paymentTime?: number;
  deliveryTime?: number;
  completeTime?: number;
  remark?: string;
  createdAt: number;
  updatedAt: number;
}

/** API 适配器 - 调用真实后端 API */
export class ApiAdapter implements IDataAdapter {

  // ========== 用户认证 ==========
  async wxLogin(code: string): Promise<IUser> {
    const response = await Http.post<ILoginResponse>(ApiPaths.login, { code });

    // 存储 token 和用户信息
    StorageUtil.set(ApiConfig.tokenKey, response.token);
    StorageUtil.set(ApiConfig.userInfoKey, response.userInfo);

    // 转换为前端 IUser 格式，处理头像 URL
    return {
      id: response.id,
      token: response.token,
      userInfo: {
        id: response.userInfo.id,
        openId: response.userInfo.openId,
        nickname: response.userInfo.nickname,
        avatarUrl: Formatter.imageUrl(response.userInfo.avatarUrl),
        phone: response.userInfo.phone,
        gender: response.userInfo.gender,
      },
    };
  }

  async phoneLogin(params: IPhoneLoginParams): Promise<IUser> {
    const response = await Http.post<ILoginResponse>(ApiPaths.phoneLogin, {
      phone: params.phone,
      password: params.password,
    });

    StorageUtil.set(ApiConfig.tokenKey, response.token);
    StorageUtil.set(ApiConfig.userInfoKey, response.userInfo);

    return {
      id: response.id,
      token: response.token,
      userInfo: {
        id: response.userInfo.id,
        openId: response.userInfo.openId,
        nickname: response.userInfo.nickname,
        avatarUrl: Formatter.imageUrl(response.userInfo.avatarUrl),
        phone: response.userInfo.phone,
        gender: response.userInfo.gender,
      },
    };
  }

  async wxPhoneLogin(params: IWxPhoneLoginParams): Promise<IUser> {
    const response = await Http.post<ILoginResponse>(ApiPaths.wxPhoneLogin, {
      code: params.code,
      encryptedData: params.encryptedData,
      iv: params.iv,
    });

    StorageUtil.set(ApiConfig.tokenKey, response.token);
    StorageUtil.set(ApiConfig.userInfoKey, response.userInfo);

    return {
      id: response.id,
      token: response.token,
      userInfo: {
        id: response.userInfo.id,
        openId: response.userInfo.openId,
        nickname: response.userInfo.nickname,
        avatarUrl: Formatter.imageUrl(response.userInfo.avatarUrl),
        phone: response.userInfo.phone,
        gender: response.userInfo.gender,
      },
    };
  }

  async getUserInfo(): Promise<IUserInfo> {
    const userInfo = StorageUtil.get<IUserInfo>(ApiConfig.userInfoKey);
    if (userInfo) {
      return {
        ...userInfo,
        avatarUrl: Formatter.imageUrl(userInfo.avatarUrl),
      };
    }
    // 如果本地没有，从后端获取
    const response = await Http.get<IUserInfo>(ApiPaths.userinfo);
    StorageUtil.set(ApiConfig.userInfoKey, response);
    return {
      ...response,
      avatarUrl: Formatter.imageUrl(response.avatarUrl),
    };
  }

  async updateUserInfo(data: Partial<IUserInfo>): Promise<IUserInfo> {
    const response = await Http.post<IUserInfo>(ApiPaths.userinfo, data);
    StorageUtil.set(ApiConfig.userInfoKey, response);
    return {
      ...response,
      avatarUrl: Formatter.imageUrl(response.avatarUrl),
    };
  }

  // ========== 商品相关 ==========
  async getProductList(params: IPageQuery): Promise<IPageResult<IProduct>> {
    const queryParams: IProductQueryParams = {
      pageIndex: params.pageIndex,
      pageSize: params.pageSize,
    };
    const response = await Http.post<IPageResponse<IProduct>>(ApiPaths.productList, queryParams);
    // 处理图片 URL
    const list = response.list.map(item => ({
      ...item,
      image: Formatter.imageUrl(item.image),
    }));
    return { list: list, total: response.total, pageIndex: response.pageIndex, pageSize: response.pageSize };
  }

  async getProductDetail(id: string): Promise<IProductDetail> {
    const product = await Http.get<IProductDetail>(`${ApiPaths.productDetail}/${id}`);
    // 处理图片 URL
    return {
      ...product,
      image: Formatter.imageUrl(product.image),
      images: Formatter.imageUrls(product.images),
      // media 字段后端不支持，前端不处理
      relatedProducts: product.relatedProducts?.map(rp => ({
        ...rp,
        image: Formatter.imageUrl(rp.image),
      })),
    };
  }

  async getProductCategories(): Promise<ICategory[]> {
    const categories = await Http.get<ICategory[]>(ApiPaths.productCategories);
    // 处理分类图标 URL
    return categories.map(cat => ({
      ...cat,
      icon: Formatter.imageUrl(cat.icon),
    }));
  }

  async searchProducts(keyword: string): Promise<IProduct[]> {
    const response = await Http.get<IPageResponse<IProduct>>(ApiPaths.productSearch, {
      keyword,
      pageIndex: 1,
      pageSize: 50,
    });
    // 处理商品图片 URL
    return response.list.map(item => ({
      ...item,
      image: Formatter.imageUrl(item.image),
    }));
  }

  async getProductReviews(params: IReviewQueryParams): Promise<IPageResult<IProductReview>> {
    // 商品 ID 放在路径中，rating 和 hasImage 只传有效值
    const queryParams: Record<string, any> = {
      pageIndex: params.pageIndex,
      pageSize: params.pageSize,
    };
    // 只在有效时添加 rating 和 hasImage
    if (params.rating !== undefined && params.rating !== null) {
      queryParams.rating = params.rating;
    }
    if (params.hasImage !== undefined && params.hasImage !== null) {
      queryParams.hasImage = params.hasImage;
    }
    const response = await Http.get<IPageResponse<IProductReview>>(
      `/product/${params.productId}/reviews`,
      queryParams
    );
    // 处理评价中的图片 URL
    const list = response.list.map(review => ({
      ...review,
      userAvatar: Formatter.imageUrl(review.userAvatar),
      images: Formatter.imageUrls(review.images),
    }));
    return { list: list, total: response.total, pageIndex: response.pageIndex, pageSize: response.pageSize };
  }

  async getAvailableCoupons(productId?: string): Promise<ICoupon[]> {
    // 后端是 POST 请求，需要传 body 参数
    const response = await Http.post<{ coupons: ICoupon[] }>(ApiPaths.couponAvailable, {
      productId,
      cartItemIds: [],
    });
    return response.coupons || [];
  }

  async claimCoupon(couponId: string): Promise<boolean> {
    // 后端路径是 /coupon/claim/{couponId}
    await Http.post<void>(`${ApiPaths.couponClaim}/${couponId}`);
    return true;
  }

  // ========== 购物车相关 ==========
  async getCartList(): Promise<ICartItem[]> {
    const response = await Http.post<ICartStateResponse>(ApiPaths.cartList);
    // 处理购物车商品图片 URL
    return (response.items || []).map(item => ({
      ...item,
      product: {
        ...item.product,
        image: Formatter.imageUrl(item.product?.image),
        images: Formatter.imageUrls(item.product?.images),
      },
    }));
  }

  async addToCart(productId: string, count: number): Promise<ICartItem> {
    return await Http.post<ICartItem>(ApiPaths.cartAdd, { productId, count });
  }

  async updateCartItem(cartItemId: string, count: number): Promise<ICartItem> {
    return await Http.put<ICartItem>(`${ApiPaths.cartUpdate}/${cartItemId}`, { count });
  }

  async removeFromCart(cartItemId: string): Promise<void> {
    await Http.delete(`${ApiPaths.cartDelete}/${cartItemId}`);
  }

  async clearCart(): Promise<void> {
    await Http.delete(ApiPaths.cartClear);
  }

  // ========== 订单相关 ==========
  async createOrder(data: ICreateOrderParams): Promise<IOrder> {
    // 使用驼峰命名格式
    const orderId = await Http.post<string>(ApiPaths.orderCreate, {
      cartItemIds: data.cartItemIds,
      addressId: data.addressId,
      remark: data.remark,
    });

    // 创建订单后返回完整订单详情
    return await this.getOrderDetail(orderId);
  }

  async getOrderList(params: IPageQuery): Promise<IPageResult<IOrder>> {
    const queryParams: IOrderQueryParams = {
      pageIndex: params.pageIndex,
      pageSize: params.pageSize,
    };
    const response = await Http.post<IPageResponse<IOrderResponse>>(ApiPaths.orderList, queryParams);
    // 处理订单商品图片 URL 和字段转换
    const list = response.list.map(order => this.transformOrderResponse(order));
    return { list: list, total: response.total, pageIndex: response.pageIndex, pageSize: response.pageSize };
  }

  async getOrderDetail(id: string): Promise<IOrder> {
    const order = await Http.get<IOrderResponse>(`${ApiPaths.orderDetail}/${id}`);
    return this.transformOrderResponse(order);
  }

  /** 转换后端订单响应为前端格式 */
  private transformOrderResponse(order: IOrderResponse): IOrder {
    return {
      id: order.id,
      orderNo: order.orderNo,
      status: order.status as OrderStatus,
      items: order.items?.map(item => ({
        id: item.id,
        productId: item.productId,
        productName: item.productName,
        productImage: Formatter.imageUrl(item.productImage),
        price: item.price,
        count: item.quantity,      // 后端 quantity → 前端 count
        subtotal: item.amount,     // 后端 amount → 前端 subtotal
      })) || [],
      totalAmount: order.totalAmount,
      addressId: order.addressId,
      paymentTime: order.paymentTime,
      deliveryTime: order.deliveryTime,
      completeTime: order.completeTime,
      remark: order.remark,
      createdAt: order.createdAt,
      updatedAt: order.updatedAt,
    };
  }

  async cancelOrder(id: string): Promise<IOrder> {
    await Http.put(`${ApiPaths.orderCancel}/${id}/cancel`);
    // 取消后返回更新后的订单详情
    return await this.getOrderDetail(id);
  }

  // ========== 地址相关 ==========
  async getAddressList(): Promise<IAddress[]> {
    return await Http.post<IAddress[]>(ApiPaths.addressList);
  }

  async getAddressDetail(id: string): Promise<IAddress> {
    return await Http.get<IAddress>(`${ApiPaths.addressDetail}/${id}`);
  }

  async saveAddress(data: IAddress): Promise<IAddress> {
    // 使用驼峰命名格式
    return await Http.post<IAddress>(ApiPaths.addressSave, {
      id: data.id,
      name: data.name,
      phone: data.phone,
      province: data.province,
      city: data.city,
      district: data.district,
      detail: data.detail,
      isDefault: data.isDefault,
    });
  }

  async deleteAddress(id: string): Promise<void> {
    await Http.delete(`${ApiPaths.addressDelete}/${id}`);
  }

  // ========== 支付相关 ==========
  async createPayment(orderId: string): Promise<IPaymentResult> {
    const response = await Http.post<IPaymentResponse>(ApiPaths.paymentCreate, {
      orderId,
    });

    // 如果返回微信支付参数，调用微信支付
    if (response.success && response.wxPayParams) {
      try {
        await this.callWxPayment(response.wxPayParams);
        return {
          success: true,
          orderId: response.orderId,
          paymentId: response.paymentId,
          message: '支付成功',
        };
      } catch (error) {
        return {
          success: false,
          orderId: response.orderId,
          message: '支付失败',
        };
      }
    }

    // Mock 模式下直接返回结果
    return {
      success: response.success,
      orderId: response.orderId,
      paymentId: response.paymentId,
      message: response.message,
    };
  }

  /** 调用微信支付 */
  private callWxPayment(params: IWxPayParams): Promise<void> {
    return new Promise((resolve, reject) => {
      wx.requestPayment({
        timeStamp: params.timeStamp,
        nonceStr: params.nonceStr,
        package: params.package,
        signType: params.signType,
        paySign: params.paySign,
        success: () => resolve(),
        fail: (err) => reject(new Error(err.errMsg || '支付失败')),
      });
    });
  }
}