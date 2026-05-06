// pages/index/index.ts

import { productService } from '../../services/index';
import { IProduct, ICategory } from '../../types/index';

// 声明全局 App 类型
interface IAppOption {
  globalData: {
    userInfo: any;
    isLoggedIn: boolean;
    selectedCategoryId: string;
    tokenChecked: boolean;
  };
  requireLogin(): boolean;
}

interface IIndexPageData {
  bannerList: string[];
  categories: ICategory[];
  hotProducts: IProduct[];
  newProducts: IProduct[];
  loading: boolean;
}

Page<IIndexPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    bannerList: [],
    categories: [],
    hotProducts: [],
    newProducts: [],
    loading: true,
  },

  onLoad() {
    // 先检查登录状态，有 token 才加载数据
    const app = getApp<IAppOption>();
    const token = wx.getStorageSync('wechat_mall_token');

    if (!token) {
      console.log('首页：无 token，跳转登录页');
      wx.redirectTo({ url: '/pages/login/login' });
      return;
    }

    console.log('首页：token 存在，开始加载数据');
    this.loadData();
  },

  /** 从登录页返回后重新加载 */
  onShow() {
    // 检查是否刚登录成功返回
    const token = wx.getStorageSync('wechat_mall_token');
    const app = getApp<IAppOption>();

    // 如果有 token 但数据还是空的，重新加载
    if (token && this.data.categories.length === 0) {
      console.log('首页 onShow：重新加载数据');
      this.loadData();
    }
  },

  async loadData() {
    this.setData({ loading: true });

    try {
      // 轮播图
      const bannerList = [
        '/static/images/banners/banner1.jpg',
        '/static/images/banners/banner2.jpg',
        '/static/images/banners/banner3.jpg',
      ];

      // 获取分类
      console.log('首页：获取分类...');
      const categories = await productService.getCategories();
      console.log('首页：分类获取成功', categories.length);

      // 获取热销和新品
      console.log('首页：获取热销商品...');
      const hotProducts = await productService.getHotProducts(6);
      console.log('首页：获取新品商品...');
      const newProducts = await productService.getNewProducts(6);

      this.setData({
        bannerList,
        categories,
        hotProducts,
        newProducts,
        loading: false,
      });

      console.log('首页：数据加载完成');
    } catch (error) {
      console.error('加载首页数据失败:', error);
      wx.showToast({ title: '加载失败', icon: 'none' });
      this.setData({ loading: false });
    }
  },

  // 搜索
  onSearch() {
    wx.navigateTo({ url: '/pages/search/search' });
  },

  // 点击分类
  onCategoryTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    const app = getApp<IAppOption>();

    // 设置全局变量传递分类ID
    app.globalData.selectedCategoryId = id || '';

    // 使用 switchTab 跳转到分类页（TabBar页面必须用switchTab）
    wx.switchTab({ url: '/pages/classify/classify' });
  },

  // 查看更多（跳转到分类页）
  onViewMoreHot() {
    wx.switchTab({ url: '/pages/classify/classify' });
  },

  // 点击商品
  onProductTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    wx.navigateTo({ url: `/pages/details/details?id=${id}` });
  },

  // 下拉刷新
  onPullDownRefresh() {
    this.loadData().then(() => {
      wx.stopPullDownRefresh();
    });
  },
});