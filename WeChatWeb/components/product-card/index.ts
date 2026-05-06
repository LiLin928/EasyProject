// components/product-card/index.ts

import { IProduct } from '../../types/index';

Component({
  properties: {
    product: {
      type: Object,
      value: {},
    },
    showPrice: {
      type: Boolean,
      value: true,
    },
    showSales: {
      type: Boolean,
      value: true,
    },
  },

  methods: {
    onTap() {
      const product = this.properties.product as IProduct;
      if (product && product.id) {
        wx.navigateTo({ url: `/pages/details/details?id=${product.id}` });
      }
    },
  },
});