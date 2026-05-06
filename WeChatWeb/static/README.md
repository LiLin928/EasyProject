# 静态资源说明

本目录存放小程序的静态资源文件。

## 目录结构

```
static/
├── tabs/                    # TabBar 图标（必需）
│   ├── home.png            # 首页图标（未选中）
│   ├── home-active.png     # 首页图标（选中）
│   ├── category.png        # 分类图标（未选中）
│   ├── category-active.png # 分类图标（选中）
│   ├── cart.png            # 购物车图标（未选中）
│   ├── cart-active.png     # 购物车图标（选中）
│   ├── user.png            # 我的图标（未选中）
│   └── user-active.png     # 我的图标（选中）
│
├── images/                  # 图片资源
│   ├── banners/            # 轮播图
│   │   ├── banner1.png
│   │   ├── banner2.png
│   │   └── banner3.png
│   │
│   ├── products/           # 商品图片
│   │   ├── product1.png
│   │   ├── product2.png
│   │   └── ...
│   │
│   ├── icons/              # 功能图标
│   │   └── default-avatar.png  # 默认头像
│   │
│   └── empty.png           # 空状态占位图
│
└── README.md               # 本说明文件
```

## 图标规格

### TabBar 图标
- 尺寸：81px × 81px（推荐）
- 格式：PNG（支持透明）
- 颜色：
  - 未选中：#999999
  - 选中：#1989fa

### 轮播图
- 尺寸：750px × 340px（推荐）
- 格式：PNG/JPG

### 商品图片
- 尺寸：600px × 600px（推荐）
- 格式：PNG/JPG

### 默认头像
- 尺寸：200px × 200px
- 格式：PNG（圆形）

## 图标下载建议

1. **阿里巴巴矢量图标库**：https://www.iconfont.cn/
   - 搜索：首页、分类、购物车、用户
   - 下载 PNG 格式，选择 81px 尺寸

2. **Flaticon**：https://www.flaticon.com/

3. **Icons8**：https://icons8.com/

## 临时占位方案

在正式图片资源添加前，项目使用 Mock 数据中的网络图片作为占位。

---

**注意**：添加图片后，请确保文件名与代码中的引用一致。