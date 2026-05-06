// config/api.config.ts

import { envConfig } from './env.dev'; // 切换环境时修改此处: './env.dev' | './env.test' | './env.prod'

/** API 基础配置 */
export const ApiConfig = envConfig;

/** API 路径 */
export const ApiPaths = {
  // 认证
  login: '/auth/login',
  userinfo: '/auth/userinfo',
  phoneLogin: '/auth/phone-login',
  wxPhoneLogin: '/auth/wx-phone-login',

  // 商品
  productList: '/product/list',
  productDetail: '/product',
  productCategories: '/product/categories',
  productSearch: '/product/search',

  // 购物车
  cartList: '/cart/list',
  cartAdd: '/cart/add',
  cartUpdate: '/cart',
  cartDelete: '/cart',
  cartClear: '/cart/clear',

  // 地址
  addressList: '/address/list',
  addressDetail: '/address',
  addressSave: '/address/save',
  addressDelete: '/address',

  // 订单
  orderCreate: '/order/create',
  orderList: '/order/list',
  orderDetail: '/order',
  orderCancel: '/order',  // 拼接为 /order/{id}/cancel

  // 支付
  paymentCreate: '/payment/create',

  // 优惠券
  couponClaimable: '/coupon/claimable',
  couponClaim: '/coupon/claim',
  couponMy: '/coupon/my',
  couponAvailable: '/coupon/available',
};