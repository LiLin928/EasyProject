// app.ts

App({
  globalData: {
    userInfo: null,
    isLoggedIn: false,
    selectedCategoryId: '', // 用于跨页面传递分类ID
    tokenChecked: false, // 标记 token 是否已检查完成
  },

  onLaunch() {
    // 小程序启动时执行
    console.log('App launched');

    // 检查登录状态
    this.checkLoginStatus();
  },

  /** 检查登录状态 */
  checkLoginStatus() {
    const token = wx.getStorageSync('wechat_mall_token');

    if (token) {
      this.globalData.isLoggedIn = true;
      this.globalData.tokenChecked = true;
      console.log('已登录，token 存在');
      return true;
    }

    this.globalData.isLoggedIn = false;
    this.globalData.tokenChecked = true;
    console.log('未登录，token 不存在');
    return false;
  },

  /** 检查是否需要登录（供页面调用） */
  requireLogin(): boolean {
    const token = wx.getStorageSync('wechat_mall_token');

    if (!token) {
      wx.showModal({
        title: '提示',
        content: '请先登录后再访问',
        confirmText: '去登录',
        showCancel: false,
        success: (res) => {
          if (res.confirm) {
            wx.redirectTo({ url: '/pages/login/login' });
          }
        },
      });
      return false;
    }

    return true;
  },

  onShow() {
    // 小程序显示时执行
    console.log('App shown');
  },

  onHide() {
    // 小程序隐藏时执行
    console.log('App hidden');
  },
});