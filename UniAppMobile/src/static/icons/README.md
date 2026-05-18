# 应用图标说明

此目录用于存放应用图标资源文件。

## 所需图标尺寸

### Android 平台

| 文件名 | 尺寸 | 用途 |
|--------|------|------|
| icon-72x72.png | 72x72 | hdpi (高密度) |
| icon-96x96.png | 96x96 | xhdpi (超高密度) |
| icon-144x144.png | 144x144 | xxhdpi (超超高密度) |
| icon-192x192.png | 192x192 | xxxhdpi (超超超高密度) |

### iOS 平台

| 文件名 | 尺寸 | 用途 |
|--------|------|------|
| icon-1024x1024.png | 1024x1024 | App Store |
| icon-180x180.png | 180x180 | iPhone App@3x |
| icon-120x120.png | 120x120 | iPhone App@2x / Spotlight@3x |
| icon-167x167.png | 167x167 | iPad Pro App@2x |
| icon-152x152.png | 152x152 | iPad App@2x |
| icon-76x76.png | 76x76 | iPad App |
| icon-87x87.png | 87x87 | iPhone Settings@3x |
| icon-80x80.png | 80x80 | Spotlight@2x |
| icon-60x60.png | 60x60 | iPhone Notification@3x |
| icon-58x58.png | 58x58 | Settings@2x |
| icon-40x40.png | 40x40 | Notification@2x / Spotlight |
| icon-29x29.png | 29x29 | Settings |
| icon-20x20.png | 20x20 | Notification |

## 图标设计规范

### 视觉要求

1. **安全区域**: 图标主体内容应保持在中心 80% 区域内
2. **圆角**: Android 需要 1/8 圆角，iOS 会自动添加圆角
3. **背景**: 建议使用纯色背景，避免透明背景
4. **分辨率**: 所有图标必须为 PNG 格式，支持透明通道

### 设计建议

- 使用简洁、辨识度高的图形
- 主色调应与应用主题色一致（#2563EB）
- 避免过多细节，小尺寸图标要清晰可辨
- 适应不同背景色显示效果

## 生成工具推荐

1. **在线工具**
   - https://icon.wuruihong.com/ - 一键生成所有尺寸
   - https://www.makeappicon.com/ - 支持多平台

2. **本地工具**
   - ImageMagick 命令行批量处理
   - Sketch / Figma 插件

## 当前状态

> **注意**: 当前目录下的图标文件为 tabBar 图标，非应用图标。
> 应用图标需要按照上述规格准备并放置到此目录。

### 待准备清单

- [ ] icon-72x72.png
- [ ] icon-96x96.png
- [ ] icon-144x144.png
- [ ] icon-192x192.png
- [ ] icon-512x512.png
- [ ] icon-1024x1024.png
- [ ] 其他 iOS 尺寸图标