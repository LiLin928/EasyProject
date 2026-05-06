# CLAUDE.md - 微信小程序鲜花商城开发规范

> 本文件基于项目代码分析整理，是开发规范的确认版本

---

## 一、项目架构规范

### 1.1 整体架构

```
WeChatWeb/
├── pages/              # 页面层 - 小程序页面
├── components/         # 组件层 - 可复用组件
├── services/           # 服务层 - 业务逻辑封装
├── stores/             # 状态层 - 全局状态管理
├── adapters/           # 适配层 - Mock/API 数据切换
├── types/              # 类型层 - TypeScript 接口定义
├── utils/              # 工具层 - 通用工具函数
├── config/             # 配置层 - 环境和API配置
├── mock-data/          # Mock层 - 模拟数据
└── static/             # 静态层 - 图片/图标资源
```

### 1.2 分层职责

| 层级 | 目录 | 职责 | 依赖方向 |
|------|------|------|----------|
| 页面层 | pages | UI展示、用户交互 | → services, stores |
| 服务层 | services | 业务逻辑、数据获取 | → adapters, types |
| 适配层 | adapters | 数据源切换、字段转换 | → utils, types |
| 状态层 | stores | 全局状态、跨页面共享 | → types |
| 类型层 | types | 接口定义、类型约束 | 无依赖 |
| 工具层 | utils | 通用函数、存储封装 | → config |
| 配置层 | config | 环境配置、API路径 | 无依赖 |

---

## 二、命名规范

### 2.1 文件命名

| 类型 | 规范 | 示例 |
|------|------|------|
| 页面目录 | 小写单词 | `index/`, `details/`, `shopping/` |
| 页面文件 | 目录同名 | `index.ts`, `index.wxml` |
| 组件目录 | 小写+中划线 | `product-card/`, `order-item/` |
| 服务文件 | 功能.service.ts | `product.service.ts` |
| Store文件 | 功能.store.ts | `cart.store.ts` |
| 类型文件 | 功能.types.ts | `order.types.ts` |
| 工具文件 | 功能.ts | `formatter.ts`, `validator.ts` |
| Mock文件 | 功能.mock.ts | `products.mock.ts` |
| 配置文件 | 功能.config.ts 或 env.环境.ts | `api.config.ts`, `env.dev.ts` |

### 2.2 变量命名

| 类型 | 规范 | 示例 |
|------|------|------|
| 变量 | camelCase（小驼峰） | `productList`, `totalCount` |
| 函数 | camelCase | `loadData`, `onProductTap` |
| 常量 | UPPER_SNAKE_CASE | `BASE_URL`, `MAX_COUNT` |
| 私有变量 | _前缀camelCase | `_instance`, `_adapter` |
| 类型接口 | I前缀PascalCase | `IProduct`, `IOrderItem` |
| 类型别名 | PascalCase | `OrderStatus`, `MessageType` |
| Enum | PascalCase | `SenderType`, `OrderStatusEnum` |

### 2.3 接口字段命名

**全部使用 camelCase（小驼峰）命名**

```typescript
// ✅ 正确示例
interface IProduct {
  id: string;
  name: string;
  categoryId: string;
  originalPrice: number;
  isHot: boolean;
  isNew: boolean;
  createdAt: number;
  updatedAt: number;
}

interface IOrder {
  orderNo: string;
  totalAmount: number;
  paymentTime: number;
  deliveryTime: number;
  addressId: string;
}

interface IAddress {
  isDefault: boolean;
  fullAddress: string;
}

interface IUser {
  openId: string;
  avatarUrl: string;
  nickname: string;
}

// ❌ 错误示例（禁止使用）
interface IProduct {
  product_id: string;      // ❌ 下划线命名
  OriginalPrice: number;   // ❌ PascalCase
  is_hot: boolean;         // ❌ 下划线命名
}
```

---

## 三、代码规范

### 3.1 TypeScript 类型定义规范

#### 3.1.1 类型文件结构

```typescript
// types/product.types.ts

// 1. 导入依赖类型
import { IBaseEntity } from './common.types';

// 2. 定义枚举/类型别名
export type MediaType = 'image' | 'video';

// 3. 定义基础接口
export interface ICategory {
  id: string;
  name: string;
  icon?: string;
}

// 4. 定义扩展接口（继承基类）
export interface IProduct extends IBaseEntity {
  id: string;
  name: string;
  price: number;
  categoryId: string;
  // ...
}

// 5. 定义查询参数接口
export interface IProductQueryParams {
  pageIndex: number;
  pageSize: number;
  categoryId?: string;
}

// 6. 导出辅助函数（可选）
export function getOrderStatusText(status: number): string {
  return OrderStatusMap[status]?.text || '未知';
}
```

