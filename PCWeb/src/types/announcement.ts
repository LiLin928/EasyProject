// 文件：src/types/announcement.ts

/**
 * 公告信息
 */
export interface Announcement {
  id: string
  title: string
  content: string
  type: number        // 1全员 2定向
  level: number       // 1普通 2重要 3紧急
  targetRoleIds?: string[]
  targetRoleNames?: string[]
  isTop: number       // 后端返回 int 类型
  topTime?: string
  publishTime?: string
  recallTime?: string
  status: number      // 0草稿 1已发布 2已撤回
  statusName?: string
  typeName?: string
  levelName?: string
  creatorName?: string
  creatorId: string
  createTime: string
  updateTime?: string
  attachments?: FileAttachment[]  // 使用 FileAttachment 类型
  isRead?: number     // 后端返回 0/1
  readCount?: number
  totalCount?: number
}

/**
 * 文件附件信息（与后端 FileRecordDto 匹配）
 */
export interface FileAttachment {
  id: string
  fileName: string
  fileSize: number
  fileExt: string
  contentType: string
  url?: string
  userId?: string
  businessId?: string
  status?: number
  createTime?: string
  fileSizeFormat?: string
}

/**
 * 公告列表查询参数
 */
export interface QueryAnnouncementParams {
  pageIndex?: number
  pageSize?: number
  title?: string
  status?: number
  type?: number
}

/**
 * 新增公告参数
 */
export interface AddAnnouncementParams {
  title: string
  content: string
  type?: number
  level?: number
  targetRoleIds?: string[]
}

/**
 * 更新公告参数
 */
export interface UpdateAnnouncementParams {
  id: string
  title: string
  content: string
  type?: number
  level?: number
  targetRoleIds?: string[]
}

/**
 * 阅读统计
 */
export interface ReadStats {
  totalCount: number
  readCount: number
  unreadCount: number
}

/**
 * 阅读详情
 */
export interface ReadDetail {
  userId: string
  userName?: string
  realName?: string
  isRead: boolean
  readTime?: string
}

/**
 * 阅读详情查询参数（前端）
 */
export interface ReadDetailParams {
  pageIndex?: number
  pageSize?: number
  isRead?: boolean  // 前端使用 boolean 篮选
}

/**
 * 阅读详情 API 参数（后端）
 */
export interface ReadDetailApiParams {
  pageIndex?: number
  pageSize?: number
  isRead?: number  // 后端使用 0/1
}