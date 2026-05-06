/**
 * Ant Workflow 节点类型定义
 * 基于钉钉审批风格的工作流节点类型
 */

// ==================== 枚举类型 ====================

/**
 * 节点类型枚举
 */
export enum AntNodeType {
  START = 'start',           // 发起人节点
  APPROVER = 'approver',     // 审批人节点
  COPYER = 'copyer',         // 抄送人节点
  CONDITION = 'condition',  // 条件分支节点
  PARALLEL = 'parallel',    // 并行分支节点
  SERVICE = 'service',       // 服务任务节点
  NOTIFICATION = 'notification', // 通知节点
  WEBHOOK = 'webhook',      // Webhook 节点
  SUBFLOW = 'subflow',      // 子流程节点
  COUNTER_SIGN = 'counter_sign', // 会签节点
  END = 'end',              // 结束节点
}

/**
 * 审批人设置类型
 */
export enum ApproverSetType {
  FIXED_USER = 1,           // 固定用户
  SUPERVISOR = 2,            // 直接主管
  INITIATOR_SELECT = 4,      // 发起人自选
  INITIATOR_SELF = 5,        // 发起人自己
  MULTI_SUPERVISOR = 7,      // 多级主管
  ROLE = 8,                  // 指定角色
  FORM_FIELD = 9,            // 表单字段
}

/**
 * 审批模式
 */
export enum ExamineMode {
  SEQUENTIAL = 1,            // 依次审批
  COUNTERSIGN = 2,           // 会签
  OR = 3,                    // 或签（任一通过即可）
}

/**
 * 无处理人时的操作
 */
export enum NoHandlerAction {
  AUTO_PASS = 1,            // 自动通过
  TRANSFER = 2,              // 转交管理员
  AUTO_REJECT = 3,           // 自动拒绝
}

/**
 * Webhook 触发事件
 */
export enum WebhookTrigger {
  BEFORE = 'before',         // 节点执行前
  AFTER = 'after',           // 节点执行后
  MANUAL = 'manual',         // 手动触发
}

/**
 * 结束类型枚举
 */
export enum EndType {
  SUCCESS = 'success',   // 成功完成
  REJECT = 'reject',     // 审批拒绝
  CANCEL = 'cancel',     // 流程取消
}

/**
 * 条件操作符
 */
export enum ConditionOperator {
  EQ = 'eq',                 // 等于
  NE = 'ne',                 // 不等于
  GT = 'gt',                 // 大于
  GTE = 'gte',               // 大于等于
  LT = 'lt',                 // 小于
  LTE = 'lte',               // 小于等于
  CONTAINS = 'contains',     // 包含
  NOT_CONTAINS = 'notContains', // 不包含
  EMPTY = 'empty',           // 为空
  NOT_EMPTY = 'notEmpty',    // 不为空
  IN = 'in',                 // 在列表中
  NOT_IN = 'notIn',          // 不在列表中
}

// ==================== 辅助接口 ====================

/**
 * 节点用户
 */
export interface NodeUser {
  /** 目标ID（用户ID或角色ID） */
  targetId: string
  /** 显示名称 */
  name: string
  /** 类型（1: 用户, 2: 角色） */
  type: number
}

/**
 * 流程权限（发起人权限配置）
 */
export interface FlowPermission {
  /** 目标ID */
  targetId: string
  /** 显示名称 */
  name: string
  /** 类型 */
  type: number
}

/**
 * 通知接收人
 */
export interface NotificationRecipient {
  /** 接收人类型 */
  type: 'user' | 'role' | 'initiator' | 'supervisor' | 'formField'
  /** 用户ID或角色编码 */
  value: string
  /** 显示名称 */
  name?: string
}

/**
 * 字段映射（用于服务任务和Webhook）
 */
export interface FieldMapping {
  /** 源字段名 */
  sourceField: string
  /** 目标字段名 */
  targetField: string
  /** 转换表达式 */
  transform?: string
}

/**
 * 条件规则（单个条件项）
 */
export interface ConditionRule {
  /** 字段ID */
  fieldId: string
  /** 字段名称 */
  fieldName: string
  /** 字段类型 */
  fieldType: 'string' | 'number' | 'date' | 'boolean' | 'array'
  /** 操作符 */
  operator: ConditionOperator
  /** 比较值 */
  value: string | number | boolean | string[]
  /** 显示名称 */
  label?: string
}

/**
 * 边条件（分支条件）
 */