#### 3.1.2 接口命名前缀规范

| 前缀 | 用途 | 示例 |
|------|------|------|
| `I` | 数据实体接口 | `IProduct`, `IOrder` |
| `I...Params` | 请求参数接口 | `ICreateOrderParams`, `IProductQueryParams` |
| `I...Response` | 后端响应接口 | `ILoginResponse`, `IOrderResponse` |
| `I...Result` | 操作结果接口 | `IPaymentResult` |
| `I...State` | 状态接口 | `ICartState`, `IChatPageData` |

#### 3.1.3 可选字段标记

```typescript
// 使用 ? 标记可选字段
interface IAddress {
  id?: string;           // 新建时无ID
  isDefault?: boolean;   // 可选属性
  fullAddress?: string;  // 计算属性，非必填
}

// 必填字段不使用 ?
interface IProduct {
  id: string;            // 必填
  name: string;          // 必填
  price: number;         // 必填
}
```

### 3.2 页面代码规范

#### 3.2.1 页面文件结构

```typescript
// pages/index/index.ts

// 1. 导入依赖
import { productService } from '../../services/index';
import { IProduct, ICategory } from '../../types/index';

// 2. 定义页面数据接口
interface IIndexPageData {
  bannerList: string[];
  categories: ICategory[];
  hotProducts: IProduct[];
  loading: boolean;
}

// 3. Page 定义（带类型）
Page<IIndexPageData, WechatMiniprogram.Page.CustomOption>({
  // 4. data 初始值
  data: {
    bannerList: [],
    categories: [],
    hotProducts: [],
    loading: true,
  },

  // 5. 生命周期方法
  onLoad() {
    this.loadData();
  },

  onShow() {
    // 处理返回逻辑
  },

  // 6. 业务方法（按功能分组）
  async loadData() {
    this.setData({ loading: true });
    try {
      const categories = await productService.getCategories();
      this.setData({ categories, loading: false });
    } catch (error) {
      wx.showToast({ title: '加载失败', icon: 'none' });
      this.setData({ loading: false });
    }
  },

  // 7. 事件处理方法（以 on 开头）
  onProductTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    wx.navigateTo({ url: `/pages/details/details?id=${id}` });
  },

  onCategoryTap(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    // ...
  },

  // 8. 下拉刷新
  onPullDownRefresh() {
    this.loadData().then(() => {
      wx.stopPullDownRefresh();
    });
  },
});
```

#### 3.2.2 方法命名规范

| 方法类型 | 命名规范 | 示例 |
|----------|----------|------|
| 生命周期 | 小驼峰 | `onLoad`, `onShow`, `onReady` |
| 事件处理 | on前缀+动作 | `onProductTap`, `onSearch`, `onRemove` |
| 数据加载 | loadData/load前缀 | `loadData`, `loadProductList` |
| 数据提交 | submit前缀 | `submitOrder`, `submitAddress` |
| 私有方法 | _前缀 | `_handleError`, `_formatData` |

### 3.3 服务层代码规范

#### 3.3.1 服务文件结构

```typescript
// services/product.service.ts

// 1. 导入依赖
import { adapter } from '../adapters/adapter.config';
import { IProduct, IPageResult, IPageQuery } from '../types/index';

// 2. 类定义（单例模式）
export class ProductService {
  private static instance: ProductService;

  // 3. 单例获取方法
  static getInstance(): ProductService {
    if (!ProductService.instance) {
      ProductService.instance = new ProductService();
    }
    return ProductService.instance;
  }

  // 4. 业务方法（带注释）
  /** 获取商品列表 */
  async getProductList(params: IPageQuery): Promise<IPageResult<IProduct>> {
    return await adapter.getProductList(params);
  }

  /** 获取商品详情 */
  async getProductDetail(id: string): Promise<IProductDetail> {
    return await adapter.getProductDetail(id);
  }

  /** 获取热销商品 */
  async getHotProducts(limit: number = 10): Promise<IProduct[]> {
    const result = await adapter.getProductList({ pageIndex: 1, pageSize: 100 });
    return result.list.filter(p => p.isHot).slice(0, limit);
  }
}

// 5. 导出单例实例（便于直接使用）
export const productService = ProductService.getInstance();
```

#### 3.3.2 服务导出规范

