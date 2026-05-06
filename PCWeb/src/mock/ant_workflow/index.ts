/**
 * Ant Workflow Mock 配置
 * 注意：API 路径使用连字符格式 (ant-workflow)
 */
import type { MockMethod } from 'vite-plugin-mock'
import { generateGuid } from '@/utils/guid'
import { WorkflowStatus, AntWorkflowInstanceStatus, AntNodeType, ApproverSetType, ExamineMode, NoHandlerAction, EndType } from '@/types/antWorkflow'

const formatNow = () => new Date().toISOString()

// ==================== 完整审批流配置（包含所有节点类型） ====================

// 使用固定的 ID，确保刷新页面后仍能找到数据
const fullWorkflowId = 'a1b2c3d4-e5f6-7890-abcd-ef1234567890'

// 预生成所有节点ID，确保edges能正确引用
const nodeIdStart = generateGuid()
const nodeIdApprover1 = generateGuid()
const nodeIdCondition = generateGuid()
const nodeIdApprover2 = generateGuid()
const nodeIdCounterSign = generateGuid()
const nodeIdParallel = generateGuid()
const nodeIdCopyer = generateGuid()
const nodeIdNotification = generateGuid()
const nodeIdService = generateGuid()
const nodeIdWebhook = generateGuid()
const nodeIdSubflow = generateGuid()
const nodeIdEnd = generateGuid()

