/**
 * 通用枚举类型定义
 */

/**
 * 通用状态枚举
 */
export enum CommonStatus {
  DISABLED = 0,  // 禁用
  ENABLED = 1,   // 启用
}

/**
 * 数据源状态枚举
 */
export enum DatasourceStatus {
  DISABLED = 0,  // 禁用
  ENABLED = 1,   // 正常
}

/**
 * 大屏发布状态枚举
 */
export enum ScreenPublishStatus {
  UNPUBLISHED = 0,  // 已下架
  PUBLISHED = 1,    // 正常
}

/**
 * 菜单可见性枚举
 */
export enum MenuVisibility {
  VISIBLE = 0,  // 显示
  HIDDEN = 1,   // 隐藏
}