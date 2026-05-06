// types/cart.types.ts

import { IProduct } from './product.types';
import { IBaseEntity } from './common.types';

/** 购物车项 */
export interface ICartItem extends IBaseEntity {
  id: string;
  productId: string;
  product: IProduct; // 关联商品信息
  count: number;
  selected?: boolean; // 是否选中（用于结算）
}

/** 购物车状态 */
export interface ICartState {
  items: ICartItem[];
  totalCount: number;
  totalPrice: number;
  selectedCount: number;
  selectedPrice: number;
}