export interface EdgeCondition {
  /** 分支ID */
  branchId: string
  /** 分支名称 */
  branchName: string
  /** 条件规则列表（AND关系） */
  rules: ConditionRule[]
  /** 优先级 */
  priority: number
  /** 是否默认分支 */
  isDefault?: boolean
}

// ==================== 节点配置接口 ====================

/**
 * 节点基础配置接口
 */
export interface BaseNodeConfig {
  /** 节点ID（GUID） */
  id: string
  /** 节点名称 */
  name: string
  /** 节点类型 */
  type: AntNodeType
  /** 节点描述 */
  description?: string
}

/**
 * 发起人节点配置
 */
export interface StartNodeConfig extends BaseNodeConfig {
  type: AntNodeType.START
  /** 发起人权限配置 */
  permissions?: FlowPermission[]
  /** 表单配置 */
  formConfig?: {
    /** 表单字段列表 */
    fields: FormFieldConfig[]
    /** 表单规则 */
    rules?: FormRule[]
  }
}

/**
 * 表单字段配置
 */
export interface FormFieldConfig {
  /** 字段ID */
  id: string
  /** 字段名 */
  name: string
  /** 字段标签 */
  label: string
  /** 字段类型 */
  type: 'text' | 'number' | 'date' | 'select' | 'multiSelect' | 'file' | 'user' | 'dept'
  /** 是否必填 */
  required?: boolean
  /** 默认值 */
  defaultValue?: unknown
  /** 选项（用于 select/multiSelect） */
  options?: { label: string; value: string }[]
  /** 占位符 */
  placeholder?: string
}

/**
 * 表单验证规则
 */
export interface FormRule {
  /** 字段ID */
  fieldId: string
  /** 规则类型 */
  rule: 'required' | 'pattern' | 'min' | 'max' | 'custom'
  /** 规则值 */
  value?: string | number
  /** 错误提示 */
  message: string
}

/**
 * 审批人节点配置
 */
export interface ApproverNodeConfig extends BaseNodeConfig {
  type: AntNodeType.APPROVER
  /** 审批人设置类型 */
  settype: ApproverSetType
  /** 审批人列表 */
  nodeUserList: NodeUser[]
  /** 主管层级 */
  directorLevel?: number
  /** 审批模式 */
  examineMode?: ExamineMode
  /** 无处理人时的操作 */
  noHandlerAction?: NoHandlerAction
  /** 选择模式 */
  selectMode?: number
  /** 选择范围 */
  selectRange?: number
  /** 结束主管层级 */
  examineEndDirectorLevel?: number
  /** 超时设置（小时） */
  timeout?: number
  /** 超时动作 */
  timeoutAction?: 'autoPass' | 'autoReject' | 'transfer'
  /** 超时转交人 */
  timeoutTransferTo?: string
}

/**
 * 抄送人节点配置
 */
export interface CopyerNodeConfig extends BaseNodeConfig {
  type: AntNodeType.COPYER
  /** 抄送人列表 */
  nodeUserList: NodeUser[]
  /** 是否允许自选抄送人 */
  allowSelfSelect?: boolean
}

/**
 * 服务任务节点配置
 */
export interface ServiceNodeConfig extends BaseNodeConfig {
  type: AntNodeType.SERVICE
  /** 任务类型 */
  taskType: 'api' | 'script' | 'expression'
  /** API调用配置 */
  apiConfig?: {
    /** 接口地址 */
    url: string
    /** 请求方法 */
    method: 'GET' | 'POST' | 'PUT' | 'DELETE'
    /** 请求头 */
    headers?: Record<string, string>
    /** 请求体 */
    body?: string
    /** 是否使用GUID作为主键参数 */
    useGuidIdParam?: boolean
    /** GUID参数名 */
    guidParamName?: string
    /** 字段映射 */
    fieldMappings?: FieldMapping[]
  }
  /** 脚本配置 */
  scriptConfig?: {
    /** 脚本内容 */
    script: string
    /** 脚本格式 */
    format?: 'javascript' | 'groovy' | 'python'
  }
  /** 表达式配置 */
  expressionConfig?: {
    /** 表达式 */
    expression: string
  }
  /** 结果变量名 */
  resultVariable?: string
  /** 错误处理 */
  errorHandling?: {
    /** 错误处理策略 */
    strategy: 'continue' | 'stop' | 'retry'
    /** 重试次数 */
    retryCount?: number
    /** 重试间隔（秒） */
    retryInterval?: number
  }
}

/**
 * 通知节点配置
 */
