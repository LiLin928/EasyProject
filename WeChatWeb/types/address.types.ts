// types/address.types.ts

import { IBaseEntity } from './common.types';

/** 地址信息 */
export interface IAddress extends IBaseEntity {
  id?: string;
  name: string; // 收货人姓名
  phone: string; // 电话
  province: string; // 省
  city: string; // 市
  district: string; // 区
  detail: string; // 详细地址
  isDefault?: boolean; // 是否默认地址
  fullAddress?: string; // 完整地址（拼接）
}

/** 地址验证结果 */
export interface IAddressValidation {
  valid: boolean;
  errors: string[];
}