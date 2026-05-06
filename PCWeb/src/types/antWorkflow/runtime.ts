/**
 * Ant Workflow 运行时类型定义
 */

// ==================== 枚举类型 ====================

/**
 * 流程实例状态枚举
 */
export enum AntWorkflowInstanceStatus {
  /** 待提交 */
  WAIT_SUBMIT = 0,
  /** 审批中 */
  APPROVING = 1,
  /** 已通过 */
  PASSED = 2,
  /** 已驳回 */
  REJECTED = 3,
  /** 已撤回 */
  WITHDRAWN = 4,
  /** 已终止 */
  TERMINATED = 5,
}

/**
 * 审批结果枚举
 */
export enum AntApproveStatus {
  /** 通过 */
  PASS = 1,
  /** 驳回 */
  REJECT = 2,
  /** 转交 */
  TRANSFER = 3,
  /** 撤回 */
  WITHDRAW = 4,
}

/**
 * 节点审批状态枚举
 */
export enum AntNodeApproveStatus {
  /** 待处理 */
  PENDING = 0,
  /** 处理中 */
  PROCESSING = 1,
  /** 已完成 */
  COMPLETED = 2,
  /** 已跳过 */
  SKIPPED = 3,
}

// ==================== DTO 定义 ====================

/**
 * 流程任务 DTO（与后端 AntWorkflowTaskDto 匹配）
 */
export interface AntWorkflowTaskDto {
  /** 任务ID */
  id: string
  /** 实例ID */
  instanceId: string
  /** 流程标题 */
  instanceTitle?: string
  /** 节点ID */
  nodeId: string
  /** 节点名称 */
  nodeName?: string
  /** 节点类型（int） */
  nodeType: number
  /** 进入时间 */
  entryTime?: string
  /** 截止时间 */
  dueTime?: string
  /** 发起人ID */
  initiatorId?: string
  /** 发起人姓名 */
  initiatorName?: string
  /** 任务类型 */
  taskType: number
}

/**
 * 流程抄送 DTO（与后端 AntWorkflowCcDto 匹配）
 */
export interface AntWorkflowCcDto {
  /** 抄送记录ID */
  id: string
  /** 实例ID */
  instanceId: string
  /** 流程标题 */
  instanceTitle?: string
  /** 节点名称 */
  nodeName?: string
  /** 发送人姓名 */
  fromUserName?: string
  /** 发送时间 */
  sendTime?: string
  /** 是否已读 */
  isRead: number
}

/**
 * 流程实例 DTO（与后端 AntWorkflowInstanceDto 匹配）
 */
export interface AntWorkflowInstanceDto {
  /** 实例ID */
  id: string
  /** 流程标题 */
  title?: string
  /** 业务单据ID */
  businessId?: string
  /** 业务类型编码 */
  businessType?: string
  /** 状态 */
  status: AntWorkflowInstanceStatus
  /** 发起人ID */
  initiatorId?: string
  /** 发起人姓名 */
  initiatorName?: string
  /** 开始时间 */
  startTime?: string
  /** 完成时间 */
  finishTime?: string
  /** 当前节点名称 */
  currentNodeName?: string
  /** 流程名称 */
  workflowName?: string
}

/**
 * 处理人 DTO
 */
export interface AntHandlerDto {
  /** 用户ID */
  userId: string
  /** 用户姓名 */
  userName?: string
}

/**
 * 节点状态 DTO（与后端 AntNodeStatusDto 匹配）
 */
export interface AntWorkflowNodeStatusDto {
  /** 节点ID */
  nodeId: string
  /** 节点名称 */
  nodeName?: string
  /** 节点类型（int） */
  nodeType: number
  /** 审批状态 */
  approveStatus: AntNodeApproveStatus
  /** 当前处理人列表 */
  handlers?: AntHandlerDto[]
}

/**
 * 执行日志 DTO（与后端 AntExecutionLogDto 匹配）
 */
export interface AntExecutionLogDto {
  /** 记录ID */
  id: string
  /** 节点名称 */
  nodeName?: string
  /** 节点类型（int） */
  nodeType?: number
  /** 审批人姓名 */
  handlerName?: string
  /** 审批状态 */
  approveStatus?: AntApproveStatus
  /** 审批意见 */
  approveDesc?: string
  /** 审批时间 */
  approveTime?: string
  /** 耗时（分钟） */
  duration?: number
  /** 转交给用户姓名 */
  transferToName?: string
}

/**
 * 流程实例详情 DTO（与后端 AntWorkflowInstanceDetailDto 匹配）
 */
