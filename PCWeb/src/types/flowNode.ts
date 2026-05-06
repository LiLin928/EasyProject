/**
 * 流程节点类型定义
 * 用于工作流设计器组件，替代原有的 bpmn-js 实现
 */

// ==================== 枚举类型 ====================

/**
 * 节点类型枚举
 */
export enum NodeType {
  START = 0,        // 发起人节点
  APPROVER = 1,     // 审批人节点
  COPYER = 2,       // 抄送节点
  CONDITION = 4,    // 条件分支节点
  SERVICE = 5,      // 服务任务节点
  NOTIFICATION = 6, // 通知节点
  PARALLEL = 7,     // 并行网关节点
}

/**
 * 审批人设置类型
 */
export enum ApproverSetType {
  FIXED_USER = 1,       // 固定用户
  SUPERVISOR = 2,       // 直接主管
  INITIATOR_SELECT = 4, // 发起人自选
  INITIATOR_SELF = 5,   // 发起人自己
  MULTI_SUPERVISOR = 7, // 多级主管
}

/**
 * 审批模式
 */
export enum ExamineMode {
  SEQUENTIAL = 1,   // 依次审批
  COUNTERSIGN = 2,  // 会签
}

/**
 * 无处理人时的操作
 */
export enum NoHanderAction {
  AUTO_PASS = 1,  // 自动通过
  TRANSFER = 2,    // 转交管理员
}

// ==================== 类型别名 ====================

/**
 * 服务任务类型
 */
export type ServiceTaskType = 'api' | 'script' | 'expression'

/**
 * 通知类型
 */
export type NotificationType = 'message' | 'email' | 'sms'

/**
 * 接收人类型
 */
export type RecipientType = 'user' | 'role' | 'initiator' | 'supervisor'

/**
 * 条件操作符
 * 1: 等于, 2: 不等于, 3: 大于, 4: 小于, 5: 大于等于, 6: 小于等于
 */
export type ConditionOperator = '1' | '2' | '3' | '4' | '5' | '6'

/**
 * 条件列类型
 */
export type ConditionColumnType = 'String' | 'Double' | 'Date'

// ==================== 辅助接口 ====================

/**
 * 节点用户
 */
export interface NodeUser {
  targetId: string   // 目标ID（用户ID或角色ID）
  name: string       // 显示名称
  type: number       // 类型（1: 用户, 2: 角色）
}

/**
 * 流程权限（发起人权限配置）
 */
export interface FlowPermission {
  targetId: string   // 目标ID
  name: string       // 显示名称
  type: number       // 类型
}

/**
 * 条件项（条件分支中的单个条件）
 */
export interface ConditionItem {
  columnId: string              // 列ID
  showName: string              // 显示名称
  columnName: string           // 列名
  columnType: ConditionColumnType // 列类型
  optType: ConditionOperator   // 操作符类型
  zdy1?: string                 // 自定义值1（用于范围查询）
  zdy2?: string                 // 自定义值2（用于范围查询）
  opt1?: string                 // 操作数1
  opt2?: string                 // 操作数2
  fixedDownBoxValue?: string    // 固定下拉值
}

/**
 * 通知接收人
 */
export interface NotificationRecipient {
  type: RecipientType   // 接收人类型
  value: string         // 用户ID或角色编码
  name?: string         // 显示名称
}

// ==================== 流程节点接口 ====================

/**
 * 流程节点基础接口
 */
export interface FlowNode {
  id: string                 // 节点ID（GUID）
  nodeName: string           // 节点名称
  type: NodeType             // 节点类型
  childNode?: FlowNode | null  // 子节点（下一个节点）
  error?: boolean            // 是否有错误
}

/**
 * 发起人节点
 */
export interface StartNode extends FlowNode {
  type: NodeType.START
  flowPermission?: FlowPermission[]  // 发起人权限配置
}

/**
 * 审批人节点
 */
export interface ApproverNode extends FlowNode {
  type: NodeType.APPROVER
  settype: ApproverSetType           // 审批人设置类型
  nodeUserList: NodeUser[]           // 审批人列表
  directorLevel?: number            // 主管层级
  examineMode?: ExamineMode          // 审批模式
  noHanderAction?: NoHanderAction    // 无处理人时的操作
  selectMode?: number               // 选择模式
  selectRange?: number             // 选择范围
  examineEndDirectorLevel?: number  // 结束主管层级
}

/**
 * 抄送节点
 */
export interface CopyerNode extends FlowNode {
  type: NodeType.COPYER
  nodeUserList: NodeUser[]           // 抄送人列表
  ccSelfSelectFlag?: number         // 是否允许自选抄送人（1: 是, 0: 否）
}

/**
 * 服务任务节点
 */
export interface ServiceNode extends FlowNode {
  type: NodeType.SERVICE
  taskType: ServiceTaskType          // 任务类型
  // API调用配置
  apiUrl?: string                    // 接口地址
  apiMethod?: 'GET' | 'POST' | 'PUT' | 'DELETE'  // 请求方法
  apiHeaders?: Record<string, string> // 请求头
  apiBody?: string                   // 请求体JSON
  useGuidIdParam?: boolean           // 是否使用GUID作为主键参数
  guidParamName?: string             // GUID参数名，默认为id
  // 返回值处理
  resultVariable?: string            // 结果存储变量名
  // 脚本配置
  script?: string
  scriptFormat?: string
  // 表达式配置
  expression?: string
}

/**
 * 通知节点
 */
export interface NotificationNode extends FlowNode {
  type: NodeType.NOTIFICATION
  notificationType: NotificationType  // 通知类型
  title?: string                      // 消息标题
  content?: string                    // 消息内容
  template?: string                   // 消息模板编码
  recipients: NotificationRecipient[] // 接收人列表
  // 邮件/SMS额外配置
  emailSubject?: string              // 邮件主题
  smsTemplateId?: string             // 短信模板ID
}

/**
 * 条件分支（条件节点中的单个分支）
 */
export interface ConditionBranch {
  id: string                          // 分支ID（GUID）
  nodeName: string                    // 分支名称
  priorityLevel: number              // 优先级
  conditionList: ConditionItem[]      // 条件列表
  nodeUserList?: NodeUser[]          // 节点用户列表
  childNode?: FlowNode | null        // 子节点
  error?: boolean                     // 是否有错误
}

/**
 * 条件分支节点
 */
export interface ConditionNode extends FlowNode {
  type: NodeType.CONDITION
  conditionNodes: ConditionBranch[]  // 条件分支列表
}

/**
 * 并行分支（并行节点中的单个分支）
 */
export interface ParallelBranch {
  id: string                    // 分支ID（GUID）
  nodeName: string              // 分支名称
  childNode?: FlowNode | null  // 子节点
}

/**
 * 并行网关节点
 */
export interface ParallelNode extends FlowNode {
  type: NodeType.PARALLEL
  parallelNodes: ParallelBranch[]  // 并行分支列表
}

// ==================== Store 配置接口 ====================

/**
 * 节点配置更新载荷
 */
export interface NodeConfigPayload {
  value: any              // 配置值
  id: string             // 节点ID
  flag?: boolean         // 标志位
  priorityLevel?: number // 优先级（用于条件分支）
}

// ==================== 校验错误接口 ====================

/**
 * 校验错误信息
 */
export interface ValidationError {
  path: string           // 错误路径
  nodeName: string       // 节点名称
  type: NodeType         // 节点类型
  message: string        // 错误信息
}