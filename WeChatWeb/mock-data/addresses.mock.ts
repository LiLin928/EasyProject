// mock-data/addresses.mock.ts

import { IAddress } from '../types/index';

export const MockAddresses = {
  list: [
    {
      id: '550e8400-e29b-41d4-a716-446655440301',
      name: '张三',
      phone: '13800138001',
      province: '北京市',
      city: '北京市',
      district: '朝阳区',
      detail: '建国路88号SOHO现代城A座1001室',
      isDefault: true,
      createdAt: Date.now() - 86400000 * 10,
    },
    {
      id: '550e8400-e29b-41d4-a716-446655440302',
      name: '李四',
      phone: '13900139002',
      province: '上海市',
      city: '上海市',
      district: '浦东新区',
      detail: '陆家嘴环路1000号恒生银行大厦',
      isDefault: false,
      createdAt: Date.now() - 86400000 * 5,
    },
  ] as IAddress[],

  defaultAddress: {
    name: '',
    phone: '',
    province: '',
    city: '',
    district: '',
    detail: '',
    isDefault: false,
  } as IAddress,
};