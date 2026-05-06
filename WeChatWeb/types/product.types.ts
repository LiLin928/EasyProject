// types/product.types.ts

import { IBaseEntity } from './common.types';

/** 商品分类 */
export interface ICategory {
  id: string;
  name: string;
  icon?: string;
  sort?: number;
  description?: string;
}

/** 媒体类型 */
export type MediaType = 'image' | 'video';

/** 商品媒体项（图片或视频） */
export interface IProductMedia {
  id: string;
  type: MediaType;
  url: string;
  thumbnail?: string;  // 视频封面图
  duration?: number;   // 视频时长（秒）
}

/** 商品规格选项 */
export interface ISpecOption {
  id: string;
  name: string;
  value: string;
  stock?: number;
  priceAdjust?: number;  // 价格调整（正数加价，负数减价）
}

/** 商品规格组 */
export interface ISpecGroup {
  id: string;
  name: string;        // 规格名称，如"尺寸"、"包装"
  required: boolean;   // 是否必选
  options: ISpecOption[];
}

/** 商品优惠券 */
export interface ICoupon {
  id: string;
  name: string;
  type: 'discount' | 'reduce' | 'percent';  // 折扣券、满减券、百分比券
  value: number;           // 优惠值
  minAmount: number;       // 最低消费金额
  startTime: number;
  endTime: number;
  description?: string;
  status: 'available' | 'used' | 'expired';
}

/** 商品评价 */
export interface IProductReview extends IBaseEntity {
  id: string;
  productId: string;
  userId: string;
  userName: string;
  userAvatar?: string;
  rating: number;          // 评分 1-5
  content: string;
  images?: string[];
  reply?: string;          // 商家回复
  replyTime?: number;
  isAnonymous: boolean;
  likes: number;
}

/** 商品信息 */
export interface IProduct extends IBaseEntity {
  id: string;
  name: string;
  description?: string;
  price: number;  // 元（后端已转换为元）
  originalPrice?: number;
  image: string;
  images?: string[];
  categoryId: string;
  category?: ICategory;
  stock: number;
  sales?: number;
  isHot?: boolean;
  isNew?: boolean;
  detail?: string;
}

/** 商品详情（扩展） */
export interface IProductDetail extends IProduct {
  // 媒体（图片+视频）
  media?: IProductMedia[];

  // 规格
  specs?: ISpecGroup[];
  defaultSpecId?: string;  // 默认选中的规格组合ID

  // 优惠券
  coupons?: ICoupon[];

  // 评价概览
  reviewSummary?: {
    total: number;
    avgRating: number;
    goodRate: number;  // 好评率
  };

  // 服务保障
  services?: string[];  // 如 ["新鲜保证", "同城配送", "极速退款"]

  // 相关推荐
  relatedProducts?: IProduct[];
}

/** 规格选择结果 */
export interface ISpecSelection {
  groupId: string;
  optionId: string;
  optionName: string;
  value: string;
}

/** SKU 信息 */
export interface ISKU {
  id: string;
  productId: string;
  specCombination: string;  // 规格组合JSON
  price: number;
  stock: number;
  image?: string;
}

/** 商品查询参数 */
export interface IProductQueryParams {
  pageIndex: number;
  pageSize: number;
  categoryId?: string;
  keyword?: string;
  isHot?: boolean;
  isNew?: boolean;
  minPrice?: number;
  maxPrice?: number;
  sortField?: string;
  sortOrder?: string;
}

/** 商品搜索参数 */
export interface IProductSearchParams {
  keyword?: string;
  categoryId?: string;
  isHot?: boolean;
  isNew?: boolean;
  minPrice?: number;
  maxPrice?: number;
  sortField?: 'price' | 'sales' | 'createdAt';
  sortOrder?: 'asc' | 'desc';
}

/** 评价查询参数 */
export interface IReviewQueryParams {
  productId: string;
  pageIndex: number;
  pageSize: number;
  rating?: number;  // 筛选评分
  hasImage?: boolean;  // 筛选有图评价
}