// 创建一个包含所有节点类型的完整审批流 DAG 配置（横向布局）
const fullDagConfig = {
  version: '1.0.0',
  nodes: [
    // 1. 发起人节点
    {
      id: nodeIdStart,
      name: '发起申请',
      type: AntNodeType.START,
      position: { x: 50, y: 200 },
      config: {
        id: nodeIdStart,
        name: '发起申请',
        type: AntNodeType.START,
        permissions: [
          { targetId: 'dept-all', name: '全员', type: 1 },
          { targetId: 'dept-hr', name: '人事部', type: 2 }
        ],
        formConfig: {
          fields: [
            { id: 'field-001', name: 'title', label: '申请标题', type: 'text', required: true, placeholder: '请输入申请标题' },
            { id: 'field-002', name: 'amount', label: '申请金额', type: 'number', required: true, placeholder: '请输入金额' },
            { id: 'field-003', name: 'urgent', label: '是否紧急', type: 'select', required: false, options: [{ label: '是', value: 'true' }, { label: '否', value: 'false' }] },
            { id: 'field-004', name: 'attachments', label: '附件', type: 'file', required: false }
          ],
          rules: [{ fieldId: 'field-001', rule: 'required', message: '申请标题不能为空' }]
        }
      }
    },
    // 2. 审批人节点（部门经理）
    {
      id: nodeIdApprover1,
      name: '部门经理审批',
      type: AntNodeType.APPROVER,
      position: { x: 230, y: 200 },
      config: {
        id: nodeIdApprover1,
        name: '部门经理审批',
        type: AntNodeType.APPROVER,
        settype: ApproverSetType.SUPERVISOR,
        nodeUserList: [{ targetId: 'user-manager-01', name: '张经理', type: 1 }],
        directorLevel: 1,
        examineMode: ExamineMode.SEQUENTIAL,
        noHandlerAction: NoHandlerAction.AUTO_PASS,
        timeout: 24,
        timeoutAction: 'autoPass'
      }
    },
    // 3. 条件分支节点
    {
      id: nodeIdCondition,
      name: '金额判断',
      type: AntNodeType.CONDITION,
      position: { x: 410, y: 200 },
      config: {
        id: nodeIdCondition,
        name: '金额判断',
        type: AntNodeType.CONDITION,
        conditionNodes: [
          { id: 'branch-high', name: '>10000', priority: 1, conditionRules: [{ fieldId: 'field-002', fieldName: 'amount', fieldType: 'number', operator: 'gt', value: 10000 }], isDefault: false },
          { id: 'branch-default', name: '默认', priority: 2, conditionRules: [], isDefault: true }
        ]
      }
    },
    // 4. 审批人节点（财务总监 - 高金额路径）
    {
      id: nodeIdApprover2,
      name: '财务总监审批',
      type: AntNodeType.APPROVER,
      position: { x: 590, y: 100 },
      config: {
        id: nodeIdApprover2,
        name: '财务总监审批',
        type: AntNodeType.APPROVER,
        settype: ApproverSetType.ROLE,
        nodeUserList: [{ targetId: 'role-finance-director', name: '财务总监', type: 2 }],
        examineMode: ExamineMode.SEQUENTIAL,
        noHandlerAction: NoHandlerAction.TRANSFER,
        timeout: 48,
        timeoutAction: 'transfer',
        timeoutTransferTo: 'user-admin'
      }
    },
    // 5. 会签节点（高金额路径）
    {
      id: nodeIdCounterSign,
      name: '财务会签',
      type: AntNodeType.COUNTER_SIGN,
      position: { x: 770, y: 100 },
      config: {
        id: nodeIdCounterSign,
        name: '财务会签',
        type: AntNodeType.COUNTER_SIGN,
        settype: ApproverSetType.FIXED_USER,
        nodeUserList: [
          { targetId: 'user-finance-01', name: '财务A', type: 1 },
          { targetId: 'user-finance-02', name: '财务B', type: 1 },
          { targetId: 'user-finance-03', name: '财务C', type: 1 }
        ],
        passCondition: { type: 'percent', percent: 60 },
        timeout: 72,
        timeoutAction: 'autoReject'
      }
    },
    // 6. 并行分支节点（默认路径）
    {
      id: nodeIdParallel,
      name: '并行处理',
      type: AntNodeType.PARALLEL,
      position: { x: 590, y: 300 },
      config: {
        id: nodeIdParallel,
        name: '并行处理',
        type: AntNodeType.PARALLEL,
        parallelNodes: [
          { id: 'parallel-branch-1', name: '抄送' },
          { id: 'parallel-branch-2', name: '通知' }
        ],
        completeCondition: 'all'
      }
    },
    // 7. 抄送人节点（并行分支1）
    {
      id: nodeIdCopyer,
      name: '抄送人事部',
      type: AntNodeType.COPYER,
      position: { x: 770, y: 260 },
      config: {
        id: nodeIdCopyer,
        name: '抄送人事部',
        type: AntNodeType.COPYER,
        nodeUserList: [
          { targetId: 'dept-hr', name: '人事部', type: 2 },
          { targetId: 'user-hr-01', name: '人事专员', type: 1 }
        ],
        allowSelfSelect: true
      }
    },
    // 8. 通知节点（并行分支2）
    {
      id: nodeIdNotification,
      name: '发送通知',
      type: AntNodeType.NOTIFICATION,
      position: { x: 770, y: 340 },
      config: {
        id: nodeIdNotification,
        name: '发送通知',
        type: AntNodeType.NOTIFICATION,
        notificationType: 'email',
        title: '审批流程通知',
        content: '您的审批申请已进入下一环节，请及时关注。',
        template: 'notification-template-001',
        recipients: [
          { type: 'initiator', value: '', name: '发起人' },
          { type: 'role', value: 'role-hr', name: '人事部' }
        ],
        emailSubject: '审批流程进度通知'
      }
    },
    // 9. 服务任务节点
    {
      id: nodeIdService,
      name: '数据同步',
      type: AntNodeType.SERVICE,
      position: { x: 950, y: 100 },
      config: {
        id: nodeIdService,
        name: '数据同步',
        type: AntNodeType.SERVICE,
        taskType: 'api',
        apiConfig: {
          url: 'https://api.example.com/workflow/sync',
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: '{ "workflowId": "{{workflowId}}" }',
          useGuidIdParam: true,
          guidParamName: 'instanceId',
          fieldMappings: [{ sourceField: 'amount', targetField: 'requestAmount' }]
        },
        resultVariable: 'syncResult',
        errorHandling: { strategy: 'retry', retryCount: 3, retryInterval: 5 }
      }
    },
    // 10. Webhook节点
    {
      id: nodeIdWebhook,
      name: 'Webhook',
      type: AntNodeType.WEBHOOK,
      position: { x: 1130, y: 100 },
      config: {
        id: nodeIdWebhook,
        name: 'Webhook',
        type: AntNodeType.WEBHOOK,
        url: 'https://external.example.com/webhook/callback',
        method: 'POST',
        headers: { 'X-API-Key': 'workflow-api-key' },
        body: '{ "event": "workflow_progress" }',
        trigger: 'after',
        authConfig: { type: 'bearer', token: 'external-token' },
        timeout: 5000
      }
    },
    // 11. 子流程节点（并行路径汇聚）
    {
      id: nodeIdSubflow,
      name: '财务子流程',
      type: AntNodeType.SUBFLOW,
      position: { x: 950, y: 300 },
      config: {
        id: nodeIdSubflow,
        name: '财务子流程',
        type: AntNodeType.SUBFLOW,
        subflowId: 'finance-detail-approval',
        subflowName: '财务明细审批',
        inputMappings: [{ sourceField: 'amount', targetField: 'detailAmount' }],
        outputMappings: [{ sourceField: 'approvalCode', targetField: 'finalCode' }],
        waitForCompletion: true
      }
    },
    // 12. 结束节点（汇聚）
    {
      id: nodeIdEnd,
      name: '流程结束',
      type: AntNodeType.END,
      position: { x: 1310, y: 200 },
      config: {
        id: nodeIdEnd,
        name: '流程结束',
        type: AntNodeType.END,
        endType: EndType.SUCCESS,
        notification: {
          enabled: true,
          type: 'email',
          title: '审批完成通知',
          content: '您的审批申请已完成。',
          recipients: [{ type: 'initiator', value: '', name: '发起人' }]
        }
      }
    }
  ],
  edges: [
    // 发起人 -> 部门经理
    { id: generateGuid(), sourceNodeId: nodeIdStart, targetNodeId: nodeIdApprover1 },
    // 部门经理 -> 条件分支
    { id: generateGuid(), sourceNodeId: nodeIdApprover1, targetNodeId: nodeIdCondition },
    // 条件分支 -> 财务总监（高金额，上方出口）
    { id: generateGuid(), sourceNodeId: nodeIdCondition, targetNodeId: nodeIdApprover2, sourcePort: 'branch-high', condition: { branchId: 'branch-high', priority: 1 } },
    // 条件分支 -> 并行分支（默认，下方出口）
    { id: generateGuid(), sourceNodeId: nodeIdCondition, targetNodeId: nodeIdParallel, sourcePort: 'branch-default', condition: { branchId: 'branch-default', isDefault: true } },
    // 财务总监 -> 会签
    { id: generateGuid(), sourceNodeId: nodeIdApprover2, targetNodeId: nodeIdCounterSign },
    // 会签 -> 服务任务
    { id: generateGuid(), sourceNodeId: nodeIdCounterSign, targetNodeId: nodeIdService },
    // 并行分支 -> 抄送人（上方出口）
    { id: generateGuid(), sourceNodeId: nodeIdParallel, targetNodeId: nodeIdCopyer, sourcePort: 'parallel-branch-1' },
    // 并行分支 -> 通知（下方出口）
    { id: generateGuid(), sourceNodeId: nodeIdParallel, targetNodeId: nodeIdNotification, sourcePort: 'parallel-branch-2' },
    // 服务任务 -> Webhook
    { id: generateGuid(), sourceNodeId: nodeIdService, targetNodeId: nodeIdWebhook },
    // 抄送人 -> 子流程
    { id: generateGuid(), sourceNodeId: nodeIdCopyer, targetNodeId: nodeIdSubflow },
    // 通知 -> 子流程
    { id: generateGuid(), sourceNodeId: nodeIdNotification, targetNodeId: nodeIdSubflow },
    // Webhook -> 结束
    { id: generateGuid(), sourceNodeId: nodeIdWebhook, targetNodeId: nodeIdEnd },
    // 子流程 -> 结束
    { id: generateGuid(), sourceNodeId: nodeIdSubflow, targetNodeId: nodeIdEnd }
  ],
  globalConfig: { maxConcurrency: 5, timeout: 300, retryTimes: 3, retryInterval: 10, failureStrategy: 'stop' }
}

