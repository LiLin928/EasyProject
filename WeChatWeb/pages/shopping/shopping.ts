// pages/shopping/shopping.ts

import { cartService, authService } from '../../services/index';
import { cartStore } from '../../stores/index';
import { ICartState, ICartItem } from '../../types/index';

interface IShoppingPageData extends ICartState {
  isEditMode: boolean;
  allSelected: boolean;
}

Page<IShoppingPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    items: [],
    totalCount: 0,
    totalPrice: 0,
    selectedCount: 0,
    selectedPrice: 0,
    isEditMode: false,
    allSelected: false,
  },

  unsubscribe: null as (() => void) | null,

  onLoad() {
    // 订阅购物车状态变化
    this.unsubscribe = cartStore.subscribe((state) => {
      const allSelected = state.items.length > 0 && state.items.every(item => item.selected);
      this.setData({ ...state, allSelected });
    });

    // 设置初始状态
    const state = cartStore.getState();
    const allSelected = state.items.length > 0 && state.items.every(item => item.selected);
    this.setData({ ...state, allSelected });
  },

  onShow() {
    // 每次显示页面时刷新购物车
    if (authService.checkLogin()) {
      cartService.initCart();
    }
  },

  onUnload() {
    if (this.unsubscribe) {
      this.unsubscribe();
    }
  },

  // 切换编辑模式
  onToggleEdit() {
    this.setData({ isEditMode: !this.data.isEditMode });
  },

  // 去逛逛（跳转首页）
  onGoShopping() {
    wx.switchTab({ url: '/pages/index/index' });
  },

  // 切换选中
  onSelect(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    cartService.toggleSelect(id);
  },

  // 全选/取消全选
  onSelectAll() {
    const { allSelected } = this.data;
    cartService.toggleSelectAll(!allSelected);
  },

  // 增加数量
  async onIncrease(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    const item = this.data.items.find(i => i.id === id);
    if (item) {
      await cartService.updateItemCount(id, item.count + 1);
    }
  },

  // 减少数量
  async onDecrease(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    const item = this.data.items.find(i => i.id === id);
    if (item && item.count > 1) {
      await cartService.updateItemCount(id, item.count - 1);
    }
  },

  // 删除商品
  async onDelete(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    wx.showModal({
      title: '提示',
      content: '确定要删除这个商品吗？',
      success: async (res) => {
        if (res.confirm) {
          await cartService.removeItem(id);
        }
      },
    });
  },

  // 去结算
  onCheckout() {
    if (!authService.requireLogin()) return;

    const selectedItems = cartService.getSelectedItems();
    if (selectedItems.length === 0) {
      wx.showToast({ title: '请选择商品', icon: 'none' });
      return;
    }

    wx.navigateTo({ url: '/pages/pay/pay' });
  },

  // 点击商品跳转详情
  onItemTap(e: WechatMiniprogram.TouchEvent) {
    const { productId } = e.currentTarget.dataset;
    wx.navigateTo({ url: `/pages/details/details?id=${productId}` });
  },

  // 清空购物车
  async onClearCart() {
    wx.showModal({
      title: '提示',
      content: '确定要清空购物车吗？',
      success: async (res) => {
        if (res.confirm) {
          await cartService.clearCart();
        }
      },
    });
  },
});