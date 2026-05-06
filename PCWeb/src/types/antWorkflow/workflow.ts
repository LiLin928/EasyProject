/**
 * Ant Workflow 流程定义类型
 */

// ==================== 流程定义接口 ====================

/**
 * 流程定义
 */
export interface AntWorkflowDefinition {
  /** 流程ID（GUID） */
  id: string
  /** 流程名称 */
  name: string
  /** 流程编码 */
  code: string
  /** 分类编码 */
  categoryCode?: string
  /** 分类名称 */
  categoryName?: string
  /** 流程描述 */
  description?: string
  /** 流程图标 */
  icon?: string
  /** 流程状态 */
  status: WorkflowStatus
  /** 当前版本 */
  currentVersion?: string
  /** 流程配置 */
  flowConfig?: string
  /** 创建人ID */
  creatorId?: string
  /** 创建人名称 */
  creatorName?: string
  /** 创建时间 */
  createTime?: string
  /** 更新时间 */
  updateTime?: string
}

/**
 * 流程状态枚举
 */
export enum WorkflowStatus {
  /** 草稿 */
  DRAFT = 0,
  /** 待审核 */
  PENDING = 1,
  /** 已发布 */
  PUBLISHED = 2,
  /** 已拒绝 */
  REJECTED = 3,
  /** 已停用 */
  DISABLED = 4,
}

/**
 * 流程版本
 */
export interface AntWorkflowVersion {
  /** 版本ID */
  id: string
  /** 流程ID */
  workflowId: string
  /** 版本号 */
  version: string
  /** 流程配置 */
  flowConfig?: string
  /** 发布时间 */
  publishTime?: string
  /** 发布人ID */
  publisherId?: string
  /** 发布人名称 */
  publisherName?: string
  /** 版本状态 */
  status: WorkflowStatus
  /** 版本备注 */
  remark?: string
}

/**
 * 流程查询参数
 */
export interface AntWorkflowQueryParams {
  /** 页码 */
  pageIndex: number
  /** 每页数量 */
  pageSize: number
  /** 流程名称 */
  name?: string
  /** 分类编码 */
  categoryCode?: string
  /** 流程状态 */
  status?: WorkflowStatus
  /** 创建人ID */
  creatorId?: string
}

/**
 * 创建流程参数
 */
export interface CreateAntWorkflowParams {
  /** 流程名称 */
  name: string
  /** 流程编码 */
  code: string
  /** 分类编码 */
  categoryCode?: string
  /** 流程描述 */
  description?: string
}

/**
 * 更新流程参数
 */
export interface UpdateAntWorkflowParams {
  /** 流程ID */
  id: string
  /** 流程名称 */
  name?: string
  /** 分类编码 */
  categoryCode?: string
  /** 流程描述 */
  description?: string
  /** 流程配置 */
  flowConfig?: string
}

/**
 * 发起流程参数
 */
export interface StartAntWorkflowParams {
  /** 流程ID */
  workflowId: string
  /** 流程标题 */
  title: string
  /** 业务类型编码 */
  businessType?: string
  /** 业务单据ID */
  businessId?: string
  /** 业务数据（JSON格式） */
  businessData?: string
  /** 表单数据（JSON格式） */
  formData?: string
  /** 发起人ID（可选，默认当前用户） */
  initiatorId?: string
}