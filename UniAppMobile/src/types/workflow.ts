// types/workflow.ts

/** 任务状态 */
export enum TaskStatus {
  Pending = 0,      // 待处理
  Approved = 1,     // 已通过
  Rejected = 2,     // 已拒绝
  Transferred = 3,  // 已转办
  Withdrawn = 4,    // 已撤回
}

/** 任务类型 */
export enum TaskType {
  Todo = 'todo',    // 待办
  Done = 'done',    // 已办
  CC = 'cc',        // 抄送
}

/** 任务信息 */
export interface TaskInfo {
  id: string
  title: string
  processName: string
  applicant: string
  applicantName: string
  createTime: string
  status: TaskStatus
  urgent: boolean       // 是否紧急
  formKey: string       // 表单标识
  processInstanceId: string
}

/** 任务详情 */
export interface TaskDetail extends TaskInfo {
  formData: Record<string, any>   // 表单数据
  approvalProgress: ApprovalNode[]  // 审批进度
  currentNodeId: string          // 当前节点ID
  canApprove: boolean            // 是否可审批
  canTransfer: boolean           // 是否可转办
}

/** 审批节点 */
export interface ApprovalNode {
  id: string
  name: string
  type: string           // 节点类型
  status: TaskStatus
  operator?: string      // 操作人
  operatorName?: string
  operateTime?: string
  comment?: string       // 审批意见
}

/** 审批参数 */
export interface ApproveParams {
  taskId: string
  approved: boolean      // true=通过, false=拒绝
  comment?: string
  transferTo?: string    // 转办目标用户ID
  ccTo?: string[]        // 抄送用户ID列表
}

/** 任务列表查询参数 */
export interface TaskQueryParams {
  pageIndex: number
  pageSize: number
  keyword?: string
  urgent?: boolean
  status?: TaskStatus
  startTime?: string
  endTime?: string
}