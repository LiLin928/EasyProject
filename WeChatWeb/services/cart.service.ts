// services/cart.service.ts

import { adapter } from '../adapters/adapter.config';
import { cartStore } from '../stores/index';
import { ICartItem } from '../types/index';

export class CartService {
  private static instance: CartService;

  static getInstance(): CartService {
    if (!CartService.instance) {
      CartService.instance = new CartService();
    }
    return CartService.instance;
  }

  /** 初始化购物车 */
  async initCart(): Promise<void> {
    const items = await adapter.getCartList();
    cartStore.setItems(items.map(item => ({ ...item, selected: true })));
  }

  /** 添加商品到购物车 */
  async addToCart(productId: string, count: number = 1, showToast: boolean = true): Promise<ICartItem> {
    // 后端返回的是购物车项 ID，需要重新获取购物车列表
    await adapter.addToCart(productId, count);

    // 重新获取购物车列表
    const items = await adapter.getCartList();
    cartStore.setItems(items.map(item => ({ ...item, selected: true })));

    // 返回新添加的购物车项
    const newItem = items.find(item => item.productId === productId);
    if (newItem) {
      if (showToast) {
        wx.showToast({ title: '已添加到购物车', icon: 'success' });
      }
      return newItem;
    }

    // 如果找不到，返回一个临时对象
    if (showToast) {
      wx.showToast({ title: '已添加到购物车', icon: 'success' });
    }
    return { id: '', productId, product: null as any, count };
  }

  /** 更新购物车商品数量 */
  async updateItemCount(itemId: string, count: number): Promise<ICartItem> {
    if (count <= 0) {
      await this.removeItem(itemId);
      return null!;
    }

    const cartItem = await adapter.updateCartItem(itemId, count);
    cartStore.updateItemCount(itemId, count);
    return cartItem;
  }

  /** 删除购物车商品 */
  async removeItem(itemId: string): Promise<void> {
    await adapter.removeFromCart(itemId);
    cartStore.removeItem(itemId);

    wx.showToast({ title: '已删除', icon: 'success' });
  }

  /** 清空购物车 */
  async clearCart(): Promise<void> {
    await adapter.clearCart();
    cartStore.clear();
  }

  /** 切换选中状态 */
  toggleSelect(itemId: string): void {
    cartStore.toggleSelect(itemId);
  }

  /** 全选/取消全选 */
  toggleSelectAll(selected: boolean): void {
    cartStore.toggleSelectAll(selected);
  }

  /** 获取选中商品 */
  getSelectedItems(): ICartItem[] {
    return cartStore.getSelectedItems();
  }

  /** 获取购物车统计 */
  getCartSummary(): { count: number; price: number } {
    const state = cartStore.getState();
    return {
      count: state.selectedCount,
      price: state.selectedPrice,
    };
  }
}

export const cartService = CartService.getInstance();