export interface AntWorkflowInstanceDetailDto {
  /** 实例基本信息 */
  instance: AntWorkflowInstanceDto
  /** 业务数据JSON */
  businessData?: string
  /** 表单数据JSON */
  formData?: string
  /** DAG配置JSON */
  flowConfig?: string
  /** 节点状态列表 */
  nodeStatusList: AntWorkflowNodeStatusDto[]
  /** 审批记录列表 */
  approveRecords: AntExecutionLogDto[]
}

/**
 * 审批记录 DTO（兼容旧名称）
 */
export interface AntWorkflowApproveRecordDto {
  /** 记录ID */
  id: string
  /** 节点名称 */
  nodeName?: string
  /** 审批人姓名 */
  handlerName?: string
  /** 审批状态 */
  approveStatus?: AntApproveStatus
  /** 审批意见 */
  approveDesc?: string
  /** 审批时间 */
  approveTime?: string
  /** 转交给用户姓名 */
  transferToName?: string
}

// ==================== 操作参数接口 ====================

/**
 * 审批通过参数
 */
export interface AntApproveParams {
  /** 任务ID */
  taskId: string
  /** 审批意见 */
  comment?: string
  /** 表单数据JSON */
  formData?: string
  /** 抄送用户ID列表 */
  ccUserIds?: string[]
}

/**
 * 审批驳回参数
 */
export interface AntRejectParams {
  /** 任务ID */
  taskId: string
  /** 驳回原因 */
  comment?: string
}

/**
 * 转交任务参数
 */
export interface AntTransferParams {
  /** 任务ID */
  taskId: string
  /** 转交给用户ID */
  toUserId: string
  /** 转交说明 */
  comment?: string
}

// ==================== 查询参数接口 ====================

/**
 * 待办任务查询参数
 */
export interface AntTodoTaskQueryParams {
  /** 页码 */
  pageIndex?: number
  /** 每页数量 */
  pageSize?: number
  /** 流程名称（模糊查询） */
  title?: string
  /** 业务类型 */
  businessType?: string
}

/**
 * 已办任务查询参数
 */
export interface AntDoneTaskQueryParams {
  /** 页码 */
  pageIndex?: number
  /** 每页数量 */
  pageSize?: number
  /** 流程名称（模糊查询） */
  title?: string
  /** 开始时间 */
  startTime?: string
  /** 结束时间 */
  endTime?: string
}

/**
 * 我发起的流程查询参数
 */
export interface AntMyInstanceQueryParams {
  /** 页码 */
  pageIndex?: number
  /** 每页数量 */
  pageSize?: number
  /** 流程名称（模糊查询） */
  workflowName?: string
  /** 业务类型 */
  businessType?: string
  /** 流程状态 */
  status?: AntWorkflowInstanceStatus
}

// ==================== 任务详情接口 ====================

/**
 * 任务详情（扩展任务DTO，包含业务数据和实例状态）
 */
export interface AntTaskDetail extends AntWorkflowTaskDto {
  /** 业务类型编码 */
  businessType?: string
  /** 业务单据ID */
  businessId?: string
  /** 业务数据（JSON格式） */
  businessData?: string
  /** 实例状态 */
  instanceStatus: AntWorkflowInstanceStatus
}

// ==================== 操作参数扩展接口 ====================

/**
 * 会签加签参数
 */
export interface AntAddSignerParams {
  /** 任务ID */
  taskId: string
  /** 加签用户ID列表 */
  userIds: string[]
  /** 加签说明 */
  comment?: string
}

// ==================== 执行日志接口 ====================

/**
 * 执行日志
 */
export interface AntExecutionLog {
  /** 日志ID */
  id: string
  /** 节点ID */
  nodeId: string
  /** 节点名称 */
  nodeName: string
  /** 节点类型 */
  nodeType: string
  /** 执行状态 */
  status: AntNodeApproveStatus
  /** 执行时间 */
  executeTime?: string
  /** 执行人ID */
  executorId?: string
  /** 执行人姓名 */
  executorName?: string
  /** 备注/意见 */
  comment?: string
}

/**
 * 启动流程参数
 */
export interface StartAntWorkflowParams {
  /** 流程定义ID */
  workflowId: string
  /** 业务单据ID */
  businessId?: string
  /** 业务类型编码 */
  businessType?: string
  /** 流程标题 */
  title?: string
  /** 业务数据JSON */
  businessData?: string
  /** 表单数据JSON */
  formData?: string
}