```typescript
// services/index.ts
// 统一导出所有服务

export { authService, AuthService } from './auth.service';
export { productService, ProductService } from './product.service';
export { cartService, CartService } from './cart.service';
// ...
```

### 3.4 适配层代码规范

#### 3.4.1 适配器接口定义

```typescript
// adapters/adapter.interface.ts

import {
  IUser,
  IProduct,
  IOrder,
  IAddress,
  // ...
} from '../types/index';

/** 数据适配器接口 - Mock 和 API 共享 */
export interface IDataAdapter {
  // ========== 用户认证 ==========
  wxLogin(code: string): Promise<IUser>;
  getUserInfo(): Promise<IUserInfo>;

  // ========== 商品相关 ==========
  getProductList(params: IPageQuery): Promise<IPageResult<IProduct>>;
  getProductDetail(id: string): Promise<IProductDetail>;

  // ========== 订单相关 ==========
  createOrder(data: ICreateOrderParams): Promise<IOrder>;
  getOrderList(params: IPageQuery): Promise<IPageResult<IOrder>>;

  // ========== 地址相关 ==========
  getAddressList(): Promise<IAddress[]>;
  saveAddress(data: IAddress): Promise<IAddress>;
}
```

#### 3.4.2 适配器配置切换

```typescript
// adapters/adapter.config.ts

import { IDataAdapter } from './adapter.interface';
import { MockAdapter } from './mock.adapter';
import { ApiAdapter } from './api.adapter';

/** Mock/API 模式切换
 * true: Mock 数据（开发阶段）
 * false: 真实 API（上线前切换）
 */
const USE_MOCK = false;

/** 获取当前适配器 */
export function getAdapter(): IDataAdapter {
  if (USE_MOCK) {
    return new MockAdapter();
  }
  return new ApiAdapter();
}

export const adapter = getAdapter();
```

#### 3.4.3 后端字段转换规范

**原则：前端类型定义统一使用 camelCase，适配器负责转换后端 PascalCase**

```typescript
// adapters/api.adapter.ts

/** 后端订单商品项响应格式（PascalCase） */
interface IOrderItemResponse {
  id: string;
  productId: string;
  productName: string;
  quantity: number;   // 后端字段名
  amount: number;     // 后端字段名
}

/** 转换后端响应为前端格式 */
private transformOrderResponse(order: IOrderResponse): IOrder {
  return {
    id: order.id,
    orderNo: order.orderNo,
    items: order.items?.map(item => ({
      id: item.id,
      productId: item.productId,
      productName: item.productName,
      count: item.quantity,      // quantity → count
      subtotal: item.amount,     // amount → subtotal
    })) || [],
    // ...
  };
}
```

### 3.5 工具类代码规范

#### 3.5.1 工具类结构

```typescript
// utils/formatter.ts

/** 格式化工具类 */
export class Formatter {

  /** 格式化价格（保留两位小数） */
  static price(value: number): string {
    return (value / 100).toFixed(2);
  }

  /** 格式化价格显示（带¥符号） */
  static priceDisplay(value: number): string {
    return '¥' + this.price(value);
  }

  /** 格式化图片 URL */
  static imageUrl(path: string | undefined | null): string {
    if (!path) return '';
    if (path.startsWith('http://') || path.startsWith('https://')) {
      return path;
    }
    if (path.startsWith('/static/')) {
      return path;
    }
    // 拼接文件服务器地址
    const cleanPath = path.startsWith('/') ? path.slice(1) : path;
    return `${envConfig.fileBaseUrl}/image?path=${cleanPath}`;
  }

  /** 格式化时间戳为日期 */
  static date(timestamp: number, format: string = 'YYYY-MM-DD'): string {
    // ...
  }
}
```

#### 3.5.2 工具类方法规范

| 类型 | 命名规范 | 示例 |
|------|----------|------|
| 格式化 | 名词/动词 | `price`, `date`, `imageUrl` |
| 验证 | is/has前缀 | `isEmpty`, `isNumber`, `hasImage` |
| 存储 | 动词 | `set`, `get`, `remove`, `clear` |

---

## 四、注释规范

### 4.1 文件头注释

```typescript
// services/product.service.ts
// 商品服务 - 提供商品相关的业务逻辑
```

### 4.2 方法注释

```typescript
/**
 * 获取商品列表
 * @param params 查询参数
 * @param params.pageIndex 页码（必填）
 * @param params.pageSize 每页数量（必填）
 * @param params.categoryId 分类ID（可选）
 * @returns 商品列表分页结果
 */
async getProductList(params: IProductQueryParams): Promise<IPageResult<IProduct>> {
  // ...
}
```

