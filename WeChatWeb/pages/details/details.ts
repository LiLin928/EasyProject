// pages/details/details.ts

import { productService, cartService, authService } from '../../services/index';
import { IProductDetail, IProductReview, ICoupon, ISpecGroup, ISpecSelection } from '../../types/index';

interface IDetailsPageData {
  product: IProductDetail | null;
  currentMediaIndex: number;
  count: number;
  loading: boolean;
  showSpecPopup: boolean;
  selectedSpecs: ISpecSelection[];
  currentPrice: number;
  currentStock: number;
  reviews: IProductReview[];
  reviewSummary: { total: number; avgRating: number; goodRate: number } | null;
  coupons: ICoupon[];
  showCouponPopup: boolean;
}

Page<IDetailsPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    product: null,
    currentMediaIndex: 0,
    count: 1,
    loading: true,
    showSpecPopup: false,
    selectedSpecs: [],
    currentPrice: 0,
    currentStock: 0,
    reviews: [],
    reviewSummary: null,
    coupons: [],
    showCouponPopup: false,
  },

  onLoad(options: { id: string }) {
    if (options.id) {
      this.loadProduct(options.id);
    }
  },

  async loadProduct(id: string) {
    this.setData({ loading: true });

    try {
      const product = await productService.getProductDetail(id);
      wx.setNavigationBarTitle({ title: product.name });

      // 计算当前价格和库存
      const currentPrice = product.price;
      const currentStock = product.stock;

      // 获取评价
      const reviewsResult = await productService.getProductReviews({
        productId: id,
        pageIndex: 1,
        pageSize: 3,
      });

      // 获取优惠券
      const coupons = await productService.getAvailableCoupons(id);

      this.setData({
        product,
        currentPrice,
        currentStock,
        reviews: reviewsResult.list,
        reviewSummary: product.reviewSummary || null,
        coupons,
        loading: false,
      });
    } catch (error) {
      console.error('加载商品详情失败:', error);
      wx.showToast({ title: '加载失败', icon: 'none' });
      this.setData({ loading: false });
    }
  },

  // 切换轮播图
  onMediaChange(e: WechatMiniprogram.SwiperChange) {
    this.setData({ currentMediaIndex: e.detail.current });
  },

  // 预览图片
  onPreviewImage(e: WechatMiniprogram.TouchEvent) {
    const { src } = e.currentTarget.dataset;
    const product = this.data.product;
    if (product && product.images) {
      wx.previewImage({
        current: src,
        urls: product.images,
      });
    }
  },

  // 分享
  onShare() {
    // 微信小程序分享通过 onShareAppMessage 实现
    wx.showShareMenu({
      withShareTicket: true,
      menus: ['shareAppMessage', 'shareTimeline'],
    });
  },

  // 显示优惠券弹窗
  onShowCouponPopup() {
    this.setData({ showCouponPopup: true });
  },

  // 关闭优惠券弹窗
  onCloseCouponPopup() {
    this.setData({ showCouponPopup: false });
  },

  // 领取优惠券
  async onClaimCoupon(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    const success = await productService.claimCoupon(id);
    if (success) {
      wx.showToast({ title: '领取成功', icon: 'success' });
      // 更新优惠券状态
      const coupons = this.data.coupons.map(c =>
        c.id === id ? { ...c, status: 'used' as const } : c
      );
      this.setData({ coupons });
    } else {
      wx.showToast({ title: '领取失败', icon: 'none' });
    }
  },

  // 显示规格弹窗
  onShowSpecPopup() {
    this.setData({ showSpecPopup: true });
  },

  // 关闭规格弹窗
  onCloseSpecPopup() {
    this.setData({ showSpecPopup: false });
  },

  // 选择规格
  onSelectSpec(e: WechatMiniprogram.TouchEvent) {
    const { groupId, optionId, optionName, value, priceAdjust } = e.currentTarget.dataset;
    const { selectedSpecs, product } = this.data;

    // 更新选中的规格
    const newSpecs = selectedSpecs.filter(s => s.groupId !== groupId);
    newSpecs.push({ groupId, optionId, optionName, value });

    // 计算价格和库存
    let priceAdjustTotal = 0;
    newSpecs.forEach(spec => {
      const group = product?.specs?.find(g => g.id === spec.groupId);
      const option = group?.options.find(o => o.id === spec.optionId);
      if (option?.priceAdjust) {
        priceAdjustTotal += option.priceAdjust;
      }
    });

    const currentPrice = (product?.price || 0) + priceAdjustTotal;

    this.setData({
      selectedSpecs: newSpecs,
      currentPrice,
    });
  },

  // 数量增加
  onCountIncrease() {
    const { count, currentStock } = this.data;
    if (count < currentStock) {
      this.setData({ count: count + 1 });
    } else {
      wx.showToast({ title: '已达最大库存', icon: 'none' });
    }
  },

  // 数量减少
  onCountDecrease() {
    const { count } = this.data;
    if (count > 1) {
      this.setData({ count: count - 1 });
    }
  },

  // 加入购物车
  async onAddToCart() {
    if (!authService.requireLogin()) return;

    const { product, count } = this.data;
    if (product) {
      await cartService.addToCart(product.id, count);
      wx.showToast({ title: '已加入购物车', icon: 'success' });
    }
  },

  // 立即购买
  async onBuyNow() {
    if (!authService.requireLogin()) return;

    const { product, count } = this.data;
    if (product) {
      try {
        // 添加到购物车（不显示 toast），返回新添加的购物车项
        const newItem = await cartService.addToCart(product.id, count, false);

        // 取消所有选中，只选中刚添加的商品
        cartService.toggleSelectAll(false);
        cartService.toggleSelect(newItem.id);

        console.log('立即购买：选中商品', newItem.id);

        // 跳转到结算页
        wx.navigateTo({ url: '/pages/pay/pay' });
      } catch (error) {
        console.error('立即购买失败:', error);
        wx.showToast({ title: '添加失败', icon: 'none' });
      }
    }
  },

  // 跳转购物车页面
  onGoToCart() {
    wx.switchTab({ url: '/pages/shopping/shopping' });
  },

  // 跳转客服
  onContactService() {
    wx.navigateTo({ url: '/pages/chat/chat' });
  },

  // 查看全部评价
  onViewAllReviews() {
    const { product } = this.data;
    if (product) {
      wx.navigateTo({ url: `/pages/reviews/list?productId=${product.id}` });
    }
  },

  // 分享配置
  onShareAppMessage() {
    const { product } = this.data;
    if (product) {
      return {
        title: product.name,
        path: `/pages/details/details?id=${product.id}`,
        imageUrl: product.image,
      };
    }
    return {};
  },

  // 分享到朋友圈
  onShareTimeline() {
    const { product } = this.data;
    if (product) {
      return {
        title: `${product.name} - 仅需¥${product.price}`,
        query: `id=${product.id}`,
        imageUrl: product.image,
      };
    }
    return {};
  },
});