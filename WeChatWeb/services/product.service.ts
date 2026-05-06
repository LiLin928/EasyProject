// services/product.service.ts

import { adapter } from '../adapters/adapter.config';
import { IProduct, IProductDetail, ICategory, IPageResult, IPageQuery, IProductReview, ICoupon, IReviewQueryParams } from '../types/index';

export class ProductService {
  private static instance: ProductService;

  static getInstance(): ProductService {
    if (!ProductService.instance) {
      ProductService.instance = new ProductService();
    }
    return ProductService.instance;
  }

  /** 获取商品列表 */
  async getProductList(params: IPageQuery): Promise<IPageResult<IProduct>> {
    return await adapter.getProductList(params);
  }

  /** 获取商品详情（扩展版） */
  async getProductDetail(id: string): Promise<IProductDetail> {
    return await adapter.getProductDetail(id);
  }

  /** 获取商品基础信息 */
  async getProductInfo(id: string): Promise<IProduct> {
    return await adapter.getProductDetail(id);
  }

  /** 获取商品分类 */
  async getCategories(): Promise<ICategory[]> {
    return await adapter.getProductCategories();
  }

  /** 搜索商品 */
  async searchProducts(keyword: string): Promise<IProduct[]> {
    return await adapter.searchProducts(keyword);
  }

  /** 获取热销商品 */
  async getHotProducts(limit: number = 10): Promise<IProduct[]> {
    const result = await adapter.getProductList({ pageIndex: 1, pageSize: 100 });
    return result.list.filter(p => p.isHot).slice(0, limit);
  }

  /** 获取新品推荐 */
  async getNewProducts(limit: number = 10): Promise<IProduct[]> {
    const result = await adapter.getProductList({ pageIndex: 1, pageSize: 100 });
    return result.list.filter(p => p.isNew).slice(0, limit);
  }

  /** 获取商品评价 */
  async getProductReviews(params: IReviewQueryParams): Promise<IPageResult<IProductReview>> {
    return await adapter.getProductReviews(params);
  }

  /** 获取可用优惠券 */
  async getAvailableCoupons(productId?: string): Promise<ICoupon[]> {
    return await adapter.getAvailableCoupons(productId);
  }

  /** 领取优惠券 */
  async claimCoupon(couponId: string): Promise<boolean> {
    return await adapter.claimCoupon(couponId);
  }
}

export const productService = ProductService.getInstance();