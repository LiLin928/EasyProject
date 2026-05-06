// stores/cart.store.ts

import { BaseStore } from './base.store';
import { ICartItem, ICartState } from '../types/index';
import { StorageUtil } from '../utils/storage';

export class CartStore extends BaseStore<ICartState> {
  private static instance: CartStore;

  static getInstance(): CartStore {
    if (!CartStore.instance) {
      CartStore.instance = new CartStore();
    }
    return CartStore.instance;
  }

  protected getInitialState(): ICartState {
    const items = StorageUtil.get<ICartItem[]>('cartList') || [];
    return this.calculateState(items);
  }

  /** 设置购物车列表 */
  setItems(items: ICartItem[]): void {
    StorageUtil.set('cartList', items);
    this.setState(this.calculateState(items));
  }

  /** 添加商品 */
  addItem(item: ICartItem): void {
    const items = [...this.state.items];
    const existIndex = items.findIndex(i => i.productId === item.productId);

    if (existIndex !== -1) {
      items[existIndex].count += item.count;
    } else {
      items.push({ ...item, selected: true });
    }

    StorageUtil.set('cartList', items);
    this.setState(this.calculateState(items));
  }

  /** 更新商品数量 */
  updateItemCount(itemId: string, count: number): void {
    const items = this.state.items.map(item =>
      item.id === itemId ? { ...item, count } : item
    );
    StorageUtil.set('cartList', items);
    this.setState(this.calculateState(items));
  }

  /** 删除商品 */
  removeItem(itemId: string): void {
    const items = this.state.items.filter(item => item.id !== itemId);
    StorageUtil.set('cartList', items);
    this.setState(this.calculateState(items));
  }

  /** 切换选中状态 */
  toggleSelect(itemId: string): void {
    const items = this.state.items.map(item =>
      item.id === itemId ? { ...item, selected: !item.selected } : item
    );
    StorageUtil.set('cartList', items);
    this.setState(this.calculateState(items));
  }

  /** 全选/取消全选 */
  toggleSelectAll(selected: boolean): void {
    const items = this.state.items.map(item => ({ ...item, selected }));
    StorageUtil.set('cartList', items);
    this.setState(this.calculateState(items));
  }

  /** 清空购物车 */
  clear(): void {
    StorageUtil.remove('cartList');
    this.setState({
      items: [],
      totalCount: 0,
      totalPrice: 0,
      selectedCount: 0,
      selectedPrice: 0,
    });
  }

  /** 清空已选中商品 */
  clearSelected(): void {
    const items = this.state.items.filter(item => !item.selected);
    StorageUtil.set('cartList', items);
    this.setState(this.calculateState(items));
  }

  /** 获取选中的商品 */
  getSelectedItems(): ICartItem[] {
    return this.state.items.filter(item => item.selected);
  }

  /** 计算状态 */
  private calculateState(items: ICartItem[]): ICartState {
    const totalCount = items.reduce((sum, item) => sum + item.count, 0);
    const totalPrice = items.reduce((sum, item) => sum + (item.product?.price || 0) * item.count, 0);

    const selectedItems = items.filter(item => item.selected);
    const selectedCount = selectedItems.reduce((sum, item) => sum + item.count, 0);
    const selectedPrice = selectedItems.reduce((sum, item) => sum + (item.product?.price || 0) * item.count, 0);

    return { items, totalCount, totalPrice, selectedCount, selectedPrice };
  }
}

/** 导出单例实例 */
export const cartStore = CartStore.getInstance();