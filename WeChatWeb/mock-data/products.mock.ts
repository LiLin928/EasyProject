// mock-data/products.mock.ts

import {
  IProduct,
  IProductDetail,
  IProductMedia,
  ISpecGroup,
  ICoupon,
  IProductReview,
} from '../types/index';

/** 生成 GUID */
export function generateGuid(): string {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0;
    const v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

/** Mock 优惠券数据 */
export const MockCoupons: ICoupon[] = [
  {
    id: generateGuid(),
    name: '新人专享券',
    type: 'reduce',
    value: 20,
    minAmount: 100,
    startTime: Date.now() - 86400000 * 7,
    endTime: Date.now() + 86400000 * 30,
    description: '满100减20，新人专享',
    status: 'available',
  },
  {
    id: generateGuid(),
    name: '满减优惠券',
    type: 'reduce',
    value: 30,
    minAmount: 200,
    startTime: Date.now() - 86400000 * 3,
    endTime: Date.now() + 86400000 * 7,
    description: '满200减30',
    status: 'available',
  },
  {
    id: generateGuid(),
    name: '限时折扣券',
    type: 'percent',
    value: 10, // 9折
    minAmount: 50,
    startTime: Date.now(),
    endTime: Date.now() + 86400000 * 3,
    description: '全场9折，限时3天',
    status: 'available',
  },
];

/** Mock 商品评价数据 */
export const MockReviews: IProductReview[] = [
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440001',
    userId: generateGuid(),
    userName: '张***',
    userAvatar: '/static/images/avatars/avatar1.png',
    rating: 5,
    content: '花很新鲜，女朋友很喜欢！配送速度也很快，包装精美，下次还会再来购买。',
    images: ['/static/images/reviews/review1-1.jpg', '/static/images/reviews/review1-2.jpg'],
    isAnonymous: false,
    likes: 12,
    createdAt: Date.now() - 86400000 * 2,
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440001',
    userId: generateGuid(),
    userName: '李**',
    rating: 5,
    content: '非常满意！花束比图片上还要漂亮，物超所值。',
    isAnonymous: true,
    likes: 8,
    createdAt: Date.now() - 86400000 * 5,
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440001',
    userId: generateGuid(),
    userName: '王**',
    userAvatar: '/static/images/avatars/avatar2.png',
    rating: 4,
    content: '整体不错，就是配送时间比预期的晚了一点，花还是很新鲜的。',
    isAnonymous: false,
    likes: 3,
    createdAt: Date.now() - 86400000 * 10,
    reply: '感谢您的反馈，我们会努力改进配送效率，期待您的再次光临！',
    replyTime: Date.now() - 86400000 * 9,
  },
  {
    id: generateGuid(),
    productId: '550e8400-e29b-41d4-a716-446655440002',
    userId: generateGuid(),
    userName: '赵***',
    rating: 5,
    content: '送给妈妈的生日礼物，妈妈很开心，花束很漂亮！',
    images: ['/static/images/reviews/review2-1.jpg'],
    isAnonymous: false,
    likes: 15,
    createdAt: Date.now() - 86400000 * 1,
  },
];

/** Mock 商品规格 */
export const MockSpecs: ISpecGroup[] = [
  {
    id: generateGuid(),
    name: '尺寸',
    required: true,
    options: [
      { id: generateGuid(), name: '标准款', value: '标准款', stock: 50, priceAdjust: 0 },
      { id: generateGuid(), name: '豪华款', value: '豪华款', stock: 30, priceAdjust: 50 },
      { id: generateGuid(), name: '至尊款', value: '至尊款', stock: 20, priceAdjust: 100 },
    ],
  },
  {
    id: generateGuid(),
    name: '包装',
    required: true,
    options: [
      { id: generateGuid(), name: '普通包装', value: '普通包装', stock: 100, priceAdjust: 0 },
      { id: generateGuid(), name: '精美礼盒', value: '精美礼盒', stock: 50, priceAdjust: 30 },
      { id: generateGuid(), name: '高档礼盒', value: '高档礼盒', stock: 30, priceAdjust: 60 },
    ],
  },
  {
    id: generateGuid(),
    name: '贺卡',
    required: false,
    options: [
      { id: generateGuid(), name: '不需要', value: '不需要', stock: 999, priceAdjust: 0 },
      { id: generateGuid(), name: '免费贺卡', value: '免费贺卡', stock: 200, priceAdjust: 0 },
      { id: generateGuid(), name: '精美贺卡', value: '精美贺卡', stock: 100, priceAdjust: 5 },
    ],
  },
];

/** Mock 商品媒体（图片+视频） */
const createMockMedia = (hasVideo: boolean = true): IProductMedia[] => {
  const media: IProductMedia[] = [
    {
      id: generateGuid(),
      type: 'image',
      url: '/static/images/products/rose-red.jpg',
    },
    {
      id: generateGuid(),
      type: 'image',
      url: '/static/images/products/rose-red-1.jpg',
    },
    {
      id: generateGuid(),
      type: 'image',
      url: '/static/images/products/rose-red-2.jpg',
    },
  ];

  if (hasVideo) {
    media.splice(1, 0, {
      id: generateGuid(),
      type: 'video',
      url: '/static/videos/rose-red.mp4',
      thumbnail: '/static/images/products/rose-red-video-cover.jpg',
      duration: 15,
    });
  }

  return media;
};

