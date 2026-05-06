// pages/address/edit.ts

import { addressService } from '../../services/index';
import { IAddress } from '../../types/index';
import { Validator } from '../../utils/index';

interface IEditPageData {
  address: IAddress;
  isEdit: boolean;
}

Page<IEditPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    address: {
      name: '',
      phone: '',
      province: '',
      city: '',
      district: '',
      detail: '',
      isDefault: false,
    },
    isEdit: false,
  },

  async onLoad(options: { id?: string }) {
    if (options.id) {
      const address = await addressService.getAddressDetail(options.id);
      this.setData({ address, isEdit: true });
      wx.setNavigationBarTitle({ title: '编辑地址' });
    }
  },

  // 输入姓名
  onNameInput(e: WechatMiniprogram.Input) {
    this.setData({ 'address.name': e.detail.value });
  },

  // 输入手机号
  onPhoneInput(e: WechatMiniprogram.Input) {
    this.setData({ 'address.phone': e.detail.value });
  },

  // 选择地区
  onRegionChange(e: WechatMiniprogram.PickerChange) {
    const region = e.detail.value as string[];
    this.setData({
      'address.province': region[0],
      'address.city': region[1],
      'address.district': region[2],
    });
  },

  // 输入详细地址
  onDetailInput(e: WechatMiniprogram.Input) {
    this.setData({ 'address.detail': e.detail.value });
  },

  // 切换默认
  onDefaultChange(e: WechatMiniprogram.SwitchChange) {
    this.setData({ 'address.isDefault': e.detail.value });
  },

  // 保存地址
  async onSave() {
    const { address } = this.data;

    // 验证
    if (!address.name.trim()) {
      wx.showToast({ title: '请输入收货人姓名', icon: 'none' });
      return;
    }

    if (!Validator.phone(address.phone)) {
      wx.showToast({ title: '请输入正确的手机号', icon: 'none' });
      return;
    }

    if (!address.province || !address.city || !address.district) {
      wx.showToast({ title: '请选择地区', icon: 'none' });
      return;
    }

    if (!address.detail.trim()) {
      wx.showToast({ title: '请输入详细地址', icon: 'none' });
      return;
    }

    try {
      await addressService.saveAddress(address);
      wx.navigateBack();
    } catch (error) {
      wx.showToast({ title: '保存失败', icon: 'none' });
    }
  },
});