### 4.3 接口字段注释

```typescript
interface IOrder {
  id: string;               // 订单ID
  orderNo: string;          // 订单编号
  status: OrderStatus;      // 订单状态
  totalAmount: number;      // 订单总金额（元）
  paymentTime?: number;     // 支付时间戳（可选）
  remark?: string;          // 订单备注（可选）
}
```

### 4.4 行内注释

```typescript
// 获取热销商品
const hotProducts = result.list.filter(p => p.isHot).slice(0, limit);

// TODO: 后续添加缓存机制
// FIXME: 临时方案，需要优化
```

---

## 五、错误处理规范

### 5.1 页面层错误处理

```typescript
async loadData() {
  this.setData({ loading: true });
  try {
    const result = await productService.getProductList({
      pageIndex: 1,
      pageSize: 10,
    });
    this.setData({
      productList: result.list,
      loading: false,
    });
  } catch (error) {
    console.error('加载商品列表失败:', error);
    wx.showToast({ title: '加载失败', icon: 'none' });
    this.setData({ loading: false });
  }
}
```

### 5.2 HTTP 封装错误处理

```typescript
// utils/http.ts
wx.request({
  success: (res) => {
    const response = res.data as HttpResponse<T>;

    if (response.code === 200) {
      resolve(response.data);
      return;
    }

    // 401 登录过期
    if (response.code === 401) {
      StorageUtil.remove(ApiConfig.tokenKey);
      wx.navigateTo({ url: '/pages/login/login' });
      reject(new Error('登录已过期'));
      return;
    }

    // 其他错误
    reject(new Error(response.message || '请求失败'));
  },
  fail: (err) => {
    reject(new Error('网络请求失败'));
  },
});
```

---

## 六、状态管理规范

### 6.1 Store 基类使用

```typescript
// stores/cart.store.ts

import { BaseStore } from './base.store';
import { ICartItem } from '../types/cart.types';

export class CartStore extends BaseStore<ICartItem[]> {
  private static instance: CartStore;

  static getInstance(): CartStore {
    if (!CartStore.instance) {
      CartStore.instance = new CartStore();
    }
    return CartStore.instance;
  }

  // 初始状态
  protected getInitialState(): ICartItem[] {
    return [];
  }

  // 计算属性（getter）
  get totalCount(): number {
    return this.getState().reduce((sum, item) => sum + item.count, 0);
  }

  get totalPrice(): number {
    return this.getState().reduce((sum, item) => sum + item.price * item.count, 0);
  }

  // 动作方法
  addToCart(product: IProduct): void {
    const items = this.getState();
    const existItem = items.find(item => item.productId === product.id);
    if (existItem) {
      existItem.count++;
      this.setState([...items]);
    } else {
      this.setState([...items, { ...product, count: 1 }]);
    }
  }
}
```

### 6.2 Store 使用规范

```typescript
// pages/shopping/shopping.ts

import { CartStore } from '../../stores/cart.store';

Page({
  data: {
    cartItems: [] as ICartItem[],
  },

  onLoad() {
    const store = CartStore.getInstance();

    // 获取当前状态
    this.setData({
      cartItems: store.getState(),
      totalPrice: store.totalPrice,
    });

    // 订阅状态变化
    store.subscribe((items) => {
      this.setData({
        cartItems: items,
        totalPrice: store.totalPrice,
      });
    });
  },

  onRemove(e: WechatMiniprogram.TouchEvent) {
    const { id } = e.currentTarget.dataset;
    CartStore.getInstance().removeFromCart(id);
  },
});
```

---

## 七、环境配置规范

### 7.1 环境配置文件

```typescript
// config/env.dev.ts
export const envConfig = {
  env: 'dev',
  baseUrl: 'http://localhost:7600/api/wechat',
  fileBaseUrl: 'http://localhost:7600/api/file',
  timeout: 30000,
  tokenKey: 'token',
  userInfoKey: 'userInfo',
};

// config/env.prod.ts
export const envConfig = {
  env: 'prod',
  baseUrl: 'https://api.example.com/api/wechat',
  fileBaseUrl: 'https://api.example.com/api/file',
  timeout: 30000,
  tokenKey: 'token',
  userInfoKey: 'userInfo',
};
```

### 7.2 API 路径配置