/** Mock 商品列表 */
export const MockProducts = {
  list: [
    {
      id: '550e8400-e29b-41d4-a716-446655440001',
      name: '红玫瑰鲜花束',
      description: '11支红玫瑰，配满天星和绿叶',
      price: 199.00,
      originalPrice: 299.00,
      image: '/static/images/products/rose-red.jpg',
      images: ['/static/images/products/rose-red.jpg', '/static/images/products/rose-red-1.jpg', '/static/images/products/rose-red-2.jpg'],
      categoryId: '550e8400-e29b-41d4-a716-446655440101',
      stock: 100,
      sales: 528,
      isHot: true,
      isNew: false,
      createdAt: Date.now() - 86400000 * 30,
    },
    {
      id: '550e8400-e29b-41d4-a716-446655440002',
      name: '粉色康乃馨花束',
      description: '20支粉色康乃馨，温馨祝福',
      price: 128.00,
      originalPrice: 168.00,
      image: '/static/images/products/carnation-pink.jpg',
      categoryId: '550e8400-e29b-41d4-a716-446655440102',
      stock: 80,
      sales: 312,
      isHot: false,
      isNew: true,
      createdAt: Date.now() - 86400000 * 7,
    },
    {
      id: '550e8400-e29b-41d4-a716-446655440003',
      name: '白色百合花束',
      description: '6支白色百合，高雅纯洁',
      price: 158.00,
      originalPrice: 198.00,
      image: '/static/images/products/lily-white.jpg',
      categoryId: '550e8400-e29b-41d4-a716-446655440103',
      stock: 50,
      sales: 186,
      isHot: true,
      isNew: false,
      createdAt: Date.now() - 86400000 * 60,
    },
    {
      id: '550e8400-e29b-41d4-a716-446655440004',
      name: '向日葵花束',
      description: '5支向日葵，阳光活力',
      price: 98.00,
      originalPrice: 128.00,
      image: '/static/images/products/sunflower.jpg',
      categoryId: '550e8400-e29b-41d4-a716-446655440104',
      stock: 60,
      sales: 234,
      isHot: false,
      isNew: true,
      createdAt: Date.now() - 86400000 * 3,
    },
    {
      id: '550e8400-e29b-41d4-a716-446655440005',
      name: '混合鲜花束',
      description: '玫瑰、康乃馨、百合混合搭配',
      price: 268.00,
      originalPrice: 358.00,
      image: '/static/images/products/mixed.jpg',
      categoryId: '550e8400-e29b-41d4-a716-446655440105',
      stock: 30,
      sales: 89,
      isHot: true,
      isNew: true,
      createdAt: Date.now() - 86400000 * 2,
    },
    {
      id: '550e8400-e29b-41d4-a716-446655440006',
      name: '蓝色妖姬花束',
      description: '10支蓝色妖姬玫瑰，独特魅力',
      price: 388.00,
      originalPrice: 488.00,
      image: '/static/images/products/blue-rose.jpg',
      categoryId: '550e8400-e29b-41d4-a716-446655440101',
      stock: 20,
      sales: 156,
      isHot: false,
      isNew: false,
      createdAt: Date.now() - 86400000 * 45,
    },
  ] as IProduct[],

  categories: [
    { id: '550e8400-e29b-41d4-a716-446655440101', name: '玫瑰', icon: '/static/images/icons/rose.png', sort: 1 },
    { id: '550e8400-e29b-41d4-a716-446655440102', name: '康乃馨', icon: '/static/images/icons/carnation.png', sort: 2 },
    { id: '550e8400-e29b-41d4-a716-446655440103', name: '百合', icon: '/static/images/icons/lily.png', sort: 3 },
    { id: '550e8400-e29b-41d4-a716-446655440104', name: '向日葵', icon: '/static/images/icons/sunflower.png', sort: 4 },
    { id: '550e8400-e29b-41d4-a716-446655440105', name: '混合花束', icon: '/static/images/icons/mixed.png', sort: 5 },
  ],

  defaultProduct: {
    id: '550e8400-e29b-41d4-a716-446655440000',
    name: '默认商品',
    price: 0,
    image: '/static/images/products/default.jpg',
    categoryId: '550e8400-e29b-41d4-a716-446655440101',
    stock: 0,
  } as IProduct,

  /** 获取商品详情 */
  getProductDetail(id: string): IProductDetail | null {
    const product = this.list.find(p => p.id === id);
    if (!product) return null;

    const detail: IProductDetail = {
      ...product,
      media: createMockMedia(true),
      specs: MockSpecs,
      coupons: MockCoupons.filter(c => c.status === 'available'),
      reviewSummary: {
        total: MockReviews.filter(r => r.productId === id).length,
        avgRating: 4.8,
        goodRate: 96,
      },
      services: ['新鲜保证', '同城配送', '极速退款', '品质保障'],
      relatedProducts: this.list.filter(p => p.categoryId === product.categoryId && p.id !== id).slice(0, 4),
      detail: `
        <h4>商品详情</h4>
        <p>精选优质鲜花，每日新鲜采摘，保证花朵饱满、色泽鲜艳。</p>
        <p>专业花艺师精心设计，每一束都充满爱意。</p>
        <p>适合送礼：生日、纪念日、告白、节日等场合。</p>
      `,
    };

    return detail;
  },

  /** 获取商品评价 */
  getProductReviews(productId: string, pageIndex: number = 1, pageSize: number = 10): { list: IProductReview[]; total: number } {
    const allReviews = MockReviews.filter(r => r.productId === productId);
    const start = (pageIndex - 1) * pageSize;
    return {
      list: allReviews.slice(start, start + pageSize),
      total: allReviews.length,
    };
  },
};