// ==================== Mock 数据 ====================

// 先创建实例ID，确保任务和实例使用相同的ID
const instanceId1 = generateGuid()
const instanceId2 = generateGuid()

// 流程定义数据
export const workflowMockData: any[] = [
  // ===== 新增：完整审批流（包含所有节点类型） =====
  {
    id: fullWorkflowId,
    name: '完整审批流程（全节点演示）',
    code: 'full-approval-demo',
    categoryCode: 'demo',
    categoryName: '演示流程',
    status: WorkflowStatus.DRAFT,
    currentVersion: '1.0.0',
    creatorId: 'user-001',
    creatorName: '管理员',
    description: '包含所有11种节点类型的完整审批流程演示：发起人→审批人→条件分支→会签→并行分支→抄送人→通知→服务任务→Webhook→子流程→结束节点',
    flowConfig: JSON.stringify(fullDagConfig),
    createTime: formatNow(),
    updateTime: formatNow(),
  },
  // ===== 原有流程 =====
  {
    id: generateGuid(),
    name: '请假审批流程',
    code: 'leave-approval',
    categoryCode: 'hr',
    categoryName: '人事管理',
    status: WorkflowStatus.PUBLISHED,
    currentVersion: '1.0.0',
    creatorId: 'user-001',
    creatorName: '管理员',
    description: '员工请假审批流程',
    flowConfig: null,
    createTime: formatNow(),
    updateTime: formatNow(),
  },
  {
    id: generateGuid(),
    name: '报销审批流程',
    code: 'expense-approval',
    categoryCode: 'finance',
    categoryName: '财务管理',
    status: WorkflowStatus.PUBLISHED,
    currentVersion: '1.2.0',
    creatorId: 'user-001',
    creatorName: '管理员',
    description: '费用报销审批流程',
    flowConfig: null,
    createTime: formatNow(),
    updateTime: formatNow(),
  },
  {
    id: generateGuid(),
    name: '合同审批流程',
    code: 'contract-approval',
    categoryCode: 'contract',
    categoryName: '合同管理',
    status: WorkflowStatus.DRAFT,
    currentVersion: '0.1.0',
    creatorId: 'user-002',
    creatorName: '张三',
    description: '合同签署审批流程',
    flowConfig: null,
    createTime: formatNow(),
    updateTime: formatNow(),
  },
]

