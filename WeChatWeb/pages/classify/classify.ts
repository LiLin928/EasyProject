// pages/classify/classify.ts

import { productService } from '../../services/index';
import { ICategory, IProduct } from '../../types/index';

// 声明全局 App 类型
interface IAppOption {
  globalData: {
    userInfo: any;
    isLoggedIn: boolean;
    selectedCategoryId: string;
  };
}

interface IClassifyPageData {
  categories: ICategory[];
  currentCategoryId: string;
  products: IProduct[];
  loading: boolean;
}

Page<IClassifyPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    categories: [],
    currentCategoryId: '',
    products: [],
    loading: true,
  },

  async onLoad() {
    await this.loadCategories();
  },

  onShow() {
    // 检查是否有从首页传递过来的分类ID
    const app = getApp<IAppOption>();
    const categoryId = app.globalData.selectedCategoryId;

    if (categoryId) {
      // 清除全局变量，避免下次还使用
      app.globalData.selectedCategoryId = '';
      this.loadProducts(categoryId);
    } else if (this.data.currentCategoryId) {
      // 已有选中的分类，刷新商品
      this.loadProducts(this.data.currentCategoryId);
    } else if (this.data.categories.length > 0) {
      // 默认加载第一个分类
      this.loadProducts(this.data.categories[0].id);
    }
  },

  async loadCategories() {
    const categories = await productService.getCategories();
    this.setData({ categories });
  },

  async loadProducts(categoryId: string) {
    this.setData({ loading: true, currentCategoryId: categoryId });
    try {
      const result = await productService.getProductList({ pageIndex: 1, pageSize: 100 });
      const products = result.list.filter(p => p.categoryId === categoryId);
      this.setData({ products, loading: false });
    } catch (error) {
      console.error('加载商品失败:', error);
      this.setData({ loading: false });
    }
  },

  onCategorySelect(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    this.loadProducts(id);
  },

  onProductTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    wx.navigateTo({ url: `/pages/details/details?id=${id}` });
  },
});