export interface NotificationNodeConfig extends BaseNodeConfig {
  type: AntNodeType.NOTIFICATION
  /** 通知类型 */
  notificationType: 'message' | 'email' | 'sms' | 'wechat'
  /** 消息标题 */
  title?: string
  /** 消息内容 */
  content?: string
  /** 消息模板编码 */
  template?: string
  /** 接收人列表 */
  recipients: NotificationRecipient[]
  /** 邮件主题 */
  emailSubject?: string
  /** 短信模板ID */
  smsTemplateId?: string
  /** 微信模板ID */
  wechatTemplateId?: string
}

/**
 * 条件分支节点配置
 */
export interface ConditionNodeConfig extends BaseNodeConfig {
  type: AntNodeType.CONDITION
  /** 条件分支列表 */
  conditionNodes: ConditionBranch[]
}

/**
 * 条件分支
 */
export interface ConditionBranch {
  /** 分支ID */
  id: string
  /** 分支名称 */
  name: string
  /** 优先级 */
  priority: number
  /** 条件规则列表 */
  conditionRules: ConditionRule[]
  /** 是否默认分支 */
  isDefault?: boolean
  /** 子节点 */
  childNode?: AntFlowNode | null
}

/**
 * 并行分支节点配置
 */
export interface ParallelNodeConfig extends BaseNodeConfig {
  type: AntNodeType.PARALLEL
  /** 并行分支列表 */
  parallelNodes: ParallelBranch[]
  /** 完成条件 */
  completeCondition?: 'all' | 'any' | 'count'
  /** 完成数量（用于 count 条件） */
  completeCount?: number
}

/**
 * 并行分支
 */
export interface ParallelBranch {
  /** 分支ID */
  id: string
  /** 分支名称 */
  name: string
  /** 子节点 */
  childNode?: AntFlowNode | null
}

/**
 * Webhook 节点配置
 */
export interface WebhookNodeConfig extends BaseNodeConfig {
  type: AntNodeType.WEBHOOK
  /** Webhook URL */
  url: string
  /** 请求方法 */
  method: 'GET' | 'POST' | 'PUT' | 'DELETE'
  /** 请求头 */
  headers?: Record<string, string>
  /** 请求体 */
  body?: string
  /** 触发事件 */
  trigger?: WebhookTrigger
  /** 字段映射 */
  fieldMappings?: FieldMapping[]
  /** 认证配置 */
  authConfig?: {
    type: 'none' | 'basic' | 'bearer' | 'api_key'
    username?: string
    password?: string
    token?: string
    apiKey?: string
    apiKeyHeader?: string
  }
  /** 超时设置（毫秒） */
  timeout?: number
  /** 重试配置 */
  retryConfig?: {
    count: number
    interval: number
  }
}

/**
 * 子流程节点配置
 */
export interface SubflowNodeConfig extends BaseNodeConfig {
  type: AntNodeType.SUBFLOW
  /** 子流程ID */
  subflowId: string
  /** 子流程名称 */
  subflowName?: string
  /** 输入参数映射 */
  inputMappings?: FieldMapping[]
  /** 输出参数映射 */
  outputMappings?: FieldMapping[]
  /** 是否等待子流程完成 */
  waitForCompletion?: boolean
}

/**
 * 会签节点配置
 */
export interface CounterSignNodeConfig extends BaseNodeConfig {
  type: AntNodeType.COUNTER_SIGN
  /** 审批人列表 */
  nodeUserList: NodeUser[]
  /** 审批人设置类型 */
  settype: ApproverSetType
  /** 通过条件 */
  passCondition: {
    /** 条件类型 */
    type: 'percent' | 'count' | 'all'
    /** 通过比例（用于 percent 类型） */
    percent?: number
    /** 通过数量（用于 count 类型） */
    count?: number
  }
  /** 超时设置（小时） */
  timeout?: number
  /** 超时动作 */
  timeoutAction?: 'autoPass' | 'autoReject' | 'transfer'
}

/**
 * 结束节点配置
 */
export interface EndNodeConfig extends BaseNodeConfig {
  type: AntNodeType.END
  /** 结束类型 */
  endType: EndType
  /** 结束时的通知配置 */
  notification?: {
    /** 是否启用通知 */
    enabled: boolean
    /** 通知类型 */
    type: 'message' | 'email' | 'sms' | 'wechat'
    /** 消息标题 */
    title?: string
    /** 消息内容 */
    content?: string
    /** 接收人列表 */
    recipients: NotificationRecipient[]
  }
  /** 结束回调 URL（可选） */
  callbackUrl?: string
}