// 流程实例数据（使用固定的ID）
export const instanceMockData: any[] = [
  {
    id: instanceId1,
    title: '张三的请假申请',
    businessId: 'leave-001',
    businessType: 'leave',
    status: AntWorkflowInstanceStatus.APPROVING,
    initiatorName: '张三',
    initiatorId: 'user-002',
    startTime: formatNow(),
    currentNodeName: '部门经理审批',
    workflowId: workflowMockData[0]?.id,
  },
  {
    id: instanceId2,
    title: '李四的报销申请',
    businessId: 'expense-001',
    businessType: 'expense',
    status: AntWorkflowInstanceStatus.PASSED,
    initiatorName: '李四',
    initiatorId: 'user-003',
    startTime: formatNow(),
    finishTime: formatNow(),
    currentNodeName: null,
    workflowId: workflowMockData[1]?.id,
  },
]

// 待办任务数据（使用相同的instanceId）
export const taskMockData: any[] = [
  {
    taskId: generateGuid(),
    instanceId: instanceId1,
    title: '张三的请假申请',
    nodeName: '部门经理审批',
    nodeType: 'approver',
    initiatorName: '张三',
    initiatorId: 'user-002',
    status: AntWorkflowInstanceStatus.APPROVING,
    entryTime: formatNow(),
    businessId: 'leave-001',
    businessType: 'leave',
  },
  {
    taskId: generateGuid(),
    instanceId: instanceId2,
    title: '李四的报销申请',
    nodeName: '财务审批',
    nodeType: 'approver',
    initiatorName: '李四',
    initiatorId: 'user-003',
    status: AntWorkflowInstanceStatus.APPROVING,
    entryTime: formatNow(),
    businessId: 'expense-001',
    businessType: 'expense',
  },
]