```typescript
// config/api.config.ts

import { envConfig } from './env.dev';

export const ApiConfig = envConfig;

export const ApiPaths = {
  // 认证
  login: '/auth/login',
  userinfo: '/auth/userinfo',

  // 商品
  productList: '/product/list',
  productDetail: '/product',
  productCategories: '/product/categories',

  // 购物车
  cartList: '/cart/list',
  cartAdd: '/cart/add',

  // 订单
  orderCreate: '/order/create',
  orderList: '/order/list',
};
```

---

## 八、WXML/WXSS 规范

### 8.1 WXML 规范

```xml
<!-- 1. 使用语义化标签 -->
<view class="product-list">
  <view class="product-item">
    <image class="product-image" src="{{item.image}}" />
    <text class="product-name">{{item.name}}</text>
  </view>
</view>

<!-- 2. 条件渲染 -->
<view wx:if="{{loading}}">加载中...</view>
<view wx:else>内容区域</view>

<!-- 3. 列表渲染（必须使用 wx:key） -->
<view wx:for="{{productList}}" wx:key="id">
  <text>{{item.name}}</text>
</view>

<!-- 4. 事件绑定 -->
<view bindtap="onProductTap" data-id="{{item.id}}">点击</view>
<input bindinput="onInput" value="{{inputValue}}" />
```

### 8.2 WXSS 规范

```wxss
/* 1. 使用 rpx 单位 */
.product-image {
  width: 200rpx;
  height: 200rpx;
}

/* 2. 类名使用中划线连接 */
.product-list { }
.product-item { }
.product-image { }

/* 3. 避免过深嵌套（最多3层） */
.product-list .product-item .product-name { }  /* ✅ */
.product-list .item .info .text .name { }      /* ❌ */

/* 4. 使用 Flex 布局 */
.product-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
```

---

## 九、数据流规范

### 9.1 数据流向

```
用户操作 → 页面事件 → 服务调用 → 适配器 → Mock/API → 返回数据 → 页面渲染
                    ↓
                 Store更新 → 订阅回调 → 页面更新
```

### 9.2 分页数据规范

```typescript
// 查询参数
interface IPageQuery {
  pageIndex: number;  // 页码（从1开始）
  pageSize: number;   // 每页数量
}

// 分页结果
interface IPageResult<T> {
  list: T[];          // 数据列表
  total: number;      // 总数量
  pageIndex: number;  // 当前页码
}
```

---

## 十、Mock 数据规范

### 10.1 Mock 文件结构

```typescript
// mock-data/products.mock.ts

import { IProduct, ICategory } from '../types/index';

/** 生成 GUID */
function generateGuid(): string {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0;
    const v = c === 'x' ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

/** Mock 商品列表 */
export const MockProducts = {
  list: [
    {
      id: generateGuid(),
      name: '红玫瑰鲜花束',
      price: 199.00,
      categoryId: 'category_001',
      isHot: true,
      isNew: false,
      createdAt: Date.now(),
    } as IProduct,
  ],

  categories: [
    { id: 'category_001', name: '玫瑰', sort: 1 },
  ],

  /** 获取商品详情 */
  getProductDetail(id: string): IProductDetail | null {
    const product = this.list.find(p => p.id === id);
    if (!product) return null;
    return { ...product, specs: [], coupons: [] };
  },
};
```

### 10.2 Mock 数据命名规范

- Mock 数据对象使用 `Mock` + 功能名前缀：`MockProducts`, `MockUser`
- Mock 数据字段与类型定义完全一致（camelCase）
- Mock 方法名与适配器接口方法名一致

---

## 十一、检查清单

### 11.1 新增功能检查

- [ ] 类型定义是否使用 camelCase
- [ ] 服务方法是否有单例导出
- [ ] 页面是否有 PageData 接口定义
- [ ] 事件方法是否以 on 前缀命名
- [ ] 是否在 services/index.ts 导出
- [ ] 是否在 types/index.ts 导出

### 11.2 API 对接检查

- [ ] 适配器是否有后端响应类型定义
- [ ] 字段转换是否正确（PascalCase → camelCase）
- [ ] 图片 URL 是否调用 Formatter.imageUrl
- [ ] 错误处理是否完整

### 11.3 代码提交检查

- [ ] 无 TypeScript 编译错误
- [ ] 无 console.log 调试代码（保留 console.error）
- [ ] 注释是否完整
- [ ] 命名是否符合规范

---

**文档版本**: 1.0
**最后更新**: 2026-04-16
**基于项目**: WeChatWeb 微信小程序鲜花商城