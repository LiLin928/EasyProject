// pages/search/search.ts

import { productService } from '../../services/index';
import { IProduct } from '../../types/index';

interface ISearchPageData {
  keyword: string;
  searchResults: IProduct[];
  hotKeywords: string[];
  searchHistory: string[];
  loading: boolean;
  searched: boolean;
}

Page<ISearchPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    keyword: '',
    searchResults: [],
    hotKeywords: ['玫瑰', '康乃馨', '百合', '向日葵', '生日花束'],
    searchHistory: [],
    loading: false,
    searched: false,
  },

  onLoad() {
    this.loadSearchHistory();
  },

  loadSearchHistory() {
    const history = wx.getStorageSync('wechat_mall_search_history') || [];
    this.setData({ searchHistory: history });
  },

  saveSearchHistory(keyword: string) {
    let history = this.data.searchHistory.filter(k => k !== keyword);
    history.unshift(keyword);
    history = history.slice(0, 10);
    wx.setStorageSync('wechat_mall_search_history', history);
    this.setData({ searchHistory: history });
  },

  clearHistory() {
    wx.removeStorageSync('wechat_mall_search_history');
    this.setData({ searchHistory: [] });
  },

  // 输入关键词
  onInput(e: WechatMiniprogram.Input) {
    this.setData({ keyword: e.detail.value });
  },

  // 清空输入
  onClearInput() {
    this.setData({ keyword: '', searchResults: [], searched: false });
  },

  // 搜索
  async onSearch() {
    const { keyword } = this.data;
    if (!keyword.trim()) {
      wx.showToast({ title: '请输入搜索内容', icon: 'none' });
      return;
    }

    this.setData({ loading: true, searched: true });
    this.saveSearchHistory(keyword);

    try {
      const results = await productService.searchProducts(keyword);
      this.setData({ searchResults: results, loading: false });
    } catch (error) {
      console.error('搜索失败:', error);
      this.setData({ loading: false });
    }
  },

  // 点击热门关键词
  onHotKeywordTap(e: WechatMiniprogram.TouchEvent) {
    const { keyword } = e.currentTarget.dataset;
    this.setData({ keyword }, () => {
      this.onSearch();
    });
  },

  // 点击历史关键词
  onHistoryTap(e: WechatMiniprogram.TouchEvent) {
    const { keyword } = e.currentTarget.dataset;
    this.setData({ keyword }, () => {
      this.onSearch();
    });
  },

  // 点击商品
  onProductTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    wx.navigateTo({ url: `/pages/details/details?id=${id}` });
  },
});