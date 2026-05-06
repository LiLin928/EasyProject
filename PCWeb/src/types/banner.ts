/**
 * 轮播图跳转类型
 */
export type BannerLinkType = 'none' | 'product' | 'category' | 'page'

/**
 * 轮播图类型
 */
export interface Banner {
  id: string                           // GUID 主键
  image: string                        // 图片 URL
  linkType: BannerLinkType             // 跳转类型
  linkValue: string                    // 跳转目标（商品ID/分类ID/页面路径）
  sort: number                         // 排序（升序）
  status: 0 | 1                        // 状态：0禁用 1启用
  createTime: string                   // 创建时间
  updateTime: string                   // 更新时间
}

/**
 * 创建轮播图参数
 */
export interface CreateBannerParams {
  image: string
  linkType: BannerLinkType
  linkValue: string
  sort?: number
  status?: 0 | 1
}

/**
 * 更新轮播图参数
 */
export interface UpdateBannerParams extends Partial<CreateBannerParams> {
  id: string
}

/**
 * 轮播图查询参数
 */
export interface BannerQueryParams {
  pageIndex?: number
  pageSize?: number
  status?: 0 | 1
}