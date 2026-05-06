/**
 * 本地枚举标签映射
 * 当 IsUseDic = false 时使用
 */
import type { DictOption } from '@/types/dict'

// 导入枚举类型
import {
  CommonStatus, DatasourceStatus, ScreenPublishStatus, MenuVisibility
} from '@/types/enums'
import {
  WorkflowStatus, PublishRequestStatus
} from '@/types/workflow'
import {
  NodeType, ApproverSetType, ExamineMode, NoHanderAction
} from '@/types/flowNode'
import {
  PipelineStatus
} from '@/types/etl/pipeline'
import {
  ExecutionStatus, TriggerType
} from '@/types/etl/execution'
import {
  ScheduleStatus, ScheduleType
} from '@/types/etl/schedule'
import {
  TaskNodeType
} from '@/types/etl/taskNode'

/**
 * 枚举标签映射表
 */
export const enumLabelMap: Record<string, Record<string | number, { zh: string; en: string }>> = {
  // ========== 通用状态 ==========
  CommonStatus: {
    [CommonStatus.DISABLED]: { zh: '禁用', en: 'Disabled' },
    [CommonStatus.ENABLED]: { zh: '启用', en: 'Enabled' }
  },

  DatasourceStatus: {
    [DatasourceStatus.DISABLED]: { zh: '禁用', en: 'Disabled' },
    [DatasourceStatus.ENABLED]: { zh: '正常', en: 'Normal' }
  },

  ScreenPublishStatus: {
    [ScreenPublishStatus.UNPUBLISHED]: { zh: '已下架', en: 'Unpublished' },
    [ScreenPublishStatus.PUBLISHED]: { zh: '正常', en: 'Published' }
  },

  MenuVisibility: {
    [MenuVisibility.VISIBLE]: { zh: '显示', en: 'Visible' },
    [MenuVisibility.HIDDEN]: { zh: '隐藏', en: 'Hidden' }
  },

  // ========== 工作流状态 ==========
  WorkflowStatus: {
    [WorkflowStatus.DRAFT]: { zh: '草稿', en: 'Draft' },
    [WorkflowStatus.PENDING]: { zh: '待审核', en: 'Pending' },
    [WorkflowStatus.PUBLISHED]: { zh: '已发布', en: 'Published' },
    [WorkflowStatus.REJECTED]: { zh: '已拒绝', en: 'Rejected' },
    [WorkflowStatus.DISABLED]: { zh: '已停用', en: 'Disabled' }
  },

  PublishRequestStatus: {
    [PublishRequestStatus.PENDING]: { zh: '待审核', en: 'Pending' },
    [PublishRequestStatus.APPROVED]: { zh: '已通过', en: 'Approved' },
    [PublishRequestStatus.REJECTED]: { zh: '已拒绝', en: 'Rejected' }
  },

  // ========== 流程节点类型 ==========
  NodeType: {
    [NodeType.START]: { zh: '发起人', en: 'Start' },
    [NodeType.APPROVER]: { zh: '审批人', en: 'Approver' },
    [NodeType.COPYER]: { zh: '抄送', en: 'Copyer' },
    [NodeType.CONDITION]: { zh: '条件分支', en: 'Condition' },
    [NodeType.SERVICE]: { zh: '服务任务', en: 'Service' },
    [NodeType.NOTIFICATION]: { zh: '通知', en: 'Notification' },
    [NodeType.PARALLEL]: { zh: '并行网关', en: 'Parallel' }
  },

  ApproverSetType: {
    [ApproverSetType.FIXED_USER]: { zh: '固定用户', en: 'Fixed User' },
    [ApproverSetType.SUPERVISOR]: { zh: '直接主管', en: 'Supervisor' },
    [ApproverSetType.INITIATOR_SELECT]: { zh: '发起人自选', en: 'Initiator Select' },
    [ApproverSetType.INITIATOR_SELF]: { zh: '发起人自己', en: 'Initiator Self' },
    [ApproverSetType.MULTI_SUPERVISOR]: { zh: '多级主管', en: 'Multi Supervisor' }
  },

  ExamineMode: {
    [ExamineMode.SEQUENTIAL]: { zh: '依次审批', en: 'Sequential' },
    [ExamineMode.COUNTERSIGN]: { zh: '会签', en: 'Countersign' }
  },

  NoHanderAction: {
    [NoHanderAction.AUTO_PASS]: { zh: '自动通过', en: 'Auto Pass' },
    [NoHanderAction.TRANSFER]: { zh: '转交管理员', en: 'Transfer' }
  },

  // ========== ETL 相关 ==========
  PipelineStatus: {
    [PipelineStatus.DRAFT]: { zh: '草稿', en: 'Draft' },
    [PipelineStatus.PUBLISHED]: { zh: '已发布', en: 'Published' },
    [PipelineStatus.ARCHIVED]: { zh: '已归档', en: 'Archived' }
  },

  ExecutionStatus: {
    'pending': { zh: '等待中', en: 'Pending' },
    'running': { zh: '运行中', en: 'Running' },
    'success': { zh: '成功', en: 'Success' },
    'failure': { zh: '失败', en: 'Failure' },
    'timeout': { zh: '超时', en: 'Timeout' },
    'cancelled': { zh: '已取消', en: 'Cancelled' },
    'retrying': { zh: '重试中', en: 'Retrying' }
  },

  TriggerType: {
    'manual': { zh: '手动触发', en: 'Manual' },
    'schedule': { zh: '定时触发', en: 'Schedule' },
    'dependency': { zh: '依赖触发', en: 'Dependency' },
    'event': { zh: '事件触发', en: 'Event' },
    'api': { zh: 'API触发', en: 'API' }
  },

  ScheduleStatus: {
    [ScheduleStatus.INACTIVE]: { zh: '已禁用', en: 'Inactive' },
    [ScheduleStatus.ACTIVE]: { zh: '已启用', en: 'Active' }
  },

  ScheduleType: {
    'cron': { zh: 'Cron表达式', en: 'Cron' },
    'manual': { zh: '手动', en: 'Manual' },
    'dependency': { zh: '依赖', en: 'Dependency' },
    'event': { zh: '事件', en: 'Event' }
  },

  TaskNodeType: {
    'datasource': { zh: '数据源', en: 'Datasource' },
    'sql': { zh: 'SQL', en: 'SQL' },
    'transform': { zh: '数据转换', en: 'Transform' },
    'output': { zh: '数据输出', en: 'Output' },
    'api': { zh: 'API调用', en: 'API' },
    'file': { zh: '文件操作', en: 'File' },
    'script': { zh: '脚本', en: 'Script' },
    'condition': { zh: '条件分支', en: 'Condition' },
    'parallel': { zh: '并行执行', en: 'Parallel' },
    'notification': { zh: '通知', en: 'Notification' },
    'subflow': { zh: '子流程', en: 'Subflow' }
  }
}

/**
 * 获取本地枚举标签
 */
export function getLocalEnumLabel(enumName: string, value: number | string, locale: string): string {
  const enumMap = enumLabelMap[enumName]
  if (!enumMap) return String(value)

  const labelInfo = enumMap[value]
  if (!labelInfo) return String(value)

  return locale === 'zh-CN' ? labelInfo.zh : labelInfo.en
}

/**
 * 获取本地枚举选项列表
 */
export function getLocalEnumOptions(enumName: string, locale: string): DictOption[] {
  const enumMap = enumLabelMap[enumName]
  if (!enumMap) return []

  return Object.entries(enumMap).map(([value, label]) => ({
    value,
    label: locale === 'zh-CN' ? label.zh : label.en
  }))
}