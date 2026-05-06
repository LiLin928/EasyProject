// services/address.service.ts

import { adapter } from '../adapters/adapter.config';
import { IAddress } from '../types/index';
import { Validator } from '../utils/index';

export class AddressService {
  private static instance: AddressService;
  private addresses: IAddress[] = [];

  static getInstance(): AddressService {
    if (!AddressService.instance) {
      AddressService.instance = new AddressService();
    }
    return AddressService.instance;
  }

  /** 初始化地址列表 */
  async initAddresses(): Promise<void> {
    this.addresses = await adapter.getAddressList();
  }

  /** 获取地址列表 */
  async getAddressList(): Promise<IAddress[]> {
    this.addresses = await adapter.getAddressList();
    return this.addresses;
  }

  /** 获取地址详情 */
  async getAddressDetail(id: string): Promise<IAddress> {
    return await adapter.getAddressDetail(id);
  }

  /** 获取默认地址 */
  getDefaultAddress(): IAddress | undefined {
    return this.addresses.find(a => a.isDefault);
  }

  /** 保存地址 */
  async saveAddress(data: IAddress): Promise<IAddress> {
    if (!this.validateAddress(data)) {
      throw new Error('地址信息不完整');
    }

    const address = await adapter.saveAddress(data);
    await this.getAddressList();

    wx.showToast({ title: data.id ? '地址已更新' : '地址已添加', icon: 'success' });
    return address;
  }

  /** 删除地址 */
  async deleteAddress(id: string): Promise<void> {
    await adapter.deleteAddress(id);
    await this.getAddressList();
    wx.showToast({ title: '地址已删除', icon: 'success' });
  }

  /** 验证地址信息 */
  private validateAddress(data: IAddress): boolean {
    const requiredFields = ['name', 'phone', 'province', 'city', 'district', 'detail'] as const;
    return requiredFields.every(field => data[field]);
  }

  /** 验证手机号 */
  validatePhone(phone: string): boolean {
    return Validator.phone(phone);
  }
}

export const addressService = AddressService.getInstance();