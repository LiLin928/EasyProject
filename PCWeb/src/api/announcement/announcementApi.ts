// 文件：src/api/announcement/announcementApi.ts
import { get, post, del } from '@/utils/request'
import type {
  Announcement,
  QueryAnnouncementParams,
  AddAnnouncementParams,
  UpdateAnnouncementParams,
  ReadStats,
  ReadDetail,
  ReadDetailApiParams
} from '@/types'

/**
 * 获取公告列表
 */
export function getAnnouncementList(params: QueryAnnouncementParams) {
  return post<{ list: Announcement[]; total: number; pageIndex: number; pageSize: number }>(
    '/api/announcement/list',
    params
  )
}

/**
 * 获取公告详情
 */
export function getAnnouncementDetail(id: string) {
  return get<Announcement>(`/api/announcement/detail/${id}`)
}

/**
 * 新增公告
 */
export function addAnnouncement(data: AddAnnouncementParams) {
  return post<string>('/api/announcement/add', data)
}

/**
 * 更新公告
 */
export function updateAnnouncement(data: UpdateAnnouncementParams) {
  return post<number>('/api/announcement/update', data)
}

/**
 * 发布公告
 */
export function publishAnnouncement(id: string) {
  return post<number>(`/api/announcement/publish/${id}`)
}

/**
 * 撤回公告
 */
export function recallAnnouncement(id: string) {
  return post<number>(`/api/announcement/recall/${id}`)
}

/**
 * 删除公告
 */
export function deleteAnnouncement(id: string) {
  return del<number>(`/api/announcement/delete/${id}`)
}

/**
 * 置顶/取消置顶
 */
export function toggleTopAnnouncement(id: string) {
  return post<number>(`/api/announcement/top/${id}`)
}

/**
 * 标记已读
 */
export function markReadAnnouncement(id: string) {
  return post<number>(`/api/announcement/read/${id}`)
}

/**
 * 获取未读公告列表
 */
export function getUnreadAnnouncementList() {
  return get<Announcement[]>('/api/announcement/unread-list')
}

/**
 * 获取阅读统计
 */
export function getReadStats(id: string) {
  return get<ReadStats>(`/api/announcement/read-stats/${id}`)
}

/**
 * 获取阅读详情列表
 * @param id 公告ID
 * @param params 查询参数（isRead: 0未读 1已读 null全部）
 */
export function getReadDetailList(id: string, params: ReadDetailApiParams) {
  return get<{ list: ReadDetail[]; total: number; pageIndex: number; pageSize: number }>(
    `/api/announcement/read-detail/${id}`,
    params
  )
}

/**
 * 更新文件的业务ID（关联附件到公告）
 * @param fileIds 文件ID列表
 * @param businessId 业务ID（公告ID）
 */
export function updateFileBusinessId(fileIds: string[], businessId: string) {
  return post<number>('/api/file/update-business-id', {
    fileIds,
    businessId
  })
}

/**
 * 删除文件
 * @param id 文件ID
 */
export function deleteFile(id: string) {
  return post<number>('/api/file/delete', id)
}