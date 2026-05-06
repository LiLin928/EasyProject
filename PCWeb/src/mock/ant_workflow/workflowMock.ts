/**
 * Ant Workflow Mock 数据
 */
import { generateGuid } from '@/utils/guid'
import { WorkflowStatus, AntWorkflowInstanceStatus, AntApproveStatus } from '@/types/antWorkflow'

const formatNow = () => new Date().toISOString()

export const workflowMockData: any[] = [
  { id: generateGuid(), name: '请假审批流程', code: 'leave-approval', categoryCode: 'hr', categoryName: '人事管理', status: WorkflowStatus.PUBLISHED, currentVersion: '1.0.0', creatorId: 'user-001', creatorName: '管理员', description: '员工请假审批流程', config: null, createTime: formatNow(), updateTime: formatNow() },
  { id: generateGuid(), name: '报销审批流程', code: 'expense-approval', categoryCode: 'finance', categoryName: '财务管理', status: WorkflowStatus.PUBLISHED, currentVersion: '1.2.0', creatorId: 'user-001', creatorName: '管理员', description: '费用报销审批流程', config: null, createTime: formatNow(), updateTime: formatNow() },
  { id: generateGuid(), name: '合同审批流程', code: 'contract-approval', categoryCode: 'contract', categoryName: '合同管理', status: WorkflowStatus.DRAFT, currentVersion: '0.1.0', creatorId: 'user-002', creatorName: '张三', description: '合同签署审批流程', config: null, createTime: formatNow(), updateTime: formatNow() },
]

// 先创建实例ID，确保任务和实例使用相同的ID
const instanceId1 = generateGuid()
const instanceId2 = generateGuid()

export const instanceMockData: any[] = [
  { id: instanceId1, title: '张三的请假申请', businessId: 'leave-001', businessType: 'leave', status: AntWorkflowInstanceStatus.APPROVING, initiatorName: '张三', initiatorId: 'user-002', startTime: formatNow(), currentNodeName: '部门经理审批', workflowId: workflowMockData[0]?.id },
  { id: instanceId2, title: '李四的报销申请', businessId: 'expense-001', businessType: 'expense', status: AntWorkflowInstanceStatus.PASSED, initiatorName: '李四', initiatorId: 'user-003', startTime: formatNow(), finishTime: formatNow(), currentNodeName: null, workflowId: workflowMockData[1]?.id },
]

export const taskMockData: any[] = [
  { taskId: generateGuid(), instanceId: instanceId1, title: '张三的请假申请', nodeName: '部门经理审批', nodeType: 'approver', initiatorName: '张三', initiatorId: 'user-002', status: AntWorkflowInstanceStatus.APPROVING, entryTime: formatNow(), businessId: 'leave-001', businessType: 'leave' },
  { taskId: generateGuid(), instanceId: instanceId2, title: '李四的报销申请', nodeName: '财务审批', nodeType: 'approver', initiatorName: '李四', initiatorId: 'user-003', status: AntWorkflowInstanceStatus.APPROVING, entryTime: formatNow(), businessId: 'expense-001', businessType: 'expense' },
]

export const ccMockData: any[] = [
  { taskId: generateGuid(), instanceId: instanceId1, title: '张三的请假申请', initiatorName: '张三', initiatorId: 'user-002', ccTime: formatNow(), isRead: false, workflowId: workflowMockData[0]?.id },
  { taskId: generateGuid(), instanceId: instanceId2, title: '李四的报销申请', initiatorName: '李四', initiatorId: 'user-003', ccTime: formatNow(), isRead: true, workflowId: workflowMockData[1]?.id },
]

export const versionMockData: any[] = [
  { id: generateGuid(), workflowId: workflowMockData[0]?.id, version: '1.0.0', status: 'published', publishTime: formatNow(), publisher: '管理员', remark: '初始版本' },
  { id: generateGuid(), workflowId: workflowMockData[0]?.id, version: '0.9.0', status: 'archived', publishTime: formatNow(), publisher: '管理员', remark: '测试版本' },
]