// ==================== 流程节点联合类型 ====================

/**
 * 流程节点联合类型
 */
export type AntFlowNode =
  | StartNodeConfig
  | ApproverNodeConfig
  | CopyerNodeConfig
  | ServiceNodeConfig
  | NotificationNodeConfig
  | ConditionNodeConfig
  | ParallelNodeConfig
  | WebhookNodeConfig
  | SubflowNodeConfig
  | CounterSignNodeConfig
  | EndNodeConfig

// ==================== 节点样式配置 ====================

/**
 * 节点样式配置
 */
export interface NodeStyleConfig {
  /** 节点图标 */
  icon: string
  /** 节点颜色 */
  color: string
  /** 背景颜色 */
  bgColor: string
  /** 边框颜色 */
  borderColor: string
  /** 节点类型名称（中文） */
  typeName: string
  /** 节点类型名称（英文） */
  typeNameEn: string
}

/**
 * 节点样式映射
 */
export const nodeStyleMap: Record<AntNodeType, NodeStyleConfig> = {
  [AntNodeType.START]: {
    icon: 'User',
    color: '#1890ff',
    bgColor: '#e6f7ff',
    borderColor: '#91d5ff',
    typeName: '发起人',
    typeNameEn: 'Start',
  },
  [AntNodeType.APPROVER]: {
    icon: 'UserFilled',
    color: '#fa8c16',
    bgColor: '#fff7e6',
    borderColor: '#ffd591',
    typeName: '审批人',
    typeNameEn: 'Approver',
  },
  [AntNodeType.COPYER]: {
    icon: 'CopyDocument',
    color: '#13c2c2',
    bgColor: '#e6fffb',
    borderColor: '#87e8de',
    typeName: '抄送人',
    typeNameEn: 'Copyer',
  },
  [AntNodeType.CONDITION]: {
    icon: 'Share',
    color: '#722ed1',
    bgColor: '#f9f0ff',
    borderColor: '#d3adf7',
    typeName: '条件分支',
    typeNameEn: 'Condition',
  },
  [AntNodeType.PARALLEL]: {
    icon: 'Grid',
    color: '#52c41a',
    bgColor: '#f6ffed',
    borderColor: '#b7eb8f',
    typeName: '并行分支',
    typeNameEn: 'Parallel',
  },
  [AntNodeType.SERVICE]: {
    icon: 'Setting',
    color: '#faad14',
    bgColor: '#fffbe6',
    borderColor: '#ffe58f',
    typeName: '服务任务',
    typeNameEn: 'Service',
  },
  [AntNodeType.NOTIFICATION]: {
    icon: 'Bell',
    color: '#eb2f96',
    bgColor: '#fff0f6',
    borderColor: '#ffadd2',
    typeName: '通知',
    typeNameEn: 'Notification',
  },
  [AntNodeType.WEBHOOK]: {
    icon: 'Link',
    color: '#2f54eb',
    bgColor: '#f0f5ff',
    borderColor: '#adc6ff',
    typeName: 'Webhook',
    typeNameEn: 'Webhook',
  },
  [AntNodeType.SUBFLOW]: {
    icon: 'Document',
    color: '#595959',
    bgColor: '#fafafa',
    borderColor: '#d9d9d9',
    typeName: '子流程',
    typeNameEn: 'Subflow',
  },
  [AntNodeType.COUNTER_SIGN]: {
    icon: 'Stamp',
    color: '#f5222d',
    bgColor: '#fff1f0',
    borderColor: '#ffa39e',
    typeName: '会签',
    typeNameEn: 'Counter Sign',
  },
  [AntNodeType.END]: {
    icon: 'CircleClose',
    color: '#909399',
    bgColor: '#f5f5f7',
    borderColor: '#d3d4d6',
    typeName: '结束',
    typeNameEn: 'End',
  },
}

/**
 * 获取节点样式配置
 * @param type 节点类型
 * @returns 节点样式配置
 */
export function getNodeStyle(type: AntNodeType): NodeStyleConfig {
  return nodeStyleMap[type] || nodeStyleMap[AntNodeType.START]
}

/**
 * 获取节点类型名称
 * @param type 节点类型
 * @param lang 语言（默认中文）
 * @returns 节点类型名称
 */
export function getNodeTypeName(type: AntNodeType, lang: 'zh' | 'en' = 'zh'): string {
  const style = getNodeStyle(type)
  return lang === 'zh' ? style.typeName : style.typeNameEn
}