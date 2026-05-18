# 多端编译指南

本文档介绍 UniAppMobile 项目的多端编译和发布流程。

## 编译环境要求

### 基础环境

- Node.js >= 18.0.0
- pnpm >= 8.0.0（推荐）或 npm >= 9.0.0

### 平台特定要求

| 平台 | 要求 |
|------|------|
| 微信小程序 | 微信开发者工具 |
| H5 | 任意 Web 服务器 |
| Android App | HBuilderX 或 Android Studio |
| iOS App | HBuilderX + Apple Developer 账号 |

## 编译命令

### 开发环境

```bash
# 安装依赖
pnpm install

# H5 开发
pnpm dev:h5

# 微信小程序开发
pnpm dev:mp-weixin

# Android App 开发
pnpm dev:app-android

# iOS App 开发
pnpm dev:app-ios
```

### 生产环境

```bash
# H5 生产编译
pnpm build:h5

# 微信小程序生产编译
pnpm build:mp-weixin

# Android App 生产编译
pnpm build:app-android

# iOS App 生产编译
pnpm build:app-ios
```

## 微信小程序发布

### 1. 编译项目

```bash
pnpm build:mp-weixin
```

编译产物位于 `dist/build/mp-weixin/`

### 2. 上传代码

方式一：使用微信开发者工具

1. 打开微信开发者工具
2. 导入项目，选择 `dist/build/mp-weixin` 目录
3. 点击「上传」按钮
4. 填写版本号和更新说明

方式二：使用 CLI（需配置）

```bash
# 安装 miniprogram-ci
pnpm add -D miniprogram-ci

# 上传脚本
node scripts/upload-weixin.js
```

### 3. 提交审核

1. 登录微信公众平台
2. 进入「版本管理」
3. 将开发版本设为体验版（可选）
4. 提交审核
5. 等待审核通过

### 4. 发布上线

审核通过后，点击「发布」即可上线。

### 微信小程序配置

确保 `manifest.json` 中的 `mp-weixin.appid` 已正确配置：

```json
{
  "mp-weixin": {
    "appid": "wx1234567890abcdef"
  }
}
```

## H5 发布

### 1. 编译项目

```bash
pnpm build:h5
```

编译产物位于 `dist/build/h5/`

### 2. 服务器部署

将编译产物上传到服务器：

```bash
# 使用 scp
scp -r dist/build/h5/* user@server:/var/www/html/

# 或使用 rsync
rsync -avz dist/build/h5/ user@server:/var/www/html/
```

### 3. Nginx 配置示例

```nginx
server {
    listen 80;
    server_name your-domain.com;
    root /var/www/html;
    index index.html;

    # 处理 SPA 路由
    location / {
        try_files $uri $uri/ /index.html;
    }

    # API 代理
    location /api {
        proxy_pass http://localhost:7600;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }

    # 静态资源缓存
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2)$ {
        expires 30d;
        add_header Cache-Control "public, immutable";
    }

    # Gzip 压缩
    gzip on;
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml;
    gzip_min_length 1000;
}
```

### 4. HTTPS 配置（推荐）

```nginx
server {
    listen 443 ssl http2;
    server_name your-domain.com;

    ssl_certificate /etc/letsencrypt/live/your-domain.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/your-domain.com/privkey.pem;

    # ... 其他配置
}

# HTTP 重定向到 HTTPS
server {
    listen 80;
    server_name your-domain.com;
    return 301 https://$server_name$request_uri;
}
```

## Android App 发布

### 方式一：HBuilderX 云打包

1. 使用 HBuilderX 打开项目
2. 点击「发行」->「原生App-云打包」
3. 选择 Android 平台
4. 填写证书信息（可使用测试证书）
5. 点击打包，等待完成
6. 下载 APK 文件

### 方式二：本地打包

1. 使用 HBuilderX 生成本地打包资源
2. 使用 Android Studio 打开项目
3. 配置签名信息
4. 构建签名 APK

### APK 签名配置

创建 `android-sign.properties`:

```properties
storeFile=your-keystore.jks
storePassword=your-store-password
keyAlias=your-key-alias
keyPassword=your-key-password
```

### 发布到应用市场

#### 国内应用市场

| 平台 | 注册地址 |
|------|----------|
| 华为应用市场 | https://developer.huawei.com/ |
| 小米应用商店 | https://dev.mi.com/ |
| OPPO 应用商店 | https://open.oppomobile.com/ |
| vivo 应用商店 | https://dev.vivo.com.cn/ |
| 腾讯应用宝 | https://open.tencent.com/ |

#### Google Play

1. 注册 Google Play Developer 账号（$25 一次性费用）
2. 创建应用
3. 上传 AAB 格式安装包
4. 填写商店信息
5. 提交审核

## iOS App 发布

### 前置条件

1. Apple Developer 账号（$99/年）
2. 已创建 App ID
3. 已创建开发/发布证书
4. 已创建 Provisioning Profile

### 1. 配置证书

在 Apple Developer 网站创建：
- 开发证书（Development Certificate）
- 发布证书（Distribution Certificate）
- 描述文件（Provisioning Profile）

### 2. 云打包

1. 使用 HBuilderX 云打包
2. 选择 iOS 平台
3. 上传证书和描述文件
4. 填写 Bundle ID
5. 点击打包

### 3. App Store Connect

1. 登录 App Store Connect
2. 创建新应用
3. 填写应用信息：
   - 名称、描述、关键词
   - 应用截图（多尺寸）
   - 应用图标（1024x1024）
   - 隐私政策 URL
4. 上传 IPA 文件
   - 使用 Transporter 应用上传
   - 或使用 Xcode 上传

### 4. 提交审核

1. 选择构建版本
2. 填写版本信息
3. 提交审核
4. 等待审核结果（通常 1-3 天）

### 5. 发布上线

审核通过后，可选择：
- 手动发布
- 自动发布

## 环境配置

### 环境变量

创建 `.env.production`:

```env
VITE_API_BASE_URL=https://api.example.com
VITE_APP_ENV=production
```

创建 `.env.development`:

```env
VITE_API_BASE_URL=http://localhost:7600
VITE_APP_ENV=development
```

### 条件编译

在代码中根据平台编译：

```typescript
// #ifdef H5
console.log('H5 平台')
// #endif

// #ifdef MP-WEIXIN
console.log('微信小程序平台')
// #endif

// #ifdef APP-PLUS
console.log('App 平台')
// #endif
```

## 编译产物说明

| 平台 | 产物目录 | 说明 |
|------|----------|------|
| H5 | dist/build/h5/ | 静态文件，可直接部署 |
| MP-WEIXIN | dist/build/mp-weixin/ | 小程序代码，需上传 |
| APP-ANDROID | dist/build/app-android/ | Android 项目文件 |
| APP-IOS | dist/build/app-ios/ | iOS 项目文件 |

## 常见问题

### Q: 微信小程序上传失败？

A: 检查以下几点：
1. appid 是否正确配置
2. 是否已开通上传权限
3. 代码是否符合小程序规范

### Q: H5 部署后路由 404？

A: 确保服务器配置了 SPA 路由回退：
```nginx
location / {
    try_files $uri $uri/ /index.html;
}
```

### Q: Android 打包失败？

A: 检查以下几点：
1. 是否有网络连接（云打包）
2. 证书配置是否正确
3. manifest.json 配置是否有误

### Q: iOS 打包需要什么？

A: 必须准备：
1. Apple Developer 账号
2. 发布证书（.p12 文件）
3. 描述文件（.mobileprovision）
4. Bundle ID