/**
 * ETL 任务流和 DAG 类型定义
 */
import type { ScheduleConfig } from './schedule'
import type { TaskNodeType, TaskNodeConfig } from './taskNode'

/**
 * 任务流状态枚举
 */
export enum PipelineStatus {
  DRAFT = 0,       // 草稿
  PUBLISHED = 1,   // 已发布
  ARCHIVED = 2,    // 已归档
}

/**
 * DAG 节点
 */
export interface DagNode {
  id: string
  name: string
  type: TaskNodeType
  position: { x: number; y: number }
  config: TaskNodeConfig
  retryTimes?: number
  retryInterval?: number
  timeout?: number
  skipOnFailure?: boolean
}

/**
 * 边条件（使用 taskNode 中定义的 ConditionRule）
 */
import type { ConditionRule } from './taskNode'

export interface EdgeCondition {
  expression?: string
  rules?: ConditionRule[]
}

/**
 * DAG 边（连线）
 */
export interface DagEdge {
  id: string
  sourceNodeId: string
  targetNodeId: string
  sourcePort?: string
  targetPort?: string
  condition?: EdgeCondition
}

/**
 * DAG 全局配置
 */
export interface DagGlobalConfig {
  maxConcurrency?: number
  timeout?: number
  retryTimes?: number
  retryInterval?: number
  failureStrategy?: 'stop' | 'continue'
}

/**
 * DAG 配置（存储到数据库）
 */
export interface DagConfig {
  version: string
  nodes: DagNode[]
  edges: DagEdge[]
  globalConfig?: DagGlobalConfig
}

/**
 * 任务流接口
 */
export interface Pipeline {
  id: string                              // GUID 主键
  name: string                            // 任务流名称
  description?: string                    // 描述
  categoryCode?: string                   // 分类编码
  categoryName?: string                   // 分类名称
  status: PipelineStatus                  // 状态
  currentVersion: string                  // 当前版本号
  dagConfig: DagConfig                    // DAG 配置 JSON
  scheduleConfig?: ScheduleConfig         // 调度配置
  creatorId: string                       // 创建人 ID
  creatorName?: string                    // 创建人名称
  createTime: string                      // 创建时间
  updateTime: string                      // 更新时间
}

/**
 * 任务流版本历史
 */
export interface PipelineVersion {
  id: string
  pipelineId: string
  version: string
  dagConfig: DagConfig
  publishTime: string
  publisherId: string
  publisherName?: string
  remark?: string
}

/**
 * 任务流查询参数
 */
export interface PipelineQueryParams {
  pageIndex: number
  pageSize: number
  name?: string
  categoryCode?: string
  status?: PipelineStatus
}

/**
 * 创建任务流参数
 */
export interface CreatePipelineParams {
  name: string
  description?: string
  categoryCode?: string
}

/**
 * 更新任务流参数
 */
export interface UpdatePipelineParams {
  id: string
  name?: string
  description?: string
  dagConfig?: DagConfig
}