// 抄送数据（使用相同的instanceId）
export const ccMockData: any[] = [
  {
    taskId: generateGuid(),
    instanceId: instanceId1,
    title: '张三的请假申请',
    initiatorName: '张三',
    initiatorId: 'user-002',
    ccTime: formatNow(),
    isRead: false,
    workflowId: workflowMockData[0]?.id,
  },
  {
    taskId: generateGuid(),
    instanceId: instanceId2,
    title: '李四的报销申请',
    initiatorName: '李四',
    initiatorId: 'user-003',
    ccTime: formatNow(),
    isRead: true,
    workflowId: workflowMockData[1]?.id,
  },
]

// 版本历史数据
export const versionMockData: any[] = [
  {
    id: generateGuid(),
    workflowId: workflowMockData[0]?.id,
    version: '1.0.0',
    status: 'published',
    publishTime: formatNow(),
    publisher: '管理员',
    remark: '初始版本',
  },
]

// ==================== Mock 处理器 ====================

const antWorkflowMocks: MockMethod[] = [
  // 流程定义 - 列表
  {
    url: '/api/ant-workflow/workflow/list',
    method: 'get',
    response: ({ query }: any) => {
      const { pageIndex = 1, pageSize = 10, name, categoryCode, status } = query || {}
      let filtered = workflowMockData

      if (name) {
        filtered = filtered.filter((w: any) => w.name.includes(name))
      }
      if (categoryCode) {
        filtered = filtered.filter((w: any) => w.categoryCode === categoryCode)
      }
      if (status !== undefined && status !== '') {
        filtered = filtered.filter((w: any) => w.status === Number(status))
      }

      const start = (pageIndex - 1) * pageSize
      const list = filtered.slice(start, start + pageSize)

      return {
        code: 200,
        message: 'success',
        data: {
          list,
          total: filtered.length,
        },
      }
    },
  },

  // 流程定义 - 详情
  {
    url: '/api/ant-workflow/workflow/detail/:id',
    method: 'get',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const workflow = workflowMockData.find((w: any) => w.id === id)
      if (!workflow) {
        return { code: 404, message: '流程不存在', data: null }
      }
      return { code: 200, message: 'success', data: workflow }
    },
  },

  // 流程定义 - 创建
  {
    url: '/api/ant-workflow/workflow/create',
    method: 'post',
    response: ({ body }: any) => {
      const { name, code, categoryCode, categoryName, description } = body || {}
      const newWorkflow = {
        id: generateGuid(),
        name: name || '新流程',
        code: code || `wf-${Date.now()}`,
        categoryCode: categoryCode || '',
        categoryName: categoryName || '',
        description: description || '',
        status: WorkflowStatus.DRAFT,
        currentVersion: '0.1.0',
        creatorId: 'user-001',
        creatorName: '管理员',
        flowConfig: null,
        createTime: formatNow(),
        updateTime: formatNow(),
      }
      workflowMockData.push(newWorkflow)
      return { code: 200, message: 'success', data: { id: newWorkflow.id } }
    },
  },

  // 流程定义 - 更新
  {
    url: '/api/ant-workflow/workflow/update',
    method: 'put',
    response: ({ body }: any) => {
      const { id, name, categoryCode, categoryName, description, flowConfig } = body || {}
      const workflow = workflowMockData.find((w: any) => w.id === id)
      if (!workflow) {
        return { code: 404, message: '流程不存在', data: null }
      }

      if (name) workflow.name = name
      if (categoryCode) workflow.categoryCode = categoryCode
      if (categoryName) workflow.categoryName = categoryName
      if (description) workflow.description = description
      if (flowConfig) workflow.flowConfig = flowConfig
      workflow.updateTime = formatNow()

      return { code: 200, message: 'success', data: { success: true } }
    },
  },

  // 流程定义 - 删除
  {
    url: '/api/ant-workflow/workflow/delete',
    method: 'delete',
    response: ({ body }: any) => {
      const { ids } = body || {}
      const idsToDelete = Array.isArray(ids) ? ids : [ids]

      idsToDelete.forEach((id: string) => {
        const index = workflowMockData.findIndex((w: any) => w.id === id)
        if (index !== -1) {
          workflowMockData.splice(index, 1)
        }
      })

      return { code: 200, message: 'success', data: { success: true } }
    },
  },

  // 流程定义 - 发布
  {
    url: '/api/ant-workflow/workflow/publish/:id',
    method: 'post',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const workflow = workflowMockData.find((w: any) => w.id === id)
      if (!workflow) {
        return { code: 404, message: '流程不存在', data: null }
      }

      workflow.status = WorkflowStatus.PUBLISHED
      workflow.currentVersion = '1.0.0'
      workflow.updateTime = formatNow()

      versionMockData.push({
        id: generateGuid(),
        workflowId: id,
        version: '1.0.0',
        status: 'published',
        publishTime: formatNow(),
        publisher: '管理员',
        remark: '首次发布',
      })

      return { code: 200, message: 'success', data: { success: true } }
    },
  },

  // 流程定义 - 停用
  {
    url: '/api/ant-workflow/workflow/disable/:id',
    method: 'put',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const workflow = workflowMockData.find((w: any) => w.id === id)
      if (!workflow) {
        return { code: 404, message: '流程不存在', data: null }
      }

      workflow.status = WorkflowStatus.DISABLED
      workflow.updateTime = formatNow()

      return { code: 200, message: 'success', data: { success: true } }
    },
  },

  // 流程定义 - 启用
  {
    url: '/api/ant-workflow/workflow/enable/:id',
    method: 'put',
    response: (config: { url: string }) => {
      const id = config.url.split('/').pop()
      const workflow = workflowMockData.find((w: any) => w.id === id)
      if (!workflow) {
        return { code: 404, message: '流程不存在', data: null }
      }

      workflow.status = WorkflowStatus.PUBLISHED
      workflow.updateTime = formatNow()

      return { code: 200, message: 'success', data: { success: true } }
    },
  },

  // 流程定义 - 复制
  {
    url: '/api/ant-workflow/workflow/copy',
    method: 'post',
    response: ({ body }: any) => {
      const { id, name } = body || {}
      const workflow = workflowMockData.find((w: any) => w.id === id)
      if (!workflow) {
        return { code: 404, message: '流程不存在', data: null }
      }

      const newWorkflow = {
        ...workflow,
        id: generateGuid(),
        name: name || `${workflow.name}（副本）`,
        code: `wf-copy-${Date.now()}`,
        status: WorkflowStatus.DRAFT,
        currentVersion: '0.1.0',
        createTime: formatNow(),
        updateTime: formatNow(),
      }
      workflowMockData.push(newWorkflow)

      return { code: 200, message: 'success', data: { id: newWorkflow.id } }
    },
  },

  // 流程定义 - 版本历史
  {
    url: '/api/ant-workflow/workflow/versions/:workflowId',
    method: 'get',
    response: (config: { url: string }) => {
      const workflowId = config.url.split('/').pop()
      const versions = versionMockData.filter((v: any) => v.workflowId === workflowId)
      return { code: 200, message: 'success', data: versions }
    },
  },

  // 任务 - 待办列表
  {
    url: '/api/ant-workflow/task/todo',
    method: 'get',
    response: () => {
      return {
        code: 200,
        message: 'success',
        data: {
          list: taskMockData,
          total: taskMockData.length,
        },
      }
    },
  },

  // 任务 - 已办列表
  {
    url: '/api/ant-workflow/task/done',
    method: 'get',
    response: () => {
      const doneTasks = taskMockData.map((t: any) => ({
        ...t,
        approveTime: formatNow(),
        approveResult: 1,
        approveComment: '同意',
      }))
      return {
        code: 200,
        message: 'success',
        data: {
          list: doneTasks,
          total: doneTasks.length,
        },
      }
    },
  },

  // 任务 - 详情
  {
    url: '/api/ant-workflow/task/detail/:taskId',
    method: 'get',
    response: (config: { url: string }) => {
      const taskId = config.url.split('/').pop()
      const task = taskMockData.find((t: any) => t.taskId === taskId)
      if (!task) {
        return { code: 404, message: '任务不存在', data: null }
      }
      return { code: 200, message: 'success', data: task }
    },
  },

  // 任务 - 审批
  {
    url: '/api/ant-workflow/task/approve',
    method: 'post',
    response: ({ body }: any) => {
      const { taskId, result, comment } = body || {}
      const task = taskMockData.find((t: any) => t.taskId === taskId)
      if (!task) {
        return { code: 404, message: '任务不存在', data: null }
      }

      const instance = instanceMockData.find((i: any) => i.id === task.instanceId)
      if (instance) {
        instance.status = result === 1 ? 2 : 3
        if (result === 1) instance.finishTime = formatNow()
      }

      return { code: 200, message: 'success', data: { success: true } }
    },
  },

  // 任务 - 转交
  {
    url: '/api/ant-workflow/task/transfer',
    method: 'post',
    response: ({ body }: any) => {
      const { taskId, targetUserId, targetUserName } = body || {}
      const task = taskMockData.find((t: any) => t.taskId === taskId)
      if (!task) {
        return { code: 404, message: '任务不存在', data: null }
      }
      return { code: 200, message: 'success', data: { success: true } }
    },
  },

  // 抄送 - 列表
  {
    url: '/api/ant-workflow/task/cc',
    method: 'get',
    response: () => {
      return {
        code: 200,
        message: 'success',
        data: {
          list: ccMockData,
          total: ccMockData.length,
        },
      }
    },
  },

  // 抄送 - 标记已读
  {
    url: '/api/ant-workflow/task/cc/mark-read',
    method: 'post',
    response: ({ body }: any) => {
      const { ids } = body || {}
      const idsToMark = Array.isArray(ids) ? ids : [ids]

      idsToMark.forEach((id: string) => {
        const cc = ccMockData.find((c: any) => c.taskId === id)
        if (cc) cc.isRead = true
      })

      return { code: 200, message: 'success', data: { success: true } }
    },
  },

  // 实例 - 我发起的
  {
    url: '/api/ant-workflow/instance/my',
    method: 'get',
    response: () => {
      return {
        code: 200,
        message: 'success',
        data: {
          list: instanceMockData,
          total: instanceMockData.length,
        },
      }
    },
  },

  // 实例 - 详情
  {
    url: '/api/ant-workflow/instance/detail/:instanceId',
    method: 'get',
    response: (config: { url: string }) => {
      const instanceId = config.url.split('/').pop()
      const instance = instanceMockData.find((i: any) => i.id === instanceId)
      if (!instance) {
        return { code: 404, message: '实例不存在', data: null }
      }

      const records = [
        { nodeId: 'node-001', nodeName: '发起申请', handlerName: instance.initiatorName, handleTime: instance.startTime, result: 0, comment: '' },
        { nodeId: 'node-002', nodeName: '部门经理审批', handlerName: '王经理', handleTime: formatNow(), result: 1, comment: '同意' },
      ]

      return { code: 200, message: 'success', data: { ...instance, records } }
    },
  },

  // 实例 - 撤回
  {
    url: '/api/ant-workflow/instance/withdraw/:instanceId',
    method: 'post',
    response: (config: { url: string }) => {
      const instanceId = config.url.split('/').pop()
      const instance = instanceMockData.find((i: any) => i.id === instanceId)
      if (!instance) {
        return { code: 404, message: '实例不存在', data: null }
      }

      instance.status = 4
      return { code: 200, message: 'success', data: { success: true } }
    },
  },

  // ==================== Runtime API ====================

  // Runtime - 我发起的流程
  {
    url: '/api/ant-workflow/runtime/my-instances',
    method: 'get',
    response: () => {
      return {
        code: 200,
        message: 'success',
        data: {
          list: instanceMockData.map((i: any) => ({
            ...i,
            workflowName: '审批流程',
          })),
          total: instanceMockData.length,
        },
      }
    },
  },

  // Runtime - 实例详情
  {
    url: '/api/ant-workflow/runtime/instance/:instanceId',
    method: 'get',
    response: (config: { url: string }) => {
      const instanceId = config.url.split('/').pop()
      const instance = instanceMockData.find((i: any) => i.id === instanceId)
      if (!instance) {
        return { code: 404, message: '实例不存在', data: null }
      }

      // 审批进度节点
      const nodes = [
        { nodeId: 'node-001', nodeName: '发起申请', nodeType: 'start', approveStatus: 2, handlerNames: [instance.initiatorName] },
        { nodeId: 'node-002', nodeName: '部门经理审批', nodeType: 'approver', approveStatus: instance.status === AntWorkflowInstanceStatus.APPROVING ? 1 : 2, handlerNames: ['王经理'] },
      ]

      // 审批记录
      const records = [
        { id: generateGuid(), nodeName: '发起申请', handlerName: instance.initiatorName, approveStatus: 1, approveDesc: '发起流程', approveTime: instance.startTime },
        { id: generateGuid(), nodeName: '部门经理审批', handlerName: '王经理', approveStatus: 1, approveDesc: '同意', approveTime: formatNow() },
      ]

      return {
        code: 200,
        message: 'success',
        data: {
          instanceId: instance.id,
          title: instance.title,
          businessId: instance.businessId,
          businessType: instance.businessType,
          status: instance.status,
          initiatorName: instance.initiatorName,
          startTime: instance.startTime,
          finishTime: instance.finishTime,
          workflowName: '审批流程',
          nodes,
          records,
        },
      }
    },
  },

  // Runtime - 取消实例
  {
    url: '/api/ant-workflow/runtime/cancel/:instanceId',
    method: 'post',
    response: (config: { url: string }) => {
      const instanceId = config.url.split('/').pop()
      const instance = instanceMockData.find((i: any) => i.id === instanceId)
      if (!instance) {
        return { code: 404, message: '实例不存在', data: null }
      }

      instance.status = 5 // 已终止
      return { code: 200, message: 'success', data: { success: true } }
    },
  },

  // Runtime - 执行日志
  {
    url: '/api/ant-workflow/runtime/logs/:instanceId',
    method: 'get',
    response: (config: { url: string }) => {
      const instanceId = config.url.split('/').pop()
      const instance = instanceMockData.find((i: any) => i.id === instanceId)
      if (!instance) {
        return { code: 404, message: '实例不存在', data: [] }
      }

      const logs = [
        { id: generateGuid(), instanceId, nodeId: 'node-001', nodeName: '发起申请', action: 'start', actionTime: instance.startTime, operatorId: instance.initiatorId, operatorName: instance.initiatorName, comment: '发起流程' },
        { id: generateGuid(), instanceId, nodeId: 'node-002', nodeName: '部门经理审批', action: 'approve', actionTime: formatNow(), operatorId: 'user-004', operatorName: '王经理', comment: '同意' },
      ]

      return { code: 200, message: 'success', data: logs }
    },
  },

  // Runtime - 发起流程
  {
    url: '/api/ant-workflow/runtime/start',
    method: 'post',
    response: ({ body }: any) => {
      const { workflowId, businessId, businessType, title } = body || {}
      const newInstance = {
        id: generateGuid(),
        title: title || '新流程实例',
        workflowId,
        workflowName: '审批流程',
        businessId,
        businessType,
        status: 1, // 审批中
        initiatorId: 'user-001',
        initiatorName: '当前用户',
        startTime: formatNow(),
        currentNodeName: '部门经理审批',
      }
      instanceMockData.push(newInstance)
      return { code: 200, message: 'success', data: { instanceId: newInstance.id } }
    },
  },
]

export default antWorkflowMocks