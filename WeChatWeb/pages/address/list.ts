// pages/address/list.ts

import { addressService } from '../../services/index';
import { IAddress } from '../../types/index';

interface IListPageData {
  addresses: IAddress[];
  loading: boolean;
  selectMode: boolean;
}

Page<IListPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    addresses: [],
    loading: true,
    selectMode: false,
  },

  async onLoad(options: { select?: string }) {
    this.setData({ selectMode: options.select === 'true' });
    await this.loadAddresses();
  },

  async onShow() {
    await this.loadAddresses();
  },

  async loadAddresses() {
    this.setData({ loading: true });
    const addresses = await addressService.getAddressList();
    this.setData({ addresses, loading: false });
  },

  // 选择地址（支付页调用）
  onSelectAddress(e: WechatMiniprogram.TouchEvent) {
    const { index } = e.currentTarget.dataset;
    if (this.data.selectMode) {
      const address = this.data.addresses[index];
      const pages = getCurrentPages();
      const prevPage = pages[pages.length - 2] as any;
      if (prevPage) {
        prevPage.setData({ selectedAddress: address });
        wx.navigateBack();
      }
    }
  },

  // 新增地址
  onAddAddress() {
    wx.navigateTo({ url: '/pages/address/edit' });
  },

  // 编辑地址
  onEditAddress(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    wx.navigateTo({ url: `/pages/address/edit?id=${id}` });
  },

  // 删除地址
  async onDeleteAddress(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    wx.showModal({
      title: '提示',
      content: '确定要删除这个地址吗？',
      success: async (res) => {
        if (res.confirm) {
          await addressService.deleteAddress(id);
          this.loadAddresses();
        }
      },
    });
  },

  // 设为默认
  async onSetDefault(e: WechatMiniprogram.TouchEvent) {
    const { index } = e.currentTarget.dataset;
    const address = this.data.addresses[index];
    await addressService.saveAddress({ ...address, isDefault: true });
    this.loadAddresses();
  },
});