// pages/pay/pay.ts

import { cartService, addressService, orderService, authService } from '../../services/index';
import { IAddress, ICartItem } from '../../types/index';

interface IPayPageData {
  selectedItems: ICartItem[];
  totalPrice: number;
  address: IAddress | null;
  remark: string;
  loading: boolean;
}

Page<IPayPageData, WechatMiniprogram.Page.CustomOption>({
  data: {
    selectedItems: [],
    totalPrice: 0,
    address: null,
    remark: '',
    loading: false,
  },

  onLoad() {
    if (!authService.requireLogin()) {
      wx.navigateBack();
      return;
    }
    this.loadData();
  },

  async loadData() {
    const selectedItems = cartService.getSelectedItems();
    const summary = cartService.getCartSummary();

    await addressService.initAddresses();
    const address = addressService.getDefaultAddress();

    if (selectedItems.length === 0) {
      wx.showToast({ title: '请选择商品', icon: 'none' });
      wx.navigateBack();
      return;
    }

    this.setData({
      selectedItems,
      totalPrice: summary.price,
      address,
    });
  },

  // 选择地址（有地址跳转列表选择，无地址跳转新增）
  onSelectAddress() {
    if (this.data.address) {
      // 有地址，跳转列表页选择
      wx.navigateTo({ url: '/pages/address/list?select=true' });
    } else {
      // 无地址，跳转新增页面
      wx.navigateTo({ url: '/pages/address/edit?from=pay' });
    }
  },

  // 输入备注
  onRemarkInput(e: WechatMiniprogram.Input) {
    this.setData({ remark: e.detail.value });
  },

  // 提交订单
  async onSubmitOrder() {
    const { address, remark, loading } = this.data;

    if (loading) return;

    if (!address) {
      wx.showToast({ title: '请选择收货地址', icon: 'none' });
      return;
    }

    this.setData({ loading: true });

    try {
      const order = await orderService.createOrder(address.id!, remark);
      wx.redirectTo({ url: `/pages/pay/result?orderId=${order.id}` });
    } catch (error) {
      console.error('下单失败:', error);
      wx.showToast({ title: '下单失败', icon: 'none' });
    } finally {
      this.setData({ loading: false });
    }
  },

  // 页面显示时刷新地址（从地址编辑/选择页返回）
  async onShow() {
    const pages = getCurrentPages();
    const currentPage = pages[pages.length - 1] as any;

    // 情况1: 从地址列表页选择返回（有 selectedAddress）
    if (currentPage.data?.selectedAddress) {
      this.setData({ address: currentPage.data.selectedAddress });
      return;
    }

    // 情况2: 从地址编辑页返回（新增/修改地址后）
    // 重新加载地址列表并获取默认地址
    await addressService.initAddresses();
    const defaultAddress = addressService.getDefaultAddress();
    if (defaultAddress) {
      this.setData({ address: defaultAddress });
    }
  },
});